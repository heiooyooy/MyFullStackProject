using Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharedModule;
using SharedModule.Interfaces;
using StackExchange.Redis;

namespace OutboxProcessor;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly ManualResetEventSlim _wakeUpSignal = new(false);
    
    public Worker(
        IServiceProvider serviceProvider,
        ILogger<Worker> logger,
        IConnectionMultiplexer redis)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _redis = redis;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();
        await subscriber.SubscribeAsync(SharedConstants.OUTBOX_SIGNAL_CHANNEL, (channel, message) =>
        {
            _logger.LogInformation("Woken up by Redis signal.");
            // 收到信号后，设置事件，唤醒主循环
            _wakeUpSignal.Set();
        });
        
        _logger.LogInformation("Outbox Processor is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 等待 Redis 信号或定时器超时
                // WaitHandle.WaitOne 可以将 ManualResetEventSlim 转换为可等待的
                var awakened = _wakeUpSignal.Wait(TimeSpan.FromSeconds(30), stoppingToken);
                if (awakened)
                {
                    _wakeUpSignal.Reset(); // 重置信号，准备下次接收
                }
                else
                {
                    _logger.LogInformation("Woken up by fallback timer.");
                }

                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Outbox Processor is stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in Outbox Processor.");
                // 等待一小段时间，避免错误循环消耗过多CPU
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        // await Task.Delay(2000);
        _logger.LogInformation("Processing outbox messages...");
        // 创建一个新的依赖注入作用域来获取Scoped服务(如DbContext)
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        // var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        var messagePublisher = scope.ServiceProvider.GetRequiredService<IOutboxMessagePublisher>();

        var messagesToProcess = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20) // deal with 20 messages a time to avoid overload
            .ToListAsync(stoppingToken);

        if (!messagesToProcess.Any())
        {
            _logger.LogInformation("No messages to process.");
            return;
        }

        _logger.LogInformation("Found {Count} messages to process.", messagesToProcess.Count);

        foreach (var message in messagesToProcess)
        {
            try
            {
                await messagePublisher.PublishOutboxMessageAsync(message);
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message {MessageId}", message.Id);
                // 这里可以添加重试逻辑或将消息标记为“失败”
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
        _logger.LogInformation("Finished processing batch.");
    }
}
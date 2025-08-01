using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SharedModule;

namespace KafkaWorkerService;

public class KafkaAnalyticsConsumer : IDisposable
{
    private readonly ILogger<KafkaAnalyticsConsumer> _logger;
    private readonly IHubContext<AnalyticsHub> _hubContext;
    private readonly IConsumer<string, string> _consumer;

    public KafkaAnalyticsConsumer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaAnalyticsConsumer> logger,
        IHubContext<AnalyticsHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = "seckill-dashboard-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    // 这个方法现在会立即返回，并将消费循环放到后台任务中
    public async void StartConsuming(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("seckill_events");
        _logger.LogInformation("Subscribed to 'seckill_events'. Starting consumer loop on a new task.");
        
        _logger.LogInformation("Kafka consumer loop started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 这个 Consume 会阻塞当前任务的线程，但不会阻塞主程序
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult?.Message?.Value != null)
                {
                    _logger.LogInformation("Received message from Kafka: {Message}", consumeResult.Message.Value);
                    var seckillEvent =
                        JsonSerializer.Deserialize<SeckillClaimSucceededEvent>(consumeResult.Message.Value);

                    if (seckillEvent != null)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveSeckillEvent", seckillEvent, stoppingToken);
                        _logger.LogInformation("Broadcast for event {EventId} complete.", seckillEvent.EventId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Cancellation requested. Kafka consumer loop is stopping.");
                break; // 正常退出循环
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Kafka consumer loop.");
                // 发生错误时，短暂等待后重试
                await Task.Delay(5000, stoppingToken);
            }
        }
    }


    // 实现 IDisposable 来正确关闭和释放消费者   
    public void Dispose()
    {
        _logger.LogInformation("Closing Kafka consumer.");
        // Close 会等待所有进行中的操作完成，是比 Dispose 更优雅的关闭方式
        _consumer.Close();
        _consumer.Dispose();
    }
}
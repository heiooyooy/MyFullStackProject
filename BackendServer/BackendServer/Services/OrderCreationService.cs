using System.Threading.Channels;
using SharedModule;
using SharedModule.Interfaces;

namespace BackendServer.Services;

public class OrderCreationService : BackgroundService
{
    private readonly ChannelReader<SeckillClaimSucceededEvent> _channelReader;
    private readonly ILogger<OrderCreationService> _logger;
    private readonly IKafkaProducer _kafkaProducer;

    public OrderCreationService(ChannelReader<SeckillClaimSucceededEvent> channelReader,
        ILogger<OrderCreationService> logger, IKafkaProducer kafkaProducer)
    {
        _channelReader = channelReader;
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Creation Service is running.");

        await foreach (var successEvent in _channelReader.ReadAllAsync(stoppingToken))
        {
            // 使用 `_ =` 来表示我们不等待这个任务完成，它会在后台运行
            // 这确保了即使 Kafka 暂时响应慢，也不会增加用户等待时间
            _ = _kafkaProducer.ProduceAsync("seckill_events", successEvent);

            // 在真实的系统中，这里会执行数据库操作和发件箱模式
            // 1. Begin Transaction
            // 2. INSERT INTO Orders ...
            // 3. INSERT INTO OutboxMessages ...
            // 4. Commit Transaction

            _logger.LogInformation(
                "[ASYNC ORDER CREATION] Received successful claim. Creating order for User '{UserId}' for Product '{ProductId}'. Event Time: {Timestamp}",
                successEvent.UserId,
                successEvent.ProductId,
                successEvent.TimestampUtc);

            // 模拟耗时的数据库操作
            await Task.Delay(500, stoppingToken);
        }
    }
}
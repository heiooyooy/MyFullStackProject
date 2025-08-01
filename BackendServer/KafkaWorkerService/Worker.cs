namespace KafkaWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly KafkaAnalyticsConsumer _consumer;

    public Worker(ILogger<Worker> logger, KafkaAnalyticsConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker is starting the Kafka consumer.");

        // 使用 Task.Run 将阻塞代码放入后台线程
        await Task.Run(() => _consumer.StartConsuming(stoppingToken), stoppingToken);
        
        _logger.LogInformation("Worker is stopping.");
    }

    // Worker 被销毁时，确保我们的消费者也被正确销毁
    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
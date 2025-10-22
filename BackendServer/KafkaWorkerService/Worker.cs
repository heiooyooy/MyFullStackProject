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
        
        await _consumer.StartConsuming(stoppingToken);
        
        _logger.LogInformation("Worker is stopping.");
    }
    
    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
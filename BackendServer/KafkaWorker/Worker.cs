namespace KafkaWorker;

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
        try
        {
            _logger.LogInformation("Starting Kafka consumers");
            var task = _consumer.StartConsumingAsync(stoppingToken);
            await task;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override void Dispose()
    {
        _consumer.StopConsumingAsync();
    }
}
using SharedModule.RabbitMQ;

namespace RabbitMQWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OutboxMessageConsumer _outboxMessageConsumer;

    public Worker(ILogger<Worker> logger, IRabbitMQConnectionFactory rabbitMqConnectionFactory,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        _outboxMessageConsumer = outboxMessageConsumer;
        _logger = logger;
        var settings = rabbitMqConnectionFactory.TestSettings();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting RabbitMQ consumers");
            var task = _outboxMessageConsumer.StartConsumingAsync(stoppingToken);
            await task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RabbitMQ consumer service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumers");

        await _outboxMessageConsumer.StopConsumingAsync();
        // await _notificationConsumer.StopConsumingAsync();

        await base.StopAsync(cancellationToken);
    }
}
using MassTransit;

namespace ElasticsearchService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ElasticFullSyncService _elasticFullSyncService;
    
    public Worker(ILogger<Worker> logger, ElasticFullSyncService elasticFullSyncService, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _elasticFullSyncService = elasticFullSyncService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _elasticFullSyncService.StartAsync(stoppingToken);
            _logger.LogInformation("Elastic Search Sycn progress is finished~");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
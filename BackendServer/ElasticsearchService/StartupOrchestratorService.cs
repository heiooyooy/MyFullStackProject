using MassTransit;

namespace ElasticsearchService;

public class StartupOrchestratorService : BackgroundService
{
    private readonly ILogger<StartupOrchestratorService> _logger;
    private readonly ElasticFullSyncService _fullSyncService; // 注入你的全量同步服务
    private readonly IBusControl _busControl; // 注入 MassTransit 的总线控制器

    public StartupOrchestratorService(
        ILogger<StartupOrchestratorService> logger,
        ElasticFullSyncService fullSyncService,
        IBusControl busControl) // MassTransit 注册后，IBusControl 就是可注入的
    {
        _logger = logger;
        _fullSyncService = fullSyncService;
        _busControl = busControl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("启动协调服务已开始...");

        try
        {
            // 第一步：执行一次性全量同步任务
            _logger.LogInformation("开始执行全量数据同步...");
            await _fullSyncService.StartAsync(stoppingToken);
            _logger.LogInformation("全量数据同步成功完成。");
            //
            // // 第二步：全量同步成功后，手动启动 RabbitMQ 消费者
            // _logger.LogInformation("正在启动 MassTransit 总线，激活 RabbitMQ 消费者...");
            //
            // // StartAsync 会连接到 RabbitMQ 并启动所有配置的 ReceiveEndpoint
            // await _busControl.StartAsync(stoppingToken);
            //
            // _logger.LogInformation("MassTransit 总线已启动，服务现在开始处理实时消息。");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "启动协调流程发生致命错误，服务将不会启动消费者。");
            // 在这里，你甚至可以决定是否要关闭整个应用
            // _hostApplicationLifetime.StopApplication();
        }

        // 这个协调服务的 ExecuteAsync 到这里就结束了。
        // 但由于 busControl.StartAsync 已经启动了 MassTransit 的内部任务，
        // 整个应用程序会因为 MassTransit 的监听而保持运行。
        _logger.LogInformation("启动协调服务任务完成。");
    }
}
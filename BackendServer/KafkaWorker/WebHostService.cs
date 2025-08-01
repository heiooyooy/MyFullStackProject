using Microsoft.AspNetCore.Builder;
using SharedModule;

namespace KafkaWorker;

public class WebHostService : IHostedService
{
    private WebApplication? _webApp;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WebHostService> _logger;

    public WebHostService(IServiceProvider serviceProvider, ILogger<WebHostService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();
        // 配置服务
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AngularDev", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Required for SignalR
            });
        });
        
        // 从主服务容器获取已注册的服务
        builder.Services.AddSignalR(options =>
        {
            // Configure keep-alive to ensure connections stay open
            options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
            options.HandshakeTimeout = TimeSpan.FromSeconds(30); // Time to complete initial handshake
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        });
  
        _webApp = builder.Build();
        _webApp.MapControllers();
        _webApp.UseCors("AngularDev");
        _webApp.UseRouting();
        
        // 注册 SignalR Hub
        _webApp.MapHub<AnalyticsHub>("/analyticsHub");
        
        // 可选：添加健康检查端点
        _webApp.MapGet("/health", () => "Worker Service with SignalR is running");
        // 添加健康检查端点
        _webApp.MapGet("/", () => new { 
            Status = "Running", 
            Service = "Worker Service with SignalR",
            Timestamp = DateTime.UtcNow 
        });
        _logger.LogInformation("Starting web host on http://localhost:5000");
        
        // 异步启动 Web 应用
        _ = Task.Run(async () =>
        {
            try
            {
                await _webApp.RunAsync("https://localhost:5000");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Web host failed to start");
            }
        }, cancellationToken);
        
        // 等待一小段时间确保 Web 应用启动
        await Task.Delay(1000, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_webApp != null)
        {
            _logger.LogInformation("Stopping web host");
            await _webApp.StopAsync(cancellationToken);
            await _webApp.DisposeAsync();
        }
    }
}
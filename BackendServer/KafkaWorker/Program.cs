using KafkaWorker;
using SharedModule;

// var builder = Host.CreateApplicationBuilder(args);
//
// // --- 1. 配置加载和依赖注入 (与您的代码相同) ---
// var basePath = AppContext.BaseDirectory;
// builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true, reloadOnChange: true);
//         
// // 这里假设您的 "RabbitMq" 节实际上是 "Kafka"
// builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
//
// builder.Services.AddHostedService<WebHostService>(); // 这里会被 .NET 主机自动调用
// builder.Services.AddHostedService<Worker>();
// builder.Services.AddSingleton<KafkaAnalyticsConsumer>();
// builder.Services.AddSingleton<IHostedService, WebHostService>();
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AngularDev", builder =>
//     {
//         builder.WithOrigins("http://localhost:5173")
//             .AllowAnyHeader()
//             .AllowAnyMethod()
//             .AllowCredentials(); // Required for SignalR
//     });
// });
// // 配置并发启动/停止（可选，提高性能）
// builder.Services.Configure<HostOptions>(options =>
// {
//     options.ServicesStartConcurrently = true;
//     options.ServicesStopConcurrently = true;
// });
//
//
//
// // --- 2. 注册 SignalR 服务 ---
// builder.Services.AddSignalR();
//
// var host = builder.Build();
// host.Run();
// 1. 使用 WebApplicationBuilder 来创建应用
var builder = WebApplication.CreateBuilder(args);


// --- 1. 配置加载和依赖注入 (与您的代码相同) ---
var basePath = AppContext.BaseDirectory;
builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true, reloadOnChange: true);
        
// 这里假设您的 "RabbitMq" 节实际上是 "Kafka"
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<KafkaAnalyticsConsumer>();
builder.Services.AddHostedService<Worker>();

// 2. 注册 SignalR 服务
builder.Services.AddSignalR();
// 3. (可选) 配置 CORS，允许前端跨域访问
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") // 你的前端地址
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// 配置中间件
app.UseRouting();

// 启用CORS
app.UseCors("AllowAll");

// 4. 映射 SignalR Hub 终结点
app.MapHub<AnalyticsHub>("/analyticsHub");

// 运行应用
// 这将同时启动 Web Host (用于SignalR) 和 Worker Service (用于后台任务)
app.Run();
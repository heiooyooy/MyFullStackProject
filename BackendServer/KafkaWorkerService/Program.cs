using KafkaWorkerService;
using SharedModule;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var basePath = AppContext.BaseDirectory;
    builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true, reloadOnChange: true);
}

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
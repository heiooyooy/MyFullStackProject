using System.Diagnostics;
using BackendServer.Extensions;
using BackendServer.Middleware;
using BackendServer.Services;
using BackendServer.Utility;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;




// ActivitySource tracingSource = new("Example.Source");

var builder = WebApplication.CreateBuilder(args);



if (builder.Environment.IsDevelopment())
{
    var basePath = AppContext.BaseDirectory;
    builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true,
        reloadOnChange: true);
}

// 从配置中读取地址（例如，得到 "seq:80"）
var seqAddress = builder.Configuration["seq"]; 
// ✅ 关键修复：在地址前加上 "http://" 协议头，构成一个完整的 URL
var seqUrl = $"http://{seqAddress}"; // 结果将是 "http://seq:80"

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // 设置最低日志级别
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // 覆盖特定来源的级别，减少干扰
    .Enrich.FromLogContext() // 使用一个 Enricher 来丰富日志上下文
    .Enrich.WithProperty("Application", "BackendServer") // 添加一个全局属性，方便在 Seq 中识别应用
    .WriteTo.Console() // **配置 Console Sink**
    .WriteTo.Seq(seqUrl)
    // .WriteTo.File( // **配置 File Sink**
    //     "logs/myapp-.txt", // 文件路径。会自动加上日期
    //     rollingInterval: RollingInterval.Day, // 每天创建一个新文件
    //     retainedFileCountLimit: 7, // 最多保留7个日志文件
    //     outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // 自定义输出格式
    .CreateBootstrapLogger();


builder.Host.UseSerilog();
// builder.Services.AddOpenTelemetry()
//     .ConfigureResource(r => r.AddService("My Service"))
//     .WithTracing(tracing =>
//     {
//         tracing.AddSource("Example.Source");
//         tracing.AddAspNetCoreInstrumentation();
//         tracing.AddHttpClientInstrumentation();
//         tracing.AddConsoleExporter();
//         tracing.AddOtlpExporter(opt =>
//         {
//             opt.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/traces");
//             opt.Protocol = OtlpExportProtocol.HttpProtobuf;
//             opt.Headers = "X-Seq-ApiKey=Ny2kLYmaWW4mIRkcMJ7f";
//         });
//     }) ;
//

builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddSingleton<LuaScriptProvider>();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();



// 使用 Serilog 的请求日志中间件
app.UseSerilogRequestLogging();

// The using statement creates a temporary service scope to ensure that any scoped services
// (like DbContext) are properly disposed of after the seeding is done.
// Using app.Services directly can cause serious issues because it resolves services from the root container,
// which lives for the entire application lifetime.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    // Apply migrations on startup
    dbContext.Database.Migrate();

    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
    //
    // var redisService = scope.ServiceProvider.GetRequiredService<RedisService>();
    // await redisService.LoadScriptsAsync();
}
    
var luaProvider = app.Services.GetRequiredService<LuaScriptProvider>();
await luaProvider.LoadAllScriptsAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Important: must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
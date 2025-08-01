using Elastic.Clients.Elasticsearch;
using ElasticsearchService;
using Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedModule.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var basePath = AppContext.BaseDirectory;
    builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true, reloadOnChange: true);
}

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
);
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMq"));
// 2. 配置并注册 Elasticsearch 客户端
builder.Services.AddSingleton(sp =>
{
    var esUri = sp.GetRequiredService<IConfiguration>()["Elasticsearch:Uri"];
    var settings = new ElasticsearchClientSettings(new Uri(esUri)).DefaultIndex("orders"); // 可以设置一个默认的索引名称
    var client = new ElasticsearchClient(settings);
    return client;
});

var test = builder.Configuration.GetConnectionString("RabbitMQ");

builder.Services.AddSingleton<ElasticFullSyncService>();

// 配置 MassTransit (连接到 RabbitMQ 并注册消费者)
builder.Services.AddMassTransit(x =>
{
    // 注册消费者
    x.AddConsumer<DataSyncEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")));
        
        // 配置接收端点，将队列绑定到消费者
        cfg.ReceiveEndpoint("es-sync-queue", e =>
        {
            e.ConfigureConsumer<DataSyncEventConsumer>(context);
        });
    });
});

// 注册我们新的“指挥官”作为托管服务
builder.Services.AddHostedService<StartupOrchestratorService>();

var host = builder.Build();
host.Run();
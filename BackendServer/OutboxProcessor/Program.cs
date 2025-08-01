using Infrastructure;
using Microsoft.EntityFrameworkCore;
using OutboxProcessor;
using SharedModule.Interfaces;
using SharedModule.RabbitMQ;
using SharedModule.Services;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var basePath = AppContext.BaseDirectory;
    builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true, reloadOnChange: true);
}

string redisConfiguration = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6378";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMq"));

// builder.Services.AddSingleton<IMessageBroker, MessageBroker>();
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
builder.Services.AddSingleton<IOutboxMessagePublisher, OutboxMessagePublisher>();


builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
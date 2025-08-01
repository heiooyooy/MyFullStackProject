using RabbitMQWorker;
using SharedModule.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var basePath = AppContext.BaseDirectory;
    builder.Configuration.AddJsonFile(Path.Combine(basePath, "sharedsettings.json"), optional: true, reloadOnChange: true);
}

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
builder.Services.AddSingleton<OutboxMessageConsumer>();
var host = builder.Build();
host.Run();
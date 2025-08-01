using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.Interfaces;

namespace SharedModule.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        try
        {
            var key = Guid.NewGuid().ToString(); // 使用一个随机key来大致均匀地分布到partition
            var value = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
            _logger.LogInformation("Successfully produced message to Kafka topic {Topic}", topic);
        }
        catch (ProduceException<string, string> e)
        {
            _logger.LogError(e, "Failed to deliver message to Kafka: {Reason}", e.Error.Reason);
            // 在真实场景中，这里可能需要一个降级策略，例如写入本地日志文件
        }
    }
    
    

    public void Dispose()
    {
        _producer.Dispose();
    }
}
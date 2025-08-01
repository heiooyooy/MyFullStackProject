using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SharedModule;

namespace KafkaWorker;

public class KafkaAnalyticsConsumer
{
    private readonly ILogger<KafkaAnalyticsConsumer> _logger;
    private readonly IHubContext<AnalyticsHub> _hubContext; // 注入 SignalR Hub 上下文
    private readonly IConsumer<string, string> _consumer;
    
    public KafkaAnalyticsConsumer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaAnalyticsConsumer> logger, IHubContext<AnalyticsHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = "seckill-dashboard-group", // 消费者组ID
            AutoOffsetReset = AutoOffsetReset.Earliest // 从最早的消息开始消费
        };
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }
    
    public async Task StartConsumingAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("seckill_events"); // 订阅我们关心的 Topic
        _logger.LogInformation("Kafka Consumer started. Subscribed to 'seckill_events' topic.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 等待并消费消息
                var consumeResult = _consumer.Consume(stoppingToken);
                if (consumeResult?.Message?.Value != null)
                {
                    _logger.LogInformation("Received message from Kafka: {Message}", consumeResult.Message.Value);

                    // 将 JSON 字符串反序列化为事件对象
                    var seckillEvent = JsonSerializer.Deserialize<SeckillClaimSucceededEvent>(consumeResult.Message.Value);

                    if (seckillEvent != null)
                    {
                        // ** 核心步骤：通过 SignalR Hub 将事件广播给所有连接的前端客户端 **
                        await _hubContext.Clients.All.SendAsync("ReceiveSeckillEvent", seckillEvent, stoppingToken);
                        // === 新增日志点 3 ===
                        _logger.LogInformation("Broadcast for event {EventId} complete.", seckillEvent.EventId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Kafka consumer stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Kafka consumer.");
                // 等待一段时间再重试，避免错误循环
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
    
    public void StopConsumingAsync()
    {
        if (_consumer != null)
        {
            _consumer.Dispose();
        }
    }
}
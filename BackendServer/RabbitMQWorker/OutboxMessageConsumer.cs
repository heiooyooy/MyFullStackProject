using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModule;
using SharedModule.RabbitMQ;

namespace RabbitMQWorker;

public class OutboxMessageConsumer
{
    private readonly IRabbitMQConnectionFactory _connectionFactory;
    private readonly ILogger<OutboxMessageConsumer> _logger;
    private IChannel? _channel;

    public OutboxMessageConsumer(
        IRabbitMQConnectionFactory connectionFactory,
        ILogger<OutboxMessageConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        // 它首先从一个共享的、可靠的连接工厂 (_connectionFactory) 获取一个信道。信道是执行绝大多数 RabbitMQ 操作的轻量级虚拟连接。
        _channel = await _connectionFactory.CreateChannelAsync();

        // 这是为了确保 RabbitMQ 中存在必要的组件。这种做法称为幂等声明，即使交换机和队列已经存在，这些命令也不会报错。
        // ExchangeDeclareAsync: 确保名为 outboxMessage.exchange 的主题交换机存在。
        // QueueDeclareAsync: 确保名为 outboxMessage.processing 的队列存在。它的作用是声明 “所有发送到名为 outboxMessage.exchange 的主题交换机的消息，都塞到这个队列来”
        // QueueBindAsync: 建立一个绑定，告诉交换机将所有 routingKey 匹配 outboxMessage.* 的消息都路由到 outboxMessage.processing 队列。
        // 发信人，即 RibbitMQ 消息的生产者，它不知道也不需要知道这个队列的存在。它只管往主题交换机发送消息。
        await _channel.ExchangeDeclareAsync(SharedConstants.OUTBOX_MESSAGE_EXCHANGE, ExchangeType.Topic, true, false);
        var queueResult = await _channel.QueueDeclareAsync("outboxmessage.processing", true, false, false);
        await _channel.QueueBindAsync(queueResult.QueueName, SharedConstants.OUTBOX_MESSAGE_EXCHANGE, SharedConstants.OUTBOX_MESSAGE_ROUTINGMATCH);

        // 这是非常关键的一步，它告诉 RabbitMQ：“一次只给我发一条消息。
        // 在我确认（Ack）处理完这条消息之前，不要给我发下一条。”
        // 这可以防止消费者因一次性接收太多消息而过载，确保了消息的平稳处理。
        await _channel.BasicQosAsync(0, 1, false);

        // 这里创建了一个异步事件消费者。核心是为其 ReceivedAsync 事件绑定一个异步委托。
        // 每当有新消息从队列到达时，这个委托就会被触发执行。
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var envelope = JsonSerializer.Deserialize<MessageEnvelope<OutboxMessage>>(json,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                if (envelope?.Data != null)
                {
                    await ProcessOutboxMessageAsync(envelope.Data);
                    // 消息确认 (Acknowledgement):
                    // 成功 (BasicAckAsync): 如果 ProcessoutboxMessageAsync 成功执行且没有抛出异常，代码会调用 BasicAckAsync。
                    // 这会告诉 RabbitMQ：“这条消息我已经成功处理完了，你可以从队列中彻底删除了。” ✅
                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                    _logger.LogInformation("Successfully processed outbox message {MessageId}", envelope.Data.Id);
                }
                else
                {
                    //失败 (BasicNackAsync): 如果发生异常或消息格式无效，代码会调用 BasicNackAsync (Negative Acknowledgement)。
                    // requeue: false: 告诉 RabbitMQ 不要把这条消息重新放回队列。这通常用于处理那些无法被修复的“毒信”（Poison Message），防止它被无限次地重复消费。
                    // requeue: true: 告诉 RabbitMQ 把消息重新放回队列的头部，以便稍后重试。这适用于那些因暂时性问题（如数据库连接抖动）而处理失败的消息。
                    _logger.LogError("Received invalid outboxMessage message");
                    await _channel.BasicNackAsync(args.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outboxMessage message");
                await _channel.BasicNackAsync(args.DeliveryTag, false, true);
            }
        };

        // 这行代码正式告诉 RabbitMQ：“好了，我已经准备好了，开始把 outboxMessage.processing 队列里的消息发给我吧！”
        // autoAck: false: 这是生产环境中必须的设置，表示我们需要手动确认消息处理。
        // 如果设为 true，消息在被投递后会立即被认为是“已处理”，如果此时消费者崩溃，消息就会丢失。
        // BasicConsumeAsync 返回的 Task 代表了整个消费者活动的生命周期。这个 Task 被设计为只要消费者还在正常监听消息，它就永远不会进入“完成”状态。
        await _channel.BasicConsumeAsync(queueResult.QueueName, false, consumer);
        _logger.LogInformation("outboxMessage consumer started");
    }

    private async Task ProcessOutboxMessageAsync(OutboxMessage outboxMessage)
    {
        // Simulate outboxMessage processing
        _logger.LogInformation("Processing outboxMessage {outboxMessageId} for customer {CustomerName} with amount {Amount}",
            outboxMessage.Id, outboxMessage.Payload, outboxMessage.ProcessedOnUtc);

        // Simulate some async work
        await Task.Delay(1000);
    }

    public async Task StopConsumingAsync()
    {
        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }
    }
}
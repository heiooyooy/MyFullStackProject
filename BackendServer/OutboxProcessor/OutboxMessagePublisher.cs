using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SharedModule;
using SharedModule.RabbitMQ;

namespace OutboxProcessor;

public class OutboxMessagePublisher : IOutboxMessagePublisher
{
    private readonly IRabbitMQConnectionFactory _connectionFactory;
    private readonly ILogger<OutboxMessagePublisher> _logger;

    public OutboxMessagePublisher(IRabbitMQConnectionFactory connectionFactory,
        ILogger<OutboxMessagePublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message,
        Dictionary<string, object>? headers = null, int priority = 0)
    {
        using var channel = await _connectionFactory.CreateChannelAsync();

        // Declare exchange
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, true, false);

        var envelope = new MessageEnvelope<T>(message);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                envelope.Headers[header.Key] = header.Value;
            }
        }

        var json = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = envelope.MessageId,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            Priority = (byte)priority,
            Headers = new Dictionary<string, object>(envelope.Headers)
        };

        await channel.BasicPublishAsync(exchange, routingKey, false, properties, body);

        _logger.LogInformation("Published message {MessageId} to exchange {Exchange} with routing key {RoutingKey}",
            envelope.MessageId, exchange, routingKey);
    }

    public async Task PublishOutboxMessageAsync(OutboxMessage outboxMessage)
    {
        await PublishAsync(SharedConstants.OUTBOX_MESSAGE_EXCHANGE, SharedConstants.OUTBOX_MESSAGE_ROUTINGKEY,
            outboxMessage,
            new Dictionary<string, object> { { "source", "api" } });
    }
}
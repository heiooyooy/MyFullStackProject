using SharedModule;

namespace OutboxProcessor;

public interface IOutboxMessagePublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T message, 
        Dictionary<string, object>? headers = null, int priority = 0);
    Task PublishOutboxMessageAsync(OutboxMessage outboxMessage);
}
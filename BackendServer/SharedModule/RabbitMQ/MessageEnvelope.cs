namespace SharedModule.RabbitMQ;

public class MessageEnvelope<T>
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string MessageType { get; set; } = typeof(T).Name;
    public T Data { get; set; }
    public Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

    public MessageEnvelope(T data)
    {
        Data = data;
    }
}
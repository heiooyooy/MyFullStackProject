namespace SharedModule;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty; // JSON-serialized data
    public DateTime? ProcessedOnUtc { get; set; }
}
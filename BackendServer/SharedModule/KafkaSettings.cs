namespace SharedModule;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ProducerTopic { get; set; } = string.Empty;
    public string ConsumerGroupId { get; set; } = string.Empty;
    public string ConsumerTopic { get; set; } = string.Empty;
}
namespace SharedModule.Interfaces;

public interface IKafkaProducer
{
    // 使用一个通用的方法来向指定 topic 发送任何类型的消息
    Task ProduceAsync<T>(string topic, T message);
}

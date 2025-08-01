using RabbitMQ.Client;

namespace SharedModule.RabbitMQ;

public interface IRabbitMQConnectionFactory
{
    Task<IConnection> CreateConnection();
    Task<IChannel> CreateChannelAsync();

    RabbitMQSettings TestSettings();
}
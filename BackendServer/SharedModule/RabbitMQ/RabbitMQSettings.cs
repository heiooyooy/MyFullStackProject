namespace SharedModule.RabbitMQ;

public class RabbitMQSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public int ConnectionTimeout { get; set; }
    public int RequestedHeartbeat { get; set; }
    public bool AutomaticRecoveryEnabled { get; set; }
    public int NetworkRecoveryInterval { get; set; }
}
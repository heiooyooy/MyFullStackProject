using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace SharedModule.RabbitMQ;

public class RabbitMQConnectionFactory : IRabbitMQConnectionFactory, IAsyncDisposable
{
    private readonly RabbitMQSettings _settings;

    private IConnection? _connection;

    // private readonly object _lock = new object();
    private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1); // Allows 1 thread at a time
    private readonly ILogger<RabbitMQConnectionFactory> _logger;

    public RabbitMQConnectionFactory(ILogger<RabbitMQConnectionFactory> logger, IOptions<RabbitMQSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<IConnection> CreateConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            await _asyncLock.WaitAsync();
            try
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _settings.HostName,
                        Port = _settings.Port,
                        UserName = _settings.UserName,
                        Password = _settings.Password,
                        VirtualHost = _settings.VirtualHost,
                        RequestedConnectionTimeout = TimeSpan.FromMilliseconds(_settings.ConnectionTimeout),
                        RequestedHeartbeat = TimeSpan.FromSeconds(_settings.RequestedHeartbeat),
                        AutomaticRecoveryEnabled = _settings.AutomaticRecoveryEnabled,
                        NetworkRecoveryInterval = TimeSpan.FromMilliseconds(_settings.NetworkRecoveryInterval),
                        // DispatchConsumersAsync = true
                    };

                    _connection = await factory.CreateConnectionAsync();
                }
            }
            finally
            {
                // Release the semaphore.
                // This must be in a finally block to ensure it's always released.
                _asyncLock.Release();
            }
        }

        return _connection;
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        var connection = await CreateConnection();
        return await connection.CreateChannelAsync();
    }


    public RabbitMQSettings TestSettings()
    {
        return this._settings;
    }

    public async ValueTask DisposeAsync()
    {
        // 这是一个更优雅的关闭方式
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}
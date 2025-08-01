using System.Text.Json;
using System.Threading.Channels;
using BackendServer.Interfaces;
using Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedModule;
using StackExchange.Redis;
using Order = SharedModule.Order;

namespace BackendServer.Services;

public class OrderService : IOrderService
{
    private readonly MyDbContext _dbContext;
    private readonly ILogger<OrderService> _logger;
    private readonly IConnectionMultiplexer _redis; // 注入Redis连接器
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(MyDbContext dbContext, 
        ILogger<OrderService> logger,
        IConnectionMultiplexer redis,
        IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _logger = logger;
        _redis = redis;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            order.OrderDate = DateTime.UtcNow;
            await _dbContext.Orders.AddAsync(order);

            var orderCreatedEvent = new { OrderId = order.OrderId, order.CustomerName, order.Amount };
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = "OrderCreatedEvent",
                Payload = JsonSerializer.Serialize(orderCreatedEvent)
            };
            await _dbContext.OutboxMessages.AddAsync(outboxMessage);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Order {OrderId} and Outbox Message {MessageId} committed to database.",
                order.OrderId, outboxMessage.Id);
            
            // 获取发布者并向 Redis Channel 发送一个轻量级信号
            var publisher = _redis.GetSubscriber();
            await publisher.PublishAsync(SharedConstants.OUTBOX_SIGNAL_CHANNEL, "new_order"); // 消息内容可以很简单

            await _publishEndpoint.Publish(new DataSyncEvent(SyncEventType.Created, order.OrderId));
            
            _logger.LogInformation("Real-time signal sent for Outbox Message {MessageId}.", outboxMessage.Id);
            return order;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Order> GetOrderById(int id)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
    }
}
using Elastic.Clients.Elasticsearch;
using Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedModule;

namespace ElasticsearchService;

public class DataSyncEventConsumer : IConsumer<DataSyncEvent>
{
    private readonly MyDbContext _dbContext;
    private readonly ElasticsearchClient _esClient;
    private readonly ILogger<DataSyncEventConsumer> _logger;

    public DataSyncEventConsumer(MyDbContext dbContext, ElasticsearchClient esClient,
        ILogger<DataSyncEventConsumer> logger)
    {
        _dbContext = dbContext;
        _esClient = esClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DataSyncEvent> context)
    {
        var syncEvent = context.Message;
        _logger.LogInformation("Received event: {Type} for EntityId: {Id}", syncEvent.EventType, syncEvent.EntityId);

        if (syncEvent.EventType == SyncEventType.Deleted)
        {
            await _esClient.DeleteAsync("orders", syncEvent.EntityId.ToString());
            return;
        }

        // 对于创建和更新，都从数据库获取最新数据
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == syncEvent.EntityId);

        if (order != null)
        {
            var orderDoc = new OrderDocument
            {
                Id = order.OrderId,
                Customer = order.CustomerName,
                OrderDate = order.OrderDate,
                Amount = order.Amount,
                Items = order.Items
            };
            // await _esClient.IndexAsync(orderDoc, idx => idx.Index("orders").Id(orderDoc.Id.ToString()));
            // 捕获响应！
            var response = await _esClient.IndexAsync(orderDoc, idx => idx.Index("orders").Id(orderDoc.Id.ToString()));

            // 检查响应是否有效！
            if (!response.IsValidResponse)
            {
                // 如果无效，记录详细的调试信息，这是金矿！
                _logger.LogError("Failed to index document {id}. Reason: {reason}. DebugInfo: {debug}",
                    orderDoc.Id, response.ElasticsearchServerError?.Error?.Reason, response.DebugInformation);
            }
            else
            {
                _logger.LogInformation("Successfully indexed document with Id {id}.", orderDoc.Id);
            }
        }
    }
}
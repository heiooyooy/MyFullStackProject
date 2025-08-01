using Elastic.Clients.Elasticsearch;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharedModule;

namespace ElasticsearchService;

public class ElasticFullSyncService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ElasticFullSyncService> _logger;

    public ElasticFullSyncService(IServiceProvider serviceProvider, ILogger<ElasticFullSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Data Hydration Service is starting.");
        await HydrateData(cancellationToken);
    }

    private async Task HydrateData(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting full data hydration from database to Elasticsearch.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var esClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();

                int page = 0;
                const int pageSize = 1000;
                bool hasMoreData = true;

                while (hasMoreData)
                {
                    var orderFromDb = await dbContext.Orders
                        .Include(o => o.Items)
                        .OrderBy(o => o.OrderId)
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken);

                    if (orderFromDb.Any())
                    {
                        _logger.LogInformation("Processing page {page} with {count} records.", page,
                            orderFromDb.Count);

                        // 使用正确的 Bulk API 语法
                        var bulkResponse = await esClient.BulkAsync(b =>
                        {
                            foreach (var order in orderFromDb)
                            {
                                var orderDocument = new OrderDocument
                                {
                                    Id = order.OrderId,
                                    Customer = order.CustomerName,
                                    OrderDate = order.OrderDate,
                                    Amount = order.Amount,
                                    Items = order.Items
                                };

                                // 正确的方式：将文档作为第一个参数传入 Index 方法
                                b.Index(orderDocument, op => op
                                        .Index("orders") // 在操作级别指定索引
                                        .Id(orderDocument.Id.ToString()) // 配置此操作的 ID
                                );
                            }
                        }, cancellationToken);


                        if (!bulkResponse.IsValidResponse)
                        {
                            _logger.LogError("Bulk indexing failed. Errors: {errors}",
                                string.Join(", ", bulkResponse.ItemsWithErrors.Select(item => item.Error.Reason)));
                            // 可以添加重试或失败处理逻辑
                        }

                        page++;
                    }
                    else
                    {
                        hasMoreData = false;
                    }
                }
            }

            _logger.LogInformation("Full data hydration completed.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Data Hydration Service is stopping.");
        return Task.CompletedTask;
    }
}
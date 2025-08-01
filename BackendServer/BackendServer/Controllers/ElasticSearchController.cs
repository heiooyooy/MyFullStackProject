using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Mvc;
using SharedModule;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElasticSearchController:ControllerBase
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchController> _logger;


    public ElasticSearchController(ElasticsearchClient client, ILogger<ElasticSearchController> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllOrders()
    {
        // 使用 SearchAsync 方法，注意其内部语法的变化
        var response = await _client.SearchAsync<Order>(s => s
            // 直接 new 一个查询对象，而不是使用层层嵌套的 Lambda
            .Query(new MatchAllQuery()) 
            .Size(1000)
        );

        // 新的响应对象检查方式
        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to get orders from Elasticsearch: {error}", response.ElasticsearchServerError);
            return StatusCode(500, "Internal Server Error");
        }

        // 获取文档的方式不变
        var orders = response.Documents;
        return Ok(orders);
    }
}
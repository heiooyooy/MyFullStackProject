using System.Text.Json;
using System.Threading.Channels;
using BackendServer.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedModule;
using SharedModule.Interfaces;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeckillController : ControllerBase
{
    private readonly RedisService _redisService;
    private readonly ChannelWriter<SeckillClaimSucceededEvent> _channelWriter;
    private readonly ILogger<SeckillController> _logger;
    private readonly IOptions<KafkaSettings> _kafkaSettings;

    public SeckillController(RedisService redisService,
        ChannelWriter<SeckillClaimSucceededEvent> channelWriter,
        ILogger<SeckillController> logger,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _redisService = redisService;
        _channelWriter = channelWriter;
        _logger = logger;
        _kafkaSettings = kafkaSettings;
    }

    // 用于测试的预热接口
    [HttpPost("preheat")]
    public async Task<IActionResult> Preheat([FromBody] PreheatRequest request)
    {
        await _redisService.PreheatStockAsync(request.ProductId, request.Stock);
        return Ok($"Stock for product {request.ProductId} is preheated to {request.Stock}.");
    }

    [HttpPost("claim")]
    public async Task<IActionResult> ClaimStock([FromBody] ClaimRequest request)
    {
        var result = await _redisService.ExecuteClaimStockScriptAsync(request.ProductId, request.UserId);


        switch (result)
        {
            case SeckillResult.Success:
                // 抢购成功，立即将创建订单的任务推送到后台队列
                var successEvent = new SeckillClaimSucceededEvent(Guid.NewGuid().ToString(), request.ProductId,
                    request.UserId, DateTime.UtcNow, HttpContext.Request.Headers["User-Agent"].ToString(),
                    HttpContext.Connection.RemoteIpAddress?.ToString());
                await _channelWriter.WriteAsync(successEvent);

                _logger.LogWarning("SUCCESS: User '{UserId}' claimed stock for Product '{ProductId}'.", request.UserId,
                    request.ProductId);
                // 立即返回成功，告知用户抢购成功，订单正在处理
                return Accepted(new { Message = "恭喜，抢购成功！订单正在后台创建中。" });

            case SeckillResult.OutOfStock:
                _logger.LogInformation(
                    "FAILED (Out of Stock): User '{UserId}' failed to claim stock for Product '{ProductId}'.",
                    request.UserId, request.ProductId);
                return Conflict(new { Message = "太遗憾了，商品已被抢光！" });

            case SeckillResult.AlreadyClaimed:
                _logger.LogInformation(
                    "FAILED (Already Claimed): User '{UserId}' tried to claim stock for Product '{ProductId}' again.",
                    request.UserId, request.ProductId);
                return Conflict(new { Message = "您已成功抢购，请勿重复下单！" });

            case SeckillResult.Error:
            default:
                _logger.LogError(
                    "ERROR: An unexpected error occurred for User '{UserId}' claiming Product '{ProductId}'.",
                    request.UserId, request.ProductId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "系统繁忙，请稍后再试！" });
        }
    }

    [HttpGet("history")]
    public ActionResult<IList<SeckillClaimSucceededEvent>> GetAllMessages()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.Value.BootstrapServers,
            // 每次请求都创建一个新的、唯一的消费者组，以确保从头开始
            GroupId = $"history-reader-{Guid.NewGuid()}",
            // 从最早的消息开始读取
            AutoOffsetReset = AutoOffsetReset.Earliest,
            // 我们不关心提交偏移量
            EnableAutoCommit = false,
            // 如果 Topic 很大，这个读取过程可能会很长，增加会话超时时间
            SessionTimeoutMs = 30000 // 30秒
        };

        var messages = new List<SeckillClaimSucceededEvent>();

        // 使用 using 确保消费者被正确关闭和释放
        using (var consumer = new ConsumerBuilder<Ignore, SeckillClaimSucceededEvent>(config)
                   .SetValueDeserializer(new JsonDeserializer<SeckillClaimSucceededEvent>())
                   .Build()
              )
        {
            try
            {
                consumer.Subscribe("seckill_events");

                while (true)
                {
                    // 设置一个超时时间，当读到末尾时可以优雅退出
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(2));

                    if (consumeResult == null)
                    {
                        // 在2秒内没有读到新消息，我们认为已经到达了 Topic 的末尾
                        break;
                    }

                    // 将消息内容添加到列表中
                    if (consumeResult.Message?.Value != null)
                    {
                        messages.Add(consumeResult.Message.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                // 在生产环境中，这里应该使用结构化日志记录错误
                // 返回一个服务器错误响应
                return StatusCode(500, $"从 Kafka 读取数据时发生错误: {ex.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }

        // 返回包含所有消息的列表
        return Ok(messages);
    }
}

public record ClaimRequest(string ProductId, string UserId);

public record PreheatRequest(string ProductId, int Stock);

// Confluent.Kafka 提供的 JSON (反)序列化器
// 您可能需要自己实现或引用一个实现了 ISerializer/IDeserializer 的库
// 下面是一个基于 System.Text.Json 的简单实现示例

public class JsonDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(data)!;
    }
}
using System.Text;
using BackendServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SSEController : ControllerBase
{
    private readonly SseNotificationService _notificationService;

    public SSEController(SseNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("time-stream")]
    public async Task GetTimeStream()
    {
        // 1. 设置关键的 HTTP 响应头
        // Content-Type 必须是 "text/event-stream"
        Response.Headers.Add("Content-Type", "text/event-stream");
        // 建议禁用缓存
        Response.Headers.Add("Cache-Control", "no-cache");
        // 用于 Nginx 等反向代理，禁用响应缓冲
        Response.Headers.Add("X-Accel-Buffering", "no");
        Response.Headers.Add("Access-Control-Allow-Origin", "*");

        var cancellationToken = HttpContext.RequestAborted;
        var messageId = 0;

        // 2. 使用一个循环来保持连接，并持续发送数据
        //    循环的退出条件是客户端关闭了连接
        while (!cancellationToken.IsCancellationRequested)
        {
            messageId++;
            var currentTime = DateTime.Now.ToString("HH:mm:ss");

            // 3. 构建符合 SSE 格式的文本数据
            //    每条消息以两个换行符 `\n\n` 结尾
            var eventData = $"id: {messageId}\n" +
                            $"event: timeUpdate\n" +
                            $"data: The server time is {currentTime}\n\n";

            // 4. 将数据写入响应体
            byte[] dataBytes = Encoding.UTF8.GetBytes(eventData);
            await Response.Body.WriteAsync(dataBytes, 0, dataBytes.Length, cancellationToken);

            // 5. **非常重要**：刷新响应体，确保数据被立即发送到客户端
            await Response.Body.FlushAsync(cancellationToken);

            // 6. 等待一段时间再发送下一条消息
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // 客户端关闭连接时，Task.Delay 会抛出此异常，是正常退出循环的方式
                break;
            }
        }
    }


    [HttpGet("channel-message")]
    public async Task GetChannelMessages()
    {
        // 1. 设置关键的 HTTP 响应头
        // Content-Type 必须是 "text/event-stream"
        Response.Headers.Add("Content-Type", "text/event-stream");
        // 建议禁用缓存
        Response.Headers.Add("Cache-Control", "no-cache");
        // 用于 Nginx 等反向代理，禁用响应缓冲
        Response.Headers.Add("X-Accel-Buffering", "no");
        Response.Headers.Add("Access-Control-Allow-Origin", "*");

        var cancellationToken = HttpContext.RequestAborted;
        var channelReader = _notificationService.CreateReader();

        // **核心改变**：使用 await foreach 循环来异步地从 Channel 中读取数据
        // 这个循环会一直等待，直到 Channel 中有新数据写入，或者 Channel 被关闭。
        await foreach (var message in channelReader.ReadAllAsync(cancellationToken))
        {
            try
            {
                // 构建并发送 SSE 消息
                var eventData = $"data: {message}\n\n";
                byte[] dataBytes = Encoding.UTF8.GetBytes(eventData);
                await Response.Body.WriteAsync(dataBytes, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 客户端关闭连接，这是正常退出
                break;
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSSEMessage([FromBody] string orderDetails)
    {
        // 1. ... 在这里处理创建订单的业务逻辑 ...
        Console.WriteLine($"新订单已创建: {orderDetails}");

        // 2. 业务逻辑完成后，发布一个通知
        //    这个消息将被写入共享的 Channel 中
        var notificationMessage = $"新订单通知: {orderDetails} - 创建于 {DateTime.Now:HH:mm:ss}";
        await _notificationService.Writer.WriteAsync(notificationMessage);

        return Ok("订单已创建，并已发送通知。");
    }
}
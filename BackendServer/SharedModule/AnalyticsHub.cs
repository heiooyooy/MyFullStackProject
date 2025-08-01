using Microsoft.AspNetCore.SignalR;

namespace SharedModule;

public class AnalyticsHub : Hub
{
    // 当有新客户端连接时，可以打印一条日志
    public override async Task OnConnectedAsync()
    {
        // 可以将连接ID与用户关联等高级操作
        await base.OnConnectedAsync();
        Console.WriteLine($"A client connected: {Context.ConnectionId}");
    }

    // 当有客户端断开连接时
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"A client disconnected: {Context.ConnectionId}");
    }
}
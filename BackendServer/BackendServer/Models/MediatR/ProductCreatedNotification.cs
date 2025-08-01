using MediatR;

namespace BackendServer.Models.MediatR;

// --- Notification ---
public record ProductCreatedNotification(ProductDto Product) : INotification;

// --- Handlers (可以有多个) ---
// 处理器1：发送欢迎邮件
public class SendWelcomeEmailHandler : INotificationHandler<ProductCreatedNotification>
{
    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"邮件模块：正在为新产品 '{notification.Product.Name}' 发送欢迎邮件...");
        // ... 发送邮件的逻辑 ...
        return Task.CompletedTask;
    }
}

// 处理器2：更新缓存
public class InvalidateCacheHandler : INotificationHandler<ProductCreatedNotification>
{
    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"缓存模块：产品 '{notification.Product.Name}' 已创建，相关缓存已失效...");
        // ... 清理缓存的逻辑 ...
        return Task.CompletedTask;
    }
}
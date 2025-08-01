using System.Threading.Channels;

namespace BackendServer.Services;

public class SseNotificationService
{
    // 创建一个无界 Channel，作为所有通知的队列。
    // 你也可以使用 CreateBounded 创建有界 Channel 来提供背压。
    private readonly Channel<string> _notificationChannel = Channel.CreateUnbounded<string>();

    // 提供对 Channel 的写入器的公共访问，以便其他服务可以发送通知
    public ChannelWriter<string> Writer => _notificationChannel.Writer;

    // 提供一个方法来获取 Channel 的读取器，供 SSE 控制器使用
    public ChannelReader<string> CreateReader() => _notificationChannel.Reader;
}
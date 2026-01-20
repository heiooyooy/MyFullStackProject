// 1. 修复后的前端代码 - 解决事件监听器时机问题
import { useEffect, useState } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

const SignalRTest = () => {
  const [events, setEvents] = useState<SeckillEvent[]>([]);
  const [totalClaims, setTotalClaims] = useState(0);

  // useEffect 会在组件加载时运行，非常适合建立连接
  useEffect(() => {
    // 创建 SignalR 连接
    const connection = new HubConnectionBuilder()
      .withUrl("/analyticsHub") // <-- 注意：这里的端口号要换成你后端API的HTTPS端口号
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect() // 添加自动重连
      .build();

    // 启动连接
    connection
      .start()
      .then(() => console.log("SignalR Connected!"))
      .catch((err) => console.error("SignalR Connection Error: ", err));

    // 监听来自服务器的 "ReceiveSeckillEvent" 事件
    connection.on("ReceiveSeckillEvent", (newEvent) => {
      console.log("New event received:", newEvent);

      // 更新状态，将新事件添加到列表的顶部
      setEvents((prevEvents) => [newEvent, ...prevEvents]);
      setTotalClaims((prevTotal) => prevTotal + 1);
    });

    // 组件卸载时，清理并关闭连接
    return () => {
      connection.stop();
    };
  }, []); // 空依赖数组意味着这个 effect 只在组件初次挂载时运行一次

  return (
    // 全局容器：深灰色背景，白色文字，使用 flex 布局，最小高度占满整个屏幕
    <div className="bg-gray-900 text-white min-h-80 font-sans">
      {/* 内容容器：设置最大宽度，水平居中 */}
      <div className="container mx-auto p-4 sm:p-6 lg:p-8">
        <header className="text-center mb-8">
          {/* 主标题：大号字体，粗体，渐变色文字效果，增加底部间距 */}
          <h1 className="text-4xl sm:text-5xl font-extrabold tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-teal-500">
            SignalR Test
          </h1>
        </header>

        {/* 主内容区：使用 Grid 布局实现响应式 */}
        {/* 在大屏幕上是 3 列布局，在小屏幕上是 1 列布局 */}
        <main className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* 左侧总览卡片 */}
          <div className="bg-gray-800 p-6 rounded-2xl shadow-lg flex flex-col items-center justify-center">
            {/* 卡片标题：浅灰色文字，增加底部间距 */}
            <h2 className="text-xl font-semibold text-gray-400 mb-4">
              总成功抢购数
            </h2>
            {/* 主要数据：超大号字体，极粗，醒目的白色 */}
            <p className="text-7xl font-extrabold text-white">
              {totalClaims.toLocaleString()}
            </p>
          </div>

          {/* 右侧实时记录 */}
          {/* 在大屏幕上占据 2 列空间 */}
          <div className="lg:col-span-2 bg-gray-800 p-6 rounded-2xl shadow-lg">
            <h2 className="text-xl font-semibold text-gray-400 mb-4">
              实时成功记录
            </h2>
            {/* 列表容器：设置最大高度并允许垂直滚动，美化滚动条 */}
            <ul className="space-y-3 max-h-96 overflow-y-auto pr-2">
              {events.map((event, index) => (
                // 列表项：动画效果，深色背景，圆角，内边距
                <li
                  key={event.eventId}
                  className="bg-gray-700/50 p-4 rounded-lg flex flex-wrap items-center gap-x-4 gap-y-2 text-sm transition-all duration-300 hover:bg-gray-700"
                >
                  {/* 使用 flex 布局来更好地对齐每一项数据 */}
                  <span className="flex items-center">
                    <strong className="text-gray-400 font-medium mr-2">
                      用户:
                    </strong>
                    <span className="text-blue-300">{event.userId}</span>
                  </span>

                  <span className="flex items-center">
                    <strong className="text-gray-400 font-medium mr-2">
                      商品:
                    </strong>
                    <span className="text-teal-300">{event.productId}</span>
                  </span>

                  <span className="flex items-center">
                    <strong className="text-gray-400 font-medium mr-2">
                      时间:
                    </strong>
                    <span className="text-gray-300">
                      {new Date(event.timestampUtc).toLocaleTimeString()}
                    </span>
                  </span>

                  <span className="flex items-center">
                    <strong className="text-gray-400 font-medium mr-2">
                      IP:
                    </strong>
                    <span className="text-gray-300">{event.ipAddress}</span>
                  </span>
                </li>
              ))}
            </ul>
          </div>
        </main>
      </div>
    </div>
  );
};

export default SignalRTest;

type SeckillEvent = {
  eventId: string;
  productId: string;
  userId: string;
  timestampUtc: string;
  userAgent: string | null;
  ipAddress: string | null;
};

// 3. 后端验证建议
/*
在你的后端 Kafka 消费者中添加这些日志来确认消息发送：

```csharp
// 在发送前添加
_logger.LogInformation("About to broadcast event {EventId} to {ClientCount} clients", 
    seckillEvent.EventId, 
    _hubContext.Clients.All.GetType());

// 发送消息
await _hubContext.Clients.All.SendAsync("ReceiveSeckillEvent", seckillEvent, stoppingToken);

// 发送后添加
_logger.LogInformation("Successfully broadcast event {EventId}", seckillEvent.EventId);
```

4. 网络层面的验证：
- 打开浏览器 F12 -> Network 标签
- 过滤 WS (WebSocket) 连接
- 查看是否有数据在 WebSocket 连接上传输
- 如果有数据传输但前端没收到，就是前端代码问题
- 如果没有数据传输，就是后端问题

5. 立即测试步骤：
1) 替换你的组件为上面的修复版本
2) 同时渲染 SignalRDebugComponent 来监控原始消息
3) 触发一个秒杀事件
4) 查看浏览器控制台输出
5) 检查调试面板是否显示收到的消息
*/

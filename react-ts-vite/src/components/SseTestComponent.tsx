import React, { useEffect, useState } from "react";

const SseTestComponent = () => {
  // 使用 useState 来存储从服务器接收到的数据
  const [serverTime, setServerTime] = useState("正在等待服务器时间...");
  // 使用另一个 state 来追踪连接状态，以便在 UI 上显示
  const [connectionStatus, setConnectionStatus] = useState("正在连接...");

  useEffect(() => {
    // useEffect 的这个函数体会在组件第一次挂载（mount）后执行
    console.log("组件已挂载，开始建立 SSE 连接...");

    // 1. 创建 EventSource 实例
    const eventSource = new EventSource("/api/sse/time-stream");

    // 2. 定义事件监听器
    // 当连接成功打开时触发
    eventSource.onopen = () => {
      console.log("SSE 连接已成功打开。");
      setConnectionStatus("已连接");
    };

    // 监听我们自定义的 'timeUpdate' 事件
    eventSource.addEventListener("timeUpdate", (event) => {
      // 使用 setServerTime 更新组件的 state，这将触发 UI 重新渲染
      setServerTime(event.data);
    });
    eventSource.onmessage = (event: MessageEvent) => {
      setServerTime(event.data);
    };

    // 当发生错误时触发
    eventSource.onerror = (error) => {
      console.error("SSE 连接发生错误:", error);
      setConnectionStatus("连接错误！");
      // 发生错误时，浏览器会自动尝试重连，但如果错误是致命的，
      // 或者我们想停止重连，可以手动关闭它。
      eventSource.close();
    };

    // 3. **非常重要**：定义清理函数 (Cleanup Function)
    //    这个函数会在组件即将卸载（unmount）时执行
    return () => {
      console.log("组件即将卸载，关闭 SSE 连接...");
      eventSource.close();
    };
  }, []); // 依赖项数组为空 `[]`，确保这个 effect 只在组件挂载和卸载时执行一次

  // 渲染组件的 UI
  return (
    <div
      style={{ border: "1px solid #ccc", padding: "20px", borderRadius: "8px" }}
    >
      <h2>服务器实时时间 (SSE)</h2>
      <p>
        连接状态: <strong>{connectionStatus}</strong>
      </p>
      <div style={{ fontSize: "2em", fontFamily: "monospace", color: "#333" }}>
        {serverTime}
      </div>
    </div>
  );
};

export default SseTestComponent;

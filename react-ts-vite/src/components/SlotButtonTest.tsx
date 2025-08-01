// src/App.tsx

import { Button } from "../shared/UI/button";

function SlotButtonTest() {
  return (
    <div className="p-10 flex flex-col items-start gap-4">
      <h1 className="text-2xl font-bold">Radix UI Slot 完整示例</h1>

      {/* --- 场景 A: 默认用法 (asChild=false) --- */}
      <div>
        <h2 className="text-lg font-semibold mb-2">场景 A: 普通按钮</h2>
        <Button onClick={() => alert("这是一个真正的 <button> 元素！")}>
          点我 (是一个 Button)
        </Button>
      </div>

      {/* --- 场景 B: 使用 asChild 和 Slot --- */}
      <div>
        <h2 className="text-lg font-semibold mb-2">场景 B: 将样式赋予链接</h2>
        <Button asChild>
          <a
            href="https://www.google.com"
            target="_blank"
            rel="noopener noreferrer"
          >
            去 Google (是一个 a 标签)
          </a>
        </Button>
      </div>
    </div>
  );
}

export default SlotButtonTest;

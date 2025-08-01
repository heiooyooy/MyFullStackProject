// src/components/ui/Button.tsx

import * as React from "react";
import { Slot } from "@radix-ui/react-slot"; // 1. 导入 Slot 组件
import { cn } from "../cn";

// 2. 定义 Button 组件接收的 Props 类型
export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  asChild?: boolean; // 添加一个可选的 asChild prop
}

// 3. 创建 Button 组件，并使用 React.forwardRef 来传递 ref
const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, asChild = false, ...props }, ref) => {
    // 4. 这是核心逻辑：根据 asChild 的值，决定渲染的“宿主”组件
    //    如果 asChild 是 true，使用 Slot；否则，使用 'button' 字符串
    const Comp = asChild ? Slot : "button";

    // 5. 渲染这个动态的 Comp 组件
    return (
      <Comp
        className={cn(
          // 这里是我们按钮的基础样式
          "inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50",
          // 一些默认的视觉样式
          "bg-blue-600 text-white hover:bg-blue-700 h-10 px-4 py-2",
          className // 允许从外部传入额外的 className
        )}
        ref={ref} // 将 ref 传递下去
        {...props} // 将所有其他 props（如 onClick, children, disabled 等）传递下去
      />
    );
  }
);
Button.displayName = "Button";

export { Button };

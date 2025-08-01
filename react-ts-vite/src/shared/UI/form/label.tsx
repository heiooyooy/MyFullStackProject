// 文件路径: components/ui/label.tsx

// 1. 导入 React 核心库和类型
import * as React from "react";
// 2. 导入 Radix UI 的 Label Primitive，并将其所有导出项放入 LabelPrimitive 对象中
import * as LabelPrimitive from "@radix-ui/react-label";
// 3. 导入 cva 用于创建样式变体
import { cva, type VariantProps } from "class-variance-authority";
// 4. 导入我们将在下一步创建的 className 合并工具
import { cn } from "../../cn";

// 5. 使用 cva 定义 Label 的基础样式
const labelVariants = cva(
  "text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
);

// 6. 定义我们的 Label 组件
const Label = React.forwardRef<
  // 7. 使用 forwardRef 使组件能接收 ref
  // 8. 第一个泛型：ref 指向的元素类型。这里是 Radix Label 的根元素，即 <label> HTML 元素
  React.ElementRef<typeof LabelPrimitive.Root>,
  // 9. 第二个泛型：组件的 props 类型。
  // 它继承了 Radix Label 的所有原生 props（如 htmlFor），且不包含 ref
  React.ComponentPropsWithoutRef<typeof LabelPrimitive.Root> &
    // 10. 并合并了 cva 创建的变体 props 类型（本例中未使用，但为标准写法）
    VariantProps<typeof labelVariants>
>(
  (
    { className, ...props },
    ref // 11. 组件的实现，接收 className, 其他 props, 和 ref
  ) => (
    // 12. 渲染 Radix 的原始 Label 组件
    <LabelPrimitive.Root
      ref={ref} // 13. 将接收到的 ref 传递给原始组件
      // 14. 调用 cn 函数，合并基础样式和从外部传入的自定义样式
      className={cn(labelVariants(), className)}
      {...props} // 15. 将所有其他 props（如 children, htmlFor）传递下去
    />
  )
);
// 16. 为组件添加 displayName，便于在 React DevTools 中调试
Label.displayName = LabelPrimitive.Root.displayName;

// 17. 导出组件
export { Label };

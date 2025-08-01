// 文件路径: App.tsx

// 1. 导入我们刚刚创建的 Label 组件
import { Label } from "./label";
// 2. 为了演示，我们还需要一个简单的 Input 组件
import { Input } from "./input"; // 假设你已经创建了之前展示的 Input 组件

function MyLabelTest() {
  return (
    // 为了美观，添加一些布局和背景色
    <div className="bg-gray-900 text-white min-h-screen p-10 font-sans">
      <h1 className="text-2xl font-bold mb-6">Label 组件使用示例</h1>

      {/* --- 示例 1: 基础用法 --- */}
      <div className="mb-8">
        <p className="mb-2 text-gray-400">
          1. 基础用法：点击 Label 可以聚焦到 Input。
        </p>
        <div className="flex flex-col space-y-2 max-w-sm">
          {/* `htmlFor` 的值必须和 Input 的 `id` 完全一致 */}
          <Label htmlFor="email">邮箱地址</Label>
          <Input type="email" id="email" placeholder="your.email@example.com" />
        </div>
      </div>

      {/* --- 示例 2: 自定义样式 --- */}
      <div className="mb-8">
        <p className="mb-2 text-gray-400">2. 通过 className 传入自定义样式。</p>
        <div className="flex flex-col space-y-2 max-w-sm">
          <Label htmlFor="username" className="text-cyan-400 font-bold text-lg">
            用户名
          </Label>
          <Input type="text" id="username" placeholder="Your awesome name" />
        </div>
      </div>

      {/* --- 示例 3: peer-disabled 效果 --- */}
      <div>
        <p className="mb-2 text-gray-400">
          3. peer-disabled 效果：当 Input 被禁用时，Label 样式会自动改变。
        </p>
        <div className="flex items-center space-x-3">
          {/* 在 Input 上添加 `peer` class */}
          <Input type="checkbox" id="terms" disabled className="peer size-4" />
          {/* 这个 Label 会自动检测到同级的 peer input 是 disabled 状态 */}
          <Label htmlFor="terms">我同意服务条款（已禁用）</Label>
        </div>
      </div>
    </div>
  );
}

export default MyLabelTest;

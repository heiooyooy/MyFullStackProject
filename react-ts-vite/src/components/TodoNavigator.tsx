import Button from "@mui/material/Button";
import { useState } from "react";
import { useGetTodoByIdQuery } from "../shared/todoRTKQuery";
import { Loader2 } from "lucide-react"; // 导入一个加载图标
const TodoNavigator = () => {
  // 1. 使用 useState 来保存和管理当前的 ID，初始值为 1
  const [currentId, setCurrentId] = useState(1);

  // 2. 将 state 变量作为参数传给 query hook
  const {
    data: todo,
    error,
    isLoading, // 只在第一次为某个 ID 加载时为 true
    isFetching, // 任何时候正在请求时都为 true
  } = useGetTodoByIdQuery(currentId);

  // 3. 按钮点击事件处理器，只负责更新 state
  const handleFetchNext = () => {
    setCurrentId((prevId) => prevId + 1);
  };

  const handleFetchPrev = () => {
    // 确保 ID 不会小于 1
    setCurrentId((prevId) => Math.max(1, prevId - 1));
  };

  return (
    <div className="p-4 max-w-md mx-auto mt-10 border rounded-lg shadow-lg">
      <h1 className="text-xl font-bold text-center mb-4">Todo 导航器</h1>

      {/* 控制按钮区域 */}
      <div className="flex justify-center gap-4 mb-4">
        <Button
          onClick={handleFetchPrev}
          disabled={isFetching || currentId <= 1}
        >
          上一个 (ID: {currentId - 1})
        </Button>
        <Button onClick={handleFetchNext} disabled={isFetching}>
          {isFetching ? "..." : `下一个 (ID: ${currentId + 1})`}
        </Button>
      </div>

      {/* 数据展示区域 */}
      <div className="h-24 p-4 border rounded bg-gray-50 flex items-center justify-center">
        {/* isFetching 用于显示所有加载状态，包括后台刷新 */}
        {isFetching ? (
          <div className="flex items-center text-gray-500">
            <Loader2 className="mr-2 h-5 w-5 animate-spin" />
            正在加载 ID: {currentId} 的数据...
          </div>
        ) : error ? (
          <div className="text-red-500">
            加载失败！ID: {currentId} 可能不存在。
          </div>
        ) : todo ? (
          <div>
            <h2 className="font-semibold text-lg">
              ({todo.id}) {todo.title}
            </h2>
            <p>状态: {todo.completed ? "✅ 已完成" : "❌ 未完成"}</p>
          </div>
        ) : null}
      </div>
    </div>
  );
};

export default TodoNavigator;

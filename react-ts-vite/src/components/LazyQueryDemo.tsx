import Button from "@mui/material/Button";
import { Loader2 } from "lucide-react";
import {
  useLazyGetTodosQuery,
  useLazyGetTodoByIdQuery,
} from "../shared/todoRTKQuery";
import type { TodoItem } from "../shared/useFetch";
import { useMemo } from "react";
import Box from "@mui/material/Box";

const LazyQueryDemo = () => {
  // 1. 设置 useLazyGetTodosQuery
  //    - fetchAllTodosTrigger 是我们用来触发“获取全部”操作的函数
  //    - listResult 对象包含了列表数据的状态
  const [fetchAllTodosTrigger, listResult] = useLazyGetTodosQuery();
  const { data: todos, isFetching: isFetchingList } = listResult;

  const firstFiveTodos = useMemo(() => {
    if (!todos) return [];

    return todos.slice(0, 5);
  }, [todos]);

  // 2. 设置 useLazyGetTodoByIdQuery
  //    - fetchTodoByIdTrigger 是我们用来触发“获取单个”操作的函数
  //    - detailResult 对象包含了单个 todo 数据的状态
  const [fetchTodoByIdTrigger, detailResult] = useLazyGetTodoByIdQuery();
  const { data: singleTodo, isFetching: isFetchingDetail } = detailResult;

  // --- 事件处理器 ---
  const handleFetchList = () => {
    // 调用第一个触发器，不需要参数
    fetchAllTodosTrigger(undefined);
  };

  const handleFetchDetail = (id: number) => {
    // 调用第二个触发器，需要传入 ID
    fetchTodoByIdTrigger(id);
  };

  return (
    <div className="p-6 max-w-2xl mx-auto font-sans">
      <h1 className="text-2xl font-bold mb-4">Lazy Query 示例</h1>

      <Box sx={{ marginBottom: "1rem" }}>
        <Button onClick={handleFetchList} disabled={isFetchingList}>
          {isFetchingList ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" /> 加载列表中...
            </>
          ) : (
            "加载所有 Todos"
          )}
        </Button>
      </Box>

      <div className="grid grid-cols-2 gap-6">
        {/* 左侧：列表区域 */}
        <div className="border p-4 rounded-lg">
          <h2 className="font-semibold mb-2">Todos 列表</h2>
          {todos ? (
            <ul className="space-y-2">
              {firstFiveTodos.map((todo: TodoItem) => (
                <li key={todo.id}>
                  <Button
                    className="w-full justify-start text-left"
                    onClick={() => handleFetchDetail(todo.id)}
                  >
                    {todo.id}. {todo.title}
                  </Button>
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-gray-500">请先点击按钮加载列表。</p>
          )}
        </div>

        {/* 右侧：详情区域 */}
        <div className="border p-4 rounded-lg">
          <h2 className="font-semibold mb-2">Todo 详情</h2>
          {isFetchingDetail ? (
            <div className="flex items-center text-gray-500">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              正在加载详情...
            </div>
          ) : singleTodo ? (
            <div>
              <p>
                <strong>ID:</strong> {singleTodo.id}
              </p>
              <p>
                <strong>标题:</strong> {singleTodo.title}
              </p>
              <p>
                <strong>状态:</strong>{" "}
                {singleTodo.completed ? "✅ 已完成" : "❌ 未完成"}
              </p>
            </div>
          ) : (
            <p className="text-gray-500">请点击左侧列表中的一项来查看详情。</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default LazyQueryDemo;

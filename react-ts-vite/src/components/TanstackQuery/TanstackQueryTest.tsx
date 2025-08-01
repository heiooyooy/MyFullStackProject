import { useQuery, useQueryClient } from "@tanstack/react-query";
import React from "react";
import type { TodoItem } from "../../shared/useFetch";
import Button from "@mui/material/Button";
import { useAppDispatch } from "../../shared/storehooks";
import { updateTodoId } from "../../shared/todoSlice";

const fetchTodosApi = async (): Promise<TodoItem[]> => {
  await new Promise((res) => setTimeout(res, 1000));
  console.log("Fetching all todos...");
  const res = await fetch(
    "https://jsonplaceholder.typicode.com/todos?_limit=5"
  );
  return res.json();
};

export const fetchTodoByIdAPI = async (id: string) => {
  console.log(`Fetching todo with id: ${id}...`);
  const res = await fetch(`https://jsonplaceholder.typicode.com/todos/${id}`);
  return res.json();
};

export const todoDetailQueryOptions = (todoId: number | null) => ({
  queryKey: ["todos", todoId],
  queryFn: () => {
    if (!todoId) {
      // 如果没有 todoId，我们不应该发起请求，可以返回 null 或抛出错误
      // 但因为下面有 enabled 控制，这里通常不会被调用
      return Promise.resolve(null);
    }
    return fetchTodoByIdAPI(String(todoId));
  },
  enabled: !!todoId, // 将 enabled 逻辑也包含进来
});

const TanstackQueryTest = () => {
  const queryClient = useQueryClient();
  const dispatch = useAppDispatch();

  const {
    data: todos,
    isLoading,
    isFetching,
    refetch,
  } = useQuery({
    queryKey: ["todos"],
    queryFn: fetchTodosApi,
  });

  const handleFetchClick = () => {
    refetch();
  };

  const handleTaskClick = (todoId: number) => {
    dispatch(updateTodoId(todoId));
    queryClient.fetchQuery(todoDetailQueryOptions(todoId));
  };

  return (
    <div>
      <Button onClick={handleFetchClick}>
        {/* 当正在获取数据时，显示不同文本 */}
        {isFetching ? "获取中..." : "获取待办事项"}
      </Button>
      {isLoading && <p className="mt-4">准备加载数据...</p>}
      <ul>
        {todos?.map((item) => (
          <li key={item.id}>
            <button
              onClick={() => handleTaskClick(item.id)}
              className="w-full text-left  hover:bg-gray-100 transition-colors cursor-pointer"
            >
              {item.title}
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default TanstackQueryTest;

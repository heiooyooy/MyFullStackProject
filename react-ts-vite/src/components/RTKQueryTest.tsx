import React from "react";
import { useGetTodoByIdQuery, useGetTodosQuery } from "../shared/todoRTKQuery";
import type { TodoItem } from "../shared/useFetch";
import Button from "@mui/material/Button";
import TodoNavigator from "./TodoNavigator";
import LazyQueryDemo from "./LazyQueryDemo";

const RTKQueryTest = () => {
  // 1. 为了避免命名冲突，重命名第一个 query 返回的加载状态
  const {
    data: todos,
    error: todosError,
    isLoading: todosIsLoading,
  } = useGetTodosQuery(undefined);

  // 2. 只有在 todos 存在时才计算 ID
  const todoIdToFetch = todos?.[0]?.id;

  // 3. 在第二个 query 中使用 skip 选项
  const {
    data: todoWithId,
    error: todoByIdError,
    isLoading: todoByIdIsLoading, // 也获取第二个 query 的加载状态
  } = useGetTodoByIdQuery(
    todoIdToFetch ? todoIdToFetch + 1 : undefined, // 传递计算后的 ID 或 undefined
    {
      // 关键！如果没有要获取的 ID，就跳过查询
      skip: !todoIdToFetch,
    }
  );

  // 4. 检查两个 query 的加载和错误状态
  if (todosIsLoading || todoByIdIsLoading) {
    return <div>加载中...</div>;
  }
  if (todosError || todoByIdError) {
    return <div>获取失败</div>;
  }

  // 确保 todos 存在且不为空才进行渲染
  if (!todos || todos.length === 0) {
    return <div>没有待办事项</div>;
  }

  return (
    <div>
      <ul>
        {/* 只显示第一项作为示例 */}
        {todos.slice(0, 1).map((item: TodoItem) => (
          <li key={item.id}>
            <Button>{item.title}</Button>
          </li>
        ))}
      </ul>

      <hr className="my-4" />

      <h2 className="font-bold">依赖查询的结果：</h2>
      {todoWithId ? (
        <div>
          <h3>{todoWithId.title}</h3>
          <p>状态: {todoWithId.completed ? "已完成" : "未完成"}</p>
        </div>
      ) : (
        <div>正在等待或加载依赖数据...</div>
      )}
      <hr className="my-4" />
      <TodoNavigator />
    </div>
  );
};

export default RTKQueryTest;

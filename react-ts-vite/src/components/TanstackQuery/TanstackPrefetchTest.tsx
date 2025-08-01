import {
  useQuery, // 1. 导入核心 Hook
} from "@tanstack/react-query";
import { todoDetailQueryOptions } from "./TanstackQueryTest";
import { useAppSelector } from "../../shared/storehooks";

const TanstackPrefetchComponent = () => {
  const todoId = useAppSelector((state) => state.todo.todoId);

  const { data: todo, isLoading } = useQuery(todoDetailQueryOptions(todoId));

  if (!todoId) {
    return <div>请先选择一个 Todo</div>;
  }

  if (isLoading) {
    return <div>加载详情中...</div>;
  }
  return (
    <div>
      <h1>{todo.title}</h1>
      <p>状态: {todo.completed ? "已完成" : "未完成"}</p>
    </div>
  );
};

export default TanstackPrefetchComponent;

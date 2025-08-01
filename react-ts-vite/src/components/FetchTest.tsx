import { useState } from "react";
import useFetch, { type TodoItem } from "../shared/useFetch";
import { useAppDispatch, useAppSelector } from "../shared/storehooks";
import Button from "@mui/material/Button";
import { increment } from "../shared/counterSlice";
import { Outlet } from "react-router-dom";

const FetchTest = () => {
  const [name, setName] = useState("SB");

  const data: TodoItem[] | null = useFetch(
    "https://jsonplaceholder.typicode.com/todos"
  );
  const count = useAppSelector((state) => state.counter.value);
  const dispatch = useAppDispatch();
  return (
    <div>
      <div>{name}</div>
      <div>{data && data[0]?.title}</div>
      <div>Count from Redux store: {count}</div>
      <Button onClick={() => dispatch(increment())}>+</Button>
    </div>
  );
};

export default FetchTest;

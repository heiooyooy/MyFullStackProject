import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import React, { useReducer } from "react";

// 1. 定义初始状态
const initialState = { count: 0 };

// 2. 创建 Reducer 函数
// 它根据不同的 action.type 来决定如何更新状态
function reducer(
  state: { count: number },
  action: { type: string; payload?: number }
) {
  switch (action.type) {
    case "increment":
      return { count: state.count + 1 };
    case "decrement":
      return { count: state.count - 1 };
    case "reset":
      return { count: 0 };
    case "add_amount":
      // 使用 action.payload 传递的附加数据
      return { count: state.count + (action.payload ?? 0) };
    default:
      throw new Error("Unknown action type");
  }
}

const ReducerTest = () => {
  // 3. 在组件中使用 useReducer
  const [state, dispatch] = useReducer(reducer, initialState);

  return (
    <>
      <div>
        <h1>Count: {state.count}</h1>
        {/* 4. 调用 dispatch 来派发不同的 action */}
        <Stack direction="row" spacing={2}>
          <Button onClick={() => dispatch({ type: "increment" })}>+</Button>
          <Button onClick={() => dispatch({ type: "decrement" })}>-</Button>
          <Button onClick={() => dispatch({ type: "reset" })}>Reset</Button>
          <Button onClick={() => dispatch({ type: "add_amount", payload: 5 })}>
            +5
          </Button>
        </Stack>
      </div>
    </>
  );
};

export default ReducerTest;

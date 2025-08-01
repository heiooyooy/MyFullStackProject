import {
  useDispatch,
  type TypedUseSelectorHook,
  useSelector,
} from "react-redux";
import type { store } from "./store";

// 我们先用 typeof store.getState 拿到了 getState 函数的类型签名 () => RootState
// 然后我们把这个函数类型放进 ReturnType 里。
// ReturnType 提取出这个函数的返回值类型，也就是 RootState 本身！
export type RootState = ReturnType<typeof store.getState>;

// 2. 推断出 `AppDispatch` 类型，它包含了所有中间件的类型
export type AppDispatch = typeof store.dispatch;

// 在整个应用中使用这些 hooks，而不是原生的 `useDispatch` 和 `useSelector`
export const useAppDispatch: () => AppDispatch = useDispatch;
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

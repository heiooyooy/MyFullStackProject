import { configureStore } from "@reduxjs/toolkit";
import counterReducer from "./counterSlice";
import todoReducer from "./todoSlice";
import { todoRTKQuery } from "./todoRTKQuery";
import authReducer from "./authSlice";
export const store = configureStore({
  reducer: {
    counter: counterReducer,
    todo: todoReducer,
    auth: authReducer,
    [todoRTKQuery.reducerPath]: todoRTKQuery.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(todoRTKQuery.middleware),
});

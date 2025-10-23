import { configureStore } from "@reduxjs/toolkit";
import counterReducer from "./counterSlice";
import todoReducer from "./todoSlice";
import { authApi, todoRTKQuery } from "./todoRTKQuery";
import authReducer from "./authSlice";
export const store = configureStore({
  reducer: {
    counter: counterReducer,
    todo: todoReducer,
    auth: authReducer,
    [todoRTKQuery.reducerPath]: todoRTKQuery.reducer,
    [authApi.reducerPath]: authApi.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .concat(todoRTKQuery.middleware)
      .concat(authApi.middleware),
});

import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import type { LoginCredentials } from "./authSlice";
import { axiosBaseQuery } from "./apiClient";

export const todoRTKQuery = createApi({
  reducerPath: "todoApi",
  baseQuery: fetchBaseQuery({
    baseUrl: "https://jsonplaceholder.typicode.com",
  }),
  endpoints: (builder) => ({
    getTodos: builder.query({
      query: () => "todos",
    }),
    getTodoById: builder.query({
      query: (id) => `todos/${id}`,
    }),
  }),
});

export const {
  useGetTodosQuery,
  useGetTodoByIdQuery,
  useLazyGetTodosQuery,
  useLazyGetTodoByIdQuery,
} = todoRTKQuery;

export const authApi = createApi({
  reducerPath: "authApi",

  // Use your custom axios-based baseQuery
  baseQuery: axiosBaseQuery(), // We call the function to return the base query

  endpoints: (builder) => ({
    registerUser: builder.mutation<string, LoginCredentials>({
      // The `query` now returns an object that matches
      // the arguments for our axiosBaseQuery
      query: (credentials) => ({
        url: "/auth/register", // This will be sent to /api/auth/register
        method: "POST",
        data: credentials, // 'data' is used by axios, not 'body'
      }),
      transformResponse: (response: { message: string }) => response.message,
    }),

    // // Example of a GET request
    // getMe: builder.query<User, void>({
    //   query: () => ({
    //     url: "/auth/me",
    //     method: "GET",
    //   }),
    // }),
  }),
});

export const { useRegisterUserMutation } = authApi;

import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import apiClient from "./apiClient";
import { loginApi, registerUserApi } from "./apiCalls";
import { useQueryClient } from "@tanstack/react-query";

const userToken = localStorage.getItem("token");

export const LOGIN_STATUS = {
  Idle: "idle",
  Succeeded: "succeeded",
  Loading: "loading",
  Failed: "failed",
} as const;

export type LoginStatus = (typeof LOGIN_STATUS)[keyof typeof LOGIN_STATUS];

export interface AuthState {
  user: any;
  token: string | null;
  status: LoginStatus;
  error: any;
}

const initialState: AuthState = {
  user: null,
  token: userToken ? userToken : null,
  status: LOGIN_STATUS.Idle,
  error: null,
};

export interface LoginCredentials {
  email: string;
  password: string;
}

export const loginUser = createAsyncThunk(
  "auth/login",
  async (credentials: LoginCredentials, { rejectWithValue }) => {
    const { token } = await loginApi(credentials);
    localStorage.setItem("authToken", token);
    return token;
  }
);

export const registerUser = createAsyncThunk(
  "auth/register",
  async (credentials: LoginCredentials, { rejectWithValue }) => {
    const { message } = await registerUserApi(credentials);
    return message;
  }
);

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(loginUser.pending, (state) => {
        state.status = LOGIN_STATUS.Loading;
        state.error = null;
      })
      .addCase(loginUser.fulfilled, (state, action) => {
        state.status = LOGIN_STATUS.Succeeded;
        state.token = action.payload;
      })
      .addCase(loginUser.rejected, (state, action) => {
        state.status = LOGIN_STATUS.Failed;
        state.error = action.payload;
      })
      .addCase(registerUser.pending, (state) => {
        state.status = LOGIN_STATUS.Loading;
        state.error = null;
      })
      .addCase(registerUser.fulfilled, (state, action) => {
        state.status = LOGIN_STATUS.Succeeded;
        state.token = action.payload;
      })
      .addCase(registerUser.rejected, (state, action) => {
        state.status = LOGIN_STATUS.Failed;
        state.error = action.payload;
      });
  },
});

export default authSlice.reducer;

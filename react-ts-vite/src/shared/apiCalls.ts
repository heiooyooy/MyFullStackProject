import apiClient from "./apiClient";
import type { LoginCredentials } from "./authSlice";

// login 函数现在非常干净，只关心自己的业务
export const loginApi = async (
  credentials: LoginCredentials
): Promise<{ token: string }> => {
  try {
    // 使用 apiClient 发起 POST 请求
    // axios 会自动将返回的 data 包装在 response.data 中
    const response = await apiClient.post<{ token: string }>(
      "/auth/login",
      credentials
    );
    // 我们直接返回 API 响应体中的数据部分
    return response.data;
  } catch (error) {
    // axios 会在请求失败时抛出错误，我们可以在这里处理或再次抛出
    console.error("登录 API 调用失败:", error);
    throw error;
  }
};

export const registerUserApi = async (
  credentials: LoginCredentials
): Promise<{ message: string }> => {
  try {
    const response = await apiClient.post<{ message: string }>(
      "/auth/register",
      credentials
    );
    return response.data;
  } catch (error) {
    console.error("登录 API 调用失败:", error);
    throw error;
  }
};

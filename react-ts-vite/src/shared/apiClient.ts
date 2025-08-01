// src/lib/apiClient.ts

import axios from "axios";

// 1. 创建一个 axios 实例
const apiClient = axios.create({
  // 设置基础 URL，这样你在调用时就不用写完整的域名了
  baseURL: "/api",
  // 设置默认的请求头
  headers: {
    "Content-Type": "application/json",
  },
});

// 2. 添加一个请求拦截器 (Request Interceptor)
//    这是 axios 的魔法所在！
apiClient.interceptors.request.use(
  (config) => {
    // 在每个请求被发送之前，执行这里的代码
    const token = localStorage.getItem("authToken"); // 从 localStorage 获取 token
    if (token) {
      // 如果 token 存在，就把它添加到 Authorization 请求头中
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    // 对请求错误做些什么
    return Promise.reject(error);
  }
);

// 你也可以添加响应拦截器来处理全局错误，比如 401 跳转到登录页
apiClient.interceptors.response.use(
  (response) => response, // 对成功的响应直接返回
  (error) => {
    if (error.response?.status === 401) {
      // 处理未授权错误，比如清除本地 token 并跳转到登录页
      console.error("未授权或 Token 过期，请重新登录");
      // window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;

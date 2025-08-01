import Button from "@mui/material/Button";
import Input from "@mui/material/Input";
import { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { LOGIN_STATUS, loginUser } from "../shared/authSlice";
import { useAppDispatch, useAppSelector } from "../shared/storehooks";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import apiClient from "../shared/apiClient";
import { Github } from "lucide-react";
import { AiFillAlipayCircle } from "react-icons/ai";
import { unwrapResult } from "@reduxjs/toolkit";
import TextField from "@mui/material/TextField";

export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
  description: string;
}

const getWeather = async (): Promise<WeatherForecast[]> => {
  const response = await apiClient.get("/WeatherForecast");
  console.log(response);
  return response.data;
};

const Login = () => {
  const dispatch = useAppDispatch();
  const queryClient = useQueryClient();

  // 从 store 中获取认证状态
  const {
    status,
    error: authError,
    user,
  } = useAppSelector((state) => state.auth);

  const { data } = useQuery({
    queryKey: ["weather"],
    queryFn: getWeather,
  });

  console.log(data);

  // 使用本地 state 管理表单输入
  const [email, setEmail] = useState("admin@default.com");
  const [password, setPassword] = useState("Password1");

  const handleSubmit = async (event: any) => {
    event.preventDefault();
    if (email && password) {
      // 派发 loginUser thunk，并传入凭证
      const resultAction = await dispatch(loginUser({ email, password }));
      const token = unwrapResult(resultAction);
      console.log(`result token is ${token}`);
      queryClient.invalidateQueries();
    }
  };

  // 登录成功后可以执行跳转等操作
  useEffect(() => {
    if (user) {
      // 例如，可以重定向到用户面板
      console.log("登录成功，跳转到用户面板...");
      // navigate('/dashboard'); // 如果使用 react-router-dom
    }
  }, [user]);

  return (
    <div>
      <h2>
        登录
        {status === LOGIN_STATUS.Succeeded && (
          <span>
            <AiFillAlipayCircle size={"40px"} color="#1976d2" />
          </span>
        )}
      </h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="email">邮箱:</label>
          <TextField
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="password">密码:</label>

          <TextField
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>

        <Button type="submit" disabled={status === LOGIN_STATUS.Loading}>
          {status === LOGIN_STATUS.Loading ? "登录中..." : "登录"}
        </Button>
      </form>
      {/* 如果登录失败，显示从 store 中获取的错误信息 */}
      {status === LOGIN_STATUS.Failed && (
        <p style={{ color: "red" }}>{authError}</p>
      )}
    </div>
  );
};

export default Login;

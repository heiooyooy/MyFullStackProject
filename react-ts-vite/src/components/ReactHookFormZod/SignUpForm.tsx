import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { signUpSchema, type SignUpFormValues } from "./validators";
import { zodResolver } from "@hookform/resolvers/zod";
import { unwrapResult } from "@reduxjs/toolkit";
import type { email } from "zod";
import { loginUser, registerUser } from "../../shared/authSlice";
import { useAppDispatch } from "../../shared/storehooks";
import { useQueryClient } from "@tanstack/react-query";

const SignUpForm = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting, isDirty },
    setError,
    clearErrors,
    watch,
  } = useForm<SignUpFormValues>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      username: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const emailField = watch("email");
  const usernameField = watch("username");
  const passwordField = watch("password");
  const confirmPasswordField = watch("confirmPassword");

  const dispatch = useAppDispatch();
  const queryClient = useQueryClient();

  useEffect(() => {
    // 当任何字段发生变化时，清除 root 错误
    if (errors.root) {
      clearErrors("root");
    }
  }, [passwordField, emailField, usernameField, confirmPasswordField]);

  const onSubmit = async (data: SignUpFormValues) => {
    if (data.email && data.password) {
      // 派发 loginUser thunk，并传入凭证
      const resultAction = await dispatch(
        registerUser({ email: data.email, password: data.password })
      );
      const token = unwrapResult(resultAction);
      console.log(`result token is ${token}`);
    }

    // console.log("Submitting sign up form", data);
    // await new Promise<void>((res) =>
    //   setTimeout(() => {
    //     res();
    //   }, 1000)
    // );
    // console.log("Submit successfully");
    // setError("root", { message: "Email is already taken" });
  };

  return (
    <div className="">
      <h1 className="text-2xl font-bold mb-6 text-center">创建账户</h1>

      {/* 5. 使用 handleSubmit 包裹你的 onSubmit 函数 */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label
            htmlFor="username"
            className="block text-sm font-medium text-gray-700"
          >
            用户名
          </label>
          {/* 6. 使用 register 将 input 和 schema 中的字段关联起来 */}
          <input
            id="username"
            type="text"
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            {...register("username")}
          />
          {/* 7. 显示该字段的验证错误信息 */}
          {errors.username && (
            <p className="mt-1 text-sm text-red-600">
              {errors.username.message}
            </p>
          )}
        </div>

        <div>
          <label htmlFor="email">邮箱</label>
          <input
            id="email"
            type="email"
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            {...register("email")}
          />
          {errors.email && (
            <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>
          )}
        </div>

        <div>
          <label htmlFor="password">密码</label>
          <input
            id="password"
            type="password"
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            {...register("password")}
          />
          {errors.password && (
            <p className="mt-1 text-sm text-red-600">
              {errors.password.message}
            </p>
          )}
        </div>

        <div>
          <label htmlFor="confirmPassword">确认密码</label>
          <input
            id="confirmPassword"
            type="password"
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            {...register("confirmPassword")}
          />
          {errors.confirmPassword && (
            <p className="mt-1 text-sm text-red-600">
              {errors.confirmPassword.message}
            </p>
          )}
        </div>

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:bg-indigo-300"
        >
          {isSubmitting ? "注册中..." : "注册"}
        </button>

        {errors.root && (
          <p className="mt-1 text-sm text-red-600">{errors.root.message}</p>
        )}
      </form>
    </div>
  );
};

export default SignUpForm;

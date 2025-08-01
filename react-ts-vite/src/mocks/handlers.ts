// src/mocks/handlers.ts
import { http, HttpResponse } from "msw";

export const handlers = [
  // 拦截 GET /api/todos 请求
  http.get("/api/todos", () => {
    return HttpResponse.json([{ id: 1, title: "学习 MSW", completed: true }]);
  }),
  // 拦截 POST /api/login 请求
  http.post("/api/test", async ({ request }) => {
    const info: any = await request.json();
    if (info.username === "admin") {
      return HttpResponse.json({ token: "mock-token-123" });
    }
    // 模拟登录失败
    return new HttpResponse(null, { status: 401 });
  }),
];

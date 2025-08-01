using System.Text.Json;
using SharedModule;

namespace BackendServer.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 讓請求繼續在 pipeline 中往下走
            await _next(context);
        }
        catch (Exception ex)
        {
            // 捕捉到任何未處理的異常
            _logger.LogError(ex, "一個未處理的異常發生了: {Message}", ex.Message);

            // 準備返回給客戶端的錯誤信息
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new ApiErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "伺服器發生內部錯誤，請稍後再試。"
            };

            // 只有在開發環境中才暴露詳細的堆疊追蹤信息
            if (_env.IsDevelopment())
            {
                response.Details = ex.StackTrace?.ToString();
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
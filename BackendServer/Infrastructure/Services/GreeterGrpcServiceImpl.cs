using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class GreeterGrpcServiceImpl : Greeter.GreeterBase
{
    [Authorize]
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        // 2. Access User Claims via GetHttpContext()
        // context.GetHttpContext() bridges gRPC context to ASP.NET Core HttpContext
        var user = context.GetHttpContext().User;

        var userName = user.Identity?.Name ?? "Unknown";
        var userId = user.FindFirst("sub")?.Value;

        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name}, authorized as {userName}"
        });
    }
}

using MediatR;

namespace BackendServer.Models.MediatR;

// --- Command ---
// 定义一个命令，它需要产品名称，并期望返回新创建的产品对象
public record CreateProductCommand(string Name, decimal Price) : IRequest<ProductDto>;

// --- DTO ---
public record ProductDto(int Id, string Name, decimal Price);

// --- Handler ---
// 为这个命令创建一个处理器
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    // 实际项目中这里会注入数据库上下文等
    public Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 模拟创建产品并存入数据库
        Console.WriteLine($"正在创建产品: {request.Name}");
        var product = new ProductDto(new Random().Next(), request.Name, request.Price);

        // 返回新创建的产品
        return Task.FromResult(product);
    }
}

public record TestCommandWithoutResponse(string name) : IRequest;

public class TestCommandWithoutResponseHandler : IRequestHandler<TestCommandWithoutResponse>
{
    public Task Handle(TestCommandWithoutResponse request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"The name from the request is {request.name}");
        return Task.CompletedTask;
    }
}
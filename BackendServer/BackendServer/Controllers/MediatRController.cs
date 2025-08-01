using BackendServer.Models.MediatR;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediatRController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediatRController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        // 1. 发送一个命令，并等待其处理结果
        var createdProduct = await _mediator.Send(command);

        TestCommandWithoutResponse tr = new TestCommandWithoutResponse("Dejun Tu");
        await _mediator.Send(tr);

        // 2. 业务完成后，发布一个通知
        //    这将触发所有订阅了此通知的 Handler
        await _mediator.Publish(new ProductCreatedNotification(createdProduct));

        return Ok(createdProduct);
    }
}
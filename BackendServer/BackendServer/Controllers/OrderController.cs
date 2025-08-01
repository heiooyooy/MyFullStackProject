using BackendServer.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedModule;
using SharedModule.Interfaces;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

    public OrderController(ILogger<OrderController> logger, MyDbContext dbContext, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        try
        {
            await _orderService.CreateOrderAsync(order);
            // 返回 201 Created，并提供一个可以获取该资源的URL
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting order {OrderId}", order.OrderId);
            return StatusCode(500, new { message = "Error submitting order" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderById(id);
        if (order != null)
            return Ok(order);

        return NotFound();
    }
}
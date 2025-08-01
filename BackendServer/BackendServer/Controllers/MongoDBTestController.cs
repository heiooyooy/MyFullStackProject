using BackendServer.Services;
using Microsoft.AspNetCore.Mvc;
using SharedModule.MongoDBExp;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MongoDBTestController : ControllerBase
{
    private readonly MongoDBTestService _mongoDbTestService;

    public MongoDBTestController(MongoDBTestService mongoDbTestService)
    {
        _mongoDbTestService = mongoDbTestService;
    }

    [HttpGet]
    public async Task<IList<MongoDBOrder>> Test()
    {
        return await _mongoDbTestService.GetOrders();
    }
    
    [HttpGet("getallorders")]
    public async Task<ActionResult<IList<MongoDBOrder>>> GetAllOrdersAsync()
    {
        return await _mongoDbTestService.GetAllOrdersAsync();
    }

    [HttpGet("getUserById")]
    public async Task<ActionResult<MongoDBUser>> GetUserByIdAsync(string userId)
    {
        return await _mongoDbTestService.GetUserByIdAsync(userId);
    }
}
using Elastic.Clients.Elasticsearch;
using Infrastructure.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SqlPlaygroundController : ControllerBase
{
    private readonly LearnSqlContext _learnSqlContext;

    public SqlPlaygroundController(LearnSqlContext learnSqlContext)
    {
        _learnSqlContext = learnSqlContext;
    }

    [HttpGet("getAllProducts")]
    public async Task<ActionResult<IList<Product>>> GetAllProducts()
    {
        var result = await _learnSqlContext.Products.ToListAsync();
        return result;
    }

    [HttpGet("test")]
    public async Task<ActionResult<object>> Test(Guid userId, int productId)
    {
        var anonymousResult = await _learnSqlContext.Products
            .OrderByDescending(p => p.Price)
            .Take(3)
            .Select(p => new NamePriceDto(p.Name, p.Price)) // EF Core 将此高效翻译为 SQL
            .ToListAsync();
        //
        // // 在内存中快速转换为元组列表
        // return anonymousResult;


        var test = await _learnSqlContext.Reviews.Select(r => new
        {
            UserName = r.User.Username,
            ProductName = r.Product.Name,
            Comment = r.Comment,
            Rating = r.Rating
        }).ToListAsync();


        var test2 = await _learnSqlContext.Products.Select(p => new
        {
            ProductName = p.Name,
            NumberOfReview = p.Reviews.Count
        }).ToListAsync();

        var test3 = await _learnSqlContext.Products.Select(p => new
        {
            ProductName = p.Name,
            AverageRating = p.Reviews.Average(r => (decimal?)r.Rating) ?? 0
        }).ToListAsync();

        var test5 = await _learnSqlContext.Reviews.GroupBy(r => r.Product).Where(g => g.Average(r => r.Rating) > 4)
            .Select(g => new
            {
                ProductName = g.Key.Name,
                AverageRating = g.Average(r => r.Rating)
            }).ToListAsync();

        var test4 = await _learnSqlContext.Products.Where(p => p.Tags.Contains("SQL")).Select(p => new
        {
            ProductName = p.Name,
            Tags = p.Tags
        }).ToListAsync();


        var result = await _learnSqlContext.Orders.Select(o => new OrderDto()
        {
            Id = o.Id,
            Username = o.User.Username,
            CreatedAt = o.CreatedAt,
            TotalAmount = o.TotalAmount,
            Items = o.OrderItems.Select(oi => new OrderItemDto()
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name, // 取得產品名稱
                Quantity = oi.Quantity,
                PricePerUnit = oi.PricePerUnit
            }).ToList()
        }).ToListAsync();


        var maxOrder = _learnSqlContext.Orders.OrderByDescending(o => o.TotalAmount).FirstOrDefault();

        var userWithOrder = await _learnSqlContext.Orders.Select(o => o.User.Username).Distinct().ToListAsync();
        var usersWithOrders =
            await _learnSqlContext.Users.Where(u => u.Orders.Any()).Select(u => u.Username).ToListAsync();
        var userWithOrderdate = await _learnSqlContext.Users.Select(u => new
        {
            Username = u.Username,
            LastOrderDate = u.Orders.OrderByDescending(o => o.CreatedAt).Select(o => (DateTime?)o.CreatedAt)
                .FirstOrDefault()
        }).ToListAsync();

        var totalPerProduct = await _learnSqlContext.OrderItems.Where(oi => oi.Order.Status == OrderStatus.PAID)
            .GroupBy(oi => oi.Product.Type)
            .Select(g => new
            {
                Type = g.Key,
                TotalRevenue = g.Sum(oi => oi.Quantity * oi.PricePerUnit)
            })
            .ToListAsync();

        var result2 = totalPerProduct.Select(x => new
        {
            Type = x.Type.ToString(), // 转为枚举名
            x.TotalRevenue
        });

        // 1. 先执行分组
        var groupedResult = await _learnSqlContext.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.PAID)
            .GroupBy(oi => oi.Product.Type)
            .ToListAsync(); // 物化成分组列表

        // groupedResult 现在是一个 List<IGrouping<ProductType, OrderItem>>

        // 2. 遍历每一个分组（每一个“篮子”）
        foreach (var group in groupedResult)
        {
            Console.WriteLine($"--- 产品类型: {group.Key} ---"); // 打印篮子的标签
            Console.WriteLine($"该类型下共有 {group.Count()} 个售出的订单项。");

            // 3. 遍历该分组内的每一个原始元素（篮子里的每一个“水果”）
            //    因为 group 本身就是一个 IEnumerable<OrderItem>
            foreach (var orderItem in group)
            {
                // 在这里，你可以访问到每一个原始的 OrderItem 对象
                Console.WriteLine(
                    $"  - 产品ID: {orderItem.ProductId}, 数量: {orderItem.Quantity}, 单价: {orderItem.PricePerUnit}");
            }

            Console.WriteLine(); // 换行
        }

        return Ok(result2);
    }


    public record NamePriceDto(string Name, decimal Price);

    public class OrderSummaryDto
    {
        public Guid OrderId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
    
    public OrderStatus Status { get; set; } // Add this property
}



public class OrderDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } // 包含子項的 DTO 列表
}
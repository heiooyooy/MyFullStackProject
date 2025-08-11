using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public Guid OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal PricePerUnit { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } 
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    
}

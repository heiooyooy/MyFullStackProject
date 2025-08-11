using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Infrastructure.DataAccess.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public List<string>? Tags { get; set; }

    public string? Attributes { get; set; }

    public DateTime CreatedAt { get; set; }

    public NpgsqlTsVector? SearchVector { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    
    public ProductType Type { get; set; } 
}

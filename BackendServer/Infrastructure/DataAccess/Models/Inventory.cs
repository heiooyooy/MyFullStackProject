using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string LicenseKey { get; set; } = null!;

    public bool IsAssigned { get; set; }

    public int? AssignedToOrderItemId { get; set; }

    public virtual OrderItem? AssignedToOrderItem { get; set; }

    public virtual Product Product { get; set; } = null!;
}

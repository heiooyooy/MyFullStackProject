using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Models;

public partial class Review
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

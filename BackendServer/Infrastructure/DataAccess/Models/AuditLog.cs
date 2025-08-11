using System;
using System.Collections.Generic;
using System.Net;

namespace Infrastructure.DataAccess.Models;

public partial class AuditLog
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? Details { get; set; }

    public IPAddress? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }
}

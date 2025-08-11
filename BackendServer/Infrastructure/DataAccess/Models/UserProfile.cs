using System;
using System.Collections.Generic;

namespace Infrastructure.DataAccess.Models;

public partial class UserProfile
{
    public Guid UserId { get; set; }

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Preferences { get; set; }

    public virtual User User { get; set; } = null!;
}

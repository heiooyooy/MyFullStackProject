using Microsoft.AspNetCore.Identity;

namespace SharedModule;

public class ApplicationUser : IdentityUser
{
    public string? CustomProperty { get; set; }
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
        
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}

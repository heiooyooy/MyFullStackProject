using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedModule;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (result.Succeeded)
        {
            // --- Generate and return the JWT ---
            var token = GenerateJwtToken(user); // We'll create this method next
            return Ok(new { Token = token });
        }

        return Unauthorized("Invalid credentials");
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginDto model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.Email
        };

        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            return BadRequest("Email already exists");
        }

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "User registered successfully" });
    }
    
    // Add this helper method to your controller
    private string GenerateJwtToken(ApplicationUser user)
    {
        // This is a simplified example. The key should be stored securely in appsettings.json!
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6mWi2glc0/bOEZGLEJdQoEQqQioEusMlj/GXKceLCuc="));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
            // You can add roles here: new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: "my-api", // Replace with your issuer
            audience: "my-clients", // Replace with your audience
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet("login-cookie")]
    public async Task<IActionResult> Login(string username, string password)
    {
        // --- 在真实应用中，这里应该验证用户名和密码 ---
        if (username == "admin" && password == "123456")
        {
            // 1. 创建用户的声明 (Claims)
            // Claims 是描述用户身份的信息，比如用户名、角色等
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("LastLoginTime", DateTime.UtcNow.ToString())
            };

            // 2. 创建身份标识 (ClaimsIdentity)
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. 创建身份主体 (ClaimsPrincipal)
            var authProperties = new AuthenticationProperties
            {
                // IsPersistent = true: 创建一个持久化 Cookie (浏览器关闭后依然存在)
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            // 4. 执行登录，这会创建加密的认证 Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok($"Welcome, {username}! You are logged in.");
        }

        return Unauthorized("Invalid username or password.");
    }
}
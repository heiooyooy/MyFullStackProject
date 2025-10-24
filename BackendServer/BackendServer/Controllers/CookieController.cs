using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CookieController : ControllerBase
{
    [HttpGet("set-session")]
    public IActionResult SetSessionCookie()
    {
        // This cookie is lost when the browser closes
        Response.Cookies.Append("MySessionCookie", "Hello from a session!");
        return Ok("Session cookie set.");
    }

    [HttpGet("set-persistent")]
    public IActionResult SetPersistentCookie()
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        };

        Response.Cookies.Append("MyPersistentCookie", "This is a secure, 7-day cookie.", cookieOptions);
        return Ok("Persistent cookie set.");
    }

    [HttpGet("read")]
    public IActionResult ReadCookies()
    {
        string sessionCookie = Request.Cookies["MySessionCookie"];
        string persistentCookie = Request.Cookies["MyPersistentCookie"];

        return Ok(new 
        { 
            SessionValue = sessionCookie, 
            PersistentValue = persistentCookie 
        });
    }

    [HttpGet("delete")]
    public IActionResult DeleteCookies()
    {
        Response.Cookies.Delete("MySessionCookie");
        Response.Cookies.Delete("MyPersistentCookie");
        return Ok("Cookies deleted.");
    }
}
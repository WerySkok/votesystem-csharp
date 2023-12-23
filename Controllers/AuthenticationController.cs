using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public class AuthenticationController : BaseController
{
    public AuthenticationController(ILogger<AuthenticationController> logger, ApplicationContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }

    [HttpGet("~/login")]
    public IActionResult Login() => Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Discord");

    [HttpPost("~/logout")]
    public IActionResult LogOutCurrentUser()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
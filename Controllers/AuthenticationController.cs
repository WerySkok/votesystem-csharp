using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(ILogger<AuthenticationController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    public class LoginRegisterForm
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRegisterForm form)
    {
        string login = form.Login;
        string password = form.Password;
        User? user = await Models.User.GetUserByLoginPassword(HttpContext, login, password);
        if (user is null) return Unauthorized();

        string role = "user";
        if (user.IsEligible) role = "voter";
        if (user.IsAdmin) role = "admin";
        var claims = new List<Claim> { new(ClaimTypes.Name, user.Id.ToString()), new(ClaimTypes.Role, role) };
        ClaimsIdentity claimsIdentity = new(claims, "Cookies");

        return SignIn(new ClaimsPrincipal(claimsIdentity));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRegisterForm form)
    {
        string login = form.Login;
        string password = form.Password;
        User? user = await Models.User.Register(HttpContext, login, password);
        if (user is null) return Unauthorized();

        string role = "user";
        if (user.IsEligible) role = "voter";
        if (user.IsAdmin) role = "admin";
        var claims = new List<Claim> { new(ClaimTypes.Name, user.Id.ToString()), new(ClaimTypes.Role, role) };
        ClaimsIdentity claimsIdentity = new(claims, "Cookies");

        return SignIn(new ClaimsPrincipal(claimsIdentity));
    }


    [HttpPost("logout")]
    public IActionResult LogOutCurrentUser()
    {
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
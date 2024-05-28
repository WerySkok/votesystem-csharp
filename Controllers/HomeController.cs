using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public class HomeController(ILogger<HomeController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    [Route("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.CurrentElection = await _db.Elections.FirstOrDefaultAsync(e => e.StartTime < DateTime.Now && DateTime.Now < e.EndTime);
        ViewBag.User = await Models.User.GetUser(HttpContext);
        string[] allowedRoles = _configuration.GetSection("discord:eligible_roles_ids").Get<string[]>()!;
        ViewBag.IsUserElligible = ViewBag.User?.HasRoles(_db, allowedRoles) ?? false;
        return View();
    }
}

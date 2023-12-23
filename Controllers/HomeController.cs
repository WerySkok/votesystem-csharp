using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationContext _db;
    private readonly IConfiguration _configuration;
    public HomeController(ILogger<HomeController> logger, ApplicationContext context, IConfiguration configuration)
    {
        _logger = logger;
        _db = context;
        _configuration = configuration;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.CurrentElection = await _db.Elections.FirstOrDefaultAsync(e => e.StartTime < DateTime.Now && DateTime.Now < e.EndTime);
        ViewBag.User = await Models.User.GetUser(HttpContext);
        string[] allowedRoles = _configuration.GetSection("discord:eligible_roles_ids").Get<string[]>()!;
        ViewBag.IsUserElligible = ViewBag.User?.HasRoles(_db, allowedRoles) ?? false;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

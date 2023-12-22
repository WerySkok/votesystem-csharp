using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApplicationContext db;
    public HomeController(ILogger<HomeController> logger, ApplicationContext context)
    {
        _logger = logger;
        db = context;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

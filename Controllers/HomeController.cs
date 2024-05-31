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
        string[] allowedRoles = _configuration.GetSection("discord:eligible_roles_ids").Get<string[]>()!;

        Election? election = await _db.Elections.FirstOrDefaultAsync(e => e.StartTime < DateTime.Now && DateTime.Now < e.EndTime);
        User? user = await Models.User.GetUserByContext(HttpContext);

        return Json(new IndexView
        {
            Election = election,
            User = user,
            IsUserElligible = user?.IsEligible ?? false
        });
    }
    public class IndexView
    {
        public Election? Election { get; set; }
        public User? User { get; set; }
        public bool IsUserElligible { get; set; }
    }
}

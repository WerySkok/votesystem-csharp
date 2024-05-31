using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;
[Route("api/utils")]
public class ApiController(ILogger<ApiController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    [HttpGet("user_data")]
    public async Task<IActionResult> UserData()
    {
        return Json((User?)HttpContext.Items["user"]);
    }

    [HttpGet("current_election")]
    public async Task<IActionResult> CurrentElection()
    {
        return Json(await _db.Elections.FirstOrDefaultAsync(e => e.StartTime < DateTime.Now && DateTime.Now < e.EndTime));
    }
}

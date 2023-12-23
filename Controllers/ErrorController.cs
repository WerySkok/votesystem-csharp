using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public class ErrorController : BaseController
{
    public ErrorController(ILogger<ErrorController> logger, ApplicationContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }

    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

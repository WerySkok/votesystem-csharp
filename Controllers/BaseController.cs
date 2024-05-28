using Microsoft.AspNetCore.Mvc;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public abstract class BaseController(ILogger<BaseController> logger, ApplicationContext context, IConfiguration configuration) : Controller
{
    protected readonly ILogger<BaseController> _logger = logger;
    protected readonly ApplicationContext _db = context;
    protected readonly IConfiguration _configuration = configuration;
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ILogger<BaseController> _logger;
    protected readonly ApplicationContext _db;
    protected readonly IConfiguration _configuration;
    public BaseController(ILogger<BaseController> logger, ApplicationContext context, IConfiguration configuration)
    {
        _logger = logger;
        _db = context;
        _configuration = configuration;
    }
}

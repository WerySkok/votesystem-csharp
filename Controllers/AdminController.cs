using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace votesystem_csharp.Controllers;
[Route("admin")]
[Authorize(Policy = "Admins")]
public class AdminController : Controller
{
    public string Index() => "You're admin!";
}
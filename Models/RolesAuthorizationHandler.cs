using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
namespace votesystem_csharp.Models;

public class RolesAuthorizationHandler(ApplicationContext db, IHttpContextAccessor ctx) : AuthorizationHandler<RolesRequirement>
{
    readonly ApplicationContext _db = db;
    readonly IHttpContextAccessor _context = ctx;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
    {
        var user = await User.GetUser(_context.HttpContext!);
        if (user == null)
        {
            context.Fail();
            return;
        }

        var hasAccess = user.HasRoles(_db, requirement.NeededRoles);
        if (hasAccess)
        {
            context.Succeed(requirement);
            return;
        }
        context.Fail();
    }
}
using Microsoft.AspNetCore.Authorization;
namespace votesystem_csharp.Models;

public class RolesRequirement : IAuthorizationRequirement
{
    public string[] NeededRoles { get; set; }

    public RolesRequirement(params string[] neededRoles)
    {
        NeededRoles = neededRoles;
    }
}
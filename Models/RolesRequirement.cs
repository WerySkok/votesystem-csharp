using Microsoft.AspNetCore.Authorization;
namespace votesystem_csharp.Models;

public class RolesRequirement(params string[] neededRoles) : IAuthorizationRequirement
{
    public string[] NeededRoles { get; set; } = neededRoles;
}
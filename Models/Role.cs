namespace votesystem_csharp.Models;

public class Role
{
    public required Guid UserId { get; set; }
    public required string RoleDiscordId { get; set; }

    public required User User { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace votesystem_csharp.Models;

public class User
{
    [Key]
    public required Guid Id { get; set; }
    public required string DiscordUserId { get; set; }
    public required string DisplayName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public List<Role> Roles { get; set; } = new();
    public List<Vote> Votes { get; set; } = new();
}
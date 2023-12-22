using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;

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

    public static async Task OnLogin(OAuthCreatingTicketContext ctx, string serverId)
    {
        ApplicationContext db = ctx.HttpContext.RequestServices.GetService<ApplicationContext>()!;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

        var userResponse = await httpClient.GetAsync("https://discord.com/api/users/@me");
        if (!userResponse.IsSuccessStatusCode) return;

        var userJson = JsonDocument.Parse(await userResponse.Content.ReadAsStringAsync()).RootElement;
        string discordId = userJson.GetProperty("id").GetString()!;
        string discordUserName = userJson.GetProperty("username").GetString()!;
        string? discordGlobalName = userJson.GetProperty("global_name").GetString();

        string displayName = discordGlobalName ?? discordUserName;

        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.DiscordUserId == discordId);
        if (existingUser == null)
        {
            existingUser = new User
            {
                Id = Guid.NewGuid(),
                DiscordUserId = discordId,
                DisplayName = displayName,
                IsAdmin = false
            };
            // Если пользователь не найден, создать новую запись в базе данных
            db.Users.Add(existingUser);
        }
        else
        {
            // Если пользователь найден, обновить его данные
            existingUser.DisplayName = displayName;
            var Roles = await db.Roles.Where(r => r.UserId == existingUser.Id).ToListAsync();
            foreach (var role in Roles)
            {
                db.Roles.Remove(role);
            }

        }

        var rolesResponse = await httpClient.GetAsync($"https://discord.com/api/v10/users/@me/guilds/{serverId}/member");
        var rolesJson = JsonDocument.Parse(await rolesResponse.Content.ReadAsStringAsync()).RootElement;

        foreach (var jsonElement in rolesJson.GetProperty("roles").EnumerateArray())
        {
            db.Roles.Add(new Role { RoleDiscordId = jsonElement.GetString(), User = existingUser, UserId = existingUser.Id });
        }

        await db.SaveChangesAsync();
    }
    public async static Task<User?> GetUser(HttpContext HttpContext)
    {
        var dbContext = HttpContext.RequestServices.GetService<ApplicationContext>()!;
        if (!(HttpContext.User?.Identity?.IsAuthenticated ?? false)) return null;
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.DiscordUserId == userIdClaim);
        }
        return null;
    }
}
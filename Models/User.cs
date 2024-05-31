using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;

namespace votesystem_csharp.Models;

public class User
{
    [Key]
    public required Guid Id { get; set; }
    public required string Login { get; set; }
    [JsonIgnore]
    public string? PasswordHash { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsEligible { get; set; } = false;
    [JsonIgnore]
    public List<Vote> Votes { get; set; } = [];

    static string HashPassword(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }


    public static async Task<User> Register(HttpContext HttpContext, string login, string password)
    {
        var dbContext = HttpContext.RequestServices.GetService<ApplicationContext>()!;
        string passwordHash = HashPassword(password);

        User user = new() { Id = new Guid(), Login = login, PasswordHash = passwordHash };
        if (!await dbContext.Users.AnyAsync())
        {
            user.IsAdmin = true; // Первый пользователь делается админом
            user.IsEligible = true;
        }

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public static async Task<User?> GetUserByLoginPassword(HttpContext HttpContext, string login, string password)
    {
        var dbContext = HttpContext.RequestServices.GetService<ApplicationContext>()!;
        string passwordHash = HashPassword(password);
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == passwordHash);
    }

    public static async Task<User?> GetUserByContext(HttpContext HttpContext)
    {
        var dbContext = HttpContext.RequestServices.GetService<ApplicationContext>()!;
        if (!(HttpContext.User?.Identity?.IsAuthenticated ?? false)) return null;
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
        if (userIdClaim != null)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userIdClaim));
        }
        return null;

    }

}
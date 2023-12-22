using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Configuration.AddJsonFile("config.json");
        string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));

        builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                })
                .AddDiscord(options =>
                {
                    options.ClientId = builder.Configuration["discord:client_id"]!;
                    options.ClientSecret = builder.Configuration["discord:client_secret"]!;

                    options.Scope.Add("guilds.members.read");
                    options.Events.OnCreatingTicket = async ctx => await User.OnLogin(ctx, builder.Configuration["discord:server_id"]!, builder.Configuration["discord:admin_role"]!);
                }
    );

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admins", policy => policy.AddRequirements(new RolesRequirement(builder.Configuration["discord:admin_role"]!)));
            options.AddPolicy("Users", policy => policy.AddRequirements(new RolesRequirement(builder.Configuration.GetSection("discord:eligible_roles_ids").Get<string[]>()!)));
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
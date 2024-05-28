using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
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
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                    options.Scope.Add("guilds.members.read");
                    options.Events.OnCreatingTicket = async ctx => await User.OnLogin(ctx, builder.Configuration["discord:server_id"]!, builder.Configuration["discord:admin_role"]!);
                }
    );

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Admins", policy => policy.AddRequirements(new RolesRequirement(builder.Configuration["discord:admin_role"]!)))
            .AddPolicy("Users", policy => policy.AddRequirements(new RolesRequirement(builder.Configuration.GetSection("discord:eligible_roles_ids").Get<string[]>()!)));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();


        var app = builder.Build();

        app.UseStatusCodePages();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
            app.UseForwardedHeaders();
        }


        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
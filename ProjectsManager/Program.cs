using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectsManager.Models;
using System;
using System.Configuration;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;

var builder = WebApplication.CreateBuilder(args);

string appDataPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);
AppDomain.CurrentDomain.SetData("SaveDir", builder.Environment.ContentRootPath);


builder.Services.AddDbContext<ProjectsManagerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectsManagerContext"));
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Add ToastNotification
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter;
});

// Подключаю куки
builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/Users/Login");

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNotyf();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.MapGet("/Users/SignIn/{id:int}", async (string? returnUrl, HttpContext context, int id, ProjectsManagerContext _context) =>
{

    var user = _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    if (user.Result != null && user.Result.Role != null)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Result.Login), new Claim(ClaimTypes.Role, user.Result.Role), new Claim("ID", user.Result.Id.ToString()) };
        // создаем объект ClaimsIdentity
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        // установка аутентификационных куки
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return Results.Redirect($"~/Users/Details/{user.Result.Id}");
    }
    return Results.Unauthorized();
});

app.MapGet("/Users/SignOut", async (HttpContext context) =>
{
    // Выход из системы и удаление куки
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Перенаправление на страницу входа или другую страницу
    return Results.Redirect("~/Users/Login");
});

app.Run();

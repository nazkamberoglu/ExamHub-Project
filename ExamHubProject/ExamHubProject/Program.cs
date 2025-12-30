using ExamHubProject.Models.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ProjectContext>(x =>
    x.UseSqlServer("Server=NAZ01\\SQLExpress;Database=ExamHubDB;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(x =>
    {
        x.LoginPath = "/Security/SignIn";
        x.LogoutPath = "/Security/LogOut";
        x.AccessDeniedPath = "/Security/AccessDenied";
        x.Cookie.HttpOnly = true;
        x.Cookie.SameSite = SameSiteMode.Lax;
        x.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        x.ExpireTimeSpan = TimeSpan.FromHours(1);
        x.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
    

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Security}/{action=SignIn}/{id?}");

app.Run();

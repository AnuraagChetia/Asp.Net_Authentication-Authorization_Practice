using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Practice_Application.Data;


var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();

var AuthCookieName = Environment.GetEnvironmentVariable("AuthCookieName");

builder.Services.AddAuthentication().AddCookie(AuthCookieName!, options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.LogoutPath = "/Authentication/Logout";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.Cookie.Name = AuthCookieName;
    });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
        {
            policy.RequireClaim("Role", "Admin");
        });
    });

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

builder.Services.AddDbContext<dbContext>(options =>
    options.UseSqlServer(connectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using First_MVC_Project.Models;

namespace First_MVC_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Add Authentication Schema for the app
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "MyAppCookie";
                options.LoginPath = "/Authentication/Validate";
                options.LogoutPath = "/Authentication/Logout";
                options.AccessDeniedPath = "/";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

            //builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllersWithViews();

            //Use connections string from appsettings.json
            //AppDbContext can be available for Dependecy Injection
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ToDoAppDb")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios.
                app.UseHsts();
            }

            // This middleware is used to redirects HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // This middleware is used to returns static files and short-circuits further request processing.
            app.UseStaticFiles();

            // This middleware is used to route requests.
            app.UseRouting();

            //This middleware is used to authenticate a user
            app.UseAuthentication();

            // This middleware is used to authorizes a user to access secure resources.
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

// Flow of Middleware components example... (Use, Run, Map)
//app.Use(async (context, next) => {
//    await context.Response.WriteAsync("Middleware1: Request\n");
//    await next();
//    await context.Response.WriteAsync("Middleware1: Response\n");
//});

//app.Use(async (context, next) => {
//    await context.Response.WriteAsync("Middleware2: Request\n");
//    await next();
//    await context.Response.WriteAsync("Middleware2: Response\n");
//});

//app.Run(async context => {
//    await context.Response.WriteAsync("Middleware3: Terminal Request\n");
//});
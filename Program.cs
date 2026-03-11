using Filharmonia.Data;
using Filharmonia.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Aplikacja startuje");

try
{
    var builder = WebApplication.CreateBuilder(args);

   
    builder.Logging.ClearProviders(); 
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); 
    builder.Host.UseNLog(); 

    builder.Services.AddRazorPages();

    
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPolicy", policy =>
            policy.RequireRole("Administrator"));
    });

   
    builder.Services.AddControllersWithViews();

    builder.Services.AddScoped<IEventService, EventService>();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login"; 
        options.LogoutPath = "/Identity/Account/Logout"; 
    });

    var app = builder.Build();

   
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Błąd w aplikacji.");
                throw;
            }
        });
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapRazorPages();

    app.Use(async (context, next) =>
    {
        logger.Info($"Żądanie URL: {context.Request.Path}");
        await next.Invoke();
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin123!";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Administrator");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        logger.Error($"Error: {error.Description}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Błąd podczas inicjalizacji ról i użytkowników.");
        }
    }

    app.MapGet("/test-login", () =>
    {
        logger.Info("Test logowania - endpoint działa poprawnie.");
        return "Login route test";
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Program zakończył działanie z błędem.");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

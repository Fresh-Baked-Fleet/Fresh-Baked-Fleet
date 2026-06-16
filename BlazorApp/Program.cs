using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Models;
using BlazorApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

LoadLocalEnv();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext
var connectionString = GetPostgresConnectionString(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
        ?? throw new InvalidOperationException("Google Client ID is missing.");

        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
        ?? throw new InvalidOperationException("Google Client Secret is missing.");
    })
    .AddIdentityCookies();

builder.Services.AddAuthorizationBuilder();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserState>();

var app = builder.Build();

// seed the database and apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        db.Database.Migrate();

        await SeedData.InitializeAsync(db, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "CRITICAL ERROR DURING DATABASE SEEDING");
        Console.WriteLine($"Database initialization failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/account/login", async (SignInManager<User> signInManager, UserManager<User> userManager, [FromForm] string emailAddress, [FromForm] string password) =>
{
    try
    { 
        var user = await userManager.FindByEmailAsync(emailAddress);
        if (user == null)
        {
            Console.WriteLine("3. User Status: NOT FOUND in database.");
        }
        else
        {
            Console.WriteLine($"3. User Status: FOUND (Id: {user.Id}).");
            
            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
            Console.WriteLine($"4. Password Check: {isPasswordCorrect}");
            Console.WriteLine($"5. Email Confirmed: {user.EmailConfirmed}");
        }

        var result = await signInManager.PasswordSignInAsync(emailAddress, password, isPersistent: false, lockoutOnFailure: false);
        Console.WriteLine($"6. Final SignIn Result: {result}\n");
        
        if (result.Succeeded)
        {
            return Results.Redirect("/");
        }
        
        return Results.Redirect("/account/login?error=true"); 
    }
    catch (Exception)
    {
        return Results.Redirect("/account/login?error=database_offline");
    }
    
});

app.MapPost("/account/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/account/login");
});

app.MapPost("/account/external-login", (SignInManager<User> signInManager, [FromForm] string provider, [FromForm] string returnUrl = "/") =>
{
    var redirectUrl = $"/account/external-callback?returnUrl={Uri.EscapeDataString(returnUrl)}";
    var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

    return Results.Challenge(properties, new[] { provider });
});

app.MapGet("/account/external-callback", async (SignInManager<User> signInManager, UserManager<User> userManager, string returnUrl = "/") =>
{
    try
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return Results.Redirect("/account/login?error=true");

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return Results.Redirect(returnUrl);
        } 

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

        if (email != null)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                await userManager.AddLoginAsync(existingUser, info);
                await signInManager.SignInAsync(existingUser, isPersistent: false);
                return Results.Redirect(returnUrl);
            }
            else
            {
                var newUser = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(newUser);
                if (createResult.Succeeded)
                {
                    await userManager.AddLoginAsync(newUser, info);
                    await signInManager.SignInAsync(newUser, isPersistent: false);
                    return Results.Redirect(returnUrl);
                }
            }
        }

        return Results.Redirect("/account/login?error=true");
    }
    catch (Exception)
    {
        return Results.Redirect("/account/login?error=database_offline");
    }
});

app.Run();

static string GetPostgresConnectionString(IConfiguration configuration)
{
    var databaseUrl = configuration["DATABASE_URL"];
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        if (!databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return databaseUrl;
        }

        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);
        var isLocalHost = uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
            uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
            uri.Host.Equals("::1", StringComparison.OrdinalIgnoreCase);

        return new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = isLocalHost ? Npgsql.SslMode.Disable : Npgsql.SslMode.Require
        }.ConnectionString;
    }

    return configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' or DATABASE_URL is missing.");
}

static void LoadLocalEnv()
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var envPaths = new[]
    {
        Path.Combine(currentDirectory, ".env"),
        Path.Combine(currentDirectory, "BlazorApp", ".env")
    };

    foreach (var envPath in envPaths)
    {
        if (File.Exists(envPath))
        {
            DotNetEnv.Env.Load(envPath);
            return;
        }
    }
}

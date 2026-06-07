using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=freshbakedfleet.db"));

builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = true;
})
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

var app = builder.Build();

// seed the database and apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.Migrate();

        SeedData.Initialize(db);
    }
    catch (Exception ex)
    {
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
    Console.WriteLine("\n--- LOGIN DIAGNOSTICS ---");
    Console.WriteLine($"1. Email Received from Form: '{emailAddress}'");
    Console.WriteLine($"2. Password Received from Form: '{password}'");
    
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
});

app.Run();

using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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

// app.MapPost("/account/login", async (SignInManager<User> signInManager, UserManager<User> userManager, [FromForm] string emailAddress, [FromForm] string password) =>
// {
//     Console.WriteLine("\n--- LOGIN DIAGNOSTICS ---");
//     Console.WriteLine($"1. Email Received from Form: '{emailAddress}'");
//     Console.WriteLine($"2. Password Received from Form: '{password}'");
    
//     var user = await userManager.FindByEmailAsync(emailAddress);
//     if (user == null)
//     {
//         Console.WriteLine("3. User Status: NOT FOUND in database.");
//     }
//     else
//     {
//         Console.WriteLine($"3. User Status: FOUND (Id: {user.Id}).");
        
//         var isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
//         Console.WriteLine($"4. Password Check: {isPasswordCorrect}");
//         Console.WriteLine($"5. Email Confirmed: {user.EmailConfirmed}");
//     }

//     var result = await signInManager.PasswordSignInAsync(emailAddress, password, isPersistent: false, lockoutOnFailure: false);
//     Console.WriteLine($"6. Final SignIn Result: {result}\n");
    
//     if (result.Succeeded)
//     {
//         return Results.Redirect("/");
//     }
    
//     return Results.Redirect("/login?error=true"); 
// });
app.Run();

using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task InitializeAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {

        string[] roleNames = { "Admin", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var poweruser = await userManager.FindByEmailAsync("murilo.luiz.hernandes@gmail.com");
        if (poweruser != null)
        {
            if (!await userManager.IsInRoleAsync(poweruser, "Admin"))
            {
                await userManager.AddToRoleAsync(poweruser, "Admin");
            }
        }

        if (!context.Products.Any())
        {
            context.SaveChanges();

            await context.SaveChangesAsync();
        }
    }
}
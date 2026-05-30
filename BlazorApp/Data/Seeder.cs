using BlazorApp.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Products.Any())
            return;

        context.Products.AddRange(ProductSeed.Products);
        context.SaveChanges();
    }
}
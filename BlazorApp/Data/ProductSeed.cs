using BlazorApp.Models;

namespace BlazorApp.Data;

public static class ProductSeed
{
    public static List<Product> Products => new()
    {
        new Product
        {
            Name = "All-Purpose Flour",
            Price = 25.99m,
            Weight = 50m,
            Available = true,
            Description = "High-quality all-purpose flour for commercial baking.",
            ImagePath = "/images/products/flour.webp"
        },

        new Product
        {
            Name = "Granulated Sugar",
            Price = 32.50m,
            Weight = 50m,
            Available = true,
            Description = "Refined granulated sugar for baking applications.",
            ImagePath = "/images/products/sugar.webp"
        },

        new Product
        {
            Name = "Active Dry Yeast",
            Price = 18.75m,
            Weight = 1m,
            Available = true,
            Description = "Commercial-grade active dry yeast.",
            ImagePath = "/images/products/yeast.webp"
        },
        new Product
        {
            Name = "Cocoa Powder",
            Price = 42.99m,
            Weight = 25m,
            Available = false,
            Description = "Premium cocoa powder for commercial baking.",
            ImagePath = "/images/products/cocoa-powder.webp"
        },
    };
}
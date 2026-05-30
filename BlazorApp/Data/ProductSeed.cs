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
            ImagePath = "/images/products/flour.jpg"
        },

        new Product
        {
            Name = "Granulated Sugar",
            Price = 32.50m,
            Weight = 50m,
            Available = true,
            Description = "Refined granulated sugar for baking applications.",
            ImagePath = "/images/products/sugar.jpg"
        },

        new Product
        {
            Name = "Active Dry Yeast",
            Price = 18.75m,
            Weight = 1m,
            Available = true,
            Description = "Commercial-grade active dry yeast.",
            ImagePath = "/images/products/yeast.jpg"
        }
    };
}
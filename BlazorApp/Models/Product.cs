namespace BlazorApp.Models;
// This class represents a product in the application.
// It includes properties for the product's ID, name, price, weight,
// availability, description, and image path.

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public bool Available { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}
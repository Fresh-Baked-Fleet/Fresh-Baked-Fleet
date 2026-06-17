namespace BlazorApp.Models;
using System.ComponentModel.DataAnnotations;

// This class represents a product in the application.
// It includes properties for the product's ID, name, price, weight,
// availability, description, and image path.

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please give the name of the product.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a price.")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than zero.")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Please enter a weight.")]
    [Range(0.01, 10000, ErrorMessage = "Weight must be greater than zero.")]
    public decimal? Weight { get; set; }

    [Required(ErrorMessage = "Please set an availability status.")]
    public bool Available { get; set; }

    [Required(ErrorMessage = "Please give a brief description of the product.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please provide an image of the product.")]
    public string ImagePath { get; set; } = string.Empty;
}
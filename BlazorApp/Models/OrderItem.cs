namespace BlazorApp.Models;
// This class represents an item in an order.
// It includes properties for the order item ID,
// the associated order, product, and quantity.

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}
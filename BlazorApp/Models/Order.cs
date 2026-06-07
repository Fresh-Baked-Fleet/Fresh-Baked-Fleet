namespace BlazorApp.Models;
// This class represents a customer's order request.
// It includes properties for the order ID, user ID, creation date,
// status, and a collection of order items associated with the order.

public enum OrderStatus
{
    Pending,
    Approved,
    Rejected,
    Fulfilled
}

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
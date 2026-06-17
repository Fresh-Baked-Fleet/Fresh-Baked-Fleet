using BlazorApp.Models;

namespace BlazorApp.Services;

public class OrderCartService
{
    private readonly List<OrderItem> _items = [];

    public event Action? OnChange;

    public IReadOnlyList<OrderItem> Items => _items;

    public int ItemCount => _items.Sum(i => i.Quantity);

    public void AddItem(Product product, int quantity = 1)
    {
        if (quantity < 1)
            return;

        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity
            });
        }

        OnChange?.Invoke();
    }

    public void RemoveItem(OrderItem item)
    {
        _items.Remove(item);
        OnChange?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        OnChange?.Invoke();
    }
}

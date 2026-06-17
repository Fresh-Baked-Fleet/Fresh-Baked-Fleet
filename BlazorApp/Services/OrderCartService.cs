using BlazorApp.Models;
using Microsoft.JSInterop;

namespace BlazorApp.Services;

public class OrderCartService
{
    private readonly ProductService _productService;
    private readonly IJSRuntime _js;
    private readonly List<OrderItem> _items = [];
    private bool _isLoaded;

    public OrderCartService(ProductService productService, IJSRuntime js)
    {
        _productService = productService;
        _js = js;
    }

    public event Action? OnChange;

    public IReadOnlyList<OrderItem> Items => _items;

    public int ItemCount => _items.Sum(i => i.Quantity);

    public async Task EnsureLoadedAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            var lines = await _js.InvokeAsync<CartLine[]>("orderCart.get");
            if (lines is { Length: > 0 })
            {
                var products = await _productService.GetProductsAsync();
                foreach (var line in lines)
                {
                    var product = products.FirstOrDefault(p => p.Id == line.ProductId);
                    if (product == null)
                        continue;

                    var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
                    if (existing != null)
                        existing.Quantity += line.Quantity;
                    else
                    {
                        _items.Add(new OrderItem
                        {
                            ProductId = product.Id,
                            Product = product,
                            Quantity = line.Quantity
                        });
                    }
                }
            }
        }
        catch (JSException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        _isLoaded = true;
    }

    public async Task AddItemAsync(Product product, int quantity = 1)
    {
        if (quantity < 1)
            return;

        await EnsureLoadedAsync();

        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
            existing.Quantity += quantity;
        else
        {
            _items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity
            });
        }

        await PersistAsync();
        OnChange?.Invoke();
    }

    public async Task RemoveItemAsync(OrderItem item)
    {
        _items.Remove(item);
        await PersistAsync();
        OnChange?.Invoke();
    }

    public async Task ClearAsync()
    {
        _items.Clear();

        try
        {
            await _js.InvokeVoidAsync("orderCart.clear");
        }
        catch (JSException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        OnChange?.Invoke();
    }

    private async Task PersistAsync()
    {
        var lines = _items
            .Select(i => new CartLine { ProductId = i.ProductId, Quantity = i.Quantity })
            .ToArray();

        try
        {
            await _js.InvokeVoidAsync("orderCart.set", (object)lines);
        }
        catch (JSException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }
}

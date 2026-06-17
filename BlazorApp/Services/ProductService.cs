using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services;

public class ProductService
{
    private readonly AppDbContext _context;
    public ProductService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<List<Product>> GetRandomProductsAsync(int count)
    {
        var products = await _context.Products.ToListAsync();
        if (count <= 0 || products.Count == 0)
        {
            return new List<Product>();
        }

        return products
            .OrderBy(_ => Random.Shared.Next())
            .Take(Math.Min(count, products.Count))
            .ToList();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }
}
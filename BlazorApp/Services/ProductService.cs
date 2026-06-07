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
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }
}
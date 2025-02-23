using Microsoft.EntityFrameworkCore;
using Products.Models;

namespace Products.ProductManager;

public class ProductsManager(ProductContext context) : IProductsManager
{
    public DbSet<Product> GetAll()
    {
        return context.Products;
    }

    public ValueTask<Product?> GetById(long productId)
    {
        return context.Products.FindAsync(productId);
    }

    public Task<int> SaveChanges()
    {
        return context.SaveChangesAsync();
    }

    public async Task Add(Product product)
    {
        context.Products.Add(product);
        await SaveChanges();
    }

    public async Task Remove(Product product)
    {
        context.Products.Remove(product);
        await SaveChanges();
    }
}
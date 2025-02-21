using Microsoft.EntityFrameworkCore;
using Products.Models;

namespace Products.ProductManager;

public class ProductsManager(ProductContext context) : IProductsManager
{
    public DbSet<Product> GetProducts()
    {
        return context.Products;
    }

    public ValueTask<Product?> GetProductById(long productId)
    {
        return context.Products.FindAsync(productId);
    }

    public Task<int> SaveChanges()
    {
        return context.SaveChangesAsync();
    }

    public async Task AddProduct(Product product)
    {
        context.Products.Add(product);
        await SaveChanges();
    }

    public async Task RemoveProduct(Product product)
    {
        context.Products.Remove(product);
        await SaveChanges();
    }
}
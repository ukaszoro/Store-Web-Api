using Microsoft.EntityFrameworkCore;
using Products.Models;

namespace Products.ProductManager;

public interface IProductsManager
{
    public DbSet<Product> GetProducts();
    public ValueTask<Product?> GetProductById(long productId);
    public Task<int> SaveChanges();
    public Task AddProduct(Product product);
    public Task RemoveProduct(Product product);
}
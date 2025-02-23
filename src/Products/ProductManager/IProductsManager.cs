using Microsoft.EntityFrameworkCore;
using Products.Models;

namespace Products.ProductManager;

public interface IProductsManager
{
    public DbSet<Product> GetAll();
    public ValueTask<Product?> GetById(long productId);
    public Task<int> SaveChanges();
    public Task Add(Product product);
    public Task Remove(Product product);
}
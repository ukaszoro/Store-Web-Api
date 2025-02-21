using Microsoft.EntityFrameworkCore;
using Products.Models;

namespace Products;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {}
    public DbSet<Product> Products { get; set; }
}
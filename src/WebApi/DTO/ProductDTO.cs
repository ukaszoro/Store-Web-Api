using Products.Models;

namespace WebApi.DTO;

public class ProductDTO : IProductBase
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
}
namespace Products.Models;

public class Product : IProductBase
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string? Secret { get; set; }
}
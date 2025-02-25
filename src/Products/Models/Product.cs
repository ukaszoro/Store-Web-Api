namespace Products.Models;

public class Product
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
}
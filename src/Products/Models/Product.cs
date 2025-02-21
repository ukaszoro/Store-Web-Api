namespace Products.Models;

public class Product
{
    public long ID { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string? Secret { get; set; }
}
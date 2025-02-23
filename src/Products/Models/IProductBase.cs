namespace Products.Models;

public interface IProductBase
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
}
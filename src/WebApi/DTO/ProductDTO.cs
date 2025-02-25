namespace WebApi.DTO;

public class ProductDTO
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
}
namespace Negotiations.Models;

public interface INegotiationBase
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public double price { get; set; }
}
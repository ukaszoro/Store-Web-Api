namespace Negotiations.Models;

public interface INegotiationBase
{
    public long ProductId { get; set; }
    public double price { get; set; }
    public NegotiationStatus status { get; set; }
}
using Negotiations.Models;

namespace WebApi.DTO;

public class NegotiationDto : INegotiationBase
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public double price { get; set; }
}
using Negotiations;
using Negotiations.Models;

namespace WebApi.DTO;

public class NegotiationDto : INegotiationBase
{
    public long ProductId { get; set; }
    public double price { get; set; }
    public NegotiationStatus status { get; set; }
}
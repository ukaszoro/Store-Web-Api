using Negotiations;
using Negotiations.Models;

namespace WebApi.DTO;

public class NegotiationDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public double Price { get; set; }
    public NegotiationStatus Status { get; set; }
}
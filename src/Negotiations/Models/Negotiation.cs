namespace Negotiations.Models;

public class Negotiation
{
    public long Id { get; set; }
    public required string UserId { get; set; }
    public long ProductId { get; set; }
    public double Price { get; set; }
    public NegotiationStatus Status { get; set; }
    public int TimesRejected { get; set; }
    public DateTime? RejectedAt { get; set; }
}
namespace Negotiations.Models;

public class Negotiation : INegotiationBase
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public long ProductId { get; set; }
    public double price { get; set; }
    public NegotiationStatus status { get; set; }
    public int TimesRejected { get; set; }
    public DateTime? RejectedAt { get; set; }
}
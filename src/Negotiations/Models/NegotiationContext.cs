using Microsoft.EntityFrameworkCore;

namespace Negotiations.Models;

public class NegotiationContext : DbContext
{
    public NegotiationContext(DbContextOptions<NegotiationContext> options) : base(options)
    {
    }

    public DbSet<Negotiations.Models.Negotiation> Negotiations { get; set; }
}
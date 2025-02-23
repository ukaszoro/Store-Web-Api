using Microsoft.EntityFrameworkCore;
using Negotiations.Models;
namespace Negotiations.NegotiationManager;


public interface INegotiationManager
{
    public DbSet<Negotiation> GetAll();
    public Task<Negotiation?> Find(long id);
    public Task<Negotiation?> Find(string userId, long productId);
    public Task<List<Negotiation>> GetAllWithProductId(long productId);
    public Task<int> SaveChanges();
    public Task Add(Negotiation negotiation);
    public Task Remove(Negotiation negotiation);
}

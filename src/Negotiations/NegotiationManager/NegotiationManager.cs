using Microsoft.EntityFrameworkCore;
using Negotiations.Models;

namespace Negotiations.NegotiationManager;

public class NegotiationManager(NegotiationContext context) : INegotiationManager
{
    public DbSet<Negotiation> GetAll()
    {
        return context.Negotiations;
    }

    public Task<Negotiation?> Find(string userId, long productId)
    {
        return GetAll().SingleOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
    }
    
    public Task<Negotiation?> Find(long id)
    {
        return GetAll().SingleOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Negotiation>> GetAllWithProductId(long productId)
    {
        return GetAll().Where(negotiation => negotiation.ProductId == productId).ToListAsync();
    }

    public Task<int> SaveChanges()
    {
        return context.SaveChangesAsync();
    }

    public async Task Add(Negotiation negotiation)
    {
        context.Negotiations.Add(negotiation);
        await SaveChanges();
    }

    public async Task Remove(Negotiation negotiation)
    {
        context.Negotiations.Remove(negotiation);
        await SaveChanges();
    }
}
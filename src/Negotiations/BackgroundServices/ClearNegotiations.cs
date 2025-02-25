using Microsoft.EntityFrameworkCore;
using Negotiations.NegotiationManager;

namespace Negotiations.BackgroundServices;

public static class ClearNegotiations
{
    public static async Task Clear(INegotiationManager negotiationManager, TimeProvider timeProvider)
    {
        var cutoff = timeProvider.GetUtcNow().AddDays(-7).DateTime;
        
        var negotiationsToBeRemoved = negotiationManager.GetAll()
            .Where(x => (x.RejectedAt != null && DateTime.Compare(x.RejectedAt.Value, cutoff) < 0))
            .AsAsyncEnumerable();
        
        await foreach (var negotiation in negotiationsToBeRemoved)
        {
            await negotiationManager.Remove(negotiation);
        }
    }
}
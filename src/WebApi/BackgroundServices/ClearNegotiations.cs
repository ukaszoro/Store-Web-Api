using Microsoft.EntityFrameworkCore;
using Negotiations.NegotiationManager;

namespace WebApi.BackgroundServices;

public static class ClearNegotiations
{
    private static readonly DateTime Cutoff = DateTime.UtcNow.AddDays(-7);

    public static async Task Clear(INegotiationManager negotiationManager)
    {
        var negotiationsToBeRemoved = negotiationManager.GetAll()
            .Where(x => x.RejectedAt != null && DateTime.Compare((DateTime)x.RejectedAt, Cutoff) < 0)
            .AsAsyncEnumerable();

        await foreach (var negotiation in negotiationsToBeRemoved)
        {
            await negotiationManager.Remove(negotiation);
        }
    }
}
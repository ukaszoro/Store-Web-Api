using Microsoft.EntityFrameworkCore;
using Moq;
using Negotiations.Models;
using Negotiations.NegotiationManager;
using NUnit.Framework;
using Negotiations.BackgroundServices;

namespace tests.NegotiationsTests;

[TestFixture]
public class ClearNegotiationsTests
{
    [Test]
    public async Task ClearTest()
    {
        DbContextOptionsBuilder<NegotiationContext> optionsBuilder = new DbContextOptionsBuilder<NegotiationContext>();
        optionsBuilder.UseInMemoryDatabase("NegotiationList");
        var negotiationContext = new NegotiationContext(optionsBuilder.Options);
        var negotiationManager = new NegotiationManager(negotiationContext);
        
        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.Setup(t => t.GetUtcNow()).Returns(new DateTime(2010, 3, 11));
        var timeProvider = timeProviderMock.Object;

        var negotiation = new Negotiation
        {
            Id = 1,
            Price = 10,
            ProductId = 1,
            UserId = "user1",
            RejectedAt = timeProvider.GetUtcNow().DateTime,
        };
        
        await negotiationManager.Add(negotiation);
        await ClearNegotiations.Clear(negotiationManager, timeProvider);

        Assert.IsNotEmpty(negotiationManager.GetAll());

        timeProviderMock.Setup(t => t.GetUtcNow()).Returns(new DateTime(2010, 3, 20));
        timeProvider = timeProviderMock.Object;
        
        await ClearNegotiations.Clear(negotiationManager, timeProvider);

        Assert.IsEmpty(negotiationManager.GetAll());
    }
}
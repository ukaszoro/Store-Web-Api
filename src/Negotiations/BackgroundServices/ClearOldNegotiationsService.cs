using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Negotiations.NegotiationManager;

namespace Negotiations.BackgroundServices;

public class ClearOldNegotiationsService(
    IServiceProvider serviceProvider, TimeProvider timeProvider) : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(60));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        var negotiationManager = scope.ServiceProvider
            .GetRequiredService<INegotiationManager>();
        
        while (!stoppingToken.IsCancellationRequested &&
               await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await ClearNegotiations.Clear(negotiationManager, timeProvider);
        }
    }
}
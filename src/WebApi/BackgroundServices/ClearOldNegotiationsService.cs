using Negotiations.NegotiationManager;

namespace WebApi.BackgroundServices;

public class ClearOldNegotiationsService(
    IServiceProvider serviceProvider) : BackgroundService
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
            await ClearNegotiations.Clear(negotiationManager);
        }
    }
}
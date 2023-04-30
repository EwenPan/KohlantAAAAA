using System.Globalization;
using System.Security.Cryptography;
using Charts.Controllers;
using Charts.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Charts.Services;

public class ChartValueGenerator : BackgroundService
{
    private static IHubContext<ChartHub> _hub;
    private readonly Buffer<Point> _data;

    public ChartValueGenerator(IHubContext<ChartHub> hub, Buffer<Point> data)
    {
        _hub = hub;
        _data = data;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            var guScore = ExternalParticipationController.score.GurvanScore;
            var naScore = ExternalParticipationController.score.NathanScore;
            ExternalParticipationController.score.GurvanScore = 0;
            ExternalParticipationController.score.NathanScore = 0;

            await _hub.Clients.All.SendAsync(
                "addChartData",
                new Point("Gurvan", guScore),
                cancellationToken: stoppingToken
            );

            await _hub.Clients.All.SendAsync(
                "addChartData",
                new Point("Nathan", naScore),
                cancellationToken: stoppingToken
            );

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
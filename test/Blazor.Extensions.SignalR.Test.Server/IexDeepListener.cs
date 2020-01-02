using System;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions.SignalR.Test.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

public class IexDeepListener : BackgroundService
{
    private IHubContext<DeepViewerHub, IMyHub> deepViewerHub;

    public IexDeepListener(IHubContext<DeepViewerHub, IMyHub> deepViewerHub) : base()
    {
        this.deepViewerHub = deepViewerHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.deepViewerHub.Clients.All.Send("Message Received");
            await Task.Delay(1000);
        }
    }
}
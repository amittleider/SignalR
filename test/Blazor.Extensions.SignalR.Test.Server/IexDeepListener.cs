using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions.SignalR.Test.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using VSLee.IEXSharp;
using VSLee.IEXSharp.Model.Stock.Request;

public class IexDeepListener : BackgroundService
{
    private IHubContext<DeepViewerHub, IMyHub> deepViewerHub;
    private IEXCloudClient sandBoxClient;

    public IexDeepListener(IHubContext<DeepViewerHub, IMyHub> deepViewerHub) : base()
    {
        this.deepViewerHub = deepViewerHub;
        this.sandBoxClient = new IEXCloudClient(publishableToken: "Tpk_0bc6fda638b84be5a2991fc334ce516c", secretToken: "Tsk_2761d3806c9c4bd6aa1ee70fc981a430", signRequest: false, useSandBox: true);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var sseClient = this.sandBoxClient.SSE.SubscribeStockQuoteUSSSE(new List<string>() { "msft" }, UTP: false, interval: StockQuoteSSEInterval.Firehose);
        var sseClient = sandBoxClient.SSE.SubscribeCryptoQuoteSSE(new List<string>() { "btcusdt" });
        sseClient.Error += (sseClient, e) =>
        {
            this.deepViewerHub.Clients.All.Send($"Error {e.Exception}");
        };

        sseClient.MessageReceived += (e) =>
        {
            foreach (var m in e)
            {
                this.deepViewerHub.Clients.All.Send($"{m}");
            }
        };

        await sseClient.StartAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            await this.deepViewerHub.Clients.All.Send("Not Stopping");
            sseClient.Close();
            await Task.Delay(1000);
        }
    }
}
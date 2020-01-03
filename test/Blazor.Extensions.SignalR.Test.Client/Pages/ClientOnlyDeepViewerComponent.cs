using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VSLee.IEXSharp;
using VSLee.IEXSharp.Helper;
using VSLee.IEXSharp.Model.Shared.Response;

namespace Blazor.Extensions.SignalR.Test.Client.Pages
{
    public class ClientOnlyDeepViewerComponent : ComponentBase
    {
        [Inject] private HttpClient _http { get; set; }
        [Inject] private HubConnectionBuilder _hubConnectionBuilder { get; set; }
        internal List<string> Messages { get; set; } = new List<string>();
        internal string SecretKey;
        internal string PublishableKey;
        internal string StockSymbol;
        private HubConnection connection;
        private IEXCloudClient sandBoxClient;
        private SSEClient<QuoteCrypto> sseClient;

        // private void OnNewQuote(List<QuoteCrypto> quote)
        // {
        //     foreach (var m in quote)
        //     {
        //         this.Messages.Add("This message is never seen");
        //         this.StateHasChanged();
        //     }
        // }

        private void OnNewMessage(string message)
        {
            this.Messages.Add(message);
            this.StateHasChanged();
        }

        private async Task ComponentMessageReceived()
        {
            this.Messages.Add("This message doesn't appear");
            await this.InvokeAsync(StateHasChanged);
        }

        internal async Task Subscribe()
        {
            await Task.Run(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        await Task.Delay(500);
                        this.OnNewMessage($"message {i}");
                    }
                }
            );

            new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    // this.sandBoxClient = new IEXCloudClient(publishableToken: "Tpk_0bc6fda638b84be5a2991fc334ce516c", secretToken: "Tsk_2761d3806c9c4bd6aa1ee70fc981a430", signRequest: false, useSandBox: true);
                    // this.sseClient = sandBoxClient.SSE.SubscribeCryptoQuoteSSE(new List<string>() { "btcusdt" });

                    // this.OnNewMessage("Starting");
                    // sseClient.StartAsync();
                    // this.OnNewMessage("Started");
                }
            ).Start();

            this.OnNewMessage("Done");


            // this.sandBoxClient = new IEXCloudClient(publishableToken: "Tpk_0bc6fda638b84be5a2991fc334ce516c", secretToken: "Tsk_2761d3806c9c4bd6aa1ee70fc981a430", signRequest: false, useSandBox: true);
            // this.sseClient = sandBoxClient.SSE.SubscribeCryptoQuoteSSE(new List<string>() { "btcusdt" });

            // sseClient.MessageReceived += OnNewQuote;
            // sseClient.MessageReceived += async (s) => await Fucky();

            // this.Messages.Add("This message is seen");
            // this.StateHasChanged();
            // Thread.Sleep(1000);
            // this.Messages.Add("Slept");
            // await Task.Run(() =>
            // {
            //     for (int i = 0; i < 4; i++)
            //     {
            //         this.Messages.Add("Fucky");
            //         Thread.Sleep(400);
            //     }
            // });

            // sseClient.StartAsync();
            // this.Messages.Add("This message never seen");
            // Console.WriteLine("Hello3");
            // this.Messages.Add("Hello3");
        }

        private void ComponentMessageReceived2(List<QuoteCrypto> obj)
        {
            this.Messages.Add("This message doesn't appear");
            this.StateHasChanged();
        }
    }
}

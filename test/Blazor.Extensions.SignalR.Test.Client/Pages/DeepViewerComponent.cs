using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Extensions.SignalR.Test.Client.Pages
{
    public class DeepViewerComponent : ComponentBase
    {
        [Inject] private HttpClient _http { get; set; }
        [Inject] private HubConnectionBuilder _hubConnectionBuilder { get; set; }
        internal string ToEverybody { get; set; }
        internal string ToGroup { get; set; }
        internal string GroupName { get; set; }
        internal List<string> Messages { get; set; } = new List<string>();

        private HubConnection connection;

        protected override async Task OnInitializedAsync()
        {
            this.connection = this._hubConnectionBuilder
                .WithUrl("/deepviewerhub",
                opt =>
                {
                    opt.LogLevel = SignalRLogLevel.None;
                    opt.Transport = HttpTransportType.WebSockets;
                    opt.SkipNegotiation = true;
                    opt.AccessTokenProvider = async () =>
                    {
                        var token = await this.GetJwtToken("DemoUser");
                        Console.WriteLine($"Access Token: {token}");
                        return token;
                    };
                })
                //.AddMessagePackProtocol()
                .Build();

            this.connection.On<string>("Send", this.Handle);
            this.connection.OnClose(exc =>
            {
                Console.WriteLine("Connection was closed! " + exc.ToString());
                return Task.CompletedTask;
            });
            await this.connection.StartAsync();
        }

        private async Task<string> GetJwtToken(string userId)
        {
            var httpResponse = await this._http.GetAsync($"generatetoken?user={userId}");
            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsStringAsync();
        }

        private Task Handle(object msg)
        {
            Console.WriteLine(msg);
            if (msg is DemoData[])
            {
                var demoDatas = msg as DemoData[];
                DumpData(demoDatas);
            }
            else if (msg is DemoData)
            {
                DumpData(msg as DemoData);
            }
            else
            {
                this.Messages.Add(msg.ToString());
                if (this.Messages.Count > 10)
                {
                    this.Messages.RemoveAt(0);
                }
            }
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        private void DumpData(params DemoData[] arr)
        {
            foreach (var demoData in arr)
            {
                this.Messages.Add($"demoData.id({demoData.Id}) | demoData.Data({demoData.Data}) | demoData.DateTime({demoData.DateTime}) | demoData.DecimalData({demoData.DecimalData}) | demoData.Bool({demoData.Bool}) | demoData.EnumData({demoData.EnumData})");
            }
        }

        private Task HandleArgs(params object[] args)
        {
            string msg = string.Join(", ", args);

            Console.WriteLine(msg);
            this.Messages.Add(msg);
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        internal async Task Broadcast()
        {
            await this.connection.InvokeAsync("Send", this.ToEverybody);
        }

        internal async Task SendToGroup()
        {
            await this.connection.InvokeAsync("SendToGroup", this.GroupName, this.ToGroup);
        }

        internal async Task JoinGroup()
        {
            await this.connection.InvokeAsync("JoinGroup", this.GroupName);
        }

        internal async Task LeaveGroup()
        {
            await this.connection.InvokeAsync("LeaveGroup", this.GroupName);
        }
    }
}

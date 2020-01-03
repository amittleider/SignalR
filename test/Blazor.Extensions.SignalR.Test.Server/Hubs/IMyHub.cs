using System.Threading.Tasks;

namespace Blazor.Extensions.SignalR.Test.Server.Hubs
{
    public interface IMyHub
    {
        Task Send(string message);
    }
}
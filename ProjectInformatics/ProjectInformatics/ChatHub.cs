using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ProjectInformatics
{
    public class ChatHub : Hub
    {
        public async Task Send(string message,string username)
        {
            await Clients.All.SendAsync("Send", message,username);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ProjectInformatics
{
    public class ChatHub : Hub
    {
        private ApplicationContext db;
        public ChatHub(ApplicationContext context)
        {
            db = context;
        }
        public async Task Send(string message,string username)
        {
            db.Messages.Add(new Entities.Message { UserName = username, MessageText = message });
            await db.SaveChangesAsync();
            await Clients.All.SendAsync("Send", message,username);
        }
    }
}

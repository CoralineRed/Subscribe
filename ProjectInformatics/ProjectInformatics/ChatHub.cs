using Microsoft.AspNetCore.SignalR;
using System.Linq;
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
        public async Task Send(string message,string username,string sendTo)
        {
            db.Messages.Add(new Entities.Message { UserName = username, MessageText = message, SendTo=sendTo });
            await db.SaveChangesAsync();
            //await Clients.All.SendAsync("Send", message,username);
            await Clients.Caller.SendAsync("Send", message, username);
            await Clients.User(db.Users.FirstOrDefault(p => p.Email == sendTo).Id.ToString()).SendAsync("Send", message, username);
        }
    }
}

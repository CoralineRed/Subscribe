using Microsoft.AspNetCore.SignalR;
using ProjectInformatics.Database;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics
{
    public class ChatHub : Hub
    {
        private IDbService db;
        public ChatHub(IDbService context)
        {
            db = context;
        }
        public async Task Send(string message,string username,string sendTo)
        {
            db.AddMessage(new Entities.Message { UserName = username, MessageText = message, SendTo=sendTo });
            //await Clients.All.SendAsync("Send", message,username);
            await Clients.Caller.SendAsync("Send", message, username);
            await Clients.User(db.GetUser(sendTo).Id.ToString()).SendAsync("Send", message, username);
        }
    }
}

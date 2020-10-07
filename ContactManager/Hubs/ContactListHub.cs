using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ContactManager.Hubs
{
    public class ContactListHub : Hub
    {
        public async Task DataUpdate()
        {
            await Clients.All.SendAsync("RefreshContactList");
        }
    }
}

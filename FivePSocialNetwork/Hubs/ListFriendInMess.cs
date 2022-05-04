using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class ListFriendInMess : Hub
    {
        public static void ListFirend()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ListFriendInMess>();
            context.Clients.All.displayChat();
        }
    }
}
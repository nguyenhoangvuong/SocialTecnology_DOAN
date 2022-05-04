using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class StatusFriendInMess : Hub
    {
        public static void StatusFriend()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<StatusFriendInMess>();
            context.Clients.All.displayChat();
        }
    }
}
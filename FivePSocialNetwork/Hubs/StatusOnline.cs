using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class StatusOnline : Hub
    {
        public static void StatusOnlineFriend()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<StatusOnline>();
            context.Clients.All.displayChat();
        }
    }
}
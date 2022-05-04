using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class DemoNotification : Hub
    {
        public static void Notification()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DemoNotification>();
            context.Clients.All.displayChat();
        }
        
    }
}
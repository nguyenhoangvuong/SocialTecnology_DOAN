using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class DemoCountNotification : Hub
    {
        public static void CountNotification()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DemoCountNotification>();
            context.Clients.All.displayChat();
        }
    }
}
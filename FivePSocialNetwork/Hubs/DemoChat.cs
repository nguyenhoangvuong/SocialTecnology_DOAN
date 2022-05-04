using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class DemoChat : Hub
    {
        public static void Message()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DemoChat>();
            context.Clients.All.displayChat();
        }
        
    }
}
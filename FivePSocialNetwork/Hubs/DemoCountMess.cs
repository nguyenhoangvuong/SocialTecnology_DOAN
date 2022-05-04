using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    public class DemoCountMess : Hub
    {
        public static void CountMess()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DemoCountMess>();
            context.Clients.All.displayChat();
        }
    }
}
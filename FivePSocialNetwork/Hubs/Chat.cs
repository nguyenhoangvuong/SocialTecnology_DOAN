using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Hubs
{
    [HubName("chat")]
    public class Chat : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Message(int id, int user_friend_id)
        {
            Clients.All.message(id, user_friend_id);
        }
    }
}
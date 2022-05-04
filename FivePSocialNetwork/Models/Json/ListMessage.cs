using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListMessage
    {
        public int message_id { get; set; }
        public string message_content { get; set; }
        public Nullable<int> messageSender_id { get; set; }
        public Nullable<int> messageRecipients_id { get; set; }
        public string message_dateSend { get; set; }
        public Nullable<bool> message_recycleBin { get; set; }
        public Nullable<bool> message_status { get; set; }
        public string messageSender_avatar { get; set; }
        public string messageSender_firstName { get; set; }
        public string messageSender_lastName { get; set; }
        public string messageSender_status { get; set; }
        public string messageRecipients_status { get; set; }
    }
}
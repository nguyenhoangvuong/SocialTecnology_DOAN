using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListNotification
    {
        public int notification_id { get; set; }
        public string notification_content { get; set; }
        public Nullable<int> receiver_id { get; set; }
        public Nullable<int> impactUser_id { get; set; }
        public Nullable<int> question_id { get; set; }
        public Nullable<bool> notification_status { get; set; }
        public string notification_dateCreate { get; set; }
        public Nullable<bool> notification_recycleBin { get; set; }
        public string impactUser_user_firstName { get; set; }
        public string impactUser_user_lastName { get; set; }
        public string impactUser_avatar { get; set; }
        public string question_title { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListFriend
    {
        public int friend_id { get; set; }
        public Nullable<int> userRequest_id { get; set; }
        public Nullable<int> userResponse_id { get; set; }
        public Nullable<bool> friend_status { get; set; }
        public string friend_dateRequest { get; set; }
        public string friend_dateResponse { get; set; }
        public string friend_dateUnfriend { get; set; }
        public Nullable<bool> friend_recycleBin { get; set; }
        public string nameuserRequest { get; set; }
        public string nameuserResponse { get; set; }
        public string avatauserResponse { get; set; }
        public string avatauserRequest { get; set; }

        public string user_firstName { get; set; }
        public string user_avatar { get; set; }
        public string user_lastName { get; set; }
        public Nullable<int> user_popular { get; set; }
        public Nullable<int> user_goldMedal { get; set; }
        public Nullable<int> user_silverMedal { get; set; }
        public Nullable<int> user_brozeMedal { get; set; }
        public Nullable<int> user_vipMedal { get; set; }
    }
}
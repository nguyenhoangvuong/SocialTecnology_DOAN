using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListPosts
    {
        public int post_id { get; set; }
        public string post_content { get; set; }
        public string post_dateCreate { get; set; }
        public string post_dateEdit { get; set; }
        public Nullable<int> user_id { get; set; }
        public string user_firstName { get; set; }
        public string user_lastName { get; set; }
        public Nullable<int> user_popular { get; set; }
        public Nullable<int> user_goldMedal { get; set; }
        public Nullable<int> user_silverMedal { get; set; }
        public Nullable<int> user_brozeMedal { get; set; }
        public Nullable<int> user_vipMedal { get; set; }
        public Nullable<bool> post_activate { get; set; }
        public string post_title { get; set; }
        public Nullable<int> post_Answer { get; set; }
        public Nullable<int> post_totalComment { get; set; }
        public Nullable<int> post_view { get; set; }
        public Nullable<int> post_totalRate { get; set; }
        public Nullable<int> post_totalLike { get; set; }
        public Nullable<bool> post_recycleBin { get; set; }
        public Nullable<bool> post_userStatus { get; set; }
        public Nullable<int> post_popular { get; set; }
        public Nullable<bool> post_admin_recycleBin { get; set; }
        public string post_keywordSearch { get; set; }
        public string technology_name { get; set; }
        public string user_avatar { get; set; }

    }
}
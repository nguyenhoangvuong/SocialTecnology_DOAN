using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListAnswer
    {
        public int answer_id { get; set; }
        public string answer_content { get; set; }
        public string answer_dateCreate { get; set; }
        public string answer_dateEdit { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<bool> answer_activate { get; set; }
        public Nullable<bool> answer_correct { get; set; }
        public Nullable<bool> answer_userStatus { get; set; }
        public Nullable<int> question_id { get; set; }
        public Nullable<int> answer_totalRate { get; set; }
        public Nullable<int> answer_medalCalculate { get; set; }
        public Nullable<bool> answer_recycleBin { get; set; }
        public Nullable<bool> answer_admin_recycleBin { get; set; }
        public string user_firstName { get; set; }
        public string user_lastName { get; set; }
        public string user_avatar { get; set; }
        public Nullable<int> user_popular { get; set; }
        public Nullable<int> user_goldMedal { get; set; }
        public Nullable<int> user_silverMedal { get; set; }
        public Nullable<int> user_brozeMedal { get; set; }
        public Nullable<int> user_vipMedal { get; set; }

    }
}
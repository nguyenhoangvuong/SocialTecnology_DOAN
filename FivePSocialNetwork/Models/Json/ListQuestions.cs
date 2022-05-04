using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListQuestions
    {
        public int question_id { get; set; }
        public string question_content { get; set; }
        public string question_dateCreate { get; set; }
        public string question_dateEdit { get; set; }
        public Nullable<int> user_id { get; set; }
        public string user_firstName { get; set; }
        public string user_lastName { get; set; }
        public Nullable<int> user_popular { get; set; }
        public Nullable<int> user_goldMedal { get; set; }
        public Nullable<int> user_silverMedal { get; set; }
        public Nullable<int> user_brozeMedal { get; set; }
        public Nullable<int> user_vipMedal { get; set; }
        public Nullable<bool> question_activate { get; set; }
        public string question_title { get; set; }
        public Nullable<int> question_Answer { get; set; }
        public Nullable<int> question_totalComment { get; set; }
        public Nullable<int> question_view { get; set; }
        public Nullable<int> question_totalRate { get; set; }
        public Nullable<int> question_medalCalculator { get; set; }
        public Nullable<bool> question_recycleBin { get; set; }
        public Nullable<bool> question_userStatus { get; set; }
        public Nullable<int> question_popular { get; set; }
        public Nullable<bool> question_admin_recycleBin { get; set; }
        public string question_keywordSearch { get; set; }
        public string technology_name { get; set; }
        public string user_avatar { get; set; }
        


    }
}
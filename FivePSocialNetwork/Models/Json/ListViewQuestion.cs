using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListViewQuestion
    {
        public int viewQuestion_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> question_id { get; set; }
        public string viewQuestion_dateCreate { get; set; }
        public string user_firstName { get; set; }
        public string user_lastName { get; set; }
        public string question_title { get; set; }

    }
}
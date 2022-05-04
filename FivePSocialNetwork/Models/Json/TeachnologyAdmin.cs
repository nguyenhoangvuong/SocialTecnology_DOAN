using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class TeachnologyAdmin
    {
        public int technology_id { get; set; }
        public string technology_name { get; set; }
        public Nullable<int> technology_popular { get; set; }
        public Nullable<bool> technology_activate { get; set; }
        public Nullable<bool> technology_recycleBin { get; set; }
        public string technology_dateCreate { get; set; }
        public string technology_dateEdit { get; set; }
        public string technology_note { get; set; }
        public Nullable<int> technology_totalQuestion { get; set; }
    }
}
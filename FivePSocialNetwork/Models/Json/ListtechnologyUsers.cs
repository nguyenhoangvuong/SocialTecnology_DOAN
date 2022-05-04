using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListtechnologyUsers
    {
        public int technologyUser_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> technology_id { get; set; }
        public string technology_dateCreate { get; set; }
        public Nullable<bool> technology_recycleBin { get; set; }
        public string technology_name { get; set; }
    }
}
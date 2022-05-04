using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class DistrictAdmin
    {
        public int district_id { get; set; }
        public string district_name { get; set; }
        public Nullable<bool> district_activate { get; set; }
        public string district_dateCreate { get; set; }
        public string district_dateEdit { get; set; }
        public Nullable<bool> district_recycleBin { get; set; }
        public Nullable<int> provincial_id { get; set; }
        public string provincial_name { get; set; }

    }
}
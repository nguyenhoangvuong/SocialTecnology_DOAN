using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class SexAdmin
    {
        public int sex_id { get; set; }
        public string sex_name { get; set; }
        public Nullable<bool> sex_activate { get; set; }
        public string sex_dateCreate { get; set; }
        public string sex_dateEdit { get; set; }
        public Nullable<bool> sex_recycleBin { get; set; }
    }
}
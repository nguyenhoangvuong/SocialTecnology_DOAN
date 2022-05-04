using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ProvincialAdmin
    {
        public int provincial_id { get; set; }
        public string provincial_name { get; set; }
        public Nullable<bool> provincial_activate { get; set; }
        public string provincial_dateCreate { get; set; }
        public string provincial_dateEdit { get; set; }
        public Nullable<bool> provincial_recycleBin { get; set; }
    }
}
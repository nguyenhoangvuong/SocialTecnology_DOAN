using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ComuneAdmin
    {
        public int commune_id { get; set; }
        public string commune_name { get; set; }
        public Nullable<bool> commune_activate { get; set; }
        public string commune_dateCreate { get; set; }
        public string commune_dateEdit { get; set; }
        public Nullable<bool> commune_recycleBin { get; set; }
        public Nullable<int> district_id { get; set; }

        public string district_name { get; set; }
    }
}
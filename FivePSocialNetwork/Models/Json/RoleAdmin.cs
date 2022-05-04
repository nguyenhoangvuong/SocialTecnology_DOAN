using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class RoleAdmin
    {
        public int role_id { get; set; }
        public string role_name { get; set; }
        public Nullable<bool> role_activate { get; set; }
        public string role_dateCreate { get; set; }
        public string role_dateEdit { get; set; }
        public Nullable<bool> role_recycleBin { get; set; }
    }
}
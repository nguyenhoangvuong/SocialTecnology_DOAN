using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class UserAdmin
    {
        public int user_id { get; set; }
        public string user_firstName { get; set; }
        public string user_lastName { get; set; }
        public string email { get; set; }
    }
}
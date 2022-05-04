using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class DenounceAdmin
    {
        public int denounceUser_id { get; set; }
        public Nullable<int> accuser_id { get; set; }
        public string nameAccuser { get; set; }
        public Nullable<int> discredit_id { get; set; }
        public string namediscredit { get; set; }
        public string denounceUser_content { get; set; }
        public string denounceUser_dateCreate { get; set; }
        public Nullable<bool> denounceUser_recycleBin { get; set; }
        public Nullable<bool> denounce_viewStatus { get; set; }
        public Nullable<bool> denounce_status { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListTechnologyQuestion
    {
        public int teachnologyQuestion_id { get; set; }
        public Nullable<int> technology_id { get; set; }
        public Nullable<int> question_id { get; set; }
        public Nullable<bool> teachnologyQuestion_recycleBin { get; set; }

        public string technology_name { get; set; }
    }
}
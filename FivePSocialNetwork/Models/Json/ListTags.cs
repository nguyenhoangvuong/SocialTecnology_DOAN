using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListTags
    {
        public int tagsQuestion_id { get; set; }
        public Nullable<int> question_id { get; set; }
        public string tagsQuestion_name { get; set; }
        public string tagsQuestion_dateCreate { get; set; }

    }
}
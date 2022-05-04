using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FivePSocialNetwork.Models.Json
{
    public class ListUsers
    {
        public int user_id { get; set; }
        public string user_pass { get; set; }
        public string user_firstName { get; set; }
        public string user_lastName { get; set; }
        public string user_email { get; set; }
        public string user_token { get; set; }
        public Nullable<int> role_id { get; set; }
        public string user_code { get; set; }
        public string user_avatar { get; set; }
        public string user_coverImage { get; set; }
        public Nullable<bool> user_activate { get; set; }
        public Nullable<bool> user_statusOnline { get; set; }
        public Nullable<bool> user_recycleBin { get; set; }
        public string user_dateCreate { get; set; }
        public string user_dateEdit { get; set; }
        public string user_dateLogin { get; set; }
        public Nullable<bool> user_emailAuthentication { get; set; }
        public Nullable<bool> user_verifyPhoneNumber { get; set; }
        public Nullable<bool> user_loginAuthentication { get; set; }
        public Nullable<int> provincial_id { get; set; }
        public Nullable<int> district_id { get; set; }
        public Nullable<int> commune_id { get; set; }
        public string user_addressRemaining { get; set; }
        public Nullable<int> sex_id { get; set; }
        public string user_linkFacebook { get; set; }
        public string user_linkGithub { get; set; }
        public string user_anotherWeb { get; set; }
        public string user_hobbyWork { get; set; }
        public string user_hobby { get; set; }
        public string user_birthday { get; set; }
        public Nullable<int> user_popular { get; set; }
        public Nullable<int> user_goldMedal { get; set; }
        public Nullable<int> user_silverMedal { get; set; }
        public Nullable<int> user_brozeMedal { get; set; }
        public Nullable<int> user_vipMedal { get; set; }
        public string user_phone { get; set; }
        public int total_answer { get; set; }
        public int total_Question { get; set; }
        public string message { get; set; }
        public string message_dateSend { get; set; }
        public Nullable<bool> message_status { get; set; }
        public DateTime temporaryDate { get; set; }
        public int hoursSend { get; set; }
        public string messageRecipients_status { get; set; }

    }
}
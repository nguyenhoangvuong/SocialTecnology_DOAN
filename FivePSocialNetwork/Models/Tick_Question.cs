//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FivePSocialNetwork.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tick_Question
    {
        public int tickQuestion_id { get; set; }
        public Nullable<int> question_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<System.DateTime> tickQuestion_dateCreate { get; set; }
        public Nullable<bool> tickQuestion_recycleBin { get; set; }
    
        public virtual Question Question { get; set; }
        public virtual User User { get; set; }
    }
}

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
    
    public partial class Search
    {
        public int search_id { get; set; }
        public string search_content { get; set; }
        public Nullable<System.DateTime> search_dateCreate { get; set; }
        public Nullable<int> user_id { get; set; }
    
        public virtual User User { get; set; }
    }
}

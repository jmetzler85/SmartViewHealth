//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartViewHealth.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public int FacilityId { get; set; }
        public string lst_mod_id { get; set; }
        public System.DateTime lst_mod_ts { get; set; }
        public string row_sta_cd { get; set; }
        public System.DateTime entry_ts { get; set; }
        public string entry_user { get; set; }
    }
}

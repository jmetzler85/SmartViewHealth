using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SmartViewHealth.Models
{
    public class HealthSystemMetaData
    {
        [Display(Name = "Health System Name")]
        [Required]
        [MinLength(3)]
        public string HealthSystemName;

        [Display(Name = "Last Modified By")]
        public string lst_mod_id;

        [Display(Name = "Created By")]
        public string entry_user;

        [Display(Name = "Last Modified Date")]
        public System.DateTime lst_mod_ts;

        [Display(Name = "Creation Date")]
        public System.DateTime entry_ts;

        [Display(Name = "Active?")]
        public string row_sta_cd;
    }

    public class FacilityMetaData
    {
        [Display(Name = "Facility Name")]
        public string FacilityName;

        [Display(Name = "Last Modified By")]
        public string lst_mod_id;

        [Display(Name = "Created By")]
        public string entry_user;

        [Display(Name = "Last Modified Date")]
        public System.DateTime lst_mod_ts;

        [Display(Name = "Creation Date")]
        public System.DateTime entry_ts;

        [Display(Name = "Active?")]
        public string row_sta_cd;
    }

    public class LocationMetaData
    {
        [Display(Name = "Location Name")]
        public string LocationName;

        [Display(Name = "Last Modified By")]
        public string lst_mod_id;

        [Display(Name = "Created By")]
        public string entry_user;

        [Display(Name = "Last Modified Date")]
        public System.DateTime lst_mod_ts;

        [Display(Name = "Creation Date")]
        public System.DateTime entry_ts;

        [Display(Name = "Active?")]
        public string row_sta_cd;
    }

    public class EquipmentMetaData
    {
        [Display(Name = "Equipment Name")]
        public string EquipmentName;

        [Display(Name = "Location")]
        public int LocationId;

        [Display(Name = "Last Modified By")]
        public string lst_mod_id;

        [Display(Name = "Created By")]
        public string entry_user;

        [Display(Name = "Last Modified Date")]
        public System.DateTime lst_mod_ts;

        [Display(Name = "Creation Date")]
        public System.DateTime entry_ts;

        [Display(Name = "Active?")]
        public string row_sta_cd;
    }

    public class Equipment_TempMetaData
    {
        [Display(Name = "Range Low")]
        public int Temp_Range_Low;

        [Display(Name = "Range High")]
        public int Temp_Range_High;

        [Display(Name = "Hour Validation")]
        public int Temp_Hour_Chk;

    }
}
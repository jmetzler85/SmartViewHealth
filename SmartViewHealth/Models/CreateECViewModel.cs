using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class CreateECViewModel
    {
        public HealthSystem _HealthSystem { get; set; }

        public Facility _Facility { get; set; }

        public Location _Location { get; set; }

        public Equipment _Equipment { get; set; }

        public List<Equipment_EC_Supplies> _Equipment_EC_Supplies { get; set; }

        public List<Equipment_EC_Meds> _Equipment_EC_Meds { get; set; }

    }
}
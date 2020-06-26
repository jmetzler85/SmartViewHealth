using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class CreateTemperatureViewModel
    {
        public HealthSystem _HealthSystem { get; set; }

        public Facility _Facility { get; set; }

        public Location _Location { get; set; }

        public Equipment _Equipment { get; set; }

        public Equipment_Temp _Equipment_Temp { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class CreateSEViewModel
    {
        public HealthSystem _HealthSystem { get; set; }

        public Facility _Facility { get; set; }

        public Location _Location { get; set; }

        public Equipment _Equipment { get; set; }

        public List<Equipment_SE_Tasks> _Equipment_SE_Tasks_Daily { get; set; }

        public List<Equipment_SE_Tasks> _Equipment_SE_Tasks_Weekly { get; set; }

        public List<Equipment_SE_Tasks> _Equipment_SE_Tasks_Monthly { get; set; }
    }
}
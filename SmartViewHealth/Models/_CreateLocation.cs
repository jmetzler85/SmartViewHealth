using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public partial class _CreateLocationContext
    {
        public HealthSystem _HealthSystem { get; set; }

        public Facility _Facility { get; set; }

        public Location _Location { get; set; }
    }
}
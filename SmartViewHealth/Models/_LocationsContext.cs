using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public partial class _LocationsContext
    {
        public HealthSystem _HealthSystem { get; set; }

        public Facility _Facility { get; set; }

        public List<Location> _Locations { get; set; }

        public Pager Pager { get; set; }
    }
}
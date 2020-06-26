using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public partial class _FacilityContext
    {
        public HealthSystem _HealthSystem { get; set; }

        public List<Facility> _Facilities { get; set; }

        public Pager Pager { get; set; }
    }
}
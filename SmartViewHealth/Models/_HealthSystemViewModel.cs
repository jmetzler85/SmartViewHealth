using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class _HealthSystemViewModel
    {
        public List<HealthSystem> _HealthSystems { get; set; }
        public Pager Pager { get; set; }
    }
}
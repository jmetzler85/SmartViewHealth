using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public partial class _EquipmentsViewModel
    {
        public HealthSystem _HealthSystem { get; set; }

        public Facility _Facility { get; set; }

        public Location _Location { get; set; }

        public List<EquipmentWithType> _Equipments { get; set; }

        public List<Equipment_Types> _EquipmentTypes { get; set; }

        public Pager Pager { get; set; }
    }
}
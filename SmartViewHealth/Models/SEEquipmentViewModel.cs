using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class SEEquipmentViewModel
    {
        public Equipment _equipment { get; set; }

        public List<Equipment_SE_Tasks> _equipment_SE_Tasks { get; set; }

        public Location _Location { get; set; }

        public Facility _Facility { get; set; }

        public HealthSystem _HealthSystem { get; set; }

        public List<Equipment_SE_Log> lstSELogs { get; set; }

        public string SETaskType { get; set; }

        public string Tasks_RU_fg { get; set; }

        public string Tasks_QandA { get; set; }

        public Equipment_SE_Log _SE_Log { get; set; }
    }
}
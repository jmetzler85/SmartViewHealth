using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class TempEquipmentViewModel
    {
        public Equipment _equipment { get; set; }

        public Equipment_Temp _equipment_temp { get; set; }

        public Location _Location { get; set; }

        public Facility _Facility { get; set; }

        public HealthSystem _HealthSystem { get; set; }

        public List<Equipment_Temp_Log> lstTempLogs { get; set; }

        public int TempLogId { get; set; }
        public decimal TempValue { get; set; }
    }
}
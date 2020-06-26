using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class PreloadEquipmentLogInfo
    {
        public int? HealthSystemId { get; set; }

        public int? FacilityId { get; set; }

        public int? LocationId { get; set; }

        public int? EquipmentId { get; set; }
    }
}
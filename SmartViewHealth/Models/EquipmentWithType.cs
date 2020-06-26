using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class EquipmentWithType : Equipment
    {
        public int EquipmentTypeId{ get; set; }

        public string EquipmentTypeName { get; set; }
    }
}
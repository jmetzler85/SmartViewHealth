using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class LoggingViewModel
    {
        public List<LoggingHealthSystemViewModel> lstHealthSystems { get; set; }

        public PreloadEquipmentLogInfo peli { get; set; }
    }
}
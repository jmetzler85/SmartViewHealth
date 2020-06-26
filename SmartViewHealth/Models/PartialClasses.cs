using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SmartViewHealth.Models
{
    
    [MetadataType(typeof(HealthSystemMetaData))]
    public partial class HealthSystem
    {
    }

    [MetadataType(typeof(FacilityMetaData))]
    public partial class Facility
    {
    }

    [MetadataType(typeof(LocationMetaData))]
    public partial class Location
    {
    }

    [MetadataType(typeof(EquipmentMetaData))]
    public partial class Equipment
    {
    }

    [MetadataType(typeof(Equipment_TempMetaData))]
    public partial class Equipment_Temp
    {
    }

}
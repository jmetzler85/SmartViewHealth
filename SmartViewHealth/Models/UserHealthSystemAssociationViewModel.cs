using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class UserHealthSystemAssociationViewModel
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public string role_name { get; set; }

        public int association_id { get; set; }       
    }
}
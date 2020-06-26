using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartViewHealth.Models
{
    public class _HealthSystemUsersContext
    {
        public List<ApplicationUser> _UsersList { get; set; }

        public HealthSystem _HealthSystem { get; set; }

        public List<UserHealthSystemAssociationViewModel> _UserHealthSystemAssociationViewModel { get; set; }

        public Pager Pager { get; set; }
    }
}
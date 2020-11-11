using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class PrivilegeViewModel
    {
        public decimal PRIVILEGE_ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string MODULE { get; set; }
        public string DESCRIPTION { get; set; }
        
    }

    public class RoleViewModel
    {
        public decimal ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
    }
}

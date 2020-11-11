using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class UserRoleMappingViewModel
    {
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string Roles { get; set; }
        public string RoleId { get; set; }
        public string Create_Person { get; set; }
    }

    public class UserRoleMappingModel
    {
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string Roles { get; set; }
        public long RoleId { get; set; }
        public decimal? IsDelete { get; set; }
    }
}

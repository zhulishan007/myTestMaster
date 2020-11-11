using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class UserMasterModel
    {
        public string UserName { get; set; }
        public string UserEmail  { get; set; }
        public string Password { get; set; }
        public string DBConnection { get; set; }
    }

    public class ConnectionModel
    {
        public string DatabaseName { get; set; }
        public string Host { get; set; }
        public bool IsDefault { get; set; }
    }
}

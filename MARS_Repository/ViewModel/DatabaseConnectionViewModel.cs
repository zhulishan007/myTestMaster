using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class DatabaseConnectionViewModel
    {
        public long ConnectionId { get; set; }
        public string ConnectionName { get; set; }
        public short? ConnectionType { get; set; }
        public string ConnectionTypeString { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Protocol { get; set; }
        public string ServiceName { get; set; }
        public string Sid { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public short? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsTested { get; set; }
        public DateTime LastTested { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class DatabaseConnNameViewModel
    {
        public long ConnectionId { get; set; }
        public string ConnectionName { get; set; }
       
    }
}

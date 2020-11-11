using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class DBconnectionViewModel
    {
        public long connectionId { get; set; }
        public string Databasename { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public string Service_Name { get; set; }
        public string DecodeMethod { get; set; }
        public string Schema { get; set; }
        public string Entities { get; set; }
        public string App { get; set; }
    }
}

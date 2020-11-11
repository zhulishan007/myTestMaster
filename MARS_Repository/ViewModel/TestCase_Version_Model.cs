using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestCase_Version_Model
    {
        public long TC_VERSION_ID { get; set; }
        public long? TESTCASEID { get; set; }
        public long? CREATORID { get; set; }
        public long? ISAVAILABLE { get; set; }
        public long VERSION { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}

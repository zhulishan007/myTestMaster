using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestStepsModel
    {
        public long TestStepId { get; set; }
        public long? Run_Order { get; set; }
        public long? KeywordId { get; set; }
        public long? TestCaseId { get; set; }
        public long? ObjectId { get; set; }
        public string CollumRowSetting { get; set; }
        public string Comment { get; set; }
        public long ObjectNameId { get; set; }

        public string KeywordName { get; set; }
        public string ObjectName { get; set; }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestCaseValidationResultModel
    {
        public long ID { get; set; }
        public string VALIDATIONMSG { get; set; }
        public long? ISVALID { get; set; }
        public long? FEEDPROCESSID { get; set; }
        public long? FEEDPROCESSDETAILID { get; set; }
    }
}

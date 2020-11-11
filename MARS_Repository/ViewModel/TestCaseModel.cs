using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestCaseModel
    {
        public long TestCaseId { get; set; }
        public string TestCaseName { get; set; }
        public string TestCaseDescription { get; set; }
        public string Application { get; set; }
        public string ApplicationId { get; set; }
        public string TestSuite { get; set; }
        public string TestSuiteId { get; set; }
       public int TotalCount { get; set; }
  }
}

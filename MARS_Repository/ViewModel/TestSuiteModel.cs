using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestSuiteModel
    {
        public long TestSuiteId { get; set; }
        public string TestSuiteName { get; set; }
        public string TestSuiteDescription { get; set; }
        public string Application { get; set; }
        public string ApplicationId { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }
    public int TotalCount { get; set; }
  }

    public class RelTestSuiteApplication
    {
        public long TestSuiteId { get; set; }
        public string TestSuiteName { get; set; }
        public long? ApplicationId { get; set; }
        public string  Application { get; set; }
    }
}

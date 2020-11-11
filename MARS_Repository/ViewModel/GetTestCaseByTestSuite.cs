using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
   public class GetTestCaseByTestSuite
    {
        public string grid { get; set; }
        public long stepid { get; set; }
        public long projectid { get; set; }
        public string testsuitename { get; set; }
 
    }
    public class GetDatasetByTestcase
    {
        public string grid { get; set; }
        public long stepid { get; set; }
        public long projectid { get; set; }
        public string testsuitename { get; set; }
        public string Testcasename { get; set; }

    }
    public class Dependencylist
    {
        public string grid { get; set; }
        public long stepid { get; set; }
        public long projectid { get; set; }
        public string stepname { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class FolderDatasetViewModel
    {
        public long? DataSetId { get; set; }
        public string DatasetName { get; set; }
        public string DatasetDesc { get; set; }
        public string TestCase { get; set; }
        public long? TestCaseId { get; set; }
        public string TestSuite { get; set; }
        public long? TestSuiteId { get; set; }
        public string Storyboard { get; set; }
        public string ProjectIds { get; set; }
        public string ProjectName { get; set; }
        public long? SEQ { get; set; }
    }
}

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
    public class DataSetTagModel
    {
        public long? Tagid { get; set; }
        public string Folder { get; set; }
        public string Group { get; set; }
        public string Set { get; set; }
        public long Folderid { get; set; }
        public long Groupid { get; set; }
        public long Setid { get; set; }
        public long? Sequence { get; set; }
        public string Expectedresults { get; set; }
        public string StepDesc { get; set; }
        public string Diary { get; set; }
        public long? Datasetid { get; set; }
        public string datasetname { get; set; }
        public string datasetdescription { get; set; }
    }

    public class TestcaseJsonViewModel
    {
        public int ord { get; set; }
        public string k_n { get; set; }
        public long k { get; set; }
        public string obj { get; set; }
        public decimal objN_Id { get; set; }
        public long o_Id { get; set; }
        public string para { get; set; }
        public string data { get; set; }

    }

    public class TestcaseJsonModel
    {
        public TestcaseJsonModel()
        {
            applications = new List<ApplicationModel>();
        }
        public List<ApplicationModel> applications { get; set; }
        public string TcJson { get; set; }
        public Guid guid { get; set; }
        public string user { get; set; }
        public string database { get; set; }
    }
}

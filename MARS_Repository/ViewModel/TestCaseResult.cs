using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestCaseResult
    {
        public string STEPS_ID { get; set; }
        public string TEST_CASE_ID { get; set; }
        public string RUN_ORDER { get; set; }
        public string SKIP { get; set; }
        public string DATASETVALUE { get; set; }
        public string DATASETDESCRIPTION { get; set; }
        public string DATASETNAME { get; set; }
        public string TEST_SUITE_ID { get; set; }
        public string DATASETIDS { get; set; }
        public string parameter { get; set; }
        public string object_happy_name { get; set; }
        public string key_word_name { get; set; }
        public string test_step_description { get; set; }
        public string test_case_name { get; set; }
        public string test_suite_name { get; set; }
        public string Application { get; set; }
        public string COMMENT { get; set; }
        public string ROW_NUM { get; set; }
        public string Data_Setting_Id { get; set; }
        public long? VERSION { get; set; }
        public long? ISAVAILABLE { get; set; }
        public string EditingUserName { get; set; }
    }
}

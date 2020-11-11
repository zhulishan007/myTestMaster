using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TreeModel
    {
        public long PROJECT_ID { get; set; }
        public string project_name { get; set; }
        public long TestsuiteId { get; set; }
        public string TestsuiteName { get; set; }
        public long StoryboardId { get; set; }
        public string StoryboardName { get; set; }
    }

    public class ProjectList
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDesc { get; set; }
        public long TestSuiteCount { get; set; }
        public long StoryBoardCount { get; set; }
    }

    public class StoryBoardListByProject
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long StoryboardId { get; set; }
        public string StoryboardName { get; set; }
        public string Storyboardescription { get; set; }

    }

    public class TestSuiteListByProject
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long TestsuiteId { get; set; }
        public string TestsuiteName { get; set; }
        public long? TestCaseCount { get; set; }
        public string TestSuiteDesc { get; set; }
    }

    public class TestCaseListByProject
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long TestsuiteId { get; set; }
        public string TestsuiteName { get; set; }
        public long TestcaseId { get; set; }
        public string TestcaseName { get; set; }
        public long DataSetCount { get; set; }
        public string TestCaseDesc { get; set; }
    }
    public class DataSetListByTestCase
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long TestsuiteId { get; set; }
        public string TestsuiteName { get; set; }
        public long TestcaseId { get; set; }
        public string TestcaseName { get; set; }
        public long Datasetid { get; set; }
        public string Datasetname { get; set; }
    }
}

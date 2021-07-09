using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestResultModel
    {
        public long HistId { get; set; }
        public long? TestCaseId { get; set; }
        public long? StoryboardDetailId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TestResultInText { get; set; }
        public long? TestModeId { get; set; }
        public string TestMode { get; set; }
        public long? TestResult { get; set; }
        public DateTime? CreatTime { get; set; }
        public string ResultAliasName { get; set; }
        public string ResultDesc { get; set; }
        public long? LatestTestMarkId { get; set; }
        public bool IsMark { get; set; }
    }
    public class TestResultSaveModel
    {
        public long HistId { get; set; }
        public string DataVersion { get; set; }
        public string CreateTime { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
    }
    public class TestResultViewModel
    {
        public long? StepId { get; set; }
        public long? BaselineStepId { get; set; }
        public long? CompareStepId { get; set; }
        public long? BaselineReportId { get; set; }
        public long? CompareReportId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? RunningResult { get; set; }
        public string BreturnValues { get; set; }
        public string CreturnValues { get; set; }
        public string RunningResultInfo { get; set; }
        public string InputValueSetting { get; set; }
        public string Keyword { get; set; }
        public string ActualInputData { get; set; }
        public long? DataOrder { get; set; }
        public string InfoPic { get; set; }
        public string Advice { get; set; }
        public string Stackinfo { get; set; }
        public string Result { get; set; }
        public string BaselineComment { get; set; }
        public string CompareComment { get; set; }
        public string COMMENT { get; set; }
        public long? Run_Order { get; set; }
    }

    public class StorybookResultViewModel
    {
        public decimal? BCount { get; set; }
        public decimal? CCount { get; set; }
        public long? BaselineReportId { get; set; }
        public long? CompareReportId { get; set; }
        public string BreturnValues { get; set; }
        public string CreturnValues { get; set; }
        public string Keyword { get; set; }
        public string Result { get; set; }
        public string COMMENT { get; set; }
        public long? Run_Order { get; set; }
    }

    public class ValidatResultViewModel
    {
        public long? BaselineStepId { get; set; }
        public long? CompareStepId { get; set; }
        public string InputValueSetting { get; set; }
        public string BreturnValues { get; set; }
        public string CreturnValues { get; set; }
        public string result { get; set; }
        public string BaselineComment { get; set; }
        public string CompareComment { get; set; }
        public int? pq_ri { get; set; }
        public int? StepNo { get; set; }
        public bool? IsValid { get; set; }
        public string ValidMsg { get; set; }
    }

    public class HistIdViewModel
    {
        public long BaseHistId { get; set; }
        public long CompareHistId { get; set; }
    }
}

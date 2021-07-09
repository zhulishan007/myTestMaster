using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
  public class StoryBoardResultModel
  {
    public long Run_order { get; set; }
    public long ProjectId { get; set; }
    public string ApplicationName { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public string Storyboardname { get; set; }
    public long Storyboardid { get; set; }
    public string ActionName { get; set; }
    public string StepName { get; set; }
    public string TestSuiteName { get; set; }
    public string TestCaseName { get; set; }
    public string DataSetName { get; set; }
    public string Dependency { get; set; }
    public string BTestResult { get; set; }
    public string BErrorcause { get; set; }
    public DateTime? BScriptstart { get; set; }
    public string Bstart { get; set; }
    public DateTime? BScriptend { get; set; }
    public string CTestResult { get; set; }
    public string CErrorcause { get; set; }
    public DateTime? CScriptstart { get; set; }
    public string Cstart { get; set; }
    public DateTime? CScriptend { get; set; }
    public string Description { get; set; }
    public long Suiteid { get; set; }
    public long Caseid { get; set; }
    public long Datasetid { get; set; }
    public long? BHistid { get; set; }
    public long? CHistid { get; set; }
    public long? storyboarddetailid { get; set; }
    public bool? IsValid { get; set; }
    public string ValidationMsg { get; set; }
    public int? RowId { get; set; }
    public long runtype { get; set; }
    public long? Dependson { get; set; }
    public long? latestmark { get; set; }
    public decimal? recordvision { get; set; }
  }
  public class StoryboardDatasetSetting
  {
    public long Settingid { get; set; }
    public long Datasetid { get; set; }
    public long Detailid { get; set; }
  }
    public class StoryBoardResultExportModel
    {
        public string STORYBOARDDETAILID { get; set; }
        public string STORYBOARDID { get; set; }
        public string PROJECTID { get; set; }
        public string APPLICATIONNAME { get; set; }
        public string RUNORDER { get; set; }
        public string PROJECTNAME { get; set; }
        public string PROJECTDESCRIPTION { get; set; }
        public string STORYBOARD_NAME { get; set; }
        public string ACTIONNAME { get; set; }
        public string STEPNAME { get; set; }
        public string SUITENAME { get; set; }
        public string CASENAME { get; set; }
        public string DATASETNAME { get; set; }
        public string DEPENDENCY { get; set; }
        public string TEST_STEP_DESCRIPTION { get; set; }
    }

    public class StoryboardResultDataModel
    {
        public long TEST_REPORT_STEP_ID { get; set; }
        public long steps_id { get; set; }
        public DateTime BEGIN_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public long RUNNING_RESULT { get; set; }
        public string Baseline_RETURN_VALUES { get; set; }
        public string Compare_RETURN_VALUES { get; set; }
        public string RUNNING_RESULT_INFO { get; set; }
        public string INPUT_VALUE_SETTING { get; set; }
        public string ACTUAL_INPUT_DATA { get; set; }
        public long DATA_ORDER { get; set; }
        public string ADVICE { get; set; }
        public string STACKINFO { get; set; }
    }
}

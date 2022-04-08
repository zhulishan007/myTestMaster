using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARSUtility.ViewModel
{
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


    public class StoryBoardExportModel
    {
        public string FOLDERNAME { get; set; }
        public string FOLDERDESC { get; set; }
        public string STORYBOARD_NAME { get; set; }
        public string ACTIONNAME { get; set; }
        public string SUITENAME { get; set; }
        public string CASENAME { get; set; }
        public string DATASETNAME { get; set; }
        public string BTEST_RESULT { get; set; }
        public string BTEST_RESULT_IN_TEXT { get; set; }
        public DateTime? BTEST_BEGIN_TIME { get; set; }
        public DateTime? BTEST_END_TIME { get; set; }
        public string CTEST_RESULT { get; set; }
        public string CTEST_RESULT_IN_TEXT { get; set; }
        public DateTime? CTEST_BEGIN_TIME { get; set; }
        public DateTime? CTEST_END_TIME { get; set; }
        public long? BHistid { get; set; }
        public long? CHistid { get; set; }
    }

    public class TestResultExportModel
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
    }
}

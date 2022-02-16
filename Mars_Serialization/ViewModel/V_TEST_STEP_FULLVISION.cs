using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mars_Serialization.Common.CommonEnum;

namespace Mars_Serialization.ViewModel
{
    public class V_TEST_STEP_FULLVISION
    {
        public Nullable<long> TEST_CASE_ID { get; set; }
        public string TEST_CASE_NAME { get; set; }
        public Nullable<long> KEY_WORD_ID { get; set; }
        public string KEY_WORD_NAME { get; set; }
        public string OBJECT_HAPPY_NAME { get; set; }
        public Nullable<long> OBJECT_ID { get; set; }
        public string OBJECT_TYPE { get; set; }
        public string QUICK_ACCESS { get; set; }
        public string ENUM_TYPE { get; set; }
        public Nullable<long> APPLICATION_ID { get; set; }
        public Nullable<long> OBJECT_NAME_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public long STEPS_ID { get; set; }
        public string COLUMN_ROW_SETTING { get; set; }
        public string COMMENTINFO { get; set; }
        public Nullable<long> IS_RUNNABLE { get; set; }
        public Nullable<long> RUN_ORDER { get; set; }
        public string VALUE_SETTING { get; set; }
        public MarsRecordStatus recordStatus = MarsRecordStatus.en_None;
    }

    public class MB_V_TEST_STEPS : V_TEST_STEP_FULLVISION
    {
        public List<DataForDataSets> dataForDataSets = new List<DataForDataSets>();
    }
    public class MB_REL_TC_DATA_SUMMARY : REL_TC_DATA
    {
        public MarsRecordStatus recordStatus = MarsRecordStatus.en_None;
    }
    public class Mars_Memory_TestCase
    {
        public MarsRecordStatus currentSyncroStatus = MarsRecordStatus.en_None;
        public long[] assignedApplications;
        public long[] assignedTestSuiteIDs;
        public string version;
        public List<MB_REL_TC_DATA_SUMMARY> assignedDataSets;
        public List<MB_V_TEST_STEPS> allSteps;
    }

    public class DataForDataSets
    {
        public long Data_Setting_Id { get; set; }
        public string DATASETVALUE { get; set; }
        public long DATA_SUMMARY_ID { get; set; }
        public int SKIP { get; set; }
        public long STEPS_ID { get; set; }
    }
    public class REL_TC_DATA
    {
        public long DATA_SUMMARY_ID { get; set; }
        public string ALIAS_NAME { get; set; }
        //public string DESCRIPTION_INFO { get; set; }
        //public Nullable<short> AVAILABLE_MARK { get; set; }
        //public Nullable<long> VERSION { get; set; }
        //public Nullable<short> SHARE_MARK { get; set; }
        //public Nullable<System.DateTime> CREATE_TIME { get; set; }
        //public Nullable<short> STATUS { get; set; }
        //public Nullable<short> DATA_SET_TYPE { get; set; }
    }
}

using MarsSerializationHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class TestStepsModel
    {
        public long TestStepId { get; set; }
        public long? Run_Order { get; set; }
        public long? KeywordId { get; set; }
        public long? TestCaseId { get; set; }
        public long? ObjectId { get; set; }
        public string CollumRowSetting { get; set; }
        public string Comment { get; set; }
        public long ObjectNameId { get; set; }

        public string KeywordName { get; set; }
        public string ObjectName { get; set; }
    }

    public class StepsJsonMemoryModel
    {
        public RecordStatus currentSyncroStatus = RecordStatus.en_None;
        public long[] assignedApplications;
        public long[] assignedTestSuiteIDs;
        public string version;
        public List<REL_TEST_CASE_DATA_SUMMARY> assignedDataSets;
        public List<VIEW_TEST_STEPS> allSteps;
    }

    public enum RecordStatus
    {
        en_None = 0x00,
        en_fromDb,
        en_NewToDb,
        en_ModifiedToDb,
        en_DeletedToDb,
        en_FailedWhenUpdateDB = -0x1
    }
    public class VIEW_TEST_STEP_FULLVISION
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
        public RecordStatus recordStatus = RecordStatus.en_None;
    }

    public class VIEW_TEST_STEPS : VIEW_TEST_STEP_FULLVISION
    {
        public List<Data_ForDataSets> dataForDataSets = new List<Data_ForDataSets>();
    }
    public class REL_TEST_CASE_DATA_SUMMARY : REL_TC_DATA
    {
        public RecordStatus recordStatus = RecordStatus.en_None;
    }

    public class Data_ForDataSets
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
    }
}

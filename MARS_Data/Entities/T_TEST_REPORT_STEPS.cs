//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MARS_Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_TEST_REPORT_STEPS
    {
        public long TEST_REPORT_STEP_ID { get; set; }
        public Nullable<long> TEST_REPORT_ID { get; set; }
        public Nullable<long> STEPS_ID { get; set; }
        public Nullable<System.DateTime> BEGIN_TIME { get; set; }
        public Nullable<System.DateTime> END_TIME { get; set; }
        public Nullable<short> RUNNING_RESULT { get; set; }
        public string RETURN_VALUES { get; set; }
        public string RUNNING_RESULT_INFO { get; set; }
        public Nullable<long> DATA_SUMMARY_ID { get; set; }
        public string INPUT_VALUE_SETTING { get; set; }
        public string ACTUAL_INPUT_DATA { get; set; }
        public Nullable<long> DATA_ORDER { get; set; }
        public byte[] INFO_PIC { get; set; }
    
        public virtual T_TEST_REPORT T_TEST_REPORT { get; set; }
    }
}

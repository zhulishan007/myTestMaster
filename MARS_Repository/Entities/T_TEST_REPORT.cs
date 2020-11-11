//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MARS_Repository.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_TEST_REPORT
    {
        public T_TEST_REPORT()
        {
            this.T_TEST_REPORT_STEPS = new HashSet<T_TEST_REPORT_STEPS>();
        }
    
        public long TEST_REPORT_ID { get; set; }
        public Nullable<long> TEST_CASE_ID { get; set; }
        public Nullable<decimal> LOOP_ID { get; set; }
        public Nullable<System.DateTime> BEGIN_TIME { get; set; }
        public Nullable<System.DateTime> END_TIME { get; set; }
        public Nullable<short> RUNNING_RESULT { get; set; }
        public string RETURN_VALUES { get; set; }
        public string RUNNING_RESULT_INFO { get; set; }
        public Nullable<long> HIST_ID { get; set; }
        public Nullable<long> APPLICATION_ID { get; set; }
        public Nullable<short> TEST_MODE { get; set; }
    
        public virtual T_TEST_CASE_SUMMARY T_TEST_CASE_SUMMARY { get; set; }
        public virtual ICollection<T_TEST_REPORT_STEPS> T_TEST_REPORT_STEPS { get; set; }
    }
}

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
    
    public partial class T_SHARED_OBJECT_POOL
    {
        public long OBJECT_POOL_ID { get; set; }
        public Nullable<long> DATA_SUMMARY_ID { get; set; }
        public string OBJECT_NAME { get; set; }
        public Nullable<long> OBJECT_ORDER { get; set; }
        public Nullable<long> LOOP_ID { get; set; }
        public string DATA_VALUE { get; set; }
        public Nullable<System.DateTime> CREATE_TIME { get; set; }
        public Nullable<long> VERSION { get; set; }
    
        public virtual T_TEST_DATA_SUMMARY T_TEST_DATA_SUMMARY { get; set; }
    }
}

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
    
    public partial class LOGREPORT
    {
        public string FILENAME { get; set; }
        public string OBJECT { get; set; }
        public string STATUS { get; set; }
        public string LOGDETAILS { get; set; }
        public decimal ID { get; set; }
        public Nullable<System.DateTime> CREATEDON { get; set; }
        public Nullable<decimal> FEEDPROCESSDETAILID { get; set; }
    }
}

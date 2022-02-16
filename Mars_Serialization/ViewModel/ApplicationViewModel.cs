using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.ViewModel
{
    public class ApplicationViewModel
    {
        public long APPLICATION_ID { get; set; }
        public string APP_SHORT_NAME { get; set; }
        public string PROCESS_IDENTIFIER { get; set; }
        public string STARTER_PATH { get; set; }
        public string STARTER_COMMAND { get; set; }
        public string VERSION { get; set; }
        public string COMMENT { get; set; }
        public Nullable<short> APPLICATION_TYPE_ID { get; set; }
        public string RECORD_CREATE_PERSON { get; set; }
        public Nullable<System.DateTime> RECORD_CREATE_DATE { get; set; }
        public string EXTRAREQUIREMENT { get; set; }
        public string EXTRAPOPUPMENU { get; set; }
        public Nullable<decimal> ISBASELINE { get; set; }
        public Nullable<decimal> IS64BIT { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.ViewModel
{
    public class OBJECT_SNAPSHOT
    {
        public long OT_TYPE_ID { get; set; }
        public Nullable<decimal> OBJECT_NAME_ID { get; set; }
        public string OBJECT_HAPPY_NAME { get; set; }
        public string COMMENT { get; set; }
        public Nullable<long> APPLICATION_ID { get; set; }
        public string ENUM_TYPE { get; set; }
        public long OBJECT_ID { get; set; }
        public string OBJECT_TYPE { get; set; }
        public string QUICK_ACCESS { get; set; }
        public Nullable<long> TYPE_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public string PEG_NAME { get; set; }
        public string PEG_QUICK_ACCESS { get; set; }
        public Nullable<long> PEG_ID { get; set; }
    }
}

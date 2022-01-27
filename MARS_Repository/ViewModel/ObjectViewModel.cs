using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class ObjectViewModel
    {
        public decimal OBJECT_NAME_ID { get; set; }
        public string OBJECT_HAPPY_NAME { get; set; }
        public string COMMENT { get; set; }
        public Nullable<long> APPLICATION_ID { get; set; }
        public string ENUM_TYPE { get; set; }
        public long OBJECT_ID { get; set; }
        public string OBJECT_TYPE { get; set; }
        public string QUICK_ACCESS { get; set; }
        public Nullable<long> TYPE_ID { get; set; }
        public Nullable<short> IS_CHECKERROR_OBJ { get; set; }
        public string TYPE_NAME { get; set; }
        public string PEG_NAME { get; set; }
        public string PEG_QUICK_ACCESS { get; set; }
        public long PEG_ID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
   public class CompareParam
    {
        public long DATA_SOURCE_ID { get; set; }
        public string DATA_SOURCE_NAME { get; set; }
        public short? DATA_SOURCE_TYPE { get; set; }
        public string DETAILS { get; set; }
        public string DB_CONNECTION { get; set; }
        public int? DB_TYPE { get; set; }
        public string TEST_CONNECTION { get; set; }
    }
}

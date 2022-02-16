using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.ViewModel
{
    public class ProjectViewModel
    {
        public long PROJECT_ID { get; set; }
        public string PROJECT_NAME { get; set; }
        public string PROJECT_DESCRIPTION { get; set; }
        public string CREATOR { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<short> STATUS { get; set; }
        public decimal TestSuiteCount { get; set; }
        public decimal StoryBoardCount { get; set; }
        public string PROJECTEXISTS { get; set; }
    }
}

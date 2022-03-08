using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.ViewModel
{
    public class GroupsViewModel
    {
        public long GROUPID { get; set; }
        public string GROUPNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<short> ACTIVE { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string CREATION_USER { get; set; }
        public string UPDATE_CREATION_USER { get; set; }
    }
}

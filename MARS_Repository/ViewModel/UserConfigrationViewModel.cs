using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class UserConfigrationViewModel
    {
        public decimal Id { get; set; }
        public string MainKey { get; set; }
        public string SubKey { get; set; }
        public decimal? UserId { get; set; }
        public byte[] BLOBValue { get; set; }
        public string BLOBValuestr { get; set; }
        public short? BLOBValueType { get; set; }
        public string BLOBType { get; set; }
        public string MARSUserName { get; set; }
        public string Description { get; set; }
        public string Create_Person { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

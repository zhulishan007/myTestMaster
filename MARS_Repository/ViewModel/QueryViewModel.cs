using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class QueryViewModel
    {
        public long QueryId { get; set; }
        public string QueryName { get; set; }
        public string QueryDescription { get; set; }
        public int isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class QueryNameViewModel
    {
        public long QueryId { get; set; }
        public string QueryName { get; set; }
    }

    public class QueryGridViewModel
    {
        public long QueryId { get; set; }
        public string QueryName { get; set; }
        public string QueryDescription { get; set; }
        public short? IsActive { get; set; }
        public long ConnectionId { get; set; }
        public string ConnectionName { get; set; }
        public short? ConnectionType { get; set; }
        public string ConnectionTypeString { get; set; }
    }
}

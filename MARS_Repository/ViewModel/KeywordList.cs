using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class KeywordList
    {
        public long KeywordId { get; set; }
        public string KeywordName { get; set; }
    }
    public class KeywordViewModel
    {
        public long KeywordId { get; set; }
        public string KeywordName { get; set; }
        public string ControlType { get; set; }
        public string ControlTypeId { get; set; }
        public string EntryFile { get; set; }
    public int TotalCount { get; set; }
    }
}

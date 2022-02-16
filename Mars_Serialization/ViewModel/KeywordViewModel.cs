using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.ViewModel
{
    public class KeywordViewModel
    {
        public long KEY_WORD_ID { get; set; }
        public string KEY_WORD_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<short> KEY_WORD_POSITION_ID { get; set; }
        public string ENTRY_IN_DATA_FILE { get; set; }
        public List<DIC_RELATION_KEYWORD> KeywordType { get; set; } = new List<DIC_RELATION_KEYWORD>();
    }

    public class DIC_RELATION_KEYWORD
    {
        public long RELATION_ID { get; set; }
        public Nullable<long> TYPE_ID { get; set; }
        public Nullable<long> KEY_WORD_ID { get; set; }
    }
}

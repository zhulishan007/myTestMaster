using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization
{
    public class CommonHelper
    {
        public string ExtractString(string s, string tag)
        {
            // You should check for errors in real-world code, omitted for brevity
            var startTag = "" + tag + "";
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            int endIndex = s.IndexOf("" + tag + "", startIndex);
            return s.Substring(startIndex, endIndex - startIndex);
        }
    }
}

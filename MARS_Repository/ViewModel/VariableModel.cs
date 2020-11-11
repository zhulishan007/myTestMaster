using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
   public class VariableModel
    {
        public long Lookupid { get; set; }
        public string Table_name { get; set; }
        public string field_name { get; set; }
        public short? value{ get; set; }
        public string Display_name { get; set; }
        public short? status { get; set; }
        public string Statusvalue { get; set; }
        public int TotalCount { get; set; }
    }

    public class VariableExportModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string BaseComp { get; set; }
    }
}

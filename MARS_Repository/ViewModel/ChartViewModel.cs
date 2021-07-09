using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class ChartViewModel
    {
        public object[] XAxis { get; set; }
        public object[] YAxis { get; set; }
        public object[,] ZAxis { get; set; }

    }

    public class AxisViewModel
    {
        public int axis_id { get; set; }
        public string axis_name { get; set; }
        public bool selected { get; set; }
    }

    public class AxisDataViewModel
    {
        public long? axisId { get; set; }
        public long? queryId { get; set; }
        public short? chartType { get; set; }
        public short? xAxis { get; set; } 
        public short? yAxis { get; set; }
        public short? zAxis { get; set; }

    }

    
}

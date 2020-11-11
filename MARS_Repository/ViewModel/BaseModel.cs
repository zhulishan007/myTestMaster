using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class BaseModel
    {
        public int status { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
    }

    public class ResultModel
    {
        public int status { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
    }
}

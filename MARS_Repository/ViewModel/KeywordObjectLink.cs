using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class KeywordObjectLink
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public string Keyword { get; set; }
        public string Object { get; set; }
        public string ValidationMsg { get; set; }
        public bool IsNotValid { get; set; }
        public int? pq_ri { get; set; }
    }
}

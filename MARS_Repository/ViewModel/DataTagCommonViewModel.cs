using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
   public class DataTagCommonViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<short> IsActive { get; set; }
        public string Active { get; set; }
    }
}

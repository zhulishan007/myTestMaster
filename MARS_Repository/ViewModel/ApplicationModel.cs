using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class ApplicationModel
    {
        public long ApplicationId { get; set; }
        public string ApplicationName { get; set; }
    }

    public class ApplicationViewModel
    {
        public long ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string ExtraRequirement { get; set; }
        public string ExtraRequirementId { get; set; }
        public string Create_Person { get; set; }
        public string Mode { get; set; }
        public decimal? IS64BIT { get; set; }
        public string Bits { get; set; }
        public string BitsId { get; set; }
    }
}

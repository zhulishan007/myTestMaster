using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARSUtility.ViewModel
{
   public class DatasetTagExportModel
    {
        public string ALIAS_NAME { get; set; }
        public string DESCRIPTION_INFO { get; set; }
        public string GROUPNAME { get; set; }
        public string GROUPDESCRIPTION { get; set; }
        public string SETNAME { get; set; }
        public string SETDESCRIPTION { get; set; }
        public string FOLDERNAME { get; set; }
        public string FOLDERDESCRIPTION { get; set; }
        public string EXPECTEDRESULTS { get; set; }
        public string STEPDESC { get; set; }
        public string DIARY { get; set; }
        public decimal? SEQUENCE { get; set; }
    }

    public class DataTagCommonViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<short> Active { get; set; }
        public string Status { get; set; }
    }

    public class DatasetTagReportExportModel
    {
        public long? PROJECT_ID { get; set; }
        public string PROJECT_NAME { get; set; }
        public long? RUN_ORDER { get; set; }
        public string STORYBOARD_NAME { get; set; }
        public long? STORYBOARD_ID { get; set; }
        public string TEST_CASE_NAME { get; set; }
        public string ALIAS_NAME { get; set; }
        public string DESCRIPTION_INFO { get; set; }
        public string GROUPNAME { get; set; }
        public string GROUPDESCRIPTION { get; set; }
        public string SETNAME { get; set; }
        public string SETDESCRIPTION { get; set; }
        public string FOLDERNAME { get; set; }
        public string FOLDERDESCRIPTION { get; set; }
        public string EXPECTEDRESULTS { get; set; }
        public decimal? SEQUENCE { get; set; }
        public string STEPDESC { get; set; }
        public string DIARY { get; set; }
    }
}

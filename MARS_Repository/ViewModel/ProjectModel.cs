using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class ProjectModel
    {
        public long ProjectId { get; set; }
        public string Project{ get; set; }
        public string ProjectDescription { get; set; }
        public long? ApplicationId { get; set; }
        public string Application{ get; set; }
        //public string ApplicationName { get; set; }
    }
    public class RelProjectApplication
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long ApplicationId { get; set; }
        public string ApplicationName { get; set; }
    }

    public class ProjectViewModel
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string CarectorName { get; set; }
        public string ApplicationId { get; set; }
        public string Application { get; set; }
        public string Status { get; set; }
        public short? StatusId { get; set; }
    public int TotalCount { get; set; }
    }
}

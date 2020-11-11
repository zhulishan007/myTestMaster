using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class ProjectByUser
    {
        public string ProjectName { get; set; }
        public long ProjectId { get; set; }
        public string username { get; set; }
        public decimal userId { get; set; }
        public bool ProjectExists { get; set; }

      
        public string ProjectDesc { get; set; }
        public long TestSuiteCount { get; set; }
        public long StoryBoardCount { get; set; }
    }
    public class Project
    {
        public string ProjectName { get; set; }
        public long ProjectId { get; set; }
        public int ProjectExists { get; set; }
        public decimal Userid { get; set; }
    }
    }

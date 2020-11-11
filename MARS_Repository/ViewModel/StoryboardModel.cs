using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class StoryboardModel
    {
        public long Storyboardid { get; set; }
        public string Storyboardname { get; set; }
        public string StoryboardDescription { get; set; }
        public long ProjectId { get; set; }
        public string Projectname { get; set; }
        public int TotalCount { get; set; }
    }

    public class InsertStoryboardModel
    {
        public long? PROJECT_ID { get; set; }
        public long? TEST_SUITE_ID { get; set; }
        public long? TEST_CASE_ID { get; set; }
        public short? RUN_TYPE { get; set; }
        public long? RUN_ORDER { get; set; }
        public long? DEPENDS_ON { get; set; }
        public long? LATEST_TEST_MARK_ID { get; set; }
        public decimal? RECORD_VERSION { get; set; }
        public string ALIAS_NAME { get; set; }
        public long? STORYBOARD_DETAIL_ID { get; set; }
    }
}

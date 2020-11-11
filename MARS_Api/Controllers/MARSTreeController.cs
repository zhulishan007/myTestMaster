using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Api.Helper;

namespace MARS_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class MARSTreeController : ApiController
    {
        // GET: ProjectList
        //Loads all the Projects
        [HttpGet]
        [Route("api/LeftPanel")]
        [AcceptVerbs("GET", "POST")]
        public List<ProjectByUser> LeftPanel(long userid)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var repMARSTree = new GetTreeRepository();
            //var lProjectList = repMARSTree.GetProjectList();
            var lProjectList = repMARSTree.GetProjectList(userid, AppConnDetails.Schema, AppConnDetails.ConnString);
            return lProjectList;
        }

        //Loads List of TestSuites in the left panel based on Project Id
        [HttpGet]
        [Route("api/GetTestSuiteByProject")]
        [AcceptVerbs("GET", "POST")]
        public List<TestSuiteListByProject> GetTestSuiteByProject(long ProjectId)
        {
            CommonHelper.SetConnectionString(Request);
            var repMARSTree = new GetTreeRepository();
            var lTestSuiteList = repMARSTree.GetTestSuiteList(ProjectId);
            return lTestSuiteList;
        }

        //Loads List of TestCases in the left panel based on ProjectId and TestSuiteId
        [HttpGet]
        [Route("api/GetTestCaseByProject")]
        [AcceptVerbs("GET", "POST")]
        public List<TestCaseListByProject> GetTestCaseByProject(long ProjectId, long TestSuiteId)
        {
            CommonHelper.SetConnectionString(Request);
            var repMARSTree = new GetTreeRepository();
            var lTestCaseList = repMARSTree.GetTestCaseList(ProjectId, TestSuiteId);
            return lTestCaseList;
        }
        [Route("api/LeftPanelStoryboard")]
        [AcceptVerbs("GET", "POST")]
        public List<StoryBoardListByProject> LeftPanelStoryboard(long ProjectId)
        {
            CommonHelper.SetConnectionString(Request);
            var repTree = new GetTreeRepository();
            var lstoryboardlist = repTree.GetStoryboardList(ProjectId);
            return lstoryboardlist;
        }

    [Route("api/LeftPanelDataSet")]
    [AcceptVerbs("GET", "POST")]
    public List<DataSetListByTestCase> LeftPanelDataSet(long lProjectId, long lTestSuiteId, long lTestCaseId, string lProjectName, string lTestSuiteName, string lTestCaseName)
    {
      CommonHelper.SetConnectionString(Request);
      var repTree = new GetTreeRepository();
      var ldatasetlist = repTree.GetDataSetList(lProjectId, lTestSuiteId, lTestCaseId, lProjectName, lTestSuiteName, lTestCaseName);
      return ldatasetlist;
    }
  }
}

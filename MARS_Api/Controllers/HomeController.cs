using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

using MARS_Repository.Repositories;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Api.Helper;

namespace MarsApi.Controllers
{
  [Authorize]
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  public class HomeController : ApiController
  {
    //Get Project Name, TestSuite Name and TestCase Name
    [HttpPost]
    [Route("api/GetTcName")]
    [AcceptVerbs("GET", "POST")]
    public string GetTcName(string lProjectId, string lTestSuiteId, string lTestCaseId)
    {
      CommonHelper.SetConnectionString(Request);
      var repProj = new TestProjectRepository();
      var repCase = new TestCaseRepository();
      var repSuite = new TestSuiteRepository();
      var lProjectName = "";
      var lTestCaseName = "";
      var lTestSuiteName = "";

      if (Convert.ToInt64(lProjectId) > 0)
      {
        lProjectName = repProj.GetProjectNameById(Convert.ToInt64(lProjectId));
      }
      if (Convert.ToInt64(lTestSuiteId) > 0)
      {
        lTestSuiteName = repSuite.GetTestSuiteNameById(Convert.ToInt64(lTestSuiteId));
      }
      if (Convert.ToInt64(lTestCaseId) > 0)
      {
        lTestCaseName = repCase.GetTestCaseNameById(Convert.ToInt64(lTestCaseId));
      }
      var lResult = lProjectName + "#" + lTestCaseName + "#" + lTestSuiteName;
      //return Json(lResult, JsonRequestBehavior.AllowGet);
      return lResult;
    }

    [Route("api/GetSBBreadcum")]
    [AcceptVerbs("GET", "POST")]
    public string GetSBBreadcum(string lProjectId, string lStoryboardId)
    {
      CommonHelper.SetConnectionString(Request);
      var repProj = new TestProjectRepository();
      var repstory = new StoryBoardRepository();
      // var repCase = new TestCaseRepository();
      //var repSuite = new TestSuiteRepository();
      var lProjectName = "";
      var lstoryboardname = "";


      if (Convert.ToInt64(lProjectId) > 0)
      {
        lProjectName = repProj.GetProjectNameById(Convert.ToInt64(lProjectId));
      }
      if (Convert.ToInt64(lStoryboardId) > 0)
      {
        lstoryboardname = repstory.GetStoryboardById(Convert.ToInt64(lStoryboardId));
      }

      var lResult = lProjectName + "#" + lstoryboardname;

      return lResult;
    }

    [Route("api/PartialRightStoryboardGrid")]
    [AcceptVerbs("GET", "POST")]
    public string PartialRightStoryboardGrid(int Projectid = 0, int Storyboardid = 0)
    {
      string result = "";
      CommonHelper.SetConnectionString(Request);
      StoryBoardRepository repo = new StoryBoardRepository();
      //ViewBag.Accesstoken = SessionManager.Accesstoken;
      //ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebApiURL"];
      
      if (Projectid == 0 && Storyboardid == 0)
      {
        result =Projectid+",";
        result =result+ Storyboardid+",";
        result = result + "0";
      }
      else
      {
        result = Projectid + ",";
        result = result + Storyboardid + ",";
        result = result + repo.GetStoryboardById(Storyboardid);
       
      }
      return result;
    }

    [Route("api/RightSideGridView")]
    [AcceptVerbs("GET", "POST")]
    public string RightSideGridView(int TestcaseId = 0, int TestsuiteId = 0, int ProjectId = 0, string VisibleDataset = "")
    {
      CommonHelper.SetConnectionString(Request);
      var result = "";
      //ViewBag.Accesstoken = SessionManager.Accesstoken;
      //ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebApiURL"];
    
      var lRep = new TestCaseRepository();
      if (TestcaseId == 0 && TestsuiteId == 0 && ProjectId == 0)
      {
        result = TestcaseId + ",";
        result = result + TestsuiteId + ",";
        result = result + ProjectId +",";
        result = result + "0" + ",";
        result = result + "";
       
      }
      else
      {
        result = TestcaseId + ",";
        result = result + TestsuiteId + ",";
        result = result + ProjectId + ",";
        result = result + lRep.GetTestCaseNameById(TestcaseId) + ",";
        result = result + VisibleDataset;
        
      }
      return result;
    }
   
  }
}

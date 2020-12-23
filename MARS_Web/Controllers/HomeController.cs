using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class HomeController : Controller
    {
        public HomeController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        
        // Main page after login User 
        public ActionResult Index(int TestcaseId = 0, int TestsuiteId = 0, int ProjectId = 0)
        {
            try
            {
                ViewBag.Accesstoken = SessionManager.Accesstoken;
                ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebApiURL"];
                var lRep = new TestCaseRepository();
                AccountRepository accrepo = new AccountRepository();
                var repo = new StoryBoardRepository();
                var _treerepository = new GetTreeRepository();
                var _etlrepository = new EntitlementRepository();
                _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                lRep.UpdateIsAvailableReload((long)SessionManager.TESTER_ID);
                ViewBag.Title = "Home Page";
                if (TestcaseId == 0 && TestsuiteId == 0 && ProjectId == 0)
                {
                    ViewBag.TestcaseId = TestcaseId;
                    ViewBag.TestsuiteId = TestsuiteId;
                    ViewBag.ProjectId = ProjectId;
                    ViewBag.TestCaseName = "0";
                }
                else
                {
                    ViewBag.TestcaseId = TestcaseId;
                    ViewBag.TestsuiteId = TestsuiteId;
                    ViewBag.ProjectId = ProjectId;
                    ViewBag.TestCaseName = lRep.GetTestCaseNameById(TestcaseId);
                    Session["TestsuiteId"] = ViewBag.TestsuiteId;
                    Session["TestcaseId"] = ViewBag.TestcaseId;
                    Session["ProjectId"] = ViewBag.ProjectId;
                }
                ViewBag.LeftPanelwidth = ConfigurationManager.AppSettings["DefultLeftPanel"];
                var activePinList = accrepo.ActivePinListByUserId((long)SessionManager.TESTER_ID);
                ViewBag.activePinList = JsonConvert.SerializeObject(activePinList);
                var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(gridlst);

                ViewBag.LeftPanelwidth = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] : Rgriddata.Resize.Trim();

                Session["PrivilegeList"] = _etlrepository.GetRolePrivilege((long)SessionManager.TESTER_ID);
                Session["RoleList"] = _etlrepository.GetRoleByUser((long)SessionManager.TESTER_ID);
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Home page | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Home page | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return View();
        }

        #region Right Side Display PqGrid
        public ActionResult PartialRightStoryboardGrid(int Projectid = 0, int Storyboardid = 0)
        {
            try
            {
                logger.Info(string.Format("open storyborad start | Projectid: {0} | Storyboardid: {1} | Username: {2}", Projectid, Storyboardid, SessionManager.TESTER_LOGIN_NAME));
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                ViewBag.Accesstoken = SessionManager.Accesstoken;
                ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebApiURL"];
                ViewBag.Title = "Home Page";
                if (Projectid == 0 && Storyboardid == 0)
                {
                    ViewBag.ProjectId = Projectid;
                    ViewBag.StoryBoardId = Storyboardid;
                    ViewBag.Storyboardname = "0";
                }
                else
                {
                    ViewBag.ProjectId = Projectid;
                    ViewBag.StoryBoardId = Storyboardid;
                    ViewBag.Storyboardname = repo.GetStoryboardById(Storyboardid);

                    Session["ProjectId"] = ViewBag.ProjectId;
                    Session["StoryBoardId"] = ViewBag.StoryBoardId;
                }

                var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repacc.GetGridList((long)userid, GridNameList.StoryboradPage);
                var SPgriddata = GridHelper.GetStoryboardPqgridwidth(gridlst);
                var Widthgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.actionwidth = SPgriddata.Action == null ? "70" : SPgriddata.Action.Trim();
                ViewBag.stepswidth = SPgriddata.Steps == null ? "100" : SPgriddata.Steps.Trim();
                ViewBag.testsuitewidth = SPgriddata.TestSuite == null ? "180" : SPgriddata.TestSuite.Trim();
                ViewBag.testcasewidth = SPgriddata.TestCase == null ? "180" : SPgriddata.TestCase.Trim();
                ViewBag.datasetwidth = SPgriddata.Dataset == null ? "180" : SPgriddata.Dataset.Trim();
                ViewBag.bresultwidth = SPgriddata.BResult == null ? "75" : SPgriddata.BResult.Trim();
                ViewBag.berrorcausewidth = SPgriddata.BErrorCause == null ? "75" : SPgriddata.BErrorCause.Trim();
                ViewBag.bscriptstartwidth = SPgriddata.BScriptStart == null ? "75" : SPgriddata.BScriptStart.Trim();
                ViewBag.bscriptdurationwidth = SPgriddata.BScriptDuration == null ? "75" : SPgriddata.BScriptDuration.Trim();
                ViewBag.cresultwidth = SPgriddata.CResult == null ? "75" : SPgriddata.CResult.Trim();
                ViewBag.cerrorcausewidth = SPgriddata.CErrorCause == null ? "75" : SPgriddata.CErrorCause.Trim();
                ViewBag.cscriptstartwidth = SPgriddata.CScriptStart == null ? "75" : SPgriddata.CScriptStart.Trim();
                ViewBag.cscriptdurationwidth = SPgriddata.CScriptDuration == null ? "75" : SPgriddata.CScriptDuration.Trim();
                ViewBag.dependencywidth = SPgriddata.Dependency == null ? "50" : SPgriddata.Dependency.Trim();
                ViewBag.descriptionwidth = SPgriddata.Dependency == null ? "100" : SPgriddata.Dependency.Trim();

                logger.Info(string.Format("successfully open storyborad | Projectid: {0} | Storyboardid: {1} | Username: {2}", Projectid, Storyboardid, SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Storyborad | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Storyborad | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView("PartialRightStoryboardGrid");
        }
        public ActionResult RightSideGridView(int TestcaseId = 0, int TestsuiteId = 0, int ProjectId = 0, string VisibleDataset = "")
        {
            try
            {
                logger.Info(string.Format("open Testcase start | Projectid: {0} | TestsuiteId: {1} | TestcaseId: {2} | Username: {3}", ProjectId, TestsuiteId, TestcaseId, SessionManager.TESTER_LOGIN_NAME));
                ViewBag.Accesstoken = SessionManager.Accesstoken;
                ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebApiURL"];
                ViewBag.Title = "Home Page";
                var lRep = new TestCaseRepository();
                lRep.Username = SessionManager.TESTER_LOGIN_NAME;
                if (TestcaseId == 0 && TestsuiteId == 0 && ProjectId == 0)
                {
                    ViewBag.TestcaseId = TestcaseId;
                    ViewBag.TestsuiteId = TestsuiteId;
                    ViewBag.ProjectId = ProjectId;
                    ViewBag.TestCaseName = "0";
                    ViewBag.VisibleDataset = "";
                }
                else
                {
                    ViewBag.TestcaseId = TestcaseId;
                    ViewBag.TestsuiteId = TestsuiteId;
                    ViewBag.VisibleDataset = VisibleDataset;
                    ViewBag.ProjectId = ProjectId;
                    ViewBag.TestCaseName = lRep.GetTestCaseNameById(TestcaseId);
                    Session["TestsuiteId"] = ViewBag.TestsuiteId;
                    Session["TestcaseId"] = ViewBag.TestcaseId;
                    Session["ProjectId"] = ViewBag.ProjectId;
                }
                var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repacc.GetGridList((long)userid, GridNameList.TestCasePage);
                var TCPgriddata = GridHelper.GetTestCasePqgridwidth(gridlst);
                var Widthgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.keywordwidth = TCPgriddata.Keyword == null ? "200" : TCPgriddata.Keyword.Trim();
                ViewBag.objectwidth = TCPgriddata.Object == null ? "200" : TCPgriddata.Object.Trim();
                ViewBag.parameterswidth = TCPgriddata.Parameters == null ? "100" : TCPgriddata.Parameters.Trim();
                ViewBag.commentwidth = TCPgriddata.Comment == null ? "100" : TCPgriddata.Comment.Trim();
                logger.Info(string.Format("open Testcase close | Projectid: {0} | TestsuiteId: {1} | TestcaseId: {2} | Username: {3}", ProjectId, TestsuiteId, TestcaseId, SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("successfully open Testcase | Projectid: {0} | TestsuiteId: {1} | TestcaseId: {2} | Username: {3}", ProjectId, TestsuiteId, TestcaseId, SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Testcase | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView("RightSideGridView");
        }
        #endregion

        public JsonResult GetTSTCDSName(string TestCasename, string TestSuitname, string Datasetname)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lRep = new TestCaseRepository();
                var result = lRep.GetTSTCDSId(TestCasename, TestSuitname, Datasetname);
                string[] fresult = result.Split(',');

                resultModel.data = fresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSBBreadcum(string lProjectId, string lStoryboardId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repProj = new TestProjectRepository();
                var repstory = new StoryBoardRepository();
                repstory.Username = SessionManager.TESTER_LOGIN_NAME;
                repProj.Username = SessionManager.TESTER_LOGIN_NAME;
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

                resultModel.data = lResult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Storyboard | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Storyboard | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Test()
        {
            return PartialView();
        }
        public ActionResult TestProjectList()
        {
            return PartialView();
        }
    }
}

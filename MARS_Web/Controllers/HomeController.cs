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
using System.Web.Script.Serialization;

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
                logger.Error(string.Format("Error occured in Home for Index method | TestCase Id : {0} | TestSuite Id : {1} | Project Id : {2} | UserName: {3}", TestcaseId, TestsuiteId, ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Home for Index method | TestCase Id : {0} | TestSuite Id : {1} | Project Id : {2} | UserName: {3}", TestcaseId, TestsuiteId, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Home for Index method | TestCase Id : {0} | TestSuite Id : {1} | Project Id : {2} | UserName: {3}", TestcaseId, TestsuiteId, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
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
                var lAppList = repo.GetApplicationListByStoryboardId(Storyboardid);
                ViewBag.applicationlst = JsonConvert.SerializeObject(lAppList);
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
                
                var result = repo.GetActions(Storyboardid);
                ViewBag.ActionList = JsonConvert.SerializeObject(result);
                var testSuiteResult = repo.GetTestSuites(Projectid);
                ViewBag.TestSuitesList = JsonConvert.SerializeObject(testSuiteResult);
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
                logger.Error(string.Format("Error occured in Home for PartialRightStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Home for PartialRightStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Home for PartialRightStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("PartialRightStoryboardGrid");
        }
        public ActionResult RightSideGridView(int TestcaseId = 0, int TestsuiteId = 0, int ProjectId = 0, string VisibleDataset = "", int storyboradId = 0, string storyboardname = "")
        {
            try
            {
                logger.Info(string.Format("open Testcase start | Projectid: {0} | TestsuiteId: {1} | TestcaseId: {2} | Username: {3}", ProjectId, TestsuiteId, TestcaseId, SessionManager.TESTER_LOGIN_NAME));
                ViewBag.Accesstoken = SessionManager.Accesstoken;
                ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebApiURL"];
                ViewBag.Title = "Home Page";
                var lRep = new TestCaseRepository();
                var lKeyRepo = new KeywordRepository();
                lRep.Username = SessionManager.TESTER_LOGIN_NAME;
                var repObject = new ObjectRepository();
                repObject.Username = SessionManager.TESTER_LOGIN_NAME;
                JavaScriptSerializer js = new JavaScriptSerializer();
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
                    ViewBag.TCstoryboradId = storyboradId;
                    ViewBag.TCstoryboardname = storyboardname;
                    ViewBag.TestcaseId = TestcaseId;
                    ViewBag.TestsuiteId = TestsuiteId;
                    ViewBag.VisibleDataset = VisibleDataset;
                    ViewBag.ProjectId = ProjectId;
                    ViewBag.TestCaseName = lRep.GetTestCaseNameById(TestcaseId);
                    ViewBag.TCApplicationList = js.Serialize(lRep.GetApplicationListByTestcaseId(TestcaseId));
                    Session["TestsuiteId"] = ViewBag.TestsuiteId;
                    Session["TestcaseId"] = ViewBag.TestcaseId;
                    Session["ProjectId"] = ViewBag.ProjectId;
                }
                //Start - Put keywords in a viewbag

                var lKeywordList = new List<string>();
                lKeywordList.Add("pegwindow");
                lKeywordList.Add("dbcompare");
                lKeywordList.Add("copyexcelrangetoclipboard");
                lKeywordList.Add("executecommand");
                lKeywordList.Add("killapplication");
                lKeywordList.Add("loop");
                lKeywordList.Add("resumenext");
                lKeywordList.Add("startapplication");
                lKeywordList.Add("waitforseconds");

                var keywordsResult = lKeyRepo.GetKeywords().Select(y => new KeywordList
                {
                    KeywordName = y.KEY_WORD_NAME
                }).ToList();
                var keywordsPegWindow = keywordsResult.Where(x => lKeywordList.Contains(x.KeywordName.ToLower().Trim())).ToList();
                ViewBag.KeywordsList = JsonConvert.SerializeObject(keywordsResult);
                ViewBag.KeywordsPegwindowList = JsonConvert.SerializeObject(keywordsPegWindow);

                var lList = repObject.GetObjectsByPegWindowType(TestcaseId).OrderBy(y => y.ObjectName).ToList();
                ViewBag.ObjectList = JsonConvert.SerializeObject(lList);

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
                logger.Error(string.Format("Error occured in Home for RightSideGridView method | TestCase Id : {0} | TestSuite Id : {1} | Project Id : {2} | UserName: {3}", TestcaseId, TestsuiteId, ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Home for RightSideGridView method | TestCase Id : {0} | TestSuite Id : {1} | Project Id : {2} | UserName: {3}", TestcaseId, TestsuiteId, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Home for RightSideGridView method | TestCase Id : {0} | TestSuite Id : {1} | Project Id : {2} | UserName: {3}", TestcaseId, TestsuiteId, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
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
                lRep.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = lRep.GetTSTCDSId(TestCasename, TestSuitname, Datasetname);
                string[] fresult = result.Split(',');

                resultModel.data = fresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Home for GetTSTCDSName method | TestCase Name : {0} | TestSuite Name : {1} | DataSet Name : {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Home for GetTSTCDSName method | TestCase Name : {0} | TestSuite Name : {1} | DataSet Name : {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Home for GetTSTCDSName method | TestCase Name : {0} | TestSuite Name : {1} | DataSet Name : {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
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
                logger.Error(string.Format("Error occured in Home for GetSBBreadcum method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", lStoryboardId, lProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Home for GetSBBreadcum method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", lStoryboardId, lProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Home for GetSBBreadcum method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", lStoryboardId, lProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
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

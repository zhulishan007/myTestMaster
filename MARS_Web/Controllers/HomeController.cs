using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using MarsSerializationHelper.ViewModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static MarsSerializationHelper.JsonSerialization.SerializationFile;

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
                TestCaseRepository lRep = new TestCaseRepository();
                //AccountRepository accrepo = new AccountRepository();
                //StoryBoardRepository repo = new StoryBoardRepository();
                //GetTreeRepository _treerepository = new GetTreeRepository();
                //EntitlementRepository _etlrepository = new EntitlementRepository
                //{
                //    Username = SessionManager.TESTER_LOGIN_NAME
                //};
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
                //ViewBag.LeftPanelwidth = ConfigurationManager.AppSettings["DefultLeftPanel"];

                //var activePinList = accrepo.ActivePinListByUserId((long)SessionManager.TESTER_ID);
                //ViewBag.activePinList = JsonConvert.SerializeObject(activePinList);

                var userid = SessionManager.TESTER_ID;
                ViewBag.LeftPanelwidth = ConfigurationManager.AppSettings["DefultLeftPanel"];

                //var repacc = new ConfigurationGridRepository();
                //repacc.Username = SessionManager.TESTER_LOGIN_NAME;                
                //var gridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                //var Rgriddata = GridHelper.GetLeftpanelgridwidth(gridlst);
                //ViewBag.LeftPanelwidth = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] : Rgriddata.Resize.Trim();

                //Session["PrivilegeList"] = _etlrepository.GetRolePrivilege((long)SessionManager.TESTER_ID);
                //Session["RoleList"] = _etlrepository.GetRoleByUser((long)SessionManager.TESTER_ID);
                //Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                var userData = GlobalVariable.UsersDictionary.FirstOrDefault(x => x.Key.Trim().ToUpper().Equals(lSchema.Trim().ToUpper())).Value.FirstOrDefault(y => y.Key.TESTER_ID == userid).Key;
                if (Session["LeftProjectList"] == null)
                    Session["LeftProjectList"] = userData.Projects.Where(x => x.ProjectExists == true).ToList();
                Session["RoleList"] = userData.Roles.Select(x => new RoleViewModel() { ROLE_ID = x.ROLE_ID, ROLE_NAME = x.ROLE_NAME }).ToList();
                Session["PrivilegeList"] = userData.Privileges.Select(x => new PrivilegeViewModel() { PRIVILEGE_ID = x.PRIVILEGE_ID, DESCRIPTION = x.DESCRIPTION, MODULE = x.MODULE, PRIVILEGE_NAME = x.PRIVILEGE_NAME }).ToList();
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
                var _object = new ObjectRepository();
                var lKeyRepo = new KeywordRepository();
                lRep.Username = SessionManager.TESTER_LOGIN_NAME;
                var repObject = new ObjectRepository();
                repObject.Username = SessionManager.TESTER_LOGIN_NAME;
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<long> appId = new List<long>();
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

                    #region GET TEST CASE NAME FROM MAPPING FILE
                    logger.Info(string.Format("GET TEST CASE NAME FROM MAPPING FILE | START | START TIME : {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));

                    string testCaseName = string.Empty;
                    string directoryPath = Path.Combine(Server.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Testcases.ToString(), SessionManager.Schema);
                    string[] filesName = new string[0];
                    if (Directory.Exists(directoryPath))
                    {
                        filesName = Directory.GetFiles(directoryPath);
                        filesName = filesName.Select(x => Path.GetFileName(x)).ToArray();
                        testCaseName = filesName.FirstOrDefault(x => x.StartsWith(TestcaseId.ToString()));
                        if (string.IsNullOrEmpty(testCaseName))
                            testCaseName = lRep.GetTestCaseNameById(TestcaseId);
                    }
                    else
                        testCaseName = lRep.GetTestCaseNameById(TestcaseId);
                    directoryPath = Path.Combine(directoryPath, testCaseName);
                    testCaseName = testCaseName.Replace(string.Format("{0}_", TestcaseId), string.Empty).Replace(".json", string.Empty);
                    ViewBag.TestCaseName = testCaseName;

                    logger.Info(string.Format("GET TEST CASE NAME FROM MAPPING FILE | END | END TIME : {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                    #endregion

                    //ViewBag.TestCaseName = lRep.GetTestCaseNameById(TestcaseId);
                    Mars_Memory_TestCase allList = new Mars_Memory_TestCase();
                    string testcaseSessionName = string.Format("{0}_Testcase_{1}", SessionManager.Schema, TestcaseId);
                    if (Session[testcaseSessionName] != null)
                        allList = Session[testcaseSessionName] as Mars_Memory_TestCase;
                    else
                    {
                        if (System.IO.File.Exists(directoryPath))
                        {
                            logger.Info(string.Format("GET TESTCASE DETAILS FROM THE MAPPING FILE | START | START TIME: {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                            string jsongString = System.IO.File.ReadAllText(directoryPath);
                            allList = JsonConvert.DeserializeObject<Mars_Memory_TestCase>(jsongString);
                            Session[testcaseSessionName] = allList;
                            logger.Info(string.Format("GET TESTCASE DETAILS FROM THE MAPPING FILE | END | END TIME: {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                        }
                        else
                        {
                            logger.Info(string.Format("GET TESTCASE DETAILS FROM THE DATABASE | START | START TIME: {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                            string fullFilePath = CreateTestcaseFolder();
                            bool status = LoadTestcaseJsonFile(fullFilePath, new List<MB_V_TEST_STEPS>(), TestcaseId, SessionManager.APP.ToString());
                            if (status)
                            {
                                string jsongString = System.IO.File.ReadAllText(directoryPath);
                                allList = JsonConvert.DeserializeObject<Mars_Memory_TestCase>(jsongString);
                                Session[testcaseSessionName] = allList;
                            }
                            logger.Info(string.Format("GET TESTCASE DETAILS FROM THE DATABASE | END | END TIME: {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                        }
                    }

                    if (allList != null)
                    {
                        appId = allList.assignedApplications.Where(x => x > 0).ToList();                       
                        var applications = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Trim().Equals(SessionManager.Schema)).Value;
                        if (applications.Count() > 0)
                        {
                            var testcaseApp = applications.Where(x => appId.Contains(x.APPLICATION_ID)).Select(y => new ApplicationModel()
                            {
                                ApplicationId = y.APPLICATION_ID,
                                ApplicationName = y.APP_SHORT_NAME
                            }).ToList();
                            ViewBag.TCApplicationList = js.Serialize(testcaseApp);
                        }
                    }
                    else
                    {
                        List<ApplicationModel> AppList = lRep.GetApplicationListByTestcaseId(TestcaseId);
                        appId = AppList.Select(x => x.ApplicationId).ToList();
                        ViewBag.TCApplicationList = js.Serialize(AppList);
                    }

                    Mars_Memory_TestCase finalList = new Mars_Memory_TestCase
                    {
                        assignedApplications = allList.assignedApplications,
                        assignedDataSets = allList.assignedDataSets,
                        assignedTestSuiteIDs = allList.assignedTestSuiteIDs,
                        currentSyncroStatus = allList.currentSyncroStatus,
                        version = allList.version,
                        allSteps = allList.allSteps.Where(x => x.recordStatus != MarsSerializationHelper.Common.CommonEnum.MarsRecordStatus.en_DeletedToDb).OrderBy(y => y.RUN_ORDER).ToList()
                    };
                    List<TestCaseResult> newResult = new List<TestCaseResult>();
                    newResult = lRep.ConvertTestcaseJsonToList(finalList, TestcaseId, SessionManager.Schema, SessionManager.APP, (long)SessionManager.TESTER_ID);
                    ViewBag.TestcaseData = newResult; //js.Serialize(newResult);

                    Session["TestsuiteId"] = ViewBag.TestsuiteId;
                    Session["TestcaseId"] = ViewBag.TestcaseId;
                    Session["ProjectId"] = ViewBag.ProjectId;
                }



                //Start - Put keywords in a viewbag
                var lKeywordList = new List<string>
                {
                    "pegwindow",
                    "dbcompare",
                    "copyexcelrangetoclipboard",
                    "executecommand",
                    "killapplication",
                    "loop",
                    "resumenext",
                    "startapplication",
                    "waitforseconds"
                };

                logger.Info(string.Format("GET KEYWORD LIST | START | START TIME : {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                var keywordsResult = lKeyRepo.GetKeywords().Select(y => new KeywordList
                {
                    KeywordName = y.KEY_WORD_NAME,
                    KeywordId = y.KEY_WORD_ID
                }).ToList();
                logger.Info(string.Format("GET KEYWORD LIST | END | END TIME : {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));

                var keywordsPegWindow = keywordsResult.Where(x => lKeywordList.Contains(x.KeywordName.ToLower().Trim())).ToList();
                ViewBag.KeywordsList = JsonConvert.SerializeObject(keywordsResult);
                ViewBag.KeywordsPegwindowList = JsonConvert.SerializeObject(keywordsPegWindow);

                #region RBJ Comment
                //var lList = repObject.GetObjectsByPegWindowType(TestcaseId).OrderBy(y => y.ObjectName).ToList();
                //ViewBag.ObjectList = JsonConvert.SerializeObject(lList);
                #endregion

                #region RBJ Code
                logger.Info(string.Format("GET OBJECT LIST FROM THE FILE | START | START TIME : {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                //appId = repObject.getApplicationIdByTestCaseId(TestcaseId);
                string folderPath = Path.Combine(Server.MapPath("~/"), "Serialization\\Object\\" + SessionManager.Schema);
                bool isThereAllFile = true;
                foreach (var ID in appId)
                {
                    string filePath = Path.Combine(folderPath, string.Format("app_{0}\\PegWindowsMapping.json", ID));
                    if (!System.IO.File.Exists(filePath))
                    {
                        isThereAllFile = false;
                        if (!isThereAllFile)
                            break;
                    }
                }
                List<ObjectList> objList = new List<ObjectList>();
                if (isThereAllFile)
                    objList = repObject.getObjectListByAppIdFromJsonFile(appId, folderPath);
                else
                    objList = repObject.GetObjectsByPegWindowType(TestcaseId).OrderBy(y => y.ObjectName).ToList();

                ViewBag.ObjectList = JsonConvert.SerializeObject(objList);

                ViewBag.AppID = appId.OrderBy(x => x).FirstOrDefault(); //_object.getApplicationIdByTestCaseId(TestcaseId).FirstOrDefault();
                logger.Info(string.Format("GET OBJECT LIST FROM THE FILE | END | END TIME : {0}", DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                #endregion

                //var userid = SessionManager.TESTER_ID;
                //var repacc = new ConfigurationGridRepository();
                //repacc.Username = SessionManager.TESTER_LOGIN_NAME;


                ViewBag.width = ConfigurationManager.AppSettings["DefultLeftPanel"] + "px";
                ViewBag.keywordwidth = "200"; ViewBag.objectwidth = "200"; ViewBag.parameterswidth = "100"; ViewBag.commentwidth = "100";

                #region OLD CODE
                //var gridlst = repacc.GetGridList((long)userid, GridNameList.TestCasePage);
                //var TCPgriddata = GridHelper.GetTestCasePqgridwidth(gridlst);
                //var Widthgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                //var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                //ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                //ViewBag.keywordwidth = TCPgriddata.Keyword == null ? "200" : TCPgriddata.Keyword.Trim();
                //ViewBag.objectwidth = TCPgriddata.Object == null ? "200" : TCPgriddata.Object.Trim();
                //ViewBag.parameterswidth = TCPgriddata.Parameters == null ? "100" : TCPgriddata.Parameters.Trim();
                //ViewBag.commentwidth = TCPgriddata.Comment == null ? "100" : TCPgriddata.Comment.Trim();
                #endregion

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
        protected string CreateTestcaseFolder()
        {
            string mainPath = Server.MapPath("~/");
            string F_Serialization = Path.Combine(mainPath, FolderName.Serialization.ToString());
            string T_Testcases = Path.Combine(F_Serialization, FolderName.Testcases.ToString());
            string D_Database = string.Empty;
            string TestcasePath = string.Empty;

            #region IF NOT EXIST CREATE MAIN SERIALIZATION FOLDER 
            if (!Directory.Exists(F_Serialization))
                Directory.CreateDirectory(F_Serialization);
            #endregion

            if (!Directory.Exists(T_Testcases))
                Directory.CreateDirectory(T_Testcases);

            if (Directory.Exists(T_Testcases))
            {
                D_Database = Path.Combine(T_Testcases, SessionManager.Schema.ToString().Trim());
                if (!Directory.Exists(D_Database))
                    Directory.CreateDirectory(D_Database);
            }
            TestcasePath = D_Database;
            return TestcasePath;
        }
    }
}

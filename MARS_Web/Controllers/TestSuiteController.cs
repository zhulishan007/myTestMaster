using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using NLog;

using MARSUtility;
using MarsSerializationHelper.ViewModel;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class TestSuiteController : Controller
    {
        MARSUtility.CommonHelper objcommon = new MARSUtility.CommonHelper();
        public TestSuiteController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        #region Crud operation for TestSuite

        //Renders partial view
        public ActionResult TestSuiteList()
        {
            try
            {
                ApplicationRepository repo = new ApplicationRepository();
                ProjectRepository prepo = new ProjectRepository();
                prepo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.Username = SessionManager.TESTER_LOGIN_NAME;

                List<T_Memory_REGISTERED_APPS> apps = new List<T_Memory_REGISTERED_APPS>();
                apps = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value;
                if (apps.Count() > 0)
                {
                    ViewBag.listApplications = apps.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).OrderBy(x => x.Text).ToList();
                }

                //var lapp = repo.ListApplication();
                //var applist = lapp.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).OrderBy(x => x.Text).ToList();
                //ViewBag.listApplications = applist;

                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repAcc.GetGridList((long)userId, GridNameList.TestSuiteList);
                var TSgriddata = GridHelper.GetTestSuitewidth(gridlst);
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.namewidth = TSgriddata.Name == null ? "20%" : TSgriddata.Name.Trim() + '%';
                ViewBag.descriptionwidth = TSgriddata.Description == null ? "20%" : TSgriddata.Description.Trim() + '%';
                ViewBag.applicationwidth = TSgriddata.Application == null ? "20%" : TSgriddata.Application.Trim() + '%';
                ViewBag.projectwidth = TSgriddata.Project == null ? "30%" : TSgriddata.Project.Trim() + '%';
                ViewBag.actionswidth = TSgriddata.Actions == null ? "10%" : TSgriddata.Actions.Trim() + '%';
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for TestSuiteList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for TestSuiteList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for TestSuiteList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("TestSuiteList");
        }

        //Get TestSuite list
        [HttpPost]
        public JsonResult DataLoad()
        {
            try
            {
                var repAcc = new TestSuiteRepository();
                //Assign values 
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                startRec = startRec + 1;
                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string DescriptionSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                string ApplicationSearch = Request.Form.GetValues("columns[2][search][value]")[0];
                string ProjectSearch = Request.Form.GetValues("columns[3][search][value]")[0];

                //Gets Test Suite list
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var data = new List<TestSuiteModel>();

                data = repAcc.ListAllTestSuites(lSchema, lConnectionStr, startRec, pageSize, colOrder, orderDir, NameSearch, DescriptionSearch, ApplicationSearch, ProjectSearch);
                //Get count of total records
                int totalRecords = 0;
                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().TotalCount;
                }

                //Get count of filtered records
                int recFilter = 0;
                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().TotalCount;
                }

                //returns data
                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                throw ex;
            }
        }

        //Add/Edit a TestSuite
        public JsonResult AddEditTestSuite(TestSuiteModel lModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var testsuiterepo = new TestSuiteRepository();
                testsuiterepo.Username = SessionManager.TESTER_LOGIN_NAME;
                //checkes if the testsuite is used in any storyboard
                    var result = testsuiterepo.CheckTestSuiteInStoryboardByProject(lModel.TestSuiteId, lModel.ProjectId);
                    if (result.Count > 0)
                        resultModel.data = result;
                    else
                    {
                        var lResult = testsuiterepo.AddEditTestSuite(lModel);
                        var repTree = new GetTreeRepository();
                        repTree.Username = SessionManager.TESTER_LOGIN_NAME;
                        var lSchema = SessionManager.Schema;
                        var lConnectionStr = SessionManager.APP;
                        //refreshes Mars Tree
                        Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                        resultModel.data = lResult;
                        resultModel.message = "Test Suite [" + lModel.TestSuiteName + "] saved successfully.";
                    }
                    resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for AddEditTestSuite method | TestSuiteId : {0} | UserName: {1}", lModel.TestSuiteId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for AddEditTestSuite method | TestSuiteId : {0} | UserName: {1}", lModel.TestSuiteId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for AddEditTestSuite method | TestSuiteId : {0} | UserName: {1}", lModel.TestSuiteId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete Testsuite
        public JsonResult DeleteTestSuite(long TestsuiteId)
        {
            logger.Info(string.Format("Delete TestSuite | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var testSuiterepo = new TestSuiteRepository();
                testSuiterepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var TestSuiteName = string.Empty;
                var lflag = testSuiterepo.CheckTestSuiteExistInStoryboard(TestsuiteId);

                if (lflag.Count <= 0)
                {
                    TestSuiteName = testSuiterepo.GetTestSuiteNameById(TestsuiteId);
                    var lResult = testSuiterepo.DeleteTestSuite(TestsuiteId);
                    Session["TestSuiteDeleteMsg"] = "Successfully TestSuite Deleted.";
                    Session["TestcaseId"] = null;
                    Session["TestsuiteId"] = null;
                    Session["ProjectId"] = null;

                    resultModel.data = lResult;
                    resultModel.message = "Test Suite [" + TestSuiteName + "] deleted Successfully";
                }
                else
                    resultModel.data = lflag;

                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for DeleteTestSuite method | TestSuiteId : {0} | UserName: {1}", TestsuiteId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for DeleteTestSuite method | TestSuiteId : {0} | UserName: {1}", TestsuiteId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for DeleteTestSuite method | TestSuiteId : {0} | UserName: {1}", TestsuiteId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks whether the TestSuite name exists in the system and if not, then renames the testsuite

        //Changes TestSuite name
        public JsonResult ChangeTestSuiteName(string TestSuiteName, string Testsuitedesc, long TestSuiteId)
        {
            logger.Info(string.Format("Rename TestSuite Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var testsuiterepo = new TestSuiteRepository();
                testsuiterepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = testsuiterepo.CheckDuplicateTestSuiteName(TestSuiteName, TestSuiteId);
                if (result)
                    resultModel.data = result;
                else{
                    var lresult = testsuiterepo.ChangeTestSuiteName(TestSuiteName, Testsuitedesc, TestSuiteId);
                    resultModel.message = "Test Suite name successfully changed.";
                    resultModel.data = lresult;
                    logger.Info(string.Format("Rename TestSuite successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                resultModel.status = 1;
                logger.Info(string.Format("Rename TestSuite Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId : {0} |TestSuite Name : {1} | TestSuite Desc : {2} | UserName: {3}", TestSuiteId, TestSuiteName, Testsuitedesc, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId : {0} |TestSuite Name : {1} | TestSuite Desc : {2} | UserName: {3}", TestSuiteId, TestSuiteName, Testsuitedesc, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId : {0} |TestSuite Name : {1} | TestSuite Desc : {2} | UserName: {3}", TestSuiteId, TestSuiteName, Testsuitedesc, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Checks whether TestSuite with the same is present in the system or not
        public JsonResult CheckDuplicateTestSuiteNameExist(string TestSuiteName, long? TestSuiteId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                TestSuiteName = TestSuiteName.Trim();
                var testsuiterepo = new TestSuiteRepository();
                testsuiterepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lresult = testsuiterepo.CheckDuplicateTestSuiteName(TestSuiteName, TestSuiteId);
                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for CheckDuplicateTestSuiteNameExist method | TestSuiteId : {0} |TestSuite Name : {1} | TestSuite Desc : {2} | UserName: {3}", TestSuiteId, TestSuiteName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for CheckDuplicateTestSuiteNameExist method | TestSuiteId : {0} |TestSuite Name : {1} | TestSuite Desc : {2} | UserName: {3}", TestSuiteId, TestSuiteName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for CheckDuplicateTestSuiteNameExist method | TestSuiteId : {0} |TestSuite Name : {1} | TestSuite Desc : {2} | UserName: {3}", TestSuiteId, TestSuiteName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Loads dropdown of Project List after selecting an Application in Add/Edit Test Suite
        public JsonResult GetProjectByApplicaton(string ApplicationId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTestSuite = new ProjectRepository();
                repTestSuite.Username = SessionManager.TESTER_LOGIN_NAME;
                var lResult = new List<RelProjectApplication>();
                if (!string.IsNullOrEmpty(ApplicationId))
                {
                    lResult = repTestSuite.ListRelProjectApp(ApplicationId);
                }
                resultModel.data = lResult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for GetProjectByApplicaton method | Application Id : {0} | UserName: {1}", ApplicationId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for GetProjectByApplicaton method | Application Id : {0} | UserName: {1}", ApplicationId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for GetProjectByApplicaton method | Application Id : {0} | UserName: {1}", ApplicationId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export and Import of TestSuite/s

        //Renders Partial view
        public ActionResult ImportTestSuite()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch(Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ImportTestSuite method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ImportTestSuite method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ImportTestSuite method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //Imports a TestSuite from the system to the Mars App
        [HttpPost]
        public ActionResult ImportTestSuites()
        {
            ViewBag.FileName = "";
            string fileName = string.Empty;
            string name = "Log_Import" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ImportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                dbtable.dt_Log = null;
                DataTable td = new DataTable();
                DataColumn sequence_counter = new DataColumn();
                sequence_counter.AutoIncrement = true;
                sequence_counter.AutoIncrementSeed = 1;
                sequence_counter.AutoIncrementStep = 1;

                td.Columns.Add(sequence_counter);
                td.Columns.Add("TimeStamp");
                td.Columns.Add("Message Type");
                td.Columns.Add("Action");

                td.Columns.Add("SpreadSheet cell Address");
                td.Columns.Add("Validation Name");
                td.Columns.Add("Validation Fail Description");
                td.Columns.Add("Application Name");
                td.Columns.Add("Project Name");
                td.Columns.Add("StoryBoard Name");

                td.Columns.Add("Test Suite Name");

                td.Columns.Add("TestCase Name");
                td.Columns.Add("Test step Number");

                td.Columns.Add("Dataset Name");
                td.Columns.Add("Dependancy");
                td.Columns.Add("Run Order");


                td.Columns.Add("Object Name");
                td.Columns.Add("Comment");
                td.Columns.Add("Error Description");
                td.Columns.Add("Program Location");
                td.Columns.Add("Tab Name");

                dbtable.dt_Log = td.Copy();
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase testsuiteUpload = files[i];
                    if (testsuiteUpload != null)
                    {
                       
                        string destinationPath = string.Empty;
                        string extension = string.Empty;

                        var uploadFileModel = new List<TestSuiteFileUploadModel>();

                        fileName = Path.GetFileNameWithoutExtension(testsuiteUpload.FileName);
                        extension = Path.GetExtension(testsuiteUpload.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                        destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                        testsuiteUpload.SaveAs(destinationPath);
                        //string TempFileLocation = WebConfigurationManager.AppSettings["TemplateLocation"];

                        string lSchema =SessionManager.Schema;
                        var lConnectionStr = SessionManager.APP;
                        MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                        dbtable.errorlog("Import is started", "Import TestCase", "", 0);

                        var lPath = helper.MasterImport(0, destinationPath, strPath, "TESTCASE", 1, "", "", "", 1, lSchema, lConnectionStr);

                        if (lPath==false)
                        {
                            dbtable.errorlog("Import is stopped", "Import TestCase", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "TESTCASE");
                            return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dbtable.errorlog("Import is completed", "Import TestCase", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "TESTCASE");
                            return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error(string.Format("Error occured in TestSuite for ImportTestSuites method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ImportTestSuites method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ImportTestSuites method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import Testcase", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "TESTCASE");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);
            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        //Downloads the file after export
        public FileResult DownloadFile(string path)
        {
            ViewBag.FileName = "";
            var result = "";
            var byteArray = System.IO.File.ReadAllBytes(path);
            var ms = new MemoryStream(byteArray);
            result = Path.GetFileName(path);
            return new FileStreamResult(ms, "application/ms-excel")
            {
                FileDownloadName = result
            };
        }

        //Exports all the TestSuites present in the system
        //public FileStreamResult ExportAllTestSuites()
        //{
        //    string FullPath = "";
        //    var exportExcel = new ExportHelper();

        //    string TempFileLocation = WebConfigurationManager.AppSettings["StoryBoardTemplateLocation"];
        //    //string luser = WebConfigurationManager.AppSettings["User"];
        //    //string lpassword = WebConfigurationManager.AppSettings["Password"];
        //    //string ldataSource = WebConfigurationManager.AppSettings["DataSource"];
        //    //string lSchema = WebConfigurationManager.AppSettings["Schema"];
        //    //var lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";
        //    string lSchema = SessionManager.Schema;
        //    var lConnectionStr = SessionManager.APP;
        //   // FullPath = exportExcel.ExportAllTestSuites(lConnectionStr, lSchema, AppDomain.CurrentDomain.BaseDirectory + "" + TempFileLocation);

        //    var byteArray = System.IO.File.ReadAllBytes(FullPath);

        //    var ms = new MemoryStream(byteArray);
        //    return new FileStreamResult(ms, "application/ms-excel")
        //    {
        //        FileDownloadName = "TestSuites" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xls"
        //    };
        //}

        //Export All TestCases of selected TestSuite by TestSuiteId
        public JsonResult ExportTestSuite(int TestSuiteId)
        {
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            MARSUtility.dbtable.dt_Log = null;
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
            sequence_counter.AutoIncrement = true;
            sequence_counter.AutoIncrementSeed = 1;
            sequence_counter.AutoIncrementStep = 1;

            td.Columns.Add(sequence_counter);
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");

            td.Columns.Add("Test Suite Name");


            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");

            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");


            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");



            MARSUtility.dbtable.dt_Log = td.Copy();
            if (TestSuiteId > 0)
            {
                var testSuiterepo = new TestSuiteRepository();
                var TestSuiteName = testSuiterepo.GetTestSuiteNameById(Convert.ToInt64(TestSuiteId));

                MARSUtility.ExportExcel excel = new MARSUtility.ExportExcel();
                MARSUtility.ExportHelper exphelper = new MARSUtility.ExportHelper();
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                string lFileName = TestSuiteName + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                 name = "Log_" + TestSuiteName+"_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
                 strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                try
                {
                  
                    MARSUtility.dbtable.errorlog("Export is started", "Export suite Excel", "", 0);
                    string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                   var presult = exphelper.ExportTestSuite(TestSuiteName, FullPath, lSchema, lConnectionStr);

                    if (presult == true)
                    {
                        MARSUtility.dbtable.errorlog("Export is completed", "Export Test suite Excel", "", 0);
                        objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", TestSuiteName, "TESTCASE");
                        MARSUtility.dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error occured in TestSuite for ExportTestSuite method | TestSuiteId : {0} | UserName: {1}", TestSuiteId, SessionManager.TESTER_LOGIN_NAME));
                    ELogger.ErrorException(string.Format("Error occured in TestSuite for ExportTestSuite method | TestSuiteId : {0} | UserName: {1}", TestSuiteId, SessionManager.TESTER_LOGIN_NAME), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ExportTestSuite method | TestSuiteId : {0} | UserName: {1}", TestSuiteId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                    int line;
                    string msg = ex.Message;
                    line = MARSUtility.dbtable.lineNo(ex);
                    MARSUtility.dbtable.errorlog("Export stopped", "Export Test Suite Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", TestSuiteName, "TESTCASE");
                    MARSUtility.dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }
                return Json(lFileName, JsonRequestBehavior.AllowGet);
            }
            MARSUtility.dbtable.errorlog("Test Suite Id is invalid/Test suite not found in the system", "Export Test Suite Excel", "", 0);
            objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "TESTCASE");
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult DownloadExcel(string FileName)
        {
            try
            {
                var byteArray =  System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/TempExport"), FileName));
                var ms = new MemoryStream(byteArray);
                return new FileStreamResult(ms, "application/ms-excel")
                {
                    FileDownloadName = FileName // + ".xlsx"
                };
            }
            catch(Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for DownloadExcel method | FileName : {0} | UserName: {1}", FileName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for DownloadExcel method | FileName : {0} | UserName: {1}", FileName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for DownloadExcel method | FileName : {0} | UserName: {1}", FileName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                var ms = new MemoryStream();
                return new FileStreamResult(ms, "application/ms-excel");
            }
        }

        public class TestSuiteFileUploadModel
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        #endregion
    }
}

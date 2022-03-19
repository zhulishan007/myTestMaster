using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.HUB;
using Microsoft.AspNet.SignalR;
using MARS_Web.Helper;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MARS_Repository.Entities;
using System.Text.RegularExpressions;
using NLog;
using MARSUtility;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static Mars_Serialization.JsonSerialization.SerializationFile;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class StoryBoardController : Controller
    {
        MARSUtility.CommonHelper objcommon = new MARSUtility.CommonHelper();
        public static bool IsNotified = false;
        public StoryBoardController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        #region Crud operation for Storyboard
        public ActionResult StoryboardList()
        {
            try
            {
                ProjectRepository repo = new ProjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.ListProject();
                var projectlist = result.Select(c => new SelectListItem { Text = c.PROJECT_NAME, Value = c.PROJECT_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.Projectname = projectlist;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for StoryBoardList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for StoryBoardList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for StoryBoardList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //Add/Update Storyboard objects values
        public ActionResult AddEditStoryboard(StoryboardModel model)
        {
            logger.Info(string.Format("Satoryboard Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var _treerepository = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                var checkduplicateflag = repo.CheckDuplicateStoryboardName(model.Storyboardname, model.Storyboardid);
                if (checkduplicateflag)
                    resultModel.data = checkduplicateflag;
                else
                {
                    var result = repo.AddEditStoryboard(model);
                     
                    var entityConnectString = SessionManager.ConnectionString;
                    InitCacheHelper.StoryBoardInit(entityConnectString, lSchema, _treerepository);

                    Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                    var flag = model.Storyboardid == 0 ? "Added" : "Updated";
                    resultModel.message = "Storyboard " + flag + " successfully";
                    resultModel.data = result;
                    logger.Info(string.Format("Satoryboard " + flag + " successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                resultModel.status = 1;
                logger.Info(string.Format("Satoryboard Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for AddEditStoryboard method | StoryBoard Id : {0} | UserName: {1}", model.Storyboardid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for AddEditStoryboard method | StoryBoard Id : {0} | UserName: {1}", model.Storyboardid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for AddEditStoryboard method | StoryBoard Id : {0} | UserName: {1}", model.Storyboardid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoad()
        {
            StoryBoardRepository repo = new StoryBoardRepository();
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

            string Storyboardsnamesearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string Storyboarddescsearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string Projectnamesearch = Request.Form.GetValues("columns[2][search][value]")[0];

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
            var data = repo.GetStoryboards(lConnectionStr, lSchema, startRec, pageSize, Storyboardsnamesearch, Storyboarddescsearch, Projectnamesearch, colOrder, orderDir);
            int totalRecords = 0;
            if (data.Count() > 0)
            {
                totalRecords = data.FirstOrDefault().TotalCount;
            }
            int recFilter = 0;
            if (data.Count() > 0)
            {
                recFilter = data.FirstOrDefault().TotalCount;
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Delete the Storyboard object data by StoryboardID
        public ActionResult DeleteStoryboard(long sid)
        {
            logger.Info(string.Format("Delete Storyboard start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var repTree = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var storyboradname = repo.GetStoryboardNamebyId(sid);
                var result = repo.DeleteStoryboard(sid);

                var entityConnectString = SessionManager.ConnectionString;
                InitCacheHelper.StoryBoardInit(entityConnectString, lSchema, repTree);

                Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                logger.Info(string.Format("Delete Storyboard successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));

                resultModel.message = "Storyboard [" + storyboradname + "] deleted Successfully";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for DeleteStoryboard method | StoryBoard Id : {0} | UserName: {1}", sid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for DeleteStoryboard method | StoryBoard Id : {0} | UserName: {1}", sid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for DeleteStoryboard method | StoryBoard Id : {0} | UserName: {1}", sid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks whether Storyboard name already exists in the system and If not, then renames it
        public JsonResult CheckDuplicateStoryboardName(string storyboardname, long? storyboardid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                storyboardname = storyboardname.Trim();
                var repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDuplicateStoryboardName(storyboardname, storyboardid);
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for CheckDuplicateStoryboardName method | StoryBoard Id : {0} | StoryBoard Name : {1} | UserName: {2}", storyboardid, storyboardname, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for CheckDuplicateStoryboardName method | StoryBoard Id : {0} | StoryBoard Name : {1} | UserName: {2}", storyboardid, storyboardname, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for CheckDuplicateStoryboardName method | StoryBoard Id : {0} | StoryBoard Name : {1} | UserName: {2}", storyboardid, storyboardname, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChangeStoryboardName(string storyboardname, string storyboarddesc, long storyboardid, long projectid)
        {
            logger.Info(string.Format("Rename Storyboard Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var controller = new LoginController();
                var checksb = repo.CheckDuplicateStoryboardName(storyboardname, storyboardid);
                if (checksb)
                {
                    resultModel.message = "Storyboard [" + storyboardname + "] already exists";
                    resultModel.data = checksb;
                    resultModel.status = 0;
                    logger.Info(string.Format("Storyboard exists in the system | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    var result = repo.ChangeStoryboardName(storyboardname, storyboarddesc, storyboardid);

                    resultModel.message = "Storyboard name successfully changed.";
                    resultModel.data = result;
                    resultModel.status = 1;

                    logger.Info(string.Format("Rename Storyboard  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Rename Storyboard successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ChangeStoryboardName method | StoryBoard Id : {0} | StoryBoard Name : {1} | StoryBoard Desc : {2} | Project Id : {3} | UserName: {4}", storyboardid, storyboardname, storyboarddesc, projectid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ChangeStoryboardName method | StoryBoard Id : {0} | StoryBoard Name : {1} | StoryBoard Desc : {2} | Project Id : {3} | UserName: {4}", storyboardid, storyboardname, storyboarddesc, projectid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ChangeStoryboardName method | StoryBoard Id : {0} | StoryBoard Name : {1} | StoryBoard Desc : {2} | Project Id : {3} | UserName: {4}", storyboardid, storyboardname, storyboarddesc, projectid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export and Import of storyboard/s
        public JsonResult ExportStoryboard(int Storyboardid, int Projectid)
        {
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            var helper = new MARSUtility.ExportExcel();
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
            if (Storyboardid > 0 && Projectid > 0)
            {
                var projrepo = new ProjectRepository();
                var Projectname = projrepo.GetProjectNameById(Convert.ToInt64(Projectid));
                var repo = new StoryBoardRepository();
                var Storyboardname = repo.GetStoryboardNameById(Convert.ToInt64(Storyboardid));

                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                Regex re = new Regex("[;\\/:*?\"<>|&']");
                var SBName = re.Replace(Storyboardname, "_").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");

                string lFileName = SBName + "_" + Projectname + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                name = "Log_" + SBName + "_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";

                strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                try
                {
                    MARSUtility.dbtable.errorlog("Export is started", "Export Storyboard Excel", "", 0);
                    var result = helper.ExportStoryboardByProject(Projectname, Storyboardname, FullPath, lSchema, lConnectionStr);
                    if (result == true)
                    {
                        MARSUtility.dbtable.errorlog("Export is completed", "Export storyboard Excel", "", 0);
                        objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
                        MARSUtility.dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error occured in StoryBoard for ExportStoryboard method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, SessionManager.TESTER_LOGIN_NAME));
                    ELogger.ErrorException(string.Format("Error occured in StoryBoard for ExportStoryboard method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, SessionManager.TESTER_LOGIN_NAME), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ExportStoryboard method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                    int line;
                    string msg = ex.Message;
                    line = MARSUtility.dbtable.lineNo(ex);
                    MARSUtility.dbtable.errorlog("Export stopped", "Export Storyboard Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
                    MARSUtility.dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }
                return Json(lFileName, JsonRequestBehavior.AllowGet);
            }
            MARSUtility.dbtable.errorlog("StoryboardId/ProjectId is invalid.", "Export Storyboard Excel", "", 0);
            objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportStoryboardsByProject(int projectid)
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
            if (projectid > 0)
            {
                var prepo = new ProjectRepository();
                MARSUtility.ExportExcel exp = new MARSUtility.ExportExcel();
                var projectname = prepo.GetProjectNameById(Convert.ToInt64(projectid));
                string lFileName = projectname + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                name = "Log_" + projectname + "_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
                strPath = Path.Combine(Server.MapPath("~/" + log_path), name);

                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                try
                {

                    MARSUtility.dbtable.errorlog("Export is started", "Export Storyboard", "", 0);
                    var presult = exp.ExportStoryboardExcel(projectname, FullPath, lSchema, lConnectionStr);
                    if (presult == true)
                    {
                        MARSUtility.dbtable.errorlog("Export is completed", "Export storyboard Excel", "", 0);
                        objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", projectname, "STORYBOARD");
                        MARSUtility.dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error occured in StoryBoard for ExportStoryboardsByProject method | Project Id : {0} | UserName: {1}", projectid, SessionManager.TESTER_LOGIN_NAME));
                    ELogger.ErrorException(string.Format("Error occured in StoryBoard for ExportStoryboardsByProject method | Project Id : {0} | UserName: {1}", projectid, SessionManager.TESTER_LOGIN_NAME), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ExportStoryboardsByProject method | Project Id : {0} | UserName: {1}", projectid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                    int line;
                    string msg = ex.Message;
                    line = MARSUtility.dbtable.lineNo(ex);
                    MARSUtility.dbtable.errorlog("Export stopped", "Export storyboard Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", projectname, "TESTCASE");
                    MARSUtility.dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }

                return Json(lFileName, JsonRequestBehavior.AllowGet);
            }
            MARSUtility.dbtable.errorlog("Project not found", "Export storyboard Excel", "", 0);
            objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public FileStreamResult ExportAllStoryboards()
        {

            string FullPath = "";
            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
            var byteArray = System.IO.File.ReadAllBytes(FullPath);

            var ms = new MemoryStream(byteArray);
            return new FileStreamResult(ms, "application/ms-excel")
            {
                FileDownloadName = "Storyboards" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xls"
            };
        }
        public ActionResult ImportStoryboard()
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
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ImportStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ImportStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ImportStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }
        public ActionResult ImportStoryboards()
        {
            ViewBag.FileName = "";
            string fileName = string.Empty;
            string name = "Log_Import" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ImportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            var _treerepository = new GetTreeRepository();
            var lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
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
                    HttpPostedFileBase storyboardupload = files[i];
                    if (storyboardupload != null)
                    {

                        string destinationPath = string.Empty;
                        string extension = string.Empty;
                        var uploadFileModel = new List<StoryboardFileUpload>();

                        fileName = Path.GetFileNameWithoutExtension(storyboardupload.FileName);
                        extension = Path.GetExtension(storyboardupload.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                        destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                        storyboardupload.SaveAs(destinationPath);
                        MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                        //string TempFileLocation = WebConfigurationManager.AppSettings["StoryBoardTemplateLocation"];
                        dbtable.errorlog("Import is started", "Import Storyboard", "", 0);
                        // var lPath = ImportStoryBoard(destinationPath, lConnectionStr, lSchema, Server.MapPath("~/Temp/"), SessionManager.TESTER_LOGIN_NAME);
                        var lPath = helper.MasterImport(0, destinationPath, strPath, "STORYBOARD", 1, "", "", "", 1, lSchema, lConnectionStr);

                        if (lPath == false)
                        {
                            dbtable.errorlog("Import is stopped", "Import Storyboard", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "STORYBOARD");
                            return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                            dbtable.errorlog("Import is completed", "Import Storyboard", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "STORYBOARD");
                            return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ImportStoryboards method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ImportStoryboards method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ImportStoryboards method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import Storyboard", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "STORYBOARD");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);
            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }
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
        #endregion

        #region PQGrid functionality of Storyboard grid

        //Get all the steps of the Storyboard grid by ProjectId and StoryboardId
        public ActionResult GetStoryBoardDetails(long Pid, long sid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                string directoryPath = Path.Combine(Server.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Storyboard.ToString(), SessionManager.Schema);
                string[] filesNames = new string[0];
                if (Directory.Exists(directoryPath))
                {
                    filesNames = Directory.GetFiles(directoryPath);
                    var filesName = filesNames.Select(x => Path.GetFileName(x)).ToArray();
                    var storyName = filesName.FirstOrDefault(x => x.StartsWith($"{Pid}_{sid}_"));
                    if (!string.IsNullOrEmpty(storyName))
                    {
                        var result = JsonFileHelper.GetFilePath(storyName, SessionManager.Schema);
                        if (!string.IsNullOrEmpty(result))
                        {
                            ConcurrentDictionary<string, string> keyValuePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(result);
                            if (keyValuePairs != null && keyValuePairs.ContainsKey("StoryBoardDetails"))
                            {
                                var jsonresult1 = keyValuePairs["StoryBoardDetails"];
                                jsonresult1 = jsonresult1.Replace("\\r", "\\\\r");
                                jsonresult1 = jsonresult1.Replace("\\n", "\\\\n");
                                jsonresult1 = jsonresult1.Replace("   ", "");
                                jsonresult1 = jsonresult1.Replace("\\", "\\\\");
                                jsonresult1 = jsonresult1.Trim();
                                resultModel.status = 1;
                                resultModel.data = jsonresult1;
                                return Json(resultModel, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                }
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var lresult = repo.GetStoryBoardDetails(lSchema, lConnectionStr, Pid, sid);

                var jsonresult = JsonConvert.SerializeObject(lresult);

                jsonresult = jsonresult.Replace("\\r", "\\\\r");
                jsonresult = jsonresult.Replace("\\n", "\\\\n");
                jsonresult = jsonresult.Replace("   ", "");
                jsonresult = jsonresult.Replace("\\", "\\\\");
                jsonresult = jsonresult.Trim();
                resultModel.status = 1;
                resultModel.data = jsonresult;

                var storyBoardName = lresult.FirstOrDefault()?.Storyboardname;
                if (!string.IsNullOrWhiteSpace(storyBoardName))
                {
                    string key = $"{Pid}_{sid}_{storyBoardName}.json";
                    var jsonFiles = JsonFileHelper.GetFilePath(key, lSchema);
                    if (!string.IsNullOrEmpty(jsonFiles))
                    {
                        ConcurrentDictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(jsonFiles);
                        if (keyValuePairs != null && keyValuePairs.ContainsKey("StoryBoardDetails"))
                        {
                            keyValuePairs["StoryBoardDetails"] = jsonresult;
                            JsonFileHelper.SaveToJsonFile(JsonConvert.SerializeObject(keyValuePairs), key, lSchema);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetStoryBoardDetails method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", sid, Pid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetStoryBoardDetails method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", sid, Pid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetStoryBoardDetails method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", sid, Pid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Loads dropdown of Actions Column in storyboard grid
        public ActionResult GetActionList(long storyboardid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.GetActions(storyboardid);

                resultModel.status = 1;
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetActionList method | StoryBoard Id : {0} | UserName: {0}", storyboardid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetActionList method | StoryBoard Id : {0} | UserName: {0}", storyboardid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetActionList method | StoryBoard Id : {0} | UserName: {0}", storyboardid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Loads dropdown of Test Suites Column based on ProjectId in Storyboard Grid
        public ActionResult GetTestSuiteListInStoryboard(long ProjectId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                GetTreeRepository repo = new GetTreeRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lresult = repo.GetTestSuiteList(ProjectId);

                resultModel.status = 1;
                resultModel.data = lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetTestSuiteListInStoryboard method | Project Id : {0} | UserName: {0}", ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetTestSuiteListInStoryboard method | Project Id : {0} | UserName: {0}", ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetTestSuiteListInStoryboard method | Project Id : {0} | UserName: {0}", ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Loads Dropdown of TestCase column based on selected TestSuite in Storyboard grid 
        public ActionResult GetTestCaseListinStoryboard(GetTestCaseByTestSuite lgrid)
        {
            logger.Info(string.Format("controller -->GetTestCaseListinStoryboard start "));
            ResultModel resultModel = new ResultModel();
            try
            {
                var lresult = new List<TestCaseListByProject>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string pgrid = lgrid.grid;
                long stepid = lgrid.stepid;
                long projectid = lgrid.projectid;

                int i = 1;
                var obj = js.Deserialize<GetTestCaseByTestSuite[]>(pgrid);

                GetTestCaseByTestSuite testcase = obj[stepid - 1];
                logger.Info(string.Format("controller-->call repo -->GetTestCaseListinStoryboard start "));
                var lresult1 = repo.GetTestCaseList(projectid, testcase.testsuitename);
                logger.Info(string.Format("controller-->call repo -->GetTestCaseListinStoryboard end "));
                /*  foreach (var item in obj)
                  {
                      if (stepid >= i)
                      {
                          lresult = repo.GetTestCaseList(projectid, item.testsuitename);
                      }
                      i++;
                  }*/
                logger.Info(string.Format("controller -->GetTestCaseListinStoryboard end "));
                resultModel.status = 1;
                resultModel.data = lresult1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetTestCaseListinStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetTestCaseListinStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetTestCaseListinStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTestCaseListinStoryboardNew(long projectId, long testSuiteId, long Storyboardid, string Storyboardname)
        {
            logger.Info(string.Format("controller -->GetTestCaseListinStoryboard start "));
            ResultModel resultModel = new ResultModel();
            try
            {
                var lresult = new List<TestCaseListByProject>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var keyValue = getStoryBoardCache(projectId, Storyboardid, Storyboardname);
                if (keyValue != null && keyValue.ContainsKey("keyValues"))
                {
                    var test = JsonConvert.DeserializeObject<Dictionary<string, List<TestCaseListByProject>>>(keyValue["keyValues"]);
                    resultModel.data = test["testCaseLists"].FindAll(r=>r.TestsuiteId==testSuiteId);
                }
                else
                {
                    resultModel.data = repo.GetTestCaseListNew(projectId, testSuiteId);
                }
                logger.Info(string.Format("controller-->call repo -->GetTestCaseListinStoryboard end "));

                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetTestCaseListinStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetTestCaseListinStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetTestCaseListinStoryboard method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestSuiteListInStoryboardNew(long ProjectId, long Storyboardid, string Storyboardname)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                GetTreeRepository repo = new GetTreeRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;

                var keyValue = getStoryBoardCache(ProjectId, Storyboardid, Storyboardname);
                if (keyValue != null && keyValue.ContainsKey("keyValues"))
                {
                    var  test = JsonConvert.DeserializeObject< Dictionary<string, List<TestCaseListByProject>> >(keyValue["keyValues"])  ;
                    resultModel.data = test["testSuiteList"];
                }
                else
                {
                    resultModel.data = repo.GetTestSuiteListNew(ProjectId);
                }
                resultModel.status = 1;

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetTestSuiteListInStoryboard method | Project Id : {0} | UserName: {0}", ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetTestSuiteListInStoryboard method | Project Id : {0} | UserName: {0}", ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetTestSuiteListInStoryboard method | Project Id : {0} | UserName: {0}", ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        //Loads Dropdown of Dataset column based on selected TestSuite and TestCase in Storyboard grid
        public ActionResult GetDatasetList(GetDatasetByTestcase lgrid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lresult = new List<DataSetListByTestCase>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string pgrid = lgrid.grid;
                long stepid = lgrid.stepid;
                long projectid = lgrid.projectid;
                int i = 1;
                var obj = js.Deserialize<GetDatasetByTestcase[]>(pgrid);
                GetDatasetByTestcase dataset = obj[stepid - 1];
                var lresult1 = repo.GetDataSetList(projectid, dataset.testsuitename, dataset.Testcasename);
                /* foreach (var item in obj)
                 {
                     if (stepid >= i)
                     {
                         lresult = repo.GetDataSetList(projectid, item.testsuitename, item.Testcasename);
                     }
                     i++;
                 }*/
                resultModel.status = 1;
                resultModel.data = lresult1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDatasetListNew(long projectid, long testCaseId, long Storyboardid, string Storyboardname)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lresult = new List<DataSetListByTestCase>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var keyValue = getStoryBoardCache(projectid, Storyboardid, Storyboardname);
                if (keyValue != null && keyValue.ContainsKey("keyValues"))
                {
                    var test = JsonConvert.DeserializeObject<Dictionary<string, List<DataSetListByTestCase>>>(keyValue["keyValues"]);
                    resultModel.data = test["datasetList"].FindAll(r=>r.TestcaseId==testCaseId);
                }
                else
                {
                    resultModel.data = repo.GetDataSetListNew(projectid, testCaseId);
                }

                resultModel.status = 1;
             }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }


        //Loads dropdown  of Dependency column based on the values inserted in steps column
        public ActionResult LoadDependency(Dependencylist dependencylist)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                logger.Info(string.Format("Load Dependency start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                List<String> Dlist = new List<String>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                string pgrid = dependencylist.grid;
                long stepid = dependencylist.stepid;
                long projectid = dependencylist.projectid;
                var obj = js.Deserialize<Dependencylist[]>(pgrid);
                int i = 1;
                foreach (var item in obj)
                {
                    if (item.stepname != "" && item.stepname != null)
                    {
                        if (stepid > i)
                        {
                            Dlist.Add(item.stepname);
                            Dlist = (from w in Dlist select w).Distinct().ToList();
                        }
                    }
                    i++;
                }
                logger.Info(string.Format("Load Dependency end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                resultModel.status = 1;
                resultModel.data = Dlist;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for LoadDependency method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for LoadDependency method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for LoadDependency method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Validates a storyboard grid on click of validate button and returns the result
        public ActionResult ValidateStoryboard(string lGridJsonData, string lStoryboardId, string lProjectId)
        {
            logger.Info(string.Format("Storyborad Check Validation start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            var result = "";
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var sbRep = new StoryBoardRepository();
                sbRep.Username = SessionManager.TESTER_LOGIN_NAME;

                //check Keyword  object linking
                //var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);
                var lobj = js.Deserialize<List<StoryBoardResultModel>>(lGridJsonData);
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                if (lobj.Count() > 0)
                {
                    string lreturnValues = sbRep.InsertFeedProcess();

                    var lvalFeed = lreturnValues.Split('~')[0];
                    var lvalFeedD = lreturnValues.Split('~')[1];

                    var ValidationResult = sbRep.InsertStgStoryboardValidationTable(lConnectionStr, lSchema, lobj, lStoryboardId, lvalFeed, lvalFeedD, lProjectId);


                    //var ValidationResult = sbRep.CheckSBGridValidation(lobj.ToList(), int.Parse(lStoryboardId));
                    //ValidationResult = ValidationResult.Where(x => x.IsValid == false).ToList();
                    result = JsonConvert.SerializeObject(ValidationResult);

                    resultModel.status = 1;
                    resultModel.data = result;
                    resultModel.message = "Storyboard validated successfully.";
                    logger.Info(string.Format("Storyborad Check Validation end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Storyboard validated successfully. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    resultModel.status = 0;
                    resultModel.message = "Cannot validate an empty storyboard.";
                    logger.Info(string.Format("Cannot validate an empty storyboard. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Storyborad Check Validation end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ValidateStoryboard method | StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | UserName: {3}", lStoryboardId, lProjectId, lGridJsonData, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ValidateStoryboard method | StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | UserName: {3}", lStoryboardId, lProjectId, lGridJsonData, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ValidateStoryboard method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | UserName: {3}", lStoryboardId, lProjectId, lGridJsonData, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Saves the storyboard grid after it is validated successfully
        public ActionResult SaveStoryboardGrid(string lGridJsonData, string lStoryboardId, string lchangedGrid, string lProjectId, string storyBoardName)
        {
            logger.Info(string.Format("Storyborad Save Start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            var result = "";
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                //check Keyword  object linking
                //var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);
                var lobj = js.Deserialize<List<StoryBoardResultModel>>(lGridJsonData);
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                if (lobj.Count() > 0)
                {
                    Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lchangedGrid);
                    SaveStoryBoardInit(lobj, dlist);
                    var sbRep = new StoryBoardRepository();
                    sbRep.Username = SessionManager.TESTER_LOGIN_NAME;
                    /*logger.Info(string.Format("Storyborad Saves 1 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    string lreturnValues = sbRep.InsertFeedProcess();
                    logger.Info(string.Format("Storyborad Saves 2 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    var lvalFeed = lreturnValues.Split('~')[0];
                    logger.Info(string.Format("Storyborad Saves 3 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    var lvalFeedD = lreturnValues.Split('~')[1];
                    logger.Info(string.Format("Storyborad Saves 4 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    var ValidationResult = sbRep.InsertStgStoryboardValidationTable(lConnectionStr, lSchema, lobj.FindAll(r => !string.IsNullOrWhiteSpace(r.status) && r.status!="delete"), lStoryboardId, lvalFeed, lvalFeedD, lProjectId);
                    logger.Info(string.Format("Storyborad Saves 5 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));*/

                    var addAndUpdateList = lobj.FindAll(r => !string.IsNullOrWhiteSpace(r.status) && (r.status == "update" || r.status == "add"));
                    List<TestCaseValidationResultModel> ValidationResult = new List<TestCaseValidationResultModel>();
                    long lvalFeedLong = IdWorker.Instance.NextId();
                    if (addAndUpdateList != null && addAndUpdateList.Count > 0)
                    {
                        logger.Info(string.Format("Storyborad Saves 1 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                        var lvalFeed = lvalFeedLong.ToString();
                        var lvalFeedD = IdWorker.Instance.NextId().ToString();
                        ValidationResult = sbRep.InsertStgStoryboardValidationTable(lConnectionStr, lSchema, addAndUpdateList, lStoryboardId, lvalFeed, lvalFeedD, lProjectId);
                        logger.Info(string.Format("Storyborad Saves 2 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    }
                    // var ValidationResult = sbRep.CheckSBGridValidation(lobj.ToList(), int.Parse(lStoryboardId));
                    //  ValidationResult = ValidationResult.Where(x => x.IsValid == false).ToList();

                    if (ValidationResult.Count() == 0)
                    {
                        /*if (dlist["deleteList"].Count > 0)
                        {
                            var lDeletedSB = new List<long>();
                            foreach (var d in dlist["deleteList"])
                            {
                                var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                                var lsbdetailId = delete.Where(x => x.Key == "detailid");
                                if (!string.IsNullOrEmpty(Convert.ToString(lsbdetailId.FirstOrDefault().Value)))
                                {
                                    lDeletedSB.Add(Convert.ToInt32(lsbdetailId.FirstOrDefault().Value.ToString()));
                                }
                            }
                            if (lDeletedSB.Count() > 0)
                            {
                                logger.Info(string.Format("Storyborad Saves 6 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                                sbRep.DeleteSBSteps(lDeletedSB);
                                logger.Info(string.Format("Storyborad Saves 7 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                            }
                        }
                        logger.Info(string.Format("Storyborad Saves 8 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                        sbRep.SaveStoryboardGrid(int.Parse(lStoryboardId), lvalFeed, lProjectId);
*/
                        string key = $"{lProjectId}_{lStoryboardId}_{storyBoardName}.json";
                        var jsonFiles = JsonFileHelper.GetFilePath(key, lSchema);
                        if (!string.IsNullOrEmpty(jsonFiles))
                        {
                            ConcurrentDictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(jsonFiles);
                            if (keyValuePairs != null && keyValuePairs.ContainsKey("StoryBoardDetails"))
                            {
                                var values = JsonConvert.DeserializeObject<List<StoryBoardResultModel>>(keyValuePairs["StoryBoardDetails"]);
                                foreach (var obj in lobj)
                                {
                                    if (obj.status == "delete")
                                    {
                                        values.RemoveAll(r => r.storyboarddetailid == obj.storyboarddetailid);
                                    }
                                    else if (obj.status == "add")
                                    {
                                        values.Add(new StoryBoardResultModel()
                                        {
                                            ProjectId = obj.ProjectId,
                                            ActionName = obj.ActionName,
                                            Run_order = obj.Run_order,
                                            StepName = obj.StepName,
                                            TestSuiteName = obj.TestSuiteName,
                                            TestCaseName = obj.TestCaseName,
                                            DataSetName = obj.DataSetName,
                                            Dependency = obj.Dependency,
                                            storyboarddetailid = IdWorker.Instance.NextId(),
                                            Storyboardid = long.Parse(lStoryboardId),
                                            Storyboardname = storyBoardName
                                        });
                                    }
                                    else if (obj.status == "update")
                                    {
                                        var value = values.FirstOrDefault(r => r.storyboarddetailid == obj.storyboarddetailid);
                                        if (value != null)
                                        {

                                            value.ProjectId = obj.ProjectId;
                                            value.ActionName = obj.ActionName;
                                            value.Run_order = obj.Run_order;
                                            value.StepName = obj.StepName;
                                            value.TestSuiteName = obj.TestSuiteName;

                                            value.DataSetName = obj.DataSetName;
                                            value.Dependency = obj.Dependency;
                                            if (value.TestCaseName != obj.TestCaseName)
                                            {
                                                value.TestCaseName = obj.TestCaseName;
                                                value.BTestResult = "";
                                                value.BErrorcause = "";
                                                var datetimeNow = DateTime.Now;
                                                value.BScriptstart = datetimeNow;
                                                value.Bstart = String.Format("{0:MM/dd/yyyy hh:mm tt}", datetimeNow);
                                                value.BScriptend = datetimeNow;
                                                value.CTestResult = "";
                                                value.CErrorcause = "";
                                                value.CScriptstart = datetimeNow;
                                                value.Cstart = String.Format("{0:MM/dd/yyyy hh:mm tt}", datetimeNow);
                                                value.CScriptend = datetimeNow;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var value = values.FirstOrDefault(r => r.storyboarddetailid != null && r.storyboarddetailid == obj.storyboarddetailid);
                                        if (value != null)
                                            value.Run_order = obj.Run_order;
                                    }
                                }
                                //var lresult = sbRep.GetStoryBoardDetails(lSchema, lConnectionStr, Convert.ToInt64(lProjectId), Convert.ToInt64(lStoryboardId));
                                var storyBoardDetails = JsonConvert.SerializeObject(values);
                                storyBoardDetails = storyBoardDetails.Replace("\\r", "\\\\r");
                                storyBoardDetails = storyBoardDetails.Replace("\\n", "\\\\n");
                                storyBoardDetails = storyBoardDetails.Replace("   ", "");
                                storyBoardDetails = storyBoardDetails.Replace("\\", "\\\\");
                                storyBoardDetails = storyBoardDetails.Trim();
                                keyValuePairs["StoryBoardDetails"] = storyBoardDetails;
                                JsonFileHelper.SaveToJsonFile(JsonConvert.SerializeObject(keyValuePairs), key, lSchema);
                            }
                        }
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            //SaveStoryBoardDetailToJsonFile(lProjectId, lStoryboardId, storyBoardName, lSchema, lConnectionStr, sbRep);
                            bool needSP = false;
                            if (addAndUpdateList != null && addAndUpdateList.Count > 0)
                            {
                                needSP = true;
                            }

                            sbRep.SaveStoryboardGrid(lConnectionStr, lSchema, long.Parse(lStoryboardId), lvalFeedLong.ToString(), lProjectId, lobj.FindAll(r => r.status == "delete"), needSP);
                            SaveStoryBoardDetailToJsonFile(lProjectId, lStoryboardId, storyBoardName, lSchema, lConnectionStr, sbRep);
                        });
                        logger.Info(string.Format("Storyborad Saves 9 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                        result = JsonConvert.SerializeObject(ValidationResult);
                        logger.Info(string.Format("Storyborad Saves 10 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    }
                    else
                    {
                        result = JsonConvert.SerializeObject(ValidationResult);
                        logger.Info(string.Format("Storyborad Saves 11 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    }
                    resultModel.status = 1;
                    resultModel.data = result;
                    resultModel.message = "Storyboard saved successfully.";
                    logger.Info(string.Format("Storyborad Save end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Storyboard saved successfully. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    resultModel.status = 0;
                    resultModel.message = "Cannot save an empty storyboard.";
                    logger.Info(string.Format("Cannot save an empty storyboard. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Storyborad Check Validation end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for SaveStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for SaveStoryboardGrid method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveStoryboardGrid method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                if (ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveStoryboardGrid method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult SaveStoryboardGridNew(string lGridJsonData, string lStoryboardId, string lchangedGrid, string lProjectId, string storyBoardName)
        {
            logger.Info(string.Format("Storyborad Save Start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            var result = "";
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                //check Keyword  object linking
                //var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);
                var lobj = js.Deserialize<List<StoryBoardResultModel>>(lGridJsonData);
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                if (lobj.Count() > 0)
                {
                    Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lchangedGrid);
                    SaveStoryBoardInit(lobj, dlist);
                    var sbRep = new StoryBoardRepository();
                    sbRep.Username = SessionManager.TESTER_LOGIN_NAME;
                    //var addAndUpdateList = lobj.FindAll(r => !string.IsNullOrWhiteSpace(r.status) && (r.status == "update" || r.status == "add"));
                    List<TestCaseValidationResultModel> ValidationResult = new List<TestCaseValidationResultModel>();
                    long lvalFeedLong = IdWorker.Instance.NextId();
                    //if (addAndUpdateList != null && addAndUpdateList.Count > 0)
                    //{
                    logger.Info(string.Format("Storyborad Saves 1 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

                    string key = $"{lProjectId}_{lStoryboardId}_{storyBoardName}.json";
                    List<StoryBoardResultModel> values = null;
                    var jsonFiles = JsonFileHelper.GetFilePath(key, lSchema);
                    if (!string.IsNullOrEmpty(jsonFiles))
                    {
                        ConcurrentDictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(jsonFiles);
                        if (keyValuePairs != null && keyValuePairs.ContainsKey("StoryBoardDetails"))
                        {
                            values = JsonConvert.DeserializeObject<List<StoryBoardResultModel>>(keyValuePairs["StoryBoardDetails"]);
                            foreach (var obj in lobj)
                            {
                                if (obj.status == "delete")
                                {
                                    values.RemoveAll(r => r.storyboarddetailid == obj.storyboarddetailid);
                                }
                                else if (obj.status == "add")
                                {
                                    obj.Storyboardid = long.Parse(lStoryboardId);
                                    obj.storyboarddetailid = IdWorker.Instance.NextId();
                                    values.Add(new StoryBoardResultModel()
                                    {
                                        ProjectId = obj.ProjectId,
                                        ActionName = obj.ActionName,
                                        Run_order = obj.Run_order,
                                        StepName = obj.StepName,
                                        ActionId = obj.ActionId,
                                        Caseid = obj.Caseid,
                                        Suiteid = obj.Suiteid,
                                        Datasetid = obj.Datasetid,
                                        TestSuiteName = obj.TestSuiteName,
                                        TestCaseName = obj.TestCaseName,
                                        DataSetName = obj.DataSetName,
                                        Dependency = obj.Dependency,
                                        storyboarddetailid = obj.storyboarddetailid,
                                        Storyboardid = obj.Storyboardid,
                                        Storyboardname = storyBoardName,
                                        status = "add"
                                    });
                                }
                                else if (obj.status == "update")
                                {
                                    var value = values.FirstOrDefault(r => r.storyboarddetailid == obj.storyboarddetailid);
                                    if (value != null)
                                    {

                                        value.ProjectId = obj.ProjectId;
                                        value.ActionName = obj.ActionName;
                                        value.Run_order = obj.Run_order;
                                        value.StepName = obj.StepName;
                                        value.TestSuiteName = obj.TestSuiteName;
                                        value.ActionId = obj.ActionId;
                                        value.Caseid = obj.Caseid;
                                        value.Suiteid = obj.Suiteid;
                                        value.Datasetid = obj.Datasetid;
                                        value.DataSetName = obj.DataSetName;
                                        value.Dependency = obj.Dependency;
                                        value.status = "update";
                                        if (value.TestCaseName != obj.TestCaseName)
                                        {
                                            value.TestCaseName = obj.TestCaseName;
                                            value.BTestResult = "";
                                            value.BErrorcause = "";
                                            var datetimeNow = DateTime.Now;
                                            value.BScriptstart = datetimeNow;
                                            value.Bstart = String.Format("{0:MM/dd/yyyy hh:mm tt}", datetimeNow);
                                            value.BScriptend = datetimeNow;
                                            value.CTestResult = "";
                                            value.CErrorcause = "";
                                            value.CScriptstart = datetimeNow;
                                            value.Cstart = String.Format("{0:MM/dd/yyyy hh:mm tt}", datetimeNow);
                                            value.CScriptend = datetimeNow;
                                        }
                                    }
                                }
                                else
                                {
                                    var value = values.FirstOrDefault(r => r.storyboarddetailid != null && r.storyboarddetailid == obj.storyboarddetailid);
                                    if (value != null && value.Run_order != obj.Run_order)
                                    {
                                        value.Run_order = obj.Run_order;
                                        value.status = "update";
                                        obj.status = "update";
                                    }
                                }
                            }
                            //var lresult = sbRep.GetStoryBoardDetails(lSchema, lConnectionStr, Convert.ToInt64(lProjectId), Convert.ToInt64(lStoryboardId));
                            var storyBoardDetails = JsonConvert.SerializeObject(values);
                            storyBoardDetails = storyBoardDetails.Replace("\\r", "\\\\r");
                            storyBoardDetails = storyBoardDetails.Replace("\\n", "\\\\n");
                            storyBoardDetails = storyBoardDetails.Replace("   ", "");
                            storyBoardDetails = storyBoardDetails.Replace("\\", "\\\\");
                            storyBoardDetails = storyBoardDetails.Trim();
                            keyValuePairs["StoryBoardDetails"] = storyBoardDetails;
                            JsonFileHelper.SaveToJsonFile(JsonConvert.SerializeObject(keyValuePairs), key, lSchema);
                        }
                        //}

                        System.Threading.Tasks.Task.Run(() =>
                        {
                            bool needSP = false;
                            if (values != null && lobj.Exists(r => !string.IsNullOrWhiteSpace(r.status) && (r.status == "update" || r.status == "add")))
                            {
                                needSP = true;
                            }

                            sbRep.SaveStoryboardGridNew(lConnectionStr, lSchema, long.Parse(lStoryboardId), lvalFeedLong.ToString(), lProjectId, lobj, needSP);
                            SaveStoryBoardDetailToJsonFile(lProjectId, lStoryboardId, storyBoardName, lSchema, lConnectionStr, sbRep);
                        });
                        logger.Info(string.Format("Storyborad Saves 9 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                        result = JsonConvert.SerializeObject(ValidationResult);
                        logger.Info(string.Format("Storyborad Saves 10 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    }

                    resultModel.status = 1;
                    resultModel.data = result;
                    resultModel.message = "Storyboard saved successfully.";
                    logger.Info(string.Format("Storyborad Save end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Storyboard saved successfully. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    resultModel.status = 0;
                    resultModel.message = "Cannot save an empty storyboard.";
                    logger.Info(string.Format("Cannot save an empty storyboard. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Storyborad Check Validation end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for SaveStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for SaveStoryboardGrid method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveStoryboardGrid method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                if (ex.InnerException != null && ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveStoryboardGrid method |  StoryBoard Id : {0} | Project Id : {1} | GridJsonData : {2} | ChangedGrid : {3} | UserName: {4}", lStoryboardId, lProjectId, lGridJsonData, lchangedGrid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }


        #region Save As storyboard with Unique name that is not present in the system with all the steps
        public ActionResult SaveAsStoryboard(string storyboardname, string storyboarddesc, long oldstoryboardid, long projectid)
        {
            logger.Info(string.Format("SaveAs Storyboard Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var repTree = new GetTreeRepository();
                var result = repo.NewSaveAsStoryboard(storyboardname, storyboarddesc, oldstoryboardid, projectid, lSchema, lConnectionStr, SessionManager.TESTER_LOGIN_NAME);
                Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                resultModel.message = result == "success" ? "Storyboard is created successfully" : "Storyboard name must be unique";
                resultModel.data = result;
                resultModel.status = result == "success" ? 1 : 0;
                logger.Info(string.Format("SaveAs Storyboard Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Storyboard SaveAs successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for SaveAsStoryboard method | StoryBoard Id : {0} | StoryBoard Name : {1} | StoryBoard Desc : {2} | Project Id : {3} | UserName: {4}", oldstoryboardid, storyboardname, storyboarddesc, projectid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for SaveAsStoryboard method | StoryBoard Id : {0} | StoryBoard Name : {1} | StoryBoard Desc : {2} | Project Id : {3} | UserName: {4}", oldstoryboardid, storyboardname, storyboarddesc, projectid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveAsStoryboard method | StoryBoard Id : {0} | StoryBoard Name : {1} | StoryBoard Desc : {2} | Project Id : {3} | UserName: {4}", oldstoryboardid, storyboardname, storyboarddesc, projectid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Downloads and Executes MARS Engine

        public ActionResult GetApplicationListByStoryboard(long lStoryboardId)
        {
            //List<ApplicationModel>
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new StoryBoardRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                var lAppList = rep.GetApplicationListByStoryboardId(lStoryboardId);
                resultModel.data = lAppList;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetApplicationListByStoryboard method | StoryBoard Id : {0} | UserName: {1}", lStoryboardId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetApplicationListByStoryboard method | StoryBoard Id : {0} | UserName: {1}", lStoryboardId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetApplicationListByStoryboard method | StoryBoard Id : {0} | UserName: {1}", lStoryboardId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Starts the Mars Engine
        public ActionResult ExecuteEngine()
        {
            StartEngine();
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public void StartEngine()
        {
            string strEngine = "c:\\elasticsearch.bat";
            System.Diagnostics.Process.Start(@"E:\MARS_Web\MARSExplorer\Mars.exe");
            new Thread(new ThreadStart(
                   new Action(() =>
                   {
                       Process pEng = new Process()
                       {
                           StartInfo = new ProcessStartInfo()
                           {
                               FileName = strEngine,
                               Arguments = String.Format("{0},{1},{2}", 5, 6, 7)
                           }
                       };
                       pEng.Start();
                       pEng.WaitForExit();
                   })
               )).Start();
        }

        //Downloads the Batch File
        public ActionResult DownlaodBatchFile(string lLoginUser, string lStoryboardName, string lStoryboardId, string lAppId, string lMode, string lContinue, string lIgnoreError)
        {
            var repAcc = new AccountRepository();
            var repApp = new ApplicationRepository();
            repApp.Username = SessionManager.TESTER_LOGIN_NAME;
            string filepath = System.Web.HttpContext.Current.Server.MapPath("~/Batch/ExecuteEngine.bat");
            var lAutoTestDriverPath = "";
            var lAutoTestDriverPath64bit = "";
            var lPath = repAcc.GetUserExePath(lLoginUser);

            decimal? lIS64Bit = 0;
            var lAppDetail = repApp.GetApplicationDetail(Convert.ToInt64(lAppId));
            if (lAppDetail != null)
            {
                lIS64Bit = lAppDetail.IS64BIT;
            }

            System.IO.File.WriteAllText(filepath, String.Empty);

            using (StreamWriter sw = System.IO.File.AppendText(filepath))
            {
                string lstr = "@echo off \n";
                sw.WriteLine(lstr);
                sw.Flush();
            }

            if (!string.IsNullOrEmpty(lPath.ExePath))
            {
                lAutoTestDriverPath = "\"" + lPath.ExePath + "\\Mars.AutoTestingDriver32\"";
                lAutoTestDriverPath64bit = "\"" + lPath.ExePath + "\\Mars.AutoTestingDriver\"";
                // lAutoTestDriver32Path = "\"" + lPath.ExePath + "\\Mars.AutoTestingDriver32.exe\"";
                var lFramePath = "\"" + lPath.ExePath + "\\TestFrameMonitor\"";

                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    string lstr = "Taskkill /F /IM TestFrameMonitor.exe \n \n";
                    sw.WriteLine(lstr);
                    sw.Flush();
                }


                if (lIS64Bit == 1)
                {
                    using (StreamWriter sw = System.IO.File.AppendText(filepath))
                    {
                        string lstr = "\n Taskkill /F /IM Mars.AutoTestingDriver.exe \n\n";
                        sw.WriteLine(lstr);
                        sw.Flush();
                    }

                    using (StreamWriter sw = System.IO.File.AppendText(filepath))
                    {

                        string lstr = "\n start \"\" " + lFramePath + " " + lLoginUser + " \n \n"
                            + " start \"\" " + lAutoTestDriverPath64bit + " " + lLoginUser + " -S " + lStoryboardName + " " + lStoryboardId + " -App " + lAppId + " -Mode " + lMode +
                            " -Continue " + lContinue + " -IgnoreError " + lIgnoreError + " \n\n";
                        sw.WriteLine(lstr);
                        sw.Flush();
                    }
                }
                else
                {
                    using (StreamWriter sw = System.IO.File.AppendText(filepath))
                    {
                        string lstr = "\n Taskkill /F /IM Mars.AutoTestingDriver32.exe \n\n";
                        sw.WriteLine(lstr);
                        sw.Flush();
                    }

                    using (StreamWriter sw = System.IO.File.AppendText(filepath))
                    {

                        string lstr = "  \n start \"\" " + lFramePath + " " + lLoginUser + " \n \n"
                            + " start \"\" " + lAutoTestDriverPath + " " + lLoginUser + " -S " + lStoryboardName + " " + lStoryboardId + " -App " + lAppId + " -Mode " + lMode +
                            " -Continue " + lContinue + " -IgnoreError " + lIgnoreError + " \n \n ";
                        sw.WriteLine(lstr);
                        sw.Flush();
                    }
                }

                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    string lstr = "pause";
                    sw.WriteLine(lstr);
                    sw.Flush();
                }
            }
            else
            {
                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    string lstr = "@echo MARS Engine file Path not exist in system.\n";
                    sw.WriteLine(lstr);
                    sw.Flush();
                }
                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    string lstr = "pause";
                    sw.WriteLine(lstr);
                    sw.Flush();
                }
            }



            // start C:\MARSExplorer\TestFrameMonitor admin


            //start C:\MARSExplorer\Mars.AutoTestingDriver32 admin -S TPGUNITTEST 27294 - App 14 - Mode Base - Continue True - IgnoreError True
            //     string lPath = AppDomain.CurrentDomain.BaseDirectory + "/Batch/ExecuteEngine.bat";

            var byteArray = System.IO.File.ReadAllBytes(filepath);
            var ms = new MemoryStream(byteArray);
            return new FileStreamResult(ms, "application/x-bat")
            {
                FileDownloadName = "ExecuteEngine.bat"
            };
        }
        #endregion

        #region SignalR Connect to refresh Pass/Fail of the Testcase in UI immediately after any changes occured in database
        public ActionResult SignalRConnect()
        {

            logger.Info(string.Format("SignalRConnect start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            string logPath = WebConfigurationManager.AppSettings["LogPathLocation"];
            EmailHelper.logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/" + logPath + "/");
            string lSchema = SessionManager.Schema;
            OracleConnection con = new OracleConnection(SessionManager.APP);
            try
            {
                logger.Info(string.Format("for grant SignalRConnect start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                OracleCommand ecmd = new OracleCommand();

                ecmd.CommandText = "grant change notification to " + lSchema;//where storyboard_detail_id = 10680
                ecmd.Connection = con;
                con.Open();
                ecmd.ExecuteReader();
                con.Close();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in SignalRConnect Grant permission | Schema: {0}", lSchema));
                ELogger.ErrorException(string.Format("Error occured in SignalRConnect Grant permission | Schema: {0}", lSchema), ex);

            }
            try
            {
                logger.Info(string.Format("After grant SignalRConnect start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "select * from " + lSchema + ".T_PROJ_TEST_RESULT ";//where storyboard_detail_id = 10680
                cmd.Connection = con;
                con.Open();
                OracleDependency dependency = new OracleDependency(cmd);
                dependency.QueryBasedNotification = true;
                cmd.Notification.IsNotifiedOnce = false;
                dependency.OnChange += new OnChangeEventHandler(RefreshResult);
                cmd.ExecuteReader();

                EmailHelper.WriteMessage("Connect SignalR 1", EmailHelper.logFilePath, DateTime.Now.ToString(), "");


                // Updating emp table so that a notification can be received when
                // the emp table is updated.
                // Start a transaction to update emp table
                //OracleTransaction txn = con.BeginTransaction();
                //// Create a new command which will update emp table
                //string updateCmdText =
                //  "update T_PROJ_TEST_RESULT set RESULT_DESC = '1' where HIST_ID= 1181";
                //OracleCommand updateCmd = new OracleCommand(updateCmdText, con);
                //// Update the emp table
                //updateCmd.ExecuteNonQuery();
                //EmailHelper.WriteMessage("Connect SignalR 2", EmailHelper.logFilePath, DateTime.Now.ToString(), "");
                ////When the transaction is committed, a notification will be sent from
                ////the database
                //txn.Commit();
                ////con.Close();
                //EmailHelper.WriteMessage("Connect SignalR 3", EmailHelper.logFilePath, DateTime.Now.ToString(), "");
                //while (IsNotified == false)
                //{
                //    Thread.Sleep(100);
                //}
                con.Close();
                logger.Info(string.Format("SignalRConnect end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in SignalRConnect | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in SignalRConnect | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        static void RefreshResult(Object sender, OracleNotificationEventArgs args)
        {
            DataTable dt = args.Details;
            string msg = "";
            EmailHelper.WriteMessage("Refresh result", EmailHelper.logFilePath, DateTime.Now.ToString(), "");
            var statsContext = GlobalHost.ConnectionManager.GetHubContext<ExecutionResult>();
            statsContext.Clients.All.addNewMessageToPage("Oracle", msg);
        }
        #endregion

        #region Shows the Test results
        //This method Get TestResult by TestCaseId and StoryboardDetailId
        public ActionResult GetTestResult(long TestCaseId, long StoryboardDetailId, string StoryboardName = "", long RunOrder = 0)
        {
            try
            {
                ViewBag.TestCaseId = TestCaseId;
                ViewBag.StoryboardDetailId = StoryboardDetailId;
                ViewBag.StoryboardName = StoryboardName;
                ViewBag.RunOrder = RunOrder;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetTestResult method | TestCase Id : {0} | StoryBoardDetail Id : {1} | StoryBoard Name : {2} | Run Order: {3} | UserName: {4}", TestCaseId, StoryboardDetailId, StoryboardName, RunOrder, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetTestResult method | TestCase Id : {0} | StoryBoardDetail Id : {1} | StoryBoard Name : {2} | Run Order: {3} | UserName: {4}", TestCaseId, StoryboardDetailId, StoryboardName, RunOrder, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetTestResult method | TestCase Id : {0} | StoryBoardDetail Id : {1} | StoryBoard Name : {2} | Run Order: {3} | UserName: {4}", TestCaseId, StoryboardDetailId, StoryboardName, RunOrder, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("_SBTestResult");
        }

        //This method Get TestResult List by TestCaseId and StoryboardDetailId
        public ActionResult GetTestResultList(long TestCaseId, long StoryboardDetailId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new StoryBoardRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                var lModel = rep.GetTestResult(TestCaseId, StoryboardDetailId);
                var jsonresult = JsonConvert.SerializeObject(lModel);
                resultModel.status = 1;
                resultModel.data = jsonresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetTestResultList method | TestCase Id : {0} | StoryBoardDetail Id : {1} | UserName: {2}", TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetTestResultList method | TestCase Id : {0} | StoryBoardDetail Id : {1} | UserName: {2}", TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetTestResultList method | TestCase Id : {0} | StoryBoardDetail Id : {1} | UserName: {2}", TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region shows Compare Result
        //This method Get Compare ResultSet by TestCaseId, StoryboardDetailId, BhistedId and ChistedId
        [HttpPost]
        public ActionResult GetCompareResultSet(long BhistedId, long ChistedId, long TestCaseId, long StoryboardDetailId)
        {
            try
            {
                var rep = new StoryBoardRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.BhistedId = BhistedId;
                ViewBag.ChistedId = ChistedId;


                ViewBag.BTestReportId = rep.GetTestReportId(BhistedId);
                ViewBag.CTestReportId = rep.GetTestReportId(ChistedId);

                ViewBag.TestCaseId = TestCaseId;
                ViewBag.StoryboardDetailId = StoryboardDetailId;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetCompareResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | TestCaseId : {2} | SBDetailId : {3} | UserName: {4}", BhistedId, ChistedId, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetCompareResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | TestCaseId : {2} | SBDetailId : {3} | UserName: {4}", BhistedId, ChistedId, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetCompareResultSet method |  Baseline HistId : {0} | Comapre HistId : {1} | TestCaseId : {2} | SBDetailId : {3} | UserName: {4}", BhistedId, ChistedId, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("_SBCompareResult");
        }

        //This method Get Compare ResultSet List by BhistedId and ChistedId
        public JsonResult GetCompareResultList(long BhistedId, long ChistedId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new StoryBoardRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                logger.Error(string.Format("called GetCompareResult SP {0} : {1}", BhistedId, ChistedId));
                var lModel = rep.GetCompareResultList(lSchema, lConnectionStr, BhistedId, ChistedId);
                logger.Error(string.Format("end  called GetCompareResult SP count :{0} ", lModel.Count()));

                var jsonresult = JsonConvert.SerializeObject(lModel);
                logger.Error(string.Format("done ckp  called GetCompareResult SP count :{0} ", lModel.Count()));
                resultModel.status = 1;
                resultModel.data = jsonresult;

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetCompareResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | UserName: {2}", BhistedId, ChistedId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetCompareResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | UserName: {2}", BhistedId, ChistedId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetCompareResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | UserName: {2}", BhistedId, ChistedId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return new JsonResult()
            {
                Data = resultModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = 86753090  // Use this value to set your maximum size for all of your Requests
            };
            // return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Add/Update ResultData objects values
        public ActionResult SaveSbResultData(string lgridchange, string lgrid, long BaselineReportId, long CompareReportId, long TestCaseId, long StoryboardDetailId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lGridData = js.Deserialize<List<ValidatResultViewModel>>(lgrid);
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                var srepo = new StoryBoardRepository();
                srepo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var stgresult = false;
                HistIdViewModel histIdViewModel = new HistIdViewModel();

                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lgridchange);
                Dictionary<String, List<TestResultViewModel>> gridlist = js.Deserialize<Dictionary<String, List<TestResultViewModel>>>(lgridchange);
                //ValidatResultViewModel
                var lValidationList = new List<ValidatResultViewModel>();
                if (lGridData.Count() > 0)
                {
                    var DuplicateDatatag = lGridData.GroupBy(x => x.InputValueSetting == null ? "" : Convert.ToString(x.InputValueSetting).ToUpper())
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key)
                                .ToList();
                    var DuplicateRows = lGridData.Where(x => DuplicateDatatag.Contains(x.InputValueSetting != null ? x.InputValueSetting.ToUpper() : "")).ToList();
                    //DuplicateRows = DuplicateRows.Where(x => x.InputValueSetting != "" && x.InputValueSetting!=null).ToList();

                    lValidationList = DuplicateRows.Select(x => new ValidatResultViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Duplicate Data tag name. " }).ToList();

                    if (dlist["updateList"].Count > 0)
                    {
                        var StepIds = new List<int>();
                        foreach (var d in dlist["updateList"])
                        {
                            var updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            StepIds.Add(Convert.ToInt32(Convert.ToString(updates.FirstOrDefault(x => x.Key == "StepNo").Value)) - 1);
                        }

                        var lUpdateRowCount = dlist["updateList"].Count;
                        var lOldRowCount = dlist["oldList"].Count;

                        for (int i = 0; i < lUpdateRowCount; i++)
                        {
                            var update = (((System.Collections.Generic.Dictionary<string, object>)dlist["updateList"][i]).ToList());
                            var old = (((System.Collections.Generic.Dictionary<string, object>)dlist["oldList"][i]).ToList());

                            if (old.Any(x => x.Key == "BreturnValues"))
                            {
                                var BStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "BaselineComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(BStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Baseline comment. " });
                                }
                            }

                            if (old.Any(x => x.Key == "CreturnValues"))
                            {
                                var CStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "CompareComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(CStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Compare comment. " });
                                }
                            }
                        }
                    }

                }
                if (lValidationList.Count() > 0)
                {
                    var lValidationResult = lValidationList.OrderBy(x => x.pq_ri).GroupBy(x => new { x.pq_ri, x.IsValid }).Select(g => new { g.Key.pq_ri, g.Key.IsValid, ValidMsg = string.Join(", ", g.Select(i => i.ValidMsg)) }).ToList();
                    var result = JsonConvert.SerializeObject(lValidationResult.OrderBy(x => x.pq_ri));
                    resultModel.data = result;
                    resultModel.message = "validation";
                    resultModel.status = 0;
                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                }

                if (BaselineReportId == 0)
                {
                    long hist_id;
                    BaselineReportId = srepo.CreateTestReportId(TestCaseId, StoryboardDetailId, 1, out hist_id);
                    histIdViewModel.BaseHistId = hist_id;
                }

                if (CompareReportId == 0)
                {
                    long hist_id;
                    CompareReportId = srepo.CreateTestReportId(TestCaseId, StoryboardDetailId, 0, out hist_id);
                    histIdViewModel.CompareHistId = hist_id;
                }

                if (dlist["updateList"].Count > 0)
                {
                    var list = new List<TestResultViewModel>();
                    string returnValues = repo.InsertFeedProcess();
                    var valFeed = returnValues.Split('~')[0];
                    valFeedD = returnValues.Split('~')[1];
                    DataTable dt = new DataTable();
                    dt.Columns.Add("FEEDPROCESSDETAILID");
                    dt.Columns.Add("BASELINEID");
                    dt.Columns.Add("COMPAREID");
                    dt.Columns.Add("BASELINEVALUE");
                    dt.Columns.Add("COMPAREVALUE");
                    dt.Columns.Add("INPUTVALUESETTING");
                    dt.Columns.Add("BASELINECOMMENT");
                    dt.Columns.Add("COMPARECOMMENT");
                    dt.Columns.Add("BASELINEREPORTID");
                    dt.Columns.Add("COMPAREREPORTID");
                    foreach (var d in dlist["updateList"])
                    {
                        var update = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                        var baseid = update.Where(x => x.Key == "BaselineStepId");
                        var compareid = update.Where(x => x.Key == "CompareStepId");
                        var basevalue = update.Where(x => x.Key == "BreturnValues");
                        var comparevalue = update.Where(x => x.Key == "CreturnValues");
                        var inputvaluesetting = update.Where(x => x.Key == "InputValueSetting");
                        var basecomment = update.Where(x => x.Key == "BaselineComment");
                        var comparecomment = update.Where(x => x.Key == "CompareComment");
                        DataRow dr = dt.NewRow();
                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                        dr["BASELINEID"] = Convert.ToString(baseid.FirstOrDefault().Value);
                        dr["COMPAREID"] = Convert.ToString(compareid.FirstOrDefault().Value);
                        dr["BASELINEVALUE"] = Convert.ToString(basevalue.FirstOrDefault().Value);
                        dr["COMPAREVALUE"] = Convert.ToString(comparevalue.FirstOrDefault().Value);
                        dr["INPUTVALUESETTING"] = Convert.ToString(inputvaluesetting.FirstOrDefault().Value);
                        dr["BASELINECOMMENT"] = Convert.ToString(basecomment.FirstOrDefault().Value);
                        dr["COMPARECOMMENT"] = Convert.ToString(comparecomment.FirstOrDefault().Value);
                        dr["BASELINEREPORTID"] = Convert.ToString(BaselineReportId);
                        dr["COMPAREREPORTID"] = Convert.ToString(CompareReportId);
                        dt.Rows.Add(dr);
                    }
                    stgresult = srepo.InsertResultsInStgTbl(lConnectionStr, lSchema, dt, int.Parse(valFeedD));
                }

                if (gridlist["addList"].Count > 0)
                {
                    var list = new List<TestResultViewModel>();
                    if (valFeedD == "")
                    {
                        string returnValues = repo.InsertFeedProcess();
                        var valFeed = returnValues.Split('~')[0];
                        valFeedD = returnValues.Split('~')[1];
                    }
                    DataTable dt = new DataTable();
                    dt.Columns.Add("FEEDPROCESSDETAILID");
                    dt.Columns.Add("BASELINEID");
                    dt.Columns.Add("COMPAREID");
                    dt.Columns.Add("BASELINEVALUE");
                    dt.Columns.Add("COMPAREVALUE");
                    dt.Columns.Add("INPUTVALUESETTING");
                    dt.Columns.Add("BASELINECOMMENT");
                    dt.Columns.Add("COMPARECOMMENT");
                    dt.Columns.Add("BASELINEREPORTID");
                    dt.Columns.Add("COMPAREREPORTID");
                    foreach (var d in gridlist["addList"])
                    {
                        var baseid = d.BaselineStepId <= 0 || d.BaselineStepId == null ? 0 : d.BaselineStepId;
                        var compareid = d.CompareStepId <= 0 || d.CompareStepId == null ? 0 : d.CompareStepId;
                        var basevalue = d.BreturnValues == null ? "" : d.BreturnValues;
                        var comparevalue = d.CreturnValues == null ? "" : d.CreturnValues;
                        var inputvaluesetting = d.InputValueSetting == null ? "" : d.InputValueSetting;
                        var basecomment = d.BaselineComment == null ? "" : d.BaselineComment;
                        var comparecomment = d.CompareComment == null ? "" : d.CompareComment;
                        DataRow dr = dt.NewRow();
                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                        dr["BASELINEID"] = Convert.ToString(baseid);
                        dr["COMPAREID"] = Convert.ToString(compareid);
                        dr["BASELINEVALUE"] = Convert.ToString(basevalue);
                        dr["COMPAREVALUE"] = Convert.ToString(comparevalue);
                        dr["INPUTVALUESETTING"] = Convert.ToString(inputvaluesetting);
                        dr["BASELINECOMMENT"] = Convert.ToString(basecomment);
                        dr["COMPARECOMMENT"] = Convert.ToString(comparecomment);
                        dr["BASELINEREPORTID"] = Convert.ToString(BaselineReportId);
                        dr["COMPAREREPORTID"] = Convert.ToString(CompareReportId);
                        dt.Rows.Add(dr);
                    }
                    stgresult = srepo.InsertResultsInStgTbl(lConnectionStr, lSchema, dt, int.Parse(valFeedD));
                }
                if (stgresult == true)
                {
                    var fresult = srepo.updateresults(lConnectionStr, lSchema, valFeedD);
                }

                if (gridlist["deleteList"].Count > 0)
                {
                    var list = new List<long>();
                    foreach (var d in gridlist["deleteList"])
                    {
                        if (d.BaselineStepId > 0)
                            list.Add((long)d.BaselineStepId);
                        if (d.CompareStepId > 0)
                            list.Add((long)d.CompareStepId);
                    }
                    var result = srepo.DeleteResultsetstep(list);
                }

                //Get compare and Baseline primary result set and check true/false

                srepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var loutput = srepo.updatePrimaryResultStatus(TestCaseId, StoryboardDetailId, lSchema, lConnectionStr);

                resultModel.message = "success";
                resultModel.status = 1;
                resultModel.data = histIdViewModel;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for SaveSbResultData method | Baseline ReportId : {0} | Comapre ReportId : {1} | Change Grid : {2} | Grid : {3} | TestCaseId : {4} | StoryBoardId : {5} | UserName: {6}", BaselineReportId, CompareReportId, lgridchange, lgrid, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for SaveSbResultData method |Baseline ReportId : {0} | Comapre ReportId : {1} | Change Grid : {2} | Grid : {3} | TestCaseId : {4} | StoryBoardId : {5} | UserName: {6}", BaselineReportId, CompareReportId, lgridchange, lgrid, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveSbResultData method | Baseline ReportId : {0} | Comapre ReportId : {1} | Change Grid : {2} | Grid : {3} | TestCaseId : {4} | StoryBoardId : {5} | UserName: {6}", BaselineReportId, CompareReportId, lgridchange, lgrid, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Validate ResultData objects values
        public ActionResult ValidationSbResultData(string lgridchange, string lgrid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lGridData = js.Deserialize<List<ValidatResultViewModel>>(lgrid);

                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lgridchange);
                //ValidatResultViewModel
                var lValidationList = new List<ValidatResultViewModel>();
                if (lGridData.Count() > 0)
                {
                    var DuplicateDatatag = lGridData.GroupBy(x => x.InputValueSetting == null ? "" : Convert.ToString(x.InputValueSetting).ToUpper())
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key)
                                .ToList();

                    var DuplicateRows = lGridData.Where(x => DuplicateDatatag.Contains(x.InputValueSetting != null ? x.InputValueSetting.ToUpper() : "")).ToList();

                    lValidationList = DuplicateRows.Select(x => new ValidatResultViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Duplicate Data tag name. " }).ToList();

                    if (dlist["updateList"].Count > 0)
                    {
                        var StepIds = new List<int>();
                        foreach (var d in dlist["updateList"])
                        {
                            var updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            StepIds.Add(Convert.ToInt32(Convert.ToString(updates.FirstOrDefault(x => x.Key == "StepNo").Value)) - 1);
                        }

                        var lUpdateRowCount = dlist["updateList"].Count;
                        var lOldRowCount = dlist["oldList"].Count;

                        for (int i = 0; i < lUpdateRowCount; i++)
                        {
                            var update = (((System.Collections.Generic.Dictionary<string, object>)dlist["updateList"][i]).ToList());
                            var old = (((System.Collections.Generic.Dictionary<string, object>)dlist["oldList"][i]).ToList());

                            if (old.Any(x => x.Key == "BreturnValues"))
                            {
                                var BStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "BaselineComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(BStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Baseline comment. " });
                                }
                            }

                            if (old.Any(x => x.Key == "CreturnValues"))
                            {
                                var CStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "CompareComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(CStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Compare comment. " });
                                }
                            }
                        }
                    }
                }
                if (lValidationList.Count() > 0)
                {
                    var lValidationResult = lValidationList.OrderBy(x => x.pq_ri).GroupBy(x => new { x.pq_ri, x.IsValid }).Select(g => new { g.Key.pq_ri, g.Key.IsValid, ValidMsg = string.Join(", ", g.Select(i => i.ValidMsg)) }).ToList();
                    var result = JsonConvert.SerializeObject(lValidationResult.OrderBy(x => x.pq_ri));
                    resultModel.data = result;
                    resultModel.message = "validation";
                    resultModel.status = 0;
                }
                else
                {
                    resultModel.message = "success";
                    resultModel.status = 1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ValidationSbResultData method | Change Grid : {0} | Grid : {1} | UserName: {2}", lgrid, lgridchange, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ValidationSbResultData method | Change Grid : {0} | Grid : {1} | UserName: {2}", lgrid, lgridchange, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ValidationSbResultData method | Change Grid : {0} | Grid : {1} | UserName: {2}", lgrid, lgridchange, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //SaveAs ResultData by BhistedId and ChistedId
        [HttpPost]
        public ActionResult SaveAsResultSet(long BhistedId, long ChistedId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new StoryBoardRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = rep.SaveAsResultSet(BhistedId, ChistedId);
                resultModel.data = result;
                resultModel.message = "Result Set SaveAs successfully.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for SaveAsResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | UserName: {2}", BhistedId, ChistedId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for SaveAsResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | UserName: {2}", BhistedId, ChistedId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveAsResultSet method | Baseline HistId : {0} | Comapre HistId : {1} | UserName: {2}", BhistedId, ChistedId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method will Save Result list
        public ActionResult SaveResultList(string lchangedGrid, long lhistedId, long latestTestMarkId, long TestCaseId, long StoryboardDetailId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var sbRep = new StoryBoardRepository();
                sbRep.Username = SessionManager.TESTER_LOGIN_NAME;
                JavaScriptSerializer js = new JavaScriptSerializer();
                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lchangedGrid);

                if (lhistedId != 0 && latestTestMarkId != 0)
                {
                    sbRep.ChangelatestTestMarkId(lhistedId, latestTestMarkId);
                }

                if (dlist["deleteList"].Count > 0)
                {
                    var lDeletedResultsSetList = new List<long>();
                    foreach (var d in dlist["deleteList"])
                    {
                        var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                        var lHistId = delete.Where(x => x.Key == "HistId");
                        if (!string.IsNullOrEmpty(Convert.ToString(lHistId.FirstOrDefault().Value)))
                        {
                            lDeletedResultsSetList.Add(Convert.ToInt32(Convert.ToString(lHistId.FirstOrDefault().Value)));
                        }
                    }
                    if (lDeletedResultsSetList.Count() > 0)
                    {
                        sbRep.DeleteResultSets(lDeletedResultsSetList);
                    }
                }
                if (dlist["updateList"].Count > 0)
                {
                    var lList = new List<TestResultSaveModel>();
                    // var abc = js.Deserialize<TestResultSaveModel[]>(dlist["updateList"].ToString());
                    foreach (var d in dlist["updateList"])
                    {
                        var update = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                        var lHistId = update.Where(x => x.Key == "HistId");
                        var lAlias = update.Where(x => x.Key == "Alias");
                        var lDesc = update.Where(x => x.Key == "Description");
                        var lModel = new TestResultSaveModel();
                        if (!string.IsNullOrEmpty(Convert.ToString(lHistId.FirstOrDefault().Value)))
                        {
                            lModel.HistId = Convert.ToInt32(Convert.ToString(lHistId.FirstOrDefault().Value));
                            lModel.Alias = Convert.ToString(lAlias.FirstOrDefault().Value);
                            lModel.Description = Convert.ToString(lDesc.FirstOrDefault().Value);
                            lList.Add(lModel);
                        }
                    }
                    if (lList.Count() > 0)
                    {
                        sbRep.UpdateResultSets(lList);
                    }
                }
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                sbRep.Username = SessionManager.TESTER_LOGIN_NAME;
                var loutput = sbRep.updatePrimaryResultStatus(TestCaseId, StoryboardDetailId, lSchema, lConnectionStr);

                resultModel.data = "success";
                resultModel.message = " Data List Saved successfully.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for SaveResultList method | HistId : {0} | Changed Grid : {1} | Latest TestMarkId : {2} | Testcase Id : {3} | StoryboardId : {4} | UserName: {5}", lhistedId, lchangedGrid, latestTestMarkId, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for SaveResultList method | HistId : {0} | Changed Grid : {1} | Latest TestMarkId : {2} | Testcase Id : {3} | StoryboardId : {4} | UserName: {5}", lhistedId, lchangedGrid, latestTestMarkId, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for SaveResultList method | HistId : {0} | Changed Grid : {1} | Latest TestMarkId : {2} | Testcase Id : {3} | StoryboardId : {4} | UserName: {5}", lhistedId, lchangedGrid, latestTestMarkId, TestCaseId, StoryboardDetailId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Get all the steps of the Storyboard grid by ProjectId and StoryboardId
        public ActionResult GetStoryboardResultData(long lCompareHistId, long lBaselineHistId)
        {
            StoryBoardRepository repo = new StoryBoardRepository();
            repo.Username = SessionManager.TESTER_LOGIN_NAME;
            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
            var lresult = repo.GetStoryBoardResultData(lSchema, lConnectionStr, lCompareHistId, lBaselineHistId);
            var jsonresult = JsonConvert.SerializeObject(lresult);
            return Json(jsonresult, JsonRequestBehavior.AllowGet);
        }

        #region ResultSet Import/Export
        //This method will export ResultSet by Storyboard
        public JsonResult ExportStoryboardResultSet(int Storyboardid, int Projectid, int Mode)
        {
            string name = "Log__ResultSetExport" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            var helper = new MARSUtility.ExportHelper();
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
            if (Storyboardid > 0 && Projectid > 0 && Mode >= 0)
            {
                var projrepo = new ProjectRepository();
                var Projectname = projrepo.GetProjectNameById(Convert.ToInt64(Projectid));
                var repo = new StoryBoardRepository();
                var Storyboardname = repo.GetStoryboardNameById(Convert.ToInt64(Storyboardid));

                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                var resultmode = Mode == 1 ? "BASELINE" : "COMPARE";
                string lFileName = "RESULT" + "_" + resultmode.ToUpper() + "_" + Projectname + "_" + Storyboardname + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                name = "Log_" + Storyboardname + "_ResultSetExport" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";

                strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                try
                {
                    MARSUtility.dbtable.errorlog("Export is started", "Export Storyboard ResultSet Excel", "", 0);
                    var result = helper.ExportResultSet(Projectname, Storyboardname, Mode, FullPath, lSchema, lConnectionStr);
                    if (result == true)
                    {
                        MARSUtility.dbtable.errorlog("Export is completed", "Export storyboard ResultSet Excel", "", 0);
                        objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                        MARSUtility.dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error occured in StoryBoard for ExportStoryboardResultSet method | StoryBoard Id: {0} | Project Id: {1} | Mode : {2} | UserName: {3}", Storyboardid, Projectid, Mode, SessionManager.TESTER_LOGIN_NAME));
                    ELogger.ErrorException(string.Format("Error occured in StoryBoard for ExportStoryboardResultSet method | StoryBoard Id: {0} | Project Id: {1} | Mode : {2} | UserName: {3}", Storyboardid, Projectid, Mode, SessionManager.TESTER_LOGIN_NAME), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ExportStoryboardResultSet method | StoryBoard Id: {0} | Project Id: {1} | Mode : {2} | UserName: {3}", Storyboardid, Projectid, Mode, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                    int line;
                    string msg = ex.Message;
                    line = MARSUtility.dbtable.lineNo(ex);
                    MARSUtility.dbtable.errorlog("Export stopped", "Export Storyboard ResultSet Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                    MARSUtility.dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }
                MARSUtility.dbtable.errorlog("Export has been stopped because there are no results to export. ", "Export ResultSet Excel", "", 0);
                objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            MARSUtility.dbtable.errorlog("StoryboardId/ProjectId is invalid.", "Export Storyboard ResultSet Excel", "", 0);
            objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        //This method will export ResultSet
        public JsonResult ExportProjectResultSet(int Projectid, int Mode)
        {
            string name = "Log__ResultSetExport" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            var helper = new MARSUtility.ExportHelper();
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
            if (Projectid > 0 && Mode >= 0)
            {
                var projrepo = new ProjectRepository();
                var Projectname = projrepo.GetProjectNameById(Convert.ToInt64(Projectid));

                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                var resultmode = Mode == 1 ? "BASELINE" : "COMPARE";
                string lFileName = "RESULT" + "_" + resultmode.ToUpper() + "_" + Projectname + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                name = "Log_" + Projectname + "_ResultSetExport" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";

                strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                try
                {
                    MARSUtility.dbtable.errorlog("Export is started", "Export ResultSet Excel", "", 0);
                    var result = helper.ExportProjectResultSet(Projectname, Mode, FullPath, lSchema, lConnectionStr);
                    if (result == true)
                    {
                        MARSUtility.dbtable.errorlog("Export is completed", "Export ResultSet Excel", "", 0);
                        objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                        MARSUtility.dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error occured in StoryBoard for ExportProjectResultSet method | Project Id: {0} | Mode : {2} | UserName: {3}", Projectid, Mode, SessionManager.TESTER_LOGIN_NAME));
                    ELogger.ErrorException(string.Format("Error occured in StoryBoard for ExportProjectResultSet method |  Project Id: {0} | Mode : {2} | UserName: {3}", Projectid, Mode, SessionManager.TESTER_LOGIN_NAME), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ExportProjectResultSet method | Project Id: {0} | Mode : {2} | UserName: {3}", Projectid, Mode, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                    int line;
                    string msg = ex.Message;
                    line = MARSUtility.dbtable.lineNo(ex);
                    MARSUtility.dbtable.errorlog("Export stopped", "Export ResultSet Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                    MARSUtility.dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }
                MARSUtility.dbtable.errorlog("Export has been stopped because there are no results to export. ", "Export ResultSet Excel", "", 0);
                objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            MARSUtility.dbtable.errorlog("ProjectId is invalid.", "Export ResultSet Excel", "", 0);
            objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportResultSet(int projectId = 0)
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);
                var repTree = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                ViewBag.projectid = projectId == 0 ? "" : projectId.ToString();
                var result = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                ViewBag.projectlist = result.Select(c => new SelectListItem { Text = c.ProjectName, Value = c.ProjectId.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ImportResultSet method | Project Id: {0} | UserName: {1}", projectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ImportResultSet method | Project Id: {0} | UserName: {1}", projectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ImportResultSet method | Project Id: {0} | UserName: {1}", projectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //This method will Import ResultSet File
        public ActionResult ImportResultSets(int ProjectId, int Mode, string Name = "", string Desc = "")
        {
            ViewBag.FileName = "";
            string fileName = string.Empty;
            string name = "Log_Import" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ImportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            var _treerepository = new GetTreeRepository();
            var lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
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
                if (ProjectId > 0 && Mode >= 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase storyboardupload = files[i];
                        if (storyboardupload != null)
                        {
                            var projrepo = new ProjectRepository();
                            var Projectname = projrepo.GetProjectNameById(Convert.ToInt64(ProjectId));
                            string destinationPath = string.Empty;
                            string extension = string.Empty;
                            var uploadFileModel = new List<StoryboardFileUpload>();

                            fileName = Path.GetFileNameWithoutExtension(storyboardupload.FileName);
                            extension = Path.GetExtension(storyboardupload.FileName);
                            fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                            destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                            storyboardupload.SaveAs(destinationPath);
                            MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                            dbtable.errorlog("Import is started", "Import ResultSet", "", 0);
                            var lPath = helper.MasterImport(0, destinationPath, strPath, "RESULTSET", 1, Projectname, Name, Desc, Mode, lSchema, lConnectionStr);

                            if (lPath == false)
                            {
                                dbtable.errorlog("Import is stopped", "Import ResultSet", "", 0);
                                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "RESULTSET");
                                return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                                dbtable.errorlog("Import is completed", "Import ResultSet", "", 0);
                                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "RESULTSET");
                                return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    dbtable.errorlog("ProjectId/Mode is invalid.", "Import ResultSet Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "RESULTSET");
                    return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for ImportResultSets method | Project Id: {0} | Mode : {1} | Name : {2} |  Desc : {3} | UserName: {4}", ProjectId, Mode, Name, Desc, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for ImportResultSets method | Project Id: {0} | Mode : {1} | Name : {2} |  Desc : {3} | UserName: {4}", ProjectId, Mode, Name, Desc, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ImportResultSets method | Project Id: {0} | Mode : {1} | Name : {2} |  Desc : {3} | UserName: {4}", ProjectId, Mode, Name, Desc, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import ResultSet", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "RESULTSET");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);
            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        //This method will export All Storyborad
        public JsonResult ExportAllStoryboradResultSet(int Projectid)
        {
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            var helper = new MARSUtility.ExportExcel();
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
            if (Projectid > 0)
            {
                var projrepo = new ProjectRepository();
                var Projectname = projrepo.GetProjectNameById(Convert.ToInt64(Projectid));
                var repo = new StoryBoardRepository();

                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                string lFileName = Projectname + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                name = "Log_" + Projectname + "_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";

                strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                try
                {
                    MARSUtility.dbtable.errorlog("Export is started", "Export Storyboard Excel", "", 0);
                    var result = helper.ExportAllStoryboradByProjectExcel(Projectname, FullPath, lSchema, lConnectionStr);
                    if (result == true)
                    {
                        MARSUtility.dbtable.errorlog("Export is completed", "Export storyboard Excel", "", 0);
                        objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
                        MARSUtility.dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error occured in StoryBoard for ExportAllStoryboradResultSet method | Project Id: {0} | UserName: {1}", Projectid, SessionManager.TESTER_LOGIN_NAME));
                    ELogger.ErrorException(string.Format("Error occured in StoryBoard for ExportAllStoryboradResultSet method | Project Id: {0} | UserName: {1}", Projectid, SessionManager.TESTER_LOGIN_NAME), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for ExportAllStoryboradResultSet method | Project Id: {0} | UserName: {1}", Projectid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                    int line;
                    string msg = ex.Message;
                    line = MARSUtility.dbtable.lineNo(ex);
                    MARSUtility.dbtable.errorlog("Export stopped", "Export Storyboard Excel", "", 0);
                    objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
                    MARSUtility.dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }
                return Json(lFileName, JsonRequestBehavior.AllowGet);
            }
            MARSUtility.dbtable.errorlog("StoryboardId/ProjectId is invalid.", "Export Storyboard Excel", "", 0);
            objcommon.excel(MARSUtility.dbtable.dt_Log, strPath, "Export", "", "STORYBOARD");
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public class StoryboardFileUpload
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        public void SaveStoryBoardInit(List<StoryBoardResultModel> lobj, Dictionary<String, List<Object>> dlist)
        {
            lobj.ForEach(r => r.status = "nomal");
            if (dlist.ContainsKey("updateList") && dlist["updateList"].Count > 0)
            {
                foreach (var d in dlist["updateList"])
                {
                    var update = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                    var lsbdetailId = update.Where(r => r.Key == "detailid").FirstOrDefault();
                    if (lsbdetailId.Value != null && !string.IsNullOrEmpty(Convert.ToString(lsbdetailId.Value)))
                    {
                        var model = lobj.FirstOrDefault(r => (Convert.ToInt64(lsbdetailId.Value)).Equals(r.storyboarddetailid));
                        if (model != null)
                            model.status = "update";
                    }
                }
            }
            if (dlist.ContainsKey("addList") && dlist["addList"].Count > 0)
            {
                lobj.ForEach(r =>
                {
                    if (r.storyboarddetailid == null || r.storyboarddetailid == 0)
                        r.status = "add";
                });
            }
            if (dlist.ContainsKey("deleteList") && dlist["deleteList"].Count > 0)
            {
                foreach (var d in dlist["deleteList"])
                {
                    var update = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                    var lsbdetailId = update.Where(x => x.Key == "detailid").FirstOrDefault();
                    if (lsbdetailId.Value != null && !string.IsNullOrEmpty(Convert.ToString(lsbdetailId.Value)))
                    {
                        var model = lobj.FirstOrDefault(r => (Convert.ToInt64(lsbdetailId.Value)).Equals(r.storyboarddetailid));
                        if (model != null)
                        {
                            model.status = "delete";
                        }
                        else
                        {
                            var newModel = new StoryBoardResultModel()
                            {
                                Run_order = 0,
                                ProjectId = 0,
                                ApplicationName = "",
                                ProjectName = "",
                                ProjectDescription = "",
                                Storyboardname = "",
                                Storyboardid = 0,
                                ActionName = "",
                                StepName = "",
                                TestSuiteName = "",
                                TestCaseName = "",
                                DataSetName = "",
                                Dependency = "",
                                BTestResult = "",
                                BErrorcause = "",
                                BScriptstart = DateTime.Now,
                                Bstart = "",
                                BScriptend = DateTime.Now,
                                CTestResult = "",
                                CErrorcause = "",
                                CScriptstart = DateTime.Now,
                                Cstart = "",
                                CScriptend = DateTime.Now,
                                Description = "",
                                Suiteid = 0,
                                Caseid = 0,
                                Datasetid = 0,
                                BHistid = 0,
                                CHistid = 0,
                                IsValid = false,
                                ValidationMsg = "",
                                RowId = 0,
                                runtype = 0,
                                Dependson = 0,
                                latestmark = 0,
                                recordvision = 0,
                                status = "delete",
                                storyboarddetailid = Convert.ToInt64(lsbdetailId.Value)
                            };
                            lobj.Add(newModel);
                        }
                    }
                }
            }

        }

        private void SaveStoryBoardDetailToJsonFile(string lProjectId, string lStoryboardId, string storyBoardName, string lSchema, string lConnectionStr, StoryBoardRepository sbRep)
        {
            string key = $"{lProjectId}_{lStoryboardId}_{storyBoardName}.json";
            var jsonFiles = JsonFileHelper.GetFilePath(key, lSchema);
            if (!string.IsNullOrEmpty(jsonFiles))
            {
                ConcurrentDictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(jsonFiles);
                if (keyValuePairs != null && keyValuePairs.ContainsKey("StoryBoardDetails"))
                {
                    var lresult = sbRep.GetStoryBoardDetails(lSchema, lConnectionStr, Convert.ToInt64(lProjectId), Convert.ToInt64(lStoryboardId));
                    var storyBoardDetails = JsonConvert.SerializeObject(lresult);
                    storyBoardDetails = storyBoardDetails.Replace("\\r", "\\\\r");
                    storyBoardDetails = storyBoardDetails.Replace("\\n", "\\\\n");
                    storyBoardDetails = storyBoardDetails.Replace("   ", "");
                    storyBoardDetails = storyBoardDetails.Replace("\\", "\\\\");
                    storyBoardDetails = storyBoardDetails.Trim();
                    keyValuePairs["StoryBoardDetails"] = storyBoardDetails;
                    JsonFileHelper.SaveToJsonFile(JsonConvert.SerializeObject(keyValuePairs), key, lSchema);
                }
            }
        }


        public ActionResult GetStoryBoardAll(long projectid, long storyboardid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lresult = new List<DataSetListByTestCase>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                StoryBoardRepository repo = new StoryBoardRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                Dictionary<string, object> keyValues = new Dictionary<string, object>();
                Task task1 = Task.Run(() =>
                {
                    var datasetList = repo.GetDataSetListNew(projectid);
                    keyValues.Add("datasetList", datasetList);
                });
                Task task2 = Task.Run(() =>
                {
                    var actions = repo.GetActions(storyboardid);
                    keyValues.Add("actions", actions);
                });
                Task task3 = Task.Run(() =>
                {
                    var testSuiteList = repo.GetTestSuites(projectid);
                    keyValues.Add("testSuiteList", testSuiteList);
                });
                Task task4 = Task.Run(() =>
                {
                    var testCaseLists = repo.GetTestCaseListNew(projectid);
                    keyValues.Add("testCaseLists", testCaseLists);
                });
                Task.WaitAll(task1, task2, task3, task4);
                resultModel.status = 1;
                resultModel.data = keyValues;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetDatasetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        private ConcurrentDictionary<string, string> getStoryBoardCache(long projectid, long storyboardid, string storyBoardName)
        {
            string key = $"{projectid}_{storyboardid}_{storyBoardName}.json";

            var result = JsonFileHelper.GetFilePath(key, SessionManager.Schema);
            if (!string.IsNullOrEmpty(result))
            {
                ConcurrentDictionary<string, string> keyValuePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(result);

                return keyValuePairs;
            }

            return null;
        }
    }
}

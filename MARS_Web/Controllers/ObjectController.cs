using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Web.Helper;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using NLog;
using System.Configuration;
using MARSUtility;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class ObjectController : Controller
    {
        MARSUtility.CommonHelper objcommon = new MARSUtility.CommonHelper();
        public ObjectController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        // GET: Object
        #region Crud operation for Object
        public ActionResult ObjectList()
        {
            try
            {
                var repo = new ApplicationRepository();
                var objrepo = new ObjectRepository();
                objrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var list = repo.ListApplication();
                ViewBag.applist = list.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).ToList();

                var typelist = objrepo.LoadObjectType();
                ViewBag.typelist = typelist.Select(c => new SelectListItem { Text = c.typename, Value = c.typeid.ToString() }).OrderBy(x => x.Text).ToList();
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                setObjectGridWidth();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for ObjectList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for ObjectList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ObjectList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //This method get all application 
        public JsonResult ApplicationList()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var objrepo = new ApplicationRepository();
                objrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var list = objrepo.ListApplicationObjectExport();
                resultModel.data = list;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for ApplicationList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for ApplicationList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ApplicationList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method will load by applicationId the data and filter them
        [HttpPost]
        public JsonResult DatLoad(string appId)
        {
            logger.Info(string.Format("Object list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

            string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
            var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
            string colDir = Request.Form.GetValues("order[0][dir]")[0];

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
            startRec = startRec + 1;

            int totalRecords = 0;
            int recFilter = 0;
            var repo = new ObjectRepository();
            repo.Username = SessionManager.TESTER_LOGIN_NAME;
            string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string quickaccess = Request.Form.GetValues("columns[1][search][value]")[0];
            string typesearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string parentsearch = Request.Form.GetValues("columns[3][search][value]")[0];
            var data = new List<Objects>();

            try
            {
                data = repo.ListObjects(lSchema, lConnectionStr, startRec, pageSize, NameSearch, typesearch, quickaccess, parentsearch, colOrder, orderDir, appId);

                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().Totalcount;
                }

                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().Totalcount;
                }
                logger.Info(string.Format("Object list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Object list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for DatLoad method | Application Id : {0} | UserName: {0}", appId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for DatLoad method | Application Id : {0} | UserName: {0}", appId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for DatLoad method | Application Id : {0} | UserName: {0}", appId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);

        }

        //This method will load all the data and filter them
        public ActionResult LoadData()
        {
            setObjectGridWidth();
            return PartialView();
        }

        //Loads dropdown of Pegwindow by ApplicationId in Add/Edit/Saveas Object
        public JsonResult LoadPegwindow(long ApplicationId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.GetPegwindowObject(ApplicationId);

                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for LoadPegwindow method | Application Id : {0} | UserName: {0}", ApplicationId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for LoadPegwindow method | Application Id : {0} | UserName: {0}", ApplicationId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for LoadPegwindow method | Application Id : {0} | UserName: {0}", ApplicationId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Add/Update object values
        public JsonResult AddEditObject(ObjectModel objmodel)
        {
            logger.Info(string.Format("Object Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                if (objmodel.ObjectId == 0)
                {
                    var validationcheck = repo.CheckObjectName(objmodel.ObjectName, objmodel.applicationid, objmodel.ObjectParent, objmodel.ObjectType);
                    if (validationcheck)
                    {
                        resultModel.status = 1;
                        resultModel.data = validationcheck;
                        resultModel.message = "Object: " + objmodel.ObjectName + " already exists in the system";
                        return Json(resultModel, JsonRequestBehavior.AllowGet);
                    }
                }
                var result = repo.AddEditObject(objmodel);
                resultModel.data = result;
                resultModel.status = 1;
                resultModel.message = "Object is saved successfully!!";
                logger.Info(string.Format("Object Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Object Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for AddEditObject method | Object Id : {0} | UserName: {0}", objmodel.ObjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for AddEditObject method | Object Id : {0} | UserName: {0}", objmodel.ObjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for AddEditObject method | Object Id : {0} | UserName: {0}", objmodel.ObjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete the object data by objectid and appid
        public JsonResult DeleteObject(long objectid, long appid,string parent)
        {
            logger.Info(string.Format("Object Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var objectids = repo.FindObjectId(objectid, appid);
                var pegwindowcheck = repo.CheckPegwindowObject(objectid, appid);
                if (pegwindowcheck == "success")
                {
                    if (objectids.Count > 0)
                    {
                        foreach (var itm in objectids)
                        {
                            var testcasename = repo.CheckObjectExistsInTestCase(itm);
                            if (testcasename.Count > 0)
                            {
                                resultModel.data = testcasename;
                                resultModel.status = 1;
                                return Json(resultModel, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    var objectname = repo.GetObjectName(objectid, appid);
                    var result = repo.DeleteObject(objectid, appid, parent);
                    resultModel.data = pegwindowcheck;
                    resultModel.message = "'Object [" + objectname + "] has been deleted.'";
                    logger.Info(string.Format("Object Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Object Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    var pegwindowname = repo.getPegwindowObjectName(objectid, appid);
                    resultModel.message = "Pegwindow object [" + pegwindowname + "] cannot be deleted";
                    resultModel.data = pegwindowcheck;
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for DeleteObject method | Object Id : {0} | Application Id : {1} | Parent : {2} | UserName: {3}", objectid, appid, parent, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for DeleteObject method | Object Id : {0} | Application Id : {1} | Parent : {2} | UserName: {3}", objectid, appid, parent, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for DeleteObject method | Object Id : {0} | Application Id : {1} | Parent : {2} | UserName: {3}", objectid, appid, parent, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Copy objects from one application to another
        //This method will check Converting Object Exists
        public JsonResult CheckConvertingObjectExists(string objectname, long appid, string parentobj, string objecttype)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var exists = repo.CheckConvertingObjectExists(objectname, appid, parentobj, objecttype);
                resultModel.data = exists;
                resultModel.message = exists == "duplicateerror" ? "Object of name[" + objectname + "] already exists" : "";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckConvertingObjectExists method | Object Name : {0} | Application Id : {1} | Parent : {2} | Object Type : {3} | UserName: {4}", objectname, appid, parentobj, objecttype, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckConvertingObjectExists method | Object Name : {0} | Application Id : {1} | Parent : {2} | Object Type : {3} | UserName: {4}", objectname, appid, parentobj, objecttype, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckConvertingObjectExists method | Object Name : {0} | Application Id : {1} | Parent : {2} | Object Type : {3} | UserName: {4}", objectname, appid, parentobj, objecttype, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method will copy all objects
        public JsonResult CopyAllObjects(long copyfromappid, long copytoappid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CopyAllObjects(copyfromappid, copytoappid, lSchema, lConnectionStr);
                resultModel.data = result;
                resultModel.message = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CopyAllObjects method | Old App Id : {0} | New App Id : {1} | UserName: {2}", copyfromappid, copytoappid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for CopyAllObjects method | Old App Id : {0} | New App Id : {1} | UserName: {2}", copyfromappid, copytoappid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CopyAllObjects method | Old App Id : {0} | New App Id : {1} | UserName: {2}", copyfromappid, copytoappid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method will Check Duplicate Object List
        public JsonResult CheckDuplicateObjectList(long copyfromappid, long copytoappid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.DuplicateObjectList(copyfromappid, copytoappid, lConnectionStr, lSchema);
                resultModel.data = result;
                resultModel.message = "success";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckDuplicateObjectList method | Old App Id : {0} | New App Id : {1} | UserName: {2}", copyfromappid, copytoappid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckDuplicateObjectList method | Old App Id : {0} | New App Id : {1} | UserName: {2}", copyfromappid, copytoappid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckDuplicateObjectList method | Old App Id : {0} | New App Id : {1} | UserName: {2}", copyfromappid, copytoappid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method will copy objects
        public JsonResult CopyObjects(List<long> model, long fromid, long toid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                JavaScriptSerializer js = new JavaScriptSerializer();
                var result = repo.CopyObjects(model, fromid, toid, lConnectionStr, lSchema);
                resultModel.data = result;
                resultModel.message = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CopyObjects method | Old Id : {0} | New Id: {1} | UserName: {2}", fromid, toid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for CopyObjects method | Old Id : {0} | New Id: {1} | UserName: {2}", fromid, toid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CopyObjects method | Old Id : {0} | New Id: {1} | UserName: {2}", fromid, toid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Returns all the Object Ids from the selected Application
        public JsonResult GetObjectIdByApp(long appid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ObjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.GetObjectId(appid);
                resultModel.data = result;
                resultModel.message = "success";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjectIdByApp method | Application Id : {0} | UserName: {0}", appid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjectIdByApp method | Application Id : {0} | UserName: {0}", appid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjectIdByApp method | Application Id : {0} | UserName: {0}", appid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export and Import of Objects
        //This method will export object
        public JsonResult ExportObject(string application)
        {

            string name = "Log_Object_Export" + "_" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string lFileName = "Objects" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {

                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);

                MARSUtility.ExportExcel excel = new MARSUtility.ExportExcel();
                MARSUtility.ExportHelper exphelper = new MARSUtility.ExportHelper();


                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                //var repo = new ObjectRepository();
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
                dbtable.errorlog("Export is started", "Export Object Excel", "", 0);
                var result = excel.ExportObjectExcel(application, FullPath, lSchema, lConnectionStr);

                if (result == true)
                {
                    dbtable.errorlog("Export is completed", "Export Object Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", application, "OBJECT");
                    dbtable.dt_Log = null;
                    return Json(lFileName, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for ExportObject method | Application : {0} | UserName: {1}", application, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for ExportObject method | Application : {0} | UserName: {1}", application, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ExportObject method | Application : {0} | UserName: {1}", application, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Export stopped", "Export Object Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", application, "OBJECT");
                dbtable.dt_Log = null;
                return Json(name, JsonRequestBehavior.AllowGet);
            }

            return Json(lFileName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportObjects()
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
                logger.Error(string.Format("Error occured in Object for ImportObjects method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for ImportObjects method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ImportObjects method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //This method will import object file 
        public ActionResult ImportFile()
        {
            string fileName = string.Empty;

            ViewBag.FileName = "";
            string name = "Log_Import" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ImportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase variableupload = files[i];
                    if (variableupload != null)
                    {

                        string destinationPath = string.Empty;
                        string extension = string.Empty;
                        var uploadFileModel = new List<ObjectFile>();
                        MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                        fileName = Path.GetFileNameWithoutExtension(variableupload.FileName);

                        extension = Path.GetExtension(variableupload.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                        destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                        variableupload.SaveAs(destinationPath);

                        // string TempFileLocation = WebConfigurationManager.AppSettings["VariableTemplateLocation"];
                        //name = "Log_"+ Path.GetFileNameWithoutExtension(variableupload.FileName) +"_Import" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";

                        // strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                        string lSchema = SessionManager.Schema;
                        var lConnectionStr = SessionManager.APP;
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
                        dbtable.errorlog("Import is started", "Import Object", "", 0);
                        // var lPath = ImportHelper.ImportObject(destinationPath, lConnectionStr, lSchema, Server.MapPath("~/Temp/"), SessionManager.TESTER_LOGIN_NAME);
                        var lPath = helper.MasterImport(0, destinationPath, strPath, "OBJECT", 1, "", "", "", 1, lSchema, lConnectionStr);

                        if (lPath == false)
                        {
                            //fileName = strPath + ".xlsx";
                            dbtable.errorlog("Import is stopped", "Import Object", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "OBJECT");
                            return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            //fileName = ".xlsx";
                            dbtable.errorlog("Import is completed", "Import Object", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "OBJECT");
                            return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for ImportFile method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for ImportFile method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ImportFile method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import Object", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "OBJECT");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);
                //if (ex.Message.Contains("ORA-01403"))
                //   return Json("Data not found",JsonRequestBehavior.AllowGet);
                //else
                //    return Json("Excel file is not valid.", JsonRequestBehavior.AllowGet);



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
        public class ObjectFile
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
        #endregion

        public void setObjectGridWidth()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repAcc.GetGridList((long)userId, GridNameList.ObjectList);
                var objgriddata = GridHelper.GetObjectwidth(gridlst);

                ViewBag.namewidth = objgriddata.Name == null ? "20%" : objgriddata.Name.Trim() + '%';
                ViewBag.internalAccesswidth = objgriddata.InternalAccess == null ? "30%" : objgriddata.InternalAccess.Trim() + '%';
                ViewBag.typewidth = objgriddata.Type == null ? "10%" : objgriddata.Type.Trim() + '%';
                ViewBag.pegwindowwidth = objgriddata.Pegwindow == null ? "20%" : objgriddata.Pegwindow.Trim() + '%';
                ViewBag.actionswidth = objgriddata.Actions == null ? "10%" : objgriddata.Actions.Trim() + '%';
                ViewBag.selectwidth = objgriddata.Select == null ? "10%" : objgriddata.Select.Trim() + '%';
            }
            catch(Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for setObjectGridWidth method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for setObjectGridWidth method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for setObjectGridWidth method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
        }

        public JsonResult RefreshCache()
        {
            logger.Info(string.Format("RefreshCache start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));

            string BaseURL = WebConfigurationManager.AppSettings["RefreshCacheURL"];
           // string URL = "http://mars11.eastus.cloudapp.azure.com:8051/api/ListCompareConfig";
            string URL = BaseURL + "/MarsEngine/RefreshCache?typeId=All&currentDBIdx=" + SessionManager.Schema;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(URL).Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result; 
                }
                logger.Info(string.Format("RefreshCache end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for RefreshCache method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Object for RefreshCache method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for RefreshCache method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}


using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;

using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using MARSUtility;
//using static MARS_Web.Helper.ImportHelper;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class VariableController : Controller
    {
        MARSUtility.CommonHelper objcommon = new MARSUtility.CommonHelper();
        public VariableController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        #region Crud operations for Variable

        //renders partial view
        public ActionResult VariableList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repAcc.GetGridList((long)userId, GridNameList.VaribleList);
                var vargriddata = GridHelper.GetVariblewidth(gridlst);
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.namewidth = vargriddata.Name == null ? "30%" : vargriddata.Name.Trim() + '%';
                ViewBag.typewidth = vargriddata.Type == null ? "20%" : vargriddata.Type.Trim() + '%';
                ViewBag.valuewidth = vargriddata.Value == null ? "20%" : vargriddata.Value.Trim() + '%';
                ViewBag.statuswidth = vargriddata.Status == null ? "20%" : vargriddata.Status.Trim() + '%';
                ViewBag.actionswidth = vargriddata.Actions == null ? "10%" : vargriddata.Actions.Trim() + '%';
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for VariableList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for VariableList method |UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for VariableList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //Gets variable list
        [HttpPost]
        public JsonResult DataLoad()
        {
            logger.Info(string.Format("variable list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var repAcc = new VariableRepository();
            repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
            int totalRecords = default(int);
            int recFilter = default(int);
            //assign values to local variable
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

            string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
            var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
            string colDir = Request.Form.GetValues("order[0][dir]")[0] + colOrder;

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
            var data = new List<VariableModel>();
            startRec = startRec + 1;
            string FieldNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string TableSearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string Displaynamesearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string statussearch = Request.Form.GetValues("columns[3][search][value]")[0];

            try
            {
                //Gets variable list
                data = repAcc.GetVariables(lSchema, lConnectionStr, startRec, pageSize, FieldNameSearch, TableSearch, Displaynamesearch, statussearch, colOrder, orderDir);

                //Gets count of total records
                totalRecords = 0;
                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().TotalCount;
                }
                //gets count of filtered records
                recFilter = 0;
                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().TotalCount;
                }
                logger.Info(string.Format("variable list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("variable list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for Dataload method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for Dataload method |UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for Dataload method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            //returns data
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data,
            }, JsonRequestBehavior.AllowGet);
        }

        //Adds/Edits a variable
        [HttpPost]
        public ActionResult AddEditVariableSave(VariableModel model)
        {
            logger.Info(string.Format("Variable Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                VariableRepository repo = new VariableRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = string.Empty;

                if (!string.IsNullOrEmpty(model.Table_name))
                {
                    result = repo.CheckVariable(model);
                }
                if(result == string.Empty)
                {
                    var lresult = repo.AddEditVariable(model);
                    resultModel.message = "Successfully Saved variable [" + model.field_name + "]";
                    resultModel.data = lresult;
                    
                    logger.Info(string.Format("Variable Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Variable Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    resultModel.message = result;
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for AddEditVariableSave method | Variable Id : {0} | UserName: {1}", model.Lookupid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for AddEditVariableSave method |  Variable Id : {0} | UserName: {1}", model.Lookupid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for AddEditVariableSave method |  Variable Id : {0} | UserName: {1}", model.Lookupid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Deletes a variable
        public JsonResult DeleteVariable(int lookupid)
        {
            logger.Info(string.Format("varible Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new VariableRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lresult = repo.DeleteVariable(lookupid);
                Session["VariableDeleteMsg"] = "Successfully Variable is Deleted.";

                resultModel.message = "success";
                resultModel.data = lresult;
                resultModel.status = 1;
                logger.Info(string.Format("varible Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("varible Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for DeleteVariable method | Variable Id : {0} | UserName: {1}", lookupid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for DeleteVariable method |  Variable Id : {0} | UserName: {1}", lookupid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for DeleteVariable method |  Variable Id : {1} | UserName: {1}", lookupid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Loads dropdown of Type and baseline/compare in Add/Edit Variable

        public JsonResult LoadTableName()
        {
            //Dropdown values
            var result = new List<SelectListItem> { new SelectListItem() {  Text="GLOBAL_VAR",Value="1"},
                new SelectListItem() { Text="MODAL_VAR",Value="2"},
                new SelectListItem() { Text="LOOP_VAR",Value="3"}

            }.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //get Baseline/Compare data by id
        public JsonResult GetBaselineCompare(int id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                resultModel.data = new List<SelectListItem> {
                new SelectListItem() { Text="BASELINE",Value="1"},
                new SelectListItem() { Text="COMPARE",Value="2"},
                new SelectListItem() { Text="",Value="0" }
            };
                resultModel.status = 1;
            }
            catch(Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for GetBaselineCompare method |  UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for GetBaselineCompare method |UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for GetBaselineCompare method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks Variable with same name exists in the system or not and returns result
        //Check Variable name already exist or not
        [HttpPost]
        public JsonResult CheckDuplicateVariableExist(string Varname, long Varid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new VariableRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDuplicateVariableName(Varid, Varname);
                resultModel.status = 1;
                resultModel.message = "success";
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for CheckDuplicateVariableExist method | Variable Id: {0} | Variable Name : {1} | UserName: {0}", Varid, Varname, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for CheckDuplicateVariableExist method | Variable Id: {0} | Variable Name : {1} | UserName: {0}", Varid, Varname, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for CheckDuplicateVariableExist method | Variable Id: {0} | Variable Name : {1} | UserName: {0}", Varid, Varname, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export and Import of Variables

        //renders partial view
        public ActionResult ImportVariables()
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
                logger.Error(string.Format("Error occured when variable page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when variable page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView();
        }
        //Downloads Exported variable file

        public ActionResult Importvariable()
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
                    HttpPostedFileBase variableupload = files[i];
                    if (variableupload != null)
                    {

                        string destinationPath = string.Empty;
                        string extension = string.Empty;
                        var uploadFileModel = new List<VariableFileUpload>();
                        MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                        fileName = Path.GetFileNameWithoutExtension(variableupload.FileName);

                        extension = Path.GetExtension(variableupload.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                        destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                        variableupload.SaveAs(destinationPath);
                       
                        string lSchema = SessionManager.Schema;
                        var lConnectionStr = SessionManager.APP;
                        dbtable.errorlog("Import is started", "Import Variable", "", 0);
                        // var lPath = ImportVariable(destinationPath, lConnectionStr, lSchema, Server.MapPath("~/Temp/"), SessionManager.TESTER_LOGIN_NAME);
                        var lPath = helper.MasterImport(0, destinationPath, strPath, "VARIABLE", 1, "", "", "", 1, lSchema, lConnectionStr);

                        if (lPath == false)
                        {
                            dbtable.errorlog("Import is stopped", "Import Variable", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "VARIABLE");
                            return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dbtable.errorlog("Import is completed", "Import Object", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "OBJECT");
                            return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                        }
                       
                       
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for Importvariable method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for Importvariable method |UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for Importvariable method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import Variable", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "VARIABLE");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);
               
            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }
        public FileResult DownloadFile(string path)
        {
            ViewBag.FileName = "";
            var result = "";
            //Path = Path.Replace(".xls", "");
            var byteArray = System.IO.File.ReadAllBytes(path);
            var ms = new MemoryStream(byteArray);
            result = Path.GetFileName(path);
            return new FileStreamResult(ms, "application/ms-excel")
            {
                FileDownloadName =result
            };

        }

        public JsonResult ExportVariable()
        {
            string lFileName = "variable" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
            string name = "Log_Variable_Import" + "_" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var repAcc = new VariableRepository();
                var exp = new MARSUtility.ExportExcel();
              
                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
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
                dbtable.errorlog("Export is started", "Export Variable Excel", "", 0);
                var presult = exp.ExportVariableExcel(FullPath, lSchema, lConnectionStr);
                if (presult == true)
                {
                    dbtable.errorlog("Export is completed", "Export variable Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "VARIABLE");
                    dbtable.dt_Log = null;
                    return Json(lFileName, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Variables for ExportVariable method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Variables for ExportVariable method |UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Variables for ExportVariable method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Export stopped", "Export Variable Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "VARIABLE");
                dbtable.dt_Log = null;
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public class VariableFileUpload
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
        #endregion
    }
}

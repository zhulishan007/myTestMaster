using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using MARSUtility;
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

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class CompareConfigController : Controller
    {
        MARSUtility.CommonHelper objcommon = new MARSUtility.CommonHelper();
        public CompareConfigController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        
        public JsonResult ExportCompareConfig()
        {
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
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

            ExportExcel excel = new ExportExcel();

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;

            string lFileName = "COMPARECONFIG_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
            string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
            name = "Log_COMPARECONFIG_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                dbtable.errorlog("Export is started", "Export CompareConfig Excel", "", 0);

                var presult = excel.ExportConfigExcel(FullPath, lSchema, lConnectionStr);

                if (presult == true)
                {
                    dbtable.errorlog("Export is completed", "Export CompareConfig Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "COMPARECONFIG");
                    dbtable.dt_Log = null;
                    return Json(lFileName, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Export stopped", "Export CompareConfig Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "CONFIG");
                dbtable.dt_Log = null;
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            return Json(lFileName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportCompareConfig()
        {
            var userId = SessionManager.TESTER_ID;
            var repAcc = new ConfigurationGridRepository();
            repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
            var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
            var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

            ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            return PartialView();
        }

        //This method will import CompareConfig file 
        public ActionResult ImportCompareConfigFile()
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
                    HttpPostedFileBase configupload = files[i];
                    if (configupload != null)
                    {

                        string destinationPath = string.Empty;
                        string extension = string.Empty;
                        var uploadFileModel = new List<CompareConfigFileUpload>();
                        MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                        fileName = Path.GetFileNameWithoutExtension(configupload.FileName);

                        extension = Path.GetExtension(configupload.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                        destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                        configupload.SaveAs(destinationPath);

                        string lSchema = SessionManager.Schema;
                        var lConnectionStr = SessionManager.APP;
                        dbtable.errorlog("Import is started", "Import CompareConfig", "", 0);
                        var lPath = helper.MasterImport(0, destinationPath, strPath, "COMPARECONFIG", 1, "", "", "", 1, lSchema, lConnectionStr);

                        if (lPath == false)
                        {
                            dbtable.errorlog("Import is stopped", "Import CompareConfig", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "COMPARECONFIG");
                            return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dbtable.errorlog("Import is completed", "Import CompareConfig", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "COMPARECONFIG");
                            return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import CompareConfig", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "COMPARECONFIG");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);

            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        public class CompareConfigFileUpload
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
    }
}
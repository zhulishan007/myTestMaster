using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class KeywordController : Controller
    {
        public KeywordController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        public string GetLogPathLocation()
        {
            string logPath = WebConfigurationManager.AppSettings["LogPathLocation"];
            return System.Web.HttpContext.Current.Server.MapPath("~/" + logPath + "/");
        }
        #region Crud operations for Keyword
        public ActionResult KeywordList()
        {
            string currentPath = GetLogPathLocation();
            try
            {
                KeywordRepository repo = new KeywordRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.currentPath = currentPath;
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repAcc.GetGridList((long)userId, GridNameList.KeywordList);
                var keygriddata = GridHelper.GetKeywordwidth(gridlst);

                var lapp = repo.ListOfKeywordType();
                var typelist = lapp.Select(c => new SelectListItem { Text = c.TYPE_NAME, Value = c.TYPE_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.listtyps = typelist;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.namewidth = keygriddata.Name == null ? "30%" : keygriddata.Name.Trim() + '%';
                ViewBag.controlTypewidth = keygriddata.ControlType == null ? "40%" : keygriddata.ControlType.Trim() + '%';
                ViewBag.entryDatawidth = keygriddata.EntryData == null ? "20%" : keygriddata.EntryData.Trim() + '%';
                ViewBag.actionswidth = keygriddata.Actions == null ? "10%" : keygriddata.Actions.Trim() + '%';
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Keyword for KeywordList method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for KeywordList method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
            }
            return PartialView("KeywordList");
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoad()
        {
            string currentPath = GetLogPathLocation();
            MARS_Repository.Helper.WriteLogMessage(string.Format("Keyword list open start | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
            var repp = new KeywordRepository();
            repp.Username = SessionManager.TESTER_LOGIN_NAME;
            repp.currentPath = currentPath;
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
            string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string ControlTypeSearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string EntryFileSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            var data = new List<KeywordViewModel>();
            int totalRecords = 0;
            int recFilter = 0;
            try
            {
                data = repp.ListAllKeyword(lSchema, lConnectionStr, startRec, pageSize, colOrder, orderDir, NameSearch, ControlTypeSearch, EntryFileSearch);

                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().TotalCount;
                }

                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().TotalCount;
                }
                MARS_Repository.Helper.WriteLogMessage(string.Format("keyword list open end | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("keyword list open successfully | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Keyword for DataLoad method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for DataLoad method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Add/Update Keyword objects values
        [HttpPost]
        public JsonResult AddEditKeyword(KeywordViewModel lModel)
        {
            string currentPath = GetLogPathLocation();
            MARS_Repository.Helper.WriteLogMessage(string.Format("keyword Add/Edit  Modal open | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new KeywordRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.currentPath = currentPath;
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                var lResult = repo.AddEditKeyword(lModel);
                var repTree = new GetTreeRepository();
                Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.message = "Successfully saved Keyword [" + lModel.KeywordName + "].";
                resultModel.data = lResult;
                resultModel.status = 1;
                MARS_Repository.Helper.WriteLogMessage(string.Format("keyword Add/Edit  Modal close | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("keyword Save successfully | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Keyword for AddEditKeyword method | KeywordId : {0} | UserName: {1} | Error: {2}", lModel.KeywordId, SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for AddEditKeyword method | KeywordId : {0} | UserName: {1} | Error: {2}", lModel.KeywordId, SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete the Keyword object data by Keywordid
        public ActionResult DeletKeyword(long Keywordid)
        {
            string currentPath = GetLogPathLocation();
            MARS_Repository.Helper.WriteLogMessage(string.Format("keyword Delete start | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
            ResultModel resultModel = new ResultModel();
            try
            {
                var keywordname = string.Empty;
                var repo = new KeywordRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.currentPath = currentPath;
                var repTree = new GetTreeRepository();
                var lflag = repo.CheckTestCaseExistsInKeyword(Keywordid);
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                keywordname = repo.GetKeywordById(Keywordid);

                if (lflag.Count <= 0)
                {
                    var result = repo.DeleteKeyword(Keywordid);
                    Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                    resultModel.message = "Keyword [" + keywordname + "] has been deleted.";
                    resultModel.data = result;
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.message = keywordname;
                    resultModel.data = lflag;
                    resultModel.status = 1;
                }
                MARS_Repository.Helper.WriteLogMessage(string.Format("keyword Delete end | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("keyword Delete successfully | Username: {0}",SessionManager.TESTER_LOGIN_NAME), currentPath);
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Keyword for DeletKeyword method | KeywordId : {0} | UserName: {1} | Error: {2}", Keywordid, SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for DeletKeyword method | KeywordId : {0} | UserName: {1} | Error: {2}", Keywordid, SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks whether the keyword with same name exists in the system or not
        //Check Keyword name already exist or not
        public JsonResult CheckDuplicateKeywordNameExist(string keywordname, long? KeywordId)
        {
            string currentPath = GetLogPathLocation();
            ResultModel resultModel = new ResultModel();
            try
            {
                keywordname = keywordname.Trim();
                var repo = new KeywordRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.currentPath = currentPath;
                var result = repo.CheckDuplicateKeywordNameExist(keywordname, KeywordId);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Keyword for CheckDuplicateKeywordNameExist method | KeywordId : {0} Keyword Name : {1} | UserName: {2} | Error: {3}", KeywordId, keywordname, SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for CheckDuplicateKeywordNameExist method | KeywordId : {0} Keyword Name : {1} | UserName: {2} | Error: {3}", KeywordId, keywordname, SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}

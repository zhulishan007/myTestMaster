using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using Mars_Serialization.ViewModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using static Mars_Serialization.JsonSerialization.SerializationFile;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class ApplicationController : Controller
    {

        public ApplicationController()
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
        #region Crud operations for Application
        public ActionResult ApplicationList()
        {
            string currentPath = GetLogPathLocation();
            try
            {
                /*var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repacc.GetGridList((long)userid, GridNameList.ApplicationList);
                var appgriddata = GridHelper.GetApplicationwidth(gridlst);
                var appgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(appgridlst);
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.namewidth = appgriddata.Name == null ? "20%" : appgriddata.Name.Trim() + '%';
                ViewBag.descriptionwidth = appgriddata.Description == null ? "20%" : appgriddata.Description.Trim() + '%';
                ViewBag.versionwidth = appgriddata.Version == null ? "10%" : appgriddata.Version.Trim() + '%';
                ViewBag.extrarequirementwidth = appgriddata.ExtraRequirement == null ? "20%" : appgriddata.ExtraRequirement.Trim() + '%';
                ViewBag.modewidth = appgriddata.Mode == null ? "10%" : appgriddata.Mode.Trim() + '%';
                ViewBag.bitswidth = appgriddata.ExplorerBits == null ? "10%" : appgriddata.ExplorerBits.Trim() + '%';
                ViewBag.actionswidth = appgriddata.Actions == null ? "10%" : appgriddata.Actions.Trim() + '%';*/

                ViewBag.width =   ConfigurationManager.AppSettings["DefultLeftPanel"] + "px"  ;
                ViewBag.namewidth =  "20%"  ;
                ViewBag.descriptionwidth ="20%";
                ViewBag.versionwidth =  "10%"  ;
                ViewBag.extrarequirementwidth = "20%"; ;
                ViewBag.modewidth =  "10%" ;
                ViewBag.bitswidth ="10%" ;
                ViewBag.actionswidth ="10%" ;
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Application for ApplicationList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Application for ApplicationList method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException: Error occured in Application for ApplicationList method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
            }
            return PartialView("ApplicationList");
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoad()
        {
            string currentPath = GetLogPathLocation();
            //Get Repository
            MARS_Repository.Helper.WriteLogMessage(string.Format("Application list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
            //var _apprepository = new ApplicationRepository();
            //_apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
            //_apprepository.currentPath = currentPath;
            List<MARS_Repository.ViewModel.ApplicationViewModel> data = new List<MARS_Repository.ViewModel.ApplicationViewModel>();
            int totalRecords = default(int);
            int recFilter = default(int);
            //Assign values in local variables
            #region Variables
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
            var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
            string colDir = Request.Form.GetValues("order[0][dir]")[0];
            string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string DescriptionSearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string VersionSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string ExtraSearch = Request.Form.GetValues("columns[3][search][value]")[0];
            string StatusSearch = Request.Form.GetValues("columns[4][search][value]")[0];
            string bitsearch = Request.Form.GetValues("columns[5][search][value]")[0];
            #endregion
            try
            {
                //Get data from List Application Object
                #region Getdata
                //data = _apprepository.GetApplicationList();
                string fullPath = Path.Combine(Server.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Application.ToString(), SessionManager.Schema, "application.json");
                if (System.IO.File.Exists(fullPath))
                {
                    string jsongString = System.IO.File.ReadAllText(fullPath);
                    var allList = JsonConvert.DeserializeObject<List<T_Memory_REGISTERED_APPS>>(jsongString);
                    if (allList.Count() > 0)
                    {
                        data = allList.Select(x => new MARS_Repository.ViewModel.ApplicationViewModel()
                        {
                            ApplicationId = x.APPLICATION_ID,
                            ApplicationName = x.APP_SHORT_NAME,
                            Description = x.PROCESS_IDENTIFIER ?? string.Empty,
                            Version = x.VERSION ?? string.Empty,
                            ExtraRequirement = x.EXTRAREQUIREMENT ?? string.Empty,
                            Mode = (x.ISBASELINE != null) ? x.ISBASELINE == 1 ? "Baseline" : "Compare" : string.Empty,
                            Bits = (x.IS64BIT != null) ? x.IS64BIT == 1 ? "64 bits" : "32 bits" : string.Empty,
                            BitsId = x.IS64BIT == null ? "" : x.IS64BIT.ToString(),
                            STARTER_COMMAND = x.STARTER_COMMAND
                        }).ToList();
                    }
                }
                #endregion

                //Check Variables Value 
                #region CheckValues               
                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ApplicationName) && x.ApplicationName.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DescriptionSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.ToString().ToLower().Contains(DescriptionSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(VersionSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Version) && x.Version.ToLower().Trim().Contains(VersionSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(ExtraSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ExtraRequirement) && x.ExtraRequirement.ToLower().Trim().Contains(ExtraSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(StatusSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Mode) && x.Mode.ToLower().Trim().Contains(StatusSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(bitsearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Bits) && x.Bits.ToLower().Trim().Contains(bitsearch.ToLower().Trim())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.ApplicationName).ToList();
                            break;
                        case "Description":
                            data = data.OrderByDescending(a => a.Description).ToList();
                            break;
                        case "Version":
                            data = data.OrderByDescending(a => a.Version).ToList();
                            break;
                        case "Extra Requirement":
                            data = data.OrderByDescending(a => a.ExtraRequirement).ToList();
                            break;
                        case "Mode":
                            data = data.OrderByDescending(a => a.Mode).ToList();
                            break;
                        case "MARS Explorer Bits":
                            data = data.OrderByDescending(a => a.Bits).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.ApplicationName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.ApplicationName).ToList();
                            break;
                        case "Description":
                            data = data.OrderBy(a => a.Description).ToList();
                            break;
                        case "Version":
                            data = data.OrderBy(a => a.Version).ToList();
                            break;
                        case "Extra Requirement":
                            data = data.OrderBy(a => a.ExtraRequirement).ToList();
                            break;
                        case "Mode":
                            data = data.OrderBy(a => a.Mode).ToList();
                            break;
                        case "MARS Explorer Bits":
                            data = data.OrderBy(a => a.Bits).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.ApplicationName).ToList();
                            break;
                    }
                }
                #endregion

                //Get Total Records
                totalRecords = data.Count();

                //Apply Search
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    data = data.Where(p => p.ApplicationName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Version.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ExtraRequirement.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Bits.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Mode.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }

                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
                MARS_Repository.Helper.WriteLogMessage(string.Format("Application list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("Application list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Application for DataLoad method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException: Error occured in Application for DataLoad method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);
            }
            //Return Result in Json Formate
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Add/Update Application objects values
        [HttpPost]
        public JsonResult AddEditApplication(MARS_Repository.ViewModel.ApplicationViewModel applicationviewmodel)
        {
            string currentPath = GetLogPathLocation();

            MARS_Repository.Helper.WriteLogMessage(string.Format("Application Add/Edit  Modal open | Application Id : {0} | UserName: {1}", applicationviewmodel.ApplicationId, SessionManager.TESTER_LOGIN_NAME), currentPath);
            ResultModel resultModel = new ResultModel();
            try
            {

                var _apprepository = new ApplicationRepository();
                _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
                _apprepository.currentPath = currentPath;
                applicationviewmodel.Create_Person = SessionManager.TESTER_LOGIN_NAME;

                var appObj = new T_Memory_REGISTERED_APPS
                {
                    APP_SHORT_NAME = applicationviewmodel.ApplicationName,
                    PROCESS_IDENTIFIER = applicationviewmodel.Description,
                    VERSION = applicationviewmodel.Version,
                    EXTRAREQUIREMENT = applicationviewmodel.ExtraRequirement,
                    RECORD_CREATE_PERSON = SessionManager.TESTER_LOGIN_NAME,
                    RECORD_CREATE_DATE = DateTime.Now,
                    STARTER_COMMAND = applicationviewmodel.STARTER_COMMAND,
                    ISBASELINE = !string.IsNullOrEmpty(applicationviewmodel.Mode) ? applicationviewmodel.Mode == "Baseline" ? 1 : 0 : 0,
                    IS64BIT = !string.IsNullOrEmpty(applicationviewmodel.BitsId) ? Convert.ToInt32(applicationviewmodel.BitsId) : 0,
                    APPLICATION_ID = applicationviewmodel.ApplicationId != 0
                                                ? applicationviewmodel.ApplicationId
                                                : _apprepository.GetApplicationSequence("T_REGISTERED_APPS_SEQ"),
                    currentSyncroStatus = applicationviewmodel.ApplicationId != 0
                                                ? Mars_Serialization.Common.CommonEnum.MarsRecordStatus.en_ModifiedToDb
                                                : Mars_Serialization.Common.CommonEnum.MarsRecordStatus.en_NewToDb
                };

                if (applicationviewmodel.ApplicationId == 0)
                {
                    GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value.Add(appObj);
                    if (GlobalVariable.AppListCache != null && GlobalVariable.AppListCache.ContainsKey(SessionManager.Schema))
                    {
                        var RegisterTbl = new T_REGISTERED_APPS
                        {
                            APPLICATION_ID = appObj.APPLICATION_ID,
                            APP_SHORT_NAME = appObj.APP_SHORT_NAME,
                            PROCESS_IDENTIFIER = appObj.PROCESS_IDENTIFIER,
                            VERSION = appObj.VERSION,
                            RECORD_CREATE_PERSON = appObj.RECORD_CREATE_PERSON,
                            EXTRAREQUIREMENT = appObj.EXTRAREQUIREMENT,
                            RECORD_CREATE_DATE = appObj.RECORD_CREATE_DATE,
                            ISBASELINE = appObj.ISBASELINE,
                            IS64BIT = appObj.IS64BIT,
                            STARTER_COMMAND = appObj.STARTER_COMMAND
                        };
                        GlobalVariable.AppListCache[SessionManager.Schema].Add(RegisterTbl);
                    }
                }
                else
                {
                    var app = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value;
                    if (app.Count > 0)
                    {
                        var singleApp = app.FirstOrDefault(x => x.APPLICATION_ID == applicationviewmodel.ApplicationId);
                        if (singleApp != null)
                        {
                            GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value.Remove(singleApp);
                            GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value.Add(appObj);
                        }
                    }
                    if (GlobalVariable.AppListCache != null && GlobalVariable.AppListCache.ContainsKey(SessionManager.Schema))
                    {
                        var singleApp = GlobalVariable.AppListCache[SessionManager.Schema].FirstOrDefault(x => x.APPLICATION_ID == applicationviewmodel.ApplicationId);
                        if (singleApp != null)
                        {
                            singleApp.APP_SHORT_NAME = appObj.APP_SHORT_NAME;
                            singleApp.PROCESS_IDENTIFIER = appObj.PROCESS_IDENTIFIER;
                            singleApp.VERSION = appObj.VERSION;
                            singleApp.RECORD_CREATE_PERSON = appObj.RECORD_CREATE_PERSON;
                            singleApp.EXTRAREQUIREMENT = appObj.EXTRAREQUIREMENT;
                            singleApp.RECORD_CREATE_DATE = appObj.RECORD_CREATE_DATE;
                            singleApp.ISBASELINE = appObj.ISBASELINE;
                            singleApp.IS64BIT = appObj.IS64BIT;
                            singleApp.STARTER_COMMAND = appObj.STARTER_COMMAND;
                        }
                     }
                }
                #region Reload Application file
                var AllApps = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value;
                ReloadApplicationFile(AllApps, Server.MapPath("~/"), SessionManager.Schema);
                #endregion

                //applicationviewmodel.ApplicationId = applicationviewmodel.ApplicationId != 0 ? applicationviewmodel.ApplicationId : appObj.APPLICATION_ID;
                //var _addeditResult = _apprepository.AddEditApplication(applicationviewmodel);
                Thread AddEditApp = new Thread(delegate ()
                {
                    bool _addeditResult = _apprepository.AddEditApplicationFromDictionary(appObj);

                })
                {
                    IsBackground = true
                };
                AddEditApp.Start();

                List<MARS_Repository.ViewModel.ProjectByUser> projects = new List<MARS_Repository.ViewModel.ProjectByUser>();
                if (InitCacheHelper.GetProjectUserFromCache(SessionManager.Schema, SessionManager.TESTER_ID, projects))
                {
                    Session["LeftProjectList"] = projects.OrderBy(r => r.ProjectName).ToList();
                }
                else
                {
                    var _treerepository = new GetTreeRepository();
                    Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, SessionManager.Schema, SessionManager.APP);
                }
                //var _treerepository = new GetTreeRepository();
                //Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, SessionManager.Schema, SessionManager.APP);

                resultModel.message = "Saved Application [" + applicationviewmodel.ApplicationName + "].";
                resultModel.data = true;
                resultModel.status = 1;
                MARS_Repository.Helper.WriteLogMessage(string.Format("Application Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("Application Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Application for AddEditApplication method | Application Id : {0} | UserName: {1}", applicationviewmodel.ApplicationId, SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException: Error occured in Application for AddEditApplication method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete the Application object data by applicationID
        public ActionResult DeletApplication(long ApplicationId)
        {
            string currentPath = GetLogPathLocation();

            MARS_Repository.Helper.WriteLogMessage(string.Format("Application Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);

            ResultModel resultModel = new ResultModel();
            try
            {
                var _apprepository = new ApplicationRepository();
                var _treerepository = new GetTreeRepository();
                _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
                _apprepository.currentPath = currentPath;
                var lflag = _apprepository.CheckTestCaseExistsInAppliction(ApplicationId);

                if (lflag.Count <= 0)
                {
                    var Applicationname = _apprepository.GetApplicationNameById(ApplicationId);

                    var app = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value;
                    if (app.Count > 0)
                    {
                        var singleApp = app.FirstOrDefault(x => x.APPLICATION_ID == ApplicationId);
                        if (singleApp != null)
                        {
                            GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value.Remove(singleApp);
                        }
                    }
                    #region Reload Application file
                    var AllApps = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value;
                    ReloadApplicationFile(AllApps, Server.MapPath("~/"), SessionManager.Schema);
                    #endregion

                    Thread DeleteApp = new Thread(delegate ()
                    {
                        bool _deleteResult = _apprepository.DeleteApplication(ApplicationId);
                    })
                    {
                        IsBackground = true
                    };
                    DeleteApp.Start();
                    if (GlobalVariable.AppListCache != null && GlobalVariable.AppListCache.ContainsKey(SessionManager.Schema)) 
                    {
                        GlobalVariable.AppListCache[SessionManager.Schema].RemoveAll(r => r == null || r.APPLICATION_ID == ApplicationId);
                    }
                    List<MARS_Repository.ViewModel.ProjectByUser> projects = new List<MARS_Repository.ViewModel.ProjectByUser>();
                    if (InitCacheHelper.GetProjectUserFromCache(SessionManager.Schema, SessionManager.TESTER_ID, projects))
                    {
                        Session["LeftProjectList"] = projects.OrderBy(r => r.ProjectName).ToList();
                    }
                    else
                    {
                         Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, SessionManager.Schema, SessionManager.APP);
                    }
                    //Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, SessionManager.Schema, SessionManager.APP);
                    resultModel.data = "success";
                    resultModel.message = "Application [" + Applicationname + "] has been deleted.";
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.data = lflag;
                    resultModel.status = 1;
                }
                MARS_Repository.Helper.WriteLogMessage(string.Format("Application Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
                MARS_Repository.Helper.WriteLogMessage(string.Format("Application Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME), currentPath);
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Application for DeletApplication method | UserName: {0} | Error: {1}", SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks whether the Application already exists in the system or not
        //Check application name already exist or not
        public JsonResult CheckDuplicateApplicationNameExist(string applicationname, long? ApplicationId)
        {
            string currentPath = GetLogPathLocation();
            ResultModel resultModel = new ResultModel();
            try
            {
                applicationname = applicationname.Trim();
                var _apprepository = new ApplicationRepository();
                _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
                _apprepository.currentPath = currentPath;
                var result = false;
                if (GlobalVariable.AppListCache != null && GlobalVariable.AppListCache.ContainsKey(SessionManager.Schema))
                {
                    if (ApplicationId != null)
                        result = GlobalVariable.AppListCache[SessionManager.Schema].Any(a => a != null && a.APPLICATION_ID!= ApplicationId && a.APP_SHORT_NAME.ToLower().Trim() == applicationname.ToLower().Trim());
                    else
                        result = GlobalVariable.AppListCache[SessionManager.Schema].Any(a => a != null && a.APP_SHORT_NAME.ToLower().Trim() == applicationname.ToLower().Trim());

                }
                else
                {
                    result = _apprepository.CheckDuplicateApplicationNameExist(applicationname, ApplicationId);
                }
                resultModel.status = 1;
                resultModel.message = "success";
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                MARS_Repository.Helper.WriteLogMessage(string.Format("Error occured in Application for CheckDuplicateApplicationNameExist method | Application Id : {0} |Application Name: {1} | UserName: {2} | Error: {3}", ApplicationId, applicationname, SessionManager.TESTER_LOGIN_NAME, ex.ToString()), currentPath);

                if (ex.InnerException != null)
                    MARS_Repository.Helper.WriteLogMessage(string.Format("InnerException: Error occured in Application for CheckDuplicateApplicationNameExist method | Application Id : {0} |Application Name: {1} | UserName: {2} | Error: {3}", ApplicationId, applicationname, SessionManager.TESTER_LOGIN_NAME, ex.InnerException.ToString()), currentPath);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetApplicationList()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                logger.Info(string.Format("Load GetApplicationList start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                GetTreeRepository repo = new GetTreeRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                
                if (GlobalVariable.AppListCache != null && GlobalVariable.AppListCache.ContainsKey(SessionManager.Schema))
                {
                    resultModel.data = GlobalVariable.AppListCache[SessionManager.Schema].Distinct();
                }
                else
                {
                    resultModel.data = repo.GetAppCache();
                }
                resultModel.status = 1;
                logger.Info(string.Format("Load GetApplicationList end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in  GetApplicationList method |   UserName: {0}",  SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in   GetApplicationList method |  UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in   GetApplicationList method | UserName: {0}",  SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
}

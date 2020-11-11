using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class ApplicationController : Controller
    {

        public ApplicationController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        #region Crud operations for Application
        public ActionResult ApplicationList()
        {
            try
            {
                var userid = SessionManager.TESTER_ID;
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
                ViewBag.actionswidth = appgriddata.Actions == null ? "10%" : appgriddata.Actions.Trim() + '%';
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when application page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when application page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView("ApplicationList");
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoad()
        {
            //Get Repository
            logger.Info(string.Format("Application list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var _apprepository = new ApplicationRepository();
            _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
            List<ApplicationViewModel> data = new List<ApplicationViewModel>();
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
                data = _apprepository.GetApplicationList();
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
                logger.Info(string.Format("Application list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Application list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when application page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when application page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
        public JsonResult AddEditApplication(ApplicationViewModel applicationviewmodel)
        {
            logger.Info(string.Format("Application Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _apprepository = new ApplicationRepository();
                _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
                applicationviewmodel.Create_Person = SessionManager.TESTER_LOGIN_NAME;
                var _addeditResult = _apprepository.AddEditApplication(applicationviewmodel);
                var _treerepository = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.message = "Saved Application [" + applicationviewmodel.ApplicationName + "].";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("Application Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Application Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Application page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Application page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete the Application object data by applicationID
        public ActionResult DeletApplication(long ApplicationId)
        {
            logger.Info(string.Format("Application Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _apprepository = new ApplicationRepository();
                var _treerepository = new GetTreeRepository();
                _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;

                var lflag = _apprepository.CheckTestCaseExistsInAppliction(ApplicationId);

                if (lflag.Count <= 0)
                {
                    var Applicationname = _apprepository.GetApplicationNameById(ApplicationId);
                    var _deleteResult = _apprepository.DeleteApplication(ApplicationId);
                    var lSchema = SessionManager.Schema;
                    var lConnectionStr = SessionManager.APP;
                    Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                    resultModel.data = "success";
                    resultModel.message = "Application[" + Applicationname + "] has been deleted.";
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.data = lflag; 
                    resultModel.status = 1;
                }
                logger.Info(string.Format("Application Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Application Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Application page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Application page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
            ResultModel resultModel = new ResultModel();
            try
            {
                applicationname = applicationname.Trim();
                var _apprepository = new ApplicationRepository();
                _apprepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = _apprepository.CheckDuplicateApplicationNameExist(applicationname, ApplicationId);
                resultModel.status = 1;
                resultModel.message = "success";
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Application page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Application page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

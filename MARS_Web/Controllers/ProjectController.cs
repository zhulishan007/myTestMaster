using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MARS_Web.Controllers
{
    public class ProjectController : Controller
    {
        public ProjectController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        #region List,Save and Remove Project/s by UserId

        //Save the Project mapping with User by UserId and refreshes the MarsTree if User is logged in the system
        public JsonResult SaveProjectChangesByUser(string model, decimal userid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTree = new GetTreeRepository();
                ProjectRepository repo = new ProjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lobj = js.Deserialize<Project[]>(model);
                var lproject = lobj.ToList();
                if (SessionManager.TESTER_ID == userid)
                {
                    var result = repo.SaveProjectByUserId(lproject, SessionManager.TESTER_ID);
                    var lSchema = SessionManager.Schema;
                    var lConnectionStr = SessionManager.APP;
                    Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                    resultModel.data = result;
                    resultModel.status = 1;
                }
                else
                {
                    var resultbyuser = repo.SaveProjectByUserId(lproject, userid);
                    resultModel.data = resultbyuser;
                    resultModel.status = 1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
          
        }

        //Lists the project by UserId 
        public ActionResult ProjectListByUserId(decimal userid)
        {
            var lProjectList = new List<ProjectByUser>();
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                if (userid == 0)
                {
                    var repTree = new GetTreeRepository();
                    lProjectList = repTree.ProjectListByUserName(SessionManager.TESTER_ID);
                }
                else
                {
                    var repTree = new GetTreeRepository();
                    lProjectList = repTree.ProjectListByUserName(userid);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView(lProjectList);

        }

        //Removes the all the project/s mapping with the User 
        public JsonResult DeleteUserProjectMapping(long userid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ProjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                AccountRepository repouser = new AccountRepository();
                var Username = repouser.GetUserName((int)userid);
                var result = repo.DeleteProjectUserMapping(userid);
                resultModel.data = new { Status = result, Username = Username };
                resultModel.status = 1;
               // return Json(new { Status = result, Username = Username }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Lists all the Users present in the system to Add/remove projects for that particular user
        public ActionResult UserList()
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
                logger.Error(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView();
        }
        [HttpPost]
        public JsonResult ListUsers()
        {
            var data = new List<UserModel>();
            int recFilter = 0;
            int totalRecords = 0;
            string draw = string.Empty;
            try
            {
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);


                string FirstNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];

                string LastNameSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                string UserNameSearch = Request.Form.GetValues("columns[2][search][value]")[0];
                string EmailSearch = Request.Form.GetValues("columns[3][search][value]")[0];
                string Projectsearch = Request.Form.GetValues("columns[4][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];
                var recacc = new AccountRepository();
                data = recacc.ListAllUsersWithProjectMapping();
                if (!string.IsNullOrEmpty(LastNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TESTER_NAME_LAST) && x.TESTER_NAME_LAST.ToLower().Trim().Contains(LastNameSearch.ToLower().Trim())).ToList();
                }

                if (!string.IsNullOrEmpty(FirstNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TESTER_NAME_F) && x.TESTER_NAME_F.ToLower().Trim().Contains(FirstNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(UserNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TESTER_LOGIN_NAME) && x.TESTER_LOGIN_NAME.ToLower().Trim().Contains(UserNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(EmailSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TESTER_MAIL) && x.TESTER_MAIL.ToLower().Trim().Contains(EmailSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(Projectsearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ProjectName) && x.ProjectName.ToLower().Trim().Contains(Projectsearch.ToLower().Trim())).ToList();
                }


                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Last Name":
                            data = data.OrderByDescending(a => a.TESTER_NAME_LAST).ToList();
                            break;
                        case "Middle Name":
                            data = data.OrderByDescending(a => a.TESTER_NAME_M).ToList();
                            break;
                        case "First Name":
                            data = data.OrderByDescending(a => a.TESTER_NAME_F).ToList();
                            break;
                        case "User Name":
                            data = data.OrderByDescending(a => a.TESTER_LOGIN_NAME).ToList();
                            break;

                        case "Email Address":
                            data = data.OrderByDescending(a => a.TESTER_MAIL).ToList();
                            break;
                        case "ProjectId":
                            data = data.OrderByDescending(a => a.ProjectName).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.TESTER_NAME_LAST).ToList();
                            break;
                    }

                }
                else
                {
                    switch (colOrder)
                    {
                        case "Last Name":
                            data = data.OrderBy(a => a.TESTER_NAME_LAST).ToList();
                            break;
                        case "Middle Name":
                            data = data.OrderBy(a => a.TESTER_NAME_M).ToList();
                            break;
                        case "First Name":
                            data = data.OrderBy(a => a.TESTER_NAME_F).ToList();
                            break;
                        case "User Name":
                            data = data.OrderBy(a => a.TESTER_LOGIN_NAME).ToList();
                            break;

                        case "Email Address":
                            data = data.OrderBy(a => a.TESTER_MAIL).ToList();
                            break;
                        case "ProjectId":
                            data = data.OrderBy(a => a.ProjectName).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.TESTER_NAME_LAST).ToList();
                            break;
                    }

                }

                 totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => (!string.IsNullOrEmpty(p.TESTER_NAME_F) && p.TESTER_NAME_F.ToLower().Contains(search.ToLower())) ||
                                           (!string.IsNullOrEmpty(p.TESTER_NAME_LAST) && p.TESTER_NAME_LAST.ToString().ToLower().Contains(search.ToLower())) ||
                                           (!string.IsNullOrEmpty(p.TESTER_NAME_M) && p.TESTER_NAME_M.ToString().ToLower().Contains(search.ToLower())) ||
                                            p.TESTER_MAIL.ToString().ToLower().Contains(search.ToLower()) ||
                                            p.TESTER_LOGIN_NAME.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Project controller | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks whether the Project exists in the system or not,If not then Renames the project
        public JsonResult ChangeProjectName(string ProjectName, string Projectdesc, long ProjectId)
        {
            logger.Info(string.Format("Project Rename Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTree = new GetTreeRepository();
                var Projectrepo = new ProjectRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Projectrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var checkduplicateproject = Projectrepo.CheckDuplicateProjectName(ProjectName, ProjectId);
                if (checkduplicateproject)
                {
                    resultModel.status = 0;
                    resultModel.message = "Project name [" + ProjectName + "] already exists";
                    resultModel.data = checkduplicateproject;
                }
                else
                {
                    var lresult = Projectrepo.ChangeProjectName(ProjectName, Projectdesc, ProjectId);
                    Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                    resultModel.status = 1;
                    resultModel.message = "Project name successfully changed";
                    resultModel.data = lresult;
                    logger.Info(string.Format("Project Rename successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                logger.Info(string.Format("Project Rename Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Crud operations for Project
        public ActionResult ProjectsList()
        {
            try
            {
                ApplicationRepository repo = new ApplicationRepository();
                ProjectRepository prepo = new ProjectRepository();
                prepo.Username = SessionManager.TESTER_LOGIN_NAME;
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var lapp = repo.ListApplication();
                var applist = lapp.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).OrderBy(x => x.Text).ToList();

                var gridlst = repAcc.GetGridList((long)userId, GridNameList.ProjectList);
                var proGridWidth = GridHelper.GetProjectwidth(gridlst);

                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.listApplications = applist;
                ViewBag.namewidth = proGridWidth.Name == null ? "20%" : proGridWidth.Name.Trim() + '%';
                ViewBag.descriptionwidth = proGridWidth.Description == null ? "25%" : proGridWidth.Description.Trim() + '%';
                ViewBag.applicationwidth = proGridWidth.Application == null ? "25%" : proGridWidth.Application.Trim() + '%';
                ViewBag.statuswidth = proGridWidth.Status == null ? "20%" : proGridWidth.Status.Trim() + '%';
                ViewBag.actionswidth = proGridWidth.Actions == null ? "10%" : proGridWidth.Actions.Trim() + '%';
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when project page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when project page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView("ProjectsList");
        }
        [HttpPost]
        public JsonResult DataLoad()
        {
            logger.Info(string.Format("Projet list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var repAcc = new ProjectRepository();
            repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
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

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;
            var data = new List<ProjectViewModel>();
            int totalRecords = 0;
            int recFilter = 0;
            string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string DescriptionSearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string ApplicationSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string StatusSearch = Request.Form.GetValues("columns[3][search][value]")[0];

            try
            {
                data = repAcc.ListAllProject(lSchema, lConnectionStr, startRec, pageSize, colOrder, orderDir, NameSearch, DescriptionSearch, ApplicationSearch, StatusSearch);
                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().TotalCount;
                }

                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().TotalCount;
                }
                logger.Info(string.Format("Projet list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Projet list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when Projet page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when Projet page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddEditProject(ProjectViewModel lModel)
        {
            logger.Info(string.Format("Project Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var projectrepo = new ProjectRepository();
                projectrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                lModel.CarectorName = SessionManager.TESTER_LOGIN_NAME;
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var lResult = projectrepo.AddEditProject(lModel);
                var repTree = new GetTreeRepository();
                Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.status = 1;
                resultModel.message = "Successfully saved Project [" + lModel.ProjectName + "]";
                resultModel.data = lResult;
                logger.Info(string.Format("project Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("project Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteProject(long projectid)
        {
            logger.Info(string.Format("project Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new ProjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var projectName = repo.GetProjectNamebyId(projectid);
                var result = repo.DeleteProject(projectid);
                var repTree = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.status = 1;
                resultModel.message = "Project [" + projectName + "] deleted Successfully.";
                resultModel.data = projectName;
                logger.Info(string.Format("project Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("project Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckDuplicateProjectNameExist(string ProjectName, long? ProjectId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                ProjectName = ProjectName.Trim();
                var Projectrepo = new ProjectRepository();
                Projectrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = Projectrepo.CheckDuplicateProjectName(ProjectName, ProjectId);
                resultModel.status = 1;
                resultModel.message = "success";
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in project page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

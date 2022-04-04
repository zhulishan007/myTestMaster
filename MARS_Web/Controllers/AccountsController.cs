using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    [SessionTimeout]

    public class AccountsController : Controller
    {

        public AccountsController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        #region Crud operation for users       
        public ActionResult ListOfUsers()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repAcc.GetGridList((long)userId, GridNameList.UserList);
                var usergriddata = GridHelper.GetUserwidth(gridlst);
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.fnamewidth = usergriddata.FName == null ? "10%" : usergriddata.FName.Trim() + '%';
                ViewBag.mnamewidth = usergriddata.MName == null ? "10%" : usergriddata.MName.Trim() + '%';
                ViewBag.lnamewidth = usergriddata.LName == null ? "10%" : usergriddata.LName.Trim() + '%';
                ViewBag.namewidth = usergriddata.Name == null ? "10%" : usergriddata.Name.Trim() + '%';
                ViewBag.emailwidth = usergriddata.Email == null ? "20%" : usergriddata.Email.Trim() + '%';
                ViewBag.companywidth = usergriddata.Company == null ? "20%" : usergriddata.Company.Trim() + '%';
                ViewBag.statustwidth = usergriddata.Status == null ? "10%" : usergriddata.Status.Trim() + '%';
                ViewBag.actionswidth = usergriddata.Actions == null ? "10%" : usergriddata.Actions.Trim() + '%';
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for ListOfUsers method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for ListOfUsers method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for ListOfUsers method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                throw;
            }
            return PartialView();
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoad()
        {
            logger.Info(string.Format("User list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var repAcc = new AccountRepository();
            repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

            var data = new List<UserModel>();
            int totalRecords = 0;
            int recFilter = 0;

            string FirstNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string MiddelNameSearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string LastNameSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string UserNameSearch = Request.Form.GetValues("columns[3][search][value]")[0];
            string EmailSearch = Request.Form.GetValues("columns[4][search][value]")[0];
            string CompanySearch = Request.Form.GetValues("columns[5][search][value]")[0];

            string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
            var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
            string colDir = Request.Form.GetValues("order[0][dir]")[0];
            try
            {
                data = repAcc.ListAllUsers().ToList();

                if (!string.IsNullOrEmpty(LastNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TESTER_NAME_LAST) && x.TESTER_NAME_LAST.ToLower().Trim().Contains(LastNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(MiddelNameSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.TESTER_NAME_M) && p.TESTER_NAME_M.ToString().ToLower().Contains(MiddelNameSearch.ToLower())).ToList();
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
                if (!string.IsNullOrEmpty(CompanySearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.COMPANY_NAME) && x.COMPANY_NAME.ToLower().Trim().Contains(CompanySearch.ToLower().Trim())).ToList();
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
                        case "Status":
                            data = data.OrderByDescending(a => a.STATUS).ToList();
                            break;
                        case "Email Address":
                            data = data.OrderByDescending(a => a.TESTER_MAIL).ToList();
                            break;
                        case "Company Name":
                            data = data.OrderByDescending(a => a.COMPANY_NAME).ToList();
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
                        case "Status":
                            data = data.OrderByDescending(a => a.STATUS).ToList();
                            break;
                        case "Email Address":
                            data = data.OrderBy(a => a.TESTER_MAIL).ToList();
                            break;
                        case "Company Name":
                            data = data.OrderBy(a => a.COMPANY_NAME).ToList();
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
                                            p.TESTER_LOGIN_NAME.ToString().ToLower().Contains(search.ToLower()) ||
                                            p.COMPANY_NAME.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
                logger.Info(string.Format("user list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("user list save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditUser(int? lid)
        {
            logger.Info(string.Format("User Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ViewBag.Header = "Add User";
            var repCompany = new CompanyRepository();
            var Accountrepo = new AccountRepository();
            Accountrepo.Username = SessionManager.TESTER_LOGIN_NAME;
            repCompany.Username = SessionManager.TESTER_LOGIN_NAME;
            var lModel = new UserModel();
            try
            {
                var lCompanyList = repCompany.GetCompanyList();
                var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.listCompany = companylist;
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                if (lid != null)
                {
                    lModel = Accountrepo.GetUserMappingById(Convert.ToInt32(lid));
                    ViewBag.Header = "Edit User";
                }
                var rolelist = Accountrepo.GetAllRoles().Select(c => new SelectListItem { Text = c.ROLE_NAME, Value = c.ROLE_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.rolelist = rolelist;

                logger.Info(string.Format("User Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for AddEditUser method | UserId: {0} | UserName: {1}", lid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for AddEditUser method | UserId: {0} | UserName: {1}", lid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for AddEditUser method | UserId: {0} | UserName: {1}", lid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView(lModel);
        }

        //Add/Update User objects values
        [HttpPost]
        public JsonResult AddEditUser(UserModel userModel)
        {
            logger.Info(string.Format("User Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                ViewBag.Header = "Edit User";
                var Accountrepo = new AccountRepository();
                var repCompany = new CompanyRepository();
                var repentil = new EntitlementRepository();
                repentil.Username = SessionManager.TESTER_LOGIN_NAME;
                Accountrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                repCompany.Username = SessionManager.TESTER_LOGIN_NAME;
                var t_TESTER = Accountrepo.ConverUserModel(userModel);
                var lCompanyList = repCompany.GetCompanyList();
                var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.listCompany = companylist;

                var lchecked = t_TESTER.AVAILABLE_MARK;
                t_TESTER.AVAILABLE_MARK = null;

                if (t_TESTER.TESTER_ID == 0)
                {
                    t_TESTER.TESTER_PWD = PasswordHelper.EncodeString(t_TESTER.TESTER_PWD);
                    ViewBag.Header = "Add User";
                }
                if (t_TESTER.TESTER_ID == 0)
                {
                    var result = Accountrepo.CheckLoginEmailExist(t_TESTER.TESTER_MAIL, t_TESTER.TESTER_ID);
                    if (result)
                    {
                        resultModel.status = 1;
                        resultModel.message = "Email [" + t_TESTER.TESTER_MAIL + "] already exists";
                        return Json(resultModel, JsonRequestBehavior.AllowGet);
                    }

                    var loginresult = Accountrepo.CheckLoginNameExist(t_TESTER.TESTER_LOGIN_NAME, t_TESTER.TESTER_ID);
                    if (loginresult)
                    {
                        resultModel.status = 1;
                        resultModel.message = "User name [" + t_TESTER.TESTER_LOGIN_NAME + "] already exists.";
                        return Json(resultModel, JsonRequestBehavior.AllowGet);
                    }
                    t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
                    //role save
                    if (!string.IsNullOrEmpty(userModel.RoleIds))
                    {
                        var roleresult = repentil.AddRole(t_TESTER.TESTER_ID, userModel.RoleIds);
                    }

                    Session["SubmitUserMsg"] = "Succefully Added User.";
                    resultModel.message = "User created successfully.";
                    resultModel.status = 1;
                    resultModel.data = true;
                }
                else
                {
                    var result = Accountrepo.CheckLoginEmailExist(t_TESTER.TESTER_MAIL, t_TESTER.TESTER_ID);
                    if (result)
                    {
                        resultModel.status = 1;
                        resultModel.message = "Email invalid";
                        return Json(resultModel, JsonRequestBehavior.AllowGet);
                    }
                    t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
                    Session["SubmitUserMsg"] = "Succefully Updated User.";
                    resultModel.message = "User Updated successfully.";
                    resultModel.status = 1;
                    resultModel.data = true;
                    logger.Info(string.Format("User Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("User Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for AddEditUser method | UserId: {0} | UserName: {1}", userModel.TESTER_ID, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for AddEditUser method | UserId: {0} | UserName: {1}", userModel.TESTER_ID, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for AddEditUser method | UserId: {0} | UserName: {1}", userModel.TESTER_ID, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete the User object data by ID
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            logger.Info(string.Format("User Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;

                var Username = repo.GetUserName(id);
                var lresult = repo.DeleteUser(id);

                resultModel.message = "User [" + Username + " ] has been deleted.";
                resultModel.data = Username;
                resultModel.status = 1;

                logger.Info(string.Format("User Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("User Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for DeleteUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for DeleteUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for DeleteUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                ELogger.ErrorException(string.Format("InnerException : Error occured in User for DeleteUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Crud operation for User mapping with Exe Path
        //loads username in the dropdown in add/edit User mapping with Exe path
        public JsonResult GetUserName()
        {
            var repAcc = new AccountRepository();
            var result = repAcc.GetAllUsers();
            var lresult = result.Select(c => new SelectListItem { Text = c.TESTER_LOGIN_NAME, Value = c.TESTER_ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(lresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserexecutionEnginePathList()
        {
            var repAcc = new AccountRepository();
            var result = repAcc.GetAllUsers();
            ViewBag.username = result.Select(c => new SelectListItem { Text = c.TESTER_LOGIN_NAME, Value = c.TESTER_ID.ToString() }).OrderBy(x => x.Text).ToList();
            return PartialView();
        }

        public JsonResult AddUserExePath(long userid, long relationid, string exepath)
        {
            var repAcc = new AccountRepository();
            var result = repAcc.AddUserPath(userid, relationid, exepath);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUserRelExePath(long Userid)
        {
            var repAcc = new AccountRepository();
            var result = repAcc.DeleteUserMapExePath(Userid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadExePathList()
        {
            try
            {
                var repAcc = new AccountRepository();

                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                var data = new List<RelUserExePath>();

                data = repAcc.ListUserExePath().ToList();

                string UserNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string Pathsearch = Request.Form.GetValues("columns[1][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];


                if (!string.IsNullOrEmpty(UserNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Username) && x.Username.ToLower().Trim().Contains(UserNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(Pathsearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.ExePath) && p.ExePath.ToString().ToLower().Contains(Pathsearch.ToLower())).ToList();
                }

                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.Username).ToList();
                            break;
                        case "Execution path":
                            data = data.OrderByDescending(a => a.ExePath).ToList();
                            break;

                        default:
                            data = data.OrderByDescending(a => a.Username).ToList();
                            break;
                    }

                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.Username).ToList();
                            break;
                        case "Execution path":
                            data = data.OrderBy(a => a.ExePath).ToList();
                            break;

                        default:
                            data = data.OrderBy(a => a.Username).ToList();
                            break;
                    }

                }

                int totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => (!string.IsNullOrEmpty(p.Username) && p.Username.ToLower().Contains(search.ToLower())) ||
                                           (!string.IsNullOrEmpty(p.ExePath) && p.ExePath.ToString().ToLower().Contains(search.ToLower()))).ToList();
                }

                int recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();


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
                logger.Error(string.Format("Error occured in User for LoadExePathList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for LoadExePathList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User forLoadExePathList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                throw ex;
            }
        }
        #endregion

        #region Checks whether Login name and Email Id exists in the system or not
        [HttpPost]
        public JsonResult CheckLoginNameExist(string lLoginName, int? lLoginId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository Accountrepo = new AccountRepository();
                Accountrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lflag = Accountrepo.CheckLoginNameExist(lLoginName, lLoginId);
                resultModel.message = "success";
                resultModel.data = lflag;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for CheckLoginNameExist method | LoginName: {0} | LoginId : {1} | UserName: {2}", lLoginName, lLoginId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for CheckLoginNameExist method | LoginName: {0} | LoginId : {1} | UserName: {2}", lLoginName, lLoginId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for CheckLoginNameExist method | LoginName: {0} | LoginId : {1} | UserName: {2}", lLoginName, lLoginId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckEmailExist(string lLoginEmail, int? lLoginId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository Accountrepo = new AccountRepository();
                Accountrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lflag = Accountrepo.CheckLoginEmailExist(lLoginEmail, lLoginId);
                resultModel.message = "success";
                resultModel.data = lflag;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for CheckEmailExist method | LoginEmail: {0} | LoginId : {1} | UserName: {2}", lLoginEmail, lLoginId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for CheckEmailExist method | LoginEmail: {0} | LoginId : {1} | UserName: {2}", lLoginEmail, lLoginId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for CheckEmailExist method | LoginEmail: {0} | LoginId : {1} | UserName: {2}", lLoginEmail, lLoginId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Changes User Status
        // Chnage user Status by Id
        [HttpPost]
        public JsonResult ChangeUserStatus(int Id, int Checked)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lresult = repo.ChangeUserStatus(Id, Checked);

                resultModel.message = "User status has been updated.";
                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for ChangeUserStatus method | UserId: {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for ChangeUserStatus method | UserId: {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for ChangeUserStatus method | UserId: {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region User Active Page
        public ActionResult ListOfUsersActivePage()
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
                logger.Error(string.Format("Error occured in User for ListOfUsersActivePage method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for ListOfUsersActivePage method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for ListOfUsersActivePage method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoadUserActivePage()
        {
            try
            {
                var repAcc = new AccountRepository();

                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                var data = new List<UserActiveModel>();

                data = repAcc.ListAllActiveUsers();

                string UserNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string PageNameSearch = Request.Form.GetValues("columns[1][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];


                if (!string.IsNullOrEmpty(UserNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Trim().Contains(UserNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(PageNameSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.PageName) && p.PageName.ToString().ToLower().Contains(PageNameSearch.ToLower())).ToList();
                }

                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "User Name":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Page Name":
                            data = data.OrderByDescending(a => a.PageName).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "User Name":
                            data = data.OrderBy(a => a.UserName).ToList();
                            break;
                        case "Page Name":
                            data = data.OrderBy(a => a.PageName).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.UserName).ToList();
                            break;
                    }

                }

                int totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => (!string.IsNullOrEmpty(p.UserName) && p.UserName.ToLower().Contains(search.ToLower())) ||
                                           (!string.IsNullOrEmpty(p.PageName) && p.PageName.ToString().ToLower().Contains(search.ToLower()))
                                            ).ToList();
                }
                int recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();

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

                logger.Error(string.Format("Error occured in User for DataLoadUserActivePage method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for DataLoadUserActivePage method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for DataLoadUserActivePage method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                throw ex;
            }
        }

        //Delete the Active User object data by ID
        [HttpPost]
        public JsonResult DeleteActiveUser(long id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                var lresult = repo.DeleteActiveUser(id);

                resultModel.message = "Pin tab has been deleted.";
                resultModel.data = "success";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for DeleteActiveUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for DeleteActiveUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for DeleteActiveUser method | UserId: {0} | UserName: {1}", id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Add/Delete User Pin/UnPin objects values
        [HttpPost]
        public JsonResult UserPinUnPinTab(string datatab, long dataid, string dataname, string linkText, long ProjectId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var userId = (long)SessionManager.TESTER_ID;
                AccountRepository repo = new AccountRepository();
                var lresult = repo.AddDetelteActivateTab(userId, userName, datatab, dataid, dataname, linkText, ProjectId);

                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in UserPinUnPinTab method | Data Id : {0} | Data Tab: {1} | Data Name : {2} | Project Id: {3} | Username : {4}", dataid, datatab, dataname, ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured User in UserPinUnPinTab method | Data Id : {0} | Data Tab: {1} | Data Name : {2} | Project Id: {3} | Username : {4}", dataid, datatab, dataname, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in UserPinUnPinTab method | Data Id : {0} | Data Tab: {1} | Data Name : {2} | Project Id: {3} | Username : {4}", dataid, datatab, dataname, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //check Pin tab already exist or not 
        [HttpPost]
        public JsonResult CheckPinExist(string datatab, long dataid, long ProjectId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                var userId = SessionManager.TESTER_ID;
                var lresult = repo.CheckPinExist(userId, datatab, dataid, ProjectId);

                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CheckPinExist method | Data Id : {0} | Data Tab: {1} | Project Id: {2} | Username : {3}", dataid, datatab, ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured User in CheckPinExist method | Data Id : {0} | Data Tab: {1} | Project Id: {2} | Username : {3}", dataid, datatab, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CheckPinExist method | Data Id : {0} | Data Tab: {1} | Project Id: {2} | Username : {3}", dataid, datatab, ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method get all storyboard
        [HttpPost]
        public JsonResult GetStoryboradNameyId(long StoryBoardid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                var list = repo.GetStoryboradNameyId(StoryBoardid);
                var lresult = list != null ? list.STORYBOARD_NAME : "";

                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetStoryboradNameyId method | StoryBoard Id : {0} | Username : {1}", StoryBoardid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured User in GetStoryboradNameyId method | StoryBoard Id : {0} | Username : {1}", StoryBoardid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetStoryboradNameyId method | StoryBoard Id : {0} | Username : {1}", StoryBoardid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method get all TestsuiteId by TeastcaseId
        [HttpPost]
        public JsonResult GetTestsuiteIdByTeastcaseId(long Tid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                var lresult = repo.GetTestsuiteIdByTeastcaseId(Tid);
                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetTestsuiteIdByTeastcaseId method | TestCase Id : {0} | Username : {1}", Tid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured User in GetTestsuiteIdByTeastcaseId method | TestCase Id : {0} | Username : {1}", Tid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetTestsuiteIdByTeastcaseId method | TestCase Id : {0} | Username : {1}", Tid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Role Mapping
        public ActionResult PrivilegeRoleMapping()
        {
            var userId = SessionManager.TESTER_ID;
            AccountRepository repo = new AccountRepository();
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddEditPrivilageRoleMapping(PrivilegeRoleMappingViewModel privilegeRoleMapviewmodel)
        {
            if (!string.IsNullOrEmpty(privilegeRoleMapviewmodel.PrivilegeId))
            {
                string[] privileges = privilegeRoleMapviewmodel.PrivilegeId.Split(',');
                string[] privilegIds = privileges.Distinct().ToArray();
                foreach (var item in privilegIds)
                {
                    privilegeRoleMapviewmodel.PrivilegeListModel.Add(new PrivilegeMapModel
                    {
                        PrivilegeId = Convert.ToInt64(item),
                        IsActive = 1
                    });
                }
            }
            logger.Info(string.Format("PrivilegeRoleMapping Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var Accountrepo = new AccountRepository();
                Accountrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var _getPreviousprivileges = Accountrepo.DeletePrivilagesByRoleId(privilegeRoleMapviewmodel.RoleId);
                var _addeditResult = Accountrepo.AddEditPrivilageRoleMapping(privilegeRoleMapviewmodel);
                var _treerepository = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                if (!_addeditResult)
                {
                    resultModel.message = "No Privileges for PrivilegeRoleMapping.";
                    resultModel.data = _addeditResult;
                    resultModel.status = 0;

                    logger.Info(string.Format("PrivilegeRoleMapping Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("PrivilegeRoleMapping Not Saved | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    resultModel.message = "Saved Privilege Role Mapping.";
                    resultModel.data = _addeditResult;
                    resultModel.status = 1;

                    logger.Info(string.Format("PrivilegeRoleMapping Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("PrivilegeRoleMapping Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in AddEditPrivilageRoleMapping method | Privilege Id : {0} | Username : {1}", privilegeRoleMapviewmodel.PrivilegeId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured User in AddEditPrivilageRoleMapping method | Privilege Id : {0} | Username : {1}", privilegeRoleMapviewmodel.PrivilegeId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in AddEditPrivilageRoleMapping method | Privilege Id : {0} | Username : {1}", privilegeRoleMapviewmodel.PrivilegeId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPrivilegesByRoleId(string roleId)
        {
            AccountRepository repo = new AccountRepository();
            var privilegesByRoleId = repo.GetPrivilegesByRoleId(Convert.ToInt64(roleId)).Select(c => new SelectListItem { Text = c.PrivilegeName, Value = c.PrivilegeId.ToString() }).ToList();
            var allPrivileges = repo.GetAllPrivileges().OrderBy(x => x.PRIVILEGE_ID).Select(c => new SelectListItem { Text = c.PRIVILEGE_NAME, Value = c.PRIVILEGE_ID.ToString() }).ToList(); ;
            allPrivileges.RemoveAll(x => privilegesByRoleId.Any(y => y.Value == x.Value));
            var result = new { privilegesByRoleId = privilegesByRoleId, allPrivileges = allPrivileges };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllPrivileges()
        {
            var userId = SessionManager.TESTER_ID;
            AccountRepository repo = new AccountRepository();
            var allPrivileges = repo.GetAllPrivileges().OrderBy(x => x.PRIVILEGE_ID).Select(c => new SelectListItem { Text = c.PRIVILEGE_NAME, Value = c.PRIVILEGE_ID.ToString() }).ToList(); ;
            return Json(allPrivileges, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllUsers()
        {
            var userId = SessionManager.TESTER_ID;
            AccountRepository repo = new AccountRepository();
            var allUsers = repo.GetAllUsers().Select(c => new SelectListItem { Text = c.TESTER_LOGIN_NAME, Value = c.TESTER_ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(allUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllRoles()
        {
            var userId = SessionManager.TESTER_ID;
            AccountRepository repo = new AccountRepository();
            var allRoles = repo.GetAllRoles().Select(c => new SelectListItem { Text = c.ROLE_NAME, Value = c.ROLE_ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(allRoles, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region User Configuration
        [HttpPost]
        public ActionResult UserConfigList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);
                var repentil = new EntitlementRepository();
                repentil.Username = SessionManager.TESTER_LOGIN_NAME;

                var userlist = repentil.GetAllUsers();
                ViewBag.userlist = userlist.Select(c => new SelectListItem { Text = c.UserName, Value = c.UserId.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for UserConfigList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for UserConfigList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for UserConfigList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        [HttpPost]
        public JsonResult UserConfigDataLoad()
        {
            //Get Repository
            logger.Info(string.Format("User Configration list start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var repAcc = new AccountRepository();
            repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
            List<UserConfigrationViewModel> data = new List<UserConfigrationViewModel>();
            int totalRecords = default(int);
            int recFilter = default(int);
            //Assign values in local userconfig
            #region userconfig
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
            var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
            string colDir = Request.Form.GetValues("order[0][dir]")[0];
            string MainKeySearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string SubKeySearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string MARSUserNameSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string BLOBValueTypeSearch = Request.Form.GetValues("columns[4][search][value]")[0];
            string Descriptionsearch = Request.Form.GetValues("columns[5][search][value]")[0];
            #endregion
            try
            {
                //Get data from List userconfig
                #region Getdata
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                data = repAcc.GetUserConfigrationList(lSchema, lConnectionStr);
                #endregion

                //Check userconfig Value 
                #region CheckValues               
                if (!string.IsNullOrEmpty(MainKeySearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.MainKey) && x.MainKey.ToLower().Trim().Contains(MainKeySearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(SubKeySearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.SubKey) && p.SubKey.ToString().ToLower().Contains(SubKeySearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(MARSUserNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.MARSUserName) && x.MARSUserName.ToLower().Trim().Contains(MARSUserNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(BLOBValueTypeSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.BLOBType) && x.BLOBType.ToLower().Trim().Contains(BLOBValueTypeSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(Descriptionsearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Trim().Contains(Descriptionsearch.ToLower().Trim())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "MainKey":
                            data = data.OrderByDescending(a => a.MainKey).ToList();
                            break;
                        case "SubKey":
                            data = data.OrderByDescending(a => a.SubKey).ToList();
                            break;
                        case "MARSUserName":
                            data = data.OrderByDescending(a => a.MARSUserName).ToList();
                            break;
                        case "BLOBValueType":
                            data = data.OrderByDescending(a => a.BLOBType).ToList();
                            break;
                        case "Description":
                            data = data.OrderByDescending(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.MainKey).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "MainKey":
                            data = data.OrderBy(a => a.MainKey).ToList();
                            break;
                        case "SubKey":
                            data = data.OrderBy(a => a.SubKey).ToList();
                            break;
                        case "MARSUserName":
                            data = data.OrderBy(a => a.MARSUserName).ToList();
                            break;
                        case "BLOBValueType":
                            data = data.OrderBy(a => a.BLOBType).ToList();
                            break;
                        case "Description":
                            data = data.OrderBy(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.MainKey).ToList();
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
                    data = data.Where(p => p.MainKey.ToString().ToLower().Contains(search.ToLower()) ||
                    p.SubKey.ToString().ToLower().Contains(search.ToLower()) ||
                    p.MARSUserName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.BLOBType.ToString().ToLower().Contains(search.ToLower()) ||
                     p.Description.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }

                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
                logger.Info(string.Format("user configration list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("user configration list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for UserConfigDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for UserConfigDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for UserConfigDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
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

        [HttpPost]
        public JsonResult AddEditUserConfigration(UserConfigrationViewModel model)
        {
            logger.Info(string.Format("User Configration Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new AccountRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                model.Create_Person = SessionManager.TESTER_LOGIN_NAME;

                var _addeditResult = repAcc.AddEditUserConfigration(model, lConnectionStr, lSchema);
                resultModel.message = "Saved User Configration [" + model.MainKey + "].";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("User Configration Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("User Configration Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for AddEditUserConfigration method | UserId : {0} | UserName: {1}", model.UserId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for AddEditUserConfigration method | UserId : {0} | UserName: {1}", model.UserId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for AddEditUserConfigration method | UserId : {0} | UserName: {1}", model.UserId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBolbValue(UserConfigrationViewModel model)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new AccountRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                model.Create_Person = SessionManager.TESTER_LOGIN_NAME;

                var _addeditResult = repAcc.UpdateBolbValue(model, lConnectionStr, lSchema);
                resultModel.message = "Saved User Configration.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("User Configration Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for AddEditUserConfigration method | UserId : {0} | UserName: {1}", model.UserId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for AddEditUserConfigration method | UserId : {0} | UserName: {1}", model.UserId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for AddEditUserConfigration method | UserId : {0} | UserName: {1}", model.UserId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteUserConfigration(long Id)
        {
            logger.Info(string.Format("User Configration Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new AccountRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var _treerepository = new GetTreeRepository();

                var miankey = repAcc.GetUserConfigrationNameById(Id);
                var _deleteResult = repAcc.DeleteUserConfigration(Id);
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                resultModel.data = "success";
                resultModel.message = "User Configration[" + miankey + "] has been deleted.";
                resultModel.status = 1;

                logger.Info(string.Format("User Configration Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("User Configration Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for DeleteUserConfigration method | UserConfigId : {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for DeleteUserConfigration method | UserConfigId : {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for DeleteUserConfigration method | UserConfigId : {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //This method get dataset list by Id 
        [HttpPost]
        public ActionResult GetDataSetListbyId(long lTestCaseId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                
                if (GlobalVariable.DataSetListCache != null && GlobalVariable.DataSetListCache.ContainsKey(SessionManager.Schema)) {
                    var list = GlobalVariable.DataSetListCache[SessionManager.Schema].FindAll(f => f.TestcaseId == lTestCaseId);
                    var datasets = list.Select(c => new DataSetListByTestCase
                    {
                        TestcaseId = lTestCaseId,
                        Datasetid = c.Datasetid,
                        Datasetname = c.Datasetname
                    }).ToList();
                    resultModel.data = datasets.OrderBy(r=>r.Datasetname);
                }
                else {
                    var repTree = new GetTreeRepository();
                    var ldatasetlist = repTree.GetDataSetListbyId(lTestCaseId);
                    resultModel.data = ldatasetlist;
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User for GetDataSetListbyId method | TestCase Id : {0} | UserName: {1}", lTestCaseId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User for GetDataSetListbyId method | TestCase Id : {0} | UserName: {1}", lTestCaseId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in User for GetDataSetListbyId method | TestCase Id : {0} | UserName: {1}", lTestCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
    public class Data
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}

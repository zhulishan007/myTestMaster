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
                logger.Error(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView();
        }

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
                logger.Error(string.Format("Error occured when user page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when user page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
                logger.Error(string.Format("Error occured in user page | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView(lModel);
        }

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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Changes User Status

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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
                logger.Error(string.Format("Error occured when User Active page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User Active page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView();
        }

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
                throw ex;
            }
        }

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
                logger.Error(string.Format("Error occured in user active page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user active page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

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
                logger.Error(string.Format("Error occured in user active page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user active page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

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
                logger.Error(string.Format("Error occured in user active page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user active page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

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
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
                logger.Error(string.Format("Error occured in PrivilegeRoleMapping page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in PrivilegeRoleMapping page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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

        [HttpPost]
        public ActionResult GetDataSetListbyId(long lTestCaseId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                var repTree = new GetTreeRepository();
                var ldatasetlist = repTree.GetDataSetListbyId(lTestCaseId);

                resultModel.data = ldatasetlist;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in user page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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

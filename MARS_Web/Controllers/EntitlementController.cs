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
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class EntitlementController : Controller
    {
        public EntitlementController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        [HttpPost]
        public ActionResult UserRoleMappingList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var repentil = new EntitlementRepository();
                repentil.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                var userlist = repentil.GetAllUsers();
                var rolelist = repentil.GetAllRoles();
                ViewBag.userlist = userlist.Select(c => new SelectListItem { Text = c.UserName, Value = c.UserId.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.rolelist = rolelist.Select(c => new SelectListItem { Text = c.ROLE_NAME, Value = c.ROLE_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User Role page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User Role page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView();
        }

        [HttpPost]
        public JsonResult DataLoad()
        {
            var data = new List<UserRoleMappingViewModel>();
            int recFilter = 0;
            int totalRecords = 0;
            string draw = string.Empty;
            try
            {
                var repentil = new EntitlementRepository();
                repentil.Username = SessionManager.TESTER_LOGIN_NAME;
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                data = repentil.ListOfUserRoleMapping();

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string RoleSearch = Request.Form.GetValues("columns[1][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(RoleSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Roles) && p.Roles.ToString().ToLower().Contains(RoleSearch.ToLower())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "User Name":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Roles":
                            data = data.OrderByDescending(a => a.Roles).ToList();
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
                        case "Roles":
                            data = data.OrderBy(a => a.Roles).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.UserName).ToList();
                            break;
                    }
                }

                totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.UserName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Roles.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }
                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User Role page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User Role page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
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
        public JsonResult AddEditUserRoleMapping(UserRoleMappingViewModel model)
        {
            logger.Info(string.Format("User Role Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                model.Create_Person = SessionManager.TESTER_LOGIN_NAME;
                var UserName = _etlrepository.GetUserName(model.UserId);
                var _addeditResult = _etlrepository.AddEditUserRoleMapping(model);
                var _treerepository = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.message = "Saved User [" + UserName + "] Role.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("User Role Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("User Role Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteUserRole(long UserId)
        {
            logger.Info(string.Format("User Role Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var _treerepository = new GetTreeRepository();
                var UserName = _etlrepository.GetUserName(UserId);
                var _deleteResult = _etlrepository.DeleteUserRole(UserId);

                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                resultModel.data = "success";
                resultModel.message = "User [" + UserName + "] Role has been deleted.";
                resultModel.status = 1;

                logger.Info(string.Format("User Role Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("User Role Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckExistOrNotUser(long UserId)
        {
            logger.Info(string.Format("Check Exist Or Not User start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var _treerepository = new GetTreeRepository();
                var result = _etlrepository.CheckExistOrNotUser(UserId);

                resultModel.data = result;
                resultModel.status = 1;

                logger.Info(string.Format("Check Exist Or Not User end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateRole(string RoleName)
        {
            logger.Info(string.Format("Cretae Role Delete start | Username: {0} | RoleName: {1}", SessionManager.TESTER_LOGIN_NAME, RoleName));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;


                var _existResult = _etlrepository.CheckRoleExist(RoleName);
                if (_existResult)
                {
                    resultModel.data = "warning";
                    resultModel.message = "Role [" + RoleName + "]  has already exists.";
                    resultModel.status = 1;
                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                }
                var _createResult = _etlrepository.CreateRole(RoleName);

                resultModel.data = "success";
                resultModel.message = "Role [" + RoleName + "]  has been created.";
                resultModel.status = 1;

                logger.Info(string.Format("Role created end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Role created successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Create Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Role Create page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllRoles()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var userId = SessionManager.TESTER_ID;
                AccountRepository repo = new AccountRepository();
                var allRoles = repo.GetAllRoles().Select(c => new SelectListItem { Text = c.ROLE_NAME, Value = c.ROLE_ID.ToString() }).OrderBy(x => x.Text).ToList(); ;

                resultModel.data = allRoles;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in User Role page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrivilegeRoleMapping()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var fristrole = string.Empty;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var repentil = new EntitlementRepository();
                repentil.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);
                var rolelist = repentil.GetAllRoles();

                if (rolelist.Count > 0)
                    fristrole = rolelist.OrderBy(x => x.ROLE_NAME).FirstOrDefault().ROLE_NAME;

                ViewBag.rolelist = rolelist.Select(c => new SelectListItem { Text = c.ROLE_NAME, Value = c.ROLE_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.PrivilegeList = repentil.GetPriviledgebyRole(fristrole).Select(c => new SelectListItem { Text = c.Name, Value = c.PrivilegeId.ToString(), Selected = (bool)c.Selected }).OrderBy(x => x.Text).ToList();
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User Role page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User Role page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView();
        }

        public ActionResult GetPrivilegesByRoleId(long roleId)
        {
            try
            {
                var repentil = new EntitlementRepository();
                var PrivilegeList = repentil.GetPriviledgebyRoleId(roleId).Select(c => new SelectListItem { Text = c.Name, Value = c.PrivilegeId.ToString(), Selected = (bool)c.Selected }).OrderBy(x => x.Text).ToList();
                ViewBag.PrivilegeList = PrivilegeList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User Privilege page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User Privilege page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView("_ListBox");
        }

        [HttpPost]
        public JsonResult AddEditPrivilageRoleMapping(PrivilegeRoleMappingViewModel model)
        {
            logger.Info(string.Format("PrivilegeRoleMapping Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var RoleName = _etlrepository.GetRoleName(model.RoleId);
                var _addeditResult = _etlrepository.AddEditPrivilageRoleMapping(model);
                var _treerepository = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.message = "Saved Privilege Role  [" + RoleName + "].";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("Privilege Role Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Privilege Role Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
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
    }
}
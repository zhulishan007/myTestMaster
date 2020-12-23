using MARS_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MARS_Web.Helper;
using System.Web.Security;
using MARS_Repository.ViewModel;
using MARS_Repository.Entities;
using System.Web.Configuration;
using NLog;

namespace MARS_Web.Controllers
{
    public class LoginController : Controller
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        #region Logs User in
        [HttpGet]
        public ActionResult Login(bool isLogout = false)
        {
            try
            {
                string MarsEnvironment = string.Empty;
                MarsConfig mc = MarsConfig.Configure(MarsEnvironment);

                LoginCookieModel lModel = CheckCookie();
                if (lModel == null || isLogout)
                {
                }
                else if (SessionManager.TESTER_ID > 0)
                {
                    var lMsg = CheckCredential(lModel.LoginName, lModel.Password, lModel.Dbconnection);
                    if (lMsg == "Succefully Logged!!")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var defaultDb = mc.GetDefaultDatabase();
                var Connlst = mc.GetConnectionDetails();
                var connlist = Connlst.Select(c => new SelectListItem { Text = c.UserName + "/" + c.Host, Value = (c.UserName + "/" + c.Host).ToString() }).ToList();
                ViewBag.connlist = connlist;
                ViewBag.defaultDb = defaultDb;

                return View();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when login page open"));
                ELogger.ErrorException(string.Format("Error occured when login page open"), ex);
                return View();
            }
        }

        public LoginCookieModel CheckCookie()
        {
            LoginCookieModel lModel = null;
            string lUserName = string.Empty, lPassword = string.Empty, lconnection = string.Empty;
            if (Request.Cookies["UserName"] != null)
                lUserName = Request.Cookies["UserName"].Value;
            if (Request.Cookies["Password"] != null)
                lPassword = Request.Cookies["Password"].Value;
            if (Request.Cookies["Dbconnection"] != null)
                lconnection = Request.Cookies["Dbconnection"].Value;
            if (!string.IsNullOrEmpty(lUserName) && !string.IsNullOrEmpty(lPassword) && !string.IsNullOrEmpty(lconnection))
                lModel = new LoginCookieModel { LoginName = lUserName, Password = lPassword, Dbconnection = lconnection };
            return lModel;
        }

        //Logs user in for 1 day if Remember me is checked
        [HttpPost]
        public ActionResult Login(string lUserLogin, string lPassword, string lconnection, string accesstoken, bool lRememberme = false)
        {
            ResultModel resultModel = new ResultModel();
            string ipAddress = Request.UserHostAddress;
            SessionManager.Accesstoken = "Bearer " + accesstoken;
            var lMsg = CheckCredential(lUserLogin, lPassword, lconnection);
            var connlist = lconnection.Split('/');
            try
            {
                if (lRememberme)
                {
                    if (lMsg == "Succefully Logged!!")
                    {
                        HttpCookie ckU = new HttpCookie("UserName");
                        ckU.Expires = DateTime.Now.AddDays(1);
                        ckU.Value = lUserLogin;
                        Response.Cookies.Add(ckU);

                        HttpCookie ckP = new HttpCookie("Password");
                        ckP.Expires = DateTime.Now.AddDays(1);
                        ckP.Value = lPassword;
                        Response.Cookies.Add(ckP);

                        HttpCookie ckdb = new HttpCookie("Dbconnection");
                        ckdb.Expires = DateTime.Now.AddDays(1);
                        ckdb.Value = lconnection;
                        Response.Cookies.Add(ckdb);
                    }
                }

                if (lMsg != "Succefully Logged!!")
                {
                    logger.Error(string.Format("{0} | UserName: {1} | Password: {2} | DataBase: {3} | Ip Address: {4}", lMsg, lUserLogin, lPassword, connlist[0], ipAddress));
                    resultModel.status = 0;
                    resultModel.message = lMsg;
                }
                else
                {
                    logger.Info(string.Format("User Login successfully | UserName: {0} | Password: {1} | DataBase: {2} | Ip Address: {3}", lUserLogin, lPassword, connlist[0], ipAddress));
                    resultModel.status = 1;
                    resultModel.message = lMsg;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in login page | UserName: {0}", lUserLogin));
                ELogger.ErrorException(string.Format("Error occured in login page | UserName: {0}", lUserLogin), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public virtual string CheckCredential(string lUserLogin, string lPassword, string lconnection)
        {
            string MarsEnvironment = string.Empty;
            var lMsg = "Error";
            string[] connlist = new string[10];
            try
            {
                if (!string.IsNullOrEmpty(lconnection))
                {
                    connlist = lconnection.Split('/');
                    MarsEnvironment = connlist[0];
                }
                MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
                DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                if (det != null && !string.IsNullOrEmpty(det.EntityConnString) && !string.IsNullOrEmpty(det.ConnString) && !string.IsNullOrEmpty(det.Host) && !string.IsNullOrEmpty(det.Schema))
                {
                    SessionManager.ConnectionString = det.EntityConnString;
                    SessionManager.Schema = det.Schema;
                    SessionManager.APP = det.ConnString;
                    SessionManager.Host = det.Host;
                }

                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;
                if (!string.IsNullOrEmpty(DBEntities.ConnectionString) && !string.IsNullOrEmpty(DBEntities.Schema))
                {
                    AccountRepository Accountrepo = new AccountRepository();
                    EntitlementRepository Entitlementrepo = new EntitlementRepository();
                    var repTestCase = new TestCaseRepository();
                    var lUser = Accountrepo.GetUserByEmailAndLoginName(lUserLogin);
                    if (lUser != null)
                    {
                        var lUserPassword = PasswordHelper.DecodeString(lUser.TESTER_PWD);
                        if (lUserPassword == lPassword)
                        {
                            SessionManager.TESTER_ID = lUser.TESTER_ID;
                            SessionManager.TESTER_MAIL = lUser.TESTER_MAIL;
                            SessionManager.TESTER_NAME_F = lUser.TESTER_NAME_F;
                            SessionManager.TESTER_NAME_M = lUser.TESTER_NAME_M;
                            SessionManager.TESTER_NAME_LAST = lUser.TESTER_NAME_LAST;
                            SessionManager.TESTER_LOGIN_NAME = lUser.TESTER_LOGIN_NAME;
                            SessionManager.TESTER_NUMBER = lUser.TESTER_NUMBER;

                            var repTree = new GetTreeRepository();
                            var lSchema = SessionManager.Schema;
                            var lConnectionStr = SessionManager.APP;
                            Session["LeftProjectList"] = repTree.GetProjectList(lUser.TESTER_ID, lSchema, lConnectionStr);
                            Session["PrivilegeList"] = Entitlementrepo.GetRolePrivilege((long)lUser.TESTER_ID);
                            Session["RoleList"] = Entitlementrepo.GetRoleByUser((long)SessionManager.TESTER_ID);
                            lMsg = "Succefully Logged!!";
                            repTestCase.UpdateIsAvailableReload((long)lUser.TESTER_ID);
                        }
                        else
                        {
                            lMsg = "Password did not match.";
                        }
                    }
                    else
                    {
                        lMsg = "User Name does not exist in system";
                    }
                }
                else
                {
                    lMsg = connlist[0] + " database does not exist.";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in login page | UserName: {0}", lUserLogin));
                ELogger.ErrorException(string.Format("Error occured in login page | UserName: {0}", lUserLogin), ex);
            }
            return lMsg;
        }

        //This method will be check User exist or not 
        [HttpPost]
        public JsonResult CheckUserExist(string lUserLogin, string lPassword, string lconnection)
        {
            ResultModel resultModel = new ResultModel();
            string ipAddress = Request.UserHostAddress;
            MarsConfig mc = MarsConfig.Configure(lconnection);
            DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();
            var lMsg = "Error";
            try
            {
                if (det != null)
                {
                    DBEntities.ConnectionString = det.EntityConnString;
                    DBEntities.Schema = det.Schema;
                }
                AccountRepository Accountrepo = new AccountRepository();
                var lUser = Accountrepo.GetUserByEmailAndLoginName(lUserLogin);
                if (lUser != null)
                {
                    var lUserPassword = PasswordHelper.DecodeString(lUser.TESTER_PWD);
                    if (lUserPassword != lPassword)
                    {
                        lMsg = "Password did not match.";
                        resultModel.message = lMsg;
                        logger.Error(string.Format("{0} | UserName: {1} | Password: {2} | DataBase: {3} | Ip Address: {4}", lMsg, lUserLogin, lPassword, lconnection, ipAddress));
                    }
                }
                else
                {
                    string msg = "User [" + lUserLogin + "] does not exist in [" + lconnection + "] database";
                    resultModel.message = msg;
                    logger.Error(string.Format("{0} | UserName: {1} | Password: {2} | DataBase: {3} | Ip Address: {4}", msg, lUserLogin, lPassword, lconnection, ipAddress));
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                ELogger.ErrorException(string.Format("Error occured in login page | UserName: {0}", lUserLogin), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Resest password if User forgets the password
        [HttpPost]
        public JsonResult ForgotPsw(string lEmail, string lConnection)
        {
            ResultModel resultModel = new ResultModel();
            string ipAddress = Request.UserHostAddress;
            string MarsEnvironment = string.Empty;
            var lMsg = "Error";
            try
            {
                if (!string.IsNullOrEmpty(lConnection))
                {
                    var connlist = lConnection.Split('/');
                    MarsEnvironment = connlist[0];
                }
                MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
                DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                if (det != null)
                {
                    DBEntities.ConnectionString = det.EntityConnString;
                    DBEntities.Schema = det.Schema;
                }
                AccountRepository Accountrepo = new AccountRepository();
                Accountrepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lUser = Accountrepo.GetUserByEmail(lEmail);

                if (lUser != null)
                {
                    var lUserPassword = PasswordHelper.DecodeString(lUser.TESTER_PWD);
                    var lUserName = lUser.TESTER_LOGIN_NAME;
                    var Emailid = lUser.TESTER_MAIL;

                    string body = string.Empty;

                    var lFilePath = AppDomain.CurrentDomain.BaseDirectory + "EmailHTML\\ForgotEmail.html";

                    using (StreamReader reader = new StreamReader(lFilePath))
                    {
                        body = reader.ReadToEnd();
                    }
                    Random generator = new Random();
                    String r = generator.Next(0, 999999).ToString("D6");
                    Accountrepo.SetTemporaryKey(r, lUser.TESTER_ID);
                    lMsg = EmailHelper.ForgotPsw(Emailid, lUserPassword, lUserName, body, r, lConnection, WebConfigurationManager.AppSettings["ForgotPswLink"]);
                    logger.Info(string.Format("{0} | Email: {1} | UserName: {2} | DataBase: {3} | Ip Address: {4}", lMsg, Emailid, lUserName, MarsEnvironment, ipAddress));
                }
                else
                {
                    lMsg = "This Email Address does not exist in the system.";
                    logger.Error(string.Format("{0} | Email: {1} | DataBase: {2} | Ip Address: {3}", lMsg, lEmail, MarsEnvironment, ipAddress));
                }
                resultModel.status = 1;
                resultModel.message = lMsg;
            }
            catch (Exception ex)
            {
                ELogger.ErrorException(string.Format("Error occured in forgot password page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPassword(string UserEmail, string TempKey, string Connstring)
        {
            try
            {
                string ipAddress = Request.UserHostAddress;
                string MarsEnvironment = string.Empty;
                if (!string.IsNullOrEmpty(Connstring))
                {
                    var connlist = Connstring.Split('/');
                    MarsEnvironment = connlist[0];
                }
                MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
                DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                if (det != null)
                {
                    DBEntities.ConnectionString = det.EntityConnString;
                    DBEntities.Schema = det.Schema;
                }

                AccountRepository Accountrepo = new AccountRepository();
                var lUser = Accountrepo.GetUserByEmail(UserEmail);
                var lUserMappId = Accountrepo.GetUserMappingByUserId(lUser.TESTER_ID);
                if (lUserMappId != null)
                {
                    if (lUserMappId.TEMP_KEY == TempKey)
                    {
                        return View();
                    }
                    else
                    {
                        logger.Error(string.Format("{0} | Email: {1} | DataBase: {2} | Ip Address: {3}", "Reset password TempKey did not match.", UserEmail, MarsEnvironment, ipAddress));
                        return null;
                    }
                }
                else
                {
                    logger.Error(string.Format("{0} | Email: {1} | DataBase: {2} | Ip Address: {3}", "Something Wrong heppans When get User Details", UserEmail, MarsEnvironment, ipAddress));
                    return null;
                }
            }
            catch (Exception ex)
            {
                ELogger.ErrorException(string.Format("Error occured in Reset password page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                return null;
            }
        }

        //This Method will chnage Password 
        [HttpPost]
        public JsonResult ResetPwd(string TESTER_PWD, string emailid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                string lMsg = string.Empty;
                string ipAddress = Request.UserHostAddress;
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                AccountRepository Accountrepo = new AccountRepository();
                var lUser = Accountrepo.GetUserByEmail(emailid);

                var lUserMappId = Accountrepo.GetUserMappingByUserId(lUser.TESTER_ID);

                if (lUserMappId != null)
                {
                    var testerid = lUser.TESTER_ID;
                    var password = PasswordHelper.EncodeString(TESTER_PWD);
                    Accountrepo.ChangeUserPassword(password, testerid);
                    Accountrepo.UpdateTempKey(lUserMappId.USER_MAPPING_ID, null);
                    Session["ResetSuccessMsg"] = "Successfully Updated Your Password";
                    logger.Info(string.Format("{0} | UserName: {1} | DataBase: {2} | Ip Address: {3}", "Successfully Updated Your Password", lUser.TESTER_LOGIN_NAME, SessionManager.Schema, ipAddress));
                    resultModel.status = 1;
                    resultModel.message = "success";
                }
                else
                {
                    lMsg = "Reset Password dialog has errors";
                    logger.Error(string.Format("{0} | UserName: {1} | DataBase: {2} | Ip Address: {3}", lMsg, lUser.TESTER_LOGIN_NAME, SessionManager.Schema, ipAddress));
                    resultModel.status = 0;
                    resultModel.message = lMsg;
                }
            }
            catch (Exception ex)
            {
                ELogger.ErrorException(string.Format("Error occured in reset password page | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Logs out the User
        public ActionResult Logout()
        {
            try
            {
                string ipAddress = Request.UserHostAddress;
                var lMsg = "Error";
                var repTestCase = new TestCaseRepository();
                repTestCase.UpdateIsAvailableReload((long)SessionManager.TESTER_ID);

                lMsg = "Succefully Logged out!!";
                logger.Info(string.Format("{0} | UserName: {1} | DataBase: {2} | Ip Address: {3}", lMsg, SessionManager.TESTER_LOGIN_NAME, SessionManager.Schema, ipAddress));
                return RedirectToAction("Login", "Login", new { isLogout = true });
            }
            catch (Exception ex)
            {
                ELogger.ErrorException(string.Format("Error occured Logout Time | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Loads the Mars Tree after the User is logged in
        [SessionTimeout]
        public ActionResult LeftPanel()
        {
            var lProjectList = new List<ProjectByUser>();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                var repTree = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                lProjectList = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }

            return PartialView(lProjectList);
        }
        [SessionTimeout]
        public ActionResult LeftPanelTestSuite(long ProjectId)
        {
            var lTestSuiteList = new List<TestSuiteListByProject>();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                var repTree = new GetTreeRepository();
                lTestSuiteList = repTree.GetTestSuiteList(ProjectId);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView(lTestSuiteList);
        }

        [SessionTimeout]
        public ActionResult LeftPanelTestCase(long ProjectId, long TestSuiteId)
        {
            var lTestSuiteList = new List<TestCaseListByProject>();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                var repTree = new GetTreeRepository();
                 lTestSuiteList = repTree.GetTestCaseList(ProjectId, TestSuiteId);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView(lTestSuiteList);
        }
        [SessionTimeout]
        public ActionResult LeftPanelDataSet(long lProjectId, long lTestSuiteId, long lTestCaseId, string lProjectName, string lTestSuiteName, string lTestCaseName)
        {
            var ldatasetlist = new List<DataSetListByTestCase>();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                var repTree = new GetTreeRepository();
                ldatasetlist = repTree.GetDataSetList(lProjectId, lTestSuiteId, lTestCaseId, lProjectName, lTestSuiteName, lTestCaseName);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView(ldatasetlist);
        }
        [SessionTimeout]
        public ActionResult LeftPanelStoryboard(long ProjectId)
        {
            var lstoryboardlist = new List<StoryBoardListByProject>();
            try
            {
                DBEntities.ConnectionString = SessionManager.ConnectionString;
                DBEntities.Schema = SessionManager.Schema;

                var repTree = new GetTreeRepository();
                lstoryboardlist = repTree.GetStoryboardList(ProjectId);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured when User page open | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
            }
            return PartialView(lstoryboardlist);
        }
        #endregion

        #region Create Dynamic Connection String 
        public List<DBconnectionViewModel> GetEncodingConnList(List<T_DBCONNECTION> dBconnections)
        {
            var data = new List<DBconnectionViewModel>();
            foreach (var item in dBconnections)
            {
                DBconnectionViewModel db = new DBconnectionViewModel();
                db.connectionId = item.DBCONNECTION_ID;
                db.DecodeMethod = item.DECODE_METHOD;
                db.Databasename = item.DATABASENAME;
                db.Host = PasswordHelper.DecodeMethod(item.HOST, item.DECODE_METHOD.Trim());
                db.Port = PasswordHelper.DecodeMethod(item.PORT, item.DECODE_METHOD.Trim());
                db.UserName = PasswordHelper.DecodeMethod(item.USERNAME, item.DECODE_METHOD.Trim());
                db.Password = PasswordHelper.DecodeMethod(item.PASSWORD, item.DECODE_METHOD.Trim());
                db.Service_Name = PasswordHelper.DecodeMethod(item.SERVICENAME, item.DECODE_METHOD.Trim());
                db.Schema = PasswordHelper.DecodeMethod(item.SCHEMA, item.DECODE_METHOD.Trim());
                data.Add(db);
            }
            return data;
        }
        public DBconnectionViewModel GetConnectionSting(string connection)
        {
            AccountRepository Accountrepo = new AccountRepository();
            DBconnectionViewModel db = new DBconnectionViewModel();
            try
            {
                var connarr = connection.Split('/');
                var lconn = Accountrepo.GetConnectionList();
                var Connlst = GetEncodingConnList(lconn);
                var dbConn = Connlst.Where(x => x.UserName == connarr[0]).FirstOrDefault();
                db.Schema = dbConn.Schema;
                db.Entities = String.Format(dbConn.Databasename,
                          dbConn.Host, dbConn.Password, dbConn.UserName);
                db.App = String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER=DEDICACATED)(SERVICE_NAME={2})));User Id={3};Password={4};pooling=false",
                         dbConn.Host, dbConn.Port, dbConn.Service_Name, dbConn.UserName, dbConn.Password);

                return db;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}

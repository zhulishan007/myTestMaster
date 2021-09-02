using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using MarsApi.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Api.Helper;
using MARS_Repository.Entities;

namespace MARS_Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        //Keeps a User logged in for 1 day
        [HttpPost]
        [Route("api/Login")]
        [AcceptVerbs("GET", "POST")]
        public UserModel Login(string lUserLogin, string lPassword, bool lRememberme = false)
        {
            CommonHelper.SetConnectionString(Request);
            var lModel = CheckCredential(lUserLogin, lPassword);

            if (lRememberme)
            {
                if (lModel != null)
                {
                    HttpCookie ckU = new HttpCookie("UserName");
                    ckU.Expires = DateTime.Now.AddDays(1);
                    ckU.Value = lUserLogin;
                    //Response.Cookies.Add(ckU);

                    HttpCookie ckP = new HttpCookie("Password");
                    ckP.Expires = DateTime.Now.AddDays(1);
                    ckP.Value = lPassword;
                    //Response.Cookies.Add(ckP);
                }
            }

            return lModel;
        }

        //Checks whether the User exists in the system or not
        [Route("api/CheckCredential")]
        [AcceptVerbs("GET", "POST")]
        public virtual UserModel CheckCredential(string lUserLogin, string lPassword)
        {
            var lMsg = "Error";
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            var lUserModel = new UserModel();
            var lUser = Accountrepo.GetUserByEmailAndLoginName(lUserLogin);
            if (lUser != null)
            {
                var lUserPassword = PasswordHelper.DecodeString(lUser.TESTER_PWD);
                if (lUserPassword == lPassword)
                {
                    //SessionManager.TESTER_ID = lUser.TESTER_ID;
                    //SessionManager.TESTER_MAIL = lUser.TESTER_MAIL;
                    //SessionManager.TESTER_NAME_F = lUser.TESTER_NAME_F;
                    //SessionManager.TESTER_NAME_M = lUser.TESTER_NAME_M;
                    //SessionManager.TESTER_NAME_LAST = lUser.TESTER_NAME_LAST;
                    //SessionManager.TESTER_LOGIN_NAME = lUser.TESTER_LOGIN_NAME;
                    //SessionManager.TESTER_NUMBER = lUser.TESTER_NUMBER;

                    //var repTree = new GetTreeRepository();
                    //Session["LeftProjectList"] = repTree.GetProjectList();
                    //  var projeclist= repTree.GetProjectList();
                    //var projeclist = repTree.GetProjectList(lUser.TESTER_ID);
                    lUserModel.TESTER_ID = lUser.TESTER_ID;
                    lUserModel.COMPANY_ID = lUser.COMPANY_ID;
                    lUserModel.TESTER_LOGIN_NAME = lUser.TESTER_LOGIN_NAME;
                    lUserModel.TESTER_MAIL = lUser.TESTER_MAIL;
                    lUserModel.TESTER_NAME_F = lUser.TESTER_NAME_F;
                    lUserModel.TESTER_NAME_LAST = lUser.TESTER_NAME_LAST;
                    lUserModel.TESTER_NAME_M = lUser.TESTER_NAME_M;

                    lMsg = "Succefully Logged!!";
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
            return lUserModel;
        }

        //Reset the password
        [HttpPost]
        [AcceptVerbs("GET", "POST")]
        [Route("api/ResetPwd")]
        public bool ResetPwd(string TESTER_PWD, string emailid)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            var lUser = Accountrepo.GetUserByEmail(emailid);

            var lUserMappId = Accountrepo.GetUserMappingByUserId(lUser.TESTER_ID);

            var flag = false;
            if (lUserMappId != null)
            {
                var testerid = lUser.TESTER_ID;
                var password = PasswordHelper.EncodeString(TESTER_PWD);
                Accountrepo.ChangeUserPassword(password, testerid);
                Accountrepo.UpdateTempKey(lUserMappId.USER_MAPPING_ID, null);
                flag = true;
                //bool luser = Accountrepo.CheckLoginEmailExist(emailid, testerid);
                // Session["ResetSuccessMsg"] = "Successfully Updated Your Password";

            }


            return flag;

        }

        //Logout a User
        [Route("api/Logout")]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage Logout()
        {
            var lMsg = "Error";
            var repTestCase = new TestCaseRepository();
            repTestCase.UpdateIsAvailableReload((long)SessionManager.TESTER_ID);
            // Session.Abandon(); // it will clear the session at the end of request
            lMsg = "Succefully Logged out!!";
            //return RedirectToAction("Login", "Login");
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri("http://localhost:55383/Login/Login");
            return response;
        }

        [HttpPost]
        [Route("api/CheckUserExist")]
        [AcceptVerbs("GET", "POST")]
        public string CheckUserExist(string lUserLogin, string lPassword, string lconnection)
        {
            CommonHelper.SetConnectionString(Request);
            MarsConfig mc = MarsConfig.Configure(lconnection);
            DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();
            var lMsg = "Error";
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
                }
            }

            return lMsg;
        }

        //Method to send mail if user clicks on forget password
        [HttpPost]
        [Route("api/ForgotPsw")]
        [AcceptVerbs("GET", "POST")]
        public string ForgotPsw(string lEmail, string lConnection)
        {
            string MarsEnvironment = string.Empty;
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
            var lMsg = "Error";

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
                lMsg = EmailHelper.ForgotPsw(Emailid, lUserPassword, lUserName, body, r);

            }

            return lMsg;
        }

        [Route("api/ResetPassword")]
        [AcceptVerbs("GET", "POST")]
        public string ResetPassword(string UserEmail, string TempKey)
        {
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);

            AccountRepository Accountrepo = new AccountRepository();
            var lUser = Accountrepo.GetUserByEmail(UserEmail);
            var lUserMappId = Accountrepo.GetUserMappingByUserId(lUser.TESTER_ID);
            if (lUserMappId != null)
            {
                if (lUserMappId.TEMP_KEY == TempKey)
                {
                    return "success";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #region Loads the Mars Tree after the User is logged in
        [Route("api/LeftPanel")]
        [AcceptVerbs("GET", "POST")]
        public List<ProjectByUser> LeftPanel()
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var repTree = new GetTreeRepository();
            var lProjectList = repTree.GetProjectList(SessionManager.TESTER_ID, AppConnDetails.Schema, AppConnDetails.ConnString);
            return lProjectList;
        }

        [Route("api/LeftPanelTestSuite")]
        [AcceptVerbs("GET", "POST")]
        public List<TestSuiteListByProject> LeftPanelTestSuite(long ProjectId)
        {
            var repTree = new GetTreeRepository();
            var lTestSuiteList = repTree.GetTestSuiteList(ProjectId);
            return lTestSuiteList;
        }

        [Route("api/LeftPanelTestCase")]
        [AcceptVerbs("GET", "POST")]
        public List<TestCaseListByProject> LeftPanelTestCase(long ProjectId, long TestSuiteId)
        {
            var repTree = new GetTreeRepository();
            var lTestSuiteList = repTree.GetTestCaseList(ProjectId, TestSuiteId);
            return lTestSuiteList;
        }

        [Route("api/LeftPanelDataSet")]
        [AcceptVerbs("GET", "POST")]
        public List<DataSetListByTestCase> LeftPanelDataSet(long lProjectId, long lTestSuiteId, long lTestCaseId, string lProjectName, string lTestSuiteName, string lTestCaseName)
        {
            var repTree = new GetTreeRepository();
            var ldatasetlist = repTree.GetDataSetList(lProjectId, lTestSuiteId, lTestCaseId, lProjectName, lTestSuiteName, lTestCaseName);
            return ldatasetlist;
        }

        [Route("api/LeftPanelStoryboard")]
        [AcceptVerbs("GET", "POST")]
        public List<StoryBoardListByProject> LeftPanelStoryboard(long ProjectId)
        {
            var repTree = new GetTreeRepository();
            var lstoryboardlist = repTree.GetStoryboardList(ProjectId);
            return lstoryboardlist;
        }
        #endregion
    }
}

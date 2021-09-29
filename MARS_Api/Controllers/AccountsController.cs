using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;

using MARS_Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Repository.Entities;
using MARS_Api.Helper;

namespace MARS_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class AccountsController : ApiController
    {
        //Bind User details
        [Route("api/AddEditUser")]
        [AcceptVerbs("GET", "POST")]
        public UserModel AddEditUser(int? lid)
        {
            CommonHelper.SetConnectionString(Request);
            //ViewBag.Header = "Add User";
            //var repCompany = new CompanyRepository();
            var Accountrepo = new AccountRepository();
            //var lModel = new T_TESTER_INFO();
            UserModel lModel = new UserModel();
            //var lCompanyList = repCompany.GetCompanyList();
            // var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).ToList();
            // ViewBag.listCompany = companylist;
            if (lid != null)
            {
                lModel = Accountrepo.GetUserMappingById((Int32)lid);
                // ViewBag.Header = "Edit User";
            }
            return lModel;
        }

        //List existing users
        [Route("api/DataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel DataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {

                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string NameSearch = string.Empty;
                string ControlTypeSearch = string.Empty;
                string EntryFileSearch = string.Empty;
                string orderDir = string.Empty;
                var repAcc = new AccountRepository();

                string search = searchModel.search.value;
                var draw = searchModel.draw;
                if (searchModel.order.Any())
                {
                    string order = searchModel.order.FirstOrDefault().column.ToString();
                    orderDir = searchModel.order.FirstOrDefault().dir.ToString();

                    colOrderIndex = searchModel.order.FirstOrDefault().column;
                    colDir = searchModel.order.FirstOrDefault().dir.ToString();
                }

                int startRec = searchModel.start;
                int pageSize = searchModel.length;

                if (searchModel.columns.Any())
                {
                    colOrder = searchModel.columns[colOrderIndex].name;

                    NameSearch = searchModel.columns[0].search.value;
                    ControlTypeSearch = searchModel.columns[1].search.value;
                    EntryFileSearch = searchModel.columns[2].search.value;
                }

                var data = new List<UserModel>();
                data = repAcc.ListAllUsers().ToList();
                int totalRecords = data.Count();
                int recFilter = 0;
                baseModel.data = data;
                baseModel.status = 1;
                baseModel.message = "Success";
                baseModel.recordsTotal = recordsTotal;
                baseModel.recordsFiltered = recFilter;
                baseModel.draw = draw;
            }
            catch (Exception ex)
            {
                baseModel.data = null;
                baseModel.status = 0;
                baseModel.message = "Error : " + ex.ToString();
            }

            return baseModel;
        }
        //Add or Edit a User
        [HttpPost]
        [Route("api/AddEditUser")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel AddEditUser(UserModel userModel)
        {

            BaseModel baseModel = new BaseModel();
            var Accountrepo = new AccountRepository();
            var repentil = new EntitlementRepository();
            var t_TESTER = Accountrepo.ConverUserModel(userModel);
            var lchecked = t_TESTER.AVAILABLE_MARK;
            t_TESTER.AVAILABLE_MARK = null;

            if (t_TESTER.TESTER_ID == 0)
            {
                t_TESTER.TESTER_PWD = PasswordHelper.EncodeString(t_TESTER.TESTER_PWD);
                var result = Accountrepo.CheckLoginEmailExist(t_TESTER.TESTER_MAIL, t_TESTER.TESTER_ID);
                if (result)
                {
                    baseModel.status = 1;
                    baseModel.message = "Email [" + t_TESTER.TESTER_MAIL + "] already exists";
                }

                var loginresult = Accountrepo.CheckLoginNameExist(t_TESTER.TESTER_LOGIN_NAME, t_TESTER.TESTER_ID);
                if (loginresult)
                {
                    baseModel.status = 1;
                    baseModel.message = "User name [" + t_TESTER.TESTER_LOGIN_NAME + "] already exists.";
                }
                t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
                //role save
                if (!string.IsNullOrEmpty(userModel.RoleIds))
                {
                    var roleresult = repentil.AddRole(t_TESTER.TESTER_ID, userModel.RoleIds);
                }

                baseModel.message = "User created successfully.";
                baseModel.status = 1;
                baseModel.data = true;
            }
            else
            {
                var result = Accountrepo.CheckLoginEmailExist(t_TESTER.TESTER_MAIL, t_TESTER.TESTER_ID);
                if (result)
                {
                    baseModel.status = 1;
                    baseModel.message = "Email invalid";
                }
                t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
                baseModel.message = "User Updated successfully.";
                baseModel.status = 1;
                baseModel.data = true;
            }

            return baseModel;
        }
        //public bool AddEditUser(T_TESTER_INFO t_TESTER)
        //{
        //    bool flag = false;
        //    CommonHelper.SetConnectionString(Request);
        //    var Accountrepo = new AccountRepository();
        //    //var repCompany = new CompanyRepository();
        //    //var lCompanyList = repCompany.GetCompanyList();
        //    //var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).ToList();
        //    var lchecked = t_TESTER.AVAILABLE_MARK;
        //    t_TESTER.AVAILABLE_MARK = null;

        //    if (t_TESTER.TESTER_ID == 0)
        //    {
        //        t_TESTER.TESTER_PWD = PasswordHelper.EncodeString(t_TESTER.TESTER_PWD);
        //    }
        //    if (t_TESTER.TESTER_ID == 0)
        //    {
        //        t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
        //        flag = true;
        //    }
        //    else
        //    {
        //        t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
        //        flag = true;
        //    }
        //    return flag;
        //}

        //Check whether Tester_Login_Name exists or not
        [HttpPost]
        [Route("api/CheckLoginNameExist")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckLoginNameExist(string lLoginName, int? lLoginId)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            var lflag = Accountrepo.CheckLoginNameExist(lLoginName, lLoginId);
            return lflag;
        }

        //Check whether Email Id exists or not
        [HttpPost]
        [Route("api/CheckEmailExist")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckEmailExist(string lLoginEmail, int? lLoginId)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            var lflag = Accountrepo.CheckLoginEmailExist(lLoginEmail, lLoginId);
            return lflag;
        }

        //Check whether User's Password exists or not
        [HttpPost]
        [Route("api/ChangeUserPassowrd")]
        [AcceptVerbs("GET", "POST")]
        public string ChangeUserPassowrd(string lOldPsw, string lNewPsw, int lUserId)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            string lMsg = "Old Password Not Matched.";
            var lUser = Accountrepo.GetUserById(lUserId);
            var lOldPassword = PasswordHelper.DecodeString(lUser.TESTER_PWD);

            if (lOldPsw == lOldPassword)
            {
                lNewPsw = PasswordHelper.EncodeString(lNewPsw);
                var lflag = Accountrepo.ChangeUserPassword(lNewPsw, (decimal)lUserId);
                lMsg = "Succefully Updated your Password";
            }

            return lMsg;
        }

        //Delete a User
        [HttpPost]
        [Route("api/DeleteUser")]
        [AcceptVerbs("GET", "POST")]
        public string DeleteUser(int id)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository repo = new AccountRepository();
            var lresult = repo.DeleteUser(id);
            return "success";
        }

        //Change User's status to Active/Inactive
        [HttpPost]
        [Route("api/ChangeUserStatus")]
        [AcceptVerbs("GET", "POST")]
        public string ChangeUserStatus(int Id, int Checked)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository repo = new AccountRepository();
            var lresult = repo.ChangeUserStatus(Id, Checked);
            return "success";
        }

        [Route("api/GetUserName")]
        [AcceptVerbs("GET", "POST")]
        public List<T_TESTER_INFO> GetUserName()
        {
            CommonHelper.SetConnectionString(Request);

            var repAcc = new AccountRepository();
            var result = repAcc.GetAllUsers();
            return result;
        }

        [Route("api/AddUserExePath")]
        [AcceptVerbs("GET", "POST")]
        public string AddUserExePath(long userid, long relationid, string exepath)
        {
            CommonHelper.SetConnectionString(Request);
            var repAcc = new AccountRepository();
            var result = repAcc.AddUserPath(userid, relationid, exepath);
            return result;
        }

        [Route("api/DeleteUserRelExePath")]
        [AcceptVerbs("GET", "POST")]
        public bool DeleteUserRelExePath(long Userid)
        {
            CommonHelper.SetConnectionString(Request);
            var repAcc = new AccountRepository();
            var result = repAcc.DeleteUserMapExePath(Userid);
            return result;
        }

        [Route("api/UsersDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel UsersDataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string FirstNameSearch = string.Empty;
                string MiddelNameSearch = string.Empty;
                string LastNameSearch = string.Empty;
                string UserNameSearch = string.Empty;
                string EmailSearch = string.Empty;
                string CompanySearch = string.Empty;

                var repAcc = new AccountRepository();

                string search = searchModel.search.value;
                var draw = searchModel.draw;
                if (searchModel.order.Any())
                {
                    string order = searchModel.order.FirstOrDefault().column.ToString();
                    string orderDir = searchModel.order.FirstOrDefault().dir.ToString();

                    colOrderIndex = searchModel.order.FirstOrDefault().column;
                    colDir = searchModel.order.FirstOrDefault().dir.ToString();
                }

                int startRec = searchModel.start;
                int pageSize = searchModel.length;

                if (searchModel.columns.Any())
                {
                    colOrder = searchModel.columns[colOrderIndex].name;

                    FirstNameSearch = searchModel.columns[0].search.value;
                    MiddelNameSearch = searchModel.columns[1].search.value;
                    LastNameSearch = searchModel.columns[2].search.value;
                    UserNameSearch = searchModel.columns[3].search.value;
                    EmailSearch = searchModel.columns[4].search.value;
                    CompanySearch = searchModel.columns[5].search.value;
                }
                var data = new List<UserModel>();
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

                int totalRecords = data.Count();
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

                int recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();

                baseModel.data = data;
                baseModel.status = 1;
                baseModel.message = "Success";
                baseModel.recordsTotal = recordsTotal;
                baseModel.recordsFiltered = recFilter;
                baseModel.draw = draw;
            }
            catch (Exception ex)
            {
                baseModel.data = null;
                baseModel.status = 0;
                baseModel.message = "Error : " + ex.ToString();
            }

            return baseModel;
        }

        [Route("api/ExePathDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel ExePathDataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string UserNameSearch = string.Empty;
                string Pathsearch = string.Empty;

                var repAcc = new AccountRepository();

                string search = searchModel.search.value;
                var draw = searchModel.draw;
                if (searchModel.order.Any())
                {
                    string order = searchModel.order.FirstOrDefault().column.ToString();
                    string orderDir = searchModel.order.FirstOrDefault().dir.ToString();

                    colOrderIndex = searchModel.order.FirstOrDefault().column;
                    colDir = searchModel.order.FirstOrDefault().dir.ToString();
                }

                int startRec = searchModel.start;
                int pageSize = searchModel.length;

                if (searchModel.columns.Any())
                {
                    colOrder = searchModel.columns[colOrderIndex].name;

                    UserNameSearch = searchModel.columns[0].search.value;
                    Pathsearch = searchModel.columns[1].search.value;
                }
                var data = new List<RelUserExePath>();
                data = repAcc.ListUserExePath().ToList();

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

                baseModel.data = data;
                baseModel.status = 1;
                baseModel.message = "Success";
                baseModel.recordsTotal = recordsTotal;
                baseModel.recordsFiltered = recFilter;
                baseModel.draw = draw;
            }
            catch (Exception ex)
            {
                baseModel.data = null;
                baseModel.status = 0;
                baseModel.message = "Error : " + ex.ToString();
            }

            return baseModel;
        }

        //This method will load all the data and filter them
        [Route("api/DataLoadUserActivePage")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel DataLoadUserActivePage([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string UserNameSearch = string.Empty;
                string PageNameSearch = string.Empty;
                var repAcc = new AccountRepository();

                string search = searchModel.search.value;
                var draw = searchModel.draw;
                if (searchModel.order.Any())
                {
                    string order = searchModel.order.FirstOrDefault().column.ToString();
                    string orderDir = searchModel.order.FirstOrDefault().dir.ToString();

                    colOrderIndex = searchModel.order.FirstOrDefault().column;
                    colDir = searchModel.order.FirstOrDefault().dir.ToString();
                }

                int startRec = searchModel.start;
                int pageSize = searchModel.length;

                if (searchModel.columns.Any())
                {
                    colOrder = searchModel.columns[colOrderIndex].name;

                    UserNameSearch = searchModel.columns[0].search.value;
                    PageNameSearch = searchModel.columns[1].search.value;

                }
                var data = repAcc.ListAllActiveUsers();

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

                baseModel.data = data;
                baseModel.status = 1;
                baseModel.message = "Success";
                baseModel.recordsTotal = recordsTotal;
                baseModel.recordsFiltered = recFilter;
                baseModel.draw = draw;
            }
            catch (Exception ex)
            {
                baseModel.data = null;
                baseModel.status = 0;
                baseModel.message = "Error : " + ex.ToString();
            }

            return baseModel;
        }

        //Delete the Active User object data by ID
        [Route("api/DeleteActiveUser")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel DeleteActiveUser(long id)
        {
            CommonHelper.SetConnectionString(Request);
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
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Add/Delete User Pin/UnPin objects values
        [Route("api/UserPinUnPinTab")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel UserPinUnPinTab(string datatab, long dataid, string dataname, string linkText, long ProjectId)
        {
            CommonHelper.SetConnectionString(Request);
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
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //check Pin tab already exist or not 
        [Route("api/CheckPinExist")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckPinExist(string datatab, long dataid, long ProjectId)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository repo = new AccountRepository();
            var userId = SessionManager.TESTER_ID;
            var lresult = repo.CheckPinExist(userId, datatab, dataid, ProjectId);
            return lresult;
        }

        //This method get all storyboard
        [Route("api/GetStoryboradNameyId")]
        [AcceptVerbs("GET", "POST")]
        public string GetStoryboradNameyId(long StoryBoardid)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository repo = new AccountRepository();
            var list = repo.GetStoryboradNameyId(StoryBoardid);
            var lresult = list != null ? list.STORYBOARD_NAME : "";
            return lresult;
        }

        //This method get all TestsuiteId by TeastcaseId
        [Route("api/GetTestsuiteIdByTeastcaseId")]
        [AcceptVerbs("GET", "POST")]
        public TestCaseTestSuiteModel GetTestsuiteIdByTeastcaseId(long Tid)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository repo = new AccountRepository();
            var lresult = repo.GetTestsuiteIdByTeastcaseId(Tid);
            return lresult;
        }

        //This method get dataset list by Id 
        [Route("api/GetDataSetListbyId")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel GetDataSetListbyId(long lTestCaseId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTree = new GetTreeRepository();
                var ldatasetlist = repTree.GetDataSetListbyId(lTestCaseId);

                resultModel.data = ldatasetlist;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }
    }
    public class Data
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}

/// <summary>
/// *********************************************************
/// Description: This Web API controller contains all API 
///              related to Account operatins like Get Users
///              Add User / Edit User etc
/// *********************************************************
/// </summary>


using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using System.Web.Script.Serialization;
using MARS_Web.Helper;
using System.Web.Http.Cors;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Repository.Entities;
using MARS_Api.Helper;

namespace MarsApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    // [Route("api/Account")]
    public class AccountController : ApiController
    {
        [System.Web.Http.Route("api/Listusers")]
        [AcceptVerbs("GET", "POST")]
        public List<UserModel> Listusers()
        {
            CommonHelper.SetConnectionString(Request);
            var Accountrepo = new AccountRepository();
            var result = Accountrepo.ListAllUsers();

            return result;
        }
        [System.Web.Http.Route("api/CreateUser")]
        //[HttpPost]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]

        public T_TESTER_INFO CreateUser(T_TESTER_INFO t_TESTER, decimal? lchecked)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            //T_TESTER_INFO testerinfo = new T_TESTER_INFO();
            var lresult = Accountrepo.CheckLoginNameExist(t_TESTER.TESTER_LOGIN_NAME, t_TESTER.TESTER_ID);
            if (lresult == true)
            {
                t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
            }
            return new T_TESTER_INFO();
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public T_TESTER_INFO DeleteUser(int id)
        {
            CommonHelper.SetConnectionString(Request);
            AccountRepository Accountrepo = new AccountRepository();
            var lresult = Accountrepo.DeleteUser(id);
            return lresult;
        }

        [System.Web.Http.Route("api/GetUserById")]
        [AcceptVerbs("GET", "POST")]
        public UserModel GetUserById(int? lid)
        {
            CommonHelper.SetConnectionString(Request);
            //ViewBag.Header = "Add User";
            var repCompany = new CompanyRepository();
            var Accountrepo = new AccountRepository();
            //var lModel = new T_TESTER_INFO();
            var lModel = new UserModel();
            var lCompanyList = repCompany.GetCompanyList();
            var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).OrderBy(x => x.Text).ToList();
            //ViewBag.listCompany = companylist;
            if (lid != null)
            {
                lModel = Accountrepo.GetUserMappingById((Int32)lid);
                //ViewBag.Header = "Edit User";
            }
            return lModel;
        }
        [System.Web.Http.Route("api/GetCompanies")]
        [AcceptVerbs("GET", "POST")]
        public List<SelectListItem> GetCompanies()
        {
            CommonHelper.SetConnectionString(Request);
            var repCompany = new CompanyRepository();
            var lCompanyList = repCompany.GetCompanyList();
            var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).OrderBy(x => x.Text).ToList();
            return companylist;
        }

        //[System.Web.Http.Route("api/AddEditUser")]
        //[AcceptVerbs("GET", "POST")]
        //public string AddEditUser(string test)
        //{
        //    //ViewBag.Header = "Add User";
        //    return "success";
        //}
        [System.Web.Http.Route("api/AddEditUser1")]
        [AcceptVerbs("GET", "POST")]
        public string AddEditUser1(UserModel value)
        {
            CommonHelper.SetConnectionString(Request);
            var Accountrepo = new AccountRepository();
            var repCompany = new CompanyRepository();
            var lCompanyList = repCompany.GetCompanyList();
            var companylist = lCompanyList.Select(c => new SelectListItem { Text = c.COMPANY_NAME, Value = c.COMPANY_ID.ToString() }).OrderBy(x => x.Text).ToList();
            T_TESTER_INFO t_TESTER = new T_TESTER_INFO();
            t_TESTER.TESTER_ID = value.TESTER_ID;
            t_TESTER.TESTER_DESC = value.TESTER_DESC;
            t_TESTER.TESTER_LOGIN_NAME = value.TESTER_LOGIN_NAME;
            t_TESTER.TESTER_MAIL = value.TESTER_MAIL;
            t_TESTER.TESTER_NAME_F = value.TESTER_NAME_F;
            t_TESTER.TESTER_NAME_LAST = value.TESTER_NAME_LAST;
            t_TESTER.TESTER_NAME_M = value.TESTER_NAME_M;
            t_TESTER.TESTER_NUMBER = value.TESTER_NUMBER;
            if (t_TESTER.TESTER_ID != 0) {
                t_TESTER.TESTER_PWD = value.TESTER_PWD;
                t_TESTER.COMPANY_ID = value.COMPANY_ID;
            }
            //t_TESTER.T_MARS_COMPANY = value.T_MARS_COMPANY;
            //t_TESTER.T_USER_MAPPING = value.T_USER_MAPPING;

            var lchecked = t_TESTER.AVAILABLE_MARK;
            t_TESTER.AVAILABLE_MARK = null;

            if (t_TESTER.TESTER_ID == 0)
            {
                t_TESTER.TESTER_PWD = PasswordHelper.EncodeString(t_TESTER.TESTER_PWD);
            }
            if (t_TESTER.TESTER_ID == 0)
            {
                t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);                
            }
            else
            {
                t_TESTER = Accountrepo.CreateNewUser(t_TESTER, lchecked);
            }
            return "success";
        }
    }

    public class Data
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}

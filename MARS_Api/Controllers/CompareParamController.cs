using MARS_Api.Helper;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;

namespace MARS_Api.Controllers
{
    [Authorize]
    public class CompareParamController : ApiController
    {
        [Route("api/ListCompareConfig")]
        [AcceptVerbs("GET", "POST")]
        public List<CompareParam> ListCompareConfig()
        {
            CommonHelper.SetConnectionString(Request);
            CompareParamRepository obj = new CompareParamRepository();
            obj.Username = SessionManager.TESTER_LOGIN_NAME;
            var lresult = obj.ListCompareConfig();
            return lresult;
        }
        //[Route("api/DeleteCompareconfig")]
        //[AcceptVerbs("GET", "POST")]
        //public bool DeleteCompareconfig(int datasourceid)
        //{
        //    CompareParamRepository obj = new CompareParamRepository();
        //    var result = obj.DeleteCompareConfig(datasourceid);
        //    return result;
        //}

        //[Route("api/AddoreditCompareConfig")]
        //[AcceptVerbs("GET", "POST")]
        //public string AddoreditCompareConfig(T_DATA_SOURCE t_DATA_SOURCE)
        //{
        //    CompareParamRepository obj = new CompareParamRepository();
        //    var result = obj.AddorEditCompareconfig(t_DATA_SOURCE);
        //    return result;
        //}
        [Route("api/AddoreditCompareConfig")]
        [AcceptVerbs("GET", "POST")]
        public string AddoreditCompareConfig(string id, string data, short datatype)
        {
            CommonHelper.SetConnectionString(Request);
            CompareParamRepository obj = new CompareParamRepository();
            obj.Username = SessionManager.TESTER_LOGIN_NAME;
            var result = obj.AddorEditCompareconfig(id,data,datatype);
            return result;
        }
        [Route("api/DeleteCompareconfig")]
        [AcceptVerbs("GET", "POST")]
        public string DeleteCompareconfig(string id,short datatype)
        {
            CommonHelper.SetConnectionString(Request);
            CompareParamRepository obj = new CompareParamRepository();
            obj.Username = SessionManager.TESTER_LOGIN_NAME;
            var result = obj.DeleteCompareConfig(id,datatype);
            return result;
        }

    }
}

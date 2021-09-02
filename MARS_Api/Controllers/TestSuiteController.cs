using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using MARS_Repository.Repositories;
using Newtonsoft.Json;
using MARS_Repository.ViewModel;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using System.Web;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Repository.Entities;
using MARS_Api.Helper;

namespace MarsApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class TestSuiteController : ApiController
    {
        //List all Test Suites
        //[Route("api/GetTestSuites")]
        //[AcceptVerbs("GET", "POST")]
        //public List<TestSuiteModel> GetTestSuites()
        //{
        //  CommonHelper.SetConnectionString(Request);
        //  var testsuiterepo = new TestSuiteRepository();
        //  var result = testsuiterepo.ListAllTestSuites();
        //  return result;
        //}

        //Add or edit Test Suite
        [Route("api/AddEditTestSuite")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditTestSuite(TestSuiteModel lModel)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            var testsuiterepo = new TestSuiteRepository();
           // var lresult = testsuiterepo.AddEditTestSuite(lModel);
            var flag = lModel.TestSuiteId == 0 ? "added" : "Saved";
            var result = testsuiterepo.CheckTestSuiteInStoryboardByProject(lModel.TestSuiteId, lModel.ProjectId);
            if (result.Count <= 0)
            {
                var editresult = testsuiterepo.AddEditTestSuite(lModel);
                resultModel.data = editresult;
                resultModel.message = "Successfully " + flag + " Test Case.";
            }
            else
            {
                resultModel.data = result;
                resultModel.message = "Test suite already exist in storyboard";
            }
            
                return resultModel;
        }

        //Change Test Suite Name
        [Route("api/ChangeTestSuiteName")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel ChangeTestSuiteName(string TestSuiteName, string testsuitedesc, long TestSuiteId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            var testsuiterepo = new TestSuiteRepository();
           // var lResult = testsuiterepo.ChangeTestSuiteName(TestSuiteName, testsuitedesc, TestSuiteId);
           
           var result = testsuiterepo.CheckDuplicateTestSuiteName(TestSuiteName, TestSuiteId);
            if (result)
            {
                resultModel.data = result;
                resultModel.message = "Test Suite name already exist";
            }
            else
            {
                var renamecase = testsuiterepo.ChangeTestSuiteName(TestSuiteName, testsuitedesc, TestSuiteId);
                resultModel.data = renamecase;
                resultModel.message = "Test Suite name successfully changed";
            }
            return resultModel;
        }

        //Check whether duplicate TestSuite exists or not
        [Route("api/CheckDuplicateTestSuiteNameExist")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckDuplicateTestSuiteNameExist(string TestSuiteName, long? TestSuiteId)
        {
            CommonHelper.SetConnectionString(Request);
            var testsuiterepo = new TestSuiteRepository();
            var lResult = testsuiterepo.CheckDuplicateTestSuiteName(TestSuiteName, TestSuiteId);
            return lResult;
        }

        //Delete a Test Suite
        [Route("api/DeleteTestSuite")]
        [AcceptVerbs("GET", "POST")]
        public bool DeleteTestSuite(long TestSuiteId)
        {
            CommonHelper.SetConnectionString(Request);
            var testsuiterepo = new TestSuiteRepository();
            var lResult = testsuiterepo.DeleteTestSuite(TestSuiteId);
            return lResult;
        }
        [Route("api/GetProjectByApplicaton")]
        [AcceptVerbs("GET", "POST")]
        public List<RelProjectApplication> GetProjectByApplicaton(string ApplicationId)
        {
            CommonHelper.SetConnectionString(Request);
            var repTestSuite = new ProjectRepository();
            var lResult = new List<RelProjectApplication>();
            if (!string.IsNullOrEmpty(ApplicationId))
            {
                lResult = repTestSuite.ListRelProjectApp(ApplicationId);
            }

            return lResult;
        }

        [Route("api/GetApplicationList")]
        [AcceptVerbs("GET", "POST")]
        public List<T_REGISTERED_APPS> GetApplicationList()
        {
            CommonHelper.SetConnectionString(Request);
            ApplicationRepository repo = new ApplicationRepository();

            var lapp = repo.ListApplication();
            //var applist = lapp.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).ToList();
            return lapp;
        }

        [Route("api/TestSuiteDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel TestSuiteDataLoad([FromBody]SearchModel searchModel)
        {
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string NameSearch = string.Empty;
                string DescriptionSearch = string.Empty;
                string ApplicationSearch = string.Empty;
                string ProjectSearch = string.Empty;
                string orderDir = string.Empty;

                var repAcc = new TestSuiteRepository();

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
                    DescriptionSearch = searchModel.columns[1].search.value;
                    ApplicationSearch = searchModel.columns[2].search.value;
                    ProjectSearch = searchModel.columns[3].search.value;
                }

                var data = repAcc.ListAllTestSuites(AppConnDetails.Schema, AppConnDetails.ConnString, startRec, pageSize, colOrder, orderDir, NameSearch, DescriptionSearch, ApplicationSearch, ProjectSearch);

                int totalRecords = 0;
                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().TotalCount;
                }
                int recFilter = 0;
                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().TotalCount;
                }

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
    }

}

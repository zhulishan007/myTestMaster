using MARS_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;


namespace MARS_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TestProjectController : ApiController
    {
        //Change Project Name
        [Route("api/ChangeTestProjectName")]
        [AcceptVerbs("GET", "POST")]
        public bool ChangeTestProjectName(string TestProjectName, long TestProjectId)
        {
            var testProjectrepo = new TestProjectRepository();
            var result = testProjectrepo.ChangeTestProjectName(TestProjectName, TestProjectId);
            return result;
        }

        //Check whether duplicate Project name exists or not
        [Route("api/CheckDuplicateTestProjectNameExist")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckDuplicateTestProjectNameExist(string TestProjectName, long? TestProjectId)
        {
            var testProjectrepo = new TestProjectRepository();
            var result = testProjectrepo.CheckDuplicateTestProjectName(TestProjectName, TestProjectId);
            return result;
        }

        [Route("api/Deletproject")]
        [AcceptVerbs("GET", "POST")]
        public bool Deletproject(long projectid)
        {
            var repo = new ProjectRepository();
            var result = repo.Deleteproject(projectid);
            var repTree = new GetTreeRepository();
            return result;
        }
    }
}
using MARS_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Api.Helper;
using MARS_Repository.ViewModel;
using System.Web.Script.Serialization;

namespace MARS_Api.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  [Authorize]
  public class ProjectController : ApiController
  {
    // Change Project Name
    [Route("api/ChangeProjectName")]
    [AcceptVerbs("GET", "POST")]
    public bool ChangeProjectName(string ProjectName, string projectdesc, long ProjectId)
    {
      CommonHelper.SetConnectionString(Request);
      var Projectrepo = new ProjectRepository();
      var result = Projectrepo.ChangeProjectName(ProjectName, projectdesc, ProjectId);
      return result;
    }

    //Check whether duplicate Projects exists in the database or not
    [Route("api/CheckDuplicateProjectNameExist")]
    [AcceptVerbs("GET", "POST")]
    public bool CheckDuplicateProjectNameExist(string ProjectName, long? ProjectId)
    {
      CommonHelper.SetConnectionString(Request);
      var Projectrepo = new ProjectRepository();
      var result = Projectrepo.CheckDuplicateProjectName(ProjectName, ProjectId);
      return result;
    }

    [Route("api/Deletproject")]
    [AcceptVerbs("GET", "POST")]
    public bool Deletproject(long projectid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new ProjectRepository();
      var result = repo.DeleteProject(projectid);

      return result;
    }

    [Route("api/ProjectDataLoad")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel ProjectDataLoad([FromBody]SearchModel searchModel)
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
        string DescriptionSearch = string.Empty;
        string ApplicationSearch = string.Empty;
        string StatusSearch = string.Empty;
        string orderDir = string.Empty;
        var repAcc = new ProjectRepository();

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
          StatusSearch = searchModel.columns[3].search.value;
        }

        var data = new List<ProjectViewModel>();

        data = repAcc.ListAllProject(AppConnDetails.Schema, AppConnDetails.ConnString, startRec, pageSize, colOrder, orderDir, NameSearch, DescriptionSearch, ApplicationSearch, StatusSearch);

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

    [Route("api/SaveProjectChangesByUser")]
    [HttpPost]
    [AcceptVerbs("GET", "POST")]
    public string SaveProjectChangesByUser(string model, decimal userid, decimal loginid)
    {
      var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
      CommonHelper.SetConnectionString(Request);
      var repTree = new GetTreeRepository();
      ProjectRepository repo = new ProjectRepository();
      JavaScriptSerializer js = new JavaScriptSerializer();
      var lobj = js.Deserialize<Project[]>(model);
      var lproject = lobj.ToList();
      if (loginid == userid)
      {
        var result = repo.SaveProjectByUserId(lproject, loginid);
        repTree.GetProjectList(loginid, AppConnDetails.Schema, AppConnDetails.ConnString);
        return result;
      }
      else
      {
        var resultbyuser = repo.SaveProjectByUserId(lproject, userid);
        return resultbyuser;
      }

    }

    [Route("api/ProjectListByUserId")]
    [AcceptVerbs("GET", "POST")]
    public List<ProjectByUser> ProjectListByUserId(decimal userid, decimal loginid)
    {
      CommonHelper.SetConnectionString(Request);
      if (userid == 0)
      {
        var repTree = new GetTreeRepository();
        var lProjectList = repTree.ProjectListByUserName(loginid);
        return lProjectList;
      }
      else
      {
        var repTree = new GetTreeRepository();
        var lProjectList = repTree.ProjectListByUserName(userid);
        return lProjectList;
      }
    }

    [Route("api/DeleteUserProjectMapping")]
    [AcceptVerbs("GET", "POST")]
    public string DeleteUserProjectMapping(long userid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new ProjectRepository();
      var result = repo.DeleteProjectUserMapping(userid);
      return result;
    }

    [HttpPost]
    [Route("api/AddEditProject")]
    [AcceptVerbs("GET", "POST")]
    public bool AddEditProject(ProjectViewModel lModel, string loginname, decimal loginid)
    {
      var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
      CommonHelper.SetConnectionString(Request);
      var projectrepo = new ProjectRepository();
      lModel.CarectorName = loginname;
      var lResult = projectrepo.AddEditProject(lModel);
      var repTree = new GetTreeRepository();
      repTree.GetProjectList(loginid, AppConnDetails.Schema, AppConnDetails.ConnString);
      return lResult;

    }

    [Route("api/UsersWithProjectMappingDataLoad")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel UsersWithProjectMappingDataLoad([FromBody]SearchModel searchModel)
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
        string LastNameSearch = string.Empty;
        string UserNameSearch = string.Empty;
        string EmailSearch = string.Empty;
        string Projectsearch = string.Empty;

        var recacc = new AccountRepository();

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
          LastNameSearch = searchModel.columns[1].search.value;
          UserNameSearch = searchModel.columns[2].search.value;
          EmailSearch = searchModel.columns[3].search.value;
          Projectsearch = searchModel.columns[4].search.value;
        }

        var data = recacc.ListAllUsersWithProjectMapping();

        if (!string.IsNullOrEmpty(LastNameSearch))
        {
          data = data.Where(x => !string.IsNullOrEmpty(x.TESTER_NAME_LAST) && x.TESTER_NAME_LAST.ToLower().Trim().Contains(LastNameSearch.ToLower().Trim())).ToList();
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
        if (!string.IsNullOrEmpty(Projectsearch))
        {
          data = data.Where(x => !string.IsNullOrEmpty(x.ProjectName) && x.ProjectName.ToLower().Trim().Contains(Projectsearch.ToLower().Trim())).ToList();
        }
        if (colDir == "desc")
        {
          switch (colOrder)
          {
            case "Last Name":
              data = data.OrderByDescending(a => a.TESTER_NAME_LAST).ToList();
              break;
            case "First Name":
              data = data.OrderByDescending(a => a.TESTER_NAME_F).ToList();
              break;
            case "User Name":
              data = data.OrderByDescending(a => a.TESTER_LOGIN_NAME).ToList();
              break;
            case "Email Address":
              data = data.OrderByDescending(a => a.TESTER_MAIL).ToList();
              break;
            case "ProjectId":
              data = data.OrderByDescending(a => a.ProjectName).ToList();
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
            case "First Name":
              data = data.OrderBy(a => a.TESTER_NAME_F).ToList();
              break;
            case "User Name":
              data = data.OrderBy(a => a.TESTER_LOGIN_NAME).ToList();
              break;

            case "Email Address":
              data = data.OrderBy(a => a.TESTER_MAIL).ToList();
              break;
            case "ProjectId":
              data = data.OrderBy(a => a.ProjectName).ToList();
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
                                  p.TESTER_LOGIN_NAME.ToString().ToLower().Contains(search.ToLower())).ToList();
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
  }
}

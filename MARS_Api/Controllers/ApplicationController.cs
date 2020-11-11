using MARS_Api.Helper;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;

namespace MARS_Api.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  [Authorize]
  public class ApplicationController : ApiController
  {
    [Route("api/CheckDuplicateApplicationNameExist")]
    [AcceptVerbs("GET", "POST")]
    public bool CheckDuplicateApplicationNameExist(string applicationname, long? ApplicationId)
    {
      CommonHelper.SetConnectionString(Request);
      applicationname = applicationname.Trim();
      var _apprepository = new ApplicationRepository();
      var result=_apprepository.CheckDuplicateApplicationNameExist(applicationname, ApplicationId);
      return result;
    }

    [HttpPost]
    [Route("api/AddEditApplication")]
    [AcceptVerbs("GET", "POST")]
    public bool AddEditApplication(ApplicationViewModel applicationviewmodel,string loginname)
    {
      CommonHelper.SetConnectionString(Request);
      var _apprepository = new ApplicationRepository();

      applicationviewmodel.Create_Person = loginname;
      var _addeditResult = _apprepository.AddEditApplication(applicationviewmodel);
      return _addeditResult;
    }

    //Delete the Application object data by applicationID
    [Route("api/DeletApplication")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel DeletApplication(long ApplicationId)
    {
      CommonHelper.SetConnectionString(Request);
      var _apprepository = new ApplicationRepository();
      var _treerepository = new GetTreeRepository();
      BaseModel baseModel = new BaseModel();
      var lflag = _apprepository.CheckTestCaseExistsInAppliction(ApplicationId);

      if (lflag.Count <= 0)
      {
        var _deleteResult = _apprepository.DeleteApplication(ApplicationId);
        baseModel.status = 1;
        baseModel.message = "success";
      }
      else
      {
        baseModel.status = 1;
        baseModel.data = lflag;
        baseModel.message = "error";
      }
      return baseModel;
    }

        [Route("api/ApplicationDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel ApplicationDataLoad([FromBody]SearchModel searchModel)
        {
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
                string VersionSearch = string.Empty;
                string ExtraSearch = string.Empty;
                string StatusSearch = string.Empty;

                var _apprepository = new ApplicationRepository();

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

                    NameSearch = searchModel.columns[0].search.value;
                    DescriptionSearch = searchModel.columns[1].search.value;
                    VersionSearch = searchModel.columns[2].search.value;
                    ExtraSearch = searchModel.columns[3].search.value;
                    StatusSearch = searchModel.columns[4].search.value;
                }

                var data = _apprepository.GetApplicationList();

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ApplicationName) && x.ApplicationName.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DescriptionSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.ToString().ToLower().Contains(DescriptionSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(VersionSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Version) && x.Version.ToLower().Trim().Contains(VersionSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(ExtraSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ExtraRequirement) && x.ExtraRequirement.ToLower().Trim().Contains(ExtraSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(StatusSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Mode) && x.ExtraRequirement.ToLower().Trim().Contains(StatusSearch.ToLower().Trim())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.ApplicationName).ToList();
                            break;
                        case "Description":
                            data = data.OrderByDescending(a => a.Description).ToList();
                            break;
                        case "Version":
                            data = data.OrderByDescending(a => a.Version).ToList();
                            break;
                        case "Extra Requirement":
                            data = data.OrderByDescending(a => a.ExtraRequirement).ToList();
                            break;
                        case "Status":
                            data = data.OrderByDescending(a => a.Mode).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.ApplicationName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.ApplicationName).ToList();
                            break;
                        case "Description":
                            data = data.OrderBy(a => a.Description).ToList();
                            break;
                        case "Version":
                            data = data.OrderBy(a => a.Version).ToList();
                            break;
                        case "Extra Requirement":
                            data = data.OrderBy(a => a.ExtraRequirement).ToList();
                            break;
                        case "Status":
                            data = data.OrderByDescending(a => a.Mode).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.ApplicationName).ToList();
                            break;
                    }
                }
                int totalRecords = data.Count();

                if (!string.IsNullOrEmpty(search) &&
                 !string.IsNullOrWhiteSpace(search))
                {
                    data = data.Where(p => p.ApplicationName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Version.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ExtraRequirement.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Mode.ToString().ToLower().Contains(search.ToLower())
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
    }
}

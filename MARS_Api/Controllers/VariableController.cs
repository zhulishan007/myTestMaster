using MARS_Api.Helper;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;

namespace MARS_Api.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  [Authorize]
  public class VariableController : ApiController
  {

    [Route("api/GetVariableByID")]
    [AcceptVerbs("GET", "POST")]
    public SYSTEM_LOOKUP GetVariableByID(int lid)
    {
      CommonHelper.SetConnectionString(Request);
      VariableRepository vrepo = new VariableRepository();
      var lresult = vrepo.GetVariableNameById(lid);
      return lresult;
    }
    [HttpPost]
    [Route("api/AddEditVariableSave")]
    [AcceptVerbs("GET", "POST")]
    public string AddEditVariableSave(VariableModel model)
    {
      CommonHelper.SetConnectionString(Request);
      VariableRepository repo = new VariableRepository();
      var lresult = repo.AddEditVariable(model);
      return lresult;
    }
    [Route("api/GetVariableByName")]
    [AcceptVerbs("GET", "POST")]
    public SYSTEM_LOOKUP GetVariableByName(string vname)
    {
      CommonHelper.SetConnectionString(Request);
      VariableRepository vrepo = new VariableRepository();
      var lresult = vrepo.GetVariableNameByName(vname);
      return lresult;
    }
    [Route("api/CheckDuplicateVariable")]
   
    [Route("api/DeleteVariable")]
    [AcceptVerbs("GET", "POST")]
    public bool DeleteVariable(int lookupid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new VariableRepository();
      var lresult = repo.DeleteVariable(lookupid);
      //Session["VariableDeleteMsg"] = "Successfully Variable is Deleted.";
      return lresult;
    }

    [Route("api/VariableDataLoad")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel VariableDataLoad([FromBody]SearchModel searchModel)
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
        string FieldNameSearch = string.Empty;
        string TableSearch = string.Empty;
        string Displaynamesearch = string.Empty;
        string statussearch = string.Empty;
        string orderDir = string.Empty;

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

          FieldNameSearch = searchModel.columns[0].search.value;
          TableSearch = searchModel.columns[1].search.value;
          Displaynamesearch = searchModel.columns[2].search.value;
          statussearch = searchModel.columns[3].search.value;
        }
        startRec = startRec + 1;
        var data = new List<VariableModel>();
        var repAcc = new VariableRepository();
        data = repAcc.GetVariables(AppConnDetails.Schema, AppConnDetails.ConnString, startRec, pageSize, FieldNameSearch, TableSearch, Displaynamesearch, statussearch, colOrder, orderDir);

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

    [Route("api/LoadTableName")]
    [AcceptVerbs("GET", "POST")]
    public List<string> LoadTableName()

    {
      CommonHelper.SetConnectionString(Request);
      var result = new List<string>();
      result.Add("GLOBAL_VAR");
      result.Add("MODAL_VAR");
      result.Add("LOOP_VAR");
      result = result.ToList();
      return result;
    }

    [Route("api/GetTypeStatusById")]
    [AcceptVerbs("GET", "POST")]
    public HttpResponseMessage GetTypeStatusById(int lookupid)
    {
      CommonHelper.SetConnectionString(Request);
      var repAcc = new VariableRepository();
      var lresult = repAcc.GetVariableById(lookupid);
      string[] result = lresult.Split(',');
      var Fresult = new
      {
        Type = result[0],
        Status = result[1]
      };
      return this.Request.CreateResponse(
        HttpStatusCode.OK,
        new { Fresult });
    }

    [Route("api/GetBaselineCompare")]
    [AcceptVerbs("GET", "POST")]
    public List<string> GetBaselineCompare(int id)
    {
      CommonHelper.SetConnectionString(Request);
      var result = new List<string>();
      result.Add("BASELINE");
      result.Add("COMPARE");
      result.Add("");
      result = result.ToList();
      return result;
    }

    [HttpPost]
    [Route("api/CheckDuplicateVariableExist")]
    [AcceptVerbs("GET", "POST")]
    public bool CheckDuplicateVariableExist(string Varname, long Varid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new VariableRepository();
      var result=repo.CheckDuplicateVariableName(Varid, Varname);
      return result;
    }
  }
}

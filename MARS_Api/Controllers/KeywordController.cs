using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using MARS_Api.Helper;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using Newtonsoft.Json;
using System.Web.Http;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Repository.ViewModel;

namespace MARS_Api.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  [Authorize]
  public class KeywordController : ApiController
  {
    // GET: Keyword
    [System.Web.Http.Route("api/GetKeywords")]
    [AcceptVerbs("GET", "POST")]
    public string GetKeywords()
    {
      CommonHelper.SetConnectionString(Request);
      KeywordRepository k = new KeywordRepository();

      var result = k.GetKeywords();
      var res = result.Select(x => x.KEY_WORD_NAME).ToList();
      var json = JsonConvert.SerializeObject(res);
      return json;
    }

    [Route("api/CheckDuplicateKeywordNameExist")]
    [AcceptVerbs("GET", "POST")]
    public bool CheckDuplicateKeywordNameExist(string keywordname, long? KeywordId)
    {
      CommonHelper.SetConnectionString(Request);
      keywordname = keywordname.Trim();
      var repo = new KeywordRepository();
      var result = repo.CheckDuplicateKeywordNameExist(keywordname, KeywordId);
      return result;
    }

    [HttpPost]
    [Route("api/AddEditKeyword")]
    [AcceptVerbs("GET", "POST")]
    public bool AddEditKeyword(KeywordViewModel lModel)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new KeywordRepository();
      var lResult = repo.AddEditKeyword(lModel);
      var repTree = new GetTreeRepository();
      return lResult;
    }
    [Route("api/DeletKeyword")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel DeletKeyword(long Keywordid)
    {
      CommonHelper.SetConnectionString(Request);
      BaseModel baseModel = new BaseModel();
      var repo = new KeywordRepository();
      var repTree = new GetTreeRepository();
      var lflag = repo.CheckTestCaseExistsInKeyword(Keywordid);

      if (lflag.Count <= 0)
      {
        var result = repo.DeleteKeyword(Keywordid);
        baseModel.status = 1;
        baseModel.message = "Success";
      }
      else
      {
        baseModel.status = 0;
        baseModel.data = lflag;
        baseModel.message = "error";
      }
      return baseModel;
    }

    [Route("api/KeywordDataLoad")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel KeywordDataLoad([FromBody]SearchModel searchModel)
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
        var repp = new KeywordRepository();

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


        var data = new List<KeywordViewModel>();
        data = repp.ListAllKeyword(AppConnDetails.Schema, AppConnDetails.ConnString, startRec, pageSize, colOrder, orderDir, NameSearch, ControlTypeSearch, EntryFileSearch);

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

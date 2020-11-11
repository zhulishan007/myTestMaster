using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using MARS_Repository.Repositories;
using Newtonsoft.Json;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Api.Helper;
using MARS_Repository.ViewModel;
using MARS_Repository.Entities;
using System.Web.Script.Serialization;

namespace MARS_Api.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  [Authorize]
  public class ObjectController : ApiController
  {
    // Get Object list based on TestCase and Application

    [Route("api/GetObjects")]
    [AcceptVerbs("GET", "POST")]
    public string GetObjects(int testcaseId)
    {
      CommonHelper.SetConnectionString(Request);
      ObjectRepository k = new ObjectRepository();
      var result = k.GetObjects(testcaseId);
      var res = result.Select(x => x.OBJECT_HAPPY_NAME).ToList().Distinct();
      var json = JsonConvert.SerializeObject(res);
      return json;
    }

    [Route("api/ObjectDataLoad")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel ObjectDataLoad([FromBody]SearchObjectModel searchModel)
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
        string quickaccess = string.Empty;
        string typesearch = string.Empty;
        string parentsearch = string.Empty;
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
        startRec = startRec + 1;
        if (searchModel.columns.Any())
        {
          colOrder = searchModel.columns[colOrderIndex].name;

          NameSearch = searchModel.columns[0].search.value;
          quickaccess = searchModel.columns[1].search.value;
          typesearch = searchModel.columns[2].search.value;
          parentsearch = searchModel.columns[3].search.value;
        }

        var repo = new ObjectRepository();
        var data = repo.ListObjects(AppConnDetails.Schema, AppConnDetails.ConnString, startRec, pageSize, NameSearch, typesearch, quickaccess, parentsearch, colOrder, orderDir, searchModel.appId);

        int totalRecords = 0;
        if (data.Count() > 0)
        {
          totalRecords = data.FirstOrDefault().Totalcount;
        }

        int recFilter = 0;
        if (data.Count() > 0)
        {
          recFilter = data.FirstOrDefault().Totalcount;
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

    [Route("api/ApplicationListInObject")]
    [AcceptVerbs("GET", "POST")]
    public List<T_REGISTERED_APPS> ApplicationListInObject()
    {
      CommonHelper.SetConnectionString(Request);
      var objrepo = new ApplicationRepository();
      var list = objrepo.ListApplication();
      return list;
    }

    [Route("api/LoadPegwindow")]
    [AcceptVerbs("GET", "POST")]
    public List<string> LoadPegwindow(long ApplicationId)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new ObjectRepository();
      var result = repo.GetPegwindowObject(ApplicationId);
      return result;
    }

    [Route("api/AddEditObject")]
    [AcceptVerbs("GET", "POST")]
    public string AddEditObject(ObjectModel objmodel)

    {
      CommonHelper.SetConnectionString(Request);
      var repo = new ObjectRepository();
      if (objmodel.ObjectId == 0)
      {
        var validationcheck = repo.CheckObjectName(objmodel.ObjectName, objmodel.applicationid, objmodel.ObjectParent, objmodel.ObjectType);
        if (validationcheck == true)
          return "Error";
      }

      var result = repo.AddEditObject(objmodel);
      return result;
    }

    [Route("api/DeleteObject")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel DeleteObject(long objectid, long appid)
    {
      CommonHelper.SetConnectionString(Request);
      BaseModel model = new BaseModel();
      var repo = new ObjectRepository();
      var objectids = repo.FindObjectId(objectid, appid);
      var pegwindowcheck = repo.CheckPegwindowObject(objectid, appid);
      if (pegwindowcheck == "success")
      {
        if (objectids.Count > 0)
        {
          foreach (var itm in objectids)
          {

            var testcasename = repo.CheckObjectExistsInTestCase(itm);
            if (testcasename.Count > 0)
            {
              //return testcasename;
              model.status = 0;
              model.message = "Error";
              model.data = testcasename;
              return model;
            }

          }

        }
        var result = repo.DeleteObject(objectid, appid);
        model.status = 1;
        model.message = "Success";
        return model;
        //return result;
      }

      model.status = 0;
      model.message = "Error";
      model.data = pegwindowcheck;
      return model;
    }

    [Route("api/CheckConvertingObjectExists")]
    [AcceptVerbs("GET", "POST")]
    public string CheckConvertingObjectExists(string objectname, long appid, string parentobj, string objecttype)
    {
      CommonHelper.SetConnectionString(Request); 
      var repo = new ObjectRepository();
      var exists = repo.CheckConvertingObjectExists(objectname, appid, parentobj, objecttype);
      return exists;
    }

    [Route("api/CopyAllObjects")]
    [AcceptVerbs("GET", "POST")]
    public string CopyAllObjects(long copyfromappid, long copytoappid)
    {
      CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var repo = new ObjectRepository();
      var result = repo.CopyAllObjects(copyfromappid, copytoappid,AppConnDetails.Schema,AppConnDetails.ConnString);
      return result;
    }

    [Route("api/CheckDuplicateObjectList")]
    [AcceptVerbs("GET", "POST")]
    public List<string> CheckDuplicateObjectList(long copyfromappid, long copytoappid)
    {
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            CommonHelper.SetConnectionString(Request); 
      var repo = new ObjectRepository();
      var result = repo.DuplicateObjectList(copyfromappid, copytoappid,AppConnDetails.ConnString,AppConnDetails.Schema);
      return result;
    }

    [Route("api/CopyObjects")]
    [AcceptVerbs("GET", "POST")]
    public string CopyObjects(List<long> model, long fromid, long toid)
    {
          
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            CommonHelper.SetConnectionString(Request);
      var repo = new ObjectRepository();
      JavaScriptSerializer js = new JavaScriptSerializer();
      
      var result = repo.CopyObjects(model, fromid, toid,AppConnDetails.ConnString,AppConnDetails.Schema);
      return result;
    }

    [Route("api/GetObjectIdByApp")]
    [AcceptVerbs("GET", "POST")]
    public List<decimal?> GetObjectIdByApp(long appid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new ObjectRepository();
      var result = repo.GetObjectId(appid);
      return result;
    }
  }
}

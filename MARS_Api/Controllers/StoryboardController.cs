using MARS_Api.Helper;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;

using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;

namespace MARS_Api.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*")]
  [Authorize]
  public class StoryboardController : ApiController
  {
    [HttpPost]
    [Route("Storyboard/AddEditStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public string AddEditStoryboard(StoryboardModel model)
    {
      CommonHelper.SetConnectionString(Request);
      StoryBoardRepository repo = new StoryBoardRepository();
      var result = repo.AddEditStoryboard(model);
      return result;
    }

    [Route("Storyboard/CheckDuplicateStoryboardName")]
    [AcceptVerbs("GET", "POST")]
    public bool CheckDuplicateStoryboardName(string storyboardname, long storyboardid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new StoryBoardRepository();
      var result = repo.CheckDuplicateStoryboardName(storyboardname, storyboardid);
      return result;
    }

    [HttpPost]
    [Route("Storyboard/ChangeStoryboardName")]
    [AcceptVerbs("GET", "POST")]
    public bool ChangeStoryboardName(string storyboardname, string storyboarddesc, long storyboardid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new StoryBoardRepository();
      var result = repo.ChangeStoryboardName(storyboardname, storyboarddesc, storyboardid);
      return result;
    }

    [Route("Storyboard/DeleteStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public bool DeleteStoryboard(long sid)
    {
      CommonHelper.SetConnectionString(Request);
      var repo = new StoryBoardRepository();
      var result = repo.DeleteStoryboard(sid);
      return result;
    }

    [HttpPost]
    [Route("Storyboard/GetStoryBoardDetails")]
    [AcceptVerbs("GET", "POST")]
    public string GetStoryBoardDetails(long Pid, long sid)
    {
      CommonHelper.SetConnectionString(Request);
      var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
      StoryBoardRepository repo = new StoryBoardRepository();
      //string luser = WebConfigurationManager.AppSettings["User"];
      //string lpassword = WebConfigurationManager.AppSettings["Password"];
      //string ldataSource = WebConfigurationManager.AppSettings["DataSource"];
      //string lSchema = WebConfigurationManager.AppSettings["Schema"];
      //var lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";
      var lresult = repo.GetStoryBoardDetails(AppConnDetails.ConnString, AppConnDetails.Schema, Pid, sid);
      var jsonresult = JsonConvert.SerializeObject(lresult);
      return jsonresult;
    }

    [Route("Storyboard/GetActionList")]
    [AcceptVerbs("GET", "POST")]
    public List<SYSTEM_LOOKUP> GetActionList(long storyboardid)
    {
      CommonHelper.SetConnectionString(Request);
      StoryBoardRepository repo = new StoryBoardRepository();
      var result = repo.GetActions(storyboardid);

      return result;
    }

    [Route("Storyboard/GetTestSuiteListInStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public List<TestSuiteListByProject> GetTestSuiteListInStoryboard(long ProjectId)
    {
      CommonHelper.SetConnectionString(Request);
      GetTreeRepository repo = new GetTreeRepository();
      var lresult = repo.GetTestSuiteList(ProjectId);
      return lresult;
    }

    [Route("Storyboard/GetTestCaseListinStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public List<TestCaseListByProject> GetTestCaseListinStoryboard(GetTestCaseByTestSuite lgrid)
    {
      CommonHelper.SetConnectionString(Request);
      var lresult = new List<TestCaseListByProject>();
      JavaScriptSerializer js = new JavaScriptSerializer();
      StoryBoardRepository repo = new StoryBoardRepository();
      string pgrid = lgrid.grid;
      long stepid = lgrid.stepid;
      long projectid = lgrid.projectid;
      int i = 1;
      var obj = js.Deserialize<GetTestCaseByTestSuite[]>(pgrid);
      //var lresult=
      foreach (var item in obj)
      {
        if (stepid >= i)
        {
          lresult = repo.GetTestCaseList(projectid, item.testsuitename);
        }
        i++;
      }

      return lresult;
    }

    [Route("Storyboard/GetDatasetList")]
    [AcceptVerbs("GET", "POST")]
    public List<DataSetListByTestCase> GetDatasetList(GetDatasetByTestcase lgrid)
    {
      CommonHelper.SetConnectionString(Request);
      var lresult = new List<DataSetListByTestCase>();
      JavaScriptSerializer js = new JavaScriptSerializer();
      StoryBoardRepository repo = new StoryBoardRepository();
      string pgrid = lgrid.grid;
      long stepid = lgrid.stepid;
      long projectid = lgrid.projectid;
      int i = 1;
      var obj = js.Deserialize<GetDatasetByTestcase[]>(pgrid);
      //var lresult=
      foreach (var item in obj)
      {
        if (stepid >= i)
        {
          lresult = repo.GetDataSetList(projectid, item.testsuitename, item.Testcasename);
        }
        i++;
      }

      return lresult;
    }

    [Route("Storyboard/LoadDependency")]
    [AcceptVerbs("GET", "POST")]
    public List<string> LoadDependency(Dependencylist dependencylist)
    {
      List<String> Dlist = new List<String>();
      JavaScriptSerializer js = new JavaScriptSerializer();
      string pgrid = dependencylist.grid;
      long stepid = dependencylist.stepid;
      long projectid = dependencylist.projectid;
      var obj = js.Deserialize<Dependencylist[]>(pgrid);
      int i = 1;
      foreach (var item in obj)
      {
        if (item.stepname != "" && item.stepname != null)
        {
          if (stepid > i)
          {
            //string[] lresult = item.stepname.Split(',');
            Dlist.Add(item.stepname);
            Dlist = (from w in Dlist select w).Distinct().ToList();
          }

        }
        i++;
      }
      return Dlist;
    }

    [Route("Storyboard/SaveAsStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public string SaveAsStoryboard(string storyboardname, string storyboarddesc, long oldstoryboardid, long projectid)
    {
      var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
      //var luser = WebConfigurationManager.AppSettings["User"];
      //var lpassword = WebConfigurationManager.AppSettings["Password"];
      //var ldataSource = WebConfigurationManager.AppSettings["DataSource"];
      //var lSchema = WebConfigurationManager.AppSettings["Schema"];
      //var lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";
      var repo = new StoryBoardRepository();
      var result = repo.NewSaveAsStoryboard(storyboardname, storyboarddesc, oldstoryboardid, projectid, AppConnDetails.Schema, AppConnDetails.ConnString, AppConnDetails.Login);
      return "";
    }

    [Route("Storyboard/ExecuteEngine")]
    [AcceptVerbs("GET", "POST")]
    public bool ExecuteEngine()
    {
      StartEngine();
      return false;
    }

    [Route("Storyboard/StartEngine")]
    [AcceptVerbs("GET", "POST")]
    public void StartEngine()
    {
      //  string strPath = typeof(StoryBoardController).Assembly.Location;
      //    string strEngine = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(strPath), "" + ".exe");
      string strEngine = "c:\\elasticsearch.bat";
      System.Diagnostics.Process.Start(@"E:\MARS_Web\MARSExplorer\Mars.exe");
      new Thread(new ThreadStart(
             new Action(() =>
             {
               Process pEng = new Process()
               {
                 StartInfo = new ProcessStartInfo()
                 {
                   FileName = strEngine,
                   Arguments = String.Format("{0},{1},{2}", 1, 2, 3)
                 }
               };
               pEng.Start();
               pEng.WaitForExit();
             })
         )).Start();
    }

    [Route("Storyboard/SignalRConnect")]
    [AcceptVerbs("GET", "POST")]
    public bool SignalRConnect()
    {
      var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
      // OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBString"]].ConnectionString);
      OracleConnection con = new OracleConnection(AppConnDetails.ConnString);
      OracleCommand cmd = new OracleCommand();

      cmd.CommandText = "select * from TESTIDE11.T_PROJ_TEST_RESULT ";//where storyboard_detail_id = 10680
      cmd.Connection = con;
      con.Open();
      OracleDependency dependency = new OracleDependency(cmd);
      dependency.QueryBasedNotification = true;
      cmd.Notification.IsNotifiedOnce = false;
      dependency.OnChange += new OnChangeEventHandler(AlertUser);
      cmd.ExecuteReader();

      return false;
    }

    [Route("Storyboard/AlertUser")]
    [AcceptVerbs("GET", "POST")]
    static void AlertUser(Object sender, OracleNotificationEventArgs args)
    {
      DataTable dt = args.Details;
      string msg = "";
      msg = "The following database objects were changed: ";
      foreach (string resource in args.ResourceNames)
        msg += resource;

      msg += "\n\n Details: ";

      for (int rows = 1; rows < dt.Rows.Count; rows++)
      {
        msg += "Resource name: " + dt.Rows[rows].ItemArray[0];

        string type = Enum.GetName(typeof(OracleNotificationInfo), dt.Rows[rows].ItemArray[1]);
        msg += "\n\nChange type: " + type;
        msg += " ";
      }
      Notify("Oracle", msg);
    }
    [Route("Storyboard/Notify")]
    [AcceptVerbs("GET", "POST")]
    static void Notify(string name, string msg)
    {
      // var statsContext = GlobalHost.ConnectionManager.GetHubContext<ExecutionResult>();
      // statsContext.Clients.All.addNewMessageToPage(name, msg);
    }

    [Route("Storyboard/GetTestResult")]
    [AcceptVerbs("GET", "POST")]
    public List<TestResultModel> GetTestResult(long TestCaseId, long StoryboardDetailId)
    {
      CommonHelper.SetConnectionString(Request);
      var rep = new StoryBoardRepository();
      var lModel = rep.GetTestResult(TestCaseId, StoryboardDetailId);
      return lModel;
    }

    [Route("Storyboard/ValidateStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public string ValidateStoryboard(string lGridJsonData, string lStoryboardId)
    {

      var result = "";
      try
      {
        CommonHelper.SetConnectionString(Request);
        JavaScriptSerializer js = new JavaScriptSerializer();
        var sbRep = new StoryBoardRepository();

        //check Keyword  object linking
        var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);

        var ValidationResult = sbRep.CheckSBGridValidation(lobj.ToList(), int.Parse(lStoryboardId));
        ValidationResult = ValidationResult.Where(x => x.IsValid != true).ToList();
        result = JsonConvert.SerializeObject(ValidationResult);

      }
      catch (Exception ex)
      {
        throw ex;
      }
      return result;
    }

    [Route("api/StoryboardDataLoad")]
    [AcceptVerbs("GET", "POST")]
    public BaseModel StoryboardDataLoad([FromBody]SearchModel searchModel)
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
        string Storyboardsnamesearch = string.Empty;
        string Storyboarddescsearch = string.Empty;
        string Projectnamesearch = string.Empty;
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

          Storyboardsnamesearch = searchModel.columns[0].search.value;
          Storyboarddescsearch = searchModel.columns[1].search.value;
          Projectnamesearch = searchModel.columns[2].search.value;
        }

        StoryBoardRepository repo = new StoryBoardRepository();
        var data = repo.GetStoryboards(AppConnDetails.ConnString, AppConnDetails.Schema, startRec, pageSize, Storyboardsnamesearch, Storyboarddescsearch, Projectnamesearch, colOrder, orderDir);

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

    [Route("Storyboard/SaveStoryboardGrid")]
    [AcceptVerbs("GET", "POST")]
    public string SaveStoryboardGrid(string lGridJsonData, string lStoryboardId, string lchangedGrid)
    {
      CommonHelper.SetConnectionString(Request);

      var result = "";
      try
      {
        JavaScriptSerializer js = new JavaScriptSerializer();
        var sbRep = new StoryBoardRepository();

        //check Keyword  object linking
        var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);

        var ValidationResult = sbRep.CheckSBGridValidation(lobj.ToList(), int.Parse(lStoryboardId));
        ValidationResult = ValidationResult.Where(x => x.IsValid != true).ToList();

        if (ValidationResult.Count() == 0)
        {
          Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lchangedGrid);

          if (dlist["deleteList"].Count > 0)
          {
            foreach (var d in dlist["deleteList"])
            {
              var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
              var lsbdetailId = delete.Where(x => x.Key == "detailid");
              if (!string.IsNullOrEmpty(lsbdetailId.FirstOrDefault().Value.ToString()))
              {
                sbRep.DeleteSBStep(Convert.ToInt32(lsbdetailId.FirstOrDefault().Value.ToString()));
              }
            }
          }
          sbRep.SaveAsStoryboardGrid(lobj.ToList(), int.Parse(lStoryboardId));
          result = JsonConvert.SerializeObject(ValidationResult);
        }
        else
        {
          result = JsonConvert.SerializeObject(ValidationResult);
        }



      }
      catch (Exception ex)
      {
        throw ex;
      }
      return result;
    }

    [Route("Storyboard/GetApplicationListByStoryboard")]
    [AcceptVerbs("GET", "POST")]
    public List<ApplicationModel> GetApplicationListByStoryboard(long lStoryboardId)
    {
      CommonHelper.SetConnectionString(Request);
      //List<ApplicationModel>
      var rep = new StoryBoardRepository();
      var lAppList = rep.GetApplicationListByStoryboardId(lStoryboardId);
      return lAppList;
    }
  }
}

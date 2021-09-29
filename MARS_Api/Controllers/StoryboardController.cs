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

            jsonresult = jsonresult.Replace("\\r", "\\\\r");
            jsonresult = jsonresult.Replace("\\n", "\\\\n");
            jsonresult = jsonresult.Replace("   ", "");
            jsonresult = jsonresult.Replace("\\", "\\\\");
            jsonresult = jsonresult.Trim();
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

            GetTestCaseByTestSuite testcase = obj[stepid - 1];
            lresult = repo.GetTestCaseList(projectid, testcase.testsuitename);

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
            GetDatasetByTestcase dataset = obj[stepid - 1];
            lresult = repo.GetDataSetList(projectid, dataset.testsuitename, dataset.Testcasename);

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
        public ResultModel ValidateStoryboard(string lGridJsonData, string lStoryboardId, string lProjectId)
        {
            ResultModel resultModel = new ResultModel();
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);

            JavaScriptSerializer js = new JavaScriptSerializer();
            var sbRep = new StoryBoardRepository();
            var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);
            if (lobj.Count() > 0)
            {
                string lreturnValues = sbRep.InsertFeedProcess();

                var lvalFeed = lreturnValues.Split('~')[0];
                var lvalFeedD = lreturnValues.Split('~')[1];

                var ValidationResult = sbRep.InsertStgStoryboardValidationTable(AppConnDetails.ConnString, AppConnDetails.Schema, lobj, lStoryboardId, lvalFeed, lvalFeedD, lProjectId);
                var result = JsonConvert.SerializeObject(ValidationResult);

                resultModel.status = 1;
                resultModel.data = result;
                resultModel.message = "Storyboard validated successfully.";
            }
            else
            {
                resultModel.status = 0;
                resultModel.message = "Cannot validate an empty storyboard.";
            }
            return resultModel;
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
        public ResultModel SaveStoryboardGrid(string lGridJsonData, string lStoryboardId, string lchangedGrid, string lProjectId)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            var result = "";
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var sbRep = new StoryBoardRepository();
                //check Keyword  object linking
                var lobj = js.Deserialize<StoryBoardResultModel[]>(lGridJsonData);
                if (lobj.Count() > 0)
                {
                    string lreturnValues = sbRep.InsertFeedProcess();
                    var lvalFeed = lreturnValues.Split('~')[0];
                    var lvalFeedD = lreturnValues.Split('~')[1];
                    var ValidationResult = sbRep.InsertStgStoryboardValidationTable(AppConnDetails.ConnString, AppConnDetails.Schema, lobj, lStoryboardId, lvalFeed, lvalFeedD, lProjectId);
                   
                    if (ValidationResult.Count() == 0)
                    {
                        Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lchangedGrid);

                        if (dlist["deleteList"].Count > 0)
                        {
                            var lDeletedSB = new List<long>();
                            foreach (var d in dlist["deleteList"])
                            {
                                var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                                var lsbdetailId = delete.Where(x => x.Key == "detailid");
                                if (!string.IsNullOrEmpty(Convert.ToString(lsbdetailId.FirstOrDefault().Value)))
                                {
                                    lDeletedSB.Add(Convert.ToInt32(lsbdetailId.FirstOrDefault().Value.ToString()));
                                }
                            }
                            if (lDeletedSB.Count() > 0)
                            {
                                sbRep.DeleteSBSteps(lDeletedSB);
                            }
                        }
                        sbRep.SaveStoryboardGrid(int.Parse(lStoryboardId), lvalFeed, lProjectId);
                        result = JsonConvert.SerializeObject(ValidationResult);
                    }
                    else
                    {
                        result = JsonConvert.SerializeObject(ValidationResult);
                    }
                    resultModel.status = 1;
                    resultModel.data = result;
                    resultModel.message = "Storyboard saved successfully.";
                }
                else
                {
                    resultModel.status = 0;
                    resultModel.message = "Cannot save an empty storyboard.";
                }
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
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

        //This method Get Compare ResultSet List by BhistedId and ChistedId
        [Route("Storyboard/GetCompareResultList")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel GetCompareResultList(long BhistedId, long ChistedId)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new StoryBoardRepository();
                var lModel = rep.GetCompareResultList(AppConnDetails.Schema, AppConnDetails.ConnString, BhistedId, ChistedId);

                var jsonresult = JsonConvert.SerializeObject(lModel);
                resultModel.status = 1;
                resultModel.data = jsonresult;

            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
             return resultModel;
        }

        //Add/Update ResultData objects values
        [Route("Storyboard/SaveSbResultData")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel SaveSbResultData(string lgridchange, string lgrid, long BaselineReportId, long CompareReportId, long TestCaseId, long StoryboardDetailId)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lGridData = js.Deserialize<List<ValidatResultViewModel>>(lgrid);
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                var srepo = new StoryBoardRepository();
                var stgresult = false;
                HistIdViewModel histIdViewModel = new HistIdViewModel();

                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lgridchange);
                Dictionary<String, List<TestResultViewModel>> gridlist = js.Deserialize<Dictionary<String, List<TestResultViewModel>>>(lgridchange);
                //ValidatResultViewModel
                var lValidationList = new List<ValidatResultViewModel>();
                if (lGridData.Count() > 0)
                {
                    var DuplicateDatatag = lGridData.GroupBy(x => x.InputValueSetting == null ? "" : Convert.ToString(x.InputValueSetting).ToUpper())
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key)
                                .ToList();
                    var DuplicateRows = lGridData.Where(x => DuplicateDatatag.Contains(x.InputValueSetting != null ? x.InputValueSetting.ToUpper() : "")).ToList();
                    //DuplicateRows = DuplicateRows.Where(x => x.InputValueSetting != "" && x.InputValueSetting!=null).ToList();

                    lValidationList = DuplicateRows.Select(x => new ValidatResultViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Duplicate Data tag name. " }).ToList();

                    if (dlist["updateList"].Count > 0)
                    {
                        var StepIds = new List<int>();
                        foreach (var d in dlist["updateList"])
                        {
                            var updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            StepIds.Add(Convert.ToInt32(Convert.ToString(updates.FirstOrDefault(x => x.Key == "StepNo").Value)) - 1);
                        }

                        var lUpdateRowCount = dlist["updateList"].Count;
                        var lOldRowCount = dlist["oldList"].Count;

                        for (int i = 0; i < lUpdateRowCount; i++)
                        {
                            var update = (((System.Collections.Generic.Dictionary<string, object>)dlist["updateList"][i]).ToList());
                            var old = (((System.Collections.Generic.Dictionary<string, object>)dlist["oldList"][i]).ToList());

                            if (old.Any(x => x.Key == "BreturnValues"))
                            {
                                var BStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "BaselineComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(BStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Baseline comment. " });
                                }
                            }

                            if (old.Any(x => x.Key == "CreturnValues"))
                            {
                                var CStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "CompareComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(CStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Compare comment. " });
                                }
                            }
                        }
                    }

                }
                if (lValidationList.Count() > 0)
                {
                    var lValidationResult = lValidationList.OrderBy(x => x.pq_ri).GroupBy(x => new { x.pq_ri, x.IsValid }).Select(g => new { g.Key.pq_ri, g.Key.IsValid, ValidMsg = string.Join(", ", g.Select(i => i.ValidMsg)) }).ToList();
                    var result = JsonConvert.SerializeObject(lValidationResult.OrderBy(x => x.pq_ri));
                    resultModel.data = result;
                    resultModel.message = "validation";
                    resultModel.status = 0;
                    return resultModel;
                }

                if (BaselineReportId == 0)
                {
                    long hist_id;
                    BaselineReportId = srepo.CreateTestReportId(TestCaseId, StoryboardDetailId, 1, out hist_id);
                    histIdViewModel.BaseHistId = hist_id;
                }

                if (CompareReportId == 0)
                {
                    long hist_id;
                    CompareReportId = srepo.CreateTestReportId(TestCaseId, StoryboardDetailId, 0, out hist_id);
                    histIdViewModel.CompareHistId = hist_id;
                }

                if (dlist["updateList"].Count > 0)
                {
                    var list = new List<TestResultViewModel>();
                    string returnValues = repo.InsertFeedProcess();
                    var valFeed = returnValues.Split('~')[0];
                    valFeedD = returnValues.Split('~')[1];
                    DataTable dt = new DataTable();
                    dt.Columns.Add("FEEDPROCESSDETAILID");
                    dt.Columns.Add("BASELINEID");
                    dt.Columns.Add("COMPAREID");
                    dt.Columns.Add("BASELINEVALUE");
                    dt.Columns.Add("COMPAREVALUE");
                    dt.Columns.Add("INPUTVALUESETTING");
                    dt.Columns.Add("BASELINECOMMENT");
                    dt.Columns.Add("COMPARECOMMENT");
                    dt.Columns.Add("BASELINEREPORTID");
                    dt.Columns.Add("COMPAREREPORTID");
                    foreach (var d in dlist["updateList"])
                    {
                        var update = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                        var baseid = update.Where(x => x.Key == "BaselineStepId");
                        var compareid = update.Where(x => x.Key == "CompareStepId");
                        var basevalue = update.Where(x => x.Key == "BreturnValues");
                        var comparevalue = update.Where(x => x.Key == "CreturnValues");
                        var inputvaluesetting = update.Where(x => x.Key == "InputValueSetting");
                        var basecomment = update.Where(x => x.Key == "BaselineComment");
                        var comparecomment = update.Where(x => x.Key == "CompareComment");
                        DataRow dr = dt.NewRow();
                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                        dr["BASELINEID"] = Convert.ToString(baseid.FirstOrDefault().Value);
                        dr["COMPAREID"] = Convert.ToString(compareid.FirstOrDefault().Value);
                        dr["BASELINEVALUE"] = Convert.ToString(basevalue.FirstOrDefault().Value);
                        dr["COMPAREVALUE"] = Convert.ToString(comparevalue.FirstOrDefault().Value);
                        dr["INPUTVALUESETTING"] = Convert.ToString(inputvaluesetting.FirstOrDefault().Value);
                        dr["BASELINECOMMENT"] = Convert.ToString(basecomment.FirstOrDefault().Value);
                        dr["COMPARECOMMENT"] = Convert.ToString(comparecomment.FirstOrDefault().Value);
                        dr["BASELINEREPORTID"] = Convert.ToString(BaselineReportId);
                        dr["COMPAREREPORTID"] = Convert.ToString(CompareReportId);
                        dt.Rows.Add(dr);
                    }
                    stgresult = srepo.InsertResultsInStgTbl(AppConnDetails.ConnString, AppConnDetails.Schema, dt, int.Parse(valFeedD));
                }

                if (gridlist["addList"].Count > 0)
                {
                    var list = new List<TestResultViewModel>();
                    if (valFeedD == "")
                    {
                        string returnValues = repo.InsertFeedProcess();
                        var valFeed = returnValues.Split('~')[0];
                        valFeedD = returnValues.Split('~')[1];
                    }
                    DataTable dt = new DataTable();
                    dt.Columns.Add("FEEDPROCESSDETAILID");
                    dt.Columns.Add("BASELINEID");
                    dt.Columns.Add("COMPAREID");
                    dt.Columns.Add("BASELINEVALUE");
                    dt.Columns.Add("COMPAREVALUE");
                    dt.Columns.Add("INPUTVALUESETTING");
                    dt.Columns.Add("BASELINECOMMENT");
                    dt.Columns.Add("COMPARECOMMENT");
                    dt.Columns.Add("BASELINEREPORTID");
                    dt.Columns.Add("COMPAREREPORTID");
                    foreach (var d in gridlist["addList"])
                    {
                        var baseid = d.BaselineStepId <= 0 || d.BaselineStepId == null ? 0 : d.BaselineStepId;
                        var compareid = d.CompareStepId <= 0 || d.CompareStepId == null ? 0 : d.CompareStepId;
                        var basevalue = d.BreturnValues == null ? "" : d.BreturnValues;
                        var comparevalue = d.CreturnValues == null ? "" : d.CreturnValues;
                        var inputvaluesetting = d.InputValueSetting == null ? "" : d.InputValueSetting;
                        var basecomment = d.BaselineComment == null ? "" : d.BaselineComment;
                        var comparecomment = d.CompareComment == null ? "" : d.CompareComment;
                        DataRow dr = dt.NewRow();
                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                        dr["BASELINEID"] = Convert.ToString(baseid);
                        dr["COMPAREID"] = Convert.ToString(compareid);
                        dr["BASELINEVALUE"] = Convert.ToString(basevalue);
                        dr["COMPAREVALUE"] = Convert.ToString(comparevalue);
                        dr["INPUTVALUESETTING"] = Convert.ToString(inputvaluesetting);
                        dr["BASELINECOMMENT"] = Convert.ToString(basecomment);
                        dr["COMPARECOMMENT"] = Convert.ToString(comparecomment);
                        dr["BASELINEREPORTID"] = Convert.ToString(BaselineReportId);
                        dr["COMPAREREPORTID"] = Convert.ToString(CompareReportId);
                        dt.Rows.Add(dr);
                    }
                    stgresult = srepo.InsertResultsInStgTbl(AppConnDetails.ConnString, AppConnDetails.Schema, dt, int.Parse(valFeedD));
                }
                if (stgresult == true)
                {
                    var fresult = srepo.updateresults(AppConnDetails.ConnString, AppConnDetails.Schema, valFeedD);
                }

                if (gridlist["deleteList"].Count > 0)
                {
                    var list = new List<long>();
                    foreach (var d in gridlist["deleteList"])
                    {
                        if (d.BaselineStepId > 0)
                            list.Add((long)d.BaselineStepId);
                        if (d.CompareStepId > 0)
                            list.Add((long)d.CompareStepId);
                    }
                    var result = srepo.DeleteResultsetstep(list);
                }

                var loutput = srepo.updatePrimaryResultStatus(TestCaseId, StoryboardDetailId, AppConnDetails.Schema, AppConnDetails.ConnString);

                resultModel.message = "success";
                resultModel.status = 1;
                resultModel.data = histIdViewModel;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Validate ResultData objects values
        [Route("Storyboard/ValidationSbResultData")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel ValidationSbResultData(string lgridchange, string lgrid)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lGridData = js.Deserialize<List<ValidatResultViewModel>>(lgrid);

                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lgridchange);
                //ValidatResultViewModel
                var lValidationList = new List<ValidatResultViewModel>();
                if (lGridData.Count() > 0)
                {
                    var DuplicateDatatag = lGridData.GroupBy(x => x.InputValueSetting == null ? "" : Convert.ToString(x.InputValueSetting).ToUpper())
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key)
                                .ToList();

                    var DuplicateRows = lGridData.Where(x => DuplicateDatatag.Contains(x.InputValueSetting != null ? x.InputValueSetting.ToUpper() : "")).ToList();

                    lValidationList = DuplicateRows.Select(x => new ValidatResultViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Duplicate Data tag name. " }).ToList();

                    if (dlist["updateList"].Count > 0)
                    {
                        var StepIds = new List<int>();
                        foreach (var d in dlist["updateList"])
                        {
                            var updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            StepIds.Add(Convert.ToInt32(Convert.ToString(updates.FirstOrDefault(x => x.Key == "StepNo").Value)) - 1);
                        }

                        var lUpdateRowCount = dlist["updateList"].Count;
                        var lOldRowCount = dlist["oldList"].Count;

                        for (int i = 0; i < lUpdateRowCount; i++)
                        {
                            var update = (((System.Collections.Generic.Dictionary<string, object>)dlist["updateList"][i]).ToList());
                            var old = (((System.Collections.Generic.Dictionary<string, object>)dlist["oldList"][i]).ToList());

                            if (old.Any(x => x.Key == "BreturnValues"))
                            {
                                var BStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "BaselineComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(BStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Baseline comment. " });
                                }
                            }

                            if (old.Any(x => x.Key == "CreturnValues"))
                            {
                                var CStepNo = Convert.ToInt32(update.FirstOrDefault(x => x.Key == "StepNo").Value);
                                var checkComment = update.FirstOrDefault(x => x.Key == "CompareComment").Value;
                                if (string.IsNullOrEmpty(Convert.ToString(checkComment)))
                                {
                                    var pq_ri = lGridData.FirstOrDefault(x => x.StepNo == Convert.ToInt32(CStepNo)).pq_ri;
                                    lValidationList.Add(new ValidatResultViewModel { IsValid = false, pq_ri = pq_ri, ValidMsg = " Please add description in Compare comment. " });
                                }
                            }
                        }
                    }
                }
                if (lValidationList.Count() > 0)
                {
                    var lValidationResult = lValidationList.OrderBy(x => x.pq_ri).GroupBy(x => new { x.pq_ri, x.IsValid }).Select(g => new { g.Key.pq_ri, g.Key.IsValid, ValidMsg = string.Join(", ", g.Select(i => i.ValidMsg)) }).ToList();
                    var result = JsonConvert.SerializeObject(lValidationResult.OrderBy(x => x.pq_ri));
                    resultModel.data = result;
                    resultModel.message = "validation";
                    resultModel.status = 0;
                }
                else
                {
                    resultModel.message = "success";
                    resultModel.status = 1;
                }
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //SaveAs ResultData by BhistedId and ChistedId
        [HttpPost]
        [Route("Storyboard/SaveAsResultSet")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel SaveAsResultSet(long BhistedId, long ChistedId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new StoryBoardRepository();
                var result = rep.SaveAsResultSet(BhistedId, ChistedId);
                resultModel.data = result;
                resultModel.message = "Result Set SaveAs successfully.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //This method will Save Result list
        [Route("Storyboard/SaveResultList")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel SaveResultList(string lchangedGrid, long lhistedId, long latestTestMarkId, long TestCaseId, long StoryboardDetailId)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var sbRep = new StoryBoardRepository();
                JavaScriptSerializer js = new JavaScriptSerializer();
                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lchangedGrid);

                if (lhistedId != 0 && latestTestMarkId != 0)
                {
                    sbRep.ChangelatestTestMarkId(lhistedId, latestTestMarkId);
                }

                if (dlist["deleteList"].Count > 0)
                {
                    var lDeletedResultsSetList = new List<long>();
                    foreach (var d in dlist["deleteList"])
                    {
                        var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                        var lHistId = delete.Where(x => x.Key == "HistId");
                        if (!string.IsNullOrEmpty(Convert.ToString(lHistId.FirstOrDefault().Value)))
                        {
                            lDeletedResultsSetList.Add(Convert.ToInt32(Convert.ToString(lHistId.FirstOrDefault().Value)));
                        }
                    }
                    if (lDeletedResultsSetList.Count() > 0)
                    {
                        sbRep.DeleteResultSets(lDeletedResultsSetList);
                    }
                }
                if (dlist["updateList"].Count > 0)
                {
                    var lList = new List<TestResultSaveModel>();
                    // var abc = js.Deserialize<TestResultSaveModel[]>(dlist["updateList"].ToString());
                    foreach (var d in dlist["updateList"])
                    {
                        var update = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                        var lHistId = update.Where(x => x.Key == "HistId");
                        var lAlias = update.Where(x => x.Key == "Alias");
                        var lDesc = update.Where(x => x.Key == "Description");
                        var lModel = new TestResultSaveModel();
                        if (!string.IsNullOrEmpty(Convert.ToString(lHistId.FirstOrDefault().Value)))
                        {
                            lModel.HistId = Convert.ToInt32(Convert.ToString(lHistId.FirstOrDefault().Value));
                            lModel.Alias = Convert.ToString(lAlias.FirstOrDefault().Value);
                            lModel.Description = Convert.ToString(lDesc.FirstOrDefault().Value);
                            lList.Add(lModel);
                        }
                    }
                    if (lList.Count() > 0)
                    {
                        sbRep.UpdateResultSets(lList);
                    }
                }
               var loutput = sbRep.updatePrimaryResultStatus(TestCaseId, StoryboardDetailId, AppConnDetails.Schema, AppConnDetails.ConnString);

                resultModel.data = "success";
                resultModel.message = " Data List Saved successfully.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        [Route("Storyboard/GetStoryboardResultData")]
        [AcceptVerbs("GET", "POST")]
        public string GetStoryboardResultData(long lCompareHistId, long lBaselineHistId)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            StoryBoardRepository repo = new StoryBoardRepository();
            var lresult = repo.GetStoryBoardResultData(AppConnDetails.Schema, AppConnDetails.ConnString, lCompareHistId, lBaselineHistId);
            var jsonresult = JsonConvert.SerializeObject(lresult);
            return jsonresult;
        }
    }
}

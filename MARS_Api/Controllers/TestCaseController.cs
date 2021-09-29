using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MARS_Repository.Repositories;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Http.Cors;
using System.Web.Configuration;
using System.IO;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using System.Web.Http;
using AcceptVerbsAttribute = System.Web.Http.AcceptVerbsAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using AuthorizeAttribute = MARS_Api.Provider.AuthorizeAttribute;
using MARS_Repository.Entities;
using MARS_Api.Helper;

namespace MARS_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class TestCaseController : ApiController
    {
        //List all applications
        [Route("api/ApplicationList")]
        [AcceptVerbs("GET", "POST")]
        public List<T_REGISTERED_APPS> ApplicationList()
        {
            CommonHelper.SetConnectionString(Request);
            var repApp = new ApplicationRepository();
            var lapp = repApp.ListApplication();
            // var applist = lapp.Select(c => new  { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).ToList();
            ///ViewBag.listApplications = applist;

            return lapp;
        }

        //Get Testcase details
        [Route("api/GetTestCaseDetails")]
        [AcceptVerbs("GET", "POST")]
        public List<TestCaseResult> GetTestCaseDetails(int testcaseId, int testsuiteid, long UserId, string dataset)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            TestCaseRepository tc = new TestCaseRepository();
            var datasetId = tc.GetDatasetId(dataset);
            var result = tc.GetTestCaseDetail(testcaseId, AppConnDetails.Schema, AppConnDetails.ConnString, UserId, datasetId).ToList();
            return result;
        }

        //Change TestCase name
        [Route("api/ChangeTestCaseName")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel ChangeTestCaseName(string TestCaseName, long TestCaseId, string TestCaseDes)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var testCaserepo = new TestCaseRepository();
                testCaserepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = testCaserepo.CheckDuplicateTestCaseName(TestCaseName, TestCaseId);
                if (result)
                {
                    resultModel.data = result;
                    resultModel.message = "Test Case name already exist";
                }
                else
                {
                    var renamecase = testCaserepo.ChangeTestCaseName(TestCaseName, TestCaseId, TestCaseDes);
                    resultModel.data = renamecase;
                    resultModel.message = "Test Case name successfully changed";
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Check whether duplicate TestCase name exists or not
        [Route("api/CheckDuplicateTestCaseNameExist")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckDuplicateTestCaseNameExist(string TestCaseName, long? TestCaseId)
        {
            CommonHelper.SetConnectionString(Request);
            var testCaserepo = new TestCaseRepository();
            var value = testCaserepo.CheckDuplicateTestCaseName(TestCaseName, TestCaseId);
            return value;
        }

        [AcceptVerbs("GET", "POST")]
        [Route("api/ValidateTestCase")]
        //Validate the TestCase grid
        public string ValidateTestCase(string lJson, string testCaseId = "986", string pKeywordObject = "", string testSuiteId = "224", string steps = "", string NewColumnsList = "", string ExistDataSetRenameList = "", string DeleteColumnsList = "", string SkipColumns = "")
        {
            var result = "";
            try
            {
                CommonHelper.SetConnectionString(Request);
                var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
                JavaScriptSerializer js = new JavaScriptSerializer();
                TestCaseRepository tc = new TestCaseRepository();

                decimal testcaseId = int.Parse(testCaseId);
                decimal testsuiteid = int.Parse(testSuiteId);
                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(pKeywordObject);
                var ValidationSteps = tc.InsertStgTestcaseValidationTable(AppConnDetails.ConnString, AppConnDetails.Schema, lobj, testCaseId);
                result = JsonConvert.SerializeObject(ValidationSteps);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }
        //public string SaveTestCase(string Json, string testCaseId = "986", string testSuiteId = "224", string steps = "", 
        //    string NewColumnsList = "", string ExistDataSetRenameList = "", string DeleteColumnsList = "", string SkipColumns = "") {
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    var skipData = js.Deserialize<Dictionary<String, List<Object>>>(SkipColumns);
        //    ///<summary>
        //    /// Update Steps Order based on the JSON for dragged and dropped rows
        //    ///</summary>
        //    #region <<Update Steps Region>>
        //    Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(Json);
        //    if (steps != "")
        //    {
        //        List<Object> stps = js.Deserialize<List<Object>>(steps);
        //        if (stps != null) {
        //            foreach (var s in stps) {
        //                var stp = (((System.Collections.Generic.Dictionary<string, object>)s).ToList());
        //                if (stp[1].Value.ToString() != stp[2].Value.ToString()) {
        //                    var stepid = int.Parse(stp[0].Value.ToString());
        //                    var newRun_Order = int.Parse(stp[1].Value.ToString());
        //                    tc.updateStepID(int.Parse(stp[0].Value.ToString()), int.Parse(stp[1].Value.ToString()));
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //    #region <<Update Data Set Name and Description>>
        //    var datasets = js.Deserialize<List<Object>>(ExistDataSetRenameList);
        //    if (datasets != null) {
        //        foreach (var d in datasets) {
        //            var ds = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
        //            if (ds[0].Value.ToString() != ds[1].Value.ToString()) {
        //                tc.updateDataSetName(ds[1].Value.ToString(), long.Parse(ds[3].Value.ToString()));
        //            }
        //            tc.updateDataSetDescription(ds[2].Value.ToString(), long.Parse(ds[3].Value.ToString()));
        //        }
        //    }
        //    #endregion
        //    ///<summary>
        //    /// Remove Data Set
        //    ///</summary>
        //    if (DeleteColumnsList != "")
        //    {
        //        List<Object> DataSet = js.Deserialize<List<Object>>(DeleteColumnsList);
        //        foreach (var d in DataSet) {
        //            var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
        //            tc.DeleteRelTestCaseDataSummary(long.Parse(testCaseId), long.Parse(stp[1].Value.ToString()));
        //        }
        //    }
        //    // Add step code
        //    if (dlist["addList"].Count > 0) {
        //        foreach (var d in dlist["addList"])
        //        {
        //            var add = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());

        //        }

        //        tc.updateSteps(int.Parse(testCaseId));
        //    }

        //    ///<summary>
        //    /// Remove Step
        //    ///</summary>
        //    if (dlist["deleteList"].Count > 0)
        //    {
        //        foreach (var d in dlist["deleteList"])
        //        {
        //            var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
        //            tc.deleteStep(int.Parse(delete[0].Value.ToString()));
        //        }

        //        tc.updateSteps(int.Parse(testCaseId));
        //    }

        //    ///<summary>
        //    /// Update Test case
        //    ///</summary>
        //    if (dlist["updateList"].Count > 0)
        //    {
        //        var update = dlist["updateList"][0];
        //        var updates = (((System.Collections.Generic.Dictionary<string, object>)update).ToList());

        //        string returnValues = tc.InsertFeedProcess();
        //        var valFeed = returnValues.Split('~')[0];
        //        var valFeedD = returnValues.Split('~')[1];

        //        // get test case id from the json and get all data set records
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("TESTSUITENAME");
        //        dt.Columns.Add("TESTCASENAME");
        //        dt.Columns.Add("TESTCASEDESCRIPTION");
        //        dt.Columns.Add("DATASETMODE");
        //        dt.Columns.Add("KEYWORD");
        //        dt.Columns.Add("OBJECT");
        //        dt.Columns.Add("PARAMETER");
        //        dt.Columns.Add("COMMENTS");
        //        dt.Columns.Add("DATASETNAME");
        //        dt.Columns.Add("DATASETVALUE");
        //        dt.Columns.Add("ROWNUMBER");
        //        dt.Columns.Add("FEEDPROCESSDETAILID");
        //        dt.Columns.Add("TABNAME");
        //        dt.Columns.Add("APPLICATION");
        //        dt.Columns.Add("SKIP");
        //        dt.Columns.Add("DATASETDESCRIPTION");
        //        dt.Columns.Add("STEPSID");

        //        if (dlist.Count > 0)
        //        {
        //            if (dlist["updateList"].Count > 0)
        //            {
        //                var a = dlist["updateList"][0];
        //                string stepId = (((System.Collections.Generic.Dictionary<string, object>)a).FirstOrDefault()).Value.ToString();
        //                if (stepId != "")
        //                {
        //                    //var query1 = tc.GetTestCaseDetailsFromStepId(int.Parse(stepId));

        //                    if (testCaseId != "" && testSuiteId != "")
        //                    {
        //                        decimal testcaseId = int.Parse(testCaseId);
        //                        decimal testsuiteid = int.Parse(testSuiteId);
        //                        var query = tc.GetTestCaseDetails(testcaseId, testsuiteid);
        //                        int rowCounter = -1;
        //                        if (query != null && query.Count > 0)
        //                        {
        //                            foreach (var q in query)
        //                            {
        //                                rowCounter++;
        //                                string DATASETNAME = q.DATASETNAME;
        //                                string DATASETVALUE = q.DATASETVALUE;
        //                                string SKIP = q.SKIP;

        //                                string[] datasetnames1 = DATASETNAME.Split(',');
        //                                if (q.DATASETVALUE == null)
        //                                {
        //                                    for (int i = 0; i < datasetnames1.Length; i++) {
        //                                        datasetnames1[i] = datasetnames1[i].Replace("_", " ");
        //                                    }
        //                                    DATASETVALUE = DATASETVALUE == null ? "" : DATASETVALUE;
        //                                    foreach (var xy in datasetnames1)
        //                                    {
        //                                        if (DATASETVALUE != null)
        //                                            DATASETVALUE += ",";

        //                                    }
        //                                }
        //                                //if (DATASETVALUE.Split(',').Length < datasetnames1.Length)
        //                                //{
        //                                //    for (int o = DATASETVALUE.Split(',').Length; o < datasetnames1.Length; o++)
        //                                //    {
        //                                //        DATASETVALUE += ",";
        //                                //    }
        //                                //}
        //                                if (datasetnames1.Length < ((updates.Count - 5) / 2))
        //                                {
        //                                    for (int i = 6; i < updates.Count; i++)
        //                                    {

        //                                        if ((!updates[i].Key.StartsWith("skip")) && (!datasetnames1.Contains(updates[i].Key.Replace("_", " "))))
        //                                        {
        //                                            if (DATASETNAME != "")
        //                                            {
        //                                                DATASETNAME += ",";
        //                                            }
        //                                            DATASETNAME += updates[i].Key;

        //                                            if (SKIP != "")
        //                                            {
        //                                                SKIP += ",";
        //                                            }
        //                                            var rowskip = (((System.Collections.Generic.List<object>)skipData[rowCounter.ToString()]).ToList())[0];
        //                                            if (rowskip != null)
        //                                            {
        //                                                var sk = (((System.Collections.Generic.Dictionary<string, object>)rowskip).ToList());
        //                                                if (sk != null)
        //                                                {
        //                                                    SKIP += sk[((i - 6) / 2)].Value.ToString();
        //                                                }
        //                                                else
        //                                                {
        //                                                    SKIP += "0"; // put skip data set value
        //                                                }
        //                                            }
        //                                            else {
        //                                                SKIP += "0"; // put skip data set value
        //                                            }

        //                                            if (DATASETVALUE != "")
        //                                            {
        //                                                DATASETVALUE += ",";
        //                                            }
        //                                            if (updates.Count >= i && updates[i].Value != null)
        //                                            {
        //                                                DATASETVALUE += updates[i].Value.ToString();
        //                                            }
        //                                            else {
        //                                                DATASETVALUE += "";
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                string[] datasetnames = DATASETNAME.Split(',');

        //                                string[] skips = SKIP != null ? SKIP.Split(',') : null;

        //                                string[] datasetvalues = DATASETVALUE != null ? DATASETVALUE.Split(',') : null;
        //                                for (int i = 0; i < datasetnames.Length; i++)
        //                                {
        //                                    DataRow dr = dt.NewRow();
        //                                    dr["STEPSID"] = q.STEPS_ID.ToString();
        //                                    dr["TESTSUITENAME"] = q.test_suite_name.ToString();
        //                                    dr["TESTCASENAME"] = q.test_case_name.ToString();
        //                                    dr["TESTCASEDESCRIPTION"] = q.test_step_description != null ? q.test_step_description.ToString() : "";
        //                                    dr["DATASETMODE"] = "";
        //                                    dr["KEYWORD"] = q.key_word_name != null ? q.key_word_name.ToString() : "";
        //                                    dr["OBJECT"] = q.object_happy_name != null ? q.object_happy_name.ToString() : "";
        //                                    dr["PARAMETER"] = q.parameter != null ? q.parameter.ToString() : "";
        //                                    dr["COMMENTS"] = q.COMMENT != null ? q.COMMENT.ToString() : "";
        //                                    dr["DATASETNAME"] = datasetnames[i].ToString();
        //                                    if (skips != null && skips.Length > 0 && skips.Length >= i)
        //                                    {
        //                                        dr["SKIP"] = skips[i].ToString();
        //                                    }
        //                                    else
        //                                    {
        //                                        dr["SKIP"] = "";
        //                                    }
        //                                    if (datasetvalues != null && datasetvalues.Length > 0 && datasetvalues.Length >= i)
        //                                    {
        //                                        dr["DATASETVALUE"] = datasetvalues[i].ToString();
        //                                    }
        //                                    else
        //                                    {
        //                                        dr["DATASETVALUE"] = "";
        //                                    }
        //                                    dr["ROWNUMBER"] = q.RUN_ORDER.ToString();
        //                                    dr["FEEDPROCESSDETAILID"] = 0;
        //                                    dr["TABNAME"] = "WebApp";
        //                                    dr["APPLICATION"] = q.Application.ToString();
        //                                    dr["DATASETDESCRIPTION"] = q.test_step_description != null ? q.test_step_description.ToString() : "";
        //                                    dr["FEEDPROCESSDETAILID"] = valFeedD;
        //                                    dt.Rows.Add(dr);
        //                                }

        //                            }

        //                            // Merge updated list from web application to the datatable
        //                            if (dt.Rows.Count > 0)
        //                            {
        //                                foreach (var d in dlist["updateList"])
        //                                {

        //                                    updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());

        //                                    string stepsID = "";
        //                                    string keyword = "";
        //                                    string obj = "";
        //                                    string parameters = "";
        //                                    string comment = "";
        //                                    string skip = "";
        //                                    string datasetname = "";
        //                                    string datasetvalue = "";

        //                                    stepsID = updates[0].Value.ToString();
        //                                    keyword = updates[1].Value.ToString();
        //                                    obj = updates[2].Value.ToString();
        //                                    parameters = updates[3].Value.ToString();
        //                                    comment = updates[4].Value.ToString();
        //                                    //datasetvalue = updates[7].Value.ToString();
        //                                    for (int i = 0; i < dt.Rows.Count; i++)
        //                                    {
        //                                        if (dt.Rows[i]["STEPSID"].ToString() == stepsID)
        //                                        {
        //                                            for (int k = 6; k < updates.Count; k++)
        //                                            {
        //                                                datasetname = updates[k].Key.ToString();
        //                                                if (datasetname.Replace("_", " ") == dt.Rows[i]["DATASETNAME"].ToString().Replace("_", " "))
        //                                                {
        //                                                    if (updates[k - 1].Key.ToString().ToLower().StartsWith("skip")) {
        //                                                        dt.Rows[i]["SKIP"] = updates[k - 1].Value.ToString() == "True" ? "4" : updates[k - 1].Value.ToString() == "4" ? "4" : "0";
        //                                                    }
        //                                                    if (updates.Count >= k && updates[k].Value != null)
        //                                                    {
        //                                                        dt.Rows[i]["DATASETVALUE"] = updates[k].Value.ToString();
        //                                                    }
        //                                                    else {
        //                                                        dt.Rows[i]["DATASETVALUE"] = "";
        //                                                    }

        //                                                    dt.Rows[i]["COMMENTS"] = comment;
        //                                                    dt.Rows[i]["PARAMETER"] = parameters;

        //                                                    dt.Rows[i]["OBJECT"] = obj;
        //                                                    dt.Rows[i]["KEYWORD"] = keyword;
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                //-------------------------------------------------------------
        //                                // Save the values to the database in staging table
        //                                //-------------------------------------------------------------
        //                                if (dt.Rows.Count > 0)
        //                                {

        //                                    OracleTransaction ltransaction;
        //                                    OracleConnection lconnection = new OracleConnection("Data Source=WIN-I13M2P5AI1B;;User Id=TESTIDE11;Password=TESTIDE11;");
        //                                    lconnection.Open();
        //                                    ltransaction = lconnection.BeginTransaction();

        //                                    OracleCommand lcmd;
        //                                    lcmd = lconnection.CreateCommand();
        //                                    lcmd.Transaction = ltransaction;

        //                                    //The name of the Procedure responsible for inserting the data in the table.
        //                                    List<DataRow> list = dt.AsEnumerable().ToList();
        //                                    lcmd.CommandText = "SP_IMPORT_FILE_TESTCASE";
        //                                    //lcmd.CommandText = "insert into TBLSTGTESTCASE(TESTSUITENAME, TESTCASENAME, TESTCASEDESCRIPTION, DATASETMODE, KEYWORD, OBJECT, PARAMETER, COMMENTS, DATASETNAME, DATASETVALUE, ROWNUMBER, FEEDPROCESSDETAILID, TABNAME, APPLICATION, ID, CREATEDON)"
        //                                    //+ "SELECT :1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14, (SELECT MAX(ID)), (SELECT SYSDATE FROM DUAL) from dual";

        //                                    string[] TESTSUITENAME_param = dt.AsEnumerable().Select(r => r.Field<string>("TESTSUITENAME")).ToArray();
        //                                    string[] TESTCASENAME_param = dt.AsEnumerable().Select(r => r.Field<string>("TESTCASENAME")).ToArray();
        //                                    string[] TESTCASEDESCRIPTION_param = dt.AsEnumerable().Select(r => r.Field<string>("TESTCASEDESCRIPTION")).ToArray();
        //                                    string[] DATASETMODE_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETMODE")).ToArray();
        //                                    string[] KEYWORD_param = dt.AsEnumerable().Select(r => r.Field<string>("KEYWORD")).ToArray();
        //                                    string[] OBJECT_param = dt.AsEnumerable().Select(r => r.Field<string>("OBJECT")).ToArray();
        //                                    string[] PARAMETER_param = dt.AsEnumerable().Select(r => r.Field<string>("PARAMETER")).ToArray();
        //                                    string[] COMMENTS_param = dt.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
        //                                    string[] DATASETNAME_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
        //                                    string[] DATASETVALUE_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETVALUE")).ToArray();
        //                                    string[] ROWNUMBER_param = dt.AsEnumerable().Select(r => r.Field<string>("ROWNUMBER")).ToArray(); ;
        //                                    string[] TABNAME_param = dt.AsEnumerable().Select(r => r.Field<string>("TABNAME")).ToArray();
        //                                    string[] FEEDPROCESSDETAILID_param = dt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
        //                                    string[] APPLICATION_param = dt.AsEnumerable().Select(r => r.Field<string>("APPLICATION")).ToArray();
        //                                    string[] SKIP_param = dt.AsEnumerable().Select(r => r.Field<string>("SKIP")).ToArray();
        //                                    string[] DATASETDESCRIPTION_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETDESCRIPTION")).ToArray();

        //                                    if (DATASETNAME_param.Length != 0)
        //                                    {
        //                                        OracleParameter TESTSUITENAME_oparam = new OracleParameter();
        //                                        TESTSUITENAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        TESTSUITENAME_oparam.Value = TESTSUITENAME_param;

        //                                        OracleParameter TESTCASENAME_oparam = new OracleParameter();
        //                                        TESTCASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        TESTCASENAME_oparam.Value = TESTCASENAME_param;

        //                                        OracleParameter TESTCASEDESCRIPTION_oparam = new OracleParameter();
        //                                        TESTCASEDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        TESTCASEDESCRIPTION_oparam.Value = TESTCASEDESCRIPTION_param;

        //                                        OracleParameter DATASETMODE_oparam = new OracleParameter();
        //                                        DATASETMODE_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        DATASETMODE_oparam.Value = DATASETMODE_param;

        //                                        OracleParameter KEYWORD_oparam = new OracleParameter();
        //                                        KEYWORD_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        KEYWORD_oparam.Value = KEYWORD_param;

        //                                        OracleParameter OBJECT_oparam = new OracleParameter();
        //                                        OBJECT_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        OBJECT_oparam.Value = OBJECT_param;

        //                                        OracleParameter PARAMETER_oparam = new OracleParameter();
        //                                        PARAMETER_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        PARAMETER_oparam.Value = PARAMETER_param;

        //                                        OracleParameter COMMENTS_oparam = new OracleParameter();
        //                                        COMMENTS_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        COMMENTS_oparam.Value = COMMENTS_param;

        //                                        OracleParameter DATASETNAME_oparam = new OracleParameter();
        //                                        DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        DATASETNAME_oparam.Value = DATASETNAME_param;

        //                                        OracleParameter DATASETVALUE_oparam = new OracleParameter();
        //                                        DATASETVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        DATASETVALUE_oparam.Value = DATASETVALUE_param;

        //                                        OracleParameter ROWNUMBER_oparam = new OracleParameter();
        //                                        ROWNUMBER_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        ROWNUMBER_oparam.Value = ROWNUMBER_param;

        //                                        OracleParameter TABNAME_oparam = new OracleParameter();
        //                                        TABNAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        TABNAME_oparam.Value = TABNAME_param;

        //                                        OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
        //                                        FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

        //                                        OracleParameter APPLICATION_oparam = new OracleParameter();
        //                                        APPLICATION_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        APPLICATION_oparam.Value = APPLICATION_param;

        //                                        OracleParameter SKIP_oparam = new OracleParameter();
        //                                        SKIP_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        SKIP_oparam.Value = SKIP_param;

        //                                        OracleParameter DATASETDESCRIPTION_oparam = new OracleParameter();
        //                                        DATASETDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                        DATASETDESCRIPTION_oparam.Value = DATASETDESCRIPTION_param;

        //                                        lcmd.ArrayBindCount = TESTSUITENAME_param.Length;

        //                                        lcmd.Parameters.Add(TESTSUITENAME_oparam);
        //                                        lcmd.Parameters.Add(TESTCASENAME_oparam);
        //                                        lcmd.Parameters.Add(TESTCASEDESCRIPTION_oparam);
        //                                        lcmd.Parameters.Add(DATASETMODE_oparam);
        //                                        lcmd.Parameters.Add(KEYWORD_oparam);
        //                                        lcmd.Parameters.Add(OBJECT_oparam);
        //                                        lcmd.Parameters.Add(PARAMETER_oparam);
        //                                        lcmd.Parameters.Add(COMMENTS_oparam);
        //                                        lcmd.Parameters.Add(DATASETNAME_oparam);
        //                                        lcmd.Parameters.Add(DATASETVALUE_oparam);
        //                                        lcmd.Parameters.Add(ROWNUMBER_oparam);
        //                                        lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
        //                                        lcmd.Parameters.Add(TABNAME_oparam);
        //                                        lcmd.Parameters.Add(APPLICATION_oparam);
        //                                        lcmd.Parameters.Add(SKIP_oparam);
        //                                        lcmd.Parameters.Add(DATASETDESCRIPTION_oparam);

        //                                        lcmd.CommandType = CommandType.StoredProcedure;
        //                                        //dt.TableName = "DETAILS";
        //                                        //OracleParameter parm1 = new OracleParameter("DETAILS", OracleDbType.RefCursor);
        //                                        //parm1.Value = dt;
        //                                        //parm1.Direction = ParameterDirection.Input;
        //                                        //lcmd.Parameters.Add(parm1);
        //                                        try
        //                                        {
        //                                            lcmd.ExecuteNonQuery();
        //                                        }
        //                                        catch (Exception lex)
        //                                        {
        //                                            ltransaction.Rollback();

        //                                            throw new Exception(lex.Message);
        //                                        }
        //                                        ltransaction.Commit();

        //                                        // Call Validation SP
        //                                        tc.ValidateSave(int.Parse(valFeed));

        //                                        var ret = tc.SaveData(int.Parse(valFeed));
        //                                        if (ret == "not saved")
        //                                        {
        //                                            var r = tc.getValidations(int.Parse(valFeed));
        //                                            var json = JsonConvert.SerializeObject(r);
        //                                            return json;
        //                                        }
        //                                        else
        //                                        {
        //                                            return "success";
        //                                        }
        //                                    }
        //                                }
        //                                //-------------------------------------------------------------
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return "";
        //}

        //Save TestCase grid

        [Route("api/SaveTestCase")]
        [AcceptVerbs("GET", "POST")]
        //Save TestCase grid
        public ResultModel SaveTestCase(string lGrid, string lChanges, string lTestCaseId, string lTestSuiteId, long UserId,
                string lDeleteColumnsList = "", string lVersion = "")
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var repTC = new TestCaseRepository();
            repTC.Username = SessionManager.TESTER_LOGIN_NAME;
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                decimal testcaseId = int.Parse(lTestCaseId);
                decimal testsuiteid = int.Parse(lTestSuiteId);
                var lUserId = SessionManager.TESTER_ID;

                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(lGrid);
                if (lobj.ToList().Count() == 0)
                {
                    resultModel.message = "You can not delete all steps.";
                    resultModel.status = 1;
                    return resultModel;
                }

                OracleTransaction ltransaction;
                var ValidationSteps = repTC.InsertStgTestcaseValidationTable(AppConnDetails.ConnString, AppConnDetails.Schema, lobj, lTestCaseId);
                OracleConnection lconnection = new OracleConnection(AppConnDetails.ConnString);

                if (ValidationSteps.Count() == 0)
                {
                    Dictionary<String, List<Object>> plist = js.Deserialize<Dictionary<String, List<Object>>>(lChanges);
                    if (plist["updateList"].Count == 0 && lVersion == "1")
                    {
                        if (plist["deleteList"].Count > 0)
                        {

                            var pVersion = string.Empty;
                            foreach (var d in plist["deleteList"])
                            {
                                if (string.IsNullOrEmpty(lVersion))
                                {
                                    var versionList = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                                    var versionId = versionList.Where(x => x.Key == "hdnVERSION");
                                    if (!string.IsNullOrEmpty(versionId.FirstOrDefault().Value.ToString()))
                                    {
                                        pVersion = versionId.FirstOrDefault().Value.ToString();
                                        lVersion = pVersion;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(lVersion))
                    {
                        var lflag = repTC.MatchTestCaseVersion(int.Parse(lTestCaseId), Convert.ToInt64(lVersion));
                        if (!lflag)
                        {
                            resultModel.message = "Another User Edited this Test Case. Please reload selected TestCase and Make changes.";
                            resultModel.data = "Another User Edited this Test Case. Please reload selected TestCase and Make changes.";
                            resultModel.status = 1;
                            return resultModel;
                        }
                    }

                    if (lDeleteColumnsList != "")
                    {
                        List<Object> DataSet = js.Deserialize<List<Object>>(lDeleteColumnsList);
                        foreach (var d in DataSet)
                        {

                            var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            repTC.DeleteRelTestCaseDataSummary(long.Parse(lTestCaseId), long.Parse(stp[2].Value.ToString()));
                        }
                    }
                    var lUpdateRownumber = js.Deserialize<SaveTestcaseModel[]>(lGrid);
                    var minStepId = lUpdateRownumber.Where(x => x.stepsID != null).Min(x => Convert.ToInt32(x.stepsID));
                    if (minStepId > 0)
                    {
                        minStepId = 1;
                    }
                    var lDatasetnameList = repTC.GetDatasetNamebyTestcaseId(int.Parse(lTestCaseId));
                    var maxOrderNumber = lUpdateRownumber.Where(x => x.stepsID != null).Max(x => Convert.ToInt32(x.pq_ri));
                    DataTable dt = new DataTable();
                    dt.Columns.Add("STEPSID");
                    dt.Columns.Add("KEYWORD");
                    dt.Columns.Add("OBJECT");
                    dt.Columns.Add("PARAMETER");
                    dt.Columns.Add("COMMENTS");
                    dt.Columns.Add("ROWNUMBER");

                    dt.Columns.Add("DATASETNAME");
                    dt.Columns.Add("DATASETID");

                    dt.Columns.Add("DATASETVALUE");
                    dt.Columns.Add("Data_Setting_Id");
                    dt.Columns.Add("SKIP");

                    dt.Columns.Add("FEEDPROCESSDETAILID");
                    dt.Columns.Add("Type");
                    dt.Columns.Add("WhichTable");

                    dt.Columns.Add("TestcaseId");
                    dt.Columns.Add("TestsuiteId");
                    dt.Columns.Add("ParentObj");

                    string returnValues = repTC.InsertFeedProcess();
                    var valFeed = returnValues.Split('~')[0];
                    var valFeedD = returnValues.Split('~')[1];
                    //delete rows
                    var lDeleteSteps = plist["deleteList"];
                    if (lDeleteSteps.Count > 0)
                    {
                        foreach (var d in lDeleteSteps)
                        {
                            var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            var stepIds = delete.Where(x => x.Key == "stepsID");
                            if (!string.IsNullOrEmpty(stepIds.FirstOrDefault().Value.ToString()))
                            {
                                DataRow dr = dt.NewRow();
                                dr["STEPSID"] = stepIds.FirstOrDefault().Value.ToString();
                                dr["FEEDPROCESSDETAILID"] = valFeedD;
                                dr["Type"] = "Delete";
                                dr["TestcaseId"] = lTestCaseId;
                                dr["TestsuiteId"] = lTestSuiteId;
                                dt.Rows.Add(dr);
                            }
                        }
                    }

                    var lOldList = plist["oldList"];
                    //update value
                    var lUpdateSteps = plist["updateList"];
                    if (lUpdateSteps.Count > 0)
                    {
                        int Rowid = 0;
                        foreach (var d in lUpdateSteps)
                        {
                            var updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            var lflagAdded = false;
                            var lKeyword = "";
                            var lObject = "";
                            var lComment = "";
                            var lParameter = "";
                            var lstepsID = "";
                            var lRun_Order = "";
                            foreach (var item in updates)
                            {
                                var lflag = true;
                                if (item.Key == "keyword")
                                {
                                    lKeyword = Convert.ToString(item.Value);
                                    lflag = false;
                                }
                                if (item.Key == "object")
                                {
                                    lObject = Convert.ToString(item.Value);
                                    lflag = false;
                                }
                                if (item.Key == "comment")
                                {
                                    lComment = Convert.ToString(item.Value);
                                    lflag = false;
                                }
                                if (item.Key == "parameters")
                                {
                                    lParameter = Convert.ToString(item.Value);
                                    lflag = false;
                                }
                                if (item.Key == "stepsID")
                                {
                                    lstepsID = Convert.ToString(item.Value);
                                    lflag = false;
                                }
                                if (item.Key == "RUN_ORDER")
                                {
                                    lRun_Order = Convert.ToString(item.Value);
                                    lflag = false;
                                }
                                if (lflag && !string.IsNullOrEmpty(lstepsID))
                                {
                                    if (Convert.ToInt32(lstepsID) > 0)
                                    {
                                        foreach (var dataset in lDatasetnameList)
                                        {
                                            var lForSkipValue = updates.Any(x => x.Key.Replace("&amp;", "&") == dataset.Data_Summary_Name);
                                            var lSplitDatasetname = false;
                                            var lDatasetname = "";
                                            if (!lForSkipValue)
                                            {
                                                if (Convert.ToString(item.Key.Replace("&amp;", "&")).Contains("skip_"))
                                                {
                                                    lDatasetname = Convert.ToString(item.Key).Split(new string[] { "skip_" }, StringSplitOptions.None)[1];
                                                    lDatasetname = lDatasetname.Replace("&amp;", "&");
                                                    if (!updates.Any(x => x.Key.Replace("&amp;", "&") == lDatasetname))
                                                        lSplitDatasetname = true;
                                                }
                                            }

                                            if (dataset.Data_Summary_Name == Convert.ToString(item.Key.Replace("&amp;", "&")) || (lSplitDatasetname && dataset.Data_Summary_Name == lDatasetname))
                                            {

                                                DataRow dr = dt.NewRow();
                                                dr["STEPSID"] = lstepsID;
                                                dr["KEYWORD"] = lKeyword;
                                                dr["OBJECT"] = lObject;
                                                dr["PARAMETER"] = lParameter;
                                                dr["COMMENTS"] = lComment;
                                                dr["ROWNUMBER"] = lRun_Order;
                                                dr["DATASETNAME"] = dataset.Data_Summary_Name;
                                                dr["DATASETID"] = dataset.DATA_SUMMARY_ID;
                                                dr["FEEDPROCESSDETAILID"] = valFeedD;
                                                dr["Type"] = "Update";

                                                if (updates.Any(x => x.Key.Replace("&amp;", "&") == "DataSettingId_" + dataset.Data_Summary_Name))
                                                {
                                                    dr["Data_Setting_Id"] = Convert.ToString(updates.FirstOrDefault(x => x.Key.Replace("&amp;", "&") == "DataSettingId_" + dataset.Data_Summary_Name).Value) == "undefined" ? "0" : Convert.ToString(updates.FirstOrDefault(x => x.Key == "DataSettingId_" + dataset.Data_Summary_Name).Value);
                                                }
                                                if (updates.Any(x => x.Key.Replace("&amp;", "&") == dataset.Data_Summary_Name))
                                                {
                                                    dr["DATASETVALUE"] = Convert.ToString(updates.FirstOrDefault(x => x.Key.Replace("&amp;", "&") == dataset.Data_Summary_Name).Value.ToString().Trim());
                                                }
                                                if (updates.Any(x => x.Key.Replace("&amp;", "&") == "skip_" + dataset.Data_Summary_Name))
                                                {
                                                    var skipValue = Convert.ToString(updates.FirstOrDefault(x => x.Key.Replace("&amp;", "&") == "skip_" + dataset.Data_Summary_Name).Value);
                                                    if (skipValue.ToUpper().Trim() == "TRUE")
                                                        dr["SKIP"] = "4";
                                                    else
                                                        dr["SKIP"] = "0";
                                                }


                                                if (((System.Collections.Generic.Dictionary<string, object>)lOldList[Rowid]).Keys.Count == 0)
                                                {
                                                    dr["WhichTable"] = "RUN_ORDER";
                                                }
                                                dr["TestcaseId"] = lTestCaseId;
                                                dr["TestsuiteId"] = lTestSuiteId;
                                                dt.Rows.Add(dr);
                                                lflagAdded = true;

                                            }
                                        }
                                    }


                                }
                            }

                            if (!lflagAdded && !string.IsNullOrEmpty(lstepsID))
                            {
                                if (Convert.ToInt32(lstepsID) > 0)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["STEPSID"] = lstepsID;
                                    dr["KEYWORD"] = lKeyword;
                                    dr["OBJECT"] = lObject;
                                    dr["PARAMETER"] = lParameter;
                                    dr["COMMENTS"] = lComment;
                                    dr["ROWNUMBER"] = lRun_Order;
                                    dr["DATASETNAME"] = "";
                                    dr["DATASETID"] = "";
                                    dr["FEEDPROCESSDETAILID"] = valFeedD;
                                    dr["Type"] = "Update";

                                    if (((System.Collections.Generic.Dictionary<string, object>)lOldList[Rowid]).Keys.Count == 0)
                                    {
                                        dr["WhichTable"] = "RUN_ORDER";
                                    }

                                    dr["TestcaseId"] = lTestCaseId;
                                    dr["TestsuiteId"] = lTestSuiteId;

                                    dt.Rows.Add(dr);
                                    lflagAdded = true;
                                }
                            }
                            Rowid++;
                        }
                    }
                    var lAddSteps = plist["addList"];
                    if (lAddSteps.Count > 0)
                    {
                        int Rowid = 0;
                        foreach (var d in lAddSteps)
                        {
                            var adds = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            var lflagAdded = false;
                            var lKeyword = "";
                            var lObject = "";
                            var lComment = "";
                            var lParameter = "";

                            var lstepsID = Convert.ToString(adds.FirstOrDefault(x => x.Key == "stepsID").Value);
                            var lRun_Order = Convert.ToString(Convert.ToInt32(adds.FirstOrDefault(x => x.Key == "pq_ri").Value) + 1);
                            if (string.IsNullOrEmpty(lstepsID) || Convert.ToInt32(lstepsID) <= 0)
                            {
                                foreach (var item in adds)
                                {
                                    var lflag = true;
                                    if (item.Key == "keyword")
                                    {
                                        lKeyword = Convert.ToString(item.Value);
                                        lflag = false;
                                    }
                                    if (item.Key == "object")
                                    {
                                        lObject = Convert.ToString(item.Value);
                                        lflag = false;
                                    }
                                    if (item.Key == "comment")
                                    {
                                        lComment = Convert.ToString(item.Value);
                                        lflag = false;
                                    }
                                    if (item.Key == "parameters")
                                    {
                                        lParameter = Convert.ToString(item.Value);
                                        lflag = false;
                                    }

                                    if (lflag)
                                    {
                                        foreach (var dataset in lDatasetnameList)
                                        {
                                            var lForSkipValue = adds.Any(x => x.Key.Replace("&amp;", "&") == dataset.Data_Summary_Name);
                                            var lSplitDatasetname = false;
                                            var lDatasetname = "";
                                            if (!lForSkipValue)
                                            {
                                                if (Convert.ToString(item.Key).Contains("skip_"))
                                                {
                                                    lDatasetname = Convert.ToString(item.Key.Replace("&amp;", "&")).Split(new string[] { "skip_" }, StringSplitOptions.None)[1];
                                                    if (!adds.Any(x => x.Key == lDatasetname))
                                                        lSplitDatasetname = true;
                                                }
                                            }

                                            if (dataset.Data_Summary_Name == Convert.ToString(item.Key.Replace("&amp;", "&")) || (lSplitDatasetname && dataset.Data_Summary_Name == lDatasetname))
                                            {

                                                DataRow dr = dt.NewRow();
                                                dr["STEPSID"] = string.IsNullOrEmpty(lstepsID) ? Convert.ToString(Convert.ToInt32(minStepId) - 1) : lstepsID;
                                                dr["KEYWORD"] = lKeyword;
                                                dr["OBJECT"] = lObject;
                                                dr["PARAMETER"] = lParameter;
                                                dr["COMMENTS"] = lComment;
                                                dr["ROWNUMBER"] = lRun_Order;
                                                dr["DATASETNAME"] = dataset.Data_Summary_Name;
                                                dr["DATASETID"] = dataset.DATA_SUMMARY_ID;
                                                dr["FEEDPROCESSDETAILID"] = valFeedD;
                                                dr["Type"] = "Update";

                                                if (adds.Any(x => x.Key.Replace("&amp;", "&") == "DataSettingId_" + dataset.Data_Summary_Name))
                                                {
                                                    dr["Data_Setting_Id"] = Convert.ToString(adds.FirstOrDefault(x => x.Key.Replace("&amp;", "&") == "DataSettingId_" + dataset.Data_Summary_Name).Value) == "undefined" ? "0" : Convert.ToString(adds.FirstOrDefault(x => x.Key == "DataSettingId_" + dataset.Data_Summary_Name).Value);
                                                }
                                                if (adds.Any(x => x.Key.Replace("&amp;", "&") == dataset.Data_Summary_Name))
                                                {
                                                    dr["DATASETVALUE"] = Convert.ToString(adds.FirstOrDefault(x => x.Key.Replace("&amp;", "&") == dataset.Data_Summary_Name).Value.ToString().Trim());
                                                }
                                                if (adds.Any(x => x.Key.Replace("&amp;", "&") == "skip_" + dataset.Data_Summary_Name))
                                                {
                                                    var skipValue = Convert.ToString(adds.FirstOrDefault(x => x.Key.Replace("&amp;", "&") == "skip_" + dataset.Data_Summary_Name).Value);
                                                    if (skipValue.ToUpper().Trim() == "TRUE")
                                                        dr["SKIP"] = "4";
                                                    else
                                                        dr["SKIP"] = "0";
                                                }

                                                dr["TestcaseId"] = lTestCaseId;
                                                dr["TestsuiteId"] = lTestSuiteId;
                                                dt.Rows.Add(dr);
                                                lflagAdded = true;
                                            }
                                        }
                                    }
                                }

                                if (!lflagAdded)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["STEPSID"] = string.IsNullOrEmpty(lstepsID) ? Convert.ToString(Convert.ToInt32(minStepId) - 1) : lstepsID;
                                    dr["KEYWORD"] = lKeyword;
                                    dr["OBJECT"] = lObject;
                                    dr["PARAMETER"] = lParameter;
                                    dr["COMMENTS"] = lComment;
                                    dr["ROWNUMBER"] = lRun_Order;
                                    dr["DATASETNAME"] = "";
                                    dr["DATASETID"] = "";
                                    dr["FEEDPROCESSDETAILID"] = valFeedD;
                                    dr["Type"] = "Update";

                                    dr["TestcaseId"] = lTestCaseId;
                                    dr["TestsuiteId"] = lTestSuiteId;

                                    dt.Rows.Add(dr);
                                    lflagAdded = true;
                                }
                                minStepId--;
                            }
                        }
                    }

                    foreach (var item in lUpdateRownumber)
                    {
                        if (Convert.ToInt32(item.stepsID) > 0)
                        {
                            DataRow dr = dt.NewRow();
                            dr["STEPSID"] = Convert.ToString(item.stepsID);
                            dr["FEEDPROCESSDETAILID"] = valFeedD;
                            dr["ROWNUMBER"] = Convert.ToString(Convert.ToInt32(item.pq_ri) + 1);
                            dr["Type"] = "UpdateRowNumber";
                            dr["TestcaseId"] = lTestCaseId;
                            dr["TestsuiteId"] = lTestSuiteId;
                            dt.Rows.Add(dr);
                        }
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(Convert.ToString(dt.Rows[i]["STEPSID"])) <= 0)
                        {
                            dt.Rows[i]["Type"] = "Add";
                        }

                        if (lUpdateRownumber.Any(x => x.stepsID == Convert.ToString(dt.Rows[i]["STEPSID"])))
                        {
                            dt.Rows[i]["ROWNUMBER"] = Convert.ToString(Convert.ToInt32(lUpdateRownumber.FirstOrDefault(x => x.stepsID == Convert.ToString(dt.Rows[i]["STEPSID"])).pq_ri) + 1);
                        }

                        if (Convert.ToString(dt.Rows[i]["SKIP"]) != "4")
                        {
                            dt.Rows[i]["SKIP"] = "0";
                        }
                    }
                    if (dt.Rows.Count == 0)
                    {
                        resultModel.message = "No change in Testcase";
                        resultModel.status = 1;
                        return resultModel;
                    }
                    //insert into Stging table
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];
                                if (Convert.ToString(row.ItemArray[12]) == "Update" || Convert.ToString(row.ItemArray[12]) == "Add")
                                {
                                    var ParentObject = string.Empty;
                                    var rowNumber = Convert.ToInt32(row.ItemArray[5]);
                                    var lGridObjList = lobj.Where(x => x.pq_ri < rowNumber).ToList();
                                    if (lGridObjList != null)
                                    {
                                        foreach (var item in lGridObjList.OrderByDescending(x => x.pq_ri))
                                        {
                                            if (item.Keyword.ToLower().Trim() == "pegwindow")
                                            {
                                                ParentObject = item.Object;
                                                //row.ItemArray[16] = ParentObject;
                                                dt.Rows[i][16] = ParentObject;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    repTC.InsertStgTestCaseSave(AppConnDetails.ConnString, AppConnDetails.Schema, dt, lTestCaseId, int.Parse(valFeedD));
                    repTC.SaveTestCaseVersion(int.Parse(lTestCaseId), (long)SessionManager.TESTER_ID);

                    resultModel.data = "success";
                }
                else
                {
                    var result = JsonConvert.SerializeObject(ValidationSteps);
                    resultModel.data = result;
                }
                resultModel.message = "Test Case saved.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Delete Test case
        [Route("api/DeleteTestCase")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel DeleteTestCase(long TestCaseId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var testCaserepo = new TestCaseRepository();
                testCaserepo.Username = SessionManager.TESTER_LOGIN_NAME;
                //Checks if the testcase is present in the storyboard
                var lflag = testCaserepo.CheckTestCaseExistsInStoryboard(TestCaseId);
                if (lflag.Count <= 0)
                {
                    var lResult = testCaserepo.DeleteTestCase(TestCaseId);
                    resultModel.data = lResult;
                    resultModel.message = "Test Case Deleted Successfully";
                }
                else
                {
                    resultModel.data = lflag;
                    resultModel.message = "Test Case exist in storyboard";
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Get TestSuite list based on application name
        [Route("api/GetTestSuiteByApplicaton")]
        [AcceptVerbs("GET", "POST")]
        public List<RelTestSuiteApplication> GetTestSuiteByApplicaton(string ApplicationId)
        {
            CommonHelper.SetConnectionString(Request);
            var repTestSuite = new TestSuiteRepository();
            var lResult = new List<RelTestSuiteApplication>();
            if (!string.IsNullOrEmpty(ApplicationId))
            {
                lResult = repTestSuite.ListRelationTestSuiteApplication(ApplicationId);
            }

            return lResult;
        }

        //Add or edit a Test case
        [Route("api/AddEditTestCase")]
        [HttpPost]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditTestCase(TestCaseModel lModel)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTestSuite = new TestCaseRepository();
                repTestSuite.Username = SessionManager.TESTER_LOGIN_NAME;
                var repTree = new GetTreeRepository();
                repTree.Username = SessionManager.TESTER_LOGIN_NAME;
                var rel = repTestSuite.CheckTestCaseTestSuiteRel(lModel.TestCaseId, Convert.ToInt32(lModel.TestSuiteId));
                var flag = lModel.TestCaseId == 0 ? "added" : "Saved";
                if (rel == true)
                {
                    //checks if the testcase is present in the storyboard
                    var result = repTestSuite.CheckTestCaseExistsInStoryboard(lModel.TestCaseId);
                    if (result.Count <= 0)
                    {
                        var editresult = repTestSuite.AddEditTestCase(lModel, SessionManager.TESTER_LOGIN_NAME);
                        resultModel.data = editresult;
                        resultModel.message = "Successfully " + flag + " Test Case.";
                    }
                    else
                    {
                        resultModel.data = result;
                        resultModel.message = "Testcase already exist in storyboard";
                    }
                }
                else
                {
                    var fresult = repTestSuite.AddEditTestCase(lModel, SessionManager.TESTER_LOGIN_NAME);
                    resultModel.data = fresult;
                    resultModel.message = "Successfully " + flag + " Test Case.";
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Get keyword list
        [Route("api/GetKeywordsList")]
        [AcceptVerbs("GET", "POST")]
        public List<KeywordList> GetKeywordsList(int stepId, string lGrid)
        {
            CommonHelper.SetConnectionString(Request);
            JavaScriptSerializer js = new JavaScriptSerializer();

            var lobj = js.Deserialize<KeywordObjectLink[]>(lGrid);
            var repKeyword = new KeywordRepository();
            repKeyword.Username = SessionManager.TESTER_LOGIN_NAME;
            var lList = new List<KeywordList>();
            int lstepId = stepId;
            var lPegStepId = 0;
            int i = 1;

            //to find the first peg window
            foreach (var item in lobj)
            {
                if (!string.IsNullOrEmpty(item.Keyword))
                {
                    if (item.Keyword.ToLower() == "pegwindow" && lPegStepId == 0)
                    {
                        lPegStepId = i;
                        break;
                    }
                }

                i++;
            }
            //Loads keyword dropdown If it is first step
            if (lstepId == 1 || lstepId < lPegStepId || lPegStepId == 0)
            {
                var lKeywordList = new List<string>();
                lKeywordList.Add("pegwindow");
                lKeywordList.Add("dbcompare");
                lKeywordList.Add("copyexcelrangetoclipboard");
                lKeywordList.Add("executecommand");
                lKeywordList.Add("killapplication");
                lKeywordList.Add("loop");
                lKeywordList.Add("resumenext");
                lKeywordList.Add("startapplication");
                lKeywordList.Add("waitforseconds");


                lList = repKeyword.GetKeywords().Where(x => lKeywordList.Contains(x.KEY_WORD_NAME.ToLower().Trim())).Select(y => new KeywordList
                {
                    KeywordId = y.KEY_WORD_ID,
                    KeywordName = y.KEY_WORD_NAME
                }).ToList();
            }
            //Loads keyword dropdown for rest of the steps
            else
            {
                lList = repKeyword.GetKeywords().Select(y => new KeywordList
                {
                    KeywordId = y.KEY_WORD_ID,
                    KeywordName = y.KEY_WORD_NAME
                }).ToList();
            }

            return lList;
        }

        //Get Object list
        [Route("api/GetObjectList")]
        [AcceptVerbs("GET", "POST")]
        public List<ObjectList> GetObjectList(string lGrid, int stepId, int lTestCaseId)
        {
            CommonHelper.SetConnectionString(Request);
            JavaScriptSerializer js = new JavaScriptSerializer();
            var repObject = new ObjectRepository();
            var repKeyword = new KeywordRepository();
            var lList = new List<ObjectList>();

            var lobj = js.Deserialize<KeywordObjectLink[]>(lGrid);

            int lPegStepId = 0;
            decimal lPegObjectId = 0;
            long llinkedKeywordId = 0;
            lobj.Where(c => c.Keyword == null).ToList().ForEach(x => { x.Keyword = ""; });

            var lPegKeywordList = lobj.Where(x => x.Keyword.ToLower() == "pegwindow").ToList();
            //var lSelectedGrid = lPegKeywordList.Where(x => x.Id == stepId).ToList();
            if (lPegKeywordList.Count() > 0)
            {
                if (stepId < lPegKeywordList.First().Id)
                {
                    lList = new List<ObjectList>();
                }
                else
                {
                    var lPegKeywordNameList = lobj.Where(x => x.Id == stepId).ToList();
                    var lPegKeywordName = lPegKeywordNameList.First().Keyword;
                    var lLinkedKeyList = repKeyword.GetKeywordByName(lPegKeywordName);
                    if (lLinkedKeyList != null)
                    {
                        llinkedKeywordId = lLinkedKeyList.KEY_WORD_ID;
                        var lPegObjectName = lPegKeywordList.Where(x => x.Id < stepId).OrderByDescending(y => y.Id).First().Object;

                        var lPegObj = repObject.GetPegObjectByObjectName(lPegObjectName);
                        if (lPegObj != null)
                        {
                            lPegObjectId = lPegObj.OBJECT_NAME_ID;
                            lList = repObject.GetObjectByParent(lTestCaseId, lPegObjectId, llinkedKeywordId).Select(y => new ObjectList
                            {
                                ObjectId = y.OBJECT_NAME_ID,
                                ObjectName = y.OBJECT_HAPPY_NAME
                            }).OrderBy(y => y.ObjectName).ToList();
                        }
                    }
                }
            }

            return lList;
        }

        [Route("api/AddEditDataset")]
        [HttpPost]
        [AcceptVerbs("GET", "POST")]
        public string AddEditDataset(long? Testcaseid, long? datasetid, string datasetname, string datasetdesc, DataSetTagModel model)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var testCaserepo = new TestCaseRepository();
            var result = testCaserepo.AddTestDataSet(Testcaseid, datasetid, datasetname, datasetdesc, model, AppConnDetails.ConnString, AppConnDetails.Schema);
            string[] result1 = result.Split(',');
            var lresult = new
            {
                datasetId = result1[0],
                msg = result1[1]
            };
            return result;
        }

        [Route("api/CheckDatasetExistsInStoryboard")]
        [AcceptVerbs("GET", "POST")]
        public List<string> CheckDatasetExistsInStoryboard(long datasetid)
        {
            CommonHelper.SetConnectionString(Request);
            var repo = new TestCaseRepository();
            var result = repo.CheckDatasetInStoryboard(datasetid);
            return result;
        }

        [Route("api/TestCaseDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel TestCaseDataLoad([FromBody]SearchModel searchModel)
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
                string TestSuiteSearch = string.Empty;
                string orderDir = string.Empty;

                var repAcc = new TestCaseRepository();

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
                    TestSuiteSearch = searchModel.columns[3].search.value;
                }


                var data = new List<TestCaseModel>();

                data = repAcc.ListAllTestCase(AppConnDetails.Schema, AppConnDetails.ConnString, startRec, pageSize, colOrder, orderDir, NameSearch, DescriptionSearch, ApplicationSearch, TestSuiteSearch);


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

        //[Route("api/GetTestSuiteByproject")]
        //[AcceptVerbs("GET", "POST")]
        //public List<TestSuiteListByProject> GetTestSuiteByproject(long ProjectId)
        //{
        //    CommonHelper.SetConnectionString(Request);
        //    var repTree = new GetTreeRepository();
        //    var lTestSuiteList = repTree.GetTestSuiteList(ProjectId);
        //    //ViewBag.testsuiteList = lTestSuiteList.Select(c => new SelectListItem { Text = c.TestsuiteName, Value = c.TestsuiteId.ToString() }).ToList();
        //    return lTestSuiteList;
        //}

        [Route("api/MoveTestCase")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel MoveTestCase(long lprojectId, long lsuiteId, long caseId)
        {
            BaseModel baseModel = new BaseModel();
            CommonHelper.SetConnectionString(Request);
            var repo = new TestCaseRepository();
            var result = repo.CheckTestCaseExistsInStoryboard(caseId);
            if (result.Count > 0)
            {
                baseModel.data = result;
                baseModel.status = 0;
                baseModel.message = "error";

            }
            else
            {
                var lresult = repo.MoveTestCase(lprojectId, caseId, lsuiteId);
                baseModel.status = 1;
                baseModel.message = "Success";
            }
            return baseModel;
        }

        [Route("api/SaveAsTestCase")]
        [AcceptVerbs("GET", "POST")]
        public string SaveAsTestCase(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string optionval, string datasetName = "", string suffix = "")
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var repo = new TestCaseRepository();
            var result = string.Empty;
            if (!string.IsNullOrEmpty(suffix))
            {
                suffix = suffix.Trim();
            }
            if (optionval == "1")
            {
                result = repo.SaveAsTestcase(testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, AppConnDetails.Schema, AppConnDetails.ConnString, SessionManager.TESTER_LOGIN_NAME);
            }
            else if (optionval == "2")
            {
                result = repo.SaveAsTestCaseOneCopiedDataSet(testcasename, oldtestcaseid, testcasedesc, datasetName, testsuiteid, projectid, suffix, AppConnDetails.Schema, AppConnDetails.ConnString, SessionManager.TESTER_LOGIN_NAME);
            }
            else if (optionval == "3")
            {
                result = repo.SaveAsTestCaseAllCopiedDataSet(testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, suffix, AppConnDetails.Schema, AppConnDetails.ConnString, SessionManager.TESTER_LOGIN_NAME);
            }
            return result;
        }

        [Route("api/UpdateIsAvailable")]
        [AcceptVerbs("GET", "POST")]
        public bool UpdateIsAvailable(string TestCaseIds)
        {
            CommonHelper.SetConnectionString(Request);
            var rep = new TestCaseRepository();
            rep.UpdateIsAvailable(TestCaseIds);
            return true;
        }

        [Route("api/GetDataSetCount")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel GetDataSetCount(long ProjectId, long TestSuiteId, long TestCaseId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new GetTreeRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.GetDatasetCount(ProjectId, TestSuiteId, TestCaseId);
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        [Route("api/CopyDataSet")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel CopyDataSet(long? testcaseid, long? oldDatasetid, string datasetname, string datasetdescription = "")
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            try
            {
                TestCaseRepository repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDuplicateDataset(datasetname, null);
                if (result)
                {
                    resultModel.message = "Duplicate";
                    resultModel.data = "Duplicate";
                }
                else
                {
                    var lresult = repo.CopyDataSet(testcaseid, oldDatasetid, datasetname, datasetdescription, AppConnDetails.ConnString, AppConnDetails.Schema);
                    string[] result1 = lresult.Split(',');
                    var fresult = new
                    {
                        datasetId = result1[1],
                        msg = result1[0]
                    };
                    resultModel.message = fresult.msg == "success" ? "Dataset [" + datasetname + "] added successfully." : "Dataset  [" + datasetname + "] is already present in the System";
                    resultModel.data = fresult;
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        [Route("api/GetTestSuiteByproject")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel GetTestSuiteByproject(long ProjectId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTree = new GetTreeRepository();
                var lTestSuiteList = repTree.GetTestSuiteList(ProjectId);
                resultModel.data = lTestSuiteList;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //This method will load all the data and filter them
        [Route("api/GroupDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel GroupDataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                var repo = new TestCaseRepository();
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string NameSearch = string.Empty;
                string DescSearch = string.Empty;

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
                    DescSearch = searchModel.columns[1].search.value;
                }

                var data = repo.ListOfGroup();

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DescSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.ToString().ToLower().Contains(DescSearch.ToLower())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                        case "Description":
                            data = data.OrderByDescending(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                        case "Description":
                            data = data.OrderBy(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                    }
                }

                int totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower())
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

        //Add/Update Group values
        [HttpPost]
        [Route("api/AddEditGroup")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditGroup(DataTagCommonViewModel model)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                var _addeditResult = repo.AddEditGroup(model);

                resultModel.message = "Saved [" + model.Name + "] Group.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Check Group name already exist or not
        [Route("api/CheckDuplicateGroupNameExist")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel CheckDuplicateGroupNameExist(string Name, long? Id)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                Name = Name.Trim();
                var repo = new TestCaseRepository();
                var result = repo.CheckDuplicateGroupNameExist(Name, Id);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //This method will load all the data and filter them
        [HttpPost]
        [Route("api/SetDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel SetDataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                var repo = new TestCaseRepository();
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string NameSearch = string.Empty;
                string DescSearch = string.Empty;

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
                    DescSearch = searchModel.columns[1].search.value;
                }

                var data = repo.ListOfSet();

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DescSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.ToString().ToLower().Contains(DescSearch.ToLower())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                        case "Description":
                            data = data.OrderByDescending(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                        case "Description":
                            data = data.OrderBy(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                    }
                }

                int totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower())
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

        //Add/Update Set values
        [HttpPost]
        [Route("api/AddEditSet")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditSet(DataTagCommonViewModel model)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                var _addeditResult = repo.AddEditSet(model);

                resultModel.message = "Saved [" + model.Name + "] Set.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Check Set name already exist or not
        [Route("api/CheckDuplicateSetNameExist")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel CheckDuplicateSetNameExist(string Name, long? Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Name = Name.Trim();
                var repo = new TestCaseRepository();
                var result = repo.CheckDuplicateSetNameExist(Name, Id);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //This method will load all the data and filter them
        [HttpPost]
        [Route("api/FolderDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel FolderDataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            try
            {
                var repo = new TestCaseRepository();
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string NameSearch = string.Empty;
                string DescSearch = string.Empty;

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
                    DescSearch = searchModel.columns[1].search.value;
                }

                var data = repo.ListOfSet();

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DescSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.ToString().ToLower().Contains(DescSearch.ToLower())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                        case "Description":
                            data = data.OrderByDescending(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                        case "Description":
                            data = data.OrderBy(a => a.Description).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                    }
                }

                int totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower())
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

        //Add/Update Folder values
        [HttpPost]
        [Route("api/AddEditFolder")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditFolder(DataTagCommonViewModel model)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                var _addeditResult = repo.AddEditFolder(model);

                resultModel.message = "Saved [" + model.Name + "] Folder.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Check Folder name already exist or not
        [Route("api/CheckDuplicateFolderNameExist")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel CheckDuplicateFolderNameExist(string Name, long? Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Name = Name.Trim();
                var repo = new TestCaseRepository();
                var result = repo.CheckDuplicateFolderNameExist(Name, Id);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Delete the DatasetTag data by datasetid
        [HttpPost]
        [Route("api/DeleteDatSetTag")]
        [AcceptVerbs("GET", "POST")]
        public string DeleteDatSetTag(long datasetid)
        {
            TestCaseRepository repo = new TestCaseRepository();
            var result = repo.DeleteTagProperties(datasetid);
            return result;
        }

        //Check Folder Sequence already exist or not
        [HttpPost]
        [Route("api/CheckFolderSequenceMapping")]
        [AcceptVerbs("GET", "POST")]
        public bool CheckFolderSequenceMapping(long FolderId, long SequenceId, long DatasetId)
        {
                var repo = new TestCaseRepository();
                var result = repo.CheckFolderSequenceMapping(FolderId, SequenceId, DatasetId);
                return result;
        }
    }
}

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
        public List<TestCaseResult> GetTestCaseDetails(int testcaseId, int testsuiteid, long UserId)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            TestCaseRepository tc = new TestCaseRepository();
            //string luser = WebConfigurationManager.AppSettings["User"];
            //string lpassword = WebConfigurationManager.AppSettings["Password"];
            //string ldataSource = WebConfigurationManager.AppSettings["DataSource"];
            //string lSchema = WebConfigurationManager.AppSettings["Schema"];
            //var lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";

            //var result = tc.GetTestCaseDetail(testcaseId, testsuiteid, lSchema, lConnectionStr, UserId).ToList();
            var result = tc.GetTestCaseDetail(testcaseId, AppConnDetails.Schema, AppConnDetails.ConnString, UserId).ToList();
            //var result = tc.GetTestCaseDetails(testcaseId, testsuiteid);
            // var json = JsonConvert.SerializeObject(result).ToList();

            return result;
        }

        //Change TestCase name
        [Route("api/ChangeTestCaseName")]
        [AcceptVerbs("GET", "POST")]
        public string ChangeTestCaseName(string TestCaseName, long TestCaseId, string TestCaseDes)
        {
            CommonHelper.SetConnectionString(Request);
            var testCaserepo = new TestCaseRepository();
            var value = testCaserepo.ChangeTestCaseName(TestCaseName, TestCaseId, TestCaseDes);
            return value;
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
        // GET: TestCase
        //TestCaseRepository tc = new TestCaseRepository();


        [AcceptVerbs("GET", "POST")]
        [Route("api/ValidateTestCase")]
        //Validate the TestCase grid
        public string ValidateTestCase(string lJson, string testCaseId = "986", string pKeywordObject = "", string testSuiteId = "224", string steps = "", string NewColumnsList = "", string ExistDataSetRenameList = "", string DeleteColumnsList = "", string SkipColumns = "")
        {
            var result = "";
            try
            {
                CommonHelper.SetConnectionString(Request);
                JavaScriptSerializer js = new JavaScriptSerializer();
                TestCaseRepository tc = new TestCaseRepository();

                decimal testcaseId = int.Parse(testCaseId);
                decimal testsuiteid = int.Parse(testSuiteId);


                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(pKeywordObject);
                var ValidationResult = tc.CheckObjectKeywordValidation(lobj.ToList(), int.Parse(testCaseId));

                var ValidationSteps = ValidationResult.Where(x => x.IsNotValid == true).ToList();
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
        public string SaveTestCase(string lJson, string testCaseId, string testSuiteId, long UserId, string pKeywordObject = "", string steps = "", string NewColumnsList = "",
               string ExistDataSetRenameList = "", string DeleteColumnsList = "", string SkipColumns = "", string Version = "")
        {
            // string filepath = System.Web.HttpContext.Current.Server.MapPath("~/AppData/Log.txt");  //Text File Path
            try
            {
                var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
                CommonHelper.SetConnectionString(Request);
                JavaScriptSerializer js = new JavaScriptSerializer();
                TestCaseRepository tc = new TestCaseRepository();
                decimal testcaseId = int.Parse(testCaseId);
                decimal testsuiteid = int.Parse(testSuiteId);
                var lUserId = SessionManager.TESTER_ID;

                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(pKeywordObject);
                var ValidationResult = tc.CheckObjectKeywordValidation(lobj.ToList(), int.Parse(testCaseId));

                var ValidationSteps = ValidationResult.Where(x => x.IsNotValid == true).ToList();

                if (ValidationSteps.Count() == 0)
                {

                    if (!String.IsNullOrEmpty(Version))
                    {
                        var lflag = tc.MatchTestCaseVersion(int.Parse(testCaseId), Convert.ToInt64(Version));
                        if (!lflag)
                        {
                            return "Another User Edited this Test Case. Please reload selected TestCase and Make changes.";
                        }
                    }


                    SkipColumns = SkipColumns.Replace("\\", "\\\\");
                    // var query = tc.GetTestCaseDetails(testcaseId, testsuiteid);
                    var skipData = js.Deserialize<Dictionary<String, List<Object>>>(SkipColumns);

                    #region <<Update Steps Region>>
                    Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lJson);
                    if (steps != "")
                    {
                        List<Object> stps = js.Deserialize<List<Object>>(steps);
                        if (stps != null)
                        {
                            foreach (var s in stps)
                            {
                                if (s != null)
                                {
                                    var stp = (((System.Collections.Generic.Dictionary<string, object>)s).ToList());
                                    //if (stp[1].Value.ToString() != stp[2].Value.ToString())
                                    //{
                                    var stepsid = stp.Where(x => x.Key == "stepsid");
                                    var stepid = Convert.ToInt32(stepsid.FirstOrDefault().Value.ToString());
                                    // var stepid = int.Parse(stp[0].Value.ToString());
                                    if (stepid > 0)
                                    {
                                        var newRun_Order = int.Parse(stp[1].Value.ToString());
                                        tc.UpdateStepID(stepid, int.Parse(stp[1].Value.ToString()));
                                    }
                                    // }
                                }
                            }
                        }
                    }
                    #endregion
                    #region <<Update Data Set Name and Description>>
                    //var datasets = js.Deserialize<List<Object>>(ExistDataSetRenameList);
                    //if (datasets != null)
                    //{
                    //    foreach (var d in datasets)
                    //    {
                    //        var ds = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                    //        if (ds.Count() > 3)
                    //        {
                    //            if (ds[0].Value.ToString() != ds[1].Value.ToString())
                    //            {
                    //                tc.updateDataSetName(ds[1].Value.ToString(), long.Parse(ds[3].Value.ToString()));
                    //            }
                    //            tc.updateDataSetDescription(ds[2].Value.ToString(), long.Parse(ds[3].Value.ToString()));
                    //        }
                    //    }
                    //}                    //var datasets = js.Deserialize<List<Object>>(ExistDataSetRenameList);
                    //if (datasets != null)
                    //{
                    //    foreach (var d in datasets)
                    //    {
                    //        var ds = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                    //        if (ds.Count() > 3)
                    //        {
                    //            if (ds[0].Value.ToString() != ds[1].Value.ToString())
                    //            {
                    //                tc.updateDataSetName(ds[1].Value.ToString(), long.Parse(ds[3].Value.ToString()));
                    //            }
                    //            tc.updateDataSetDescription(ds[2].Value.ToString(), long.Parse(ds[3].Value.ToString()));
                    //        }
                    //    }
                    //}
                    #endregion

                    if (DeleteColumnsList != "")
                    {
                        List<Object> DataSet = js.Deserialize<List<Object>>(DeleteColumnsList);
                        foreach (var d in DataSet)
                        {
                            var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            tc.DeleteRelTestCaseDataSummary(long.Parse(testCaseId), long.Parse(stp[1].Value.ToString()));
                        }
                    }

                    //if (NewColumnsList != "")
                    //{
                    //    List<Object> DataSet = js.Deserialize<List<Object>>(NewColumnsList);
                    //    foreach (var d in DataSet)
                    //    {
                    //        var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                    //        if (stp.Count() > 2)
                    //        {
                    //            tc.AddTestDataSet(long.Parse(testCaseId), stp[1].Value.ToString(), stp[2].Value.ToString());
                    //        }
                    //        else
                    //        {
                    //            tc.AddTestDataSet(long.Parse(testCaseId), stp[1].Value.ToString(), "");
                    //        }
                    //    }
                    //}



                    ///<summary>
                    /// Remove Step
                    ///</summary>
                    if (dlist["deleteList"].Count > 0)
                    {
                        foreach (var d in dlist["deleteList"])
                        {
                            var delete = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            var stepid = delete.Where(x => x.Key == "stepsID");
                            if (!string.IsNullOrEmpty(stepid.FirstOrDefault().Value.ToString()))
                            {
                                tc.DeleteStep(Convert.ToInt32(stepid.FirstOrDefault().Value.ToString()));
                            }
                        }

                        tc.UpdateSteps(int.Parse(testCaseId));
                    }

                    ///<summary>
                    /// Update Test case
                    ///</summary>

                    string returnValues = tc.InsertFeedProcess();

                    var valFeed = returnValues.Split('~')[0];
                    var valFeedD = returnValues.Split('~')[1];

                    //string luser = WebConfigurationManager.AppSettings["User"];
                    //string lpassword = WebConfigurationManager.AppSettings["Password"];
                    //string ldataSource = WebConfigurationManager.AppSettings["DataSource"];
                    //string lSchema = WebConfigurationManager.AppSettings["Schema"];
                    //var lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";
                    //string lSchema = SessionManager.Schema;
                    //var lConnectionStr = SessionManager.APP;
                    //var query = tc.GetTestCaseDetail(Convert.ToInt64(testcaseId), Convert.ToInt64(testsuiteid), lSchema, lConnectionStr, (long)SessionManager.TESTER_ID);
                    var query = tc.GetTestCaseDetail(Convert.ToInt64(testcaseId), AppConnDetails.Schema, AppConnDetails.ConnString, (long)SessionManager.TESTER_ID);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("TESTSUITENAME");
                    dt.Columns.Add("TESTCASENAME");
                    dt.Columns.Add("TESTCASEDESCRIPTION");
                    dt.Columns.Add("DATASETMODE");
                    dt.Columns.Add("KEYWORD");
                    dt.Columns.Add("OBJECT");
                    dt.Columns.Add("PARAMETER");
                    dt.Columns.Add("COMMENTS");
                    dt.Columns.Add("DATASETNAME");
                    dt.Columns.Add("DATASETVALUE");
                    dt.Columns.Add("ROWNUMBER");
                    dt.Columns.Add("FEEDPROCESSDETAILID");
                    dt.Columns.Add("TABNAME");
                    dt.Columns.Add("APPLICATION");
                    dt.Columns.Add("SKIP");
                    dt.Columns.Add("DATASETDESCRIPTION");
                    dt.Columns.Add("STEPSID");
                    dt.Columns.Add("Data_Setting_Id");
                    dt.Columns.Add("DATASETID");

                    DataTable ddt = new DataTable();
                    ddt.Columns.Add("TESTSUITENAME");
                    ddt.Columns.Add("TESTCASENAME");
                    ddt.Columns.Add("TESTCASEDESCRIPTION");
                    ddt.Columns.Add("DATASETMODE");
                    ddt.Columns.Add("KEYWORD");
                    ddt.Columns.Add("OBJECT");
                    ddt.Columns.Add("PARAMETER");
                    ddt.Columns.Add("COMMENTS");
                    ddt.Columns.Add("DATASETNAME");
                    ddt.Columns.Add("DATASETVALUE");
                    ddt.Columns.Add("ROWNUMBER");
                    ddt.Columns.Add("FEEDPROCESSDETAILID");
                    ddt.Columns.Add("TABNAME");
                    ddt.Columns.Add("APPLICATION");
                    ddt.Columns.Add("SKIP");
                    ddt.Columns.Add("DATASETDESCRIPTION");
                    ddt.Columns.Add("STEPSID");
                    ddt.Columns.Add("DATA_SETTING_ID");
                    ddt.Columns.Add("DATASETID");
                    int rowCounter = -1;

                    var lskipData = js.Deserialize<Dictionary<String, List<Object>>>(SkipColumns);
                    if (dlist["updateList"].Count > 0 || dlist["addList"].Count > 0 || lskipData.Count > 0)
                    {
                        if (query != null && query.Count > 0)
                        {
                            foreach (var q in query)
                            {
                                rowCounter++;
                                string DATASETNAME = q.DATASETNAME;
                                string DATASETID = q.DATASETIDS;
                                string DATASETVALUE = q.DATASETVALUE;
                                string SKIP = q.SKIP;
                                string[] datasetnames1 = DATASETNAME.Split(',');
                                if (q.DATASETVALUE == null)
                                {
                                    for (int i = 0; i < datasetnames1.Length; i++)
                                    {
                                        datasetnames1[i] = datasetnames1[i];//.Replace("_", " ");  cherish
                                    }
                                    DATASETVALUE = DATASETVALUE == null ? "" : DATASETVALUE;
                                    foreach (var xy in datasetnames1)
                                    {
                                        if (DATASETVALUE != null)
                                            DATASETVALUE += ",";

                                    }
                                }
                                string DataSettingIds = q.Data_Setting_Id;
                                //if (datasetnames1.Length < ((updates.Count - 5) / 2))
                                //{
                                //    for (int i = 6; i < updates.Count; i++)
                                //    {

                                //        if ((!updates[i].Key.StartsWith("skip")) && (!datasetnames1.Contains(updates[i].Key.Replace("_", " "))))
                                //        {
                                //            if (DATASETNAME != "")
                                //            {
                                //                DATASETNAME += ",";
                                //            }
                                //            DATASETNAME += updates[i].Key;

                                //            if (SKIP != "")
                                //            {
                                //                SKIP += ",";
                                //            }
                                //            var rowskip = (((System.Collections.Generic.List<object>)skipData[rowCounter.ToString()]).ToList())[0];
                                //            if (rowskip != null)
                                //            {
                                //                var sk = (((System.Collections.Generic.Dictionary<string, object>)rowskip).ToList());
                                //                if (sk != null)
                                //                {
                                //                    SKIP += sk[((i - 6) / 2)].Value.ToString();
                                //                }
                                //                else
                                //                {
                                //                    SKIP += "0"; // put skip data set value
                                //                }
                                //            }
                                //            else
                                //            {
                                //                SKIP += "0"; // put skip data set value
                                //            }

                                //            if (DATASETVALUE != "")
                                //            {
                                //                DATASETVALUE += ",";
                                //            }
                                //            if (updates.Count >= i && updates[i].Value != null)
                                //            {
                                //                DATASETVALUE += updates[i].Value.ToString();
                                //            }
                                //            else
                                //            {
                                //                DATASETVALUE += "";
                                //            }
                                //        }
                                //    }
                                //}
                                string[] datasetnames = DATASETNAME.Split(',');
                                string[] datasetids = DATASETID.Split(',');
                                string[] skips = SKIP != null ? SKIP.Split(',') : null;

                                string[] datasetvalues = DATASETVALUE != null ? DATASETVALUE.Split(',') : null;
                                string[] datasetDescription = q.DATASETDESCRIPTION != null ? q.DATASETDESCRIPTION.Split(',') : null;
                                string[] DataSettingId = DataSettingIds != null ? DataSettingIds.Split(',') : null;
                                for (int i = 0; i < datasetnames.Length; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["STEPSID"] = q.STEPS_ID.ToString();
                                    dr["TESTSUITENAME"] = q.test_suite_name.ToString();
                                    dr["TESTCASENAME"] = q.test_case_name.ToString();
                                    dr["TESTCASEDESCRIPTION"] = q.test_step_description != null ? q.test_step_description.ToString() : "";
                                    dr["DATASETMODE"] = "";
                                    dr["KEYWORD"] = q.key_word_name != null ? q.key_word_name.ToString() : "";
                                    dr["OBJECT"] = q.object_happy_name != null ? q.object_happy_name.ToString() : "";
                                    dr["PARAMETER"] = q.parameter != null ? q.parameter.ToString() : "";
                                    dr["COMMENTS"] = q.COMMENT != null ? q.COMMENT.ToString() : "";
                                    dr["DATASETNAME"] = datasetnames[i].ToString();
                                    dr["DATASETID"] = datasetids[i].ToString();

                                    if (DataSettingId != null && DataSettingId.Length > 0 && DataSettingId.Length > i)
                                    {
                                        dr["DATA_SETTING_ID"] = DataSettingId[i].ToString();
                                    }
                                    else
                                    {
                                        dr["DATA_SETTING_ID"] = null;
                                    }


                                    if (skips != null && skips.Length > 0 && skips.Length >= i)
                                    {
                                        dr["SKIP"] = skips[i].ToString();
                                    }
                                    else
                                    {
                                        dr["SKIP"] = "";
                                    }
                                    if (datasetvalues != null && datasetvalues.Length > 0 && datasetvalues.Length >= i)
                                    {
                                        if (datasetvalues.Count() > i)
                                        {
                                            dr["DATASETVALUE"] = datasetvalues[i].ToString();
                                        }
                                        else
                                        {
                                            dr["DATASETVALUE"] = "";
                                        }
                                    }
                                    else
                                    {
                                        dr["DATASETVALUE"] = "";
                                    }
                                    dr["ROWNUMBER"] = q.RUN_ORDER.ToString();
                                    dr["FEEDPROCESSDETAILID"] = 0;
                                    dr["TABNAME"] = "WebApp";
                                    dr["APPLICATION"] = q.Application.ToString();
                                    //if (datasetDescription.Length > i)
                                    //{
                                    dr["DATASETDESCRIPTION"] = datasetDescription[i];
                                    //}
                                    //else {
                                    //    dr["DATASETDESCRIPTION"] = "";
                                    // }
                                    dr["FEEDPROCESSDETAILID"] = valFeedD;
                                    dt.Rows.Add(dr);
                                }

                            }


                            // Merge updated list from web application to the datatable

                        }

                    }

                    if (dlist["updateList"].Count > 0)//dlist["addList"].Count > 0
                    {
                        var update = dlist["updateList"][0];
                        var updates = (((System.Collections.Generic.Dictionary<string, object>)update).ToList());




                        // get test case id from the ljson and get all data set records


                        if (dlist.Count > 0)
                        {
                            if (dlist["updateList"].Count > 0)
                            {
                                // var a = dlist["updateList"][0];
                                // string stepId = (((System.Collections.Generic.Dictionary<string, object>)a).FirstOrDefault()).Value.ToString();
                                //if (stepId != "")
                                //{
                                //    if (testCaseId != "" && testSuiteId != "")
                                //    {

                                //    }
                                //}
                                if (dt.Rows.Count > 0)
                                {
                                    foreach (var d in dlist["updateList"])
                                    {
                                        updates = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());

                                        string stepsID = "";
                                        string lKeyword = "";
                                        string lObject = "";
                                        string lParameter = "";
                                        string lComment = "";



                                        foreach (var item in updates)
                                        {
                                            if (item.Key == "keyword")
                                            {
                                                lKeyword = Convert.ToString(item.Value);
                                            }
                                            if (item.Key == "object")
                                            {
                                                lObject = Convert.ToString(item.Value);
                                            }
                                            if (item.Key == "comment")
                                            {
                                                lComment = Convert.ToString(item.Value);
                                            }
                                            if (item.Key == "parameters")
                                            {
                                                lParameter = Convert.ToString(item.Value);
                                            }
                                            if (item.Key == "stepsID")
                                            {
                                                stepsID = Convert.ToString(item.Value);
                                            }
                                        }
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            if (dt.Rows[i]["STEPSID"].ToString() == stepsID)
                                            {
                                                dt.Rows[i]["COMMENTS"] = lComment;
                                                dt.Rows[i]["PARAMETER"] = lParameter;
                                                dt.Rows[i]["OBJECT"] = lObject;
                                                dt.Rows[i]["KEYWORD"] = lKeyword;
                                                string DATASETNAME = query.FirstOrDefault().DATASETNAME;
                                                if (!string.IsNullOrEmpty(DATASETNAME))
                                                {
                                                    string[] datasetnamesArray = query.FirstOrDefault().DATASETNAME.Split(',');
                                                    foreach (var item in datasetnamesArray)
                                                    {
                                                        if (dt.Rows[i]["DATASETNAME"].ToString().ToUpper() == item.ToUpper())
                                                        {
                                                            foreach (var up in updates)
                                                            {

                                                                //  if (up.Key.ToUpper().Replace("_", " ") == item.ToUpper()) cherish
                                                                if (up.Key.ToUpper().Replace("&AMP;", "&").Replace("\\", "\\\\") == item.ToUpper().Replace("&AMP;", "&").Replace("\\", "\\\\"))
                                                                {
                                                                    if (up.Value != null)
                                                                    {
                                                                        dt.Rows[i]["DATASETVALUE"] = up.Value.ToString();
                                                                    }
                                                                    else
                                                                    {
                                                                        dt.Rows[i]["DATASETVALUE"] = "";
                                                                    }

                                                                    //var lSkipList = updates.Where(x => x.Key.ToUpper() == "SKIP_" + item.ToUpper()).ToList();
                                                                    //if (lSkipList.Count() > 0)
                                                                    // {
                                                                    //     dt.Rows[i]["SKIP"] = lSkipList.FirstOrDefault().Value.ToString().ToUpper() == "TRUE" ? 4 : 0;
                                                                    //}
                                                                    //else
                                                                    //{
                                                                    //   dt.Rows[i]["SKIP"] = 0;
                                                                    //}
                                                                }
                                                                else if (up.Key.ToUpper().Replace("&AMP;", "&") == "SKIP_" + item.ToUpper().Replace("&AMP;", "&"))
                                                                {
                                                                    dt.Rows[i]["SKIP"] = up.Value.ToString().ToUpper() == "TRUE" ? 4 : 0;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                        //    string DATASETNAME = query.FirstOrDefault().DATASETNAME;
                                        //if (!string.IsNullOrEmpty(DATASETNAME))
                                        //{
                                        //    string[] datasetnamesArray = query.FirstOrDefault().DATASETNAME.Split(',');
                                        //    foreach (var datasetName in datasetnamesArray) {
                                        //        foreach (var item in updates)
                                        //        {
                                        //            if (item.Key.ToUpper() == datasetName.ToUpper())
                                        //            {

                                        //            }
                                        //        }
                                        //     }
                                        //}
                                        //shivam code
                                        //for (int i = 0; i < dt.Rows.Count; i++)
                                        //{
                                        //    if (dt.Rows[i]["STEPSID"].ToString() == stepsID)
                                        //    {
                                        //        for (int k = 6; k < updates.Count; k++)
                                        //        {
                                        //            datasetname = updates[k].Key.ToString();
                                        //            if (datasetname.Replace("_", " ") == dt.Rows[i]["DATASETNAME"].ToString().Replace("_", " "))
                                        //            {
                                        //                if (updates[k - 1].Key.ToString().ToLower().StartsWith("skip"))
                                        //                {
                                        //                    dt.Rows[i]["SKIP"] = updates[k - 1].Value.ToString() == "True" ? "4" : updates[k - 1].Value.ToString() == "4" ? "4" : "0";
                                        //                }
                                        //                if (updates.Count >= k && updates[k].Value != null)
                                        //                {
                                        //                    dt.Rows[i]["DATASETVALUE"] = updates[k].Value.ToString();
                                        //                }
                                        //                else
                                        //                {
                                        //                    dt.Rows[i]["DATASETVALUE"] = "";
                                        //                }

                                        //                dt.Rows[i]["COMMENTS"] = comment;
                                        //                dt.Rows[i]["PARAMETER"] = parameters;

                                        //                dt.Rows[i]["OBJECT"] = obj;
                                        //                dt.Rows[i]["KEYWORD"] = keyword;
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                    }


                                }
                            }
                        }
                    }

                    if (dlist["addList"].Count > 0)
                    {
                        var addList = dlist["addList"][0];
                        var addsList = (((System.Collections.Generic.Dictionary<string, object>)addList).ToList());


                        if (addsList.Count > 0)
                        {
                            var lastdtRownumber = dt.Rows.Count;
                            var newrowid = 1;
                            foreach (var d in dlist["addList"])
                            {
                                newrowid = newrowid - 1;
                                var add = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                                string lKeyword = "", lObject = "", lComment = "", lParameter = "";
                                foreach (var item in add)
                                {
                                    if (item.Key == "keyword")
                                    {
                                        lKeyword = item.Value.ToString();
                                    }
                                    if (item.Key == "object" && item.Value != null)
                                    {
                                        lObject = item.Value.ToString();
                                    }
                                    if (item.Key == "comment" && item.Value != null)
                                    {
                                        lComment = item.Value.ToString();

                                    }
                                    if (item.Key == "parameters" && item.Value != null)
                                    {
                                        lParameter = item.Value.ToString();

                                    }
                                }
                                string DATASETNAME = query.FirstOrDefault().DATASETNAME;
                                if (!string.IsNullOrEmpty(DATASETNAME))
                                {
                                    string[] datasetnamesArray = query.FirstOrDefault().DATASETNAME.Split(',');
                                    for (int i = 0; i < datasetnamesArray.Length; i++)
                                    {
                                        datasetnamesArray[i] = datasetnamesArray[i];
                                    }
                                    lastdtRownumber = lastdtRownumber + 1;
                                    string[] datasetDescription = query.FirstOrDefault().DATASETDESCRIPTION != null ? query.FirstOrDefault().DATASETDESCRIPTION.Split(',') : null;
                                    string[] datasetid = query.FirstOrDefault().DATASETIDS != null ? query.FirstOrDefault().DATASETIDS.Split(',') : null;
                                    int datSetDec = 0;
                                    foreach (var datasetName in datasetnamesArray)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["STEPSID"] = newrowid;
                                        dr["TESTSUITENAME"] = query.FirstOrDefault().test_suite_name.ToString();
                                        dr["TESTCASENAME"] = query.FirstOrDefault().test_case_name.ToString();
                                        dr["TESTCASEDESCRIPTION"] = query.FirstOrDefault().test_step_description != null ? query.FirstOrDefault().test_step_description.ToString() : "";
                                        dr["DATASETNAME"] = datasetName;
                                        foreach (var item in add)
                                        {
                                            // if (item.Key.ToUpper().Replace("_", " ") == datasetName.ToUpper().Replace("_", " ")) cherish
                                            if (item.Key.ToUpper() == datasetName.ToUpper())
                                            {
                                                if (item.Value != null)
                                                {
                                                    dr["DATASETVALUE"] = item.Value.ToString();
                                                }
                                                //var lSkipList = add.Where(x => x.Key.ToUpper() == "SKIP_" + datasetName.ToUpper()).ToList();
                                                //if(lSkipList.Count() > 0)
                                                //{
                                                //    dr["skip"] = lSkipList.FirstOrDefault().Value.ToString().ToUpper() == "TRUE" ? 4 : 0; 
                                                //}
                                                //else
                                                //{
                                                //    dr["skip"] = 0;
                                                //}
                                            }
                                            //else if (item.Key.ToUpper().Replace("_", " ") == "SKIP " + datasetName.ToUpper().Replace("_", " ")) cherish
                                            else if (item.Key.ToUpper() == "SKIP_" + datasetName.ToUpper())
                                            {
                                                dr["skip"] = item.Value.ToString().ToUpper() == "TRUE" ? 4 : 0;
                                            }
                                        }

                                        dr["KEYWORD"] = lKeyword;// q.key_word_name != null ? q.key_word_name.ToString() : "";
                                        dr["OBJECT"] = lObject;//q.object_happy_name != null ? q.object_happy_name.ToString() : "";
                                        dr["PARAMETER"] = lParameter;// q.parameter != null ? q.parameter.ToString() : "";
                                        dr["COMMENTS"] = lComment;// q.COMMENT != null ? q.COMMENT.ToString() : "";
                                        dr["ROWNUMBER"] = lastdtRownumber;//q.RUN_ORDER.ToString();
                                        dr["FEEDPROCESSDETAILID"] = 0;
                                        dr["TABNAME"] = "WebApp";
                                        dr["APPLICATION"] = query.FirstOrDefault().Application.ToString();
                                        //dr["DATASETDESCRIPTION"] = query.FirstOrDefault().test_step_description != null ? query.FirstOrDefault().test_step_description.ToString() : "";
                                        dr["DATASETDESCRIPTION"] = datasetDescription[datSetDec];
                                        dr["DATASETID"] = datasetid[datSetDec];
                                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                                        dt.Rows.Add(dr);
                                        datSetDec++;
                                    }
                                }




                                //if (q.DATASETVALUE == null)
                                //{
                                //    for (int i = 0; i < datasetnames1.Length; i++)
                                //    {
                                //        datasetnames1[i] = datasetnames1[i].Replace("_", " ");
                                //    }
                                //    DATASETVALUE = DATASETVALUE == null ? "" : DATASETVALUE;
                                //    foreach (var xy in datasetnames1)
                                //    {
                                //        if (DATASETVALUE != null)
                                //            DATASETVALUE += ",";

                                //    }
                                //}


                                //dr["DATASETNAME"] = datasetnames[i].ToString();
                                // if (skips != null && skips.Length > 0 && skips.Length >= i)
                                // {
                                //     dr["SKIP"] = skips[i].ToString();
                                // }
                                // else
                                // {
                                //     dr["SKIP"] = "";
                                // }
                                // if (datasetvalues != null && datasetvalues.Length > 0 && datasetvalues.Length >= i)
                                // {
                                //     if (datasetvalues.Count() > i)
                                //     {
                                //         dr["DATASETVALUE"] = datasetvalues[i].ToString();
                                //     }
                                //     else
                                //     {
                                //         dr["DATASETVALUE"] = "";
                                //     }
                                // }
                                // else
                                // {
                                //     dr["DATASETVALUE"] = "";
                                // }





                            }
                        }
                    }

                    //for new add list

                    if (steps != "")
                    {
                        List<Object> stps = js.Deserialize<List<Object>>(steps);
                        if (stps != null)
                        {
                            foreach (var s in stps)
                            {
                                if (s != null)
                                {
                                    var stp = (((System.Collections.Generic.Dictionary<string, object>)s).ToList());
                                    //if (stp[1].Value.ToString() != stp[2].Value.ToString())
                                    //{
                                    var stepid = int.Parse(stp[0].Value.ToString());
                                    //if (stepid > 0)
                                    //{
                                    var newRun_Order = int.Parse(stp[1].Value.ToString());
                                    var stepsID = stp[0].Value.ToString();
                                    tc.UpdateStepID(int.Parse(stp[0].Value.ToString()), int.Parse(stp[1].Value.ToString()));
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (dr["STEPSID"].ToString() == stepsID)
                                        {
                                            dr["ROWNUMBER"] = newRun_Order;
                                            ddt.Rows.Add(dr.ItemArray);
                                        }
                                    }
                                    //for (int i = 0; i < dt.Rows.Count; i++)
                                    //{
                                    //    if (dt.Rows[i]["STEPSID"].ToString() == stepsID) {
                                    //        ddt.Rows.Add(dt.Rows[i]);
                                    //    }                                    //}
                                    //}

                                    // }
                                }
                            }
                        }
                    }




                    if (lskipData.Count > 0)
                    {

                        if (NewColumnsList != "")
                        {
                            List<Object> DataSet = js.Deserialize<List<Object>>(NewColumnsList);
                            foreach (var d in DataSet)
                            {
                                var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                                //tc.AddTestDataSet(long.Parse(testCaseId), stp[1].Value.ToString(), stp[2].Value.ToString());
                                int k = 1;
                                foreach (var item in lskipData)
                                {

                                    foreach (Dictionary<string, object> skipColumn in item.Value)
                                    {
                                        foreach (KeyValuePair<string, object> skipitem in skipColumn)
                                        {
                                            if (skipitem.Key.ToString() == "skip_" + stp[1].Value.ToString())
                                            {

                                                DataRow dr = ddt.AsEnumerable().Where(r => r.Field<string>("ROWNUMBER") == k.ToString()
                                                && r.Field<string>("DATASETNAME").Replace("&amp;", "&") == stp[1].Value.ToString()
                                                ).FirstOrDefault();
                                                if (dr != null)
                                                {
                                                    dr["SKIP"] = skipitem.Value.ToString(); //changes the Product_name
                                                }
                                                //var lData = ddt.AsEnumerable().Where(r => r.Field<string>("ROWNUMBER") == k.ToString());
                                                //for (int i = 0; i < lData.Count(); i++)
                                                //{



                                                //}
                                            }
                                        }

                                    }

                                    k++;
                                }

                            }
                        }




                    }


                    //-------------------------------------------------------------
                    // Save the values to the database in staging table
                    //-------------------------------------------------------------
                    if (ddt.Rows.Count > 0)
                    {

                        OracleTransaction ltransaction;
                        //luser = WebConfigurationManager.AppSettings["User"];
                        //lpassword = WebConfigurationManager.AppSettings["Password"];
                        //ldataSource = WebConfigurationManager.AppSettings["DataSource"];
                        //lSchema = WebConfigurationManager.AppSettings["Schema"];
                        //lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";
                        //lSchema = SessionManager.Schema;
                        //lConnectionStr = SessionManager.APP;
                        OracleConnection lconnection = new OracleConnection(AppConnDetails.ConnString);
                        lconnection.Open();
                        ltransaction = lconnection.BeginTransaction();



                        //string cmdquery = @"insert into TBLSTGTESTCASE ( TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION) values(TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,NEWCOUNT_ID,CURRENTDATE,SKIP,DATASETDESCRIPTION)";
                        string cmdquery = "insert into TBLSTGWEBTESTCASE ( TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION,STEPSID,TESTSUITEID,TESTCASEID,DATA_SETTING_ID,DATASETID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14,:15,:16,:17,:18,:19,:20,:21,:22,:23)";
                        //string cmdquery = "insert into TBLSTGWEBTESTCASE ( TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION,STEPSID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14,:15,:16,:17,:18,:19)";
                        int[] ids = new int[ddt.Rows.Count];

                        using (var lcmd = lconnection.CreateCommand())
                        {
                            lcmd.CommandText = cmdquery;
                            // lcmd.CommandType = CommandType.Text;                          

                            //  lcmd.Transaction = ltransaction;
                            // In order to use ArrayBinding, the ArrayBindCount property
                            // of OracleCommand object must be set to the number of records to be inserted
                            lcmd.ArrayBindCount = ids.Length;



                            string[] TESTSUITENAME_param = ddt.AsEnumerable().Select(r => r.Field<string>("TESTSUITENAME")).ToArray();
                            string[] TESTCASENAME_param = ddt.AsEnumerable().Select(r => r.Field<string>("TESTCASENAME")).ToArray();
                            string[] TESTCASEDESCRIPTION_param = ddt.AsEnumerable().Select(r => r.Field<string>("TESTCASEDESCRIPTION")).ToArray();
                            string[] DATASETMODE_param = ddt.AsEnumerable().Select(r => r.Field<string>("DATASETMODE")).ToArray();
                            string[] KEYWORD_param = ddt.AsEnumerable().Select(r => r.Field<string>("KEYWORD")).ToArray();
                            string[] OBJECT_param = ddt.AsEnumerable().Select(r => r.Field<string>("OBJECT")).ToArray();
                            string[] PARAMETER_param = ddt.AsEnumerable().Select(r => r.Field<string>("PARAMETER")).ToArray();
                            string[] COMMENTS_param = ddt.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
                            string[] DATASETNAME_param = ddt.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                            string[] DATASETVALUE_param = ddt.AsEnumerable().Select(r => r.Field<string>("DATASETVALUE")).ToArray();
                            string[] ROWNUMBER_param = ddt.AsEnumerable().Select(r => r.Field<string>("ROWNUMBER")).ToArray(); ;

                            string[] FEEDPROCESSDETAILID_param = ddt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                            string[] TABNAME_param = ddt.AsEnumerable().Select(r => r.Field<string>("TABNAME")).ToArray();
                            string[] APPLICATION_param = ddt.AsEnumerable().Select(r => r.Field<string>("APPLICATION")).ToArray();

                            string[] ID_param = new string[ids.Length];
                            for (int p = 0; p < ids.Length; p++)
                            {
                                ID_param[p] = "1";
                            }

                            DateTime[] CREATEDON_param = new DateTime[ids.Length];
                            for (int p = 0; p < ids.Length; p++)
                            {
                                CREATEDON_param[p] = DateTime.Now;
                            }

                            string[] SKIP_param = ddt.AsEnumerable().Select(r => r.Field<string>("SKIP")).ToArray();
                            string[] DATASETDESCRIPTION_param = ddt.AsEnumerable().Select(r => r.Field<string>("DATASETDESCRIPTION")).ToArray();

                            string[] STEPSID_param = ddt.AsEnumerable().Select(r => r.Field<string>("STEPSID")).ToArray();


                            string[] TESTSUITEID_param = new string[ids.Length];
                            for (int p = 0; p < ids.Length; p++)
                            {
                                TESTSUITEID_param[p] = Convert.ToString(testsuiteid);
                            }

                            string[] TESTCASEID_param = new string[ids.Length];
                            for (int p = 0; p < ids.Length; p++)
                            {
                                TESTCASEID_param[p] = Convert.ToString(testcaseId);
                            }

                            string[] DATA_SETTING_ID_param = ddt.AsEnumerable().Select(r => r.Field<string>("DATA_SETTING_ID")).ToArray();
                            string[] DATASETID_param = ddt.AsEnumerable().Select(r => r.Field<string>("DATASETID")).ToArray();

                            OracleParameter TESTSUITENAME_oparam = new OracleParameter();
                            TESTSUITENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                            TESTSUITENAME_oparam.Value = TESTSUITENAME_param;

                            OracleParameter TESTCASENAME_oparam = new OracleParameter();
                            TESTCASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                            TESTCASENAME_oparam.Value = TESTCASENAME_param;

                            OracleParameter TESTCASEDESCRIPTION_oparam = new OracleParameter();
                            TESTCASEDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                            TESTCASEDESCRIPTION_oparam.Value = TESTCASEDESCRIPTION_param;

                            OracleParameter DATASETMODE_oparam = new OracleParameter();
                            DATASETMODE_oparam.OracleDbType = OracleDbType.Varchar2;
                            DATASETMODE_oparam.Value = DATASETMODE_param;

                            OracleParameter KEYWORD_oparam = new OracleParameter();
                            KEYWORD_oparam.OracleDbType = OracleDbType.Varchar2;
                            KEYWORD_oparam.Value = KEYWORD_param;

                            OracleParameter OBJECT_oparam = new OracleParameter();
                            OBJECT_oparam.OracleDbType = OracleDbType.Varchar2;
                            OBJECT_oparam.Value = OBJECT_param;

                            OracleParameter PARAMETER_oparam = new OracleParameter();
                            PARAMETER_oparam.OracleDbType = OracleDbType.Varchar2;
                            PARAMETER_oparam.Value = PARAMETER_param;

                            OracleParameter COMMENTS_oparam = new OracleParameter();
                            COMMENTS_oparam.OracleDbType = OracleDbType.Varchar2;
                            COMMENTS_oparam.Value = COMMENTS_param;

                            OracleParameter DATASETNAME_oparam = new OracleParameter();
                            DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                            DATASETNAME_oparam.Value = DATASETNAME_param;

                            OracleParameter DATASETVALUE_oparam = new OracleParameter();
                            DATASETVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                            DATASETVALUE_oparam.Value = DATASETVALUE_param;

                            OracleParameter ROWNUMBER_oparam = new OracleParameter();
                            ROWNUMBER_oparam.OracleDbType = OracleDbType.Varchar2;
                            ROWNUMBER_oparam.Value = ROWNUMBER_param;

                            OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                            FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                            FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                            OracleParameter TABNAME_oparam = new OracleParameter();
                            TABNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                            TABNAME_oparam.Value = TABNAME_param;

                            OracleParameter APPLICATION_oparam = new OracleParameter();
                            APPLICATION_oparam.OracleDbType = OracleDbType.Varchar2;
                            APPLICATION_oparam.Value = APPLICATION_param;

                            OracleParameter ID_oparam = new OracleParameter();
                            ID_oparam.OracleDbType = OracleDbType.Varchar2;
                            ID_oparam.Value = ID_param;

                            OracleParameter CREATEDON_oparam = new OracleParameter();
                            CREATEDON_oparam.OracleDbType = OracleDbType.Date;
                            CREATEDON_oparam.Value = CREATEDON_param;

                            OracleParameter SKIP_oparam = new OracleParameter();
                            SKIP_oparam.OracleDbType = OracleDbType.Varchar2;
                            SKIP_oparam.Value = SKIP_param;

                            OracleParameter DATASETDESCRIPTION_oparam = new OracleParameter();
                            DATASETDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                            DATASETDESCRIPTION_oparam.Value = DATASETDESCRIPTION_param;

                            OracleParameter STEPSID_oparam = new OracleParameter();
                            STEPSID_oparam.OracleDbType = OracleDbType.Varchar2;
                            STEPSID_oparam.Value = STEPSID_param;


                            OracleParameter TESTSUITEID_oparam = new OracleParameter();
                            TESTSUITEID_oparam.OracleDbType = OracleDbType.Varchar2;
                            TESTSUITEID_oparam.Value = TESTSUITEID_param;

                            OracleParameter TESTCASEID_oparam = new OracleParameter();
                            TESTCASEID_oparam.OracleDbType = OracleDbType.Varchar2;
                            TESTCASEID_oparam.Value = TESTCASEID_param;

                            OracleParameter DATA_SETTING_ID_oparam = new OracleParameter();
                            DATA_SETTING_ID_oparam.OracleDbType = OracleDbType.Varchar2;
                            DATA_SETTING_ID_oparam.Value = DATA_SETTING_ID_param;

                            OracleParameter DATASETID_oparam = new OracleParameter();
                            DATASETID_oparam.OracleDbType = OracleDbType.Varchar2;
                            DATASETID_oparam.Value = DATASETID_param;



                            lcmd.Parameters.Add(TESTSUITENAME_oparam);
                            lcmd.Parameters.Add(TESTCASENAME_oparam);
                            lcmd.Parameters.Add(TESTCASEDESCRIPTION_oparam);
                            lcmd.Parameters.Add(DATASETMODE_oparam);
                            lcmd.Parameters.Add(KEYWORD_oparam);
                            lcmd.Parameters.Add(OBJECT_oparam);
                            lcmd.Parameters.Add(PARAMETER_oparam);
                            lcmd.Parameters.Add(COMMENTS_oparam);
                            lcmd.Parameters.Add(DATASETNAME_oparam);
                            lcmd.Parameters.Add(DATASETVALUE_oparam);
                            lcmd.Parameters.Add(ROWNUMBER_oparam);
                            lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                            lcmd.Parameters.Add(TABNAME_oparam);
                            lcmd.Parameters.Add(APPLICATION_oparam);
                            lcmd.Parameters.Add(ID_oparam);
                            lcmd.Parameters.Add(CREATEDON_oparam);
                            lcmd.Parameters.Add(SKIP_oparam);
                            lcmd.Parameters.Add(DATASETDESCRIPTION_oparam);
                            lcmd.Parameters.Add(STEPSID_oparam);
                            lcmd.Parameters.Add(TESTSUITEID_oparam);
                            lcmd.Parameters.Add(TESTCASEID_oparam);
                            lcmd.Parameters.Add(DATA_SETTING_ID_oparam);
                            lcmd.Parameters.Add(DATASETID_oparam);
                            //int result = 

                            //if (result == bulkData.Count)
                            //    returnValue = true;
                            try
                            {

                                lcmd.ExecuteNonQuery();
                            }
                            catch (Exception lex)
                            {

                                ltransaction.Rollback();

                                throw new Exception(lex.Message);
                            }

                            ltransaction.Commit();
                            lconnection.Close();

                        }

                        var ret = tc.SaveData(int.Parse(valFeed), AppConnDetails.ConnString, AppConnDetails.Schema);

                        if (ret == "not saved")
                        {

                            var r = tc.GetValidations(int.Parse(valFeed));

                            var result = JsonConvert.SerializeObject(r);

                            return result;
                        }
                        else
                        {

                            tc.SaveTestCaseVersion(int.Parse(testCaseId), (long)SessionManager.TESTER_ID);

                            return "success";
                        }

                    }
                }
                else
                {
                    var result = JsonConvert.SerializeObject(ValidationSteps);
                    return result;
                }
                //-------------------------------------------------------------
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        //Delete Test case
        [Route("api/DeleteTestCase")]
        [AcceptVerbs("GET", "POST")]
        public bool DeleteTestCase(long TestCaseId)
        {
            CommonHelper.SetConnectionString(Request);
            var testCaserepo = new TestCaseRepository();
            var lResult = testCaserepo.DeleteTestCase(TestCaseId);
            //Session["TestCaseDeleteMsg"] = "Successfully TestCase Deleted.";
            // Session["TestcaseId"] = null;
            //Session["TestsuiteId"] = null;
            //Session["ProjectId"] = null;
            return lResult;
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
        public bool AddEditTestCase(TestCaseModel lModel)
        {
            CommonHelper.SetConnectionString(Request);
            var repTestSuite = new TestCaseRepository();
            var lResult =
                 repTestSuite.AddEditTestCase(lModel, SessionManager.TESTER_LOGIN_NAME);

            return lResult;
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
            var lList = new List<KeywordList>();

            var lPegStepId = 0;
            int i = 1;
            foreach (var item in lobj)
            {
                if (!string.IsNullOrEmpty(item.Keyword))
                {
                    if (item.Keyword.ToLower() == "pegwindow" && lPegStepId == 0)
                    {
                        lPegStepId = i;
                    }
                }
                i++;
            }



            if (stepId == 1 || stepId < lPegStepId || lPegStepId == 0)
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

            var lobj = js.Deserialize<KeywordObjectLink[]>(lGrid);
            var repObject = new ObjectRepository();
            var repKeyword = new KeywordRepository();
            var lList = new List<ObjectList>();

            long lPegKeywordId = 0;
            int lPegStepId = 0;
            int i = 1;
            decimal lPegObjectId = 0;
            long llinkedKeywordId = 0;
            foreach (var item in lobj)
            {
                var tblKeyword = repKeyword.GetKeywordByName(item.Keyword);
                if (tblKeyword != null)
                {
                    if (stepId >= i)
                    {

                        // var lKeywordPegType = repKeyword.CheckKeywordPegType(tblKeyword.KEY_WORD_ID);
                        var lKeywordPegType = false;
                        if (!string.IsNullOrEmpty(item.Keyword))
                        {
                            if (item.Keyword.ToLower() == "pegwindow")
                            {
                                lKeywordPegType = true;
                            }
                        }
                        if (lKeywordPegType)
                        {
                            lPegKeywordId = tblKeyword.KEY_WORD_ID;
                            if (!string.IsNullOrEmpty(item.Object))
                            {
                                lPegObjectId = repObject.GetObjectByObjectName(item.Object).OBJECT_NAME_ID;
                            }
                            lPegStepId = i;
                        }
                    }

                    if (stepId == i)
                    {
                        llinkedKeywordId = tblKeyword.KEY_WORD_ID;
                    }
                }
                i++;
            }

            if (lPegStepId == 0)
            {
                lList = new List<ObjectList>();
            }
            else if (lPegStepId == stepId)
            {
                lList = repObject.GetObjectsByPegWindowType(lTestCaseId).OrderBy(y => y.ObjectName).ToList();

            }
            else if (lPegStepId < stepId)
            {
                lList = repObject.GetObjectByParent(lTestCaseId, lPegObjectId, llinkedKeywordId).Select(y => new ObjectList
                {
                    ObjectId = y.OBJECT_NAME_ID,
                    ObjectName = y.OBJECT_HAPPY_NAME
                }).OrderBy(y => y.ObjectName).ToList();

            }


            //if (lPegStepId > 0 || stepId < lPegStepId || lPegStepId == 0)
            //{
            //    lList = repObject.GetObjectsByPegWindowType(lTestCaseId).Select(y => new ObjectList
            //    {
            //        ObjectId = y.OBJECT_NAME_ID,
            //        ObjectName = y.OBJECT_HAPPY_NAME
            //    }).ToList();
            //}
            //else
            //{
            //    lList = repObject.GetObjects(lTestCaseId).Select(y => new ObjectList
            //    {
            //        ObjectId = y.OBJECT_NAME_ID,
            //        ObjectName = y.OBJECT_HAPPY_NAME
            //    }).ToList();
            //}

            return lList;
        }

        [Route("api/AddEditDataset")]
        [HttpPost]
        [AcceptVerbs("GET", "POST")]
        public string AddEditDataset(long? Testcaseid, long? datasetid, string datasetname, string datasetdesc,DataSetTagModel model)
        {
            CommonHelper.SetConnectionString(Request);
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var testCaserepo = new TestCaseRepository();
            var result = testCaserepo.AddTestDataSet(Testcaseid, datasetid, datasetname, datasetdesc,model, AppConnDetails.ConnString, AppConnDetails.Schema);
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
        public string SaveAsTestCase(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid)
        {
            var AppConnDetails = CommonHelper.SetAppConnectionString(Request);
            var repo = new TestCaseRepository();
            var result = repo.SaveAsTestcase(testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, AppConnDetails.Schema, AppConnDetails.ConnString, AppConnDetails.Login);
            return result;
        }

        [Route("api/UpdateIsAvailable")]
        [AcceptVerbs("GET", "POST")]
        public bool UpdateIsAvailable(string TestCaseIds)
        {
            var rep = new TestCaseRepository();
            rep.UpdateIsAvailable(TestCaseIds);
            return true;
        }
    }
}

using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.Repositories
{
    public class StoryBoardRepository
    {
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        public string Username = string.Empty;

        public string GetStoryboardNameById(long Storyboardid)
        {
            try
            {
                logger.Info(string.Format("Get StoryboardName Id start | Storyboardid: {0} | Username: {1}", Storyboardid, Username));
                var storyboarname = enty.T_STORYBOARD_SUMMARY.FirstOrDefault(x => x.STORYBOARD_ID == Storyboardid).STORYBOARD_NAME;
                logger.Info(string.Format("Get StoryboardName Id end | Storyboardid: {0} | Username: {1}", Storyboardid, Username));
                return storyboarname;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad GetStoryboardNameById method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad GetStoryboardNameById method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string AddEditStoryboard(StoryboardModel Model)
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.Storyboardname) || !string.IsNullOrEmpty(Model.StoryboardDescription))
                {
                    Model.Storyboardname = Model.Storyboardname.Trim();
                    Model.StoryboardDescription = Model.StoryboardDescription.Trim();
                }
                var result = enty.T_STORYBOARD_SUMMARY.Find(Model.Storyboardid);
                if (result == null)
                {
                    logger.Info(string.Format("Add Storyboard start | Storyboard: {0} | Username: {1}", Model.Storyboardname, Username));
                    T_STORYBOARD_SUMMARY summary = new T_STORYBOARD_SUMMARY();
                    var storyboardid = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    summary.STORYBOARD_ID = storyboardid;
                    summary.STORYBOARD_NAME = Model.Storyboardname;
                    summary.DESCRIPTION = Model.StoryboardDescription;
                    summary.ASSIGNED_PROJECT_ID = Model.ProjectId;
                    enty.T_STORYBOARD_SUMMARY.Add(summary);
                    enty.SaveChanges();
                    logger.Info(string.Format("Add Storyboard end | Storyboard: {0} | Username: {1}", Model.Storyboardname, Username));
                    return "success";
                }
                else
                {
                    logger.Info(string.Format("Edit Storyboard start | Storyboard: {0} | Storyboardid: {1} | Username: {2}", Model.Storyboardname, Model.Storyboardid, Username));
                    result.STORYBOARD_NAME = Model.Storyboardname;
                    result.DESCRIPTION = Model.StoryboardDescription;
                    result.ASSIGNED_PROJECT_ID = Model.ProjectId;
                    enty.SaveChanges();
                    logger.Info(string.Format("Edit Storyboard end | Storyboard: {0} | Storyboardid: {1} | Username: {2}", Model.Storyboardname, Model.Storyboardid, Username));
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad AddEditStoryboard method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad AddEditStoryboard method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public IList<StoryboardModel> GetStoryboards(string lconstring, string schema, int startrec, int pagesize, string Storyboardsnamesearch, string Storyboarddescsearch, string Projectnamesearch, string colname, string colorder)
        {
            try
            {
                logger.Info(string.Format("GetStoryboards start | Username: {0}", Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;
                OracleParameter[] ladd_refer_image = new OracleParameter[8];
                ladd_refer_image[0] = new OracleParameter("Startrec", OracleDbType.Long);
                ladd_refer_image[0].Value = startrec;

                ladd_refer_image[1] = new OracleParameter("totalpagesize", OracleDbType.Long);
                ladd_refer_image[1].Value = pagesize;

                ladd_refer_image[2] = new OracleParameter("ColumnName", OracleDbType.Varchar2);
                ladd_refer_image[2].Value = colname;

                ladd_refer_image[3] = new OracleParameter("Columnorder", OracleDbType.Varchar2);
                ladd_refer_image[3].Value = colorder;

                ladd_refer_image[4] = new OracleParameter("SName", OracleDbType.Varchar2);
                ladd_refer_image[4].Value = Storyboardsnamesearch;

                ladd_refer_image[5] = new OracleParameter("SDesc", OracleDbType.Varchar2);
                ladd_refer_image[5].Value = Storyboarddescsearch;

                ladd_refer_image[6] = new OracleParameter("Projname", OracleDbType.Varchar2);
                ladd_refer_image[6].Value = Projectnamesearch;


                ladd_refer_image[7] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[7].Direction = ParameterDirection.Output;
                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "Sp_List_Storyboards";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<StoryboardModel> result = dt.AsEnumerable().Select(row =>
                      new StoryboardModel
                      {
                          Storyboardid = row.Field<long>("StoryboardId"),
                          Storyboardname = Convert.ToString(row.Field<string>("Storyboardname")),
                          StoryboardDescription = Convert.ToString(row.Field<string>("description")),
                          ProjectId = row.Field<long>("ProjectId"),
                          Projectname = Convert.ToString(row.Field<string>("ProjectName")),
                          TotalCount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                      }).ToList();

                logger.Info(string.Format("GetStoryboards end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad GetStoryboards method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad GetStoryboards method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public IList<StoryBoardResultModel> GetStoryBoardDetails(string schema, string lconstring, long Projectid, long Storyboardid)
        {
            try
            {
                logger.Info(string.Format("Get StoryBoard Details start | storyboradId: {0} | UserName: {1}", Storyboardid, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("PROJECTID", OracleDbType.Long);
                ladd_refer_image[0].Value = Projectid;

                ladd_refer_image[1] = new OracleParameter("Storyboardid", OracleDbType.Long);
                ladd_refer_image[1].Value = Storyboardid;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_STORYBOARD_DETAILS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                IList<StoryBoardResultModel> result = dt.AsEnumerable().Select(row =>
                  new StoryBoardResultModel
                  {
                      Run_order = row.Field<long>("RunOrder"),
                      ApplicationName = row.Field<string>("ApplicationName"),
                      ProjectId = row.Field<long>("ProjectId"),
                      ProjectName = row.Field<string>("ProjectName"),
                      ProjectDescription = row.Field<string>("ProjectDescription"),
                      Storyboardname = row.Field<string>("storyboard_name"),
                      Storyboardid = row.Field<long>("storyboardid"),
                      ActionName = row.Field<string>("ActionName"),
                      StepName = row.Field<string>("StepName"),
                      TestSuiteName = row.Field<string>("SuiteName"),
                      TestCaseName = row.Field<string>("CaseName"),
                      DataSetName = row.Field<string>("DataSetName"),
                      BTestResult = row.Field<string>("BTEST_RESULT"),
                      BErrorcause = row.Field<string>("BTEST_RESULT_IN_TEXT"),
                      BScriptstart = row.Field<DateTime?>("BTEST_BEGIN_TIME"),
                      BScriptend = row.Field<DateTime?>("BTEST_END_TIME"),
                      CTestResult = row.Field<string>("CTEST_RESULT"),
                      CErrorcause = row.Field<string>("CTEST_RESULT_IN_TEXT"),
                      CScriptstart = row.Field<DateTime?>("CTEST_BEGIN_TIME"),
                      CScriptend = row.Field<DateTime?>("CTEST_END_TIME"),
                      Dependency = row.Field<string>("Dependency"),
                      Description = row.Field<string>("test_step_description"),
                      Suiteid = row.Field<long>("suiteid"),
                      Caseid = row.Field<long>("caseid"),
                      Datasetid = row.Field<long>("datasetid"),
                      storyboarddetailid = row.Field<long>("storyboarddetailid"),
                      BHistid = row.Field<long?>("BHIST_ID"),
                      CHistid = row.Field<long?>("CHIST_ID")
                  }).ToList();
                if (result.Count() > 1)
                {
                    result = result.Where(x => x.Run_order != 0).OrderBy(x => Convert.ToInt32(x.Run_order)).ToList();
                }
                logger.Info(string.Format("Get StoryBoard Details end | storyboradId: {0} | UserName: {1}", Storyboardid, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in GetStoryBoardDetails method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in GetStoryBoardDetails method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public List<SYSTEM_LOOKUP> GetActions(long sid)
        {
            try
            {
                logger.Info(string.Format("Get Actions start | storyboradId: {0} | UserName: {1}", sid, Username));
                var lresult = new List<SYSTEM_LOOKUP>();
                var result = enty.SYSTEM_LOOKUP.Where(x => x.FIELD_NAME == "RUN_TYPE" && x.TABLE_NAME == "T_PROJ_TC_MGR").ToList();
                foreach (var item in result)
                {
                    lresult = result.ToList();
                    if (item.DISPLAY_NAME == "FAILUE")
                    {
                        lresult.Remove(item);
                    }
                }
                logger.Info(string.Format("Get Actions end | storyboradId: {0} | UserName: {1}", sid, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in GetActions method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in GetActions method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<TestCaseListByProject> GetTestCaseList(long lProjectId, string TestSuitename)
        {
            try
            {
                logger.Info(string.Format("Get TestCase List start | TestSuitename: {0} | UserName: {1}", TestSuitename, Username));
                var lTestcaseTree = new List<TestCaseListByProject>();

                var lList = from t1 in enty.T_TEST_PROJECT
                            join t2 in enty.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                            join t3 in enty.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                            join t4 in enty.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                            join t5 in enty.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                            join t6 in enty.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                            where t1.PROJECT_ID == lProjectId && t3.TEST_SUITE_NAME == TestSuitename
                            select new TestCaseListByProject
                            {
                                ProjectId = t1.PROJECT_ID,
                                ProjectName = t1.PROJECT_NAME,
                                TestcaseId = t5.TEST_CASE_ID,
                                TestcaseName = t5.TEST_CASE_NAME,
                                TestsuiteId = t3.TEST_SUITE_ID,
                                TestsuiteName = t3.TEST_SUITE_NAME,
                                TestCaseDesc = t5.TEST_STEP_DESCRIPTION,
                                DataSetCount = (long)t6.DATA_SUMMARY_ID
                            };
                var lResult = lList.Distinct().ToList();

                lResult = lResult.GroupBy(x => new { x.ProjectId, x.ProjectName, x.TestsuiteId, x.TestsuiteName, x.TestcaseId, x.TestcaseName, x.TestCaseDesc }).Select(gcs => new TestCaseListByProject()
                {
                    ProjectId = gcs.Key.ProjectId,
                    ProjectName = gcs.Key.ProjectName,
                    TestsuiteId = gcs.Key.TestsuiteId,
                    TestsuiteName = gcs.Key.TestsuiteName,
                    TestcaseId = gcs.Key.TestcaseId,
                    TestcaseName = gcs.Key.TestcaseName,
                    TestCaseDesc = gcs.Key.TestCaseDesc,
                    DataSetCount = gcs.Count()
                }).ToList();

                if (lResult.Count() > 0)
                {
                    lTestcaseTree = lResult.Distinct().OrderBy(x => x.TestcaseName).ToList();
                }
                logger.Info(string.Format("Get TestCase List end | TestSuitename: {0} | UserName: {1}", TestSuitename, Username));
                return lTestcaseTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in GetTestCaseList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in GetTestCaseList method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public List<DataSetListByTestCase> GetDataSetList(long Projectid, string TestSuitename, string Testcasename)
        {
            logger.Info(string.Format("Get DataSet List start | Projectid: {0} | TestSuitename: {1} | Testcasename: {2} | UserName: {3}", Projectid, TestSuitename, Testcasename, Username));
            try
            {
                var lDatasettree = new List<DataSetListByTestCase>();
                var lList = from t1 in enty.T_TEST_PROJECT
                            join t2 in enty.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                            join t3 in enty.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                            join t4 in enty.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                            join t5 in enty.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                            join t6 in enty.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                            join t7 in enty.T_TEST_DATA_SUMMARY on t6.DATA_SUMMARY_ID equals t7.DATA_SUMMARY_ID
                            where t1.PROJECT_ID == Projectid && t3.TEST_SUITE_NAME == TestSuitename && t5.TEST_CASE_NAME == Testcasename
                            select new DataSetListByTestCase
                            {
                                ProjectId = t1.PROJECT_ID,
                                ProjectName = t1.PROJECT_NAME,
                                TestcaseId = t5.TEST_CASE_ID,
                                TestcaseName = t5.TEST_CASE_NAME,
                                TestsuiteId = t3.TEST_SUITE_ID,
                                TestsuiteName = t3.TEST_SUITE_NAME,
                                Datasetid = t7.DATA_SUMMARY_ID,
                                Datasetname = t7.ALIAS_NAME
                            };
                var lResult = lList.Distinct().ToList();

                lResult = lResult.GroupBy(x => new { x.ProjectId, x.ProjectName, x.TestsuiteId, x.TestsuiteName, x.TestcaseId, x.TestcaseName, x.Datasetid, x.Datasetname }).Select(gcs => new DataSetListByTestCase()
                {
                    ProjectId = gcs.Key.ProjectId,
                    ProjectName = gcs.Key.ProjectName,
                    TestsuiteId = gcs.Key.TestsuiteId,
                    TestsuiteName = gcs.Key.TestsuiteName,
                    TestcaseId = gcs.Key.TestcaseId,
                    TestcaseName = gcs.Key.TestcaseName,
                    Datasetid = gcs.Key.Datasetid,
                    Datasetname = gcs.Key.Datasetname
                }).ToList();

                if (lResult.Count() > 0)
                {
                    lDatasettree = lResult.Distinct().OrderBy(x => x.TestcaseName).ToList();
                }
                logger.Info(string.Format("Get DataSet List end | Projectid: {0} | TestSuitename: {1} | Testcasename: {2} | UserName: {3}", Projectid, TestSuitename, Testcasename, Username));
                return lDatasettree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in GetDataSetList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in GetDataSetList method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string GetStoryboardById(long Storyboardid)
        {
            try
            {
                logger.Info(string.Format("Get Storyboard start | Storyboardid: {0} | UserName: {1}", Storyboardid, Username));
                var lstoryboardname = enty.T_STORYBOARD_SUMMARY.FirstOrDefault(x => x.STORYBOARD_ID == Storyboardid).STORYBOARD_NAME;
                logger.Info(string.Format("Get Storyboard end | Storyboardid: {0} | UserName: {1}", Storyboardid, Username));
                return lstoryboardname;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in storyboard GetStoryboardById method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in storyboard GetStoryboardById method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool ChangeStoryboardName(string storyboardname, string storyboarddesc, long storyboardid)
        {
            try
            {
                logger.Info(string.Format("Change StoryboardName start | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                if (!string.IsNullOrEmpty(storyboardname))
                {
                    storyboardname = storyboardname.Trim();
                }
                var lresult = false;
                var result = CheckDuplicateStoryboardName(storyboardname, storyboardid);
                if (result == true)
                {
                    logger.Info(string.Format("Change StoryboardName end | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                    return lresult;
                }
                else
                {
                    var sname = enty.T_STORYBOARD_SUMMARY.Find(storyboardid);
                    sname.STORYBOARD_NAME = storyboardname;
                    sname.DESCRIPTION = storyboarddesc;
                    enty.SaveChanges();
                    lresult = true;
                }
                logger.Info(string.Format("Change StoryboardName end | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in storyboard ChangeStoryboardName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in storyboard ChangeStoryboardName method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string GetStoryboardNamebyId(long storyboardid)
        {
            try
            {
                logger.Info(string.Format("Get StoryboardName start | storyboardid: {0} | UserName: {1}", storyboardid, Username));
                var storyboard = enty.T_STORYBOARD_SUMMARY.FirstOrDefault(x => x.STORYBOARD_ID == storyboardid).STORYBOARD_NAME;
                logger.Info(string.Format("Get StoryboardName end | storyboardid: {0} | UserName: {1}", storyboardid, Username));
                return storyboard;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in storyboard GetStoryboardNamebyId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in storyboard GetStoryboardNamebyId method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool DeleteStoryboard(long storyboardid)
        {
            try
            {
                logger.Info(string.Format("Delete Storyboard start | storyboardid: {0} | UserName: {1}", storyboardid, Username));
                var flag = false;
                var storyboard = enty.T_STORYBOARD_SUMMARY.FirstOrDefault(x => x.STORYBOARD_ID == storyboardid);

                if (storyboard != null)
                {

                    var proj = enty.T_PROJ_TC_MGR.Where(x => x.STORYBOARD_ID == storyboard.STORYBOARD_ID).ToList();

                    foreach (var a in proj)
                    {
                        var result = enty.T_STORYBOARD_DATASET_SETTING.Where(x => x.STORYBOARD_DETAIL_ID == a.STORYBOARD_DETAIL_ID).ToList();
                        foreach (var item in result)
                        {
                            enty.T_STORYBOARD_DATASET_SETTING.Remove(item);
                            enty.SaveChanges();
                        }
                        enty.T_PROJ_TC_MGR.Remove(a);
                        enty.SaveChanges();
                    }

                    var shistory = enty.T_STORYBOARD_HISTORY.Where(x => x.STORYBOARD_ID == storyboard.STORYBOARD_ID).ToList();

                    foreach (var r in shistory)
                    {
                        enty.T_STORYBOARD_HISTORY.Remove(r);
                        enty.SaveChanges();
                    }

                    enty.T_STORYBOARD_SUMMARY.Remove(storyboard);
                    enty.SaveChanges();
                    flag = true;
                }
                logger.Info(string.Format("Delete Storyboard end | storyboardid: {0} | UserName: {1}", storyboardid, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in storyboard DeleteStoryboard method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in storyboard DeleteStoryboard method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool CheckDuplicateStoryboardName(string storyboardname, long? storyboardid)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate StoryboardName start | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                var lresult = false;
                if (storyboardid != null)
                {
                    lresult = enty.T_STORYBOARD_SUMMARY.Any(x => x.STORYBOARD_ID != storyboardid && x.STORYBOARD_NAME.ToLower().Trim() == storyboardname.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_STORYBOARD_SUMMARY.Any(x => x.STORYBOARD_NAME.ToLower().Trim() == storyboardname.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate StoryboardName end | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Storyboard CheckDuplicateStoryboardName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Storyboard CheckDuplicateStoryboardName method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<TestResultModel> GetTestResult(long TestCaseId, long SBDetailId)
        {
            try
            {
                logger.Info(string.Format("Get TestResult start | TestCaseId: {0} | SBDetailId: {1} | UserName: {2}", TestCaseId, SBDetailId, Username));
                List<TestResultModel> listresult = new List<TestResultModel>();
                var lList = enty.T_PROJ_TEST_RESULT.Where(x => x.TEST_CASE_ID == TestCaseId && x.STORYBOARD_DETAIL_ID == SBDetailId).Select(y => new TestResultModel
                {
                    BeginTime = y.TEST_BEGIN_TIME,
                    EndTime = y.TEST_END_TIME,
                    CreatTime = y.CREATE_TIME,
                    HistId = y.HIST_ID,
                    ResultAliasName = y.RESULT_ALIAS_NAME,
                    ResultDesc = y.RESULT_DESC,
                    StoryboardDetailId = y.STORYBOARD_DETAIL_ID,
                    TestCaseId = y.TEST_CASE_ID,
                    TestModeId = y.TEST_MODE,
                    TestMode = y.TEST_MODE == 1 ? "BaseLine" : "Comparison",
                    TestResult = y.TEST_RESULT,
                    TestResultInText = y.TEST_RESULT_IN_TEXT,
                    LatestTestMarkId = y.LATEST_TEST_MARK_ID

                }).OrderBy(z => z.HistId).ToList();

                lList.ForEach(item =>
                {
                    item.ResultAliasName = item.ResultAliasName == null || item.ResultAliasName == "null" ? "" : item.ResultAliasName;
                    item.ResultDesc = item.ResultDesc == null || item.ResultDesc == "null" ? "" : item.ResultDesc;
                });
                var maxBaselineMarkIds = lList.Where(x => x.TestModeId == 1).ToList();
                var maxCompareMarkIds = lList.Where(x => x.TestModeId != 1).ToList();
                long maxBaselineMarkId = 0;
                long maxCompareMarkId = 0;
                if (maxBaselineMarkIds.Count() > 0)
                {
                    maxBaselineMarkId = (long)(maxBaselineMarkIds.Max(x => x.LatestTestMarkId));
                    //lList.Where(x => x.LatestTestMarkId == maxBaselineMarkId && x.TestModeId == 1).ToList().ForEach(item =>
                    //{
                    //    item.IsMark = true;
                    //});

                    var maxHistId = lList.Where(x => x.LatestTestMarkId == maxBaselineMarkId && x.TestModeId == 1).Max(y => y.HistId);
                    lList.Where(x => x.LatestTestMarkId == maxBaselineMarkId && x.TestModeId == 1 && x.HistId == maxHistId).ToList().ForEach(item =>
                    {
                        item.IsMark = true;
                    });
                }
                if (maxCompareMarkIds.Count() > 0)
                {
                    maxCompareMarkId = (long)(maxCompareMarkIds.Max(x => x.LatestTestMarkId));
                    //lList.Where(x => x.LatestTestMarkId == maxCompareMarkId && x.TestModeId != 1).ToList().ForEach(item =>
                    //{
                    //    item.IsMark = true;
                    //});
                    var maxHistId = lList.Where(x => x.LatestTestMarkId == maxCompareMarkId && x.TestModeId != 1).Max(y => y.HistId);
                    lList.Where(x => x.LatestTestMarkId == maxCompareMarkId && x.TestModeId != 1 && x.HistId == maxHistId).ToList().ForEach(item =>
                    {
                        item.IsMark = true;
                    });
                }
                logger.Info(string.Format("Get TestResult end | TestCaseId: {0} | SBDetailId: {1} | UserName: {2}", TestCaseId, SBDetailId, Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Storyboard GetTestResult method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Storyboard GetTestResult method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public IList<TestResultViewModel> GetCompareResultList(string schema, string lconstring, long BhistedId, long ChistedId)
        {
            try
            {
                logger.Info(string.Format("Get Compare stroyborad ResultList start | baseline histid: {0} | compare histid: {1} | UserName: {2}", BhistedId, ChistedId, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;


                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("Compare_HISTID", OracleDbType.Long);
                ladd_refer_image[0].Value = ChistedId;

                ladd_refer_image[1] = new OracleParameter("Baseline_HISTID", OracleDbType.Long);
                ladd_refer_image[1].Value = BhistedId;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_STORYBOARD_RESULT";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                IList<TestResultViewModel> result = dt.AsEnumerable().Select(row =>
                  new TestResultViewModel
                  {
                      StepId = row.Field<long?>("STEPS_ID"),
                      BaselineStepId = row.Field<long?>("BaselineStepID"),
                      CompareStepId = row.Field<long?>("CompareStepID"),
                      BaselineReportId = row.Field<long?>("BaselineReportID"),
                      CompareReportId = row.Field<long?>("CompareReportID"),
                      BeginTime = row.Field<DateTime?>("BEGIN_TIME"),
                      EndTime = row.Field<DateTime?>("END_TIME"),
                      RunningResult = row.Field<short?>("RUNNING_RESULT"),
                      BreturnValues = row.Field<string>("Baseline_RETURN_VALUES"),
                      CreturnValues = row.Field<string>("Compare_RETURN_VALUES"),
                      RunningResultInfo = row.Field<string>("RUNNING_RESULT_INFO"),
                      InputValueSetting = row.Field<string>("INPUT_VALUE_SETTING"),
                      Keyword = row.Field<string>("key_word_name"),
                      ActualInputData = row.Field<string>("ACTUAL_INPUT_DATA"),
                      DataOrder = row.Field<long?>("DATA_ORDER"),
                      InfoPic = row.Field<string>("INFO_PIC"),
                      Advice = row.Field<string>("ADVICE"),
                      Stackinfo = row.Field<string>("STACKINFO"),
                      BaselineComment = row.Field<string>("BaselineComment"),
                      CompareComment = row.Field<string>("CompareComment"),
                      COMMENT = row.Field<string>("COMMENT")
                  }).ToList();

                result.ToList().ForEach(item =>
                {
                    item.StepId = item.StepId == null ? 0 : item.StepId;
                    item.BaselineStepId = item.BaselineStepId == null ? 0 : item.BaselineStepId;
                    item.CompareStepId = item.CompareStepId == null ? 0 : item.CompareStepId;
                    item.BaselineReportId = item.BaselineReportId == null ? 0 : item.BaselineReportId;
                    item.CompareReportId = item.CompareReportId == null ? 0 : item.CompareReportId;
                    item.BreturnValues = item.BreturnValues == null ? "" : item.BreturnValues.Trim();
                    item.CreturnValues = item.CreturnValues == null ? "" : item.CreturnValues.Trim();
                    item.RunningResultInfo = item.RunningResultInfo == null ? "" : item.RunningResultInfo.Trim();
                    item.InputValueSetting = item.InputValueSetting == null ? "" : item.InputValueSetting.Trim();
                    item.ActualInputData = item.ActualInputData == null ? "" : item.ActualInputData.Trim();
                    item.InfoPic = item.InfoPic == null ? "" : item.InfoPic.Trim();
                    item.Advice = item.Advice == null ? "" : item.Advice.Trim();
                    item.Stackinfo = item.Stackinfo == null ? "" : item.Stackinfo.Trim();
                    item.BaselineComment = item.BaselineComment == null ? "" : item.BaselineComment.Trim();
                    item.CompareComment = item.CompareComment == null ? "" : item.CompareComment.Trim();
                    item.COMMENT = item.COMMENT == null ? "" : item.COMMENT.Trim();

                    if (item.Keyword == "CaptureValue")
                        item.Result = "";
                    else if (item.COMMENT.Contains("TOL:"))
                    {
                        var splitTOL = item.COMMENT.Split(' ');
                        var lfun = splitTOL[0].Trim();
                        var lparameter = splitTOL[1].Trim();

                        bool flagDP = decimal.TryParse(lparameter, out decimal i);
                        bool flagDB = decimal.TryParse(item.BreturnValues, out decimal j);
                        bool flagDC = decimal.TryParse(item.CreturnValues, out decimal k);

                        bool flagIP = int.TryParse(lparameter, out int ii);
                        bool flagIB = int.TryParse(item.BreturnValues, out int jj);
                        bool flagIC = int.TryParse(item.CreturnValues, out int kk);

                        if (lfun.Contains("TOL_COMPARE") && (flagDP || flagIP) && (flagDB || flagIB) && (flagDC || flagIC))
                        {
                            item.BreturnValues = item.BreturnValues.Replace(",", "");
                            item.CreturnValues = item.CreturnValues.Replace(",", "");
                            lparameter = lparameter.Replace(",", "");
                            try
                            {
                                if (Math.Abs((Convert.ToDecimal(item.BreturnValues) - Convert.ToDecimal(item.CreturnValues))) < Convert.ToDecimal(lparameter))
                                {
                                    item.Result = "TRUE";
                                }
                                else
                                {
                                    item.Result = "FALSE";
                                }
                            }
                            catch(Exception ex)
                            {
                                item.Result = "FALSE";
                                logger.Error(string.Format("Tolerance value has not propered: Baseline value: {0} | Compare Value: {1} | Parameter value: {2}", item.BreturnValues, item.CreturnValues, lparameter));
                                ELogger.ErrorException(string.Format("Error occured Tolerance value has not propered: Baseline value: {0} | Compare Value: {1} | Parameter value: {2}", item.BreturnValues, item.CreturnValues, lparameter), ex);
                            }
                        }
                        else
                        {
                            item.Result = item.BreturnValues == item.CreturnValues ? "TRUE" : "FALSE";
                        }
                    }
                    else
                        item.Result = item.BreturnValues == item.CreturnValues ? "TRUE" : "FALSE";
                });
                logger.Info(string.Format("Get Compare stroyborad ResultList end | baseline histid: {0} | compare histid: {1} | UserName: {2}", BhistedId, ChistedId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in GetCompareResultList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in GetCompareResultList method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public long CreateTestReportId(long TestCaseId, long StoryboardDetailId, Int16 Mode, out long hist_id)
        {
            try
            {
                logger.Info(string.Format("Create TestReport Id start | TestCaseId: {0} | StoryboardDetailId: {1} | Mode: {2} | UserName: {3}", TestCaseId, StoryboardDetailId, Mode, Username));
                //create T_PROJ_TEST_RESULT
                hist_id = Helper.NextTestSuiteId("SEQ_TESTRESULT_ID");
                var tblPTR = new T_PROJ_TEST_RESULT();
                tblPTR.HIST_ID = hist_id;
                tblPTR.TEST_CASE_ID = TestCaseId;
                tblPTR.CREATE_TIME = DateTime.Now;
                tblPTR.LATEST_TEST_MARK_ID = Helper.NextTestSuiteId("SEQ_TESTRESULT_ID");
                tblPTR.STORYBOARD_DETAIL_ID = StoryboardDetailId;
                tblPTR.TEST_MODE = Mode;
                tblPTR.TEST_RESULT_IN_TEXT = "SUCCESS";
                tblPTR.RESULT_DESC = "Manual";
                tblPTR.TEST_RESULT = 1;
                enty.T_PROJ_TEST_RESULT.Add(tblPTR);
                enty.SaveChanges();

                var tblreport = new T_TEST_REPORT();
                var Id = Helper.NextTestSuiteId("SEQ_TESTRESULT_ID");
                tblreport.TEST_REPORT_ID = Id;
                tblreport.TEST_CASE_ID = TestCaseId;
                tblreport.LOOP_ID = 1;
                tblreport.HIST_ID = tblPTR.HIST_ID;
                tblreport.TEST_MODE = Mode;
                enty.T_TEST_REPORT.Add(tblreport);
                enty.SaveChanges();
                logger.Info(string.Format("Create TestReport Id end | TestCaseId: {0} | StoryboardDetailId: {1} | Mode: {2} | UserName: {3}", TestCaseId, StoryboardDetailId, Mode, Username));
                return Id;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in GetCompareResultList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in GetCompareResultList method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool SaveAsResultSet(long BhistedId, long ChistedId)
        {
            try
            {
                logger.Info(string.Format("SaveAs ResultSet start | baseline histid: {0} | compare histid: {1} | UserName: {2}", BhistedId, ChistedId, Username));
                var result = enty.SP_SAVEAS_RESULTSET(ChistedId, BhistedId);
                logger.Info(string.Format("SaveAs ResultSet end | baseline histid: {0} | compare histid: {1} | UserName: {2}", BhistedId, ChistedId, Username));
                enty.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in SaveAsResultSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in SaveAsResultSet method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool updateresults(string constring, string schema, string feedprocessdetailid)
        {
            try
            {
                logger.Info(string.Format("update results start | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                var result = enty.SP_SAVE_SB_RESULTS(feedprocessdetailid);
                enty.SaveChanges();
                logger.Info(string.Format("update results end | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyborad in updateresults method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyborad in updateresults method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool InsertResultsInStgTbl(string constring, string schema, DataTable dt, long feedprocessdetailid)
        {
            logger.Info(string.Format("Insert ResultsIn StgTbl start | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
            bool flag = false;
            OracleTransaction ltransaction;

            OracleConnection lconnection = new OracleConnection(constring);
            lconnection.Open();
            ltransaction = lconnection.BeginTransaction();
            string lcmdquery = "insert into TBLSTGSBRESULT ( FEEDPROCESSDETAILID,BASELINEID,COMPAREID,BASELINEVALUE,COMPAREVALUE,INPUTVALUESETTING,BASELINECOMMENT,COMPARECOMMENT,BASELINEREPORTID,COMPAREREPORTID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10)";
            int[] ids = new int[dt.Rows.Count];
            using (var lcmd = lconnection.CreateCommand())
            {
                lcmd.CommandText = lcmdquery;
                lcmd.ArrayBindCount = ids.Length;
                string[] FEEDPROCESSDETAILID_param = dt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                string[] BASELINEID_param = dt.AsEnumerable().Select(r => r.Field<string>("BASELINEID")).ToArray();
                string[] COMPAREID_param = dt.AsEnumerable().Select(r => r.Field<string>("COMPAREID")).ToArray();
                string[] BASEVALUE_param = dt.AsEnumerable().Select(r => r.Field<string>("BASELINEVALUE")).ToArray();
                string[] COMPVALUE_param = dt.AsEnumerable().Select(r => r.Field<string>("COMPAREVALUE")).ToArray();
                string[] INPUTVALUE_param = dt.AsEnumerable().Select(r => r.Field<string>("INPUTVALUESETTING")).ToArray();
                string[] BASELINECOMMENT_param = dt.AsEnumerable().Select(r => r.Field<string>("BASELINECOMMENT")).ToArray();
                string[] COMPARECOMMENT_param = dt.AsEnumerable().Select(r => r.Field<string>("COMPARECOMMENT")).ToArray();
                string[] BASELINEREPORTID_param = dt.AsEnumerable().Select(r => r.Field<string>("BASELINEREPORTID")).ToArray();
                string[] COMPAREREPORTID_param = dt.AsEnumerable().Select(r => r.Field<string>("COMPAREREPORTID")).ToArray();

                OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                OracleParameter BASELINEID_oparam = new OracleParameter();
                BASELINEID_oparam.OracleDbType = OracleDbType.Varchar2;
                BASELINEID_oparam.Value = BASELINEID_param;

                OracleParameter COMPAREID_oparam = new OracleParameter();
                COMPAREID_oparam.OracleDbType = OracleDbType.Varchar2;
                COMPAREID_oparam.Value = COMPAREID_param;

                OracleParameter BASEVALUE_oparam = new OracleParameter();
                BASEVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                BASEVALUE_oparam.Value = BASEVALUE_param;

                OracleParameter COMPVALUE_oparam = new OracleParameter();
                COMPVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                COMPVALUE_oparam.Value = COMPVALUE_param;

                OracleParameter INPUTVALUE_oparam = new OracleParameter();
                INPUTVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                INPUTVALUE_oparam.Value = INPUTVALUE_param;

                OracleParameter BASELINECOMMENT_oparam = new OracleParameter();
                BASELINECOMMENT_oparam.OracleDbType = OracleDbType.Varchar2;
                BASELINECOMMENT_oparam.Value = BASELINECOMMENT_param;

                OracleParameter COMPARECOMMENT_oparam = new OracleParameter();
                COMPARECOMMENT_oparam.OracleDbType = OracleDbType.Varchar2;
                COMPARECOMMENT_oparam.Value = COMPARECOMMENT_param;

                OracleParameter BASELINEREPORTID_oparam = new OracleParameter();
                BASELINEREPORTID_oparam.OracleDbType = OracleDbType.Varchar2;
                BASELINEREPORTID_oparam.Value = BASELINEREPORTID_param;

                OracleParameter COMPAREREPORTID_oparam = new OracleParameter();
                COMPAREREPORTID_oparam.OracleDbType = OracleDbType.Varchar2;
                COMPAREREPORTID_oparam.Value = COMPAREREPORTID_param;

                lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                lcmd.Parameters.Add(BASELINEID_oparam);
                lcmd.Parameters.Add(COMPAREID_oparam);
                lcmd.Parameters.Add(BASEVALUE_oparam);
                lcmd.Parameters.Add(COMPVALUE_oparam);
                lcmd.Parameters.Add(INPUTVALUE_oparam);
                lcmd.Parameters.Add(BASELINECOMMENT_oparam);
                lcmd.Parameters.Add(COMPARECOMMENT_oparam);
                lcmd.Parameters.Add(BASELINEREPORTID_oparam);
                lcmd.Parameters.Add(COMPAREREPORTID_oparam);
                try
                {
                    lcmd.ExecuteNonQuery();
                    flag = true;
                }
                catch (Exception lex)
                {
                    flag = false;
                    ltransaction.Rollback();
                    logger.Error(string.Format("Error occured Storyborad in InsertResultsInStgTbl method | UserName: {0}", Username));
                    ELogger.ErrorException(string.Format("Error occured Storyborad in InsertResultsInStgTbl method | UserName: {0}", Username), lex);
                    throw;
                }
                logger.Info(string.Format("Insert ResultsIn StgTbl end | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                ltransaction.Commit();
                lconnection.Close();
            }
            return flag;
        }
        public List<StoryBoardResultModel> OldCheckSBGridValidation(List<StoryBoardResultModel> lGridList, long lStoryboardId)
        {
            try
            {
                logger.Info(string.Format("OldCheckSBGridValidation start | StoryboardId: {0} | Username: {1}", lStoryboardId, Username));
                int i = 0;
                var lStepsList = new List<string>();
                var StepsList = new List<string>();
                var lProjectId = enty.T_STORYBOARD_SUMMARY.Where(x => x.STORYBOARD_ID == lStoryboardId).FirstOrDefault().ASSIGNED_PROJECT_ID;
                foreach (StoryBoardResultModel item in lGridList)
                {
                    var lflag = false;
                    var lValidationMsg = "";

                    item.RowId = i;
                    i++;
                    if (item.ActionName != "EXECUTE" && item.ActionName != "RUN" && item.ActionName != "SKIP" && item.ActionName != "DONE")
                    {
                        lflag = true;
                        lValidationMsg = "Invalid Action.";
                    }
                    //check TestSuite Exist or not
                    var lTSExist = enty.T_TEST_SUITE.Where(x => x.TEST_SUITE_NAME.ToUpper().Trim() == item.TestSuiteName.ToUpper().Trim());
                    if (lTSExist.Count() == 0)
                    {
                        lflag = true;
                        lValidationMsg = lValidationMsg + " Test Suite " + item.TestSuiteName + " does not exist.";
                    }

                    //check TestCase Exist or not
                    var lTCExist = enty.T_TEST_CASE_SUMMARY.Where(x => x.TEST_CASE_NAME.ToUpper().Trim() == item.TestCaseName.ToUpper().Trim());
                    if (lTCExist.Count() == 0)
                    {
                        lflag = true;
                        lValidationMsg = lValidationMsg + " Test Case " + item.TestCaseName + " does not exist";
                    }

                    //check Dataset Exist or not
                    var lDatasetExist = enty.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME.ToUpper().Trim() == item.DataSetName.ToUpper().Trim());
                    if (lDatasetExist.Count() == 0)
                    {
                        lflag = true;
                        lValidationMsg = lValidationMsg + " Dataset " + item.DataSetName + " does not exist";
                    }

                    item.StepName = item.StepName == null ? "" : item.StepName;
                    StepsList.Add(item.StepName.Trim());
                    if (!string.IsNullOrEmpty(item.StepName))
                    {
                        lStepsList.Add(item.StepName.Trim());
                        item.StepName = string.IsNullOrEmpty(item.StepName) ? "" : item.StepName;

                        var lStepNameDuplicate = lStepsList.Count(str => item.StepName.Trim() == str);

                        if (lStepNameDuplicate > 1)
                        {
                            lflag = true;
                            lValidationMsg = lValidationMsg + " Duplicate Step names are not allowed.";
                        }
                    }


                    var lTestCaseIds = new List<long>();
                    var lTestSuiteIds = new List<long>();
                    var lTestDataSetIds = new List<long>();

                    if (lTCExist.Count() > 0)
                    {
                        //lTestCaseId = lTCExist.FirstOrDefault().TEST_CASE_ID;
                        lTestCaseIds = lTCExist.Select(x => x.TEST_CASE_ID).ToList();
                    }

                    if (lTSExist.Count() > 0)
                    {
                        //lTestSuiteId = lTSExist.FirstOrDefault().TEST_SUITE_ID;
                        lTestSuiteIds = lTSExist.Select(x => x.TEST_SUITE_ID).ToList();
                    }

                    if (lDatasetExist.Count() > 0)
                    {
                        lTestDataSetIds = lDatasetExist.Select(x => x.DATA_SUMMARY_ID).ToList();
                    }


                    if (lTSExist.Count() > 0)
                    {
                        var lRelProjTestSuite = enty.REL_TEST_SUIT_PROJECT.Any(x => x.PROJECT_ID == lProjectId && lTestSuiteIds.Contains((long)x.TEST_SUITE_ID));
                        if (!lRelProjTestSuite)
                        {
                            lflag = true;
                            lValidationMsg = lValidationMsg + "Test Suite and Storyboard should be in same Project.";
                        }
                    }

                    if (lTCExist.Count() > 0 && lTSExist.Count() > 0)
                    {
                        var lRelTCTSExist = enty.REL_TEST_CASE_TEST_SUITE.Where(x => lTestCaseIds.Contains((long)x.TEST_CASE_ID) &&
                            lTestSuiteIds.Contains((long)x.TEST_SUITE_ID)).ToList();
                        if (lRelTCTSExist.Count() == 0)
                        {
                            lflag = true;
                            lValidationMsg = lValidationMsg + " Test Suite [" + item.TestSuiteName + "] does not contain Test Case [" + item.TestCaseName + "].";
                        }
                    }

                    if (lDatasetExist.Count() > 0 && lTSExist.Count() > 0)
                    {
                        var lRelTCDSExist = enty.REL_TC_DATA_SUMMARY.Where(x => lTestCaseIds.Contains((long)x.TEST_CASE_ID) &&
                            lTestDataSetIds.Contains((long)x.DATA_SUMMARY_ID)).ToList();
                        if (lRelTCDSExist.Count() == 0)
                        {

                            lflag = true;
                            lValidationMsg = lValidationMsg + " Test Case [" + item.TestCaseName + "] does not contain DataSet [" + item.DataSetName + "].";
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Dependency) && item.Dependency.ToUpper() != "NONE")
                    {
                        var lDependencyValid = StepsList.Count(str => str.Contains(item.Dependency));
                        int stplistcount = StepsList.Count - 1;
                        for (int j = 0; j <= stplistcount; j++)
                        {
                            if (j == stplistcount && StepsList[j] == item.Dependency && j == item.RowId)
                            {
                                lflag = true;
                                lValidationMsg = lValidationMsg + "Dependency value does not exist in the list.";
                            }
                        }
                        if (lDependencyValid == 0)
                        {
                            lflag = true;
                            lValidationMsg = lValidationMsg + " Dependency " + item.Dependency + " is not valid.";
                        }
                    }

                    if (lflag)
                    {
                        item.IsValid = false;
                        item.ValidationMsg = lValidationMsg;
                    }
                    else
                    {
                        item.IsValid = true;
                    }
                }
                logger.Info(string.Format("OldCheckSBGridValidation end | StoryboardId: {0} | Username: {1}", lStoryboardId, Username));
                return lGridList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad OldCheckSBGridValidation method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad OldCheckSBGridValidation method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<StoryBoardResultModel> CheckSBGridValidation(List<StoryBoardResultModel> lGridList, long lStoryboardId)
        {
            try
            {
                logger.Info(string.Format("Check storyborad Grid Validation start | StoryboardId: {0} | UserName: {1}", lStoryboardId, Username));
                int i = 0;
                var lStepsList = new List<string>();
                var StepsList = new List<string>();
                var lProjectId = enty.T_STORYBOARD_SUMMARY.Where(x => x.STORYBOARD_ID == lStoryboardId).FirstOrDefault().ASSIGNED_PROJECT_ID;

                var lInValidActionList = lGridList.Where(x => x.ActionName != "EXECUTE" && x.ActionName != "RUN" && x.ActionName != "SKIP" && x.ActionName != "DONE").ToList();
                if (lInValidActionList.Count() > 0)
                {
                    lInValidActionList.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Invalid Action. "; x.IsValid = false; });
                }

                var lInValidTSList = lGridList.Select(y => y.TestSuiteName.ToUpper().Trim()).Except(enty.T_TEST_SUITE.Select(x => x.TEST_SUITE_NAME.ToUpper().Trim())).ToList();
                if (lInValidTSList.Count() > 0)
                {
                    var lInValidTS = lGridList.Where(x => lInValidTSList.Contains(x.TestSuiteName.ToUpper().Trim())).ToList();
                    lInValidTS.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Test Suite [" + lInValidTS.FirstOrDefault().TestSuiteName + "] does not exist. "; x.IsValid = false; });
                }

                var lInValidTCList = lGridList.Select(y => y.TestCaseName.ToUpper().Trim()).Except(enty.T_TEST_CASE_SUMMARY.Select(x => x.TEST_CASE_NAME.ToUpper().Trim())).ToList();
                if (lInValidTCList.Count() > 0)
                {
                    var lInValidTC = lGridList.Where(x => lInValidTCList.Contains(x.TestCaseName.ToUpper().Trim())).ToList();
                    lInValidTC.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Test Case [" + lInValidTC.FirstOrDefault().TestCaseName + "] does not exist. "; x.IsValid = false; });
                }

                var lInValidDSList = lGridList.Select(y => y.DataSetName.ToUpper().Trim()).Except(enty.T_TEST_DATA_SUMMARY.Select(x => x.ALIAS_NAME.ToUpper().Trim())).ToList();
                if (lInValidDSList.Count() > 0)
                {
                    var lInValidDS = lGridList.Where(x => lInValidDSList.Contains(x.DataSetName.ToUpper().Trim())).ToList();
                    lInValidDS.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Dataset [" + lInValidDS.FirstOrDefault().DataSetName + "] does not exist. "; x.IsValid = false; });
                }

                var lInValidStepsList = lGridList.Where(x => x.StepName != null && x.StepName != "").GroupBy(s => s.StepName)
                                 .Where(g => g.Count() > 1)
                                 .Select(g => g.Key);
                if (lInValidStepsList.Count() > 0)
                {
                    var lInValidStesps = lGridList.Where(x => lInValidStepsList.Contains(x.StepName)).ToList();
                    lInValidStesps.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Duplicate Step names are not allowed. "; x.IsValid = false; });
                }

                //TestSuite and Storyboard should be in same Project. 
                var lMathingTSList = lGridList.Select(x => x.TestSuiteName.ToUpper().Trim()).Intersect(enty.T_TEST_SUITE.Select(x => x.TEST_SUITE_NAME.ToUpper().Trim())).ToList();
                var lMatchTSIdList = enty.T_TEST_SUITE.Where(y => lMathingTSList.Contains(y.TEST_SUITE_NAME.ToUpper().Trim())).ToList();

                var lInValidRelProjTestSuite = lMatchTSIdList.Select(y => y.TEST_SUITE_ID).Except(enty.REL_TEST_SUIT_PROJECT.Where(z => z.PROJECT_ID == lProjectId).Select(x => (long)x.TEST_SUITE_ID));
                var lInValidTSPrjList = lMatchTSIdList.Where(x => lInValidRelProjTestSuite.Contains(x.TEST_SUITE_ID)).Select(y => y.TEST_SUITE_NAME.ToUpper().Trim()).ToList();

                if (lInValidTSPrjList.Count() > 0)
                {
                    var lInValidTSPrj = lGridList.Where(x => lInValidTSPrjList.Contains(x.TestSuiteName.ToUpper().Trim())).ToList();
                    lInValidTSPrj.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Test Suite and Storyboard should be in same Project. "; x.IsValid = false; });
                }

                var lMatchingTSTCList = (from u in lGridList
                                         join tc in enty.T_TEST_CASE_SUMMARY on u.TestCaseName.ToUpper().Trim() equals tc.TEST_CASE_NAME.ToUpper().Trim()
                                         join ts in enty.T_TEST_SUITE on u.TestSuiteName.ToUpper().Trim() equals ts.TEST_SUITE_NAME.ToUpper().Trim()
                                         select new
                                         {
                                             u.RowId,
                                             ts.TEST_SUITE_ID,
                                             tc.TEST_CASE_ID,
                                             ts.TEST_SUITE_NAME,
                                             tc.TEST_CASE_NAME//,
                                         }).Distinct().ToList();

                foreach (var item in lMatchingTSTCList)
                {

                    var lValid = enty.REL_TEST_CASE_TEST_SUITE.Where(x => x.TEST_SUITE_ID == item.TEST_SUITE_ID &&
                                     x.TEST_CASE_ID == item.TEST_CASE_ID).ToList();
                    if (lValid.Count() == 0)
                    {
                        var lInValidTSTC = lGridList.Where(x => x.TestSuiteName.ToUpper().Trim() == item.TEST_SUITE_NAME.ToUpper().Trim()
                        && x.TestCaseName.ToUpper().Trim() == item.TEST_CASE_NAME.ToUpper().Trim() && x.RowId == item.RowId).ToList();
                        lInValidTSTC.ForEach(x => { x.ValidationMsg = x.ValidationMsg + " Test Suite [" + item.TEST_SUITE_NAME + "] does not contain Test Case [" + item.TEST_CASE_NAME + "]. "; x.IsValid = false; });
                    }
                }
                var lDataSummaryList = enty.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME != null).ToList();
                var lMatchingTCDSList = (from u in lGridList
                                         join tc in enty.T_TEST_CASE_SUMMARY on u.TestCaseName.ToUpper().Trim() equals tc.TEST_CASE_NAME.ToUpper().Trim()
                                         join ts in lDataSummaryList on u.DataSetName.ToUpper().Trim() equals ts.ALIAS_NAME.ToUpper().Trim()
                                         select new
                                         {
                                             u.RowId,
                                             ts.DATA_SUMMARY_ID,
                                             tc.TEST_CASE_ID,
                                             ts.ALIAS_NAME,
                                             tc.TEST_CASE_NAME//,
                                         }).Distinct().ToList();
                foreach (var item in lMatchingTCDSList)
                {

                    var lValid = enty.REL_TC_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == item.DATA_SUMMARY_ID &&
                                     x.TEST_CASE_ID == item.TEST_CASE_ID).ToList();
                    if (lValid.Count() == 0)
                    {
                        var lInValidTSTC = lGridList.Where(x => x.DataSetName.ToUpper().Trim() == item.ALIAS_NAME.ToUpper().Trim()
                        && x.TestCaseName.ToUpper().Trim() == item.TEST_CASE_NAME.ToUpper().Trim() && x.RowId == item.RowId).ToList();
                        lInValidTSTC.ForEach(x => { x.ValidationMsg = x.ValidationMsg + " Test Case [" + item.TEST_CASE_NAME + "] does not contain Dataset [" + item.ALIAS_NAME + "]. "; x.IsValid = false; });
                    }
                }

                var lDependecyList = lGridList.Where(x => x.Dependency != null && x.Dependency != "" && x.Dependency.ToUpper().Trim() != "NONE").ToList();
                lGridList.Where(y => y.StepName == null).ToList().ForEach(x => { x.StepName = ""; });
                var lValidDepedenctList = (from parents in lGridList
                                           from all in lDependecyList
                                           where
                              parents.RowId < all.RowId
                                && parents.StepName.ToUpper().Trim() == all.Dependency.ToUpper().Trim()
                                           select all).ToList();

                var lInValidDepedencyList = lDependecyList.Except(lValidDepedenctList).ToList();

                if (lInValidDepedencyList.Count() > 0)
                {
                    lInValidDepedencyList.ForEach(x => { x.ValidationMsg = x.ValidationMsg + "Dependency [" + lInValidDepedencyList.FirstOrDefault().Dependency + "] is not valid. "; x.IsValid = false; });
                }
                logger.Info(string.Format("Check storyborad Grid Validation end | StoryboardId: {0} | UserName: {1}", lStoryboardId, Username));
                return lGridList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in CheckSBGridValidation method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in CheckSBGridValidation method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool CheckDuplicateStoryboardNameSaveAs(string storyboardname, long storyboardid)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate StoryboardName SaveAs start | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                var lresult = false;
                if (storyboardid != 0)
                {
                    lresult = enty.T_STORYBOARD_SUMMARY.Any(x => x.STORYBOARD_NAME.ToLower().Trim() == storyboardname.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate StoryboardName SaveAs end | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in CheckDuplicateStoryboardNameSaveAs method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in CheckDuplicateStoryboardNameSaveAs method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool DeleteResultsetstep(List<long> list)
        {
            try
            {
                logger.Info(string.Format("Delete Resultset step start | UserName: {0}", Username));
                var flag = false;

                foreach (var id in list)
                {
                    var result = enty.T_TEST_REPORT_STEPS.FirstOrDefault(x => x.TEST_REPORT_STEP_ID == id);
                    if (result != null)
                    {
                        enty.T_TEST_REPORT_STEPS.Remove(result);
                        enty.SaveChanges();
                    }
                    var obj = enty.REL_TEST_REPORT_STEPS_COMMENT.FirstOrDefault(x => x.TEST_REPORT_STEP_ID == id);
                    if (obj != null)
                    {
                        enty.REL_TEST_REPORT_STEPS_COMMENT.Remove(obj);
                        enty.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("Delete Resultset step end | UserName: {0}", Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in DeleteResultsetstep method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in DeleteResultsetstep method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void NewSaveAsStoryboardGrid(List<StoryBoardResultModel> lModel, long lStoryboardId)
        {
            try
            {
                logger.Info(string.Format("New SaveAs StoryboardGrid start | StoryboardId: {0} | UserName: {1}", lStoryboardId, Username));
                var lProjTCMGRList = enty.T_PROJ_TC_MGR.Where(x => x.STORYBOARD_ID == lStoryboardId).ToList();
                var lRunTypeList = enty.SYSTEM_LOOKUP.Where(x => x.FIELD_NAME == "RUN_TYPE" && x.TABLE_NAME == "T_PROJ_TC_MGR").ToList();
                var ltblStoryboard = enty.T_STORYBOARD_SUMMARY.Find(lStoryboardId);
                var lDataSummaryList = enty.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME != null).ToList();

                var lUpdateSB = (from grid in lModel
                                 join ts in enty.T_TEST_SUITE on grid.TestSuiteName.ToUpper().Trim() equals ts.TEST_SUITE_NAME.ToUpper().Trim()
                                 join tc in enty.T_TEST_CASE_SUMMARY on grid.TestCaseName.ToUpper().Trim() equals tc.TEST_CASE_NAME.ToUpper().Trim()
                                 join ds in lDataSummaryList on grid.DataSetName.ToUpper().Trim() equals ds.ALIAS_NAME.ToUpper().Trim()
                                 join type in lRunTypeList on grid.ActionName.ToUpper().Trim() equals type.DISPLAY_NAME.ToUpper().Trim()
                                 where grid.storyboarddetailid > 0
                                 select new
                                 {
                                     grid.storyboarddetailid,
                                     ts.TEST_SUITE_ID,
                                     tc.TEST_CASE_ID,
                                     grid.RowId,
                                     type.VALUE,
                                     grid.StepName,
                                     ds.DATA_SUMMARY_ID,
                                     grid.Dependency
                                 }).ToList();

                var lSBUpdate = (from tbl in enty.T_PROJ_TC_MGR.AsEnumerable()
                                 join up in lUpdateSB on tbl.STORYBOARD_DETAIL_ID equals (long)up.storyboarddetailid
                                 where tbl.STORYBOARD_ID == lStoryboardId
                                 select new { db = tbl, update = up });

                var lSBDataSetting = (from tbl in enty.T_STORYBOARD_DATASET_SETTING.AsEnumerable()
                                      join up in lUpdateSB on tbl.STORYBOARD_DETAIL_ID equals up.storyboarddetailid
                                      select new { db = tbl, update = up }
                                      );


                if (lSBUpdate.Count() > 0)
                {
                    lSBUpdate.ToList().ForEach(item =>
                    {
                        item.db.TEST_CASE_ID = item.update.TEST_CASE_ID; item.db.TEST_SUITE_ID = item.update.TEST_SUITE_ID;
                        item.db.RUN_ORDER = (long)item.update.RowId + 1; item.db.RUN_TYPE = item.update.VALUE; item.db.ALIAS_NAME = item.update.StepName;
                        item.db.DEPENDS_ON = (string.IsNullOrEmpty(item.update.Dependency) || item.update.Dependency.ToUpper().Trim() == "NONE") ? null : item.db.DEPENDS_ON;
                    });
                    enty.SaveChanges();
                }

                if (lSBDataSetting.Count() > 0)
                {
                    lSBDataSetting.ToList().ForEach(item =>
                    {
                        item.db.DATA_SUMMARY_ID = item.update.DATA_SUMMARY_ID;
                    });
                    enty.SaveChanges();
                }

                //Add new steps
                var lNewSB = lModel.Where(y => y.storyboarddetailid <= 0 || y.storyboarddetailid == null).ToList();

                foreach (var item in lNewSB)
                {
                    item.storyboarddetailid = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                }

                var lAddSB = (from grid in lNewSB
                              join ts in enty.T_TEST_SUITE on grid.TestSuiteName.ToUpper().Trim() equals ts.TEST_SUITE_NAME.ToUpper().Trim()
                              join tc in enty.T_TEST_CASE_SUMMARY on grid.TestCaseName.ToUpper().Trim() equals tc.TEST_CASE_NAME.ToUpper().Trim()
                              join ds in lDataSummaryList on grid.DataSetName.ToUpper().Trim() equals ds.ALIAS_NAME.ToUpper().Trim()
                              join type in lRunTypeList on grid.ActionName.ToUpper().Trim() equals type.DISPLAY_NAME.ToUpper().Trim()
                              //where grid.storyboarddetailid == 0
                              select new
                              {
                                  storyboarddetailid = (long)grid.storyboarddetailid,
                                  ts.TEST_SUITE_ID,
                                  tc.TEST_CASE_ID,
                                  grid.RowId,
                                  type.VALUE,
                                  grid.StepName,
                                  ds.DATA_SUMMARY_ID,
                                  grid.Dependency
                              }).ToList();

                var lSBAdd = (from tbl in lAddSB
                              select new T_PROJ_TC_MGR
                              {
                                  STORYBOARD_DETAIL_ID = tbl.storyboarddetailid,
                                  STORYBOARD_ID = lStoryboardId,
                                  PROJECT_ID = ltblStoryboard.ASSIGNED_PROJECT_ID,
                                  TEST_SUITE_ID = tbl.TEST_SUITE_ID,
                                  TEST_CASE_ID = tbl.TEST_CASE_ID,
                                  RUN_TYPE = tbl.VALUE,
                                  RUN_ORDER = (long)tbl.RowId + 1,
                                  LATEST_TEST_MARK_ID = null,
                                  RECORD_VERSION = null,
                                  ALIAS_NAME = tbl.StepName
                              });
                foreach (var item in lSBAdd)
                {
                    enty.T_PROJ_TC_MGR.Add(item);
                }
                enty.SaveChanges();

                var lSBDataSettingAdd = (from tbl in lAddSB
                                         select new T_STORYBOARD_DATASET_SETTING
                                         {
                                             SETTING_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ"),
                                             STORYBOARD_DETAIL_ID = tbl.storyboarddetailid,
                                             DATA_SUMMARY_ID = tbl.DATA_SUMMARY_ID
                                         });
                foreach (var item in lSBDataSettingAdd)
                {
                    enty.T_STORYBOARD_DATASET_SETTING.Add(item);
                }
                enty.SaveChanges();

                //depedency Update
                var lUpdateDPSB = (from grid in lModel
                                   join ts in enty.T_TEST_SUITE on grid.TestSuiteName.ToUpper().Trim() equals ts.TEST_SUITE_NAME.ToUpper().Trim()
                                   join tc in enty.T_TEST_CASE_SUMMARY on grid.TestCaseName.ToUpper().Trim() equals tc.TEST_CASE_NAME.ToUpper().Trim()
                                   join ds in lDataSummaryList on grid.DataSetName.ToUpper().Trim() equals ds.ALIAS_NAME.ToUpper().Trim()
                                   join type in lRunTypeList on grid.ActionName.ToUpper().Trim() equals type.DISPLAY_NAME.ToUpper().Trim()
                                   select new
                                   {
                                       grid.storyboarddetailid,
                                       ts.TEST_SUITE_ID,
                                       tc.TEST_CASE_ID,
                                       grid.RowId,
                                       type.VALUE,
                                       grid.StepName,
                                       ds.DATA_SUMMARY_ID,
                                       grid.Dependency
                                   }).ToList();


                var lDepdencyUpdateSB = lUpdateDPSB.Where(x => x.Dependency != "" && x.Dependency != null && x.Dependency.ToUpper().Trim() != "NONE").ToList();
                var ltblData = (from t in enty.T_PROJ_TC_MGR.AsEnumerable()
                                where t.STORYBOARD_ID == lStoryboardId && t.ALIAS_NAME != "" && t.ALIAS_NAME != null
                                select t).ToList();

                var lDepdencyUpdate = (from t in enty.T_PROJ_TC_MGR.AsEnumerable()
                                       join ds in lDepdencyUpdateSB on t.STORYBOARD_DETAIL_ID equals ds.storyboarddetailid
                                       join sp in ltblData on ds.Dependency.ToUpper().Trim() equals sp.ALIAS_NAME.ToUpper().Trim()
                                       select new { db = t, up = sp }).ToList();
                if (lDepdencyUpdate.Count() > 0)
                {
                    lDepdencyUpdate.ForEach(x => { x.db.DEPENDS_ON = x.up.STORYBOARD_DETAIL_ID; });
                }
                enty.SaveChanges();
                logger.Info(string.Format("New SaveAs StoryboardGrid end | StoryboardId: {0} | UserName: {1}", lStoryboardId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in NewSaveAsStoryboardGrid method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in NewSaveAsStoryboardGrid method | UserName: {0}", Username), ex);
                throw;
            }


            var lTestDataSummaryList = enty.T_TEST_DATA_SUMMARY.Where(y => y.STATUS == null).ToList();
            if (lTestDataSummaryList.Count() > 0)
            {
                lTestDataSummaryList.ForEach(x => { x.STATUS = 0; });
                enty.SaveChanges();
            }
        }

        public void SaveAsStoryboardGrid(List<StoryBoardResultModel> lModel, long lStoryboardId)
        {
            try
            {
                logger.Info(string.Format("SaveAsStoryboardGrid start | StoryboardId: {0} | Username: {1}", lStoryboardId, Username));
                var lProjTCMGRList = enty.T_PROJ_TC_MGR.Where(x => x.STORYBOARD_ID == lStoryboardId).ToList();
                var lRunTypeList = enty.SYSTEM_LOOKUP.Where(x => x.FIELD_NAME == "RUN_TYPE" && x.TABLE_NAME == "T_PROJ_TC_MGR").ToList();
                var ltblStoryboard = enty.T_STORYBOARD_SUMMARY.Find(lStoryboardId);
                int RunOrder = 1;
                foreach (StoryBoardResultModel item in lModel)
                {
                    var lTestSuite = enty.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_NAME.ToUpper().Trim() == item.TestSuiteName.ToUpper().Trim());
                    var lTestCase = enty.T_TEST_CASE_SUMMARY.FirstOrDefault(x => x.TEST_CASE_NAME.ToUpper().Trim() == item.TestCaseName.ToUpper().Trim());
                    var lDataSet = enty.T_TEST_DATA_SUMMARY.FirstOrDefault(x => x.ALIAS_NAME.ToUpper().Trim() == item.DataSetName.ToUpper().Trim());


                    if (lTestSuite != null && lTestCase != null && lDataSet != null && item.storyboarddetailid != null && item.storyboarddetailid > 0)
                    {
                        var ltblMGR = enty.T_PROJ_TC_MGR.Find(item.storyboarddetailid);
                        ltblMGR.TEST_CASE_ID = lTestCase.TEST_CASE_ID;
                        ltblMGR.TEST_SUITE_ID = lTestSuite.TEST_SUITE_ID;
                        var lDependency = enty.T_PROJ_TC_MGR.FirstOrDefault(x => x.ALIAS_NAME.ToUpper() == item.Dependency.ToUpper() && x.STORYBOARD_ID == lStoryboardId);
                        if (lDependency != null)
                        {
                            ltblMGR.DEPENDS_ON = lDependency.STORYBOARD_DETAIL_ID;
                        }
                        else
                        {
                            ltblMGR.DEPENDS_ON = null;
                        }

                        ltblMGR.RUN_ORDER = RunOrder;
                        var lRunTypeValue = lRunTypeList.FirstOrDefault(x => x.DISPLAY_NAME.ToUpper() == item.ActionName.ToUpper());
                        ltblMGR.RUN_TYPE = lRunTypeValue.VALUE;
                        ltblMGR.ALIAS_NAME = item.StepName;

                        enty.SaveChanges();

                        var ltblSBSetting = enty.T_STORYBOARD_DATASET_SETTING.FirstOrDefault(x => x.STORYBOARD_DETAIL_ID == item.storyboarddetailid);
                        ltblSBSetting.DATA_SUMMARY_ID = lDataSet.DATA_SUMMARY_ID;
                        enty.SaveChanges();
                    }
                    else
                    {
                        var tblprj = new T_PROJ_TC_MGR();
                        var lStoryboardDetailId = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblprj.STORYBOARD_DETAIL_ID = lStoryboardDetailId;
                        tblprj.STORYBOARD_ID = lStoryboardId;
                        tblprj.PROJECT_ID = ltblStoryboard.ASSIGNED_PROJECT_ID;
                        tblprj.TEST_SUITE_ID = lTestSuite.TEST_SUITE_ID;
                        tblprj.TEST_CASE_ID = lTestCase.TEST_CASE_ID;
                        var lRunTypeValue = lRunTypeList.FirstOrDefault(x => x.DISPLAY_NAME.ToUpper() == item.ActionName.ToUpper());
                        tblprj.RUN_TYPE = lRunTypeValue.VALUE;
                        tblprj.RUN_ORDER = RunOrder;
                        var lDependency = enty.T_PROJ_TC_MGR.FirstOrDefault(x => x.ALIAS_NAME.ToUpper() == item.Dependency.ToUpper() && x.STORYBOARD_ID == lStoryboardId);
                        if (lDependency != null)
                        {
                            tblprj.DEPENDS_ON = lDependency.STORYBOARD_DETAIL_ID;
                        }
                        else
                        {
                            tblprj.DEPENDS_ON = null;
                        }
                        tblprj.LATEST_TEST_MARK_ID = null;
                        tblprj.RECORD_VERSION = null;
                        tblprj.ALIAS_NAME = item.StepName;
                        enty.T_PROJ_TC_MGR.Add(tblprj);
                        enty.SaveChanges();


                        var tblsetting = new T_STORYBOARD_DATASET_SETTING();
                        tblsetting.SETTING_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblsetting.STORYBOARD_DETAIL_ID = lStoryboardDetailId;
                        tblsetting.DATA_SUMMARY_ID = lDataSet.DATA_SUMMARY_ID;
                        enty.T_STORYBOARD_DATASET_SETTING.Add(tblsetting);
                        enty.SaveChanges();
                    }

                    RunOrder++;
                }
                logger.Info(string.Format("SaveAsStoryboardGrid end | StoryboardId: {0} | Username: {1}", lStoryboardId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad SaveAsStoryboardGrid method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad SaveAsStoryboardGrid method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void DeleteSBStep(long lStoryboardDetailId)
        {
            try
            {
                logger.Info(string.Format("DeleteSBStep start | StoryboardDetailId: {0} | Username: {1}", lStoryboardDetailId, Username));
                var lSBDetailSetting = enty.T_STORYBOARD_DATASET_SETTING.Where(x => x.STORYBOARD_DETAIL_ID == lStoryboardDetailId).ToList();

                enty.T_STORYBOARD_DATASET_SETTING.Where(x => x.STORYBOARD_DETAIL_ID == lStoryboardDetailId)
                   .ToList().ForEach(p => enty.T_STORYBOARD_DATASET_SETTING.Remove(p));

                var lSBDetail = enty.T_PROJ_TC_MGR.Find(lStoryboardDetailId);
                if (lSBDetail != null)
                {
                    enty.T_PROJ_TC_MGR.Remove(lSBDetail);
                    enty.SaveChanges();
                }
                logger.Info(string.Format("DeleteSBStep end | StoryboardDetailId: {0} | Username: {1}", lStoryboardDetailId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad DeleteSBStep method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad DeleteSBStep method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<StoryBoardResultExportModel> ExportStoryboardList(string pstoryboard, string pproject, string lstrConn, string schema)
        {
            try
            {
                logger.Info(string.Format("Export Stoaryborad start | Storyboardname: {0} | project: {1} | Username: {2}", pstoryboard, pproject, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("PROJECT", OracleDbType.Varchar2);
                ladd_refer_image[0].Value = pproject;

                ladd_refer_image[1] = new OracleParameter("Storyboardname", OracleDbType.Varchar2);
                ladd_refer_image[1].Value = pstoryboard;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_EXPORT_STORYBOARDNEW";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<StoryBoardResultExportModel> resultList = dt.AsEnumerable().Select(row =>
                new StoryBoardResultExportModel
                {
                    STORYBOARDDETAILID = Convert.ToString(row.Field<long>("STORYBOARDDETAILID")),
                    STORYBOARDID = Convert.ToString(row.Field<long>("STORYBOARDID")),
                    PROJECTID = Convert.ToString(row.Field<long>("PROJECTID")),
                    APPLICATIONNAME = Convert.ToString(row.Field<string>("APPLICATIONNAME")),
                    RUNORDER = Convert.ToString(row.Field<long>("RUNORDER")),
                    PROJECTNAME = Convert.ToString(row.Field<string>("PROJECTNAME")),
                    PROJECTDESCRIPTION = Convert.ToString(row.Field<string>("PROJECTDESCRIPTION")),
                    STORYBOARD_NAME = Convert.ToString(row.Field<string>("STORYBOARD_NAME")),
                    ACTIONNAME = Convert.ToString(row.Field<string>("ACTIONNAME")),
                    STEPNAME = Convert.ToString(row.Field<string>("STEPNAME")),
                    SUITENAME = Convert.ToString(row.Field<string>("SUITENAME")),
                    CASENAME = Convert.ToString(row.Field<string>("CASENAME")),
                    DATASETNAME = Convert.ToString(row.Field<string>("DATASETNAME")),
                    DEPENDENCY = Convert.ToString(row.Field<string>("DEPENDENCY")),
                    TEST_STEP_DESCRIPTION = Convert.ToString(row.Field<string>("TEST_STEP_DESCRIPTION")),

                }).ToList();
                logger.Info(string.Format("Export Stoaryborad end | Storyboardname: {0} | project: {1} | Username: {2}", pstoryboard, pproject, Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured stoaryborad in ExportStoryboardList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured stoaryborad in ExportStoryboardList method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void DeleteSBSteps(List<long> lStoryboardDetailId)
        {
            try
            {
                logger.Info(string.Format("Delete SBSteps start| Username: {0}", Username));

                enty.T_STORYBOARD_DATASET_SETTING.Where(x => lStoryboardDetailId.Contains((long)x.STORYBOARD_DETAIL_ID))
            .ToList().ForEach(p => enty.T_STORYBOARD_DATASET_SETTING.Remove(p));


                enty.T_PROJ_TC_MGR.Where(x => lStoryboardDetailId.Contains((long)x.STORYBOARD_DETAIL_ID))
                  .ToList().ForEach(p => enty.T_PROJ_TC_MGR.Remove(p));
                enty.SaveChanges();

                logger.Info(string.Format("Delete SBSteps end | Username: {0}", Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured stoaryborad in DeleteSBSteps method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured stoaryborad in DeleteSBSteps method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<ApplicationModel> GetApplicationListByStoryboardId(long lStoryboardId)
        {
            try
            {
                logger.Info(string.Format("Get ApplicationList By StoryboardId start | StoryboardId: {0} | Username: {1}", lStoryboardId, Username));
                var lAppList = new List<ApplicationModel>();
                var lProjectId = enty.T_STORYBOARD_SUMMARY.FirstOrDefault(x => x.STORYBOARD_ID == lStoryboardId).ASSIGNED_PROJECT_ID;
                if (lProjectId != null)
                {
                    var lList = from u in enty.REL_APP_PROJ
                                join r in enty.T_REGISTERED_APPS on u.APPLICATION_ID equals r.APPLICATION_ID
                                where u.PROJECT_ID == lProjectId
                                select new ApplicationModel
                                {
                                    ApplicationId = r.APPLICATION_ID,
                                    ApplicationName = r.APP_SHORT_NAME
                                };
                    lAppList = lList.ToList();
                }
                logger.Info(string.Format("Get ApplicationList By StoryboardId end | StoryboardId: {0} | Username: {1}", lStoryboardId, Username));
                return lAppList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured stoaryborad in GetApplicationListByStoryboardId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured stoaryborad in GetApplicationListByStoryboardId method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string NewSaveAsStoryboard(string storyboardname, string storyboarddesc, long oldsid, long projectid, string schema, string constring, string LoginName)
        {
            try
            {
                logger.Info(string.Format("New SaveAs Storyboard start | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                if (!string.IsNullOrEmpty(storyboardname))
                {
                    storyboardname = storyboardname.Trim();
                }
                var lflag = "success";
                var result = CheckDuplicateStoryboardNameSaveAs(storyboardname, oldsid);
                if (result == true)
                {
                    var sresult = enty.T_STORYBOARD_SUMMARY.Find(oldsid);

                    if (sresult.STORYBOARD_NAME == storyboardname && sresult.DESCRIPTION != storyboarddesc)
                    {
                        lflag = "description cannot be changed";
                        return lflag;
                    }

                    lflag = "error";
                    return lflag;
                }
                var tbl = new T_STORYBOARD_SUMMARY();
                tbl.STORYBOARD_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                tbl.STORYBOARD_NAME = storyboardname;
                tbl.DESCRIPTION = storyboarddesc;
                tbl.CREATE_TIME = DateTime.Now;
                tbl.LATEST_VERISON = null;
                tbl.ASSIGNED_PROJECT_ID = projectid;
                enty.T_STORYBOARD_SUMMARY.Add(tbl);
                enty.SaveChanges();

                var lresult = (from t in enty.T_PROJ_TC_MGR
                               where t.STORYBOARD_ID == oldsid
                               select new InsertStoryboardModel
                               {
                                   PROJECT_ID = t.PROJECT_ID,
                                   TEST_SUITE_ID = t.TEST_SUITE_ID,
                                   TEST_CASE_ID = t.TEST_CASE_ID,
                                   RUN_TYPE = t.RUN_TYPE,
                                   RUN_ORDER = t.RUN_ORDER,
                                   DEPENDS_ON = t.DEPENDS_ON,
                                   LATEST_TEST_MARK_ID = t.LATEST_TEST_MARK_ID,
                                   RECORD_VERSION = t.RECORD_VERSION,
                                   ALIAS_NAME = t.ALIAS_NAME,
                                   STORYBOARD_DETAIL_ID = 0
                               }).ToList();

                foreach (var item in lresult)
                {
                    item.STORYBOARD_DETAIL_ID = (long)Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                }
                var AddSb = (from addresult in lresult
                             select new
                             {
                                 storyboarddetailid = addresult.STORYBOARD_DETAIL_ID,
                                 storyboardid = tbl.STORYBOARD_ID,
                                 addresult.PROJECT_ID,
                                 addresult.TEST_SUITE_ID,
                                 addresult.TEST_CASE_ID,
                                 addresult.RUN_TYPE,
                                 addresult.RUN_ORDER,
                                 addresult.DEPENDS_ON,
                                 addresult.LATEST_TEST_MARK_ID,
                                 addresult.RECORD_VERSION,
                                 addresult.ALIAS_NAME
                             }).ToList();

                var saveitems = (from addsb in AddSb
                                 select new T_PROJ_TC_MGR
                                 {
                                     STORYBOARD_DETAIL_ID = (long)addsb.storyboarddetailid,
                                     STORYBOARD_ID = addsb.storyboardid,
                                     PROJECT_ID = addsb.PROJECT_ID,
                                     TEST_SUITE_ID = addsb.TEST_SUITE_ID,
                                     TEST_CASE_ID = addsb.TEST_CASE_ID,
                                     RUN_TYPE = addsb.RUN_TYPE,
                                     RUN_ORDER = addsb.RUN_ORDER,
                                     DEPENDS_ON = addsb.DEPENDS_ON,
                                     LATEST_TEST_MARK_ID = addsb.LATEST_TEST_MARK_ID,
                                     RECORD_VERSION = addsb.RECORD_VERSION,
                                     ALIAS_NAME = addsb.ALIAS_NAME
                                 }).ToList();
                foreach (var item in saveitems)
                {
                    enty.T_PROJ_TC_MGR.Add(item);
                }
                enty.SaveChanges();



                var lSBDataSettingList = (from sb in AddSb
                                          join t1 in enty.T_PROJ_TC_MGR on sb.RUN_ORDER equals t1.RUN_ORDER
                                          join t2 in enty.T_STORYBOARD_DATASET_SETTING on t1.STORYBOARD_DETAIL_ID equals t2.STORYBOARD_DETAIL_ID
                                          where t1.STORYBOARD_ID == oldsid
                                          select new T_STORYBOARD_DATASET_SETTING
                                          {
                                              SETTING_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ"),
                                              STORYBOARD_DETAIL_ID = sb.storyboarddetailid,
                                              DATA_SUMMARY_ID = t2.DATA_SUMMARY_ID
                                          }
                                          ).ToList();
                foreach (var item in lSBDataSettingList)
                {
                    enty.T_STORYBOARD_DATASET_SETTING.Add(item);
                }
                enty.SaveChanges();
                logger.Info(string.Format("New SaveAs Storyboard end | storyboardname: {0} | UserName: {1}", storyboardname, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Storyborad SaveAs NewSaveAsStoryboard method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Storyborad SaveAs NewSaveAsStoryboard method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<StoryboardResultDataModel> GetStoryBoardResultData(string schema, string lconstring, long lCompareHistId, long lBaselineHistId)
        {
            try
            {
                logger.Info(string.Format("GetStoryBoardResultData start | BaselineHistId: {0} |lCompareHistId: {1} | Username: {2}", lBaselineHistId, lCompareHistId, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("Compare_HISTID", OracleDbType.Long);
                ladd_refer_image[0].Value = lCompareHistId;

                ladd_refer_image[1] = new OracleParameter("Baseline_HISTID", OracleDbType.Long);
                ladd_refer_image[1].Value = lBaselineHistId;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_STORYBOARD_RESULT";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<StoryboardResultDataModel> result = dt.AsEnumerable().Select(row =>
                  new StoryboardResultDataModel
                  {
                      TEST_REPORT_STEP_ID = row.Field<long>("TEST_REPORT_STEP_ID"),
                      Baseline_RETURN_VALUES = row.Field<string>("Baseline_RETURN_VALUES"),
                      Compare_RETURN_VALUES = row.Field<string>("Compare_RETURN_VALUES"),
                      INPUT_VALUE_SETTING = row.Field<string>("INPUT_VALUE_SETTING")
                  }).ToList();

                logger.Info(string.Format("GetStoryBoardResultData end | BaselineHistId: {0} |lCompareHistId: {1} | Username: {2}", lBaselineHistId, lCompareHistId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in stoaryborad GetStoryBoardResultData method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in stoaryborad GetStoryBoardResultData method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool DeleteResultSets(List<long> lDeleteResultSetsList)
        {
            try
            {
                logger.Info(string.Format("Delete ResultSets List start | Username: {0}", Username));
                var lflag = false;
                try
                {
                    foreach (var item in lDeleteResultSetsList)
                    {
                        enty.DeleteResultSets(item);
                        enty.SaveChanges();
                    }
                    lflag = true;
                }
                catch (Exception ex)
                {
                    lflag = false;
                }
                logger.Info(string.Format("Delete ResultSets List end | Username: {0}", Username));
                return lflag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in DeleteResultSets method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in DeleteResultSets method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void ChangelatestTestMarkId(long lhistedId, long latestTestMarkId)
        {
            try
            {
                logger.Info(string.Format("Change latestTest MarkId start | histedId: {0} | Username: {1}", lhistedId, Username));
                var result = enty.T_PROJ_TEST_RESULT.Where(x => x.HIST_ID == lhistedId && x.LATEST_TEST_MARK_ID == latestTestMarkId).FirstOrDefault();
                if (result != null)
                {
                    var list = enty.T_PROJ_TEST_RESULT.Where(x => x.TEST_CASE_ID == result.TEST_CASE_ID && x.STORYBOARD_DETAIL_ID == result.STORYBOARD_DETAIL_ID && x.TEST_MODE == result.TEST_MODE).ToList();
                    if (list.Count() > 0)
                    {
                        var maxval = list.Max(x => x.LATEST_TEST_MARK_ID);
                        if (maxval != latestTestMarkId || maxval < 0)
                        {
                            result.LATEST_TEST_MARK_ID = Helper.NextTestSuiteId("SEQ_TESTRESULT_ID");
                            result.CREATE_TIME = DateTime.Now;
                            enty.SaveChanges();
                        }
                    }
                }
                logger.Info(string.Format("Change latestTest MarkId end | histedId: {0} | Username: {1}", lhistedId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in ChangelatestTestMarkId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in ChangelatestTestMarkId method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool UpdateResultSets(List<TestResultSaveModel> lList)
        {
            var lflag = false;
            try
            {
                logger.Info(string.Format("Update ResultSets start | Username: {0}", Username));
                var lUpdateList = (from tbl in enty.T_PROJ_TEST_RESULT.AsEnumerable()
                                   join up in lList on tbl.HIST_ID equals up.HistId
                                   select new { db = tbl, update = up }).ToList();


                lUpdateList.ToList().ForEach(item =>
                {
                    item.db.RESULT_ALIAS_NAME = item.update.Alias;
                    item.db.RESULT_DESC = item.update.Description;
                });
                enty.SaveChanges();
                lflag = true;
                logger.Info(string.Format("Update ResultSets end | Username: {0}", Username));
                return lflag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyboard in ChangelatestTestMarkId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyboard in ChangelatestTestMarkId method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public long GetTestReportId(long HistId)
        {
            long TestReportId = 0;
            try
            {
                logger.Info(string.Format("Get TestReportId start | HistId: {0} | Username: {1}", HistId, Username));
                var lList = enty.T_TEST_REPORT.Where(x => x.HIST_ID == HistId).ToList();
                if (lList.Count() > 0)
                {
                    TestReportId = lList.FirstOrDefault().TEST_REPORT_ID;
                }
                logger.Info(string.Format("Get TestReportId end | HistId: {0} | Username: {1}", HistId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyboard in GetTestReportId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyboard in GetTestReportId method | UserName: {0}", Username), ex);
                throw;
            }
            return TestReportId;
        }

        public List<TestResultModel> GetPrimartTestResult(long TestCaseId, long SBDetailId)
        {
            try
            {
                logger.Info(string.Format("Get Primart TestResult start | TestCaseId: {0} | SBDetailId: {1} | Username: {2}", TestCaseId, SBDetailId, Username));
                List<TestResultModel> listresult = new List<TestResultModel>();
                var lList = enty.T_PROJ_TEST_RESULT.Where(x => x.TEST_CASE_ID == TestCaseId && x.STORYBOARD_DETAIL_ID == SBDetailId).Select(y => new TestResultModel
                {
                    HistId = y.HIST_ID,
                    StoryboardDetailId = y.STORYBOARD_DETAIL_ID,
                    TestCaseId = y.TEST_CASE_ID,
                    TestModeId = y.TEST_MODE,
                    TestMode = y.TEST_MODE == 1 ? "BaseLine" : "Comparison",
                    TestResult = y.TEST_RESULT,
                    LatestTestMarkId = y.LATEST_TEST_MARK_ID
                }).OrderBy(z => z.HistId).ToList();

                lList.ForEach(item =>
                {
                    item.ResultAliasName = item.ResultAliasName == null || item.ResultAliasName == "null" ? "" : item.ResultAliasName;
                    item.ResultDesc = item.ResultDesc == null || item.ResultDesc == "null" ? "" : item.ResultDesc;
                });
                var maxBaselineMarkIds = lList.Where(x => x.TestModeId == 1).ToList();
                var maxCompareMarkIds = lList.Where(x => x.TestModeId != 1).ToList();
                long maxBaselineMarkId = 0;
                long maxCompareMarkId = 0;
                if (maxBaselineMarkIds.Count() > 0)
                {
                    maxBaselineMarkId = (long)(maxBaselineMarkIds.Max(x => x.LatestTestMarkId));
                    var maxHistId = lList.Where(x => x.LatestTestMarkId == maxBaselineMarkId && x.TestModeId == 1).Max(y => y.HistId);
                    lList.Where(x => x.LatestTestMarkId == maxBaselineMarkId && x.TestModeId == 1 && x.HistId == maxHistId).ToList().ForEach(item =>
                    {
                        item.IsMark = true;
                    });
                }
                if (maxCompareMarkIds.Count() > 0)
                {
                    maxCompareMarkId = (long)(maxCompareMarkIds.Max(x => x.LatestTestMarkId));
                    var maxHistId = lList.Where(x => x.LatestTestMarkId == maxCompareMarkId && x.TestModeId != 1).Max(y => y.HistId);
                    lList.Where(x => x.LatestTestMarkId == maxCompareMarkId && x.TestModeId != 1 && x.HistId == maxHistId).ToList().ForEach(item =>
                    {
                        item.IsMark = true;
                    });
                }
                List<TestResultModel> listprimaryresult = new List<TestResultModel>();
                listprimaryresult = lList.Where(x => x.IsMark).ToList();
                logger.Info(string.Format("Get Primart TestResult end | TestCaseId: {0} | SBDetailId: {1} | Username: {2}", TestCaseId, SBDetailId, Username));
                return listprimaryresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Storyboard in GetPrimartTestResult method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Storyboard in GetPrimartTestResult method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool UpdateResult(long lHistId, short lValue)
        {
            logger.Info(string.Format("Update Result start | HistId: {0} | Username: {1} | Value: {2}", lHistId, Username, lValue));
            var flag = false;
            try
            {
                var ltbl = enty.T_PROJ_TEST_RESULT.Find(lHistId);
                ltbl.TEST_RESULT = lValue;
                enty.SaveChanges();
                flag = true;
                logger.Info(string.Format("Update Result end | HistId: {0} | Username: {1}", lHistId, Username));
            }
            catch (Exception ex)
            {
                flag = false;
                logger.Error(string.Format("Error occured Storyboard in GetTestReportId method | UserName: {0} | HistId: {1} | Value: {2}", Username, lHistId,  lValue));
                ELogger.ErrorException(string.Format("Error occured Storyboard in GetTestReportId method | UserName: {0} | HistId: {1} | Value: {2}", Username, lHistId, lValue), ex);
                throw;
            }
            return flag;
        }

        public bool updatePrimaryResultStatus(long TestCaseId,long StoryboardDetailId,string lSchema, string lConnectionStr)
        {
            logger.Info(string.Format("Update Primary Result start | TestCaseId: {0} | StoryboardDetailId: {1}", TestCaseId, StoryboardDetailId));
            var flag = false;
            try {
                var lPrimaryResultList = GetTestResult(TestCaseId, StoryboardDetailId);
                long lPrimaryBaselineResultHistId = 0;
                long lPrimaryCompareResultHistId = 0;
                var output = false;
                if (lPrimaryResultList.Count() > 0)
                {
                    if (lPrimaryResultList.Any(x => x.TestModeId == 1))
                    {
                        lPrimaryBaselineResultHistId = lPrimaryResultList.FirstOrDefault(x => x.TestModeId == 1 && x.IsMark).HistId;
                    }
                    if (lPrimaryResultList.Any(x => x.TestModeId != 1))
                    {
                        lPrimaryCompareResultHistId = lPrimaryResultList.FirstOrDefault(x => x.TestModeId != 1 && x.IsMark).HistId;
                    }

                    logger.Info(string.Format("Update Primary Result start | Baseline HistId: {0} | Compare HistId: {1}", lPrimaryBaselineResultHistId, lPrimaryCompareResultHistId));

                    var lResultSetList = GetCompareResultList(lSchema, lConnectionStr, lPrimaryBaselineResultHistId, lPrimaryCompareResultHistId);
                    var lPassList = lResultSetList.Where(x => x.Result == "TRUE").ToList();
                    var lFAILList = lResultSetList.Where(x => x.Result == "FALSE").ToList();
                    logger.Info(string.Format("Update Primary Result start | Pass List Count: {0} | Fail List Count: {1}", lPassList.Count(), lFAILList.Count()));
                    if (lResultSetList.Count() > 0)
                    {
                        if (lFAILList.Count() > 0 && lPassList.Count() > 0 && lPrimaryCompareResultHistId > 0)
                        {
                            //update result "Partial"
                            output = UpdateResult(lPrimaryCompareResultHistId, 2);
                        }
                        else if (lFAILList.Count() == 0 && lPassList.Count() > 0 && lPrimaryCompareResultHistId > 0)
                        {
                            //update result "PASS"
                            output = UpdateResult(lPrimaryCompareResultHistId, 1);
                        }
                        else if (lResultSetList.Where(x => x.BaselineStepId > 0).Count() == 0 && lPrimaryCompareResultHistId > 0)
                        {
                            //update result "Partial"
                            output = UpdateResult(lPrimaryCompareResultHistId, 2);
                        }
                        else if (lFAILList.Count() > 0 && lPassList.Count() == 0 && lPrimaryCompareResultHistId > 0)
                        {
                            //update result "Partial"
                            output = UpdateResult(lPrimaryCompareResultHistId, 0);
                        }
                    }
                }
                flag = true;
               
            }
            catch(Exception ex)
            {
                flag = false;
                logger.Error(string.Format("Error occured Storyboard in updatePrimaryResultStatus method | UserName: {0} | | TestCaseId: {1} | StoryboardDetailId: {2}", Username, TestCaseId, StoryboardDetailId));
                ELogger.ErrorException(string.Format("Error occured Storyboard in updatePrimaryResultStatus method | UserName: {0} | | TestCaseId: {1} | StoryboardDetailId: {2}", Username, TestCaseId, StoryboardDetailId), ex);
                throw;
            }
            return flag;
        }
    }
}

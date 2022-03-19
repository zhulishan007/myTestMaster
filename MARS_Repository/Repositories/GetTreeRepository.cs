
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARS_Repository.Entities;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using NLog;

namespace MARS_Repository.Repositories
{
    public class GetTreeRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        public List<ProjectByUser> ProjectListByUserName(decimal username)
        {
            try
            {
                logger.Info(string.Format("ProjectListByUserName start | username: {0}", username));
                var list = new List<ProjectByUser>();
                var result = (from t in entity.T_TEST_PROJECT
                              join t1 in entity.REL_PROJECT_USER on t.PROJECT_ID equals t1.PROJECT_ID
                              join t2 in entity.T_TESTER_INFO on t1.USER_ID equals t2.TESTER_ID
                              where t2.TESTER_ID == username
                              select new ProjectByUser
                              {
                                  ProjectId = t.PROJECT_ID,
                                  userId = t2.TESTER_ID,
                                  ProjectName = t.PROJECT_NAME,

                              }).ToList();
                if (result.Count > 0)
                {
                    foreach (var itm in result)
                    {
                        list = (from t1 in entity.T_TEST_PROJECT
                                where t1.PROJECT_NAME != null
                                select new ProjectByUser
                                {
                                    ProjectId = t1.PROJECT_ID,
                                    ProjectName = t1.PROJECT_NAME,
                                    ProjectExists = true,
                                    userId = itm.userId
                                }).ToList();
                        foreach (var item in list)
                        {
                            var flag = result.Count(s => s.ProjectName == item.ProjectName);
                            if (flag == 1)
                            {
                                item.ProjectExists = true;
                            }
                            else
                            {
                                item.ProjectExists = false;
                            }
                        }
                    }
                }
                else
                {
                    list = (from t1 in entity.T_TEST_PROJECT
                            where t1.PROJECT_NAME != null
                            select new ProjectByUser
                            {
                                ProjectId = t1.PROJECT_ID,
                                ProjectName = t1.PROJECT_NAME,
                                ProjectExists = false,
                                userId = username
                            }).ToList();
                }

                list = list.Distinct().OrderBy(x => x.ProjectName.ToUpper()).ToList();
                logger.Info(string.Format("ProjectListByUserName end | username: {0}", username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in ProjectListByUserName method | UserId: {0} | Username: {1}", username, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetRoleByUser method | UserId: {0} | Username: {1}", username, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in GetRoleByUser method |UserId: {0} | Username: {1}", username, Username), ex.InnerException);
                throw;
            }
        }

        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }

        public List<ProjectByUser> GetProjectList(decimal lUserid, string lSchema, string lConnectionStr)
        {
            try
            {
                logger.Info(string.Format("Get ProjectList start | UserId: {0}", lUserid));
                var lProjectTree = new List<ProjectByUser>();
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lConnectionStr);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[2];
                ladd_refer_image[0] = new OracleParameter("UserId", OracleDbType.Long);
                ladd_refer_image[0].Value = lUserid;
                ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[1].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = lSchema + "." + "SP_Get_LeftPanelProjectList";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];

                List<ProjectByUser> resultList = dt.AsEnumerable().Select(row =>
                  new ProjectByUser
                  {
                      ProjectId = row.Field<long>("PROJECT_ID"),
                      userId = row.Field<decimal>("TESTER_ID"),
                      ProjectName = Convert.ToString(row.Field<string>("PROJECT_NAME")),
                      ProjectDesc = Convert.ToString(row.Field<string>("PROJECT_DESCRIPTION")),
                      TestSuiteCount = Convert.ToInt32(row.Field<decimal>("TestSuiteCount")),
                      StoryBoardCount = Convert.ToInt32(row.Field<decimal>("StoryBoardCount"))
                  }).ToList();

                var lResult = resultList.Distinct().OrderBy(x => x.ProjectName).ToList();
                logger.Info(string.Format("Get ProjectList end | UserId: {0}", lUserid));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetProjectList method | UserId: {0} | Username: {1}", lUserid, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetProjectList method | UserId: {0} | Username: {1}", lUserid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in GetProjectList method | UserId: {0}", lUserid), ex.InnerException);
                throw;
            }
        }

        public List<TestSuiteListByProject> GetTestSuiteList(long lProjectId)
        {
            try
            {
                logger.Info(string.Format("Get TestSuiteList start | ProjectId: {0} | UserName: {1}", lProjectId, Username));
                var lTestSuiteTree = new List<TestSuiteListByProject>();
                var lList = from t1 in entity.T_TEST_PROJECT
                            join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                            join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                            where t1.PROJECT_ID == lProjectId
                            select new TestSuiteListByProject
                            {
                                ProjectId = t1.PROJECT_ID,
                                ProjectName = t1.PROJECT_NAME,
                                TestsuiteId = t3.TEST_SUITE_ID,
                                TestsuiteName = t3.TEST_SUITE_NAME,
                                TestCaseCount = 0,
                                TestSuiteDesc = t3.TEST_SUITE_DESCRIPTION
                            };

                var lResult = lList.Distinct().ToList();

                var lgrp = (from t3 in lResult
                            join t4 in entity.REL_TEST_CASE_TEST_SUITE on t3.TestsuiteId equals t4.TEST_SUITE_ID
                            join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                            group t3 by new { t3.ProjectId, t3.ProjectName, t3.TestsuiteId, t3.TestsuiteName, t3.TestSuiteDesc } into empg
                            select new TestSuiteListByProject
                            {
                                ProjectId = empg.Key.ProjectId,
                                ProjectName = empg.Key.ProjectName,

                                TestsuiteId = empg.Key.TestsuiteId,
                                TestsuiteName = empg.Key.TestsuiteName,
                                TestSuiteDesc = empg.Key.TestSuiteDesc,
                                TestCaseCount = empg.Count()
                            }).Distinct().ToList();

                var lLastResult = (from res in lResult
                                   join grp in lgrp on res.TestsuiteId equals grp.TestsuiteId into joined
                                   from grp in joined.DefaultIfEmpty()

                                   select new TestSuiteListByProject
                                   {
                                       ProjectId = res.ProjectId,
                                       ProjectName = res.ProjectName,
                                       TestsuiteId = res.TestsuiteId,
                                       TestsuiteName = res.TestsuiteName,
                                       TestCaseCount = grp == null ? 0 : grp.TestCaseCount,
                                       TestSuiteDesc = res.TestSuiteDesc
                                   }).Distinct().ToList();

                if (lLastResult.Count() > 0)
                {
                    lTestSuiteTree = lLastResult.Distinct().OrderBy(x => x.TestsuiteName).ToList();
                }

                logger.Info(string.Format("Get TestSuiteList end | ProjectId: {0} | UserName: {1}", lProjectId, Username));
                return lTestSuiteTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestSuiteList method | ProjectId: {0} | Username: {1}", lProjectId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestSuiteList method | ProjectId: {0} | Username: {1}", lProjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestSuiteList in GetRoleByUser method |ProjectId: {0} | Username: {1}", lProjectId, Username), ex.InnerException);
                throw;
            }
        }

        public List<TestSuiteListByProject> GetTestSuiteListNew(long lProjectId)
        {
            try
            {
                logger.Info(string.Format("Get TestSuiteList start | ProjectId: {0} | UserName: {1}", lProjectId, Username));
                var lList = from t1 in entity.T_TEST_PROJECT
                            join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                            join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                            where t1.PROJECT_ID == lProjectId
                            select new TestSuiteListByProject
                            {
                                ProjectId =lProjectId,
                                TestsuiteId = t3.TEST_SUITE_ID,
                                TestsuiteName = t3.TEST_SUITE_NAME,
                                TestSuiteDesc = t3.TEST_SUITE_DESCRIPTION
                            };


                var lResult = lList.Distinct().OrderBy(r=>r.TestsuiteName).ToList();
              
                logger.Info(string.Format("Get TestSuiteList end | ProjectId: {0} | UserName: {1}", lProjectId, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestSuiteList method | ProjectId: {0} | Username: {1}", lProjectId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestSuiteList method | ProjectId: {0} | Username: {1}", lProjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestSuiteList in GetRoleByUser method |ProjectId: {0} | Username: {1}", lProjectId, Username), ex.InnerException);
                throw;
            }
        }

        public List<StoryBoardListByProject> GetStoryboardList(long lProjectId)
        {
            try
            {
                logger.Info(string.Format("Get Storyboard List start | ProjectId: {0} | UserName: {1}", lProjectId, Username));
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var lStoryboardTree = new List<StoryBoardListByProject>();
                var lResult = from t2 in entity.T_STORYBOARD_SUMMARY
                              join t1 in entity.T_TEST_PROJECT on t2.ASSIGNED_PROJECT_ID equals t1.PROJECT_ID
                              where t2.ASSIGNED_PROJECT_ID == lProjectId && t2.STORYBOARD_NAME != null
                              select new StoryBoardListByProject
                              {
                                  ProjectId = t1.PROJECT_ID,
                                  ProjectName = t1.PROJECT_NAME,
                                  StoryboardId = t2.STORYBOARD_ID,
                                  StoryboardName = t2.STORYBOARD_NAME,
                                  Storyboardescription = t2.DESCRIPTION
                              };

                //if (lResult.Count() > 0)
                //{
                    lStoryboardTree = lResult.Distinct().OrderBy(x => x.StoryboardName).ToList();
                //}
                sw.Stop();
                sw.Restart();
                //               var result =  entity.Database.SqlQuery<StoryBoardListByProject>($@"select distinct t1.PROJECT_ID as  ProjectId, t1.PROJECT_NAME as  ProjectName ,
                //t2.STORYBOARD_ID as StoryboardId, t2.STORYBOARD_NAME as StoryboardName,
                // t2.DESCRIPTION as Storyboardescription  from T_STORYBOARD_SUMMARY t2
                // join T_TEST_PROJECT t1 on t1.PROJECT_ID = t2.ASSIGNED_PROJECT_ID
                //where t2.ASSIGNED_PROJECT_ID = {lProjectId} and t2.STORYBOARD_NAME is not null");
                //                var ss = result.ToList();
                /*var lStoryboardTree1 = new List<StoryBoardListByProject>();
                using (OracleConnection con = new OracleConnection(@"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=40.76.111.78)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICACATED)(SERVICE_NAME=orclpdb.hybkaalv5llunibolgwj0r0pdf.bx.internal.cloudapp.net)));User Id=GEN_MARS_20;Password=GEN_MARS_20;pooling=false;Connection Timeout=100;"))
                {
                    OracleCommand cmd = new OracleCommand
                    {
                        CommandText = $@"select distinct t1.PROJECT_ID as  ProjectId, t1.PROJECT_NAME as  ProjectName ,
                t2.STORYBOARD_ID as StoryboardId, t2.STORYBOARD_NAME as StoryboardName,
                 t2.DESCRIPTION as Storyboardescription  from T_STORYBOARD_SUMMARY t2
                 join T_TEST_PROJECT t1 on t1.PROJECT_ID = t2.ASSIGNED_PROJECT_ID
                where t2.ASSIGNED_PROJECT_ID = {lProjectId} and t2.STORYBOARD_NAME is not null",
                        Connection = con
                    };
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        StoryBoardListByProject sb = new StoryBoardListByProject();
                        sb.ProjectId = (long) dr["ProjectId"];
                        sb.ProjectName = dr["ProjectName"].ToString();
                        sb.StoryboardId = (long)dr["StoryboardId"];
                        sb.StoryboardName =  dr["StoryboardName"].ToString(); 
                        sb.Storyboardescription = dr["Storyboardescription"].ToString();

                    }
                }

                sw.Stop();*/
                 
                logger.Info(string.Format("Get Storyboard List end | ProjectId: {0} | UserName: {1}", lProjectId, Username));
                
                return lStoryboardTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetStoryboardList method | ProjectId: {0} | Username: {1}", lProjectId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetStoryboardList method | ProjectId: {0} | Username: {1}", lProjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetStoryboardList in GetRoleByUser method |ProjectId: {0} | Username: {1}", lProjectId, Username), ex.InnerException);
                throw;
            }
        }

        public List<TestCaseListByProject> GetTestCaseList(long lProjectId, long lTestSuiteId)
        {
            try
            {
                logger.Info(string.Format("Get TestCase List start | ProjectId: {0} | TestSuiteId: {1} | UserName: {2}", lProjectId, lTestSuiteId, Username));
                var lTestcaseTree = new List<TestCaseListByProject>();

                var lList = from t1 in entity.T_TEST_PROJECT
                            join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                            join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                            join t4 in entity.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                            join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                            join t6 in entity.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                            where t1.PROJECT_ID == lProjectId && t3.TEST_SUITE_ID == lTestSuiteId
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
                logger.Info(string.Format("Get TestCase List end | ProjectId: {0} | TestSuiteId: {1} | UserName: {2}", lProjectId, lTestSuiteId, Username));
                return lTestcaseTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestCaseList method | ProjectId: {0} | TestSuiteId : {1} | Username: {2}", lProjectId, lTestSuiteId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestCaseList method | ProjectId: {0} | TestSuiteId : {1} | Username: {2}", lProjectId, lTestSuiteId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestCaseList in GetRoleByUser method | ProjectId: {0} | TestSuiteId : {1} | Username: {2}", lProjectId, lTestSuiteId, Username), ex.InnerException);
                throw;
            }
        }
        public List<TestCaseListByProject> GetDatasetCount(long lProjectId, long lTestSuiteId, long TestCaseId)
        {
            try
            {
                logger.Info(string.Format("Get Dataset Count start | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, TestCaseId, Username));
                var lList = from t1 in entity.T_TEST_PROJECT
                            join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                            join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                            join t4 in entity.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                            join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                            join t6 in entity.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                            where t1.PROJECT_ID == lProjectId && t3.TEST_SUITE_ID == lTestSuiteId && t5.TEST_CASE_ID == TestCaseId
                            select new TestCaseListByProject
                            {
                                DataSetCount = (long)t6.DATA_SUMMARY_ID
                            };
                var lResult = lList.Distinct().ToList();
                lResult = lResult.GroupBy(x => new { x.ProjectId, x.ProjectName, x.TestsuiteId, x.TestsuiteName, x.TestcaseId, x.TestcaseName, x.TestCaseDesc }).Select(gcs => new TestCaseListByProject()
                {

                    DataSetCount = gcs.Count()
                }).ToList();

                logger.Info(string.Format("Get Dataset Count end | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, TestCaseId, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetDatasetCount method | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, TestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetDatasetCount method | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, TestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in GetDatasetCount method | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, TestCaseId, Username), ex.InnerException);
                throw;
            }
        }
        public List<DataSetListByTestCase> GetDataSetList(long lProjectId, long lTestSuiteId, long lTestCaseId, string lProjectName, string lTestSuiteName, string lTestCaseName)
        {
            try
            {
                logger.Info(string.Format("Get Dataset List start | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, lTestCaseId, Username));
                var lList = new List<DataSetListByTestCase>();
                var lResult = from t1 in entity.T_TEST_DATA_SUMMARY
                              join t2 in entity.REL_TC_DATA_SUMMARY on t1.DATA_SUMMARY_ID equals t2.DATA_SUMMARY_ID
                              where t2.TEST_CASE_ID == lTestCaseId
                              select new DataSetListByTestCase
                              {
                                  ProjectId = lProjectId,
                                  ProjectName = lProjectName,
                                  TestsuiteId = lTestSuiteId,
                                  TestsuiteName = lTestSuiteName,
                                  TestcaseId = lTestCaseId,
                                  TestcaseName = lTestCaseName,
                                  Datasetid = t1.DATA_SUMMARY_ID,
                                  Datasetname = t1.ALIAS_NAME
                              };
                if (lResult.Count() > 0)
                {
                    lList = lResult.OrderBy(x => x.Datasetname).ToList();
                }
                logger.Info(string.Format("Get Dataset List end | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, lTestCaseId, Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetDataSetList method | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, lTestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetDataSetList method | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, lTestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in GetDataSetList method | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", lProjectId, lTestSuiteId, lTestCaseId, Username), ex.InnerException);
                throw;
            }
        }

        public List<DataSetListByTestCase> GetDataSetListbyId(long lTestCaseId)
        {
            logger.Info(string.Format("Get DataSetList start | TestCaseId: {0} | UserName: {1}", lTestCaseId, Username));
            try
            {
                var lList = new List<DataSetListByTestCase>();
                var lResult = from t1 in entity.T_TEST_DATA_SUMMARY
                              join t2 in entity.REL_TC_DATA_SUMMARY on t1.DATA_SUMMARY_ID equals t2.DATA_SUMMARY_ID
                              where t2.TEST_CASE_ID == lTestCaseId
                              select new DataSetListByTestCase
                              {
                                  TestcaseId = lTestCaseId,
                                  Datasetid = t1.DATA_SUMMARY_ID,
                                  Datasetname = t1.ALIAS_NAME
                              };
                if (lResult.Count() > 0)
                {
                    lList = lResult.OrderBy(x => x.Datasetname).ToList();
                }
                logger.Info(string.Format("Get DataSetList end | TestCaseId: {0} | UserName: {1}", lTestCaseId, Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetDataSetListbyId method | TestCaseId: {0} | UserName: {1}", lTestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetDataSetList method | TestCaseId: {0} | UserName: {1}", lTestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in GetDataSetList method | TestCaseId: {0} | UserName: {1}", lTestCaseId, Username), ex.InnerException);
                throw;
            }
        }
        public List<StoryBoardListByProject> GetStoryboardList(List<Mars_Serialization.ViewModel.ProjectByUser> projectByUserList)
        {
            try
            {
                List<long>  list = projectByUserList.Select(r => r.ProjectId).ToList();
                var lStoryboardTree = new List<StoryBoardListByProject>();
                var lResult = from t2 in entity.T_STORYBOARD_SUMMARY
                              join t1 in entity.T_TEST_PROJECT on t2.ASSIGNED_PROJECT_ID equals t1.PROJECT_ID
                              where t2.ASSIGNED_PROJECT_ID!= null && list.Contains( (long)t2.ASSIGNED_PROJECT_ID ) && t2.STORYBOARD_NAME != null
                              select new StoryBoardListByProject
                              {
                                  ProjectId = t1.PROJECT_ID,
                                  ProjectName = t1.PROJECT_NAME,
                                  StoryboardId = t2.STORYBOARD_ID,
                                  StoryboardName = t2.STORYBOARD_NAME,
                                  Storyboardescription = t2.DESCRIPTION
                              };

                if (lResult.Count() > 0)
                {
                    lStoryboardTree = lResult.Distinct().OrderBy(x => x.StoryboardName).ToList();
                }
 
                return lStoryboardTree;
            }
            catch (Exception ex)
            {
                logger.Error(ex.StackTrace);
                ELogger.ErrorException(string.Format("Error occured GetTree in GetStoryboardList method | ProjectId: all projects | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetStoryboardList in GetRoleByUser method |ProjectId: all projects | Username: {0}",  Username), ex.InnerException);
                throw;
            }
        }



        public List<StoryBoardListByProject> GetStoryboardListCache()
        {
            try
            {
                logger.Info(string.Format("Get All Storyboard List start  | UserName: {0}",  Username));

                var lStoryboardTree = new List<StoryBoardListByProject>();
                var lResult = from t2 in entity.T_STORYBOARD_SUMMARY
                              join t1 in entity.T_TEST_PROJECT on t2.ASSIGNED_PROJECT_ID equals t1.PROJECT_ID
                              where  t2.STORYBOARD_NAME != null
                              select new StoryBoardListByProject
                              {
                                  ProjectId = t1.PROJECT_ID,
                                  ProjectName = t1.PROJECT_NAME,
                                  StoryboardId = t2.STORYBOARD_ID,
                                  StoryboardName = t2.STORYBOARD_NAME,
                                  Storyboardescription = t2.DESCRIPTION
                              };

                lStoryboardTree = lResult.Distinct().ToList();
                 
                logger.Info(string.Format("Get All Storyboard List end  | UserName: {0}", Username));

                return lStoryboardTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetStoryboardList method   Username: {0}",  Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetStoryboardList method | Username: {0}",  Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetStoryboardList in GetRoleByUser method Username: {0}",Username), ex.InnerException);
                throw;
            }
        }

        public List<TestSuiteListByProject> GetTestSuiteListCache(string connectString="")
        {
            try
            {
                logger.Info(string.Format("Get TestSuiteList start  UserName: {0}",  Username));
                var lTestSuiteTree = new List<TestSuiteListByProject>();

                if (!string.IsNullOrWhiteSpace(connectString))
                {
                    using (OracleConnection con = new OracleConnection (connectString))
                    {
                        OracleCommand cmd = new OracleCommand
                        {
                            CommandText = @"select t1.PROJECT_ID, t1.PROJECT_NAME, t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,  t3.TEST_SUITE_DESCRIPTION,
                                count(t5.TEST_CASE_ID) as testCaseCount from T_TEST_PROJECT t1 
                                join REL_TEST_SUIT_PROJECT t2 on t2.PROJECT_ID = t1.PROJECT_ID 
                                join T_TEST_SUITE t3 on t3.TEST_SUITE_ID =t2.TEST_SUITE_ID
                                 join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                 join T_TEST_CASE_SUMMARY t5 on  t4.TEST_CASE_ID = t5.TEST_CASE_ID 
                                 group by t1.PROJECT_ID,   t1.PROJECT_NAME,   t3.TEST_SUITE_ID,  t3.TEST_SUITE_NAME,   t3.TEST_SUITE_DESCRIPTION",
                            Connection = con
                        };
                        con.Open();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                TestSuiteListByProject testSuite = new TestSuiteListByProject();
                                testSuite.ProjectId = (long)dr["PROJECT_ID"];
                                testSuite.ProjectName = dr["PROJECT_NAME"].ToString();
                                testSuite.TestsuiteId = (long)dr["TEST_SUITE_ID"];
                                testSuite.TestsuiteName = dr["TEST_SUITE_NAME"].ToString();
                                testSuite.TestSuiteDesc = dr["TEST_SUITE_DESCRIPTION"].ToString();
                                testSuite.TestCaseCount = (long)((decimal)dr["testCaseCount"]);
                                lTestSuiteTree.Add(testSuite);
                            }
                        }
                    }
                }
                else
                {
                    var lList = from t1 in entity.T_TEST_PROJECT
                                join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                                join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                                join t4 in entity.REL_TEST_CASE_TEST_SUITE on t3.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                                join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                                select new TestSuiteListByProject
                                {
                                    ProjectId = t1.PROJECT_ID,
                                    ProjectName = t1.PROJECT_NAME,
                                    TestsuiteId = t3.TEST_SUITE_ID,
                                    TestsuiteName = t3.TEST_SUITE_NAME,
                                    TestCaseCount = (long)t5.TEST_CASE_ID,
                                    TestSuiteDesc = t3.TEST_SUITE_DESCRIPTION
                                };

                    var lResult = lList.Distinct().ToList();
                    lResult = lResult.GroupBy(x => new { x.ProjectId, x.TestsuiteId, x.TestsuiteName, x.ProjectName, x.TestSuiteDesc }).Select(c => new TestSuiteListByProject
                    {
                        ProjectId = c.Key.ProjectId,
                        ProjectName = c.Key.ProjectName,
                        TestsuiteId = c.Key.TestsuiteId,
                        TestsuiteName = c.Key.TestsuiteName,
                        TestCaseCount = c.Count(),
                        TestSuiteDesc = c.Key.TestSuiteDesc
                    }).ToList();


                    if (lResult.Count() > 0)
                    {
                        lTestSuiteTree = lResult.Distinct().OrderBy(x => x.TestsuiteName).ToList();
                    }
                }

                logger.Info(string.Format("Get TestSuiteList end | UserName: {0}",  Username));
                return lTestSuiteTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestSuiteList method | Username: {0}",  Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestSuiteList method | Username: {0}",  Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestSuiteList in GetRoleByUser method  Username: {0}",  Username), ex.InnerException);
                throw;
            }
        }

        public List<TestCaseListByProject> GetTestCaseListCache(string connectString = "")
        {
            try
            {
                logger.Info(string.Format("Get All TestCase List Cache start   UserName: {0}",   Username));
                var lTestcaseTree = new List<TestCaseListByProject>();
                if (!string.IsNullOrWhiteSpace(connectString))
                {
                    using (OracleConnection con = new OracleConnection(connectString))
                    {
                        OracleCommand cmd = new OracleCommand
                        {
                            CommandText = @"select t1.PROJECT_ID,   t1.PROJECT_NAME,  t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION,
                                         count(t6.DATA_SUMMARY_ID)  as DataSetCount  from T_TEST_PROJECT t1 
                                         join REL_TEST_SUIT_PROJECT t2 on t2.PROJECT_ID = t1.PROJECT_ID 
                                         join T_TEST_SUITE t3 on t3.TEST_SUITE_ID =t2.TEST_SUITE_ID
                                         join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                         join T_TEST_CASE_SUMMARY t5 on  t5.TEST_CASE_ID = t4.TEST_CASE_ID
                                         join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                         group by t1.PROJECT_ID,   t1.PROJECT_NAME,   t3.TEST_SUITE_ID,  t3.TEST_SUITE_NAME,  
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION",
                            Connection = con
                        };
                        con.Open();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                TestCaseListByProject testSuite = new TestCaseListByProject();
                                testSuite.ProjectId = (long)dr["PROJECT_ID"];
                                testSuite.ProjectName = dr["PROJECT_NAME"].ToString();
                                testSuite.TestsuiteId = (long)dr["TEST_SUITE_ID"];
                                testSuite.TestsuiteName = dr["TEST_SUITE_NAME"].ToString();
                                testSuite.TestcaseId = (long)dr["TEST_CASE_ID"];
                                testSuite.TestcaseName = dr["TEST_CASE_NAME"].ToString();
                                testSuite.TestCaseDesc = dr["TEST_STEP_DESCRIPTION"].ToString();
                                testSuite.DataSetCount = (long)((decimal)dr["DataSetCount"]);
                                lTestcaseTree.Add(testSuite);
                            }
                        }
                    }
                }
                else
                {
                    var lList = from t1 in entity.T_TEST_PROJECT
                                join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                                join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                                join t4 in entity.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                                join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                                join t6 in entity.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
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
                }
                logger.Info(string.Format("Get TestCase List end UserName: {0}",   Username));
                return lTestcaseTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestCaseList    | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestCaseList method   | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestCaseList in GetRoleByUser method   Username: {0}",  Username), ex.InnerException);
                throw;
            }
        }

        public List<DataSetListByTestCase> GetDataSetListCache(string connectString = "")
        {
            try
            {
                logger.Info(string.Format("Get Dataset List Cache UserName: {0}",  Username));
                var lList = new List<DataSetListByTestCase>();

                if (!string.IsNullOrWhiteSpace(connectString))
                {

                    using (OracleConnection con = new OracleConnection(connectString))
                    {
                        OracleCommand cmd = new OracleCommand
                        {
                            CommandText = @"select t1.PROJECT_ID,   t1.PROJECT_NAME,  t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,
                                             t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t7.DATA_SUMMARY_ID,t7.ALIAS_NAME
                                               from T_TEST_PROJECT t1 
                                             join REL_TEST_SUIT_PROJECT t2 on t2.PROJECT_ID = t1.PROJECT_ID 
                                             join T_TEST_SUITE t3 on t3.TEST_SUITE_ID =t2.TEST_SUITE_ID
                                             join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                             join T_TEST_CASE_SUMMARY t5 on  t5.TEST_CASE_ID = t4.TEST_CASE_ID
                                             join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                             join T_TEST_DATA_SUMMARY t7 on t7.DATA_SUMMARY_ID =t6.DATA_SUMMARY_ID",
                            Connection = con
                        };
                        con.Open();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DataSetListByTestCase dataSet = new DataSetListByTestCase();
                                dataSet.ProjectId = (long)dr["PROJECT_ID"];
                                dataSet.ProjectName = dr["PROJECT_NAME"].ToString();
                                dataSet.TestsuiteId = (long)dr["TEST_SUITE_ID"];
                                dataSet.TestsuiteName = dr["TEST_SUITE_NAME"].ToString();
                                dataSet.TestcaseId = (long)dr["TEST_CASE_ID"];
                                dataSet.TestcaseName =  dr["TEST_CASE_NAME"].ToString();
                                dataSet.Datasetid = (long)dr["DATA_SUMMARY_ID"];
                                dataSet.Datasetname = dr["ALIAS_NAME"].ToString();
                                lList.Add(dataSet);
                            }
                        }
                    }

                }
                else
                {
                    var lResult = from t1 in entity.T_TEST_PROJECT
                                  join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                                  join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                                  join t4 in entity.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                                  join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                                  join t6 in entity.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                                  join t7 in entity.T_TEST_DATA_SUMMARY on t6.DATA_SUMMARY_ID equals t7.DATA_SUMMARY_ID
                                  select new DataSetListByTestCase
                                  {
                                      ProjectId = t1.PROJECT_ID,
                                      ProjectName = t1.PROJECT_NAME,
                                      TestsuiteId = t3.TEST_SUITE_ID,
                                      TestsuiteName = t3.TEST_SUITE_NAME,
                                      TestcaseId = t5.TEST_CASE_ID,
                                      TestcaseName = t5.TEST_CASE_NAME,
                                      Datasetid = t7.DATA_SUMMARY_ID,
                                      Datasetname = t7.ALIAS_NAME
                                  };
                    if (lResult.Count() > 0)
                    {
                        lList = lResult.OrderBy(x => x.Datasetname).ToList();
                    }
                }
                logger.Info(string.Format("Get Dataset List Cache end UserName: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetDataSetList method   UserName: {0}",  Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetDataSetList method | UserName: {0}",  Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in GetDataSetList method  | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_PROJECT> GetProjectListCache()
        {
            try
            {
                logger.Info(string.Format("Get Project List Cache UserName: {0}", Username));
                var lResult = entity.T_TEST_PROJECT.ToList();        
                logger.Info(string.Format("Get Project List Cache end UserName: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in Project method   UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in Project method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in Project method  | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
    }
}

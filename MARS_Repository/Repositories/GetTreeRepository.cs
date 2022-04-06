
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
using System.Data.Common;

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
                            /*CommandText = @"select t1.PROJECT_ID,   t1.PROJECT_NAME,  t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION,
                                         count(t6.DATA_SUMMARY_ID)  as DataSetCount  from T_TEST_PROJECT t1 
                                         join REL_TEST_SUIT_PROJECT t2 on t2.PROJECT_ID = t1.PROJECT_ID 
                                         join T_TEST_SUITE t3 on t3.TEST_SUITE_ID =t2.TEST_SUITE_ID
                                         join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                         join T_TEST_CASE_SUMMARY t5 on  t5.TEST_CASE_ID = t4.TEST_CASE_ID
                                         join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                         group by t1.PROJECT_ID,   t1.PROJECT_NAME,   t3.TEST_SUITE_ID,  t3.TEST_SUITE_NAME,  
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION",*/
                            CommandText = @"select distinct  t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION,
                                         count(t6.DATA_SUMMARY_ID)  as DataSetCount  from   T_TEST_SUITE t3 
                                         join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                         join T_TEST_CASE_SUMMARY t5 on  t5.TEST_CASE_ID = t4.TEST_CASE_ID
                                         join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                         group by   t3.TEST_SUITE_ID,  t3.TEST_SUITE_NAME,  
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION",

                            Connection = con
                        };
                        con.Open();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                TestCaseListByProject testSuite = new TestCaseListByProject();
                                //testSuite.ProjectId = (long)dr["PROJECT_ID"];
                                //testSuite.ProjectName = dr["PROJECT_NAME"].ToString();
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
                            /*CommandText = @"select t1.PROJECT_ID,   t1.PROJECT_NAME,  t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,
                                             t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t7.DATA_SUMMARY_ID,t7.ALIAS_NAME
                                               from T_TEST_PROJECT t1 
                                             join REL_TEST_SUIT_PROJECT t2 on t2.PROJECT_ID = t1.PROJECT_ID 
                                             join T_TEST_SUITE t3 on t3.TEST_SUITE_ID =t2.TEST_SUITE_ID
                                             join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                             join T_TEST_CASE_SUMMARY t5 on  t5.TEST_CASE_ID = t4.TEST_CASE_ID
                                             join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                             join T_TEST_DATA_SUMMARY t7 on t7.DATA_SUMMARY_ID =t6.DATA_SUMMARY_ID",*/
                            CommandText = @"select distinct  t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t7.DATA_SUMMARY_ID,t7.ALIAS_NAME,t7.DESCRIPTION_INFO
                                               from  T_TEST_CASE_SUMMARY  T5
                                             join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                             join T_TEST_DATA_SUMMARY t7 on t7.DATA_SUMMARY_ID =t6.DATA_SUMMARY_ID",
                            Connection = con
                        };
                        con.Open();
                        //cmd.ExecuteNonQuery();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DataSetListByTestCase dataSet = new DataSetListByTestCase();
                                //dataSet.ProjectId = (long)dr["PROJECT_ID"];
                                //dataSet.ProjectName = dr["PROJECT_NAME"].ToString();
                                //dataSet.TestsuiteId = (long)dr["TEST_SUITE_ID"];
                                //dataSet.TestsuiteName = dr["TEST_SUITE_NAME"].ToString();
                                dataSet.TestcaseId = (long)dr["TEST_CASE_ID"];
                                dataSet.TestcaseName =  dr["TEST_CASE_NAME"].ToString();
                                dataSet.Datasetid = (long)dr["DATA_SUMMARY_ID"];
                                dataSet.Datasetname = dr["ALIAS_NAME"].ToString();
                                dataSet.Description = dr["DESCRIPTION_INFO"].ToString();
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

        public List<SYSTEM_LOOKUP> GetActionsCache()
        {
            try
            {
                logger.Info(string.Format("Get Actions start | UserName: {0}",   Username));
                
                var result = entity.SYSTEM_LOOKUP.Where(x => x.FIELD_NAME == "RUN_TYPE" && x.TABLE_NAME == "T_PROJ_TC_MGR" && x.DISPLAY_NAME != "FAILUE").ToList();
                 
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetActions method| UserName: {0}",  Username));
                ELogger.ErrorException(string.Format("Error occured StoryBoard in GetActions method | UserName: {0}",  Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured StoryBoard in GetActions method | UserName: {0}",  Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_FOLDER> GetFolderCache()
        {
            try
            {
                logger.Info(string.Format("Get FOLDER start | UserName: {0}", Username));

                var result = entity.T_TEST_FOLDER.Where(x => !string.IsNullOrEmpty(x.FOLDERNAME)).ToList();

                logger.Info(string.Format("Get FOLDER end | UserName: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in GetTreeRepository for GetFolderCache method| UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTreeRepository in GetFolderCache method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTreeRepository in GetFolderCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public List<T_FOLDER_FILTER> GetFilterCache()
        {
            try
            {
                logger.Info(string.Format("GetFilterList start | UserName: {0}", Username));
                var List = entity.T_FOLDER_FILTER.Distinct().ToList();
                logger.Info(string.Format("GetFilterList end | UserName: {0}", Username));
                return List;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetFilterList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetFilterList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetFilterList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<REL_FOLDER_FILTER> GetRelFolderFilterCache()
        {
            try
            {
                logger.Info(string.Format("GetRelFolderFilterCache start  UserName: {0}",  Username));
                var lResult = entity.REL_FOLDER_FILTER.ToList();

                logger.Info(string.Format("GetRelFolderFilterCache end   | UserName: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetRelFolderFilterCache method | UserName: {0}",  Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetRelFolderFilterCache method |UserName: {0}",  Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetRelFolderFilterCache method | UserName: {0}",  Username), ex.InnerException);
                throw;
            }
        }

        public List<T_REGISTERED_APPS> GetAppCache()
        {
            try
            {
                logger.Info(string.Format("GetAppCache start  UserName: {0}", Username));
                var lResult = entity.T_REGISTERED_APPS.ToList();

                logger.Info(string.Format("GetAppCache end   | UserName: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetAppCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetAppCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetAppCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_SET> GetSetCache()
        {
            try
            {
                logger.Info(string.Format("GetSetCache start  UserName: {0}", Username));
                var lResult = entity.T_TEST_SET.ToList();

                logger.Info(string.Format("GetSetCache end   | UserName: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetSetCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetSetCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetSetCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }


        public List<T_TEST_GROUP> GetGroupCache()
        {
            try
            {
                logger.Info(string.Format("GetGroupCache start  UserName: {0}", Username));
                var lResult = entity.T_TEST_GROUP.ToList();

                logger.Info(string.Format("GetGroupCache end   | UserName: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetGroupCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetGroupCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetGroupCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_DATASETTAG> GetDataSetTagCache()
        {
            try
            {
                logger.Info(string.Format("GetDataSetTagCache start  UserName: {0}", Username));
                var lResult = entity.T_TEST_DATASETTAG.ToList();
                logger.Info(string.Format("GetDataSetTagCache end   | UserName: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetDataSetTagCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetDataSetTagCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetDataSetTagCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<StoryBoardListByProject> GetStoryboardListCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get All Storyboard List start  | UserName: {0}", Username));

                var lStoryboardTree = new List<StoryBoardListByProject>();
                cmd.CommandText = @"select distinct  t1.PROJECT_ID, t1.PROJECT_NAME,t2.STORYBOARD_ID,t2.STORYBOARD_NAME,t2.DESCRIPTION
                                        from  T_STORYBOARD_SUMMARY  T2
                                        join T_TEST_PROJECT t1 on  t1.PROJECT_ID =t2.ASSIGNED_PROJECT_ID  where  t2.STORYBOARD_NAME is not null";

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        StoryBoardListByProject project = new StoryBoardListByProject();
                        project.ProjectId = Helper.GetDBValue<long>(dr["PROJECT_ID"],0);
                        project.ProjectName = Helper.GetDBValue<string>( dr["PROJECT_NAME"],"");
                        project.StoryboardId = Helper.GetDBValue<long>(dr["STORYBOARD_ID"],0);
                        project.StoryboardName = Helper.GetDBValue<string>(dr["STORYBOARD_NAME"],"");
                        project.Storyboardescription = Helper.GetDBValue<string>(dr["DESCRIPTION"],"");
                        lStoryboardTree.Add(project);
                    }
                }

                logger.Info(string.Format("Get All Storyboard List end  | UserName: {0}", Username));

                return lStoryboardTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetStoryboardList method   Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetStoryboardList method | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetStoryboardList in GetRoleByUser method Username: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<TestSuiteListByProject> GetTestSuiteListCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get TestSuiteList start  UserName: {0}", Username));
                var lTestSuiteTree = new List<TestSuiteListByProject>();
                    
                cmd.CommandText = @"select t1.PROJECT_ID, t1.PROJECT_NAME, t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,  t3.TEST_SUITE_DESCRIPTION,
                        count(t5.TEST_CASE_ID) as testCaseCount from T_TEST_PROJECT t1 
                        join REL_TEST_SUIT_PROJECT t2 on t2.PROJECT_ID = t1.PROJECT_ID 
                        join T_TEST_SUITE t3 on t3.TEST_SUITE_ID =t2.TEST_SUITE_ID
                            join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                            join T_TEST_CASE_SUMMARY t5 on  t4.TEST_CASE_ID = t5.TEST_CASE_ID 
                            group by t1.PROJECT_ID,   t1.PROJECT_NAME,   t3.TEST_SUITE_ID,  t3.TEST_SUITE_NAME,   t3.TEST_SUITE_DESCRIPTION";                     
                        
                using (DbDataReader dr = cmd.ExecuteReader())
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

                logger.Info(string.Format("Get TestSuiteList end | UserName: {0}", Username));
                return lTestSuiteTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestSuiteList method | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestSuiteList method | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestSuiteList in GetRoleByUser method  Username: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<TestCaseListByProject> GetTestCaseListCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get All TestCase List Cache start   UserName: {0}", Username));
                var lTestcaseTree = new List<TestCaseListByProject>();
                cmd.CommandText = @"select distinct  t3.TEST_SUITE_ID, t3.TEST_SUITE_NAME,
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION,
                                         count(t6.DATA_SUMMARY_ID)  as DataSetCount  from   T_TEST_SUITE t3 
                                         join REL_TEST_CASE_TEST_SUITE t4 on t4.TEST_SUITE_ID =t3.TEST_SUITE_ID
                                         join T_TEST_CASE_SUMMARY t5 on  t5.TEST_CASE_ID = t4.TEST_CASE_ID
                                         join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                         group by   t3.TEST_SUITE_ID,  t3.TEST_SUITE_NAME,  
                                         t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t5.TEST_STEP_DESCRIPTION";
                 
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TestCaseListByProject testSuite = new TestCaseListByProject();
                        //testSuite.ProjectId = (long)dr["PROJECT_ID"];
                        //testSuite.ProjectName = dr["PROJECT_NAME"].ToString();
                        testSuite.TestsuiteId = (long)dr["TEST_SUITE_ID"];
                        testSuite.TestsuiteName = dr["TEST_SUITE_NAME"].ToString();
                        testSuite.TestcaseId = (long)dr["TEST_CASE_ID"];
                        testSuite.TestcaseName = dr["TEST_CASE_NAME"].ToString();
                        testSuite.TestCaseDesc = dr["TEST_STEP_DESCRIPTION"].ToString();
                        testSuite.DataSetCount = (long)((decimal)dr["DataSetCount"]);
                        lTestcaseTree.Add(testSuite);
                    }
                }

                logger.Info(string.Format("Get TestCase List end UserName: {0}", Username));
                return lTestcaseTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetTestCaseList  | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetTestCaseList method   | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTestCaseList in GetRoleByUser method   Username: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<DataSetListByTestCase> GetDataSetListCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get Dataset List Cache UserName: {0}", Username));
                var lList = new List<DataSetListByTestCase>();

                cmd.CommandText = @"select distinct  t5.TEST_CASE_ID, t5.TEST_CASE_NAME,t7.DATA_SUMMARY_ID,t7.ALIAS_NAME,t7.DESCRIPTION_INFO
                                        from  T_TEST_CASE_SUMMARY  T5
                                        join REL_TC_DATA_SUMMARY t6 on t6.TEST_CASE_ID = t5.TEST_CASE_ID
                                        join T_TEST_DATA_SUMMARY t7 on t7.DATA_SUMMARY_ID =t6.DATA_SUMMARY_ID";
                       
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DataSetListByTestCase dataSet = new DataSetListByTestCase();

                        dataSet.TestcaseId = (long)dr["TEST_CASE_ID"];
                        dataSet.TestcaseName = dr["TEST_CASE_NAME"].ToString();
                        dataSet.Datasetid = (long)dr["DATA_SUMMARY_ID"];
                        dataSet.Datasetname = dr["ALIAS_NAME"].ToString();
                        dataSet.Description = dr["DESCRIPTION_INFO"].ToString();
                        lList.Add(dataSet);
                    }
                }
                logger.Info(string.Format("Get Dataset List Cache End UserName: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured GetTree in GetDataSetList method   UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTree in GetDataSetList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTree in GetDataSetList method  | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_PROJECT> GetProjectListCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get Project List Cache UserName: {0}", Username));
                var lList = new List<T_TEST_PROJECT>();
                cmd.CommandText = @"select * from T_TEST_PROJECT";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_TEST_PROJECT project = new T_TEST_PROJECT();

                        project.PROJECT_ID = Helper.GetDBValue<long>(dr["PROJECT_ID"],0);
                        project.PROJECT_NAME = Helper.GetDBValue<string>(dr["PROJECT_NAME"],"");
                        project.PROJECT_DESCRIPTION = Helper.GetDBValue<string>(dr["PROJECT_DESCRIPTION"],"");
                        project.CREATOR = Helper.GetDBValue<string>(dr["CREATOR"].ToString(),"");
                        project.CREATE_DATE = Helper.GetDBValue<DateTime>(dr["CREATE_DATE"],DateTime.MinValue);
                        project.STATUS = Helper.GetDBValue<short>(dr["STATUS"],0);
                        lList.Add(project);
                    }
                }
 
                logger.Info(string.Format("Get Project List Cache end UserName: {0}", Username));
                return lList;
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

        public List<SYSTEM_LOOKUP> GetActionsCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get Actions start | UserName: {0}", Username));

                var lList = new List<SYSTEM_LOOKUP>();
                cmd.CommandText = @"select * from SYSTEM_LOOKUP where FIELD_NAME='RUN_TYPE' and TABLE_NAME='T_PROJ_TC_MGR' and DISPLAY_NAME <>'FAILUE' ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SYSTEM_LOOKUP lookup = new SYSTEM_LOOKUP();
                        lookup.LOOKUP_ID = Helper.GetDBValue<long>(dr["LOOKUP_ID"], 0);
                        lookup.TABLE_NAME = Helper.GetDBValue<string>(dr["TABLE_NAME"], "");
                        lookup.FIELD_NAME = Helper.GetDBValue<string>(dr["FIELD_NAME"], "");
                        lookup.DISPLAY_NAME = Helper.GetDBValue<string>(dr["DISPLAY_NAME"].ToString(), "");
                        lookup.VALUE = Helper.GetDBValue<short>(dr["VALUE"], 0);
                        lookup.STATUS = Helper.GetDBValue<short>(dr["STATUS"], 0);
                        lList.Add(lookup);
                    }
                }

                logger.Info(string.Format("Get Actions end | UserName: {0}", Username));

                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetActions method| UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured StoryBoard in GetActions method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured StoryBoard in GetActions method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_FOLDER> GetFolderCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("Get FOLDER start | UserName: {0}", Username));
                var lList = new List<T_TEST_FOLDER>();
                cmd.CommandText = @"select * from T_TEST_FOLDER where FOLDERNAME is not null ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_TEST_FOLDER folder = new T_TEST_FOLDER();
                        folder.FOLDERID = Helper.GetDBValue<long>(dr["FOLDERID"], 0);
                        folder.DESCRIPTION = Helper.GetDBValue<string>(dr["DESCRIPTION"], "");
                        folder.FOLDERNAME = Helper.GetDBValue<string>(dr["FOLDERNAME"], "");
                        folder.CREATION_DATE = Helper.GetDBValue<DateTime>(dr["CREATION_DATE"],DateTime.MinValue);
                        folder.UPDATE_DATE = Helper.GetDBValue<DateTime>(dr["UPDATE_DATE"], DateTime.MinValue);
                        folder.ACTIVE = Helper.GetDBValue<short>(dr["ACTIVE"], 0);
                        folder.CREATION_USER = Helper.GetDBValue<string>(dr["CREATION_USER"], "");
                        folder.UPDATE_CREATION_USER = Helper.GetDBValue<string>(dr["UPDATE_CREATION_USER"], "");
                        lList.Add(folder);
                    }
                }

                logger.Info(string.Format("Get FOLDER start | UserName: {0}", Username));

                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in GetTreeRepository for GetFolderCache method| UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured GetTreeRepository in GetFolderCache method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured GetTreeRepository in GetFolderCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public List<T_FOLDER_FILTER> GetFilterCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("GetFilterList start | UserName: {0}", Username));
                var list = new List<T_FOLDER_FILTER>();
                cmd.CommandText = @"select distinct * from T_FOLDER_FILTER ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_FOLDER_FILTER folder = new T_FOLDER_FILTER();
                        folder.FILTER_ID = Helper.GetDBValue<long>(dr["FILTER_ID"], 0);
                        folder.FILTER_NAME = Helper.GetDBValue<string>(dr["FILTER_NAME"], "");
                        folder.PROJECT_IDS = Helper.GetDBValue<string>(dr["PROJECT_IDS"], "");
                        folder.STORYBOARD_IDS = Helper.GetDBValue<string>(dr["STORYBOARD_IDS"],"");
                        list.Add(folder);
                    }
                }
                logger.Info(string.Format("GetFilterList end | UserName: {0}", Username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetFilterList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetFilterList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetFilterList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<REL_FOLDER_FILTER> GetRelFolderFilterCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("GetRelFolderFilterCache start  UserName: {0}", Username));
                var list = new List<REL_FOLDER_FILTER>();
                cmd.CommandText = @"select  * from REL_FOLDER_FILTER ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        REL_FOLDER_FILTER folder = new REL_FOLDER_FILTER();
                        folder.FILTER_ID = Helper.GetDBValue<long>(dr["FILTER_ID"], 0);
                        folder.REL_FOLDER_FILTER_ID = Helper.GetDBValue<long>(dr["REL_FOLDER_FILTER_ID"], 0);
                        folder.FOLDER_ID = Helper.GetDBValue<long>(dr["FOLDER_ID"], 0);
                        list.Add(folder);
                    }
                }
                //var lResult = entity.REL_FOLDER_FILTER.ToList();
                logger.Info(string.Format("GetRelFolderFilterCache end   | UserName: {0}", Username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetRelFolderFilterCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetRelFolderFilterCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetRelFolderFilterCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_REGISTERED_APPS> GetAppCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("GetAppCache start  UserName: {0}", Username));

                var list = new List<T_REGISTERED_APPS>();
                cmd.CommandText = @"select  * from T_REGISTERED_APPS ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_REGISTERED_APPS app = new T_REGISTERED_APPS();
                        app.APPLICATION_ID = Helper.GetDBValue<long>(dr["APPLICATION_ID"], 0);
                        app.APP_SHORT_NAME = Helper.GetDBValue<string>(dr["APP_SHORT_NAME"], "");
                        app.PROCESS_IDENTIFIER = Helper.GetDBValue<string>(dr["PROCESS_IDENTIFIER"], "");
                        app.STARTER_PATH = Helper.GetDBValue<string>(dr["STARTER_PATH"], "");
                        app.STARTER_COMMAND = Helper.GetDBValue<string>(dr["STARTER_COMMAND"], "");
                        app.VERSION = Helper.GetDBValue<string>(dr["VERSION"], "");
                        app.COMMENT = Helper.GetDBValue<string>(dr["COMMENT"], "");
                        app.APPLICATION_TYPE_ID = Helper.GetDBValue<short>(dr["APPLICATION_TYPE_ID"], 0);
                        app.RECORD_CREATE_PERSON = Helper.GetDBValue<string>(dr["RECORD_CREATE_PERSON"], "");
                        app.RECORD_CREATE_DATE = Helper.GetDBValue<DateTime>(dr["RECORD_CREATE_DATE"], DateTime.MinValue);
                        app.EXTRAREQUIREMENT = Helper.GetDBValue<string>(dr["EXTRAREQUIREMENT"], ""); 
                        app.EXTRAPOPUPMENU = Helper.GetDBValue<string>(dr["EXTRAPOPUPMENU"], "");
                        app.ISBASELINE = Helper.GetDBValue<decimal>(dr["ISBASELINE"], 0);
                        app.IS64BIT = Helper.GetDBValue<decimal>(dr["IS64BIT"], 0);

                        list.Add(app);
                    }
                }
                //var lResult = entity.T_REGISTERED_APPS.ToList();
                logger.Info(string.Format("GetAppCache end   | UserName: {0}", Username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetAppCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetAppCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetAppCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_SET> GetSetCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("GetSetCache start  UserName: {0}", Username));

                var list = new List<T_TEST_SET>();
                cmd.CommandText = @"select  * from T_TEST_SET ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_TEST_SET set = new T_TEST_SET();
                        set.SETID = Helper.GetDBValue<long>(dr["SETID"], 0);
                        set.SETNAME = Helper.GetDBValue<string>(dr["SETNAME"], "");
                        set.DESCRIPTION = Helper.GetDBValue<string>(dr["DESCRIPTION"], "");
                        set.ACTIVE = Helper.GetDBValue<short>(dr["ACTIVE"], 0);
                        set.CREATION_DATE = Helper.GetDBValue<DateTime>(dr["CREATION_DATE"], DateTime.MinValue);
                        set.UPDATE_DATE = Helper.GetDBValue<DateTime>(dr["UPDATE_DATE"], DateTime.MinValue);
                        set.CREATION_USER = Helper.GetDBValue<string>(dr["CREATION_USER"], "");
                        set.UPDATE_CREATION_USER = Helper.GetDBValue<string>(dr["UPDATE_CREATION_USER"], "");

                        list.Add(set);
                    }
                }
                //var lResult = entity.T_TEST_SET.ToList();

                logger.Info(string.Format("GetSetCache end   | UserName: {0}", Username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetSetCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetSetCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetSetCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_GROUP> GetGroupCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("GetGroupCache start  UserName: {0}", Username));
                var list = new List<T_TEST_GROUP>();
                cmd.CommandText = @"select  * from T_TEST_GROUP ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_TEST_GROUP group = new T_TEST_GROUP();
                        group.GROUPID = Helper.GetDBValue<long>(dr["GROUPID"], 0);
                        group.GROUPNAME = Helper.GetDBValue<string>(dr["GROUPNAME"], "");
                        group.DESCRIPTION = Helper.GetDBValue<string>(dr["DESCRIPTION"], "");
                        group.ACTIVE = Helper.GetDBValue<short>(dr["ACTIVE"], 0);
                        group.CREATION_DATE = Helper.GetDBValue<DateTime>(dr["CREATION_DATE"], DateTime.MinValue);
                        group.UPDATE_DATE = Helper.GetDBValue<DateTime>(dr["UPDATE_DATE"], DateTime.MinValue);
                        group.CREATION_USER = Helper.GetDBValue<string>(dr["CREATION_USER"], "");
                        group.UPDATE_CREATION_USER = Helper.GetDBValue<string>(dr["UPDATE_CREATION_USER"], "");

                        list.Add(group);
                    }
                }
                //var lResult = entity.T_TEST_GROUP.ToList();

                logger.Info(string.Format("GetGroupCache end   | UserName: {0}", Username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetGroupCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetGroupCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetGroupCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_DATASETTAG> GetDataSetTagCache(DbCommand cmd)
        {
            try
            {
                logger.Info(string.Format("GetDataSetTagCache start  UserName: {0}", Username));
                var list = new List<T_TEST_DATASETTAG>();
                cmd.CommandText = @"select  * from T_TEST_DATASETTAG ";
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T_TEST_DATASETTAG set = new T_TEST_DATASETTAG();
                        set.GROUPID = Helper.GetDBValue<string>(dr["GROUPID"], "");
                        set.T_TEST_DATASETTAG_ID = Helper.GetDBValue<long>(dr["T_TEST_DATASETTAG_ID"], 0);
                        set.SEQUENCE = Helper.GetDBValue<long>(dr["SEQUENCE"],0);
                        set.DATASETID = Helper.GetDBValue<long>(dr["DATASETID"], 0);
                        set.SETID = Helper.GetDBValue<string>(dr["SETID"],"");
                        set.FOLDERID = Helper.GetDBValue<string>(dr["FOLDERID"], "");
                        set.EXPECTEDRESULTS = Helper.GetDBValue<string>(dr["EXPECTEDRESULTS"], "");
                        set.STEPDESC = Helper.GetDBValue<string>(dr["STEPDESC"], "");
                        set.DIARY = Helper.GetDBValue<string>(dr["DIARY"], "");
                        list.Add(set);
                    }
                }
                //var lResult = entity.T_TEST_DATASETTAG.ToList();
                logger.Info(string.Format("GetDataSetTagCache end   | UserName: {0}", Username));
                return list;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetDataSetTagCache method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetDataSetTagCache method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetDataSetTagCache method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }


    }
}

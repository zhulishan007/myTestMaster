
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
                logger.Error(string.Format("Error occured in ProjectListByUserName method | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in ProjectListByUserName method | Username: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured in GetProjectList method | UserId: {0}", lUserid));
                ELogger.ErrorException(string.Format("Error occured in GetProjectList method | UserId: {0}", lUserid), ex);
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
                logger.Error(string.Format("Error occured TestSuite in GetTestSuiteList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestSuite in GetTestSuiteList method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<StoryBoardListByProject> GetStoryboardList(long lProjectId)
        {
            try
            {
                logger.Info(string.Format("Get Storyboard List start | ProjectId: {0} | UserName: {1}", lProjectId, Username));
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

                if (lResult.Count() > 0)
                {
                    lStoryboardTree = lResult.Distinct().OrderBy(x => x.StoryboardName).ToList();
                }
                logger.Info(string.Format("Get Storyboard List end | ProjectId: {0} | UserName: {1}", lProjectId, Username));

                return lStoryboardTree;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestSuite in GetTestSuiteList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestSuite in GetTestSuiteList method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<TestCaseListByProject> GetTestCaseList(long lProjectId, long lTestSuiteId)
        {
            try
            {
                logger.Info(string.Format("Get TestCase List start | ProjectId: {0} | TestSuiteId: {1} | UserName: {2}", lProjectId, lTestSuiteId,  Username));
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
                logger.Error(string.Format("Error occured TestCase in GetTestCaseList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseList method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in GetDatasetCount method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetDatasetCount method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in GetDatasetCount method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetDatasetCount method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in GetDataSetListbyId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetDataSetListbyId method | UserName: {0}", Username), ex);
                throw;
            }
        }
    }
}



using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARS_Repository;
using MARS_Repository.ViewModel;
using System.Data;
using MARS_Repository.Entities;
using Oracle.ManagedDataAccess.Client;
using NLog;
using System.Transactions;

namespace MARS_Repository.Repositories
{
    public class TestSuiteRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public List<TestSuiteModel> ListAllTestSuites(string schema, string lconstring, int startrec, int pagesize, string colname, string colorder, string namesearch, string descsearch, string appsearch, string projectsearch)
        {
            try
            {
                logger.Info(string.Format("ListAllTestSuites start | UserName: {0}",  Username));
                var result = new List<TestCaseModel>();

                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;



                OracleParameter[] ladd_refer_image = new OracleParameter[9];
                ladd_refer_image[0] = new OracleParameter("Startrec", OracleDbType.Long);
                ladd_refer_image[0].Value = startrec;

                ladd_refer_image[1] = new OracleParameter("totalpagesize", OracleDbType.Long);
                ladd_refer_image[1].Value = pagesize;

                ladd_refer_image[2] = new OracleParameter("ColumnName", OracleDbType.Varchar2);
                ladd_refer_image[2].Value = colname;

                ladd_refer_image[3] = new OracleParameter("Columnorder", OracleDbType.Varchar2);
                ladd_refer_image[3].Value = colorder;

                ladd_refer_image[4] = new OracleParameter("NameSearch", OracleDbType.Varchar2);
                ladd_refer_image[4].Value = namesearch;

                ladd_refer_image[5] = new OracleParameter("DescriptionSearch", OracleDbType.Varchar2);
                ladd_refer_image[5].Value = descsearch;

                ladd_refer_image[6] = new OracleParameter("AppSearch", OracleDbType.Varchar2);
                ladd_refer_image[6].Value = appsearch;

                ladd_refer_image[7] = new OracleParameter("ProjectSearch", OracleDbType.Varchar2);
                ladd_refer_image[7].Value = projectsearch;

                ladd_refer_image[8] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[8].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_LIST_TESTSUITES";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];


                List<TestSuiteModel> resultList = dt.AsEnumerable().Select(row =>
                          new TestSuiteModel
                          {
                              TestSuiteId = row.Field<long>("testsuiteid"),
                              TestSuiteName = Convert.ToString(row.Field<string>("suitename")),
                              TestSuiteDescription = Convert.ToString(row.Field<string>("description")),
                              ProjectId = Convert.ToString(row.Field<string>("projectid")),
                              Project = Convert.ToString(row.Field<string>("projectname")),
                              ApplicationId = Convert.ToString(row.Field<string>("applicationid")),
                              Application = Convert.ToString(row.Field<string>("Applicationame")),
                              TotalCount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                          //DATASETDESCRIPTION = row.Field<string>("DATASETDESCRIPTION"),
                      }).ToList();

                logger.Info(string.Format("ListAllTestSuites end | UserName: {0}", Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ListAllTestSuites method | Schema : {0} | Connection String : {1} | UserName: {2}", schema, lconstring, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ListAllTestSuites method | Schema : {0} | Connection String : {1} | UserName: {2}", schema, lconstring, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ListAllTestSuites method | Schema : {0} | Connection String : {1} | UserName: {2}", schema, lconstring, Username), ex.InnerException);
                throw;
            }
        }
        public bool ListProjectsByTestSuite(long suiteid, string projectid)
        {
            try
            {
                logger.Info(string.Format("ListProjectsByTestSuite start | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username));
                var lflag = false;
                var lProjectSplit = projectid.Split(',').Select(Int64.Parse).ToList();
                var lList = (from x in enty.T_TEST_SUITE
                             join y in enty.REL_TEST_SUIT_PROJECT on x.TEST_SUITE_ID equals y.TEST_SUITE_ID
                             join z in enty.T_TEST_PROJECT on y.PROJECT_ID equals z.PROJECT_ID
                             where x.TEST_SUITE_ID == suiteid
                             select new TestSuiteListByProject
                             {
                                 ProjectId = z.PROJECT_ID,
                                 ProjectName = z.PROJECT_NAME,
                             }).ToList();
                foreach (var itm in lList)
                {
                    var lDependencyValid = lProjectSplit.Contains(itm.ProjectId);
                    if (lDependencyValid == true)
                    {
                        lflag = true;

                    }
                    else
                    {
                        lflag = false;
                        return lflag;
                    }

                }
                logger.Info(string.Format("ListProjectsByTestSuite end | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username));
                return lflag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ListProjectsByTestSuite method | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ListProjectsByTestSuite method | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ListProjectsByTestSuite method | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username), ex.InnerException);
                throw;
            }
        }
       
        public bool AddEditTestSuite(TestSuiteModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.TestSuiteName))
                    {
                        lEntity.TestSuiteName = lEntity.TestSuiteName.Trim();
                    }
                    var flag = false;
                    if (lEntity.TestSuiteId == 0)
                    {
                        logger.Info(string.Format("Add TestSuite start | TestSuiteName: {0} | UserName: {1}", lEntity.TestSuiteName, Username));
                        var tbl = new T_TEST_SUITE();
                        tbl.TEST_SUITE_ID = Helper.NextTestSuiteId("T_TEST_SUITE_SEQ");
                        tbl.TEST_SUITE_NAME = lEntity.TestSuiteName;
                        tbl.TEST_SUITE_DESCRIPTION = lEntity.TestSuiteDescription;
                        lEntity.TestSuiteId = tbl.TEST_SUITE_ID;
                        enty.T_TEST_SUITE.Add(tbl);
                        enty.SaveChanges();

                        logger.Info(string.Format("Add TestSuite end | TestSuiteName: {0} | UserName: {1}", lEntity.TestSuiteName, Username));
                        flag = true;
                    }
                    else
                    {
                        logger.Info(string.Format("Edit TestSuite start | TestSuiteName: {0} | UserName: {1}", lEntity.TestSuiteName, Username));
                        var tbl = enty.T_TEST_SUITE.Find(lEntity.TestSuiteId);
                        #region Testcase and Application Mapping Delete
                        var lAppList = enty.REL_APP_TESTSUITE.Where(x => x.TEST_SUITE_ID == lEntity.TestSuiteId).ToList();
                        foreach (var item in lAppList)
                        {
                            enty.REL_APP_TESTSUITE.Remove(item);
                        }

                        #endregion


                        #region Testcase and TestSuite Mapping Delete
                        var lTCTSList = enty.REL_TEST_SUIT_PROJECT.Where(x => x.TEST_SUITE_ID == lEntity.TestSuiteId).ToList();
                        foreach (var item in lTCTSList)
                        {
                            enty.REL_TEST_SUIT_PROJECT.Remove(item);
                        }
                        enty.SaveChanges();
                        #endregion
                        if (tbl != null)
                        {
                            tbl.TEST_SUITE_NAME = lEntity.TestSuiteName;
                            tbl.TEST_SUITE_DESCRIPTION = lEntity.TestSuiteDescription;
                            enty.SaveChanges();
                        }
                        logger.Info(string.Format("Edit TestSuite end | TestSuiteName: {0} | UserName: {1}", lEntity.TestSuiteName, Username));
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(lEntity.ApplicationId))
                    {

                        var lAppSplit = lEntity.ApplicationId.Split(',').Select(Int64.Parse).ToList();

                        if (lAppSplit.Count() > 0)
                        {
                            lAppSplit = lAppSplit.Distinct().ToList();
                            foreach (var item in lAppSplit)
                            {
                                var lApptbl = new REL_APP_TESTSUITE();
                                lApptbl.APPLICATION_ID = item;
                                lApptbl.TEST_SUITE_ID = lEntity.TestSuiteId;
                                lApptbl.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_APP_TESTSUITE_SEQ");
                                enty.REL_APP_TESTSUITE.Add(lApptbl);
                                enty.SaveChanges();
                            }
                        }
                        if (!string.IsNullOrEmpty(lEntity.ProjectId))
                        {
                            var lProjectSplit = lEntity.ProjectId.Split(',').Select(Int64.Parse).ToList();
                            if (lProjectSplit.Count() > 0)
                            {

                                lProjectSplit = lProjectSplit.Distinct().ToList();
                                foreach (var item in lProjectSplit)
                                {
                                    var lTSTC = new REL_TEST_SUIT_PROJECT();
                                    lTSTC.TEST_SUITE_ID = lEntity.TestSuiteId;
                                    lTSTC.PROJECT_ID = item;
                                    lTSTC.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_TEST_SUIT_PROJECT_SEQ");
                                    enty.REL_TEST_SUIT_PROJECT.Add(lTSTC);
                                    enty.SaveChanges();

                                }
                            }
                        }
                        flag = true;
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for AddEditTestSuite method | TestSuiteId: {0} | UserName: {1}", lEntity.TestSuiteId, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for AddEditTestSuite method | TestSuiteId: {0} | UserName: {1}", lEntity.TestSuiteId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for AddEditTestSuite method | TestSuiteId: {0} | UserName: {1}", lEntity.TestSuiteId, Username), ex.InnerException);
                throw;
            }
        }
        public string ChangeTestSuiteName(string lTestSuiteName, string testsuitedesc, long lTestSuiteId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Change TestSuiteName start | TestSuiteName : {0}| UserName: {1}", lTestSuiteName, Username));
                    if (!string.IsNullOrEmpty(lTestSuiteName))
                        lTestSuiteName = lTestSuiteName.Trim();

                    var lTestSuite = enty.T_TEST_SUITE.Find(lTestSuiteId);
                    lTestSuite.TEST_SUITE_NAME = lTestSuiteName;
                    lTestSuite.TEST_SUITE_DESCRIPTION = testsuitedesc;
                    enty.SaveChanges();
                    logger.Info(string.Format("Change TestSuiteName end | TestSuiteName : {0}| UserName: {1}", lTestSuiteName, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId: {0} | TestSuiteName : {1} | TestSuite Desc : {2} | UserName: {3}", lTestSuiteId, lTestSuiteName, testsuitedesc, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId: {0} | TestSuiteName : {1} | TestSuite Desc : {2} | UserName: {3}", lTestSuiteId, lTestSuiteName, testsuitedesc, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId: {0} | TestSuiteName : {1} | TestSuite Desc : {2} | UserName: {3}", lTestSuiteId, lTestSuiteName, testsuitedesc, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateTestSuiteName(string lTestSuiteName, long? lTestSuiteId)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate TestSuiteName start | TestSuiteName: {0} | UserName: {1}", lTestSuiteName,Username));
                var lresult = false;
                if (lTestSuiteId != null)
                {
                    lresult = enty.T_TEST_SUITE.Any(x => x.TEST_SUITE_ID != lTestSuiteId && x.TEST_SUITE_NAME.ToLower().Trim() == lTestSuiteName.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_TEST_SUITE.Any(x => x.TEST_SUITE_NAME.ToLower().Trim() == lTestSuiteName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate TestSuiteName end | TestSuiteName: {0} | UserName: {1}", lTestSuiteName, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId: {0} | TestSuiteName : {1} | UserName: {2}", lTestSuiteId, lTestSuiteName, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId: {0} | TestSuiteName : {1} | UserName: {2}", lTestSuiteId, lTestSuiteName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ChangeTestSuiteName method | TestSuiteId: {0} | TestSuiteName : {1} | UserName: {2}", lTestSuiteId, lTestSuiteName, Username), ex.InnerException);
                throw;
            }
        }
        public string GetTestSuiteNameById(long SuiteId)
        {
            try
            {
                logger.Info(string.Format("Get TestSuiteName start | TestSuiteId: {0} | UserName: {1}", SuiteId, Username));
                var lSuiteName = enty.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_ID == SuiteId).TEST_SUITE_NAME;
                logger.Info(string.Format("Get TestSuiteName start | TestSuiteId: {0} | UserName: {1}", SuiteId, Username));
                return lSuiteName;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestSuite in GetTestSuiteNameById method | TestSuiteId: {0} | UserName: {1}", SuiteId, Username));
                ELogger.ErrorException(string.Format("Error occured TestSuite in GetTestSuiteNameById method | TestSuiteId: {0} | UserName: {1}", SuiteId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestSuite in GetTestSuiteNameById method | TestSuiteId: {0} | UserName: {1}", SuiteId, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteTestSuite(long TestSuiteId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete TestSuite start | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                    var flag = false;

                    var testSuite = enty.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_ID == TestSuiteId);
                    if (testSuite != null)
                    {

                        var RelAppTS = enty.REL_APP_TESTSUITE.Where(x => x.TEST_SUITE_ID == testSuite.TEST_SUITE_ID).ToList();
                        foreach (var a in RelAppTS)
                        {
                            enty.REL_APP_TESTSUITE.Remove(a);
                            enty.SaveChanges();
                        }

                        var RelPTS = enty.REL_TEST_SUIT_PROJECT.Where(x => x.TEST_SUITE_ID == testSuite.TEST_SUITE_ID).ToList();

                        foreach (var r in RelPTS)
                        {
                            enty.REL_TEST_SUIT_PROJECT.Remove(r);
                            enty.SaveChanges();
                        }

                        var RelTCTS = enty.REL_TEST_CASE_TEST_SUITE.Where(x => x.TEST_SUITE_ID == testSuite.TEST_SUITE_ID).ToList();

                        foreach (var r in RelTCTS)
                        {
                            enty.REL_TEST_CASE_TEST_SUITE.Remove(r);
                            enty.SaveChanges();
                        }
                        enty.T_TEST_SUITE.Remove(testSuite);
                        enty.SaveChanges();
                        flag = true;
                    }
                    logger.Info(string.Format("Delete TestSuite end | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for DeleteTestSuite method | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for DeleteTestSuite method | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for DeleteTestSuite method | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username), ex.InnerException);
                throw;
            }
        }
        public List<TestSuiteListByProject> ListProjectsByTestSuite(long suiteid)
        {
            try
            {
                logger.Info(string.Format("Get TestSuiteName start | TestSuiteId: {0} | UserName: {1}", suiteid, Username));
                var lflag = false;
                var lList = (from x in enty.T_TEST_SUITE
                             join y in enty.REL_TEST_SUIT_PROJECT on x.TEST_SUITE_ID equals y.TEST_SUITE_ID
                             join z in enty.T_TEST_PROJECT on y.PROJECT_ID equals z.PROJECT_ID
                             where x.TEST_SUITE_ID == suiteid
                             select new TestSuiteListByProject
                             {
                                 ProjectId = z.PROJECT_ID,
                                 ProjectName = z.PROJECT_NAME,
                             }).ToList();

                logger.Info(string.Format("Get TestSuiteName start | TestSuiteId: {0} | UserName: {1}", suiteid, Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ListProjectsByTestSuite method | TestSuiteId: {0} | UserName: {1}", suiteid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ListProjectsByTestSuite method | TestSuiteId: {0} | UserName: {1}", suiteid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ListProjectsByTestSuite method | TestSuiteId: {0} | UserName: {1}", suiteid, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> CheckTestSuiteInStoryboardByProject(long suiteid, string projectid)
        {
            try
            {
                logger.Info(string.Format("CheckTestSuiteInStoryboardByProject start | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username));
                var lProjectSplit = new List<long>();
                var fresult = new List<string>();
                //   var flag = false;
                if (projectid != null)
                {
                    lProjectSplit = projectid.Split(',').Select(Int64.Parse).ToList();
                }

                var result = ListProjectsByTestSuite(suiteid);
                foreach (var item in result)
                {
                    if (!lProjectSplit.Contains(item.ProjectId))
                    {
                        var lresult = CheckTestSuiteExist(suiteid, item.ProjectId);
                        if (lresult.Count > 0)
                        {
                            return lresult;
                        }
                        // flag = true;
                        return lresult;
                    }
                }
                logger.Info(string.Format("CheckTestSuiteInStoryboardByProject end | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username));
                return fresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for CheckTestSuiteInStoryboardByProject method | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for CheckTestSuiteInStoryboardByProject method | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for CheckTestSuiteInStoryboardByProject method | TestSuiteId: {0} | projectid: {1} | UserName: {2}", suiteid, projectid, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> CheckTestSuiteExist(long TestSuiteId, long projectid)
        {
            try
            {
                logger.Info(string.Format("CheckTestSuiteExist start | TestSuiteId: {0} | projectid: {1} | UserName: {2}", TestSuiteId, projectid, Username));
                List<string> suitename = new List<string>();
                var lStoryboardList = enty.T_PROJ_TC_MGR.Where(x => x.TEST_SUITE_ID == TestSuiteId && x.PROJECT_ID == projectid).ToList();

                if (lStoryboardList.Count() > 0)
                {
                    foreach (var item in lStoryboardList)
                    {
                        var sname = enty.T_STORYBOARD_SUMMARY.Find(item.STORYBOARD_ID);

                        suitename.Add(sname.STORYBOARD_NAME);
                        suitename = (from w in suitename select w).Distinct().ToList();
                    }
                    logger.Info(string.Format("CheckTestSuiteExist end | TestSuiteId: {0} | projectid: {1} | UserName: {2}", TestSuiteId, projectid, Username));
                    return suitename;
                }
                logger.Info(string.Format("CheckTestSuiteExist end | TestSuiteId: {0} | projectid: {1} | UserName: {2}", TestSuiteId, projectid, Username));
                return suitename;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for CheckTestSuiteInStoryboardByProject method | TestSuiteId: {0} | ProjectId: {1} | UserName: {2}", TestSuiteId, projectid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for CheckTestSuiteInStoryboardByProject method | TestSuiteId: {0} | ProjectId: {1} | UserName: {2}", TestSuiteId, projectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for CheckTestSuiteInStoryboardByProject method | TestSuiteId: {0} | ProjectId: {1} | UserName: {2}", TestSuiteId, projectid, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> CheckTestSuiteExistInStoryboard(long TestSuiteId)
        {
            try
            {
                logger.Info(string.Format("Check TestSuite Exist In Storyboard start | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                List<string> suitename = new List<string>();
                var lStoryboardList = enty.T_PROJ_TC_MGR.Where(x => x.TEST_SUITE_ID == TestSuiteId).ToList();

                if (lStoryboardList.Count() > 0)
                {
                    foreach (var item in lStoryboardList)
                    {
                        var sname = enty.T_STORYBOARD_SUMMARY.Find(item.STORYBOARD_ID);

                        suitename.Add(sname.STORYBOARD_NAME);
                        suitename = (from w in suitename select w).Distinct().ToList();
                    }
                    logger.Info(string.Format("Check TestSuite Exist In Storyboard end | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                    return suitename;
                }
                logger.Info(string.Format("Check TestSuite Exist In Storyboard end | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                return suitename;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for CheckTestSuiteExistInStoryboard method | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for CheckTestSuiteExistInStoryboard method | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for CheckTestSuiteExistInStoryboard method | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username), ex.InnerException);
                throw;
            }
        }

        public bool ExportTestSuite(long TestSuiteId, string Path)
        {
            try
            {
                logger.Info(string.Format("ExportTestSuite start | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                var flag = false;
                var lTestSuite = enty.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_ID == TestSuiteId);
                logger.Info(string.Format("ExportTestSuite end | TestSuiteId: {0} | UserName: {1}", TestSuiteId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestSuite for ExportTestSuite method | TestSuiteId: {0} | Path : {1} | UserName: {2}", TestSuiteId, Path, Username));
                ELogger.ErrorException(string.Format("Error occured in TestSuite for ExportTestSuite method | TestSuiteId: {0} | Path : {1} | UserName: {2}", TestSuiteId, Path, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestSuite for ExportTestSuite method | TestSuiteId: {0} | Path : {1} | UserName: {2}", TestSuiteId, Path, Username), ex.InnerException);
                throw;
            }
        }


        public List<RelTestSuiteApplication> ListRelationTestSuiteApplication(string ApplicationId)
        {
            try
            {
                logger.Info(string.Format("ListRelationTestSuiteApplication start | ApplicationId: {0} | UserName: {1}", ApplicationId, Username));
                var lResult = new List<RelTestSuiteApplication>();

                var lApplicationIds = ApplicationId.Split(',').Select(Int64.Parse).ToList();
                var lList = from x in enty.T_TEST_SUITE
                            join y in enty.REL_APP_TESTSUITE on x.TEST_SUITE_ID equals y.TEST_SUITE_ID
                            join z in enty.T_REGISTERED_APPS on y.APPLICATION_ID equals z.APPLICATION_ID
                            where lApplicationIds.Contains(z.APPLICATION_ID)
                            select new RelTestSuiteApplication
                            {
                                TestSuiteId = x.TEST_SUITE_ID,
                                TestSuiteName = x.TEST_SUITE_NAME,
                            };

                if (lList.Count() > 0)
                    lResult = lList.Distinct().OrderBy(y => y.TestSuiteName).ToList();

                logger.Info(string.Format("ListRelationTestSuiteApplication end | ApplicationId: {0} | UserName: {1}", ApplicationId, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestSuite in ListRelationTestSuiteApplication method | ApplicationId: {0} | UserName: {1}", ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured TestSuite in ListRelationTestSuiteApplication method | ApplicationId: {0} | UserName: {1}", ApplicationId, Username), ex);
                if(ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestSuite in ListRelationTestSuiteApplication method | ApplicationId: {0} | UserName: {1}", ApplicationId, Username), ex.InnerException);
                throw;
            }
        }
    }
}

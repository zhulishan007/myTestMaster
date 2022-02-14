using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using MARS_Repository.ViewModel;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using MARS_Repository.Entities;
using NLog;
using System.Transactions;
using MarsSerializationHelper.ViewModel;
using MoreLinq;
using MarsSerializationHelper.Common;

namespace MARS_Repository.Repositories
{
    public class TestCaseRepository
    {
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        public string Username = string.Empty;

        public string GetTSTCDSId(string TestCasename, string TestSuitname, string Datasetname)
        {
            try
            {
                logger.Info(string.Format("GetTSTCDSId start | TestCaseName: {0} | TestSuitname: {1} | Datasetname: {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, Username));
                var llist = new List<string>();
                var suiteid = entity.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_NAME == TestSuitname).TEST_SUITE_ID;
                var caseid = entity.T_TEST_CASE_SUMMARY.FirstOrDefault(x => x.TEST_CASE_NAME == TestCasename).TEST_CASE_ID;
                var datasetid = entity.T_TEST_DATA_SUMMARY.FirstOrDefault(x => x.ALIAS_NAME == Datasetname).DATA_SUMMARY_ID;

                logger.Info(string.Format("GetTSTCDSId end | TestCaseName: {0} | TestSuitname: {1} | Datasetname: {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, Username));
                return suiteid + "," + caseid + "," + datasetid;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTSTCDSId method | TestCaseName: {0} | TestSuitname: {1} | Datasetname: {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTSTCDSId method | TestCaseName: {0} | TestSuitname: {1} | Datasetname: {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetTSTCDSId method | TestCaseName: {0} | TestSuitname: {1} | Datasetname: {2} | UserName: {3}", TestCasename, TestSuitname, Datasetname, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckTestCaseTestSuiteRel(long testcaseId, long testsuiteid)
        {
            try
            {
                logger.Info(string.Format("Check TestCase TestSuite Rel start | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseId, testsuiteid, Username));
                var flag = false;
                var lStoryboardList = entity.T_PROJ_TC_MGR.Where(x => x.TEST_CASE_ID == testcaseId).ToList();
                foreach (var item in lStoryboardList)
                {
                    if (item.TEST_SUITE_ID != testsuiteid)
                    {
                        flag = true;
                    }
                }
                logger.Info(string.Format("Check TestCase TestSuite Rel end | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseId, testsuiteid, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckTestCaseTestSuiteRel method | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseId, testsuiteid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckTestCaseTestSuiteRel method | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseId, testsuiteid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckTestCaseTestSuiteRel method | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseId, testsuiteid, Username), ex.InnerException);
                throw;
            }
        }
        public string ChangeTestCaseName(string lTestCaseName, long lTestCaseId, string TestCaseDes)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Change TestCaseName start | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username));
                    if (!string.IsNullOrEmpty(lTestCaseName))
                        lTestCaseName = lTestCaseName.Trim();

                    var lTestCase = entity.T_TEST_CASE_SUMMARY.Find(lTestCaseId);
                    lTestCase.TEST_CASE_NAME = lTestCaseName;
                    lTestCase.TEST_STEP_DESCRIPTION = TestCaseDes;
                    entity.SaveChanges();
                    logger.Info(string.Format("Change TestCaseName end | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ChangeTestCaseName method | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ChangeTestCaseName method | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in ChangeTestCaseName method | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username), ex.InnerException);
                throw;
            }
        }
        public bool CheckDuplicateTestCaseName(string lTestCaseName, long? lTestCaseId)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate TestCaseName start | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username));
                var lresult = false;
                if (lTestCaseId != null)
                {
                    lresult = entity.T_TEST_CASE_SUMMARY.Any(x => x.TEST_CASE_ID != lTestCaseId && x.TEST_CASE_NAME.ToLower().Trim() == lTestCaseName.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_CASE_SUMMARY.Any(x => x.TEST_CASE_NAME.ToLower().Trim() == lTestCaseName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate TestCaseName end | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckDuplicateTestCaseName method | TestCaseName: {0} | lTestCaseId: {1} | UserName: {2}", lTestCaseName, lTestCaseId, Username), ex.InnerException);
                throw;
            }
        }
        public bool CheckDuplicateDataset(string lDatasetname, long? ldatasetid)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Dataset start | Datasetname: {0} | datasetid: {1} | UserName: {2}", lDatasetname, ldatasetid, Username));
                var lresult = false;
                if (ldatasetid != null)
                {
                    lresult = entity.T_TEST_DATA_SUMMARY.Any(x => x.DATA_SUMMARY_ID != ldatasetid && x.ALIAS_NAME.ToLower().Trim() == lDatasetname.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_DATA_SUMMARY.Any(x => x.ALIAS_NAME.ToLower().Trim() == lDatasetname.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Dataset end | Datasetname: {0} | datasetid: {1} | UserName: {2}", lDatasetname, ldatasetid, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Dataset in CheckDuplicateDataset method | Datasetname: {0} | datasetid: {1} | UserName: {2}", lDatasetname, ldatasetid, Username));
                ELogger.ErrorException(string.Format("Error occured Dataset in CheckDuplicateDataset method | Datasetname: {0} | datasetid: {1} | UserName: {2}", lDatasetname, ldatasetid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Dataset in CheckDuplicateDataset method | Datasetname: {0} | datasetid: {1} | UserName: {2}", lDatasetname, ldatasetid, Username), ex.InnerException);
                throw;
            }
        }
        public string InsertFeedProcess()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Insert FeedProcess start | UserName: {0}", Username));

                    logger.Info(string.Format("TBLFEEDPROCESS_SEQ : Getting TestCase Feed Process Id start | UserName: {0}", Username));
                    var feedprocessID = Helper.NextTestSuiteId("TBLFEEDPROCESS_SEQ");
                    logger.Info(string.Format("TBLFEEDPROCESS_SEQ : Getting TestCase Feed Process Id end | Feed Process Id : {0} | UserName: {1}", feedprocessID, Username));

                    var ltbl = new TBLFEEDPROCESS();
                    ltbl.FEEDPROCESSID = feedprocessID;
                    ltbl.FEEDPROCESSSTATUS = "Insert-WebApp";
                    ltbl.FEEDRUNON = System.DateTime.Now;
                    ltbl.CREATEDBY = "WebApp";
                    ltbl.CREATEDON = System.DateTime.Now;

                    entity.TBLFEEDPROCESSes.Add(ltbl);
                    logger.Info(string.Format("Save TestCase FeedProcess changes start | UserName: {0}", Username));
                    //rutvi
                    //entity.SaveChanges();
                    logger.Info(string.Format("Save TestCase FeedProcess changes end | UserName: {0}", Username));

                    logger.Info(string.Format("TBLFEEDPROCESSDETAILS_SEQ : Getting TestCase Feed Process Detail Id start | UserName: {0}", Username));
                    var feedprocessDetailsID = Helper.NextTestSuiteId("TBLFEEDPROCESSDETAILS_SEQ");
                    logger.Info(string.Format("TBLFEEDPROCESSDETAILS_SEQ : Getting TestCase Feed Process Detail Id end | Feed Process Detail Id : {0} | UserName: {1}", feedprocessDetailsID, Username));

                    entity.TBLFEEDPROCESSDETAILS.Add(new TBLFEEDPROCESSDETAIL { CREATEDBY = "WebApp", FEEDPROCESSDETAILID = feedprocessDetailsID, CREATEDON = System.DateTime.Now, FEEDPROCESSID = feedprocessID, FEEDPROCESSSTATUS = "INPROGRESS", FILENAME = "WebAppImport", FILETYPE = "TESTCASE" });

                    logger.Info(string.Format("Save TestCase FeedProcessDetail changes start | UserName: {0}", Username));
                    entity.SaveChanges();
                    logger.Info(string.Format("Save TestCase FeedProcessDetail changes end | UserName: {0}", Username));

                    logger.Info(string.Format("Insert FeedProcess end | UserName: {0}", Username));
                    scope.Complete();
                    return Convert.ToString(feedprocessID) + "~" + Convert.ToString(feedprocessDetailsID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Dataset in InsertFeedProcess method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Dataset in InsertFeedProcess method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Dataset in InsertFeedProcess method | UserName: {0}", Username), ex.InnerException);
                if (ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Dataset in InsertFeedProcess method | UserName: {0}", Username), ex.InnerException.InnerException);
                throw;
            }
        }
        public List<string> CheckTestCaseExistsInStoryboard(long testcaseId)
        {
            try
            {
                logger.Info(string.Format("Check TestCase Exists In Storyboard start | testcaseId: {0} | UserName: {1}", testcaseId, Username));
                List<string> storyboarname = new List<string>();
                var lStoryboardList = entity.T_PROJ_TC_MGR.Where(x => x.TEST_CASE_ID == testcaseId).ToList();

                if (lStoryboardList.Count() > 0)
                {
                    foreach (var item in lStoryboardList)
                    {
                        var sname = entity.T_STORYBOARD_SUMMARY.Find(item.STORYBOARD_ID);

                        storyboarname.Add(sname.STORYBOARD_NAME);
                        storyboarname = (from w in storyboarname select w).Distinct().ToList();
                    }
                    logger.Info(string.Format("Check TestCase Exists In Storyboard end | testcaseId: {0} | UserName: {1}", testcaseId, Username));
                    return storyboarname;
                }
                logger.Info(string.Format("Check TestCase Exists In Storyboard end | testcaseId: {0} | UserName: {1}", testcaseId, Username));
                return storyboarname;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckTestCaseExistsInStoryboard method | testcaseId: {0} | UserName: {1}", testcaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckTestCaseExistsInStoryboard method | testcaseId: {0} | UserName: {1}", testcaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckTestCaseExistsInStoryboard method | testcaseId: {0} | UserName: {1}", testcaseId, Username), ex.InnerException);
                throw;
            }
        }
        public string GetTestcaseNameById(long caseId)
        {
            try
            {
                logger.Info(string.Format("Get TestcaseName start | testcaseId: {0} | UserName: {1}", caseId, Username));
                var lcaseName = entity.T_TEST_CASE_SUMMARY.FirstOrDefault(x => x.TEST_CASE_ID == caseId).TEST_CASE_NAME;
                logger.Info(string.Format("Get TestcaseName end | testcaseId: {0} | UserName: {1}", caseId, Username));
                return lcaseName;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTestcaseNameById method | testcaseId: {0} | UserName: {1}", caseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestcaseNameById method | testcaseId: {0} | UserName: {1}", caseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetTestcaseNameById method | testcaseId: {0} | UserName: {1}", caseId, Username), ex.InnerException);
                throw;
            }

        }
        public string MoveTestCase(long projectId, long testcaseid, long testsuiteid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Move TestCase start | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseid, testsuiteid, Username));
                    var result = entity.REL_TEST_CASE_TEST_SUITE.FirstOrDefault(x => x.TEST_CASE_ID == testcaseid);
                    if (result != null)
                    {
                        result.TEST_CASE_ID = testcaseid;
                        result.TEST_SUITE_ID = testsuiteid;
                        entity.SaveChanges();
                        logger.Info(string.Format("Move TestCase end | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseid, testsuiteid, Username));
                        scope.Complete();
                        return "success";
                    }
                }
                logger.Info(string.Format("Move TestCase end | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseid, testsuiteid, Username));
                return "error";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in MoveTestCase method | Project Id : {0} | TestSuite Id : {1} | testcaseId: {2} | UserName: {3}", projectId, testsuiteid, testcaseid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in MoveTestCase method | Project Id : {0} | TestSuite Id : {1} | testcaseId: {2} | UserName: {3}", projectId, testsuiteid, testcaseid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in MoveTestCase method | Project Id : {0} | TestSuite Id : {1} | testcaseId: {2} | UserName: {3}", projectId, testsuiteid, testcaseid, Username), ex.InnerException);
                throw;
            }
        }
        public string ValidateSave(int feedprocessid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("ValidateSave start | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                    ObjectParameter op = new ObjectParameter("RESULT", "");
                    entity.USP_MAPPING_VALIDATION(feedprocessid, op);
                    entity.SaveChanges();
                    logger.Info(string.Format("ValidateSave end | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                    scope.Complete();
                    return "validated";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ValidateSave method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ValidateSave method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in ValidateSave method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username), ex.InnerException);
                throw;
            }
        }
        public string SaveData(int feedprocessid, string lstrConn, string schema)
        {
            try
            {
                logger.Info(string.Format("Save Testcase Data start | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
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
                ladd_refer_image[0] = new OracleParameter("FEEDPROCESSID1", OracleDbType.Long);
                ladd_refer_image[0].Value = feedprocessid;

                ladd_refer_image[1] = new OracleParameter("ISOVERWRITE", OracleDbType.Long);
                ladd_refer_image[1].Value = 1;

                ladd_refer_image[2] = new OracleParameter("RESULT", OracleDbType.Clob);
                ladd_refer_image[2].Direction = ParameterDirection.Output;


                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "USP_FEEDPROCESSMAPPING_WEB_D";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                logger.Info(string.Format("Save Testcase Data end | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                return "saved";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveData method | FeedProcess Id : {0} | Connection String : {1} | Schema : {2} | UserName: {3}", feedprocessid, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveData method | FeedProcess Id : {0} | Connection String : {1} | Schema : {2} | UserName: {3}", feedprocessid, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveData method | FeedProcess Id : {0} | Connection String : {1} | Schema : {2} | UserName: {3}", feedprocessid, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }
        public string Validate(int feedprocessid)
        {
            try
            {
                logger.Info(string.Format(" Validation start | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                if (entity.TBLLOGREPORTs.Where(x => x.FEEDPROCESSID == feedprocessid).Count() == 0)
                {
                    logger.Info(string.Format(" Validation end | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                    return "Success";
                }
                else
                {
                    logger.Info(string.Format(" Validation end | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                    return "Validation Failed";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in Validate method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in Validate method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in Validate method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username), ex.InnerException);
                throw;
            }
        }
        public List<TBLLOGREPORT> GetValidations(int feedprocessid)
        {
            try
            {
                logger.Info(string.Format("get Testcase Validation start | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                var result = entity.TBLLOGREPORTs.Where(x => x.FEEDPROCESSID == feedprocessid).ToList();
                logger.Info(string.Format("get Testcase Validation end | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetValidations method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetValidations method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetValidations method | FeedProcess Id : {0} | UserName: {1}", feedprocessid, Username), ex.InnerException);
                throw;
            }
        }
        public string DeleteStep(int stepID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete Step start | stepID: {0} | UserName: {1}", stepID, Username));
                    var testReport = entity.T_TEST_REPORT_STEPS.Where(x => x.STEPS_ID == stepID).ToList();

                    foreach (var v in testReport)
                    {
                        entity.T_TEST_REPORT_STEPS.Remove(v);
                    }
                    //rutvi
                    //entity.SaveChanges();

                    var stepsSettings = entity.TEST_DATA_SETTING.Where(x => x.STEPS_ID == stepID).ToList();

                    foreach (var v in stepsSettings)
                    {
                        entity.TEST_DATA_SETTING.Remove(v);
                    }
                    //rutvi
                    //entity.SaveChanges();

                    var tStep = entity.T_TEST_STEPS.Where(x => x.STEPS_ID == stepID).ToList();
                    foreach (var t in tStep)
                    {
                        entity.T_TEST_STEPS.Remove(t);
                    }

                    entity.SaveChanges();
                    logger.Info(string.Format("Delete Step end | stepID: {0} | UserName: {1}", stepID, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {

                logger.Error(string.Format("Error occured TestCase in DeleteStep method | FeedProcess Id : {0} | UserName: {1}", stepID, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteStep method | FeedProcess Id : {0} | UserName: {1}", stepID, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in DeleteStep method | FeedProcess Id : {0} | UserName: {1}", stepID, Username), ex.InnerException);
                throw;
            }
        }

        public string AddStep(long TestCaseId, long TestSuiteId, string lKeywordName, string lComment, string lObjectName, string lParameter, long RunOrder = 0)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("AddStep start | TestCaseId: {0} | TestSuiteId: {1} | UserName: {2}", TestCaseId, TestSuiteId, Username));
                    var lKeyword = entity.T_KEYWORD.Where(x => x.KEY_WORD_NAME.ToUpper() == lKeywordName.ToUpper()).ToList();
                    long lKeywordId = 0;
                    if (lKeyword.Count() > 0)
                    {
                        lKeywordId = lKeyword.FirstOrDefault().KEY_WORD_ID;
                    }

                    var lObject = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_HAPPY_NAME.ToUpper() == lObjectName.ToUpper()).ToList();
                    long lObjectNameId = 0;
                    long lObjectId = 0;
                    if (lObject.Count() > 0)
                    {
                        lObjectId = lObject.FirstOrDefault().OBJECT_ID;
                        lObjectNameId = (long)lObject.FirstOrDefault().OBJECT_NAME_ID;
                    }

                    var ltbl = new T_TEST_STEPS();
                    ltbl.STEPS_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    ltbl.TEST_CASE_ID = TestCaseId;
                    ltbl.RUN_ORDER = RunOrder;
                    ltbl.KEY_WORD_ID = lKeywordId;
                    ltbl.OBJECT_ID = lObjectId;
                    ltbl.OBJECT_NAME_ID = lObjectNameId;
                    ltbl.COLUMN_ROW_SETTING = lParameter;
                    ltbl.COMMENT = lComment;
                    entity.T_TEST_STEPS.Add(ltbl);
                    entity.SaveChanges();

                    logger.Info(string.Format("AddStep end | TestCaseId: {0} | TestSuiteId: {1} | UserName: {2}", TestCaseId, TestSuiteId, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in AddStep method | TestSuite Id : {0} | TestCase Id : {1} | Keyword : {2} | Comment : {3} | Object : {4} | Parameter : {5} | UserName: {6}", TestSuiteId, TestCaseId, lKeywordName, lComment, lObjectName, lParameter, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in AddStep method | TestSuite Id : {0} | TestCase Id : {1} | Keyword : {2} | Comment : {3} | Object : {4} | Parameter : {5} | UserName: {6}", TestSuiteId, TestCaseId, lKeywordName, lComment, lObjectName, lParameter, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in AddStep method | TestSuite Id : {0} | TestCase Id : {1} | Keyword : {2} | Comment : {3} | Object : {4} | Parameter : {5} | UserName: {6}", TestSuiteId, TestCaseId, lKeywordName, lComment, lObjectName, lParameter, Username), ex.InnerException);
                throw;
            }
        }

        public void UpdateSteps(int testcaseId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Update Steps start | TestcaseId: {0} | UserName: {1}", testcaseId, Username));
                    var steps = entity.T_TEST_STEPS.OrderBy(x => x.RUN_ORDER).Where(x => x.TEST_CASE_ID == testcaseId).ToList();

                    int counter = 1;
                    foreach (var st in steps)
                    {
                        st.RUN_ORDER = counter;
                        counter++;
                    }
                    entity.SaveChanges();
                    logger.Info(string.Format("Update Steps end | TestcaseId: {0} | UserName: {1}", testcaseId, Username));
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateSteps method | TestCase Id : {0} | UserName: {1}", testcaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateSteps method | TestCase Id : {0} | UserName: {1}", testcaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateSteps method | TestCase Id : {0} | UserName: {1}", testcaseId, Username), ex.InnerException);
                throw;
            }
        }
        public void UpdateStepID(int stepsId, int RUN_ORDER)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Update StepID start | stepsId: {0} | UserName: {1}", stepsId, Username));
                    var step = entity.T_TEST_STEPS.Find(stepsId);
                    if (step != null)
                    {
                        step.RUN_ORDER = RUN_ORDER;
                    }
                    entity.SaveChanges();
                    logger.Info(string.Format("Update StepID end | stepsId: {0} | UserName: {1}", stepsId, Username));
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateStepID method | Step Id : {0} | Run Order: {1} | UserName: {2}", stepsId, RUN_ORDER, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateStepID method | Step Id : {0} | Run Order: {1} | UserName: {2}", stepsId, RUN_ORDER, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateStepID method | Step Id : {0} | Run Order: {1} | UserName: {2}", stepsId, RUN_ORDER, Username), ex.InnerException);
                throw;
            }
        }
        public string GetTestCaseNameById(long CaseId)
        {
            try
            {
                logger.Info(string.Format("Get TestCaseName start | TestCaseId: {0} | UserName: {1}", CaseId, Username));
                var lCaseName = entity.T_TEST_CASE_SUMMARY.FirstOrDefault(x => x.TEST_CASE_ID == CaseId).TEST_CASE_NAME;
                logger.Info(string.Format("Get TestCaseName end | TestCaseId: {0} | UserName: {1}", CaseId, Username));
                return lCaseName;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTestCaseNameById method | testcaseId: {0} | UserName: {1}", CaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseNameById method | testcaseId: {0} | UserName: {1}", CaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetTestCaseNameById method | testcaseId: {0} | UserName: {1}", CaseId, Username), ex.InnerException);
                throw;
            }
        }
        public string UpdateRunOrder(int stepId, int newRun_Order)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("UpdateRunOrder start | stepId: {0} | newRun_Order: {1} | UserName: {2}", stepId, newRun_Order, Username));
                    var step = entity.T_TEST_STEPS.Where(x => x.STEPS_ID == stepId).FirstOrDefault();
                    if (step != null)
                    {
                        step.RUN_ORDER = newRun_Order;
                        entity.SaveChanges();
                    }
                    logger.Info(string.Format("UpdateRunOrder end | stepId: {0} | newRun_Order: {1} | UserName: {2}", stepId, newRun_Order, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateRunOrder method | Step Id : {0} | Run Order: {1} | UserName: {2}", stepId, newRun_Order, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateRunOrder method | Step Id : {0} | Run Order: {1} | UserName: {2}", stepId, newRun_Order, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateRunOrder method | Step Id : {0} | Run Order: {1} | UserName: {2}", stepId, newRun_Order, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> CheckDatasetInStoryboard(long datasetid)
        {
            try
            {
                logger.Info(string.Format("Check Dataset In Storyboard start | DatasetId: {0} | UserName: {1}", datasetid, Username));
                var result = entity.T_STORYBOARD_DATASET_SETTING.Where(x => x.DATA_SUMMARY_ID == datasetid).ToList();
                List<string> llist = new List<string>();
                var resultitem = new List<T_STORYBOARD_SUMMARY>();
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        resultitem = (from t in entity.T_STORYBOARD_SUMMARY
                                      join t1 in entity.T_PROJ_TC_MGR on t.STORYBOARD_ID equals t1.STORYBOARD_ID
                                      join t2 in entity.T_STORYBOARD_DATASET_SETTING on t1.STORYBOARD_DETAIL_ID equals t2.STORYBOARD_DETAIL_ID
                                      where t2.STORYBOARD_DETAIL_ID == item.STORYBOARD_DETAIL_ID
                                      select t).ToList();
                        foreach (var itm in resultitem)
                        {
                            llist.Add(itm.STORYBOARD_NAME);
                        }
                        llist = llist.Distinct().ToList();
                    }
                }
                logger.Info(string.Format("Check Dataset In Storyboard end | DatasetId: {0} | UserName: {1}", datasetid, Username));
                return llist;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDatasetInStoryboard method | Dataset Id : {0} | UserName: {1}", datasetid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDatasetInStoryboard method | Dataset Id : {0} | UserName: {1}", datasetid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckDatasetInStoryboard method | Dataset Id : {0} | UserName: {1}", datasetid, Username), ex.InnerException);
                throw;
            }
        }
        public string DeleteRelTestCaseDataSummary(long testCaseId, long dataSetId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete RelTestCaseDataSummary start | TestCaseId: {0} | dataSetId: {1} | UserName: {2}", testCaseId, dataSetId, Username));
                    var dataseting = (from setting in entity.TEST_DATA_SETTING
                                      where dataSetId == setting.DATA_SUMMARY_ID
                                      select setting);

                    foreach (var item in dataseting)
                    {
                        entity.TEST_DATA_SETTING.Remove(item);
                        //rutvi
                        //entity.SaveChanges();
                    }

                    var relTcDataSummary = (from rt in entity.REL_TC_DATA_SUMMARY
                                            where rt.TEST_CASE_ID == testCaseId && rt.DATA_SUMMARY_ID == dataSetId
                                            select rt).FirstOrDefault();
                    if (relTcDataSummary != null)
                        entity.REL_TC_DATA_SUMMARY.Remove(relTcDataSummary);
                    entity.SaveChanges();

                    var datasetobjlist = entity.REL_TC_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == dataSetId).ToList();

                    if (!datasetobjlist.Any())
                    {
                        var obj1 = entity.T_SHARED_OBJECT_POOL.Where(x => x.DATA_SUMMARY_ID == dataSetId).ToList();
                        logger.Info(string.Format("Delete Dataset in T_SHARED_OBJECT_POOL start | DataSetId: {0} | UserName: {1}", dataSetId, Username));
                        foreach (var itm in obj1)
                        {
                            entity.T_SHARED_OBJECT_POOL.Remove(itm);
                        }
                        logger.Info(string.Format("Delete Dataset in T_SHARED_OBJECT_POOL end | DataSetId: {0} | UserName: {1}", dataSetId, Username));
                        //entity.SaveChanges();

                        var datasetobj = entity.T_TEST_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == dataSetId).FirstOrDefault();

                        logger.Info(string.Format("Delete Dataset  start | DataSetId: {0} | UserName: {1}", dataSetId, Username));
                        if (datasetobj != null)
                            entity.T_TEST_DATA_SUMMARY.Remove(datasetobj);
                        entity.SaveChanges();

                        logger.Info(string.Format("Delete Dataset  end | DataSetId: {0} | UserName: {1}", dataSetId, Username));
                    }

                    var sharedObject = (from rt in entity.T_SHARED_OBJECT_POOL
                                        where rt.DATA_SUMMARY_ID == dataSetId
                                        select rt);
                    foreach (var item in sharedObject)
                    {
                        entity.T_SHARED_OBJECT_POOL.Remove(item);
                        //rutiv
                        //entity.SaveChanges();
                    }


                    var datasummary = (from rt in entity.T_TEST_DATA_SUMMARY
                                       where rt.DATA_SUMMARY_ID == dataSetId
                                       select rt).FirstOrDefault();
                    if (datasummary != null)
                        entity.T_TEST_DATA_SUMMARY.Remove(datasummary);
                    entity.SaveChanges();



                    // entity.SaveChanges();
                    logger.Info(string.Format("Delete RelTestCaseDataSummary end | TestCaseId: {0} | dataSetId: {1} | UserName: {2}", testCaseId, dataSetId, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in DeleteRelTestCaseDataSummary method | TestCaseId: {0} | dataSetId: {1} | UserName: {2}", testCaseId, dataSetId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteRelTestCaseDataSummary method | TestCaseId: {0} | dataSetId: {1} | UserName: {2}", testCaseId, dataSetId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in DeleteRelTestCaseDataSummary method | TestCaseId: {0} | dataSetId: {1} | UserName: {2}", testCaseId, dataSetId, Username), ex.InnerException);
                throw;
            }
        }

        public string AddTestDataSet(long? testCaseId, long? datasetid, string DataSetName, string DataSetDesc, DataSetTagModel tagmodel, string lConnectionStr, string lSchema)
        {
            try
            {
                logger.Info(string.Format("Add TestDataSet start | TestCaseId: {0} | DataSetName: {1} | UserName: {2}", testCaseId, DataSetName, Username));
                if (!string.IsNullOrEmpty(DataSetName))
                {
                    DataSetName = DataSetName.Trim();
                }
                var lresult = CheckDuplicateDataset(DataSetName, datasetid);
                if (lresult == true)
                {
                    logger.Info(string.Format("CheckDuplicateDataset lresult: {0}", lresult));
                    return datasetid + ",error";
                }
                var ds = entity.T_TEST_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == datasetid).SingleOrDefault();
                if (ds == null && lresult == false)
                {
                    if (DataSetDesc == null)
                    {
                        DataSetDesc = "";
                    }

                    var lDataSummary = new T_TEST_DATA_SUMMARY();
                    lDataSummary.DATA_SUMMARY_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    lDataSummary.ALIAS_NAME = DataSetName;
                    lDataSummary.DESCRIPTION_INFO = DataSetDesc;
                    entity.T_TEST_DATA_SUMMARY.Add(lDataSummary);
                    //entity.SaveChanges();// -- check
                    var ldatasetid = lDataSummary.DATA_SUMMARY_ID;

                    var lRelTCDataSet = new REL_TC_DATA_SUMMARY();
                    lRelTCDataSet.ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    lRelTCDataSet.TEST_CASE_ID = testCaseId;
                    lRelTCDataSet.DATA_SUMMARY_ID = lDataSummary.DATA_SUMMARY_ID;
                    entity.REL_TC_DATA_SUMMARY.Add(lRelTCDataSet);
                    entity.SaveChanges();

                    logger.Info(string.Format("Add TestDataSet end | TestCaseId: {0} | DataSetName: {1} | UserName: {2}", testCaseId, DataSetName, Username));
                    return ldatasetid + ",success";

                }
                else
                {
                    ds.ALIAS_NAME = DataSetName;
                    ds.DESCRIPTION_INFO = DataSetDesc;
                    entity.SaveChanges();

                    var seqcheck = entity.T_TEST_DATASETTAG.Where(x => x.FOLDERID == tagmodel.Folderid.ToString() && x.SEQUENCE == tagmodel.Sequence && x.DATASETID != datasetid).ToList();
                    if (seqcheck.Count > 0)
                    {
                        return datasetid + ",sequence error";
                    }

                    //find list
                    //var result = entity.T_TEST_DATASETTAG.Where(x => x.DATASETID == datasetid).FirstOrDefault();
                    var result = entity.T_TEST_DATASETTAG.Any(x => x.DATASETID == datasetid);
                    logger.Info(string.Format("result 9 result -->T_TEST_DATASETTAG {0} ", result));

                    if (result)
                    {
                        //result.FOLDERID = tagmodel.Folderid;
                        //result.SETID = tagmodel.Setid;
                        //result.GROUPID = tagmodel.Groupid;
                        //result.STEPDESC = tagmodel.StepDesc;
                        //result.SEQUENCE = tagmodel.Sequence;
                        //result.EXPECTEDRESULTS = tagmodel.Expectedresults;
                        //result.DIARY = tagmodel.Diary;
                        //logger.Info(string.Format("result 10 start -->T_TEST_DATASETTAG "));
                        //entity.SaveChanges();
                        //logger.Info(string.Format("result 10 end -->T_TEST_DATASETTAG "));

                        //changed oracle connection to context connection
                        //OracleTransaction ltransaction;
                        //OracleConnection lconnection = new OracleConnection(lConnectionStr);
                        var lconnection = entity.Database.Connection;
                        lconnection.Open();
                        using (var ltransaction = lconnection.BeginTransaction())
                        {
                            string lcmdquery = "UPDATE " + lSchema + ".T_TEST_DATASETTAG SET FOLDERID = :1, SETID = :2, GROUPID = :3, STEPDESC = :4, " +
                                "SEQUENCE = :5, EXPECTEDRESULTS = :6, DIARY = :7 WHERE DATASETID = :8";

                            using (var lcmd = lconnection.CreateCommand())
                            {
                                lcmd.CommandText = lcmdquery;

                                OracleParameter FOLDERID_oparam = new OracleParameter();
                                FOLDERID_oparam.OracleDbType = OracleDbType.Long;
                                FOLDERID_oparam.Value = tagmodel.Folderid;

                                OracleParameter SETID_oparam = new OracleParameter();
                                SETID_oparam.OracleDbType = OracleDbType.Long;
                                SETID_oparam.Value = tagmodel.Setid;

                                OracleParameter GROUPID_oparam = new OracleParameter();
                                GROUPID_oparam.OracleDbType = OracleDbType.Long;
                                GROUPID_oparam.Value = tagmodel.Groupid;

                                OracleParameter STEPDESC_oparam = new OracleParameter();
                                STEPDESC_oparam.OracleDbType = OracleDbType.Varchar2;
                                STEPDESC_oparam.Value = tagmodel.StepDesc;

                                OracleParameter SEQUENCE_oparam = new OracleParameter();
                                SEQUENCE_oparam.OracleDbType = OracleDbType.Decimal;
                                SEQUENCE_oparam.Value = tagmodel.Sequence;

                                OracleParameter EXPECTEDRESULTS_oparam = new OracleParameter();
                                EXPECTEDRESULTS_oparam.OracleDbType = OracleDbType.Varchar2;
                                EXPECTEDRESULTS_oparam.Value = tagmodel.Expectedresults;

                                OracleParameter DIARY_oparam = new OracleParameter();
                                DIARY_oparam.OracleDbType = OracleDbType.Varchar2;
                                DIARY_oparam.Value = tagmodel.Diary;


                                OracleParameter DATASETID_oparam = new OracleParameter();
                                DATASETID_oparam.OracleDbType = OracleDbType.Long;
                                DATASETID_oparam.Value = datasetid;

                                lcmd.Parameters.Add(FOLDERID_oparam);
                                lcmd.Parameters.Add(SETID_oparam);
                                lcmd.Parameters.Add(GROUPID_oparam);
                                lcmd.Parameters.Add(STEPDESC_oparam);
                                lcmd.Parameters.Add(SEQUENCE_oparam);
                                lcmd.Parameters.Add(EXPECTEDRESULTS_oparam);
                                lcmd.Parameters.Add(DIARY_oparam);
                                lcmd.Parameters.Add(DATASETID_oparam);

                                lcmd.ExecuteNonQuery();

                                ltransaction.Commit();
                                lconnection.Close();
                            }
                        }

                    }
                    else
                    {
                        if (tagmodel.Folderid != 0 || tagmodel.Sequence != null || tagmodel.Groupid != 0 || tagmodel.Setid != 0 || tagmodel.StepDesc != null || tagmodel.Expectedresults != null || tagmodel.Diary != null)
                        {
                            var dtmodel = new T_TEST_DATASETTAG();
                            logger.Info(string.Format("result 11 start -->SEQ_T_TEST_DATASETTAG "));
                            dtmodel.T_TEST_DATASETTAG_ID = Helper.NextTestSuiteId("SEQ_T_TEST_DATASETTAG");
                            logger.Info(string.Format("result 11 end -->SEQ_T_TEST_DATASETTAG "));
                            dtmodel.DATASETID = datasetid;
                            dtmodel.FOLDERID = tagmodel.Folderid.ToString();
                            dtmodel.SETID = tagmodel.Setid.ToString();
                            dtmodel.GROUPID = tagmodel.Groupid.ToString();
                            dtmodel.STEPDESC = tagmodel.StepDesc;
                            dtmodel.SEQUENCE = (long)tagmodel.Sequence;
                            dtmodel.EXPECTEDRESULTS = tagmodel.Expectedresults;
                            dtmodel.DIARY = tagmodel.Diary;
                            logger.Info(string.Format("result 12 start -->T_TEST_DATASETTAG "));
                            entity.T_TEST_DATASETTAG.Add(dtmodel);
                            entity.SaveChanges();
                            logger.Info(string.Format("result 12 end -->T_TEST_DATASETTAG "));
                        }
                    }
                    logger.Info(string.Format("Add TestDataSet end | TestCaseId: {0} | DataSetName: {1} | UserName: {2}", testCaseId, DataSetName, Username));
                    return datasetid + ",success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Dataset in AddTestDataSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Dataset in AddTestDataSet method | UserName: {0}", Username), ex);
                ELogger.ErrorException(string.Format("Inner Error occured Dataset in AddTestDataSet method | UserName: {0}", Username), ex.InnerException);
                ELogger.ErrorException(string.Format("Inner Error occured Dataset in AddTestDataSet method | UserName: {0}", Username), ex.InnerException.InnerException);
                ELogger.ErrorException(string.Format("Inner Error occured Dataset in AddTestDataSet method | UserName: {0}", Username), ex.InnerException.InnerException.InnerException);
                ELogger.ErrorException(string.Format("stack occured Dataset in AddTestDataSet method | UserName: {0}", ex.StackTrace), ex);
                throw;
            }
        }

        public bool DeleteTestCase(long TestCaseId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var datasetids = entity.REL_TC_DATA_SUMMARY.Where(x => x.TEST_CASE_ID == TestCaseId).Select(x => x.DATA_SUMMARY_ID).ToList();
                    logger.Info(string.Format("Delete TestCase start | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                    var flag = false;
                    entity.DeleteTestCase(TestCaseId);

                    var exsitDatasetIds = entity.REL_TC_DATA_SUMMARY.Where(a => datasetids.Any(b => a.DATA_SUMMARY_ID == b)).Select(x => x.DATA_SUMMARY_ID).ToList();
                    var deleteDatasetids = datasetids.Except(exsitDatasetIds).ToList();

                    foreach (var item in deleteDatasetids)
                    {
                        var obj1 = entity.T_SHARED_OBJECT_POOL.Where(x => x.DATA_SUMMARY_ID == item).ToList();
                        logger.Info(string.Format("Delete Dataset in T_SHARED_OBJECT_POOL start | TestCaseId: {0} | DataSetId: {1} | UserName: {2}", TestCaseId, item, Username));
                        foreach (var itm in obj1)
                        {
                            entity.T_SHARED_OBJECT_POOL.Remove(itm);
                        }
                        //rutvi
                        //entity.SaveChanges();
                        logger.Info(string.Format("Delete  Dataset in T_SHARED_OBJECT_POOL End | TestCaseId: {0} | DataSetId: {1} | UserName: {2}", TestCaseId, item, Username));

                        var obj = entity.T_TEST_DATA_SUMMARY.FirstOrDefault(x => x.DATA_SUMMARY_ID == item);
                        logger.Info(string.Format("Delete Dataset start | TestCaseId: {0} | DataSetId: {1} | UserName: {2}", TestCaseId, item, Username));
                        if (obj != null)
                            entity.T_TEST_DATA_SUMMARY.Remove(obj);
                        entity.SaveChanges();

                        logger.Info(string.Format("Delete Dataset End | TestCaseId: {0} | DataSetId: {1} | UserName: {2}", TestCaseId, item, Username));
                    }

                    flag = true;
                    logger.Info(string.Format("Delete TestCase end | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in DeleteTestCase method | testcaseId: {0} | UserName: {1}", TestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteTestCase method | testcaseId: {0} | UserName: {1}", TestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in DeleteTestCase method | testcaseId: {0} | UserName: {1}", TestCaseId, Username), ex.InnerException);
                throw;
            }
        }
        public List<TestCaseModel> ListAllTestCase(string schema, string lconstring, int startrec, int pagesize, string colname, string colorder, string namesearch, string descsearch, string appsearch, string suitesearch)
        {
            try
            {
                logger.Info(string.Format("ListAllTestCase start | UserName: {0}", Username));
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

                ladd_refer_image[7] = new OracleParameter("SuiteSearch", OracleDbType.Varchar2);
                ladd_refer_image[7].Value = suitesearch;

                ladd_refer_image[8] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[8].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_LIST_TESTCASES";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];


                List<TestCaseModel> resultList = dt.AsEnumerable().Select(row =>
                          new TestCaseModel
                          {
                              TestCaseId = row.Field<long>("testcaseid"),
                              TestCaseName = Convert.ToString(row.Field<string>("casename")),
                              TestCaseDescription = Convert.ToString(row.Field<string>("description")),
                              TestSuiteId = Convert.ToString(row.Field<string>("suiteid")),
                              TestSuite = Convert.ToString(row.Field<string>("Suitename")),
                              ApplicationId = Convert.ToString(row.Field<string>("applicationid")),
                              Application = Convert.ToString(row.Field<string>("Applicationame")),
                              TotalCount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                          }).ToList();

                logger.Info(string.Format("ListAllTestCase end | UserName: {0}", Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ListAllTestCase method | Connection string : {0} | Schema {1} | UserName: {2}", lconstring, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ListAllTestCase method | Connection string : {0} | Schema {1} | UserName: {2}", lconstring, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in ListAllTestCase method | Connection string : {0} | Schema {1} | UserName: {2}", lconstring, schema, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditTestCase(TestCaseModel lEntity, string LoginName)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.TestCaseName))
                    {
                        lEntity.TestCaseName = lEntity.TestCaseName.Trim();
                    }
                    var flag = false;
                    if (lEntity.TestCaseId == 0)
                    {
                        logger.Info(string.Format("Add TestCase start | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username));
                        var tbl = new T_TEST_CASE_SUMMARY();
                        tbl.TEST_CASE_ID = Helper.NextTestSuiteId("T_TEST_CASE_SUMMARY_SEQ");
                        tbl.TEST_CASE_NAME = lEntity.TestCaseName;
                        tbl.TEST_STEP_DESCRIPTION = lEntity.TestCaseDescription;
                        tbl.TEST_STEP_CREATE_TIME = DateTime.Now;
                        tbl.TEST_STEP_CREATOR = LoginName;
                        lEntity.TestCaseId = tbl.TEST_CASE_ID;

                        entity.T_TEST_CASE_SUMMARY.Add(tbl);
                        //rutvi
                        //entity.SaveChanges();
                        logger.Info(string.Format("Add TestCase end | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username));
                        #region insert for default Dataset

                        var tblDataSummary = new T_TEST_DATA_SUMMARY();
                        tblDataSummary.ALIAS_NAME = lEntity.TestCaseName;
                        tblDataSummary.DATA_SUMMARY_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblDataSummary.SHARE_MARK = 1;
                        tblDataSummary.STATUS = 0;
                        tblDataSummary.CREATE_TIME = DateTime.Now;
                        entity.T_TEST_DATA_SUMMARY.Add(tblDataSummary);
                        //rutvi
                        //entity.SaveChanges();


                        var tblMapping = new REL_TC_DATA_SUMMARY();
                        tblMapping.ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblMapping.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        tblMapping.DATA_SUMMARY_ID = tblDataSummary.DATA_SUMMARY_ID;
                        entity.REL_TC_DATA_SUMMARY.Add(tblMapping);
                        //rutvi
                        //entity.SaveChanges();


                        var tblStep = new T_TEST_STEPS();
                        tblStep.STEPS_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblStep.RUN_ORDER = 1;
                        tblStep.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        entity.T_TEST_STEPS.Add(tblStep);
                        entity.SaveChanges();
                        #endregion

                        flag = true;
                    }
                    else
                    {
                        logger.Info(string.Format("Edit TestCase start | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username));
                        var tbl = entity.T_TEST_CASE_SUMMARY.Find(lEntity.TestCaseId);
                        if (tbl != null)
                        {
                            tbl.TEST_CASE_NAME = lEntity.TestCaseName;
                            tbl.TEST_STEP_DESCRIPTION = lEntity.TestCaseDescription;
                            //rutvi
                            //entity.SaveChanges();

                            #region Testcase and Application Mapping Delete
                            var lAppList = entity.REL_APP_TESTCASE.Where(x => x.TEST_CASE_ID == lEntity.TestCaseId).ToList();
                            foreach (var item in lAppList)
                            {
                                entity.REL_APP_TESTCASE.Remove(item);
                            }

                            #endregion

                            #region Testcase and TestSuite Mapping Delete
                            var lTCTSList = entity.REL_TEST_CASE_TEST_SUITE.Where(x => x.TEST_CASE_ID == lEntity.TestCaseId).ToList();
                            foreach (var item in lTCTSList)
                            {
                                entity.REL_TEST_CASE_TEST_SUITE.Remove(item);
                            }
                            entity.SaveChanges();
                            #endregion
                            logger.Info(string.Format("Edit TestCase end | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username));
                        }
                        flag = true;
                    }

                    #region Testcase and Application Mapping Adding
                    if (!string.IsNullOrEmpty(lEntity.ApplicationId))
                    {

                        var lAppSplit = lEntity.ApplicationId.Split(',').Select(Int64.Parse).ToList();

                        if (lAppSplit.Count() > 0)
                        {
                            lAppSplit = lAppSplit.Distinct().ToList();
                            foreach (var item in lAppSplit)
                            {
                                var lApptbl = new REL_APP_TESTCASE();
                                lApptbl.APPLICATION_ID = item;
                                lApptbl.TEST_CASE_ID = lEntity.TestCaseId;
                                lApptbl.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_APP_TESTCASE_SEQ");
                                entity.REL_APP_TESTCASE.Add(lApptbl);
                                //rutvi
                                // entity.SaveChanges(); 
                            }
                        }
                        if (!string.IsNullOrEmpty(lEntity.TestSuiteId))
                        {
                            var lTestSuiteSplit = lEntity.TestSuiteId.Split(',').Select(Int64.Parse).ToList();
                            if (lTestSuiteSplit.Count() > 0)
                            {
                                lTestSuiteSplit = lTestSuiteSplit.Distinct().ToList();
                                foreach (var item in lTestSuiteSplit)
                                {
                                    if (item != 0)
                                    {


                                        var lTSTC = new REL_TEST_CASE_TEST_SUITE();
                                        lTSTC.TEST_CASE_ID = lEntity.TestCaseId;
                                        lTSTC.TEST_SUITE_ID = item;
                                        lTSTC.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_TEST_CASE_TEST_SUITE_SEQ");
                                        entity.REL_TEST_CASE_TEST_SUITE.Add(lTSTC);
                                        //rutvi
                                        //entity.SaveChanges();
                                    }
                                }
                            }

                        }
                        entity.SaveChanges();

                        flag = true;
                    }
                    scope.Complete();
                    return flag;
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in AddEditTestCase method | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in AddEditTestCase method | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in AddEditTestCase method | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username), ex.InnerException);
                throw;
            }
        }
        public string UpdateDataSetName(string datasetName, long dataSetId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("UpdateDataSetName start | datasetName: {0} | UserName: {1}", datasetName, Username));
                    if (!string.IsNullOrEmpty(datasetName))
                    {
                        datasetName = datasetName.Trim();
                    }
                    var ds = entity.T_TEST_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == dataSetId).SingleOrDefault();
                    if (ds != null)
                    {
                        ds.ALIAS_NAME = datasetName;
                    }
                    entity.SaveChanges();
                    logger.Info(string.Format("UpdateDataSetName end | datasetName: {0} | UserName: {1}", datasetName, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateDataSetName method | DataSet Id: {0} | DataSet Name : {1} | UserName: {1}", dataSetId, datasetName, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateDataSetName method | DataSet Id: {0} | DataSet Name : {1} | UserName: {1}", dataSetId, datasetName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateDataSetName method | DataSet Id: {0} | DataSet Name : {1} | UserName: {1}", dataSetId, datasetName, Username), ex.InnerException);
                throw;
            }
        }
        public string UpdateDataSetDescription(string dataSetDescription, long dataSetId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("UpdateDataSetDescription start | dataSetId: {0} | UserName: {1}", dataSetId, Username));
                    var ds = entity.T_TEST_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == dataSetId).SingleOrDefault();
                    if (ds != null)
                    {
                        ds.DESCRIPTION_INFO = dataSetDescription;
                    }
                    entity.SaveChanges();
                    logger.Info(string.Format("UpdateDataSetDescription end | dataSetId: {0} | UserName: {1}", dataSetId, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateDataSetDescription method | DataSet Id: {0} | DataSet Desc : {1} | UserName: {1}", dataSetId, dataSetDescription, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateDataSetDescription method | DataSet Id: {0} | DataSet Desc : {1} | UserName: {1}", dataSetId, dataSetDescription, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateDataSetDescription method | DataSet Id: {0} | DataSet Desc : {1} | UserName: {1}", dataSetId, dataSetDescription, Username), ex.InnerException);
                throw;
            }
        }
        public long GetKeywordIdByName(string keywordName)
        {
            return entity.T_KEYWORD.FirstOrDefault(x => x.KEY_WORD_NAME.Trim().ToLower().Equals(keywordName.Trim().ToLower())).KEY_WORD_ID;
        }
        public ObjectViewModel GetObjectIdByName(string objectName)
        {
            return (from OI in entity.T_OBJECT_NAMEINFO
                    join RO in entity.T_REGISTED_OBJECT on OI.OBJECT_NAME_ID equals RO.OBJECT_NAME_ID
                    where OI.OBJECT_HAPPY_NAME.Trim().ToLower().Equals(objectName.Trim().ToLower())
                    select new ObjectViewModel()
                    {
                        OBJECT_HAPPY_NAME = OI.OBJECT_HAPPY_NAME,
                        OBJECT_ID = RO.OBJECT_ID,
                        OBJECT_TYPE = RO.OBJECT_TYPE,
                        OBJECT_NAME_ID = (decimal)RO.OBJECT_NAME_ID,
                        COMMENT = RO.COMMENT
                    }).FirstOrDefault();
            //return entity.V_OBJECT_SNAPSHOT.FirstOrDefault(x => x.OBJECT_HAPPY_NAME.Trim().ToLower().Equals(objectName.ToLower().Trim()));
        }
        public List<TestCaseResult> ConvertTestcaseJsonToList(Mars_Memory_TestCase testCaseObj, long TestCaseId, string schema, string lstrConn, long UserId, long datasetId = 0)
        {
            List<TestCaseResult> resultList = new List<TestCaseResult>();
            try
            {
                logger.Info(string.Format("Get TestCase Detail start | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));

                string test_suiteName = entity.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_ID == testCaseObj.assignedTestSuiteIDs.FirstOrDefault()).TEST_SUITE_NAME;
                //var lVersion = GetTestCaseVersion(TestCaseId, UserId);
                resultList = testCaseObj.allSteps.Select(x => new TestCaseResult()
                {
                    STEPS_ID = x.STEPS_ID.ToString(),
                    RUN_ORDER = x.RUN_ORDER.ToString(),
                    TEST_CASE_ID = x.TEST_CASE_ID.ToString(),
                    SKIP = string.Join(",", x.dataForDataSets.OrderBy(y => y.DATA_SUMMARY_ID).Select(c => c.SKIP).ToList()),
                    DATASETVALUE = string.Join(",", x.dataForDataSets.OrderBy(y => y.DATA_SUMMARY_ID).Select(c => c.DATASETVALUE).ToList()),
                    DATASETDESCRIPTION = string.Empty, // Need to get
                    DATASETNAME = string.Join(",", testCaseObj.assignedDataSets.OrderBy(y => y.DATA_SUMMARY_ID).Select(c => c.ALIAS_NAME).ToList()),
                    TEST_SUITE_ID = testCaseObj.assignedTestSuiteIDs.FirstOrDefault().ToString(),
                    DATASETIDS = string.Join(",", testCaseObj.assignedDataSets.OrderBy(y => y.DATA_SUMMARY_ID).Select(c => c.DATA_SUMMARY_ID).ToList()),
                    parameter = x.COLUMN_ROW_SETTING,
                    object_happy_name = x.OBJECT_HAPPY_NAME,
                    key_word_name = x.KEY_WORD_NAME,
                    test_step_description = string.Empty, // Need to get
                    test_case_name = x.TEST_CASE_NAME,
                    test_suite_name = test_suiteName,
                    Application = string.Empty, // Need to get application name
                    COMMENT = x.COMMENTINFO,
                    ROW_NUM = 1.ToString(),
                    Data_Setting_Id = string.Join(",", x.dataForDataSets.OrderBy(y => y.DATA_SUMMARY_ID).Select(c => c.Data_Setting_Id).ToList())
                }).ToList();
                resultList = resultList.DistinctBy(x => x.RUN_ORDER).ToList();
                resultList.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.DATASETVALUE))
                        x.DATASETVALUE = x.DATASETVALUE.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", "##").Replace("&apos;", "'");
                    if (!string.IsNullOrEmpty(x.parameter))
                    {
                        if (x.parameter.Contains("\\n") || x.parameter.Contains("\""))
                        {
                            x.parameter = x.parameter.Replace("\\n", "\\n");
                            x.parameter = x.parameter.Replace("\"", "\"");
                        }
                        x.parameter = x.parameter.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",").Replace("&apos;", "'");
                    }
                    if (!string.IsNullOrEmpty(x.COMMENT))
                    {
                        if (x.COMMENT.Contains("\\n"))
                            x.COMMENT = x.COMMENT.Replace("\\n", "\n");
                        x.COMMENT = x.COMMENT.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",").Replace("&apos;", "'");
                    }
                    x.key_word_name = x.key_word_name ?? string.Empty;
                    x.object_happy_name = x.object_happy_name ?? string.Empty;
                });
                //resultList.ForEach(x => x.VERSION = lVersion.VERSION);
                //resultList.ForEach(x => x.ISAVAILABLE = lVersion.ISAVAILABLE);

                //var lEditedUserName = "";
                //if (lVersion.CREATORID > 0)
                //{
                //    var ltblEdited = entity.T_TESTER_INFO.FirstOrDefault(x => x.TESTER_ID == lVersion.CREATORID);
                //    if (ltblEdited != null)
                //        lEditedUserName = ltblEdited.TESTER_LOGIN_NAME;
                //}
                //resultList.ForEach(x => x.EditingUserName = lEditedUserName);
                if (resultList.Count() > 1)
                    resultList = resultList.Where(x => x.RUN_ORDER.Trim() != "0" && !string.IsNullOrEmpty(x.RUN_ORDER)).OrderBy(x => Convert.ToInt32(x.RUN_ORDER)).ToList();

                logger.Info(string.Format("Convert Testcase Json To List end | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ConvertTestcaseJsonToList method | TestCase Id: {0} | Connection String : {1} | Schema : {2} |  UserName: {3}", TestCaseId, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ConvertTestcaseJsonToList method | TestCase Id: {0} | Connection String : {1} | Schema : {2} |  UserName: {3}", TestCaseId, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in ConvertTestcaseJsonToList method | TestCase Id: {0} | Connection String : {1} | Schema : {2} |  UserName: {3}", TestCaseId, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }
        public IList<TestCaseResult> GetTestCaseDetail(long TestCaseId, string schema, string lstrConn, long UserId, long datasetId = 0)
        {
            try
            {
                logger.Info(string.Format("Get TestCase Detail start | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;


                var testsuiteid = entity.REL_TEST_CASE_TEST_SUITE.FirstOrDefault(x => x.TEST_CASE_ID == TestCaseId).TEST_SUITE_ID;
                OracleParameter[] ladd_refer_image = new OracleParameter[4];
                ladd_refer_image[0] = new OracleParameter("TESTSUITEID", OracleDbType.Long);
                ladd_refer_image[0].Value = testsuiteid;

                ladd_refer_image[1] = new OracleParameter("TESTCASEID", OracleDbType.Long);
                ladd_refer_image[1].Value = TestCaseId;

                ladd_refer_image[2] = new OracleParameter("DATASETID", OracleDbType.Long);
                ladd_refer_image[2].Value = datasetId;

                ladd_refer_image[3] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[3].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GetTestCaseDetail";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                var dt = new DataTable();
                dt = lds.Tables[0];


                List<TestCaseResult> resultList = dt.AsEnumerable().Select(row =>
                    new TestCaseResult
                    {
                        STEPS_ID = Convert.ToString(row.Field<long?>("STEPS_ID")),
                        RUN_ORDER = Convert.ToString(row.Field<decimal?>("RUN_ORDER")),
                        TEST_CASE_ID = Convert.ToString(row.Field<long?>("TEST_CASE_ID")),
                        SKIP = row.Field<string>("SKIP"),
                        DATASETVALUE = row.Field<string>("DATASETVALUE"),
                        DATASETDESCRIPTION = row.Field<string>("DATASETDESCRIPTION"),
                        DATASETNAME = row.Field<string>("DATASETNAME"),
                        TEST_SUITE_ID = Convert.ToString(row.Field<long?>("TEST_SUITE_ID")),
                        DATASETIDS = row.Field<string>("DATASETIDS"),
                        parameter = row.Field<string>("parameter"),
                        object_happy_name = row.Field<string>("object_happy_name"),
                        key_word_name = row.Field<string>("key_word_name"),
                        test_step_description = row.Field<string>("test_step_description"),
                        test_case_name = row.Field<string>("test_case_name"),
                        test_suite_name = row.Field<string>("test_suite_name"),
                        Application = row.Field<string>("Application"),
                        COMMENT = row.Field<string>("COMMENT"),
                        ROW_NUM = Convert.ToString(row.Field<decimal?>("ROW_NUM")),
                        Data_Setting_Id = row.Field<string>("Data_Setting_Id"),

                    }).ToList();

                foreach (var item in resultList)
                {
                    var flag = false;

                    if (item.DATASETVALUE != null)
                    {
                        item.DATASETVALUE = item.DATASETVALUE.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", "##").Replace("&apos;", "'");
                        //if (item.DATASETVALUE.Contains("&amp;") || item.DATASETVALUE.Contains("~") || item.DATASETVALUE.Contains(@"\") || item.DATASETVALUE.Contains("&quot;"))
                        //{
                        //    item.DATASETVALUE = item.DATASETVALUE.Replace("&amp;", "&");
                        //    item.DATASETVALUE = item.DATASETVALUE.Replace("~", "##");
                        //    item.DATASETVALUE = item.DATASETVALUE.Replace("&quot;", "\"");
                        //}
                    }
                    if (item.parameter != null)
                    {
                        if (item.parameter.Contains("\\n") || item.parameter.Contains("\""))
                        {
                            item.parameter = item.parameter.Replace("\\n", "\\n");
                            item.parameter = item.parameter.Replace("\"", "\"");
                        }
                        item.parameter = item.parameter.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",").Replace("&apos;", "'");
                    }
                    if (item.COMMENT != null)
                    {
                        if (item.COMMENT.Contains("\\n"))
                        {
                            item.COMMENT = item.COMMENT.Replace("\\n", "\n");
                        }
                        item.COMMENT = item.COMMENT.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",").Replace("&apos;", "'");
                    }
                    item.key_word_name = item.key_word_name == null ? "" : item.key_word_name;
                    item.object_happy_name = item.object_happy_name == null ? "" : item.object_happy_name;
                }


                var lVersion = GetTestCaseVersion(TestCaseId, UserId);
                resultList.ForEach(x => x.VERSION = lVersion.VERSION);
                resultList.ForEach(x => x.ISAVAILABLE = lVersion.ISAVAILABLE);
                var lEditedUserName = "";
                if (lVersion.CREATORID > 0)
                {
                    var ltblEdited = entity.T_TESTER_INFO.FirstOrDefault(x => x.TESTER_ID == lVersion.CREATORID);
                    if (ltblEdited != null)
                    {
                        lEditedUserName = ltblEdited.TESTER_LOGIN_NAME;
                    }
                }
                resultList.ForEach(x => x.EditingUserName = lEditedUserName);
                if (resultList.Count() > 1)
                {
                    resultList = resultList.Where(x => x.RUN_ORDER != "0" && !string.IsNullOrEmpty(x.RUN_ORDER)).OrderBy(x => Convert.ToInt32(x.RUN_ORDER)).ToList();
                }

                logger.Info(string.Format("Get TestCase Detail end | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTestCaseDetail method | TestCase Id: {0} | Connection String : {1} | Schema : {2} |  UserName: {3}", TestCaseId, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseDetail method | TestCase Id: {0} | Connection String : {1} | Schema : {2} |  UserName: {3}", TestCaseId, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetTestCaseDetail method | TestCase Id: {0} | Connection String : {1} | Schema : {2} |  UserName: {3}", TestCaseId, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }

        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }

        public List<KeywordObjectLink> CheckObjectKeywordValidation(List<KeywordObjectLink> lList, long lTestCaseId)
        {
            try
            {
                logger.Info(string.Format("CheckObjectKeywordValidation start | datasetName: {0} | UserName: {1}", lTestCaseId, Username));
                long lParentPegwindowId = 0;
                var lObjectPegName = new List<string>();
                var lTypeId = entity.T_GUI_COMPONENT_TYPE_DIC.FirstOrDefault(x => x.TYPE_NAME.ToUpper() == "PEGWINDOW").TYPE_ID;
                var lApplicationList = entity.REL_APP_TESTCASE.Where(x => x.TEST_CASE_ID == lTestCaseId).Select(y => y.APPLICATION_ID).ToList();

                var fristPegWindowId = lList.Where(x => x.Keyword == "PegWindow").Select(x => x.Id).FirstOrDefault();
                var checkpegwindow = lList.Where(x => x.Keyword == "PegWindow").ToList();
                if (fristPegWindowId == 0 && checkpegwindow.Count == 0)
                {
                    if (lList.Count > 0)
                    {
                        fristPegWindowId = lList.LastOrDefault().Id;
                    }
                }

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
                // var lKeywordTypeIdList = entity.T_DIC_RELATION_KEYWORD.Where(x => x.TYPE_ID == lTypeId).ToList();
                foreach (KeywordObjectLink item in lList)
                {
                    if (fristPegWindowId != null)
                    {
                        if (item.Id <= fristPegWindowId)
                        {
                            var inExistOrNot = lKeywordList.Contains(item.Keyword.ToString().ToLower());
                            if (!inExistOrNot)
                            {
                                item.ValidationMsg = item.ValidationMsg + "[" + item.Keyword + "] does not exist in Keyword list. ";
                                item.IsNotValid = true;
                            }
                        }
                    }
                    //first check pegwindow or not
                    var lKeyword = entity.T_KEYWORD.FirstOrDefault(x => x.KEY_WORD_NAME == item.Keyword);
                    if (lKeyword != null)//&& item.Keyword.ToUpper() == "PEGWINDOW"
                    {
                        var lKeywordId = lKeyword.KEY_WORD_ID;
                        var lKeywordType = entity.T_DIC_RELATION_KEYWORD.Where(x => x.KEY_WORD_ID == lKeywordId).Select(y => y.TYPE_ID).ToList();

                        if (lKeywordType != null)
                        {
                            // var lKeywordTypeId = lKeywordType.TYPE_ID;                      
                            if (lKeywordType.Contains(lTypeId) && item.Keyword.ToUpper() == "PEGWINDOW")
                            {
                                lParentPegwindowId = (long)lKeywordType.First().Value;
                                if (!string.IsNullOrEmpty(item.Object))
                                {
                                    var lObjectName = entity.T_OBJECT_NAMEINFO.FirstOrDefault(x => x.OBJECT_HAPPY_NAME == item.Object);
                                    if (lObjectName != null)
                                    {
                                        var lObject = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == lObjectName.OBJECT_NAME_ID).ToList();
                                        if (lObject != null)
                                        {
                                            lObjectPegName = lObject.Select(x => x.OBJECT_TYPE).ToList();
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(item.Object))
                            {
                                var lObjectName = entity.T_OBJECT_NAMEINFO.FirstOrDefault(x => x.OBJECT_HAPPY_NAME == item.Object);
                                if (lObjectName != null)
                                {
                                    var lObject = entity.T_REGISTED_OBJECT.FirstOrDefault(x => x.OBJECT_NAME_ID == lObjectName.OBJECT_NAME_ID && lApplicationList.Contains((long)x.APPLICATION_ID));
                                    if (lObject != null)
                                    {
                                        var lObjectId = lObject.OBJECT_ID;
                                        var lObjTypeId = lObject.TYPE_ID;

                                        //check object-application linking
                                        if (!lApplicationList.Contains((long)lObject.APPLICATION_ID))
                                        {
                                            //check Parent-child linking
                                            item.ValidationMsg = item.ValidationMsg + "Object Application has not linked. ";
                                            item.IsNotValid = true;
                                        }

                                        //check keyword-object linking
                                        if (lKeywordType.Contains(lObjTypeId))
                                        {
                                            var pList = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == lObjectName.OBJECT_NAME_ID).Select(y => y.OBJECT_TYPE).ToList();
                                            //check Parent-child linking
                                            var lpegflag = true;
                                            foreach (var lType in pList)
                                            {
                                                if (lObjectPegName.Contains(lType))
                                                {
                                                    lpegflag = false;
                                                }
                                            }
                                            if (lpegflag)
                                            {
                                                item.ValidationMsg = item.ValidationMsg + "Object Pegwindow has not linked. ";
                                                item.IsNotValid = true;
                                            }

                                        }
                                        else
                                        {
                                            item.ValidationMsg = item.ValidationMsg + "Object and Keyword has not linked. ";
                                            item.IsNotValid = true;
                                        }
                                    }
                                    else
                                    {
                                        item.ValidationMsg = item.ValidationMsg + "Object has not exist in system. ";
                                        item.IsNotValid = true;
                                    }
                                }
                                else
                                {
                                    item.ValidationMsg = item.ValidationMsg + "Object has not exist in system. ";
                                    item.IsNotValid = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        item.ValidationMsg = item.ValidationMsg + "Keyword has not exist in system. ";
                        item.IsNotValid = true;
                    }
                }
                logger.Info(string.Format("CheckObjectKeywordValidation end | datasetName: {0} | UserName: {1}", lTestCaseId, Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckObjectKeywordValidation method | testcaseId: {0} | UserName: {1}", lTestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckObjectKeywordValidation method | testcaseId: {0} | UserName: {1}", lTestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckObjectKeywordValidation method | testcaseId: {0} | UserName: {1}", lTestCaseId, Username), ex.InnerException);
                throw;
            }
        }
        public bool CheckDuplicateTestCasenameSaveAs(string testcase, long testcaseid)
        {
            try
            {
                logger.Info(string.Format("CheckDuplicateTestCasenameSaveAs start | testcase: {0} | UserName: {1}", testcase, Username));
                var lresult = false;
                if (testcaseid != 0)
                {
                    lresult = entity.T_TEST_CASE_SUMMARY.Any(x => x.TEST_CASE_NAME.ToLower().Trim() == testcase.ToLower().Trim());
                }
                logger.Info(string.Format("CheckDuplicateTestCasenameSaveAs end | testcase: {0} | UserName: {1}", testcase, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCasenameSaveAs method | TestCase Id: {0} | TestCase Name : {1} | UserName: {2}", testcaseid, testcase, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCasenameSaveAs method | TestCase Id: {0} | TestCase Name : {1} | UserName: {2}", testcaseid, testcase, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckDuplicateTestCasenameSaveAs method | TestCase Id: {0} | TestCase Name : {1} | UserName: {2}", testcaseid, testcase, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateDatasetName(string dataset)
        {
            try
            {
                logger.Info(string.Format("CheckDuplicateDatasetName start | dataset: {0} | UserName: {1}", dataset, Username));
                var lresult = false;
                if (!string.IsNullOrEmpty(dataset))
                {
                    lresult = entity.T_TEST_DATA_SUMMARY.Any(x => x.ALIAS_NAME.ToLower().Trim() == dataset.ToLower().Trim());
                }
                logger.Info(string.Format("CheckDuplicateDatasetName end | dataset: {0} | UserName: {1}", dataset, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateDatasetName method | DataSet: {0} | UserName: {1}", dataset, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateDatasetName method | DataSet: {0} | UserName: {1}", dataset, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckDuplicateDatasetName method | DataSet: {0} | UserName: {1}", dataset, Username), ex.InnerException);
                throw;
            }
        }

        public string SaveAsTestcase(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string schema, string constring, string LoginName)
        {
            logger.Info(string.Format("SaveAs Testcase start | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(testcasename))
                    {
                        testcasename = testcasename.Trim();
                    }

                    var lflag = "success";
                    var result = CheckDuplicateTestCasenameSaveAs(testcasename, oldtestcaseid);
                    if (result == true)
                    {
                        var sresult = entity.T_TEST_CASE_SUMMARY.Find(oldtestcaseid);
                        if (sresult.TEST_CASE_NAME == testcasename && sresult.TEST_STEP_DESCRIPTION != testcasedesc)
                        {
                            lflag = "description cannot be changed";
                            return lflag;
                        }
                        lflag = "error";
                        return lflag;
                    }
                    var tbl = new T_TEST_CASE_SUMMARY();
                    tbl.TEST_CASE_ID = Helper.NextTestSuiteId("T_TEST_CASE_SUMMARY_SEQ");
                    tbl.TEST_CASE_NAME = testcasename;
                    tbl.TEST_STEP_DESCRIPTION = testcasedesc;
                    tbl.TEST_STEP_CREATE_TIME = DateTime.Now;
                    tbl.TEST_STEP_CREATOR = LoginName;
                    entity.T_TEST_CASE_SUMMARY.Add(tbl);
                    //rutvi
                    //entity.SaveChanges();

                    var lTSTC = new REL_TEST_CASE_TEST_SUITE();
                    lTSTC.TEST_CASE_ID = tbl.TEST_CASE_ID;
                    lTSTC.TEST_SUITE_ID = testsuiteid;
                    lTSTC.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_TEST_CASE_TEST_SUITE_SEQ");
                    entity.REL_TEST_CASE_TEST_SUITE.Add(lTSTC);
                    //rutvi
                    //entity.SaveChanges();

                    var resultdataset = from t1 in entity.T_TEST_PROJECT
                                        join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                                        join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                                        join t4 in entity.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                                        join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                                        join t6 in entity.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                                        join t7 in entity.T_TEST_DATA_SUMMARY on t6.DATA_SUMMARY_ID equals t7.DATA_SUMMARY_ID
                                        where t1.PROJECT_ID == projectid && t3.TEST_SUITE_ID == testsuiteid && t5.TEST_CASE_ID == oldtestcaseid
                                        select new DataSetListByTestCase
                                        {
                                            Datasetid = t7.DATA_SUMMARY_ID,
                                            Datasetname = t7.ALIAS_NAME
                                        };
                    var tblDataSummary = new T_TEST_DATA_SUMMARY();
                    tblDataSummary.ALIAS_NAME = testcasename;
                    tblDataSummary.DATA_SUMMARY_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    tblDataSummary.SHARE_MARK = 1;
                    tblDataSummary.STATUS = 0;
                    tblDataSummary.CREATE_TIME = DateTime.Now;
                    entity.T_TEST_DATA_SUMMARY.Add(tblDataSummary);
                    //rutvi
                    //entity.SaveChanges();


                    var tblMapping = new REL_TC_DATA_SUMMARY();
                    tblMapping.ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    tblMapping.TEST_CASE_ID = tbl.TEST_CASE_ID;
                    tblMapping.DATA_SUMMARY_ID = tblDataSummary.DATA_SUMMARY_ID;
                    entity.REL_TC_DATA_SUMMARY.Add(tblMapping);
                    entity.SaveChanges();

                    var resultteststeps = from t in entity.T_TEST_STEPS
                                          where t.TEST_CASE_ID == oldtestcaseid
                                          select new
                                          {
                                              t.RUN_ORDER,
                                              t.COLUMN_ROW_SETTING,
                                              t.COMMENT,
                                              t.IS_RUNNABLE,
                                              t.KEY_WORD_ID,
                                              t.OBJECT_ID,
                                              t.OBJECT_NAME_ID,
                                              t.STEPS_ID,
                                              t.TEST_CASE_ID,
                                              t.VALUE_SETTING
                                          };
                    var lresultteststeps = resultteststeps.ToList();
                    foreach (var item in lresultteststeps)
                    {
                        var tblStep = new T_TEST_STEPS();
                        tblStep.STEPS_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblStep.RUN_ORDER = item.RUN_ORDER;
                        tblStep.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        tblStep.COLUMN_ROW_SETTING = item.COLUMN_ROW_SETTING;
                        tblStep.COMMENT = item.COMMENT;
                        tblStep.IS_RUNNABLE = item.IS_RUNNABLE;
                        tblStep.KEY_WORD_ID = item.KEY_WORD_ID;
                        tblStep.OBJECT_ID = item.OBJECT_ID;
                        tblStep.OBJECT_NAME_ID = item.OBJECT_NAME_ID;
                        tblStep.VALUE_SETTING = item.VALUE_SETTING;
                        entity.T_TEST_STEPS.Add(tblStep);
                        entity.SaveChanges();
                    }

                    var resultapp = from t in entity.T_TEST_SUITE
                                    join t2 in entity.REL_APP_TESTSUITE on t.TEST_SUITE_ID equals t2.TEST_SUITE_ID
                                    join t3 in entity.T_REGISTERED_APPS on t2.APPLICATION_ID equals t3.APPLICATION_ID
                                    where t.TEST_SUITE_ID == testsuiteid
                                    select t3.APPLICATION_ID;
                    var lresultApp = resultapp.Distinct().ToList();
                    foreach (var item in lresultApp)
                    {
                        var lApptbl = new REL_APP_TESTCASE();
                        lApptbl.APPLICATION_ID = item;
                        lApptbl.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        lApptbl.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_APP_TESTCASE_SEQ");
                        entity.REL_APP_TESTCASE.Add(lApptbl);
                        entity.SaveChanges();
                    }

                    logger.Info(string.Format("SaveAs Testcase end | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestcase method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | UserName: {7}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestcase method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | UserName: {7}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveAsTestcase method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | UserName: {7}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, Username), ex.InnerException);
                throw;
            }
        }

        public string SaveAsTestCaseOneCopiedDataSet_Temp(string testcasename, long oldtestcaseid, string testcasedesc, string datasetName, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // add new coding
                    if (!string.IsNullOrEmpty(testcasename))
                    {
                        testcasename = testcasename.Trim();
                    }

                    var lflag = "success";
                    // check testcase 
                    var result = CheckDuplicateTestCasenameSaveAs(testcasename, oldtestcaseid);
                    if (result == true)
                    {
                        var sresult = entity.T_TEST_CASE_SUMMARY.Find(oldtestcaseid);
                        if (sresult.TEST_CASE_NAME == testcasename && sresult.TEST_STEP_DESCRIPTION != testcasedesc)
                        {
                            lflag = "description cannot be changed";
                            return lflag;
                        }
                        lflag = "error";
                        return lflag;
                    }

                    var datasetwithsuffix = datasetName.Trim() + suffix.Trim();
                    var datasetresult = CheckDuplicateDatasetName(datasetwithsuffix);

                    if (datasetresult == true)
                    {
                        lflag = "This dataset already exist";
                        return lflag;
                    }

                    // create testcase
                    var tbl = new T_TEST_CASE_SUMMARY();
                    tbl.TEST_CASE_ID = Helper.NextTestSuiteId("T_TEST_CASE_SUMMARY_SEQ");
                    tbl.TEST_CASE_NAME = testcasename;
                    tbl.TEST_STEP_DESCRIPTION = testcasedesc;
                    tbl.TEST_STEP_CREATE_TIME = DateTime.Now;
                    tbl.TEST_STEP_CREATOR = LoginName;
                    entity.T_TEST_CASE_SUMMARY.Add(tbl);
                    //rutvi
                    //entity.SaveChanges();


                    // create reletion testcase and testsuite 
                    var lTSTC = new REL_TEST_CASE_TEST_SUITE();
                    lTSTC.TEST_CASE_ID = tbl.TEST_CASE_ID;
                    lTSTC.TEST_SUITE_ID = testsuiteid;
                    lTSTC.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_TEST_CASE_TEST_SUITE_SEQ");
                    entity.REL_TEST_CASE_TEST_SUITE.Add(lTSTC);
                    entity.SaveChanges();

                    // create dataset 
                    var tblDataSummary = new T_TEST_DATA_SUMMARY();
                    tblDataSummary.ALIAS_NAME = datasetwithsuffix;
                    tblDataSummary.DATA_SUMMARY_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    tblDataSummary.SHARE_MARK = 1;
                    tblDataSummary.STATUS = 0;
                    tblDataSummary.CREATE_TIME = DateTime.Now;
                    entity.T_TEST_DATA_SUMMARY.Add(tblDataSummary);
                    entity.SaveChanges();

                    var tblMapping = new REL_TC_DATA_SUMMARY();
                    tblMapping.ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                    tblMapping.TEST_CASE_ID = tbl.TEST_CASE_ID;
                    tblMapping.DATA_SUMMARY_ID = tblDataSummary.DATA_SUMMARY_ID;
                    entity.REL_TC_DATA_SUMMARY.Add(tblMapping);
                    entity.SaveChanges();

                    var resultteststeps = from t in entity.T_TEST_STEPS
                                          where t.TEST_CASE_ID == oldtestcaseid
                                          select new
                                          {
                                              t.RUN_ORDER,
                                              t.COLUMN_ROW_SETTING,
                                              t.COMMENT,
                                              t.IS_RUNNABLE,
                                              t.KEY_WORD_ID,
                                              t.OBJECT_ID,
                                              t.OBJECT_NAME_ID,
                                              t.STEPS_ID,
                                              t.TEST_CASE_ID,
                                              t.VALUE_SETTING
                                          };
                    var lresultteststeps = resultteststeps.ToList();
                    foreach (var item in lresultteststeps)
                    {
                        var tblStep = new T_TEST_STEPS();
                        tblStep.STEPS_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblStep.RUN_ORDER = item.RUN_ORDER;
                        tblStep.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        tblStep.COLUMN_ROW_SETTING = item.COLUMN_ROW_SETTING;
                        tblStep.COMMENT = item.COMMENT;
                        tblStep.IS_RUNNABLE = item.IS_RUNNABLE;
                        tblStep.KEY_WORD_ID = item.KEY_WORD_ID;
                        tblStep.OBJECT_ID = item.OBJECT_ID;
                        tblStep.OBJECT_NAME_ID = item.OBJECT_NAME_ID;
                        tblStep.VALUE_SETTING = item.VALUE_SETTING;
                        entity.T_TEST_STEPS.Add(tblStep);
                        entity.SaveChanges();

                        // get data value form Test_data_setting using stepid and datasummeryid
                        var olddatasummarydata = entity.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME.Trim() == datasetName.Trim()).FirstOrDefault();
                        var datavalue = entity.TEST_DATA_SETTING.Where(x => x.STEPS_ID == item.STEPS_ID && x.DATA_SUMMARY_ID == olddatasummarydata.DATA_SUMMARY_ID).FirstOrDefault();

                        if (datavalue != null)
                        {
                            // add datavalue
                            var tbldatasetting = new TEST_DATA_SETTING();
                            tbldatasetting.DATA_SETTING_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                            tbldatasetting.STEPS_ID = tblStep.STEPS_ID;
                            tbldatasetting.DATA_SUMMARY_ID = tblDataSummary.DATA_SUMMARY_ID;
                            tbldatasetting.LOOP_ID = datavalue.LOOP_ID;
                            tbldatasetting.DATA_VALUE = datavalue.DATA_VALUE;
                            tbldatasetting.POOL_ID = datavalue.POOL_ID;
                            tbldatasetting.VALUE_OR_OBJECT = datavalue.VALUE_OR_OBJECT;
                            tbldatasetting.VERSION = datavalue.VERSION;
                            tbldatasetting.DATA_DIRECTION = datavalue.DATA_DIRECTION;
                            tbldatasetting.CREATE_TIME = DateTime.Now;
                            entity.TEST_DATA_SETTING.Add(tbldatasetting);
                            entity.SaveChanges();
                        }
                    }

                    var resultapp = from t in entity.T_TEST_SUITE
                                    join t2 in entity.REL_APP_TESTSUITE on t.TEST_SUITE_ID equals t2.TEST_SUITE_ID
                                    join t3 in entity.T_REGISTERED_APPS on t2.APPLICATION_ID equals t3.APPLICATION_ID
                                    where t.TEST_SUITE_ID == testsuiteid
                                    select t3.APPLICATION_ID;
                    var lresultApp = resultapp.Distinct().ToList();
                    foreach (var item in lresultApp)
                    {
                        var lApptbl = new REL_APP_TESTCASE();
                        lApptbl.APPLICATION_ID = item;
                        lApptbl.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        lApptbl.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_APP_TESTCASE_SEQ");
                        entity.REL_APP_TESTCASE.Add(lApptbl);
                        entity.SaveChanges();
                    }
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet_Temp method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Dataset Name : {7} | Suffix : {8} | UserName: {9}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, datasetName, suffix, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet_Temp method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Dataset Name : {7} | Suffix : {8} | UserName: {9}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, datasetName, suffix, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveAsTestCaseOneCopiedDataSet_Temp method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Dataset Name : {7} | Suffix : {8} | UserName: {9}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, datasetName, suffix, Username), ex.InnerException);
                throw;
            }

        }

        public string SaveAsTestCaseOneCopiedDataSet(string testcasename, long oldtestcaseid, string testcasedesc, string datasetName, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            logger.Info(string.Format("SaveAs TestCas eOne Copied DataSet start | Oldtestcaseid: {0} | Testcasename: {1} | DatasetName: {2} | UserName: {3}", oldtestcaseid, testcasename, datasetName, Username));
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ObjectParameter RESULT = new ObjectParameter("RESULT", typeof(string));

                    var lresult = entity.SP_SavaAs_TestCase_One_Dataset(oldtestcaseid, testcasename, testcasedesc, testsuiteid, projectid, datasetName, suffix, LoginName, RESULT);

                    logger.Info(string.Format("SaveAs TestCas eOne Copied DataSet end | Oldtestcaseid: {0} | Testcasename: {1} | DatasetName: {2} | UserName: {3}", oldtestcaseid, testcasename, datasetName, Username));
                    scope.Complete();
                    return RESULT.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Dataset Name : {7} | Suffix : {8} | UserName: {9}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, datasetName, suffix, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Dataset Name : {7} | Suffix : {8} | UserName: {9}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, datasetName, suffix, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveAsTestCaseOneCopiedDataSet method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Dataset Name : {7} | Suffix : {8} | UserName: {9}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, datasetName, suffix, Username), ex.InnerException);
                throw;
            }
        }

        public string SaveAsTestCaseAllCopiedDataSet_Temp(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // add new coding
                    if (!string.IsNullOrEmpty(testcasename))
                    {
                        testcasename = testcasename.Trim();
                    }

                    var lflag = "success";
                    // check testcase 
                    var result = CheckDuplicateTestCasenameSaveAs(testcasename, oldtestcaseid);
                    if (result == true)
                    {
                        var sresult = entity.T_TEST_CASE_SUMMARY.Find(oldtestcaseid);
                        if (sresult.TEST_CASE_NAME == testcasename && sresult.TEST_STEP_DESCRIPTION != testcasedesc)
                        {
                            lflag = "description cannot be changed";
                            return lflag;
                        }
                        lflag = "error";
                        return lflag;
                    }

                    //check dublicate dataset
                    var resultdataset = from t1 in entity.T_TEST_PROJECT
                                        join t2 in entity.REL_TEST_SUIT_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                                        join t3 in entity.T_TEST_SUITE on t2.TEST_SUITE_ID equals t3.TEST_SUITE_ID
                                        join t4 in entity.REL_TEST_CASE_TEST_SUITE on t2.TEST_SUITE_ID equals t4.TEST_SUITE_ID
                                        join t5 in entity.T_TEST_CASE_SUMMARY on t4.TEST_CASE_ID equals t5.TEST_CASE_ID
                                        join t6 in entity.REL_TC_DATA_SUMMARY on t5.TEST_CASE_ID equals t6.TEST_CASE_ID
                                        join t7 in entity.T_TEST_DATA_SUMMARY on t6.DATA_SUMMARY_ID equals t7.DATA_SUMMARY_ID
                                        where t1.PROJECT_ID == projectid && t3.TEST_SUITE_ID == testsuiteid && t5.TEST_CASE_ID == oldtestcaseid
                                        select new DataSetListByTestCase
                                        {
                                            Datasetid = t7.DATA_SUMMARY_ID,
                                            Datasetname = t7.ALIAS_NAME
                                        };
                    var datasetlst = resultdataset.ToList();
                    var datasetlstwithsuffix = datasetlst.Select(x => x.Datasetname.Trim() + suffix.Trim()).ToList();
                    var checkdublicatedataset = entity.T_TEST_DATA_SUMMARY.ToList().Where(a => datasetlstwithsuffix.Any(b => a.ALIAS_NAME.Trim() == b.Trim()));

                    if (checkdublicatedataset.Count() > 0)
                    {
                        lflag = "This dataset already exist";
                        return lflag;
                    }
                    // create testcase
                    var tbl = new T_TEST_CASE_SUMMARY();
                    tbl.TEST_CASE_ID = Helper.NextTestSuiteId("T_TEST_CASE_SUMMARY_SEQ");
                    tbl.TEST_CASE_NAME = testcasename;
                    tbl.TEST_STEP_DESCRIPTION = testcasedesc;
                    tbl.TEST_STEP_CREATE_TIME = DateTime.Now;
                    tbl.TEST_STEP_CREATOR = LoginName;
                    entity.T_TEST_CASE_SUMMARY.Add(tbl);
                    //rutvi
                    //entity.SaveChanges();

                    // create reletion testcase and testsuite 
                    var lTSTC = new REL_TEST_CASE_TEST_SUITE();
                    lTSTC.TEST_CASE_ID = tbl.TEST_CASE_ID;
                    lTSTC.TEST_SUITE_ID = testsuiteid;
                    lTSTC.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_TEST_CASE_TEST_SUITE_SEQ");
                    entity.REL_TEST_CASE_TEST_SUITE.Add(lTSTC);
                    entity.SaveChanges();

                    //create all dataset
                    var objdataset = datasetlstwithsuffix.ToList().Select(a => new T_TEST_DATA_SUMMARY
                    {
                        ALIAS_NAME = a.Trim(),
                        DATA_SUMMARY_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ"),
                        SHARE_MARK = 1,
                        STATUS = 0,
                        CREATE_TIME = DateTime.Now
                    }).ToList();
                    CreateAllDataset(constring, objdataset);

                    //create all dataset mapping
                    var objdatasetmapping = objdataset.ToList().Select(a => new REL_TC_DATA_SUMMARY
                    {
                        ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ"),
                        TEST_CASE_ID = tbl.TEST_CASE_ID,
                        DATA_SUMMARY_ID = a.DATA_SUMMARY_ID,
                        CREATE_TIME = DateTime.Now
                    }).ToList();
                    CreateAllDatasetMapping(constring, objdatasetmapping);

                    //get old step 
                    // create sp and then get stepid using new testcaseid

                    var resultteststeps = from t in entity.T_TEST_STEPS
                                          where t.TEST_CASE_ID == oldtestcaseid
                                          select new
                                          {
                                              t.RUN_ORDER,
                                              t.COLUMN_ROW_SETTING,
                                              t.COMMENT,
                                              t.IS_RUNNABLE,
                                              t.KEY_WORD_ID,
                                              t.OBJECT_ID,
                                              t.OBJECT_NAME_ID,
                                              t.STEPS_ID,
                                              t.TEST_CASE_ID,
                                              t.VALUE_SETTING
                                          };
                    var lresultteststeps = resultteststeps.ToList();

                    foreach (var item in lresultteststeps)
                    {
                        var tblStep = new T_TEST_STEPS();
                        tblStep.STEPS_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        tblStep.RUN_ORDER = item.RUN_ORDER;
                        tblStep.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        tblStep.COLUMN_ROW_SETTING = item.COLUMN_ROW_SETTING;
                        tblStep.COMMENT = item.COMMENT;
                        tblStep.IS_RUNNABLE = item.IS_RUNNABLE;
                        tblStep.KEY_WORD_ID = item.KEY_WORD_ID;
                        tblStep.OBJECT_ID = item.OBJECT_ID;
                        tblStep.OBJECT_NAME_ID = item.OBJECT_NAME_ID;
                        tblStep.VALUE_SETTING = item.VALUE_SETTING;
                        entity.T_TEST_STEPS.Add(tblStep);
                        entity.SaveChanges();

                        //old dataset 
                        foreach (var dataset in datasetlst)
                        {
                            // get old datasetid 
                            var olddataset = entity.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME.Trim() == dataset.Datasetname.Trim()).FirstOrDefault();
                            var datavalue = entity.TEST_DATA_SETTING.Where(x => x.STEPS_ID == item.STEPS_ID && x.DATA_SUMMARY_ID == olddataset.DATA_SUMMARY_ID).FirstOrDefault();

                            //get new datasetid
                            var newdataset = entity.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME.Trim() == dataset.Datasetname.Trim() + suffix.Trim()).FirstOrDefault();

                            // add datavalue
                            var tbldatasetting = new TEST_DATA_SETTING();
                            tbldatasetting.DATA_SETTING_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                            tbldatasetting.STEPS_ID = tblStep.STEPS_ID;
                            tbldatasetting.DATA_SUMMARY_ID = newdataset.DATA_SUMMARY_ID;
                            tbldatasetting.LOOP_ID = datavalue.LOOP_ID;
                            tbldatasetting.DATA_VALUE = datavalue.DATA_VALUE;
                            tbldatasetting.POOL_ID = datavalue.POOL_ID;
                            tbldatasetting.VALUE_OR_OBJECT = datavalue.VALUE_OR_OBJECT;
                            tbldatasetting.VERSION = datavalue.VERSION;
                            tbldatasetting.DATA_DIRECTION = datavalue.DATA_DIRECTION;
                            tbldatasetting.CREATE_TIME = DateTime.Now; ;
                            entity.TEST_DATA_SETTING.Add(tbldatasetting);
                            entity.SaveChanges();
                        }
                    }

                    //reletion netween testsuite and application
                    var resultapp = from t in entity.T_TEST_SUITE
                                    join t2 in entity.REL_APP_TESTSUITE on t.TEST_SUITE_ID equals t2.TEST_SUITE_ID
                                    join t3 in entity.T_REGISTERED_APPS on t2.APPLICATION_ID equals t3.APPLICATION_ID
                                    where t.TEST_SUITE_ID == testsuiteid
                                    select t3.APPLICATION_ID;
                    var lresultApp = resultapp.Distinct().ToList();
                    foreach (var item in lresultApp)
                    {
                        var lApptbl = new REL_APP_TESTCASE();
                        lApptbl.APPLICATION_ID = item;
                        lApptbl.TEST_CASE_ID = tbl.TEST_CASE_ID;
                        lApptbl.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_APP_TESTCASE_SEQ");
                        entity.REL_APP_TESTCASE.Add(lApptbl);
                        entity.SaveChanges();
                    }
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet_Temp method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Suffix : {7} | UserName: {8}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, suffix, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet_Temp method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Suffix : {7} | UserName: {8}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, suffix, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveAsTestCaseAllCopiedDataSet_Temp method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Suffix : {7} | UserName: {8}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, suffix, Username), ex.InnerException);
                throw;
            }
        }

        public string SaveAsTestCaseAllCopiedDataSet(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("SaveAs TestCase All Copied DataSet start | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
                    ObjectParameter RESULT = new ObjectParameter("RESULT", typeof(string));
                    var lresult = entity.SP_SavaAs_TestCase_AllDataset(oldtestcaseid, testcasename, testcasedesc, testsuiteid, projectid, suffix, LoginName, RESULT);

                    logger.Info(string.Format("SaveAs TestCase All Copied DataSet end | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
                    scope.Complete();
                    return RESULT.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Suffix : {7} | UserName: {8}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, suffix, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Suffix : {7} | UserName: {8}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, suffix, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveAsTestCaseAllCopiedDataSet method | ProjectId: {0} | TestSuite Id : {1} | TestCase Id : {2} | Testcase Name : {3} | Testcase Desc : {4} | Conn string : {5} | Schema : {6} | Suffix : {7} | UserName: {8}", projectid, testsuiteid, oldtestcaseid, testcasename, testcasedesc, constring, schema, suffix, Username), ex.InnerException);
                throw;
            }
        }

        public void CreateAllDataset(string constring, List<T_TEST_DATA_SUMMARY> objdataset)
        {
            try
            {
                logger.Info(string.Format("CreateAllDataset start | UserName: {0}", Username));
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(constring);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();

                string lcmdquery = "insert into T_TEST_DATA_SUMMARY ( DATA_SUMMARY_ID,ALIAS_NAME,SHARE_MARK,STATUS,CREATE_TIME) values(:1,:2,:3,:4,:5)";
                int[] lids = new int[objdataset.Count()];
                using (var lcmd = lconnection.CreateCommand())
                {
                    lcmd.CommandText = lcmdquery;
                    lcmd.ArrayBindCount = lids.Length;

                    string[] DATA_SUMMARY_ID_param = objdataset.Select(x => x.DATA_SUMMARY_ID.ToString()).ToArray();
                    string[] ALIAS_NAME_param = objdataset.Select(x => x.ALIAS_NAME.ToString()).ToArray();
                    string[] SHARE_MARK_param = objdataset.Select(x => x.SHARE_MARK.ToString()).ToArray();
                    string[] STATUS_param = objdataset.Select(x => x.STATUS.ToString()).ToArray();
                    string[] CREATE_TIME_param = objdataset.Select(x => Convert.ToDateTime(x.CREATE_TIME).ToString("dd-MMM-yyyy")).ToArray();

                    OracleParameter DATA_SUMMARY_ID_oparam = new OracleParameter();
                    DATA_SUMMARY_ID_oparam.OracleDbType = OracleDbType.Varchar2;
                    DATA_SUMMARY_ID_oparam.Value = DATA_SUMMARY_ID_param;

                    OracleParameter ALIAS_NAME_oparam = new OracleParameter();
                    ALIAS_NAME_oparam.OracleDbType = OracleDbType.Varchar2;
                    ALIAS_NAME_oparam.Value = ALIAS_NAME_param;

                    OracleParameter SHARE_MARK_oparam = new OracleParameter();
                    SHARE_MARK_oparam.OracleDbType = OracleDbType.Varchar2;
                    SHARE_MARK_oparam.Value = SHARE_MARK_param;

                    OracleParameter STATUS_oparam = new OracleParameter();
                    STATUS_oparam.OracleDbType = OracleDbType.Varchar2;
                    STATUS_oparam.Value = STATUS_param;

                    OracleParameter CREATE_TIME_oparam = new OracleParameter();
                    CREATE_TIME_oparam.OracleDbType = OracleDbType.Varchar2;
                    CREATE_TIME_oparam.Value = CREATE_TIME_param;

                    lcmd.Parameters.Add(DATA_SUMMARY_ID_oparam);
                    lcmd.Parameters.Add(ALIAS_NAME_oparam);
                    lcmd.Parameters.Add(SHARE_MARK_oparam);
                    lcmd.Parameters.Add(STATUS_oparam);
                    lcmd.Parameters.Add(CREATE_TIME_oparam);
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
                logger.Info(string.Format("CreateAllDataset end | UserName: {0}", Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CreateAllDataset method | Conn string : {0} | UserName: {1}", constring, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CreateAllDataset method | Conn string : {0} | UserName: {1}", constring, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CreateAllDataset method | Conn string : {0} | UserName: {1}", constring, Username), ex.InnerException);
                throw;
            }

        }

        public void CreateAllDatasetMapping(string constring, List<REL_TC_DATA_SUMMARY> rEL_TCs)
        {
            try
            {
                logger.Info(string.Format("CreateAllDatasetMapping start | UserName: {0}", Username));
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(constring);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();

                string lcmdquery = "insert into REL_TC_DATA_SUMMARY ( ID,TEST_CASE_ID,DATA_SUMMARY_ID,CREATE_TIME) values(:1,:2,:3,:4)";
                int[] lids = new int[rEL_TCs.Count()];
                using (var lcmd = lconnection.CreateCommand())
                {
                    lcmd.CommandText = lcmdquery;
                    lcmd.ArrayBindCount = lids.Length;

                    string[] ID_param = rEL_TCs.Select(x => x.ID.ToString()).ToArray();
                    string[] TEST_CASE_ID_param = rEL_TCs.Select(x => x.TEST_CASE_ID.ToString()).ToArray();
                    string[] DATA_SUMMARY_ID_param = rEL_TCs.Select(x => x.DATA_SUMMARY_ID.ToString()).ToArray();
                    string[] CREATE_TIME_param = rEL_TCs.Select(x => Convert.ToDateTime(x.CREATE_TIME).ToString("dd-MMM-yyyy")).ToArray();

                    OracleParameter ID_oparam = new OracleParameter();
                    ID_oparam.OracleDbType = OracleDbType.Varchar2;
                    ID_oparam.Value = ID_param;

                    OracleParameter TEST_CASE_ID_oparam = new OracleParameter();
                    TEST_CASE_ID_oparam.OracleDbType = OracleDbType.Varchar2;
                    TEST_CASE_ID_oparam.Value = TEST_CASE_ID_param;

                    OracleParameter DATA_SUMMARY_ID_oparam = new OracleParameter();
                    DATA_SUMMARY_ID_oparam.OracleDbType = OracleDbType.Varchar2;
                    DATA_SUMMARY_ID_oparam.Value = DATA_SUMMARY_ID_param;

                    OracleParameter CREATE_TIME_oparam = new OracleParameter();
                    CREATE_TIME_oparam.OracleDbType = OracleDbType.Varchar2;
                    CREATE_TIME_oparam.Value = CREATE_TIME_param;

                    lcmd.Parameters.Add(ID_oparam);
                    lcmd.Parameters.Add(TEST_CASE_ID_oparam);
                    lcmd.Parameters.Add(DATA_SUMMARY_ID_oparam);
                    lcmd.Parameters.Add(CREATE_TIME_oparam);
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
                logger.Info(string.Format("CreateAllDatasetMapping end | UserName: {0}", Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CreateAllDatasetMapping method | Conn string : {0} | UserName: {1}", constring, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CreateAllDatasetMapping method | Conn string : {0} | UserName: {1}", constring, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CreateAllDatasetMapping method | Conn string : {0} | UserName: {1}", constring, Username), ex.InnerException);
                throw;
            }
        }

        public void convertsteps(string constring, string schema, long oldtestcaseid, long newtestcaseid)
        {
            try
            {
                logger.Info(string.Format("convertsteps start | oldtestcaseid: {0}| newtestcaseid: {1} | UserName: {2}", oldtestcaseid, newtestcaseid, Username));
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(constring);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                lcmd.CommandType = CommandType.StoredProcedure;

                lcmd.Parameters.Add(new OracleParameter("OLDTESTCASEID", OracleDbType.Int32)).Value = oldtestcaseid;
                lcmd.Parameters.Add(new OracleParameter("NEWTESTCASEID", OracleDbType.Varchar2)).Value = newtestcaseid;

                lcmd.CommandText = schema + "." + "SP_T_TESTSTEP";

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
                logger.Info(string.Format("convertsteps end | oldtestcaseid: {0}| newtestcaseid: {1} | UserName: {2}", oldtestcaseid, newtestcaseid, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in convertsteps method | Old TestCase Id : {0} | New TestCase Id : {1} | Conn string : {2} | Schema : {3} | UserName: {4}", oldtestcaseid, newtestcaseid, constring, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in convertsteps method | Old TestCase Id : {0} | New TestCase Id : {1} | Conn string : {2} | Schema : {3} | UserName: {4}", oldtestcaseid, newtestcaseid, constring, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in convertsteps method | Old TestCase Id : {0} | New TestCase Id : {1} | Conn string : {2} | Schema : {3} | UserName: {4}", oldtestcaseid, newtestcaseid, constring, schema, Username), ex.InnerException);
                throw;
            }
        }

        public TestCase_Version_Model GetTestCaseVersion(long TestCaseId, long UserId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("GetTestCaseVersion start | TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username));
                    var ltbl = entity.T_TESTCASE_VERSION.Where(x => x.TESTCASEID == (long)TestCaseId).FirstOrDefault();
                    if (ltbl != null)
                    {
                        if ((ltbl.ISAVAILABLE == 1 && ltbl.CREATETIME < DateTime.Now.AddMinutes(-10)) || ltbl.CREATORID == 0)
                        {
                            ltbl.CREATORID = UserId;
                            ltbl.CREATETIME = DateTime.Now;
                        }
                        ltbl.ISAVAILABLE = 1;
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TC_Version = new T_TESTCASE_VERSION();
                        TC_Version.CREATETIME = DateTime.Now;
                        TC_Version.CREATORID = UserId;
                        TC_Version.TESTCASEID = TestCaseId;
                        TC_Version.ISAVAILABLE = 1;
                        TC_Version.TC_VERSION_ID = Helper.NextTestSuiteId("T_TEST_CASE_SUMMARY_SEQ"); ;
                        TC_Version.VERSIONID = 1;
                        entity.T_TESTCASE_VERSION.Add(TC_Version);
                        entity.SaveChanges();
                    }

                    var lResult = entity.T_TESTCASE_VERSION.Where(x => x.TESTCASEID == TestCaseId).Select(y => new TestCase_Version_Model
                    {
                        TESTCASEID = (long)y.TESTCASEID,
                        TC_VERSION_ID = y.TC_VERSION_ID,
                        CREATORID = y.CREATORID,
                        CREATETIME = y.CREATETIME,
                        ISAVAILABLE = y.CREATORID == UserId ? 0 : y.CREATORID == 0 ? 0 : y.ISAVAILABLE,
                        VERSION = y.VERSIONID
                    }).FirstOrDefault();

                    logger.Info(string.Format("GetTestCaseVersion end | TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username));
                    scope.Complete();
                    return lResult;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTestCaseVersion method | testcaseId: {0} | UserName: {1}", TestCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseVersion method | testcaseId: {0} | UserName: {1}", TestCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetTestCaseVersion method | testcaseId: {0} | UserName: {1}", TestCaseId, Username), ex.InnerException);
                throw;
            }
        }

        public void SaveTestCaseVersion(long TestCaseId, long UserId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Save TestCase Version start | TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username));
                    var ltbl = entity.T_TESTCASE_VERSION.Where(x => x.TESTCASEID == TestCaseId).FirstOrDefault();
                    if (ltbl != null)
                    {
                        ltbl.VERSIONID = ltbl.VERSIONID + 1;
                        ltbl.CREATORID = 0;
                        ltbl.CREATETIME = DateTime.Now;
                        ltbl.ISAVAILABLE = 0;
                        entity.SaveChanges();
                        logger.Info(string.Format("Save TestCase Version end | TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username));
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CheckDuplicateTestCaseName method TestCaseId: {0} | UserId: {1} | UserName: {2}", TestCaseId, UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool MatchTestCaseVersion(long TestCaseId, long VersionId)
        {
            try
            {
                logger.Info(string.Format("Match TestCase Version start | TestCaseId: {0} | VersionId: {1} | UserName: {2}", TestCaseId, VersionId, Username));
                var lresult = entity.T_TESTCASE_VERSION.Any(x => x.TESTCASEID == TestCaseId && x.VERSIONID == VersionId);
                logger.Info(string.Format("Match TestCase Version end | TestCaseId: {0} | VersionId: {1} | UserName: {2}", TestCaseId, VersionId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in MatchTestCaseVersion method | TestCaseId: {0} | VersionId: {1} | UserName: {2}", TestCaseId, VersionId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in MatchTestCaseVersion method | TestCaseId: {0} | VersionId: {1} | UserName: {2}", TestCaseId, VersionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in MatchTestCaseVersion method | TestCaseId: {0} | VersionId: {1} | UserName: {2}", TestCaseId, VersionId, Username), ex.InnerException);
                throw;
            }
        }

        public void UpdateIsAvailable(string TestCaseIds)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Update Is Available start | TestCaseId: {0} | UserName: {1}", TestCaseIds, Username));
                    var TestCaseArray = TestCaseIds.Split(',').ToList();
                    foreach (var item in TestCaseArray)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var lTestCaseId = Convert.ToInt64(item);
                            var tbl = entity.T_TESTCASE_VERSION.Where(x => x.TESTCASEID == lTestCaseId).FirstOrDefault();
                            if (tbl != null)
                            {
                                tbl.ISAVAILABLE = 0;
                                tbl.CREATORID = 0;

                            }
                        }
                    }

                    entity.SaveChanges();
                    logger.Info(string.Format("Update Is Available end | TestCaseId: {0} | UserName: {1}", TestCaseIds, Username));
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateIsAvailable method | testcaseId: {0} | UserName: {1}", TestCaseIds, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateIsAvailable method | testcaseId: {0} | UserName: {1}", TestCaseIds, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateIsAvailable method | testcaseId: {0} | UserName: {1}", TestCaseIds, Username), ex.InnerException);
                throw;
            }
        }

        public void UpdateIsAvailableReload(long UserId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Update UserId start | UserId: {0}", UserId));
                    var tblList = entity.T_TESTCASE_VERSION.Where(x => x.CREATORID == UserId).ToList();
                    tblList.ForEach(x => { x.ISAVAILABLE = 0; x.CREATORID = 0; });
                    //foreach (var item in tblList)
                    //{
                    //    item.ISAVAILABLE = 0;
                    //    item.CREATORID = 0;
                    //}
                    entity.SaveChanges();
                    logger.Info(string.Format("Update UserId end | UserId: {0}", UserId));
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateIsAvailableReload method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateIsAvailableReload method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in UpdateIsAvailableReload method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public string CopyDataSet(long? testCaseId, long? olddatasetid, string datasetname, string datasetdesc, string lstrConn, string schema)
        {
            try
            {
                logger.Info(string.Format("Copy DataSet start | datasetname: {0} | UserName: {1}", datasetname, Username));
                if (!string.IsNullOrEmpty(datasetname) || !string.IsNullOrEmpty(datasetdesc))
                {
                    datasetname = datasetname.Trim();
                    datasetdesc = datasetdesc.Trim();
                }
                var lDataSummary = new T_TEST_DATA_SUMMARY();
                lDataSummary.DATA_SUMMARY_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                lDataSummary.ALIAS_NAME = datasetname;
                lDataSummary.DESCRIPTION_INFO = datasetdesc;
                entity.T_TEST_DATA_SUMMARY.Add(lDataSummary);
                //rutvi
                //entity.SaveChanges();
                var ldatasetid = lDataSummary.DATA_SUMMARY_ID;

                var lRelTCDataSet = new REL_TC_DATA_SUMMARY();
                lRelTCDataSet.ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                lRelTCDataSet.TEST_CASE_ID = testCaseId;
                lRelTCDataSet.DATA_SUMMARY_ID = lDataSummary.DATA_SUMMARY_ID;
                entity.REL_TC_DATA_SUMMARY.Add(lRelTCDataSet);
                entity.SaveChanges();
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;


                //var testsuiteid = entity.REL_TEST_CASE_TEST_SUITE.FirstOrDefault(x => x.TEST_CASE_ID == TestCaseId).TEST_SUITE_ID;
                OracleParameter[] ladd_refer_image = new OracleParameter[2];
                ladd_refer_image[0] = new OracleParameter("OLDDATASETID", OracleDbType.Long);
                ladd_refer_image[0].Value = olddatasetid;

                ladd_refer_image[1] = new OracleParameter("NEWDATASETID", OracleDbType.Long);
                ladd_refer_image[1].Value = ldatasetid;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schema + "." + "SP_COPYDATASETS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                logger.Info(string.Format("Copy DataSet end | datasetname: {0} | UserName: {1}", datasetname, Username));
                return "success," + ldatasetid;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CopyDataSet method | TestCase Id : {0} | Dataset Id : {1} | Dataset Name : {2} | Dataset Desc : {3} | Conn string : {4} | Schema : {5} | UserName: {6}", testCaseId, olddatasetid, datasetname, datasetdesc, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CopyDataSet method |  TestCase Id : {0} | Dataset Id : {1} | Dataset Name : {2} | Dataset Desc : {3} | Conn string : {4} | Schema : {5} | UserName: {6}", testCaseId, olddatasetid, datasetname, datasetdesc, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in CopyDataSet method |  TestCase Id : {0} | Dataset Id : {1} | Dataset Name : {2} | Dataset Desc : {3} | Conn string : {4} | Schema : {5} | UserName: {6}", testCaseId, olddatasetid, datasetname, datasetdesc, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }

        public List<TestCaseValidationResultModel> ExecuteCheckValidationTestCase(long feedProcessId, string schema, string lstrConn)
        {
            try
            {
                logger.Info(string.Format("ExecuteCheckValidationTestCase start | feedProcessId: {0} | UserName: {1}", feedProcessId, Username));
                ObjectParameter op = new ObjectParameter("RESULT", "");
                entity.SP_CheckValidationTestCase(feedProcessId, op);
                entity.SaveChanges();

                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection pconnection = GetOracleConnection(lstrConn);
                pconnection.Open();

                OracleTransaction ptransaction;
                ptransaction = pconnection.BeginTransaction();

                OracleCommand pcmd;
                pcmd = pconnection.CreateCommand();
                pcmd.Transaction = ptransaction;

                OracleParameter[] padd_refer_image = new OracleParameter[2];
                padd_refer_image[0] = new OracleParameter("FEEDPROCESSID1", OracleDbType.Long);
                padd_refer_image[0].Value = feedProcessId;

                padd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                padd_refer_image[1].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in padd_refer_image)
                {
                    pcmd.Parameters.Add(p);
                }

                //The name of the Procedure responsible for inserting the data in the table.
                pcmd.CommandText = schema + "." + "SP_GetTestCaseValidationResult";
                pcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(pcmd);
                dataAdapter.Fill(lds);

                var dt = new DataTable();
                dt = lds.Tables[0];

                List<TestCaseValidationResultModel> resultList = dt.AsEnumerable().Select(row =>
                    new TestCaseValidationResultModel
                    {
                        ID = Convert.ToInt64(row.Field<decimal>("ID")),
                        ISVALID = Convert.ToInt64(row.Field<decimal>("ISVALID")),
                        FEEDPROCESSDETAILID = Convert.ToInt64(row.Field<decimal?>("FEEDPROCESSDETAILID")),
                        FEEDPROCESSID = Convert.ToInt64(row.Field<decimal>("FEEDPROCESSID")),
                        VALIDATIONMSG = row.Field<string>("VALIDATIONMSG"),
                    }).ToList();

                var lGroupResult = resultList.GroupBy(x => new { x.ID }).Select(g => new TestCaseValidationResultModel
                {
                    ID = g.Key.ID,
                    VALIDATIONMSG = string.Join(",", g.Select(i => i.VALIDATIONMSG)),

                }).OrderBy(z => z.ID).ToList();

                logger.Info(string.Format("ExecuteCheckValidationTestCase end | feedProcessId: {0} | UserName: {1}", feedProcessId, Username));
                return lGroupResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ExecuteCheckValidationTestCase method | FeedProcess Id : {0} | Conn string : {1} | Schema : {2} | UserName: {3}", feedProcessId, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ExecuteCheckValidationTestCase method | FeedProcess Id : {0} | Conn string : {1} | Schema : {2} | UserName: {3}", feedProcessId, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in ExecuteCheckValidationTestCase method |  FeedProcess Id : {0} | Conn string : {1} | Schema : {2} | UserName: {3}", feedProcessId, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }

        public List<TestCaseValidationResultModel> InsertStgTestcaseValidationTable(string lConnectionStr, string lschema, KeywordObjectLink[] lobj, string testCaseId)
        {
            try
            {
                logger.Info(string.Format("Insert StgTestcase ValidationTable start | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
                OracleTransaction ltransaction;

                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();

                string lreturnValues = InsertFeedProcess();

                var lvalFeed = lreturnValues.Split('~')[0];
                var lvalFeedD = lreturnValues.Split('~')[1];

                string lcmdquery = "insert into TBLSTGTESTCASEVALID ( ID,STEPID,KEYWORD,OBJECT,TESTCASEID,FEEDPROCESSID,FEEDPROCESSDETAILID) values(:1,:2,:3,:4,:5,:6,:7)";
                int[] lids = new int[lobj.ToList().Count()];
                var ValidationSteps = new List<TestCaseValidationResultModel>();
                using (var lcmd = lconnection.CreateCommand())
                {
                    lcmd.CommandText = lcmdquery;
                    lcmd.ArrayBindCount = lids.Length;

                    string[] ID_param = lobj.ToList().Select(r => r.pq_ri.ToString()).ToArray();
                    string[] STEPID_param = lobj.ToList().Select(r => r.StepId.ToString()).ToArray();
                    string[] KEYWORD_param = lobj.ToList().Select(r => r.Keyword).ToArray();
                    string[] OBJECT_param = lobj.ToList().Select(r => r.Object).ToArray();
                    string[] TESTCASEID_param = new string[lids.Length];
                    for (int p = 0; p < lids.Length; p++)
                    {
                        TESTCASEID_param[p] = testCaseId;
                    }
                    string[] FEEDPROCESSID_param = new string[lids.Length];
                    for (int p = 0; p < lids.Length; p++)
                    {
                        FEEDPROCESSID_param[p] = lvalFeed;
                    }
                    string[] FEEDPROCESSDETAILID_param = new string[lids.Length];
                    for (int p = 0; p < lids.Length; p++)
                    {
                        FEEDPROCESSDETAILID_param[p] = lvalFeedD;
                    }

                    OracleParameter ID_oparam = new OracleParameter();
                    ID_oparam.OracleDbType = OracleDbType.Varchar2;
                    ID_oparam.Value = ID_param;

                    OracleParameter STEPID_oparam = new OracleParameter();
                    STEPID_oparam.OracleDbType = OracleDbType.Varchar2;
                    STEPID_oparam.Value = STEPID_param;

                    OracleParameter KEYWORD_oparam = new OracleParameter();
                    KEYWORD_oparam.OracleDbType = OracleDbType.Varchar2;
                    KEYWORD_oparam.Value = KEYWORD_param;

                    OracleParameter OBJECT_oparam = new OracleParameter();
                    OBJECT_oparam.OracleDbType = OracleDbType.Varchar2;
                    OBJECT_oparam.Value = OBJECT_param;

                    OracleParameter TESTCASEID_oparam = new OracleParameter();
                    TESTCASEID_oparam.OracleDbType = OracleDbType.Varchar2;
                    TESTCASEID_oparam.Value = TESTCASEID_param;

                    OracleParameter FEEDPROCESSID_oparam = new OracleParameter();
                    FEEDPROCESSID_oparam.OracleDbType = OracleDbType.Varchar2;
                    FEEDPROCESSID_oparam.Value = FEEDPROCESSID_param;

                    OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                    FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                    FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;


                    lcmd.Parameters.Add(ID_oparam);
                    lcmd.Parameters.Add(STEPID_oparam);
                    lcmd.Parameters.Add(KEYWORD_oparam);
                    lcmd.Parameters.Add(OBJECT_oparam);
                    lcmd.Parameters.Add(TESTCASEID_oparam);
                    lcmd.Parameters.Add(FEEDPROCESSID_oparam);
                    lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
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
                    logger.Info(string.Format("Insert StgTestcase ValidationTable end | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
                    //check validation
                    ValidationSteps = ExecuteCheckValidationTestCase(int.Parse(lvalFeed), lschema, lConnectionStr);
                    return ValidationSteps;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase InsertStgTestcaseValidationTable method | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase InsertStgTestcaseValidationTable method | TestCaseId: {0} | UserName: {1}", testCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase InsertStgTestcaseValidationTable method | TestCaseId: {0} | UserName: {1}", testCaseId, Username), ex.InnerException);
                if (ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase InsertStgTestcaseValidationTable method | TestCaseId: {0} | UserName: {1}", testCaseId, Username), ex.InnerException.InnerException);

                throw;
            }
        }

        public List<DataSummaryModel> GetDatasetNamebyTestcaseId(long lTestcaseId)
        {
            try
            {
                logger.Info(string.Format("GetDatasetNamebyTestcaseId start | TestCaseId: {0} | UserName: {1}", lTestcaseId, Username));
                var lDatasetName = from u in entity.T_TEST_DATA_SUMMARY
                                   join ds in entity.REL_TC_DATA_SUMMARY on u.DATA_SUMMARY_ID equals ds.DATA_SUMMARY_ID
                                   where ds.TEST_CASE_ID == lTestcaseId
                                   select new DataSummaryModel { Data_Summary_Name = u.ALIAS_NAME, DATA_SUMMARY_ID = u.DATA_SUMMARY_ID };

                logger.Info(string.Format("GetDatasetNamebyTestcaseId end | TestCaseId: {0} | UserName: {1}", lTestcaseId, Username));
                return lDatasetName.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetDatasetNamebyTestcaseId method | testcaseId: {0} | UserName: {1}", lTestcaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetDatasetNamebyTestcaseId method | testcaseId: {0} | UserName: {1}", lTestcaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in GetDatasetNamebyTestcaseId method | testcaseId: {0} | UserName: {1}", lTestcaseId, Username), ex.InnerException);
                throw;
            }
        }

        public void InsertStgTestCaseSave(string lConnectionStr, string lschema, DataTable dt, string testCaseId, long valFeedD)
        {
            try
            {
                logger.Info(string.Format("InsertStgTestCaseSave start | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
                OracleTransaction ltransaction;
                logger.Info(string.Format("insert TBLSTGTESTCASESAVE table start | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();

                string lcmdquery = "insert into TBLSTGTESTCASESAVE ( STEPSID,KEYWORD,OBJECT,PARAMETER,LCOMMENTS,ROWNUMBER,DATASETNAME,DATASETID,DATASETVALUE,DATA_SETTING_ID,SKIP,FEEDPROCESSDETAILID,TYPE,WHICHTABLE,TESTCASEID,TESTSUITEID,ParentObj) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14,:15,:16,:17)";
                int[] ids = new int[dt.Rows.Count];
                using (var lcmd = lconnection.CreateCommand())
                {
                    lcmd.CommandText = lcmdquery;
                    lcmd.ArrayBindCount = ids.Length;

                    string[] STEPSID_param = dt.AsEnumerable().Select(r => r.Field<string>("STEPSID")).ToArray();
                    string[] KEYWORD_param = dt.AsEnumerable().Select(r => r.Field<string>("KEYWORD")).ToArray();
                    string[] OBJECT_param = dt.AsEnumerable().Select(r => r.Field<string>("OBJECT")).ToArray();
                    string[] PARAMETER_param = dt.AsEnumerable().Select(r => r.Field<string>("PARAMETER")).ToArray();
                    string[] COMMENTS_param = dt.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
                    string[] ROWNUMBER_param = dt.AsEnumerable().Select(r => r.Field<string>("ROWNUMBER")).ToArray();
                    string[] DATASETNAME_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                    string[] DATASETID_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETID")).ToArray();
                    string[] DATASETVALUE_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETVALUE")).ToArray();
                    string[] Data_Setting_Id_param = dt.AsEnumerable().Select(r => r.Field<string>("Data_Setting_Id")).ToArray();
                    string[] SKIP_param = dt.AsEnumerable().Select(r => r.Field<string>("SKIP")).ToArray();
                    string[] FEEDPROCESSDETAILID_param = dt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                    string[] Type_param = dt.AsEnumerable().Select(r => r.Field<string>("Type")).ToArray();
                    string[] WhichTable_param = dt.AsEnumerable().Select(r => r.Field<string>("WhichTable")).ToArray();
                    string[] TestcaseId_param = dt.AsEnumerable().Select(r => r.Field<string>("TestcaseId")).ToArray();
                    string[] TestsuiteId_param = dt.AsEnumerable().Select(r => r.Field<string>("TestsuiteId")).ToArray();
                    string[] ParentObj_param = dt.AsEnumerable().Select(r => r.Field<string>("ParentObj")).ToArray();

                    OracleParameter STEPSID_oparam = new OracleParameter();
                    STEPSID_oparam.OracleDbType = OracleDbType.Varchar2;
                    STEPSID_oparam.Value = STEPSID_param;

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

                    OracleParameter ROWNUMBER_oparam = new OracleParameter();
                    ROWNUMBER_oparam.OracleDbType = OracleDbType.Varchar2;
                    ROWNUMBER_oparam.Value = ROWNUMBER_param;

                    OracleParameter DATASETNAME_oparam = new OracleParameter();
                    DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                    DATASETNAME_oparam.Value = DATASETNAME_param;

                    OracleParameter DATASETID_oparam = new OracleParameter();
                    DATASETID_oparam.OracleDbType = OracleDbType.Varchar2;
                    DATASETID_oparam.Value = DATASETID_param;

                    OracleParameter DATASETVALUE_oparam = new OracleParameter();
                    DATASETVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                    DATASETVALUE_oparam.Value = DATASETVALUE_param;

                    OracleParameter Data_Setting_Id_oparam = new OracleParameter();
                    Data_Setting_Id_oparam.OracleDbType = OracleDbType.Varchar2;
                    Data_Setting_Id_oparam.Value = Data_Setting_Id_param;

                    OracleParameter SKIP_oparam = new OracleParameter();
                    SKIP_oparam.OracleDbType = OracleDbType.Varchar2;
                    SKIP_oparam.Value = SKIP_param;

                    OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                    FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                    FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                    OracleParameter TYPE_oparam = new OracleParameter();
                    TYPE_oparam.OracleDbType = OracleDbType.Varchar2;
                    TYPE_oparam.Value = Type_param;

                    OracleParameter WhichTable_oparam = new OracleParameter();
                    WhichTable_oparam.OracleDbType = OracleDbType.Varchar2;
                    WhichTable_oparam.Value = WhichTable_param;

                    OracleParameter TestcaseId_oparam = new OracleParameter();
                    TestcaseId_oparam.OracleDbType = OracleDbType.Varchar2;
                    TestcaseId_oparam.Value = TestcaseId_param;

                    OracleParameter TestsuiteId_oparam = new OracleParameter();
                    TestsuiteId_oparam.OracleDbType = OracleDbType.Varchar2;
                    TestsuiteId_oparam.Value = TestsuiteId_param;

                    OracleParameter ParentObj_oparam = new OracleParameter();
                    ParentObj_oparam.OracleDbType = OracleDbType.Varchar2;
                    ParentObj_oparam.Value = ParentObj_param;

                    lcmd.Parameters.Add(STEPSID_oparam);
                    lcmd.Parameters.Add(KEYWORD_oparam);
                    lcmd.Parameters.Add(OBJECT_oparam);
                    lcmd.Parameters.Add(PARAMETER_oparam);
                    lcmd.Parameters.Add(COMMENTS_oparam);
                    lcmd.Parameters.Add(ROWNUMBER_oparam);
                    lcmd.Parameters.Add(DATASETNAME_oparam);
                    lcmd.Parameters.Add(DATASETID_oparam);
                    lcmd.Parameters.Add(DATASETVALUE_oparam);
                    lcmd.Parameters.Add(Data_Setting_Id_oparam);
                    lcmd.Parameters.Add(SKIP_oparam);
                    lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                    lcmd.Parameters.Add(TYPE_oparam);
                    lcmd.Parameters.Add(WhichTable_oparam);
                    lcmd.Parameters.Add(TestcaseId_oparam);
                    lcmd.Parameters.Add(TestsuiteId_oparam);
                    lcmd.Parameters.Add(ParentObj_oparam);

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
                    logger.Info(string.Format("insert TBLSTGTESTCASESAVE table end | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
                    lconnection.Close();

                    ObjectParameter op = new ObjectParameter("RESULT", "");
                    logger.Info(string.Format("SP_SaveTestcase SP start Execution | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
                    entity.SP_SaveTestcase(valFeedD, int.Parse(testCaseId), op);
                    entity.SaveChanges();
                    logger.Info(string.Format("SP_SaveTestcase SP end Execution | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
                }
                logger.Info(string.Format("InsertStgTestCaseSave end | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in InsertStgTestCaseSave method | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in InsertStgTestCaseSave method | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in InsertStgTestCaseSave method | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username), ex.InnerException);
                if (ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in InsertStgTestCaseSave method | TestCaseId: {0} | valFeedD: {1} | UserName: {2}", testCaseId, valFeedD, Username), ex.InnerException.InnerException);

                throw;
            }
        }

        public List<DataTagCommonViewModel> ListOfGroup()
        {
            try
            {
                logger.Info(string.Format("ListOfGroup start | Username: {0}", Username));
                var lList = entity.T_TEST_GROUP.Select(y => new DataTagCommonViewModel
                {
                    Id = y.GROUPID,
                    Name = y.GROUPNAME,
                    Description = y.DESCRIPTION,
                    IsActive = y.ACTIVE,
                    Active = y.ACTIVE == 1 ? "Y" : "N"
                }).OrderBy(z => z.Name).ToList();
                if (lList.Any())
                {
                    lList.RemoveAll(x => x.Name == null);
                }
                logger.Info(string.Format("ListOfGroup end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for ListOfGroup method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for ListOfGroup method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for ListOfGroup method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditGroup(DataTagCommonViewModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.Name))
                    {
                        lEntity.Name = lEntity.Name.Trim();
                    }
                    var flag = false;
                    if (lEntity.Id == 0)
                    {
                        logger.Info(string.Format("Add Group start | Group: {0} | UserName: {1}", lEntity.Name, Username));
                        var tbl = new T_TEST_GROUP();
                        tbl.GROUPID = Helper.NextTestSuiteId("SEQ_T_TEST_GROUP");
                        tbl.GROUPNAME = lEntity.Name;
                        tbl.DESCRIPTION = lEntity.Description;
                        tbl.ACTIVE = lEntity.IsActive;
                        tbl.CREATION_DATE = DateTime.Now;
                        tbl.UPDATE_DATE = DateTime.Now;
                        tbl.CREATION_USER = Username;
                        tbl.UPDATE_CREATION_USER = Username;
                        lEntity.Id = tbl.GROUPID;
                        entity.T_TEST_GROUP.Add(tbl);
                        entity.SaveChanges();
                        flag = true;
                        logger.Info(string.Format("Add Group end | Group: {0} | UserName: {1}", lEntity.Name, Username));
                    }
                    else
                    {
                        logger.Info(string.Format("Edit Group start | Group: {0} | GroupId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                        var tbl = entity.T_TEST_GROUP.Find(lEntity.Id);

                        if (tbl != null)
                        {
                            tbl.DESCRIPTION = lEntity.Description;
                            tbl.ACTIVE = lEntity.IsActive;
                            tbl.UPDATE_DATE = DateTime.Now;
                            tbl.UPDATE_CREATION_USER = Username;
                            entity.SaveChanges();
                        }
                        flag = true;
                        logger.Info(string.Format("Edit Group end | Group: {0} | GroupId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for AddEditGroup method | Group: {0} | GroupId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for AddEditGroup method | Group: {0} | GroupId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for AddEditGroup method | Group: {0} | GroupId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateGroupNameExist(string Name, long? Id)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Group Name Exist start | Group: {0} | GroupId: {1} | UserName: {2}", Name, Id, Username));
                var lresult = false;
                if (Id != null)
                {
                    lresult = entity.T_TEST_GROUP.Any(x => x.GROUPID != Id && x.GROUPNAME.ToLower().Trim() == Name.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_GROUP.Any(x => x.GROUPNAME.ToLower().Trim() == Name.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Group Name Exist end | Group: {0} | GroupId: {1} | UserName: {2}", Name, Id, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for CheckDuplicateGroupNameExist method | Group: {0} | GroupId: {1} | UserName: {2}", Name, Id, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for CheckDuplicateGroupNameExist method | Group: {0} | GroupId: {1} | UserName: {2}", Name, Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for CheckDuplicateGroupNameExist method | Group: {0} | GroupId: {1} | UserName: {2}", Name, Id, Username), ex.InnerException);
                throw;
            }
        }

        public List<DataTagCommonViewModel> ListOfSet()
        {
            try
            {
                logger.Info(string.Format("ListOfSet start | Username: {0}", Username));
                var lList = entity.T_TEST_SET.Select(y => new DataTagCommonViewModel
                {
                    Id = y.SETID,
                    Name = y.SETNAME,
                    Description = y.DESCRIPTION,
                    IsActive = y.ACTIVE,
                    Active = y.ACTIVE == 1 ? "Y" : "N"
                }).OrderBy(z => z.Name).ToList();
                if (lList.Any())
                {
                    lList.RemoveAll(x => x.Name == null);
                }
                logger.Info(string.Format("ListOfSet end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for ListOfSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for ListOfSet method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for ListOfSet method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditSet(DataTagCommonViewModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.Name))
                    {
                        lEntity.Name = lEntity.Name.Trim();
                    }
                    var flag = false;
                    if (lEntity.Id == 0)
                    {
                        logger.Info(string.Format("Add Set start | Set: {0} | UserName: {1}", lEntity.Name, Username));
                        var tbl = new T_TEST_SET();
                        tbl.SETID = Helper.NextTestSuiteId("SEQ_T_TEST_SET");
                        tbl.SETNAME = lEntity.Name;
                        tbl.DESCRIPTION = lEntity.Description;
                        tbl.ACTIVE = lEntity.IsActive;
                        tbl.CREATION_DATE = DateTime.Now;
                        tbl.UPDATE_DATE = DateTime.Now;
                        tbl.CREATION_USER = Username;
                        tbl.UPDATE_CREATION_USER = Username;
                        lEntity.Id = tbl.SETID;
                        entity.T_TEST_SET.Add(tbl);
                        entity.SaveChanges();
                        flag = true;
                        logger.Info(string.Format("Add Set end | Set: {0} | UserName: {1}", lEntity.Name, Username));
                    }
                    else
                    {
                        logger.Info(string.Format("Edit Set start | Set: {0} | SetId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                        var tbl = entity.T_TEST_SET.Find(lEntity.Id);

                        if (tbl != null)
                        {
                            tbl.DESCRIPTION = lEntity.Description;
                            tbl.ACTIVE = lEntity.IsActive;
                            tbl.UPDATE_DATE = DateTime.Now;
                            tbl.UPDATE_CREATION_USER = Username;
                            entity.SaveChanges();
                        }
                        flag = true;
                        logger.Info(string.Format("Edit Set end | Set: {0} | SetId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for AddEditSet method | Set: {0} | SetId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for AddEditSet method | Set: {0} | SetId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for AddEditSet method | Set: {0} | SetId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateSetNameExist(string Name, long? Id)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Set Name Exist start | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username));
                var lresult = false;
                if (Id != null)
                {
                    lresult = entity.T_TEST_SET.Any(x => x.SETID != Id && x.SETNAME.ToLower().Trim() == Name.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_SET.Any(x => x.SETNAME.ToLower().Trim() == Name.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Set Name Exist end | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for CheckDuplicateSetNameExist method | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for CheckDuplicateSetNameExist method | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for CheckDuplicateSetNameExist method | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username), ex.InnerException);
                throw;
            }
        }

        public List<DataTagCommonViewModel> ListOfFolder()
        {
            try
            {
                logger.Info(string.Format("ListOfFolder start | Username: {0}", Username));
                var lList = entity.T_TEST_FOLDER.Select(y => new DataTagCommonViewModel
                {
                    Id = y.FOLDERID,
                    Name = y.FOLDERNAME,
                    Description = y.DESCRIPTION,
                    IsActive = y.ACTIVE,
                    Active = y.ACTIVE == 1 ? "Y" : "N"
                }).OrderBy(z => z.Name).ToList();

                if (lList.Any())
                {
                    lList.RemoveAll(x => x.Name == null);
                }
                logger.Info(string.Format("ListOfFolder end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for ListOfFolder method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for ListOfFolder method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for ListOfFolder method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditFolder(DataTagCommonViewModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.Name))
                    {
                        lEntity.Name = lEntity.Name.Trim();
                    }
                    var flag = false;
                    if (lEntity.Id == 0)
                    {
                        logger.Info(string.Format("Add Folder start | Folder: {0} | UserName: {1}", lEntity.Name, Username));
                        var tbl = new T_TEST_FOLDER();
                        tbl.FOLDERID = Helper.NextTestSuiteId("SEQ_T_TEST_FOLDER");
                        tbl.FOLDERNAME = lEntity.Name;
                        tbl.DESCRIPTION = lEntity.Description;
                        tbl.ACTIVE = lEntity.IsActive;
                        tbl.CREATION_DATE = DateTime.Now;
                        tbl.UPDATE_DATE = DateTime.Now;
                        tbl.CREATION_USER = Username;
                        tbl.UPDATE_CREATION_USER = Username;
                        lEntity.Id = tbl.FOLDERID;
                        entity.T_TEST_FOLDER.Add(tbl);
                        entity.SaveChanges();
                        flag = true;
                        logger.Info(string.Format("Add Folder end | Folder: {0} | UserName: {1}", lEntity.Name, Username));
                    }
                    else
                    {
                        logger.Info(string.Format("Edit Folder start | Folder: {0} | FolderId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                        var tbl = entity.T_TEST_FOLDER.Find(lEntity.Id);

                        if (tbl != null)
                        {
                            tbl.DESCRIPTION = lEntity.Description;
                            tbl.ACTIVE = lEntity.IsActive;
                            tbl.UPDATE_DATE = DateTime.Now;
                            tbl.UPDATE_CREATION_USER = Username;
                            entity.SaveChanges();
                        }
                        flag = true;
                        logger.Info(string.Format("Edit Folder end | Folder: {0} | FolderId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for AddEditFolder method | Folder: {0} | FolderId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for AddEditFolder method | Folder: {0} | FolderId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for AddEditFolder method | Folder: {0} | FolderId: {1} | UserName: {2}", lEntity.Name, lEntity.Id, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateFolderNameExist(string Name, long? Id)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Folder Name Exist start | Folder: {0} | FolderId: {1} | UserName: {2}", Name, Id, Username));
                var lresult = false;
                if (Id != null)
                {
                    lresult = entity.T_TEST_FOLDER.Any(x => x.FOLDERID != Id && x.FOLDERNAME.ToLower().Trim() == Name.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_FOLDER.Any(x => x.FOLDERNAME.ToLower().Trim() == Name.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Folder Name Exist end | Folder: {0} | FolderId: {1} | UserName: {2}", Name, Id, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for CheckDuplicateFolderNameExist method | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for CheckDuplicateFolderNameExist method | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for CheckDuplicateFolderNameExist method | Set: {0} | SetId: {1} | UserName: {2}", Name, Id, Username), ex.InnerException);
                throw;
            }
        }
        public List<T_TEST_FOLDER> GetFolders()
        {
            try
            {
                logger.Info(string.Format("GetFolders start | Username: {0}", Username));
                var folders = entity.T_TEST_FOLDER.Where(y => y.ACTIVE == 1).Distinct().ToList();
                logger.Info(string.Format("GetFolders end | Username: {0}", Username));
                return folders;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetFolders method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetFolders method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetFolders method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public List<T_TEST_SET> GetSets()
        {
            try
            {
                logger.Info(string.Format("GetSets start | Username: {0}", Username));
                var sets = entity.T_TEST_SET.Where(y => y.ACTIVE == 1).Distinct().ToList();
                logger.Info(string.Format("GetSets end | Username: {0}", Username));
                return sets;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetSets method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetSets method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetSets method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public List<T_TEST_GROUP> GetGroups()
        {
            try
            {
                logger.Info(string.Format("GetGroups start | Username: {0}", Username));
                var groups = entity.T_TEST_GROUP.Where(y => y.ACTIVE == 1).Distinct().ToList();
                logger.Info(string.Format("GetGroups end | Username: {0}", Username));
                return groups;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetGroups method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetGroups method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetGroups method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public List<DataSetTagModel> GetTagDetails(long datasetid)
        {
            try
            {
                logger.Info(string.Format("GetTagDetails start | datasetid: {0} | Username: {1}", datasetid, Username));
                //var tags = (from k in entity.T_TEST_DATASETTAG
                //            join k1 in entity.T_TEST_GROUP on long.Parse(k.GROUPID) equals k1.GROUPID into grp
                //            from k2 in grp.DefaultIfEmpty() 
                //            join k3 in entity.T_TEST_FOLDER on long.Parse(k.FOLDERID) equals k3.FOLDERID into fold
                //            from k4 in fold.DefaultIfEmpty()
                //            join k5 in entity.T_TEST_SET on long.Parse(k.SETID) equals k5.SETID into set
                //            from k6 in set.DefaultIfEmpty()

                //            where k.DATASETID == datasetid
                //            select new DataSetTagModel
                //            {
                //                Group = k2.GROUPNAME == null ? "" : k2.GROUPNAME,
                //                Set = k6.SETNAME == null ? "" : k6.SETNAME,
                //                Folder = k4.FOLDERNAME == null ? "" : k4.FOLDERNAME,
                //                Sequence = k.SEQUENCE == null ? 0 : k.SEQUENCE,
                //                Expectedresults = k.EXPECTEDRESULTS == null ? "" : k.EXPECTEDRESULTS,
                //                Diary = k.DIARY == null ? "" : k.DIARY,
                //                Datasetid = k.DATASETID == null ? 0 : k.DATASETID,
                //                Tagid = k.T_TEST_DATASETTAG_ID,
                //                StepDesc = k.STEPDESC == null ? "" : k.STEPDESC,

                //            }

                //     ).ToList();

                var tags = (from k in entity.T_TEST_DATASETTAG.AsEnumerable()
                            join k1 in entity.T_TEST_GROUP.AsEnumerable() on new { groupId = k.GROUPID } equals new { groupId = k1.GROUPID.ToString() } into grp
                            from k2 in grp.DefaultIfEmpty()
                            join k3 in entity.T_TEST_FOLDER.AsEnumerable() on new { folderId = k.FOLDERID } equals new { folderId = k3.FOLDERID.ToString() } into fold
                            from k4 in fold.DefaultIfEmpty()
                            join k5 in entity.T_TEST_SET.AsEnumerable() on new { setId = k.SETID } equals new { setId = k5.SETID.ToString() } into set
                            from k6 in set.DefaultIfEmpty()
                            where k.DATASETID == datasetid
                            select new DataSetTagModel
                            {
                                Group = k2 == null ? "" : k2.GROUPNAME,
                                Set = k6 == null ? "" : k6.SETNAME,
                                Folder = k4 == null ? "" : k4.FOLDERNAME,
                                Sequence = k == null ? 0 : k.SEQUENCE,
                                Expectedresults = k == null ? "" : k.EXPECTEDRESULTS,
                                Diary = k == null ? "" : k.DIARY,
                                Datasetid = k == null ? 0 : k.DATASETID,
                                Tagid = k.T_TEST_DATASETTAG_ID,
                                StepDesc = k == null ? "" : k.STEPDESC
                            }).ToList();
                logger.Info(string.Format("GetTagDetails end | datasetid: {0} | Username: {1}", datasetid, Username));
                return tags;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetTagDetails method | DataSet Id: {0} | UserName: {1}", datasetid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetTagDetails method | DataSet Id: {0} | UserName: {1}", datasetid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetTagDetails method | DataSet Id: {0} | UserName: {1}", datasetid, Username), ex.InnerException);
                throw;
            }
        }
        public List<DataSummaryModel> GetDataSetName(long datasetid)
        {
            try
            {
                logger.Info(string.Format("GetDataSetName start | datasetid: {0} | Username: {1}", datasetid, Username));
                var dataset = entity.T_TEST_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == datasetid).Select(x => new DataSummaryModel
                {
                    DATA_SUMMARY_ID = x.DATA_SUMMARY_ID,
                    Data_Summary_Name = x.ALIAS_NAME,
                    Dataset_desc = x.DESCRIPTION_INFO
                }
        ).Distinct().ToList();
                logger.Info(string.Format("GetDataSetName end | datasetid: {0} | Username: {1}", datasetid, Username));
                return dataset;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetDataSetName method | DataSet Id: {0} | UserName: {1}", datasetid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetDataSetName method | DataSet Id: {0} | UserName: {1}", datasetid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetDataSetName method | DataSet Id: {0} | UserName: {1}", datasetid, Username), ex.InnerException);
                throw;
            }
        }

        public string DeleteTagProperties(long datasetid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("DeleteTagProperties start | datasetid: {0} | Username: {1}", datasetid, Username));
                    var result = entity.T_TEST_DATASETTAG.Where(x => x.DATASETID == datasetid).FirstOrDefault();
                    if (result != null)
                    {
                        entity.T_TEST_DATASETTAG.Remove(result);
                        entity.SaveChanges();
                    }
                    logger.Info(string.Format("DeleteTagProperties end | datasetid: {0} | Username: {1}", datasetid, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for DeleteTagProperties method | DataSet Id: {0} | UserName: {1}", datasetid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for DeleteTagProperties method | DataSet Id: {0} | UserName: {1}", datasetid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for DeleteTagProperties method | DataSet Id: {0} | UserName: {1}", datasetid, Username), ex.InnerException);
                throw;
            }
        }
        public bool CheckFolderSequenceMapping(long FolderId, long SequenceId, long datasetid)
        {
            try
            {
                logger.Info(string.Format("CheckFolderSequenceMapping start | datasetid: {0} | FolderId: {1} | SequenceId: {2} | Username: {2}", datasetid, FolderId, SequenceId, Username));
                bool result = entity.T_TEST_DATASETTAG.Any(x => x.FOLDERID == FolderId.ToString() && x.SEQUENCE == SequenceId && x.DATASETID != datasetid);
                logger.Info(string.Format("CheckFolderSequenceMapping end | datasetid: {0} | FolderId: {1} | SequenceId: {2} | Username: {2}", datasetid, FolderId, SequenceId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for CheckFolderSequenceMapping method | Folder Id : {0} | Sequesnce Id : {1} | DataSet Id: {2} | UserName: {3}", FolderId, SequenceId, datasetid, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for CheckFolderSequenceMapping method | Folder Id : {0} | Sequesnce Id : {1} | DataSet Id: {2} | UserName: {3}", FolderId, SequenceId, datasetid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for CheckFolderSequenceMapping method | Folder Id : {0} | Sequesnce Id : {1} | DataSet Id: {2} | UserName: {3}", FolderId, SequenceId, datasetid, Username), ex.InnerException);
                throw;
            }
        }

        public List<ApplicationModel> GetApplicationListByTestcaseId(long TestcaseId)
        {
            try
            {
                logger.Info(string.Format("Get ApplicationList By TestcaseId start | TestcaseId: {0} | Username: {1} | Start time: {2}", TestcaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                var lAppList = new List<ApplicationModel>();
                var lList = from u in entity.REL_APP_TESTCASE
                            join r in entity.T_REGISTERED_APPS on u.APPLICATION_ID equals r.APPLICATION_ID
                            where u.TEST_CASE_ID == TestcaseId
                            select new ApplicationModel
                            {
                                ApplicationId = r.APPLICATION_ID,
                                ApplicationName = r.APP_SHORT_NAME
                            };
                lAppList = lList.ToList();
                logger.Info(string.Format("Get ApplicationList By TestcaseId end | TestcaseId: {0} | Username: {1} | End time: {2}", TestcaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                return lAppList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestcaseId for GetApplicationListByTestcaseId method | TestcaseId: {0} | UserName: {1}", TestcaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestcaseId in GetApplicationListByTestcaseId method | TestcaseId: {0} | UserName: {1}", TestcaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestcaseId in GetApplicationListByTestcaseId method | TestcaseId: {0} | UserName: {1}", TestcaseId, Username), ex.InnerException);
                throw;
            }
        }

        public long GetDatasetId(string Dataset)
        {
            try
            {
                logger.Info(string.Format("GetDatasetId start | Dataset: {0} | Username: {1}", Dataset, Username));
                long DatasetId = 0;
                if (!string.IsNullOrEmpty(Dataset))
                {
                    var obj = entity.T_TEST_DATA_SUMMARY.Where(x => x.ALIAS_NAME == Dataset).FirstOrDefault();
                    if (obj != null)
                        DatasetId = obj.DATA_SUMMARY_ID;
                }
                logger.Info(string.Format("GetDatasetId end | Dataset: {0} | Username: {1}", Dataset, Username));
                return DatasetId;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestcaseId for GetDatasetId method | Dataset: {0} | UserName: {1}", Dataset, Username));
                ELogger.ErrorException(string.Format("Error occured TestcaseId in GetDatasetId method | Dataset: {0} | UserName: {1}", Dataset, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestcaseId in GetDatasetId method | Dataset: {0} | UserName: {1}", Dataset, Username), ex.InnerException);
                throw;
            }
        }

        public IList<FolderDatasetViewModel> GetFolderDatasetList(string schema, string lconstring, long FolderId)
        {
            try
            {
                logger.Info(string.Format("Get Folder Dataset List start | FolderId: {0} | UserName: {1}", FolderId, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;


                OracleParameter[] ladd_refer_image = new OracleParameter[2];
                ladd_refer_image[0] = new OracleParameter("FolderId", OracleDbType.Long);
                ladd_refer_image[0].Value = FolderId;

                ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[1].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_FOLDERDATASETLIST";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<FolderDatasetViewModel> result = dt.AsEnumerable().Select(row =>
                  new FolderDatasetViewModel
                  {
                      DataSetId = row.Field<long?>("DATA_SUMMARY_ID"),
                      DatasetName = row.Field<string>("ALIAS_NAME"),
                      DatasetDesc = row.Field<string>("DESCRIPTION_INFO"),
                      TestCase = row.Field<string>("TEST_CASE_NAME"),
                      TestCaseId = row.Field<long?>("TEST_CASE_ID"),
                      TestSuite = row.Field<string>("TEST_SUITE_NAME"),
                      TestSuiteId = row.Field<long?>("TEST_SUITE_ID"),
                      Storyboard = row.Field<string>("STORYBOARD_NAME"),
                      ProjectIds = row.Field<string>("ASSIGNED_PROJECT_ID"),
                      ProjectName = row.Field<string>("PROJECT_NAME"),
                      SEQ = row.Field<long?>("sequence")
                  }).OrderBy(x => x.SEQ).ToList();

                result.ForEach(item =>
                {
                    item.DatasetDesc = item.DatasetDesc == null || item.DatasetDesc == "null" ? "" : item.DatasetDesc;
                    item.Storyboard = item.Storyboard == null || item.Storyboard == "null" ? "" : item.Storyboard;
                    item.ProjectName = item.ProjectName == null || item.ProjectName == "null" ? "" : item.ProjectName;
                    item.SEQ = item.SEQ == null ? 0 : item.SEQ;
                });
                logger.Info(string.Format("Get Folder Dataset List end | FolderId: {0} | UserName: {1}", FolderId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for GetFolderDatasetList method | ConnectionString: {0} | Schema : {1} | FolderId: {2} | UserName: {3}", lconstring, schema, FolderId, Username));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for GetFolderDatasetList method | ConnectionString: {0} | Schema : {1} | FolderId: {2} | UserName: {3}", lconstring, schema, FolderId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in StoryBoard for GetFolderDatasetList method | ConnectionString: {0} | Schema : {1} | FolderId: {2} | UserName: {3}", lconstring, schema, FolderId, Username), ex.InnerException);
                throw;
            }
        }

        public FolderDatasetViewModel GetDataset(string schema, string lconstring, long datasetId)
        {
            try
            {
                logger.Info(string.Format("Get Dataset List start | UserName: {0}", Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;


                OracleParameter[] ladd_refer_image = new OracleParameter[2];
                ladd_refer_image[0] = new OracleParameter("DatasetId", OracleDbType.Long);
                ladd_refer_image[0].Value = datasetId;

                ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[1].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_DATASETLIST";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                FolderDatasetViewModel result = dt.AsEnumerable().Select(row =>
                  new FolderDatasetViewModel
                  {
                      DataSetId = row.Field<long?>("DATA_SUMMARY_ID"),
                      DatasetName = row.Field<string>("ALIAS_NAME"),
                      DatasetDesc = row.Field<string>("DESCRIPTION_INFO"),
                      TestCase = row.Field<string>("TEST_CASE_NAME"),
                      TestCaseId = row.Field<long?>("TEST_CASE_ID"),
                      TestSuite = row.Field<string>("TEST_SUITE_NAME"),
                      TestSuiteId = row.Field<long?>("TEST_SUITE_ID"),
                      Storyboard = row.Field<string>("STORYBOARD_NAME"),
                      ProjectIds = row.Field<string>("ASSIGNED_PROJECT_ID"),
                      ProjectName = row.Field<string>("PROJECT_NAME"),
                  }).FirstOrDefault();

                result.DatasetDesc = result.DatasetDesc == null || result.DatasetDesc == "null" ? "" : result.DatasetDesc;
                result.Storyboard = result.Storyboard == null || result.Storyboard == "null" ? "" : result.Storyboard;
                result.ProjectName = result.ProjectName == null || result.ProjectName == "null" ? "" : result.ProjectName;

                logger.Info(string.Format("Get Dataset List end | UserName: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetDataset method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetDataset method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetDataset method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public IList<DataSummaryModel> GetDatasets()
        {
            try
            {
                logger.Info(string.Format("Get Dataset List start | UserName: {0}", Username));

                var dataset = (from u in entity.T_TEST_DATA_SUMMARY
                               join ds in entity.REL_TC_DATA_SUMMARY on u.DATA_SUMMARY_ID equals ds.DATA_SUMMARY_ID
                               join tc in entity.T_TEST_CASE_SUMMARY on ds.TEST_CASE_ID equals tc.TEST_CASE_ID
                               select new DataSummaryModel { Data_Summary_Name = u.ALIAS_NAME, DATA_SUMMARY_ID = u.DATA_SUMMARY_ID }).Distinct().ToList();

                logger.Info(string.Format("Get Dataset List 123 | list: {0}", dataset));
                logger.Info(string.Format("Get Dataset List end | UserName: {0}", Username));
                return dataset;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetDatasets method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetDatasets method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetDatasets method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public bool InsertFolderDatasetInStgTbl(string constring, string schema, DataTable dt, long feedprocessdetailid)
        {
            logger.Info(string.Format("Insert FolderDataset StgTbl start | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
            bool flag = false;

            dt = dt.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable();

            OracleTransaction ltransaction;

            OracleConnection lconnection = new OracleConnection(constring);
            lconnection.Open();
            ltransaction = lconnection.BeginTransaction();
            string lcmdquery = "insert into TBLSTGFOLDERDATASET ( FEEDPROCESSDETAILID,FOLDERID,DATASETID,DATASETNAME,DESCRIPTION,TESTCASE,TESTSUITE,STORYBOARDS,SEQ) values(:1,:2,:3,:4,:5,:6,:7,:8,:9)";
            int[] ids = new int[dt.Rows.Count];
            using (var lcmd = lconnection.CreateCommand())
            {
                lcmd.CommandText = lcmdquery;
                lcmd.ArrayBindCount = ids.Length;
                string[] FEEDPROCESSDETAILID_param = dt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                string[] FOLDERID_param = dt.AsEnumerable().Select(r => r.Field<string>("FOLDERID")).ToArray();
                string[] DATASETID_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETID")).ToArray();
                string[] DATASETNAME_param = dt.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                string[] DESCRIPTION_param = dt.AsEnumerable().Select(r => r.Field<string>("DESCRIPTION")).ToArray();
                string[] TESTCASE_param = dt.AsEnumerable().Select(r => r.Field<string>("TESTCASE")).ToArray();
                string[] TESTSUITE_param = dt.AsEnumerable().Select(r => r.Field<string>("TESTSUITE")).ToArray();
                string[] STORYBOARDS_param = dt.AsEnumerable().Select(r => r.Field<string>("STORYBOARDS")).ToArray();
                string[] SEQ_param = dt.AsEnumerable().Select(r => r.Field<string>("SEQ")).ToArray();

                OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                OracleParameter FOLDERID_oparam = new OracleParameter();
                FOLDERID_oparam.OracleDbType = OracleDbType.Varchar2;
                FOLDERID_oparam.Value = FOLDERID_param;

                OracleParameter DATASETID_oparam = new OracleParameter();
                DATASETID_oparam.OracleDbType = OracleDbType.Varchar2;
                DATASETID_oparam.Value = DATASETID_param;

                OracleParameter DATASETNAME_oparam = new OracleParameter();
                DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                DATASETNAME_oparam.Value = DATASETNAME_param;

                OracleParameter DESCRIPTION_oparam = new OracleParameter();
                DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                DESCRIPTION_oparam.Value = DESCRIPTION_param;

                OracleParameter TESTCASE_oparam = new OracleParameter();
                TESTCASE_oparam.OracleDbType = OracleDbType.Varchar2;
                TESTCASE_oparam.Value = TESTCASE_param;

                OracleParameter TESTSUITE_oparam = new OracleParameter();
                TESTSUITE_oparam.OracleDbType = OracleDbType.Varchar2;
                TESTSUITE_oparam.Value = TESTSUITE_param;

                OracleParameter STORYBOARDS_oparam = new OracleParameter();
                STORYBOARDS_oparam.OracleDbType = OracleDbType.Varchar2;
                STORYBOARDS_oparam.Value = STORYBOARDS_param;

                OracleParameter SEQ_oparam = new OracleParameter();
                SEQ_oparam.OracleDbType = OracleDbType.Varchar2;
                SEQ_oparam.Value = SEQ_param;


                lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                lcmd.Parameters.Add(FOLDERID_oparam);
                lcmd.Parameters.Add(DATASETID_oparam);
                lcmd.Parameters.Add(DATASETNAME_oparam);
                lcmd.Parameters.Add(DESCRIPTION_oparam);
                lcmd.Parameters.Add(TESTCASE_oparam);
                lcmd.Parameters.Add(TESTSUITE_oparam);
                lcmd.Parameters.Add(STORYBOARDS_oparam);
                lcmd.Parameters.Add(SEQ_oparam);
                try
                {
                    lcmd.ExecuteNonQuery();
                    flag = true;
                }
                catch (Exception ex)
                {
                    flag = false;
                    ltransaction.Rollback();
                    logger.Error(string.Format("Error occured in Testcase for InsertFolderDatasetInStgTbl method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username));
                    ELogger.ErrorException(string.Format("Error occured Testcase in InsertFolderDatasetInStgTbl method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username), ex);
                    if (ex.InnerException != null)
                        ELogger.ErrorException(string.Format("InnerException : Error occured Testcase in InsertFolderDatasetInStgTbl method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username), ex.InnerException);
                    throw;
                }
                logger.Info(string.Format("Insert FolderDataset StgTbl end | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                ltransaction.Commit();
                lconnection.Close();
            }
            return flag;
        }

        public bool updateFolderDataset(string constring, string schema, string feedprocessdetailid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("update folder Dataset start | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                    var result = entity.SP_SAVE_FOLDER_DATASET(feedprocessdetailid);
                    entity.SaveChanges();
                    logger.Info(string.Format("update folder Dataset end | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for folderDataset method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in folderDataset method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in folderDataset method |Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteFolderDatasetStep(List<long> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete FolderDataset step start | UserName: {0}", Username));
                    var flag = false;
                    foreach (var id in list)
                    {
                        var obj = entity.T_TEST_DATASETTAG.Where(x => x.DATASETID == id).FirstOrDefault();

                        if (obj != null)
                            entity.T_TEST_DATASETTAG.Remove(obj);

                        //entity.DeleteDatsSets(id);
                        entity.SaveChanges();
                    }
                    flag = true;
                    logger.Info(string.Format("Delete FolderDataset step end | UserName: {0}", Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for DeleteFolderDatasetStep method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Testcase in DeleteFolderDatasetStep method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Testcase in DeleteFolderDatasetStep method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }


        public List<FolderDataSetViewModel> ReSequenceFolderDataset(List<FolderDataSetViewModel> gridlist)
        {
            try
            {
                List<FolderDataSetViewModel> folderDataSets = new List<FolderDataSetViewModel>();
                var emptySeqList = gridlist.Where(x => string.IsNullOrEmpty(x.SEQ)).ToList();
                var sortbySeqList = gridlist.Where(x => !string.IsNullOrEmpty(x.SEQ)).OrderBy(x => x.SEQ).ToList();
                sortbySeqList.AddRange(emptySeqList);
                int SEQ = 0;

                foreach (var item in sortbySeqList)
                {
                    FolderDataSetViewModel folderData = new FolderDataSetViewModel();
                    folderData.DataSetId = item.DataSetId;
                    folderData.DatasetName = item.DatasetName;
                    folderData.DatasetDesc = item.DatasetDesc == null ? "" : item.DatasetDesc;
                    folderData.TestCase = item.TestCase == null ? "" : item.TestCase;
                    folderData.TestSuite = item.TestSuite == null ? "" : item.TestSuite;
                    folderData.ProjectIds = item.ProjectIds == null ? "" : item.ProjectIds;
                    folderData.ProjectName = item.ProjectName == null ? "" : item.ProjectName;
                    folderData.Storyboard = item.Storyboard == null ? "" : item.Storyboard;

                    SEQ = SEQ + 10;
                    folderData.SEQ = SEQ.ToString();
                    folderDataSets.Add(folderData);
                }
                return folderDataSets;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for ReSequenceFolderDataset method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Testcase in ReSequenceFolderDataset method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Testcase in ReSequenceFolderDataset method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteFolderDataset(string constring, string schema, string feedprocessdetailid, long FolderId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete folder Dataset start | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));

                    var cmdText = "delete  T_TEST_DATASETTAG where DATASETID not in (select TO_NUMBER(DATASETID) from TBLSTGFOLDERDATASET where FEEDPROCESSDETAILID = " + feedprocessdetailid + ") and folderid= " + FolderId;
                    using (OracleConnection conn = new OracleConnection(constring))
                    using (OracleCommand cmd = new OracleCommand(cmdText, conn))
                    {
                        conn.Open();

                        cmd.ExecuteNonQuery();
                    }


                    logger.Info(string.Format("Delete folder Dataset end | feedprocessdetailid: {0} | UserName: {1}", feedprocessdetailid, Username));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for DeleteFolderDataset method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteFolderDataset method | Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in DeleteFolderDataset method |Connection string : {0} | Schema: {1} | Feed Process Id : {2} | UserName: {3}", constring, schema, feedprocessdetailid, Username), ex.InnerException);
                throw;
            }
        }

        public IList<T_FOLDER_FILTER> GetFilterList()
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

        public List<StoryboardModel> GetStoryboardById(string ProjectIds)
        {
            try
            {
                logger.Info(string.Format("GetStoryboardById start | ProjectIds: {0} | UserName: {1}", ProjectIds, Username));
                List<StoryboardModel> List = new List<StoryboardModel>();
                var ids = ProjectIds.Split(',');
                foreach (var id in ids)
                {
                    long Id = long.Parse(id);
                    var lResult = (from t2 in entity.T_STORYBOARD_SUMMARY
                                   join t1 in entity.T_TEST_PROJECT on t2.ASSIGNED_PROJECT_ID equals t1.PROJECT_ID
                                   where t2.ASSIGNED_PROJECT_ID == Id && t2.STORYBOARD_NAME != null
                                   select new StoryboardModel
                                   {
                                       Storyboardid = t2.STORYBOARD_ID,
                                       Storyboardname = t2.STORYBOARD_NAME,
                                   }).ToList();

                    List.AddRange(lResult);
                }
                logger.Info(string.Format("GetStoryboardById end | ProjectIds: {0} | UserName: {1}", ProjectIds, Username));
                return List;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetStoryboardById method | ProjectIds: {0} | UserName: {1}", ProjectIds, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetStoryboardById method | ProjectIds: {0} | UserName: {1}", ProjectIds, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetStoryboardById method | ProjectIds: {0} | UserName: {1}", ProjectIds, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateFilterName(string Name)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate StoryboardName start | Filtername: {0} | UserName: {1}", Name, Username));
                var lresult = entity.T_FOLDER_FILTER.Any(x => x.FILTER_NAME.ToLower().Trim() == Name.ToLower().Trim());
                logger.Info(string.Format("Check Duplicate StoryboardName end | Filtername: {0} | UserName: {1}", Name, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for CheckDuplicateFilterName method | Filtername: {0} | UserName: {1}", Name, Username));
                ELogger.ErrorException(string.Format("Error occured StoryBoard in CheckDuplicateFilterName method | Filtername: {0} | UserName: {1}", Name, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured StoryBoard in CheckDuplicateFilterName method | Filtername: {0} | UserName: {1}", Name, Username), ex.InnerException);
                throw;
            }
        }

        public bool SaveFolderFilter(string ProjectIds, string SBIds, string filtername, int Id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("SaveFolderFilter start | filtername: {0} | ProjectIds: {1} | SBIds: {2} | UserName: {3}", filtername, ProjectIds, SBIds, Username));
                    var flag = false;
                    if (Id > 0)
                    {
                        var filter = entity.T_FOLDER_FILTER.Where(x => x.FILTER_ID == Id).FirstOrDefault();
                        if (filter != null)
                        {
                            filter.FILTER_NAME = filtername;
                            filter.PROJECT_IDS = ProjectIds;
                            filter.STORYBOARD_IDS = SBIds;
                            entity.SaveChanges();
                            flag = true;
                        }
                    }
                    else
                    {
                        T_FOLDER_FILTER filter = new T_FOLDER_FILTER();
                        var filterID = Helper.NextTestSuiteId("SEQ_T_FOLDER_FILTER");
                        filter.FILTER_ID = filterID;
                        filter.FILTER_NAME = filtername;
                        filter.PROJECT_IDS = ProjectIds;
                        filter.STORYBOARD_IDS = SBIds;
                        entity.T_FOLDER_FILTER.Add(filter);
                        entity.SaveChanges();
                        flag = true;
                    }

                    logger.Info(string.Format("SaveFolderFilter end | filtername: {0} | ProjectIds: {1} | SBIds: {2} | UserName: {3}", filtername, ProjectIds, SBIds, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for SaveFolderFilter method | filtername: {0} | ProjectIds: {1} | SBIds: {2} | UserName: {3}", filtername, ProjectIds, SBIds, Username));
                ELogger.ErrorException(string.Format("Error occured Testcase in SaveFolderFilter method | filtername: {0} | ProjectIds: {1} | SBIds: {2} | UserName: {3}", filtername, ProjectIds, SBIds, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Testcase in SaveFolderFilter method | filtername: {0} | ProjectIds: {1} | SBIds: {2} | UserName: {3}", filtername, ProjectIds, SBIds, Username), ex.InnerException);
                throw;
            }
        }

        public T_FOLDER_FILTER GetFilter(long filterID)
        {
            try
            {
                logger.Info(string.Format("GetFilterList start | UserName: {0}", Username));
                var result = entity.T_FOLDER_FILTER.Where(x => x.FILTER_ID == filterID).FirstOrDefault();
                logger.Info(string.Format("GetFilterList end | UserName: {0}", Username));
                return result;
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

        public bool SaveFolderForFilter(long FolderID, long FilterId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("SaveFolderFilter start | FolderID: {0} | FilterId: {1} | UserName: {2}", FolderID, FilterId, Username));
                    var flag = false;
                    var filterIsExist = entity.REL_FOLDER_FILTER.Where(x => x.FOLDER_ID == FolderID).FirstOrDefault();
                    if (filterIsExist == null)
                    {
                        REL_FOLDER_FILTER filter = new REL_FOLDER_FILTER();
                        var filterID = Helper.NextTestSuiteId("SEQ_REL_FOLDER_FILTER");
                        filter.REL_FOLDER_FILTER_ID = filterID;
                        filter.FOLDER_ID = FolderID;
                        filter.FILTER_ID = FilterId;
                        entity.REL_FOLDER_FILTER.Add(filter);
                        entity.SaveChanges();
                        flag = true;
                    }
                    else
                    {
                        filterIsExist.FOLDER_ID = FolderID;
                        filterIsExist.FILTER_ID = FilterId;
                        entity.SaveChanges();
                        flag = true;
                    }
                    logger.Info(string.Format("SaveFolderFilter end | FolderID: {0} | FilterId: {1} | UserName: {2}", FolderID, FilterId, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for SaveFolderFilter method | FolderID: {0} | FilterId: {1} | UserName: {2}", FolderID, FilterId, Username));
                ELogger.ErrorException(string.Format("Error occured Testcase in SaveFolderFilter method | FolderID: {0} | FilterId: {1} | UserName: {2}", FolderID, FilterId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Testcase in SaveFolderFilter method | FolderID: {0} | FilterId: {1} | UserName: {2}", FolderID, FilterId, Username), ex.InnerException);
                throw;
            }
        }

        public string GetFilterByFolderId(long FolderID)
        {
            try
            {
                logger.Info(string.Format("GetFilterByFolderId start | FolderID : {0} | UserName: {1}", FolderID, Username));
                var lResult = (from t2 in entity.T_FOLDER_FILTER
                               join t1 in entity.REL_FOLDER_FILTER on t2.FILTER_ID equals t1.FILTER_ID
                               where t1.FOLDER_ID == FolderID && t2.FILTER_NAME != null
                               select t2.FILTER_ID).FirstOrDefault();

                string result = lResult.ToString();
                logger.Info(string.Format("GetFilterByFolderId end | FolderID : {0} | UserName: {1}", FolderID, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase for GetFilterByFolderId method | FolderID : {0} | UserName: {1}", FolderID, Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase for GetFilterByFolderId method | FolderID : {0} | UserName: {1}", FolderID, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase for GetFilterByFolderId method | FolderID : {0} | UserName: {1}", FolderID, Username), ex.InnerException);
                throw;
            }
        }

        public List<FilterModelView> GetFolderFilterList()
        {
            try
            {
                logger.Info(string.Format("GetFilterList start | UserName: {0}", Username));
                var folderlist = entity.T_FOLDER_FILTER.Select(x => new FilterModelView
                {
                    Id = x.FILTER_ID,
                    Name = x.FILTER_NAME,
                    ProjectIds = x.PROJECT_IDS,
                    StoryboradIds = x.STORYBOARD_IDS,
                }
        ).ToList();

                folderlist.ToList().ForEach(item =>
                {
                    item.Project = "";
                    item.Storyborad = "";
                    if (!string.IsNullOrEmpty(item.ProjectIds))
                    {
                        List<int> projectId = item.ProjectIds.Split(',').Select(int.Parse).ToList();
                        var projectList = entity.T_TEST_PROJECT.Where(a => projectId.Any(b => a.PROJECT_ID == b)).Select(x => x.PROJECT_NAME).ToList();
                        if (projectList.Any())
                            item.Project = String.Join(",", projectList);
                    }

                    if (!string.IsNullOrEmpty(item.StoryboradIds))
                    {
                        List<int> storyboradId = item.StoryboradIds.Split(',').Select(int.Parse).ToList();
                        var storyboradList = entity.T_STORYBOARD_SUMMARY.Where(a => storyboradId.Any(b => a.STORYBOARD_ID == b)).Select(x => x.STORYBOARD_NAME).ToList();
                        if (storyboradList.Any())
                            item.Storyborad = String.Join(",", storyboradList);
                    }
                });

                logger.Info(string.Format("GetFilterList start | UserName: {0}", Username));
                return folderlist;
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

        public bool DeleteFilter(int Id)
        {
            try
            {
                bool result = false;
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete Filter start | FilterId: {0} | UserName: {1}", Id, Username));

                    var filter = entity.T_FOLDER_FILTER.Where(x => x.FILTER_ID == Id).FirstOrDefault();
                    if (filter != null)
                    {
                        entity.T_FOLDER_FILTER.Remove(filter);
                        result = true;
                    }

                    entity.SaveChanges();
                    logger.Info(string.Format("Delete Filter end | FilterId: {0} | UserName: {1}", Id, Username));
                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {

                logger.Error(string.Format("Error occured TestCase in DeleteFilter method | Filter Id : {0} | UserName: {1}", Id, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteFilter method | Filter Id : {0} | UserName: {1}", Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in DeleteFilter method | Filter Id : {0} | UserName: {1}", Id, Username), ex.InnerException);
                throw;
            }
        }
        public bool SaveTestcaseSessionInDatabase(string connectionString, long testCaseId, Mars_Memory_TestCase steps)
        {
            try
            {
                logger.Info(string.Format("SAVE TESTCASE SESSION VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | START : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));

                List<MB_V_TEST_STEPS> addedSteps = steps.allSteps.Where(x => x.recordStatus.Equals(CommonEnum.MarsRecordStatus.en_NewToDb)).ToList();
                List<MB_V_TEST_STEPS> updatedSteps = steps.allSteps.Where(x => x.recordStatus.Equals(CommonEnum.MarsRecordStatus.en_ModifiedToDb)).ToList();
                List<MB_V_TEST_STEPS> deletedSteps = steps.allSteps.Where(x => x.recordStatus.Equals(CommonEnum.MarsRecordStatus.en_DeletedToDb)).ToList();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    OracleTransaction transaction;
                    // Start a local transaction
                    transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    // Assign transaction object for a pending local transaction
                    command.Transaction = transaction;

                    try
                    {
                        string dropTempTable = "DECLARE cnt NUMBER; BEGIN SELECT COUNT(*) INTO cnt FROM user_tables WHERE table_name = 'T_TEST_STEPS_TEMP'; IF cnt <> 0 THEN EXECUTE IMMEDIATE 'DROP TABLE T_TEST_STEPS_TEMP'; END IF; END;";
                        command.CommandText = dropTempTable;
                        command.ExecuteNonQuery();

                        string tempTestStepsTableQuery = "CREATE GLOBAL TEMPORARY TABLE T_TEST_STEPS_TEMP( STATUS VARCHAR2(50 BYTE), STEPS_ID NUMBER(16,0), RUN_ORDER NUMBER(38,0), KEY_WORD_ID NUMBER(16,0), TEST_CASE_ID NUMBER(16,0), OBJECT_ID NUMBER(16,0), COLUMN_ROW_SETTING VARCHAR2(128 BYTE), VALUE_SETTING VARCHAR2(128 BYTE), COMMENTS VARCHAR2(128 BYTE), IS_RUNNABLE NUMBER DEFAULT 0, OBJECT_NAME_ID NUMBER ) ON COMMIT PRESERVE ROWS";
                        command.CommandText = tempTestStepsTableQuery;
                        command.ExecuteNonQuery();

                        if (steps.allSteps.Count() > 0)
                        {
                            command.CommandText = string.Empty;
                            foreach (var item in addedSteps)
                            {
                                command.CommandText = "INSERT INTO T_TEST_STEPS_TEMP (STATUS, STEPS_ID, RUN_ORDER, KEY_WORD_ID, TEST_CASE_ID, OBJECT_ID, COLUMN_ROW_SETTING, VALUE_SETTING, COMMENTS, IS_RUNNABLE, OBJECT_NAME_ID) VALUES " +
                                                                           "('Added', T_TEST_STEPS_SEQ.nextval, " + item.RUN_ORDER + ", " + item.KEY_WORD_ID + ", " + testCaseId + ", '" + (item.OBJECT_ID != null ? item.OBJECT_ID : null) + "', '" + (!string.IsNullOrEmpty(item.COLUMN_ROW_SETTING) ? item.COLUMN_ROW_SETTING : string.Empty) + "', '" + (!string.IsNullOrEmpty(item.VALUE_SETTING) ? item.VALUE_SETTING : "") + "', '" + (!string.IsNullOrEmpty(item.COMMENTINFO) ? item.COMMENTINFO : "") + "', " + (item.IS_RUNNABLE != null ? item.IS_RUNNABLE : 0) + ", " + (item.OBJECT_NAME_ID != null ? item.OBJECT_NAME_ID : -1) + ") ";
                                command.ExecuteNonQuery();
                            }
                            foreach (var item in updatedSteps)
                            {
                                command.CommandText = "INSERT INTO T_TEST_STEPS_TEMP (STATUS, STEPS_ID, RUN_ORDER, KEY_WORD_ID, TEST_CASE_ID, OBJECT_ID, COLUMN_ROW_SETTING, VALUE_SETTING, COMMENTS, IS_RUNNABLE, OBJECT_NAME_ID) VALUES " +
                                                                            "('Updated', " + item.STEPS_ID + ", " + item.RUN_ORDER + ", " + item.KEY_WORD_ID + ", " + testCaseId + ", " + item.OBJECT_ID + ", '" + (!string.IsNullOrEmpty(item.COLUMN_ROW_SETTING) ? item.COLUMN_ROW_SETTING : string.Empty) + "', '" + (!string.IsNullOrEmpty(item.VALUE_SETTING) ? item.VALUE_SETTING : "") + "', '" + (!string.IsNullOrEmpty(item.COMMENTINFO) ? item.COMMENTINFO : "") + "', " + (item.IS_RUNNABLE != null ? item.IS_RUNNABLE : 0) + ", " + (item.OBJECT_NAME_ID != null ? item.OBJECT_NAME_ID : -1) + ") ";
                                command.ExecuteNonQuery();
                            }
                            foreach (var item in deletedSteps)
                            {
                                command.CommandText = "INSERT INTO T_TEST_STEPS_TEMP (STATUS, STEPS_ID, RUN_ORDER, KEY_WORD_ID, TEST_CASE_ID, OBJECT_ID, COLUMN_ROW_SETTING, VALUE_SETTING, COMMENTS, IS_RUNNABLE, OBJECT_NAME_ID) VALUES " +
                                                                           "('Deleted', " + item.STEPS_ID + ", " + item.RUN_ORDER + ", " + item.KEY_WORD_ID + ", " + testCaseId + ", " + item.OBJECT_ID + ", '" + (!string.IsNullOrEmpty(item.COLUMN_ROW_SETTING) ? item.COLUMN_ROW_SETTING : string.Empty) + "', '" + (!string.IsNullOrEmpty(item.VALUE_SETTING) ? item.VALUE_SETTING : "") + "', '" + (!string.IsNullOrEmpty(item.COMMENTINFO) ? item.COMMENTINFO : "") + "', " + (item.IS_RUNNABLE != null ? item.IS_RUNNABLE : 0) + ", " + (item.OBJECT_NAME_ID != null ? item.OBJECT_NAME_ID : -1) + ") ";
                                command.ExecuteNonQuery();
                            }

                            OracleCommand cmd = new OracleCommand
                            {
                                CommandText = "SELECT * FROM T_TEST_STEPS_TEMP",
                                Connection = connection
                            };
                            OracleDataReader dr = cmd.ExecuteReader();
                            var dataTable = new DataTable();
                            dataTable.Load(dr);
                            DataRow[] addedRow = dataTable.Select("STATUS = 'Added'");
                            if (addedRow.Length > 0)
                            {
                                bool addTestcaseStatus = InsertTestcaseStepsInDatabase(addedRow, testCaseId, connectionString);
                            }

                            DataRow[] updatedRow = dataTable.Select("STATUS = 'Updated'");
                            if (updatedRow.Length > 0)
                            {
                                bool updateTestcaseStatus = UpdateTestcaseStepsInDatabase(updatedRow, testCaseId, connectionString);
                            }
                            DataRow[] deletedRow = dataTable.Select("STATUS = 'Deleted'");
                            if (deletedRow.Length > 0)
                            {
                                bool deleteTestcaseStatus = DeleteTestcaseStepsInDatabase(deletedRow, testCaseId, connectionString);
                            }
                            command.CommandText = "TRUNCATE TABLE T_TEST_STEPS_TEMP";
                            command.ExecuteNonQuery();

                            command.CommandText = "DROP TABLE T_TEST_STEPS_TEMP";
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        connection.Close();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        command.CommandText = "TRUNCATE TABLE T_TEST_STEPS_TEMP";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE T_TEST_STEPS_TEMP";
                        command.ExecuteNonQuery();

                        logger.Error(string.Format("Error occured TestCase in SaveTestcaseSessionInDatabase method | Testcase Id : {0} | UserName: {1}", testCaseId, Username));
                        ELogger.ErrorException(string.Format("Error occured TestCase in SaveTestcaseSessionInDatabase method | Testcase Id : {0} | UserName: {1}", testCaseId, Username), ex);
                        if (ex.InnerException != null)
                            ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveTestcaseSessionInDatabase method | Testcase Id : {0} | UserName: {1}", testCaseId, Username), ex.InnerException);
                        return false;
                    }
                }
                logger.Info(string.Format("SAVE TESTCASE SESSION VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | END : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveTestcaseSessionInDatabase method | Testcase Id : {0} | UserName: {1}", testCaseId, Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveTestcaseSessionInDatabase method | Testcase Id : {0} | UserName: {1}", testCaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured TestCase in SaveTestcaseSessionInDatabase method | Testcase Id : {0} | UserName: {1}", testCaseId, Username), ex.InnerException);
                return false;
            }
        }
        public bool InsertTestcaseStepsInDatabase(DataRow[] addedRow, long testCaseId, string lConnectionStr)
        {
            try
            {
                logger.Info(string.Format("INSERT NEW ADDED TESTCASE STEPS VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | START : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();
                string lcmdquery = "INSERT INTO T_TEST_STEPS (STEPS_ID, RUN_ORDER, KEY_WORD_ID, TEST_CASE_ID, OBJECT_ID, COLUMN_ROW_SETTING, VALUE_SETTING, \"COMMENT\", IS_RUNNABLE, OBJECT_NAME_ID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10)";

                int[] ids = new int[addedRow.Length];
                using (var lcmd = lconnection.CreateCommand())
                {
                    lcmd.CommandText = lcmdquery;
                    lcmd.ArrayBindCount = ids.Length;

                    decimal[] STEPS_ID_param = addedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("STEPS_ID"))).ToArray();
                    decimal[] RUN_ORDER_param = addedRow.AsEnumerable().Select(r => r.Field<decimal>("RUN_ORDER")).ToArray();
                    decimal[] KEY_WORD_ID_param = addedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("KEY_WORD_ID"))).ToArray();
                    decimal[] TEST_CASE_ID_param = addedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("TEST_CASE_ID"))).ToArray();
                    decimal[] OBJECT_ID_param = addedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long?>("OBJECT_ID"))).ToArray();
                    decimal?[] OBJECT_ID_NEW_Param = OBJECT_ID_param.Select(x => x > 0 ? x : (decimal?)null).ToArray();
                    string[] COLUMN_ROW_SETTING_param = addedRow.AsEnumerable().Select(r => r.Field<string>("COLUMN_ROW_SETTING")).ToArray();
                    string[] VALUE_SETTING_param = addedRow.AsEnumerable().Select(r => r.Field<string>("VALUE_SETTING")).ToArray();
                    string[] COMMENT_param = addedRow.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
                    long[] IS_RUNNABLE_param = addedRow.AsEnumerable().Select(r => Convert.ToInt64(r.Field<decimal>("IS_RUNNABLE"))).ToArray();
                    long[] OBJECT_NAME_ID_param = addedRow.AsEnumerable().Select(r => Convert.ToInt64(r.Field<decimal?>("OBJECT_NAME_ID"))).ToArray();

                    OracleParameter STEPS_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        Value = STEPS_ID_param
                    };
                    OracleParameter RUN_ORDER_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = RUN_ORDER_param
                    };
                    OracleParameter KEY_WORD_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = KEY_WORD_ID_param
                    };
                    OracleParameter TEST_CASE_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = TEST_CASE_ID_param
                    };
                    OracleParameter OBJECT_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = OBJECT_ID_NEW_Param
                    };
                    OracleParameter COLUMN_ROW_SETTING_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        IsNullable = true,
                        Value = COLUMN_ROW_SETTING_param
                    };
                    OracleParameter VALUE_SETTING_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        IsNullable = true,
                        Value = VALUE_SETTING_param
                    };
                    OracleParameter COMMENT_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        IsNullable = true,
                        Value = COMMENT_param
                    };
                    OracleParameter IS_RUNNABLE_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Long,
                        IsNullable = true,
                        Value = IS_RUNNABLE_param
                    };
                    OracleParameter OBJECT_NAME_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Long,
                        IsNullable = true,
                        Value = OBJECT_NAME_ID_param
                    };
                    lcmd.Parameters.Add(STEPS_ID_oparam);
                    lcmd.Parameters.Add(RUN_ORDER_oparam);
                    lcmd.Parameters.Add(KEY_WORD_ID_oparam);
                    lcmd.Parameters.Add(TEST_CASE_ID_oparam);
                    lcmd.Parameters.Add(OBJECT_ID_oparam);
                    lcmd.Parameters.Add(COLUMN_ROW_SETTING_oparam);
                    lcmd.Parameters.Add(VALUE_SETTING_oparam);
                    lcmd.Parameters.Add(COMMENT_oparam);
                    lcmd.Parameters.Add(IS_RUNNABLE_oparam);
                    lcmd.Parameters.Add(OBJECT_NAME_ID_oparam);

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

                    logger.Info(string.Format("INSERT NEW ADDED TESTCASE STEPS VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | END : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool UpdateTestcaseStepsInDatabase(DataRow[] updatedRow, long testCaseId, string lConnectionStr)
        {
            try
            {
                logger.Info(string.Format("UPDATE TESTCASE STEPS VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | START : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));

                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();
                string lcmdquery = "UPDATE T_TEST_STEPS SET RUN_ORDER = :1, KEY_WORD_ID = :2, TEST_CASE_ID = :3, OBJECT_ID = :4, COLUMN_ROW_SETTING = :5, VALUE_SETTING = :6, \"COMMENT\" = :7, IS_RUNNABLE = :8, OBJECT_NAME_ID = :9 WHERE STEPS_ID = :10";

                int[] ids = new int[updatedRow.Length];
                using (var lcmd = lconnection.CreateCommand())
                {
                    lcmd.CommandText = lcmdquery;
                    lcmd.ArrayBindCount = ids.Length;

                    decimal[] STEPS_ID_param = updatedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("STEPS_ID"))).ToArray();
                    decimal[] RUN_ORDER_param = updatedRow.AsEnumerable().Select(r => r.Field<decimal>("RUN_ORDER")).ToArray();
                    decimal[] KEY_WORD_ID_param = updatedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("KEY_WORD_ID"))).ToArray();
                    decimal[] TEST_CASE_ID_param = updatedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("TEST_CASE_ID"))).ToArray();
                    decimal[] OBJECT_ID_param = updatedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("OBJECT_ID"))).ToArray();
                    decimal?[] OBJECT_ID_NEW_Param = OBJECT_ID_param.Select(x => x > 0 ? x : (decimal?)null).ToArray();
                    string[] COLUMN_ROW_SETTING_param = updatedRow.AsEnumerable().Select(r => r.Field<string>("COLUMN_ROW_SETTING")).ToArray();
                    string[] VALUE_SETTING_param = updatedRow.AsEnumerable().Select(r => r.Field<string>("VALUE_SETTING")).ToArray();
                    string[] COMMENT_param = updatedRow.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
                    long[] IS_RUNNABLE_param = updatedRow.AsEnumerable().Select(r => Convert.ToInt64(r.Field<decimal>("IS_RUNNABLE"))).ToArray();
                    long[] OBJECT_NAME_ID_param = updatedRow.AsEnumerable().Select(r => Convert.ToInt64(r.Field<decimal?>("OBJECT_NAME_ID"))).ToArray();


                    OracleParameter RUN_ORDER_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = RUN_ORDER_param
                    };
                    OracleParameter KEY_WORD_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = KEY_WORD_ID_param
                    };
                    OracleParameter TEST_CASE_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = TEST_CASE_ID_param
                    };
                    OracleParameter OBJECT_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        IsNullable = true,
                        Value = OBJECT_ID_NEW_Param
                    };
                    OracleParameter COLUMN_ROW_SETTING_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        IsNullable = true,
                        Value = COLUMN_ROW_SETTING_param
                    };
                    OracleParameter VALUE_SETTING_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        IsNullable = true,
                        Value = VALUE_SETTING_param
                    };
                    OracleParameter COMMENT_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        IsNullable = true,
                        Value = COMMENT_param
                    };
                    OracleParameter IS_RUNNABLE_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Long,
                        IsNullable = true,
                        Value = IS_RUNNABLE_param
                    };
                    OracleParameter OBJECT_NAME_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Long,
                        IsNullable = true,
                        Value = OBJECT_NAME_ID_param
                    };
                    OracleParameter STEPS_ID_oparam = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        Value = STEPS_ID_param
                    };

                    lcmd.Parameters.Add(RUN_ORDER_oparam);
                    lcmd.Parameters.Add(KEY_WORD_ID_oparam);
                    lcmd.Parameters.Add(TEST_CASE_ID_oparam);
                    lcmd.Parameters.Add(OBJECT_ID_oparam);
                    lcmd.Parameters.Add(COLUMN_ROW_SETTING_oparam);
                    lcmd.Parameters.Add(VALUE_SETTING_oparam);
                    lcmd.Parameters.Add(COMMENT_oparam);
                    lcmd.Parameters.Add(IS_RUNNABLE_oparam);
                    lcmd.Parameters.Add(OBJECT_NAME_ID_oparam);
                    lcmd.Parameters.Add(STEPS_ID_oparam);

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
                    logger.Info(string.Format("UPDATE TESTCASE STEPS VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | END : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeleteTestcaseStepsInDatabase(DataRow[] deletedRow, long testCaseId, string lConnectionStr)
        {
            try
            {
                logger.Info(string.Format("DELTE TESTCASE STEPS VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | START : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));

                string deleteReportStepQuery = "DELETE FROM T_TEST_REPORT_STEPS WHERE STEPS_ID IN ({STEP_ID})";
                string deleteDatasettingsQuery = "DELETE FROM TEST_DATA_SETTING WHERE STEPS_ID IN ({STEP_ID})";
                string deleteStepQuery = "DELETE FROM T_TEST_STEPS WHERE STEPS_ID IN ({STEP_ID})";

                using (OracleConnection connection = new OracleConnection(lConnectionStr))
                {
                    connection.Open();
                    OracleCommand command = connection.CreateCommand();
                    OracleTransaction transaction;
                    transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    command.Transaction = transaction;

                    try
                    {
                        decimal[] STEPS_ID_param = deletedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("STEPS_ID"))).ToArray();
                        deleteReportStepQuery = deleteReportStepQuery.Replace("{STEP_ID}", string.Join(",", STEPS_ID_param));
                        deleteDatasettingsQuery = deleteDatasettingsQuery.Replace("{STEP_ID}", string.Join(",", STEPS_ID_param));
                        deleteStepQuery = deleteStepQuery.Replace("{STEP_ID}", string.Join(",", STEPS_ID_param));

                        command.CommandText = deleteReportStepQuery;
                        command.ExecuteNonQuery();

                        command.CommandText = deleteDatasettingsQuery;
                        command.ExecuteNonQuery();

                        command.CommandText = deleteStepQuery;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        connection.Close();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                //int[] ids = new int[deletedRow.Length];
                //using (var lcmd = lconnection.CreateCommand())
                //{
                //    decimal[] STEPS_ID_param = deletedRow.AsEnumerable().Select(r => Convert.ToDecimal(r.Field<long>("STEPS_ID"))).ToArray();
                //    deleteReportStepQuery = deleteReportStepQuery.Replace("{STEP_ID}", string.Join(",", STEPS_ID_param));
                //    deleteDatasettingsQuery = deleteDatasettingsQuery.Replace("{STEP_ID}", string.Join(",", STEPS_ID_param));
                //    deleteStepQuery = deleteStepQuery.Replace("{STEP_ID}", string.Join(",", STEPS_ID_param));

                //    lcmd.CommandText = string.Format("{0} {1} {2}", deleteReportStepQuery, deleteDatasettingsQuery, deleteStepQuery);
                //    try
                //    {
                //        lcmd.ExecuteNonQuery();
                //    }
                //    catch (Exception lex)
                //    {
                //        ltransaction.Rollback();
                //        throw new Exception(lex.Message);
                //    }
                //    ltransaction.Commit();
                //    lconnection.Close();
                //    logger.Info(string.Format("DELETE TESTCASE STEPS VALUES INTO DATABASE | TESTCASEID : {0} | USERNAME: {1} | END : {2}", testCaseId, Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")));
                //}
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

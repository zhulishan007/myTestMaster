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
                logger.Info(string.Format("GetTSTCDSId start | TestCaseName: {0} | UserName: {1}", TestCasename, Username));
                var llist = new List<string>();
                var suiteid = entity.T_TEST_SUITE.FirstOrDefault(x => x.TEST_SUITE_NAME == TestSuitname).TEST_SUITE_ID;
                var caseid = entity.T_TEST_CASE_SUMMARY.FirstOrDefault(x => x.TEST_CASE_NAME == TestCasename).TEST_CASE_ID;
                var datasetid = entity.T_TEST_DATA_SUMMARY.FirstOrDefault(x => x.ALIAS_NAME == Datasetname).DATA_SUMMARY_ID;

                logger.Info(string.Format("GetTSTCDSId end | TestCaseName: {0} | UserName: {1}", TestCasename, Username));
                return suiteid + "," + caseid + "," + datasetid;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTSTCDSId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTSTCDSId method | UserName: {0}", Username), ex);
                throw;
            }
        }
      
        public bool CheckTestCaseTestSuiteRel(long testcaseId, long testsuiteid)
        {
            try
            {
                logger.Info(string.Format("Check TestCase TestSuite Rel start | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseId, testsuiteid,Username));
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
                logger.Error(string.Format("Error occured TestCase in CheckTestCaseTestSuiteRel method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckTestCaseTestSuiteRel method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string ChangeTestCaseName(string lTestCaseName, long lTestCaseId, string TestCaseDes)
        {
            try
            {
                logger.Info(string.Format("Change TestCaseName start | TestCaseName: {0} | UserName: {1}", lTestCaseName, Username));
                if (!string.IsNullOrEmpty(lTestCaseName))
                    lTestCaseName = lTestCaseName.Trim();

                var lTestCase = entity.T_TEST_CASE_SUMMARY.Find(lTestCaseId);
                lTestCase.TEST_CASE_NAME = lTestCaseName;
                lTestCase.TEST_STEP_DESCRIPTION = TestCaseDes;
                entity.SaveChanges();
                logger.Info(string.Format("Change TestCaseName end | TestCaseName: {0} | UserName: {1}", lTestCaseName, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ChangeTestCaseName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ChangeTestCaseName method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool CheckDuplicateTestCaseName(string lTestCaseName, long? lTestCaseId)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate TestCaseName start | TestCaseName: {0} | UserName: {1}", lTestCaseName, Username));
                var lresult = false;
                if (lTestCaseId != null)
                {
                    lresult = entity.T_TEST_CASE_SUMMARY.Any(x => x.TEST_CASE_ID != lTestCaseId && x.TEST_CASE_NAME.ToLower().Trim() == lTestCaseName.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_CASE_SUMMARY.Any(x => x.TEST_CASE_NAME.ToLower().Trim() == lTestCaseName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate TestCaseName end | TestCaseName: {0} | UserName: {1}", lTestCaseName, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool CheckDuplicateDataset(string lDatasetname, long? ldatasetid)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Dataset start | Datasetname: {0} | UserName: {1}", lDatasetname, Username));
                var lresult = false;
                if (ldatasetid != null)
                {
                    lresult = entity.T_TEST_DATA_SUMMARY.Any(x => x.DATA_SUMMARY_ID != ldatasetid && x.ALIAS_NAME.ToLower().Trim() == lDatasetname.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_TEST_DATA_SUMMARY.Any(x => x.ALIAS_NAME.ToLower().Trim() == lDatasetname.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Dataset end | Datasetname: {0} | UserName: {1}", lDatasetname, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Dataset in CheckDuplicateDataset method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Dataset in CheckDuplicateDataset method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string InsertFeedProcess()
        {
            try
            {
                logger.Info(string.Format("Insert FeedProcess start | UserName: {0}", Username));
                var feedprocessID = Helper.NextTestSuiteId("TBLFEEDPROCESS_SEQ");

                var ltbl = new TBLFEEDPROCESS();
                ltbl.FEEDPROCESSID = feedprocessID;
                ltbl.FEEDPROCESSSTATUS = "Insert-WebApp";
                ltbl.FEEDRUNON = System.DateTime.Now;
                ltbl.CREATEDBY = "WebApp";
                ltbl.CREATEDON = System.DateTime.Now;

                entity.TBLFEEDPROCESSes.Add(ltbl);

                entity.SaveChanges();

                var feedprocessDetailsID = Helper.NextTestSuiteId("TBLFEEDPROCESSDETAILS_SEQ");

                entity.TBLFEEDPROCESSDETAILS.Add(new TBLFEEDPROCESSDETAIL { CREATEDBY = "WebApp", FEEDPROCESSDETAILID = feedprocessDetailsID, CREATEDON = System.DateTime.Now, FEEDPROCESSID = feedprocessID, FEEDPROCESSSTATUS = "INPROGRESS", FILENAME = "WebAppImport", FILETYPE = "TESTCASE" });
                entity.SaveChanges();
                logger.Info(string.Format("Insert FeedProcess end | UserName: {0}", Username));
                return Convert.ToString(feedprocessID) + "~" + Convert.ToString(feedprocessDetailsID);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in InsertFeedProcess method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in InsertFeedProcess method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CheckTestCaseExistsInStoryboard method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckTestCaseExistsInStoryboard method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in GetTestcaseNameById method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestcaseNameById method | UserName: {0}", Username), ex);
                throw;
            }

        }
        public string MoveTestCase(long projectId, long testcaseid, long testsuiteid)
        {
            try
            {
                logger.Info(string.Format("Move TestCase start | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseid, testsuiteid, Username));
                var result = entity.REL_TEST_CASE_TEST_SUITE.FirstOrDefault(x => x.TEST_CASE_ID == testcaseid);
                if (result != null)
                {
                    result.TEST_CASE_ID = testcaseid;
                    result.TEST_SUITE_ID = testsuiteid;
                    entity.SaveChanges();
                    logger.Info(string.Format("Move TestCase end | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseid, testsuiteid, Username));
                    return "success";
                }
                logger.Info(string.Format("Move TestCase end | testcaseId: {0} | testsuiteid: {1} | UserName: {2}", testcaseid, testsuiteid, Username));
                return "error";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in MoveTestCase method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in MoveTestCase method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string ValidateSave(int feedprocessid)
        {
            try
            {
                logger.Info(string.Format("ValidateSave start | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                ObjectParameter op = new ObjectParameter("RESULT", "");
                entity.USP_MAPPING_VALIDATION(feedprocessid, op);
                entity.SaveChanges();
                logger.Info(string.Format("ValidateSave end | feedprocessid: {0} | UserName: {1}", feedprocessid, Username));
                return "validated";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in ValidateSave method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ValidateSave method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in SaveData method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveData method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in Validate method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in Validate method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string DeleteStep(int stepID)
        {
            try
            {
                logger.Info(string.Format("Delete Step start | stepID: {0} | UserName: {1}", stepID, Username));
                var testReport = entity.T_TEST_REPORT_STEPS.Where(x => x.STEPS_ID == stepID).ToList();

                foreach (var v in testReport)
                {
                    entity.T_TEST_REPORT_STEPS.Remove(v);
                }
                entity.SaveChanges();

                var stepsSettings = entity.TEST_DATA_SETTING.Where(x => x.STEPS_ID == stepID).ToList();

                foreach (var v in stepsSettings)
                {
                    entity.TEST_DATA_SETTING.Remove(v);
                }
                entity.SaveChanges();

                var tStep = entity.T_TEST_STEPS.Where(x => x.STEPS_ID == stepID).ToList();
                foreach (var t in tStep)
                {
                    entity.T_TEST_STEPS.Remove(t);
                }
                entity.SaveChanges();
                logger.Info(string.Format("Delete Step end | stepID: {0} | UserName: {1}", stepID, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in DeleteStep method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteStep method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string AddStep(long TestCaseId, long TestSuiteId, string lKeywordName, string lComment, string lObjectName, string lParameter, long RunOrder = 0)
        {
            try
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
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in AddStep method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in AddStep method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void UpdateSteps(int testcaseId)
        {
            try
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
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateSteps method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateSteps method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public void UpdateStepID(int stepsId, int RUN_ORDER)
        {
            try
            {
                logger.Info(string.Format("Update StepID start | stepsId: {0} | UserName: {1}", stepsId, Username));
                var step = entity.T_TEST_STEPS.Find(stepsId);
                if (step != null)
                {
                    step.RUN_ORDER = RUN_ORDER;
                }
                entity.SaveChanges();
                logger.Info(string.Format("Update StepID end | stepsId: {0} | UserName: {1}", stepsId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateStepID method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateStepID method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in GetTestCaseNameById method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseNameById method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string UpdateRunOrder(int stepId, int newRun_Order)
        {
            try
            {
                logger.Info(string.Format("UpdateRunOrder start | stepId: {0} | newRun_Order: {1} | UserName: {2}", stepId, newRun_Order, Username));
                var step = entity.T_TEST_STEPS.Where(x => x.STEPS_ID == stepId).FirstOrDefault();
                if (step != null)
                {
                    step.RUN_ORDER = newRun_Order;
                    entity.SaveChanges();
                }
                logger.Info(string.Format("UpdateRunOrder end | stepId: {0} | newRun_Order: {1} | UserName: {2}", stepId, newRun_Order, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateRunOrder method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateRunOrder method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CheckDatasetInStoryboard method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDatasetInStoryboard method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string DeleteRelTestCaseDataSummary(long testCaseId, long dataSetId)
        {
            try
            {
                logger.Info(string.Format("Delete RelTestCaseDataSummary start | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
                var dataseting = (from setting in entity.TEST_DATA_SETTING
                                  where dataSetId == setting.DATA_SUMMARY_ID
                                  select setting);

                foreach (var item in dataseting)
                {
                    entity.TEST_DATA_SETTING.Remove(item);
                    entity.SaveChanges();
                }

                var relTcDataSummary = (from rt in entity.REL_TC_DATA_SUMMARY
                                        where rt.TEST_CASE_ID == testCaseId && rt.DATA_SUMMARY_ID == dataSetId
                                        select rt).FirstOrDefault();
                if (relTcDataSummary != null)
                    entity.REL_TC_DATA_SUMMARY.Remove(relTcDataSummary);
                entity.SaveChanges();



                var sharedObject = (from rt in entity.T_SHARED_OBJECT_POOL
                                    where rt.DATA_SUMMARY_ID == dataSetId
                                    select rt);
                foreach (var item in sharedObject)
                {
                    entity.T_SHARED_OBJECT_POOL.Remove(item);
                    entity.SaveChanges();
                }


                var datasummary = (from rt in entity.T_TEST_DATA_SUMMARY
                                   where rt.DATA_SUMMARY_ID == dataSetId
                                   select rt).FirstOrDefault();
                if (datasummary != null)
                    entity.T_TEST_DATA_SUMMARY.Remove(datasummary);
                entity.SaveChanges();



                entity.SaveChanges();
                logger.Info(string.Format("Delete RelTestCaseDataSummary end | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in DeleteRelTestCaseDataSummary method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteRelTestCaseDataSummary method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string AddTestDataSet(long? testCaseId, long? datasetid, string DataSetName, string DataSetDesc,DataSetTagModel tagmodel)
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
                    entity.SaveChanges();
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
                    var result = entity.T_TEST_DATASETTAG.Where(x => x.DATASETID == datasetid).SingleOrDefault();
                    var seqcheck = entity.T_TEST_DATASETTAG.Where(x => x.FOLDERID == tagmodel.Folderid && x.SEQUENCE == tagmodel.Sequence && x.DATASETID != datasetid).ToList();
                    if (seqcheck.Count > 0)
                    {
                        return datasetid + ",sequence error";
                    }
                    if (result != null)
                    {

                        result.FOLDERID = tagmodel.Folderid;
                        result.SETID = tagmodel.Setid;
                        result.GROUPID = tagmodel.Groupid;
                        result.STEPDESC = tagmodel.StepDesc;
                        result.SEQUENCE = tagmodel.Sequence;
                        result.EXPECTEDRESULTS = tagmodel.Expectedresults;
                        result.DIARY = tagmodel.Diary;
                        entity.SaveChanges();
                    }
                    else
                    {
                        if (tagmodel.Folderid != 0 || tagmodel.Sequence != null || tagmodel.Groupid != 0 || tagmodel.Setid != 0 || tagmodel.StepDesc != null || tagmodel.Expectedresults != null || tagmodel.Diary != null)
                        {
                            var dtmodel = new T_TEST_DATASETTAG();
                            dtmodel.T_TEST_DATASETTAG_ID = Helper.NextTestSuiteId("SEQ_T_TEST_DATASETTAG");
                            dtmodel.DATASETID = datasetid;
                            dtmodel.FOLDERID = tagmodel.Folderid;
                            dtmodel.SETID = tagmodel.Setid;
                            dtmodel.GROUPID = tagmodel.Groupid;
                            dtmodel.STEPDESC = tagmodel.StepDesc;
                            dtmodel.SEQUENCE = tagmodel.Sequence;
                            dtmodel.EXPECTEDRESULTS = tagmodel.Expectedresults;
                            dtmodel.DIARY = tagmodel.Diary;
                            entity.T_TEST_DATASETTAG.Add(dtmodel);
                            entity.SaveChanges();
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
                throw;
            }
        }

        public bool DeleteTestCase(long TestCaseId)
        {
            try
            {
                logger.Info(string.Format("Delete TestCase start | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                var flag = false;
                entity.DeleteTestCase(TestCaseId);
                flag = true;
                logger.Info(string.Format("Delete TestCase end | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in DeleteTestCase method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in DeleteTestCase method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in ListAllTestCase method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ListAllTestCase method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool AddEditTestCase(TestCaseModel lEntity, string LoginName)
        {
            try
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
                    entity.SaveChanges();
                    logger.Info(string.Format("Add TestCase end | TestCaseName: {0} | UserName: {1}", lEntity.TestCaseName, Username));
                    #region insert for default Dataset

                    var tblDataSummary = new T_TEST_DATA_SUMMARY();
                    tblDataSummary.ALIAS_NAME = lEntity.TestCaseName;
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
                        entity.SaveChanges();

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
                            entity.SaveChanges();
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
                                    entity.SaveChanges();
                                }
                            }
                        }
                    }


                    flag = true;
                }
                return flag;
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in AddEditTestCase method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in AddEditTestCase method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string UpdateDataSetName(string datasetName, long dataSetId)
        {
            try
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
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateDataSetName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateDataSetName method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string UpdateDataSetDescription(string dataSetDescription, long dataSetId)
        {
            try
            {
                logger.Info(string.Format("UpdateDataSetDescription start | dataSetId: {0} | UserName: {1}", dataSetId, Username));
                var ds = entity.T_TEST_DATA_SUMMARY.Where(x => x.DATA_SUMMARY_ID == dataSetId).SingleOrDefault();
                if (ds != null)
                {
                    ds.DESCRIPTION_INFO = dataSetDescription;
                }
                entity.SaveChanges();
                logger.Info(string.Format("UpdateDataSetDescription end | dataSetId: {0} | UserName: {1}", dataSetId, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateDataSetDescription method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateDataSetDescription method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public IList<TestCaseResult> GetTestCaseDetail(long TestCaseId, string schema, string lstrConn, long UserId)
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
                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("TESTSUITEID", OracleDbType.Long);
                ladd_refer_image[0].Value = testsuiteid;

                ladd_refer_image[1] = new OracleParameter("TESTCASEID", OracleDbType.Long);
                ladd_refer_image[1].Value = TestCaseId;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;



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
                        if (item.DATASETVALUE.Contains("&amp;") || item.DATASETVALUE.Contains("~") || item.DATASETVALUE.Contains(@"\") || item.DATASETVALUE.Contains("&quot;"))
                        {
                            item.DATASETVALUE = item.DATASETVALUE.Replace("&amp;", "&");
                            item.DATASETVALUE = item.DATASETVALUE.Replace("~", "##");
                            item.DATASETVALUE = item.DATASETVALUE.Replace("&quot;", "\"");
                        }
                    }
                    if (item.parameter != null)
                    {
                        if (item.parameter.Contains("\\n") || item.parameter.Contains("\""))
                        {
                            item.parameter = item.parameter.Replace("\\n", "\\n");
                            item.parameter = item.parameter.Replace("\"", "\"");
                        }
                    }
                    if (item.COMMENT != null)
                    {
                        if (item.COMMENT.Contains("\\n"))
                        {
                            item.COMMENT = item.parameter.Replace("\\n", "\n");
                        }
                    }
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
                logger.Error(string.Format("Error occured TestCase in GetTestCaseDetail method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseDetail method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CheckObjectKeywordValidation method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckObjectKeywordValidation method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCasenameSaveAs method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCasenameSaveAs method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateDatasetName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateDatasetName method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string SaveAsTestcase(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string schema, string constring, string LoginName)
        {
            logger.Info(string.Format("SaveAs Testcase start | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
            try
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
                entity.SaveChanges();

                var lTSTC = new REL_TEST_CASE_TEST_SUITE();
                lTSTC.TEST_CASE_ID = tbl.TEST_CASE_ID;
                lTSTC.TEST_SUITE_ID = testsuiteid;
                lTSTC.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_TEST_CASE_TEST_SUITE_SEQ");
                entity.REL_TEST_CASE_TEST_SUITE.Add(lTSTC);
                entity.SaveChanges();

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
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestcase method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestcase method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string SaveAsTestCaseOneCopiedDataSet_Temp(string testcasename, long oldtestcaseid, string testcasedesc, string datasetName, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            try
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
                entity.SaveChanges();


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
                        tbldatasetting.CREATE_TIME = DateTime.Now; ;
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

                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet_Temp method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet_Temp method | UserName: {0}", Username), ex);
                throw;
            }
           
        }

        public string SaveAsTestCaseOneCopiedDataSet(string testcasename, long oldtestcaseid, string testcasedesc, string datasetName, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            logger.Info(string.Format("SaveAs TestCas eOne Copied DataSet start | Oldtestcaseid: {0} | Testcasename: {1} | DatasetName: {2} | UserName: {3}", oldtestcaseid, testcasename, datasetName, Username));
            try
            {
                ObjectParameter RESULT = new ObjectParameter("RESULT", typeof(string));

                var lresult = entity.SP_SavaAs_TestCase_One_Dataset(oldtestcaseid, testcasename, testcasedesc, testsuiteid, projectid, datasetName, suffix, LoginName, RESULT);

                logger.Info(string.Format("SaveAs TestCas eOne Copied DataSet end | Oldtestcaseid: {0} | Testcasename: {1} | DatasetName: {2} | UserName: {3}", oldtestcaseid, testcasename, datasetName, Username));
                return RESULT.Value.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseOneCopiedDataSet method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string SaveAsTestCaseAllCopiedDataSet_Temp(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            try
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
                entity.SaveChanges();

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

                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet_Temp method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet_Temp method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string SaveAsTestCaseAllCopiedDataSet(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string suffix, string schema, string constring, string LoginName)
        {
            try
            {
                logger.Info(string.Format("SaveAs TestCase All Copied DataSet start | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
                ObjectParameter RESULT = new ObjectParameter("RESULT", typeof(string));
                var lresult = entity.SP_SavaAs_TestCase_AllDataset(oldtestcaseid, testcasename, testcasedesc, testsuiteid, projectid, suffix, LoginName, RESULT);

                logger.Info(string.Format("SaveAs TestCase All Copied DataSet end | Oldtestcaseid: {0} | Testcasename: {1} | UserName: {2}", oldtestcaseid, testcasename, Username));
                return RESULT.Value.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in SaveAsTestCaseAllCopiedDataSet method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CreateAllDataset method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CreateAllDataset method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in CreateAllDatasetMapping method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CreateAllDatasetMapping method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in convertsteps method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in convertsteps method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public TestCase_Version_Model GetTestCaseVersion(long TestCaseId, long UserId)
        {
            try
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
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in GetTestCaseVersion method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetTestCaseVersion method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void SaveTestCaseVersion(long TestCaseId, long UserId)
        {
            try
            {
                logger.Info(string.Format("Save TestCase Version start | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                var ltbl = entity.T_TESTCASE_VERSION.Where(x => x.TESTCASEID == TestCaseId).FirstOrDefault();
                if (ltbl != null)
                {
                    ltbl.VERSIONID = ltbl.VERSIONID + 1;
                    ltbl.CREATORID = 0;
                    ltbl.CREATETIME = DateTime.Now;
                    ltbl.ISAVAILABLE = 0;
                    entity.SaveChanges();
                    logger.Info(string.Format("Save TestCase Version end | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in CheckDuplicateTestCaseName method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool MatchTestCaseVersion(long TestCaseId, long VersionId)
        {
            try
            {
                logger.Info(string.Format("Match TestCase Version start | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                var lresult = entity.T_TESTCASE_VERSION.Any(x => x.TESTCASEID == TestCaseId && x.VERSIONID == VersionId);
                logger.Info(string.Format("Match TestCase Version end | TestCaseId: {0} | UserName: {1}", TestCaseId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in MatchTestCaseVersion method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in MatchTestCaseVersion method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void UpdateIsAvailable(string TestCaseIds)
        {
            try
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
                            entity.SaveChanges();
                        }
                    }
                }
                logger.Info(string.Format("Update Is Available end | TestCaseId: {0} | UserName: {1}", TestCaseIds, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in UpdateIsAvailable method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in UpdateIsAvailable method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void UpdateIsAvailableReload(long UserId)
        {
            try
            {
                logger.Info(string.Format("Update UserId start | UserId: {0}", UserId));
                var tblList = entity.T_TESTCASE_VERSION.Where(x => x.CREATORID == UserId).ToList();
                foreach (var item in tblList)
                {
                    item.ISAVAILABLE = 0;
                    item.CREATORID = 0;
                    entity.SaveChanges();
                }
                logger.Info(string.Format("Update UserId end | UserId: {0}", UserId));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Logout in UpdateIsAvailableReload method | UserId: {0}", UserId));
                ELogger.ErrorException(string.Format("Error occured Logout in UpdateIsAvailableReload method | UserId: {0}", UserId), ex);
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
                entity.SaveChanges();
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
                logger.Error(string.Format("Error occured dataset in CopyDataSet method | UserId: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured dataset in CopyDataSet method | UserId: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in ExecuteCheckValidationTestCase method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in ExecuteCheckValidationTestCase method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured in TestCase InsertStgTestcaseValidationTable method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in TestCase InsertStgTestcaseValidationTable method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase in GetDatasetNamebyTestcaseId method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in GetDatasetNamebyTestcaseId method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public void InsertStgTestCaseSave(string lConnectionStr, string lschema, DataTable dt, string testCaseId, long valFeedD)
        {
            try
            {
                logger.Info(string.Format("InsertStgTestCaseSave start | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
                OracleTransaction ltransaction;

                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                lconnection.Open();
                ltransaction = lconnection.BeginTransaction();

                string lcmdquery = "insert into TBLSTGTESTCASESAVE ( STEPSID,KEYWORD,OBJECT,PARAMETER,LCOMMENTS,ROWNUMBER,DATASETNAME,DATASETID,DATASETVALUE,DATA_SETTING_ID,SKIP,FEEDPROCESSDETAILID,TYPE,WHICHTABLE,TESTCASEID,TESTSUITEID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14,:15,:16)";
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

                    ObjectParameter op = new ObjectParameter("RESULT", "");
                    entity.SP_SaveTestcase(valFeedD, int.Parse(testCaseId), op);
                    entity.SaveChanges();
                }
                logger.Info(string.Format("InsertStgTestCaseSave end | TestCaseId: {0} | UserName: {1}", testCaseId, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase in InsertStgTestCaseSave method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase in InsertStgTestCaseSave method | UserName: {0}", Username), ex);
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
                logger.Info(string.Format("ListOfGroup end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase Repository in ListOfGroup method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in ListOfGroup method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool AddEditGroup(DataTagCommonViewModel lEntity)
        {
            try
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
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Group page in AddEditGroup method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Group page in AddEditGroup method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured Group page in CheckDuplicateGroupNameExist method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Group page in CheckDuplicateGroupNameExist method | UserName: {0}", Username), ex);
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
                logger.Info(string.Format("ListOfSet end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase Repository in ListOfSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in ListOfSet method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool AddEditSet(DataTagCommonViewModel lEntity)
        {
            try
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
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Set page in AddEditSet method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Set page in AddEditSet method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured Set page in CheckDuplicateSetNameExist method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Set page in CheckDuplicateSetNameExist method | UserName: {0}", Username), ex);
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
                logger.Info(string.Format("ListOfFolder end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase Repository in ListOfFolder method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in ListOfFolder method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool AddEditFolder(DataTagCommonViewModel lEntity)
        {
            try
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
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Folder page in AddEditFolder method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Folder page in AddEditFolder method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured Folder page in CheckDuplicateFolderNameExist method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Folder page in CheckDuplicateFolderNameExist method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase Repository in GetFolders method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in GetFolders method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase Repository in GetSets method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in GetSets method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase Repository in GetGroups method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in GetGroups method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public List<DataSetTagModel> GetTagDetails(long datasetid)
        {
            try
            {
                logger.Info(string.Format("GetTagDetails start | datasetid: {0} | Username: {1}", datasetid, Username));
                var tags = (from k in entity.T_TEST_DATASETTAG
                            join k1 in entity.T_TEST_GROUP on k.GROUPID equals k1.GROUPID into grp
                            from k2 in grp.DefaultIfEmpty()
                            join k3 in entity.T_TEST_FOLDER on k.FOLDERID equals k3.FOLDERID into fold
                            from k4 in fold.DefaultIfEmpty()
                            join k5 in entity.T_TEST_SET on k.SETID equals k5.SETID into set
                            from k6 in set.DefaultIfEmpty()

                            where k.DATASETID == datasetid
                            select new DataSetTagModel
                            {
                                Group = k2.GROUPNAME == null ? "" : k2.GROUPNAME,
                                Set = k6.SETNAME == null ? "" : k6.SETNAME,
                                Folder = k4.FOLDERNAME == null ? "" : k4.FOLDERNAME,
                                Sequence = k.SEQUENCE == null ? 0 : k.SEQUENCE,
                                Expectedresults = k.EXPECTEDRESULTS == null ? "" : k.EXPECTEDRESULTS,
                                Diary = k.DIARY == null ? "" : k.DIARY,
                                Datasetid = k.DATASETID == null ? 0 : k.DATASETID,
                                Tagid = k.T_TEST_DATASETTAG_ID,
                                StepDesc = k.STEPDESC == null ? "" : k.STEPDESC,

                            }

                     ).ToList();
                logger.Info(string.Format("GetTagDetails end | datasetid: {0} | Username: {1}", datasetid, Username));
                return tags;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase Repository in GetTagDetails method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in GetTagDetails method | UserName: {0}", Username), ex);
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
                logger.Error(string.Format("Error occured TestCase Repository in GetDataSetName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in GetDataSetName method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string DeleteTagProperties(long datasetid)
        {
            try
            {
                logger.Info(string.Format("DeleteTagProperties start | datasetid: {0} | Username: {1}", datasetid, Username));
                var result = entity.T_TEST_DATASETTAG.Where(x => x.DATASETID == datasetid).FirstOrDefault();
                if (result != null)
                {
                    entity.T_TEST_DATASETTAG.Remove(result);
                    entity.SaveChanges();
                }
                logger.Info(string.Format("DeleteTagProperties end | datasetid: {0} | Username: {1}", datasetid, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase Repository in DeleteTagProperties method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in DeleteTagProperties method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool CheckFolderSequenceMapping(long FolderId, long SequenceId,long datasetid)
        {
            try
            {
                logger.Info(string.Format("CheckFolderSequenceMapping start | datasetid: {0} | FolderId: {1} | SequenceId: {2} | Username: {2}", datasetid, FolderId, SequenceId, Username));
                bool result = entity.T_TEST_DATASETTAG.Any(x => x.FOLDERID == FolderId && x.SEQUENCE == SequenceId && x.DATASETID != datasetid);
                logger.Info(string.Format("CheckFolderSequenceMapping end | datasetid: {0} | FolderId: {1} | SequenceId: {2} | Username: {2}", datasetid, FolderId, SequenceId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestCase Repository in CheckFolderSequenceMapping method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestCase Repository in CheckFolderSequenceMapping method | UserName: {0}", Username), ex);
                throw;
            }
        }
    }
}

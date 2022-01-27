using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using MarsSerializationHelper.ViewModel;
using MARSUtility;
using Newtonsoft.Json;
using NLog;
using Oracle.ManagedDataAccess.Client;
using static MarsSerializationHelper.JsonSerialization.SerializationFile;
using EmailHelper = MARS_Web.Helper.EmailHelper;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class TestCaseController : Controller
    {
        MARSUtility.CommonHelper objcommon = new MARSUtility.CommonHelper();
        public TestCaseController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        #region Crud operation for TestCase
        //Renders partial view
        public ActionResult TestCaseList()
        {
            try
            {
                var repApp = new ApplicationRepository();
                repApp.Username = SessionManager.TESTER_LOGIN_NAME;

                List<T_Memory_REGISTERED_APPS> apps = new List<T_Memory_REGISTERED_APPS>();
                apps = GlobalVariable.AllApps.FirstOrDefault(x => x.Key.Equals(SessionManager.Schema)).Value;
                if (apps.Count() > 0)
                {
                    ViewBag.listApplications = apps.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).OrderBy(x => x.Text).ToList();
                }

                //var lapp = repApp.ListApplication();
                //var applist = lapp.Select(c => new SelectListItem { Text = c.APP_SHORT_NAME, Value = c.APPLICATION_ID.ToString() }).OrderBy(x => x.Text).ToList();
                //ViewBag.listApplications = applist;

                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repAcc.GetGridList((long)userId, GridNameList.TestCaseList);
                var TCgriddata = GridHelper.GetTestCasewidth(gridlst);
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.namewidth = TCgriddata.Name == null ? "20%" : TCgriddata.Name.Trim() + '%';
                ViewBag.descriptionwidth = TCgriddata.Description == null ? "30%" : TCgriddata.Description.Trim() + '%';
                ViewBag.applicationwidth = TCgriddata.Application == null ? "20%" : TCgriddata.Application.Trim() + '%';
                ViewBag.testSuitewidth = TCgriddata.TestSuite == null ? "20%" : TCgriddata.TestSuite.Trim() + '%';
                ViewBag.actionswidth = TCgriddata.Actions == null ? "10%" : TCgriddata.Actions.Trim() + '%';

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for TestCaseList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for TestCaseList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for TestCaseList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("");
        }

        //Loads TestCase List
        [HttpPost]
        public JsonResult DataLoad()
        {
            try
            {
                var repAcc = new TestCaseRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                //Assign values in local variables
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];
                startRec = startRec + 1;
                var data = new List<TestCaseModel>();
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string DescriptionSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                string ApplicationSearch = Request.Form.GetValues("columns[2][search][value]")[0];
                string TestSuiteSearch = Request.Form.GetValues("columns[3][search][value]")[0];

                //Gets Testcase list
                data = repAcc.ListAllTestCase(lSchema, lConnectionStr, startRec, pageSize, colOrder, orderDir, NameSearch, DescriptionSearch, ApplicationSearch, TestSuiteSearch);

                //Get total number of records
                int totalRecords = 0;
                if (data.Count() > 0)
                {
                    totalRecords = data.FirstOrDefault().TotalCount;
                }
                //Get filtered records count
                int recFilter = 0;
                if (data.Count() > 0)
                {
                    recFilter = data.FirstOrDefault().TotalCount;
                }

                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for DataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for DataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for DataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                throw ex;
            }
        }

        //Deletes Testcase from the system if it is not used in Storyboard
        public JsonResult DeleteTestCase(long TestCaseId)
        {
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
                    Session["TestcaseId"] = null;
                    Session["TestsuiteId"] = null;
                    Session["ProjectId"] = null;
                    resultModel.data = lResult;
                    resultModel.message = "Test Case Deleted Successfully";
                }
                else
                    resultModel.data = lflag;

                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for DeleteTestCase method | TestCaseId: {0} | UserName: {1}", TestCaseId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for DeleteTestCase method | TestCaseId: {0} | UserName: {1}", TestCaseId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for DeleteTestCase method | TestCaseId: {0} | UserName: {1}", TestCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Adds/Edits a Testcase
        [HttpPost]
        public ActionResult AddEditTestCase(TestCaseModel lModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTestSuite = new TestCaseRepository();
                repTestSuite.Username = SessionManager.TESTER_LOGIN_NAME;
                var repTree = new GetTreeRepository();
                repTree.Username = SessionManager.TESTER_LOGIN_NAME;
                var rel = repTestSuite.CheckTestCaseTestSuiteRel(lModel.TestCaseId, Convert.ToInt32(lModel.TestSuiteId));
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var flag = lModel.TestCaseId == 0 ? "added" : "Saved";
                if (rel == true)
                {
                    //checks if the testcase is present in the storyboard
                    var result = repTestSuite.CheckTestCaseExistsInStoryboard(lModel.TestCaseId);
                    if (result.Count <= 0)
                    {
                        var editresult = repTestSuite.AddEditTestCase(lModel, SessionManager.TESTER_LOGIN_NAME);
                        Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                        resultModel.data = editresult;
                        resultModel.message = "Successfully " + flag + " Test Case.";
                    }
                    else
                        resultModel.data = result;
                }
                else
                {
                    var fresult = repTestSuite.AddEditTestCase(lModel, SessionManager.TESTER_LOGIN_NAME);
                    resultModel.data = fresult;
                    resultModel.message = "Successfully " + flag + " Test Case.";
                    Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for AddEditTestCase method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for AddEditTestCase method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);

                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for AddEditTestCase method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Loads dropdown of TestSUite by Application Id in Add/Edit TestCase
        public JsonResult GetTestSuiteByApplicaton(string ApplicationId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTestSuite = new TestSuiteRepository();
                var lResult = new List<RelTestSuiteApplication>();
                if (!string.IsNullOrEmpty(ApplicationId))
                {
                    lResult = repTestSuite.ListRelationTestSuiteApplication(ApplicationId);
                }
                resultModel.data = lResult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetTestSuiteByApplicaton method | ApplicationId: {0} | UserName: {1}", ApplicationId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetTestSuiteByApplicaton method | ApplicationId: {0} | UserName: {1}", ApplicationId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetTestSuiteByApplicaton method | ApplicationId: {0} | UserName: {1}", ApplicationId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Move TestCase from one TestSuite to another

        //Loads dropdown of TestSuite By projectid when clicked on Move TestCase
        public ActionResult GetTestSuiteByproject(long ProjectId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repTree = new GetTreeRepository();
                var lTestSuiteList = repTree.GetTestSuiteList(ProjectId);
                ViewBag.testsuiteList = lTestSuiteList.Select(c => new SelectListItem { Text = c.TestsuiteName, Value = c.TestsuiteId.ToString() }).OrderBy(x => x.Text).ToList();
                resultModel.data = lTestSuiteList;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetTestSuiteByproject method | ProjectId: {0} | UserName: {1}", ProjectId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetTestSuiteByproject method | ProjectId: {0} | UserName: {1}", ProjectId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetTestSuiteByproject method | ProjectId: {0} | UserName: {1}", ProjectId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Moves a Testcase to another TestSuite if it is not present in any storyboard
        public JsonResult MoveTestCase(long lprojectId, long lsuiteId, long caseId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckTestCaseExistsInStoryboard(caseId);
                var lresult = string.Empty;
                if (result.Count > 0)
                    resultModel.data = result;
                else
                {
                    var testSuiterepo = new TestSuiteRepository();
                    testSuiterepo.Username = SessionManager.TESTER_LOGIN_NAME;
                    var TestSuiteName = testSuiterepo.GetTestSuiteNameById(lsuiteId);
                    var Testcase = repo.GetTestcaseNameById(caseId);
                    lresult = repo.MoveTestCase(lprojectId, caseId, lsuiteId);

                    resultModel.message = "Testcase [" + Testcase + "] moved to TestSuite [" + TestSuiteName + "]";
                    resultModel.data = lresult;
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for MoveTestCase method | projectId: {0} | TestSuiteId: {1} | TestCaseId : {2} | UserName: {3}", lprojectId, lsuiteId, caseId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for MoveTestCase method | projectId: {0} | TestSuiteId: {1} | TestCaseId : {2} | UserName: {3}", lprojectId, lsuiteId, caseId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for MoveTestCase method | projectId: {0} | TestSuiteId: {1} | TestCaseId : {2} | UserName: {3}", lprojectId, lsuiteId, caseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Checks whether the testcase name is present in the system and if not, then renames it

        //Changes TestCase name
        public JsonResult ChangeTestCaseName(string TestCaseName, long TestCaseId, string TestCaseDes)
        {
            logger.Info(string.Format("Rename TestCase Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var testCaserepo = new TestCaseRepository();
                testCaserepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = testCaserepo.CheckDuplicateTestCaseName(TestCaseName, TestCaseId);
                if (result)
                    resultModel.data = result;
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
                logger.Error(string.Format("Error occured in TestCase controller for ChangeTestCaseName method | TestCaseName: {0} | TestCaseId: {1} | TestCaseDesc : {2} | UserName: {3}", TestCaseName, TestCaseId, TestCaseDes, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for ChangeTestCaseName method | TestCaseName: {0} | TestCaseId: {1} | TestCaseDesc : {2} | UserName: {3}", TestCaseName, TestCaseId, TestCaseDes, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for ChangeTestCaseName method | TestCaseName: {0} | TestCaseId: {1} | TestCaseDesc : {2} | UserName: {3}", TestCaseName, TestCaseId, TestCaseDes, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Checks whether the testcase is present in the system or not
        public JsonResult CheckDuplicateTestCaseNameExist(string TestCaseName, long? TestCaseId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                TestCaseName = TestCaseName.Trim();
                var testCaserepo = new TestCaseRepository();
                testCaserepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = testCaserepo.CheckDuplicateTestCaseName(TestCaseName, TestCaseId);
                resultModel.status = 1;
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for CheckDuplicateTestCaseNameExist method | TestCaseName: {0} | TestCaseId: {1} | UserName: {2}", TestCaseName, TestCaseId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CheckDuplicateTestCaseNameExist method | TestCaseName: {0} | TestCaseId: {1} | UserName: {2}", TestCaseName, TestCaseId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for CheckDuplicateTestCaseNameExist method | TestCaseName: {0} | TestCaseId: {1} | UserName: {2}", TestCaseName, TestCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PQGrid functionality of TestCase Grid
        public ActionResult AddNewRowTestCaseInSession(long oldRunOrder, long NewRowRunOrder, long testCaseId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                logger.Info(string.Format("Add testcase row in session in TestCase controller for AddNewRowTestCaseInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for AddNewRowTestCaseInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for AddNewRowTestCaseInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for AddNewRowTestCaseInSession method | TestCaseId: {0} | UserName: {1}", testCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTestCaseRowInSession(long stepId, long runOrder, long testCaseId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                logger.Info(string.Format("Delete testcase row in session in TestCase controller for DeleteTestCaseRowInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

                Mars_Memory_TestCase allList = new Mars_Memory_TestCase();
                string testcaseSessionName = string.Format("{0}_Testcase_{1}", SessionManager.Schema, testCaseId);
                if (Session[testcaseSessionName] != null)
                    allList = Session[testcaseSessionName] as Mars_Memory_TestCase;
                List<MB_V_TEST_STEPS> step = allList.allSteps.Where(x => x.STEPS_ID == stepId).ToList();
                bool isPegWindow = step.Any(x => x.KEY_WORD_NAME.Trim().ToLower().Equals("pegwindow"));
                if (step.Count() > 0)
                {
                    allList.allSteps = allList.allSteps.Except(step).OrderBy(x => x.RUN_ORDER).ToList();
                    allList.allSteps.ForEach(x =>
                    {
                        if (x.RUN_ORDER > runOrder)
                        {
                            x.RUN_ORDER--;

                            if (x.KEY_WORD_NAME.Trim().ToLower().Equals("pegwindow"))
                                isPegWindow = false;

                            if (isPegWindow)
                                x.OBJECT_HAPPY_NAME = string.Empty;
                        }
                    });
                }
                Session[testcaseSessionName] = allList;
                resultModel.status = 1;
                resultModel.data = true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for DeleteTestCaseRowInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for DeleteTestCaseRowInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for DeleteTestCaseRowInSession method | TestCaseId: {0} | UserName: {1}", testCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateTestcaseValueInSession(string PropertyName, string PropertyValue, long stepId, long testCaseId, bool isSkipChange, bool skipValue)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                logger.Info(string.Format("Update testcase values in session in TestCase controller for UpdateTestcaseValueInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

                TestCaseRepository tc = new TestCaseRepository();
                Mars_Memory_TestCase allList = new Mars_Memory_TestCase();
                string testcaseSessionName = string.Format("{0}_Testcase_{1}", SessionManager.Schema, testCaseId);
                if (Session[testcaseSessionName] != null)
                    allList = Session[testcaseSessionName] as Mars_Memory_TestCase;
                List<MB_V_TEST_STEPS> step = allList.allSteps.Where(x => x.STEPS_ID == stepId).ToList();
                if (isSkipChange)
                {
                    PropertyName = PropertyName.Replace("skip_", "");
                    var mainDataset = allList.assignedDataSets.FirstOrDefault(x => x.ALIAS_NAME.Trim().ToLower().Equals(PropertyName.Trim().ToLower()));
                    if (mainDataset != null)
                    {
                        long datasetID = mainDataset.DATA_SUMMARY_ID;
                        var dataForDataSet = allList.allSteps.Where(c => c.STEPS_ID == stepId).Select(x => x.dataForDataSets).ToList();
                        if (dataForDataSet.Count() > 0)
                        {
                            dataForDataSet.ForEach(x =>
                            {
                                x.ForEach(c =>
                                {
                                    if (c.DATA_SUMMARY_ID == datasetID)
                                        c.SKIP = skipValue ? 4 : 0;
                                });
                            });
                        }
                    }
                }
                else
                {
                    if (PropertyName.Trim().Equals("keyword"))
                    {
                        if (step.Count() > 0)
                        {
                            long keywordId = tc.GetKeywordIdByName(PropertyValue);
                            step.ForEach(x =>
                            {
                                x.KEY_WORD_NAME = PropertyValue;
                                x.KEY_WORD_ID = keywordId;
                            });
                        }
                    }
                    else if (PropertyName.Trim().Equals("object"))
                    {
                        if (step.Count() > 0)
                        {
                            ObjectViewModel objct = tc.GetObjectIdByName(PropertyValue);
                            step.ForEach(x =>
                            {
                                x.OBJECT_HAPPY_NAME = PropertyValue;
                                x.OBJECT_ID = objct != null ? objct.OBJECT_ID : x.OBJECT_ID;
                                x.OBJECT_NAME_ID = objct != null ? (long)objct.OBJECT_NAME_ID : x.OBJECT_NAME_ID;
                                x.OBJECT_TYPE = objct != null ? objct.OBJECT_TYPE : x.OBJECT_TYPE;
                            });
                        }
                    }
                    else if (PropertyName.Trim().Equals("comment"))
                    {
                        if (step.Count() > 0)
                            step.ForEach(x => { x.COMMENTINFO = PropertyValue; });
                    }
                    else if (PropertyName.Trim().Equals("parameters"))
                    {
                        if (step.Count() > 0)
                            step.ForEach(x => { x.COLUMN_ROW_SETTING = PropertyValue; });
                    }
                    else
                    {
                        var mainDataset = allList.assignedDataSets.FirstOrDefault(x => x.ALIAS_NAME.Trim().ToLower().Equals(PropertyName.Trim().ToLower()));
                        if (mainDataset != null)
                        {
                            long datasetID = mainDataset.DATA_SUMMARY_ID;
                            var dataForDataSet = allList.allSteps.Where(c => c.STEPS_ID == stepId).Select(x => x.dataForDataSets).ToList();
                            if (dataForDataSet.Count() > 0)
                            {
                                dataForDataSet.ForEach(x =>
                                {
                                    x.ForEach(c =>
                                    {
                                        if (c.DATA_SUMMARY_ID == datasetID)
                                            c.DATASETVALUE = PropertyValue;
                                    });
                                });
                            }
                        }
                    }
                }
                Session[testcaseSessionName] = allList;
                resultModel.status = 1;
                resultModel.data = true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for UpdateTestcaseValueInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for UpdateTestcaseValueInSession method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for UpdateTestcaseValueInSession method | TestCaseId: {0} | UserName: {1}", testCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        //Loads all the steps of TestCase grid by TestCaseId
        public ActionResult GetTestCaseDetails(int testcaseId, string dataset)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                TestCaseRepository tc = new TestCaseRepository();
                tc.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                var datasetId = tc.GetDatasetId(dataset);

                #region TESTCASE JSON FILE CODE
                string testCaseName = tc.GetTestCaseNameById(testcaseId);
                List<TestCaseResult> newResult = new List<TestCaseResult>();
                Mars_Memory_TestCase allList = new Mars_Memory_TestCase();
                string testcaseSessionName = string.Format("{0}_Testcase_{1}", SessionManager.Schema, testcaseId);
                if (Session[testcaseSessionName] != null)
                    allList = Session[testcaseSessionName] as Mars_Memory_TestCase;
                else
                {
                    string fileName = string.Format("{0}_{1}.json", testcaseId, testCaseName.Replace("/", "")); //testcaseId + "_" + testCaseName.Replace("/", "") + ".json";
                    string fullPath = Path.Combine(Server.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Testcases.ToString(), SessionManager.Schema, fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        string jsongString = System.IO.File.ReadAllText(fullPath);
                        allList = JsonConvert.DeserializeObject<Mars_Memory_TestCase>(jsongString);
                        Session[testcaseSessionName] = allList;
                    }
                }
                newResult = tc.ConvertTestcaseJsonToList(allList, testcaseId, lSchema, lConnectionStr, (long)SessionManager.TESTER_ID, datasetId);
                #endregion

                //var result = tc.GetTestCaseDetail(testcaseId, lSchema, lConnectionStr, (long)SessionManager.TESTER_ID, datasetId);

                var json = JsonConvert.SerializeObject(newResult);
                resultModel.status = 1;
                resultModel.data = json;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetTestCaseDetails method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetTestCaseDetails method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetTestCaseDetails method | TestCaseId: {0} | UserName: {1}", testcaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Saves TestCase grid if it is validated successfully
        public ActionResult SaveTestCase(string lGrid, string lChanges, string lTestCaseId, string lTestSuiteId,
                string lDeleteColumnsList = "", string lVersion = "")
        {
            string logPath = WebConfigurationManager.AppSettings["LogPathLocation"];
            EmailHelper.logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/" + logPath + "/");
            var repTC = new TestCaseRepository();
            repTC.Username = SessionManager.TESTER_LOGIN_NAME;
            logger.Info(string.Format("Testcase Save Start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                JavaScriptSerializer js = new JavaScriptSerializer();

                decimal testcaseId = int.Parse(lTestCaseId);
                decimal testsuiteid = int.Parse(lTestSuiteId);
                var lUserId = SessionManager.TESTER_ID;

                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(lGrid);
                if (lobj.ToList().Count() == 0)
                {
                    logger.Info(string.Format("Test Case Save end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    resultModel.message = "You can not delete all steps.";
                    resultModel.status = 1;
                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                }

                OracleTransaction ltransaction;
                lSchema = SessionManager.Schema;
                logger.Info(string.Format("before validation check | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                var ValidationSteps = repTC.InsertStgTestcaseValidationTable(lConnectionStr, lSchema, lobj, lTestCaseId);
                logger.Info(string.Format("after validation validation check | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

                OracleConnection lconnection = new OracleConnection(lConnectionStr);

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
                            return Json(resultModel, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (lDeleteColumnsList != "")
                    {
                        logger.Info(string.Format("start delete column list | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                        List<Object> DataSet = js.Deserialize<List<Object>>(lDeleteColumnsList);
                        foreach (var d in DataSet)
                        {

                            var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            repTC.DeleteRelTestCaseDataSummary(long.Parse(lTestCaseId), long.Parse(stp[2].Value.ToString()));
                        }
                        logger.Info(string.Format("end delete column list | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    }
                    logger.Info(string.Format("create table object | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
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
                    logger.Info(string.Format("update  table object | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
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
                    logger.Info(string.Format("add start  table object | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
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
                    logger.Info(string.Format("add end  table object | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

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
                        logger.Info(string.Format("Test Case Save end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                        resultModel.message = "No change in Testcase";
                        resultModel.status = 1;
                        return Json(resultModel, JsonRequestBehavior.AllowGet);
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
                    repTC.InsertStgTestCaseSave(lConnectionStr, lSchema, dt, lTestCaseId, int.Parse(valFeedD));
                    repTC.SaveTestCaseVersion(int.Parse(lTestCaseId), (long)SessionManager.TESTER_ID);

                    logger.Info(string.Format("Test Case Save end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                    resultModel.data = "success";
                }
                else
                {
                    var result = JsonConvert.SerializeObject(ValidationSteps);
                    resultModel.data = result;
                }
                logger.Info(string.Format("Test Case Save end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Test Case saved successfully. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                resultModel.message = "Test Case saved.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for SaveTestCase method | TestCaseId: {0} | TestSuiteId: {1} | UserName: {2}", lTestCaseId, lTestSuiteId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for SaveTestCase method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for SaveTestCase method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                if (ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for SaveTestCase method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveTestCase_History(string lJson, string testCaseId, string testSuiteId, string pKeywordObject = "", string steps = "",
             string DeleteColumnsList = "", string SkipColumns = "", string Version = "")
        {
            string logPath = WebConfigurationManager.AppSettings["LogPathLocation"];
            EmailHelper.logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/" + logPath + "/");
            EmailHelper.WriteMessage("start savetestcase", EmailHelper.logFilePath, "", "");
            //string filepath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log.txt");  //Text File Path
            try
            {

                //using (StreamWriter sw = System.IO.File.AppendText(filepath))
                //{
                //    string error = "start 1";
                //    sw.WriteLine(error);
                //    sw.Flush();
                //    sw.Close();
                //}
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                EmailHelper.WriteMessage(lConnectionStr, EmailHelper.logFilePath, "connection string", "");
                JavaScriptSerializer js = new JavaScriptSerializer();
                TestCaseRepository tc = new TestCaseRepository();
                tc.Username = SessionManager.TESTER_LOGIN_NAME;
                decimal testcaseId = int.Parse(testCaseId);
                decimal testsuiteid = int.Parse(testSuiteId);
                var lUserId = SessionManager.TESTER_ID;

                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(pKeywordObject);
                //var ValidationResult = tc.CheckObjectKeywordValidation(lobj.ToList(), int.Parse(testCaseId));
                //var ValidationSteps = new List<string>();
                // var ValidationSteps = ValidationResult.Where(x => x.IsNotValid == true).ToList();
                OracleTransaction ltransaction;
                lSchema = SessionManager.Schema;
                EmailHelper.WriteMessage("before validation", EmailHelper.logFilePath, "", "");
                var ValidationSteps = tc.InsertStgTestcaseValidationTable(lConnectionStr, lSchema, lobj, testCaseId);

                EmailHelper.WriteMessage("after validation validation", EmailHelper.logFilePath, "", "");

                OracleConnection lconnection = new OracleConnection(lConnectionStr);

                EmailHelper.WriteMessage("after connection ", EmailHelper.logFilePath, "", "");

                if (ValidationSteps.Count() == 0)
                {

                    Dictionary<String, List<Object>> plist = js.Deserialize<Dictionary<String, List<Object>>>(lJson);
                    if (plist["updateList"].Count == 0 && Version == "1")
                    {
                        if (plist["deleteList"].Count > 0)
                        {

                            var lVersion = string.Empty;
                            foreach (var d in plist["deleteList"])
                            {
                                if (string.IsNullOrEmpty(lVersion))
                                {
                                    var versionList = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                                    var versionId = versionList.Where(x => x.Key == "hdnVERSION");
                                    if (!string.IsNullOrEmpty(versionId.FirstOrDefault().Value.ToString()))
                                    {
                                        lVersion = versionId.FirstOrDefault().Value.ToString();
                                        Version = lVersion;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }


                    if (!String.IsNullOrEmpty(Version))
                    {
                        var lflag = tc.MatchTestCaseVersion(int.Parse(testCaseId), Convert.ToInt64(Version));
                        if (!lflag)
                        {
                            return Json("Another User Edited this Test Case. Please reload selected TestCase and Make changes.", JsonRequestBehavior.AllowGet);
                        }
                    }

                    EmailHelper.WriteMessage("start delete count ", EmailHelper.logFilePath, "", "");


                    #region <<Update Steps Region>>

                    if (steps != "")
                    {
                        EmailHelper.WriteMessage("start steps 1 ", EmailHelper.logFilePath, "", "");
                        List<Object> stps = js.Deserialize<List<Object>>(steps);
                        EmailHelper.WriteMessage("start steps  2", EmailHelper.logFilePath, "", "");

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
                                        EmailHelper.WriteMessage("start step update ", EmailHelper.logFilePath, "stepid: " + stepid.ToString() + "  Run Order: " + stp[1].Value.ToString(), "");
                                        var newRun_Order = int.Parse(stp[1].Value.ToString());
                                        tc.UpdateStepID(stepid, int.Parse(stp[1].Value.ToString()));
                                        EmailHelper.WriteMessage("end step update ", EmailHelper.logFilePath, "stepid: " + stepid.ToString() + "  Run Order: " + stp[1].Value.ToString(), "");
                                    }
                                    // }
                                }
                            }
                        }
                    }
                    #endregion

                    EmailHelper.WriteMessage("delete column list", EmailHelper.logFilePath, "", "");

                    if (DeleteColumnsList != "")
                    {
                        EmailHelper.WriteMessage("start delete column list", EmailHelper.logFilePath, "", "");
                        List<Object> DataSet = js.Deserialize<List<Object>>(DeleteColumnsList);
                        foreach (var d in DataSet)
                        {

                            var stp = (((System.Collections.Generic.Dictionary<string, object>)d).ToList());
                            EmailHelper.WriteMessage("delete it", EmailHelper.logFilePath, " Datasetids: " + stp[2].Value.ToString(), "");
                            tc.DeleteRelTestCaseDataSummary(long.Parse(testCaseId), long.Parse(stp[2].Value.ToString()));
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
                    EmailHelper.WriteMessage("end delete count ", EmailHelper.logFilePath, "", "");


                    ///<summary>
                    /// Remove Step
                    ///</summary>
                    Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lJson);
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
                    EmailHelper.WriteMessage("start  feed process  ", EmailHelper.logFilePath, "", "");
                    var valFeed = returnValues.Split('~')[0];
                    var valFeedD = returnValues.Split('~')[1];
                    EmailHelper.WriteMessage("  feed process ID ", EmailHelper.logFilePath, valFeed.ToString(), valFeedD.ToString());
                    //string luser = WebConfigurationManager.AppSettings["User"];
                    //string lpassword = WebConfigurationManager.AppSettings["Password"];
                    //string ldataSource = WebConfigurationManager.AppSettings["DataSource"];
                    //string lSchema = WebConfigurationManager.AppSettings["Schema"];
                    //var lConnectionStr = "Data Source=" + ldataSource + ";User Id=" + luser + ";Password=" + lpassword + ";";


                    //var query = tc.GetTestCaseDetail(Convert.ToInt64(testcaseId), Convert.ToInt64(testsuiteid), lSchema, lConnectionStr, (long)SessionManager.TESTER_ID);
                    var query = tc.GetTestCaseDetail(Convert.ToInt64(testcaseId), lSchema, lConnectionStr, (long)SessionManager.TESTER_ID);
                    EmailHelper.WriteMessage("datatable start  ", EmailHelper.logFilePath, "", "");
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
                    EmailHelper.WriteMessage("Feed Process generate", EmailHelper.logFilePath, "", "");
                    var lskipData = js.Deserialize<Dictionary<String, List<Object>>>(SkipColumns);
                    EmailHelper.WriteMessage("Feed Process generate 1", EmailHelper.logFilePath, "", "");
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
                                EmailHelper.WriteMessage("datasetnames1", EmailHelper.logFilePath, "", "");
                                if (q.DATASETVALUE == null)
                                {
                                    for (int i = 0; i < datasetnames1.Length; i++)
                                    {
                                        EmailHelper.WriteMessage("datasetnames1 value", EmailHelper.logFilePath, datasetnames1[i], "");
                                        datasetnames1[i] = datasetnames1[i];//.Replace("_", " ");  cherish
                                    }
                                    DATASETVALUE = DATASETVALUE == null ? "" : DATASETVALUE;
                                    EmailHelper.WriteMessage("DATASETVALUE value", EmailHelper.logFilePath, DATASETVALUE, "");
                                    foreach (var xy in datasetnames1)
                                    {
                                        EmailHelper.WriteMessage("DATASETname start", EmailHelper.logFilePath, DATASETVALUE, "");
                                        if (DATASETVALUE != null)
                                            DATASETVALUE += ",";


                                        EmailHelper.WriteMessage("DATASETVALUE value=", EmailHelper.logFilePath, DATASETVALUE, "");
                                    }
                                }
                                string DataSettingIds = q.Data_Setting_Id;

                                string[] datasetnames = DATASETNAME.Split(',');
                                string[] datasetids = DATASETID.Split(',');
                                string[] skips = SKIP != null ? SKIP.Split(',') : null;

                                string[] datasetvalues = DATASETVALUE != null ? DATASETVALUE.Split(',') : null;
                                string[] datasetDescription = q.DATASETDESCRIPTION != null ? q.DATASETDESCRIPTION.Split(',') : null;
                                string[] DataSettingId = DataSettingIds != null ? DataSettingIds.Split(',') : null;
                                EmailHelper.WriteMessage("Array bound ", EmailHelper.logFilePath, "", "");
                                EmailHelper.WriteMessage("Array bound DATASETNAME", EmailHelper.logFilePath, DATASETNAME, "");
                                EmailHelper.WriteMessage("Array bound datasetids", EmailHelper.logFilePath, DATASETID, "");
                                EmailHelper.WriteMessage("Array bound DATASETVALUE", EmailHelper.logFilePath, DATASETVALUE, "");
                                EmailHelper.WriteMessage("Array bound q.DATASETDESCRIPTION", EmailHelper.logFilePath, q.DATASETDESCRIPTION, "");
                                EmailHelper.WriteMessage("Array bound DataSettingIds", EmailHelper.logFilePath, DataSettingIds, "");
                                for (int i = 0; i < datasetnames.Length; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    EmailHelper.WriteMessage("Inside loop Step1:" + q.STEPS_ID.ToString() + " # " + q.test_suite_name.ToString() + " #" + q.test_case_name.ToString(), EmailHelper.logFilePath, "", "");
                                    EmailHelper.WriteMessage("Inside loop Datasetname 1:" + datasetnames[i].ToString(), EmailHelper.logFilePath, "", "");
                                    EmailHelper.WriteMessage("Inside loop Datasetids 1:" + datasetids[i].ToString(), EmailHelper.logFilePath, "", "");
                                    EmailHelper.WriteMessage("Inside loop settingsid 1:" + DataSettingId[i].ToString(), EmailHelper.logFilePath, "", "");

                                    dr["STEPSID"] = q.STEPS_ID.ToString();
                                    dr["TESTSUITENAME"] = q.test_suite_name.ToString();
                                    dr["TESTCASENAME"] = q.test_case_name.ToString();
                                    dr["TESTCASEDESCRIPTION"] = q.test_step_description != null ? q.test_step_description.ToString() : "";
                                    dr["DATASETMODE"] = "";
                                    EmailHelper.WriteMessage("Inside loop keyword 1:", EmailHelper.logFilePath, "", "");
                                    dr["KEYWORD"] = q.key_word_name != null ? q.key_word_name.ToString() : "";
                                    EmailHelper.WriteMessage("Inside loop object 1:", EmailHelper.logFilePath, "", "");
                                    dr["OBJECT"] = q.object_happy_name != null ? q.object_happy_name.ToString() : "";
                                    EmailHelper.WriteMessage("Inside loop parameter 1:", EmailHelper.logFilePath, "", "");
                                    dr["PARAMETER"] = q.parameter != null ? q.parameter.ToString() : "";
                                    EmailHelper.WriteMessage("Inside loop comments 1:", EmailHelper.logFilePath, "", "");
                                    dr["COMMENTS"] = q.COMMENT != null ? q.COMMENT.ToString() : "";
                                    EmailHelper.WriteMessage("Inside loop dataset 1:", EmailHelper.logFilePath, "", "");
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
                                        EmailHelper.WriteMessage("Inside loop skips 1:" + skips[i].ToString(), EmailHelper.logFilePath, "", "");
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
                                            EmailHelper.WriteMessage("Inside loop Datasetvalue 1:" + datasetvalues[i].ToString(), EmailHelper.logFilePath, "", "");
                                            dr["DATASETVALUE"] = datasetvalues[i].ToString().Trim();
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
                                    if (datasetDescription != null && datasetDescription.Length > 0 && datasetDescription.Length > i)
                                    {
                                        EmailHelper.WriteMessage("Inside loop description 1:" + datasetDescription[i].ToString(), EmailHelper.logFilePath, "", "");
                                        dr["DATASETDESCRIPTION"] = datasetDescription[i].ToString();
                                    }
                                    else
                                    {
                                        dr["DATASETDESCRIPTION"] = null;
                                    }

                                    dr["FEEDPROCESSDETAILID"] = valFeedD;
                                    dt.Rows.Add(dr);
                                    EmailHelper.WriteMessage("added:", EmailHelper.logFilePath, "", "");
                                }

                            }

                        }

                    }
                    EmailHelper.WriteMessage("Feed Process generate 2", EmailHelper.logFilePath, "", "");
                    if (dlist["updateList"].Count > 0)//dlist["addList"].Count > 0
                    {
                        var update = dlist["updateList"][0];
                        var updates = (((System.Collections.Generic.Dictionary<string, object>)update).ToList());

                        if (dlist.Count > 0)
                        {
                            if (dlist["updateList"].Count > 0)
                            {
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
                                                                        dt.Rows[i]["DATASETVALUE"] = up.Value.ToString().Trim();
                                                                    }
                                                                    else
                                                                    {
                                                                        dt.Rows[i]["DATASETVALUE"] = "";
                                                                    }
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
                                    }
                                }
                            }
                        }
                    }
                    EmailHelper.WriteMessage("Feed Process generate 3", EmailHelper.logFilePath, "", "");
                    if (dlist["addList"].Count > 0)
                    {
                        var addList = dlist["addList"][0];
                        var addsList = (((System.Collections.Generic.Dictionary<string, object>)addList).ToList());

                        EmailHelper.WriteMessage("add list", EmailHelper.logFilePath, "", "");
                        if (addsList.Count > 0)
                        {
                            var lastdtRownumber = dt.Rows.Count;
                            var newrowid = 1;
                            foreach (var d in dlist["addList"])
                            {
                                EmailHelper.WriteMessage("new rowid " + newrowid.ToString(), EmailHelper.logFilePath, "", "");
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
                                EmailHelper.WriteMessage("complete keyword", EmailHelper.logFilePath, "", "");
                                string DATASETNAME = query.FirstOrDefault().DATASETNAME;
                                if (!string.IsNullOrEmpty(DATASETNAME))
                                {
                                    EmailHelper.WriteMessage("inside if ", EmailHelper.logFilePath, "", "");
                                    string[] datasetnamesArray = query.FirstOrDefault().DATASETNAME.Split(',');
                                    for (int i = 0; i < datasetnamesArray.Length; i++)
                                    {
                                        datasetnamesArray[i] = datasetnamesArray[i];
                                    }
                                    EmailHelper.WriteMessage("complete for ", EmailHelper.logFilePath, "", "");
                                    lastdtRownumber = lastdtRownumber + 1;
                                    string[] datasetDescription = query.FirstOrDefault().DATASETDESCRIPTION != null ? query.FirstOrDefault().DATASETDESCRIPTION.Split(',') : null;
                                    string[] datasetid = query.FirstOrDefault().DATASETIDS != null ? query.FirstOrDefault().DATASETIDS.Split(',') : null;
                                    int datSetDec = 0;
                                    EmailHelper.WriteMessage("complete beforeforeach ", EmailHelper.logFilePath, "", "");
                                    foreach (var datasetName in datasetnamesArray)
                                    {
                                        EmailHelper.WriteMessage("Array 3", EmailHelper.logFilePath, "", "");
                                        EmailHelper.WriteMessage("Array 3", EmailHelper.logFilePath, newrowid.ToString(), "");

                                        DataRow dr = dt.NewRow();
                                        dr["STEPSID"] = newrowid;
                                        dr["TESTSUITENAME"] = query.FirstOrDefault().test_suite_name.ToString();
                                        dr["TESTCASENAME"] = query.FirstOrDefault().test_case_name.ToString();
                                        dr["TESTCASEDESCRIPTION"] = query.FirstOrDefault().test_step_description != null ? query.FirstOrDefault().test_step_description.ToString() : "";
                                        dr["DATASETNAME"] = datasetName;
                                        EmailHelper.WriteMessage("Array 3 dataset value", EmailHelper.logFilePath, "", "");
                                        foreach (var item in add)
                                        {
                                            EmailHelper.WriteMessage(" foreach ", EmailHelper.logFilePath, "", "");
                                            if (item.Key.ToUpper() == datasetName.ToUpper())
                                            {
                                                if (item.Value != null)
                                                {
                                                    dr["DATASETVALUE"] = item.Value.ToString().Trim();
                                                }

                                            }
                                            //else if (item.Key.ToUpper().Replace("_", " ") == "SKIP " + datasetName.ToUpper().Replace("_", " ")) cherish
                                            else if (item.Key.ToUpper() == "SKIP_" + datasetName.ToUpper())
                                            {
                                                if (item.Value != null)
                                                {
                                                    dr["skip"] = item.Value.ToString().ToUpper() == "TRUE" ? 4 : 0;
                                                }
                                            }
                                            EmailHelper.WriteMessage(" foreach end", EmailHelper.logFilePath, "", "");
                                        }
                                        EmailHelper.WriteMessage("Array 3 keyword", EmailHelper.logFilePath, "", "");
                                        dr["KEYWORD"] = lKeyword;// q.key_word_name != null ? q.key_word_name.ToString() : "";
                                        dr["OBJECT"] = lObject;//q.object_happy_name != null ? q.object_happy_name.ToString() : "";
                                        dr["PARAMETER"] = lParameter;// q.parameter != null ? q.parameter.ToString() : "";
                                        dr["COMMENTS"] = lComment;// q.COMMENT != null ? q.COMMENT.ToString() : "";
                                        dr["ROWNUMBER"] = lastdtRownumber;//q.RUN_ORDER.ToString();
                                        dr["FEEDPROCESSDETAILID"] = 0;
                                        dr["TABNAME"] = "WebApp";
                                        dr["APPLICATION"] = query.FirstOrDefault().Application.ToString();
                                        //dr["DATASETDESCRIPTION"] = query.FirstOrDefault().test_step_description != null ? query.FirstOrDefault().test_step_description.ToString() : "";
                                        if (datasetDescription != null && datasetDescription.Length > 0 && datasetDescription.Length > datSetDec)
                                        {
                                            EmailHelper.WriteMessage("Array loop description 2:" + datasetDescription[datSetDec].ToString(), EmailHelper.logFilePath, "", "");
                                            dr["DATASETDESCRIPTION"] = datasetDescription[datSetDec];
                                        }
                                        else
                                        {
                                            dr["DATASETDESCRIPTION"] = null;
                                        }

                                        dr["DATASETID"] = datasetid[datSetDec];
                                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                                        dt.Rows.Add(dr);
                                        datSetDec++;
                                        EmailHelper.WriteMessage(" added: " + datSetDec.ToString(), EmailHelper.logFilePath, "", "");
                                    }
                                }
                            }
                        }
                    }
                    EmailHelper.WriteMessage("Feed Process generate 4", EmailHelper.logFilePath, "", "");
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
                                }
                            }
                        }
                    }

                    EmailHelper.WriteMessage("Feed Process generate 5", EmailHelper.logFilePath, "", "");

                    //-------------------------------------------------------------
                    // Save the values to the database in staging table
                    //-------------------------------------------------------------
                    if (ddt.Rows.Count > 0)
                    {
                        EmailHelper.WriteMessage("Feed Process generate 6", EmailHelper.logFilePath, "", "");
                        //OracleTransaction ltransaction;
                        lSchema = SessionManager.Schema;
                        lConnectionStr = SessionManager.APP;
                        // OracleConnection lconnection = new OracleConnection(lConnectionStr);
                        lconnection.Open();
                        ltransaction = lconnection.BeginTransaction();



                        //string cmdquery = @"insert into TBLSTGTESTCASE ( TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION) values(TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,NEWCOUNT_ID,CURRENTDATE,SKIP,DATASETDESCRIPTION)";
                        string cmdquery = "insert into TBLSTGWEBTESTCASE ( TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION,STEPSID,TESTSUITEID,TESTCASEID,DATA_SETTING_ID,DATASETID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14,:15,:16,:17,:18,:19,:20,:21,:22,:23)";
                        //string cmdquery = "insert into TBLSTGWEBTESTCASE ( TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION,STEPSID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14,:15,:16,:17,:18,:19)";
                        int[] ids = new int[ddt.Rows.Count];
                        EmailHelper.WriteMessage("Feed Process generate 7", EmailHelper.logFilePath, "", "");
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

                            EmailHelper.WriteMessage("Feed Process generate 8", EmailHelper.logFilePath, "", "");

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
                            try
                            {
                                EmailHelper.WriteMessage("Feed Process generate 9", EmailHelper.logFilePath, "", "");
                                lcmd.ExecuteNonQuery();
                            }
                            catch (Exception lex)
                            {
                                EmailHelper.WriteMessage(lex.Message.ToString(), EmailHelper.logFilePath, "Save Testcase Execute non query", "");
                                ltransaction.Rollback();

                                throw new Exception(lex.Message);
                            }

                            ltransaction.Commit();
                            lconnection.Close();

                        }

                        var ret = tc.SaveData(int.Parse(valFeed), lConnectionStr, lSchema);

                        if (ret == "not saved")
                        {

                            var r = tc.GetValidations(int.Parse(valFeed));

                            var result = JsonConvert.SerializeObject(r);

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            tc.SaveTestCaseVersion(int.Parse(testCaseId), (long)SessionManager.TESTER_ID);

                            return Json("success", JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    var result = JsonConvert.SerializeObject(ValidationSteps);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                EmailHelper.WriteMessage(ex.Message.ToString(), EmailHelper.logFilePath, "Save Testcase controller Exception", "");
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //Validates TestCase grid and returns the result accordingly
        public ActionResult ValidateTestCase(string lJson, string testCaseId = "986", string pKeywordObject = "", string testSuiteId = "224", string steps = "", string NewColumnsList = "", string ExistDataSetRenameList = "", string DeleteColumnsList = "", string SkipColumns = "")
        {
            logger.Info(string.Format("Testcase Check Validation start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            var result = "";
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                TestCaseRepository tc = new TestCaseRepository();
                tc.Username = SessionManager.TESTER_LOGIN_NAME;
                //check Keyword  object linking
                var lobj = js.Deserialize<KeywordObjectLink[]>(pKeywordObject);
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var ValidationSteps = tc.InsertStgTestcaseValidationTable(lConnectionStr, lSchema, lobj, testCaseId);
                result = JsonConvert.SerializeObject(ValidationSteps);
                resultModel.message = "Test Case validated successfully.";
                resultModel.data = result;
                resultModel.status = 1;
                logger.Info(string.Format("Test Case Check Validation end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Test Case validated successfully. | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for ValidateTestCase method | TestCaseId: {0} | pKeywordObject: {1} | TestSuiteId: {2} | UserName: {3}", testCaseId, pKeywordObject, testSuiteId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for ValidateTestCase method | TestCaseId: {0} | pKeywordObject: {1} | TestSuiteId: {2} | UserName: {3}", testCaseId, pKeywordObject, testSuiteId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for ValidateTestCase method | TestCaseId: {0} | pKeywordObject: {1} | TestSuiteId: {2} | UserName: {3}", testCaseId, pKeywordObject, testSuiteId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Loads keywords in the dropdown for every step in the TestCase grid
        [HttpPost]
        public ActionResult GetKeywordsList(GetKeywordList KeywordList)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                string lGrid = KeywordList.Grid;
                int stepId = KeywordList.stepId;
                JavaScriptSerializer js = new JavaScriptSerializer();

                var lobj = js.Deserialize<KeywordObjectLink[]>(lGrid);
                var repKeyword = new KeywordRepository();
                repKeyword.Username = SessionManager.TESTER_LOGIN_NAME;
                var lList = new List<KeywordList>();
                int lstepId = stepId;
                var lPegStepId = 0;
                int i = 1;
                /*foreach (var item in lobj)
                {
                    if (!string.IsNullOrEmpty(item.Keyword))
                    {
                        if (!string.IsNullOrEmpty(item.Keyword))
                        {
                            if (item.Keyword.ToLower() == "pegwindow" && lPegStepId == 0)
                            {
                                lPegStepId = i;
                            }
                        }
                    }
                    i++;
                }*/
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
                resultModel.status = 1;
                resultModel.data = lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetKeywordsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetKeywordsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetKeywordsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        #region STEPS
        //Appliction -- dbid
        //find applicationfolder
        //get step using stepid --var lPegKeywordNameList = lobj.Where(x => x.Id == stepId).ToList();
        //find pegwindow -- var lPegObjectName = lPegKeywordList.Where(x => x.Id < stepId).OrderByDescending(y => y.Id).First().Object;
        //find pegwindow id from database using object name
        //read file from pegwindow first char
        //find list using pegwind id
        // bind in object dropdown




        //aplication id --database
        //find applicationfolder
        //find pegwindow -- var lPegObjectName = lPegKeywordList.Where(x => x.Id < stepId).OrderByDescending(y => y.Id).First().Object;
        //find file using object pegwindow name
        //bind in object dropdown
        #endregion

        //Loads Objects in the dropdown of Object column for every step in TestCase grid
        [HttpGet]
        public ActionResult GetObjectsList(long appId, string objectName, string keyworkName)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(objectName))
                {
                    resultModel.data = new List<ObjectList>();
                    resultModel.status = 1;
                }
                else
                {
                    var repObject = new ObjectRepository();
                    string folderPath = Path.Combine(Server.MapPath("~/"), "Serialization\\Object\\" + SessionManager.Schema + "\\app_" + appId + "\\" + objectName.FirstOrDefault() + ".json");
                    T_OBJECT_NAMEINFO lPegObj = repObject.GetPegObjectByObjectName(objectName);

                    var typeId = repObject.getTypeIdByKeywordName(keyworkName);
                    var objectList = repObject.GetObjectByParentFromJsonFile(appId, folderPath, lPegObj.OBJECT_NAME_ID, typeId);
                    resultModel.data = objectList;
                    resultModel.status = 1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetObjectsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetObjectsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetObjectsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetObjectsList(GetObjectList ObjectList)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repObject = new ObjectRepository();
                repObject.Username = SessionManager.TESTER_LOGIN_NAME;
                var repKeyword = new KeywordRepository();
                repKeyword.Username = SessionManager.TESTER_LOGIN_NAME;
                var lList = new List<ObjectList>();

                JavaScriptSerializer js = new JavaScriptSerializer();
                string lGrid = ObjectList.Grid;
                int stepId = ObjectList.stepId;
                int lTestCaseId = ObjectList.TestCaseId;

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
                    //else if (lSelectedGrid.Count() > 0)
                    //{
                    //    lList = repObject.GetObjectsByPegWindowType(lTestCaseId).OrderBy(y => y.ObjectName).ToList();
                    //}
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
                resultModel.status = 1;
                resultModel.data = lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetObjectsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetObjectsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetObjectsList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExecuteTestCase(string lGrid, string lTestCaseId, string lTestSuiteId, long storyboradId, string storyborad)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                TestcaseJsonModel testcaseJson = new TestcaseJsonModel();
                TestCaseRepository repTC = new TestCaseRepository();
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<TestcaseJsonViewModel> tcJson = new List<TestcaseJsonViewModel>();
                KeywordRepository repKeyword = new KeywordRepository();
                ObjectRepository repObject = new ObjectRepository();
                repObject.Username = SessionManager.TESTER_LOGIN_NAME;
                repTC.Username = SessionManager.TESTER_LOGIN_NAME;
                repKeyword.Username = SessionManager.TESTER_LOGIN_NAME;
                var plist = js.Deserialize<List<Object>>(lGrid);

                var test = ((System.Collections.Generic.Dictionary<string, object>)plist[0]).ToList().Count;
                if (((System.Collections.Generic.Dictionary<string, object>)plist[0]).ToList().Count < 14)
                {
                    foreach (var d in plist)
                    {
                        TestcaseJsonViewModel tcJsonView = new TestcaseJsonViewModel();
                        var stp = ((System.Collections.Generic.Dictionary<string, object>)d).ToList();
                        tcJsonView.k_n = stp[1].Value.ToString();
                        tcJsonView.obj = stp[2].Value.ToString();
                        tcJsonView.ord = Convert.ToInt32(stp[5].Value);
                        tcJsonView.para = stp[3].Value.ToString();
                        tcJsonView.data = stp[9].Value.ToString();

                        var KeyList = repKeyword.GetKeywordByName(tcJsonView.k_n);
                        tcJsonView.k = KeyList != null ? KeyList.KEY_WORD_ID : 0;

                        var Obj = repObject.GetPegObjectIdByObjectName(tcJsonView.obj);
                        tcJsonView.objN_Id = Obj != null ? Obj.ObjectNameID : 0;
                        tcJsonView.o_Id = Obj != null ? Obj.ObjectId : 0;
                        tcJson.Add(tcJsonView);
                    }
                    tcJson = tcJson.OrderBy(x => x.ord).ToList();
                    testcaseJson.TcJson = js.Serialize(tcJson);
                    testcaseJson.guid = Guid.NewGuid();
                    testcaseJson.user = SessionManager.TESTER_LOGIN_NAME;
                    testcaseJson.database = SessionManager.Schema;

                    var testcaseId = Convert.ToInt64(lTestCaseId);
                    //testcaseJson.applications = repTC.GetApplicationListByTestcaseId(testcaseId);
                    resultModel.data = testcaseJson;
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.data = 0;
                    resultModel.status = 0;
                    resultModel.message = "Execute Testcase steps not working with multiple datasets";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in StoryBoard for Testcase method | StoryBoard Id : {0} | UserName: {1}", lTestCaseId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in StoryBoard for Testcase method | StoryBoard Id : {0} | UserName: {1}", lTestCaseId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Testcase for ExecuteTestCase method | StoryBoard Id : {0} | UserName: {1}", lTestCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export individual TestCase from TestSuite
        public JsonResult ExportTestCase(int TestCaseId, int TestSuiteId)
        {
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            dbtable.dt_Log = null;
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
            sequence_counter.AutoIncrement = true;
            sequence_counter.AutoIncrementSeed = 1;
            sequence_counter.AutoIncrementStep = 1;

            td.Columns.Add(sequence_counter);
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");

            td.Columns.Add("Test Suite Name");


            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");

            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");


            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");



            dbtable.dt_Log = td.Copy();
            if (TestCaseId > 0 && TestSuiteId > 0)
            {
                MARSUtility.ExportExcel excel = new MARSUtility.ExportExcel();
                MARSUtility.ExportHelper exphelper = new MARSUtility.ExportHelper();
                var testCaserepo = new TestCaseRepository();
                testCaserepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var TestCaseName = testCaserepo.GetTestCaseNameById(Convert.ToInt64(TestCaseId));
                var testSuiterepo = new TestSuiteRepository();
                var TestSuiteName = testSuiterepo.GetTestSuiteNameById(Convert.ToInt64(TestSuiteId));

                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                Regex re = new Regex("[;\\/:*?\"<>|&']");
                var TestCase = re.Replace(TestCaseName, "_").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");

                string lFileName = TestCase + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
                name = "Log_" + TestCase + "_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
                strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
                try
                {
                    dbtable.errorlog("Export is started", "Export suite Excel", "", 0);

                    var presult = exphelper.ExportTestCase(TestCaseName, TestSuiteName, lConnectionStr, lSchema, FullPath);

                    if (presult == true)
                    {
                        dbtable.errorlog("Export is completed", "Export Test Case Excel", "", 0);
                        objcommon.excel(dbtable.dt_Log, strPath, "Export", TestSuiteName, "TESTCASE");
                        dbtable.dt_Log = null;
                        return Json(lFileName, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    int line;
                    string msg = ex.Message;
                    line = dbtable.lineNo(ex);
                    dbtable.errorlog("Export stopped", "Export Test Case Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", TestSuiteName, "TESTCASE");
                    dbtable.dt_Log = null;
                    return Json(name, JsonRequestBehavior.AllowGet);
                }

                return Json(lFileName, JsonRequestBehavior.AllowGet);
            }
            else
            {
                dbtable.errorlog("TestSuite/Test Case Id is invalid.", "Export Test Case Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "TESTCASE");
                return Json(name, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Add/Edit Dataset in TestCase
        //Add/Edit a dataset in a TestCase
        [HttpPost]
        public JsonResult AddEditDataset(long? Testcaseid, long? datasetid, string datasetname, string datasetdesc, DataSetTagModel tagmodel)
        {
            logger.Info(string.Format("Add/Edit Dataset start | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var testCaserepo = new TestCaseRepository();
                testCaserepo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = testCaserepo.AddTestDataSet(Testcaseid, datasetid, datasetname, datasetdesc, tagmodel, lConnectionStr, lSchema);
                string[] result1 = result.Split(',');
                var lresult = new
                {
                    datasetId = result1[0],
                    msg = result1[1]
                };

                var flag = datasetid == 0 ? "added" : "saved";
                resultModel.data = lresult;
                resultModel.message = lresult.msg == "success" ? "Dataset is " + flag + " successfully" : "DataSet already exist in system.";
                resultModel.status = 1;
                logger.Info(string.Format("Add/Edit Dataset end | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for AddEditDataset method | Testcaseid: {0} | datasetid: {1} | datasetname: {2} | datasetdesc: {3} | UserName: {4}", Testcaseid, datasetid, datasetname, datasetdesc, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for AddEditDataset method | Testcaseid: {0} | datasetid: {1} | datasetname: {2} | datasetdesc: {3} | UserName: {4}", Testcaseid, datasetid, datasetname, datasetdesc, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for AddEditDataset method | Testcaseid: {0} | datasetid: {1} | datasetname: {2} | datasetdesc: {3} | UserName: {4}", Testcaseid, datasetid, datasetname, datasetdesc, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Checks whether the Dataset used in the Storyboard
        public JsonResult CheckDatasetExistsInStoryboard(long datasetid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDatasetInStoryboard(datasetid);
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for CheckDatasetExistsInStoryboard method | datasetid: {0} | UserName: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CheckDatasetExistsInStoryboard method | datasetid: {0} | UserName: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for CheckDatasetExistsInStoryboard method | datasetid: {0} | UserName: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataSetCount(long ProjectId, long TestSuiteId, long TestCaseId)
        {
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
                logger.Error(string.Format("Error occured in TestCase controller for GetDataSetCount method  | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", ProjectId, TestSuiteId, TestCaseId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetDataSetCount method  | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", ProjectId, TestSuiteId, TestCaseId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetDataSetCount method  | ProjectId: {0} | TestSuiteId: {1} | TestCaseId: {2} | UserName: {3}", ProjectId, TestSuiteId, TestCaseId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CopyDataSet(long? testcaseid, long? oldDatasetid, string datasetname, string datasetdescription = "")
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
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
                    var lresult = repo.CopyDataSet(testcaseid, oldDatasetid, datasetname, datasetdescription, lConnectionStr, lSchema);
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
                logger.Error(string.Format("Error occured in TestCase controller for CopyDataSet method | TestCaseId: {0} | OldTestCaseId: {1} | datasetname: {2} | datasetdescription: {3} | UserName: {4}", testcaseid, oldDatasetid, datasetname, datasetdescription, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CopyDataSet method | TestCaseId: {0} | OldTestCaseId: {1} | datasetname: {2} | datasetdescription: {3} | UserName: {4}", testcaseid, oldDatasetid, datasetname, datasetdescription, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for CopyDataSet method | TestCaseId: {0} | OldTestCaseId: {1} | datasetname: {2} | datasetdescription: {3} | UserName: {4}", testcaseid, oldDatasetid, datasetname, datasetdescription, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Notifies User if any other User had made changes in TestCase grid
        public JsonResult UpdateIsAvailable(string TestCaseIds)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                //if (!string.IsNullOrEmpty(TestCaseIds))
                //{
                //    foreach(var testCaseId in TestCaseIds.Split(','))
                //    {
                //        if (!string.IsNullOrEmpty(testCaseId))
                //        {
                //            string testcaseSessionName = string.Format("{0}_Testcase_{1}", SessionManager.Schema, testCaseId);
                //            if (Session[testcaseSessionName] != null)
                //            {
                //                Session.Remove(testcaseSessionName);
                //            }
                //        }                        
                //    }
                //}
                var rep = new TestCaseRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                rep.UpdateIsAvailable(TestCaseIds);
                resultModel.status = 1;
                resultModel.data = true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for UpdateIsAvailable method | TestCaseIds: {0} | UserName: {1}", TestCaseIds, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for UpdateIsAvailable method | TestCaseIds: {0} | UserName: {1}", TestCaseIds, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for UpdateIsAvailable method | TestCaseIds: {0} | UserName: {1}", TestCaseIds, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetCloseTabLog(string flag, string tabname)
        {
            if (flag == "1")
                logger.Info(string.Format("Close Tab | TabName: {0} | UserName: {1}", tabname, SessionManager.TESTER_LOGIN_NAME));
            else if (flag == "2")
                logger.Info(string.Format("Close all but not this tab | TabName: {0} | UserName: {1}", tabname, SessionManager.TESTER_LOGIN_NAME));
            else if (flag == "3")
                logger.Info(string.Format("Close All Tab | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Saves a copy of TestCase with different name and all the steps of TestCase grid
        public ActionResult SaveAsTestCase(string testcasename, long oldtestcaseid, string testcasedesc, long testsuiteid, long projectid, string optionval, string datasetName = "", string suffix = "")
        {
            logger.Info(string.Format("SaveAs Testcase Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = string.Empty;
                if (!string.IsNullOrEmpty(suffix))
                {
                    suffix = suffix.Trim();
                }
                if (optionval == "1")
                {
                    result = repo.SaveAsTestcase(testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, lSchema, lConnectionStr, SessionManager.TESTER_LOGIN_NAME);
                }
                else if (optionval == "2")
                {
                    result = repo.SaveAsTestCaseOneCopiedDataSet(testcasename, oldtestcaseid, testcasedesc, datasetName, testsuiteid, projectid, suffix, lSchema, lConnectionStr, SessionManager.TESTER_LOGIN_NAME);
                }
                else if (optionval == "3")
                {
                    result = repo.SaveAsTestCaseAllCopiedDataSet(testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, suffix, lSchema, lConnectionStr, SessionManager.TESTER_LOGIN_NAME);
                }

                resultModel.message = result == "success" ? "TestCase created successfully." : "Dataset already exist in the system, please use another suffix.";
                resultModel.status = 1;
                resultModel.data = result;
                logger.Info(string.Format("SaveAs Testcase successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("SaveAs Testcase Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for SaveAsTestCase method | testcasename: {0} | OldTestCaseId: {1} | testcasedesc: {2} | testsuiteid: {3} | projectid: {4} | optionval: {5} | datasetName: {6} | suffix: {7} | UserName: {8}", testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, optionval, datasetName, suffix, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for SaveAsTestCase method | testcasename: {0} | OldTestCaseId: {1} | testcasedesc: {2} | testsuiteid: {3} | projectid: {4} | optionval: {5} | datasetName: {6} | suffix: {7} | UserName: {8}", testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, optionval, datasetName, suffix, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for SaveAsTestCase method | testcasename: {0} | OldTestCaseId: {1} | testcasedesc: {2} | testsuiteid: {3} | projectid: {4} | optionval: {5} | datasetName: {6} | suffix: {7} | UserName: {8}", testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, optionval, datasetName, suffix, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                if (ex.InnerException.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for SaveAsTestCase method | testcasename: {0} | OldTestCaseId: {1} | testcasedesc: {2} | testsuiteid: {3} | projectid: {4} | optionval: {5} | datasetName: {6} | suffix: {7} | UserName: {8}", testcasename, oldtestcaseid, testcasedesc, testsuiteid, projectid, optionval, datasetName, suffix, SessionManager.TESTER_LOGIN_NAME), ex.InnerException.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        #region DataTag
        [HttpPost]
        public ActionResult GroupList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GroupList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GroupList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GroupList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult GroupDataLoad()
        {
            var data = new List<DataTagCommonViewModel>();
            int recFilter = 0;
            int totalRecords = 0;
            string draw = string.Empty;
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                data = repo.ListOfGroup();

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string DescSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                //string ActiveSearch = Request.Form.GetValues("columns[2][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

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

                totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }
                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GroupDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GroupDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GroupDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Add/Update Group values
        [HttpPost]
        public JsonResult AddEditGroup(DataTagCommonViewModel model)
        {
            logger.Info(string.Format("Group Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;

                var _addeditResult = repo.AddEditGroup(model);

                resultModel.message = "Saved [" + model.Name + "] Group.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("Group Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Group Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for AddEditGroup method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for AddEditGroup method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for AddEditGroup method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Check Group name already exist or not
        public JsonResult CheckDuplicateGroupNameExist(string Name, long? Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Name = Name.Trim();
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDuplicateGroupNameExist(Name, Id);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for CheckDuplicateGroupNameExist method | Name: {0} | Id: {1} | Username: {2}", Name, Id, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CheckDuplicateGroupNameExist method | Name: {0} | Id: {1} | Username: {2}", Name, Id, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured  in TestCase controller for CheckDuplicateGroupNameExist method | Name: {0} | Id: {1} | Username: {2}", Name, Id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for SetList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for SetList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for SetList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return PartialView();
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult SetDataLoad()
        {
            var data = new List<DataTagCommonViewModel>();
            int recFilter = 0;
            int totalRecords = 0;
            string draw = string.Empty;
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                data = repo.ListOfSet();

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string DescSearch = Request.Form.GetValues("columns[1][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

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

                totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }
                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for SetDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for SetDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for SetDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Add/Update Set values
        [HttpPost]
        public JsonResult AddEditSet(DataTagCommonViewModel model)
        {
            logger.Info(string.Format("Set Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;

                var _addeditResult = repo.AddEditSet(model);

                resultModel.message = "Saved [" + model.Name + "] Set.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("Set Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Set Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for AddEditSet method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for AddEditSet method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for AddEditSet method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Check Set name already exist or not
        public JsonResult CheckDuplicateSetNameExist(string Name, long? Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Name = Name.Trim();
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDuplicateSetNameExist(Name, Id);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for CheckDuplicateSetNameExist method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CheckDuplicateSetNameExist method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for CheckDuplicateSetNameExist method | Name: {0} | Id: {1} | Username: {2}", Name, Id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FolderList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                ProjectRepository repo = new ProjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);
                var result = repo.ListProject();
                var projectlist = result.Select(c => new SelectListItem { Text = c.PROJECT_NAME, Value = c.PROJECT_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.ProjectList = JsonConvert.SerializeObject(projectlist);
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for FolderList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for FolderList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for FolderList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return PartialView();
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult FolderDataLoad()
        {
            var data = new List<DataTagCommonViewModel>();
            int recFilter = 0;
            int totalRecords = 0;
            string draw = string.Empty;
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                data = repo.ListOfFolder();

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string DescSearch = Request.Form.GetValues("columns[1][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

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

                totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Description.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }
                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for FolderDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for FolderDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for FolderDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Add/Update Folder values
        [HttpPost]
        public JsonResult AddEditFolder(DataTagCommonViewModel model)
        {
            logger.Info(string.Format("Folder Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;

                var _addeditResult = repo.AddEditFolder(model);

                resultModel.message = "Saved [" + model.Name + "] Folder.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("Folder Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Folder Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for AddEditFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for AddEditFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for AddEditFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Check Folder name already exist or not
        public JsonResult CheckDuplicateFolderNameExist(string Name, long? Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Name = Name.Trim();
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckDuplicateFolderNameExist(Name, Id);
                resultModel.message = "success";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for CheckDuplicateFolderNameExist method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CheckDuplicateFolderNameExist method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for CheckDuplicateFolderNameExist method | Name: {0} | Id: {1} | Username: {2}", Name, Id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Get DatasetTag Details
        public ActionResult GetDatasetTagDetails(long datasetid)
        {
            try
            {
                TestCaseRepository repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var dataresults = repo.GetTagDetails(datasetid);
                ViewBag.Folder = dataresults.Count == 0 ? "" : dataresults[0].Folder;
                ViewBag.Group = dataresults.Count == 0 ? "" : dataresults[0].Group;
                ViewBag.Set = dataresults.Count == 0 ? "" : dataresults[0].Set;
                ViewBag.Expresult = dataresults.Count == 0 ? "" : dataresults[0].Expectedresults;
                ViewBag.Sequence = dataresults.Count == 0 ? 0 : dataresults[0].Sequence;
                ViewBag.Diary = dataresults.Count == 0 ? "" : dataresults[0].Diary;
                ViewBag.StepDesc = dataresults.Count == 0 ? "" : dataresults[0].StepDesc;
                var groups = repo.GetGroups();
                ViewBag.Taggroup = groups.Select(x => new SelectListItem { Text = x.GROUPNAME, Value = Convert.ToString(x.GROUPID), Selected = x.GROUPNAME == (dataresults.Count == 0 ? "" : dataresults[0].Group) ? true : false }).Distinct().ToList();

                var folder = repo.GetFolders();
                ViewBag.TagFolder = folder.Select(x => new SelectListItem { Text = x.FOLDERNAME, Value = Convert.ToString(x.FOLDERID), Selected = x.FOLDERNAME == (dataresults.Count == 0 ? "" : dataresults[0].Folder) ? true : false }).Distinct().ToList();

                var sets = repo.GetSets();
                ViewBag.TagSet = sets.Select(x => new SelectListItem { Text = x.SETNAME, Value = Convert.ToString(x.SETID), Selected = x.SETNAME == (dataresults.Count == 0 ? "" : dataresults[0].Set) ? true : false }).Distinct().ToList();

                var dataset = repo.GetDataSetName(datasetid);
                ViewBag.datasetid = datasetid;
                ViewBag.Datasetname = dataset[0].Data_Summary_Name;
                ViewBag.Datasetdesc = dataset[0].Dataset_desc;
                return PartialView("GetDatasetTagDetails");

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for GetDatasetTagDetails method | datasetid: {0} | Username: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for GetDatasetTagDetails method | datasetid: {0} | Username: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for GetDatasetTagDetails method | datasetid: {0} | Username: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                throw;
            }
        }

        //Delete the DatasetTag data by datasetid
        [HttpPost]
        public ActionResult DeleteDatSetTag(long datasetid)
        {
            try
            {
                TestCaseRepository repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.DeleteTagProperties(datasetid);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for DeleteDatSetTag method | datasetid: {0} | Username: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for DeleteDatSetTag method | datasetid: {0} | Username: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for DeleteDatSetTag method | datasetid: {0} | Username: {1}", datasetid, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                throw;
            }

        }

        //Check Folder Sequence already exist or not
        [HttpPost]
        public ActionResult CheckFolderSequenceMapping(long FolderId, long SequenceId, long DatasetId)
        {
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = repo.CheckFolderSequenceMapping(FolderId, SequenceId, DatasetId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for CheckFolderSequenceMapping method | FolderId: {0} | SequenceId: {1} | DatasetId: {2} | Username: {3}", FolderId, SequenceId, DatasetId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for CheckFolderSequenceMapping method | FolderId: {0} | SequenceId: {1} | DatasetId: {2} | Username: {3}", FolderId, SequenceId, DatasetId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for CheckFolderSequenceMapping method | FolderId: {0} | SequenceId: {1} | DatasetId: {2} | Username: {3}", FolderId, SequenceId, DatasetId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                throw;
            }
        }
        #endregion

        #region DatasetTag Import/Export
        //Export DatasetTag
        public JsonResult ExportDatasetTag()
        {
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            dbtable.dt_Log = null;
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
            sequence_counter.AutoIncrement = true;
            sequence_counter.AutoIncrementSeed = 1;
            sequence_counter.AutoIncrementStep = 1;

            td.Columns.Add(sequence_counter);
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");

            td.Columns.Add("Test Suite Name");


            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");

            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");


            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            dbtable.dt_Log = td.Copy();

            ExportExcel excel = new ExportExcel();

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;

            string lFileName = "DATASETTAG_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
            string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
            name = "Log_DATASETTAG_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                dbtable.errorlog("Export is started", "Export DatasetTag Excel", "", 0);

                var presult = excel.ExportDatasetTagExcel(FullPath, lSchema, lConnectionStr);

                if (presult == true)
                {
                    dbtable.errorlog("Export is completed", "Export DatasetTag Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "DATASETTAG");
                    dbtable.dt_Log = null;
                    return Json(lFileName, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Export stopped", "Export DatasetTag Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "DATASETTAG");
                dbtable.dt_Log = null;
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            return Json(lFileName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportDatasetTag()
        {
            var userId = SessionManager.TESTER_ID;
            var repAcc = new ConfigurationGridRepository();
            repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
            var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
            var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

            ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            return PartialView();
        }

        //This method will import datasettag file 
        public ActionResult ImportDatasetTagFile()
        {
            ViewBag.FileName = "";
            string fileName = string.Empty;
            string name = "Log_Import" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ImportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                dbtable.dt_Log = null;
                DataTable td = new DataTable();
                DataColumn sequence_counter = new DataColumn();
                sequence_counter.AutoIncrement = true;
                sequence_counter.AutoIncrementSeed = 1;
                sequence_counter.AutoIncrementStep = 1;

                td.Columns.Add(sequence_counter);
                td.Columns.Add("TimeStamp");
                td.Columns.Add("Message Type");
                td.Columns.Add("Action");

                td.Columns.Add("SpreadSheet cell Address");
                td.Columns.Add("Validation Name");
                td.Columns.Add("Validation Fail Description");
                td.Columns.Add("Application Name");
                td.Columns.Add("Project Name");
                td.Columns.Add("StoryBoard Name");

                td.Columns.Add("Test Suite Name");

                td.Columns.Add("TestCase Name");
                td.Columns.Add("Test step Number");

                td.Columns.Add("Dataset Name");
                td.Columns.Add("Dependancy");
                td.Columns.Add("Run Order");


                td.Columns.Add("Object Name");
                td.Columns.Add("Comment");
                td.Columns.Add("Error Description");
                td.Columns.Add("Program Location");
                td.Columns.Add("Tab Name");

                dbtable.dt_Log = td.Copy();
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase datasettagupload = files[i];
                    if (datasettagupload != null)
                    {

                        string destinationPath = string.Empty;
                        string extension = string.Empty;
                        var uploadFileModel = new List<DatasetTagFileUpload>();
                        MARSUtility.ImportHelper helper = new MARSUtility.ImportHelper();
                        fileName = Path.GetFileNameWithoutExtension(datasettagupload.FileName);

                        extension = Path.GetExtension(datasettagupload.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("dd_mm_yyyy") + "_" + DateTime.Now.TimeOfDay.ToString("hh") + "_" + DateTime.Now.TimeOfDay.ToString("mm") + "_" + DateTime.Now.TimeOfDay.ToString("ss") + "" + extension;
                        destinationPath = Path.Combine(Server.MapPath("~/Import/"), fileName);
                        datasettagupload.SaveAs(destinationPath);

                        string lSchema = SessionManager.Schema;
                        var lConnectionStr = SessionManager.APP;
                        dbtable.errorlog("Import is started", "Import DatasetTag", "", 0);
                        var lPath = helper.MasterImport(0, destinationPath, strPath, "DATASETTAG", 1, "", "", "", 1, lSchema, lConnectionStr);

                        if (lPath == false)
                        {
                            dbtable.errorlog("Import is stopped", "Import DatasetTag", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "DATASETTAG");
                            return Json(strPath + ",validation", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dbtable.errorlog("Import is completed", "Import DatasetTag", "", 0);
                            objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "DATASETTAG");
                            return Json(fileName + ",success", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Import is stopped", "Import DatasetTag", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Import", "", "DATASETTAG");
                dbtable.dt_Log = null;
                return Json(strPath + ",exception", JsonRequestBehavior.AllowGet);

            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        //Export DatasetTag Report
        public JsonResult ExportDatasetTagReport()
        {
            bool presult = false;
            var repo = new TestCaseRepository();
            repo.Username = SessionManager.TESTER_LOGIN_NAME;
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            dbtable.dt_Log = null;
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
            sequence_counter.AutoIncrement = true;
            sequence_counter.AutoIncrementSeed = 1;
            sequence_counter.AutoIncrementStep = 1;

            td.Columns.Add(sequence_counter);
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");

            td.Columns.Add("Test Suite Name");


            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");

            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");


            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            dbtable.dt_Log = td.Copy();

            ExportExcel excel = new ExportExcel();

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;

            string lFileName = "REPORTDATASETTAG_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
            string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lFileName);
            name = "Log_REPORTDATASETTAG_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                dbtable.errorlog("Export is started", "Export ReportDatasetTag Excel", "", 0);
                presult = excel.ExportReportDatasetTagExcel(FullPath, lSchema, lConnectionStr);

                if (presult == true)
                {
                    dbtable.errorlog("Export is completed", "Export ReportDatasetTag Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "REPORTDATASETTAG");
                    dbtable.dt_Log = null;
                    return Json(lFileName, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Export stopped", "Export ReportDatasetTag Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "REPORTDATASETTAG");
                dbtable.dt_Log = null;
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            return Json(lFileName, JsonRequestBehavior.AllowGet);
        }

        public class DatasetTagFileUpload
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
        #endregion

        [HttpPost]
        public ActionResult GetFolderDatasetList(long lId, string name)
        {
            try
            {
                var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                var lRep = new TestCaseRepository();
                lRep.Username = SessionManager.TESTER_LOGIN_NAME;
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var lList = lRep.GetDatasets();

                ViewBag.datasetlist = JsonConvert.SerializeObject(lList);

                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
                ViewBag.FolderName = name;
                ViewBag.FolderId = lId;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for GetFolderDatasetList method | FolderId : {0} | UserName: {1}", lId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Testcase for GetFolderDatasetList method | FolderId : {0} | UserName: {1}", lId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("Error occured in Testcase for GetFolderDatasetList method | FolderId : {0} | UserName: {1}", lId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("_FolderDataSetList");
        }

        public JsonResult GetFolderDatasets(long FolderId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new TestCaseRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var lModel = rep.GetFolderDatasetList(lSchema, lConnectionStr, FolderId);

                var jsonresult = JsonConvert.SerializeObject(lModel);
                resultModel.status = 1;
                resultModel.data = jsonresult;

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for GetFolderDatasetList method | FolderId : {0} | UserName: {1}", FolderId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Testcase for GetFolderDatasetList method | FolderId : {0} | UserName: {1}", FolderId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("Error occured in Testcase for GetFolderDatasetList method | FolderId : {0} | UserName: {1}", FolderId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return new JsonResult()
            {
                Data = resultModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = 86753090  // Use this value to set your maximum size for all of your Requests
            };
        }

        public ActionResult SaveFolderDatasetData(string lgridchange, string lgrid, long FolderId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lGridData = js.Deserialize<List<FolderDataSetViewModel>>(lgrid);
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var stgresult = false;

                Dictionary<String, List<Object>> dlist = js.Deserialize<Dictionary<String, List<Object>>>(lgridchange);
                //ValidatResultViewModel
                var lValidationList = new List<FolderDataSetViewModel>();
                if (lGridData.Count() > 0)
                {
                    var dataset = lGridData.Where(x => x.DataSetId == null).ToList();
                    lValidationList.AddRange(dataset.Select(x => new FolderDataSetViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Please Select Dataset" }).ToList());

                    var description = lGridData.Where(x => string.IsNullOrEmpty(x.DatasetDesc)).ToList();
                    lValidationList.AddRange(description.Select(x => new FolderDataSetViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Please add Dataset Description" }).ToList());

                    //var seq = lGridData.Where(x => x.SEQ == null).ToList();
                    //lValidationList.AddRange(seq.Select(x => new FolderDataSetViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " PLease add SEQ" }).ToList());

                    var DuplicateDataset = lGridData.GroupBy(x => string.IsNullOrEmpty(x.DatasetName) ? "" : Convert.ToString(x.DatasetName).ToUpper())
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key)
                                .ToList();
                    var DuplicateRows = lGridData.Where(x => DuplicateDataset.Contains(x.DatasetName != null ? x.DatasetName.ToUpper() : "")).ToList();

                    lValidationList.AddRange(DuplicateRows.Select(x => new FolderDataSetViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Duplicate DataSet name " }).ToList());


                    lGridData.ForEach(item =>
                    {
                        item.IsValid = int.TryParse(item.SEQ, out int result);
                    });
                    lValidationList.AddRange(lGridData.Where(x => x.IsValid == false).Select(x => new FolderDataSetViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = "Please add Correct SEQ. Only numaric value allow" }).ToList());

                    var DuplicateSeq = lGridData.GroupBy(x => x.SEQ)
                                                    .Where(g => g.Count() > 1)
                                                    .Select(y => y.Key)
                                                    .ToList();
                    var DuplicateSeqRows = lGridData.Where(x => DuplicateSeq.Contains(x.SEQ)).ToList();
                    lValidationList.AddRange(DuplicateSeqRows.Select(x => new FolderDataSetViewModel { IsValid = false, pq_ri = x.pq_ri, ValidMsg = " Duplicate SEQ " }).ToList());

                }
                if (lValidationList.Count() > 0)
                {
                    var lValidationResult = lValidationList.OrderBy(x => x.pq_ri).GroupBy(x => new { x.pq_ri, x.IsValid }).Select(g => new { g.Key.pq_ri, g.Key.IsValid, ValidMsg = string.Join(", ", g.Select(i => i.ValidMsg)) }).ToList();
                    var result = JsonConvert.SerializeObject(lValidationResult.OrderBy(x => x.pq_ri));
                    resultModel.data = result;
                    resultModel.message = "validation";
                    resultModel.status = 0;
                    return Json(resultModel, JsonRequestBehavior.AllowGet);
                }

                string returnValues = repo.InsertFeedProcess();
                var valFeed = returnValues.Split('~')[0];
                valFeedD = returnValues.Split('~')[1];
                DataTable dt = new DataTable();
                dt.Columns.Add("FEEDPROCESSDETAILID");
                dt.Columns.Add("FOLDERID");
                dt.Columns.Add("DATASETID");
                dt.Columns.Add("DATASETNAME");
                dt.Columns.Add("DESCRIPTION");
                dt.Columns.Add("TESTCASE");
                dt.Columns.Add("TESTSUITE");
                dt.Columns.Add("STORYBOARDS");
                dt.Columns.Add("SEQ");

                //if(reSeqFlag > 0)
                //{
                if (lGridData.Count() > 0)
                {
                    foreach (var item in lGridData)
                    {
                        DataRow dr = dt.NewRow();
                        dr["FEEDPROCESSDETAILID"] = valFeedD;
                        dr["FOLDERID"] = Convert.ToString(FolderId);
                        dr["DATASETID"] = Convert.ToString(item.DataSetId);
                        dr["DATASETNAME"] = Convert.ToString(item.DatasetName);
                        dr["DESCRIPTION"] = Convert.ToString(item.DatasetDesc);
                        dr["TESTCASE"] = Convert.ToString(item.TestCase);
                        dr["TESTSUITE"] = Convert.ToString(item.TestSuite);
                        dr["STORYBOARDS"] = Convert.ToString(item.Storyboard);
                        dr["SEQ"] = Convert.ToString(item.SEQ);
                        dt.Rows.Add(dr);
                    }
                    stgresult = repo.InsertFolderDatasetInStgTbl(lConnectionStr, lSchema, dt, int.Parse(valFeedD));
                    if (stgresult == true)
                    {
                        var fresult = repo.updateFolderDataset(lConnectionStr, lSchema, valFeedD);
                        //Delete Step
                        repo.DeleteFolderDataset(lConnectionStr, lSchema, valFeedD, FolderId);
                    }
                }

                resultModel.message = "success";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for SaveFolderDatasetData method | FolderId : {0} | UserName: {1}", FolderId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for SaveFolderDatasetData method | FolderId : {0} | UserName: {1}", FolderId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for SaveFolderDatasetData method | FolderId : {0} | UserName: {1}", FolderId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReSequenceFolderDataset(string lgrid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var lGridData = js.Deserialize<List<FolderDataSetViewModel>>(lgrid);
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                var gridlist = JsonConvert.DeserializeObject<List<FolderDataSetViewModel>>(lgrid);
                var result = repo.ReSequenceFolderDataset(gridlist);

                resultModel.message = "success";
                resultModel.status = 1;
                resultModel.data = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for ReSequenceFolderDataset method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for ReSequenceFolderDataset method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for ReSequenceFolderDataset method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateStoryboardForFolder(long folderId, string fName)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var valFeedD = string.Empty;
                StoryBoardRepository srepo = new StoryBoardRepository();
                srepo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                string lreturnValues = srepo.InsertFeedProcess();
                var lvalFeed = lreturnValues.Split('~')[0];
                var lvalFeedD = lreturnValues.Split('~')[1];
                logger.Info(string.Format("Storyborad Saves 4 | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                //var result = srepo.CreateStoryboradForFolder(folderId, proId, SBName, SBDesc);
                var result = srepo.CreateStoryboradForFolder(folderId, fName, lSchema, lConnectionStr, lvalFeed, lvalFeedD);

                resultModel.data = result;
                resultModel.message = "Storyboard saved successfully.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFolderDatasetDetails(string DatasetName)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var rep = new TestCaseRepository();
                rep.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var DatasetId = rep.GetDatasetId(DatasetName);
                if (DatasetId > 0)
                {
                    var lModel = rep.GetDataset(lSchema, lConnectionStr, DatasetId);
                    var jsonresult = JsonConvert.SerializeObject(lModel);
                    resultModel.data = jsonresult;
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.message = "Please Select Dataset.";
                    resultModel.status = 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for GetFolderDatasetDetails method | DatasetName : {0} | UserName: {1}", DatasetName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Testcase for GetFolderDatasetDetails method | DatasetName : {0} | UserName: {1}", DatasetName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("Error occured in Testcase for GetFolderDatasetDetails method | DatasetName : {0} | UserName: {1}", DatasetName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return new JsonResult()
            {
                Data = resultModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = 86753090  // Use this value to set your maximum size for all of your Requests
            };
        }

        public ActionResult RunStoryboardForFolder(long folderId = 0)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                string filter = "";
                var result = repo.GetFilterList();
                if (folderId > 0)
                    filter = repo.GetFilterByFolderId(folderId);

                resultModel.message = filter;
                resultModel.status = 1;
                resultModel.data = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoryboardById(string ProjectIds)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var result = repo.GetStoryboardById(ProjectIds);
                resultModel.message = "success";
                resultModel.status = 1;
                resultModel.data = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for CreateStoryboardForFolder method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveFolderFilter(string ProjectIds, string storybaordIds, string filtername, int Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                if (Id <= 0)
                {
                    var checkduplicateflag = repo.CheckDuplicateFilterName(filtername);
                    if (checkduplicateflag)
                    {
                        resultModel.data = checkduplicateflag;
                        resultModel.message = "Dublicate FilterName";
                        resultModel.status = 0;
                        return Json(resultModel, JsonRequestBehavior.AllowGet);
                    }
                }

                var saveresult = repo.SaveFolderFilter(ProjectIds, storybaordIds, filtername, Id);
                var result = repo.GetFilterList();
                resultModel.data = JsonConvert.SerializeObject(result);
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for SaveFolderFilter method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for SaveFolderFilter method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for SaveFolderFilter method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveFolderForFilter(long FolderID, long FilterId, string Fname)
        {
            ResultModel resultModel = new ResultModel();
            PathViewModel pathView = new PathViewModel();
            try
            {
                var valFeedD = string.Empty;
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                var batchfilename = Fname + ".bat";
                var logFile = Fname + ".log";
                string MARS_REPORT_BATCHFILE_PATH = WebConfigurationManager.AppSettings["MARS_REPORT_BATCHFILE_PATH"];
                string MARS_REPORT_REPORTOUTPUT_PATH = WebConfigurationManager.AppSettings["MARS_REPORT_REPORTOUTPUT_PATH"];
                string MARS_REPORT_REPORTOUTPUT_LOG_PATH = WebConfigurationManager.AppSettings["MARS_REPORT_REPORTOUTPUT_LOG_PATH"];
                string MARSBATCHFILE_PATH = WebConfigurationManager.AppSettings["MARSBATCHFILE_PATH"];
                var fullpath = Path.Combine(Server.MapPath("~/" + MARS_REPORT_BATCHFILE_PATH), batchfilename);
                var outputpath = Server.MapPath("~/" + MARS_REPORT_REPORTOUTPUT_PATH);
                var logpath = Path.Combine(Server.MapPath("~/" + MARS_REPORT_REPORTOUTPUT_LOG_PATH), logFile);
                var checkduplicateflag = repo.SaveFolderForFilter(FolderID, FilterId);
                if (checkduplicateflag)
                {
                    var filename = ExportDatasetTagReportExcal(FilterId.ToString(), Fname);

                    if (!string.IsNullOrEmpty(filename))
                    {
                        string text = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~/" + MARS_REPORT_BATCHFILE_PATH), MARSBATCHFILE_PATH));
                        text = text.Replace("Path1", outputpath);
                        text = text.Replace("Path2", Fname);
                        text = text.Replace("Path3", filename);
                        text = text.Replace("Path4", logpath);

                        using (StreamWriter sw = System.IO.File.CreateText(fullpath))
                        {
                            sw.Write(text);
                        }
                        var result = ExecuteCommand(fullpath);

                        if (result.Contains("Success"))
                        {
                            var filelist = System.IO.File.ReadAllLines(logpath).ToList();
                            var t1 = filelist.Where(x => x.Contains("Report is saved to")).FirstOrDefault();
                            if (t1 != null)
                                pathView.ReportPath = t1.Replace("Report is saved to ", "");

                            pathView.LogPath = logpath;
                            resultModel.data = pathView;
                            resultModel.status = 1;
                        }
                        else
                        {
                            resultModel.message = result;
                            resultModel.status = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in testcase for SaveFolderFilter method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in testcase for SaveFolderFilter method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in testcase for SaveFolderFilter method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FolderRun(PathViewModel pathView)
        {
            ViewBag.logfile = pathView.LogPath;
            ViewBag.reportfile = pathView.ReportPath;
            return PartialView("FolderRun");
        }

        [HttpPost]
        public ActionResult FilterList()
        {
            try
            {
                var userId = SessionManager.TESTER_ID;
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                ProjectRepository repo = new ProjectRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var Widthgridlst = repAcc.GetGridList((long)userId, GridNameList.ResizeLeftPanel);
                var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);
                var result = repo.ListProject();
                var projectlist = result.Select(c => new SelectListItem { Text = c.PROJECT_NAME, Value = c.PROJECT_ID.ToString() }).OrderBy(x => x.Text).ToList();
                ViewBag.ProjectList = JsonConvert.SerializeObject(projectlist);
                ViewBag.width = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] + "px" : Rgriddata.Resize.Trim() + "px";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for FilterList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for FilterList method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for FilterList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return PartialView();
        }

        [HttpPost]
        public JsonResult FilterDataLoad()
        {
            var data = new List<FilterModelView>();
            int recFilter = 0;
            int totalRecords = 0;
            string draw = string.Empty;
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                data = repo.GetFolderFilterList();

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string ProjectSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                string StoryboradSearch = Request.Form.GetValues("columns[2][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(ProjectSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Project) && p.Project.ToString().ToLower().Contains(ProjectSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(StoryboradSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Storyborad) && p.Storyborad.ToString().ToLower().Contains(StoryboradSearch.ToLower())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.Name).ToList();
                            break;
                        case "Project":
                            data = data.OrderByDescending(a => a.Project).ToList();
                            break;
                        case "Storyborad":
                            data = data.OrderByDescending(a => a.Storyborad).ToList();
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
                        case "Project":
                            data = data.OrderBy(a => a.Project).ToList();
                            break;
                        case "Storyborad":
                            data = data.OrderBy(a => a.Storyborad).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.Name).ToList();
                            break;
                    }
                }

                totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Project.ToString().ToLower().Contains(search.ToLower()) ||
                     p.Storyborad.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }
                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in TestCase controller for FolderDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in TestCase controller for FolderDataLoad method | Username: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in TestCase controller for FolderDataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFilter(int Id)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repo = new TestCaseRepository();
                repo.Username = SessionManager.TESTER_LOGIN_NAME;
                var lresult = repo.DeleteFilter(Id);

                resultModel.message = "success";
                resultModel.data = lresult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Testcase for DeleteFilter method | Filter Id : {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Testcase for DeleteFilter method |  Filter Id : {0} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Testcase for DeleteFilter method |  Filter Id : {1} | UserName: {1}", Id, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public string ExportDatasetTagReportExcal(string filterID, string Fname)
        {
            bool presult = false;
            var repo = new TestCaseRepository();
            repo.Username = SessionManager.TESTER_LOGIN_NAME;
            string name = "Log_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string log_path = WebConfigurationManager.AppSettings["ExportLogPath"];
            string strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            dbtable.dt_Log = null;
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
            sequence_counter.AutoIncrement = true;
            sequence_counter.AutoIncrementSeed = 1;
            sequence_counter.AutoIncrementStep = 1;

            td.Columns.Add(sequence_counter);
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");

            td.Columns.Add("Test Suite Name");


            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");

            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");


            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            dbtable.dt_Log = td.Copy();

            ExportExcel excel = new ExportExcel();

            string lSchema = SessionManager.Schema;
            var lConnectionStr = SessionManager.APP;

            string lFileName = Fname + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
            string MARS_REPORT_REPORTTEMPLATES = WebConfigurationManager.AppSettings["MARS_REPORT_REPORTTEMPLATES"];
            string FullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/" + MARS_REPORT_REPORTTEMPLATES), lFileName);
            name = "Log_" + Fname + "_Export" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
            strPath = Path.Combine(Server.MapPath("~/" + log_path), name);
            try
            {
                dbtable.errorlog("Export is started", "Export ReportDatasetTag Excel", "", 0);


                var Filterobj = repo.GetFilter(long.Parse(filterID));
                if (Filterobj != null)
                    presult = excel.ExportReportDatasetTagByFilterExcel(FullPath, lSchema, lConnectionStr, Filterobj.PROJECT_IDS, Filterobj.STORYBOARD_IDS, Fname);

                if (presult == true)
                {
                    dbtable.errorlog("Export is completed", "Export ReportDatasetTag Excel", "", 0);
                    objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "REPORTDATASETTAG");
                    dbtable.dt_Log = null;
                    return lFileName;
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog("Export stopped", "Export ReportDatasetTag Excel", "", 0);
                objcommon.excel(dbtable.dt_Log, strPath, "Export", "", "REPORTDATASETTAG");
                dbtable.dt_Log = null;
                return lFileName;
            }
            return lFileName;
        }

        public string ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;
            processInfo = new ProcessStartInfo();
            string MARS_EXE_PATH = WebConfigurationManager.AppSettings["MARS_REPORT_EXE"];
            processInfo.WorkingDirectory = Server.MapPath("~/" + MARS_EXE_PATH);
            processInfo.FileName = "cmd.exe";
            processInfo.Arguments = "/C " + command;
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            process.Close();

            return output;
        }
    }
}

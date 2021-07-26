using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class ConfigurationGridController : Controller
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        public ConfigurationGridController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        [HttpPost]
        public ActionResult GridList()
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
                logger.Error(string.Format("Error occured in ConfiguratGridListionGrid for GridList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for GridList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for GridList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView();
        }

        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoadGridList()
        {
            var data = new List<GridModel>();
            string draw = string.Empty;
            int totalRecords = 0;
            int recFilter = 0;
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                string search = Request.Form.GetValues("search[value]")[0];
                draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                data = repAcc.ListAllGrid();

                string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.GridName) && x.GridName.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }

                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Grid Name":
                            data = data.OrderByDescending(a => a.GridName).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.GridName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Grid Name":
                            data = data.OrderBy(a => a.GridName).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.GridName).ToList();
                            break;
                    }
                }

                 totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => (!string.IsNullOrEmpty(p.GridName) && p.GridName.ToLower().Contains(search.ToLower()))).ToList();
                }
                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
               
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfiguratGridListionGrid for DataLoadGridList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for DataLoadGridList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for DataLoadGridList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //This method save application grid width
        [HttpPost]
        public ActionResult SaveAppGridWidth(AppGridWidthModel appGridWidth)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditAppGridWidth(appGridWidth, (long)userId, userName);

                resultModel.message = "Successfully submitted Application Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveAppGridWidth method | Name Id : {0} | UserName: {1}", appGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveAppGridWidth method | Name Id : {0} | UserName: {1}", appGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveAppGridWidth method | Name Id : {0} | UserName: {1}", appGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method get grid width by Id
        [HttpPost]
        public ActionResult GetGridbyId(long Id, string gridName)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var result = repAcc.GetGridbyId(Id, (long)userId);
                if (gridName.Trim() == GridNameList.ApplicationList)
                {
                    var appGridWidth = GridHelper.GetApplicationwidth(result, Id);
                    resultModel.data = appGridWidth;
                }
                else if (gridName.Trim() == GridNameList.ProjectList)
                {
                    var proGridWidth = GridHelper.GetProjectwidth(result, Id);
                    resultModel.data = proGridWidth;
                }
                else if (gridName.Trim() == GridNameList.KeywordList)
                {
                    var keyGridWidth = GridHelper.GetKeywordwidth(result, Id);
                    resultModel.data = keyGridWidth;
                }
                else if (gridName.Trim() == GridNameList.TestSuiteList)
                {
                    var TsGridWidth = GridHelper.GetTestSuitewidth(result, Id);
                    resultModel.data = TsGridWidth;
                }
                else if (gridName.Trim() == GridNameList.TestCaseList)
                {
                    var TsGridWidth = GridHelper.GetTestCasewidth(result, Id);
                    resultModel.data = TsGridWidth;
                }
                else if (gridName.Trim() == GridNameList.ObjectList)
                {
                    var objGridWidth = GridHelper.GetObjectwidth(result, Id);
                    resultModel.data = objGridWidth;
                }
                else if (gridName.Trim() == GridNameList.VaribleList)
                {
                    var varGridWidth = GridHelper.GetVariblewidth(result, Id);
                    resultModel.data = varGridWidth;
                }
                else if (gridName.Trim() == GridNameList.UserList)
                {
                    var userGridWidth = GridHelper.GetUserwidth(result, Id);
                    resultModel.data = userGridWidth;
                }
                else if (gridName.Trim() == GridNameList.TestCasePage)
                {
                    var TCPGridWidth = GridHelper.GetTestCasePqgridwidth(result, Id);
                    resultModel.data = TCPGridWidth;
                }
                else if (gridName.Trim() == GridNameList.StoryboradPage)
                {
                    var SPGridWidth = GridHelper.GetStoryboardPqgridwidth(result, Id);
                    resultModel.data = SPGridWidth;
                }
                else if (gridName.Trim() == GridNameList.ResizeLeftPanel)
                {
                    var RGridWidth = GridHelper.GetLeftpanelgridwidth(result, Id);
                    RGridWidth.Resize = RGridWidth.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] : RGridWidth.Resize;
                    resultModel.data = RGridWidth;
                }
                else
                {
                    resultModel.data = "error";
                }
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for GetGridbyId method | Grid Id : {0} | Grid Name : {1} | UserName: {2}", Id, gridName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for GetGridbyId method | Grid Id : {0} | Grid Name : {1} | UserName: {2}", Id, gridName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for GetGridbyId method | Grid Id : {0} | Grid Name : {1} | UserName: {2}", Id, gridName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
         
        }

        //This method save project grid width
        [HttpPost]
        public ActionResult SaveProjectGridWidth(ProjectGridWidthModel projectGridWidth)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditProjectGridWidth(projectGridWidth, (long)userId, userName);
                resultModel.message = "Successfully submitted Project Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveProjectGridWidth method | Name Id : {0} | UserName: {1}", projectGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveProjectGridWidth method | Name Id : {0} | UserName: {1}", projectGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveProjectGridWidth method | Name Id : {0} | UserName: {1}", projectGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save Keyworad grid width
        [HttpPost]
        public ActionResult SaveKeywordGridWidth(KeywordGridWidthModel keywordGridWidth)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditKeywordGridWidth(keywordGridWidth, (long)userId, userName);
                resultModel.message = "Successfully submitted Keyword Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveKeywordGridWidth method | Name Id : {0} | UserName: {1}", keywordGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveKeywordGridWidth method | Name Id : {0} | UserName: {1}", keywordGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveKeywordGridWidth method | Name Id : {0} | UserName: {1}", keywordGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
            
        }

        //This method save Testsuite grid width
        [HttpPost]
        public ActionResult SaveTestSuiteGridWidth(TestSuiteGridWidthModel TSModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditTestSuiteGridWidth(TSModel, (long)userId, userName);
                resultModel.message = "Successfully submitted Test Suite Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveTestSuiteGridWidth method | Name Id : {0} | UserName: {1}", TSModel.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveTestSuiteGridWidth method | Name Id : {0} | UserName: {1}", TSModel.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveTestSuiteGridWidth method | Name Id : {0} | UserName: {1}", TSModel.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save Testcase grid width
        [HttpPost]
        public ActionResult SaveTestCaseGridWidth(TestCaseGridWidthModel TCModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditTestCaseGridWidth(TCModel, (long)userId, userName);
                resultModel.message = "Successfully submitted Test Case Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveTestCaseGridWidth method | Name Id : {0} | UserName: {1}", TCModel.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveTestCaseGridWidth method | Name Id : {0} | UserName: {1}", TCModel.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveTestCaseGridWidth method | Name Id : {0} | UserName: {1}", TCModel.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save object grid width
        [HttpPost]
        public ActionResult SaveObjectGridWidth(ObjectGridWidthModel ObjModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditObjectGridWidth(ObjModel, (long)userId, userName);
                resultModel.message = "Successfully submitted Object Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveObjectGridWidth method | Object Id : {0} | UserName: {1}", ObjModel.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveObjectGridWidth method | Object Id : {0} | UserName: {1}", ObjModel.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveObjectGridWidth method | Object Id : {0} | UserName: {1}", ObjModel.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save varibel grid width
        [HttpPost]
        public ActionResult SaveVaribleGridWidth(VariableGridWidthModel variableGridWidth)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditVaribleGridWidth(variableGridWidth, (long)userId, userName);
                resultModel.message = "Successfully submitted Varible Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveVaribleGridWidth method | Name Id : {0} | UserName: {1}", variableGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveVaribleGridWidth method | Name Id : {0} | UserName: {1}", variableGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveVaribleGridWidth method | Name Id : {0} | UserName: {1}", variableGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save user grid width
        [HttpPost]
        public ActionResult SaveUserGridWidth(UserGridWidthModel userGridWidth)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditUserGridWidth(userGridWidth, (long)userId, userName);
                resultModel.message = "Successfully submitted Users Grid Width.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveUserGridWidth method | Name Id : {0} | UserName: {1}", userGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveUserGridWidth method | Name Id : {0} | UserName: {1}", userGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveUserGridWidth method | Name Id : {0} | UserName: {1}", userGridWidth.NameId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save Testcase pqgrid width
        [HttpPost]
        public ActionResult SaveTestCasePqGridWidth(TestcasePqGridWidthModel testcasePqGrid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditTestCasePqGridWidth(testcasePqGrid, (long)userId, userName);
                resultModel.message = "Width of the columns in Test case editor is changed successfully.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveTestCasePqGridWidth method | Name Id : {0} | UserName: {1}", testcasePqGrid.GridId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveTestCasePqGridWidth method | Name Id : {0} | UserName: {1}", testcasePqGrid.GridId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveTestCasePqGridWidth method | Name Id : {0} | UserName: {1}", testcasePqGrid.GridId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save storyboard pqgrid width
        [HttpPost]
        public ActionResult SaveStoryboardPqGridWidth(StoryboardPqGridWidthModel storyboardPqGrid)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditStoryboardPqGridWidth(storyboardPqGrid, (long)userId, userName);
                resultModel.message = "Width of the columns in storyboard editor is changed successfully.";
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveStoryboardPqGridWidth method | Name Id : {0} | UserName: {1}", storyboardPqGrid.GridId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveStoryboardPqGridWidth method | Name Id : {0} | UserName: {1}", storyboardPqGrid.GridId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveStoryboardPqGridWidth method | Name Id : {0} | UserName: {1}", storyboardPqGrid.GridId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //This method save left panel grid width
        [HttpPost]
        public ActionResult SaveLeftPanelGridWidth(LeftPanelPqGridWidthModel leftPanel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var repAcc = new ConfigurationGridRepository();
                repAcc.Username = SessionManager.TESTER_LOGIN_NAME;
                var userId = SessionManager.TESTER_ID;
                var userName = SessionManager.TESTER_LOGIN_NAME;
                var result = repAcc.AddEditLeftPanelGridWidth(leftPanel, (long)userId, userName);
                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in ConfigurationGrid for SaveLeftPanelGridWidth method | Name Id : {0} | UserName: {1}", leftPanel.GridId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in ConfigurationGrid for SaveLeftPanelGridWidth method | Name Id : {0} | UserName: {1}", leftPanel.GridId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in ConfigurationGrid for SaveLeftPanelGridWidth method | Name Id : {0} | UserName: {1}", leftPanel.GridId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
}
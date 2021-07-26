using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class QueryController : Controller
    {
        public QueryController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        //Get QueryList tab. 
        public ActionResult QueryList()
        {
            try
            {
                var userid = SessionManager.TESTER_ID;

                //to load databaseNames in the dropdown
                var db_repo = new DatabaseConnectionRepository();
                var dbNames = db_repo.GetDatabaseNames();
                ViewBag.databaseNameList = dbNames;

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for QueryList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Query for QueryList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for QueryList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("QueryList");
        }

        //Loads the data into the grid and applies filter and ordering
        [HttpPost]
        public JsonResult DataLoad()
        {
            //Get Repository
            logger.Info(string.Format("Query list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var query_repo = new QueryRepository();
            query_repo.Username = SessionManager.TESTER_LOGIN_NAME;
            List<QueryGridViewModel> data = new List<QueryGridViewModel>();
            int totalRecords = default(int);
            int recFilter = default(int);
            //Assign values in local variables
            #region Variables
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
            var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
            string colDir = Request.Form.GetValues("order[0][dir]")[0];
            string QueryNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string DbNameSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string TypeSearch = Request.Form.GetValues("columns[3][search][value]")[0];

            #endregion
            try
            {
                //Get data from List Connection Object
                #region Getdata
                data = query_repo.GetQueryList();
                #endregion

                //Check Variables Value 
                #region CheckValues               
                if (!string.IsNullOrEmpty(QueryNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.QueryName) && x.QueryName.ToLower().Trim().Contains(QueryNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(TypeSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.ConnectionTypeString) && p.ConnectionTypeString.ToString().ToLower().Contains(TypeSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(DbNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ConnectionName) && x.ConnectionName.ToLower().Trim().Contains(DbNameSearch.ToLower().Trim())).ToList();
                }

                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "QueryName":
                            data = data.OrderByDescending(a => a.QueryName).ToList();
                            break;
                        case "ConnectionName":
                            data = data.OrderByDescending(a => a.ConnectionName).ToList();
                            break;
                        case "ConnectionTypeString":
                            data = data.OrderByDescending(a => a.ConnectionTypeString).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.QueryName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "QueryName":
                            data = data.OrderBy(a => a.QueryName).ToList();
                            break;
                        case "ConnectionName":
                            data = data.OrderBy(a => a.ConnectionName).ToList();
                            break;
                        case "ConnectionTypeString":
                            data = data.OrderBy(a => a.ConnectionTypeString).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.ConnectionName).ToList();
                            break;
                    }
                }
                #endregion

                //Get Total Records
                totalRecords = data.Count();

                //Apply Search
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    data = data.Where(p => p.ConnectionName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ConnectionTypeString.ToString().ToLower().Contains(search.ToLower()) ||
                    p.QueryName.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }

                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
                logger.Info(string.Format("Query list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Query list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Query for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            //Return Result in Json Formate
            return Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        //Adds/Edits the data
        [HttpPost]
        public JsonResult AddEditQuery(QueryGridViewModel queryModel)
        {
            logger.Info(string.Format("Query Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _queryRepository = new QueryRepository();
                bool _addeditResult = false;
                _queryRepository.Username = SessionManager.TESTER_LOGIN_NAME;
                int[] required_cols = { 2, 3 };
                /* start: code for checking if query is valid by executing it */
                //ChartHelper helper = new ChartHelper();
                //DatabaseConnectionRepository connectionRepository = new DatabaseConnectionRepository();
                //var connectionViewModel = connectionRepository.GetConnectionById(queryModel.ConnectionId);

                ////to fetch the connection string
                //string sConnString = helper.GetConnectionString(connectionViewModel);

                //var result = _queryRepository.ExecuteQuery(sConnString, queryModel.QueryDescription, (short)connectionViewModel.ConnectionType);

                /* end */
                //if (result != null && _queryRepository.CheckColumnCount(queryModel.QueryDescription, required_cols))
                if (_queryRepository.CheckColumnCount(queryModel.QueryDescription, required_cols))
                {
                    _addeditResult = _queryRepository.AddEditQuery(queryModel);
                    resultModel.message = "Saved Query [" + queryModel.QueryName + "].";
                    resultModel.status = 1;

                }
                else
                {
                    resultModel.message = "Number of columns in a query should be 2 or 3";
                    resultModel.status = 0;

                }
                resultModel.data = _addeditResult;
               
                logger.Info(string.Format("Query Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Query Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for AddEditQuery method | Query Id : {0} | UserName: {1}", queryModel.QueryId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Query for AddEditQuery method | Query Id : {0} | UserName: {1}", queryModel.QueryId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for AddEditQuery method | Query Id : {0} | UserName: {1}", queryModel.QueryId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Check Connection name already exist or not
        public JsonResult CheckDuplicateQueryNameExist(string queryName, long? queryId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                queryName = queryName.Trim();
                var _queryRepository = new QueryRepository();
                _queryRepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = _queryRepository.CheckDuplicateQueryNameExist(queryName, queryId);
                resultModel.status = 1;
                resultModel.message = "success";
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for CheckDuplicateQueryNameExist method | Query Id : {0} | Query Name : {1} | UserName: {2}", queryId, queryName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Query for CheckDuplicateQueryNameExist method | Query Id : {0} | Query Name : {1} | UserName: {2}", queryId, queryName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for CheckDuplicateQueryNameExist method | Query Id : {0} | Query Name : {1} | UserName: {2}", queryId, queryName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Deletes the data on the queryId
        [HttpPost]
        public ActionResult DeleteQuery(long QueryId)
        {
            logger.Info(string.Format("Query Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _queryRepository = new QueryRepository();
                _queryRepository.Username = SessionManager.TESTER_LOGIN_NAME;

                var lflag = _queryRepository.CheckQueryExist(QueryId);

                if (lflag)
                {
                    var queryName = _queryRepository.GetQueryNameById(QueryId);
                    var _deleteResult = _queryRepository.DeleteQuery(QueryId);
                    resultModel.data = "success";
                    resultModel.message = "Query[" + queryName + "] has been deleted.";
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.data = lflag;
                    resultModel.status = 1;
                }
                logger.Info(string.Format("Query Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Query Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for DeleteQuery method | Query Id : {0} | UserName: {1}", QueryId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Query for DeleteQuery method | Query Id : {0} | UserName: {1}", QueryId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for DeleteQuery method | Query Id : {0} | UserName: {1}", QueryId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadQueryNameByConnection(string ConnId)
        {
            logger.Info(string.Format("Load query names start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var query_repo = new QueryRepository();
            query_repo.Username = SessionManager.TESTER_LOGIN_NAME;
            ResultModel resultModel = new ResultModel();
            try
            {
                List<QueryNameViewModel> queryNames = null;
                if (!string.IsNullOrEmpty(ConnId))
                {
                    long cid = Int64.Parse(ConnId);
                    queryNames = query_repo.GetQueryNameByConnection(cid);
                }
                logger.Info(string.Format("Load query names end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Load query names successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                resultModel.data = queryNames;
                resultModel.status = 1;
            }

            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
                logger.Error(string.Format("Error occured in Query for LoadQueryNameByConnection method | Connection Id : {0} | UserName: {1}", ConnId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Query for LoadQueryNameByConnection method | Connection Id : {0} | UserName: {1}", ConnId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for LoadQueryNameByConnection method | Connection Id : {0} | UserName: {1}", ConnId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }


    }
}
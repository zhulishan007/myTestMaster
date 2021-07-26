using MARS_Repository.Repositories;
using MARS_Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MARS_Repository.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using System.Configuration;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class ChartController : Controller
    {

        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        // GET: Chart
        [HttpGet]
        public ActionResult ChartIndex()
        {
            AccountRepository accrepo = new AccountRepository();
            var _etlrepository = new EntitlementRepository();
            _etlrepository.Username = SessionManager.TESTER_LOGIN_NAME;
            var activePinList = accrepo.ActivePinListByUserId((long)SessionManager.TESTER_ID);
            ViewBag.activePinList = JsonConvert.SerializeObject(activePinList);
            var userid = SessionManager.TESTER_ID;
            var repacc = new ConfigurationGridRepository();
            repacc.Username = SessionManager.TESTER_LOGIN_NAME;
            var gridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
            var Rgriddata = GridHelper.GetLeftpanelgridwidth(gridlst);

            ViewBag.LeftPanelwidth = Rgriddata.Resize == null ? ConfigurationManager.AppSettings["DefultLeftPanel"] : Rgriddata.Resize.Trim();

            Session["PrivilegeList"] = _etlrepository.GetRolePrivilege((long)SessionManager.TESTER_ID);
            Session["RoleList"] = _etlrepository.GetRoleByUser((long)SessionManager.TESTER_ID);
            return View();
        }

        //Get QueryList tab. 
        public ActionResult ShowChart()
        {
            try
            {
                logger.Info(string.Format("Show chart open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));

                var userid = SessionManager.TESTER_ID;

                //to load databaseNames in the dropdown
                var db_repo = new DatabaseConnectionRepository();
                var dbNames = db_repo.GetDatabaseNames();
                ViewBag.databaseNameList = dbNames;

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for ShowChart method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Chart for ShowChart method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for ShowChart method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("ShowChart");
        }
        //displays the chart based on the input recieved
        [HttpPost]
        public JsonResult DisplayChart(string ConnId, string QueryId, string ChartType)
        {
            logger.Info(string.Format("Display chart open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();

            try
            {
                //to fetch the connection
                DatabaseConnectionRepository connectionRepository = new DatabaseConnectionRepository();
                ChartHelper helper = new ChartHelper();
                ChartRepository chartRepository = new ChartRepository();
                ChartViewModel chartViewModel = new ChartViewModel();

                var connectionViewModel = connectionRepository.GetConnectionById(Int64.Parse(ConnId));

                //to fetch the connection string
                string sConnString = helper.GetConnectionString(connectionViewModel);


                //to fetch the query to be executed.
                QueryRepository queryRepository = new QueryRepository();

                var query = queryRepository.GetQueryById(Int64.Parse(QueryId));

                //check no of parameters for the query
                if (helper.getParameterCount(ChartType, query.QueryDescription))
                {

                    //execute query and return dataset
                    var res = queryRepository.ExecuteQuery(sConnString, query.QueryDescription, short.Parse(connectionViewModel.ConnectionType.ToString()));
                    if (helper.IsIntegerColumnPresent(res) >= 0)
                    {
                        string chartName = "";
                        string str_json = "";
                        AxisDataViewModel axisDataModel = chartRepository.AxisDataExists(Int64.Parse(QueryId), Int16.Parse(ChartType));
                        int x_index =-1, y_index =-1, z_index = -1;
                        if (axisDataModel != null)
                        {
                            x_index = (int)axisDataModel.xAxis;
                            if (ChartType != "1")
                            {
                                y_index = (int)axisDataModel.yAxis;
                            }
                            z_index = (int)axisDataModel.zAxis;
                        }
                        else
                        {
                            z_index = helper.IsIntegerColumnPresent(res);
                           int i = 0;
                            if(i == z_index)
                            {
                                i++;
                            }
                            x_index = i;
                            i += 1;
                            if (i == z_index)
                            {
                                i++;
                            }
                            y_index = i;
                        }
                        if (ChartType == "1")
                        {
                            str_json = chartRepository.CreatePieChart(res, x_index, z_index);
                            chartName = "PieChart";
                        }
                        else if (ChartType == "2")
                        {
                            str_json = chartRepository.CreateBarChart(res, x_index, y_index, z_index);
                            chartName = "BarChart";
                        }
                        else
                        {
                            chartViewModel = chartRepository.CreateThreeDChart(res, x_index, y_index, z_index);
                            str_json = JsonConvert.SerializeObject(chartViewModel, Formatting.Indented);
                            chartName = "ThreeDChart";
                        }
                        ViewBag.axis = str_json;
                        resultModel.data = ConvertViewToString(chartName, null);
                        resultModel.status = 1;
                    }
                    else
                    {
                        resultModel.status = 0;
                        resultModel.message = "Atleast 1 column should return a numeric datatype.";
                    }
                }
                else
                {
                    resultModel.status = 0;
                    resultModel.message = "Invalid number of parameters. For Pie chart use a query with 2 parameters. \n For bar chart and 3D Chart use query with 3 parameters.";
                }
                logger.Info(string.Format("DisplayChart close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Chart displayed successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
                logger.Error(string.Format("Error occured in Chart for DisplayChart method | Connection Id : {0} | Query Id : {1} | Chart Type : {2} | UserName: {3}", ConnId, QueryId, ChartType, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Chart for DisplayChart method | Connection Id : {0} | Query Id : {1} | Chart Type : {2} | UserName: {3}", ConnId, QueryId, ChartType, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for DisplayChart method | Connection Id : {0} | Query Id : {1} | Chart Type : {2} | UserName: {3}", ConnId, QueryId, ChartType, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        private string ConvertViewToString(string viewName, object model)
        {
            StringWriter writer = null;
            try
            {
                ViewData.Model = model;
                using ( writer = new StringWriter())
                {
                    ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                    vResult.View.Render(vContext, writer);
                    
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for ConvertViewToString method | View Name : {0} | UserName: {1}", viewName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Chart for ConvertViewToString method | View Name : {0} | UserName: {1}", viewName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for ConvertViewToString method | Connection Id : {0} | View Name : {0} | UserName: {1}", viewName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return writer.ToString();
        }
        public JsonResult LoadAxisData(string ConnId, string QueryId, string ChartType)
        {
            logger.Info(string.Format("LoadAxisData  open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();

            try
            {
                //to fetch the connection
                DatabaseConnectionRepository connectionRepository = new DatabaseConnectionRepository();
                ChartHelper helper = new ChartHelper();
                ChartRepository chartRepository = new ChartRepository();
                ChartViewModel chartViewModel = new ChartViewModel();

                var connectionViewModel = connectionRepository.GetConnectionById(Int64.Parse(ConnId));

                //to fetch the connection string
                string sConnString = helper.GetConnectionString(connectionViewModel);


                //to fetch the query to be executed.
                QueryRepository queryRepository = new QueryRepository();

                var query = queryRepository.GetQueryById(Int64.Parse(QueryId));

                //check no of parameters for the query
                if (helper.getParameterCount(ChartType, query.QueryDescription))
                {
                    //get data if axis data already exists
                    AxisDataViewModel axisDataViewModel = chartRepository.AxisDataExists(Int64.Parse(QueryId), short.Parse(ChartType));

                    //execute query and return dataset
                    var res = queryRepository.ExecuteQuery(sConnString, query.QueryDescription, short.Parse(connectionViewModel.ConnectionType.ToString()));

                    //fetch data columns and put them in the view bag
                    DataTable dataTable = res.Tables[0];
                    var dataColumns = dataTable.Columns;
                    List<AxisViewModel> x_axis = new List<AxisViewModel>();
                    List<AxisViewModel> y_axis = new List<AxisViewModel>();
                    List<AxisViewModel> z_axis = new List<AxisViewModel>();
                    string[] acceptedTypes = { "Decimal", "Double", "Int16", "Int32", "Int64" };
                    for (int i = 0; i < dataColumns.Count; i++)
                    {
                        if (axisDataViewModel == null || axisDataViewModel.xAxis != i) //check if it is selected
                        {
                            x_axis.Add(new AxisViewModel { axis_id = i, axis_name = dataColumns[i].ColumnName });
                        }
                        else
                        {
                            x_axis.Add(new AxisViewModel { axis_id = i, axis_name = dataColumns[i].ColumnName, selected = true });
                        }
                        if (axisDataViewModel == null || axisDataViewModel.chartType != 1)
                        {
                            if (axisDataViewModel == null || axisDataViewModel.yAxis != i)
                            {
                                y_axis.Add(new AxisViewModel { axis_id = i, axis_name = dataColumns[i].ColumnName });
                            }
                            else
                            {
                                y_axis.Add(new AxisViewModel { axis_id = i, axis_name = dataColumns[i].ColumnName, selected = true });
                            }
                        }
                        foreach (string type in acceptedTypes)
                        {
                            if (dataColumns[i].DataType.Name == type)
                            {
                                if (axisDataViewModel == null || axisDataViewModel.zAxis != i)
                                {
                                    z_axis.Add(new AxisViewModel { axis_id = i, axis_name = dataColumns[i].ColumnName });
                                }
                                else
                                {
                                    z_axis.Add(new AxisViewModel { axis_id = i, axis_name = dataColumns[i].ColumnName, selected = true });
                                }
                            }
                        }
                    }

                    resultModel.data = new
                    {
                        xaxis = x_axis,
                        yaxis = y_axis,
                        zaxis = z_axis
                    };
                    if (axisDataViewModel != null)
                    {
                        resultModel.status = (int)axisDataViewModel.axisId;
                    }
                    else
                    {
                        resultModel.status = 0;
                    }
                }
                else
                {
                    resultModel.status = -1;
                    resultModel.message = "Invalid number of parameters. For Pie chart use a query with 2 parameters. \n For bar chart and 3D Chart use query with 3 parameters.";
                }

                logger.Info(string.Format("Axis modal opened successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                resultModel.status = -1;
                resultModel.message = ex.Message.ToString();
                logger.Error(string.Format("Error occured in Chart for LoadAxisData method | Connection Id : {0} | Query Id : {1} | Chart Type : {2} | UserName: {3}", ConnId, QueryId, ChartType, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Chart for LoadAxisData method | Connection Id : {0} | Query Id : {1} | Chart Type : {2} | UserName: {3}", ConnId, QueryId, ChartType, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for LoadAxisData method | Connection Id : {0} | Query Id : {1} | Chart Type : {2} | UserName: {3}", ConnId, QueryId, ChartType, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAxisData(AxisDataViewModel axisDataViewModel)
        {
            logger.Info(string.Format("LoadAxisData  open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();

            try
            {
                ChartRepository chartRepository = new ChartRepository();
                chartRepository.Username = SessionManager.TESTER_LOGIN_NAME;
                if (axisDataViewModel.xAxis != axisDataViewModel.yAxis &&
                    axisDataViewModel.yAxis != axisDataViewModel.zAxis &&
                    axisDataViewModel.xAxis != axisDataViewModel.zAxis)
                {
                    var _addeditResult = chartRepository.SaveAxisData(axisDataViewModel);
                    resultModel.status = 1;
                    resultModel.data = _addeditResult;
                    resultModel.message = "Saved axis data";
                    logger.Info(string.Format("Axis Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                    logger.Info(string.Format("Axis Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                }
                else
                {
                    resultModel.status = 0;
                    resultModel.message = "Please fill unique data for all the axis";
                }
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
                logger.Error(string.Format("Error occured in Chart for SaveAxisData method | Axis Id : {0} | UserName: {1}", axisDataViewModel.axisId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Chart for SaveAxisData method | Axis Id : {0} | UserName: {1}", axisDataViewModel.axisId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for SaveAxisData method | Axis Id : {0} | UserName: {1}", axisDataViewModel.axisId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);

            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
}
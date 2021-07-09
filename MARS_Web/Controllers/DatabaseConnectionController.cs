using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    [SessionTimeout]
    public class DatabaseConnectionController : Controller
    {
        public DatabaseConnectionController()
        {
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
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


        public ActionResult DatabaseConnection()
        {
            return View();
        }

        public ActionResult DatabaseList()
        {
            try
            {
                var userid = SessionManager.TESTER_ID;
                var repacc = new ConfigurationGridRepository();
                repacc.Username = SessionManager.TESTER_LOGIN_NAME;
                var gridlst = repacc.GetGridList((long)userid, GridNameList.ApplicationList);
                
            
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for DatabaseList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for DatabaseList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for DatabaseList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return PartialView("DatabaseList");
        }

        //Add/Update Connection objects values
        [HttpPost]
        public JsonResult AddEditConnection(DatabaseConnectionViewModel connectionviewmodel)
        {
            logger.Info(string.Format("Connection Add/Edit  Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _connrepository = new DatabaseConnectionRepository();
                _connrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                connectionviewmodel.CreatedBy = SessionManager.TESTER_LOGIN_NAME;
                connectionviewmodel.CreatedOn = DateTime.Now;
                connectionviewmodel.ModifiedBy = "";
                //                connectionviewmodel.ModifiedOn = "";

                var _addeditResult = _connrepository.AddEditConnection(connectionviewmodel);
                var _treerepository = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;
                // Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                resultModel.message = "Saved Connection [" + connectionviewmodel.ConnectionName + "].";
                resultModel.data = _addeditResult;
                resultModel.status = 1;

                logger.Info(string.Format("Connection Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Connection Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for AddEditConnection method | Connection Id : {0} | UserName: {1}", connectionviewmodel.ConnectionId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for AddEditConnection method | Connection Id : {0} | UserName: {1}", connectionviewmodel.ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for AddEditConnection method | Connection Id : {0} | UserName: {1}", connectionviewmodel.ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //Delete the Connection object data by ConnectionID
        public ActionResult DeleteConnection(long ConnectionId)
        {
            logger.Info(string.Format("Connection Delete start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            try
            {
                var _connrepository = new DatabaseConnectionRepository();
                var _treerepository = new GetTreeRepository();
                _connrepository.Username = SessionManager.TESTER_LOGIN_NAME;

                var lflag = _connrepository.CheckConnectionExist(ConnectionId);

                if (lflag)
                {
                    var Connectionname = _connrepository.GetConnectionnNameById(ConnectionId);
                    var _deleteResult = _connrepository.DeleteConnection(ConnectionId);
                    var lSchema = SessionManager.Schema;
                    var lConnectionStr = SessionManager.APP;
                    //  Session["LeftProjectList"] = _treerepository.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);

                    resultModel.data = "success";
                    resultModel.message = "Connection[" + Connectionname + "] has been deleted.";
                    resultModel.status = 1;
                }
                else
                {
                    resultModel.data = lflag;
                    resultModel.status = 1;
                }
                logger.Info(string.Format("Connection Delete end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Connection Delete successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {

                logger.Error(string.Format("Error occured in Database Connection for DeleteConnection method | Connection Id : {0} | UserName: {1}", ConnectionId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for DeleteConnection method | Connection Id : {0} | UserName: {1}", ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for DeleteConnection method | Connection Id : {0} | UserName: {1}", ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        //This method will load all the data and filter them
        [HttpPost]
        public JsonResult DataLoad()
        {
            //Get Repository
            logger.Info(string.Format("Connection list open start | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            var _connrepository = new DatabaseConnectionRepository();
            _connrepository.Username = SessionManager.TESTER_LOGIN_NAME;
            List<DatabaseConnectionViewModel> data = new List<DatabaseConnectionViewModel>();
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
            string NameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
            string TypeSearch = Request.Form.GetValues("columns[1][search][value]")[0];
            string HostSearch = Request.Form.GetValues("columns[2][search][value]")[0];
            string PortSearch = Request.Form.GetValues("columns[3][search][value]")[0];
            string ProtocolSearch = Request.Form.GetValues("columns[4][search][value]")[0];
            string ServiceSearch = Request.Form.GetValues("columns[5][search][value]")[0];
            string SidSearch = Request.Form.GetValues("columns[6][search][value]")[0];
            string UserSearch = Request.Form.GetValues("columns[7][search][value]")[0];
            #endregion
            try
            {
                //Get data from List Connection Object
                #region Getdata
                data = _connrepository.GetConnectionList();
                #endregion

                //Check Variables Value 
                #region CheckValues               
                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ConnectionName) && x.ConnectionName.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(TypeSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.ConnectionTypeString) && p.ConnectionTypeString.ToString().ToLower().Contains(TypeSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(HostSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Host) && x.Host.ToLower().Trim().Contains(HostSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(PortSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Port.ToString()) && x.Port.ToString().ToLower().Trim().Contains(PortSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(ProtocolSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Protocol) && x.Protocol.ToLower().Trim().Contains(ProtocolSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(ServiceSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.ServiceName) && x.ServiceName.ToLower().Trim().Contains(ServiceSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(SidSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Sid) && x.Sid.ToLower().Trim().Contains(SidSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(UserSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.UserId) && x.UserId.ToLower().Trim().Contains(UserSearch.ToLower().Trim())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderByDescending(a => a.ConnectionName).ToList();
                            break;
                        case "Type":
                            data = data.OrderByDescending(a => a.ConnectionTypeString).ToList();
                            break;
                        case "Host":
                            data = data.OrderByDescending(a => a.Host).ToList();
                            break;
                        case "Port":
                            data = data.OrderByDescending(a => a.Port).ToList();
                            break;
                        case "Protocol":
                            data = data.OrderByDescending(a => a.Protocol).ToList();
                            break;
                        case "ServiceName":
                            data = data.OrderByDescending(a => a.ServiceName).ToList();
                            break;
                        case "Sid":
                            data = data.OrderByDescending(a => a.Sid).ToList();
                            break;
                        case "UserId":
                            data = data.OrderByDescending(a => a.UserId).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.ConnectionName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "Name":
                            data = data.OrderBy(a => a.ConnectionName).ToList();
                            break;
                        case "Type":
                            data = data.OrderBy(a => a.ConnectionTypeString).ToList();
                            break;
                        case "Host":
                            data = data.OrderBy(a => a.Host).ToList();
                            break;
                        case "Port":
                            data = data.OrderBy(a => a.Port).ToList();
                            break;
                        case "Protocol":
                            data = data.OrderBy(a => a.Protocol).ToList();
                            break;
                        case "ServiceName":
                            data = data.OrderBy(a => a.ServiceName).ToList();
                            break;
                        case "Sid":
                            data = data.OrderBy(a => a.Sid).ToList();
                            break;
                        case "UserId":
                            data = data.OrderBy(a => a.UserId).ToList();
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
                    p.Host.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Port.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Protocol.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ServiceName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Sid.ToString().ToLower().Contains(search.ToLower()) ||
                    p.UserId.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }

                recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();
                logger.Info(string.Format("Connection list open end | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Connection list open successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
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
        #region Checks whether the Connection already exists in the system or not
        //Check Connection name already exist or not
        public JsonResult CheckDuplicateConnectionNameExist(string connectionName, long? connectionId)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                connectionName = connectionName.Trim();
                var _connrepository = new DatabaseConnectionRepository();
                _connrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                var result = _connrepository.CheckDuplicateConnectionNameExist(connectionName, connectionId);
                resultModel.status = 1;
                resultModel.message = "success";
                resultModel.data = result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for CheckDuplicateConnectionNameExist method | ConnectionId : {0} | Connection Name : {1} | UserName: {2}", connectionId, connectionName, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for CheckDuplicateConnectionNameExist method | ConnectionId : {0} | Connection Name : {1} | UserName: {2}", connectionId, connectionName, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for CheckDuplicateConnectionNameExist method | ConnectionId : {0} | Connection Name : {1} | UserName: {2}", connectionId, connectionName, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Test Connection

        public JsonResult TestConnection(DatabaseConnectionViewModel connectionviewmodel)
        {
            logger.Info(string.Format("Connection Test Modal open | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            ResultModel resultModel = new ResultModel();
            string sConnString = "";
            try
            {
                
                var _connrepository = new DatabaseConnectionRepository();
                _connrepository.Username = SessionManager.TESTER_LOGIN_NAME;
                bool testSucess = false;
                connectionviewmodel.Port = connectionviewmodel.Port == null ? 0 : connectionviewmodel.Port;

                if (connectionviewmodel.ConnectionType == 1)
                {
                    sConnString = new OracleConnectionStringBuilder(connectionviewmodel.Host, int.Parse(connectionviewmodel.Port.ToString()), connectionviewmodel.ServiceName, connectionviewmodel.UserId, connectionviewmodel.Password, "", true).Create();
                  
                }
                if (connectionviewmodel.ConnectionType == 2)
                {
                    sConnString = new SQLServerConnectionStringBuilder(connectionviewmodel.Host, int.Parse(connectionviewmodel.Port.ToString()), connectionviewmodel.ServiceName, connectionviewmodel.UserId, connectionviewmodel.Password, connectionviewmodel.Sid, "", true).Create();
                    //sConnString = "Data Source=13.90.224.87\\MSSQL2;Initial Catalog=new_lotools;Persist Security Info=True;User ID=sa;Password=Admin123123;Connect Timeout=36000";
                    //sConnString = "Data Source=CS-LC-17;Initial Catalog=practice;Integrated Security=True";
                }
                if (connectionviewmodel.ConnectionType == 3)
                {
                    sConnString = new SybaseConnectionStringBuilder(connectionviewmodel.Host, int.Parse(connectionviewmodel.Port.ToString()), connectionviewmodel.ServiceName, connectionviewmodel.UserId, connectionviewmodel.Password, connectionviewmodel.Sid, "", true).Create();
                    //sConnString = "Server=CS-LC-17;Port=5000;Database=charts_mars;Uid=sa;Pwd=adminadmin;";
                }
                testSucess = _connrepository.TestConnection(sConnString, connectionviewmodel);
                connectionviewmodel.IsTested = true;
                connectionviewmodel.LastTested = DateTime.Now;
                if (connectionviewmodel.ConnectionId != 0)
                {
                    _connrepository.UpdateTestConnection(connectionviewmodel);
                }
                resultModel.message = testSucess ? "Connection Tested." : "Connection Failed";
                resultModel.data = testSucess;
                resultModel.status = testSucess ? 1 : 0;

                logger.Info(string.Format("Connection Add/Edit  Modal close | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
                logger.Info(string.Format("Connection Save successfully | Username: {0}", SessionManager.TESTER_LOGIN_NAME));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for TestConnection method | Connection String : {0} | UserName: {1}", sConnString, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for TestConnection method | Connection String : {0} | UserName: {1}", sConnString, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for TestConnection method | Connection String : {0} | UserName: {1}", sConnString, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }



        #endregion
    }
}
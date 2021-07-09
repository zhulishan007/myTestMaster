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
    [SessionTimeout]
    public class DBConnectionController : Controller
    {
        public DBConnectionController()
        {
            // GetConectionString();
            DBEntities.ConnectionString = SessionManager.ConnectionString;
            DBEntities.Schema = SessionManager.Schema;
        }
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        // GET: DBConnection
        public ActionResult ConnectionList()
        {
            return PartialView("ConnectionList");
        }
        [HttpPost]
        public JsonResult DataLoad()
        {
            try
            {
                var repp = new AccountRepository();

                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

                var getList = repp.GetConnectionList();
                var data = GetEncodingConnList(getList);

                string UserNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string PasswordSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                string HostSearch = Request.Form.GetValues("columns[2][search][value]")[0];
                string PortSearch = Request.Form.GetValues("columns[3][search][value]")[0];
                string SchemaSearch = Request.Form.GetValues("columns[4][search][value]")[0];
                string Service_NameSearch = Request.Form.GetValues("columns[5][search][value]")[0];
                string DecodeMethodSearch = Request.Form.GetValues("columns[6][search][value]")[0];

                if (!string.IsNullOrEmpty(UserNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Trim().Contains(UserNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(PasswordSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Password) && p.Password.ToString().ToLower().Contains(PasswordSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(HostSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Host) && x.Host.ToLower().Trim().Contains(HostSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(PortSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Port) && x.Port.ToLower().Trim().Contains(PortSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(SchemaSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Schema) && x.Schema.ToLower().Trim().Contains(SchemaSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(Service_NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Service_Name) && x.Service_Name.ToLower().Trim().Contains(Service_NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DecodeMethodSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.DecodeMethod) && x.DecodeMethod.ToLower().Trim().Contains(DecodeMethodSearch.ToLower().Trim())).ToList();
                }

                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "UserName":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Password":
                            data = data.OrderByDescending(a => a.Password).ToList();
                            break;
                        case "Host":
                            data = data.OrderByDescending(a => a.Host).ToList();
                            break;
                        case "Port":
                            data = data.OrderByDescending(a => a.Port).ToList();
                            break;
                        case "Schema":
                            data = data.OrderByDescending(a => a.Schema).ToList();
                            break;
                        case "Service_Name":
                            data = data.OrderByDescending(a => a.Service_Name).ToList();
                            break;
                        case "DecodeMethod":
                            data = data.OrderByDescending(a => a.DecodeMethod).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "UserName":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Password":
                            data = data.OrderByDescending(a => a.Password).ToList();
                            break;
                        case "Host":
                            data = data.OrderByDescending(a => a.Host).ToList();
                            break;
                        case "Port":
                            data = data.OrderByDescending(a => a.Port).ToList();
                            break;
                        case "Schema":
                            data = data.OrderByDescending(a => a.Schema).ToList();
                            break;
                        case "Service_Name":
                            data = data.OrderByDescending(a => a.Service_Name).ToList();
                            break;
                        case "DecodeMethod":
                            data = data.OrderByDescending(a => a.DecodeMethod).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                    }
                }

                int totalRecords = data.Count();

                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.UserName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Password.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Host.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Port.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Schema.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Service_Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.DecodeMethod.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }

                int recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();


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
                logger.Error(string.Format("Error occured in DbConnecion for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in DbConnecion for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in DbConnecion for DataLoad method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult AddEditConnection(DBconnectionViewModel lModel)
        {
            try
            {
                var reppa = new AccountRepository();
                lModel.UserName = PasswordHelper.EncodeMethod(lModel.UserName, lModel.DecodeMethod.Trim());
                lModel.Password = PasswordHelper.EncodeMethod(lModel.Password, lModel.DecodeMethod.Trim());
                lModel.Port = PasswordHelper.EncodeMethod(lModel.Port, lModel.DecodeMethod.Trim());
                lModel.Host = PasswordHelper.EncodeMethod(lModel.Host, lModel.DecodeMethod.Trim());
                lModel.Service_Name = PasswordHelper.EncodeMethod(lModel.Service_Name, lModel.DecodeMethod.Trim());
                lModel.Schema = PasswordHelper.EncodeMethod(lModel.Schema, lModel.DecodeMethod.Trim());
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                if (lModel.connectionId > 0)
                {
                    var lResult = reppa.AddEditConnection(lModel);
                    var repTree = new GetTreeRepository();

                    Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                    return Json(lResult, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var addresult = reppa.AddEditConnection(lModel);
                    var repTree = new GetTreeRepository();

                    Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
                    return Json(addresult, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in DbConnecion for AddEditConnection method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in DbConnecion for AddEditConnection method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in DbConnecion for AddEditConnection method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
                throw ex;
            }
        }

        public JsonResult CheckDuplicateConnectionExist(string Username, string Password, long? ConnectionId)
        {
            var lresult = false;
            try
            {
                var repo = new AccountRepository();
               
                var getList = repo.GetConnectionList();
                var data = GetEncodingConnList(getList);

                if (ConnectionId != null)
                {
                    lresult = data.Any(x => x.connectionId != ConnectionId && x.UserName.ToLower().Trim() == Username.ToLower().Trim() && x.Password.ToLower().Trim() == Password.ToLower().Trim());
                }
                else
                {
                    lresult = data.Any(x => x.UserName.ToLower().Trim() == Username.ToLower().Trim() && x.Password.ToLower().Trim() == Password.ToLower().Trim());
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in DbConnecion for CheckDuplicateConnectionExist method | User : {0} | Connection Id : {1} | UserName: {2}", Username, ConnectionId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in DbConnecion for CheckDuplicateConnectionExist method | User : {0} | Connection Id : {1} | UserName: {2}", Username, ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in DbConnecion for CheckDuplicateConnectionExist method | User : {0} | Connection Id : {1} | UserName: {2}", Username, ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            //return lresult;
            return Json(lresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletConnection(long ConnectionId)
        {
            var result = false;
            try
            {
                var repo = new AccountRepository();
                result = repo.DeletConnection(ConnectionId);
                var repTree = new GetTreeRepository();
                var lSchema = SessionManager.Schema;
                var lConnectionStr = SessionManager.APP;

                Session["LeftProjectList"] = repTree.GetProjectList(SessionManager.TESTER_ID, lSchema, lConnectionStr);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in DbConnecion for DeletConnection method | Connection Id {0} | UserName: {1}", ConnectionId, SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in DbConnecion for DeletConnection method | Connection Id {0} | UserName: {1}", ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in DbConnecion for DeletConnection method | Connection Id {0} | UserName: {1}", ConnectionId, SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<DBconnectionViewModel> GetEncodingConnList(List<T_DBCONNECTION> dBconnections)
        {
            var data = new List<DBconnectionViewModel>();
            try
            {
                foreach (var item in dBconnections)
                {
                    DBconnectionViewModel db = new DBconnectionViewModel();
                    db.connectionId = item.DBCONNECTION_ID;
                    db.DecodeMethod = item.DECODE_METHOD;
                    db.Databasename = item.DATABASENAME;
                    db.Host = PasswordHelper.DecodeMethod(item.HOST, item.DECODE_METHOD.Trim());
                    db.Port = PasswordHelper.DecodeMethod(item.PORT, item.DECODE_METHOD.Trim());
                    db.UserName = PasswordHelper.DecodeMethod(item.USERNAME, item.DECODE_METHOD.Trim());
                    db.Password = PasswordHelper.DecodeMethod(item.PASSWORD, item.DECODE_METHOD.Trim());
                    db.Service_Name = PasswordHelper.DecodeMethod(item.SERVICENAME, item.DECODE_METHOD.Trim());
                    db.Schema = PasswordHelper.DecodeMethod(item.SCHEMA, item.DECODE_METHOD.Trim());
                    data.Add(db);
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in DbConnecion for GetEncodingConnList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME));
                ELogger.ErrorException(string.Format("Error occured in DbConnecion for GetEncodingConnList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in DbConnecion for GetEncodingConnList method | UserName: {0}", SessionManager.TESTER_LOGIN_NAME), ex.InnerException);
            }
            return data;
        }
    }
}
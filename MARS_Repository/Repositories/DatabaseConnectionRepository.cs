using MARS_Repository.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARS_Repository.ViewModel;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.CData.Sybase;
using System.Data;

namespace MARS_Repository.Repositories
{
    public class DatabaseConnectionRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;
        public bool AddEditConnection(DatabaseConnectionViewModel ConnModelEntity, string lConnectionStr, string lSchema)
        {
            try
            {

                if (!string.IsNullOrEmpty(ConnModelEntity.ConnectionName))
                {
                    ConnModelEntity.ConnectionName = ConnModelEntity.ConnectionName.Trim();
                }
                var flag = false;
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                lconnection.Open();

                if (ConnModelEntity.ConnectionId == 0)
                {
                    logger.Info(string.Format("Add database connection start | Database Connection: {0} | Username: {1}", ConnModelEntity.ConnectionName, Username));
                    #region using table T_DATABASE_CONNECTIONS
                    //var RegisterTbl = new T_DATABASE_CONNECTIONS();
                    //RegisterTbl.CONNECTION_ID = Helper.NextTestSuiteId("SEQ_T_DBCONNECTION");
                    //RegisterTbl.CONNECTION_NAME = ConnModelEntity.ConnectionName;
                    //RegisterTbl.CONNECTION_TYPE = ConnModelEntity.ConnectionType;
                    //RegisterTbl.HOST_NAME = ConnModelEntity.Host;
                    //RegisterTbl.PORT_NUMBER = ConnModelEntity.Port;
                    //RegisterTbl.PROTOCOL = ConnModelEntity.Protocol;
                    //RegisterTbl.SERVICE_NAME = ConnModelEntity.ServiceName;
                    //RegisterTbl.DB_SID = ConnModelEntity.Sid;
                    //RegisterTbl.DB_USERNAME = ConnModelEntity.UserId;
                    //RegisterTbl.DB_PASSWORD = ConnModelEntity.Password;
                    //RegisterTbl.CREATION_DATE = DateTime.Now;
                    //RegisterTbl.CREATEDBY = Username;
                    //RegisterTbl.ACTIVE = ConnModelEntity.IsActive;
                    //RegisterTbl.MODIFIEDBY = Username;
                    //RegisterTbl.MODIFIED_DATE = DateTime.Now; ;
                    //RegisterTbl.CONNECTION_STRING = "";
                    //RegisterTbl.IS_TESTED = 0;
                    //ConnModelEntity.ConnectionId = RegisterTbl.CONNECTION_ID;
                    //enty.T_DATABASE_CONNECTIONS.Add(RegisterTbl);
                    //enty.SaveChanges();
                    #endregion
                    long createId = Helper.NextTestSuiteId("SEQ_T_DBCONNECTION");
                    ltransaction = lconnection.BeginTransaction();
                    string lcmdquery = "INSERT INTO " + lSchema + ".T_DBCONNECTION(DBCONNECTION_ID, DATABASENAME, USERNAME, PASSWORD, PORT, HOST, SERVICENAME, SCHEMA, DECODE_METHOD, DATABASE_VALUE) values(:1,:2,null,null,null,null,null,null,null, utl_raw.cast_to_raw(:3))";
                    using (var lcmd = lconnection.CreateCommand())
                    {
                        lcmd.CommandText = lcmdquery;

                        OracleParameter DBCONNECTION_ID_oparam = new OracleParameter();
                        DBCONNECTION_ID_oparam.OracleDbType = OracleDbType.Long;
                        DBCONNECTION_ID_oparam.Value = createId;

                        OracleParameter DATABASENAME_oparam = new OracleParameter();
                        DATABASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                        DATABASENAME_oparam.Value = ConnModelEntity.ConnectionName;

                        OracleParameter DATABASE_VALUE_oparam = new OracleParameter();
                        DATABASE_VALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                        DATABASE_VALUE_oparam.Value = ConnModelEntity.DatabaseValueJson;

                        lcmd.Parameters.Add(DBCONNECTION_ID_oparam);
                        lcmd.Parameters.Add(DATABASENAME_oparam);
                        lcmd.Parameters.Add(DATABASE_VALUE_oparam);

                        lcmd.ExecuteNonQuery();
                        ltransaction.Commit();
                    }

                    flag = true;
                    logger.Info(string.Format("Add  database connection end | Database Connection: {0} | Username: {1}", ConnModelEntity.ConnectionName, Username));

                }
                else
                {
                    logger.Info(string.Format("Edit connection start | Database Connection: {0} | ConnectionId: {1} | Username: {2}", ConnModelEntity.ConnectionName, ConnModelEntity.ConnectionId, Username));
                    #region #region using table T_DATABASE_CONNECTIONS
                    //var RegisterTbl = enty.T_DATABASE_CONNECTIONS.Find(ConnModelEntity.connectionId);
                    //if (RegisterTbl != null)
                    //{
                    //    RegisterTbl.CONNECTION_NAME = ConnModelEntity.ConnectionName;
                    //    RegisterTbl.CONNECTION_TYPE = ConnModelEntity.ConnectionType;
                    //    RegisterTbl.HOST_NAME = ConnModelEntity.Host;
                    //    RegisterTbl.PORT_NUMBER = ConnModelEntity.Port;
                    //    RegisterTbl.PROTOCOL = ConnModelEntity.Protocol;
                    //    RegisterTbl.SERVICE_NAME = ConnModelEntity.ServiceName;
                    //    RegisterTbl.DB_SID = ConnModelEntity.Sid;
                    //    RegisterTbl.DB_USERNAME = ConnModelEntity.UserId;
                    //    RegisterTbl.DB_PASSWORD = ConnModelEntity.Password;
                    //    RegisterTbl.CREATEDBY = Username;
                    //    RegisterTbl.CREATION_DATE = DateTime.Now;
                    //    RegisterTbl.MODIFIEDBY = Username;
                    //    RegisterTbl.MODIFIED_DATE = DateTime.Now;
                    //    RegisterTbl.CONNECTION_STRING = "";
                    //    RegisterTbl.ACTIVE = ConnModelEntity.IsActive;
                    //    enty.SaveChanges();
                    //}
                    #endregion
                    ltransaction = lconnection.BeginTransaction();
                    string lcmdquery = "UPDATE " + lSchema + ".T_DBCONNECTION SET DATABASENAME = :1, DATABASE_VALUE = utl_raw.cast_to_raw(:2) WHERE DBCONNECTION_ID = :3";
                    using (var lcmd = lconnection.CreateCommand())
                    {
                        lcmd.CommandText = lcmdquery;

                        OracleParameter DATABASENAME_oparam = new OracleParameter();
                        DATABASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                        DATABASENAME_oparam.Value = ConnModelEntity.ConnectionName;


                        OracleParameter DATABASE_VALUE_oparam = new OracleParameter();
                        DATABASE_VALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                        DATABASE_VALUE_oparam.Value = ConnModelEntity.DatabaseValueJson.ToString();

                        OracleParameter DBCONNECTION_ID_oparam = new OracleParameter();
                        DBCONNECTION_ID_oparam.OracleDbType = OracleDbType.Long;
                        DBCONNECTION_ID_oparam.Value = ConnModelEntity.ConnectionId;


                        lcmd.Parameters.Add(DATABASENAME_oparam);
                        lcmd.Parameters.Add(DATABASE_VALUE_oparam);
                        lcmd.Parameters.Add(DBCONNECTION_ID_oparam);
                        lcmd.ExecuteNonQuery();
                        ltransaction.Commit();
                    }
                    flag = true;
                    logger.Info(string.Format("Edit database connection end | Database Connection: {0} | ConnectionId: {1} | Username: {2}", ConnModelEntity.ConnectionName, ConnModelEntity.ConnectionId, Username));
                }
                lconnection.Close();
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for AddEditConnection method | Connection id :{0} | Connection Name: {1} | UserName: {0}", ConnModelEntity.ConnectionId, ConnModelEntity.ConnectionName, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for AddEditConnection method | Connection id :{0} | Connection Name: {1} | UserName: {0}", ConnModelEntity.ConnectionId, ConnModelEntity.ConnectionName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for AddEditConnection method | Connection id :{0} | Connection Name: {1} | UserName: {0}", ConnModelEntity.ConnectionId, ConnModelEntity.ConnectionName, Username), ex.InnerException);
                throw;
            }
        }

        #region fetchConnection data

        public List<DatabaseConnectionViewModel> GetDBConnectionList(string lconstring, string schema)
        {
            try
            {
                logger.Info(string.Format("GetDBConnectionList start | Username: {0}", Username));
                DataSet lds = new DataSet();
                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[1];
                ladd_refer_image[0] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[0].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_DBCONNECTION";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DatabaseConnectionViewModel> resultList = dt.AsEnumerable().Select(row =>
                    new DatabaseConnectionViewModel
                    {
                        ConnectionId = row.Field<long>("DBCONNECTION_ID"),
                        ConnectionName = row.Field<string>("DATABASENAME"),
                        DatabaseValueJson = row.Field<byte[]>("BLOBValuestr") == null ? "" : Encoding.UTF8.GetString(row.Field<byte[]>("BLOBValuestr")),
                    }).ToList();

                logger.Info(string.Format("GetDBConnectionList end | Username: {0}", Username));
                return resultList.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Account in GetDBConnectionList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Account in GetDBConnectionList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Account in GetDBConnectionList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public DatabaseConnectionViewModel GetDBConnectionListById(long connid, string lconstring, string schema)
        {
            try
            {
                logger.Info(string.Format("GetDBConnectionListById start | Username: {0}", Username));
                DataSet lds = new DataSet();
                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[1];
                ladd_refer_image[0] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[0].Direction = ParameterDirection.Output;

                OracleParameter CONNID_oparam = new OracleParameter("CONNECTION_ID", OracleDbType.Int16);
                CONNID_oparam.Value = connid;
                CONNID_oparam.Direction = ParameterDirection.Input;

                lcmd.Parameters.Add(CONNID_oparam);
                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }
                
                lcmd.CommandText = schema + "." + "SP_GET_DBCONNECTIONBYID";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                DatabaseConnectionViewModel resultList = dt.AsEnumerable().Select(row =>
                    new DatabaseConnectionViewModel
                    {
                        ConnectionId = row.Field<long>("DBCONNECTION_ID"),
                        ConnectionName = row.Field<string>("DATABASENAME"),
                        DatabaseValueJson = row.Field<byte[]>("BLOBValuestr") == null ? "" : Encoding.UTF8.GetString(row.Field<byte[]>("BLOBValuestr")),
                    }).FirstOrDefault();

                logger.Info(string.Format("GetDBConnectionListById end | Username: {0}", Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Account in GetDBConnectionList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Account in GetDBConnectionList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Account in GetDBConnectionList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public String GetConnectionnNameById(long ConnectionId)
        {
            try
            {
                logger.Info(string.Format("Get ConnectionName start | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                var result = enty.T_DBCONNECTION.FirstOrDefault(x => x.DBCONNECTION_ID == ConnectionId).DATABASENAME;
                logger.Info(string.Format("Get ConnectionName end | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for GetConnectionnNameById method | Connection Id : {0} | UserName: {1}", ConnectionId, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for GetConnectionnNameById method | Connection Id : {0} | UserName: {1}", ConnectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for GetConnectionnNameById method | Connection Id : {0} | UserName: {1}", ConnectionId, Username), ex.InnerException);
                throw;
            }
        }

        public List<DatabaseConnNameViewModel> GetDatabaseNames()
        {
            try
            {
                logger.Info(string.Format("List Database Connections Names start | UserName: {0}", Username));
                var connNameList = (from x in enty.T_DBCONNECTION
                                   // where x.ACTIVE == 1
                                    select new DatabaseConnNameViewModel
                                    {
                                        ConnectionId = x.DBCONNECTION_ID,
                                        ConnectionName = x.DATABASENAME
                                    }).ToList();
                logger.Info(string.Format("List Database Connections  Names end | UserName: {0}", Username));
                return connNameList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for GetDatabaseNames method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for GetDatabaseNames method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for GetDatabaseNames method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        #endregion
        public bool CheckDuplicateConnectionNameExist(string connName, long? ConnectionId)
        {
            try
            {
                var lresult = false;
                logger.Info(string.Format("Check Duplicate ConnectionName Exist start | UserName: {0}", Username));
                if (ConnectionId != null)
                {
                    lresult = enty.T_DBCONNECTION.Any(x => x.DBCONNECTION_ID != ConnectionId && x.DATABASENAME.ToLower().Trim() == connName.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_DBCONNECTION.Any(x => x.DATABASENAME.ToLower().Trim() == connName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate ConnectionName Exist end | UserName: {0}", Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for CheckDuplicateConnectionNameExist method | Connection Id : {0} | Connection Name: {1} | UserName: {2}", ConnectionId, connName, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for CheckDuplicateConnectionNameExist method | Connection Id : {0} | Connection Name: {1} | UserName: {2}", ConnectionId, connName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for CheckDuplicateConnectionNameExist method | Connection Id : {0} | Connection Name: {1} | UserName: {2}", ConnectionId, connName, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckConnectionExist(long? ConnectionId)
        {
            try
            {
                var lresult = false;
                logger.Info(string.Format("Check Connection Exist start | UserName: {0}", Username));
                if (ConnectionId != null)
                {
                    lresult = enty.T_DBCONNECTION.Any(x => x.DBCONNECTION_ID != ConnectionId);
                }

                logger.Info(string.Format("Check Connection end | UserName: {0}", Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for CheckConnectionExist method | Connection Id : {0} | UserName: {1}", ConnectionId, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for CheckConnectionExist method | Connection Id : {0} | UserName: {1}", ConnectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection CheckConnectionExist GetConnectionnNameById method | Connection Id : {0} | UserName: {1}", ConnectionId, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteConnection(long ConnectionId)
        {
            try
            {
                logger.Info(string.Format("Delete Connection start | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                var flag = false;
                var item = enty.T_DBCONNECTION.FirstOrDefault(x => x.DBCONNECTION_ID == ConnectionId);
                enty.T_DBCONNECTION.Remove(item);
                enty.SaveChanges();
                flag = true;

                logger.Info(string.Format("Delete Connection end | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for DeleteConnection method | Connection Id : {0} | UserName: {1}", ConnectionId, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for DeleteConnection method | Connection Id : {0} | UserName: {1}", ConnectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for DeleteConnection method | Connection Id : {0} | UserName: {1}", ConnectionId, Username), ex.InnerException);
                throw;
            }

        }

        public bool TestConnection(string sConnectionString, DatabaseConnectionViewModel connModel)
        {
            try
            {
                short? connectionType = connModel.ConnectionType;

                if (connectionType == 1)
                {
                    using (OracleConnection connection = new OracleConnection(sConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            connModel.ErrorMessage = "Connection Tested Successfully";
                            return true;
                        }

                        catch (OracleException ex)
                        {
                            connModel.ErrorMessage = ex.Message;
                            return false;
                        }
                        catch (Exception ex)
                        {
                            connModel.ErrorMessage = ex.Message;
                            return false;
                        }

                    }
                }
                else if (connectionType == 2)
                {
                    using (SqlConnection connection = new SqlConnection(sConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            connModel.ErrorMessage = "Connection Tested Successfully";
                            return true;
                        }

                        catch (SqlException ex)
                        {
                            connModel.ErrorMessage = ex.Message;
                            return false;
                        }
                        catch (Exception ex)
                        {
                            connModel.ErrorMessage = ex.Message;
                            return false;
                        }

                    }

                }
                else if (connectionType == 3)
                {
                    using (SybaseConnection connection = new SybaseConnection(sConnectionString))
                    {
                        try
                        {
                            connection.Open();//not returning whn connection is false
                            connModel.ErrorMessage = "Connection Tested Successfully";
                            return true;
                        }
                        catch (SybaseException ex)
                        {
                            connModel.ErrorMessage = ex.Message;
                            return false;
                        }
                        catch (Exception ex)
                        {
                            connModel.ErrorMessage = ex.Message;
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for TestConnection method | Connection string : {0} | UserName: {1}", sConnectionString, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for TestConnection method | Connection string : {0} | UserName: {1}", sConnectionString, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for TestConnection method | Connection string : {0} | UserName: {1}", sConnectionString, Username), ex.InnerException);
                throw;
            }
            return false;
        }



        //to insert test connection changes to database
        public void UpdateTestConnection(DatabaseConnectionViewModel ConnModelEntity)
        {
            try
            {
                logger.Info(string.Format("Add test connection data start | Database Connection: {0} | Username: {1}", ConnModelEntity.ConnectionName, Username));
                if (ConnModelEntity.ConnectionId != 0)
                {
                    var RegisterTbl = enty.T_DATABASE_CONNECTIONS.Find(ConnModelEntity.ConnectionId);
                    if (RegisterTbl != null)
                    {

                        RegisterTbl.IS_TESTED = Convert.ToSByte(ConnModelEntity.IsTested ? 1 : 0); ;
                        RegisterTbl.LAST_TESTED = ConnModelEntity.LastTested;
                        RegisterTbl.ERROR_MESSAGE = ConnModelEntity.ErrorMessage;
                        //enty.T_DATABASE_CONNECTIONS.Add(RegisterTbl);
                        enty.SaveChanges();
                    }
                }
                logger.Info(string.Format("Add test connection data end | ConnectionId: {0} | Username: {1}", ConnModelEntity.ConnectionName, Username));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for UpdateTestConnection method | Connection Id : {0} | UserName: {1}", ConnModelEntity.ConnectionId, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for UpdateTestConnection method | Connection Id : {0} | UserName: {1}", ConnModelEntity.ConnectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for UpdateTestConnection method | Connection Id : {0} | UserName: {1}", ConnModelEntity.ConnectionId, Username), ex.InnerException);
                throw;
            }
        }

        public DatabaseConnectionViewModel GetConnectionById(long connId)
        {
            try
            {
                logger.Info(string.Format("Get Database Connection by Id start | UserName: {0}", Username));
                var conn = enty.T_DATABASE_CONNECTIONS.FirstOrDefault(x => x.CONNECTION_ID == connId);
                DatabaseConnectionViewModel objDbConnViewModel = new DatabaseConnectionViewModel();
                objDbConnViewModel.ConnectionId = conn.CONNECTION_ID;
                objDbConnViewModel.ConnectionName = conn.CONNECTION_NAME;
                objDbConnViewModel.ConnectionType = conn.CONNECTION_TYPE;
                objDbConnViewModel.ConnectionTypeString = "";
                switch (objDbConnViewModel.ConnectionType)
                {
                    case 1:
                        objDbConnViewModel.ConnectionTypeString = "Oracle";
                        break;
                    case 2:
                        objDbConnViewModel.ConnectionTypeString = "SQL Server";
                        break;
                    case 3:
                        objDbConnViewModel.ConnectionTypeString = "SyBase";
                        break;
                }
                objDbConnViewModel.Host = conn.HOST_NAME;
                objDbConnViewModel.Port = conn.PORT_NUMBER;
                objDbConnViewModel.Protocol = conn.PROTOCOL;
                objDbConnViewModel.ServiceName = conn.SERVICE_NAME;
                objDbConnViewModel.Sid = conn.DB_SID;
                objDbConnViewModel.UserId = conn.DB_USERNAME;
                objDbConnViewModel.Password = conn.DB_PASSWORD;
                objDbConnViewModel.IsActive = conn.ACTIVE;
                return objDbConnViewModel;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Database Connection for UpdateTestConnection method | Connection Id : {0} | UserName: {1}", connId, Username));
                ELogger.ErrorException(string.Format("Error occured in Database Connection for UpdateTestConnection method | Connection Id : {0} | UserName: {1}", connId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Database Connection for UpdateTestConnection method | Connection Id : {0} | UserName: {1}", connId, Username), ex.InnerException);
                throw;
            }
        }
    }
}

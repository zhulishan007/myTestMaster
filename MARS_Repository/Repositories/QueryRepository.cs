

using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.CData.Sybase;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.Repositories
{
    public class QueryRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        //to add a new query or edit an exiting query
        public bool AddEditQuery(QueryGridViewModel queryViewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(queryViewModel.QueryName))
                {
                    queryViewModel.QueryName = queryViewModel.QueryName.Trim();
                }
                var flag = false;
                if (queryViewModel.QueryId == 0)
                {
                    logger.Info(string.Format("Add query start | Query: {0} | Username: {1}", queryViewModel.QueryName, Username));

                    var RegisterTbl = new T_QUERY();
                    RegisterTbl.QUERY_ID = Helper.NextTestSuiteId("T_QUERY_SEQ"); ;
                    RegisterTbl.QUERY_NAME = queryViewModel.QueryName;
                    RegisterTbl.QUERY_DESC = queryViewModel.QueryDescription.Replace("'", "''");
                    RegisterTbl.CREATED_DATE = DateTime.Now;
                    RegisterTbl.CREATEDBY = Username;
                    RegisterTbl.IS_ACTIVE = queryViewModel.IsActive;
                    RegisterTbl.MODIFIEDBY = Username;
                    RegisterTbl.MODIFIED_DATE = DateTime.Now;
                    RegisterTbl.CONN_ID = queryViewModel.ConnectionId;
                    RegisterTbl.IS_ACTIVE = queryViewModel.IsActive;
                    queryViewModel.QueryId = RegisterTbl.QUERY_ID;
                    enty.T_QUERY.Add(RegisterTbl);
                    enty.SaveChanges();

                    flag = true;
                    logger.Info(string.Format("Add query end | Query: {0} | Username: {1}", queryViewModel.QueryName, Username));

                }
                else
                {
                    var RegisterTbl = enty.T_QUERY.Find(queryViewModel.QueryId);
                    //var RelTbl = enty.REL_DB_QUERY.FirstOrDefault(x => x.QUERY_ID == queryViewModel.QueryId);
                    logger.Info(string.Format("Edit query start | Query: {0} | Query Id: {1} | Username: {2}", queryViewModel.QueryId, queryViewModel.QueryName, Username));
                    if (RegisterTbl != null)
                    {
                        RegisterTbl.QUERY_NAME = queryViewModel.QueryName;
                        RegisterTbl.QUERY_DESC = queryViewModel.QueryDescription.Replace("'", "''"); ;
                        RegisterTbl.MODIFIEDBY = Username;
                        RegisterTbl.MODIFIED_DATE = DateTime.Now;
                        RegisterTbl.IS_ACTIVE = queryViewModel.IsActive;
                        RegisterTbl.CONN_ID = queryViewModel.ConnectionId;
                        enty.SaveChanges();

                    }
                    flag = true;
                    logger.Info(string.Format("Edit query connection end | Query: {0} | QueryId: {1} | Username: {2}", queryViewModel.QueryName, queryViewModel.QueryId, Username));
                }
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for AddEditQuery method | Query Id : {0} | UserName: {1}", queryViewModel.QueryId, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for AddEditQuery method | Query Id : {0} | UserName: {1}", queryViewModel.QueryId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for AddEditQuery method | Query Id : {0} | UserName: {1}", queryViewModel.QueryId, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteQuery(long queryId)
        {
            try
            {
                logger.Info(string.Format("Delete Query start | QueryId: {0} | Username: {1}", queryId, Username));
                var flag = false;
                var item = enty.T_QUERY.FirstOrDefault(x => x.QUERY_ID == queryId);
                enty.T_QUERY.Remove(item);
                enty.SaveChanges();
                flag = true;

                logger.Info(string.Format("Delete Query end | QueryId: {0} | Username: {1}", queryId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for DeleteQuery method | Query Id : {0} | UserName: {1}", queryId, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for DeleteQuery method | Query Id : {0} | UserName: {1}", queryId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for DeleteQuery method | Query Id : {0} | UserName: {1}", queryId, Username), ex.InnerException);
                throw;
            }

        }

        //checks if the query exists
        public bool CheckQueryExist(long? QueryId)
        {
            try
            {
                var lresult = false;
                logger.Info(string.Format("Check Query Exist start | UserName: {0}", Username));
                if (QueryId != null)
                {
                    lresult = enty.T_QUERY.Any(x => x.QUERY_ID != QueryId);
                }

                logger.Info(string.Format("Check Query end | UserName: {0}", Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for CheckQueryExist method | Query Id : {0} | UserName: {1}", QueryId, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for CheckQueryExist method | Query Id : {0} | UserName: {1}", QueryId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for CheckQueryExist method | Query Id : {0} | UserName: {1}", QueryId, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateQueryNameExist(string queryName, long? queryId)
        {
            try
            {
                var lresult = false;
                logger.Info(string.Format("Check Duplicate QueryName Exist start | UserName: {0}", Username));
                if (queryId != null)
                {
                    lresult = enty.T_QUERY.Any(x => x.QUERY_ID != queryId && x.QUERY_NAME.ToLower().Trim() == queryName.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_QUERY.Any(x => x.QUERY_NAME.ToLower().Trim() == queryName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate QueryName Exist end | UserName: {0}", Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for CheckDuplicateQueryNameExist method | Query Id : {0} |  Query Name : {1} | UserName: {1}", queryId, queryName, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for CheckDuplicateQueryNameExist method | Query Id : {0} |  Query Name : {1} | UserName: {1}", queryId, queryName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for CheckDuplicateQueryNameExist method | Query Id : {0} |  Query Name : {1} | UserName: {1}", queryId, queryName, Username), ex.InnerException);
                throw;
            }
        }

        //get query name by id
        public String GetQueryNameById(long queryId)
        {
            try
            {
                logger.Info(string.Format("Get GetQueryNameById start | QueryId: {0} | Username: {1}", queryId, Username));
                var result = enty.T_QUERY.FirstOrDefault(x => x.QUERY_ID == queryId).QUERY_NAME;
                logger.Info(string.Format("Get GetQueryNameById end | QueryId: {0} | Username: {1}", queryId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for GetQueryNameById method | Query Id : {0} | UserName: {1}", queryId, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for GetQueryNameById method | Query Id : {0} | UserName: {1}", queryId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for GetQueryNameById method | Query Id : {0} | UserName: {1}", queryId, Username), ex.InnerException);
                throw;
            }
        }

        //get the columns of the query
        public List<QueryGridViewModel> GetQueryList()
        {
            try
            {
                logger.Info(string.Format("Get Query List start | UserName: {0}", Username));
                List<QueryGridViewModel> queryList = new List<QueryGridViewModel>();

                var query = (from q in enty.T_QUERY
                             join db in enty.T_DATABASE_CONNECTIONS on q.CONN_ID equals db.CONNECTION_ID

                             select new QueryGridViewModel
                             {
                                 QueryId = q.QUERY_ID,
                                 QueryName = q.QUERY_NAME,
                                 QueryDescription = q.QUERY_DESC,
                                 IsActive = q.IS_ACTIVE,
                                 ConnectionId = db.CONNECTION_ID,
                                 ConnectionName = db.CONNECTION_NAME,
                                 ConnectionType = db.CONNECTION_TYPE

                             }).ToList();

                foreach (var item in query)
                {
                    QueryGridViewModel queryGridViewModel = new QueryGridViewModel();
                    queryGridViewModel.QueryId = item.QueryId;
                    queryGridViewModel.QueryName = item.QueryName;
                    queryGridViewModel.QueryDescription = item.QueryDescription.Replace("''","'");
                    queryGridViewModel.IsActive = item.IsActive;
                    queryGridViewModel.ConnectionId = item.ConnectionId;
                    queryGridViewModel.ConnectionName = item.ConnectionName;
                    queryGridViewModel.ConnectionType = item.ConnectionType;

                    switch (queryGridViewModel.ConnectionType)
                    {
                        case 1:
                            queryGridViewModel.ConnectionTypeString = "Oracle";
                            break;
                        case 2:
                            queryGridViewModel.ConnectionTypeString = "SQL Server";
                            break;
                        case 3:
                            queryGridViewModel.ConnectionTypeString = "SyBase";
                            break;
                    }
                    queryList.Add(queryGridViewModel);
                }
                logger.Info(string.Format("Get Query List end | UserName: {0}", Username));

                return queryList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for GetQueryList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Query for GetQueryList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for GetQueryList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        //get query names for dropdown on connection id
        public List<QueryNameViewModel> GetQueryNameByConnection(long ConnId)
        {
            try
            {
                logger.Info(string.Format("List Query Names start | UserName: {0}", Username));
                var queryNameList = (from x in enty.T_QUERY
                                     where x.CONN_ID == ConnId
                                     where x.IS_ACTIVE == 1
                                     select new QueryNameViewModel
                                     {
                                         QueryId = x.QUERY_ID,
                                         QueryName = x.QUERY_NAME
                                     }).ToList();
                logger.Info(string.Format("List Query Names end | UserName: {0}", Username));
                return queryNameList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for GetQueryNameByConnection method | Connection Id : {0} | UserName: {1}",ConnId,  Username));
                ELogger.ErrorException(string.Format("Error occured in Query for GetQueryNameByConnection method | Connection Id : {0} | UserName: {1}", ConnId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for GetQueryNameByConnection method | Connection Id : {0} | UserName: {1}", ConnId, Username), ex.InnerException);
                throw;
            }
        }

        public List<QueryNameViewModel> GetQueryNames()
        {
            try
            {
                logger.Info(string.Format("GetQueryNamess start | UserName: {0}", Username));
                var queryNameList = (from x in enty.T_QUERY
                                     where x.IS_ACTIVE == 1
                                     select new QueryNameViewModel
                                     {
                                         QueryId = x.QUERY_ID,
                                         QueryName = x.QUERY_NAME
                                     }).ToList();
                logger.Info(string.Format("GetQueryNames end | UserName: {0}", Username));
                return queryNameList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for GetQueryNames method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Query for GetQueryNames method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for GetQueryNames method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        //to check if a query contains exactly 2 or 3 columns
        public bool CheckColumnCount(string strQuery, int[] required)
        {
            try
            {
                 strQuery = strQuery.ToLower();
                string[] separator = { "select", "from" };
                string[] strArray = strQuery.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                string[] strColumns = strArray[0].Split(',');
                //for pie chart - 2 parameters, for bar and 3d chart - 3 parameters
                int count = 0; bool flag = false;
                for (int i = 0; i < strColumns.Length; i++)
                {
                    if(flag)
                    {
                        count++;
                    }
                    if(strColumns[i].Contains("("))
                    {
                        flag = true;
                   }
                    if(strColumns[i].Contains(")"))
                    {
                        flag = false;
                    }
                    for(int j =0; j < required.Length; j++)
                    {
                        if (strColumns.Length - count == required[j])
                            return true;
                    }
                    
                   
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for CheckColumnCount method | Connection String : {0} | Required Count : {1} | UserName: {2}", strQuery, required, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for CheckColumnCount method | Connection String : {0} | Required Count : {1} | UserName: {2}", strQuery, required, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for CheckColumnCount method | Connection String : {0} | Required Count : {1} | UserName: {2}", strQuery, required, Username), ex.InnerException);
                return false;
            }
        }

        public QueryGridViewModel GetQueryById(long queryId)
        {
            try
            {
                logger.Info(string.Format("Get GetQueryById start | QueryId: {0} | Username: {1}", queryId, Username));
                var result = (from q in enty.T_QUERY
                              join db in enty.T_DATABASE_CONNECTIONS on q.CONN_ID equals db.CONNECTION_ID
                              where q.QUERY_ID == queryId
                              select new QueryGridViewModel
                              {
                                  QueryId = q.QUERY_ID,
                                  QueryName = q.QUERY_NAME,
                                  QueryDescription = q.QUERY_DESC,
                                  IsActive = q.IS_ACTIVE,
                                  ConnectionId = db.CONNECTION_ID,
                                  ConnectionName = db.CONNECTION_NAME,
                                  ConnectionType = db.CONNECTION_TYPE

                              }).FirstOrDefault();
                logger.Info(string.Format("Get GetQueryById end | QueryId: {0} | Username: {1}", queryId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for GetQueryById method | QueryId: {0} | Username: {1}", queryId, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for GetQueryById method | QueryId: {0} | Username: {1}", queryId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for GetQueryById method | QueryId: {0} | Username: {1}", queryId, Username), ex.InnerException);
                throw;
            }
        }

        public DataSet ExecuteQuery(string connString, string queryDescription, short connType)
        {
            var result = false;
            DataSet dataSet = null;
            DbDataAdapter adapter = null;
            try
            {
                
                if (connType == 1)
                {
                    using (OracleConnection connection = new OracleConnection(connString))
                    {
                        queryDescription = queryDescription.Replace("''", "'");
                        queryDescription = queryDescription.Replace(";", "");
                        //queryDescription = "q'[" + queryDescription + "]'";
                        OracleCommand command = new OracleCommand(queryDescription , connection);
                    
                        command.CommandType = System.Data.CommandType.Text;

                        connection.Open();
                        adapter = new OracleDataAdapter(command);
                        dataSet = new DataSet("chart");
                        adapter.Fill(dataSet);
                    }
                }
                else if (connType == 2)
                {
                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        SqlCommand command = new SqlCommand(queryDescription, connection);
                        command.CommandType = System.Data.CommandType.Text;

                        connection.Open();
                        adapter = new SqlDataAdapter(command);
                        dataSet = new DataSet("chart");
                        adapter.Fill(dataSet);

                    }
                }
                else if (connType == 3)
                {
                    using (SybaseConnection connection = new SybaseConnection(connString))
                    {
                        SybaseCommand command = new SybaseCommand(queryDescription, connection);
                        command.CommandType = System.Data.CommandType.Text;

                        connection.Open();
                        adapter = new SybaseDataAdapter(command);
                        dataSet = new DataSet("chart");
                        adapter.Fill(dataSet);
                    }
                }
               

            }
            catch (DbException ex)
            {
                logger.Error(string.Format("Error occured in Query for ExecuteQuery method | Connection string: {0} | Query Description : {1} | Connection Type : {2} | Username: {3}", connString, queryDescription, connType, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for ExecuteQuery method | Connection string: {0} | Query Description : {1} | Connection Type : {2} | Username: {3}", connString, queryDescription, connType, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for ExecuteQuery method | Connection string: {0} | Query Description : {1} | Connection Type : {2} | Username: {3}", connString, queryDescription, connType, Username), ex.InnerException);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Query for ExecuteQuery method | Connection string: {0} | Query Description : {1} | Connection Type : {2} | Username: {3}", connString, queryDescription, connType, Username));
                ELogger.ErrorException(string.Format("Error occured in Query for ExecuteQuery method | Connection string: {0} | Query Description : {1} | Connection Type : {2} | Username: {3}", connString, queryDescription, connType, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Query for ExecuteQuery method | Connection string: {0} | Query Description : {1} | Connection Type : {2} | Username: {3}", connString, queryDescription, connType, Username), ex.InnerException);
                throw;
            }
            return dataSet;

        }
        //public bool CheckQuery(string query)
        //{
        //    ParseResult res = Parser.Parse(query);
        //    return true;
        //}


    }
}


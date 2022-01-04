using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MARS_Repository.Repositories
{
    public class VariableRepository
    {
        DBEntities entity = Helper.GetMarsEntitiesInstance();// new DBEntities();
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;
        public string currentPath = string.Empty;
        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public SYSTEM_LOOKUP GetVariableNameById(int lookupid)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Check Duplicate VariableName start | Id: {0} | Username: {1}", lookupid, Username), currentPath);
                var result = entity.SYSTEM_LOOKUP.FirstOrDefault(x => x.LOOKUP_ID == lookupid);
                var lresult = result.TABLE_NAME + ',' + result.STATUS;
                Helper.WriteLogMessage(string.Format("Check Duplicate VariableName end | Id: {0} | Username: {1}", lookupid, Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for GetVariableNameById method | VariableId : {0} | UserName: {1} | Error: {2}", lookupid,Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for GetVariableNameById method | VariableId : {0} | UserName: {1} | Error: {2}", lookupid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public string GetVariableById(int lookupid)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("GetVariableById start | lookupid: {0} | Username: {1}", lookupid, Username), currentPath);
                var result = entity.SYSTEM_LOOKUP.FirstOrDefault(x => x.LOOKUP_ID == lookupid);
                var lresult = result.TABLE_NAME + ',' + result.STATUS;
                Helper.WriteLogMessage(string.Format("GetVariableById end | lookupid: {0} | Username: {1}", lookupid, Username), currentPath);
                return lresult;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for GetVariableById method | VariableId : {0} | UserName: {1} | Error: {2}", lookupid, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for GetVariableById method | VariableId : {0} | UserName: {1} | Error: {2}", lookupid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public SYSTEM_LOOKUP GetVariableNameByName(string vname)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("GetVariableNameByName start | VariableName: {0} | Username: {1}", vname, Username), currentPath);
                var result = entity.SYSTEM_LOOKUP.FirstOrDefault(x => x.FIELD_NAME.ToLower().Trim() == vname.ToLower().Trim());
                Helper.WriteLogMessage(string.Format("GetVariableNameByName end | VariableName: {0} | Username: {1}", vname, Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for GetVariableNameByName method | VariableName : {0} | UserName: {1} | Error: {2}", vname, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for GetVariableNameByName method | VariableName : {0} | UserName: {1} | Error: {2}", vname, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public bool CheckDuplicateVariableName(long lookupid, string name)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Check Duplicate VariableName start | Id: {0} | VariableName: {1} | Username: {2}", lookupid, name, Username), currentPath);
                var lresult = false;
                if (lookupid != 0)
                {
                    lresult = entity.SYSTEM_LOOKUP.Any(x => x.LOOKUP_ID != lookupid && x.FIELD_NAME.ToLower().Trim() == name.ToLower().Trim());
                }
                else
                {
                    lresult = entity.SYSTEM_LOOKUP.Any(x => x.FIELD_NAME.ToLower().Trim() == name.ToLower().Trim());
                }

                Helper.WriteLogMessage(string.Format("Check Duplicate VariableName end | Id: {0} | VariableName: {1} | Username: {2}", lookupid, name, Username), currentPath);
                return lresult;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for CheckDuplicateVariableName method | VariableId : {0} | Variable Name : {1} | UserName: {2} | Error: {3}", lookupid, name, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for CheckDuplicateVariableName method | VariableId : {0} | Variable Name : {1} | UserName: {2} | Error: {3}", lookupid, name, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public List<VariableModel> GetVariables(string schema, string lconstring, int startrec, int pagesize, string FieldNameSearch, string TableSearch, string Displaynamesearch, string statussearch, string colname, string colorder)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get Variable List start | UserName: {0}", Username), currentPath);
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

                ladd_refer_image[4] = new OracleParameter("FieldNameSearch", OracleDbType.Varchar2);
                ladd_refer_image[4].Value = FieldNameSearch;

                ladd_refer_image[5] = new OracleParameter("tablesearch", OracleDbType.Varchar2);
                ladd_refer_image[5].Value = TableSearch;

                ladd_refer_image[6] = new OracleParameter("displaynamesearch", OracleDbType.Varchar2);
                ladd_refer_image[6].Value = Displaynamesearch;

                ladd_refer_image[7] = new OracleParameter("statussearch", OracleDbType.Varchar2);
                ladd_refer_image[7].Value = statussearch;

                ladd_refer_image[8] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[8].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }
                lcmd.CommandText = schema + "." + "SP_GET_VARIABLE_DETAILS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];

                List<VariableModel> resultList = dt.AsEnumerable().Select(row =>
                    new VariableModel
                    {
                        Lookupid = row.Field<long>("lookupid"),
                        field_name = Convert.ToString(row.Field<string>("Name")),
                        Display_name = Convert.ToString(row.Field<string>("Value")),
                        Table_name = Convert.ToString(row.Field<string>("Type")),
                        Statusvalue = Convert.ToString(row.Field<string>("Base")),
                        TotalCount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                    }).ToList();

                Helper.WriteLogMessage(string.Format("Get Variable List end | UserName: {0}", Username), currentPath);
                return resultList;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for GetVariables method | Connection String : {0} | Schema : {1} | UserName: {2} | Error: {3}", lconstring, schema, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for GetVariables method | Connection String : {0} | Schema : {1} | UserName: {2} | Error: {3}", lconstring, schema, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public List<VariableExportModel> ExportVariableList(string lstrConn, string schema)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Export Variable start | Username: {0}", Username), currentPath);
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
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

                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schema + "." + "SP_EXPORT_VARIABLE";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];

                List<VariableExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new VariableExportModel
                    {
                        Name = row.Field<string>("Name"),
                        Value = Convert.ToString(row.Field<string>("Value")),
                        Type = Convert.ToString(row.Field<string>("Type")),
                        BaseComp = Convert.ToString(row.Field<string>("Base/Comp")),

                    }).ToList();
                Helper.WriteLogMessage(string.Format("Export Variable end | Username: {0}", Username), currentPath);
                return resultList;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for ExportVariableList method | Connection String : {0} | Schema : {1} | UserName: {2} | Error: {3}", lstrConn, schema, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for ExportVariableList method | Connection String : {0} | Schema : {1} | UserName: {2} | Error: {3}", lstrConn, schema, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public bool DeleteVariable(int lid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Helper.WriteLogMessage(string.Format("Delete Variable start | VariableId: {0} | Username: {1}", lid, Username), currentPath);
                    var flag = false;
                    var lresult = GetVariableNameById(lid);
                    if (lresult != null)
                    {
                        entity.SYSTEM_LOOKUP.Remove(lresult);
                        entity.SaveChanges();
                        flag = true;
                    }
                    Helper.WriteLogMessage(string.Format("Delete Variable end | VariableId: {0} | Username: {1}", lid, Username), currentPath);
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for DeleteVariable method | VariableId : {0} | UserName: {1} | Error: {2}", lid, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for DeleteVariable method | VariableId : {0} | UserName: {1} | Error: {2}", lid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public string AddEditVariable(VariableModel lookup)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var lresult = entity.SYSTEM_LOOKUP.Find(lookup.Lookupid);
                    if (lookup != null)
                    {
                        if (lookup.Table_name == "2")
                        {
                            lookup.Table_name = "MODAL_VAR";
                        }
                        else if (lookup.Table_name == "1")
                        {
                            lookup.Table_name = "GLOBAL_VAR";
                        }
                        else if (lookup.Table_name == "3")
                        {
                            lookup.Table_name = "LOOP_VAR";
                        }
                        var lflag = CheckDuplicateVariableName(lookup.Lookupid, lookup.field_name);
                        if (lresult == null)
                        {
                            Helper.WriteLogMessage(string.Format("Add variable start | variable: {0} | Username: {1}", lookup.field_name, Username), currentPath);
                            SYSTEM_LOOKUP tbl = new SYSTEM_LOOKUP();
                            var lookupID = Helper.NextTestSuiteId("SYSTEM_LOOKUP_SEQ");
                            tbl.LOOKUP_ID = lookupID;
                            tbl.FIELD_NAME = lookup.field_name;
                            tbl.DISPLAY_NAME = lookup.Display_name;
                            tbl.TABLE_NAME = lookup.Table_name;
                            tbl.STATUS = lookup.status;
                            tbl.VALUE = lookup.value;
                            entity.SYSTEM_LOOKUP.Add(tbl);
                            entity.SaveChanges();

                            Helper.WriteLogMessage(string.Format("Add variable end | variable: {0} | Username: {1}", lookup.field_name, Username), currentPath);
                            return "success";
                        }
                        else
                        {
                            Helper.WriteLogMessage(string.Format("Edit variable start | variable: {0} | variableId: {1} | Username: {2}", lookup.field_name, lookup.Lookupid, Username), currentPath);

                            lresult.FIELD_NAME = lookup.field_name;
                            lresult.DISPLAY_NAME = lookup.Display_name;
                            lresult.STATUS = lookup.status;
                            lresult.VALUE = lookup.value;
                            lresult.TABLE_NAME = lookup.Table_name;
                            entity.SaveChanges();
                            Helper.WriteLogMessage(string.Format("Edit variable end | variable: {0} | variableId: {1} | Username: {2}", lookup.field_name, lookup.Lookupid, Username), currentPath);
                        }
                    }
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for AddEditVariable method | VariableId : {0} | UserName: {1} | Error: {2}", lookup.Lookupid, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for AddEditVariable method | VariableId : {0} | UserName: {1} | Error: {2}", lookup.Lookupid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public string CheckVariable(VariableModel lookup)
        {
            try
            {
                var lresult = string.Empty;
                var flag = false;

                if (lookup.Table_name == "2")
                {
                    lookup.Table_name = "MODAL_VAR";
                }
                else if (lookup.Table_name == "1")
                {
                    lookup.Table_name = "GLOBAL_VAR";
                }
                else if (lookup.Table_name == "3")
                {
                    lookup.Table_name = "LOOP_VAR";
                }

                if (lookup.Lookupid == 0)
                {
                    if (lookup.status > 0)
                        flag = entity.SYSTEM_LOOKUP.Any(x => x.FIELD_NAME.ToLower().Trim() == lookup.field_name.ToLower().Trim() && x.TABLE_NAME == lookup.Table_name && x.STATUS == lookup.status);
                    else
                        flag = entity.SYSTEM_LOOKUP.Any(x => x.FIELD_NAME.ToLower().Trim() == lookup.field_name.ToLower().Trim() && x.TABLE_NAME == lookup.Table_name);

                    if (flag)
                        lresult = "Varible already exist.";
                }
                else
                {
                    if (lookup.status > 0)
                        flag = entity.SYSTEM_LOOKUP.Any(x => x.FIELD_NAME.ToLower().Trim() == lookup.field_name.ToLower().Trim() && x.TABLE_NAME == lookup.Table_name && x.STATUS == lookup.status && x.LOOKUP_ID != lookup.Lookupid);
                    else
                        flag = entity.SYSTEM_LOOKUP.Any(x => x.FIELD_NAME.ToLower().Trim() == lookup.field_name.ToLower().Trim() && x.TABLE_NAME == lookup.Table_name && x.LOOKUP_ID != lookup.Lookupid);

                    if (flag)
                        lresult = "Varible already exist.";
                }

                return lresult;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Variable for CheckVariable method | VariableId : {0} | UserName: {1} | Error: {2}", lookup.Lookupid, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Variable for CheckVariable method | VariableId : {0} | UserName: {1} | Error: {2}", lookup.Lookupid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
    }
}


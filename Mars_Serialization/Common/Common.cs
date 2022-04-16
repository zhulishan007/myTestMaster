using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using System.Threading.Tasks;

namespace Mars_Serialization.Common
{
    public static class Common
    {
        static Logger logger = LogManager.GetLogger("Log");

        #region GET DATA FROM THE DATABASE
        public static DataTable GetRecordAsDatatable(string connectionString, string query)
        {
            var d = (new Random()).NextDouble();
            logger.Info($"GetRecordAsDatatable begins...[{connectionString}] -[{d}]");
            logger.Info($"GetRecordAsDatatable begin...[{connectionString}]");
            try
            {
                OracleConnection con = new OracleConnection(connectionString);
                OracleCommand cmd = new OracleCommand
                {
                    CommandText = query,
                    Connection = con
                };
                if (con.State != ConnectionState.Open)
                    con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dr);
                con.Close();
                return dataTable;
            }
            catch (Exception ex)
            {
                logger.Error($"Exctption when want to open [{connectionString}] \r\n\t[{ex.Message}], \r\n\t[{ex.StackTrace}]");
                throw;
            }
            finally {
                logger.Info($"GetRecordAsDatatable end. [{connectionString}], \r\n\t Id=[{d}]");
            }
        }
        #endregion

        #region CONVERT DATATABLE TO LIST
        public static List<T> ConvertDataTableToList<T>(DataTable dt) where T : class, new()
        {
            List<T> lstItems = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                    lstItems.Add(ConvertDataRowToGenericType<T>(row));
            else
                lstItems = null;
            return lstItems;
        }
        private static T ConvertDataRowToGenericType<T>(DataRow row) where T : class, new()
        {
            Type entityType = typeof(T);
            T objEntity = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                PropertyInfo property = entityType.GetProperty(column.ColumnName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                try
                {
                    if (property != null && property.CanWrite)
                        property.SetValue(objEntity, value, null);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objEntity;
        }
        #endregion
    }
}

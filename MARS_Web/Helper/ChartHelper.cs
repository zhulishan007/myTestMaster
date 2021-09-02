using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Helper
{
    public class ChartHelper
    {

        public string GetConnectionString(DatabaseConnectionViewModel connectionViewModel)
        {
            string sConnString = "";
            //to fetch the connection string
            connectionViewModel.Port = connectionViewModel.Port == null ? 0 : connectionViewModel.Port;

            if (connectionViewModel.ConnectionType == 1)
            {
                sConnString = new OracleConnectionStringBuilder(connectionViewModel.Host, int.Parse(connectionViewModel.Port.ToString()), connectionViewModel.ServiceName, connectionViewModel.UserId, connectionViewModel.Password, "", true).Create();

            }
            if (connectionViewModel.ConnectionType == 2)
            {
                sConnString = new SQLServerConnectionStringBuilder(connectionViewModel.Host, int.Parse(connectionViewModel.Port.ToString()), connectionViewModel.ServiceName, connectionViewModel.UserId, connectionViewModel.Password, connectionViewModel.Sid, "", true).Create();
                //sConnString = "Data Source=13.90.224.87\\MSSQL2;Initial Catalog=new_lotools;Persist Security Info=True;User ID=sa;Password=Admin123123;Connect Timeout=36000";
            }
            if (connectionViewModel.ConnectionType == 3)
            {
                sConnString = new SybaseConnectionStringBuilder(connectionViewModel.Host, int.Parse(connectionViewModel.Port.ToString()), connectionViewModel.ServiceName, connectionViewModel.UserId, connectionViewModel.Password, connectionViewModel.Sid, "", true).Create();
                //sConnString = "Server=CS-LC-17;Port=5000;Database=charts_mars;Uid=sa;Pwd=adminadmin;";
            }

            return sConnString;
        }

        public bool getParameterCount(string ChartType, string Query)
        {
            QueryRepository queryRepository = new QueryRepository();
            int[] requiredColumns = new int[1];
            if (ChartType == "1")//Pie chart
                requiredColumns[0] = 2;
            else
                requiredColumns[0] = 3;
            return queryRepository.CheckColumnCount(Query, requiredColumns);
        }

        public int IsIntegerColumnPresent(DataSet dataSet)
        {
            DataTable dataTable = dataSet.Tables[0];
            var dataColumns = dataTable.Columns;
            string[] acceptedTypes = { "Decimal", "Double", "Int16", "Int32", "Int64" };
            for (int i = 0; i < dataColumns.Count; i++)
            {
                foreach (string type in acceptedTypes)
                {
                    if (dataColumns[i].DataType.Name == type)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int IsDateTimeColumnPresent(DataSet dataSet)
        {
            DataTable dataTable = dataSet.Tables[0];
            var dataColumns = dataTable.Columns;
            //string[] acceptedTypes = { "DateTime" };
            for (int i = 0; i < dataColumns.Count; i++)
            {
                    if (dataColumns[i].DataType.Name == "DateTime")
                    {
                        return i;
                    }
                
            }
            return -1;
        }

        public bool IsDateTime(DataColumn column)
        {
            if (column.DataType.Name == "DateTime")
            {
                return true;
            }
            return false;
        }

    }
}
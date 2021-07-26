using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public class SQLServerConnectionStringBuilder
    {
        private string serviceName;
        private string otherOptions;
        private string password;
        private int port;
        private string hostName;

        public bool IsDirty { get; private set; }
        private string sid;
        private string username;
        private bool pooling;
        private string statementCacheSize;
//        Data Source = 13.90.224.87\\MSSQL2;Initial Catalog = new_lotools; Persist Security Info=True;User ID = sa; Password=Admin123123;Connect Timeout = 36000
        public SQLServerConnectionStringBuilder(string host, int port, string service, string userName, string password, string sid, string cacheSize, bool pooling)
        {
            this.hostName = host;
            this.port = port;
            this.serviceName = service;
            this.username = userName;
            this.password = password;
            this.statementCacheSize = cacheSize;
            this.pooling = pooling;
            this.sid = sid;
        }

        protected internal string Create()
        {
            string connectionString="";
            //datasource = host, instance name = service name, initial catalog - sid
            connectionString = String.Format("Data Source = {0}", hostName);
            if (serviceName != null)
                connectionString = connectionString + String.Format("\\{0}", serviceName);
            connectionString = connectionString + String.Format(";Initial Catalog = {0};", sid);
            if (username == null || username == "")
                connectionString = connectionString + String.Format("Integrated Security=True;");
            else
                connectionString = connectionString + String.Format("Persist Security Info=True;User ID = {0}; Password = {1};", username, password);
            connectionString = connectionString + "Connect Timeout = 8";
            //working code
            /*if (username == null || username == "")
            {
                connectionString = String.Format(" Data Source = {0};Initial Catalog = {1};Integrated Security=True; Connect Timeout = 8",
                            hostName, sid);
            }
            else
            {
                connectionString = String.Format(" Data Source = {0}\\{1};Initial Catalog = {2}; Persist Security Info=True;User ID = {3}; Password = {4};Connect Timeout = 8",
                            hostName, serviceName, sid, username, password);

            }
            */

            return connectionString;
        }
    }
}
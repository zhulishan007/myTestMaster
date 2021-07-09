using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public class OracleConnectionStringBuilder
    {
        private string serviceName;
        private string otherOptions;
        private string password;
        private int port;
        private string hostName;

        public bool IsDirty { get; private set; }

        private string username;
        private bool pooling;
        private string statementCacheSize;

        public OracleConnectionStringBuilder(string host, int port, string service, string userName, string password, string cacheSize, bool pooling)
        {
            this.hostName = host;
            this.port = port;
            this.serviceName = service;
            this.username = userName;
            this.password = password;
            this.statementCacheSize = cacheSize;
            this.pooling = pooling;
        }

        protected internal string Create()
        {
            string connectionString =  String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER=DEDICACATED)(SERVICE_NAME={2})));User Id={3};Password={4};pooling=false;Connection Timeout=100;",
                        hostName, port, serviceName, username, password);
            return connectionString;
        }
    }
}
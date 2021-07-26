using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public class SybaseConnectionStringBuilder
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
        
        public SybaseConnectionStringBuilder(string host, int port, string service, string userName, string password, string sid, string cacheSize, bool pooling)
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
            string connectionString = String.Format("Server={0},{1}; Database={2};Uid={3};Pwd={4};",
                        hostName, port, sid, username, password);
            return connectionString;
        }
    }
}
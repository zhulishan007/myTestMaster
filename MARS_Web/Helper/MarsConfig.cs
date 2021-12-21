using MARS_Repository.ViewModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace MARS_Web.Helper
{
    public class MarsConfig
    {
        private string marsConfigFile;
        private string marsEnvironment;
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        private static MarsConfig marsConfigInstance;

        private static XDocument marsConfig;

        public Dictionary<string, string> AppSettings { get; set; }
        public MarsConfig()
        {
        }

        public MarsConfig(string marsConfigFile, string marsEnvironment)
        {
            this.marsConfigFile = marsConfigFile;
            this.marsEnvironment = marsEnvironment;
        }

        public static MarsConfig Configure(string marsEnvironment)
        {
            string marsHomeFolder = HttpContext.Current.Server.MapPath("/Config");
            string marsConfigFile = marsHomeFolder + @"\Mars.config";
            return Configure(marsConfigFile, marsEnvironment);
        }
        public static MarsConfig Configure(string marsConfigFile, string marsEnvironment)
        {
            marsConfigInstance = new MarsConfig(marsConfigFile, marsEnvironment);
            marsConfig = XDocument.Load(marsConfigFile);

            return marsConfigInstance;
        }

        public DatabaseConnectionDetails GetDatabaseConnectionDetails(string modifiedPassword = null)
        {
            DatabaseConnectionDetails databaseConnectionDetails = null;
            logger.Info(string.Format("Get Database Connection Details start | Database: {0}", marsEnvironment));
            try
            {
                databaseConnectionDetails = GetDatabaseConnectionDetailsImp(marsConfig, modifiedPassword);
                logger.Info(string.Format("Get Database Connection Details end | Database: {0}", marsEnvironment));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in GetDatabaseConnectionDetails"));
                ELogger.ErrorException(string.Format("Error occured in GetDatabaseConnectionDetails | Database: {0}", marsEnvironment), ex);
            }

            return databaseConnectionDetails;
        }
        private DatabaseConnectionDetails GetDatabaseConnectionDetailsImp(XDocument config, string modifiedPassword)
        {
            DatabaseConnectionDetails databaseConnectionDetails = null;
            var env = GetMarsEnvironment(config);

            var details = env.Elements("DatabaseConnectionDetails").FirstOrDefault();

            string ConnString = details.Attributes("ConnString").FirstOrDefault().Value;
            string Schema = details.Attributes("Schema").FirstOrDefault().Value;
            string UserName = details.Attributes("UserName").FirstOrDefault().Value;

            string Password = null;
            if (modifiedPassword != null)
                Password = modifiedPassword;
            else
                Password = details.Attributes("Password").FirstOrDefault().Value;

            string Host = details.Attributes("Host").FirstOrDefault().Value;
            string Port = details.Attributes("Port").FirstOrDefault().Value;
            string Type = details.Attributes("Type").FirstOrDefault().Value;
            string ServiceName = details.Attributes("ServiceName").FirstOrDefault().Value;

            if (ConnString.Trim().Length == 0)
                ConnString = new OracleConnectionStringBuilder(Host, int.Parse(Port), ServiceName, UserName, Password, "", true).Create();

            //string EntityConnString = BuildEntityConnString(Host, UserName, Password);
            string EntityConnString = BuildEntityConnString(ConnString);

            databaseConnectionDetails = new DatabaseConnectionDetails(Host, Port, ServiceName, Type, ConnString, EntityConnString, Schema, UserName, Password);

            return databaseConnectionDetails;
        }
        private IEnumerable<XElement> GetMarsEnvironment(XDocument config)
        {
            XDocument xdoc = config;
            var marsEnvironments = xdoc.Element("configuration").Element("MarsEnvironments").Elements();
            if (!string.IsNullOrEmpty(marsEnvironment))
            {
                var env = from e in marsEnvironments where e.Attribute("name").Value.Equals(marsEnvironment) select e;
                return env;
            }
            else
            {
                var xmlDocument = new XmlDocument();
                using (var xmlReader = xdoc.CreateReader())
                {
                    xmlDocument.Load(xmlReader);
                }
                var xmlStr = JsonConvert.SerializeXmlNode(xmlDocument);
                var findDefult = xmlStr.IndexOf("default");
                var substr = xmlStr.Substring(findDefult);
                var splitvalue = substr.Split(',');
                var splitDefultval = splitvalue[0].Split(':');
                var defultmarsEnvironment = splitDefultval[1].Trim('"');
                var env = from e in marsEnvironments where e.Attribute("name").Value.Equals(defultmarsEnvironment) select e;
                return env;
            }
        }

        public string GetDefaultDatabase()
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = marsConfig.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            var xmlStr = JsonConvert.SerializeXmlNode(xmlDocument);
            var findDefult = xmlStr.IndexOf("default");
            var substr = xmlStr.Substring(findDefult);
            var splitvalue = substr.Split(',');
            var splitDefultval = splitvalue[0].Split(':');
            var defultmarsEnvironment = splitDefultval[1].Trim('"');

            return defultmarsEnvironment;
        }

        //private string BuildEntityConnString(string Host, string UserName, string Password)
        //{
        //    string entityConnString = string.Format("metadata=res://*/Entities.Db_Context.csdl|res://*/Entities.Db_Context.ssdl|res://*/Entities.Db_Context.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=\"DATA SOURCE={0};PASSWORD={1};USER ID={2}\"", Host, Password, UserName);
        //    return entityConnString;
        //}

        private string BuildEntityConnString(string connString)
        {
            string entityConnString = "";
            entityConnString = "metadata=res://*/Model.MarsModel.csdl|res://*/Model.MarsModel.ssdl|res://*/Model.MarsModel.msl;" +
                "provider=Oracle.ManagedDataAccess.Client;" +
                "provider connection string=" + "\"" + connString + "\"";
            return entityConnString;
        }

        public List<DBconnectionViewModel> GetConnectionDetails()
        {
            List<DBconnectionViewModel> dBconnection = new List<DBconnectionViewModel>();

            try
            {
                var marsEnvironments = marsConfig.Element("configuration").Element("MarsEnvironments").Elements();

                var databaseList = marsEnvironments.Attributes();

                foreach (var item in databaseList)
                {
                    DBconnectionViewModel db = new DBconnectionViewModel();
                    db.Databasename = item.Value;

                    var env = from e in marsEnvironments where e.Attribute("name").Value.Equals(item.Value) select e;
                    var details = env.Elements("DatabaseConnectionDetails").FirstOrDefault();
                    db.Host = details.Attributes("Host").FirstOrDefault().Value;
                    db.Schema = details.Attributes("Schema").FirstOrDefault().Value;
                    db.UserName = details.Attributes("UserName").FirstOrDefault().Value;
                    db.Password = details.Attributes("Password").FirstOrDefault().Value;
                    db.Port = details.Attributes("Port").FirstOrDefault().Value;
                    db.Service_Name = details.Attributes("ServiceName").FirstOrDefault().Value;
                    dBconnection.Add(db);
                }
            }
            catch (Exception ex)
            {
            }

            return dBconnection;
        }
    }
}
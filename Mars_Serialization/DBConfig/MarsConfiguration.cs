using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Mars_Serialization.DBConfig
{
    public class MarsConfiguration
    {
        private string marsConfigFile;
        private string marsEnvironment;
        private static MarsConfiguration marsConfigInstance;

        private static XDocument marsConfig;


        public MarsConfiguration(string marsConfigFile, string marsEnvironment)
        {
            this.marsConfigFile = marsConfigFile;
            this.marsEnvironment = marsEnvironment;
        }

        //public static MarsConfiguration Configure(string path,string marsEnvironment)
        //{

        //    return Configure(path, marsEnvironment);
        //}
        public static MarsConfiguration Configure(string path, string marsEnvironment)
        {
            marsConfigInstance = new MarsConfiguration(path, marsEnvironment);
            marsConfig = XDocument.Load(path);

            return marsConfigInstance;
        }
        public DatabaseConnectionDetails GetDatabaseConnectionDetails(string modifiedPassword = null)
        {
            DatabaseConnectionDetails databaseConnectionDetails = null;

            try
            {
                databaseConnectionDetails = GetDatabaseConnectionDetailsImp(marsConfig, modifiedPassword);
            }
            catch (Exception ex)
            {
            }

            return databaseConnectionDetails;
        }
        private DatabaseConnectionDetails GetDatabaseConnectionDetailsImp(XDocument config, string modifiedPassword)
        {
            DatabaseConnectionDetails databaseConnectionDetails = null;
            var env = GetMarsEnvironment(config);

            var details = env.Elements("DatabaseConnectionDetails").FirstOrDefault();
            if (details == null)
                return null;
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

        private string BuildEntityConnString(string connString)
        {
            string entityConnString = "";
            entityConnString = "metadata=res://*/Entity.MarsEntity.csdl|res://*/Entity.MarsEntity.ssdl|res://*/Entity.MarsEntity.msl;" +
                "provider=Oracle.ManagedDataAccess.Client;" +
                "provider connection string=" + "\"" + connString + "\"";
            return entityConnString;
        }

    }
}

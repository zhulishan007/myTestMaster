using MARS_Repository.ViewModel;
using MARS_Web.MarsUtility;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MARS_Web.Helper
{

    public class MarsRESTFulLink
    {
        private static MarsLog Logger = MarsLog.GetLogger(Mars_LogType._normal, typeof(MarsRESTFulLink));
        private static MarsLog Elogger = MarsLog.GetLogger(Mars_LogType._errLog, typeof(MarsRESTFulLink));

        private const string cnst_targetXmlNodeName = "MarsRESTFulLink";
        public string HostName;
        public int Port;

        public static bool WriteToXmlNode(MarsRESTFulLink inst, XmlDocument targetXml, ref string strError, ref string strStack)
        {
            Logger.LogBegin("WriteToXmlNode");
            try
            {
                if (inst == null)
                {
                    strError = "object is null";
                    strStack = Environment.StackTrace;
                    return false;
                }
                if (targetXml == null)
                {
                    strError = "no available XmlDoc";
                    strStack = Environment.StackTrace;
                    return false;
                }
                XmlNodeList targetElement = targetXml.GetElementsByTagName(cnst_targetXmlNodeName);
                XmlElement targetElementOne = null;
                XmlWriter w = null;
                if ((targetElement == null) || (targetElement.Count <= 0))
                {
                    targetElementOne = targetXml.CreateElement(cnst_targetXmlNodeName);
                    w = targetElementOne.CreateNavigator().AppendChild();
                }
                else
                {
                    if (targetElement[0].NodeType != XmlNodeType.Element)
                    {
                        strError = $"node MarsRestful is not an element, it is [{targetElement[0].NodeType}]";
                        strStack = Environment.StackTrace;
                        return false;
                    }
                    targetElementOne = (XmlElement)targetElement[0];
                    w = targetElementOne.CreateNavigator().AppendChild();
                }

                new XmlSerializer(inst.GetType()).Serialize(w, inst);
                w.Close();
                return true;
            }
            catch (Exception e)
            {
                Elogger.Error(e.Message, e);
                return false;
            }
            finally
            {
                Logger.LogEnd();
            }
            
        }

        public static MarsRESTFulLink LoadFromXmlConfig(XmlDocument sourceXml, ref bool isOk, ref string strError, ref string strStack, ref string strAdv)
        {
            Logger.LogBegin("LoadFromXmlConfig");
            try
            {
                XmlNodeList targetElement = sourceXml.GetElementsByTagName(cnst_targetXmlNodeName);
                if ((targetElement == null) || (targetElement.Count <= 0))
                {
                    strAdv = $"Please check Marsconfig.xml, and macke sure, there is a node {cnst_targetXmlNodeName}";
                    strError = $"no {cnst_targetXmlNodeName} find from Marsconfig.xml.";
                    strAdv = Environment.StackTrace;
                    isOk = false;
                    return null;
                }
                XmlNode n = targetElement[0];
                if (n.NodeType != XmlNodeType.Element)
                {
                    strAdv = $"Please check Marsconfig.xml, and macke sure, there is a element node {cnst_targetXmlNodeName}";
                    strError = $"no Element Node {cnst_targetXmlNodeName} find from Marsconfig.xml.";
                    strAdv = Environment.StackTrace;
                    isOk = false;
                    return null;
                }
                
                var r = new XmlNodeReader(n);
                //x.CreateNavigator()
                var o = new XmlSerializer(typeof(MarsRESTFulLink)).Deserialize(r);
                if ((o == null)||((o as MarsRESTFulLink)==null))
                {
                    strAdv = $"Make sure the xml node {cnst_targetXmlNodeName} is right format";
                    strError = "Can't convert XmlNode to Mars RESTful linker object";
                    strStack = Environment.StackTrace;
                    isOk = false;
                    return null;
                }
                isOk = true;
                return o as MarsRESTFulLink;
            }
            catch(Exception e)
            {
                strStack = e.StackTrace;
                strAdv = "Contact Marquis";
                Logger.Error(strError = e.Message, e);
                isOk = false;
                return null;
            }
            finally
            {
                Logger.LogEnd();
            }
        }
    }

    public class MarsConfig
    {
        private string marsConfigFile;
        private string marsEnvironment;
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");

        private static MarsConfig marsConfigInstance;

        private static XDocument marsConfig;

        public static MarsRESTFulLink restfulInfo { get; set; }

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

            bool isOk = false;
            string strError = "", strAdv = "", strStack = "";
            XmlDocument x = new XmlDocument();
            x.Load(marsConfigFile);
            restfulInfo = MarsRESTFulLink.LoadFromXmlConfig(x, ref isOk, ref strError, ref strStack, ref strAdv);

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
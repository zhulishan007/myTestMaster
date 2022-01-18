using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using MarsSerializationHelper.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using static MarsSerializationHelper.JsonSerialization.SerializationFile;

namespace MARS_Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #region SERIALIZATION JSON FILES 
            MarsConfig mc = MarsConfig.Configure(string.Empty);
            var dbNameList = mc.GetConnectionDetails().Select(x => x.Schema).ToList();
            Load_Serializations_Files(dbNameList);
            #endregion
        }

        protected void Load_Serializations_Files(List<string> dbNameList)
        {
            var usersData = new ConcurrentDictionary<string, ConcurrentDictionary<UserViewModal, List<MarsSerializationHelper.ViewModel.ProjectByUser>>>();
            var appsData = new ConcurrentDictionary<string, List<T_Memory_REGISTERED_APPS>>();
            Thread Serializations = new Thread(delegate ()
            {
                if (dbNameList.Count() > 0)
                {
                    foreach (var databaseName in dbNameList)
                    {
                        MarsConfig mc = MarsConfig.Configure(databaseName);
                        DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                        var userDictionary = MarsSerializationHelper.JsonSerialization.SerializationFile.GetDictionary(det.ConnString);
                        usersData.TryAdd(databaseName, userDictionary);                      

                        string marsHomeFolder = HostingEnvironment.MapPath("/Config");
                        string marsConfigFile = marsHomeFolder + @"\Mars.config";
                        MarsSerializationHelper.JsonSerialization.SerializationFile.ChangeConnectionString(databaseName, marsConfigFile);
                        MarsSerializationHelper.JsonSerialization.SerializationFile.CreateJsonFiles(databaseName, HostingEnvironment.MapPath("~/"), det.ConnString);

                        var appDictionary = GetAppData(databaseName);
                        appsData.TryAdd(databaseName, appDictionary);
                    }
                    GlobalVariable.UsersDictionary = usersData;
                    GlobalVariable.AllApps = appsData;
                }
            })
            {
                IsBackground = true
            };
            Serializations.Start();
        }

        protected List<T_Memory_REGISTERED_APPS> GetAppData(string databaseName)
        {
            var allList = new List<T_Memory_REGISTERED_APPS>();
            string fullPath = Path.Combine(HostingEnvironment.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Application.ToString(), databaseName, "application.json");
            if (File.Exists(fullPath))
            {
                string jsongString = File.ReadAllText(fullPath);
                allList = JsonConvert.DeserializeObject<List<T_Memory_REGISTERED_APPS>>(jsongString);
            }
            return allList;
        }
    }
}

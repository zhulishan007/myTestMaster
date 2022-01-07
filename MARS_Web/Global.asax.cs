using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
            Thread Serializations = new Thread(delegate ()
            {
                if (dbNameList.Count() > 0)
                {
                    foreach (var databaseName in dbNameList)
                    {
                        MarsConfig mc = MarsConfig.Configure(databaseName);
                        DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                        string marsHomeFolder = HostingEnvironment.MapPath("/Config");
                        string marsConfigFile = marsHomeFolder + @"\Mars.config";
                        MarsSerializationHelper.JsonSerialization.SerializationFile.ChangeConnectionString(databaseName, marsConfigFile);
                        MarsSerializationHelper.JsonSerialization.SerializationFile.CreateJsonFiles(databaseName, HostingEnvironment.MapPath("~/"), det.ConnString);
                    }
                }
            })
            {
                IsBackground = true
            };
            Serializations.Start();
        }
    }
}

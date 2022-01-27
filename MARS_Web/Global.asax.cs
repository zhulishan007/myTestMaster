﻿using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using MarsSerializationHelper.ViewModel;
using Newtonsoft.Json;
using NLog;
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
        readonly Logger logger = LogManager.GetLogger("Log");
        readonly Logger ELogger = LogManager.GetLogger("ErrorLog");

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #region SERIALIZATION JSON FILES 
            MarsConfig mc = MarsConfig.Configure(string.Empty);
            var dbNameList = mc.GetConnectionDetails().Select(x => x.Schema).ToList();
            bool status = Load_Serializations_Files(dbNameList);
            #endregion
        }

        protected bool Load_Serializations_Files(List<string> dbNameList)
        {
            bool status = false;
            try
            {
                logger.Info(string.Format("Load_Serializations_Files | Database Names: {0} ", string.Join(", ", dbNameList)));

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
                    logger.Info(string.Format("Successfully load all the serialization files | Database Names: {0} ", string.Join(", ", dbNameList)));
                    status = true;
                });
                Serializations.IsBackground = true;
                Serializations.Start();
                return status;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Load_Serializations_Files for create json file."));
                ELogger.ErrorException(string.Format("Error occured in Load_Serializations_Files for create json file"), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Load_Serializations_Files for create json file "), ex.InnerException);
                return status;
            }
        }

        protected List<T_Memory_REGISTERED_APPS> GetAppData(string databaseName)
        {
            List<T_Memory_REGISTERED_APPS> allList = new List<T_Memory_REGISTERED_APPS>();
            try
            {
                logger.Info(string.Format("GetAppData -- Get Application list database wise | Database Names: {0} ", databaseName));

                string fullPath = Path.Combine(HostingEnvironment.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Application.ToString(), databaseName, "application.json");
                if (File.Exists(fullPath))
                {
                    string jsongString = File.ReadAllText(fullPath);
                    allList = JsonConvert.DeserializeObject<List<T_Memory_REGISTERED_APPS>>(jsongString);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in GetAppData for getting application list database wise  | Database Names: {0} ", databaseName));
                ELogger.ErrorException(string.Format("Error occured in GetAppData for getting application list database wise. "), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("Error occured in GetAppData for getting application list database wise. "), ex.InnerException);
            }
            return allList;
        }
    }
}

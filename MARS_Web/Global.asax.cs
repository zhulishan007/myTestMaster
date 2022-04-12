using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using Mars_Serialization.ViewModel;
using MARS_Web.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using static Mars_Serialization.JsonSerialization.SerializationFile;

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
            WebSocketHelper.WebSocketInstance.StartServer();
            #region SERIALIZATION JSON FILES 
            MarsConfig mc = MarsConfig.Configure(string.Empty);
            var dbNameList = mc.GetConnectionDetails().Select(x => x.Schema).ToList();
            bool status = Load_Serializations_Files(dbNameList);
            #endregion
        }


        protected bool LoadCaches(List<string> dbNameList,
            ConcurrentDictionary<string, ConcurrentDictionary<UserViewModal, 
                List<Mars_Serialization.ViewModel.ProjectByUser>>> userData,
            ConcurrentDictionary<string, List<T_Memory_REGISTERED_APPS>> appsData,
            ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.KeywordViewModel>> keywordsData,
            ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.GroupsViewModel>> groupsData ,
            ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.FoldersViewModel>> foldersData,
            ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.SetsViewModel>> setsData
            )
        {
            logger.Info($"LoadCaches begin....dbNames:[{dbNameList.ToArray()}]");
            try
            {
                if (dbNameList.Count() > 0)
                {
                    foreach (var databaseName in dbNameList)
                    {
                        MarsConfig mc = MarsConfig.Configure(databaseName);
                        DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();
                        (new InitCacheHelper()).InitAll(det.EntityConnString, det.ConnString, databaseName);
                        /* Task.Run(() =>
                         {
                             DBEntities.ConnectionString = det.EntityConnString;
                             DBEntities.Schema = databaseName;
                             var repTree = new GetTreeRepository();
                             var project = repTree.GetProjectListCache();
                             projects.TryAdd(databaseName, project);
                             var  storyboardlistCache = repTree.GetStoryboardListCache();
                             storyBoards.TryAdd(databaseName, storyboardlistCache);
                             var testSuit = repTree.GetTestSuiteListCache(det.ConnString);
                             testSuits.TryAdd(databaseName, testSuit);
                             var testCaseListCache = repTree.GetTestCaseListCache(det.ConnString);
                             testCases.TryAdd(databaseName, testCaseListCache);
                             var dataSet = repTree.GetDataSetListCache(det.ConnString);
                             dataSets.TryAdd(databaseName, dataSet);
                         });*/
                        logger.Info($"\t current DB Load_Serializations_Files:[{databaseName}]");
                        #region GET USER DATA AND STORE IN MEMORY
                        var userDictionary = Mars_Serialization.JsonSerialization.SerializationFile
                        .GetDictionary(det.ConnString);
                        userData.TryAdd(databaseName, userDictionary);
                        #endregion

                        System.Threading.Tasks.Task.Run(() =>
                        {
                            JsonFileHelper.InitStoryBoardJson(databaseName);
                        });

                        #region GET APPLICATOINS DATA AND STORE IN MEMORY
                        var appDictionary = GetAppData(databaseName);
                        appsData.TryAdd(databaseName, appDictionary);
                        #endregion

                        #region GET KEYWORDS DATA AND STORE IN MEMORY
                        var keywordDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetKeywordList(det.ConnString);
                        keywordsData.TryAdd(databaseName, keywordDictionary);
                        #endregion

                        #region GET GROUPS DATA AND STORE IN MEMORY
                        var groupDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetAllGroups(det.ConnString);
                        groupsData.TryAdd(databaseName, groupDictionary);
                        #endregion

                        #region GET FOLDERS DATA AND STORE IN MEMORY
                        var folderDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetAllFolders(det.ConnString);
                        foldersData.TryAdd(databaseName, folderDictionary);
                        #endregion

                        #region GET SETS DATA AND STORE IN MEMORY
                        var setDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetAllSets(det.ConnString);
                        setsData.TryAdd(databaseName, setDictionary);
                        #endregion
                    }


                    foreach (var databaseName in dbNameList)
                    {
                        MarsConfig mc = MarsConfig.Configure(databaseName);
                        DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                        //var userDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetDictionary(det.ConnString);
                        //usersData.TryAdd(databaseName, userDictionary);

                        string marsHomeFolder = HostingEnvironment.MapPath("/Config");
                        string marsConfigFile = marsHomeFolder + @"\Mars.config";
                        Mars_Serialization.JsonSerialization.SerializationFile.ChangeConnectionString(databaseName, marsConfigFile);
                        //Mars_Serialization.JsonSerialization.SerializationFile.CreateJsonFiles(databaseName, HostingEnvironment.MapPath("~/"), det.ConnString);

                        //var appDictionary = GetAppData(databaseName);
                        //appsData.TryAdd(databaseName, appDictionary);
                    }
                }
                return true;
            }catch(Exception e)
            {
                logger.Error($"LoadCaches exceptions:[{e.Message}] \r\n{e.StackTrace}");
                return false;
            }
            finally{
                logger.Info($"LoadCaches end");
            }
        }

        protected bool Load_Serializations_Files(List<string> dbNameList)
        {
            bool status = false;
            try
            {
                logger.Info(string.Format("Load_Serializations_Files | Database Names: {0} ", string.Join(", ", dbNameList)));

                var usersData = new ConcurrentDictionary<string, ConcurrentDictionary<UserViewModal, List<Mars_Serialization.ViewModel.ProjectByUser>>>();
                var appsData = new ConcurrentDictionary<string, List<T_Memory_REGISTERED_APPS>>();
                var keywordsData = new ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.KeywordViewModel>>();
                var groupsData = new ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.GroupsViewModel>>();
                var foldersData = new ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.FoldersViewModel>>();
                var setsData = new ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.SetsViewModel>>();
				GlobalVariable.UsersDictionary = usersData;
                GlobalVariable.AllApps = appsData;
                GlobalVariable.AllKeywords = keywordsData;
                GlobalVariable.AllGroups = groupsData;
                GlobalVariable.AllFolders = foldersData;
                GlobalVariable.AllSets = setsData;

                GlobalVariable.StoryBoardListCache = new ConcurrentDictionary<string, List<StoryBoardListByProject>>();
                GlobalVariable.TestCaseListCache = new ConcurrentDictionary<string, List<TestCaseListByProject>>();
                GlobalVariable.DataSetListCache = new ConcurrentDictionary<string, List<DataSetListByTestCase>>();
                GlobalVariable.TestSuiteListCache = new ConcurrentDictionary<string, List<TestSuiteListByProject>>();
                GlobalVariable.ProjectListCache = new ConcurrentDictionary<string, List<T_TEST_PROJECT>>();
                GlobalVariable.ActionsCache = new ConcurrentDictionary<string, List<SYSTEM_LOOKUP>>();
                GlobalVariable.FolderListCache = new ConcurrentDictionary<string, List<T_TEST_FOLDER>>();
                GlobalVariable.FolderFilterListCache = new ConcurrentDictionary<string, List<T_FOLDER_FILTER>>();
                GlobalVariable.RelFolderFilterListCache = new ConcurrentDictionary<string, List<REL_FOLDER_FILTER>>();
                GlobalVariable.AppListCache = new ConcurrentDictionary<string, List<T_REGISTERED_APPS>>();
                GlobalVariable.DataSetTagListCache  =new ConcurrentDictionary<string, List<T_TEST_DATASETTAG>>();
                GlobalVariable.GroupListCache = new ConcurrentDictionary<string, List<T_TEST_GROUP>>();
                GlobalVariable.SetListCache = new ConcurrentDictionary<string, List<T_TEST_SET>>();


                Thread Serializations = new Thread(delegate ()
                {
                    LoadCaches(dbNameList, usersData, appsData, keywordsData, groupsData, foldersData,
                        setsData);
                    if (dbNameList.Count() > 0)
                    {
                        
                        /*
                        foreach (var databaseName in dbNameList)
                        {


                            MarsConfig mc = MarsConfig.Configure(databaseName);
                            DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();
                            InitCacheHelper.InitAll(det.EntityConnString,det.ConnString,databaseName);
                            / * Task.Run(() =>
                             {
                                 DBEntities.ConnectionString = det.EntityConnString;
                                 DBEntities.Schema = databaseName;
                                 var repTree = new GetTreeRepository();
                                 var project = repTree.GetProjectListCache();
                                 projects.TryAdd(databaseName, project);
                                 var  storyboardlistCache = repTree.GetStoryboardListCache();
                                 storyBoards.TryAdd(databaseName, storyboardlistCache);
                                 var testSuit = repTree.GetTestSuiteListCache(det.ConnString);
                                 testSuits.TryAdd(databaseName, testSuit);
                                 var testCaseListCache = repTree.GetTestCaseListCache(det.ConnString);
                                 testCases.TryAdd(databaseName, testCaseListCache);
                                 var dataSet = repTree.GetDataSetListCache(det.ConnString);
                                 dataSets.TryAdd(databaseName, dataSet);
                             });* /
                            logger.Info($"\t current DB Load_Serializations_Files:[{databaseName}]");
                            #region GET USER DATA AND STORE IN MEMORY
                            var userDictionary = Mars_Serialization.JsonSerialization.SerializationFile
                            .GetDictionary(det.ConnString);
                            usersData.TryAdd(databaseName, userDictionary);
                            #endregion

 							System.Threading.Tasks.Task.Run(() =>
                            {
                                JsonFileHelper.InitStoryBoardJson(databaseName);
                            });
                           
                            #region GET APPLICATOINS DATA AND STORE IN MEMORY
                            var appDictionary = GetAppData(databaseName);
                            appsData.TryAdd(databaseName, appDictionary);
                            #endregion

                            #region GET KEYWORDS DATA AND STORE IN MEMORY
                            var keywordDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetKeywordList(det.ConnString);
                            keywordsData.TryAdd(databaseName, keywordDictionary);
                            #endregion

                            #region GET GROUPS DATA AND STORE IN MEMORY
                            var groupDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetAllGroups(det.ConnString);
                            groupsData.TryAdd(databaseName, groupDictionary);
                            #endregion

                            #region GET FOLDERS DATA AND STORE IN MEMORY
                            var folderDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetAllFolders(det.ConnString);
                            foldersData.TryAdd(databaseName, folderDictionary);
                            #endregion

                            #region GET SETS DATA AND STORE IN MEMORY
                            var setDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetAllSets(det.ConnString);
                            setsData.TryAdd(databaseName, setDictionary);
                            #endregion
                    
                        }

                       
                        foreach (var databaseName in dbNameList)
                        {
                            MarsConfig mc = MarsConfig.Configure(databaseName);
                            DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();

                            //var userDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetDictionary(det.ConnString);
                            //usersData.TryAdd(databaseName, userDictionary);

                            string marsHomeFolder = HostingEnvironment.MapPath("/Config");
                            string marsConfigFile = marsHomeFolder + @"\Mars.config";
                            Mars_Serialization.JsonSerialization.SerializationFile.ChangeConnectionString(databaseName, marsConfigFile);
                            //Mars_Serialization.JsonSerialization.SerializationFile.CreateJsonFiles(databaseName, HostingEnvironment.MapPath("~/"), det.ConnString);

                            //var appDictionary = GetAppData(databaseName);
                            //appsData.TryAdd(databaseName, appDictionary);
                        } */
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using NLog;
using MARS_Repository.ViewModel;
using System.Data;
using Mars_Serialization.Common;

namespace MARS_Web.Helper
{
    public class JsonFileHelper
    {
        static Logger logger = LogManager.GetLogger("Log");
        static Logger ELogger = LogManager.GetLogger("ErrorLog");

        public static bool IsForWeb = true;

        public static string GetFilePath(string fileName, string databaseName,string caseName="Storyboard")
        {
            string fullName = IsForWeb?Path.Combine(HostingEnvironment.MapPath("~/"), "Serialization" , caseName, databaseName,fileName): fileName;
            if (File.Exists(fullName))
            {
                return File.ReadAllText(fullName);
            }

            return string.Empty;
        }

        public static void SaveToJsonFile(string jsonData, string fileName, string databaseName, string caseName = "Storyboard")
        {
            var path = IsForWeb? Path.Combine(HostingEnvironment.MapPath("~/"), "Serialization", caseName, databaseName):fileName;
            if (IsForWeb)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            string fullName = IsForWeb?Path.Combine(path, fileName):fileName;

            using (StreamWriter sw = new StreamWriter(fullName, false))
            {
                sw.Write(jsonData);
            }
        }

        public static bool HasCashFile(string databaseName, string storyboardName, long projectId, long storyBoardId, ref string strTargetName,  string caseName = "Storyboard", string path = "") 
        {
            if (string.IsNullOrEmpty(path))
                path = Path.Combine(HostingEnvironment.MapPath("~/"), "Serialization", caseName, databaseName);
            else
                path = Path.Combine(path, "Serialization", caseName, databaseName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = $"{projectId}_{storyBoardId}_{storyboardName}.json";
            if (File.Exists(strTargetName = Path.Combine(path, fileName))) 
            {
                return true;
            }
            return false;
        }
        public static void InitStoryBoardJson(string databaseName,string path="", long dataId=0,bool needReflesh=false,string strJsonPath=null)
        {
            logger.Info($"InitStoryBoardJson begin...db-[{databaseName}] pth:[{path}] dataId:[{dataId}]");
            MarsConfig mc = null;
            if (string.IsNullOrWhiteSpace(path))
                mc = MarsConfig.Configure(databaseName);
            else
                mc = MarsConfig.Configure(path,databaseName);

            DatabaseConnectionDetails det = mc.GetDatabaseConnectionDetails();
            DBEntities.ConnectionString = det.EntityConnString;
            DBEntities.Schema = databaseName;
            List<Mars_Serialization.ViewModel.ProjectByUser> projectByUserList = new List<Mars_Serialization.ViewModel.ProjectByUser>();
            if (GlobalVariable.UsersDictionary == null || !GlobalVariable.UsersDictionary.ContainsKey(databaseName) || GlobalVariable.UsersDictionary[databaseName] == null)
            {
                string projectQuery = "SELECT PROJECT_ID, PROJECT_NAME, PROJECT_DESCRIPTION, CREATOR, CREATE_DATE, STATUS, (CASE WHEN EXISTS (SELECT 1 FROM REL_PROJECT_USER WHERE  PROJECT_ID = P.PROJECT_ID) THEN 'YES' ELSE 'NO' END) AS PROJECTEXISTS, (SELECT COUNT(*) AS TestSuiteCount FROM REL_TEST_SUIT_PROJECT WHERE PROJECT_ID = P.PROJECT_ID) AS TestSuiteCount, (SELECT COUNT(*) AS StoryBoardCount FROM T_STORYBOARD_SUMMARY tss WHERE tss.ASSIGNED_PROJECT_ID = P.PROJECT_ID and tss.STORYBOARD_NAME is not null) AS StoryBoardCount FROM T_TEST_PROJECT P ORDER BY PROJECT_NAME";
                DataTable projectDatatable = Common.GetRecordAsDatatable(det.ConnString, projectQuery);
                List<Mars_Serialization.ViewModel.ProjectViewModel> projectList = Common.ConvertDataTableToList<Mars_Serialization.ViewModel.ProjectViewModel>(projectDatatable);

                projectByUserList = projectList.Select(u => new Mars_Serialization.ViewModel.ProjectByUser()
                {
                    userId = 0,
                    username = "",
                    ProjectDesc = u.PROJECT_DESCRIPTION,
                    ProjectExists = u.PROJECTEXISTS.Trim().ToUpper() == "YES" ? true : false,
                    ProjectId = u.PROJECT_ID,
                    ProjectName = u.PROJECT_NAME,
                    TestSuiteCount = (int)u.TestSuiteCount,
                    StoryBoardCount = (int)u.StoryBoardCount
                }).ToList();
                var usersData = new ConcurrentDictionary<string, ConcurrentDictionary<Mars_Serialization.ViewModel.UserViewModal, List<Mars_Serialization.ViewModel.ProjectByUser>>> ();
                var userDictionary = Mars_Serialization.JsonSerialization.SerializationFile.GetDictionary(det.ConnString);
                usersData.TryAdd(databaseName, userDictionary);
                GlobalVariable.UsersDictionary = usersData;
            }
            else
            {
                var list = GlobalVariable.UsersDictionary[databaseName].Keys.ToList();
                foreach (var userViewModal in list)
                {
                    projectByUserList.AddRange(userViewModal.Projects.FindAll(r => r.ProjectExists == true).Distinct());
                }
                projectByUserList.Distinct();
            }

            var repTree = new GetTreeRepository();
            var lstoryboardlist = repTree.GetStoryboardList(projectByUserList);
            if (lstoryboardlist != null && lstoryboardlist.Count > 0)
            {
                string strTargetName = "";
                foreach (var project in lstoryboardlist)
                {
                    strTargetName = "";
                    if (dataId != 0 && project.StoryboardId != dataId)
                        continue;
                    Console.WriteLine($"Storyboard:[{project.StoryboardName}]-begins");
                    if (!needReflesh)
                    {
                        if (HasCashFile(databaseName, project.StoryboardName, project.ProjectId, project.StoryboardId, ref strTargetName, "Storyboard", strJsonPath)) // path ))
                        {
                            Console.WriteLine($"{strTargetName} exists, skip");
                            continue;
                        }
                    }
                    //System.Threading.Tasks.Task.Run(() =>
                    //{
                        StoryBoardCashJson(databaseName, det, project.StoryboardName,strTargetName, project.ProjectId, project.StoryboardId, needReflesh);
                    //});
                }
            }
        }

        public static void StoryBoardCashJson(string databaseName, DatabaseConnectionDetails det,string storyBoardName, string strTargetFileName, long Projectid = 0, long Storyboardid = 0,bool needReflesh=false)
        {
            var users = GlobalVariable.UsersDictionary[databaseName].Values.FirstOrDefault();
            var user = users.FirstOrDefault();
            try
            {
                string key =string.IsNullOrEmpty(strTargetFileName)?$"{Projectid}_{Storyboardid}_{storyBoardName}.json": strTargetFileName;
              
                var result = GetFilePath(key, databaseName);
                Console.WriteLine($"\t Prepare {key} data....");
                if (string.IsNullOrWhiteSpace(result) || needReflesh)
                {
                    ConcurrentDictionary<string, string> keyValuePairs = new ConcurrentDictionary<string, string>();
                    //storyboardInfo.TryAdd(key, keyValuePairs);
                    logger.Info(string.Format("open storyborad start | Projectid: {0} | Storyboardid: {1} | Username: {2} ", Projectid, Storyboardid, user.username));
                    StoryBoardRepository repo = new StoryBoardRepository();
                    repo.Username = user.username;

                    /*Dictionary<string, object> keyValues = new Dictionary<string, object>();
                   
                    var datasetList = repo.GetDataSetListNew(Projectid);
                    keyValues.Add("datasetList", datasetList);
                   
                    var actions = repo.GetActions(Storyboardid);
                    keyValues.Add("actions", actions);
                   
                    var testSuiteList = repo.GetTestSuites(Projectid);
                    keyValues.Add("testSuiteList", testSuiteList);
                   
                    var testCaseLists = repo.GetTestCaseListNew(Projectid);
                    keyValues.Add("testCaseLists", testCaseLists);*/
                   
                    
                    var lAppList = repo.GetApplicationListByStoryboardId(Storyboardid);
                    keyValuePairs.TryAdd("applicationlst", JsonConvert.SerializeObject(lAppList));
                    var storyboardname = repo.GetStoryboardById(Storyboardid);
                    keyValuePairs.TryAdd("Storyboardname", storyboardname);
                    
                    var actionList = repo.GetActions(Storyboardid);
                    var testSuiteResult = repo.GetTestSuites(Projectid);
                    var userid = user.userId;
                    var repacc = new ConfigurationGridRepository();
                    repacc.Username = user.username;
                    var gridlst = repacc.GetGridList((long)userid, GridNameList.StoryboradPage);
                    var SPgriddata = GridHelper.GetStoryboardPqgridwidth(gridlst);
                    var Widthgridlst = repacc.GetGridList((long)userid, GridNameList.ResizeLeftPanel);
                    var Rgriddata = GridHelper.GetLeftpanelgridwidth(Widthgridlst);

                    keyValuePairs.TryAdd("ActionList", JsonConvert.SerializeObject(actionList));
                    keyValuePairs.TryAdd("TestSuitesList", JsonConvert.SerializeObject(testSuiteResult));
                    keyValuePairs.TryAdd("gridlst", JsonConvert.SerializeObject(gridlst));
                    keyValuePairs.TryAdd("Widthgridlst", JsonConvert.SerializeObject(Widthgridlst));

                    var projectName = testSuiteResult.FirstOrDefault()?.ProjectName;
                    keyValuePairs.TryAdd("ProjectName", projectName);
                    var lresult = repo.GetStoryBoardDetails(databaseName, det.ConnString, Projectid, Storyboardid);
                    var storyBoardDetails = JsonConvert.SerializeObject(lresult);
                    storyBoardDetails = storyBoardDetails.Replace("\\r", "\\\\r");
                    storyBoardDetails = storyBoardDetails.Replace("\\n", "\\\\n");
                    storyBoardDetails = storyBoardDetails.Replace("   ", "");
                    storyBoardDetails = storyBoardDetails.Replace("\\", "\\\\");
                    storyBoardDetails = storyBoardDetails.Trim();
                    keyValuePairs.TryAdd("StoryBoardDetails", storyBoardDetails);
                    //keyValuePairs.TryAdd("keyValues", JsonConvert.SerializeObject(keyValues));
                    string jsonData = JsonConvert.SerializeObject(keyValuePairs);
                    SaveToJsonFile(jsonData, IsForWeb?$"{Projectid}_{Storyboardid}_{storyboardname}.json":key, databaseName);
                    Console.WriteLine($"\t file {key} is created");

                    logger.Info(string.Format("successfully open storyborad | Projectid: {0} | Storyboardid: {1} | Username: admin", Projectid, Storyboardid));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\t Exceptions:{ex.Message} from StoryBoardCacheJson\r\n{ex.StackTrace}");
                logger.Error(string.Format("Error occured in Home for PartialRightStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, user.username));
                ELogger.ErrorException(string.Format("Error occured in Home for PartialRightStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, user.username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Home for PartialRightStoryboardGrid method | StoryBoard Id : {0} | Project Id : {1} | UserName: {2}", Storyboardid, Projectid, user.username), ex.InnerException);
            }
         }

        public static void ChangeJsonFileName(string originalName, string targetName)
        {
            if (!File.Exists(targetName) && File.Exists(originalName))
            {
                var file = new FileInfo(originalName);
                file.MoveTo(targetName);
                //File.Copy(originalName, targetName);
            }
        }

        public static string GetJsonFilePath(string databaseName, string type)
        {
            return Path.Combine(HostingEnvironment.MapPath("~/"), "Serialization", type, databaseName);
        }

        public static void DeleteFile(string databaseName, long storyBoardId,string storyBoardName, long projectid = 0)
        {
            var filepath = GetJsonFilePath(databaseName, "Storyboard");
            if (projectid != 0)
            {
                var name = $"{projectid}_{storyBoardId}_{storyBoardName}.json";
                var fileName = Path.Combine(filepath, name);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(filepath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i.Name.EndsWith($"_{storyBoardId}_{storyBoardName}.json"))
                    {
                        System.IO.File.Delete(i.FullName);
                    }
                }
            }
        }
    }
}
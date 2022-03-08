using Mars_Serialization.DBConfig;
using Mars_Serialization.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.JsonSerialization
{
    public static class SerializationFile
    {
        public static string conString;
        public static void ChangeConnectionString(string databaseName, string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
                configPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Config\\Mars.config";

            MarsConfiguration mars = MarsConfiguration.Configure(configPath, databaseName);
            DatabaseConnectionDetails det = mars.GetDatabaseConnectionDetails();
            Console.ForegroundColor = ConsoleColor.White;
            //DBEntities.ConnectionString = det.EntityConnString;
            //DBEntities.Schema = databaseName;
            conString = det.ConnString;
            //CreateJsonFiles(det.Schema, Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, det.ConnString);
            //GetDictionary(det.ConnString);
            //GetKeywordList(det.ConnString);
        }
        public static void CreateJsonFiles(string databaseName, string projectPath, string connectionString)
        {
            #region DECLARATION
            string F_Serialization = string.Empty;
            string O_Object = string.Empty;
            string D_Database = string.Empty;
            string K_Keyword = string.Empty;
            string A_Application = string.Empty;
            string T_Testcases = string.Empty;
            #endregion

            #region CREATE FOLDERS
            //string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            F_Serialization = Path.Combine(projectPath, FolderName.Serialization.ToString());
            O_Object = Path.Combine(F_Serialization, FolderName.Object.ToString());
            K_Keyword = Path.Combine(F_Serialization, FolderName.Keyword.ToString());
            A_Application = Path.Combine(F_Serialization, FolderName.Application.ToString());
            T_Testcases = Path.Combine(F_Serialization, FolderName.Testcases.ToString());

            #region CREATE MAIN SERIALIZATION FOLDER
            if (!Directory.Exists(F_Serialization))
                Directory.CreateDirectory(F_Serialization);
            #endregion

            DataTable appData = Common.Common.GetRecordAsDatatable(connectionString, "SELECT * FROM T_REGISTERED_APPS");
            var appList = Common.Common.ConvertDataTableToList<ApplicationViewModel>(appData);

            #region CREATE OBJECT FOLDER
            if (!Directory.Exists(O_Object))
                Directory.CreateDirectory(O_Object);

            if (Directory.Exists(O_Object))
            {
                D_Database = Path.Combine(O_Object, databaseName.Trim());
                if (!Directory.Exists(D_Database))
                    Directory.CreateDirectory(D_Database);
                ApplicationFolder(D_Database, appList, FolderName.Object.ToString());
            }
            #endregion

            #region CREATE KEYWORD FOLDER
            //if (!Directory.Exists(K_Keyword))
            //    Directory.CreateDirectory(K_Keyword);

            //if (Directory.Exists(K_Keyword))
            //{
            //    D_Database = Path.Combine(K_Keyword, databaseName.Trim());
            //    if (!Directory.Exists(D_Database))
            //        Directory.CreateDirectory(D_Database);
            //    ApplicationFolder(D_Database, appList, FolderName.Keyword.ToString());
            //}
            #endregion

            #region CREATE APPLICATION FOLDER
            if (!Directory.Exists(A_Application))
                Directory.CreateDirectory(A_Application);

            if (Directory.Exists(A_Application))
            {
                D_Database = Path.Combine(A_Application, databaseName.Trim());
                if (!Directory.Exists(D_Database))
                    Directory.CreateDirectory(D_Database);
                ApplicationFolder(D_Database, appList, FolderName.Application.ToString());
            }
            #endregion

            #region CREATE TEST CASES FOLDER 
            if (!Directory.Exists(T_Testcases))
                Directory.CreateDirectory(T_Testcases);

            if (Directory.Exists(T_Testcases))
            {
                D_Database = Path.Combine(T_Testcases, databaseName.Trim());
                if (!Directory.Exists(D_Database))
                    Directory.CreateDirectory(D_Database);
                ApplicationFolder(D_Database, appList, FolderName.Testcases.ToString());
            }
            #endregion
            #endregion
        }

        public static void ApplicationFolder(string folderPath, List<ApplicationViewModel> appList, string flag)
        {
            try
            {
                if (appList.Count() > 0)
                {
                    if (FolderName.Object.ToString().Equals(flag))
                    {
                        string allObjectsListQuery = "SELECT * FROM MV_OBJECT_SNAPSHOT";
                        DataTable allAppPegWindowsData = Common.Common.GetRecordAsDatatable(conString, allObjectsListQuery);
                        var allAppPegWindowsList = Common.Common.ConvertDataTableToList<OBJECT_SNAPSHOT>(allAppPegWindowsData);

                        foreach (var app in appList)
                        {
                            #region CREATE APPLICATION FOLDER
                            string appPath = Path.Combine(folderPath, "app_" + app.APPLICATION_ID);
                            if (!Directory.Exists(appPath))
                                Directory.CreateDirectory(appPath);
                            #endregion

                            if (flag.Equals(FolderName.Object.ToString()))
                            {
                                #region CREATE OBJECT JSON FILE IN APPLICATION FOLDER
                                var appPegWindow = allAppPegWindowsList.Where(h => h.APPLICATION_ID == app.APPLICATION_ID && h.TYPE_ID == 1).Distinct().OrderBy(v => v.OBJECT_TYPE).ToList();
                                if (appPegWindow.Count() > 0)
                                {
                                    string parentPagWindowPath = Path.Combine(appPath, "PegWindowsMapping.json");
                                    if (!File.Exists(parentPagWindowPath))
                                    {
                                        string pegWindowJsonData = JsonConvert.SerializeObject(appPegWindow);
                                        pegWindowJsonData = JValue.Parse(pegWindowJsonData).ToString(Formatting.Indented);
                                        File.WriteAllText(parentPagWindowPath, pegWindowJsonData);
                                    }
                                    //if (File.Exists(parentPagWindowPath))
                                    //    File.Delete(parentPagWindowPath);
                                    //File.WriteAllText(parentPagWindowPath, pegWindowJsonData);
                                }

                                var pegW_Char = appPegWindow.Select(x => x.OBJECT_TYPE.ToUpper().FirstOrDefault()).Distinct().ToList();
                                foreach (var P_char in pegW_Char)
                                {
                                    string filePath = Path.Combine(appPath, string.Format("{0}.json", P_char));
                                    if (!File.Exists(filePath))
                                    {
                                        var pegWindow = appPegWindow.Where(x => x.OBJECT_TYPE.ToUpper().Trim().StartsWith(P_char.ToString().ToUpper().Trim())).ToList();
                                        //var finalObjectList = _db.MV_OBJECT_SNAPSHOT.ToList().Where(a => pegWindow.Any(b => a.PEG_ID.Equals(b.PEG_ID))).OrderBy(v => v.PEG_NAME).ToList();
                                        var finalObjectList = allAppPegWindowsList.Where(a => pegWindow.Any(b => a.PEG_ID.Equals(b.PEG_ID))).OrderBy(v => v.PEG_NAME).ToList();

                                        string objectJsonData = JsonConvert.SerializeObject(finalObjectList);
                                        objectJsonData = JValue.Parse(objectJsonData).ToString(Formatting.Indented);
                                        File.WriteAllText(filePath, objectJsonData);
                                    }
                                    //if (File.Exists(filePath))
                                    //    File.Delete(filePath);
                                    //File.WriteAllText(filePath, objectJsonData);
                                }
                                #endregion
                            }
                        }
                    }
                    else if (FolderName.Application.ToString().Equals(flag))
                    {
                        if (appList.Count() > 0)
                        {
                            string parentPagWindowPath = Path.Combine(folderPath, "application.json");
                            if (!File.Exists(parentPagWindowPath))
                            {
                                string applicationJsonData = JsonConvert.SerializeObject(appList);
                                applicationJsonData = JValue.Parse(applicationJsonData).ToString(Formatting.Indented);
                                File.WriteAllText(parentPagWindowPath, applicationJsonData);
                            }
                            //if (File.Exists(parentPagWindowPath))
                            //    File.Delete(parentPagWindowPath);
                            //File.WriteAllText(parentPagWindowPath, applicationJsonData);
                        }
                    }
                    else if (FolderName.Testcases.ToString().Equals(flag))
                    {
                        List<MB_V_TEST_STEPS> testCases = new List<MB_V_TEST_STEPS>();
                        string query = "SELECT CAST(TEST_CASE_ID AS NUMBER(16,0)) AS TEST_CASE_ID, TEST_CASE_NAME, CAST(KEY_WORD_ID AS NUMBER(16,0)) AS KEY_WORD_ID, KEY_WORD_NAME, OBJECT_HAPPY_NAME, CAST(OBJECT_ID AS NUMBER(16,0)) AS OBJECT_ID, OBJECT_TYPE, QUICK_ACCESS, ENUM_TYPE, CAST(APPLICATION_ID AS NUMBER(16,0)) AS APPLICATION_ID, CAST(OBJECT_NAME_ID AS NUMBER(16,0)) AS OBJECT_NAME_ID, TYPE_NAME, STEPS_ID, COLUMN_ROW_SETTING, COMMENTINFO, CAST(IS_RUNNABLE AS NUMBER(16,0)) AS IS_RUNNABLE, CAST(RUN_ORDER AS NUMBER(16,0)) AS RUN_ORDER, VALUE_SETTING FROM V_TEST_STEPS_FULLVISION"; //"SELECT * FROM V_TEST_STEPS_FULLVISION ORDER BY TEST_CASE_ID";
                        DataTable testCaseDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                        testCases = Common.Common.ConvertDataTableToList<MB_V_TEST_STEPS>(testCaseDatatable);
                        if (testCases.Count() > 0)
                        {
                            long[] testCasesId = testCases.Select(x => (long)x.TEST_CASE_ID).Distinct().ToArray();
                            foreach (var id in testCasesId)
                            {
                                bool status = LoadTestcaseJsonFile(folderPath, testCases, id, conString);
                                //var testCasesListById = testCases.Where(x => x.TEST_CASE_ID == id).ToList();
                                //if (testCasesListById.Count() > 0)
                                //{
                                //    string fileName = string.Format("{0}_{1}.json", id, testCasesListById.FirstOrDefault().TEST_CASE_NAME.Replace("/", ""));
                                //    string filePath = Path.Combine(folderPath, fileName);
                                //    if (!File.Exists(filePath))
                                //    {
                                //        long[] AppId = testCasesListById.Select(x => (long)x.APPLICATION_ID).Distinct().ToArray();

                                //        string queryTestSuiteId = "SELECT TEST_SUITE_ID FROM REL_TEST_CASE_TEST_SUITE WHERE TEST_CASE_ID = '" + id + "'";
                                //        DataTable testSuiteIDDatatable = Common.Common.GetRecordAsDatatable(conString, queryTestSuiteId);
                                //        long[] testSuiteID = testSuiteIDDatatable.AsEnumerable().Select(r => r.Field<long>("TEST_SUITE_ID")).ToArray();

                                //        string queryAssignedDataSets = "SELECT ttds.DATA_SUMMARY_ID, ttds.ALIAS_NAME FROM REL_TC_DATA_SUMMARY reltcds INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID WHERE reltcds.TEST_CASE_ID = '" + id + "'";
                                //        DataTable assignedDataSetsDatatable = Common.Common.GetRecordAsDatatable(conString, queryAssignedDataSets);
                                //        List<MB_REL_TC_DATA_SUMMARY> assignedDataSets = new List<MB_REL_TC_DATA_SUMMARY>();
                                //        assignedDataSets = Common.Common.ConvertDataTableToList<MB_REL_TC_DATA_SUMMARY>(assignedDataSetsDatatable);

                                //        testCasesListById.ForEach(x =>
                                //        {
                                //            List<DataForDataSets> DataSettingsValues = new List<DataForDataSets>();
                                //            string queryDataSettingIdAndValues = "SELECT * FROM ( SELECT TESTDS.DATA_VALUE AS DATASETVALUE, testds.Data_Setting_Id AS Data_Setting_Id, CAST(ttds.DATA_SUMMARY_ID AS NUMBER(16,0)) AS DATA_SUMMARY_ID, TESTDS.DATA_DIRECTION AS SKIP FROM REL_TC_DATA_SUMMARY reltcds INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID AND testds.steps_id = ts1.STEPS_ID WHERE reltcds.TEST_CASE_ID = " + x.TEST_CASE_ID + " AND ts1.STEPS_ID = " + x.STEPS_ID + " ORDER BY ttds.ALIAS_NAME)"; //"SELECT * FROM ( SELECT TESTDS.DATA_VALUE AS DATASETVALUE,  testds.Data_Setting_Id AS Data_Setting_Id FROM REL_TC_DATA_SUMMARY reltcds INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID AND testds.steps_id = ts1.STEPS_ID WHERE reltcds.TEST_CASE_ID = " + x.TEST_CASE_ID + " AND ts1.STEPS_ID = " + x.STEPS_ID + " ORDER BY ttds.ALIAS_NAME )";
                                //            DataTable dataSettingIdAndValuesDatatable = Common.Common.GetRecordAsDatatable(conString, queryDataSettingIdAndValues);
                                //            DataSettingsValues = Common.Common.ConvertDataTableToList<DataForDataSets>(dataSettingIdAndValuesDatatable);

                                //            x.dataForDataSets = DataSettingsValues;
                                //        });

                                //        Mars_Memory_TestCase obj = new Mars_Memory_TestCase
                                //        {
                                //            allSteps = testCasesListById.OrderBy(x => x.RUN_ORDER).ToList(),
                                //            assignedApplications = AppId,
                                //            assignedTestSuiteIDs = testSuiteID,
                                //            assignedDataSets = assignedDataSets
                                //        };

                                //        string testcaseJsonData = JsonConvert.SerializeObject(obj);
                                //        testcaseJsonData = JValue.Parse(testcaseJsonData).ToString(Formatting.Indented);

                                //        File.WriteAllText(filePath, testcaseJsonData);
                                //    }
                                //    //if (File.Exists(filePath))
                                //    //    File.Delete(filePath);
                                //    //File.WriteAllText(filePath, testcaseJsonData);
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<UserViewModal> GetUsersList(string conString)
        {
            List<UserViewModal> users = new List<UserViewModal>();
            try
            {
                string query = "SELECT TESTER_ID, TESTER_NAME_LAST, TESTER_NAME_M, TESTER_NAME_F, TESTER_LOGIN_NAME, TESTER_PWD, AVAILABLE_MARK, TESTER_MAIL, TESTER_NUMBER, COMPANY_ID, CREATOR_NAME, CREATE_TIME, TESTER_DESC, (CASE WHEN EXISTS (SELECT 1 FROM T_USER_MAPPING WHERE TESTER_ID = U.TESTER_ID AND IS_DELETED = 1) THEN 'YES' ELSE 'NO' END) AS IS_DELETED FROM T_TESTER_INFO U ORDER BY TESTER_LOGIN_NAME";
                DataTable usersDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                users = Common.Common.ConvertDataTableToList<UserViewModal>(usersDatatable);
                if (users.Count() > 0)
                {
                    users.ForEach(x =>
                    {
                        string projectQuery = "SELECT PROJECT_ID, PROJECT_NAME, PROJECT_DESCRIPTION, CREATOR, CREATE_DATE, STATUS, (CASE WHEN EXISTS (SELECT 1 FROM REL_PROJECT_USER WHERE USER_ID = " + x.TESTER_ID + " AND PROJECT_ID = P.PROJECT_ID) THEN 'YES' ELSE 'NO' END) AS PROJECTEXISTS, (SELECT COUNT(*) AS TestSuiteCount FROM REL_TEST_SUIT_PROJECT WHERE PROJECT_ID = P.PROJECT_ID) AS TestSuiteCount, (SELECT COUNT(*) AS StoryBoardCount FROM T_STORYBOARD_SUMMARY tss WHERE tss.ASSIGNED_PROJECT_ID = P.PROJECT_ID and tss.STORYBOARD_NAME is not null) AS StoryBoardCount FROM T_TEST_PROJECT P ORDER BY PROJECT_NAME";
                        DataTable projectDatatable = Common.Common.GetRecordAsDatatable(conString, projectQuery);
                        List<ProjectViewModel> projectList = Common.Common.ConvertDataTableToList<ProjectViewModel>(projectDatatable);

                        x.Projects = projectList.Select(u => new ProjectByUser()
                        {
                            userId = x.TESTER_ID,
                            username = x.TESTER_LOGIN_NAME,
                            ProjectDesc = u.PROJECT_DESCRIPTION,
                            ProjectExists = u.PROJECTEXISTS.Trim().ToUpper() == "YES" ? true : false,
                            ProjectId = u.PROJECT_ID,
                            ProjectName = u.PROJECT_NAME,
                            TestSuiteCount = (int)u.TestSuiteCount,
                            StoryBoardCount = (int)u.StoryBoardCount
                        }).ToList();

                        string roleQuery = "SELECT R.ROLE_ID, R.ROLE_NAME FROM T_TEST_USER_ROLE_MAPPING UR INNER JOIN T_TEST_ROLES R ON UR.ROLE_ID = R.ROLE_ID WHERE USER_ID = " + x.TESTER_ID + " ORDER BY R.ROLE_NAME";
                        DataTable rolesDatatable = Common.Common.GetRecordAsDatatable(conString, roleQuery);
                        List<UserRoleViewModel> rolesList = Common.Common.ConvertDataTableToList<UserRoleViewModel>(rolesDatatable);
                        x.Roles = rolesList;

                        string PrivilegesQuery = "SELECT PRIVILEGE_ID, DESCRIPTION, MODULE, PRIVILEGE_NAME FROM T_TEST_PRIVILEGE WHERE PRIVILEGE_ID IN (SELECT PRIVILEGE_ID FROM T_TEST_PRIVILEGE_ROLE_MAPPING WHERE ROLE_ID IN (SELECT ROLE_ID FROM T_TEST_USER_ROLE_MAPPING WHERE USER_ID = " + x.TESTER_ID + "))";
                        DataTable privilegesDatatable = Common.Common.GetRecordAsDatatable(conString, PrivilegesQuery);
                        List<UserPrivilegeViewModel> privilegesList = Common.Common.ConvertDataTableToList<UserPrivilegeViewModel>(privilegesDatatable);
                        x.Privileges = privilegesList;
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return users;
        }
        public static List<KeywordViewModel> GetKeywordList(string conString)
        {
            List<KeywordViewModel> keywords = new List<KeywordViewModel>();
            try
            {
                string query = "SELECT KEY_WORD_ID, KEY_WORD_NAME, DESCRIPTION, KEY_WORD_POSITION_ID, ENTRY_IN_DATA_FILE FROM T_KEYWORD";
                DataTable keywordDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                keywords = Common.Common.ConvertDataTableToList<KeywordViewModel>(keywordDatatable);
                if (keywords.Count() > 0)
                {
                    List<DIC_RELATION_KEYWORD> keywordRel = new List<DIC_RELATION_KEYWORD>();
                    string keywordRelQuery = "SELECT RELATION_ID, TYPE_ID, KEY_WORD_ID FROM T_DIC_RELATION_KEYWORD";
                    DataTable keywordRelDatatable = Common.Common.GetRecordAsDatatable(conString, keywordRelQuery);
                    keywordRel = Common.Common.ConvertDataTableToList<DIC_RELATION_KEYWORD>(keywordRelDatatable);

                    keywords.ForEach(x => { x.KeywordType = keywordRel.Where(y => y.KEY_WORD_ID == x.KEY_WORD_ID).ToList(); });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return keywords;
        }
        public static ConcurrentDictionary<UserViewModal, List<ProjectByUser>> GetDictionary(string conString)
        {
            var dictionaryData = new ConcurrentDictionary<UserViewModal, List<ProjectByUser>>();
            var data = GetUsersList(conString);
            if (data.Count() > 0)
            {
                foreach (var item in data)
                {
                    dictionaryData.TryAdd(item, item.Projects);
                }
            }
            return dictionaryData;
        }

        public static void ReloadApplicationFile(List<T_Memory_REGISTERED_APPS> apps, string path, string databaseName)
        {
            string A_Application = Path.Combine(path, FolderName.Serialization.ToString(), FolderName.Application.ToString(), databaseName);
            if (apps.Count() > 0)
            {
                string applicationJsonData = JsonConvert.SerializeObject(apps);
                applicationJsonData = JValue.Parse(applicationJsonData).ToString(Formatting.Indented);
                string parentPagWindowPath = Path.Combine(A_Application, "application.json");
                if (File.Exists(parentPagWindowPath))
                    File.Delete(parentPagWindowPath);
                File.WriteAllText(parentPagWindowPath, applicationJsonData);
            }
        }
        public static List<GroupsViewModel> GetAllGroups(string conString)
        {
            List<GroupsViewModel> groups = new List<GroupsViewModel>();
            try
            {
                string query = "SELECT * FROM T_TEST_GROUP WHERE ACTIVE = 1";
                DataTable groupsDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                groups = Common.Common.ConvertDataTableToList<GroupsViewModel>(groupsDatatable);
            }
            catch (Exception ex)
            {
                throw;
            }
            return groups;
        }
        public static List<FoldersViewModel> GetAllFolders(string conString)
        {
            List<FoldersViewModel> folders = new List<FoldersViewModel>();
            try
            {
                string query = "SELECT * FROM T_TEST_FOLDER WHERE ACTIVE = 1";
                DataTable foldersDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                folders = Common.Common.ConvertDataTableToList<FoldersViewModel>(foldersDatatable);
            }
            catch (Exception ex)
            {
                throw;
            }
            return folders;
        }
        public static List<SetsViewModel> GetAllSets(string conString)
        {
            List<SetsViewModel> sets = new List<SetsViewModel>();
            try
            {
                string query = "SELECT * FROM T_TEST_SET WHERE ACTIVE = 1";
                DataTable setsDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                sets = Common.Common.ConvertDataTableToList<SetsViewModel>(setsDatatable);
            }
            catch (Exception ex)
            {
                throw;
            }
            return sets;
        }
        public static bool LoadTestcaseJsonFile(string folderPath, List<MB_V_TEST_STEPS> testCases, long testcaseId, string conString)
        {
            try
            {
                if (testCases.Count() <= 0)
                {
                    string query = "SELECT CAST(TEST_CASE_ID AS NUMBER(16,0)) AS TEST_CASE_ID, TEST_CASE_NAME, CAST(KEY_WORD_ID AS NUMBER(16,0)) AS KEY_WORD_ID, KEY_WORD_NAME, OBJECT_HAPPY_NAME, CAST(OBJECT_ID AS NUMBER(16,0)) AS OBJECT_ID, OBJECT_TYPE, QUICK_ACCESS, ENUM_TYPE, CAST(APPLICATION_ID AS NUMBER(16,0)) AS APPLICATION_ID, CAST(OBJECT_NAME_ID AS NUMBER(16,0)) AS OBJECT_NAME_ID, TYPE_NAME, STEPS_ID, COLUMN_ROW_SETTING, COMMENTINFO, CAST(IS_RUNNABLE AS NUMBER(16,0)) AS IS_RUNNABLE, CAST(RUN_ORDER AS NUMBER(16,0)) AS RUN_ORDER, VALUE_SETTING FROM V_TEST_STEPS_FULLVISION"; //"SELECT * FROM V_TEST_STEPS_FULLVISION ORDER BY TEST_CASE_ID";
                    DataTable testCaseDatatable = Common.Common.GetRecordAsDatatable(conString, query);
                    testCases = Common.Common.ConvertDataTableToList<MB_V_TEST_STEPS>(testCaseDatatable);
                }
                var testCasesListById = testCases.Where(x => x.TEST_CASE_ID == testcaseId).ToList();
                if (testCasesListById.Count() > 0)
                {
                    string fileName = string.Format("{0}_{1}.json", testcaseId, testCasesListById.FirstOrDefault().TEST_CASE_NAME.Replace("/", ""));
                    string filePath = Path.Combine(folderPath, fileName);
                    if (!File.Exists(filePath))
                    {
                        long[] AppId = testCasesListById.Select(x => (long)x.APPLICATION_ID).Distinct().ToArray();

                        string queryTestSuiteId = "SELECT TEST_SUITE_ID FROM REL_TEST_CASE_TEST_SUITE WHERE TEST_CASE_ID = '" + testcaseId + "'";
                        DataTable testSuiteIDDatatable = Common.Common.GetRecordAsDatatable(conString, queryTestSuiteId);
                        long[] testSuiteID = testSuiteIDDatatable.AsEnumerable().Select(r => r.Field<long>("TEST_SUITE_ID")).ToArray();

                        string queryAssignedDataSets = "SELECT ttds.DATA_SUMMARY_ID, ttds.ALIAS_NAME, ttds.DESCRIPTION_INFO FROM REL_TC_DATA_SUMMARY reltcds INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID WHERE reltcds.TEST_CASE_ID = '" + testcaseId + "'";
                        DataTable assignedDataSetsDatatable = Common.Common.GetRecordAsDatatable(conString, queryAssignedDataSets);
                        List<MB_REL_TC_DATA_SUMMARY> assignedDataSets = new List<MB_REL_TC_DATA_SUMMARY>();
                        assignedDataSets = Common.Common.ConvertDataTableToList<MB_REL_TC_DATA_SUMMARY>(assignedDataSetsDatatable);


                        List<DataForDataSets> DataSettingsValues = new List<DataForDataSets>();
                        var listIds = testCasesListById.Select(x => x.STEPS_ID).ToList();
                        string listIdStr = string.Join(",", listIds);
                        string alldataSettingIdAndValues = "SELECT * FROM ( SELECT TESTDS.DATA_VALUE AS DATASETVALUE, testds.Data_Setting_Id AS Data_Setting_Id, CAST(ttds.DATA_SUMMARY_ID AS NUMBER(16,0)) AS DATA_SUMMARY_ID, TESTDS.DATA_DIRECTION AS SKIP, TS1.STEPS_ID FROM REL_TC_DATA_SUMMARY reltcds INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID AND testds.steps_id = ts1.STEPS_ID WHERE reltcds.TEST_CASE_ID = " + testcaseId + " AND ts1.STEPS_ID IN (" + listIdStr + ") ORDER BY ttds.ALIAS_NAME)";
                        DataTable allDataSettingIdAndValuesDatatable = Common.Common.GetRecordAsDatatable(conString, alldataSettingIdAndValues);
                        DataSettingsValues = Common.Common.ConvertDataTableToList<DataForDataSets>(allDataSettingIdAndValuesDatatable);

                        foreach (var item in testCasesListById)
                        {
                            item.dataForDataSets = DataSettingsValues.Where(x => x.STEPS_ID == item.STEPS_ID).ToList();
                        }

                        //testCasesListById.ForEach(x =>
                        //{
                        //    //List<DataForDataSets> DataSettingsValues = new List<DataForDataSets>();
                        //    //string queryDataSettingIdAndValues = "SELECT * FROM ( SELECT TESTDS.DATA_VALUE AS DATASETVALUE, testds.Data_Setting_Id AS Data_Setting_Id, CAST(ttds.DATA_SUMMARY_ID AS NUMBER(16,0)) AS DATA_SUMMARY_ID, TESTDS.DATA_DIRECTION AS SKIP FROM REL_TC_DATA_SUMMARY reltcds INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID AND testds.steps_id = ts1.STEPS_ID WHERE reltcds.TEST_CASE_ID = " + x.TEST_CASE_ID + " AND ts1.STEPS_ID = " + x.STEPS_ID + " ORDER BY ttds.ALIAS_NAME)"; 
                        //    //DataTable dataSettingIdAndValuesDatatable = Common.Common.GetRecordAsDatatable(conString, queryDataSettingIdAndValues);
                        //    //DataSettingsValues = Common.Common.ConvertDataTableToList<DataForDataSets>(dataSettingIdAndValuesDatatable);
                        //    //x.dataForDataSets = DataSettingsValues;
                        //    //x.dataForDataSets = DataSettingsValues.Where(y => y.STEPS_ID == x.STEPS_ID).ToList();
                        //});

                        Mars_Memory_TestCase obj = new Mars_Memory_TestCase
                        {
                            allSteps = testCasesListById.OrderBy(x => x.RUN_ORDER).ToList(),
                            assignedApplications = AppId,
                            assignedTestSuiteIDs = testSuiteID,
                            assignedDataSets = assignedDataSets
                        };

                        string testcaseJsonData = JsonConvert.SerializeObject(obj);
                        testcaseJsonData = JValue.Parse(testcaseJsonData).ToString(Formatting.Indented);

                        File.WriteAllText(filePath, testcaseJsonData);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region ENUM
        public enum FolderName
        {
            Serialization,
            Object,
            Keyword,
            Application,
            Testcases
        }
        #endregion
    }
}

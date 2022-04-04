﻿using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MARS_Repository.ViewModel;
using Oracle.ManagedDataAccess.Client;

namespace MARS_Web.Helper
{
    public class InitCacheHelper
    {
        public static void InitAll(string entityConnString, string connString,string databaseName)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            var repTree = new GetTreeRepository();
            ProjectInit(entityConnString,databaseName, repTree);
            StoryBoardInit(entityConnString,databaseName, repTree);
            TestSuitInit(entityConnString,databaseName, repTree, connString);
            TestCaseInit(entityConnString,databaseName, repTree, connString);
            DataSetInit(entityConnString,databaseName, repTree,  connString);
            ActionsInit(entityConnString, databaseName, repTree);
            FolderInit(entityConnString, databaseName, repTree);
            FolderFilterInit(entityConnString, databaseName, repTree);
            RelFolderFilterInit(entityConnString, databaseName, repTree);
            AppInit(entityConnString, databaseName, repTree);
            GroupInit(entityConnString, databaseName, repTree);
            DataSetTagInit(entityConnString, databaseName, repTree);
            SetInit(entityConnString, databaseName, repTree);
        }

        public static void ProjectInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var project = repTree.GetProjectListCache();
                if (GlobalVariable.ProjectListCache.ContainsKey(databaseName))
                    GlobalVariable.ProjectListCache[databaseName] = project;
                else
                    GlobalVariable.ProjectListCache.TryAdd(databaseName, project);
            });
        }

        public static void TestSuitInit(String entityConnString, string databaseName, GetTreeRepository repTree,string connString)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var testSuit = repTree.GetTestSuiteListCache(connString);
                if (GlobalVariable.TestSuiteListCache.ContainsKey(databaseName))
                    GlobalVariable.TestSuiteListCache[databaseName] = testSuit;
                else
                    GlobalVariable.TestSuiteListCache.TryAdd(databaseName, testSuit);
            });
        }

        public static void StoryBoardInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var storyboard = repTree.GetStoryboardListCache();
                if (GlobalVariable.StoryBoardListCache.ContainsKey(databaseName))
                    GlobalVariable.StoryBoardListCache[databaseName] = storyboard;
                else
                    GlobalVariable.StoryBoardListCache.TryAdd(databaseName, storyboard);
            });
        }

        public static void TestCaseInit(String entityConnString, string databaseName, GetTreeRepository repTree, string connString)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var testCase = repTree.GetTestCaseListCache(connString);
                if (GlobalVariable.TestCaseListCache.ContainsKey(databaseName))
                    GlobalVariable.TestCaseListCache[databaseName] = testCase;
                else
                    GlobalVariable.TestCaseListCache.TryAdd(databaseName, testCase);
            });
        }

        public static void DataSetInit(String entityConnString, string databaseName, GetTreeRepository repTree, string connString)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var dataSet = repTree.GetDataSetListCache(connString);
                if (GlobalVariable.DataSetListCache.ContainsKey(databaseName))
                    GlobalVariable.DataSetListCache[databaseName] = dataSet;
                else
                    GlobalVariable.DataSetListCache.TryAdd(databaseName, dataSet);
            });
        }

        public static void ActionsInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var actions = repTree.GetActionsCache();
                if (GlobalVariable.ActionsCache.ContainsKey(databaseName))
                    GlobalVariable.ActionsCache[databaseName] = actions;
                else
                    GlobalVariable.ActionsCache.TryAdd(databaseName, actions);
            });
        }

        public static void FolderInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var folders = repTree.GetFolderCache();
                if (GlobalVariable.FolderListCache.ContainsKey(databaseName))
                    GlobalVariable.FolderListCache[databaseName] = folders;
                else
                    GlobalVariable.FolderListCache.TryAdd(databaseName, folders);
            });
        }

        public static void FolderFilterInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var folders = repTree.GetFilterCache();
                if (GlobalVariable.FolderFilterListCache.ContainsKey(databaseName))
                    GlobalVariable.FolderFilterListCache[databaseName] = folders;
                else
                    GlobalVariable.FolderFilterListCache.TryAdd(databaseName, folders);
            });
        }

        public static void RelFolderFilterInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var folders = repTree.GetRelFolderFilterCache();
                if (GlobalVariable.RelFolderFilterListCache.ContainsKey(databaseName))
                    GlobalVariable.RelFolderFilterListCache[databaseName] = folders;
                else
                    GlobalVariable.RelFolderFilterListCache.TryAdd(databaseName, folders);
            });
        }

        public static void AppInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var apps = repTree.GetAppCache();
                if (GlobalVariable.AppListCache.ContainsKey(databaseName))
                    GlobalVariable.AppListCache[databaseName] = apps;
                else
                    GlobalVariable.AppListCache.TryAdd(databaseName, apps);
            });
        }


        public static void FolderAddOrEdit(string databaseName,  DataTagCommonViewModel lEntity,bool isadd=false,string username="")
        {
            if (GlobalVariable.FolderListCache != null && GlobalVariable.FolderListCache.ContainsKey(databaseName))
            {
                if (isadd)
                {
                    GlobalVariable.FolderListCache[databaseName].Add(new T_TEST_FOLDER()
                    {
                        FOLDERNAME = lEntity.Name,
                        DESCRIPTION = lEntity.Description,
                        ACTIVE = lEntity.IsActive,
                        CREATION_DATE = DateTime.Now,
                        UPDATE_DATE = DateTime.Now,
                        CREATION_USER = username,
                        UPDATE_CREATION_USER = username,
                        FOLDERID = lEntity.Id
                    });
                }
                else
                {
                    var folder = GlobalVariable.FolderListCache[databaseName].FirstOrDefault(r => r.FOLDERID == lEntity.Id);
                    if (folder!=null)
                    {
                        folder.DESCRIPTION = lEntity.Description;
                        folder.ACTIVE = lEntity.IsActive;
                    }
                }
            
            }
        }

        public static void GroupInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var folders = repTree.GetGroupCache();
                if (GlobalVariable.GroupListCache.ContainsKey(databaseName))
                    GlobalVariable.GroupListCache[databaseName] = folders;
                else
                    GlobalVariable.GroupListCache.TryAdd(databaseName, folders);
            });
        }

        public static void DataSetTagInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var folders = repTree.GetDataSetTagCache();
                if (GlobalVariable.DataSetTagListCache.ContainsKey(databaseName))
                    GlobalVariable.DataSetTagListCache[databaseName] = folders;
                else
                    GlobalVariable.DataSetTagListCache.TryAdd(databaseName, folders);
            });
        }

        public static void SetInit(String entityConnString, string databaseName, GetTreeRepository repTree)
        {
            DBEntities.ConnectionString = entityConnString;
            DBEntities.Schema = databaseName;
            Task.Run(() =>
            {
                var folders = repTree.GetSetCache();
                if (GlobalVariable.SetListCache.ContainsKey(databaseName))
                    GlobalVariable.SetListCache[databaseName] = folders;
                else
                    GlobalVariable.SetListCache.TryAdd(databaseName, folders);
            });
        }

        public static bool GetProjectUserFromCache(string lSchema,decimal testid,List<ProjectByUser> projectList)
        {
            bool result = false;
            if (GlobalVariable.UsersDictionary != null && GlobalVariable.UsersDictionary.ContainsKey(lSchema) &&
                GlobalVariable.ProjectListCache != null && GlobalVariable.ProjectListCache.ContainsKey(lSchema) &&
                GlobalVariable.StoryBoardListCache != null && GlobalVariable.StoryBoardListCache.ContainsKey(lSchema)&&
                GlobalVariable.TestSuiteListCache != null && GlobalVariable.TestSuiteListCache.ContainsKey(lSchema))
            {
                var key = GlobalVariable.UsersDictionary[lSchema].Keys.FirstOrDefault(r => r.TESTER_ID == testid);
                if (key != null)
                {
                    var list = GlobalVariable.UsersDictionary[lSchema][key].Select(r => r.ProjectId);
                    var projects = GlobalVariable.ProjectListCache[lSchema].FindAll(r => list.Contains(r.PROJECT_ID));

                    foreach (var project in projects)
                    {
                        var p = new ProjectByUser()
                        {
                            ProjectId = project.PROJECT_ID,
                            userId = testid,
                            ProjectName = project.PROJECT_NAME,
                            ProjectDesc = project.PROJECT_DESCRIPTION 
                        };
                        p.TestSuiteCount = GlobalVariable.TestSuiteListCache[lSchema].Count(r => r.ProjectId == project.PROJECT_ID);
                        p.StoryBoardCount = GlobalVariable.StoryBoardListCache[lSchema].Count(r =>r!=null && r.ProjectId == project.PROJECT_ID);
                        projectList.Add(p);
                    }
                    result = true;
                }
            }

            return result;
        }

        public static bool CheckStoryBoardFromCache(string lSchema, string storyBoardName, long storyBoardId)
        {
            bool result = false;
            if (storyBoardId != null && storyBoardId > 0)
            {
                result = GlobalVariable.StoryBoardListCache[lSchema].Any(r => r.StoryboardName.ToLower().Trim() == storyBoardName.ToLower().Trim()
                     && r.StoryboardId != storyBoardId);
            }
            else
            {
                result = GlobalVariable.StoryBoardListCache[lSchema].Any(r => r.StoryboardName.ToLower().Trim() == storyBoardName.ToLower().Trim());
            }

            return result;
        }

        public static bool CheckTestSuiteFromCache(string lSchema, string suiteName, long? suiteId)
        {
            bool result = false;
            if (suiteId != null && suiteId > 0)
            {
                result = GlobalVariable.TestSuiteListCache[lSchema].Any(r => r.TestsuiteName.ToLower().Trim() == suiteName.ToLower().Trim()
                     && r.TestsuiteId != suiteId);
            }
            else
            {
                result = GlobalVariable.TestSuiteListCache[lSchema].Any(r => r.TestsuiteName.ToLower().Trim() == suiteName.ToLower().Trim());
            }

            return result;
        }

        public static void RefreshTestSuiteName(string lSchema, string suiteName, string suiteDesc, long suiteId)
        {
            if (GlobalVariable.TestSuiteListCache != null && GlobalVariable.TestSuiteListCache.ContainsKey(lSchema))
            {
                var suites = GlobalVariable.TestSuiteListCache[lSchema].FindAll(r => r.TestsuiteId == suiteId);
                suites.ForEach(r => {
                    r.TestsuiteName = suiteName;
                    r.TestSuiteDesc = suiteDesc;
                });
            }

            if (GlobalVariable.TestCaseListCache != null && GlobalVariable.TestCaseListCache.ContainsKey(lSchema))
            {
                var cases = GlobalVariable.TestCaseListCache[lSchema].FindAll(r => r.TestsuiteId == suiteId);
                cases.ForEach(r => {
                    r.TestsuiteName = suiteName;
                 });
            }

            if (GlobalVariable.DataSetListCache != null && GlobalVariable.DataSetListCache.ContainsKey(lSchema))
            {
                var datasets = GlobalVariable.DataSetListCache[lSchema].FindAll(r => r.TestsuiteId == suiteId);
                datasets.ForEach(r => {
                    r.TestsuiteName = suiteName;
                 });
            }
        }

        public static void DeleteTestCase(string lSchema, long testCaseId)
        {   
            if (GlobalVariable.TestCaseListCache != null && GlobalVariable.TestCaseListCache.ContainsKey(lSchema))
            {
                  GlobalVariable.TestCaseListCache[lSchema].RemoveAll(r => r.TestcaseId == testCaseId);
                 
            }

            if (GlobalVariable.DataSetListCache != null && GlobalVariable.DataSetListCache.ContainsKey(lSchema))
            {
                 GlobalVariable.DataSetListCache[lSchema].RemoveAll(r => r.TestcaseId == testCaseId);
            }
        }

    }
}
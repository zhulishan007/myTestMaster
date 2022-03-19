using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

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


    }
}
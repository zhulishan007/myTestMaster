using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using Mars_Serialization.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Hosting;
using static Mars_Serialization.JsonSerialization.SerializationFile;

namespace MARS_Web.Helper
{
    public static class GlobalVariable
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<UserViewModal, List<Mars_Serialization.ViewModel.ProjectByUser>>> userInfo = null;
        public static ConcurrentDictionary<string, ConcurrentDictionary<UserViewModal, List<Mars_Serialization.ViewModel.ProjectByUser>>> UsersDictionary { 
            get => userInfo; 
            set => userInfo=value; }
        public static ConcurrentDictionary<string, List<T_Memory_REGISTERED_APPS>> AllApps { get; set; }
        public static ConcurrentDictionary<string, List<Mars_Serialization.ViewModel.KeywordViewModel>> AllKeywords { get; set; }
        public static ConcurrentDictionary<string, List<GroupsViewModel>> AllGroups { get; set; }
        public static ConcurrentDictionary<string, List<FoldersViewModel>> AllFolders { get; set; }
        public static ConcurrentDictionary<string, List<SetsViewModel>> AllSets { get; set; }

        public static ConcurrentDictionary<string, List<StoryBoardListByProject>> StoryBoardListCache { get; set; }
        public static ConcurrentDictionary<string, List<TestCaseListByProject>> TestCaseListCache { get; set; }
        public static ConcurrentDictionary<string, List<DataSetListByTestCase>> DataSetListCache { get; set; }
        public static ConcurrentDictionary<string, List<TestSuiteListByProject>> TestSuiteListCache { get; set; }
        public static ConcurrentDictionary<string, List<T_TEST_PROJECT>> ProjectListCache { get; set; }
        public static ConcurrentDictionary<string, List<SYSTEM_LOOKUP>> ActionsCache { get; set; }
        
        public static ConcurrentDictionary<string, List<T_TEST_FOLDER>> FolderListCache { get; set; }

        public static ConcurrentDictionary<string, List<T_FOLDER_FILTER>> FolderFilterListCache { get; set; }

        public static ConcurrentDictionary<string, List<REL_FOLDER_FILTER>> RelFolderFilterListCache { get; set; }
        public static ConcurrentDictionary<string, List<T_REGISTERED_APPS>> AppListCache { get; set; }

        public static ConcurrentDictionary<string, List<T_TEST_GROUP>> GroupListCache { get; set; }
        public static ConcurrentDictionary<string, List<T_TEST_SET>> SetListCache { get; set; }    
        public static ConcurrentDictionary<string, List<T_TEST_DATASETTAG>> DataSetTagListCache { get; set; }
    }

    //public static class ConvertJsonToList
    //{
    //    public static List<T> ConvertJsonStringToList<T>(string filePath) where T : class, new()
    //    {
    //        List<T> lstItems = new List<T>();
    //        string fullPath = Path.Combine(HostingEnvironment.MapPath("~/"), FolderName.Serialization.ToString(), FolderName.Application.ToString(), SessionManager.Schema, "application.json");
    //        if (System.IO.File.Exists(fullPath))
    //        {
    //            string jsongString = System.IO.File.ReadAllText(fullPath);
    //            lstItems = JsonConvert.DeserializeObject<List<T>>(jsongString);
    //        }            
    //        return lstItems;
    //    }

    //    public static List<T> Deserialize<T>(string jsonString)
    //    {
    //        using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
    //        {
    //            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
    //            return (List<T>)serializer.ReadObject(ms);
    //        }
    //    }
    //}
}
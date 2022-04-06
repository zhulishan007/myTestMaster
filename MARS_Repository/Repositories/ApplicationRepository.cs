using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Mars_Serialization.Common.CommonEnum;

namespace MARS_Repository.Repositories
{

    public class ApplicationRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;
        public string currentPath = string.Empty;

        public List<T_REGISTERED_APPS> ListApplication()
        {
            try
            {
                Helper.WriteLogMessage(string.Format("List Application start | UserName: {0}", Username), currentPath);
                var result = enty.T_REGISTERED_APPS.ToList();
                Helper.WriteLogMessage(string.Format("List Application end | UserName: {0}", Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in ListApplication method | UserName: {0} | Error: {1}", Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in ListApplication method | UserName: {0} | Error: {1}", Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public List<string> ListApplicationObjectExport()
        {
            try
            {
                logger.Info(string.Format("List Application start | UserName: {0}", Username), currentPath);
                var list = enty.T_REGISTERED_APPS.Select(x => x.APP_SHORT_NAME).ToList();
                logger.Info(string.Format("List Application end | UserName: {0}", Username), currentPath);
                return list;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in ListApplicationObjectExport method | UserName: {0}", Username), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in ListApplicationObjectExport method | UserName: {0}", Username), currentPath);
                throw;
            }
        }
        public ApplicationViewModel GetApplicationDetail(long AppId)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get Application Detail start | AppId: {0} | Username: {1}", AppId, Username), currentPath);
                var lResult = new ApplicationViewModel();
                var lList = enty.T_REGISTERED_APPS.Where(x => x.APPLICATION_ID == AppId).Select(y => new ApplicationViewModel
                {
                    ApplicationId = y.APPLICATION_ID,
                    ApplicationName = y.APP_SHORT_NAME,
                    IS64BIT = y.IS64BIT
                }).ToList();

                if (lList.Count() > 0)
                {
                    lResult = lList.FirstOrDefault();
                }
                Helper.WriteLogMessage(string.Format("Get Application Detail end | AppId: {0} | Username: {1}", AppId, Username), currentPath);
                return lResult;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in GetApplicationDetail method | AppId: {0} | UserName: {1} | Error: {2}", AppId, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in GetApplicationDetail method | AppId: {0} | UserName: {1} | Error: {2}", AppId, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public List<ApplicationViewModel> GetApplicationList()
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get Application List start | UserName: {0}", Username), currentPath);
                List<ApplicationViewModel> applst = new List<ApplicationViewModel>();
                var lapp = enty.T_REGISTERED_APPS.ToList();
                foreach (var item in lapp)
                {
                    ApplicationViewModel objApplicationviewmodel = new ApplicationViewModel();
                    objApplicationviewmodel.ApplicationId = item.APPLICATION_ID;
                    objApplicationviewmodel.ApplicationName = item.APP_SHORT_NAME;
                    objApplicationviewmodel.Description = item.PROCESS_IDENTIFIER;
                    objApplicationviewmodel.Version = item.VERSION;
                    objApplicationviewmodel.ExtraRequirement = item.EXTRAREQUIREMENT;
                    if (item.ISBASELINE != null)
                        objApplicationviewmodel.Mode = (item.ISBASELINE == 1 ? "Baseline" : "Compare");
                    else
                        objApplicationviewmodel.Mode = "";
                    if (item.IS64BIT != null)
                        objApplicationviewmodel.Bits = (item.IS64BIT == 1 ? "64 bits" : "32 bits");
                    else
                        objApplicationviewmodel.Bits = "";

                    objApplicationviewmodel.BitsId = (item.IS64BIT == null ? "" : item.IS64BIT.ToString());

                    applst.Add(objApplicationviewmodel);
                }
                Helper.WriteLogMessage(string.Format("Get Application List end | UserName: {0}", Username), currentPath);

                return applst;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in GetApplicationList method | UserName: {0} | Error: {1}", Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in GetApplicationList method | UserName: {0} | Error: {1}", Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public bool CheckDuplicateApplicationNameExist(string applicationname, long? ApplicationId)
        {
            try
            {
                var lresult = false;
                Helper.WriteLogMessage(string.Format("Check Duplicate ApplicationName Exist start | UserName: {0}", Username), currentPath);
                if (ApplicationId != null)
                {
                    lresult = enty.T_REGISTERED_APPS.Any(x => x.APPLICATION_ID != ApplicationId && x.APP_SHORT_NAME.ToLower().Trim() == applicationname.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_REGISTERED_APPS.Any(x => x.APP_SHORT_NAME.ToLower().Trim() == applicationname.ToLower().Trim());
                }
                Helper.WriteLogMessage(string.Format("Check Duplicate ApplicationName Exist end | UserName: {0}", Username), currentPath);
                return lresult;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in CheckDuplicateApplicationNameExist method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in CheckDuplicateApplicationNameExist method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public bool AddEditApplicationFromDictionary(Mars_Serialization.ViewModel.T_Memory_REGISTERED_APPS objApp)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var flag = false;
                    if (objApp.currentSyncroStatus.Equals(MarsRecordStatus.en_NewToDb))
                    {
                        Helper.WriteLogMessage(string.Format("Add application start | Application: {0} | Username: {1}", objApp.APP_SHORT_NAME, Username), currentPath);
                        var RegisterTbl = new T_REGISTERED_APPS
                        {
                            APPLICATION_ID = objApp.APPLICATION_ID,
                            APP_SHORT_NAME = objApp.APP_SHORT_NAME,
                            PROCESS_IDENTIFIER = objApp.PROCESS_IDENTIFIER,
                            VERSION = objApp.VERSION,
                            RECORD_CREATE_PERSON = objApp.RECORD_CREATE_PERSON,
                            EXTRAREQUIREMENT = objApp.EXTRAREQUIREMENT,
                            RECORD_CREATE_DATE = objApp.RECORD_CREATE_DATE,
                            ISBASELINE = objApp.ISBASELINE,
                            IS64BIT = objApp.IS64BIT,
                            STARTER_COMMAND = objApp.STARTER_COMMAND
                        };
                        enty.T_REGISTERED_APPS.Add(RegisterTbl);
                        enty.SaveChanges();
                        flag = true;
                        Helper.WriteLogMessage(string.Format("Add application end | Application: {0} | Username: {1}", RegisterTbl.APP_SHORT_NAME, Username), currentPath);
                    }
                    else if (objApp.currentSyncroStatus.Equals(MarsRecordStatus.en_ModifiedToDb))
                    {
                        var RegisterTbl = enty.T_REGISTERED_APPS.Find(objApp.APPLICATION_ID);
                        Helper.WriteLogMessage(string.Format("Edit application start | Application: {0} | ApplicationId: {1} | Username: {2}", objApp.APP_SHORT_NAME, objApp.APPLICATION_ID, Username), currentPath);
                        if (RegisterTbl != null)
                        {
                            RegisterTbl.APP_SHORT_NAME = objApp.APP_SHORT_NAME;
                            RegisterTbl.PROCESS_IDENTIFIER = objApp.PROCESS_IDENTIFIER;
                            RegisterTbl.VERSION = objApp.VERSION;
                            RegisterTbl.EXTRAREQUIREMENT = objApp.EXTRAREQUIREMENT;
                            RegisterTbl.ISBASELINE = objApp.ISBASELINE;
                            RegisterTbl.IS64BIT = objApp.IS64BIT;
                            RegisterTbl.STARTER_COMMAND = objApp.STARTER_COMMAND;
                            enty.SaveChanges();
                        }
                        flag = true;
                        Helper.WriteLogMessage(string.Format("Edit application end | Application: {0} | ApplicationId: {1} | Username: {2}", objApp.APP_SHORT_NAME, objApp.APPLICATION_ID, Username), currentPath);
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1} | Error: {2}", objApp.APPLICATION_ID, Username, ex), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1} | Error: {2}", objApp.APPLICATION_ID, Username, ex.InnerException), currentPath);
                throw;
            }
        }
        public long GetApplicationSequence(string SeqName)
        {
            return Helper.NextTestSuiteId(SeqName);
        }
        public bool AddEditApplication(ApplicationViewModel AppModelEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(AppModelEntity.ApplicationName))
                    {
                        AppModelEntity.ApplicationName = AppModelEntity.ApplicationName.Trim();
                    }
                    var flag = false;
                    if (AppModelEntity.ApplicationId == 0)
                    {
                        Helper.WriteLogMessage(string.Format("Add application start | Application: {0} | Username: {1}", AppModelEntity.ApplicationName, Username), currentPath);

                        var RegisterTbl = new T_REGISTERED_APPS();
                        RegisterTbl.APPLICATION_ID = Helper.NextTestSuiteId("T_REGISTERED_APPS_SEQ");
                        RegisterTbl.APP_SHORT_NAME = AppModelEntity.ApplicationName;
                        RegisterTbl.PROCESS_IDENTIFIER = AppModelEntity.Description;
                        RegisterTbl.VERSION = AppModelEntity.Version;
                        RegisterTbl.RECORD_CREATE_PERSON = AppModelEntity.Create_Person;
                        RegisterTbl.EXTRAREQUIREMENT = AppModelEntity.ExtraRequirement;
                        RegisterTbl.RECORD_CREATE_DATE = DateTime.Now;
                        if (!string.IsNullOrEmpty(AppModelEntity.Mode))
                            RegisterTbl.ISBASELINE = (AppModelEntity.Mode == "Baseline" ? 1 : 0);
                        else
                            RegisterTbl.ISBASELINE = null;

                        if (!string.IsNullOrEmpty(AppModelEntity.BitsId))
                            RegisterTbl.IS64BIT = Convert.ToInt32(AppModelEntity.BitsId);
                        else
                            RegisterTbl.IS64BIT = null;

                        AppModelEntity.ApplicationId = RegisterTbl.APPLICATION_ID;
                        enty.T_REGISTERED_APPS.Add(RegisterTbl);
                        enty.SaveChanges();

                        flag = true;
                        Helper.WriteLogMessage(string.Format("Add application end | Application: {0} | Username: {1}", AppModelEntity.ApplicationName, Username), currentPath);

                    }
                    else
                    {
                        var RegisterTbl = enty.T_REGISTERED_APPS.Find(AppModelEntity.ApplicationId);
                        Helper.WriteLogMessage(string.Format("Edit application start | Application: {0} | ApplicationId: {1} | Username: {2}", AppModelEntity.ApplicationName, AppModelEntity.ApplicationId, Username), currentPath);
                        if (RegisterTbl != null)
                        {
                            RegisterTbl.APP_SHORT_NAME = AppModelEntity.ApplicationName;
                            RegisterTbl.PROCESS_IDENTIFIER = AppModelEntity.Description;
                            RegisterTbl.VERSION = AppModelEntity.Version;
                            RegisterTbl.EXTRAREQUIREMENT = AppModelEntity.ExtraRequirement;
                            if (!string.IsNullOrEmpty(AppModelEntity.Mode))
                                RegisterTbl.ISBASELINE = (AppModelEntity.Mode == "Baseline" ? 1 : 0);
                            else
                                RegisterTbl.ISBASELINE = null;

                            if (!string.IsNullOrEmpty(AppModelEntity.BitsId))
                                RegisterTbl.IS64BIT = Convert.ToInt32(AppModelEntity.BitsId);
                            else
                                RegisterTbl.IS64BIT = null;
                            enty.SaveChanges();
                        }
                        flag = true;
                        Helper.WriteLogMessage(string.Format("Edit application end | Application: {0} | ApplicationId: {1} | Username: {2}", AppModelEntity.ApplicationName, AppModelEntity.ApplicationId, Username), currentPath);
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1} | Error: {2}", AppModelEntity.ApplicationId, Username, ex), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1} | Error: {2}", AppModelEntity.ApplicationId, Username, ex.InnerException), currentPath);
                throw;
            }
        }
        public List<string> CheckTestCaseExistsInAppliction(long ApplicationId)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Check TestCase Exists In Appliction start | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                List<string> Applicationname  = (from t1 in enty.REL_APP_PROJ
                             join t2 in enty.T_TEST_PROJECT on t1.PROJECT_ID equals t2.PROJECT_ID
                             where t1.APPLICATION_ID == ApplicationId
                             select t2.PROJECT_NAME).ToList();

                /*var lApplicationList = enty.REL_APP_PROJ.Where(x => x.APPLICATION_ID == ApplicationId).ToList();

                if (lApplicationList.Count() > 0)
                {
                    foreach (var item in lApplicationList)
                    {
                        var sname = enty.T_TEST_PROJECT.Find(item.PROJECT_ID);

                        Applicationname.Add(sname.PROJECT_NAME);
                        Applicationname = (from w in Applicationname select w).Distinct().ToList();
                    }
                    logger.Info(string.Format("Check TestCase Exists In Appliction end | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                    return Applicationname;
                }*/
                Helper.WriteLogMessage(string.Format("Check TestCase Exists In Appliction end | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                return Applicationname;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in CheckTestCaseExistsInAppliction method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in CheckTestCaseExistsInAppliction method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex.InnerException), currentPath);
                throw;
            }

        }
        public bool DeleteApplication(long ApplicationId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Helper.WriteLogMessage(string.Format("Delete Application start | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                    var flag = false;
                    var result = enty.T_REGISTERED_APPS.FirstOrDefault(x => x.APPLICATION_ID == ApplicationId);
                    if (result != null)
                    {
                        var relappprj = enty.REL_APP_PROJ.Where(x => x.APPLICATION_ID == ApplicationId).ToList();
                        foreach (var item in relappprj)
                        {
                            enty.REL_APP_PROJ.Remove(item);
                            enty.SaveChanges();
                        }
                        var relsuiteprj = enty.REL_APP_TESTSUITE.Where(x => x.APPLICATION_ID == ApplicationId).ToList();
                        foreach (var item in relsuiteprj)
                        {
                            enty.REL_APP_TESTSUITE.Remove(item);
                            enty.SaveChanges();
                        }
                        var reltestcaseprj = enty.REL_APP_TESTCASE.Where(x => x.APPLICATION_ID == ApplicationId).ToList();
                        foreach (var item in reltestcaseprj)
                        {
                            enty.REL_APP_TESTCASE.Remove(item);
                            enty.SaveChanges();
                        }
                        var relobjprj = enty.REL_OBJ_APP.Where(x => x.APPLICATION_ID == ApplicationId).ToList();
                        foreach (var item in relobjprj)
                        {
                            enty.REL_OBJ_APP.Remove(item);
                            enty.SaveChanges();
                        }

                        enty.T_REGISTERED_APPS.Remove(result);
                        enty.SaveChanges();
                        flag = true;
                    }
                    Helper.WriteLogMessage(string.Format("Delete Application end | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in DeleteApplication method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in DeleteApplication method | AppId: {0} | UserName: {1} | Error:{2}", ApplicationId, Username, ex.InnerException), currentPath);
                throw;
            }

        }
        public String GetApplicationNameById(long ApplicationId)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get ApplicationName start | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                var result = enty.T_REGISTERED_APPS.FirstOrDefault(x => x.APPLICATION_ID == ApplicationId).APP_SHORT_NAME;
                Helper.WriteLogMessage(string.Format("Get ApplicationName end | ApplicationId: {0} | Username: {1}", ApplicationId, Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured Application in GetApplicationNameById method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured Application in GetApplicationNameById method | AppId: {0} | UserName: {1} | Error: {2}", ApplicationId, Username, ex.InnerException), currentPath);
                throw;
            }
        }
    }
}

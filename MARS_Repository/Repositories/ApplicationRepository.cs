using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.Repositories
{

    public class ApplicationRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        public List<T_REGISTERED_APPS> ListApplication()
        {
            try
            {
                logger.Info(string.Format("List Application start | UserName: {0}", Username));
                var result = enty.T_REGISTERED_APPS.ToList();
                logger.Info(string.Format("List Application end | UserName: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in ListApplication method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Application in ListApplication method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in ListApplication method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public List<string> ListApplicationObjectExport()
        {
            try
            {
                logger.Info(string.Format("List Application start | UserName: {0}", Username));
                var list = enty.T_REGISTERED_APPS.Select(x => x.APP_SHORT_NAME).ToList();
                logger.Info(string.Format("List Application end | UserName: {0}", Username));
                return list;
            }
            catch(Exception ex)
            {
                logger.Error(string.Format("Error occured Application in ListApplicationObjectExport method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Application in ListApplicationObjectExport method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in ListApplicationObjectExport method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public ApplicationViewModel GetApplicationDetail(long AppId)
        {
            try
            {
                logger.Info(string.Format("Get Application Detail start | AppId: {0} | Username: {1}", AppId, Username));
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
                logger.Info(string.Format("Get Application Detail end | AppId: {0} | Username: {1}", AppId, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in GetApplicationDetail method | AppId: {0} | UserName: {1}", AppId, Username));
                ELogger.ErrorException(string.Format("Error occured Application in GetApplicationDetail method | AppId: {0} | UserName: {1}", AppId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in GetApplicationDetail method | AppId: {0} | UserName: {1}", AppId, Username), ex.InnerException);
                throw;
            }
        }

        public List<ApplicationViewModel> GetApplicationList()
        {
            try
            {
                logger.Info(string.Format("Get Application List start | UserName: {0}", Username));
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
                logger.Info(string.Format("Get Application List end | UserName: {0}", Username));

                return applst;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in GetApplicationList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Application in GetApplicationList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in GetApplicationList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateApplicationNameExist(string applicationname, long? ApplicationId)
        {
            try
            {
                var lresult = false;
                logger.Info(string.Format("Check Duplicate ApplicationName Exist start | UserName: {0}", Username));
                if (ApplicationId != null)
                {
                    lresult = enty.T_REGISTERED_APPS.Any(x => x.APPLICATION_ID != ApplicationId && x.APP_SHORT_NAME.ToLower().Trim() == applicationname.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_REGISTERED_APPS.Any(x => x.APP_SHORT_NAME.ToLower().Trim() == applicationname.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate ApplicationName Exist end | UserName: {0}", Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in CheckDuplicateApplicationNameExist method | AppId: {0} | UserName: {1}", ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured Application in CheckDuplicateApplicationNameExist method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in CheckDuplicateApplicationNameExist method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex.InnerException);
                throw;
            } 
        }

        public bool AddEditApplication(ApplicationViewModel AppModelEntity)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppModelEntity.ApplicationName))
                {
                    AppModelEntity.ApplicationName = AppModelEntity.ApplicationName.Trim();
                }
                var flag = false;
                if (AppModelEntity.ApplicationId == 0)
                {
                    logger.Info(string.Format("Add application start | Application: {0} | Username: {1}", AppModelEntity.ApplicationName, Username));

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
                    logger.Info(string.Format("Add application end | Application: {0} | Username: {1}", AppModelEntity.ApplicationName, Username));

                }
                else
                {
                    var RegisterTbl = enty.T_REGISTERED_APPS.Find(AppModelEntity.ApplicationId);
                    logger.Info(string.Format("Edit application start | Application: {0} | ApplicationId: {1} | Username: {2}", AppModelEntity.ApplicationName, AppModelEntity.ApplicationId, Username));
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
                    logger.Info(string.Format("Edit application end | Application: {0} | ApplicationId: {1} | Username: {2}", AppModelEntity.ApplicationName, AppModelEntity.ApplicationId, Username));
                }
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1}", AppModelEntity.ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1}", AppModelEntity.ApplicationId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in AddEditApplication method | AppId: {0} | UserName: {1}", AppModelEntity.ApplicationId, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> CheckTestCaseExistsInAppliction(long ApplicationId)
        {
            try
            {
                logger.Info(string.Format("Check TestCase Exists In Appliction start | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                List<string> Applicationname = new List<string>();
                var lApplicationList = enty.REL_APP_PROJ.Where(x => x.APPLICATION_ID == ApplicationId).ToList();

                if (lApplicationList.Count() > 0)
                {
                    foreach (var item in lApplicationList)
                    {
                        var sname = enty.T_TEST_PROJECT.Find(item.PROJECT_ID);

                        Applicationname.Add(sname.PROJECT_NAME);
                        Applicationname = (from w in Applicationname select w).Distinct().ToList();
                    }
                    logger.Info(string.Format("Check TestCase Exists In Appliction end | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                    return Applicationname;
                }
                logger.Info(string.Format("Check TestCase Exists In Appliction end | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                return Applicationname;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in CheckTestCaseExistsInAppliction method | AppId: {0} | UserName: {1}", ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured Application in CheckTestCaseExistsInAppliction method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in CheckTestCaseExistsInAppliction method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex.InnerException);
                throw;
            }
           
        }
        public bool DeleteApplication(long ApplicationId)
        {
            try
            {
                logger.Info(string.Format("Delete Application start | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
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
                logger.Info(string.Format("Delete Application end | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in DeleteApplication method | AppId: {0} | UserName: {1}", ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured Application in DeleteApplication method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in DeleteApplication method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex.InnerException);
                throw;
            }
           
        }
        public String GetApplicationNameById(long ApplicationId)
        {
            try
            {
                logger.Info(string.Format("Get ApplicationName start | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                var result = enty.T_REGISTERED_APPS.FirstOrDefault(x => x.APPLICATION_ID == ApplicationId).APP_SHORT_NAME;
                logger.Info(string.Format("Get ApplicationName end | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Application in GetApplicationNameById method | AppId: {0} | UserName: {1}", ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured Application in GetApplicationNameById method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Application in GetApplicationNameById method | AppId: {0} | UserName: {1}", ApplicationId, Username), ex.InnerException);
                throw;
            }
        }
    }
}

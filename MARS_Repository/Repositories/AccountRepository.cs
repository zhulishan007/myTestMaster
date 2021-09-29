using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARS_Repository.Entities;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System.Transactions;

namespace MARS_Repository.Repositories
{
    public class AccountRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        internal const string USER_SEQ = "SEQ_TESTER_ID";

        public List<T_TESTER_INFO> GetAllUsers()
        {
            try
            {
                logger.Info(string.Format("Get All User start | Username: {0}", Username));
                var result = entity.T_TESTER_INFO.ToList();
                logger.Info(string.Format("Get All User end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetAllUsers method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetAllUsers method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetAllUsers method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public string CheckLoginNameAndEmail(string loginname, string emailid)
        {
            try
            {
                logger.Info(string.Format("Check LoginName And Email start | emailid: {0} | Username: {1}", emailid, Username));
                var loginexists = entity.T_TESTER_INFO.Any(x => (x.TESTER_LOGIN_NAME).ToLower().Trim() == loginname.ToLower().Trim());
                logger.Info(string.Format("Check LoginName And Email end | emailid: {0} | Username: {1}", emailid, Username));
                if (loginexists)
                    return "Login name invalid";

                var emailexists = entity.T_TESTER_INFO.Any(x => (x.TESTER_MAIL).ToLower().Trim() == emailid.ToLower().Trim());
                if (emailexists)
                    return "Email invalid";

                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CheckLoginNameAndEmail method | Email Id : {0} | UserName: {1}", emailid, Username));
                ELogger.ErrorException(string.Format("Error occured User in CheckLoginNameAndEmail method | Email Id : {0} | UserName: {1}", emailid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CheckLoginNameAndEmail method | Email Id : {0} | UserName: {1}", emailid, Username), ex.InnerException);
                throw;
            }
        }
        public bool DeleteUserMapExePath(long userid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("DeleteUserMapExePath start | User id: {0} | Username: {1}", userid, Username));
                    var flag = false;
                    var result = entity.T_RELATION_USER_ENGINEPATH.FirstOrDefault(x => x.RELATIONID == userid);
                    if (result != null)
                    {
                        entity.T_RELATION_USER_ENGINEPATH.Remove(result);
                        entity.SaveChanges();
                        flag = true;
                        scope.Complete();
                        return flag;
                    }
                    logger.Info(string.Format("DeleteUserMapExePath end | User id: {0} | Username: {1}", userid, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in DeleteUserMapExePath method | User Id : {0} | UserName: {1}", userid, Username));
                ELogger.ErrorException(string.Format("Error occured User in DeleteUserMapExePath method | User Id : {0} | UserName: {1}", userid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in DeleteUserMapExePath method | User Id : {0} | UserName: {1}", userid, Username), ex.InnerException);
                throw;
            }
        }
        public string AddUserPath(long userid, long relid, string exepath)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("AddUserPath start | User id: {0} | Username: {1}", userid, Username));
                    if (relid == 0)
                    {
                        var result = entity.T_RELATION_USER_ENGINEPATH.Where(x => x.USERID == userid).ToList();
                        if (result.Count == 0)
                        {
                            var userpath = new T_RELATION_USER_ENGINEPATH();
                            userpath.RELATIONID = Helper.NextTestSuiteId("SEQ_REL_USER_ENGINEPATH");
                            userpath.USERID = userid;
                            userpath.ENGINEPATH = exepath;
                            entity.T_RELATION_USER_ENGINEPATH.Add(userpath);
                            entity.SaveChanges();
                            logger.Info(string.Format("AddUserPath end | User id: {0} | Username: {1}", userid, Username));
                            return "success";
                        }
                    }
                    else
                    {
                        var lresult = entity.T_RELATION_USER_ENGINEPATH.Find(relid);
                        if (lresult != null)
                        {
                            lresult.ENGINEPATH = exepath;
                            entity.SaveChanges();
                            logger.Info(string.Format("AddUserPath end | User id: {0} | Username: {1}", userid, Username));
                            return "success";
                        }
                    }
                    logger.Info(string.Format("AddUserPath end | User id: {0} | Username: {1}", userid, Username));
                    scope.Complete();
                    return "error";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in AddUserPath method | User Id : {0} | Relation Id : {1} | UserName: {2}", userid, relid, Username));
                ELogger.ErrorException(string.Format("Error occured User in AddUserPath method | User Id : {0} | Relation Id : {1} | UserName: {2}", userid, relid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in AddUserPath method | User Id : {0} | Relation Id : {1} | UserName: {2}", userid, relid, Username), ex.InnerException);
                throw;
            }
        }
        public List<RelUserExePath> ListUserExePath()
        {
            logger.Info(string.Format("ListUserExePath start | Username: {0}", Username));
            try
            {
                List<RelUserExePath> relUsers = new List<RelUserExePath>();
                var result = (from t in entity.T_TESTER_INFO
                              join t2 in entity.T_RELATION_USER_ENGINEPATH on t.TESTER_ID equals t2.USERID
                              select new RelUserExePath
                              {
                                  userid = t2.USERID,
                                  Username = t.TESTER_LOGIN_NAME,
                                  ExePath = t2.ENGINEPATH,
                                  Relid = t2.RELATIONID

                              }).ToList();

                logger.Info(string.Format("ListUserExePath end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ListUserExePath method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in ListUserExePath method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ListUserExePath method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public RelUserExePath GetUserExePath(string lusername)
        {
            try
            {
                logger.Info(string.Format("GetUserExePath start | Username: {0}", Username));
                var result = new RelUserExePath();
                var lUsers = entity.T_TESTER_INFO.Where(x => x.TESTER_LOGIN_NAME.ToUpper().Trim() == lusername.ToUpper().Trim()).ToList();
                if (lUsers.Count() > 0)
                {
                    var lUserId = lUsers.FirstOrDefault().TESTER_ID;
                    var lList = entity.T_RELATION_USER_ENGINEPATH.Where(x => x.USERID == lUserId).ToList();
                    if (lList.Count() > 0)
                    {
                        result = lList.Select(x => new RelUserExePath { userid = x.USERID, ExePath = x.ENGINEPATH }).FirstOrDefault();
                    }
                }
                logger.Info(string.Format("GetUserExePath end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserExePath method | ParaUserName : {0} | UserName: {1}", lusername, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserExePath method | ParaUserName : {0} | UserName: {1}", lusername, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserExePath method | ParaUserName : {0} | UserName: {1}", lusername, Username), ex.InnerException);
                throw;
            }
        }
        public T_TESTER_INFO GetUserById(int lUserId)
        {
            try
            {
                logger.Info(string.Format("Delete User start | User id: {0} | Username: {1}", lUserId, Username));
                var result = entity.T_TESTER_INFO.FirstOrDefault(x => x.TESTER_ID == lUserId);
                logger.Info(string.Format("Delete User start | User id: {0} | Username: {1}", lUserId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserById method | UserId : {0} | UserName: {1}", lUserId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserById method | UserId : {0} | UserName: {1}", lUserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserById method | UserId : {0} | UserName: {1}", lUserId, Username), ex.InnerException);
                throw;
            }
        }
        public UserModel GetUserMappingById(int lUserId)
        {
            try
            {
                logger.Info(string.Format("Get User Mapping Id start | User id: {0} | Username: {1}", lUserId, Username));
                var lList = entity.T_TESTER_INFO.Where(x => x.TESTER_ID == lUserId).Select(y => new UserModel
                {
                    TESTER_ID = y.TESTER_ID,
                    COMPANY_ID = y.COMPANY_ID,
                    TESTER_LOGIN_NAME = y.TESTER_LOGIN_NAME,
                    TESTER_MAIL = y.TESTER_MAIL,
                    TESTER_NAME_F = y.TESTER_NAME_F,
                    TESTER_NAME_LAST = y.TESTER_NAME_LAST,
                    TESTER_NAME_M = y.TESTER_NAME_M,

                }).ToList();

                foreach (UserModel item in lList)
                {
                    if (entity.T_USER_MAPPING.Any(x => x.TESTER_ID == item.TESTER_ID))
                    {
                        item.STATUS = entity.T_USER_MAPPING.FirstOrDefault(x => x.TESTER_ID == item.TESTER_ID).STATUS;
                    }
                }
                var result = new UserModel();
                if (lList.Count() > 0)
                {
                    result = lList.FirstOrDefault();
                }
                logger.Info(string.Format("Get User Mapping Id end | User id: {0} | Username: {1}", lUserId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserMappingById method | UserId : {0} | UserName: {1}", lUserId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserMappingById method | UserId : {0} | UserName: {1}", lUserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserMappingById method | UserId : {0} | UserName: {1}", lUserId, Username), ex.InnerException);
                throw;
            }
        }

        public T_TESTER_INFO GetUserByEmail(string lEmailId)
        {
            try
            {
                logger.Info(string.Format("Get User Email start | Email: {0}", lEmailId));
                var result = entity.T_TESTER_INFO.FirstOrDefault(x => x.TESTER_MAIL.ToLower().Trim() == lEmailId.ToLower().Trim() || x.TESTER_LOGIN_NAME.ToLower().Trim() == lEmailId.ToLower().Trim());
                logger.Info(string.Format("Get User Email start | Email: {0}", lEmailId));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserByEmail method | Email Id : {0} | UserName: {1}", lEmailId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserByEmail method | Email Id : {0} | UserName: {1}", lEmailId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserByEmail method | Email Id : {0} | UserName: {1}", lEmailId, Username), ex.InnerException);
                throw;
            }
        }

        public T_USER_MAPPING UpdateTempKey(decimal UserMapId, string TempKey)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("UpdateTempKey start | UserMapId: {0} | Username: {1}", UserMapId, Username));
                    var table = entity.T_USER_MAPPING.Find(UserMapId);
                    if (table != null)
                    {

                        table.TEMP_KEY = TempKey;

                        entity.SaveChanges();
                    }
                    logger.Info(string.Format("UpdateTempKey end | UserMapId: {0} | Username: {1}", UserMapId, Username));
                    scope.Complete();
                    return table;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in UpdateTempKey method | User Map Id : {0} | Temp Key : {1} | UserName: {2}", UserMapId, TempKey, Username));
                ELogger.ErrorException(string.Format("Error occured User in UpdateTempKey method | User Map Id : {0} | Temp Key : {1} | UserName: {2}", UserMapId, TempKey, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in UpdateTempKey method | User Map Id : {0} | Temp Key : {1} | UserName: {2}", UserMapId, TempKey, Username), ex.InnerException);
                throw;
            }
        }

        public T_USER_MAPPING GetUserMappingByUserId(decimal UserId)
        {
            try
            {
                logger.Info(string.Format("Get User Mapping Id start | UserId: {0}", UserId));
                var table = entity.T_USER_MAPPING.FirstOrDefault(x => x.TESTER_ID == UserId);
                logger.Info(string.Format("Get User Mapping Id end | UserId: {0}", UserId));
                if (table != null)
                {
                    return table;
                }
                else
                {
                    return new T_USER_MAPPING();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserMappingByUserId method | User Id : {0} |  UserName: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserMappingByUserId method | User Id : {0} |  UserName: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserMappingByUserId method | User Id : {0} | UserName: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public T_TESTER_INFO GetUserByEmailAndLoginName(string lUserLogin)
        {
            try
            {
                logger.Info(string.Format("Get User Info start| UserName: {0}", lUserLogin));
                var result = entity.T_TESTER_INFO.FirstOrDefault(x => x.TESTER_MAIL.ToLower().Trim() == lUserLogin.ToLower().Trim() || x.TESTER_LOGIN_NAME.ToLower().Trim() == lUserLogin.ToLower().Trim());

                if (result != null)
                {
                    var checkIsDelete = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == result.TESTER_ID).FirstOrDefault();
                    if (checkIsDelete != null)
                    {
                        if (checkIsDelete.IS_DELETED == 1)
                        {
                            result = null;
                        }
                    }
                }

                logger.Info(string.Format("Get User Info end| UserName: {0}", lUserLogin));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserByEmailAndLoginName method | Login Name : {0} |  UserName: {1}", lUserLogin, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserByEmailAndLoginName method | Login Name : {0} |  UserName: {1}", lUserLogin, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserByEmailAndLoginName method | Login Name : {0} | UserName: {1}", lUserLogin, Username), ex.InnerException);
                throw;
            }

        }
        public List<UserModel> ListAllUsersWithProjectMapping()
        {
            try
            {
                logger.Info(string.Format("ListAllUsersWithProjectMapping start | Username: {0}", Username));
                var lList = new List<UserModel>();
                var result = entity.T_TESTER_INFO.ToList().Select(a => new UserModel
                {
                    TESTER_ID = a.TESTER_ID,
                    TESTER_NAME_F = a.TESTER_NAME_F,
                    TESTER_NAME_M = a.TESTER_NAME_M,
                    TESTER_NAME_LAST = a.TESTER_NAME_LAST,
                    TESTER_LOGIN_NAME = a.TESTER_LOGIN_NAME,
                    TESTER_MAIL = a.TESTER_MAIL,
                }).ToList();
                var flag = 0;
                foreach (UserModel item in result)
                {
                    flag = 0;
                    var rs = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == item.TESTER_ID).ToList();
                    foreach (var itm in rs)
                    {
                        if (itm.IS_DELETED == 1)
                        {
                            flag = 1;
                        }
                    }
                    if (flag != 1)
                    {
                        var model = new UserModel();
                        model.TESTER_ID = item.TESTER_ID;
                        model.TESTER_NAME_F = item.TESTER_NAME_F;
                        model.TESTER_NAME_LAST = item.TESTER_NAME_LAST;
                        model.TESTER_LOGIN_NAME = item.TESTER_LOGIN_NAME;
                        model.TESTER_MAIL = item.TESTER_MAIL;
                        var projectlist = (from t in entity.REL_PROJECT_USER
                                           join t1 in entity.T_TEST_PROJECT on t.PROJECT_ID equals t1.PROJECT_ID
                                           where t.USER_ID == item.TESTER_ID
                                           select new
                                           {
                                               t1.PROJECT_ID,
                                               t1.PROJECT_NAME
                                           }
                                         );
                        model.ProjectName = string.Join(",", projectlist.Where(y => !string.IsNullOrEmpty(y.PROJECT_NAME)).Select(x => x.PROJECT_NAME).Distinct());
                        model.ProjectId = string.Join(",", projectlist.Where(y => y.PROJECT_ID > 0).Select(x => x.PROJECT_ID).Distinct());
                        lList.Add(model);
                    }
                }
                logger.Info(string.Format("ListAllUsersWithProjectMapping end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ListAllUsersWithProjectMapping method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in ListAllUsersWithProjectMapping method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ListAllUsersWithProjectMapping method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<UserModel> ListAllUsers()
        {
            try
            {
                logger.Info(string.Format("List Users start | UserName: {0}", Username));
                var lList = new List<UserModel>();
                var result = entity.T_TESTER_INFO.ToList().Select(a => new UserModel
                {
                    TESTER_ID = a.TESTER_ID,
                    TESTER_NAME_F = a.TESTER_NAME_F,
                    TESTER_NAME_M = a.TESTER_NAME_M,
                    TESTER_NAME_LAST = a.TESTER_NAME_LAST,
                    TESTER_LOGIN_NAME = a.TESTER_LOGIN_NAME,
                    TESTER_MAIL = a.TESTER_MAIL,
                    COMPANY_NAME = a.T_MARS_COMPANY.COMPANY_NAME

                }).ToList();

                if (result.Count() > 0)
                {
                    lList = result.ToList();
                    foreach (UserModel item in lList.ToList())
                    {
                        decimal lDeletedflag = 0;
                        decimal lEnable = 0;
                        var lT_User_MappingList = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == item.TESTER_ID).ToList();

                        if (lT_User_MappingList.Count() > 0)
                        {
                            lEnable = Convert.ToDecimal(lT_User_MappingList.FirstOrDefault().STATUS);
                            lDeletedflag = Convert.ToDecimal(lT_User_MappingList.FirstOrDefault().IS_DELETED);
                        }
                        item.STATUS = lEnable;
                        item.IsDeleted = lDeletedflag;
                        if (item.IsDeleted == 1)
                        {
                            lList.Remove(item);
                        }
                    }
                }
                logger.Info(string.Format("List Users start | UserName: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ListAllUsers method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in ListAllUsers method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ListAllUsers method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<UserActiveModel> ListAllActiveUsers()
        {
            try
            {
                logger.Info(string.Format("List All Active Users start | UserName: {0}", Username));
                var result = entity.T_USER_ACTIVEPAGE.ToList().Select(a => new UserActiveModel
                {
                    ACTIVE_ID = a.ACTIVEPAGE_ID,
                    USER_ID = a.USER_ID,
                    PageName = SetPagename(a.PAGE_NAME, a.PAGE_ID),
                    UserName = a.USER_ID == null ? "" : entity.T_TESTER_INFO.Where(x => x.TESTER_ID == a.USER_ID).FirstOrDefault().TESTER_LOGIN_NAME,

                }).Distinct().ToList();

                logger.Info(string.Format("List All Active Users end | UserName: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ListAllActiveUsers method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in ListAllActiveUsers method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ListAllActiveUsers method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public string SetPagename(string pagename, decimal PgId)
        {
            try
            {
                logger.Info(string.Format("SetPagename start | pagename: {0} | Username: {1}", pagename, Username));
                string result = string.Empty;
                if (pagename == "TestCase")
                    result = pagename + " >" + entity.T_TEST_CASE_SUMMARY.Where(x => x.TEST_CASE_ID == PgId).FirstOrDefault().TEST_CASE_NAME;
                else if (pagename == "Storyboard")
                    result = pagename + " >" + entity.T_STORYBOARD_SUMMARY.Where(x => x.STORYBOARD_ID == PgId).FirstOrDefault().STORYBOARD_NAME;
                else
                    result = pagename;

                logger.Info(string.Format("SetPagename end | pagename: {0} | Username: {1}", pagename, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in SetPagename method | Page Name : {0} | Page Id : {1} | UserName: {2}", pagename, PgId, Username));
                ELogger.ErrorException(string.Format("Error occured User in SetPagename method | Page Name : {0} | Page Id : {1} | UserName: {2}", pagename, PgId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in SetPagename method | Page Name : {0} | Page Id : {1} | UserName: {2}", pagename, PgId, Username), ex.InnerException);
                throw;
            }
        }
        public T_TESTER_INFO CreateNewUser(T_TESTER_INFO t_TESTER, decimal? lchecked)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    T_TESTER_INFO tester = new T_TESTER_INFO();
                    var lresult = entity.T_TESTER_INFO.Find(t_TESTER.TESTER_ID);
                    var result = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == t_TESTER.TESTER_ID).ToList();
                    if (lresult != null)
                    {
                        logger.Info(string.Format("Edit New User start | New User Name: {0} | Username: {1}", t_TESTER.TESTER_LOGIN_NAME, Username));
                        lresult.TESTER_NAME_F = t_TESTER.TESTER_NAME_F;
                        lresult.TESTER_NAME_M = t_TESTER.TESTER_NAME_M;
                        lresult.TESTER_NAME_LAST = t_TESTER.TESTER_NAME_LAST;
                        lresult.TESTER_MAIL = t_TESTER.TESTER_MAIL;
                        lresult.TESTER_LOGIN_NAME = t_TESTER.TESTER_LOGIN_NAME;
                        lresult.COMPANY_ID = t_TESTER.COMPANY_ID;
                        entity.SaveChanges();
                        logger.Info(string.Format("Edit New User end | New User Name: {0} | Username: {1}", t_TESTER.TESTER_LOGIN_NAME, Username));
                    }
                    else
                    {
                        logger.Info(string.Format("Create New User start | New User Name: {0} | Username: {1}", t_TESTER.TESTER_LOGIN_NAME, Username));
                        ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));
                        var projectId = entity.GETNEXT_VAL(USER_SEQ, outparam);
                        var lUserId = long.Parse(outparam.Value.ToString());
                        t_TESTER.TESTER_ID = lUserId;

                        entity.T_TESTER_INFO.Add(t_TESTER);
                        entity.SaveChanges();
                        logger.Info(string.Format("Create New User end | New User Name: {0} | Username: {1}", t_TESTER.TESTER_LOGIN_NAME, Username));
                    }
                    if (result.Count() > 0)
                    {
                        var lusermodel = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == t_TESTER.TESTER_ID).FirstOrDefault();
                        lusermodel.STATUS = lchecked;
                        entity.SaveChanges();
                    }
                    else
                    {
                        ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));
                        var projectId = entity.GETNEXT_VAL(USER_SEQ, outparam);
                        var lUserId = long.Parse(outparam.Value.ToString());
                        T_USER_MAPPING mapper = new T_USER_MAPPING();
                        mapper.USER_MAPPING_ID = lUserId;
                        mapper.STATUS = lchecked;
                        mapper.TESTER_ID = t_TESTER.TESTER_ID;
                        entity.T_USER_MAPPING.Add(mapper);
                        entity.SaveChanges();
                    }
                    scope.Complete();
                    return t_TESTER;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CreateNewUser method | Tested Id : {0} | UserName: {1}", t_TESTER.TESTER_ID, Username));
                ELogger.ErrorException(string.Format("Error occured User in CreateNewUser method | Tested Id : {0} | UserName: {1}", t_TESTER.TESTER_ID, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CreateNewUser method | Tested Id : {0} | UserName: {1}", t_TESTER.TESTER_ID, Username), ex.InnerException);
                throw;
            }
        }

        public T_TESTER_INFO ChangeUserPassword(string lNewPsw, decimal lTesterId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("ChangeUserPassword start | New Password: {0} | User id: {1} | Username: {2}", lNewPsw, lTesterId, Username));
                    T_TESTER_INFO tester = new T_TESTER_INFO();
                    var lresult = entity.T_TESTER_INFO.Find(lTesterId);
                    if (lresult != null)
                    {
                        lresult.TESTER_PWD = lNewPsw;
                        entity.SaveChanges();
                    }
                    logger.Info(string.Format("ChangeUserPassword end | New Password: {0} | User id: {1} | Username: {2}", lNewPsw, lTesterId, Username));
                    scope.Complete();
                    return lresult;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ChangeUserPassword method | Tested Id : {0} | UserName: {1}", lTesterId, Username));
                ELogger.ErrorException(string.Format("Error occured User in ChangeUserPassword method | Tested Id : {0} | UserName: {1}", lTesterId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ChangeUserPassword method | Tested Id : {0} | UserName: {1}", lTesterId, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckLoginNameExist(string LoginName, decimal? TesterId)
        {
            try
            {
                logger.Info(string.Format("Check Login Name Exist start | LoginName: {0} | Username: {1}", LoginName, Username));
                bool lresult = false;
                if (TesterId != null && TesterId > 0)
                {
                    var fresult = (from t in entity.T_TESTER_INFO
                                   join t1 in entity.T_USER_MAPPING on t.TESTER_ID equals t1.TESTER_ID
                                   where t1.IS_DELETED == 1 && t.TESTER_LOGIN_NAME.ToUpper().Trim() == LoginName.ToUpper().Trim() && t.TESTER_ID != TesterId
                                   select t
                                 ).ToList();
                    if (fresult.Count > 0)
                    {
                        return lresult;
                    }
                    lresult = entity.T_TESTER_INFO.Any(x => x.TESTER_LOGIN_NAME.ToLower().Trim() == LoginName.ToLower().Trim() && x.TESTER_ID != TesterId);
                }
                else
                {
                    var fresult = (from t in entity.T_TESTER_INFO
                                   join t1 in entity.T_USER_MAPPING on t.TESTER_ID equals t1.TESTER_ID
                                   where t1.IS_DELETED == 1 && t.TESTER_LOGIN_NAME.ToUpper().Trim() == LoginName.ToUpper().Trim()
                                   select t
                                 ).ToList();
                    if (fresult.Count > 0)
                    {
                        return lresult;
                    }
                    lresult = entity.T_TESTER_INFO.Any(x => x.TESTER_LOGIN_NAME.ToLower().Trim() == LoginName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Login Name Exist end | LoginName: {0} | Username: {1}", LoginName, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CheckLoginNameExist method | Tested Id : {0} | UserName: {1}", TesterId, Username));
                ELogger.ErrorException(string.Format("Error occured User in CheckLoginNameExist method | Tested Id : {0} | UserName: {1}", TesterId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CheckLoginNameExist method | Tested Id : {0} | UserName: {1}", TesterId, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckLoginEmailExist(string LoginEmail, decimal? TesterId)
        {
            try
            {
                logger.Info(string.Format("Check Login Email Exist start | Email: {0} | Username: {1}", LoginEmail, Username));
                bool lresult = false;
                if (TesterId != null && TesterId > 0)
                {
                    var fresult = (from t in entity.T_TESTER_INFO
                                   join t1 in entity.T_USER_MAPPING on t.TESTER_ID equals t1.TESTER_ID
                                   where t1.IS_DELETED == 1 && t.TESTER_MAIL.ToUpper().Trim() == LoginEmail.ToUpper().Trim() && t.TESTER_ID != TesterId
                                   select t
                                ).ToList();
                    if (fresult.Count > 0)
                        return lresult;
                    lresult = entity.T_TESTER_INFO.Any(x => x.TESTER_MAIL.ToLower().Trim() == LoginEmail.ToLower().Trim() && x.TESTER_ID != TesterId);
                }
                else
                {
                    var fresult = (from t in entity.T_TESTER_INFO
                                   join t1 in entity.T_USER_MAPPING on t.TESTER_ID equals t1.TESTER_ID
                                   where t1.IS_DELETED == 1 && t.TESTER_MAIL.ToUpper().Trim() == LoginEmail.ToUpper().Trim()
                                   select t
                                 ).ToList();

                    if (fresult.Count > 0)
                    {
                        return lresult;
                    }
                    lresult = entity.T_TESTER_INFO.Any(x => x.TESTER_MAIL.ToLower().Trim() == LoginEmail.ToLower().Trim());
                }
                logger.Info(string.Format("Check Login Email Exist end | Email: {0} | Username: {1}", LoginEmail, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CheckLoginEmailExist method | Tested Id : {0} | UserName: {1}", TesterId, Username));
                ELogger.ErrorException(string.Format("Error occured User in CheckLoginEmailExist method | Tested Id : {0} | UserName: {1}", TesterId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CheckLoginEmailExist method | Tested Id : {0} | UserName: {1}", TesterId, Username), ex.InnerException);
                throw;
            }
        }

        public long GetUserID()
        {
            try
            {
                logger.Info(string.Format("GetUserID start | Username: {0}", Username));
                ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));

                var projectId = entity.GETNEXT_VAL(USER_SEQ, outparam);
                logger.Info(string.Format("GetUserID end | Username: {0}", Username));
                return long.Parse(outparam.Value.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserID method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserID method |  UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserID method |  UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public T_USER_MAPPING GetUserId(decimal id)
        {
            try
            {
                logger.Info(string.Format("GetUser Id start | User id: {0}", id));
                var result = entity.T_USER_MAPPING.FirstOrDefault(x => x.TESTER_ID == id);
                logger.Info(string.Format("GetUser Id start | User id: {0}", id));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserId method | UserId: {0} | UserName: {1}", id, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserId method | UserId: {0} | UserName: {1}", id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserId method | UserId: {0} | UserName: {1}", id, Username), ex.InnerException);
                throw;
            }
        }
        public void SetTemporaryKey(string tempkey, decimal testerid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("SetTemporaryKey start | testerid: {0} | tempkey: {1}", testerid, tempkey));
                    T_USER_MAPPING User = new T_USER_MAPPING();
                    var ruser = GetUserId(testerid);
                    if (ruser != null)
                    {
                        ruser.TEMP_KEY = tempkey;
                        entity.SaveChanges();
                    }
                    else
                    {
                        ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));
                        var projectId = entity.GETNEXT_VAL(USER_SEQ, outparam);
                        var lUserId = long.Parse(outparam.Value.ToString());
                        User.USER_MAPPING_ID = lUserId;
                        User.TESTER_ID = testerid;
                        User.TEMP_KEY = tempkey;
                        entity.T_USER_MAPPING.Add(User);
                        entity.SaveChanges();
                    }
                    scope.Complete();
                    logger.Info(string.Format("SetTemporaryKey end | User id: {0} | tempkey: {1}", testerid, tempkey));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in SetTemporaryKey method | UserId: {0} | Temp Key {1} | UserName: {2}", testerid, tempkey, Username));
                ELogger.ErrorException(string.Format("Error occured User in SetTemporaryKey method | UserId: {0} | Temp Key {1} | UserName: {2}", testerid, tempkey, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in SetTemporaryKey method | UserId: {0} | Temp Key {1} | UserName: {2}", testerid, tempkey, Username), ex.InnerException);
                throw;
            }
        }

        public string GetUserName(int id)
        {
            try
            {
                logger.Info(string.Format("Get user name start | User id: {0} | Username: {1}", id, Username));
                var username = entity.T_TESTER_INFO.FirstOrDefault(x => x.TESTER_ID == id).TESTER_LOGIN_NAME;
                logger.Info(string.Format("Get user name end | User id: {0} | Username: {1}", id, Username));
                return username;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserName method | UserId: {0} | UserName: {1}", id, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserName method | UserId: {0} | UserName: {1}", id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserName method | UserId: {0} | UserName: {1}", id, Username), ex.InnerException);
                throw;
            }
        }
        public T_TESTER_INFO DeleteUser(int id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete User start | User id: {0} | Username: {1}", id, Username));
                    var rUser = new T_USER_MAPPING();
                    var lUser = new T_TESTER_INFO();
                    if (id > 0)
                    {
                        lUser = GetUserById(id);
                        rUser = GetUserId(id);

                        if (lUser != null && rUser != null)
                        {
                            var list = entity.REL_PROJECT_USER.Where(x => x.USER_ID == id).ToList();
                            foreach (var item in list)
                            {
                                entity.REL_PROJECT_USER.Remove(item);
                                entity.SaveChanges();
                            }
                            rUser.TESTER_ID = lUser.TESTER_ID;
                            rUser.IS_DELETED = 1;
                            entity.SaveChanges();
                        }
                        else
                        {
                            if (rUser == null)
                            {
                                ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));
                                var projectId = entity.GETNEXT_VAL(USER_SEQ, outparam);
                                var lUserId = long.Parse(outparam.Value.ToString());
                                var lmappingtable = new T_USER_MAPPING();
                                lmappingtable.USER_MAPPING_ID = lUserId;
                                lmappingtable.TESTER_ID = lUser.TESTER_ID;
                                lmappingtable.IS_DELETED = 1;
                                entity.T_USER_MAPPING.Add(lmappingtable);
                                entity.SaveChanges();
                            }
                        }
                    }
                    logger.Info(string.Format("Delete User end | User id: {0} | Username: {1}", id, Username));
                    scope.Complete();
                    return lUser;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in DeleteUser method | UserId: {0} | UserName: {1}", id, Username));
                ELogger.ErrorException(string.Format("Error occured User in DeleteUser method | UserId: {0} | UserName: {1}", id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in DeleteUser method | UserId: {0} | UserName: {1}", id, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckPinExist(decimal useId, string datatab, long dataid, long ProjectId)
        {
            try
            {
                logger.Info(string.Format("Check Pin Exist start | User id: {0} | Username: {1}", useId, Username));
                var result = false;

                var list = entity.T_USER_ACTIVEPAGE.Where(x => x.USER_ID == useId && x.PAGE_NAME.Trim().ToLower() == datatab.Trim().ToLower() && x.PAGE_ID == dataid && x.PROJECT_ID == ProjectId).ToList();
                if (list.Count > 0)
                    result = true;
                logger.Info(string.Format("Check Pin Exist end | User id: {0} | Username: {1}", useId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CheckPinExist method | UserId: {0} | Data Id : {1} | Data Tab: {2} | Project Id: {3} | Username : {4}", useId, dataid, datatab, ProjectId, Username)); 
                ELogger.ErrorException(string.Format("Error occured User in CheckPinExist method | UserId: {0} | Data Id : {1} | Data Tab: {2} | Project Id: {3} | Username : {4}", useId, dataid, datatab, ProjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CheckPinExist method | UserId: {0} | Data Id : {1} | Data Tab: {2} | Project Id: {3} | Username : {4}", useId, dataid, datatab, ProjectId, Username), ex.InnerException);
                throw;
            }
        }

        public T_USER_ACTIVEPAGE DeleteActiveUser(long id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete Active User start | User id: {0} | Username: {1}", id, Username));
                    var lUser = new T_USER_ACTIVEPAGE();
                    if (id > 0)
                    {
                        var list = entity.T_USER_ACTIVEPAGE.Where(x => x.ACTIVEPAGE_ID == id).ToList();
                        foreach (var item in list)
                        {
                            entity.T_USER_ACTIVEPAGE.Remove(item);
                            entity.SaveChanges();
                        }
                        lUser = entity.T_USER_ACTIVEPAGE.FirstOrDefault(x => x.ACTIVEPAGE_ID == id);
                    }
                    logger.Info(string.Format("Delete Active User end | User id: {0} | Username: {1}", id, Username));
                    scope.Complete();
                    return lUser;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in DeleteActiveUser method | UserId: {0} | UserName: {1}", id, Username));
                ELogger.ErrorException(string.Format("Error occured User in DeleteActiveUser method | UserId: {0} | UserName: {1}", id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in DeleteActiveUser method | UserId: {0} | UserName: {1}", id, Username), ex.InnerException);
                throw;
            }
        }

        public T_STORYBOARD_SUMMARY GetStoryboradNameyId(long sid)
        {
            try
            {
                logger.Info(string.Format("Get StoryboradName start | Stroryboard id: {0} | Username: {1}", sid, Username));
                var lResult = entity.T_STORYBOARD_SUMMARY.Where(x => x.STORYBOARD_ID == sid).FirstOrDefault();
                logger.Info(string.Format("Get StoryboradName end | Stroryboard id: {0} | Username: {1}", sid, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetStoryboradNameyId method | StoryBoard Id: {0} | UserName: {1}", sid, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetStoryboradNameyId method | StoryBoard Id: {0} | UserName: {1}", sid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetStoryboradNameyId method | StoryBoard Id: {0} | UserName: {1}", sid, Username), ex.InnerException);
                throw;
            }
        }

        public TestCaseTestSuiteModel GetTestsuiteIdByTeastcaseId(long Tid)
        {
            try
            {
                logger.Info(string.Format("Get TestsuiteId start | TeastcaseId: {0} | Username: {1}", Tid, Username));
                var list = from t1 in entity.T_TEST_CASE_SUMMARY
                           join t2 in entity.REL_TEST_CASE_TEST_SUITE on t1.TEST_CASE_ID equals t2.TEST_CASE_ID
                           where t1.TEST_CASE_ID == Tid
                           select new TestCaseTestSuiteModel
                           {
                               TestSuiteId = t2.TEST_SUITE_ID,
                               TestCaseName = t1.TEST_CASE_NAME,
                           };
                var lResult = list.FirstOrDefault();
                logger.Info(string.Format("Get TestsuiteId end | TeastcaseId: {0} | Username: {1}", Tid, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetTestsuiteIdByTeastcaseId method | Testcase Id: {0} | UserName: {1}", Tid, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetTestsuiteIdByTeastcaseId method | Testcase Id: {0} | UserName: {1}", Tid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetTestsuiteIdByTeastcaseId method | Testcase Id: {0} | UserName: {1}", Tid, Username), ex.InnerException);
                throw;
            }
        }

        public string AddDetelteActivateTab(long useId, string userName, string datatab, long dataid, string dataname, string linkText, long ProjectId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string result = string.Empty;
                    logger.Info(string.Format("Change Activate Tab start | UserId: {0} | UserName: {1}", useId, Username));
                    if (linkText.Trim() == "Pin Tab")
                    {
                        var pagename = dataid == 0 ? dataname : datatab;
                        var user = entity.T_USER_ACTIVEPAGE.Where(x => x.USER_ID == useId && x.PAGE_NAME.Trim() == pagename.Trim() && x.PAGE_ID == dataid && x.PROJECT_ID == ProjectId).FirstOrDefault();
                        if (user == null)
                        {
                            var lUser = new T_USER_ACTIVEPAGE();
                            lUser.ACTIVEPAGE_ID = Helper.NextTestSuiteId("T_ACTIVEUSERPAGE_SEQ");
                            lUser.USER_ID = useId;
                            lUser.PAGE_ID = dataid;
                            lUser.PROJECT_ID = ProjectId;
                            lUser.PAGE_NAME = dataid == 0 ? dataname : datatab;
                            lUser.CREATEDBY = userName;
                            lUser.CREATEDDATE = DateTime.Now;
                            entity.T_USER_ACTIVEPAGE.Add(lUser);
                            entity.SaveChanges();
                        }
                        result = "Successfully AddPin";
                    }
                    else if (linkText.Trim() == "UnPin Tab")
                    {
                        var pagename = dataid == 0 ? dataname : datatab;
                        var list = entity.T_USER_ACTIVEPAGE.Where(x => x.USER_ID == useId && x.PAGE_NAME.Trim() == pagename.Trim() && x.PAGE_ID == dataid && x.PROJECT_ID == ProjectId).ToList();
                        foreach (var item in list)
                        {
                            entity.T_USER_ACTIVEPAGE.Remove(item);
                            entity.SaveChanges();
                        }
                        list = entity.T_USER_ACTIVEPAGE.Where(x => x.USER_ID == useId && x.PAGE_NAME.Trim() == pagename.Trim() && x.PAGE_ID == dataid && x.PROJECT_ID == ProjectId).ToList();

                        if (list.Count == 0)
                            result = "Successfully Remove Pin";
                    }
                    logger.Info(string.Format("Change Activate Tab end | UserId: {0} | UserName: {1}", useId, Username));
                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in AddDetelteActivateTab method | UserId: {0} | Data Id : {1} | Data Tab: {2} | Project Id: {3} | Username : {4}", useId, dataid, datatab, ProjectId, Username));
                ELogger.ErrorException(string.Format("Error occured User in AddDetelteActivateTab method | UserId: {0} | Data Id : {1} | Data Tab: {2} | Project Id: {3} | Username : {4}", useId, dataid, datatab, ProjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in AddDetelteActivateTab method | UserId: {0} | Data Id : {1} | Data Tab: {2} | Project Id: {3} | Username : {4}", useId, dataid, datatab, ProjectId, Username), ex.InnerException);
                throw;
            }
        }

        public List<T_USER_ACTIVEPAGE> ActivePinListByUserId(long userId)
        {
            try
            {
                logger.Info(string.Format("ActivePinListByUserId start | User id: {0} | Username: {1}", userId, Username));
                var result = entity.T_USER_ACTIVEPAGE.Where(x => x.USER_ID == userId).ToList();
                logger.Info(string.Format("ActivePinListByUserId end | User id: {0} | Username: {1}", userId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ActivePinListByUserId method | UserId: {0} | UserName: {1}", userId, Username));
                ELogger.ErrorException(string.Format("Error occured User in ActivePinListByUserId method | UserId: {0} | UserName: {1}", userId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ActivePinListByUserId method | UserId: {0} | UserName: {1}", userId, Username), ex.InnerException);
                throw;
            }
        }

        public T_USER_MAPPING ChangeUserStatus(int id, int check)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Change User Status start | UserId: {0} | UserName: {1}", id, Username));
                    var result = GetUserId(id);
                    if (result != null)
                    {
                        result.STATUS = check;
                        entity.SaveChanges();
                    }
                    else
                    {
                        ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));
                        var projectId = entity.GETNEXT_VAL(USER_SEQ, outparam);
                        var lUserId = long.Parse(outparam.Value.ToString());
                        T_USER_MAPPING mapper = new T_USER_MAPPING();
                        mapper.USER_MAPPING_ID = lUserId;
                        mapper.STATUS = check;
                        mapper.TESTER_ID = id;
                        entity.T_USER_MAPPING.Add(mapper);
                        entity.SaveChanges();
                    }
                    logger.Info(string.Format("Change User Status end | UserId: {0} | UserName: {1}", id, Username));
                    scope.Complete();
                    return new T_USER_MAPPING();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ChangeUserStatus method | UserId: {0} | UserName: {1}", id, Username));
                ELogger.ErrorException(string.Format("Error occured User in ChangeUserStatus method | UserId: {0} | UserName: {1}", id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ChangeUserStatus method | UserId: {0} | UserName: {1}", id, Username), ex.InnerException);
                throw;
            }
        }

        public List<T_DBCONNECTION> GetConnectionList()
        {
            try
            {
                logger.Info(string.Format("GetConnectionList start | Username: {0}", Username));
                var ConnList = entity.T_DBCONNECTION.ToList();
                logger.Info(string.Format("GetConnectionList end | Username: {0}", Username));
                return ConnList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetConnectionList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetConnectionList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetConnectionList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditConnection(DBconnectionViewModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("AddEditConnection start | Username: {0}", Username));
                    if (!string.IsNullOrEmpty(lEntity.UserName))
                    {
                        lEntity.UserName = lEntity.UserName.Trim();
                        lEntity.Password = lEntity.Password.Trim();
                    }
                    var flag = false;
                    if (lEntity.connectionId == 0)
                    {
                        var tbl = new T_DBCONNECTION();
                        tbl.DBCONNECTION_ID = Helper.NextTestSuiteId("SEQ_T_DBCONNECTION");
                        tbl.DATABASENAME = "metadata=res://*/Entities.Db_Context.csdl|res://*/Entities.Db_Context.ssdl|res://*/Entities.Db_Context.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=\"DATA SOURCE={0};PASSWORD={1};USER ID={2}\"";
                        tbl.USERNAME = lEntity.UserName;
                        tbl.PASSWORD = lEntity.Password;
                        tbl.PORT = lEntity.Port;
                        tbl.HOST = lEntity.Host;
                        tbl.SERVICENAME = lEntity.Service_Name;
                        tbl.SCHEMA = lEntity.Schema;
                        tbl.DECODE_METHOD = lEntity.DecodeMethod;
                        lEntity.connectionId = tbl.DBCONNECTION_ID;
                        entity.T_DBCONNECTION.Add(tbl);
                        entity.SaveChanges();
                        flag = true;
                    }
                    else
                    {
                        var tbl = entity.T_DBCONNECTION.Find(lEntity.connectionId);

                        if (tbl != null)
                        {
                            tbl.USERNAME = lEntity.UserName;
                            tbl.PASSWORD = lEntity.Password;
                            tbl.PORT = lEntity.Port;
                            tbl.HOST = lEntity.Host;
                            tbl.SERVICENAME = lEntity.Service_Name;
                            tbl.SCHEMA = lEntity.Schema;
                            tbl.DECODE_METHOD = lEntity.DecodeMethod;
                            entity.SaveChanges();
                        }
                        flag = true;
                    }
                    logger.Info(string.Format("AddEditConnection end | Username: {0}", Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in AddEditConnection method | ConnectionId : {0} | UserName: {1}", lEntity.connectionId,  Username));
                ELogger.ErrorException(string.Format("Error occured User in AddEditConnection method | ConnectionId : {0} | UserName: {1}", lEntity.connectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in AddEditConnection method | ConnectionId : {0} | UserName: {1}", lEntity.connectionId, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateConnectionExist(string Username, string Password, long? ConnectionId)
        {
            try
            {
                logger.Info(string.Format("CheckDuplicateConnectionExist start | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                var lresult = false;
                if (ConnectionId != null)
                {
                    lresult = entity.T_DBCONNECTION.Any(x => x.DBCONNECTION_ID != ConnectionId && x.USERNAME.ToLower().Trim() == Username.ToLower().Trim() && x.PASSWORD.ToLower().Trim() == Password.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_DBCONNECTION.Any(x => x.USERNAME.ToLower().Trim() == Username.ToLower().Trim() && x.PASSWORD.ToLower().Trim() == Password.ToLower().Trim());
                }
                logger.Info(string.Format("CheckDuplicateConnectionExist end | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in CheckDuplicateConnectionExist method | ConnectionId : {0} | UserName: {1}", ConnectionId, Username));
                ELogger.ErrorException(string.Format("Error occured User in CheckDuplicateConnectionExist method | ConnectionId : {0} | UserName: {1}", ConnectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in CheckDuplicateConnectionExist method | ConnectionId : {0} | UserName: {1}", ConnectionId, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeletConnection(long ConnectionId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("DeletConnection start | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                    var flag = false;
                    var result = entity.T_DBCONNECTION.FirstOrDefault(x => x.DBCONNECTION_ID == ConnectionId);
                    if (result != null)
                    {
                        entity.T_DBCONNECTION.Remove(result);
                        entity.SaveChanges();
                        flag = true;
                    }
                    logger.Info(string.Format("DeletConnection end | ConnectionId: {0} | Username: {1}", ConnectionId, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in DeletConnection method | ConnectionId : {0} | UserName: {1}", ConnectionId, Username));
                ELogger.ErrorException(string.Format("Error occured User in DeletConnection method | ConnectionId : {0} | UserName: {1}", ConnectionId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in DeletConnection method | ConnectionId : {0} | UserName: {1}", ConnectionId, Username), ex.InnerException);
                throw;
            }

        }

        #region User Role Privilages

        public List<T_TEST_ROLES> GetAllRoles()
        {
            try
            {
                logger.Info(string.Format("GetAllRoles start | Username: {0}", Username));
                var result = entity.T_TEST_ROLES.ToList();
                logger.Info(string.Format("GetAllRoles end | Username: {0}", Username));
                return result;

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetAllRoles method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetAllRoles method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetAllRoles method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public T_TEST_ROLES GetRoleById(long roleId)
        {
            try
            {
                logger.Info(string.Format("GetRoleById start | roleId: {0} | Username: {1}", roleId, Username));
                var result = entity.T_TEST_ROLES.Where(x => x.ROLE_ID == roleId).FirstOrDefault();
                logger.Info(string.Format("GetRoleById end | roleId: {0} | Username: {1}", roleId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetRoleById method | RoleId : {0} | UserName: {1}", roleId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetRoleById method | RoleId : {0} | UserName: {1}", roleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetRoleById method | RoleId : {0} | UserName: {1}", roleId, Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_PRIVILEGE> GetAllPrivileges()
        {
            try
            {
                logger.Info(string.Format("GetAllPrivileges start | Username: {0}", Username));
                var result = entity.T_TEST_PRIVILEGE.ToList();
                logger.Info(string.Format("GetAllPrivileges end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetAllPrivileges method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetAllPrivileges method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetAllPrivileges method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_PRIVILEGE_ROLE_MAPPING> GetAllPrivilegesRoleMapping()
        {
            try
            {
                logger.Info(string.Format("GetAllPrivilegesRoleMapping start | Username: {0}", Username));
                var result = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.ToList();
                logger.Info(string.Format("GetAllPrivilegesRoleMapping end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetAllPrivilegesRoleMapping method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetAllPrivilegesRoleMapping method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetAllPrivilegesRoleMapping method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_USER_ROLE_MAPPING> GetAllUserRoleMapping()
        {
            try
            {
                logger.Info(string.Format("GetAllUserRoleMapping start | Username: {0}", Username));
                var result = entity.T_TEST_USER_ROLE_MAPPING.ToList();
                logger.Info(string.Format("GetAllUserRoleMapping end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetAllUserRoleMapping method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetAllUserRoleMapping method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetAllUserRoleMapping method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_USER_ROLE_MAPPING> GetUserRoleMappingByUserId(long userId)
        {
            try
            {
                logger.Info(string.Format("GetUserRoleMappingByUserId start | Username: {0}", Username));
                var result = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == userId).ToList();
                logger.Info(string.Format("GetUserRoleMappingByUserId end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserRoleMappingByUserId method | UserId: {0} | UserName: {1}", userId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserRoleMappingByUserId method | UserId: {0} | UserName: {1}", userId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserRoleMappingByUserId method | UserId: {0} | UserName: {1}", userId, Username), ex.InnerException);
                throw;
            }
        }

        public T_TEST_PRIVILEGE_ROLE_MAPPING GetPrivilegesRoleMappingByPrivilegeRoleMappingId(long privilegeRoleMappingId)
        {
            try
            {
                logger.Info(string.Format("GetPrivilegesRoleMappingByPrivilegeRoleMappingId start | privilegeRoleMappingId {0} | Username: {1}", privilegeRoleMappingId, Username));
                var result = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.PRIVILEGE_ROLE_MAP_ID == privilegeRoleMappingId).FirstOrDefault();
                logger.Info(string.Format("GetPrivilegesRoleMappingByPrivilegeRoleMappingId end | privilegeRoleMappingId {0} | Username: {1}", privilegeRoleMappingId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetPrivilegesRoleMappingByPrivilegeRoleMappingId method | privilegeRoleMappingId: {0} | UserName: {1}", privilegeRoleMappingId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetPrivilegesRoleMappingByPrivilegeRoleMappingId method | privilegeRoleMappingId: {0} | UserName: {1}", privilegeRoleMappingId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetPrivilegesRoleMappingByPrivilegeRoleMappingId method | privilegeRoleMappingId: {0} | UserName: {1}", privilegeRoleMappingId, Username), ex.InnerException);
                throw;
            }
        }

        public List<T_TEST_PRIVILEGE_ROLE_MAPPING> GetPrivilegesRoleMappingByRoleId(long roleId)
        {
            try
            {
                logger.Info(string.Format("GetPrivilegesRoleMappingByRoleId start | Username: {0}", Username));
                var result = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == roleId).ToList();
                logger.Info(string.Format("GetPrivilegesRoleMappingByRoleId end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetPrivilegesRoleMappingByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetPrivilegesRoleMappingByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetPrivilegesRoleMappingByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditPrivilageRoleMapping(PrivilegeRoleMappingViewModel privilegeRoleMappingEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool flag = false;
                    if (privilegeRoleMappingEntity.PrivilegeRoleMappingId == 0)
                    {
                        logger.Info(string.Format("Add PrivilegeRoleMapping start | Application: {0} | Username: {1}", "PrivilegeRoleMapping", Username));
                        if (privilegeRoleMappingEntity.PrivilegeListModel.Count > 0)
                        {
                            foreach (var item in privilegeRoleMappingEntity.PrivilegeListModel)
                            {
                                var privilegeRoleMappingTbl = new T_TEST_PRIVILEGE_ROLE_MAPPING();
                                privilegeRoleMappingTbl.PRIVILEGE_ROLE_MAP_ID = Helper.NextTestSuiteId("REL_PRIVILEGE_ROLE_MAPPING");
                                privilegeRoleMappingTbl.ROLE_ID = privilegeRoleMappingEntity.RoleId;
                                privilegeRoleMappingTbl.PRIVILEGE_ID = item.PrivilegeId;
                                privilegeRoleMappingTbl.ISACTIVE = item.IsActive.Value;
                                privilegeRoleMappingTbl.CREATOR = Username;
                                privilegeRoleMappingTbl.CREATOR_DATE = DateTime.Now;

                                entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Add(privilegeRoleMappingTbl);
                                entity.SaveChanges();
                            }
                            flag = true;
                            logger.Info(string.Format("Add PrivilegeRoleMapping end | Application: {0} | Username: {1}", "PrivilegeRoleMapping", Username));
                        }
                        else
                        {
                            flag = false;
                            logger.Info(string.Format("Add PrivilegeRoleMapping privilege not present. end | Application: {0} | Username: {1}", "PrivilegeRoleMapping", Username));
                        }
                    }
                    else
                    {
                        var privilegeRoleMappingTbl = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Find(privilegeRoleMappingEntity.PrivilegeRoleMappingId);
                        logger.Info(string.Format("Edit PrivilegeRoleMapping start | Application: {0} | PrivilegeRoleMappingId: {1} | Username: {2}", "PrivilegeRoleMapping", privilegeRoleMappingEntity.PrivilegeRoleMappingId, Username));
                        if (privilegeRoleMappingTbl != null)
                        {
                            foreach (var item in privilegeRoleMappingEntity.PrivilegeListModel)
                            {
                                privilegeRoleMappingTbl.ROLE_ID = privilegeRoleMappingEntity.RoleId;
                                privilegeRoleMappingTbl.PRIVILEGE_ID = item.PrivilegeId;
                                privilegeRoleMappingTbl.ISACTIVE = item.IsActive.Value;
                                entity.SaveChanges();
                            }
                        }
                        flag = true;
                        logger.Info(string.Format("Edit PrivilegeRoleMapping end | Application: {0} |PrivilegeRoleMappingId: {1} | Username: {2}", "PrivilegeRoleMapping", privilegeRoleMappingEntity.PrivilegeRoleMappingId, Username));
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in AddEditPrivilageRoleMapping method | PrivilageRoleMappingId: {0} | UserName: {1}", privilegeRoleMappingEntity.PrivilegeRoleMappingId, Username));
                ELogger.ErrorException(string.Format("Error occured User in AddEditPrivilageRoleMapping method | PrivilageRoleMappingId: {0} | UserName: {1}", privilegeRoleMappingEntity.PrivilegeRoleMappingId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in AddEditPrivilageRoleMapping method | PrivilageRoleMappingId: {0} | UserName: {1}", privilegeRoleMappingEntity.PrivilegeRoleMappingId, Username), ex.InnerException);
                throw;
            }
        }

        //Get All role privilege map list
        public List<PrivilegesDataModel> GetPrivilegesByRoleId(long roleId)
        {
            try
            {
                logger.Info(string.Format("GetPrivilegesByRoleId start | User id: {0} | Username: {1}", roleId, Username));
                List<PrivilegesDataModel> privilegesList = new List<PrivilegesDataModel>();
                var data = from tpr in entity.T_TEST_PRIVILEGE_ROLE_MAPPING
                           join tp in entity.T_TEST_PRIVILEGE on tpr.PRIVILEGE_ID equals tp.PRIVILEGE_ID
                           where tpr.ROLE_ID == roleId
                           select new
                           {
                               tp.PRIVILEGE_ID,
                               tp.PRIVILEGE_NAME
                           };

                foreach (var item in data)
                {
                    privilegesList.Add(new PrivilegesDataModel
                    {
                        PrivilegeId = (long)item.PRIVILEGE_ID,
                        PrivilegeName = item.PRIVILEGE_NAME
                    });
                }
                logger.Info(string.Format("GetPrivilegesByRoleId end | User id: {0} | Username: {1}", roleId, Username));
                return privilegesList.Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetPrivilegesByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetPrivilegesByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetPrivilegesByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeletePrivilagesByRoleId(long roleId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete PrivilegeRoleMapping start | RoleId: {0} | Username: {1}", roleId, Username));
                    var flag = false;
                    var result = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == roleId).ToList();
                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Remove(item);
                            entity.SaveChanges();
                        }
                        flag = true;
                    }
                    logger.Info(string.Format("Delete PrivilegeRoleMapping end | RoleId: {0} | Username: {1}", roleId, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in DeletePrivilagesByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username));
                ELogger.ErrorException(string.Format("Error occured User in DeletePrivilagesByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in DeletePrivilagesByRoleId method | RoleId: {0} | UserName: {1}", roleId, Username), ex.InnerException);
                throw;
            }
        }

        public T_TESTER_INFO ConverUserModel(UserModel userModel)
        {
            try
            {
                logger.Info(string.Format("ConverUserModel start | Username: {0}", Username));
                T_TESTER_INFO obj = new T_TESTER_INFO();
                obj.TESTER_NAME_LAST = userModel.TESTER_NAME_LAST;
                obj.TESTER_NAME_M = userModel.TESTER_NAME_M;
                obj.TESTER_NAME_F = userModel.TESTER_NAME_F;
                obj.TESTER_MAIL = userModel.TESTER_MAIL;
                obj.TESTER_LOGIN_NAME = userModel.TESTER_LOGIN_NAME;
                obj.TESTER_PWD = userModel.TESTER_PWD;
                obj.COMPANY_ID = userModel.COMPANY_ID;
                obj.AVAILABLE_MARK = userModel.AVAILABLE_MARK;
                obj.TESTER_ID = userModel.TESTER_ID;
                logger.Info(string.Format("ConverUserModel end | Username: {0}", Username));
                return obj;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ConverUserModel method | TesterId: {0} | UserName: {1}", userModel.TESTER_ID, Username));
                ELogger.ErrorException(string.Format("Error occured User in ConverUserModel method | TesterId: {0} | UserName: {1}", userModel.TESTER_ID, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ConverUserModel method | TesterId: {0} | UserName: {1}", userModel.TESTER_ID, Username), ex.InnerException);
                throw;
            }
        }

        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }

        public List<UserConfigrationViewModel> GetUserConfigrationList(string schema, string lconstring)
        {
            try
            {
                logger.Info(string.Format("GetUserConfigrationList start | Username: {0}", Username));
                DataSet lds = new DataSet();
                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[1];
                ladd_refer_image[0] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[0].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_USER_CONFIGURATION";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<UserConfigrationViewModel> resultList = dt.AsEnumerable().Select(row =>
                    new UserConfigrationViewModel
                    {
                        Id = row.Field<decimal>("USERCONFIGID"),
                        MainKey = row.Field<string>("MAINKEY"),
                        SubKey = row.Field<string>("SUBKEY"),
                        UserId = row.Field<decimal?>("USERID"),
                        MARSUserName = row.Field<string>("MARSUserName"),
                        BLOBValue = row.Field<byte[]>("BLOBValuestr"),
                        //BLOBValuestr = row.Field<string>("BLOBValuestr"),
                        BLOBValueType = row.Field<short?>("BLOBVALUETYPE"),
                        BLOBType = row.Field<string>("BLOBType"),
                        Description = row.Field<string>("DESCRIPTION")

                    }).ToList();

                resultList.ForEach(item =>
                {
                    item.BLOBValuestr = item.BLOBValue == null ? "" : Encoding.UTF8.GetString(item.BLOBValue);
                });
                logger.Info(string.Format("GetUserConfigrationList end | Username: {0}", Username));
                return resultList.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in ConverUserModel method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in ConverUserModel method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in ConverUserModel method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditUserConfigration(UserConfigrationViewModel modelEntity, string lConnectionStr, string lSchema)
        {
            try
            {
                var flag = false;
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(lConnectionStr);
                if (modelEntity.Id == 0)
                {
                    logger.Info(string.Format("Add User Configration start | Mainkey: {0} | Username: {1}", modelEntity.MainKey, Username));
                    long createId = Helper.NextTestSuiteId("T_SEQ_USERCONFIG");
                  
                    lconnection.Open();
                    ltransaction = lconnection.BeginTransaction();
                    string lcmdquery = "INSERT INTO " + lSchema + ".T_USER_CONFIGURATION(USERCONFIGID, MAINKEY, SUBKEY, USERID, BLOBVALUE, BLOBVALUETYPE, DESCRIPTION, CREATEDBY, CREATEDON) values(:1,:2,:3,:4,utl_raw.cast_to_raw(:5),:6,:7,:8,:9)";
                    using (var lcmd = lconnection.CreateCommand())
                    {
                        lcmd.CommandText = lcmdquery;

                        OracleParameter USERCONFIGID_oparam = new OracleParameter();
                        USERCONFIGID_oparam.OracleDbType = OracleDbType.Long;
                        USERCONFIGID_oparam.Value = createId;

                        OracleParameter MAINKEY_oparam = new OracleParameter();
                        MAINKEY_oparam.OracleDbType = OracleDbType.Varchar2;
                        MAINKEY_oparam.Value = modelEntity.MainKey;

                        OracleParameter SUBKEY_oparam = new OracleParameter();
                        SUBKEY_oparam.OracleDbType = OracleDbType.Varchar2;
                        SUBKEY_oparam.Value = modelEntity.SubKey;

                        OracleParameter USERID_oparam = new OracleParameter();
                        USERID_oparam.OracleDbType = OracleDbType.Long;
                        USERID_oparam.Value = modelEntity.UserId;

                        OracleParameter BLOBVALUE_oparam = new OracleParameter();
                        BLOBVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                        BLOBVALUE_oparam.Value = modelEntity.BLOBValuestr;

                        OracleParameter BLOBVALUETYPE_oparam = new OracleParameter();
                        BLOBVALUETYPE_oparam.OracleDbType = OracleDbType.Int16;
                        BLOBVALUETYPE_oparam.Value = modelEntity.BLOBValueType;

                        OracleParameter DESCRIPTION_oparam = new OracleParameter();
                        DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                        DESCRIPTION_oparam.Value = modelEntity.Description;

                        OracleParameter CREATEDBY_oparam = new OracleParameter();
                        CREATEDBY_oparam.OracleDbType = OracleDbType.Varchar2;
                        CREATEDBY_oparam.Value = modelEntity.Create_Person;

                        OracleParameter CREATEDON_oparam = new OracleParameter();
                        CREATEDON_oparam.OracleDbType = OracleDbType.Date;
                        CREATEDON_oparam.Value = DateTime.Now;

                        lcmd.Parameters.Add(USERCONFIGID_oparam);
                        lcmd.Parameters.Add(MAINKEY_oparam);
                        lcmd.Parameters.Add(SUBKEY_oparam);
                        lcmd.Parameters.Add(USERID_oparam);
                        lcmd.Parameters.Add(BLOBVALUE_oparam);
                        lcmd.Parameters.Add(BLOBVALUETYPE_oparam);
                        lcmd.Parameters.Add(DESCRIPTION_oparam);
                        lcmd.Parameters.Add(CREATEDBY_oparam);
                        lcmd.Parameters.Add(CREATEDON_oparam);

                        lcmd.ExecuteNonQuery();
                        flag = true;

                        logger.Info(string.Format("Add User Configration end | Mainkey: {0} | Username: {1}", modelEntity.MainKey, Username));
                        ltransaction.Commit();
                    }
                }
                else
                {
                    logger.Info(string.Format("Edit User Configration start | Mainkey: {0} | Username: {1}", modelEntity.MainKey, Username));
                    lconnection.Open();
                    ltransaction = lconnection.BeginTransaction();
                    string lcmdquery = "UPDATE " + lSchema + ".T_USER_CONFIGURATION SET MAINKEY = :1, SUBKEY = :2, USERID = :3, BLOBVALUE = utl_raw.cast_to_raw(:4), " +
                        "BLOBVALUETYPE = :5, DESCRIPTION = :6, MODIFYBY = :7, MODIFYON = :8 WHERE USERCONFIGID = :9";
                    using (var lcmd = lconnection.CreateCommand())
                    {
                        lcmd.CommandText = lcmdquery;
                        
                        OracleParameter MAINKEY_oparam = new OracleParameter();
                        MAINKEY_oparam.OracleDbType = OracleDbType.Varchar2;
                        MAINKEY_oparam.Value = modelEntity.MainKey;

                        OracleParameter SUBKEY_oparam = new OracleParameter();
                        SUBKEY_oparam.OracleDbType = OracleDbType.Varchar2;
                        SUBKEY_oparam.Value = modelEntity.SubKey;

                        OracleParameter USERID_oparam = new OracleParameter();
                        USERID_oparam.OracleDbType = OracleDbType.Long;
                        USERID_oparam.Value = modelEntity.UserId;

                        OracleParameter BLOBVALUE_oparam = new OracleParameter();
                        BLOBVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                        BLOBVALUE_oparam.Value = modelEntity.BLOBValuestr;

                        OracleParameter BLOBVALUETYPE_oparam = new OracleParameter();
                        BLOBVALUETYPE_oparam.OracleDbType = OracleDbType.Int16;
                        BLOBVALUETYPE_oparam.Value = modelEntity.BLOBValueType;

                        OracleParameter DESCRIPTION_oparam = new OracleParameter();
                        DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                        DESCRIPTION_oparam.Value = modelEntity.Description;

                        OracleParameter MODIFYEDBY_oparam = new OracleParameter();
                        MODIFYEDBY_oparam.OracleDbType = OracleDbType.Varchar2;
                        MODIFYEDBY_oparam.Value = modelEntity.Create_Person;

                        OracleParameter MODIFYEDON_oparam = new OracleParameter();
                        MODIFYEDON_oparam.OracleDbType = OracleDbType.Date;
                        MODIFYEDON_oparam.Value = DateTime.Now;

                        OracleParameter USERCONFIGID_oparam = new OracleParameter();
                        USERCONFIGID_oparam.OracleDbType = OracleDbType.Long;
                        USERCONFIGID_oparam.Value = modelEntity.Id;

                        lcmd.Parameters.Add(MAINKEY_oparam);
                        lcmd.Parameters.Add(SUBKEY_oparam);
                        lcmd.Parameters.Add(USERID_oparam);
                        lcmd.Parameters.Add(BLOBVALUE_oparam);
                        lcmd.Parameters.Add(BLOBVALUETYPE_oparam);
                        lcmd.Parameters.Add(DESCRIPTION_oparam);
                        lcmd.Parameters.Add(MODIFYEDBY_oparam);
                        lcmd.Parameters.Add(MODIFYEDON_oparam);
                        lcmd.Parameters.Add(USERCONFIGID_oparam);

                        lcmd.ExecuteNonQuery();
                        flag = true;

                        logger.Info(string.Format("Edit User Configration end | Mainkey: {0} | Username: {1}", modelEntity.MainKey, Username));
                        ltransaction.Commit();
                    }
                }

                lconnection.Close();
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in AddEditUserConfigration method | UserId: {0} | UserName: {1}", modelEntity.Id,  Username));
                ELogger.ErrorException(string.Format("Error occured User in AddEditUserConfigration method | UserId: {0} | UserName: {1}", modelEntity.Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in AddEditUserConfigration method | UserId: {0} | UserName: {1}", modelEntity.Id, Username), ex.InnerException);
                throw;
            }
        }

        public bool UpdateBolbValue(UserConfigrationViewModel modelEntity, string lConnectionStr, string lSchema)
        {
            try
            {
                var flag = false;
                OracleTransaction ltransaction;
                OracleConnection lconnection = new OracleConnection(lConnectionStr);
               
                    logger.Info(string.Format("Edit User BolbValue Configration start | Id: {0} | Username: {1}", modelEntity.Id, Username));
                    lconnection.Open();
                    ltransaction = lconnection.BeginTransaction();
                    string lcmdquery = "UPDATE " + lSchema + ".T_USER_CONFIGURATION SET BLOBVALUE = utl_raw.cast_to_raw(:1), " +
                        "MODIFYBY = :2, MODIFYON = :3 WHERE USERCONFIGID = :4";
                    using (var lcmd = lconnection.CreateCommand())
                    {
                        lcmd.CommandText = lcmdquery;

                        OracleParameter BLOBVALUE_oparam = new OracleParameter();
                        BLOBVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                        BLOBVALUE_oparam.Value = modelEntity.BLOBValuestr;

                        OracleParameter MODIFYEDBY_oparam = new OracleParameter();
                        MODIFYEDBY_oparam.OracleDbType = OracleDbType.Varchar2;
                        MODIFYEDBY_oparam.Value = modelEntity.Create_Person;

                        OracleParameter MODIFYEDON_oparam = new OracleParameter();
                        MODIFYEDON_oparam.OracleDbType = OracleDbType.Date;
                        MODIFYEDON_oparam.Value = DateTime.Now;

                        OracleParameter USERCONFIGID_oparam = new OracleParameter();
                        USERCONFIGID_oparam.OracleDbType = OracleDbType.Long;
                        USERCONFIGID_oparam.Value = modelEntity.Id;

                        lcmd.Parameters.Add(BLOBVALUE_oparam);
                        lcmd.Parameters.Add(MODIFYEDBY_oparam);
                        lcmd.Parameters.Add(MODIFYEDON_oparam);
                        lcmd.Parameters.Add(USERCONFIGID_oparam);

                        lcmd.ExecuteNonQuery();
                        flag = true;

                    logger.Info(string.Format("Edit User BolbValue Configration start | Id: {0} | Username: {1}", modelEntity.Id, Username));
                    ltransaction.Commit();
                    }

                lconnection.Close();
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in UpdateBolbValue method | UserId: {0} | UserName: {1}", modelEntity.Id, Username));
                ELogger.ErrorException(string.Format("Error occured User in UpdateBolbValue method | UserId: {0} | UserName: {1}", modelEntity.Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in UpdateBolbValue method | UserId: {0} | UserName: {1}", modelEntity.Id, Username), ex.InnerException);
                throw;
            }
        }

        public String GetUserConfigrationNameById(long Id)
        {
            try
            {
                logger.Info(string.Format("Get User ConfigrationName start | Id: {0} | Username: {1}", Id, Username));
                var result = entity.T_USER_CONFIGURATION.FirstOrDefault(x => x.USERCONFIGID == Id).MAINKEY;
                logger.Info(string.Format("Get User ConfigrationName end | Id: {0} | Username: {1}", Id, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetUserConfigrationNameById method | UserId: {0} | UserName: {1}", Id, Username));
                ELogger.ErrorException(string.Format("Error occured User in GetUserConfigrationNameById method | UserId: {0} | UserName: {1}", Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetUserConfigrationNameByIdmethod | UserId: {0} | UserName: {1}", Id, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteUserConfigration(long Id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete User Configration start | Id: {0} | Username: {1}", Id, Username));
                    var flag = false;
                    var result = entity.T_USER_CONFIGURATION.FirstOrDefault(x => x.USERCONFIGID == Id);
                    if (result != null)
                    {
                        entity.T_USER_CONFIGURATION.Remove(result);
                        entity.SaveChanges();
                        flag = true;
                    }
                    logger.Info(string.Format("Delete User Configration end | Id: {0} | Username: {1}", Id, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in DeleteUserConfigration method | UserId: {0} | UserName: {1}", Id, Username));
                ELogger.ErrorException(string.Format("Error occured User in DeleteUserConfigration method | UserId: {0} | UserName: {1}", Id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in DeleteUserConfigration | UserId: {0} | UserName: {1}", Id, Username), ex.InnerException);
                throw;
            }

        }
        #endregion
    }
}

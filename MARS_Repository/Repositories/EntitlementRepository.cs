using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MARS_Repository.Repositories
{
    public class EntitlementRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        public List<UserRoleMappingModel> GetAllUsers()
        {
            try
            {
                logger.Info(string.Format("Get All User start | Username: {0}", Username));
                List<UserRoleMappingModel> models = new List<UserRoleMappingModel>();

                var UserList = entity.T_TESTER_INFO.ToList();

                foreach (var item in UserList)
                {
                    var obj = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == item.TESTER_ID).FirstOrDefault();
                    if (obj == null || obj.IS_DELETED != 1)
                    {
                        UserRoleMappingModel user = new UserRoleMappingModel();
                        user.UserId = (long)item.TESTER_ID;
                        user.UserName = item.TESTER_LOGIN_NAME;
                        models.Add(user);
                    }
                }
                logger.Info(string.Format("Get All User end | Username: {0}", Username));
                return models;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetAllUsers method | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetAllUsers method | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetAllUsers method | Username: {0}", Username), ex.InnerException);
                throw;
            }
        }

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
                logger.Error(string.Format("Error occured Entitlement in GetAllRoles method | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetAllRoles method | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetAllRoles method | Username: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<UserRoleMappingViewModel> ListOfUserRoleMapping()
        {
            try
            {
                logger.Info(string.Format("ListOfUserRoleMapping start | Username: {0}", Username));
                List<UserRoleMappingViewModel> lList = new List<UserRoleMappingViewModel>();
                List<UserRoleMappingModel> models = new List<UserRoleMappingModel>();

                var result = (from x in entity.T_TEST_USER_ROLE_MAPPING
                              join y in entity.T_TESTER_INFO on x.USER_ID equals y.TESTER_ID
                              join z in entity.T_TEST_ROLES on x.ROLE_ID equals z.ROLE_ID
                              select new UserRoleMappingModel
                              {
                                  RoleId = (long)x.ROLE_ID,
                                  Roles = z.ROLE_NAME,
                                  UserId = (long)x.USER_ID,
                                  UserName = y.TESTER_LOGIN_NAME,
                              }).ToList();


                foreach (var item in result)
                {
                    var obj = entity.T_USER_MAPPING.Where(x => x.TESTER_ID == item.UserId).FirstOrDefault();
                    if (obj == null || obj.IS_DELETED != 1)
                    {
                        UserRoleMappingModel user = new UserRoleMappingModel();
                        user.UserId = item.UserId;
                        user.UserName = item.UserName;
                        user.RoleId = item.RoleId;
                        user.Roles = item.Roles;
                        models.Add(user);
                    }
                }

                var userlist = models.GroupBy(p => p.UserId).Select(g => g.First()).ToList();

                foreach (var item in userlist)
                {
                    var model = new UserRoleMappingViewModel();

                    var userrolelist = result.Where(x => x.UserId == item.UserId).ToList();

                    model.UserId = item.UserId;
                    model.UserName = item.UserName;
                    model.Roles = string.Join(",", userrolelist.Where(y => !string.IsNullOrEmpty(y.Roles)).Select(x => x.Roles).Distinct());
                    model.RoleId = string.Join(",", userrolelist.Where(y => y.RoleId > 0).Select(x => x.RoleId).Distinct());
                    lList.Add(model);
                }
                logger.Info(string.Format("ListOfUserRoleMapping end | Username: {0}", Username));
                return lList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in ListOfUserRoleMapping method | Username: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in ListOfUserRoleMapping method | Username: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in ListOfUserRoleMapping method | Username: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public string GetUserName(long UserId)
        {
            try
            {
                var Name = string.Empty;
                logger.Info(string.Format("Get User Name start | UserId: {0} | Username: {1}", UserId, Username));

                Name = entity.T_TESTER_INFO.Where(x => x.TESTER_ID == UserId).FirstOrDefault().TESTER_LOGIN_NAME;

                logger.Info(string.Format("Get User Name end | UserId: {0} | Username: {1}", UserId, Username));

                return Name;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetUserName method |  UserId: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetUserName method | UserId: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetUserName method |  UserId: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditUserRoleMapping(UserRoleMappingViewModel model)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var flag = false;
                    List<string> roleid = new List<string>();
                    logger.Info(string.Format("Add User Role start | RoleIds: {0} | Username: {1}", model.RoleId, Username));

                    if (!string.IsNullOrEmpty(model.RoleId))
                        roleid = model.RoleId.Split(',').ToList();

                    var userlst = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == model.UserId).ToList();
                    if (userlst.Count > 0)
                    {
                        foreach (var user in userlst)
                        {
                            var checkExistOrNot = roleid.Any(x => x == Convert.ToString(user.ROLE_ID));
                            if (!checkExistOrNot)
                            {
                                var Id = Convert.ToDecimal(user.ROLE_ID);
                                var userroletbl = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == model.UserId && x.ROLE_ID == Id).FirstOrDefault();
                                if (userroletbl != null)
                                {
                                    entity.T_TEST_USER_ROLE_MAPPING.Remove(userroletbl);
                                    entity.SaveChanges();
                                }
                            }
                        }
                    }

                    foreach (var item in roleid)
                    {
                        var Id = Convert.ToDecimal(item);
                        var userroletbl = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == model.UserId && x.ROLE_ID == Id).FirstOrDefault();
                        if (userroletbl == null)
                        {
                            var UserRoleModel = new T_TEST_USER_ROLE_MAPPING();
                            UserRoleModel.USER_ROLE_MAP_ID = Helper.NextTestSuiteId("REL_T_TEST_USER_ROLE_MAPPING");
                            UserRoleModel.USER_ID = model.UserId;
                            UserRoleModel.ROLE_ID = Convert.ToDecimal(item);
                            UserRoleModel.ISACTIVE = 1;
                            UserRoleModel.CREATOR = model.Create_Person;
                            UserRoleModel.CREATOR_DATE = DateTime.Now;

                            entity.T_TEST_USER_ROLE_MAPPING.Add(UserRoleModel);
                            entity.SaveChanges();
                        }
                    }
                    logger.Info(string.Format("Add User Role end | RoleIds: {0} | Username: {1}", model.RoleId, Username));
                    flag = true;
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in AddEditUserRoleMapping method |  RoleIds: {0} | Username: {1}", model.RoleId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in AddEditUserRoleMapping method |  RoleIds: {0} | Username: {1}", model.RoleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in AddEditUserRoleMapping method |  RoleIds: {0} | Username: {1}", model.RoleId, Username), ex.InnerException);
                throw;
            }
        }

        public bool DeleteUserRole(long UserId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete User Role start | UserId: {0} | Username: {1}", UserId, Username));
                    var flag = false;
                    var result = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == UserId).ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            entity.T_TEST_USER_ROLE_MAPPING.Remove(item);
                            entity.SaveChanges();
                        }
                        flag = true;
                    }
                    logger.Info(string.Format("Delete User Role end | UserId: {0} | Username: {1}", UserId, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in DeleteUserRole method |  UserId: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in DeleteUserRole method |  UserId: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in DeleteUserRole method |  UserId: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckExistOrNotUser(long UserId)
        {
            try
            {
                var lresult = false;
                logger.Info(string.Format("Check Duplicate User Exist start | UserName: {0}", Username));
                if (UserId != null)
                {
                    var result = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == UserId).ToList();
                    if (result.Count > 0)
                        lresult = true;
                }

                logger.Info(string.Format("Check Duplicate User Exist end | UserName: {0}", Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in CheckExistOrNotUser method |  UserId: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in CheckExistOrNotUser method |  UserId: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in DeleteUserRole method |  UserId: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool CreateRole(string Role)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Create Role start | UserId: {0} | Role: {1}", Username, Role));
                    var flag = false;
                    var lRoleModel = new T_TEST_ROLES();
                    lRoleModel.ROLE_ID = Helper.NextTestSuiteId("SEQ_T_TEST_ROLES");
                    lRoleModel.ROLE_NAME = Role;
                    lRoleModel.CREATOR = Username;
                    lRoleModel.ISACTIVE = 1;
                    lRoleModel.CREATOR_DATE = DateTime.Now;

                    entity.T_TEST_ROLES.Add(lRoleModel);
                    entity.SaveChanges();
                    flag = true;
                    logger.Info(string.Format("Create Role end  | Username: {0}", Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in CreateRole method |  Role: {0} | Username: {1}", Role, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in CreateRole method |  Role: {0} | Username: {1}", Role, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in CreateRole method |  Role: {0} | Username: {1}", Role, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckRoleExist(string Role)
        {
            try
            {
                logger.Info(string.Format("Create Role start | UserId: {0} | Role: {1}", Username, Role));
                var lResult = entity.T_TEST_ROLES.Any(x => x.ROLE_NAME.ToUpper() == Role.ToUpper());

                logger.Info(string.Format("Create Role end  | Username: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in CheckRoleExist method |  Role: {0} | Username: {1}", Role, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in CheckRoleExist method |  Role: {0} | Username: {1}", Role, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in CheckRoleExist method |  Role: {0} | Username: {1}", Role, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddRole(decimal UserId, string RoleIds)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var flag = false;
                    List<string> roleid = new List<string>();
                    logger.Info(string.Format("Add User Role start | RoleIds: {0} | Username: {1}", RoleIds, Username));

                    if (!string.IsNullOrEmpty(RoleIds))
                        roleid = RoleIds.Split(',').ToList();

                    foreach (var item in roleid)
                    {
                        var Id = Convert.ToDecimal(item);
                        var userroletbl = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == UserId && x.ROLE_ID == Id).FirstOrDefault();
                        if (userroletbl == null)
                        {
                            var UserRoleModel = new T_TEST_USER_ROLE_MAPPING();
                            UserRoleModel.USER_ROLE_MAP_ID = Helper.NextTestSuiteId("REL_T_TEST_USER_ROLE_MAPPING");
                            UserRoleModel.USER_ID = UserId;
                            UserRoleModel.ROLE_ID = Convert.ToDecimal(item);
                            UserRoleModel.ISACTIVE = 1;
                            UserRoleModel.CREATOR = Username;
                            UserRoleModel.CREATOR_DATE = DateTime.Now;

                            entity.T_TEST_USER_ROLE_MAPPING.Add(UserRoleModel);
                            entity.SaveChanges();
                        }
                    }
                    logger.Info(string.Format("Add User Role end | RoleIds: {0} | Username: {1}", RoleIds, Username));
                    flag = true;
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in AddRole method | User Id : {0} |Role: {1} | Username: {1}", UserId, RoleIds, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in AddRole method |  User Id : {0} |Role: {1} | Username: {1}", UserId, RoleIds, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in AddRole method | User Id : {0} |Role: {1} | Username: {1}", UserId, RoleIds, Username), ex.InnerException);
                throw;
            }
        }

        public List<RolePrivilegeMappingViewModel> GetPriviledgebyRole(string RoleName)
        {
            var lPrivilegeList = new List<RolePrivilegeMappingViewModel>();
            try
            {
                logger.Info(string.Format("GetPriviledgebyRole start | RoleName: {0} | Username: {1}", RoleName, Username));

                lPrivilegeList = entity.T_TEST_PRIVILEGE.Select(y => new RolePrivilegeMappingViewModel
                {
                    PrivilegeId = (long)y.PRIVILEGE_ID,
                    Name = y.PRIVILEGE_NAME,
                    Module = y.MODULE,
                    Desc = y.DESCRIPTION,
                    Selected = false
                }).ToList();
                var lRoleId = entity.T_TEST_ROLES.FirstOrDefault(x => x.ROLE_NAME.ToUpper() == RoleName.ToUpper()).ROLE_ID;
                if (lRoleId > 0)
                {
                    var lSelectedPrivileges = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == lRoleId).Distinct().Select(y => y.PRIVILEGE_ID).ToList();

                    if (lSelectedPrivileges.Count() > 0)
                    {
                        lPrivilegeList.Where(y => lSelectedPrivileges.Contains((long)y.PrivilegeId)).ToList().ForEach(item => { item.Selected = true; });
                    }
                }
                logger.Info(string.Format("GetPriviledgebyRole end | RoleName: {0} | Username: {1}", RoleName, Username));
                return lPrivilegeList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetPriviledgebyRole method |  Role: {0} | Username: {1}", RoleName, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetPriviledgebyRole method |  Role: {0} | Username: {1}", RoleName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetPriviledgebyRole method |  Role: {0} | Username: {1}", RoleName, Username), ex.InnerException);
                throw;
            }
        }

        public List<RolePrivilegeMappingViewModel> GetPriviledgebyRoleId(long RoleId)
        {
            var lPrivilegeList = new List<RolePrivilegeMappingViewModel>();
            try
            {
                logger.Info(string.Format("GetPriviledgebyRoleId start | RoleId: {0} | Username: {1}", RoleId, Username));
                lPrivilegeList = entity.T_TEST_PRIVILEGE.Select(y => new RolePrivilegeMappingViewModel
                {
                    PrivilegeId = (long)y.PRIVILEGE_ID,
                    Name = y.PRIVILEGE_NAME,
                    Module = y.MODULE,
                    Desc = y.DESCRIPTION,
                    Selected = false
                }).ToList();
                if (RoleId > 0)
                {
                    var lSelectedPrivileges = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == RoleId).Distinct().Select(y => y.PRIVILEGE_ID).ToList();

                    if (lSelectedPrivileges.Count() > 0)
                    {
                        lPrivilegeList.Where(y => lSelectedPrivileges.Contains((long)y.PrivilegeId)).ToList().ForEach(item => { item.Selected = true; });
                    }
                }
                logger.Info(string.Format("GetPriviledgebyRoleId end | RoleId: {0} | Username: {1}", RoleId, Username));
                return lPrivilegeList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetPriviledgebyRoleId method |  Role: {0} | Username: {1}", RoleId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetPriviledgebyRoleId method |  Role: {0} | Username: {1}", RoleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetPriviledgebyRoleId method |  Role: {0} | Username: {1}", RoleId, Username), ex.InnerException);
                throw; ;
            }
        }

        public string GetRoleName(long RoleId)
        {
            try
            {
                var RoleName = string.Empty;
                logger.Info(string.Format("Get Role Name start | RoleId: {0} | Username: {1}", RoleId, Username));

                RoleName = entity.T_TEST_ROLES.Where(x => x.ROLE_ID == RoleId).FirstOrDefault().ROLE_NAME;

                logger.Info(string.Format("Get Role Name end | RoleId: {0} | Username: {1}", RoleId, Username));

                return RoleName;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetRoleName method |  Role: {0} | Username: {1}", RoleId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetRoleName method |  Role: {0} | Username: {1}", RoleId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetRoleName method |  Role: {0} | Username: {1}", RoleId, Username), ex.InnerException);
                throw; ;
            }
        }

        public bool AddEditPrivilageRoleMapping(PrivilegeRoleMappingViewModel model)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var flag = false;
                    List<string> privilegeid = new List<string>();
                    logger.Info(string.Format("Add Privilage for Role start | PrivilegeIds: {0} | Username: {1}", model.PrivilegeId, Username));

                    if (!string.IsNullOrEmpty(model.PrivilegeId))
                        privilegeid = model.PrivilegeId.Split(',').ToList();

                    var roleprivilagelst = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == model.RoleId).ToList();
                    if (roleprivilagelst.Count > 0)
                    {
                        foreach (var role in roleprivilagelst)
                        {
                            var checkExistOrNot = privilegeid.Any(x => x == Convert.ToString(role.PRIVILEGE_ID));
                            if (!checkExistOrNot)
                            {
                                var Id = Convert.ToDecimal(role.PRIVILEGE_ID);
                                var privilageroletbl = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == model.RoleId && x.PRIVILEGE_ID == Id).FirstOrDefault();
                                if (privilageroletbl != null)
                                {
                                    entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Remove(privilageroletbl);
                                    entity.SaveChanges();
                                }
                            }
                        }
                    }

                    foreach (var item in privilegeid)
                    {
                        var Id = Convert.ToDecimal(item);
                        var privilageroletbl = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => x.ROLE_ID == model.RoleId && x.PRIVILEGE_ID == Id).FirstOrDefault();
                        if (privilageroletbl == null)
                        {
                            var PrivilageRoleModel = new T_TEST_PRIVILEGE_ROLE_MAPPING();
                            PrivilageRoleModel.PRIVILEGE_ROLE_MAP_ID = Helper.NextTestSuiteId("REL_PRIVILEGE_ROLE_MAPPING");
                            PrivilageRoleModel.ROLE_ID = model.RoleId;
                            PrivilageRoleModel.PRIVILEGE_ID = Convert.ToDecimal(item);
                            PrivilageRoleModel.ISACTIVE = 1;
                            PrivilageRoleModel.CREATOR = Username;
                            PrivilageRoleModel.CREATOR_DATE = DateTime.Now;

                            entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Add(PrivilageRoleModel);
                            entity.SaveChanges();
                        }
                    }
                    logger.Info(string.Format("Add Privilage for Role end | PrivilegeIds: {0} | Username: {1}", model.PrivilegeId, Username));
                    flag = true;
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in AddEditPrivilageRoleMapping method | PrivilegeIds: {0} | Username: {1}", model.PrivilegeId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in AddEditPrivilageRoleMapping method | PrivilegeIds: {0} | Username: {1}", model.PrivilegeId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in AddEditPrivilageRoleMapping method |PrivilegeIds: {0} | Username: {1}", model.PrivilegeId, Username), ex.InnerException);
                throw; 
            }
        }
        public List<PrivilegeViewModel> GetRolePrivilege(long UserId)
        {
            try
            {
                logger.Info(string.Format("GetRolePrivilege start | UserId: {0} | Username: {1}", UserId, Username));
                var lPriviledgeUserList = new List<PrivilegeViewModel>();
                #region tiger,reduce db op
                var rp = from r in entity.T_TEST_USER_ROLE_MAPPING
                         from x in entity.T_TEST_PRIVILEGE_ROLE_MAPPING
                         from p in entity.T_TEST_PRIVILEGE
                         where r.USER_ID == UserId
                         && x.ROLE_ID == r.ROLE_ID
                         && x.PRIVILEGE_ID == p.PRIVILEGE_ID
                         select p;
                foreach (var y in rp)
                {
                    lPriviledgeUserList.Add(new PrivilegeViewModel() {
                        PRIVILEGE_ID = y.PRIVILEGE_ID,
                        DESCRIPTION = y.DESCRIPTION,
                        MODULE = y.MODULE,
                        PRIVILEGE_NAME = y.PRIVILEGE_NAME
                    });
                }
                /// old code
                //var lRoleList = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == UserId).ToList();
                //if (lRoleList.Count() > 0)
                //{
                //    var lRoleIds = lRoleList.Select(x => x.ROLE_ID).ToList();
                //    var lPrivilegeList = entity.T_TEST_PRIVILEGE_ROLE_MAPPING.Where(x => lRoleIds.Contains(x.ROLE_ID)).ToList();
                //    if (lPrivilegeList.Count() > 0)
                //    {
                //        var lPrivilegeIds = lPrivilegeList.Select(x => x.PRIVILEGE_ID).ToList();
                //        lPriviledgeUserList = entity.T_TEST_PRIVILEGE.Where(x => lPrivilegeIds.Contains(x.PRIVILEGE_ID)).Select(y => new PrivilegeViewModel
                //        {
                //            PRIVILEGE_ID = y.PRIVILEGE_ID,
                //            DESCRIPTION = y.DESCRIPTION,
                //            MODULE = y.MODULE,
                //            PRIVILEGE_NAME = y.PRIVILEGE_NAME
                //        }).ToList();
                //    }
                //}
                /// old code 
                #endregion
                logger.Info(string.Format("GetRolePrivilege end | UserId: {0} | Username: {1}", UserId, Username));
                return lPriviledgeUserList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetRolePrivilege method | UserId: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetRolePrivilege method | UserId: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetRolePrivilege method |UserId: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public List<RoleViewModel> GetRoleByUser(long UserId)
        {
            try
            {
                logger.Info(string.Format("GetRoleByUser start | UserId: {0} | Username: {1}", UserId, Username));
                var lRoleUserList = new List<RoleViewModel>();
                var lRoleList = entity.T_TEST_USER_ROLE_MAPPING.Where(x => x.USER_ID == UserId).ToList();
                if (lRoleList.Count() > 0)
                {
                    var lRoleIds = lRoleList.Select(x => x.ROLE_ID).ToList();

                    lRoleUserList = entity.T_TEST_ROLES.Where(p => lRoleIds.Any(p2 => p2 == p.ROLE_ID)).ToList().Select(y => new RoleViewModel
                    {
                        ROLE_ID = y.ROLE_ID,
                        ROLE_NAME = y.ROLE_NAME
                    }).ToList();
                }
                logger.Info(string.Format("GetRoleByUser end | UserId: {0} | Username: {1}", UserId, Username));
                return lRoleUserList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Entitlement in GetRoleByUser method | UserId: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Entitlement in GetRoleByUser method | UserId: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Entitlement in GetRoleByUser method |UserId: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }
    }
}

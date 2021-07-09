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
    public class ConfigurationGridRepository
    {
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        public string Username = string.Empty;

        public List<GridModel> ListAllGrid()
        {
            try
            {
                logger.Info(string.Format("List All Grid start | Username: {0}", Username));
                var result = entity.T_MASTERGRIDLIST.ToList().Select(a => new GridModel
                {
                    GridId = a.GRID_ID,
                    GridName = a.GRIDNAME,
                }).ToList();
                logger.Info(string.Format("List All Grid end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in ListAllGrid method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Grid in ListAllGrid method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in ListAllGrid method |UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public List<GridViewModel> GetGridbyId(long Id, long userId)
        {
            try
            {
                logger.Info(string.Format("Get Grid start | User id: {0} | Username: {1}", userId, Username));
                var result = from t1 in entity.T_MASTERGRIDLIST
                             join t2 in entity.T_GRIDLIST on t1.GRID_ID equals t2.GRID_ID
                             where t2.GRID_ID == Id && t2.USER_ID == userId
                             select new GridViewModel
                             {
                                 GridId = t2.ID,
                                 GridName = t1.GRIDNAME,
                                 GridColomn = t2.COLUMNNAME,
                                 GridSize = t2.COLUMNSIZE
                             };
                logger.Info(string.Format("Get Grid end | User id: {0} | Username: {1}", userId, Username));
                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in GetGridbyId method | User Id: {0} | Username: {1}", userId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in GetGridbyId method | User Id: {0} | Username: {1}", userId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in GetGridbyId method | User Id: {0} | Username: {1}", userId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditAppGridWidth(AppGridWidthModel appGridWidth, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Application AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (appGridWidth != null)
                {
                    if (appGridWidth.NameId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Name";
                        appgrid.COLUMNSIZE = appGridWidth.Name;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.NameId);
                        appgrid.COLUMNSIZE = appGridWidth.Name;
                        entity.SaveChanges();
                    }


                    if (appGridWidth.DescriptionId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Description";
                        appgrid.COLUMNSIZE = appGridWidth.Description;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.DescriptionId);
                        appgrid.COLUMNSIZE = appGridWidth.Description;
                        entity.SaveChanges();
                    }

                    if (appGridWidth.VersionId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Version";
                        appgrid.COLUMNSIZE = appGridWidth.Version;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.VersionId);
                        appgrid.COLUMNSIZE = appGridWidth.Version;
                        entity.SaveChanges();
                    }

                    if (appGridWidth.ExtraRequirementId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Extra Requirement";
                        appgrid.COLUMNSIZE = appGridWidth.ExtraRequirement;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.ExtraRequirementId);
                        appgrid.COLUMNSIZE = appGridWidth.ExtraRequirement;
                        entity.SaveChanges();
                    }

                    if (appGridWidth.ModeId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Mode";
                        appgrid.COLUMNSIZE = appGridWidth.Mode;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.ModeId);
                        appgrid.COLUMNSIZE = appGridWidth.Mode;
                        entity.SaveChanges();
                    }

                    if (appGridWidth.ExplorerBitsId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "MARS Explorer Bits";
                        appgrid.COLUMNSIZE = appGridWidth.ExplorerBits;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.ExplorerBitsId);
                        appgrid.COLUMNSIZE = appGridWidth.ExplorerBits;
                        entity.SaveChanges();
                    }

                    if (appGridWidth.ActionsId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Actions";
                        appgrid.COLUMNSIZE = appGridWidth.Actions;
                        appgrid.GRID_ID = appGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(appGridWidth.ActionsId);
                        appgrid.COLUMNSIZE = appGridWidth.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;

                    logger.Info(string.Format("Application AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                }
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditAppGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditAppGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditAppGridWidthUs method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public List<GridViewModel> GetGridList(long userId, string gridname)
        {
            try
            {
                logger.Info(string.Format("Get GridList start | UseName: {0}", Username));
                var gridval = entity.T_MASTERGRIDLIST.Where(x => x.GRIDNAME.ToLower().Trim() == gridname.ToLower().Trim()).FirstOrDefault();
                var result = entity.T_GRIDLIST.Where(x => x.USER_ID == userId && x.GRID_ID == gridval.GRID_ID).ToList().Select(x => new GridViewModel
                {
                    GridId = x.ID,
                    GridColomn = x.COLUMNNAME,
                    GridSize = x.COLUMNSIZE
                }).ToList();
                logger.Info(string.Format("Get GridList end | UseName: {0}", Username));

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in GetGridList method | User Id: {0} | Grid Name : {1} | Username: {1}", userId, gridname, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in GetGridList method | User Id: {0} | Grid Name : {1} | Username: {1}", userId, gridname, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in GetGridList method | User Id: {0} | Grid Name : {1} | Username: {1}", userId, gridname, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditProjectGridWidth(ProjectGridWidthModel proGridWidth, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Project AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (proGridWidth != null)
                {
                    if (proGridWidth.NameId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Name";
                        appgrid.COLUMNSIZE = proGridWidth.Name;
                        appgrid.GRID_ID = proGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(proGridWidth.NameId);
                        appgrid.COLUMNSIZE = proGridWidth.Name;
                        entity.SaveChanges();
                    }


                    if (proGridWidth.DescriptionId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Description";
                        appgrid.COLUMNSIZE = proGridWidth.Description;
                        appgrid.GRID_ID = proGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(proGridWidth.DescriptionId);
                        appgrid.COLUMNSIZE = proGridWidth.Description;
                        entity.SaveChanges();
                    }

                    if (proGridWidth.ApplicationId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Application";
                        appgrid.COLUMNSIZE = proGridWidth.Application;
                        appgrid.GRID_ID = proGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(proGridWidth.ApplicationId);
                        appgrid.COLUMNSIZE = proGridWidth.Application;
                        entity.SaveChanges();
                    }

                    if (proGridWidth.StatusId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Status";
                        appgrid.COLUMNSIZE = proGridWidth.Status;
                        appgrid.GRID_ID = proGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(proGridWidth.StatusId);
                        appgrid.COLUMNSIZE = proGridWidth.Status;
                        entity.SaveChanges();
                    }


                    if (proGridWidth.ActionsId == 0)
                    {
                        var appgrid = new T_GRIDLIST();
                        appgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        appgrid.COLUMNNAME = "Actions";
                        appgrid.COLUMNSIZE = proGridWidth.Actions;
                        appgrid.GRID_ID = proGridWidth.GridId;
                        appgrid.USER_ID = UserId;
                        appgrid.CREATEDBY = userName;
                        appgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(appgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var appgrid = entity.T_GRIDLIST.Find(proGridWidth.ActionsId);
                        appgrid.COLUMNSIZE = proGridWidth.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("Project AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));

                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditProjectGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditProjectGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditProjectGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditKeywordGridWidth(KeywordGridWidthModel keywordGridWidth, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Keyword AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (keywordGridWidth != null)
                {
                    if (keywordGridWidth.NameId == 0)
                    {
                        var keygrid = new T_GRIDLIST();
                        keygrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        keygrid.COLUMNNAME = "Name";
                        keygrid.COLUMNSIZE = keywordGridWidth.Name;
                        keygrid.GRID_ID = keywordGridWidth.GridId;
                        keygrid.USER_ID = UserId;
                        keygrid.CREATEDBY = userName;
                        keygrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(keygrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var keygrid = entity.T_GRIDLIST.Find(keywordGridWidth.NameId);
                        keygrid.COLUMNSIZE = keywordGridWidth.Name;
                        entity.SaveChanges();
                    }


                    if (keywordGridWidth.ControlTypeId == 0)
                    {
                        var keygrid = new T_GRIDLIST();
                        keygrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        keygrid.COLUMNNAME = "Control Type";
                        keygrid.COLUMNSIZE = keywordGridWidth.ControlType;
                        keygrid.GRID_ID = keywordGridWidth.GridId;
                        keygrid.USER_ID = UserId;
                        keygrid.CREATEDBY = userName;
                        keygrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(keygrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var keygrid = entity.T_GRIDLIST.Find(keywordGridWidth.ControlTypeId);
                        keygrid.COLUMNSIZE = keywordGridWidth.ControlType;
                        entity.SaveChanges();
                    }

                    if (keywordGridWidth.EntryDataId == 0)
                    {
                        var keygrid = new T_GRIDLIST();
                        keygrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        keygrid.COLUMNNAME = "Entry in Data File";
                        keygrid.COLUMNSIZE = keywordGridWidth.EntryData;
                        keygrid.GRID_ID = keywordGridWidth.GridId;
                        keygrid.USER_ID = UserId;
                        keygrid.CREATEDBY = userName;
                        keygrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(keygrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var keygrid = entity.T_GRIDLIST.Find(keywordGridWidth.EntryDataId);
                        keygrid.COLUMNSIZE = keywordGridWidth.EntryData;
                        entity.SaveChanges();
                    }


                    if (keywordGridWidth.ActionsId == 0)
                    {
                        var keygrid = new T_GRIDLIST();
                        keygrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        keygrid.COLUMNNAME = "Actions";
                        keygrid.COLUMNSIZE = keywordGridWidth.Actions;
                        keygrid.GRID_ID = keywordGridWidth.GridId;
                        keygrid.USER_ID = UserId;
                        keygrid.CREATEDBY = userName;
                        keygrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(keygrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var keygrid = entity.T_GRIDLIST.Find(keywordGridWidth.ActionsId);
                        keygrid.COLUMNSIZE = keywordGridWidth.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("Keyword AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditKeywordGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditKeywordGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditKeywordGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
           
        }

        public bool AddEditTestSuiteGridWidth(TestSuiteGridWidthModel TSModel, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("TestSuite AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (TSModel != null)
                {
                    if (TSModel.NameId == 0)
                    {
                        var TSgrid = new T_GRIDLIST();
                        TSgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TSgrid.COLUMNNAME = "Name";
                        TSgrid.COLUMNSIZE = TSModel.Name;
                        TSgrid.GRID_ID = TSModel.GridId;
                        TSgrid.USER_ID = UserId;
                        TSgrid.CREATEDBY = userName;
                        TSgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TSgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TSgrid = entity.T_GRIDLIST.Find(TSModel.NameId);
                        TSgrid.COLUMNSIZE = TSModel.Name;
                        entity.SaveChanges();
                    }

                    if (TSModel.DescriptionId == 0)
                    {
                        var TSgrid = new T_GRIDLIST();
                        TSgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TSgrid.COLUMNNAME = "Description";
                        TSgrid.COLUMNSIZE = TSModel.Description;
                        TSgrid.GRID_ID = TSModel.GridId;
                        TSgrid.USER_ID = UserId;
                        TSgrid.CREATEDBY = userName;
                        TSgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TSgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TSgrid = entity.T_GRIDLIST.Find(TSModel.DescriptionId);
                        TSgrid.COLUMNSIZE = TSModel.Description;
                        entity.SaveChanges();
                    }

                    if (TSModel.ApplicationId == 0)
                    {
                        var TSgrid = new T_GRIDLIST();
                        TSgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TSgrid.COLUMNNAME = "Application";
                        TSgrid.COLUMNSIZE = TSModel.Application;
                        TSgrid.GRID_ID = TSModel.GridId;
                        TSgrid.USER_ID = UserId;
                        TSgrid.CREATEDBY = userName;
                        TSgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TSgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TSgrid = entity.T_GRIDLIST.Find(TSModel.ApplicationId);
                        TSgrid.COLUMNSIZE = TSModel.Application;
                        entity.SaveChanges();
                    }

                    if (TSModel.ProjectId == 0)
                    {
                        var TSgrid = new T_GRIDLIST();
                        TSgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TSgrid.COLUMNNAME = "Project";
                        TSgrid.COLUMNSIZE = TSModel.Project;
                        TSgrid.GRID_ID = TSModel.GridId;
                        TSgrid.USER_ID = UserId;
                        TSgrid.CREATEDBY = userName;
                        TSgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TSgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TSgrid = entity.T_GRIDLIST.Find(TSModel.ProjectId);
                        TSgrid.COLUMNSIZE = TSModel.Project;
                        entity.SaveChanges();
                    }

                    if (TSModel.ActionsId == 0)
                    {
                        var TSgrid = new T_GRIDLIST();
                        TSgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TSgrid.COLUMNNAME = "Actions";
                        TSgrid.COLUMNSIZE = TSModel.Actions;
                        TSgrid.GRID_ID = TSModel.GridId;
                        TSgrid.USER_ID = UserId;
                        TSgrid.CREATEDBY = userName;
                        TSgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TSgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TSgrid = entity.T_GRIDLIST.Find(TSModel.ActionsId);
                        TSgrid.COLUMNSIZE = TSModel.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("TestSuite AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditTestSuiteGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditTestSuiteGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditTestSuiteGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditTestCaseGridWidth(TestCaseGridWidthModel TCModel, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("TestCase AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (TCModel != null)
                {
                    if (TCModel.NameId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Name";
                        Vargrid.COLUMNSIZE = TCModel.Name;
                        Vargrid.GRID_ID = TCModel.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(TCModel.NameId);
                        Vargrid.COLUMNSIZE = TCModel.Name;
                        entity.SaveChanges();
                    }

                    if (TCModel.DescriptionId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Description";
                        Vargrid.COLUMNSIZE = TCModel.Description;
                        Vargrid.GRID_ID = TCModel.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(TCModel.DescriptionId);
                        Vargrid.COLUMNSIZE = TCModel.Description;
                        entity.SaveChanges();
                    }

                    if (TCModel.ApplicationId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Application";
                        Vargrid.COLUMNSIZE = TCModel.Application;
                        Vargrid.GRID_ID = TCModel.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(TCModel.ApplicationId);
                        Vargrid.COLUMNSIZE = TCModel.Application;
                        entity.SaveChanges();
                    }

                    if (TCModel.TestSuiteId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "TestSuite";
                        Vargrid.COLUMNSIZE = TCModel.TestSuite;
                        Vargrid.GRID_ID = TCModel.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(TCModel.TestSuiteId);
                        Vargrid.COLUMNSIZE = TCModel.TestSuite;
                        entity.SaveChanges();
                    }

                    if (TCModel.ActionsId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Actions";
                        Vargrid.COLUMNSIZE = TCModel.Actions;
                        Vargrid.GRID_ID = TCModel.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(TCModel.ActionsId);
                        Vargrid.COLUMNSIZE = TCModel.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("TestCase AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured grid page in AddEditTestCaseGridWidth method | Username: {0} |", Username));
                ELogger.ErrorException(string.Format("Error occured grid page in AddEditTestCaseGridWidth method | Username: {0}", Username), ex);
                throw;
            }
        }

        public bool AddEditObjectGridWidth(ObjectGridWidthModel ObjModel, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Object AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (ObjModel != null)
                {
                    if (ObjModel.NameId == 0)
                    {
                        var objgrid = new T_GRIDLIST();
                        objgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        objgrid.COLUMNNAME = "Object Name";
                        objgrid.COLUMNSIZE = ObjModel.Name;
                        objgrid.GRID_ID = ObjModel.GridId;
                        objgrid.USER_ID = UserId;
                        objgrid.CREATEDBY = userName;
                        objgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(objgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var objgrid = entity.T_GRIDLIST.Find(ObjModel.NameId);
                        objgrid.COLUMNSIZE = ObjModel.Name;
                        entity.SaveChanges();
                    }

                    if (ObjModel.InternalAccessId == 0)
                    {
                        var objgrid = new T_GRIDLIST();
                        objgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        objgrid.COLUMNNAME = "Internal Access";
                        objgrid.COLUMNSIZE = ObjModel.InternalAccess;
                        objgrid.GRID_ID = ObjModel.GridId;
                        objgrid.USER_ID = UserId;
                        objgrid.CREATEDBY = userName;
                        objgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(objgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var objgrid = entity.T_GRIDLIST.Find(ObjModel.InternalAccessId);
                        objgrid.COLUMNSIZE = ObjModel.InternalAccess;
                        entity.SaveChanges();
                    }

                    if (ObjModel.TypeId == 0)
                    {
                        var objgrid = new T_GRIDLIST();
                        objgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        objgrid.COLUMNNAME = "Type";
                        objgrid.COLUMNSIZE = ObjModel.Type;
                        objgrid.GRID_ID = ObjModel.GridId;
                        objgrid.USER_ID = UserId;
                        objgrid.CREATEDBY = userName;
                        objgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(objgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var objgrid = entity.T_GRIDLIST.Find(ObjModel.TypeId);
                        objgrid.COLUMNSIZE = ObjModel.Type;
                        entity.SaveChanges();
                    }

                    if (ObjModel.PegwindowId == 0)
                    {
                        var objgrid = new T_GRIDLIST();
                        objgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        objgrid.COLUMNNAME = "Parent Pegwindow";
                        objgrid.COLUMNSIZE = ObjModel.Pegwindow;
                        objgrid.GRID_ID = ObjModel.GridId;
                        objgrid.USER_ID = UserId;
                        objgrid.CREATEDBY = userName;
                        objgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(objgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var objgrid = entity.T_GRIDLIST.Find(ObjModel.PegwindowId);
                        objgrid.COLUMNSIZE = ObjModel.Pegwindow;
                        entity.SaveChanges();
                    }

                    if (ObjModel.ActionsId == 0)
                    {
                        var objgrid = new T_GRIDLIST();
                        objgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        objgrid.COLUMNNAME = "Actions";
                        objgrid.COLUMNSIZE = ObjModel.Actions;
                        objgrid.GRID_ID = ObjModel.GridId;
                        objgrid.USER_ID = UserId;
                        objgrid.CREATEDBY = userName;
                        objgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(objgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var objgrid = entity.T_GRIDLIST.Find(ObjModel.ActionsId);
                        objgrid.COLUMNSIZE = ObjModel.Actions;
                        entity.SaveChanges();
                    }

                    if (ObjModel.SelectId == 0)
                    {
                        var objgrid = new T_GRIDLIST();
                        objgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        objgrid.COLUMNNAME = "Select All";
                        objgrid.COLUMNSIZE = ObjModel.Select;
                        objgrid.GRID_ID = ObjModel.GridId;
                        objgrid.USER_ID = UserId;
                        objgrid.CREATEDBY = userName;
                        objgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(objgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var objgrid = entity.T_GRIDLIST.Find(ObjModel.SelectId);
                        objgrid.COLUMNSIZE = ObjModel.Select;
                        entity.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("Object AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditObjectGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditObjectGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditObjectGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditVaribleGridWidth(VariableGridWidthModel variableGridWidth, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Variable AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (variableGridWidth != null)
                {
                    if (variableGridWidth.NameId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Name";
                        Vargrid.COLUMNSIZE = variableGridWidth.Name;
                        Vargrid.GRID_ID = variableGridWidth.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(variableGridWidth.NameId);
                        Vargrid.COLUMNSIZE = variableGridWidth.Name;
                        entity.SaveChanges();
                    }

                    if (variableGridWidth.TypeId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Type";
                        Vargrid.COLUMNSIZE = variableGridWidth.Type;
                        Vargrid.GRID_ID = variableGridWidth.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(variableGridWidth.TypeId);
                        Vargrid.COLUMNSIZE = variableGridWidth.Type;
                        entity.SaveChanges();
                    }

                    if (variableGridWidth.ValueId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Value";
                        Vargrid.COLUMNSIZE = variableGridWidth.Value;
                        Vargrid.GRID_ID = variableGridWidth.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(variableGridWidth.ValueId);
                        Vargrid.COLUMNSIZE = variableGridWidth.Value;
                        entity.SaveChanges();
                    }

                    if (variableGridWidth.StatusId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Status";
                        Vargrid.COLUMNSIZE = variableGridWidth.Status;
                        Vargrid.GRID_ID = variableGridWidth.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(variableGridWidth.StatusId);
                        Vargrid.COLUMNSIZE = variableGridWidth.Status;
                        entity.SaveChanges();
                    }

                    if (variableGridWidth.ActionsId == 0)
                    {
                        var Vargrid = new T_GRIDLIST();
                        Vargrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Vargrid.COLUMNNAME = "Actions";
                        Vargrid.COLUMNSIZE = variableGridWidth.Actions;
                        Vargrid.GRID_ID = variableGridWidth.GridId;
                        Vargrid.USER_ID = UserId;
                        Vargrid.CREATEDBY = userName;
                        Vargrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Vargrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Vargrid = entity.T_GRIDLIST.Find(variableGridWidth.ActionsId);
                        Vargrid.COLUMNSIZE = variableGridWidth.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;
                }
                logger.Info(string.Format("Variable AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditVaribleGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditVaribleGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditVaribleGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditUserGridWidth(UserGridWidthModel userGridWidth, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("User AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (userGridWidth != null)
                {
                    if (userGridWidth.FNameId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "First Name";
                        Usergrid.COLUMNSIZE = userGridWidth.FName;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.FNameId);
                        Usergrid.COLUMNSIZE = userGridWidth.FName;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.MNameId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "Middle Name";
                        Usergrid.COLUMNSIZE = userGridWidth.MName;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.MNameId);
                        Usergrid.COLUMNSIZE = userGridWidth.MName;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.LNameId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "Last Name";
                        Usergrid.COLUMNSIZE = userGridWidth.LName;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.LNameId);
                        Usergrid.COLUMNSIZE = userGridWidth.LName;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.NameId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "User Name";
                        Usergrid.COLUMNSIZE = userGridWidth.Name;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.NameId);
                        Usergrid.COLUMNSIZE = userGridWidth.Name;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.EmailId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "Email Address";
                        Usergrid.COLUMNSIZE = userGridWidth.Email;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.EmailId);
                        Usergrid.COLUMNSIZE = userGridWidth.Email;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.CompanyId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "Company Name";
                        Usergrid.COLUMNSIZE = userGridWidth.Company;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.CompanyId);
                        Usergrid.COLUMNSIZE = userGridWidth.Company;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.StatusId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "Status";
                        Usergrid.COLUMNSIZE = userGridWidth.Status;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.StatusId);
                        Usergrid.COLUMNSIZE = userGridWidth.Status;
                        entity.SaveChanges();
                    }

                    if (userGridWidth.ActionsId == 0)
                    {
                        var Usergrid = new T_GRIDLIST();
                        Usergrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Usergrid.COLUMNNAME = "Actions";
                        Usergrid.COLUMNSIZE = userGridWidth.Actions;
                        Usergrid.GRID_ID = userGridWidth.GridId;
                        Usergrid.USER_ID = UserId;
                        Usergrid.CREATEDBY = userName;
                        Usergrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Usergrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Usergrid = entity.T_GRIDLIST.Find(userGridWidth.ActionsId);
                        Usergrid.COLUMNSIZE = userGridWidth.Actions;
                        entity.SaveChanges();
                    }
                    flag = true;
                    logger.Info(string.Format("User AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                }
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditUserGridWidth method | User Id: {0} | Username: {1}", UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditUserGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditUserGridWidth method | User Id: {0} | Username: {1}", UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditTestCasePqGridWidth(TestcasePqGridWidthModel testcasePqGrid, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Testcase PqGrid AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (testcasePqGrid != null)
                {
                    if (testcasePqGrid.KeywordId == 0)
                    {
                        var TCPgrid = new T_GRIDLIST();
                        TCPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TCPgrid.COLUMNNAME = "Keyword";
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Keyword;
                        TCPgrid.GRID_ID = testcasePqGrid.GridId;
                        TCPgrid.USER_ID = UserId;
                        TCPgrid.CREATEDBY = userName;
                        TCPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TCPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TCPgrid = entity.T_GRIDLIST.Find(testcasePqGrid.KeywordId);
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Keyword;
                        entity.SaveChanges();
                    }

                    if (testcasePqGrid.ObjectId == 0)
                    {
                        var TCPgrid = new T_GRIDLIST();
                        TCPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TCPgrid.COLUMNNAME = "Object";
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Object;
                        TCPgrid.GRID_ID = testcasePqGrid.GridId;
                        TCPgrid.USER_ID = UserId;
                        TCPgrid.CREATEDBY = userName;
                        TCPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TCPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TCPgrid = entity.T_GRIDLIST.Find(testcasePqGrid.ObjectId);
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Object;
                        entity.SaveChanges();
                    }

                    if (testcasePqGrid.ParametersId == 0)
                    {
                        var TCPgrid = new T_GRIDLIST();
                        TCPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TCPgrid.COLUMNNAME = "Parameters";
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Parameters;
                        TCPgrid.GRID_ID = testcasePqGrid.GridId;
                        TCPgrid.USER_ID = UserId;
                        TCPgrid.CREATEDBY = userName;
                        TCPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TCPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TCPgrid = entity.T_GRIDLIST.Find(testcasePqGrid.ParametersId);
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Parameters;
                        entity.SaveChanges();
                    }

                    if (testcasePqGrid.CommentId == 0)
                    {
                        var TCPgrid = new T_GRIDLIST();
                        TCPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        TCPgrid.COLUMNNAME = "Comment";
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Comment;
                        TCPgrid.GRID_ID = testcasePqGrid.GridId;
                        TCPgrid.USER_ID = UserId;
                        TCPgrid.CREATEDBY = userName;
                        TCPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(TCPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var TCPgrid = entity.T_GRIDLIST.Find(testcasePqGrid.CommentId);
                        TCPgrid.COLUMNSIZE = testcasePqGrid.Comment;
                        entity.SaveChanges();
                    }

                    flag = true;
                }
                logger.Info(string.Format("Testcase PqGrid AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditTestCasePqGridWidth method | TestCaseGrid: {0} | User Id: {1} | Username: {2}", testcasePqGrid.KeywordId, UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditTestCasePqGridWidth method | TestCaseGrid: {0} | User Id: {1} | Username: {2}", testcasePqGrid.KeywordId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditTestCasePqGridWidth method | TestCaseGrid: {0} | User Id: {1} | Username: {2}", testcasePqGrid.KeywordId, UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditStoryboardPqGridWidth(StoryboardPqGridWidthModel storyboardPqGrid, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Storyboard PqGrid AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (storyboardPqGrid != null)
                {
                    if (storyboardPqGrid.ActionId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Action";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Action;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.ActionId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Action;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.StepsId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Steps";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Steps;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.StepsId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Steps;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.TestSuiteId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Test Suite";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.TestSuite;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.TestSuiteId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.TestSuite;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.TestCaseId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Test Case";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.TestCase;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.TestCaseId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.TestCase;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.DatasetId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Dataset";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Dataset;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.DatasetId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Dataset;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.BResultId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Base Line Data Result";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BResult;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.BResultId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BResult;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.BErrorCauseId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Base Line Data Error cause";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BErrorCause;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.BErrorCauseId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BErrorCause;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.BScriptStartId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Base Line Data Script Start";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BScriptStart;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.BScriptStartId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BScriptStart;
                        entity.SaveChanges();
                    }
                    if (storyboardPqGrid.BScriptDurationId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Base Line Data Script Duration";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BScriptDuration;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.BScriptDurationId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.BScriptDuration;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.CResultId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Comparison Line Data Result";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CResult;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.CResultId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CResult;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.CErrorCauseId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Comparison Line Data Error cause";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CErrorCause;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.CErrorCauseId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CErrorCause;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.CScriptStartId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Comparison Line Data Script Start";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CScriptStart;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.CScriptStartId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CScriptStart;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.CScriptDurationId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Comparison Line Data Script Duration";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CScriptDuration;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.CScriptDurationId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.CScriptDuration;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.DependencyId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Dependency";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Dependency;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.DependencyId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Dependency;
                        entity.SaveChanges();
                    }

                    if (storyboardPqGrid.DescriptionId == 0)
                    {
                        var SPgrid = new T_GRIDLIST();
                        SPgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        SPgrid.COLUMNNAME = "Description";
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Description;
                        SPgrid.GRID_ID = storyboardPqGrid.GridId;
                        SPgrid.USER_ID = UserId;
                        SPgrid.CREATEDBY = userName;
                        SPgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(SPgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var SPgrid = entity.T_GRIDLIST.Find(storyboardPqGrid.DescriptionId);
                        SPgrid.COLUMNSIZE = storyboardPqGrid.Description;
                        entity.SaveChanges();
                    }

                    flag = true;
                }
                logger.Info(string.Format("Storyboard PqGrid AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditStoryboardPqGridWidth method | StoryboardPqGrid: {0} | User Id: {1} | Username: {2}", storyboardPqGrid.GridId, UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditStoryboardPqGridWidth method | StoryboardPqGrid: {0} | User Id: {1} | Username: {2}", storyboardPqGrid.GridId, UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditStoryboardPqGridWidth method | StoryboardPqGrid: {0} | User Id: {1} | Username: {2}", storyboardPqGrid.GridId, UserId, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditLeftPanelGridWidth(LeftPanelPqGridWidthModel LRmodel, long UserId, string userName)
        {
            try
            {
                logger.Info(string.Format("Storyboard PqGrid AddEdit Grid start | User id: {0} | Username: {1}", UserId, Username));
                var flag = false;
                if (LRmodel != null)
                {
                    if (LRmodel.ResizeId == 0)
                    {
                        var Rgrid = new T_GRIDLIST();
                        Rgrid.ID = Helper.NextTestSuiteId("T_GRIDLIST_SEQ");
                        Rgrid.COLUMNNAME = "Resize Left Panel";
                        Rgrid.COLUMNSIZE = LRmodel.Resize;
                        Rgrid.GRID_ID = LRmodel.GridId;
                        Rgrid.USER_ID = UserId;
                        Rgrid.CREATEDBY = userName;
                        Rgrid.CREATEDDATE = DateTime.Now;
                        entity.T_GRIDLIST.Add(Rgrid);
                        entity.SaveChanges();
                    }
                    else
                    {
                        var Rgrid = entity.T_GRIDLIST.Find(LRmodel.ResizeId);
                        Rgrid.COLUMNSIZE = LRmodel.Resize;
                        entity.SaveChanges();
                    }

                    flag = true;
                }
                logger.Info(string.Format("Storyboard PqGrid AddEdit Grid end | User id: {0} | Username: {1}", UserId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Grid in AddEditLeftPanelGridWidth method | LeftPanelGrid: {0} | User Id: {1} | Username: {2}", LRmodel.GridId, UserId, Username));
                ELogger.ErrorException(string.Format("Error occured Grid in AddEditLeftPanelGridWidth method | LeftPanelGrid: {0} | User Id: {1} | Username: {2}", LRmodel.GridId, UserId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured Grid in AddEditLeftPanelGridWidth method | LeftPanelGrid: {0} | User Id: {1} | Username: {2}", LRmodel.GridId, UserId, Username), ex.InnerException);
                throw;
            }
        }
    }
}

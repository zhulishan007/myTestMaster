using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MARS_Repository.Repositories
{
    public class ProjectRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        public string GetProjectNameById(long Projectid)
        {
            try
            {
                logger.Info(string.Format("Get ProjectName Id start | projectid: {0} | Username: {1}", Projectid, Username));
                var lprojectname = enty.T_TEST_PROJECT.FirstOrDefault(x => x.PROJECT_ID == Projectid).PROJECT_NAME;
                logger.Info(string.Format("Get ProjectName Id start | projectid: {0} | Username: {1}", Projectid, Username));
                return lprojectname;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for GetProjectNameById method | Project Id : {0} | UserName: {1}", Projectid, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for GetProjectNameById method | Project Id : {0} | UserName: {1}", Projectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for GetProjectNameById method | Project Id : {0} | UserName: {1}", Projectid, Username), ex.InnerException);
                throw;
            }
        }
        public List<T_TEST_PROJECT> ListProject()
        {
            try
            {
                logger.Info(string.Format("List Project start | Username: {0}", Username));
                var result = enty.T_TEST_PROJECT.Where(x => x.PROJECT_NAME != null).ToList();
                logger.Info(string.Format("List Project end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for ListProject method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Project for ListProject method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for ListProject method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public string SaveProjectByUserId(List<Project> model, decimal userid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("SaveProjectByUserId start | User id: {0} | Username: {1}", userid, Username));
                    foreach (var item in model)
                    {
                        if (item.ProjectExists == 0)
                        {
                            var result = enty.REL_PROJECT_USER.Where(x => x.USER_ID == userid && x.PROJECT_ID == item.ProjectId).ToList();
                            if (result != null)
                            {
                                foreach (var itm in result)
                                {
                                    enty.REL_PROJECT_USER.Remove(itm);
                                    enty.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var lresult = enty.REL_PROJECT_USER.Where(x => x.USER_ID == userid && x.PROJECT_ID == item.ProjectId).ToList();
                            if (lresult.Count <= 0)
                            {
                                var tbl = new REL_PROJECT_USER();
                                tbl.RELATION_ID = Helper.NextTestSuiteId("SEQ_REL_PROJECT_USER");
                                tbl.PROJECT_ID = item.ProjectId;
                                tbl.USER_ID = userid;
                                enty.REL_PROJECT_USER.Add(tbl);
                                enty.SaveChanges();
                            }
                        }
                    }
                    logger.Info(string.Format("SaveProjectByUserId end | User id: {0} | Username: {1}", userid, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for SaveProjectByUserId method | User Id : {0} | UserName: {1}", userid, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for SaveProjectByUserId method | User Id : {0} | UserName: {1}", userid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for SaveProjectByUserId method | User Id : {0} | UserName: {1}", userid, Username), ex.InnerException);
                throw;
            }
        }

        public string DeleteProjectUserMapping(long uid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("DeleteProjectUserMapping start | User id: {0} | Username: {1}", uid, Username));
                    var result = enty.REL_PROJECT_USER.Where(x => x.USER_ID == uid).ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            enty.REL_PROJECT_USER.Remove(item);
                            enty.SaveChanges();
                        }
                        logger.Info(string.Format("DeleteProjectUserMapping end | User id: {0} | Username: {1}", uid, Username));
                        scope.Complete();
                        return "success";
                    }
                    logger.Info(string.Format("DeleteProjectUserMapping end | User id: {0} | Username: {1}", uid, Username));
                    scope.Complete();
                    return "error";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for DeleteProjectUserMapping method | User Id : {0} | UserName: {1}", uid, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for DeleteProjectUserMapping method | User Id : {0} | UserName: {1}", uid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for DeleteProjectUserMapping method | User Id : {0} | UserName: {1}", uid, Username), ex.InnerException);
                throw;
            }
        }
        public List<ProjectModel> ListProjectModel()
        {
            try
            {
                logger.Info(string.Format("ListProjectModel start | Username: {0}", Username));
                var lResult = new List<ProjectModel>();
                var lList = from x in enty.T_TEST_PROJECT
                            join y in enty.REL_APP_PROJ on x.PROJECT_ID equals y.PROJECT_ID
                            select new ProjectModel
                            {
                                ProjectId = x.PROJECT_ID,
                                Project = x.PROJECT_NAME,
                                ApplicationId = y.APPLICATION_ID
                            };

                if (lList.Count() > 0)
                    lResult = lList.ToList();

                logger.Info(string.Format("ListProjectModel end | Username: {0}", Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for ListProjectModel method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Project for ListProjectModel method |UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for ListProjectModel method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }

        public bool ChangeProjectName(string lProjectName, string lProjectdesc, long lProjectId)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Change ProjectName start | ProjectName: {0} | UserName: {1}", lProjectName, Username));
                    var lresult = false;
                    var lProject = enty.T_TEST_PROJECT.Find(lProjectId);
                    lProject.PROJECT_NAME = lProjectName;
                    lProject.PROJECT_DESCRIPTION = lProjectdesc;
                    enty.SaveChanges();
                    lresult = true;
                    logger.Info(string.Format("Change ProjectName end | ProjectName: {0} | UserName: {1}", lProjectName, Username));
                    scope.Complete();
                    return lresult;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for ChangeProjectName method | Project Id : {0} | Project Name : {1} | Preject Desc : {2} | UserName: {3}", lProjectId, lProjectName, lProjectdesc, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for ChangeProjectName method | Project Id : {0} | Project Name : {1} | Preject Desc : {2} | UserName: {3}", lProjectId, lProjectName, lProjectdesc, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for ChangeProjectName method | Project Id : {0} | Project Name : {1} | Preject Desc : {2} | UserName: {3}", lProjectId, lProjectName, lProjectdesc, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckDuplicateProjectName(string lProjectName, long? lProjectId)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Project Exist start | ProjectName: {0} | UserName: {1}", lProjectName, Username));
                var lresult = false;
                if (lProjectId != null)
                {
                    lresult = enty.T_TEST_PROJECT.Any(x => x.PROJECT_ID != lProjectId && x.PROJECT_NAME.ToLower().Trim() == lProjectName.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_TEST_PROJECT.Any(x => x.PROJECT_NAME.ToLower().Trim() == lProjectName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Project Exist start | ProjectName: {0} | UserName: {1}", lProjectName, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for CheckDuplicateProjectName method | Project Id : {0} | Project Name : {1} | UserName: {2}", lProjectId, lProjectName, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for CheckDuplicateProjectName method | Project Id : {0} | Project Name : {1} | UserName: {2}", lProjectId, lProjectName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for CheckDuplicateProjectName method |Project Id : {0} | Project Name : {1} | UserName: {2}", lProjectId, lProjectName, Username), ex.InnerException);
                throw;
            }
        }
        public List<RelProjectApplication> ListRelProjectApp(string ApplicationId)
        {
            try
            {
                logger.Info(string.Format("ListRelProjectApp start | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                var lResult = new List<RelProjectApplication>();

                var lApplicationIds = ApplicationId.Split(',').Select(Int64.Parse).ToList();
                var lList = from x in enty.T_TEST_PROJECT
                            join y in enty.REL_APP_PROJ on x.PROJECT_ID equals y.PROJECT_ID
                            join z in enty.T_REGISTERED_APPS on y.APPLICATION_ID equals z.APPLICATION_ID
                            where lApplicationIds.Contains(z.APPLICATION_ID)
                            select new RelProjectApplication
                            {
                                ProjectId = x.PROJECT_ID,
                                ProjectName = x.PROJECT_NAME,
                            };

                if (lList.Count() > 0)
                    lResult = lList.Distinct().OrderBy(x => x.ProjectName).ToList();

                logger.Info(string.Format("ListRelProjectApp end | ApplicationId: {0} | Username: {1}", ApplicationId, Username));
                return lResult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for ListRelProjectApp method | Application Id : {0} | UserName: {1}", ApplicationId, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for ListRelProjectApp method | Application Id : {0} | UserName: {1}", ApplicationId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for ListRelProjectApp method | Application Id : {0} | UserName: {1}", ApplicationId, Username), ex.InnerException);
                throw;
            }
        }
        public List<StoryBoardResultExportModel> ExportProject(string projectname, string lstrConn, string schema)
        {
            try
            {
                logger.Info(string.Format("Export All Stoaryborad start | projectname: {0} | Username: {1}", projectname, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[2];
                ladd_refer_image[0] = new OracleParameter("PROJECT", OracleDbType.Varchar2);
                ladd_refer_image[0].Value = projectname;

                ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[1].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_EXPORT_STORYBOARD";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                lconnection.Close();
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<StoryBoardResultExportModel> resultList = dt.AsEnumerable().Select(row =>
                new StoryBoardResultExportModel
                {
                    APPLICATIONNAME = Convert.ToString(row.Field<string>("APPLICATIONNAME")),
                    RUNORDER = row.Field<long?>("RUNORDER") == null ? "" : Convert.ToString(row.Field<long>("RUNORDER")),
                    PROJECTNAME = row.Field<string>("PROJECTNAME") == null ? "" : Convert.ToString(row.Field<string>("PROJECTNAME")),
                    PROJECTDESCRIPTION = row.Field<string>("PROJECTDESCRIPTION") == null ? "" : Convert.ToString(row.Field<string>("PROJECTDESCRIPTION")),
                    STORYBOARD_NAME = row.Field<string>("STORYBOARD_NAME") == null ? "" :Convert.ToString(row.Field<string>("STORYBOARD_NAME")),
                    ACTIONNAME = row.Field<string>("ACTIONNAME") == null ? "" :Convert.ToString(row.Field<string>("ACTIONNAME")),
                    STEPNAME = row.Field<string>("STEPNAME") == null ? "" : Convert.ToString(row.Field<string>("STEPNAME")),
                    SUITENAME = row.Field<string>("SUITENAME") == null ? "" : Convert.ToString(row.Field<string>("SUITENAME")),
                    CASENAME = row.Field<string>("CASENAME") == null ? "" : Convert.ToString(row.Field<string>("CASENAME")),
                    DATASETNAME = row.Field<string>("DATASETNAME") == null ? "" : Convert.ToString(row.Field<string>("DATASETNAME")),
                    DEPENDENCY = row.Field<string>("DEPENDENCY") == null ? "" :Convert.ToString(row.Field<string>("DEPENDENCY")),
                }).ToList();

                logger.Info(string.Format("Export All Stoaryborad end | projectname: {0} | Username: {1}", projectname, Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for ExportProject method | Project Name : {0} | Connection String : {1} | Schema : {2} | UserName: {2}", projectname, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for ExportProject method | Project Name : {0} | Connection String : {1} | Schema : {2} | UserName: {2}", projectname, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for ExportProject method | Project Name : {0} | Connection String : {1} | Schema : {2} | UserName: {2}", projectname, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }
        public String GetProjectNamebyId(long projectid)
        {
            try
            {
                logger.Info(string.Format("Get ProjectName Id start | projectid: {0} | Username: {1}", projectid, Username));
                var result = enty.T_TEST_PROJECT.FirstOrDefault(x => x.PROJECT_ID == projectid).PROJECT_NAME;
                logger.Info(string.Format("Get ProjectName Id end | projectid: {0} | Username: {1}", projectid, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for GetProjectNamebyId method | Project Id : {0} | UserName: {1}", projectid, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for GetProjectNamebyId method | Project Id : {0} | UserName: {1}", projectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for GetProjectNamebyId method | Project Id : {0} | UserName: {1}", projectid, Username), ex.InnerException);
                throw;
            }
        }
        public bool DeleteProject(long projectid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete Project start | projectid: {0} | Username: {1}", projectid, Username));
                    var flag = false;
                    var result = enty.T_TEST_PROJECT.FirstOrDefault(x => x.PROJECT_ID == projectid);
                    if (result != null)
                    {
                        var relappprj = enty.REL_APP_PROJ.Where(x => x.PROJECT_ID == projectid).ToList();
                        foreach (var item in relappprj)
                        {
                            enty.REL_APP_PROJ.Remove(item);
                            enty.SaveChanges();
                        }
                        var relsuiteprj = enty.REL_TEST_SUIT_PROJECT.Where(x => x.PROJECT_ID == projectid).ToList();
                        foreach (var item in relsuiteprj)
                        {
                            enty.REL_TEST_SUIT_PROJECT.Remove(item);
                            enty.SaveChanges();
                        }
                        var dp = (from p in enty.T_STORYBOARD_DATASET_SETTING
                                  from s in enty.T_PROJ_TC_MGR
                                  where p.STORYBOARD_DETAIL_ID == s.STORYBOARD_DETAIL_ID
                                  && s.PROJECT_ID == projectid
                                  select p).ToList();
                        foreach (var itm in dp)
                        {
                            enty.T_STORYBOARD_DATASET_SETTING.Remove(itm);
                            enty.SaveChanges();
                        }
                        var q = (from p in enty.T_PROJ_TC_MGR
                                 where p.PROJECT_ID == projectid
                                 select p).ToList();
                        foreach (var itm in q)
                        {
                            enty.T_PROJ_TC_MGR.Remove(itm);
                            enty.SaveChanges();
                        }

                        var dpr = (from p in enty.T_PROJECT_DATA_SOURCE
                                   where p.PROJECT_ID == projectid
                                   select p).ToList();
                        foreach (var itm in dpr)
                        {
                            enty.T_PROJECT_DATA_SOURCE.Remove(itm);
                            enty.SaveChanges();
                        }
                        flag = true;
                        enty.T_TEST_PROJECT.Remove(result);
                        enty.SaveChanges();
                    }
                    logger.Info(string.Format("Delete Project end | projectid: {0} | Username: {1}", projectid, Username));
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for DeleteProject method | Project Id : {0} | UserName: {1}", projectid, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for DeleteProject method | Project Id : {0} | UserName: {1}", projectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for DeleteProject method | Project Id : {0} | UserName: {1}", projectid, Username), ex.InnerException);
                throw;
            }
        }
        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public List<ProjectViewModel> ListAllProject(string schema, string lconstring, int startrec, int pagesize, string colname, string colorder, string namesearch, string descsearch, string appsearch, string statussearch)
        {
            try
            {
                logger.Info(string.Format("List Project start | UserName: {0}", Username));
                var result = new List<TestCaseModel>();

                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[9];
                ladd_refer_image[0] = new OracleParameter("Startrec", OracleDbType.Long);
                ladd_refer_image[0].Value = startrec;

                ladd_refer_image[1] = new OracleParameter("totalpagesize", OracleDbType.Long);
                ladd_refer_image[1].Value = pagesize;

                ladd_refer_image[2] = new OracleParameter("ColumnName", OracleDbType.Varchar2);
                ladd_refer_image[2].Value = colname;

                ladd_refer_image[3] = new OracleParameter("Columnorder", OracleDbType.Varchar2);
                ladd_refer_image[3].Value = colorder;

                ladd_refer_image[4] = new OracleParameter("NameSearch", OracleDbType.Varchar2);
                ladd_refer_image[4].Value = namesearch;

                ladd_refer_image[5] = new OracleParameter("DescriptionSearch", OracleDbType.Varchar2);
                ladd_refer_image[5].Value = descsearch;

                ladd_refer_image[6] = new OracleParameter("AppSearch", OracleDbType.Varchar2);
                ladd_refer_image[6].Value = appsearch;

                ladd_refer_image[7] = new OracleParameter("StatusSearch", OracleDbType.Varchar2);
                ladd_refer_image[7].Value = statussearch;

                ladd_refer_image[8] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[8].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_LIST_PROJECTS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];


                List<ProjectViewModel> resultList = dt.AsEnumerable().Select(row =>
                          new ProjectViewModel
                          {
                              ProjectId = row.Field<long>("projectid"),
                              ProjectName = Convert.ToString(row.Field<string>("projectname")),
                              ProjectDescription = Convert.ToString(row.Field<string>("description")),
                              Status = Convert.ToString(row.Field<string>("projectstatus")),
                              StatusId = Convert.ToInt16(row.Field<short?>("statusid")),
                              ApplicationId = Convert.ToString(row.Field<string>("applicationid")),
                              Application = Convert.ToString(row.Field<string>("Applicationame")),
                              TotalCount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                          }).ToList();
                logger.Info(string.Format("List Project end | UserName: {0}", Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for ListAllProject method | Connection String : {0} | Schema : {1} | UserName: {2}", lconstring, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for ListAllProject method | Connection String : {0} | Schema : {1} | UserName: {2}", lconstring, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for ListAllProject method | Connection String : {0} | Schema : {1} | UserName: {2}", lconstring, schema, Username), ex.InnerException);
                throw;
            }
        }

        public bool AddEditProject(ProjectViewModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.ProjectName))
                    {
                        lEntity.ProjectName = lEntity.ProjectName.Trim();
                    }
                    var flag = false;
                    if (lEntity.ProjectId == 0)
                    {
                        logger.Info(string.Format("Add Project start | Project: {0} | Username: {1}", lEntity.ProjectName, Username));
                        var tbl = new T_TEST_PROJECT();
                        tbl.PROJECT_ID = Helper.NextTestSuiteId("T_TEST_PROJECT_SEQ");
                        tbl.PROJECT_NAME = lEntity.ProjectName;
                        tbl.PROJECT_DESCRIPTION = lEntity.ProjectDescription;
                        tbl.STATUS = Convert.ToInt16(lEntity.Status);
                        tbl.CREATOR = lEntity.CarectorName;
                        tbl.CREATE_DATE = DateTime.Now;
                        lEntity.ProjectId = tbl.PROJECT_ID;
                        enty.T_TEST_PROJECT.Add(tbl);
                        enty.SaveChanges();
                        flag = true;
                        logger.Info(string.Format("Add Project end | Project: {0} | Username: {1}", lEntity.ProjectName, Username));
                    }
                    else
                    {
                        logger.Info(string.Format("Edit Project start | Project: {0} | ProjectId: {1} | Username: {2}", lEntity.ProjectName, lEntity.ProjectId, Username));
                        var tbl = enty.T_TEST_PROJECT.Find(lEntity.ProjectId);
                        #region Application Mapping Delete
                        var lAppList = enty.REL_APP_PROJ.Where(x => x.PROJECT_ID == lEntity.ProjectId).ToList();
                        foreach (var item in lAppList)
                        {
                            enty.REL_APP_PROJ.Remove(item);
                        }
                        enty.SaveChanges();
                        logger.Info(string.Format("Edit Project end | Project: {0} | ProjectId: {1} | Username: {2}", lEntity.ProjectName, lEntity.ProjectId, Username));
                        #endregion

                        if (tbl != null)
                        {
                            tbl.PROJECT_NAME = lEntity.ProjectName;
                            tbl.PROJECT_DESCRIPTION = lEntity.ProjectDescription;
                            tbl.STATUS = Convert.ToInt16(lEntity.Status);
                            enty.SaveChanges();
                        }
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(lEntity.ApplicationId))
                    {
                        var lAppSplit = lEntity.ApplicationId.Split(',').Select(Int64.Parse).ToList();

                        if (lAppSplit.Count() > 0)
                        {
                            lAppSplit = lAppSplit.Distinct().ToList();
                            foreach (var item in lAppSplit)
                            {
                                var lApptbl = new REL_APP_PROJ();
                                lApptbl.APPLICATION_ID = item;
                                lApptbl.PROJECT_ID = lEntity.ProjectId;
                                lApptbl.RELATIONSHIP_ID = Helper.NextTestSuiteId("REL_APP_PROJ_SEQ");
                                enty.REL_APP_PROJ.Add(lApptbl);
                                enty.SaveChanges();
                            }
                        }

                        flag = true;
                    }
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Project for AddEditProject method | Project Id : {0} | UserName: {1}", lEntity.ProjectId, Username));
                ELogger.ErrorException(string.Format("Error occured in Project for AddEditProject method | Project Id : {0} | UserName: {1}", lEntity.ProjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Project for AddEditProject method | Project Id : {0} | UserName: {1}", lEntity.ProjectId, Username), ex.InnerException);
                throw;
            }
        }
    }
}

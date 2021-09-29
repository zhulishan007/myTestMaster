using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using Oracle.ManagedDataAccess.Client;

namespace MARS_Repository.Repositories
{
    public class ObjectRepository
    {
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        public string Username = string.Empty;

        public List<T_OBJECT_NAMEINFO> GetObjects(int testcaseId)
        {
            try
            {
                logger.Info(string.Format("Get Objects start | TestcaseId: {0} | UserName: {1}", testcaseId, Username));
                var query = (from k in entity.T_OBJECT_NAMEINFO
                             join tr in entity.T_REGISTED_OBJECT on k.OBJECT_NAME_ID equals tr.OBJECT_NAME_ID
                             join tc in entity.T_TEST_CASE_SUMMARY on testcaseId equals tc.TEST_CASE_ID
                             join aptc in entity.REL_APP_TESTCASE on tc.TEST_CASE_ID equals aptc.TEST_CASE_ID
                             where tr.APPLICATION_ID == aptc.APPLICATION_ID
                             orderby k.OBJECT_HAPPY_NAME
                             select k).ToList<T_OBJECT_NAMEINFO>();

                logger.Info(string.Format("Get Objects end | TestcaseId: {0} | UserName: {1}", testcaseId, Username));
                return query;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjects method | TestCase Id : {0} | UserName: {1}", testcaseId, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjects method | TestCase Id : {0} | UserName: {1}", testcaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjects method | TestCase Id : {0} | UserName: {1}", testcaseId, Username), ex.InnerException);
                throw;
            }
        }

        public List<ObjectList> GetObjectsByPegWindowType(int testcaseId)
        {
            try
            {
                logger.Info(string.Format("Get Objects By PegWindowType start | TestcaseId: {0} | UserName: {1}", testcaseId, Username));
                var lPegObjectTypeId = entity.T_GUI_COMPONENT_TYPE_DIC.FirstOrDefault(x => x.TYPE_NAME.ToLower() == "pegwindow").TYPE_ID;

                var query = (from k in entity.T_REGISTED_OBJECT
                             join obj_n in entity.T_OBJECT_NAMEINFO on k.OBJECT_NAME_ID equals obj_n.OBJECT_NAME_ID
                             join ot in entity.T_GUI_COMPONENT_TYPE_DIC on k.TYPE_ID equals ot.TYPE_ID
                             join peg in entity.T_OBJECT_NAMEINFO on k.OBJECT_NAME_ID equals peg.OBJECT_NAME_ID
                             join tc in entity.T_TEST_CASE_SUMMARY on testcaseId equals tc.TEST_CASE_ID
                             join aptc in entity.REL_APP_TESTCASE on tc.TEST_CASE_ID equals aptc.TEST_CASE_ID
                             where k.APPLICATION_ID == aptc.APPLICATION_ID && k.TYPE_ID == lPegObjectTypeId
                             && k.OBJECT_TYPE == peg.OBJECT_HAPPY_NAME
                             orderby k.OBJECT_HAPPY_NAME
                             select new ObjectList
                             { ObjectId = (decimal)k.OBJECT_NAME_ID, ObjectName = k.OBJECT_HAPPY_NAME }).Distinct().ToList();

                logger.Info(string.Format("Get Objects By PegWindowType end | TestcaseId: {0} | UserName: {1}", testcaseId, Username));
                return query;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjectsByPegWindowType method | TestCase Id : {0} | UserName: {1}", testcaseId, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjectsByPegWindowType method | TestCase Id : {0} | UserName: {1}", testcaseId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjectsByPegWindowType method | TestCase Id : {0} | UserName: {1}", testcaseId, Username), ex.InnerException);
                throw;
            }
        }

        public T_OBJECT_NAMEINFO GetObjectByObjectName(string lObjectName)
        {
            try
            {
                logger.Info(string.Format("Get Objects start | ObjectName: {0} | UserName: {1}", lObjectName, Username));
                var query = entity.T_OBJECT_NAMEINFO.FirstOrDefault(y => y.OBJECT_HAPPY_NAME == lObjectName);
                logger.Info(string.Format("Get Objects end | ObjectName: {0} | UserName: {1}", lObjectName, Username));
                return query;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjectByObjectName method | Object Name : {0} | UserName: {1}", lObjectName, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjectByObjectName method | Object Name : {0} | UserName: {1}", lObjectName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjectByObjectName method | Object Name : {0} | UserName: {1}", lObjectName, Username), ex.InnerException);
                throw;
            }
        }

        public T_OBJECT_NAMEINFO GetPegObjectByObjectName(string lObjectName)
        {
            try
            {
                logger.Info(string.Format("Get Peg Objects start | ObjectName: {0} | UserName: {1}", lObjectName, Username));
                var lPegObjectList = entity.T_OBJECT_NAMEINFO.Where(x => x.PEGWINDOW_MARK == 1).ToList();
                var query = new T_OBJECT_NAMEINFO();
                if (lPegObjectList.Count() > 0)
                {
                    query = lPegObjectList.FirstOrDefault(y => y.OBJECT_HAPPY_NAME.ToUpper() == lObjectName.ToUpper());
                    logger.Info(string.Format("Get Peg Objects end | ObjectName: {0} | UserName: {1}", lObjectName, Username));
                }
                return query;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetPegObjectByObjectName method | Object Name : {0} | UserName: {1}", lObjectName, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetPegObjectByObjectName method | Object Name : {0} | UserName: {1}", lObjectName, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetPegObjectByObjectName method | Object Name : {0} | UserName: {1}", lObjectName, Username), ex.InnerException);
                throw;
            }
        }

        public List<T_OBJECT_NAMEINFO> GetObjectByParent(int testcaseId, decimal lPegObjectId, long llinkedKeywordId)
        {
            try
            {
                logger.Info(string.Format("Get Object By Parent start | PegObjectId: {0} | TestcaseId: {1} | UserName: {2}", lPegObjectId, testcaseId, Username));
                var lPegObjectName = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == lPegObjectId && x.TYPE_ID == 1).FirstOrDefault().OBJECT_TYPE;

                var lAppId = entity.REL_APP_TESTCASE.Where(x => x.TEST_CASE_ID == testcaseId).FirstOrDefault().APPLICATION_ID;

                var linkedObject = (from k in entity.T_OBJECT_NAMEINFO
                                    join tr in entity.T_REGISTED_OBJECT on k.OBJECT_NAME_ID equals tr.OBJECT_NAME_ID
                                    join rdic in entity.T_DIC_RELATION_KEYWORD on tr.TYPE_ID equals rdic.TYPE_ID
                                    where tr.APPLICATION_ID == lAppId && tr.OBJECT_TYPE == lPegObjectName &&
                                    rdic.KEY_WORD_ID == llinkedKeywordId

                                    orderby k.OBJECT_HAPPY_NAME
                                    select k).Distinct().ToList<T_OBJECT_NAMEINFO>();

                logger.Info(string.Format("Get Object By Parent end | PegObjectId: {0} | TestcaseId: {1} | UserName: {2}", lPegObjectId, testcaseId, Username));
                return linkedObject;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjectByParent method | PegObjectId: {0} | TestcaseId: {1} | Keyword Id : {2} | UserName: {3}", lPegObjectId, testcaseId, llinkedKeywordId, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjectByParent method | PegObjectId: {0} | TestcaseId: {1} | Keyword Id : {2} | UserName: {3}", lPegObjectId, testcaseId, llinkedKeywordId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjectByParent method | PegObjectId: {0} | TestcaseId: {1} | Keyword Id : {2} | UserName: {3}", lPegObjectId, testcaseId, llinkedKeywordId, Username), ex.InnerException);
                throw;
            }
        }
        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public List<Objects> ListObjects(string schema, string lconstring, int startrec, int pagesize, string NameSearch, string type, string quickaccess, string parent, string colname, string colorder, string appid)
        {
            try
            {
                logger.Info(string.Format("Get object list start | ApplicationId: {0} | Username: {1}", appid, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[10];
                ladd_refer_image[0] = new OracleParameter("Startrec", OracleDbType.Long);
                ladd_refer_image[0].Value = startrec;

                ladd_refer_image[1] = new OracleParameter("totalpagesize", OracleDbType.Long);
                ladd_refer_image[1].Value = pagesize;

                ladd_refer_image[2] = new OracleParameter("ColumnName", OracleDbType.Varchar2);
                ladd_refer_image[2].Value = colname;

                ladd_refer_image[3] = new OracleParameter("Columnorder", OracleDbType.Varchar2);
                ladd_refer_image[3].Value = colorder;

                ladd_refer_image[4] = new OracleParameter("objectname", OracleDbType.Varchar2);
                ladd_refer_image[4].Value = NameSearch;

                ladd_refer_image[5] = new OracleParameter("typename", OracleDbType.Varchar2);
                ladd_refer_image[5].Value = type;

                ladd_refer_image[6] = new OracleParameter("quickaccess", OracleDbType.Varchar2);
                ladd_refer_image[6].Value = quickaccess;

                ladd_refer_image[7] = new OracleParameter("parent", OracleDbType.Varchar2);
                ladd_refer_image[7].Value = parent;
                ladd_refer_image[8] = new OracleParameter("APPLICATION", OracleDbType.Varchar2);
                ladd_refer_image[8].Value = appid;

                ladd_refer_image[9] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[9].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "Sp_List_Objects";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<Objects> resultList = dt.AsEnumerable().Select(row =>
                    new Objects
                    {
                        ObjectId = row.Field<decimal>("objectid"),
                        ObjectName = Convert.ToString(row.Field<string>("OBJECTNAME")),
                        ObjectType = Convert.ToString(row.Field<string>("TYPE")),
                        Quickaccess = Convert.ToString(row.Field<string>("QuickAccess")),
                        ObjectParent = Convert.ToString(row.Field<string>("PARENT")),
                        EnumType = Convert.ToString(row.Field<string>("ENUM_TYPE")),
                        applicationid = Convert.ToInt64(row.Field<Int64>("applicationid")),
                        description = Convert.ToString(row.Field<string>("description")),
                        autocheckerror = Convert.ToInt16(row.Field<short?>("checkerror")),
                        Totalcount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                    }).ToList();

                logger.Info(string.Format("Get object list end | ApplicationId: {0} | Username: {1}", appid, Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for ListObjects method | Connection String: {0} | Schema: {1} | UserName: {2}", lconstring, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for ListObjects method | Connection String: {0} | Schema: {1} | UserName: {2}", lconstring, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ListObjects method | Connection String: {0} | Schema: {1} | UserName: {2}", lconstring, schema, Username), ex.InnerException);
                throw;
            }
        }

        public List<ObjectExportModel> ExportObject(string pApplication, string lstrconn, string schema)
        {
            try
            {
                logger.Info(string.Format("Object export start | ApplicationId: {0} | Username: {1}", pApplication, Username));
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(lstrconn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[2];
                ladd_refer_image[0] = new OracleParameter("APPLICATION", OracleDbType.Varchar2);
                ladd_refer_image[0].Value = pApplication;
                ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[1].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_EXPORT_EXPORTOBJECT";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<ObjectExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new ObjectExportModel
                    {
                        OBJECTNAME = row.Field<string>("OBJECT NAME"),
                        TYPE = row.Field<string>("TYPE"),
                        QUICK_ACCESS = row.Field<string>("QUICK_ACCESS"),
                        PARENT = row.Field<string>("PARENT"),
                        COMMENT = row.Field<string>("COMMENT"),
                        ENUM_TYPE = row.Field<string>("ENUM_TYPE"),
                        SQL = row.Field<string>("SQL"),

                    }).ToList();

                logger.Info(string.Format("Object export end | ApplicationId: {0} | Username: {1}", pApplication, Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for ExportObject method | Application Id : {0} | Connection String: {1} | Schema: {2} | UserName: {3}", pApplication, lstrconn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for ExportObject method | Application Id : {0} | Connection String: {1} | Schema: {2} | UserName: {3}", pApplication, lstrconn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for ExportObject method | Application Id : {0} | Connection String: {1} | Schema: {2} | UserName: {3}", pApplication, lstrconn, schema, Username), ex.InnerException);
                throw;
            }
        }

        public List<string> GetPegwindowObject(long appid)
        {
            try
            {
                logger.Info(string.Format("Get Pegwindow object list start | ApplicationId: {0} | Username: {1}", appid, Username));
                var peg = entity.T_REGISTED_OBJECT.Where(x => x.APPLICATION_ID == appid).OrderBy(x => x.OBJECT_TYPE).Select(x => x.OBJECT_TYPE).ToList();
                peg = peg.Distinct().ToList();

                logger.Info(string.Format("Get Pegwindow object list end | ApplicationId: {0} | Username: {1}", appid, Username));
                return peg;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetPegwindowObject method | Application Id : {0} | UserName: {1}", appid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetPegwindowObject method | Application Id : {0} | UserName: {1}", appid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetPegwindowObject method | Application Id : {0} | UserName: {1}", appid, Username), ex.InnerException);
                throw;
            }
        }
        public List<objectType> LoadObjectType()
        {
            try
            {
                logger.Info(string.Format("Load Object Type start | Username: {0}", Username));
                var result = (from t in entity.T_GUI_COMPONENT_TYPE_DIC
                              select new objectType
                              {
                                  typeid = t.TYPE_ID,
                                  typename = t.TYPE_NAME
                              }).OrderBy(x => x.typename).ToList();

                logger.Info(string.Format("Load Object Type end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for LoadObjectType method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured in Object for LoadObjectType method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for LoadObjectType method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        public string AddEditObject(ObjectModel model)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (model.ObjectId == 0)
                    {
                        logger.Info(string.Format("Add object start | ApplicationId: {0} | Object: {1} | Username: {2}", model.applicationid, model.ObjectName, Username));

                        var objectexist = entity.T_OBJECT_NAMEINFO.Where(x => x.OBJECT_HAPPY_NAME.Trim() == model.ObjectName.Trim()).FirstOrDefault();
                        var objnameinfo = new T_OBJECT_NAMEINFO();
                        if (objectexist != null)
                        {
                            objnameinfo.OBJECT_NAME_ID = objectexist.OBJECT_NAME_ID;
                            if (objectexist.PEGWINDOW_MARK == null)
                            {
                                if (model.ObjectName.ToLower() == model.ObjectParent.ToLower())
                                {
                                    objectexist.PEGWINDOW_MARK = 1;
                                    entity.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            if (model.description == null)
                            {
                                model.description = "";
                            }

                            objnameinfo.OBJECT_NAME_ID = Helper.NextTestSuiteId("SEQ_MARS_OBJECT_ID");
                            objnameinfo.OBJECT_HAPPY_NAME = model.ObjectName;
                            objnameinfo.OBJNAME_DESCRIPTION = model.description;
                            if (model.ObjectName.ToLower() == model.ObjectParent.ToLower())
                                objnameinfo.PEGWINDOW_MARK = 1;
                            else
                                objnameinfo.PEGWINDOW_MARK = null;
                            entity.T_OBJECT_NAMEINFO.Add(objnameinfo);
                            entity.SaveChanges();
                        }


                        if (model.EnumType == null)

                            model.EnumType = "";

                        if (model.Quickaccess == null)
                        {
                            model.Quickaccess = "";
                        }
                        var registerdobject = new T_REGISTED_OBJECT();
                        registerdobject.OBJECT_ID = Helper.NextTestSuiteId("SEQ_MARS_OBJECT_ID");
                        registerdobject.OBJECT_HAPPY_NAME = model.ObjectName;
                        registerdobject.APPLICATION_ID = model.applicationid;
                        registerdobject.TYPE_ID = model.ObjectType;
                        registerdobject.QUICK_ACCESS = model.Quickaccess;
                        registerdobject.OBJECT_TYPE = model.ObjectParent;
                        registerdobject.ENUM_TYPE = model.EnumType;
                        registerdobject.OBJECT_NAME_ID = objnameinfo.OBJECT_NAME_ID;
                        registerdobject.IS_CHECKERROR_OBJ = model.autocheckerror;
                        entity.T_REGISTED_OBJECT.Add(registerdobject);
                        entity.SaveChanges();
                        logger.Info(string.Format("Add object end | ApplicationId: {0} | Object: {1} | Username: {2}", model.applicationid, model.ObjectName, Username));
                    }
                    else
                    {
                        logger.Info(string.Format("Edit object start | ApplicationId: {0} | Object: {1} | ObjectId: {2} | Username: {3}", model.applicationid, model.ObjectName, model.ObjectId, Username));
                        var result = entity.T_OBJECT_NAMEINFO.Find(model.ObjectId);
                        if (result != null)
                        {

                            result.OBJECT_HAPPY_NAME = model.ObjectName;
                            result.OBJNAME_DESCRIPTION = model.description;
                            if (model.ObjectName.ToLower() == model.ObjectParent.ToLower())
                                result.PEGWINDOW_MARK = 1;
                            else
                                result.PEGWINDOW_MARK = null;

                            entity.SaveChanges();

                            var registeredobject = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == model.ObjectId && x.APPLICATION_ID == model.applicationid && x.OBJECT_TYPE.ToLower().Trim() == model.ObjectParent.ToLower().Trim()).ToList();
                            foreach (var itm in registeredobject)
                            {

                                itm.OBJECT_HAPPY_NAME = model.ObjectName;
                                itm.APPLICATION_ID = model.applicationid;
                                itm.TYPE_ID = model.ObjectType;
                                itm.QUICK_ACCESS = model.Quickaccess;
                                itm.OBJECT_TYPE = model.ObjectParent;
                                itm.ENUM_TYPE = model.EnumType;
                                itm.OBJECT_NAME_ID = model.ObjectId;
                                itm.IS_CHECKERROR_OBJ = model.autocheckerror;
                                entity.SaveChanges();
                            }
                        }
                        logger.Info(string.Format("Edit object start | ApplicationId: {0} | Object: {1} | ObjectId: {2} | Username: {3}", model.applicationid, model.ObjectName, model.ObjectId, Username));
                    }
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for AddEditObject method | Application Id : {0} | Object: {1} | ObjectId: {2} | Username: {3}", model.applicationid, model.ObjectName, model.ObjectId, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for AddEditObject method | Application Id : {0} | Object: {1} | ObjectId: {2} | Username: {3}", model.applicationid, model.ObjectName, model.ObjectId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for AddEditObject method | Application Id : {0} | Object: {1} | ObjectId: {2} | Username: {3}", model.applicationid, model.ObjectName, model.ObjectId, Username), ex.InnerException);
                throw;
            }
        }
        public List<long> FindObjectId(long objecid, long appid)
        {
            try
            {
                logger.Info(string.Format("Check Pegwindow Object start | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objecid, Username));
                List<long> objectids = new List<long>();
                var regobj = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == objecid && x.APPLICATION_ID == appid).ToList();
                if (regobj.Count > 0)
                {
                    foreach (var item in regobj)
                    {
                        objectids.Add(item.OBJECT_ID);

                    }
                    objectids = objectids.Distinct().ToList();
                }
                logger.Info(string.Format("Check Pegwindow Object end | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objecid, Username));
                return objectids;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for FindObjectId method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objecid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for FindObjectId method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objecid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for FindObjectId method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objecid, Username), ex.InnerException);
                throw;
            }
        }
        public string CheckPegwindowObject(long objectid, long appid)
        {
            try
            {
                logger.Info(string.Format("Check Pegwindow Object start | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username));
                var result = entity.T_OBJECT_NAMEINFO.Find(objectid);
                if (result != null)
                {
                    var regobj = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == objectid).ToList();
                    foreach (var itm in regobj)
                    {
                        if (result.OBJECT_HAPPY_NAME.ToLower() == itm.OBJECT_TYPE.ToLower())
                        {
                            logger.Info(string.Format("Check Pegwindow Object end | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username));
                            return "Pegwindow object";
                        }
                    }
                }
                logger.Info(string.Format("Check Pegwindow Object end | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckPegwindowObject method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckPegwindowObject method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckPegwindowObject method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username), ex.InnerException);
                throw;
            }
        }
        public string getPegwindowObjectName(long objectid, long appid)
        {
            try
            {
                logger.Info(string.Format("Get Pegwindow ObjectName start | objectid: {0} | applicationId: {1} | Username: {2}", objectid, appid, Username));
                var result = entity.T_OBJECT_NAMEINFO.Find(objectid).OBJECT_HAPPY_NAME;
                logger.Info(string.Format("Get Pegwindow ObjectName end | objectid: {0} | applicationId: {1} | Username: {2}", objectid, appid, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for getPegwindowObjectName method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for getPegwindowObjectName method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for getPegwindowObjectName method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, objectid, Username), ex.InnerException);
                throw;
            }
        }
        public string DeleteObject(long id, long appid, string parent)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("Delete Object start | objectid: {0} | applicationId: {1} | Username: {2}", id, appid, Username));
                    var result = entity.T_OBJECT_NAMEINFO.Find(id);
                    if (result != null)
                    {
                        var regobj = entity.T_REGISTED_OBJECT.Where(x => x.OBJECT_NAME_ID == id && x.APPLICATION_ID == appid && x.OBJECT_TYPE.ToLower().Trim() == parent.ToLower().Trim()).ToList();
                        foreach (var itm in regobj)
                        {
                            var app = entity.REL_OBJ_APP.Where(x => x.OBJECT_ID == itm.OBJECT_ID).ToList();
                            foreach (var i in app)
                            {
                                entity.REL_OBJ_APP.Remove(i);
                                entity.SaveChanges();
                            }
                            entity.T_REGISTED_OBJECT.Remove(itm);
                            entity.SaveChanges();
                        }
                    }
                    //entity.T_OBJECT_NAMEINFO.Remove(result);
                    //entity.SaveChanges();
                    logger.Info(string.Format("Delete Object end | objectid: {0} | applicationId: {1} | Username: {2}", id, appid, Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for DeleteObject method | ApplicationId: {0} | ObjectId: {1} | Parent : {2} | Username: {3}", appid, id, parent, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for DeleteObject method | ApplicationId: {0} | ObjectId: {1} | Parent : {2} | Username: {3}", appid, id, parent, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for DeleteObject method | ApplicationId: {0} | ObjectId: {1} | Parent : {2} | Username: {3}", appid, id, parent, Username), ex.InnerException);
                throw;
            }
        }

        public string GetObjectName(long id, long appid)
        {
            try
            {
                logger.Info(string.Format("Get ObjectName start | objectid: {0} | applicationId: {1} | Username: {2}", id, appid, Username));
                var result = entity.T_OBJECT_NAMEINFO.Find(id).OBJECT_HAPPY_NAME;
                logger.Info(string.Format("Get ObjectName end | objectid: {0} | Username: {1}", id, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjectName method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, id, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjectName method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjectName method | ApplicationId: {0} | ObjectId: {1} | Username: {2}", appid, id, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> CheckObjectExistsInTestCase(long objectid)
        {
            try
            {
                logger.Info(string.Format("Check Object Exists In TestCase start | objectid: {0} | Username: {1}", objectid, Username));
                List<string> testcase = new List<string>();
                var steps = entity.T_TEST_STEPS.Where(x => x.OBJECT_ID == objectid).Select(x => x.TEST_CASE_ID);
                if (steps != null)
                {
                    foreach (var itm in steps)
                    {
                        if (itm != null)
                        {
                            var testcasename = entity.T_TEST_CASE_SUMMARY.Find(itm).TEST_CASE_NAME;
                            testcase.Add(testcasename);
                        }
                    }
                    testcase = testcase.Distinct().ToList();
                }
                logger.Info(string.Format("Check Object Exists In TestCase end | objectid: {0} | Username: {1}", objectid, Username));
                return testcase;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckObjectExistsInTestCase method | ObjectId: {0} | Username: {2}", objectid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckObjectExistsInTestCase method | ObjectId: {0} | Username: {2}", objectid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckObjectExistsInTestCase method | ObjectId: {0} | Username: {2}", objectid, Username), ex.InnerException);
                throw;
            }
        }
        public string CopyAllObjects(long copyfromappid, long copytoappid, string schema, string lstrConn)
        {
            try
            {
                logger.Info(string.Format("Copy All Objects start | Old ApplicationId: {0} | New ApplicationId: {1} | Username: {2}", copyfromappid, copytoappid, Username));

                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("OLDAPPID", OracleDbType.Long);
                ladd_refer_image[0].Value = copyfromappid;

                ladd_refer_image[1] = new OracleParameter("NEWAPPID", OracleDbType.Long);
                ladd_refer_image[1].Value = copytoappid;
                ladd_refer_image[2] = new OracleParameter("RESULT", OracleDbType.Clob);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }
                lcmd.CommandText = schema + "." + "SP_COPYOBJECTS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                logger.Info(string.Format("Copy All Objects end | Old ApplicationId: {0} | New ApplicationId: {1} | Username: {2}", copyfromappid, copytoappid, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CopyAllObjects method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", copyfromappid, copytoappid, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CopyAllObjects method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", copyfromappid, copytoappid, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CopyAllObjects method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", copyfromappid, copytoappid, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }
        public string CopyObjects(List<long> model, long fromid, long toid, string lstrConn, string schema)
        {
            try
            {
                logger.Info(string.Format("Copy Objects start | Old ApplicationId: {0} | New ApplicationId: {1} | Username: {2}", fromid, toid, Username));
                var objectidarray = String.Join(",", model);
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[4];
                ladd_refer_image[0] = new OracleParameter("Objectid", OracleDbType.Varchar2);
                ladd_refer_image[0].Value = objectidarray;

                ladd_refer_image[1] = new OracleParameter("OLDAPPID", OracleDbType.Long);
                ladd_refer_image[1].Value = fromid;

                ladd_refer_image[2] = new OracleParameter("NEWAPPID", OracleDbType.Long);
                ladd_refer_image[2].Value = toid;
                ladd_refer_image[3] = new OracleParameter("RESULT", OracleDbType.Clob);
                ladd_refer_image[3].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_COPY_SELECTED_OBJECTS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                logger.Info(string.Format("Copy Objects end | Old ApplicationId: {0} | New ApplicationId: {1} | Username: {2}", fromid, toid, Username));
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CopyObjects method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", fromid, toid, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CopyObjects method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", fromid, toid, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CopyObjects method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", fromid, toid, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }

        public bool CheckObjectExists(string objectname, long appid, string objecttype, long? typeid)
        {
            try
            {
                logger.Info(string.Format("Check Object Exists start | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                var flag = false;
                var objects = (from t in entity.T_OBJECT_NAMEINFO
                               join t1 in entity.T_REGISTED_OBJECT on t.OBJECT_NAME_ID equals t1.OBJECT_NAME_ID
                               where t.OBJECT_HAPPY_NAME == objectname && t1.APPLICATION_ID == appid && t1.OBJECT_TYPE == objecttype && t1.TYPE_ID == typeid
                               select new { t.OBJECT_NAME_ID, t.OBJECT_HAPPY_NAME, t1.APPLICATION_ID }).ToList();
                if (objects.Count > 0)
                {
                    flag = true;
                    logger.Info(string.Format("Check Object Exists end | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                    return flag;
                }
                logger.Info(string.Format("Check Object Exists end | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckObjectExists method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | TypeId : {3} | Username: {4}", appid, objectname, objecttype, typeid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckObjectExists method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | TypeId : {3} | Username: {4}", appid, objectname, objecttype, typeid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckObjectExists method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | TypeId : {3} | Username: {4}", appid, objectname, objecttype, typeid, Username), ex.InnerException);
                throw;
            }
        }
        public string CheckConvertingObjectExists(string objectname, long appid, string parentobj, string objecttype)
        {
            try
            {
                logger.Info(string.Format("Check Converting Object Exists start | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                var typeid = entity.T_GUI_COMPONENT_TYPE_DIC.FirstOrDefault(x => x.TYPE_NAME.ToUpper() == objecttype.ToUpper()).TYPE_ID;
                var objxists = (from t in entity.T_OBJECT_NAMEINFO
                                join t1 in entity.T_REGISTED_OBJECT on t.OBJECT_NAME_ID equals t1.OBJECT_NAME_ID
                                where t.OBJECT_HAPPY_NAME == objectname && t1.APPLICATION_ID == appid && t1.OBJECT_TYPE.ToLower() == parentobj.ToLower() && t1.TYPE_ID == typeid
                                select new { t.OBJECT_HAPPY_NAME, t1.APPLICATION_ID }).ToList();
                logger.Info(string.Format("Check Converting Object Exists end | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                if (objxists.Count > 0)
                {
                    return "duplicateerror";
                }
                return "success";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckConvertingObjectExists method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | ParentType : {3} | Username: {4}", appid, objectname, objecttype, parentobj, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckConvertingObjectExists method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | ParentType : {3} | Username: {4}", appid, objectname, objecttype, parentobj, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckConvertingObjectExists method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | ParentType : {3} | Username: {4}", appid, objectname, objecttype, parentobj, Username), ex.InnerException);
                throw;
            }
        }
        public bool CheckObjectName(string objectname, long appid, string parent, long? objecttype)
        {
            try
            {
                logger.Info(string.Format("Check ObjectName start | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                var flag = false;
                var result = entity.T_OBJECT_NAMEINFO.Where(x => x.OBJECT_HAPPY_NAME == objectname).Select(x => x.OBJECT_NAME_ID);
                if (result != null)
                {
                    var lresult = (from t in entity.T_OBJECT_NAMEINFO
                                   join t1 in entity.T_REGISTED_OBJECT on t.OBJECT_NAME_ID equals t1.OBJECT_NAME_ID
                                   where t.OBJECT_HAPPY_NAME.ToLower() == objectname.ToLower() && t1.APPLICATION_ID == appid && t1.OBJECT_TYPE == parent && t1.TYPE_ID == objecttype
                                   select t.OBJECT_HAPPY_NAME).ToList();
                    if (lresult.Count > 0)
                    {
                        flag = true;
                        logger.Info(string.Format("Check ObjectName end | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                        return flag;
                    }

                }
                logger.Info(string.Format("Check ObjectName end | ApplicationId: {0} | objectname: {1} | Username: {2}", appid, objectname, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for CheckObjectName method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | ParentType : {3} | Username: {4}", appid, objectname, objecttype, parent, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for CheckObjectName method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | ParentType : {3} | Username: {4}", appid, objectname, objecttype, parent, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for CheckObjectName method | ApplicationId: {0} | ObjectName: {1} | ObjectType : {2} | ParentType : {3} | Username: {4}", appid, objectname, objecttype, parent, Username), ex.InnerException);
                throw;
            }
        }
        public List<string> DuplicateObjectList(long copyfromappid, long copytoappid, string lstrConn, string schema)
        {
            var objects = new List<string>();

            try
            {
                logger.Info(string.Format("Duplicate Object List start | Old ApplicationId: {0} | New ApplicationId: {1} | Username: {2}", copyfromappid, copytoappid, Username));
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lstrConn);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("OLDAPPID", OracleDbType.Long);
                ladd_refer_image[0].Value = copyfromappid;

                ladd_refer_image[1] = new OracleParameter("NEWAPPID", OracleDbType.Long);
                ladd_refer_image[1].Value = copytoappid;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }
                lcmd.CommandText = schema + "." + "SP_VALIDATEOBJECTS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                var dt = new DataTable();
                dt = lds.Tables[0];
                objects = dt.AsEnumerable().Select(row => Convert.ToString(row.Field<string>("objectname"))).Distinct().ToList();

                logger.Info(string.Format("Duplicate Object List end | Old ApplicationId: {0} | New ApplicationId: {1} | Username: {2}", copyfromappid, copytoappid, Username));
                return objects;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for DuplicateObjectList method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", copyfromappid, copytoappid, lstrConn, schema, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for DuplicateObjectList method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", copyfromappid, copytoappid, lstrConn, schema, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for DuplicateObjectList method | Old ApplicationId: {0} | New ApplicationId: {1} | Connection String : {2} | Schema : {3} | Username: {4}", copyfromappid, copytoappid, lstrConn, schema, Username), ex.InnerException);
                throw;
            }
        }
        public List<decimal?> GetObjectId(long appid)
        {
            try
            {
                logger.Info(string.Format("Get Object Id start | ApplicationId: {0} | Username: {1}", appid, Username));
                var objectid = new List<decimal?>();
                var result = entity.T_REGISTED_OBJECT.Where(x => x.APPLICATION_ID == appid).OrderBy(x => x.OBJECT_HAPPY_NAME).ToList();
                foreach (var itm in result)
                {
                    objectid.Add(itm.OBJECT_NAME_ID);
                }
                objectid = objectid.Distinct().ToList();
                logger.Info(string.Format("Get Object Id end | ApplicationId: {0} | Username: {1}", appid, Username));
                return objectid;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetObjectId method | Application Id : {0} | UserName: {1}", appid, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetObjectId method | Application Id : {0} | UserName: {1}", appid, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetObjectId Method | Application Id : {0} | UserName: {1}", appid, Username), ex.InnerException);
                throw;
            }
        }

        public ObjectIds GetPegObjectIdByObjectName(string lObjectName)
        {
            try
            {
                logger.Info(string.Format("Get Peg Objects start | lObjectName: {0} | UserName: {1}", lObjectName, Username));
                var result = (from k in entity.T_REGISTED_OBJECT
                             join obj_n in entity.T_OBJECT_NAMEINFO on k.OBJECT_NAME_ID equals obj_n.OBJECT_NAME_ID
                             where k.OBJECT_HAPPY_NAME.ToUpper() == lObjectName.ToUpper()
                             orderby k.OBJECT_HAPPY_NAME
                             select new ObjectIds
                             { ObjectId = (long)k.OBJECT_ID, ObjectNameID = (decimal)k.OBJECT_NAME_ID }).FirstOrDefault();

                logger.Info(string.Format("Get Peg Objects end | lObjectName: {0} | UserName: {1}", lObjectName, Username));

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Object for GetPegObjectIdByObjectNameId method | lObjectName : {0} | UserName: {1}", lObjectName, Username));
                ELogger.ErrorException(string.Format("Error occured in Object for GetPegObjectIdByObjectNameId method | lObjectName : {0} | UserName: {1}", lObjectName , Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Object for GetPegObjectIdByObjectNameId method | lObjectName : {0} | UserName: {1}", lObjectName , Username), ex.InnerException);
                throw;
            }
        }
    }
}

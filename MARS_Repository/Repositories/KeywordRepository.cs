using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
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
    public class KeywordRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;
        public string currentPath = string.Empty;

        public List<T_KEYWORD> GetKeywords()
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get Keywords start | UserName: {0} | Start time: {1}", Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")), currentPath);
                var result = (from k in entity.T_KEYWORD
                              where k.KEY_WORD_NAME != null
                              orderby k.KEY_WORD_NAME
                              select k).ToList<T_KEYWORD>();
                Helper.WriteLogMessage(string.Format("Get Keywords end | UserName: {0} | Start time: {1}", Username, DateTime.Now.ToString("HH:mm:ss.ffff tt")), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in GetKeywords method | UserName: {0} | Error: {1}",  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in GetKeywords method | UserName: {0} | Error: {1}", Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public T_KEYWORD GetKeywordByName(string lKeywordName)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get Keyword start | KeywordName: {0} | UserName: {1}", lKeywordName, Username), currentPath);
                var result = (from k in entity.T_KEYWORD
                              where k.KEY_WORD_NAME == lKeywordName
                              select k).ToList<T_KEYWORD>().FirstOrDefault();
                Helper.WriteLogMessage(string.Format("Get Keyword end | KeywordName: {0} | UserName: {1}", lKeywordName, Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in GetKeywordByName method | Keyword Name : {0} | UserName: {1} | Error: {2} ", lKeywordName,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in GetKeywordByName method | Keyword Name : {0} | UserName: {1} | Error: {2} ", lKeywordName, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public bool CheckKeywordPegType(long lKeywordId)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("CheckKeywordPegType start | KeywordId: {0} | UserName: {1}", lKeywordId, Username), currentPath);
                var lTypeId = entity.T_GUI_COMPONENT_TYPE_DIC.Where(x => x.TYPE_NAME == "Pegwindow").FirstOrDefault().TYPE_ID;
                var result = entity.T_DIC_RELATION_KEYWORD.Any(x => x.KEY_WORD_ID == lKeywordId && x.TYPE_ID == lTypeId);
                Helper.WriteLogMessage(string.Format("CheckKeywordPegType end | KeywordId: {0} | UserName: {1}", lKeywordId, Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in CheckKeywordPegType method | Keyword Id : {0} | UserName: {1} | Error: {2} ", lKeywordId,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in CheckKeywordPegType method | Keyword Id : {0} | UserName: {1} | Error: {2}", lKeywordId, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public List<T_GUI_COMPONENT_TYPE_DIC> ListOfKeywordType()
        {
            try
            {
                Helper.WriteLogMessage(string.Format("ListOfKeywordType start | UserName: {0}", Username), currentPath);
                var result = entity.T_GUI_COMPONENT_TYPE_DIC.ToList();
                Helper.WriteLogMessage(string.Format("ListOfKeywordType end | UserName: {0}", Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in ListOfKeywordType method | UserName: {0} | Error: {1}",  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in ListOfKeywordType method | UserName: {0} | Error: {1}", Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public List<KeywordViewModel> ListAllKeyword(string schema, string lconstring, int startrec, int pagesize, string colname, string colorder, string namesearch, string typesearch, string entryinfilesearch)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("ListAllKeyword start | UserName: {0}", Username), currentPath);
                var result = new List<KeywordViewModel>();

                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[8];
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

                ladd_refer_image[5] = new OracleParameter("TypeSearch", OracleDbType.Varchar2);
                ladd_refer_image[5].Value = typesearch;

                ladd_refer_image[6] = new OracleParameter("EntryInFIle", OracleDbType.Varchar2);
                ladd_refer_image[6].Value = entryinfilesearch;


                ladd_refer_image[7] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[7].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_LIST_KEYWORDS";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];

                List<KeywordViewModel> resultList = dt.AsEnumerable().Select(row =>
                          new KeywordViewModel
                          {
                              KeywordId = row.Field<long>("keywordid"),
                              KeywordName = Convert.ToString(row.Field<string>("keywordname")),
                              ControlType = Convert.ToString(row.Field<string>("typename")),
                              ControlTypeId = Convert.ToString(row.Field<string>("typeid")),
                              EntryFile = Convert.ToString(row.Field<string>("entry")),
                              TotalCount = Convert.ToInt32(row.Field<decimal>("RESULT_COUNT"))
                          }).ToList();

                Helper.WriteLogMessage(string.Format("ListAllKeyword end | UserName: {0}", Username), currentPath);
                return resultList;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in ListAllKeyword method | Connection string : {0} | Schema : {1} | UserName: {2} | Error: {3}", lconstring, schema,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in ListAllKeyword method | UserName: {0} | Error: {1}", Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
        public bool CheckDuplicateKeywordNameExist(string keywordname, long? KeywordId)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Check Duplicate Keyword Name Exist start | Keyword: {0} | KeywordId: {1} | UserName: {2}", keywordname, KeywordId, Username), currentPath);
                var lresult = false;
                if (KeywordId != null)
                {
                    lresult = entity.T_KEYWORD.Any(x => x.KEY_WORD_ID != KeywordId && x.KEY_WORD_NAME.ToLower().Trim() == keywordname.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_KEYWORD.Any(x => x.KEY_WORD_NAME.ToLower().Trim() == keywordname.ToLower().Trim());
                }
                Helper.WriteLogMessage(string.Format("Check Duplicate Keyword Name Exist end | Keyword: {0} | KeywordId: {1} | UserName: {2}", keywordname, KeywordId, Username), currentPath);
                return lresult;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in CheckDuplicateKeywordNameExist method | KeywordId : {0} | Keyword Name : {1} | UserName: {2} | Error: {3}", KeywordId, keywordname,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in CheckDuplicateKeywordNameExist method | KeywordId : {0} | Keyword Name : {1} | UserName: {2} | Error: {3}", KeywordId, keywordname, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public bool AddEditKeyword(KeywordViewModel lEntity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(lEntity.KeywordName))
                    {
                        lEntity.KeywordName = lEntity.KeywordName.Trim();
                    }
                    var flag = false;
                    if (lEntity.KeywordId == 0)
                    {
                        Helper.WriteLogMessage(string.Format("Add Keyword start | Keyword: {0} | UserName: {1}", lEntity.KeywordName, Username), currentPath);
                        var tbl = new T_KEYWORD();
                        tbl.KEY_WORD_ID = Helper.NextTestSuiteId("T_KEYWORD_SEQ");
                        tbl.KEY_WORD_NAME = lEntity.KeywordName;
                        tbl.ENTRY_IN_DATA_FILE = lEntity.EntryFile;
                        lEntity.KeywordId = tbl.KEY_WORD_ID;
                        entity.T_KEYWORD.Add(tbl);
                        entity.SaveChanges();
                        flag = true;
                        Helper.WriteLogMessage(string.Format("Add Keyword end | Keyword: {0} | UserName: {1}", lEntity.KeywordName, Username), currentPath);
                    }
                    else
                    {
                        Helper.WriteLogMessage(string.Format("Edit Keyword start | Keyword: {0} | KeywordId: {1} | UserName: {2}", lEntity.KeywordName, lEntity.KeywordId, Username), currentPath);
                        var tbl = entity.T_KEYWORD.Find(lEntity.KeywordId);
                        #region Type Mapping Delete
                        var ltypeList = entity.T_DIC_RELATION_KEYWORD.Where(x => x.KEY_WORD_ID == lEntity.KeywordId).ToList();
                        foreach (var item in ltypeList)
                        {
                            entity.T_DIC_RELATION_KEYWORD.Remove(item);
                        }
                        entity.SaveChanges();
                        #endregion

                        if (tbl != null)
                        {
                            tbl.KEY_WORD_NAME = lEntity.KeywordName;
                            tbl.ENTRY_IN_DATA_FILE = lEntity.EntryFile;
                            entity.SaveChanges();
                        }
                        flag = true;
                        Helper.WriteLogMessage(string.Format("Edit Keyword end | Keyword: {0} | KeywordId: {1} | UserName: {2}", lEntity.KeywordName, lEntity.KeywordId, Username), currentPath);
                    }
                    if (!string.IsNullOrEmpty(lEntity.ControlTypeId))
                    {
                        var lTypeSplit = lEntity.ControlTypeId.Split(',').Select(Int64.Parse).ToList();

                        if (lTypeSplit.Count() > 0)
                        {
                            lTypeSplit = lTypeSplit.Distinct().ToList();
                            foreach (var item in lTypeSplit)
                            {
                                var ltypetbl = new T_DIC_RELATION_KEYWORD();
                                ltypetbl.TYPE_ID = item;
                                ltypetbl.KEY_WORD_ID = lEntity.KeywordId;
                                ltypetbl.RELATION_ID = Helper.NextTestSuiteId("T_DIC_RELATION_KEYWORD_SEQ");
                                entity.T_DIC_RELATION_KEYWORD.Add(ltypetbl);
                                entity.SaveChanges();
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
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in AddEditKeyword method | Keyword Id : {0} | UserName: {1} | Error: {2}", lEntity.KeywordId,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in AddEditKeyword method | Keyword Id : {0} | UserName: {1} | Error: {2}", lEntity.KeywordId, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public List<string> CheckTestCaseExistsInKeyword(long Keywordid)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Check TestCase Exists In Keyword start | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                List<string> Keywordname = new List<string>();
                var lKeywordList = entity.T_TEST_STEPS.Where(x => x.KEY_WORD_ID == Keywordid).ToList();

                if (lKeywordList.Count() > 0)
                {
                    foreach (var item in lKeywordList)
                    {
                        var sname = entity.T_TEST_CASE_SUMMARY.Find(item.TEST_CASE_ID);

                        Keywordname.Add(sname.TEST_CASE_NAME);
                        Keywordname = (from w in Keywordname select w).Distinct().ToList();
                    }
                    Helper.WriteLogMessage(string.Format("Check TestCase Exists In Keyword end | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                    return Keywordname;
                }
                Helper.WriteLogMessage(string.Format("Check TestCase Exists In Keyword end | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                return Keywordname;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in CheckTestCaseExistsInKeyword method | Keyword Id : {0} | UserName: {1} | Error: {2}", Keywordid,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in CheckTestCaseExistsInKeyword method | Keyword Id : {0} | UserName: {1} | Error: {2}", Keywordid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public bool DeleteKeyword(long Keywordid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Helper.WriteLogMessage(string.Format("Delete Keyword start | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                    var flag = false;
                    var result = entity.T_KEYWORD.FirstOrDefault(x => x.KEY_WORD_ID == Keywordid);
                    if (result != null)
                    {
                        var reltypeorj = entity.T_DIC_RELATION_KEYWORD.Where(x => x.KEY_WORD_ID == Keywordid).ToList();
                        foreach (var item in reltypeorj)
                        {
                            entity.T_DIC_RELATION_KEYWORD.Remove(item);
                            entity.SaveChanges();
                        }

                        entity.T_KEYWORD.Remove(result);
                        entity.SaveChanges();
                        flag = true;
                    }
                    Helper.WriteLogMessage(string.Format("Delete Keyword end | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                    scope.Complete();
                    return flag;
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in DeleteKeyword method | Keyword Id : {0} | UserName: {1} | Error: {2}", Keywordid,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in DeleteKeyword method | Keyword Id : {0} | UserName: {1} | Error: {2}", Keywordid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }

        public string GetKeywordById(long Keywordid)
        {
            try
            {
                Helper.WriteLogMessage(string.Format("Get Keyword Id start | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                var result = entity.T_KEYWORD.FirstOrDefault(x => x.KEY_WORD_ID == Keywordid).KEY_WORD_NAME;
                Helper.WriteLogMessage(string.Format("Get Keyword Id end | KeywordId: {0} | UserName: {1}", Keywordid, Username), currentPath);
                return result;
            }
            catch (Exception ex)
            {
                Helper.WriteLogMessage(string.Format("Error occured in Keyword for in GetKeywordById method | Keyword Id : {0} | UserName: {1} | Error: {2} ", Keywordid,  Username, ex.ToString()), currentPath);
                if (ex.InnerException != null)
                    Helper.WriteLogMessage(string.Format("InnerException : Error occured in Keyword for in GetKeywordById method | Keyword Id : {0} | UserName: {1} | Error: {2} ", Keywordid, Username, ex.InnerException.ToString()), currentPath);
                throw;
            }
        }
    }
}

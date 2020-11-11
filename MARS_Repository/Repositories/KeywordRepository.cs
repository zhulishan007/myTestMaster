using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<T_KEYWORD> GetKeywords()
        {
            try
            {
                logger.Info(string.Format("Get Keywords start | UserName: {0}", Username));
                var result = (from k in entity.T_KEYWORD
                              where k.KEY_WORD_NAME != null
                              orderby k.KEY_WORD_NAME
                              select k).ToList<T_KEYWORD>();
                logger.Info(string.Format("Get Keywords end | UserName: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Testcase in GetKeywords method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Testcase in GetKeywords method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public T_KEYWORD GetKeywordByName(string lKeywordName)
        {
            try
            {
                logger.Info(string.Format("Get Keyword start | KeywordName: {0} | UserName: {1}", lKeywordName,Username));
                var result = (from k in entity.T_KEYWORD
                              where k.KEY_WORD_NAME == lKeywordName
                              select k).ToList<T_KEYWORD>().FirstOrDefault();
                logger.Info(string.Format("Get Keyword end | KeywordName: {0} | UserName: {1}", lKeywordName, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured Testcase in GetKeywordByName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured Testcase in GetKeywordByName method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool CheckKeywordPegType(long lKeywordId)
        {
            try
            {
                logger.Info(string.Format("CheckKeywordPegType start | KeywordId: {0} | UserName: {1}", lKeywordId, Username));
                var lTypeId = entity.T_GUI_COMPONENT_TYPE_DIC.Where(x => x.TYPE_NAME == "Pegwindow").FirstOrDefault().TYPE_ID;
                var result = entity.T_DIC_RELATION_KEYWORD.Any(x => x.KEY_WORD_ID == lKeywordId && x.TYPE_ID == lTypeId);
                logger.Info(string.Format("CheckKeywordPegType end | KeywordId: {0} | UserName: {1}", lKeywordId, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured KeywordRepository in CheckKeywordPegType method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured KeywordRepository in CheckKeywordPegType method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<T_GUI_COMPONENT_TYPE_DIC> ListOfKeywordType()
        {
            try
            {
                logger.Info(string.Format("ListOfKeywordType start | UserName: {0}", Username));
                var result = entity.T_GUI_COMPONENT_TYPE_DIC.ToList();
                logger.Info(string.Format("ListOfKeywordType end | UserName: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured KeywordRepository in ListOfKeywordType method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured KeywordRepository in ListOfKeywordType method | UserName: {0}", Username), ex);
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
                logger.Info(string.Format("ListAllKeyword start | UserName: {0}", Username));
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

                logger.Info(string.Format("ListAllKeyword end | UserName: {0}", Username));
                return resultList;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured KeywordRepository in ListAllKeyword method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured KeywordRepository in ListAllKeyword method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public bool CheckDuplicateKeywordNameExist(string keywordname, long? KeywordId)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate Keyword Name Exist start | Keyword: {0} | KeywordId: {1} | UserName: {2}", keywordname, KeywordId, Username));
                var lresult = false;
                if (KeywordId != null)
                {
                    lresult = entity.T_KEYWORD.Any(x => x.KEY_WORD_ID != KeywordId && x.KEY_WORD_NAME.ToLower().Trim() == keywordname.ToLower().Trim());
                }
                else
                {
                    lresult = entity.T_KEYWORD.Any(x => x.KEY_WORD_NAME.ToLower().Trim() == keywordname.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate Keyword Name Exist end | Keyword: {0} | KeywordId: {1} | UserName: {2}", keywordname, KeywordId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured keyword page in CheckDuplicateKeywordNameExist method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured keyword page in CheckDuplicateKeywordNameExist method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool AddEditKeyword(KeywordViewModel lEntity)
        {
            try
            {
                if (!string.IsNullOrEmpty(lEntity.KeywordName))
                {
                    lEntity.KeywordName = lEntity.KeywordName.Trim();
                }
                var flag = false;
                if (lEntity.KeywordId == 0)
                {
                    logger.Info(string.Format("Add Keyword start | Keyword: {0} | UserName: {1}", lEntity.KeywordName, Username));
                    var tbl = new T_KEYWORD();
                    tbl.KEY_WORD_ID = Helper.NextTestSuiteId("T_KEYWORD_SEQ");
                    tbl.KEY_WORD_NAME = lEntity.KeywordName;
                    tbl.ENTRY_IN_DATA_FILE = lEntity.EntryFile;
                    lEntity.KeywordId = tbl.KEY_WORD_ID;
                    entity.T_KEYWORD.Add(tbl);
                    entity.SaveChanges();
                    flag = true;
                    logger.Info(string.Format("Add Keyword end | Keyword: {0} | UserName: {1}", lEntity.KeywordName, Username));
                }
                else
                {
                    logger.Info(string.Format("Edit Keyword start | Keyword: {0} | KeywordId: {1} | UserName: {2}", lEntity.KeywordName, lEntity.KeywordId, Username));
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
                    logger.Info(string.Format("Edit Keyword end | Keyword: {0} | KeywordId: {1} | UserName: {2}", lEntity.KeywordName, lEntity.KeywordId, Username));
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
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured keyword page in AddEditKeyword method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured keyword page in AddEditKeyword method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public List<string> CheckTestCaseExistsInKeyword(long Keywordid)
        {
            try
            {
                logger.Info(string.Format("Check TestCase Exists In Keyword start | KeywordId: {0} | UserName: {1}", Keywordid, Username));
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
                    logger.Info(string.Format("Check TestCase Exists In Keyword end | KeywordId: {0} | UserName: {1}", Keywordid, Username));
                    return Keywordname;
                }
                logger.Info(string.Format("Check TestCase Exists In Keyword end | KeywordId: {0} | UserName: {1}", Keywordid, Username));
                return Keywordname;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured keyword page in CheckTestCaseExistsInKeyword method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured keyword page in CheckTestCaseExistsInKeyword method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool DeleteKeyword(long Keywordid)
        {
            try
            {
                logger.Info(string.Format("Delete Keyword start | KeywordId: {0} | UserName: {1}", Keywordid, Username));
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
                logger.Info(string.Format("Delete Keyword end | KeywordId: {0} | UserName: {1}", Keywordid, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured keyword page in DeleteKeyword method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured keyword page in DeleteKeyword method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public string GetKeywordById(long Keywordid)
        {
            try
            {
                logger.Info(string.Format("Get Keyword Id start | KeywordId: {0} | UserName: {1}", Keywordid, Username));
                var result = entity.T_KEYWORD.FirstOrDefault(x => x.KEY_WORD_ID == Keywordid).KEY_WORD_NAME;
                logger.Info(string.Format("Get Keyword Id end | KeywordId: {0} | UserName: {1}", Keywordid, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured keyword page in GetKeywordById method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured keyword page in GetKeywordById method | UserName: {0}", Username), ex);
                throw;
            }
        }
    }
}

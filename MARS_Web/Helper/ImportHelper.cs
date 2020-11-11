using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


namespace MARS_Web.Helper
{
    public static class ImportHelper
    {
        public static OracleConnection GetOracleConnection(string StrConnection)
        {
            return new OracleConnection(StrConnection);
        }
        public static string Import_TestSuiteName = string.Empty;
        public static string Import_Storyboard = string.Empty;
        public static string Import_Variable = string.Empty;
        public static string Import_Object = string.Empty;
        public static string logFilePath = string.Empty;

        public static string ImportTestSuite(string lFilePath, string lstrConn, string lSchema, string lExportLogPath, string LoginName)
        {
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
            var lreturnpath = "";
            sequence_counter.AutoIncrement = true;
            sequence_counter.AutoIncrementSeed = 1;
            sequence_counter.AutoIncrementStep = 1;

            td.Columns.Add(sequence_counter);
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("TestCase Name");
            td.Columns.Add("Dataset Name");
            td.Columns.Add("Test step Number");

            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            dbtable.dt_Log = td.Copy();

            var lValidationLogFeedProcessId = 0;
            string lOperation = "INSERT";
            string lCreatedBy = LoginName;
            string lStatus = "INPROCESS";
            string lOperationobject = "INSERT";
            string lCREATEDBY = LoginName;
            string lStatusobject = "INPROCESS";
            int lFeedProcessId = FeedProcessHelper.FeedProcess(0, lOperation, lCreatedBy, lStatus, lstrConn, lSchema);
            bool lFinalResult = true;
            string lFileName = Path.GetFileName(lFilePath);
            string lFullPath = lFilePath;
            int lFeedProcessDetailsId = 0;
            int lDEFAULT_FEEDPROCESSDETAIL_ID = 0;
            string lFileType = "TESTCASE";
            string fileName = Import_TestSuiteName;
            fileName = Path.GetFileNameWithoutExtension(fileName);
            lFeedProcessDetailsId = FeedProcessHelper.FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, lSchema);

            if (lFeedProcessDetailsId != 0)
            {
                // call Import Method for Objects File
                bool lResultImportTestCase = false;
                lResultImportTestCase = ImportTestCaseExcel(lFullPath, lFeedProcessDetailsId, lExportLogPath, lstrConn, lSchema);
                
                if (lResultImportTestCase == true)
                {
                    
                    lOperationobject = "UPDATE";
                    lStatusobject = "COMPLETED";
                    int lSuccess1 = FeedProcessHelper.FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, lSchema);
                }

                if (dbtable.dt_Log.Rows.Count > 0)
                {
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, lExportLogPath);
                    
                    return lreturnpath;
                }
                lFinalResult = FeedProcessHelper.MappingValidation(lFeedProcessId, lstrConn, lSchema);
                
                System.Threading.Thread.Sleep(200);
                DataTable dt = new DataTable();
                
                dt = FeedProcessHelper.DbExcel(lFeedProcessId, lstrConn, lSchema);
                
                dbtable.dt_Log.AcceptChanges();
                System.Threading.Thread.Sleep(200);
                
                if (dt.Rows.Count != 0)
                {
                    
                    dbtable.dt_Log.Merge(dt);
                    try
                    {
                        dbtable.dt_Log.Columns.RemoveAt(17);
                        dbtable.dt_Log.Columns.RemoveAt(16);
                    }
                    catch(Exception ex)
                    {
                        WriteMessage("Exception:"+ex.Message, logFilePath, "TestSuite", Import_TestSuiteName);
                    }
                    System.Threading.Thread.Sleep(200);
                }

                if (lFinalResult && dbtable.dt_Log.Rows.Count <= 6)
                {
                    
                    lFinalResult = FeedProcessHelper.DatawareHouseMapping(lFeedProcessId, 1, lstrConn, lSchema);
                    System.Threading.Thread.Sleep(200);

                    if (lFinalResult)
                    {
                        lOperation = "UPDATE";
                        lCreatedBy = LoginName;
                        lStatus = (lFinalResult) ? "COMPLETED" : "ERROR";
                        int lSuccess = FeedProcessHelper.FeedProcess(lFeedProcessId, lOperation, lCreatedBy, lStatus, lstrConn, lSchema);
                        string lFileNameExport = "LOGREPORT-" + Path.GetFileName(lFilePath);
                        string lExportLogFile = lExportLogPath;
                        //return "1";
                    }
                    
                }
                else
                {
                    lValidationLogFeedProcessId = lFeedProcessId;
                    
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, lExportLogPath);
                    
                }
            }
            return lreturnpath;
        }

        //public static string ImportTestCaseExcel(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string lstrConn, string schema)
        //{
        //    try
        //    {
        //        DataSet ds = ImportExcelXLS(pFilePath, true);
        //        if (ds.Tables.Count == 0)
        //        {
        //            dbtable.ErrorLog("File is opened in bckground", "", "", 0);
        //            return "File is opened in bckground.... Please close it before importing!!";
        //        }
        //        DataTable dtImport = null;
        //        string lTSName = string.Empty, lTCName = string.Empty, lTCDesc = string.Empty;
        //        string lApplication = string.Empty;

        //        //int flag = 0;
        //        DataTable table = new DataTable();
        //        string lNewApplicationName = string.Empty;
        //        //   dbtable.errorlog("Spreadsheet format validation started ", "", "", 0);

        //        table = ValidateExcel(ds, LogPath);
        //        dbtable.dt_Log.Merge(table);
        //        //excel(table, LogPath);
        //        System.Threading.Thread.Sleep(200);

        //        for (int i = 0; i < ds.Tables.Count; i++)
        //        {
        //            string SheetName = ds.Tables[i].TableName.ToString();

        //            lApplication = "";
        //            for (int m = 1; m < ds.Tables[i].Columns.Count; m++)
        //            {
        //                lNewApplicationName = (string)(ds.Tables[i].Rows[0][m].ToString());
        //                if (!string.IsNullOrEmpty(lNewApplicationName))
        //                {
        //                    if (string.IsNullOrEmpty(lApplication))
        //                    {
        //                        lApplication = lNewApplicationName;
        //                    }
        //                    else
        //                    {
        //                        lApplication += ',' + lNewApplicationName;
        //                    }
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(ds.Tables[i].Rows[0][0].ToString()))
        //            {
        //                lTSName = ds.Tables[i].Rows[1][1].ToString();
        //                lTCName = ds.Tables[i].Rows[2][1].ToString();
        //                lTCDesc = ds.Tables[i].Rows[3][1].ToString();

        //                if (!string.IsNullOrEmpty(ds.Tables[i].Rows[0][0].ToString()))
        //                {
        //                    if (!string.IsNullOrEmpty(ds.Tables[i].Rows[1][0].ToString()))
        //                    {
        //                        if (!string.IsNullOrEmpty(ds.Tables[i].Rows[2][0].ToString()))
        //                        {
        //                            if (!string.IsNullOrEmpty(ds.Tables[i].Rows[3][0].ToString()))
        //                            {
        //                                if (!string.IsNullOrEmpty(ds.Tables[i].Rows[4][0].ToString()))
        //                                {
        //                                    if (!string.IsNullOrEmpty(ds.Tables[i].Rows[5][0].ToString()))
        //                                    {
        //                                        if (!string.IsNullOrEmpty(ds.Tables[i].Rows[5][1].ToString()))
        //                                        {
        //                                            if (!string.IsNullOrEmpty(ds.Tables[i].Rows[5][2].ToString()))
        //                                            {

        //                                                if (!string.IsNullOrEmpty(ds.Tables[i].Rows[5][3].ToString()))
        //                                                {
        //                                                    if (!string.IsNullOrEmpty(ds.Tables[i].Rows[0][1].ToString()))
        //                                                    {
        //                                                        if (!string.IsNullOrEmpty(ds.Tables[i].Rows[1][1].ToString()))
        //                                                        {
        //                                                            if (!string.IsNullOrEmpty(ds.Tables[i].Rows[2][1].ToString()))
        //                                                            {
        //                                                                var arrayNames = (from DataColumn x in ds.Tables[i].Columns
        //                                                                                  select x.ColumnName).ToArray();

        //                                                                var columnname = (from DataColumn x in ds.Tables[i].Columns
        //                                                                                  where ds.Tables[i].Rows[5][x.ColumnName].ToString() == ""
        //                                                                                  select x.ColumnName).ToArray();

        //                                                                dtImport = new DataTable();
        //                                                                dtImport.Columns.Add("TESTSUITENAME");
        //                                                                dtImport.Columns.Add("TESTCASENAME");
        //                                                                dtImport.Columns.Add("TESTCASEDESCRIPTION");
        //                                                                dtImport.Columns.Add("DATASETMODE");
        //                                                                dtImport.Columns.Add("KEYWORD");
        //                                                                dtImport.Columns.Add("OBJECT");
        //                                                                dtImport.Columns.Add("PARAMETER");
        //                                                                dtImport.Columns.Add("COMMENTS");
        //                                                                dtImport.Columns.Add("DATASETNAME");
        //                                                                dtImport.Columns.Add("DATASETVALUE");
        //                                                                dtImport.Columns.Add("ROWNUMBER");
        //                                                                dtImport.Columns.Add("FEEDPROCESSDETAILID");
        //                                                                dtImport.Columns.Add("TABNAME");
        //                                                                dtImport.Columns.Add("APPLICATION");
        //                                                                dtImport.Columns.Add("SKIP");
        //                                                                dtImport.Columns.Add("DATASETDESCRIPTION");

        //                                                                for (int k = 4; k < arrayNames.Length; k++)
        //                                                                {
        //                                                                    int l = -1;
        //                                                                    foreach (DataRow dr1 in ds.Tables[i].Rows)
        //                                                                    {
        //                                                                        l++;
        //                                                                        if (l > 7 && k % 2 == 0)
        //                                                                        {
        //                                                                            if (!string.IsNullOrEmpty(ds.Tables[i].Rows[5][k].ToString()))
        //                                                                            {
        //                                                                                DataRow dr = dtImport.NewRow();
        //                                                                                dr["TESTSUITENAME"] = lTSName;
        //                                                                                dr["TESTCASENAME"] = lTCName;
        //                                                                                dr["TESTCASEDESCRIPTION"] = lTCDesc;
        //                                                                                dr["KEYWORD"] = Convert.ToString(dr1[0].ToString());
        //                                                                                dr["OBJECT"] = Convert.ToString(dr1[1].ToString());
        //                                                                                dr["PARAMETER"] = Convert.ToString(dr1[2].ToString());
        //                                                                                dr["COMMENTS"] = Convert.ToString(dr1[3].ToString());
        //                                                                                dr["DATASETMODE"] = ds.Tables[i].Rows[4][k].ToString();
        //                                                                                dr["DATASETNAME"] = ds.Tables[i].Rows[5][k].ToString();
        //                                                                                dr["DATASETDESCRIPTION"] = ds.Tables[i].Rows[6][k].ToString();
        //                                                                                dr["DATASETVALUE"] = dr1[arrayNames[k + 1]].ToString();
        //                                                                                dr["SKIP"] = (!string.IsNullOrEmpty(dr1[arrayNames[k]].ToString()) && dr1[arrayNames[k]].ToString().ToUpper() == "SKIP" ? 4 : 0);
        //                                                                                dr["ROWNUMBER"] = l - 7;
        //                                                                                dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
        //                                                                                dr["TABNAME"] = ds.Tables[i].TableName;
        //                                                                                dr["APPLICATION"] = lApplication;

        //                                                                                dtImport.Rows.Add(dr);
        //                                                                            }
        //                                                                            else
        //                                                                            {
        //                                                                                break;
        //                                                                            }
        //                                                                        }
        //                                                                    }
        //                                                                }

        //                                                                if (dtImport.Rows.Count > 0)
        //                                                                {
        //                                                                    OracleTransaction ltransaction;
        //                                                                    OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
        //                                                                    lconnection.Open();
        //                                                                    ltransaction = lconnection.BeginTransaction();

        //                                                                    OracleCommand lcmd;
        //                                                                    lcmd = lconnection.CreateCommand();
        //                                                                    lcmd.Transaction = ltransaction;

        //                                                                    List<DataRow> list = dtImport.AsEnumerable().ToList();
        //                                                                    lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_TESTCASE";

        //                                                                    string[] TESTSUITENAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTSUITENAME")).ToArray();
        //                                                                    string[] TESTCASENAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTCASENAME")).ToArray();
        //                                                                    string[] TESTCASEDESCRIPTION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTCASEDESCRIPTION")).ToArray();
        //                                                                    string[] DATASETMODE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETMODE")).ToArray();
        //                                                                    string[] KEYWORD_param = dtImport.AsEnumerable().Select(r => r.Field<string>("KEYWORD")).ToArray();
        //                                                                    string[] OBJECT_param = dtImport.AsEnumerable().Select(r => r.Field<string>("OBJECT")).ToArray();
        //                                                                    string[] PARAMETER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("PARAMETER")).ToArray();
        //                                                                    string[] COMMENTS_param = dtImport.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
        //                                                                    string[] DATASETNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
        //                                                                    string[] DATASETVALUE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETVALUE")).ToArray();
        //                                                                    string[] ROWNUMBER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("ROWNUMBER")).ToArray(); ;
        //                                                                    string[] TABNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TABNAME")).ToArray();
        //                                                                    string[] FEEDPROCESSDETAILID_param = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
        //                                                                    string[] APPLICATION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("APPLICATION")).ToArray();
        //                                                                    string[] SKIP_param = dtImport.AsEnumerable().Select(r => r.Field<string>("SKIP")).ToArray();
        //                                                                    string[] DATASETDESCRIPTION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETDESCRIPTION")).ToArray();

        //                                                                    if (DATASETNAME_param.Length != 0)
        //                                                                    {
        //                                                                        OracleParameter TESTSUITENAME_oparam = new OracleParameter();
        //                                                                        TESTSUITENAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        TESTSUITENAME_oparam.Value = TESTSUITENAME_param;

        //                                                                        OracleParameter TESTCASENAME_oparam = new OracleParameter();
        //                                                                        TESTCASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        TESTCASENAME_oparam.Value = TESTCASENAME_param;

        //                                                                        OracleParameter TESTCASEDESCRIPTION_oparam = new OracleParameter();
        //                                                                        TESTCASEDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        TESTCASEDESCRIPTION_oparam.Value = TESTCASEDESCRIPTION_param;

        //                                                                        OracleParameter DATASETMODE_oparam = new OracleParameter();
        //                                                                        DATASETMODE_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        DATASETMODE_oparam.Value = DATASETMODE_param;

        //                                                                        OracleParameter KEYWORD_oparam = new OracleParameter();
        //                                                                        KEYWORD_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        KEYWORD_oparam.Value = KEYWORD_param;

        //                                                                        OracleParameter OBJECT_oparam = new OracleParameter();
        //                                                                        OBJECT_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        OBJECT_oparam.Value = OBJECT_param;

        //                                                                        OracleParameter PARAMETER_oparam = new OracleParameter();
        //                                                                        PARAMETER_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        PARAMETER_oparam.Value = PARAMETER_param;

        //                                                                        OracleParameter COMMENTS_oparam = new OracleParameter();
        //                                                                        COMMENTS_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        COMMENTS_oparam.Value = COMMENTS_param;

        //                                                                        OracleParameter DATASETNAME_oparam = new OracleParameter();
        //                                                                        DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        DATASETNAME_oparam.Value = DATASETNAME_param;

        //                                                                        OracleParameter DATASETVALUE_oparam = new OracleParameter();
        //                                                                        DATASETVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        DATASETVALUE_oparam.Value = DATASETVALUE_param;

        //                                                                        OracleParameter ROWNUMBER_oparam = new OracleParameter();
        //                                                                        ROWNUMBER_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        ROWNUMBER_oparam.Value = ROWNUMBER_param;

        //                                                                        OracleParameter TABNAME_oparam = new OracleParameter();
        //                                                                        TABNAME_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        TABNAME_oparam.Value = TABNAME_param;

        //                                                                        OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
        //                                                                        FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

        //                                                                        OracleParameter APPLICATION_oparam = new OracleParameter();
        //                                                                        APPLICATION_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        APPLICATION_oparam.Value = APPLICATION_param;

        //                                                                        OracleParameter SKIP_oparam = new OracleParameter();
        //                                                                        SKIP_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        SKIP_oparam.Value = SKIP_param;

        //                                                                        OracleParameter DATASETDESCRIPTION_oparam = new OracleParameter();
        //                                                                        DATASETDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
        //                                                                        DATASETDESCRIPTION_oparam.Value = DATASETDESCRIPTION_param;

        //                                                                        lcmd.ArrayBindCount = TESTSUITENAME_param.Length;

        //                                                                        lcmd.Parameters.Add(TESTSUITENAME_oparam);
        //                                                                        lcmd.Parameters.Add(TESTCASENAME_oparam);
        //                                                                        lcmd.Parameters.Add(TESTCASEDESCRIPTION_oparam);
        //                                                                        lcmd.Parameters.Add(DATASETMODE_oparam);
        //                                                                        lcmd.Parameters.Add(KEYWORD_oparam);
        //                                                                        lcmd.Parameters.Add(OBJECT_oparam);
        //                                                                        lcmd.Parameters.Add(PARAMETER_oparam);
        //                                                                        lcmd.Parameters.Add(COMMENTS_oparam);
        //                                                                        lcmd.Parameters.Add(DATASETNAME_oparam);
        //                                                                        lcmd.Parameters.Add(DATASETVALUE_oparam);
        //                                                                        lcmd.Parameters.Add(ROWNUMBER_oparam);
        //                                                                        lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
        //                                                                        lcmd.Parameters.Add(TABNAME_oparam);
        //                                                                        lcmd.Parameters.Add(APPLICATION_oparam);
        //                                                                        lcmd.Parameters.Add(SKIP_oparam);
        //                                                                        lcmd.Parameters.Add(DATASETDESCRIPTION_oparam);

        //                                                                        lcmd.CommandType = CommandType.StoredProcedure;
        //                                                                        try
        //                                                                        {
        //                                                                            lcmd.ExecuteNonQuery();
        //                                                                        }
        //                                                                        catch (Exception lex)
        //                                                                        {
        //                                                                            ltransaction.Rollback();

        //                                                                            throw new Exception(lex.Message);
        //                                                                        }
        //                                                                        ltransaction.Commit();
        //                                                                    }
        //                                                                    else
        //                                                                    {
        //                                                                        return "14";// Dataset Not Found
        //                                                                    }
        //                                                                }
        //                                                                else
        //                                                                {
        //                                                                    return "15";// Dataset Not Found
        //                                                                }
        //                                                            }
        //                                                            else
        //                                                            {
        //                                                                return "13"; // Test case Name data not found
        //                                                            }
        //                                                        }
        //                                                        else
        //                                                        {
        //                                                            return "12"; // Test Suite Name data not found
        //                                                        }
        //                                                    }
        //                                                    else
        //                                                    {
        //                                                        return "11"; // Application data not found
        //                                                    }

        //                                                }
        //                                                else
        //                                                {
        //                                                    return "10";// Commnet  Label not found
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                return "9";// Parameter  Label not found
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            return "8";// Object  Label not found
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        return "7";// Keyword  Label not found
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    return "6";// Mode  Label not found
        //                                }

        //                            }
        //                            else
        //                            {
        //                                return "5";// Test Case Description  Label not found
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return "4";// Test Case Name Label not found
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return "3";// Test Suite Name Label not found
        //                    }
        //                }
        //                else
        //                {
        //                    return "2"; // Application Label not found
        //                }
        //            }
        //        }
        //        return "0";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static bool ImportTestCaseExcel(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string lstrConn, string schema)
        {
            try
            {
                bool lResult = false;
                string lTSName = string.Empty, lTCName = string.Empty, lTCDesc = string.Empty;
                string lApplication = string.Empty;

                DataTable table = new DataTable();
                string lNewApplicationName = string.Empty;

                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();

                    if (worksheetcount > 0)
                    {
                        DataTable dtImport = new DataTable();
                        dtImport.Columns.Add("TESTSUITENAME");
                        dtImport.Columns.Add("TESTCASENAME");
                        dtImport.Columns.Add("TESTCASEDESCRIPTION");
                        dtImport.Columns.Add("DATASETMODE");
                        dtImport.Columns.Add("KEYWORD");
                        dtImport.Columns.Add("OBJECT");
                        dtImport.Columns.Add("PARAMETER");
                        dtImport.Columns.Add("COMMENTS");
                        dtImport.Columns.Add("DATASETNAME");
                        dtImport.Columns.Add("DATASETVALUE");
                        dtImport.Columns.Add("ROWNUMBER");
                        dtImport.Columns.Add("FEEDPROCESSDETAILID");
                        dtImport.Columns.Add("TABNAME");
                        dtImport.Columns.Add("APPLICATION");
                        dtImport.Columns.Add("SKIP");
                        dtImport.Columns.Add("DATASETDESCRIPTION");
                        dtImport.AcceptChanges();
                        for (int i = 0; i < worksheetcount; i++)
                        {
                            Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(i);
                            Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                            List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                            var tabName = mysheet.Name;
                            DataTable dt = new DataTable();

                            WriteMessage("Start Validation", logFilePath, "TestSuite", Import_TestSuiteName);

                            dt = ValidateExcel(rows, doc, tabName);
                            if (dt.Rows.Count != 0)
                            {
                                WriteMessage("Validation Error Occur", logFilePath, "TestSuite", Import_TestSuiteName);
                                lResult = false;
                                dbtable.dt_Log.Merge(dt);
                                return lResult;
                            }
                            WriteMessage("End Validation", logFilePath, "TestSuite", Import_TestSuiteName);
                            var fristRow = rows[0].Descendants<Cell>().ToList();
                            var secondRow = rows[1].Descendants<Cell>().ToList();
                            var thirdRow = rows[2].Descendants<Cell>().ToList();
                            var fourthRow = rows[3].Descendants<Cell>().ToList();
                            var fifthRow = rows[4].Descendants<Cell>().ToList();
                            var headerlst = rows[5].Descendants<Cell>().ToList();
                            var seventhRow = rows[6].Descendants<Cell>().ToList();
                            
                            lApplication = "";
                            for (int m = 1; m < fristRow.Count; m++)
                            {
                                if (fristRow[m].CellValue != null)
                                {
                                    lNewApplicationName = GetValue(doc, fristRow[m]);
                                    if (string.IsNullOrEmpty(lApplication))
                                    {
                                        lApplication = lNewApplicationName;
                                    }
                                    else
                                    {
                                        lApplication += ',' + lNewApplicationName;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                           
                            lTSName = secondRow[1].CellValue == null ? "" : GetValue(doc, secondRow[1]).Trim();
                            lTCName = thirdRow[1].CellValue == null ? "" : GetValue(doc, thirdRow[1]).Trim();
                            lTCDesc = fourthRow[1].CellValue == null ? "" : GetValue(doc, fourthRow[1]).Trim();
                            var hCount = headerlst.Count - 4;
                            hCount = hCount / 2;
                            for (int k = 4; k < headerlst.Count; k++)
                            {
                                int l = -1;
                                for (int j = 0; j < rows.Count; j++)
                                {
                                    l++;
                                    if (l > 7 && k % 2 == 0)
                                    {
                                        var celllst = rows[j].Descendants<Cell>().ToList();
                                        if (headerlst[k].CellValue != null)
                                        {
                                            DataRow dr = dtImport.NewRow();
                                            dr["TESTSUITENAME"] = lTSName;
                                            dr["TESTCASENAME"] = lTCName;
                                            dr["TESTCASEDESCRIPTION"] = lTCDesc;
                                            dr["KEYWORD"] = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]).Trim();
                                            dr["OBJECT"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]).Trim();
                                            dr["PARAMETER"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]).Trim();
                                            dr["COMMENTS"] = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]).Trim();
                                            dr["DATASETNAME"] = headerlst[k].CellValue == null ? "" : GetValue(doc, headerlst[k]).Trim();

                                            if (headerlst[k].CellReference != null)
                                            {
                                                var cIndex = GetColumnIndex(headerlst[k].CellReference);
                                                for (int p = 1; p < fifthRow.Count; p++)
                                                {
                                                    var modeIndex = GetColumnIndex(fifthRow[p].CellReference);
                                                    if (cIndex == modeIndex)
                                                        dr["DATASETMODE"] = fifthRow[p].CellValue == null ? "" : GetValue(doc, fifthRow[p]).Trim();
                                                }
                                                for (int q = 0; q < seventhRow.Count; q++)
                                                {
                                                    var decIndex = GetColumnIndex(seventhRow[q].CellReference);
                                                    if (cIndex == decIndex)
                                                        dr["DATASETDESCRIPTION"] = seventhRow[q].CellValue == null ? "" : GetValue(doc, seventhRow[q]).Trim();
                                                }
                                            }
                                            if (dr["DATASETMODE"].ToString() == "")
                                                dr["DATASETMODE"] = "";

                                            if (dr["DATASETDESCRIPTION"].ToString() == "")
                                                dr["DATASETDESCRIPTION"] = "";

                                            dr["DATASETVALUE"] = celllst[k + 1].CellValue == null ? "" : GetValue(doc, celllst[k + 1]).Trim();
                                            dr["SKIP"] = celllst[k].CellValue != null && GetValue(doc, celllst[k]).ToUpper().Trim() == "SKIP" ? 4 : 0;
                                            dr["ROWNUMBER"] = l - 7;
                                            dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                            dr["TABNAME"] = tabName;
                                            dr["APPLICATION"] = lApplication;

                                            dtImport.Rows.Add(dr);
                                            dtImport.AcceptChanges();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (dtImport.Rows.Count > 0)
                        {
                            OracleTransaction ltransaction;
                            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
                            lconnection.Open();
                            ltransaction = lconnection.BeginTransaction();

                            OracleCommand lcmd;
                            lcmd = lconnection.CreateCommand();
                            lcmd.Transaction = ltransaction;

                            List<DataRow> list = dtImport.AsEnumerable().ToList();
                            lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_TESTCASE";

                            string[] TESTSUITENAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTSUITENAME")).ToArray();
                            string[] TESTCASENAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTCASENAME")).ToArray();
                            string[] TESTCASEDESCRIPTION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTCASEDESCRIPTION")).ToArray();
                            string[] DATASETMODE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETMODE")).ToArray();
                            string[] KEYWORD_param = dtImport.AsEnumerable().Select(r => r.Field<string>("KEYWORD")).ToArray();
                            string[] OBJECT_param = dtImport.AsEnumerable().Select(r => r.Field<string>("OBJECT")).ToArray();
                            string[] PARAMETER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("PARAMETER")).ToArray();
                            string[] COMMENTS_param = dtImport.AsEnumerable().Select(r => r.Field<string>("COMMENTS")).ToArray();
                            string[] DATASETNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                            string[] DATASETVALUE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETVALUE")).ToArray();
                            string[] ROWNUMBER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("ROWNUMBER")).ToArray(); ;
                            string[] TABNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TABNAME")).ToArray();
                            string[] FEEDPROCESSDETAILID_param = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                            string[] APPLICATION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("APPLICATION")).ToArray();
                            string[] SKIP_param = dtImport.AsEnumerable().Select(r => r.Field<string>("SKIP")).ToArray();
                            string[] DATASETDESCRIPTION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETDESCRIPTION")).ToArray();

                            if (DATASETNAME_param.Length != 0)
                            {
                                OracleParameter TESTSUITENAME_oparam = new OracleParameter();
                                TESTSUITENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                TESTSUITENAME_oparam.Value = TESTSUITENAME_param;

                                OracleParameter TESTCASENAME_oparam = new OracleParameter();
                                TESTCASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                TESTCASENAME_oparam.Value = TESTCASENAME_param;

                                OracleParameter TESTCASEDESCRIPTION_oparam = new OracleParameter();
                                TESTCASEDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                                TESTCASEDESCRIPTION_oparam.Value = TESTCASEDESCRIPTION_param;

                                OracleParameter DATASETMODE_oparam = new OracleParameter();
                                DATASETMODE_oparam.OracleDbType = OracleDbType.Varchar2;
                                DATASETMODE_oparam.Value = DATASETMODE_param;

                                OracleParameter KEYWORD_oparam = new OracleParameter();
                                KEYWORD_oparam.OracleDbType = OracleDbType.Varchar2;
                                KEYWORD_oparam.Value = KEYWORD_param;

                                OracleParameter OBJECT_oparam = new OracleParameter();
                                OBJECT_oparam.OracleDbType = OracleDbType.Varchar2;
                                OBJECT_oparam.Value = OBJECT_param;

                                OracleParameter PARAMETER_oparam = new OracleParameter();
                                PARAMETER_oparam.OracleDbType = OracleDbType.Varchar2;
                                PARAMETER_oparam.Value = PARAMETER_param;

                                OracleParameter COMMENTS_oparam = new OracleParameter();
                                COMMENTS_oparam.OracleDbType = OracleDbType.Varchar2;
                                COMMENTS_oparam.Value = COMMENTS_param;

                                OracleParameter DATASETNAME_oparam = new OracleParameter();
                                DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                DATASETNAME_oparam.Value = DATASETNAME_param;

                                OracleParameter DATASETVALUE_oparam = new OracleParameter();
                                DATASETVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                                DATASETVALUE_oparam.Value = DATASETVALUE_param;

                                OracleParameter ROWNUMBER_oparam = new OracleParameter();
                                ROWNUMBER_oparam.OracleDbType = OracleDbType.Varchar2;
                                ROWNUMBER_oparam.Value = ROWNUMBER_param;

                                OracleParameter TABNAME_oparam = new OracleParameter();
                                TABNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                TABNAME_oparam.Value = TABNAME_param;

                                OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                                FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                                FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                                OracleParameter APPLICATION_oparam = new OracleParameter();
                                APPLICATION_oparam.OracleDbType = OracleDbType.Varchar2;
                                APPLICATION_oparam.Value = APPLICATION_param;

                                OracleParameter SKIP_oparam = new OracleParameter();
                                SKIP_oparam.OracleDbType = OracleDbType.Varchar2;
                                SKIP_oparam.Value = SKIP_param;

                                OracleParameter DATASETDESCRIPTION_oparam = new OracleParameter();
                                DATASETDESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                                DATASETDESCRIPTION_oparam.Value = DATASETDESCRIPTION_param;

                                lcmd.ArrayBindCount = TESTSUITENAME_param.Length;

                                lcmd.Parameters.Add(TESTSUITENAME_oparam);
                                lcmd.Parameters.Add(TESTCASENAME_oparam);
                                lcmd.Parameters.Add(TESTCASEDESCRIPTION_oparam);
                                lcmd.Parameters.Add(DATASETMODE_oparam);
                                lcmd.Parameters.Add(KEYWORD_oparam);
                                lcmd.Parameters.Add(OBJECT_oparam);
                                lcmd.Parameters.Add(PARAMETER_oparam);
                                lcmd.Parameters.Add(COMMENTS_oparam);
                                lcmd.Parameters.Add(DATASETNAME_oparam);
                                lcmd.Parameters.Add(DATASETVALUE_oparam);
                                lcmd.Parameters.Add(ROWNUMBER_oparam);
                                lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                                lcmd.Parameters.Add(TABNAME_oparam);
                                lcmd.Parameters.Add(APPLICATION_oparam);
                                lcmd.Parameters.Add(SKIP_oparam);
                                lcmd.Parameters.Add(DATASETDESCRIPTION_oparam);

                                lcmd.CommandType = CommandType.StoredProcedure;

                                try
                                {
                                    //WriteMessage("Start Insert data in Stagging Table", logFilePath, "TestSuite", Import_TestSuiteName);
                                    lcmd.ExecuteNonQuery();
                                    lResult = true;
                                    //WriteMessage("End Insert data in Stagging", logFilePath, "TestSuite", Import_TestSuiteName);
                                }
                                catch (Exception lex)
                                {
                                    WriteMessage("Error Insert data in Stagging : " + lex.Message.ToString(), logFilePath, "TestSuite", Import_TestSuiteName);
                                    lResult = false;
                                    ltransaction.Rollback();
                                    throw new Exception(lex.Message);
                                }
                                ltransaction.Commit();
                            }
                        }
                       
                    }
                }
                return lResult;
            }
            catch (Exception ex)
            {
                WriteMessage("Error :" + ex.ToString(), logFilePath, "TestSuite", Import_TestSuiteName);
                throw ex;
            }
        }

        public static DataSet ImportExcelXLS(string FileName, bool hasHeaders)
        {
            try
            {
                string HDR = hasHeaders ? "Yes" : "No";
                string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=1\";", FileName);
                bool ans;

                DataSet output = new DataSet();
                ans = IsFileInUse(FileName);
                if (ans == true)
                {
                    return (output);
                }

                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    conn.Open();

                    DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    for (int x = 1; x <= dt.Rows.Count; x++)

                    {
                        if (dt.Rows[x - 1][2].ToString().ToLower().Contains("_xlnm#_filterdatabase"))

                        {

                            dt.Rows.RemoveAt(x - 1);
                            x--;
                        }
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        bool flag = true;
                        string sheet = row["TABLE_NAME"].ToString().Trim(new char[] { (char)39 }); ;
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "A1:IU500" + "]", conn);
                        cmd.CommandType = CommandType.Text;
                        DataTable outputTable = new DataTable(sheet);
                        new OleDbDataAdapter(cmd).Fill(outputTable);
                        DataTable final = new DataTable(sheet);
                        if (outputTable.Columns.Count >= 253)
                        {
                            OleDbCommand cmd1 = new OleDbCommand("SELECT * FROM [" + sheet + "IV1:ABF500" + "]", conn);
                            DataTable temp = new DataTable(sheet);
                            new OleDbDataAdapter(cmd1).Fill(temp);

                            //outputTable.Merge(temp);
                            final = outputTable.Clone();
                            foreach (DataColumn dc in temp.Columns)
                            {
                                //  dc.ColumnName = "xx1" + dc.ColumnName;
                                //  outputTable.Columns.Add(dc);
                                string newColumnName = dc.ColumnName;
                                int colNum = 1;
                                while (final.Columns.Contains(newColumnName))
                                {
                                    newColumnName = string.Format("{0}_{1}", dc.ColumnName, ++colNum);
                                }

                                final.Columns.Add(newColumnName, dc.DataType);

                            }
                            var mergedRows = outputTable.AsEnumerable().Zip(temp.AsEnumerable(),
              (r1, r2) => r1.ItemArray.Concat(r2.ItemArray).ToArray());
                            foreach (object[] rowFields in mergedRows)
                                final.Rows.Add(rowFields);

                            final.AcceptChanges();
                            //outputTable.AcceptChanges();
                            flag = false;

                        }
                        if (flag == true)
                        {
                            output.Tables.Add(outputTable);
                        }
                        else
                        {
                            output.Tables.Add(final);
                        }
                        //output.Tables.Add(outputTable);
                        //new OleDbDataAdapter(cmd).Fill(outputTable);

                    }
                    conn.Close();
                    int i;
                    output.AcceptChanges();
                }

                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static bool IsFileInUse(string f)
        {
            try
            {
                using (var stream = new FileStream(f, FileMode.Open, FileAccess.Read)) { }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        //static DataTable ValidateExcel(DataSet ds1, string LogPath)
        //{
        //    int flag = 0;
        //    string Application_name = null;
        //    string Testcase_name;
        //    string title = null;
        //    string Description = null;
        //    string Datasetname = null;
        //    int TestStepNumber = 0;
        //    string objectname = null;
        //    string comment = null;
        //    string location = null;
        //    string TabName = null;
        //    DataTable td = new DataTable();
        //    td.Columns.Add("TimeStamp");
        //    td.Columns.Add("Message Type");
        //    td.Columns.Add("Action");
        //    td.Columns.Add("SpreadSheet cell Address");
        //    td.Columns.Add("Validation Name");
        //    td.Columns.Add("Validation Fail Description");
        //    td.Columns.Add("Application Name");
        //    td.Columns.Add("TestCase Name");
        //    td.Columns.Add("Test step Number");
        //    td.Columns.Add("Dataset Name");
        //    td.Columns.Add("Object Name");
        //    td.Columns.Add("Comment");
        //    td.Columns.Add("Error Description");
        //    td.Columns.Add("Program Location");
        //    td.Columns.Add("Tab Name");

        //    foreach (DataTable table in ds1.Tables)
        //    {
        //        if (table.TableName.Contains("_xlnm#_FilterDatabase"))
        //        {
        //            ds1.Tables.Remove(table);
        //            break;
        //        }
        //    }
        //    for (int t = 0; t < ds1.Tables.Count; t++)
        //    {
        //        int count = ds1.Tables[t].Columns.Count - 1;
        //        int rcount = ds1.Tables[t].Rows.Count;

        //        TabName = ds1.Tables[t].TableName.Replace('$', ' ');

        //        if (count == 0)
        //        {
        //            title = "empty tab";
        //            Description = "any empty tab should not be there";
        //            ErrorlogExcel(1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, "", TabName);
        //            Excel(td, LogPath);
        //            return td;
        //        }
        //        for (int i = 1; i < count; i++)
        //        {
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[0][i].ToString()))
        //            {
        //                Application_name = Application_name + Convert.ToString(ds1.Tables[t].Rows[0][i]) + ",";
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        Testcase_name = Convert.ToString(ds1.Tables[t].Rows[2][1]);


        //        int Row = 0;
        //        int Column = 0;
        //        if (ds1.Tables[t].Rows[Row][Column].ToString() != "Application")
        //        {
        //            flag = 1;
        //            title = "APPLICATION TAG";
        //            Description = "APPLICATION TAG NOT FOUND";
        //            ErrorlogExcel(Row + 1, Column, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[Row + 1][Column].ToString() != "Test Suite Name")
        //        {
        //            flag = 1;
        //            title = "Test suite name TAG";
        //            Description = "Test suite name TAG NOT FOUND";
        //            ErrorlogExcel(Row + 2, Column, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[Row + 2][Column].ToString() != "Test Case Name")
        //        {
        //            flag = 1;
        //            title = "Test case name TAG";
        //            Description = "Test case name TAG NOT FOUND";
        //            ErrorlogExcel(Row + 3, Column, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[Row + 3][Column].ToString() != "Test Case Description")
        //        {
        //            flag = 1;
        //            title = "Test case Discription TAG";
        //            Description = "Test case Discription TAG NOT FOUND";
        //            ErrorlogExcel(Row + 4, Column, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[5][0].ToString().ToUpper().Trim() != "KEYWORD")
        //        {
        //            flag = 1;
        //            title = "keyword Tag";
        //            Description = "Keyword TAG NOT FOUND";
        //            ErrorlogExcel(6, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[5][1].ToString().ToUpper().Trim() != "OBJECT")
        //        {
        //            flag = 1;
        //            title = "object TAG";
        //            Description = "object TAG NOT FOUND";
        //            ErrorlogExcel(6, 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[5][2].ToString().ToUpper().Trim() != "PARAMETERS")
        //        {
        //            flag = 1;
        //            title = "Parameters";
        //            Description = "Parameter TAG NOT FOUND";
        //            ErrorlogExcel(6, 2, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[5][3].ToString().ToUpper().Trim() != "COMMENT")
        //        {
        //            flag = 1;
        //            title = "Comment TAG";
        //            Description = "Comment TAG NOT FOUND";
        //            ErrorlogExcel(6, 3, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        if (ds1.Tables[t].Rows[4][Column + 0].ToString().ToUpper().Trim() != "MODE")
        //        {
        //            flag = 1;
        //            title = "Mode TAG";
        //            Description = "Mode TAG NOT FOUND";
        //            ErrorlogExcel(5, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (string.IsNullOrEmpty(ds1.Tables[t].Rows[Row + i][1].ToString()))
        //            {
        //                flag = 1;
        //                title = " non Empty Field";
        //                Description = "Field should not be Empty";
        //                ErrorlogExcel(Row + i + 1, 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[6][i].ToString()))
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(7, i, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[7][i].ToString()))
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(8, i, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        for (int i = 1; i < 4; i++)
        //        {
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[4][i].ToString()))
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(5, i, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        for (int i = 4; i < count; i = i + 2)
        //        {
        //            if (ds1.Tables[t].Rows[4][i].ToString().ToUpper().Trim() != "X" && ds1.Tables[t].Rows[4][i].ToString().ToUpper().Trim() != "D" && ds1.Tables[t].Rows[4][i].ToString() != "")
        //            {
        //                flag = 1;
        //                title = "Mode";
        //                Description = "Invalid Mode";
        //                ErrorlogExcel(5, i, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[4][i + 1].ToString()))
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(5, i + 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        int Flag1 = 1;
        //        int Flag2 = 1;
        //        for (int j = 1; j <= count; j++)
        //        {
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[0][j].ToString()))
        //            {
        //                Flag1 = 0;
        //            }
        //            else
        //            {
        //                Flag1 = 1;
        //                Flag2 = 0;
        //            }
        //            if (Flag1 == 0 && Flag2 == 0)
        //            {
        //                flag = 1;
        //                title = "Applicaton name";
        //                Description = "Invalid application name";
        //                ErrorlogExcel(1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        if (string.IsNullOrEmpty(ds1.Tables[t].Rows[5][4].ToString()))
        //        {
        //            title = "Data-set Name";
        //            Description = "Dataset name should be there";
        //            ErrorlogExcel(6, 4, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //        }

        //        Flag1 = 1;
        //        Flag2 = 1;
        //        int dcount = 0;
        //        int Flag3 = 1;
        //        int Flag4 = 1;
        //        for (int j = 1; j <= count - 4; j += 2)
        //        {
        //            Flag1 = 1;
        //            Flag3 = 1;
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[5][3 + j].ToString()))
        //            {
        //                Flag1 = 0;
        //                dcount = dcount + 1;
        //            }
        //            else
        //            {
        //                Flag1 = 1;
        //                Flag2 = 0;
        //            }
        //            if (Flag1 == 0 && Flag2 == 0)
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(6, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[6][3 + j].ToString()) && string.IsNullOrEmpty(ds1.Tables[t].Rows[5][3 + j].ToString()))
        //            {
        //                Flag3 = 0;


        //            }
        //            else
        //            {
        //                Flag3 = 1;
        //                Flag4 = 0;
        //            }
        //            if (Flag3 == 0 && Flag4 == 0)
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(7, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        Flag1 = 1;
        //        Flag2 = 1;
        //        Flag3 = 1;
        //        Flag4 = 1;
        //        for (int j = 1; j <= count - 5; j += 2)
        //        {
        //            Flag1 = 1;
        //            Flag3 = 1;
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[5][4 + j].ToString()))
        //            {
        //                Flag1 = 0;
        //            }
        //            else
        //            {
        //                Flag1 = 1;
        //                Flag2 = 0;
        //            }
        //            if (Flag1 == 0 && Flag2 == 0)
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(6, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //            if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[6][4 + j].ToString()))
        //            {
        //                Flag3 = 0;
        //            }
        //            else
        //            {
        //                Flag3 = 1;
        //                Flag4 = 0;
        //            }
        //            if (Flag3 == 0 && Flag4 == 0)
        //            {
        //                flag = 1;
        //                title = "Empty Field";
        //                Description = "Field must be Empty";
        //                ErrorlogExcel(7, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        for (int j = 1; j <= dcount; j = j + 2)
        //        {
        //            if (ds1.Tables[t].Rows[7][3 + j].ToString().ToUpper().Trim() != "SKIP FLAG")
        //            {
        //                flag = 1;
        //                title = "skip flag tag";
        //                Description = "skip flag tag not found";
        //                ErrorlogExcel(8, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //            if (ds1.Tables[t].Rows[7][4 + j].ToString().ToUpper().Trim() != "DATA")
        //            {
        //                flag = 1;
        //                title = "Data tag";
        //                Description = "Data tag not found";
        //                ErrorlogExcel(8, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        Flag1 = 1;
        //        Flag2 = 2;
        //        int temp = 0;
        //        int row_cc = 0;
        //        int dataset_coloum = 4 + (2 * dcount);
        //        for (int j = 2; j < dataset_coloum; j++)
        //        {
        //            for (int x = 1; x < 4; x++)
        //            {
        //                if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[x][j].ToString()))
        //                {
        //                    title = "Empty Field";
        //                    Description = "Field must be Empty";
        //                    ErrorlogExcel(x + 1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //                }
        //            }
        //        }

        //        for (int j = dataset_coloum; j <= count; j++)
        //        {
        //            Flag1 = 1;
        //            Flag3 = 1;

        //            temp++;
        //            for (int x = 1; x < rcount; x++)
        //            {
        //                if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[x][j].ToString()))
        //                {
        //                    Flag1 = 0;
        //                }
        //                else
        //                {
        //                    Flag1 = 1;
        //                    Flag2 = 0;
        //                }
        //                if (Flag1 == 0 && Flag2 == 0)
        //                {
        //                    flag = 1;
        //                    title = "Empty Field";
        //                    Description = "Field must be Empty";
        //                    ErrorlogExcel(x + 1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //                }
        //            }
        //        }

        //        for (int r = 8; r < rcount; r++)
        //        {
        //            if (string.IsNullOrEmpty(ds1.Tables[t].Rows[r][0].ToString()))
        //            {
        //                if (string.IsNullOrEmpty(ds1.Tables[t].Rows[r + 1][0].ToString()))
        //                {
        //                    row_cc++;
        //                }
        //                flag = 1;
        //                title = " non Empty Field";
        //                Description = "keyword Field can not be empty";
        //                TestStepNumber = r - 7;
        //                ErrorlogExcel(r + 1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }
        //        int set_row = 0;
        //        if (row_cc == 1)
        //        {
        //            set_row = rcount - 1;
        //        }
        //        else if (row_cc == 0)
        //        {
        //            set_row = rcount;
        //        }
        //        {
        //            set_row = rcount - row_cc - 1;
        //        }
        //        int dflag = 0;
        //        for (int k = 4; k < dataset_coloum - 1; k += 2)
        //        {
        //            for (int x = 8; x <= (rcount - row_cc - 1); x++)
        //            {
        //                if (ds1.Tables[t].Rows[x][k].ToString().ToUpper() != "SKIP")
        //                {
        //                    if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[x][k].ToString()))
        //                    {
        //                        flag = 1;
        //                        title = "skip column value";
        //                        Description = "Invalid input";
        //                        Datasetname = ds1.Tables[t].Rows[5][k].ToString();
        //                        TestStepNumber = x - 7;
        //                        objectname = ds1.Tables[t].Rows[x][1].ToString();
        //                        comment = ds1.Tables[t].Rows[x][3].ToString();
        //                        ErrorlogExcel(x + 1, k, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //                    }
        //                }
        //            }
        //            if (ds1.Tables[t].Rows[4][k].ToString().ToUpper() == "D")
        //            {
        //                dflag++;
        //            }
        //            if (dflag == dcount)
        //            {
        //                flag = 1;
        //                title = "MOde D";
        //                Description = "All the Datasets are marked with mode 'D'.Atleast one dataset must be there without marked d!! ";
        //                ErrorlogExcel(5, k, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //            }
        //        }

        //        TestStepNumber = 0;
        //        Datasetname = null;
        //        objectname = null;
        //        comment = null;






        //        TestStepNumber = 0;

        //        for (int x = set_row + 1; x < rcount; x++)
        //        {
        //            for (int k = 0; k < count; k++)
        //            {
        //                if (!string.IsNullOrEmpty(ds1.Tables[t].Rows[x][k].ToString()))
        //                {
        //                    flag = 1;
        //                    title = "Empty Field";
        //                    Description = "Field must be Empty";
        //                    ErrorlogExcel(x + 1, k, Application_name, td, title, Description, Datasetname, 0, objectname, comment, location, Testcase_name, TabName);
        //                }

        //            }
        //        }


        //        Application_name = null;


        //    }
        //    if (flag == 1)
        //    {
        //        // excel(td, LogPath);
        //        return td;
        //    }
        //    else
        //    {
        //        return td;
        //    }
        //}

        //public static class dbtable
        //{
        //    public static DataTable dt_Log = new DataTable();



        //    public static void errorlog(string ex, string action, string location, int line)
        //    {

        //        DateTime now = DateTime.Now;
        //        DataRow dr = dbtable.dt_Log.NewRow();

        //        dr["TimeStamp"] = now;
        //        dr["Message Type"] = "Debug";
        //        dr["Action"] = action;
        //        dr["SpreadSheet cell Address"] = "";
        //        dr["Validation Name"] = "";
        //        dr["Validation Fail Description"] = ex;
        //        dr["Application Name"] = "";
        //        dr["TestCase Name"] = "";
        //        dr["Dataset Name"] = "";
        //        dr["Test step Number"] = "";


        //        dr["Object Name"] = "";
        //        dr["Comment"] = "";
        //        dr["Error Description"] = line;
        //        dr["Program Location"] = location;
        //        dr["Tab Name"] = "";
        //        dbtable.dt_Log.Rows.Add(dr);


        //    }

        //}
        static DataTable ValidateExcel(List<Row> rows, SpreadsheetDocument doc, string TabName)
        {
            int flag = 0;
            string Application_name = string.Empty;
            string Testcase_name = string.Empty;
            string title = string.Empty;
            string Description = string.Empty;
            string Datasetname = string.Empty;
            int TestStepNumber = 0;
            string objectname = string.Empty;
            string comment = string.Empty;
            string location = string.Empty;
            DataTable td = new DataTable();
            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");
            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");
            td.Columns.Add("Dataset Name");
            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            if (rows.Any())
            {
                int rcount = rows.Count;
                var fristRow = rows[0].Descendants<Cell>().ToList();
                var secondRow = rows[1].Descendants<Cell>().ToList();
                var thirdRow = rows[2].Descendants<Cell>().ToList();
                var fourthRow = rows[3].Descendants<Cell>().ToList();
                var fifthRow = rows[4].Descendants<Cell>().ToList();
                var headerlst = rows[5].Descendants<Cell>().ToList();
                var seventhRow = rows[6].Descendants<Cell>().ToList();
                var eightRow = rows[7].Descendants<Cell>().ToList();
                int count = headerlst.Count - 1;

                if (fristRow[0].CellValue != null)
                {
                    if (GetValue(doc, fristRow[0]) != "Application")
                    {
                        flag = 1;
                        title = "APPLICATION TAG";
                        Description = "APPLICATION TAG NOT FOUND";
                        ErrorlogExcel(1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "APPLICATION TAG";
                    Description = "APPLICATION TAG NOT FOUND";
                    ErrorlogExcel(1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                for (int i = 1; i < fristRow.Count; i++)
                {
                    if (fristRow[i].CellValue != null)
                    {
                        Application_name = Application_name + GetValue(doc, fristRow[i]) + ",";
                    }
                    else
                    {
                        break;
                    }
                }
                if (secondRow[0].CellValue != null)
                {
                    if (GetValue(doc, secondRow[0]) != "Test Suite Name")
                    {
                        flag = 1;
                        title = "Test Suite Name TAG";
                        Description = "Test Suite Name TAG NOT FOUND";
                        ErrorlogExcel(2, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "Test Suite Name TAG";
                    Description = "Test Suite Name TAG NOT FOUND";
                    ErrorlogExcel(2, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (thirdRow[0].CellValue != null)
                {
                    if (GetValue(doc, thirdRow[0]) != "Test Case Name")
                    {
                        flag = 1;
                        title = "Test Case Name TAG";
                        Description = "Test Case Name TAG NOT FOUND";
                        ErrorlogExcel(3, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    Testcase_name = GetValue(doc, thirdRow[1]);
                }
                else
                {
                    flag = 1;
                    title = "Test Case Name TAG";
                    Description = "Test Case Name TAG NOT FOUND";
                    ErrorlogExcel(3, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (fourthRow[0].CellValue != null)
                {
                    if (GetValue(doc, fourthRow[0]) != "Test Case Description")
                    {
                        flag = 1;
                        title = "Test Case Description TAG";
                        Description = "Test Case Description TAG NOT FOUND";
                        ErrorlogExcel(4, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "Test Case Description TAG";
                    Description = "Test Case Description TAG NOT FOUND";
                    ErrorlogExcel(4, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (fifthRow[0].CellValue != null)
                {
                    if (GetValue(doc, fifthRow[0]).ToUpper().Trim() != "MODE")
                    {
                        flag = 1;
                        title = "MODE TAG";
                        Description = "MODE TAG NOT FOUND";
                        ErrorlogExcel(5, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "MODE TAG";
                    Description = "MODE TAG NOT FOUND";
                    ErrorlogExcel(5, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (headerlst[0].CellValue != null)
                {
                    if (GetValue(doc, headerlst[0]).ToUpper().Trim() != "KEYWORD")
                    {
                        flag = 1;
                        title = "KEYWORD TAG";
                        Description = "KEYWORD TAG NOT FOUND";
                        ErrorlogExcel(6, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "KEYWORD TAG";
                    Description = "KEYWORD TAG NOT FOUND";
                    ErrorlogExcel(6, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (headerlst[1].CellValue != null)
                {
                    if (GetValue(doc, headerlst[1]).ToUpper().Trim() != "OBJECT")
                    {
                        flag = 1;
                        title = "OBJECT TAG";
                        Description = "OBJECT TAG NOT FOUND";
                        ErrorlogExcel(6, 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "OBJECT TAG";
                    Description = "OBJECT TAG NOT FOUND";
                    ErrorlogExcel(6, 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (headerlst[2].CellValue != null)
                {
                    if (GetValue(doc, headerlst[2]).ToUpper().Trim() != "PARAMETERS")
                    {
                        flag = 1;
                        title = "PARAMETERS TAG";
                        Description = "PARAMETERS TAG NOT FOUND";
                        ErrorlogExcel(6, 2, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "PARAMETERS TAG";
                    Description = "PARAMETERS TAG NOT FOUND";
                    ErrorlogExcel(6, 2, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (headerlst[3].CellValue != null)
                {
                    if (GetValue(doc, headerlst[3]).ToUpper().Trim() != "COMMENT")
                    {
                        flag = 1;
                        title = "COMMENT TAG";
                        Description = "COMMENT TAG NOT FOUND";
                        ErrorlogExcel(6, 3, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "COMMENT TAG";
                    Description = "COMMENT TAG NOT FOUND";
                    ErrorlogExcel(6, 3, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                for (int i = 0; i < seventhRow.Count; i++)
                {
                    if (seventhRow[i].CellReference != null)
                    {
                        var cIndex = Convert.ToInt32(GetColumnIndex(seventhRow[i].CellReference));
                        if (cIndex < 4)
                        {
                            if (seventhRow[i].CellValue != null)
                            {
                                if (GetValue(doc, seventhRow[i]) != "")
                                {
                                    flag = 1;
                                    title = "Empty Field";
                                    Description = "Field must be Empty";
                                    ErrorlogExcel(7, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < eightRow.Count; i++)
                {
                    if (eightRow[i].CellReference != null)
                    {
                        var cIndex = Convert.ToInt32(GetColumnIndex(eightRow[i].CellReference));
                        if (cIndex < 4)
                        {
                            if (eightRow[i].CellValue != null)
                            {
                                if (GetValue(doc, eightRow[i]) != "")
                                {
                                    flag = 1;
                                    title = "Empty Field";
                                    Description = "Field must be Empty";
                                    ErrorlogExcel(7, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                        }
                    }
                }
                for (int i = 1; i < fifthRow.Count; i++)
                {
                    if (i < fifthRow.Count)
                    {
                        if (fifthRow[i].CellValue != null)
                        {
                            var cIndex = Convert.ToInt32(GetColumnIndex(fifthRow[i].CellReference));
                            if (cIndex < 4)
                            {
                                if (GetValue(doc, fifthRow[i]) != "")
                                {
                                    flag = 1;
                                    title = "Empty Field";
                                    Description = "Field must be Empty";
                                    ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                        }
                    }
                }
                var hCount = headerlst.Count;
                if (headerlst.Count % 2 != 0)
                {
                    hCount = hCount + 1;
                }
                hCount = hCount - 4;
                hCount = hCount / 2;

                int t = 4;
                int dflag = 0;
                for (int i = 1; i <= hCount; i++)
                {
                    for (int j = 1; j < fifthRow.Count; j++)
                    {
                        if (j < fifthRow.Count)
                        {
                            if (fifthRow[j].CellValue != null)
                            {
                                var cIndex = Convert.ToInt32(GetColumnIndex(fifthRow[j].CellReference));
                                if (t == cIndex)
                                {
                                    if (GetValue(doc, fifthRow[j]).ToUpper().Trim() != "X" && GetValue(doc, fifthRow[j]).ToUpper().Trim() != "D" && GetValue(doc, fifthRow[j]) != "")
                                    {
                                        flag = 1;
                                        title = "Mode";
                                        Description = "Invalid Mode";
                                        ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                    }
                                    if (GetValue(doc, fifthRow[j]).Trim() == "d" && GetValue(doc, fifthRow[j]) != "")
                                    {
                                        flag = 1;
                                        title = "Mode";
                                        Description = "Invalid Mode, Please enter D instead of d.";
                                        ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                    }
                                    if (GetValue(doc, fifthRow[j]).ToUpper().Trim() == "D")
                                    {
                                        dflag++;
                                    }
                                }
                            }
                        }
                        if (j + 1 < fifthRow.Count)
                        {
                            var cIndex = Convert.ToInt32(GetColumnIndex(fifthRow[j + 1].CellReference));
                            if (t + 1 == cIndex)
                            {
                                if (fifthRow[j + 1].CellValue != null)
                                {
                                    flag = 1;
                                    title = "Empty Field";
                                    Description = "Field must be Empty";
                                    ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                        }
                    }
                    t = t + 2;
                }

                int Flag1 = 1;
                int Flag2 = 1;
                for (int j = 1; j <= count; j++)
                {
                    if (j < fristRow.Count)
                    {
                        if (fristRow[j].CellValue != null)
                        {
                            if (GetValue(doc, fristRow[j]) != "")
                            {
                                Flag1 = 0;
                            }
                        }
                        else
                        {
                            Flag1 = 1;
                            Flag2 = 0;
                        }
                        if (Flag1 == 0 && Flag2 == 0)
                        {
                            flag = 1;
                            title = "Applicaton name";
                            Description = "Invalid application name";
                            ErrorlogExcel(1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                }
                if (headerlst.Count > 4)
                {
                    if (headerlst[4].CellValue == null)
                    {
                        title = "Data-set Name";
                        Description = "Dataset name should be there";
                        ErrorlogExcel(6, 4, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                else
                {
                    title = "Data-set Name";
                    Description = "Dataset name should be there";
                    ErrorlogExcel(6, 4, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                Flag1 = 1;
                Flag2 = 1;
                int dcount = 0;
                int Flag3 = 1;
                int Flag4 = 1;
                for (int j = 1; j <= count - 4; j += 2)
                {
                    Flag1 = 1;
                    Flag3 = 1;
                    if (headerlst[j + 3].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[j + 3]) != "")
                        {
                            Flag1 = 0;
                            dcount = dcount + 1;
                        }
                    }
                    else
                    {
                        Flag1 = 1;
                        Flag2 = 0;
                    }
                    if (Flag1 == 0 && Flag2 == 0)
                    {
                        flag = 1;
                        title = "Empty Field";
                        Description = "Field must be Empty";
                        ErrorlogExcel(6, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if ((3 + j) < seventhRow.Count && (3 + j) < headerlst.Count)
                    {
                        if (seventhRow[3 + j].CellValue != null && headerlst[j + 3].CellValue == null)
                        {
                            Flag3 = 0;
                        }
                        else
                        {
                            Flag3 = 1;
                            Flag4 = 0;
                        }
                    }
                    if (Flag3 == 0 && Flag4 == 0)
                    {
                        flag = 1;
                        title = "Empty Field";
                        Description = "Field must be Empty";
                        ErrorlogExcel(7, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                Flag1 = 1;
                Flag2 = 1;
                Flag3 = 1;
                Flag4 = 1;
                for (int j = 1; j <= count - 5; j += 2)
                {
                    Flag1 = 1;
                    Flag3 = 1;
                    if (headerlst[j + 4].CellValue != null)
                    {
                        Flag1 = 0;
                    }
                    else
                    {
                        Flag1 = 1;
                        Flag2 = 0;
                    }
                    if (Flag1 == 0 && Flag2 == 0)
                    {
                        flag = 1;
                        title = "Empty Field";
                        Description = "Field must be Empty";
                        ErrorlogExcel(6, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (j + 4 < seventhRow.Count)
                    {
                        if (seventhRow[j + 4].CellValue != null)
                        {
                            Flag3 = 0;
                        }
                        else
                        {
                            Flag3 = 1;
                            Flag4 = 0;
                        }
                    }
                    if (Flag3 == 0 && Flag4 == 0)
                    {
                        flag = 1;
                        title = "Empty Field";
                        Description = "Field must be Empty";
                        ErrorlogExcel(7, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                for (int j = 0; j < eightRow.Count; j = j + 2)
                {
                    if (eightRow[j].CellReference != null)
                    {
                        var cIndex = Convert.ToInt32(GetColumnIndex(eightRow[j].CellReference));
                        if (cIndex > 3)
                        {
                            if (eightRow[j].CellValue != null)
                            {
                                if (GetValue(doc, eightRow[j]).ToUpper().Trim() != "SKIP FLAG")
                                {
                                    flag = 1;
                                    title = "skip flag tag";
                                    Description = "skip flag tag not found";
                                    ErrorlogExcel(8, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                            else
                            {
                                flag = 1;
                                title = "skip flag tag";
                                Description = "skip flag tag not found";
                                ErrorlogExcel(8, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                        }
                    }
                    if (j + 1 < eightRow.Count)
                    {
                        if (eightRow[j + 1].CellReference != null)
                        {
                            var cIndex = Convert.ToInt32(GetColumnIndex(eightRow[j + 1].CellReference));
                            if (cIndex > 4)
                            {
                                if (eightRow[j + 1].CellValue != null)
                                {
                                    if (GetValue(doc, eightRow[j + 1]).ToUpper().Trim() != "DATA")
                                    {
                                        flag = 1;
                                        title = "Data tag";
                                        Description = "Data tag not found";
                                        ErrorlogExcel(8, 5 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                    }
                                }
                                else
                                {
                                    flag = 1;
                                    title = "Data tag";
                                    Description = "Data tag not found";
                                    ErrorlogExcel(8, 5 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                        }
                    }
                }
                Flag1 = 1;
                Flag2 = 2;
                int temp = 0;
                int row_cc = 0;
                int dataset_coloum = 4 + (2 * dcount);
                for (int j = 2; j < dataset_coloum; j++)
                {
                    for (int x = 1; x < 4; x++)
                    {
                        var celllist = rows[x].Descendants<Cell>().ToList();
                        if (j < celllist.Count)
                        {
                            if (celllist[j].CellValue != null)
                            {
                                title = "Empty Field";
                                Description = "Field must be Empty";
                                ErrorlogExcel(x + 1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                        }
                    }
                }
                for (int r = 8; r < rcount; r++)
                {
                    var celllist = rows[r].Descendants<Cell>().ToList();
                    if (celllist[0].CellValue == null)
                    {
                        if (r + 1 < rcount)
                        {
                            celllist = rows[r + 1].Descendants<Cell>().ToList();
                            if (celllist[0].CellValue == null)
                            {
                                row_cc++;
                            }
                        }
                        flag = 1;
                        title = " non Empty Field";
                        Description = "keyword Field can not be empty";
                        TestStepNumber = r - 7;
                        ErrorlogExcel(r + 1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
                int set_row = 0;
                if (row_cc == 1)
                {
                    set_row = rcount - 1;
                }
                else if (row_cc == 0)
                {
                    set_row = rcount;
                }
                {
                    set_row = rcount - row_cc - 1;
                }

                // if (dflag == dcount)
                if (dflag == hCount)
                {
                    var cVal = headerlst.Count - 2;
                    flag = 1;
                    title = "MOde D";
                    Description = "All the Datasets are marked with mode 'D'.Atleast one dataset must be there without marked d!! ";
                    ErrorlogExcel(5, cVal, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }

                for (int k = 4; k < dataset_coloum - 1; k += 2)
                {
                    for (int x = 8; x <= (rcount - row_cc - 1); x++)
                    {
                        var celllist = rows[x].Descendants<Cell>().ToList();
                        if(celllist[k].CellValue != null)
                        {
                            if (GetValue(doc, celllist[k]).ToUpper().Trim() != "SKIP")
                            {
                                if (GetValue(doc, celllist[k]) != "")
                                {
                                    flag = 1;
                                    title = "skip column value";
                                    Description = "Invalid input";

                                    Datasetname = GetValue(doc, headerlst[k]);
                                    TestStepNumber = x - 7;
                                    objectname = GetValue(doc, celllist[1]);
                                    comment = GetValue(doc, celllist[3]);
                                    ErrorlogExcel(x + 1, k, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }
                        }
                    }
                }

                TestStepNumber = 0;
                Datasetname = null;
                objectname = null;
                comment = null;
                TestStepNumber = 0;

                for (int x = set_row + 1; x < rcount; x++)
                {
                    var celllist = rows[x].Descendants<Cell>().ToList();
                    for (int k = 0; k < count; k++)
                    {
                        if (celllist[k].CellValue != null)
                        {
                            flag = 1;
                            title = "Empty Field";
                            Description = "Field must be Empty";
                            ErrorlogExcel(x + 1, k, Application_name, td, title, Description, Datasetname, 0, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                }
                Application_name = null;
            }
            return td;
        }
        public static class dbtable
        {

            public static DataTable dt_Log = new DataTable();
            public static void ErrorLog(string ex, string action, string location, int line)
            {
                DateTime now = DateTime.Now;
                DataRow dr = dbtable.dt_Log.NewRow();
                dr["TimeStamp"] = now;
                dr["Message Type"] = "Debug";
                dr["Action"] = action;
                dr["SpreadSheet cell Address"] = "";
                dr["Validation Name"] = "";
                dr["Validation Fail Description"] = ex;
                dr["Application Name"] = "";
                dr["TestCase Name"] = "";
                dr["Dataset Name"] = "";
                dr["Test step Number"] = "";
                dr["Object Name"] = "";
                dr["Comment"] = "";
                dr["Error Description"] = line;
                dr["Program Location"] = location;
                dr["Tab Name"] = "";
                dbtable.dt_Log.Rows.Add(dr);
            }
            public static int LineNo(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                return line;
            }
        }
        static void ErrorlogStoryboardExcel(int r, int c, string app_name, string project_name, string storyboard_name, string TestSuite_name, string Dependancy, string run_order, DataTable dt, string title, string Description, string Datasetname, int TestStepNumber, string objectname, string comment, string location, string Testcase_name, string TabName)
        {
            char c1;
            char c2 = ' ';

            c1 = (char)((c % 26) + 65);
            if (c >= 26)
            {
                c2 = (char)(((c / 26) - 1) + 65);
            }



            //  cell = cell + ","+(c1 + r.ToString());

            DateTime now = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["TimeStamp"] = now;
            dr["Message Type"] = " validation Error";
            dr["Action"] = "validation";

            dr["SpreadSheet cell Address"] = c2.ToString() + c1.ToString() + r.ToString();
            dr["Validation Name"] = title.ToString();
            dr["Validation Fail Description"] = Description.ToString();
            dr["Application Name"] = app_name;
            dr["Project Name"] = project_name;
            dr["StoryBoard Name"] = storyboard_name;
            dr["Test Suite Name"] = TestSuite_name;
            dr["TestCase Name"] = Testcase_name;
            dr["Test step Number"] = TestStepNumber;

            dr["Dataset Name"] = Datasetname;
            dr["Dependancy"] = Dependancy;
            dr["Run Order"] = run_order;




            dr["Object Name"] = objectname;
            dr["Comment"] = comment;
            dr["Error Description"] = "Error";
            dr["Program Location"] = "Error";
            dr["Tab Name"] = TabName;
            dt.Rows.Add(dr);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Tab Name:" + TabName + " :-" + "check input in cell:- " + c2 + c1 + r + " ==> " + "Error Description:- " + Description);




        }
        static void ErrorlogExcel(int r, int c, string app_name, DataTable dt, string title, string Description, string Datasetname, int TestStepNumber, string objectname, string comment, string location, string Testcase_name, string TabName)
        {
            char c1;
            char c2 = ' ';
            c1 = (char)((c % 26) + 65);
            if (c >= 26)
            {
                c2 = (char)(((c / 26) - 1) + 65);
            }
            DateTime now = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["TimeStamp"] = now;
            dr["Message Type"] = " validation Error";
            dr["Action"] = "validation";
            dr["SpreadSheet cell Address"] = c2.ToString() + c1.ToString() + r.ToString();
            dr["Validation Name"] = title.ToString();
            dr["Validation Fail Description"] = Description.ToString();
            dr["Application Name"] = app_name;
            dr["TestCase Name"] = Testcase_name;
            dr["Dataset Name"] = Datasetname;
            dr["Test step Number"] = TestStepNumber;
            dr["Object Name"] = objectname;
            dr["Comment"] = comment;
            dr["Error Description"] = "Error";
            dr["Program Location"] = "Error";
            dr["Tab Name"] = TabName;
            dt.Rows.Add(dr);
        }
        //public static void Excel(DataTable dv, string LogPath)

        //{
        //    XL.Application lexcelApp1 = new XL.Application();
        //    XL.Workbook lexcelWorkBook1 = null;
        //    XL._Worksheet excelWS = null;
        //    lexcelWorkBook1 = lexcelApp1.Workbooks.Add();
        //    object lmisValue = System.Reflection.Missing.Value;
        //    try
        //    {
        //        excelWS = (XL._Worksheet)lexcelWorkBook1.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //        excelWS = (Microsoft.Office.Interop.Excel.Worksheet)lexcelWorkBook1.ActiveSheet;
        //        excelWS.Name = "Error log";
        //        excelWS.Cells[1, 1].Value = "Sequence Counter";
        //        excelWS.Cells[1, 2].Value = "TimeStamp";
        //        excelWS.Cells[1, 3].Value = "Message Type";
        //        excelWS.Cells[1, 4].Value = "Action";
        //        excelWS.Cells[1, 5].Value = "SpreadSheet cell Address";
        //        excelWS.Cells[1, 6].Value = "Validation Name";
        //        excelWS.Cells[1, 7].Value = "Validation Fail Description";
        //        excelWS.Cells[1, 8].Value = "Application Name";
        //        excelWS.Cells[1, 9].Value = "TestCase Name";
        //        excelWS.Cells[1, 10].Value = "Data set Name ";
        //        excelWS.Cells[1, 11].Value = "Test step Number";
        //        excelWS.Cells[1, 12].Value = "Object Name";
        //        excelWS.Cells[1, 13].Value = "Comment";
        //        excelWS.Cells[1, 14].Value = "Error Description";
        //        excelWS.Cells[1, 15].Value = "Program Location";
        //        excelWS.Cells[1, 16].Value = "Tab Name";
        //        for (int i = 1; i <= dv.Rows.Count; i++)
        //        {
        //            excelWS.Columns.AutoFit();
        //            for (int j = 1; j <= dv.Columns.Count; j++)
        //            {
        //                excelWS.Cells[i + 1, j] = dv.Rows[i - 1][j - 1].ToString();
        //            }
        //        }
        //        string logpath = LogPath == "" ? ConfigurationManager.AppSettings["LogPath"] : LogPath;
        //        string fname = "Errorlog" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss");
        //        string strPath = System.IO.Path.Combine(logpath, fname);
        //        lexcelWorkBook1.SaveAs(strPath, XL.XlFileFormat.xlOpenXMLWorkbook, lmisValue, lmisValue, lmisValue, lmisValue, XL.XlSaveAsAccessMode.xlExclusive, lmisValue, lmisValue, lmisValue, lmisValue, lmisValue);
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //    finally
        //    {
        //        lexcelWorkBook1.Close(0);
        //        lexcelApp1.Quit();
        //    }


        //}
        //public static Columns LogReportColumnStyle()
        //{
        //    Columns columns = new Columns(
        //             new Column
        //             {
        //                 Min = 1,
        //                 Max = 1,
        //                 Width = 8,
        //                 CustomWidth = true,
        //                 BestFit = true
        //             },
        //              new Column
        //              {
        //                  Min = 2,
        //                  Max = 4,
        //                  Width = 30,
        //                  CustomWidth = true,
        //                  BestFit = true
        //              },
        //               new Column
        //               {
        //                   Min = 5,
        //                   Max = 5,
        //                   Width = 9,
        //                   CustomWidth = true,
        //                   BestFit = true
        //               },
        //                new Column
        //                {
        //                    Min = 6,
        //                    Max = 6,
        //                    Width = 30,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                },
        //                new Column
        //                {
        //                    Min = 7,
        //                    Max = 7,
        //                    Width = 60,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                },
        //                new Column
        //                {
        //                    Min = 8,
        //                    Max = 12,
        //                    Width = 30,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                },
        //                new Column
        //                {
        //                    Min = 13,
        //                    Max = 13,
        //                    Width = 15,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                },
        //                new Column
        //                {
        //                    Min = 14,
        //                    Max = 19,
        //                    Width = 30,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                },
        //                new Column
        //                {
        //                    Min = 20,
        //                    Max = 21,
        //                    Width = 35,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                },
        //                new Column
        //                {
        //                    Min = 22,
        //                    Max = 22,
        //                    Width = 65,
        //                    CustomWidth = true,
        //                    BestFit = true
        //                }
        //             );
        //    return columns;
        //}
        //public static void Excel(DataTable dv,string LogPath)
        //{
        //    using (SpreadsheetDocument document = SpreadsheetDocument.Create(LogPath, SpreadsheetDocumentType.Workbook))
        //    {
        //        WorkbookPart workbookPart = document.AddWorkbookPart();
        //        workbookPart.Workbook = new Workbook();

        //        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //        worksheetPart.Worksheet = new Worksheet();

        //        WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
        //        ExportHelper ex = new ExportHelper();
        //        stylePart.Stylesheet = ex.GenerateStylesheet();
        //        stylePart.Stylesheet.Save();
                
        //        worksheetPart.Worksheet.AppendChild((LogReportColumnStyle()));

        //        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

        //        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Log" };

        //        sheets.Append(sheet);

        //        workbookPart.Workbook.Save();
        //        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        //        Row row = new Row();

        //        row.Append(
        //            ex.ConstructCell("Sequence Counter", CellValues.String, 2),
        //            ex.ConstructCell("TimeStamp", CellValues.String, 2),
        //            ex.ConstructCell("Message Type", CellValues.String, 2),
        //            ex.ConstructCell("Action", CellValues.String, 2),
        //            ex.ConstructCell("SpreadSheet cell Address", CellValues.String, 2),
        //            ex.ConstructCell("Validation Name", CellValues.String, 2),
        //            ex.ConstructCell("Validation Fail Description", CellValues.String, 2),
        //            ex.ConstructCell("Application Name", CellValues.String, 2),
        //            ex.ConstructCell("Project Name", CellValues.String, 2),
        //            ex.ConstructCell("Storyboard Name", CellValues.String, 2),
        //            ex.ConstructCell("Test Suite Name", CellValues.String, 2),
        //            ex.ConstructCell("TestCase Name", CellValues.String, 2),
        //            ex.ConstructCell("Test Step Name", CellValues.String, 2),
        //            ex.ConstructCell("Data Set Name", CellValues.String, 2),
        //            ex.ConstructCell("Dependancy", CellValues.String, 2),
        //            ex.ConstructCell("Run Order", CellValues.String, 2),
        //            ex.ConstructCell("Object Name", CellValues.String, 2),
        //            ex.ConstructCell("Comment", CellValues.String, 2),
        //            ex.ConstructCell("Error Description", CellValues.String, 2),
        //            ex.ConstructCell("Program Location", CellValues.String, 2),
        //            ex.ConstructCell("Tab Name", CellValues.String, 2),
        //            ex.ConstructCell("General", CellValues.String, 2));
        //        sheetData.AppendChild(row);
        //        Row drow = new Row();
        //        if (dv.Rows.Count > 0)
        //        {
        //            for (int i = 1; i <= dv.Rows.Count; i++)
        //            {
        //                drow = new Row();
        //                for (int j = 1; j <= dv.Columns.Count; j++)
        //                {
        //                    drow.Append(ex.ConstructCell(dv.Rows[i - 1][j - 1] == null ? "" : dv.Rows[i - 1][j - 1].ToString(), CellValues.String, 1));

        //                }
        //                sheetData.AppendChild(drow);
        //            }
        //        }
        //        worksheetPart.Worksheet.Save();
        //    }
        //}
        //public static string ImportStoryBoard(string pFilePath, string lstrConn, string schema, string logpath, string LoginName)
        //{
        //    var lValidationLogFeedProcessId = 0;
        //    var lreturnpath = "";
        //    string lOperation = "INSERT";
        //    string lCreatedBy = LoginName;
        //    string lStatus = "INPROCESS";
        //    string lOperationobject = "INSERT";
        //    string lCREATEDBY = LoginName;
        //    string lStatusobject = "INPROCESS";
        //    int lFeedProcessDetailsId = 0;
        //    int lDEFAULT_FEEDPROCESSDETAIL_ID = 0;
        //    string lFileType = "STORYBOARD";
        //    string fileName = Import_Storyboard;
        //    string lFileName = Path.GetFileName(pFilePath);
        //    bool lFinalResult = true;
        //    int lFeedProcessId = FeedProcessHelper.FeedProcess(0, lOperation, lCreatedBy, lStatus, lstrConn, schema);
        //    lFeedProcessDetailsId = FeedProcessHelper.FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
        //    try
        //    {
        //        string lResultImportStoryBoard = string.Empty;
        //        lResultImportStoryBoard = ImportStoryboardExcel(pFilePath, lFeedProcessDetailsId, logpath, lstrConn, schema);

        //        System.Threading.Thread.Sleep(200);
        //        if (lResultImportStoryBoard == "0")
        //        {
        //            lOperationobject = "UPDATE";
        //            lStatusobject = "COMPLETED";
        //            int lSuccess1 = FeedProcessHelper.FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
        //            //return "";
        //        }
        //        if (dbtable.dt_Log.Rows.Count > 0)
        //        {

        //            lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
        //            return lreturnpath;
        //        }
        //        lFinalResult = FeedProcessHelper.MappingValidation(lFeedProcessId, lstrConn, schema);
        //        System.Threading.Thread.Sleep(200);
        //        DataTable dt = new DataTable();
        //        dt = FeedProcessHelper.DbExcel(lFeedProcessId, lstrConn, schema);
        //        dbtable.dt_Log.AcceptChanges();
        //        System.Threading.Thread.Sleep(200);
        //        if (dt.Rows.Count != 0)
        //        {
        //            dbtable.dt_Log.Merge(dt);
        //            try
        //            {
        //                dbtable.dt_Log.Columns.RemoveAt(17);
        //                dbtable.dt_Log.Columns.RemoveAt(16);
        //            }
        //            catch
        //            {

        //            }
        //            System.Threading.Thread.Sleep(200);
        //        }

        //        if (lFinalResult && dbtable.dt_Log.Rows.Count <= 6)
        //        {
        //            lFinalResult = FeedProcessHelper.DatawareHouseMapping(lFeedProcessId, 1, lstrConn, schema);
        //            System.Threading.Thread.Sleep(200);

        //            if (lFinalResult)
        //            {
        //                lOperation = "UPDATE";
        //                lCreatedBy = LoginName;
        //                lStatus = (lFinalResult) ? "COMPLETED" : "ERROR";
        //                int lSuccess = FeedProcessHelper.FeedProcess(lFeedProcessId, lOperation, lCreatedBy, lStatus, lstrConn, schema);
        //                string lFileNameExport = "LOGREPORT-" + Path.GetFileName(pFilePath);
        //                string lExportLogFile = logpath;
        //                //return "1";
        //            }
        //        }
        //        else
        //        {
        //            lValidationLogFeedProcessId = lFeedProcessId;
        //            lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
        //        }
        //        // Console.ForegroundColor = ConsoleColor.White;
        //        // Console.WriteLine("\nSpreadsheet format validation completed at {0}..", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));


        //        return lreturnpath;

        //    }
        //    catch (Exception ex)
        //    {
        //        //int line;
        //        //string msg = ex.Message;
        //        /// line = dbtable.lineNo(ex);
        //        // dbtable.errorlog(msg, "Import testcase excel", SomeGlobalVariables.functionName, line);
        //        throw new Exception("Error from:importTestCaseExcel1" + ex.Message);
        //    }

        //}

        public static string ImportStoryBoard(string pFilePath, string lstrConn, string schema, string logpath, string LoginName)
        {
            var lValidationLogFeedProcessId = 0;
            var lreturnpath = "";
            string lOperation = "INSERT";
            string lCreatedBy = LoginName;
            string lStatus = "INPROCESS";
            string lOperationobject = "INSERT";
            string lCREATEDBY = LoginName;
            string lStatusobject = "INPROCESS";
            int lFeedProcessDetailsId = 0;
            int lDEFAULT_FEEDPROCESSDETAIL_ID = 0;
            string lFileType = "STORYBOARD";
            string fileName = Import_Storyboard;
            string lFileName = Path.GetFileName(pFilePath);
            bool lFinalResult = true;
            int lFeedProcessId = FeedProcessHelper.FeedProcess(0, lOperation, lCreatedBy, lStatus, lstrConn, schema);
            lFeedProcessDetailsId = FeedProcessHelper.FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
            try
            {
                bool lResultImportStoryBoard = false;
                lResultImportStoryBoard = ImportStoryboardExcel(pFilePath, lFeedProcessDetailsId, logpath, lstrConn, schema);

                System.Threading.Thread.Sleep(200);
                if (lResultImportStoryBoard == true)
                {
                    lOperationobject = "UPDATE";
                    lStatusobject = "COMPLETED";
                    int lSuccess1 = FeedProcessHelper.FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
                }
                if (dbtable.dt_Log.Rows.Count > 0)
                {
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
                    return lreturnpath;
                }
                lFinalResult = FeedProcessHelper.MappingValidation(lFeedProcessId, lstrConn, schema);
                System.Threading.Thread.Sleep(200);
                DataTable dt = new DataTable();
                dt = FeedProcessHelper.DbExcel(lFeedProcessId, lstrConn, schema);
                dbtable.dt_Log.AcceptChanges();
                System.Threading.Thread.Sleep(200);
                if (dt.Rows.Count != 0)
                {
                    dbtable.dt_Log.Merge(dt);
                    try
                    {
                        dbtable.dt_Log.Columns.RemoveAt(17);
                        dbtable.dt_Log.Columns.RemoveAt(16);
                    }
                    catch
                    {
                    }
                    System.Threading.Thread.Sleep(200);
                }

                if (lFinalResult && dbtable.dt_Log.Rows.Count <= 6)
                {
                    lFinalResult = FeedProcessHelper.DatawareHouseMapping(lFeedProcessId, 1, lstrConn, schema);
                    System.Threading.Thread.Sleep(200);

                    if (lFinalResult)
                    {
                        lOperation = "UPDATE";
                        lCreatedBy = LoginName;
                        lStatus = (lFinalResult) ? "COMPLETED" : "ERROR";
                        int lSuccess = FeedProcessHelper.FeedProcess(lFeedProcessId, lOperation, lCreatedBy, lStatus, lstrConn, schema);
                        string lFileNameExport = "LOGREPORT-" + Path.GetFileName(pFilePath);
                        string lExportLogFile = logpath;
                    }
                }
                else
                {
                    lValidationLogFeedProcessId = lFeedProcessId;
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
                }
                return lreturnpath;
            }
            catch (Exception ex)
            {
                WriteMessage("Error :" + ex.ToString(), logFilePath, "Storyboard", Import_Storyboard);
                throw new Exception("Error from:importTestCaseExcel1" + ex.Message);
            }
        }
        //public static string ImportStoryboardExcel(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string lstrConn, string schema)
        //{
        //    DataSet ds = ImportExcelXLS(pFilePath, true);
        //    if (ds.Tables.Count == 0)
        //    {
        //        // dbtable.errorlog("File is opened in bckground", "", "", 0);
        //        // Console.ForegroundColor = ConsoleColor.White;
        //        // Console.WriteLine("File is opened in bckground.... Please close it before importing..");
        //        return "File is opened in bckground.... Please close it before importing!!";
        //    }
        //    DataTable dtImport = null;
        //    string lProjectName = string.Empty, lProjectDesc = string.Empty, lStoryBoardName = string.Empty;
        //    string lApplication = string.Empty;
        //    //int flag = 0;
        //    DataTable table = new DataTable();
        //    string lNewApplicationName = string.Empty;
        //    table = ValidateStoryboard(ds, LogPath);
        //    dbtable.dt_Log.Merge(table);
        //    for (int i = 0; i < ds.Tables.Count; i++)
        //    {

        //        string SheetName = ds.Tables[i].TableName.ToString();
        //        //if (!string.IsNullOrEmpty(ds.Tables[i].Rows[0][0].ToString()))
        //        //{


        //        lApplication = "";
        //        for (int m = 1; m < ds.Tables[i].Columns.Count; m++)
        //        {

        //            lNewApplicationName = (string)(ds.Tables[i].Rows[0][m].ToString());
        //            if (!string.IsNullOrEmpty(lNewApplicationName))
        //            {
        //                if (string.IsNullOrEmpty(lApplication))
        //                {
        //                    lApplication = lNewApplicationName;
        //                }
        //                else
        //                {
        //                    lApplication += ',' + lNewApplicationName;
        //                }
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }


        //        lProjectName = ds.Tables[i].Rows[1][1].ToString();
        //        lProjectDesc = ds.Tables[i].Rows[2][1].ToString();
        //        lStoryBoardName = ds.Tables[i].Rows[3][1].ToString();



        //        var arrayNames = (from DataColumn x in ds.Tables[i].Columns
        //                              //where ds.Tables[i].Rows[5][x.ColumnName].ToString() != ""
        //                          select x.ColumnName).ToArray();

        //        var columnname = (from DataColumn x in ds.Tables[i].Columns
        //                          where ds.Tables[i].Rows[4][x.ColumnName].ToString() == ""
        //                          select x.ColumnName).ToArray();

        //        dtImport = new DataTable();
        //        dtImport.Columns.Add("RUNORDER");
        //        dtImport.Columns.Add("APPLICATIONNAME");
        //        dtImport.Columns.Add("PROJECTNAME");
        //        dtImport.Columns.Add("PROJECTDETAIL");
        //        dtImport.Columns.Add("STORYBOARDNAME");
        //        dtImport.Columns.Add("ACTIONNAME");
        //        dtImport.Columns.Add("STEPNAME");
        //        dtImport.Columns.Add("SUITENAME");
        //        dtImport.Columns.Add("CASENAME");
        //        dtImport.Columns.Add("DATASETNAME");
        //        dtImport.Columns.Add("DEPENDENCY");
        //        dtImport.Columns.Add("FEEDPROCESSDETAILID");
        //        dtImport.Columns.Add("TABNAME");




        //        int l = -1;
        //        foreach (DataRow dr1 in ds.Tables[i].Rows)
        //        {
        //            l++;
        //            if (l > 4)
        //            {

        //                DataRow dr = dtImport.NewRow();
        //                dr["RUNORDER"] = Convert.ToString(dr1[0].ToString());
        //                dr["APPLICATIONNAME"] = lApplication;
        //                dr["PROJECTNAME"] = lProjectName;

        //                dr["PROJECTDETAIL"] = lProjectDesc;
        //                dr["STORYBOARDNAME"] = lStoryBoardName;
        //                dr["ACTIONNAME"] = Convert.ToString(dr1[1].ToString());
        //                dr["STEPNAME"] = Convert.ToString(dr1[2].ToString());

        //                dr["SUITENAME"] = Convert.ToString(dr1[3].ToString());
        //                dr["CASENAME"] = Convert.ToString(dr1[4].ToString());
        //                dr["DATASETNAME"] = Convert.ToString(dr1[5].ToString());

        //                dr["DEPENDENCY"] = Convert.ToString(dr1[6].ToString());

        //                dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
        //                dr["TABNAME"] = ds.Tables[i].TableName;

        //                dtImport.Rows.Add(dr);
        //            }







        //        }



        //        if (dtImport.Rows.Count > 0)
        //        {
        //            OracleTransaction ltransaction;
        //            OracleConnection lconnection = GetOracleConnection(lstrConn);
        //            lconnection.Open();
        //            ltransaction = lconnection.BeginTransaction();

        //            OracleCommand lcmd;
        //            lcmd = lconnection.CreateCommand();
        //            lcmd.Transaction = ltransaction;

        //            //The name of the Procedure responsible for inserting the data in the table.
        //            List<DataRow> list = dtImport.AsEnumerable().ToList();
        //            lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_StoryBoard";
        //            //lcmd.CommandText = "insert into TBLSTGTESTCASE(TESTSUITENAME, TESTCASENAME, TESTCASEDESCRIPTION, DATASETMODE, KEYWORD, OBJECT, PARAMETER, COMMENTS, DATASETNAME, DATASETVALUE, ROWNUMBER, FEEDPROCESSDETAILID, TABNAME, APPLICATION, ID, CREATEDON)"
        //            //+ "SELECT :1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13,:14, (SELECT MAX(ID)), (SELECT SYSDATE FROM DUAL) from dual";

        //            string[] lRUNORDER = dtImport.AsEnumerable().Select(r => r.Field<string>("RUNORDER")).ToArray();
        //            //  Decimal[] lRUNORDER = pRUNORDER.Select(Decimal.Parse).ToArray();
        //            string[] lAPPLICATIONNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("APPLICATIONNAME")).ToArray();
        //            string[] lPROJECTNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("PROJECTNAME")).ToArray();
        //            string[] lPROJECTDETAIL = dtImport.AsEnumerable().Select(r => r.Field<string>("PROJECTDETAIL")).ToArray();
        //            string[] lSTORYBOARDNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("STORYBOARDNAME")).ToArray();
        //            string[] lACTIONNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("ACTIONNAME")).ToArray();
        //            string[] lSTEPNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("STEPNAME")).ToArray();
        //            string[] lSUITENAME = dtImport.AsEnumerable().Select(r => r.Field<string>("SUITENAME")).ToArray();
        //            string[] lCASENAME = dtImport.AsEnumerable().Select(r => r.Field<string>("CASENAME")).ToArray();
        //            string[] lDATASETNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
        //            string[] lDEPENDENCY = dtImport.AsEnumerable().Select(r => r.Field<string>("DEPENDENCY")).ToArray();
        //            string[] lFEEDPROCESSDETAILID = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
        //            //   Decimal[] lFEEDPROCESSDETAILID = rFEEDPROCESSDETAILID.Select(Decimal.Parse).ToArray();
        //            string[] lTABNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("TABNAME")).ToArray();


        //            if (lDATASETNAME.Length != 0)
        //            {
        //                OracleParameter RUNORDER = new OracleParameter();
        //                RUNORDER.OracleDbType = OracleDbType.Varchar2;
        //                RUNORDER.Value = lRUNORDER;

        //                OracleParameter APPLICATIONNAME = new OracleParameter();
        //                APPLICATIONNAME.OracleDbType = OracleDbType.Varchar2;
        //                APPLICATIONNAME.Value = lAPPLICATIONNAME;

        //                OracleParameter PROJECTNAME = new OracleParameter();
        //                PROJECTNAME.OracleDbType = OracleDbType.Varchar2;
        //                PROJECTNAME.Value = lPROJECTNAME;

        //                OracleParameter PROJECTDETAIL = new OracleParameter();
        //                PROJECTDETAIL.OracleDbType = OracleDbType.Varchar2;
        //                PROJECTDETAIL.Value = lPROJECTDETAIL;

        //                OracleParameter STORYBOARDNAME = new OracleParameter();
        //                STORYBOARDNAME.OracleDbType = OracleDbType.Varchar2;
        //                STORYBOARDNAME.Value = lSTORYBOARDNAME;

        //                OracleParameter ACTIONNAME = new OracleParameter();
        //                ACTIONNAME.OracleDbType = OracleDbType.Varchar2;
        //                ACTIONNAME.Value = lACTIONNAME;

        //                OracleParameter STEPNAME = new OracleParameter();
        //                STEPNAME.OracleDbType = OracleDbType.Varchar2;
        //                STEPNAME.Value = lSTEPNAME;

        //                OracleParameter SUITENAME = new OracleParameter();
        //                SUITENAME.OracleDbType = OracleDbType.Varchar2;
        //                SUITENAME.Value = lSUITENAME;

        //                OracleParameter CASENAME = new OracleParameter();
        //                CASENAME.OracleDbType = OracleDbType.Varchar2;
        //                CASENAME.Value = lCASENAME;

        //                OracleParameter DATASETNAME = new OracleParameter();
        //                DATASETNAME.OracleDbType = OracleDbType.Varchar2;
        //                DATASETNAME.Value = lDATASETNAME;

        //                OracleParameter DEPENDENCY = new OracleParameter();
        //                DEPENDENCY.OracleDbType = OracleDbType.Varchar2;
        //                DEPENDENCY.Value = lDEPENDENCY;


        //                OracleParameter FEEDPROCESSDETAILID = new OracleParameter();
        //                FEEDPROCESSDETAILID.OracleDbType = OracleDbType.Varchar2;
        //                FEEDPROCESSDETAILID.Value = lFEEDPROCESSDETAILID;

        //                OracleParameter TABNAME = new OracleParameter();
        //                TABNAME.OracleDbType = OracleDbType.Varchar2;
        //                TABNAME.Value = lTABNAME;


        //                lcmd.ArrayBindCount = lPROJECTNAME.Length;

        //                lcmd.Parameters.Add(RUNORDER);
        //                lcmd.Parameters.Add(APPLICATIONNAME);
        //                lcmd.Parameters.Add(PROJECTNAME);
        //                lcmd.Parameters.Add(PROJECTDETAIL);
        //                lcmd.Parameters.Add(STORYBOARDNAME);
        //                lcmd.Parameters.Add(ACTIONNAME);
        //                lcmd.Parameters.Add(STEPNAME);
        //                lcmd.Parameters.Add(SUITENAME);
        //                lcmd.Parameters.Add(CASENAME);
        //                lcmd.Parameters.Add(DATASETNAME);
        //                lcmd.Parameters.Add(DEPENDENCY);
        //                lcmd.Parameters.Add(FEEDPROCESSDETAILID);
        //                lcmd.Parameters.Add(TABNAME);


        //                lcmd.CommandType = CommandType.StoredProcedure;
        //                //dtImport.TableName = "DETAILS";
        //                //OracleParameter parm1 = new OracleParameter("DETAILS", OracleDbType.RefCursor);
        //                //parm1.Value = dtImport;
        //                //parm1.Direction = ParameterDirection.Input;
        //                //lcmd.Parameters.Add(parm1);
        //                try
        //                {
        //                    lcmd.ExecuteNonQuery();
        //                }
        //                catch (Exception lex)
        //                {
        //                    ltransaction.Rollback();

        //                    throw new Exception(lex.Message);
        //                }
        //                ltransaction.Commit();
        //            }

        //            //else
        //            //{
        //            //    return "No testcase found in Sheet : " + SheetName;
        //            //}

        //            //}

        //        }
        //    }
        //    return "0";
        //}

        public static bool ImportStoryboardExcel(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string lstrConn, string schema)
        {
            try
            {
                bool lResult = false;
                string lProjectName = string.Empty, lProjectDesc = string.Empty, lStoryBoardName = string.Empty;
                string lApplication = string.Empty, lNewApplicationName = string.Empty;
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();

                    if (worksheetcount > 0)
                    {
                        DataTable dtImport = new DataTable();
                        dtImport.Columns.Add("RUNORDER");
                        dtImport.Columns.Add("APPLICATIONNAME");
                        dtImport.Columns.Add("PROJECTNAME");
                        dtImport.Columns.Add("PROJECTDETAIL");
                        dtImport.Columns.Add("STORYBOARDNAME");
                        dtImport.Columns.Add("ACTIONNAME");
                        dtImport.Columns.Add("STEPNAME");
                        dtImport.Columns.Add("SUITENAME");
                        dtImport.Columns.Add("CASENAME");
                        dtImport.Columns.Add("DATASETNAME");
                        dtImport.Columns.Add("DEPENDENCY");
                        dtImport.Columns.Add("FEEDPROCESSDETAILID");
                        dtImport.Columns.Add("TABNAME");
                        dtImport.AcceptChanges();
                        for (int i = 0; i < worksheetcount; i++)
                        {
                            Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(i);
                            Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                            List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                            var tabName = mysheet.Name;
                            DataTable dt = new DataTable();

                            dt = ValidateStoryboard(rows, doc, tabName);
                            if (dt.Rows.Count != 0)
                            {
                                lResult = false;
                                dbtable.dt_Log.Merge(dt);
                                return lResult;
                            }
                            var fristRow = rows[0].Descendants<Cell>().ToList();
                            var secondRow = rows[1].Descendants<Cell>().ToList();
                            var thirdRow = rows[2].Descendants<Cell>().ToList();
                            var fourthRow = rows[3].Descendants<Cell>().ToList();

                            //lApplication = fristRow[1].CellValue == null ? "" : GetValue(doc, fristRow[1]);
                            lApplication = "";
                            for (int m = 1; m < fristRow.Count; m++)
                            {
                                if (fristRow[m].CellValue != null)
                                {
                                    lNewApplicationName = GetValue(doc, fristRow[m]);
                                    if (string.IsNullOrEmpty(lApplication))
                                    {
                                        lApplication = lNewApplicationName;
                                    }
                                    else
                                    {
                                        lApplication += ',' + lNewApplicationName;
                                    }
                                }
                                else
                                    break;
                            }

                            lProjectName = secondRow[1].CellValue == null ? "" : GetValue(doc, secondRow[1]);
                            lProjectDesc = thirdRow[1].CellValue == null ? "" : GetValue(doc, thirdRow[1]);
                            lStoryBoardName = fourthRow[1].CellValue == null ? "" : GetValue(doc, fourthRow[1]);

                            for (int j = 5; j < rows.Count; j++)
                            {
                                var celllst = rows[j].Descendants<Cell>().ToList();
                                DataRow dr = dtImport.NewRow();
                                dr["RUNORDER"] = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                                dr["APPLICATIONNAME"] = lApplication;
                                dr["PROJECTNAME"] = lProjectName;
                                dr["PROJECTDETAIL"] = lProjectDesc;
                                dr["STORYBOARDNAME"] = lStoryBoardName;

                                if (celllst.Count < 7)
                                {
                                    dr["ACTIONNAME"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]);
                                    dr["STEPNAME"] = "";
                                    dr["SUITENAME"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]);
                                    dr["CASENAME"] = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                                    dr["DATASETNAME"] = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                                    dr["DEPENDENCY"] = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                                }
                                else
                                {
                                    dr["ACTIONNAME"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]);
                                    dr["STEPNAME"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]);
                                    dr["SUITENAME"] = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                                    dr["CASENAME"] = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                                    dr["DATASETNAME"] = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                                    dr["DEPENDENCY"] = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                                }
                                dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                dr["TABNAME"] = tabName;

                                dtImport.Rows.Add(dr);
                                dtImport.AcceptChanges();
                            }
                        }

                        if (dtImport.Rows.Count > 0)
                        {
                            OracleTransaction ltransaction;
                            OracleConnection lconnection = GetOracleConnection(lstrConn);
                            lconnection.Open();
                            ltransaction = lconnection.BeginTransaction();

                            OracleCommand lcmd;
                            lcmd = lconnection.CreateCommand();
                            lcmd.Transaction = ltransaction;

                            List<DataRow> list = dtImport.AsEnumerable().ToList();
                            lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_StoryBoard";

                            string[] lRUNORDER = dtImport.AsEnumerable().Select(r => r.Field<string>("RUNORDER")).ToArray();
                            string[] lAPPLICATIONNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("APPLICATIONNAME")).ToArray();
                            string[] lPROJECTNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("PROJECTNAME")).ToArray();
                            string[] lPROJECTDETAIL = dtImport.AsEnumerable().Select(r => r.Field<string>("PROJECTDETAIL")).ToArray();
                            string[] lSTORYBOARDNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("STORYBOARDNAME")).ToArray();
                            string[] lACTIONNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("ACTIONNAME")).ToArray();
                            string[] lSTEPNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("STEPNAME")).ToArray();
                            string[] lSUITENAME = dtImport.AsEnumerable().Select(r => r.Field<string>("SUITENAME")).ToArray();
                            string[] lCASENAME = dtImport.AsEnumerable().Select(r => r.Field<string>("CASENAME")).ToArray();
                            string[] lDATASETNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                            string[] lDEPENDENCY = dtImport.AsEnumerable().Select(r => r.Field<string>("DEPENDENCY")).ToArray();
                            string[] lFEEDPROCESSDETAILID = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                            string[] lTABNAME = dtImport.AsEnumerable().Select(r => r.Field<string>("TABNAME")).ToArray();


                            if (lDATASETNAME.Length != 0)
                            {
                                OracleParameter RUNORDER = new OracleParameter();
                                RUNORDER.OracleDbType = OracleDbType.Varchar2;
                                RUNORDER.Value = lRUNORDER;

                                OracleParameter APPLICATIONNAME = new OracleParameter();
                                APPLICATIONNAME.OracleDbType = OracleDbType.Varchar2;
                                APPLICATIONNAME.Value = lAPPLICATIONNAME;

                                OracleParameter PROJECTNAME = new OracleParameter();
                                PROJECTNAME.OracleDbType = OracleDbType.Varchar2;
                                PROJECTNAME.Value = lPROJECTNAME;

                                OracleParameter PROJECTDETAIL = new OracleParameter();
                                PROJECTDETAIL.OracleDbType = OracleDbType.Varchar2;
                                PROJECTDETAIL.Value = lPROJECTDETAIL;

                                OracleParameter STORYBOARDNAME = new OracleParameter();
                                STORYBOARDNAME.OracleDbType = OracleDbType.Varchar2;
                                STORYBOARDNAME.Value = lSTORYBOARDNAME;

                                OracleParameter ACTIONNAME = new OracleParameter();
                                ACTIONNAME.OracleDbType = OracleDbType.Varchar2;
                                ACTIONNAME.Value = lACTIONNAME;

                                OracleParameter STEPNAME = new OracleParameter();
                                STEPNAME.OracleDbType = OracleDbType.Varchar2;
                                STEPNAME.Value = lSTEPNAME;

                                OracleParameter SUITENAME = new OracleParameter();
                                SUITENAME.OracleDbType = OracleDbType.Varchar2;
                                SUITENAME.Value = lSUITENAME;

                                OracleParameter CASENAME = new OracleParameter();
                                CASENAME.OracleDbType = OracleDbType.Varchar2;
                                CASENAME.Value = lCASENAME;

                                OracleParameter DATASETNAME = new OracleParameter();
                                DATASETNAME.OracleDbType = OracleDbType.Varchar2;
                                DATASETNAME.Value = lDATASETNAME;

                                OracleParameter DEPENDENCY = new OracleParameter();
                                DEPENDENCY.OracleDbType = OracleDbType.Varchar2;
                                DEPENDENCY.Value = lDEPENDENCY;


                                OracleParameter FEEDPROCESSDETAILID = new OracleParameter();
                                FEEDPROCESSDETAILID.OracleDbType = OracleDbType.Varchar2;
                                FEEDPROCESSDETAILID.Value = lFEEDPROCESSDETAILID;

                                OracleParameter TABNAME = new OracleParameter();
                                TABNAME.OracleDbType = OracleDbType.Varchar2;
                                TABNAME.Value = lTABNAME;

                                lcmd.ArrayBindCount = lPROJECTNAME.Length;

                                lcmd.Parameters.Add(RUNORDER);
                                lcmd.Parameters.Add(APPLICATIONNAME);
                                lcmd.Parameters.Add(PROJECTNAME);
                                lcmd.Parameters.Add(PROJECTDETAIL);
                                lcmd.Parameters.Add(STORYBOARDNAME);
                                lcmd.Parameters.Add(ACTIONNAME);
                                lcmd.Parameters.Add(STEPNAME);
                                lcmd.Parameters.Add(SUITENAME);
                                lcmd.Parameters.Add(CASENAME);
                                lcmd.Parameters.Add(DATASETNAME);
                                lcmd.Parameters.Add(DEPENDENCY);
                                lcmd.Parameters.Add(FEEDPROCESSDETAILID);
                                lcmd.Parameters.Add(TABNAME);


                                lcmd.CommandType = CommandType.StoredProcedure;

                                try
                                {
                                    lcmd.ExecuteNonQuery();
                                    lResult = true;
                                }
                                catch (Exception lex)
                                {
                                    WriteMessage("Error :" + lex.ToString(), logFilePath, "Storyboard", Import_Storyboard);
                                    lResult = false;
                                    ltransaction.Rollback();
                                    throw new Exception(lex.Message);
                                }
                                ltransaction.Commit();
                            }
                        }
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Error from:importExcelStoryboard" + ex.Message);
            }
        }

        static DataTable ValidateStoryboard(List<Row> rows, SpreadsheetDocument doc, string TabName)
        {
            int flag = 0;

            string Application_name = string.Empty;
            string project_name = string.Empty;
            string storyboard_name = string.Empty;
            string run_order = string.Empty;
            string Dependancy = string.Empty;

            string Datasetname = string.Empty;
            int TestStepNumber = 0;
            string TestSuite_name = string.Empty;
            string TestCase_name = string.Empty;
            string DataSet_name = string.Empty;
            string title = string.Empty;
            string Description = string.Empty;
            string objectname = string.Empty;
            string comment = string.Empty;
            string location = string.Empty;
            int step_count = 0;
            int colCount = default(int);

            DataTable td = new DataTable();

            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");
            td.Columns.Add("Test Suite Name");

            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");
            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");

            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            if (rows.Any())
            {
                var fristRow = rows[0].Descendants<Cell>().ToList();
                var secondRow = rows[1].Descendants<Cell>().ToList();
                var thirdRow = rows[2].Descendants<Cell>().ToList();
                var fourthRow = rows[3].Descendants<Cell>().ToList();
                var headerlst = rows[4].Descendants<Cell>().ToList();

                if (fristRow[0].CellValue != null)
                {
                    if (GetValue(doc, fristRow[0]) != "Application")
                    {
                        flag = 1;
                        title = "APPLICATION TAG";
                        Description = "APPLICATION TAG NOT FOUND";
                        ErrorlogStoryboardExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "APPLICATION TAG";
                    Description = "APPLICATION TAG NOT FOUND";
                    ErrorlogStoryboardExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                if (fristRow[1].CellValue == null)
                {
                    flag = 1;
                    title = " non Empty Field";
                    Description = "Field should not be Empty";
                    ErrorlogStoryboardExcel(1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                //Application_name = GetValue(doc, fristRow[1]);
                //if (fristRow.Count > 2)
                //{
                //    for (int j = 2; j < fristRow.Count; j++)
                //    {
                //        flag = 1;
                //        title = "Empty Field";
                //        Description = "Field should be Empty";
                //        ErrorlogStoryboardExcel(1, Convert.ToInt32(GetColumnIndex(fristRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                //    }
                //}
                for (int i = 1; i < fristRow.Count; i++)
                {
                    if (fristRow[i].CellValue != null)
                    {
                        Application_name = Application_name + GetValue(doc, fristRow[i]) + ",";
                    }
                    else
                        break;
                }
                if (secondRow[0].CellValue != null)
                {
                    if (GetValue(doc, secondRow[0]) != "Project Name")
                    {
                        flag = 1;
                        title = "Project Name TAG";
                        Description = "Project Name TAG NOT FOUND";
                        ErrorlogStoryboardExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "Project Name TAG";
                    Description = "Project Name TAG NOT FOUND";
                    ErrorlogStoryboardExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                if (secondRow[1].CellValue == null)
                {
                    flag = 1;
                    title = " non Empty Field";
                    Description = "Field should not be Empty";
                    ErrorlogStoryboardExcel(2, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                project_name = GetValue(doc, secondRow[1]);
                if (secondRow.Count > 2)
                {
                    for (int j = 2; j < secondRow.Count; j++)
                    {
                        if (secondRow[j].CellValue != null)
                        {
                            if (GetValue(doc, secondRow[j]) != "")
                            {
                                flag = 1;
                                title = "Empty Field";
                                Description = "Field should be Empty";
                                ErrorlogStoryboardExcel(2, Convert.ToInt32(GetColumnIndex(secondRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                            }
                        }
                    }
                }

                if (thirdRow[0].CellValue != null)
                {
                    if (GetValue(doc, thirdRow[0]) != "Project Description")
                    {
                        flag = 1;
                        title = "Project Description TAG";
                        Description = "Project Description TAG NOT FOUND";
                        ErrorlogStoryboardExcel(3, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "Project Description TAG";
                    Description = "Project Description TAG NOT FOUND";
                    ErrorlogStoryboardExcel(3, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                if (thirdRow[1].CellValue == null)
                {
                    flag = 1;
                    title = " non Empty Field";
                    Description = "Field should not be Empty";
                    ErrorlogStoryboardExcel(3, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                if (thirdRow.Count > 2)
                {
                    for (int j = 2; j < thirdRow.Count; j++)
                    {
                        if (thirdRow[j].CellValue != null)
                        {
                            if (GetValue(doc, thirdRow[j]) != "")
                            {
                                flag = 1;
                                title = "Empty Field";
                                Description = "Field should be Empty";
                                ErrorlogStoryboardExcel(3, Convert.ToInt32(GetColumnIndex(thirdRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                            }
                        }
                    }
                }

                if (fourthRow[0].CellValue != null)
                {
                    if (GetValue(doc, fourthRow[0]) != "Storyboard Name")
                    {
                        flag = 1;
                        title = "Storyboard Name TAG";
                        Description = "Storyboard Name TAG NOT FOUND";
                        ErrorlogStoryboardExcel(4, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }
                }
                else
                {
                    flag = 1;
                    title = "Storyboard Name TAG";
                    Description = "Storyboard Name TAG NOT FOUND";
                    ErrorlogStoryboardExcel(4, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                if (fourthRow[1].CellValue == null)
                {
                    flag = 1;
                    title = " non Empty Field";
                    Description = "Field should not be Empty";
                    ErrorlogStoryboardExcel(4, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                }
                storyboard_name = GetValue(doc, fourthRow[1]);
                if (fourthRow.Count > 2)
                {
                    for (int j = 2; j < fourthRow.Count; j++)
                    {
                        if (fourthRow[j].CellValue != null)
                        {
                            if (GetValue(doc, fourthRow[j]) != "")
                            {
                                flag = 1;
                                title = "Empty Field";
                                Description = "Field should be Empty";
                                ErrorlogStoryboardExcel(4, Convert.ToInt32(GetColumnIndex(fourthRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                            }
                        }
                    }
                }

                if (headerlst.Any())
                {
                    if (headerlst[0].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[0]) != "Row Number")
                        {
                            flag = 1;
                            title = "Order TAG";
                            Description = "Order TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                    if (headerlst[1].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[1]) != "Action")
                        {
                            flag = 1;
                            title = "Action TAG";
                            Description = "Action TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                    if (headerlst[2].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[2]) != "Step Name")
                        {
                            flag = 1;
                            title = "Step Name TAG";
                            Description = "Step Name TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                    if (headerlst[3].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[3]) != "Test Suit Name")
                        {
                            flag = 1;
                            title = "Test Suit Name TAG";
                            Description = "Test Suit Name TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                    if (headerlst[4].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[4]) != "Test Case Name")
                        {
                            flag = 1;
                            title = "Test Case Name TAG";
                            Description = "Test Case Name TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 4, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                    if (headerlst[5].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[5]) != "Data Set Name")
                        {
                            flag = 1;
                            title = "Data Set Name TAG";
                            Description = "Data Set Name TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 5, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                    if (headerlst[6].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[6]) != "Dependency")
                        {
                            flag = 1;
                            title = "Dependency TAG";
                            Description = "Dependency TAG NOT FOUND";
                            ErrorlogStoryboardExcel(5, 6, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                }

                colCount = headerlst.Count;
                for (int j = 7; j <= (colCount + 1); j++)
                {
                    for (int k = 1; k < rows.Count; k++)
                    {
                        var celllist = rows[k].Descendants<Cell>().ToList();

                        for (int i = 7; i < celllist.Count; i++)
                        {
                            var c = GetColumnIndex(celllist[i].CellReference);
                            flag = 1;
                            title = "Empty field";
                            Description = "Field must be empty";
                            ErrorlogStoryboardExcel(k + 1, Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                }

                for (int i = 5; i < rows.Count; i++)
                {
                    var celllst = rows[i].Descendants<Cell>().ToList();
                    if (celllst.Any())
                    {
                        if (celllst[0].CellValue == null)
                        {
                            flag = 1;
                            title = " Order value Field";
                            Description = "Field should not be Empty";
                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                            ErrorlogStoryboardExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                        if (i > 5)
                        {
                            if (celllst[0].CellValue != null)
                            {
                                var cellpreviouslst = rows[i - 1].Descendants<Cell>().ToList();
                                if (cellpreviouslst[0].CellValue != null)
                                {
                                    try
                                    {
                                        if (Convert.ToInt32(GetValue(doc, celllst[0])) != Convert.ToInt32(GetValue(doc, cellpreviouslst[0])) + 1)
                                        {
                                            flag = 1;
                                            title = " Row number Field";
                                            Description = "Row Number is not Proper";
                                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                                            ErrorlogStoryboardExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var c = GetColumnIndex(celllst[0].CellReference);
                                        flag = 1;
                                        title = "Empty field";
                                        Description = "Field should be Empty";
                                        ErrorlogStoryboardExcel(Convert.ToInt32(rows[i].RowIndex.ToString()), Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                        for (int p = 1; p < celllst.Count; p++)
                                        {
                                            c = GetColumnIndex(celllst[p].CellReference);
                                            flag = 1;
                                            title = "Empty field";
                                            Description = "Field should be Empty";
                                            ErrorlogStoryboardExcel(Convert.ToInt32(rows[i].RowIndex.ToString()), Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                        }
                                        goto EndCondition;
                                    }
                                }
                            }
                        }

                        if (celllst[1].CellValue == null)
                        {
                            flag = 1;
                            title = " Action value Field";
                            Description = "Field should not be Empty";
                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                            ErrorlogStoryboardExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                        if (!(GetValue(doc, celllst[1]).ToLower() == "run" || GetValue(doc, celllst[1]).ToLower() == "execute" || GetValue(doc, celllst[1]).ToLower() == "skip" || GetValue(doc, celllst[1]).ToLower() == "done"))
                        {
                            flag = 1;
                            title = " Action value Field";
                            Description = "Action value is not proper";
                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                            ErrorlogStoryboardExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                        if (celllst[3].CellValue == null)
                        {
                            flag = 1;
                            title = " Test Suite name value Field";
                            Description = "Field should not be Empty";
                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);

                            ErrorlogStoryboardExcel(i + 1, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                        if (celllst[4].CellValue == null)
                        {
                            flag = 1;
                            title = " Test Case Name value Field";
                            Description = "Field should not be Empty";
                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);

                            ErrorlogStoryboardExcel(i + 1, 4, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                        if (celllst[5].CellValue == null)
                        {
                            flag = 1;
                            title = " Data Set Name value Field";
                            Description = "Field should not be Empty";
                            TestSuite_name = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            TestCase_name = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                            Datasetname = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                            Dependancy = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                            run_order = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);

                            ErrorlogStoryboardExcel(i + 1, 5, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }
                    }
                EndCondition:;
                }
            }
            return td;
        }
        public static string ImportVariable(string pFilePath, string lstrConn, string schema, string logpath, string LoginName)
        {
            var lValidationLogFeedProcessId = 0;
            var lreturnpath = "";
            string lOperation = "INSERT";
            string lCreatedBy = LoginName;
            string lStatus = "INPROCESS";
            string lOperationobject = "INSERT";
            string lCREATEDBY = LoginName;
            string lStatusobject = "INPROCESS";
            int lFeedProcessDetailsId = 0;
            int lDEFAULT_FEEDPROCESSDETAIL_ID = 0;
            string lFileType = "VARIABLE";
            string fileName = Import_Variable;
            string lFileName = Path.GetFileName(pFilePath);
            bool lFinalResult = true;
            int lFeedProcessId = FeedProcessHelper.FeedProcess(0, lOperation, lCreatedBy, lStatus, lstrConn, schema);
            lFeedProcessDetailsId = FeedProcessHelper.FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
            try
            {
                bool lResultImportStoryBoard = false;
                lResultImportStoryBoard = ImportExcelVariable(pFilePath, lFeedProcessDetailsId, schema, lstrConn);

                System.Threading.Thread.Sleep(200);
                if (lResultImportStoryBoard == true)
                {
                    lOperationobject = "UPDATE";
                    lStatusobject = "COMPLETED";
                    int lSuccess1 = FeedProcessHelper.FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
                }
                if (dbtable.dt_Log.Rows.Count > 0)
                {
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
                    return lreturnpath;
                }
                lFinalResult = FeedProcessHelper.MappingValidation(lFeedProcessId, lstrConn, schema);
                System.Threading.Thread.Sleep(200);
                DataTable dt = new DataTable();
                dt = FeedProcessHelper.DbExcel(lFeedProcessId, lstrConn, schema);
                dbtable.dt_Log.AcceptChanges();
                System.Threading.Thread.Sleep(200);
                if (dt.Rows.Count != 0)
                {
                    dbtable.dt_Log.Merge(dt);
                    try
                    {
                        dbtable.dt_Log.Columns.RemoveAt(17);
                        dbtable.dt_Log.Columns.RemoveAt(16);
                    }
                    catch
                    {
                    }
                    System.Threading.Thread.Sleep(200);
                }

                if (lFinalResult && dbtable.dt_Log.Rows.Count <= 6)
                {
                    lFinalResult = FeedProcessHelper.DatawareHouseMapping(lFeedProcessId, 1, lstrConn, schema);
                    System.Threading.Thread.Sleep(200);

                    if (lFinalResult)
                    {
                        lOperation = "UPDATE";
                        lCreatedBy = LoginName;
                        lStatus = (lFinalResult) ? "COMPLETED" : "ERROR";
                        int lSuccess = FeedProcessHelper.FeedProcess(lFeedProcessId, lOperation, lCreatedBy, lStatus, lstrConn, schema);
                        string lFileNameExport = "LOGREPORT-" + Path.GetFileName(pFilePath);
                        string lExportLogFile = logpath;
                    }
                }
                else
                {
                    lValidationLogFeedProcessId = lFeedProcessId;
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
                }

                return lreturnpath;

            }
            catch (Exception ex)
            {
                WriteMessage("Error :" + ex.ToString(), logFilePath, "Variable", Import_Variable);
                throw new Exception("Error from:importTestCaseExcel1" + ex.Message);
            }
        }

        public static bool ImportExcelVariable(string pFilePath, int pFEEDPROCESSDETAILID, string schema, string constring)
        {
            try
            {
                bool lResult = false;
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();
                    Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(0);
                    Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                    List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                    DataTable dt = new DataTable();

                    dt = VariableValidation(rows, doc);

                    if (dt.Rows.Count != 0)
                    {
                        lResult = false;
                        dbtable.dt_Log.Merge(dt);
                        return lResult;
                    }

                    DataTable ldt = new DataTable();
                    ldt.Columns.Add("NAME");
                    ldt.Columns.Add("VALUE");
                    ldt.Columns.Add("TYPE");
                    ldt.Columns.Add("BASE/COMP");
                    ldt.Columns.Add("FEEDPROCESSDETAILID");

                    ldt.AcceptChanges();

                    for (int i = 1; i < rows.Count(); i++)
                    {
                        var celllst = rows[i].Descendants<Cell>().ToList();
                        if (celllst.Any())
                        {
                            DataRow dr = ldt.NewRow();
                            dr["NAME"] = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                            dr["VALUE"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]);
                            dr["TYPE"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]);
                            dr["BASE/COMP"] = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]);
                            dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                            ldt.Rows.Add(dr);
                            ldt.AcceptChanges();
                        }
                    }
                    OracleConnection lconnection = GetOracleConnection(constring);
                    lconnection.Open();

                    if (ldt.Rows.Count > 0)
                    {
                        for (int i = 0; i < ldt.Rows.Count; i++)
                        {
                            OracleTransaction ltransaction;
                            ltransaction = lconnection.BeginTransaction();

                            OracleCommand lcmd;
                            lcmd = lconnection.CreateCommand();
                            lcmd.Transaction = ltransaction;

                            lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_VARIABLE";
                            lcmd.CommandType = CommandType.StoredProcedure;

                            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["FEEDPROCESSDETAILID"]);
                            lcmd.Parameters.Add(new OracleParameter("NAME", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["NAME"]);
                            lcmd.Parameters.Add(new OracleParameter("VALUE", OracleDbType.Clob)).Value = Convert.ToString(ldt.Rows[i]["VALUE"]);
                            lcmd.Parameters.Add(new OracleParameter("TYPE", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["TYPE"]);
                            lcmd.Parameters.Add(new OracleParameter("BASE/COMP", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["BASE/COMP"]);

                            try
                            {
                                lcmd.ExecuteNonQuery();
                                lResult = true;
                            }
                            catch (Exception lex)
                            {
                                WriteMessage("Error :" + lex.ToString(), logFilePath, "Variable", Import_Variable);
                                lResult = false;
                                ltransaction.Rollback();
                                throw new Exception(lex.Message);
                            }
                            ltransaction.Commit();
                        }
                    }
                    lconnection.Close();
                    return lResult;
                }
            }
            catch (Exception ex)
            {
                WriteMessage("Error :" + ex.ToString(), logFilePath, "Variable", Import_Variable);
                throw new Exception("Error from:importExcelObject" + ex.Message);
            }
        }
        //private static XL.Range GetDataRange(ref XL.Worksheet xlWs)
        //{
        //    try
        //    {
        //        XL.Range rng = xlWs.Cells.SpecialCells(XL.XlCellType.xlCellTypeLastCell);
        //        XL.Range dataRange = xlWs.Range["A1", rng.Address];
        //        return dataRange;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error from:getDataRange" + ex.Message);
        //    }
        //}
        //static DataTable VariableValidation(XL.Worksheet xlWorksheet, XL.Range xlRange, DataSet ds)
        //{
        //    int flag = 0;

        //    string Application_name = null;
        //    string Testcase_name = null;
        //    string title = null;
        //    string Description = null;
        //    string Datasetname = null;
        //    int TestStepNumber = 0;
        //    string objectname = null;
        //    string comment = null;
        //    string location = null;
        //    string project_name = null;
        //    string storyboard_name = null;
        //    string run_order = null;
        //    string Dependancy = null;
        //    string TestSuite_name = null;
        //    string TabName = null;
        //    int rowCount1 = xlRange.Rows.Count;
        //    int rowCount = xlRange.Rows.Count - 1044576;
        //    int colCount = xlRange.Columns.Count;
        //    int lastUsedRow = xlWorksheet.Cells.Find("*", System.Reflection.Missing.Value,
        //                       System.Reflection.Missing.Value, System.Reflection.Missing.Value,
        //                       XL.XlSearchOrder.xlByRows, XL.XlSearchDirection.xlPrevious,
        //                       false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;
        //    DataTable td = new DataTable();


        //    td.Columns.Add("TimeStamp");
        //    td.Columns.Add("Message Type");
        //    td.Columns.Add("Action");

        //    td.Columns.Add("SpreadSheet cell Address");
        //    td.Columns.Add("Validation Name");
        //    td.Columns.Add("Validation Fail Description");
        //    td.Columns.Add("Application Name");
        //    td.Columns.Add("Project Name");
        //    td.Columns.Add("StoryBoard Name");
        //    td.Columns.Add("Test Suite Name");


        //    td.Columns.Add("TestCase Name");
        //    td.Columns.Add("Test step Number");
        //    td.Columns.Add("Dataset Name");
        //    td.Columns.Add("Dependancy");
        //    td.Columns.Add("Run Order");



        //    td.Columns.Add("Object Name");
        //    td.Columns.Add("Comment");
        //    td.Columns.Add("Error Description");
        //    td.Columns.Add("Program Location");
        //    td.Columns.Add("Tab Name");



        //    if (xlWorksheet.Cells[1, 1].Value.ToUpper() != "NAME")

        //    {
        //        flag = 1;
        //        title = "Name Tag";
        //        Description = "Name Tag NOT FOUND";

        //        ErrorVariabeLogExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //    }
        //    if (xlWorksheet.Cells[1, 2].Value.ToUpper() != "VALUE")

        //    {
        //        flag = 1;
        //        title = "VALUE Tag";
        //        Description = "VALUE Tag  NOT FOUND";
        //        // Application_name = xlWorksheet.Cells[1, 1].Value;
        //        ErrorVariabeLogExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //    }
        //    if (xlWorksheet.Cells[1, 3].Value.ToUpper() != "TYPE")

        //    {
        //        flag = 1;
        //        title = "TYPE";
        //        Description = "TYPE TAG NOT FOUND";
        //        // Application_name = xlWorksheet.Cells[1, 1].Value;
        //        ErrorVariabeLogExcel(2, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //    }
        //    if (xlWorksheet.Cells[1, 4].Value != "Base/Comp")

        //    {
        //        flag = 1;
        //        title = "Base/Comp";
        //        Description = "Base/Comp TAG NOT FOUND";
        //        // Application_name = xlWorksheet.Cells[1, 1].Value;
        //        ErrorVariabeLogExcel(2, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //    }
        //    //for (int j = 1; j < 5; j++)
        //    //{

        //    //    if (xlWorksheet.Cells[, j].Value != null)
        //    //    {
        //    //        flag = 1;
        //    //        title = "Empty field";
        //    //        Description = "Field must be empty";
        //    //        Application_name = xlWorksheet.Cells[1, 1].Value;
        //    //        ErrorlogExcel(1, j - 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
        //    //    }
        //    //}
        //    int setrow = 0;
        //    for (int i = 2; i <= lastUsedRow; i++)
        //    {

        //        if (xlWorksheet.Cells[i, 1].Value == null)
        //        {
        //            title = "Non Empty field";
        //            Description = "Field must not be empty";
        //            // Application_name = xlWorksheet.Cells[1, 1].Value;
        //            ErrorVariabeLogExcel(i, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);

        //        }
        //        if (!(xlWorksheet.Cells[i, 3].Value != "GLOBAL_VAR" || xlWorksheet.Cells[i, 3].Value != "MODAL_VAR" || xlWorksheet.Cells[i, 3].Value != "LOOP_VAR"))
        //        {
        //            title = "Type Value";
        //            Description = "type value is not proper";
        //            //Application_name = xlWorksheet.Cells[1, 1].Value;
        //            ErrorVariabeLogExcel(i, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);

        //        }
        //        if (!(xlWorksheet.Cells[i, 4].Value != "BASELINE" || xlWorksheet.Cells[i, 3].Value != "COMPARE" || xlWorksheet.Cells[i, 3].Value != null))
        //        {
        //            title = "Base/comp Value";
        //            Description = "Base/comp Value is not proper";
        //            // Application_name = xlWorksheet.Cells[1, 1].Value;
        //            ErrorVariabeLogExcel(i, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);

        //        }


        //    }



        //    return td;
        //}

        static DataTable VariableValidation(List<Row> rows, SpreadsheetDocument doc)
        {
            int flag = 0;

            string Application_name = null;
            string Testcase_name = null;
            string title = null;
            string Description = null;
            string Datasetname = null;
            int TestStepNumber = 0;
            string objectname = null;
            string comment = null;
            string location = null;
            string project_name = null;
            string storyboard_name = null;
            string run_order = null;
            string Dependancy = null;
            string TestSuite_name = null;
            string TabName = null;

            DataTable td = new DataTable();

            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");
            td.Columns.Add("Test Suite Name");

            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");
            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");

            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            if (rows.Any())
            {
                var hedar = rows[0].Descendants<Cell>();
                var hedarlst = hedar.ToList();

                if (GetValue(doc, hedarlst[0]).ToUpper() != "NAME")
                {
                    flag = 1;
                    title = "Name Tag";
                    Description = "Name Tag NOT FOUND";

                    ErrorVariabeLogExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[1]).ToUpper() != "VALUE")
                {
                    flag = 1;
                    title = "VALUE Tag";
                    Description = "VALUE Tag  NOT FOUND";
                    ErrorVariabeLogExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[2]).ToUpper() != "TYPE")
                {
                    flag = 1;
                    title = "TYPE";
                    Description = "TYPE TAG NOT FOUND";
                    ErrorVariabeLogExcel(2, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[3]) != "Base/Comp")
                {
                    flag = 1;
                    title = "Base/Comp";
                    Description = "Base/Comp TAG NOT FOUND";
                    ErrorVariabeLogExcel(2, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
            }

            for (int i = 1; i < rows.Count; i++)
            {
                var celllst = rows[i].Descendants<Cell>().ToList();

                if (celllst.Any())
                {
                    if (celllst[0].CellValue == null)
                    {
                        title = "Non Empty field";
                        Description = "Field must not be empty";
                        ErrorVariabeLogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (!(GetValue(doc, celllst[2]) != "GLOBAL_VAR" || GetValue(doc, celllst[2]) != "MODAL_VAR" || GetValue(doc, celllst[2]) != "LOOP_VAR"))
                    {
                        title = "Type Value";
                        Description = "type value is not proper";
                        ErrorVariabeLogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (!(GetValue(doc, celllst[3]) != "BASELINE" || GetValue(doc, celllst[2]) != "COMPARE" || celllst[2].CellValue != null))
                    {
                        title = "Base/comp Value";
                        Description = "Base/comp Value is not proper";
                        ErrorVariabeLogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }
            }
            return td;
        }

        private static string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = string.Empty;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerText == null ? "" : cell.CellValue.InnerText;
            }

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                string val = doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                return val;
            }
            return value;
            //string value = cell.CellValue.InnerText;
            //if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            //{
            //    return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            //}
            //return value;
        }

        static void ErrorVariabeLogExcel(int r, int c, string app_name, string projectname, string storyboard_name, string TestSuite_name, string Dependancy, string run_order, DataTable dt, string title, string Description, string Datasetname, int TestStepNumber, string objectname, string comment, string loc, string testcasename, string tabname)
        {
            char c1;
            char c2 = ' ';

            c1 = (char)((c % 26) + 65);
            if (c >= 26)
            {
                c2 = (char)(((c / 26) - 1) + 65);
            }



            //  cell = cell + ","+(c1 + r.ToString());

            DateTime now = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["TimeStamp"] = now;
            dr["Message Type"] = " validation Error";
            dr["Action"] = "validation";

            dr["SpreadSheet cell Address"] = c2.ToString() + c1.ToString() + r.ToString();
            dr["Validation Name"] = title.ToString();
            dr["Validation Fail Description"] = Description.ToString();
            dr["Application Name"] = app_name;
            dr["Project Name"] = projectname;
            dr["StoryBoard Name"] = storyboard_name;
            dr["Test Suite Name"] = TestSuite_name;
            dr["TestCase Name"] = testcasename;
            dr["Test step Number"] = TestStepNumber;

            dr["Dataset Name"] = Datasetname;
            dr["Dependancy"] = Dependancy;
            dr["Run Order"] = run_order;




            dr["Object Name"] = objectname;
            dr["Comment"] = comment;
            dr["Error Description"] = "Error";
            dr["Program Location"] = "Error";
            dr["Tab Name"] = tabname;
            dt.Rows.Add(dr);
        }
        public static string ImportObject(string pFilePath, string lstrConn, string schema, string logpath, string LoginName)
        {
            var lValidationLogFeedProcessId = 0;
            var lreturnpath = "";
            string lOperation = "INSERT";
            string lCreatedBy = LoginName;
            string lStatus = "INPROCESS";
            string lOperationobject = "INSERT";
            string lCREATEDBY = LoginName;
            string lStatusobject = "INPROCESS";
            int lFeedProcessDetailsId = 0;
            int lDEFAULT_FEEDPROCESSDETAIL_ID = 0;
            string lFileType = "OBJECT";
            string fileName = Import_Object;
            string lFileName = Path.GetFileName(pFilePath);
            bool lFinalResult = true;
            int lFeedProcessId = FeedProcessHelper.FeedProcess(0, lOperation, lCreatedBy, lStatus, lstrConn, schema);
            lFeedProcessDetailsId = FeedProcessHelper.FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
            try
            {
                bool lResultImportStoryBoard = false;
                lResultImportStoryBoard = ImportExcelObject(pFilePath, lFeedProcessDetailsId, schema, lstrConn);

                System.Threading.Thread.Sleep(200);
                if (lResultImportStoryBoard == true)
                {
                    lOperationobject = "UPDATE";
                    lStatusobject = "COMPLETED";
                    int lSuccess1 = FeedProcessHelper.FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, lstrConn, schema);
                }
                if (dbtable.dt_Log.Rows.Count > 0)
                {

                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
                    return lreturnpath;
                }
                lFinalResult = FeedProcessHelper.MappingValidation(lFeedProcessId, lstrConn, schema);
                System.Threading.Thread.Sleep(200);
                DataTable dt = new DataTable();
                dt = FeedProcessHelper.DbExcel(lFeedProcessId, lstrConn, schema);
                dbtable.dt_Log.AcceptChanges();
                System.Threading.Thread.Sleep(200);
                if (dt.Rows.Count != 0)
                {
                    dbtable.dt_Log.Merge(dt);
                    try
                    {
                        dbtable.dt_Log.Columns.RemoveAt(17);
                        dbtable.dt_Log.Columns.RemoveAt(16);
                    }
                    catch
                    {

                    }
                    System.Threading.Thread.Sleep(200);
                }

                if (lFinalResult && dbtable.dt_Log.Rows.Count <= 6)
                {
                    lFinalResult = FeedProcessHelper.DatawareHouseMapping(lFeedProcessId, 1, lstrConn, schema);
                    System.Threading.Thread.Sleep(200);

                    if (lFinalResult)
                    {
                        lOperation = "UPDATE";
                        lCreatedBy = LoginName;
                        lStatus = (lFinalResult) ? "COMPLETED" : "ERROR";
                        int lSuccess = FeedProcessHelper.FeedProcess(lFeedProcessId, lOperation, lCreatedBy, lStatus, lstrConn, schema);
                        string lFileNameExport = "LOGREPORT-" + Path.GetFileName(pFilePath);
                        string lExportLogFile = logpath;
                    }
                }
                else
                {
                    lValidationLogFeedProcessId = lFeedProcessId;
                    lreturnpath = FeedProcessHelper.Excel(dbtable.dt_Log, fileName, logpath);
                }
                return lreturnpath;
            }
            catch (Exception ex)
            {
                throw new Exception("Error from:importTestCaseExcel1" + ex.Message);
            }
        }
      
        public static bool ImportExcelObject(string pFilePath, int pFEEDPROCESSDETAILID, string schema, string constring)
        {
            try
            {
                bool lResult = false;

                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {

                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();
                    Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(0);
                    Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                    List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                    DataTable dt = new DataTable();

                    dt = ObjectValidation(rows, doc);

                    if (dt.Rows.Count != 0)
                    {
                        lResult = false;
                        dbtable.dt_Log.Merge(dt);
                        return lResult;
                    }

                    var fristRow = rows[0].Descendants<Cell>().ToList();
                    string ApplicationName = GetValue(doc, fristRow[0]);

                    DataTable ldt = new DataTable();
                    ldt.Columns.Add("OBJECTNAME");
                    ldt.Columns.Add("OBJECTTYPE");
                    ldt.Columns.Add("QUICKACCESS");
                    ldt.Columns.Add("PARENT");
                    ldt.Columns.Add("OBJECTCOMMENT");
                    ldt.Columns.Add("ENUMTYPE");
                    ldt.Columns.Add("OBJECTSQL");
                    ldt.Columns.Add("APPLICATIONNAME");
                    ldt.Columns.Add("FEEDPROCESSDETAILID");

                    ldt.AcceptChanges();


                    for (int i = 2; i < rows.Count; i++)
                    {
                        var celllst = rows[i].Descendants<Cell>().ToList();
                        DataRow dr = ldt.NewRow();
                        dr["OBJECTNAME"] = GetValue(doc, celllst[0]);
                        dr["OBJECTTYPE"] = GetValue(doc, celllst[1]);
                        dr["QUICKACCESS"] = GetValue(doc, celllst[2]);
                        dr["PARENT"] = GetValue(doc, celllst[3]);
                        dr["OBJECTCOMMENT"] = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]);
                        dr["ENUMTYPE"] = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]);
                        dr["OBJECTSQL"] = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]);
                        dr["APPLICATIONNAME"] = ApplicationName;
                        dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                        ldt.Rows.Add(dr);
                        ldt.AcceptChanges();
                    }

                    OracleConnection lconnection = GetOracleConnection(constring);
                    lconnection.Open();


                    if (ldt.Rows.Count > 0)
                    {
                        for (int i = 0; i < ldt.Rows.Count; i++)
                        {
                            OracleTransaction ltransaction;
                            ltransaction = lconnection.BeginTransaction();

                            OracleCommand lcmd;
                            lcmd = lconnection.CreateCommand();
                            lcmd.Transaction = ltransaction;

                            //The name of the Procedure responsible for inserting the data in the table.
                            lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_OBJECT";
                            lcmd.CommandType = CommandType.StoredProcedure;

                            lcmd.Parameters.Add(new OracleParameter("OBJECTNAME", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["OBJECTNAME"]);
                            lcmd.Parameters.Add(new OracleParameter("OBJECTTYPE", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["OBJECTTYPE"]);
                            lcmd.Parameters.Add(new OracleParameter("QUICKACCESS", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["QUICKACCESS"]);
                            lcmd.Parameters.Add(new OracleParameter("PARENT", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["PARENT"]);
                            lcmd.Parameters.Add(new OracleParameter("OBJECTCOMMENT", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["OBJECTCOMMENT"]);
                            lcmd.Parameters.Add(new OracleParameter("ENUMTYPE", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["ENUMTYPE"]);
                            lcmd.Parameters.Add(new OracleParameter("OBJECTSQL", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["OBJECTSQL"]);
                            lcmd.Parameters.Add(new OracleParameter("APPLICATIONNAME", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["APPLICATIONNAME"]);
                            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32)).Value = Convert.ToInt32(ldt.Rows[i]["FEEDPROCESSDETAILID"]);

                            try
                            {
                                lcmd.ExecuteNonQuery();
                                lResult = true;
                            }
                            catch (Exception lex)
                            {
                                WriteMessage("Error :" + lex.ToString(), logFilePath, "Object", Import_Object);
                                lResult = false;
                                ltransaction.Rollback();

                                throw new Exception(lex.Message);
                            }

                            ltransaction.Commit();
                        }
                    }
                    lconnection.Close();

                    return lResult;
                }
            }
            catch (Exception ex)
            {
                WriteMessage("Error :" + ex.ToString(), logFilePath, "Object", Import_Object);
                throw new Exception("Error from:importExcelObject" + ex.Message);
            }
        }
        
        static DataTable ObjectValidation(List<Row> rows, SpreadsheetDocument doc)
        {
            int flag = 0;

            string Application_name = null;
            string Testcase_name = null;
            string title = null;
            string Description = null;
            string Datasetname = null;
            int TestStepNumber = 0;
            string objectname = null;
            string comment = null;
            string location = null;
            string project_name = null;
            string storyboard_name = null;
            string run_order = null;
            string Dependancy = null;
            string TestSuite_name = null;
            string TabName = null;
            int colCount = default(int);

            DataTable td = new DataTable();


            td.Columns.Add("TimeStamp");
            td.Columns.Add("Message Type");
            td.Columns.Add("Action");

            td.Columns.Add("SpreadSheet cell Address");
            td.Columns.Add("Validation Name");
            td.Columns.Add("Validation Fail Description");
            td.Columns.Add("Application Name");
            td.Columns.Add("Project Name");
            td.Columns.Add("StoryBoard Name");
            td.Columns.Add("Test Suite Name");


            td.Columns.Add("TestCase Name");
            td.Columns.Add("Test step Number");
            td.Columns.Add("Dataset Name");
            td.Columns.Add("Dependancy");
            td.Columns.Add("Run Order");



            td.Columns.Add("Object Name");
            td.Columns.Add("Comment");
            td.Columns.Add("Error Description");
            td.Columns.Add("Program Location");
            td.Columns.Add("Tab Name");

            if (rows.Any())
            {
                var fristRow = rows[0].Descendants<Cell>().ToList();
                var hedarlst = rows[1].Descendants<Cell>().ToList();

                if (fristRow[0].CellValue == null)
                {
                    flag = 1;
                    title = "Application name";
                    Description = "Application name NOT FOUND";
                    ErrorVariabeLogExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }

                if (GetValue(doc, hedarlst[0]) != "OBJECT NAME")
                {
                    flag = 1;
                    title = "OBJECT_HAPPY_NAME";
                    Description = "OBJECT_HAPPY_NAME TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[1]) != "TYPE")
                {
                    flag = 1;
                    title = "TYPE";
                    Description = "TYPE TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[2]) != "QUICK_ACCESS")
                {
                    flag = 1;
                    title = "QUICK_ACCESS";
                    Description = "QUICK_ACCESS TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[3]) != "PARENT")
                {
                    flag = 1;
                    title = "PARENT";
                    Description = "PARENT TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[4]) != "COMMENT")
                {
                    flag = 1;
                    title = "COMMENT";
                    Description = "COMMENT TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 4, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[5]) != "ENUM_TYPE")
                {
                    flag = 1;
                    title = "ENUM_TYPE";
                    Description = "ENUM_TYPE TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 5, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }
                if (GetValue(doc, hedarlst[6]) != "SQL")
                {
                    flag = 1;
                    title = "SQL";
                    Description = "SQL TAG NOT FOUND";
                    Application_name = GetValue(doc, fristRow[0]);
                    ErrorVariabeLogExcel(2, 6, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                }

                if (fristRow.Count > 1)
                {
                    for (int j = 1; j < fristRow.Count; j++)
                    {
                        var celllist = fristRow[j].Descendants<Cell>().ToList();
                        if(!string.IsNullOrEmpty(GetValue(doc, fristRow[j])))
                            {
                            flag = 1;
                            title = "Empty field";
                            Description = "Field must be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorVariabeLogExcel(1, Convert.ToInt32(GetColumnIndex(fristRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                       
                    }
                }

                colCount = hedarlst.Count;
                for (int j = 7; j <= (colCount + 1); j++)
                {
                    for (int k = 1; k < rows.Count; k++)
                    {
                        var celllist = rows[k].Descendants<Cell>().ToList();

                        for (int i = 7; i < celllist.Count; i++)
                        {
                            var c = GetColumnIndex(celllist[i].CellReference);
                            flag = 1;
                            title = "Empty field";
                            Description = "Field must be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorVariabeLogExcel(k + 1, Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                }
                for (int i = 2; i < rows.Count; i++)
                {
                    var celllst = rows[i].Descendants<Cell>().ToList();

                    if (celllst.Any())
                    {
                        if (celllst[0].CellValue == null)
                        {
                            flag = 1;
                            title = " NON Empty field";
                            Description = "Field must NOT be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorVariabeLogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                        if (celllst[1].CellValue == null)
                        {
                            flag = 1;
                            title = " NON Empty field";
                            Description = "Field must NOT be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorVariabeLogExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                        if (celllst[2].CellValue == null)
                        {
                            flag = 1;
                            title = " NON Empty field";
                            Description = "Field must NOT be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorVariabeLogExcel(i + 1, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                        if (celllst[3].CellValue == null)
                        {
                            flag = 1;
                            title = " NON Empty field";
                            Description = "Field must NOT be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorVariabeLogExcel(i + 1, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                }
            }
            return td;
        }

        private static int? GetColumnIndex(string cellReference)
        {
            if (string.IsNullOrEmpty(cellReference))
            {
                return null;
            }

            string columnReference = Regex.Replace(cellReference.ToUpper(), @"[\d]", string.Empty);

            int columnNumber = -1;
            int mulitplier = 1;

            foreach (char c in columnReference.ToCharArray().Reverse())
            {
                columnNumber += mulitplier * ((int)c - 64);

                mulitplier = mulitplier * 26;
            }

            return columnNumber;
        }
        private static void WriteToFile(string filePath, string content, bool append)
        {
            using (StreamWriter sw = new StreamWriter(filePath, append))
            {
                sw.WriteAsync(content);
                sw.Flush();
                sw.Close();
            }
        }

        public static void WriteMessage(string message, string currentPath, string logtype, string filename)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string Filepath = currentPath + "\\Log." + dt.Day + "." + dt.Month + "." + dt.Year + ".txt";

                string Content = DateTime.Now + " | " + Convert.ToString(logtype) + " | " + filename + " | " + message;
                WriteToFile(Filepath, Content + Environment.NewLine, true);
            }
            catch (Exception EX)
            {
                string s = EX.Message;
            }
        }
    }
}

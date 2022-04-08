using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARSUtility
{
    public class ImportHelper
    {
        ImportExcel excel = new ImportExcel();
        //public string schema = "";
        //public string constring = "";
        //public OracleConnection GetOracleConnection()
        //{

        //    string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    string configFile = System.IO.Path.Combine(appPath, "MarsConsole.exe.config");
        //    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        //    configFileMap.ExeConfigFilename = configFile;
        //    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        //    string ldataSource = config.AppSettings.Settings["DataSource"].Value;
        //    schema = config.AppSettings.Settings["Schema"].Value;
        //    constring = ldataSource;
        //    return new OracleConnection(ldataSource);
        //}
        public OracleConnection GetOracleConnection(string constring)
        {
            return new OracleConnection(constring);
        }
        //Import starts here for all excel files
        public bool MasterImport(int lISOVERWRITE, string lFolderLocation, string lExportLogPath, string type, int cflag, string project, string name, string desc, int resultmode, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->MasterImport";
            bool lFinalResult = true;
            try
            {
                if (!string.IsNullOrEmpty(lFolderLocation))
                {
                    string lOperation = "INSERT";
                    string lCreatedBy = "SYSTEM";
                    string lStatus = "INPROCESS";
                    int lFeedProcessId = FeedProcess(0, lOperation, lCreatedBy, lStatus, schemaname, constring);
                    if (lFeedProcessId != 0)
                    {
                        string lFileName = Path.GetFileName(lFolderLocation);
                        string lFullPath = lFolderLocation;
                        int lFeedProcessDetailsId = 0;
                        int lDEFAULT_FEEDPROCESSDETAIL_ID = 0;
                        string lOperationobject = "INSERT";
                        string lCREATEDBY = "SYSTEM";
                        string lStatusobject = "INPROCESS";
                        string lFileType = string.Empty;
                        bool lResult = false;
                        if (type.ToUpper() == "OBJECT")
                        {
                            lFileType = "OBJECT";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                            if (lFeedProcessDetailsId != 0)
                            {
                                // call Import Method for Objects File
                                lResult = ImportExcel.ImportExcelObject(lFullPath, lFeedProcessDetailsId, schemaname, constring);
                                if (lResult)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        if (type.ToUpper() == "VARIABLE")
                        {
                            lFileType = "VARIABLE";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                            if (lFeedProcessDetailsId != 0)
                            {

                                lResult = ImportExcel.ImportExcelVariable(lFullPath, lFeedProcessDetailsId, schemaname, constring);
                                if (lResult)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        if (type.ToUpper() == "COMPARECONFIG")
                        {
                            // call Import Method for Compareparam File
                            lFileType = "COMPAREPARAM";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                            if (lFeedProcessDetailsId != 0)
                            {

                                lResult = ImportExcel.ImportExcelCompareConfig(lFullPath, lFeedProcessDetailsId, schemaname, constring);
                                if (lResult)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        if (type.ToUpper() == "STORYBOARD")
                        {

                            lFileType = "STORYBOARD";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                            if (lFeedProcessDetailsId != 0)
                            {

                                bool lResultImportStoryBoard = false;
                                lResultImportStoryBoard = ImportExcel.ImportExcelStoryboard(lFullPath, lFeedProcessDetailsId, lExportLogPath, constring, schemaname);
                                //lResult = importTestCaseExcel1(lFullPath, lFeedProcessDetailsId);
                                if (lResultImportStoryBoard)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }

                                else
                                {
                                    return false;
                                }
                            }
                        }
                        if (type.ToUpper() == "TESTCASE")
                        {

                            lFileType = "TESTCASE";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                            if (lFeedProcessDetailsId != 0)
                            {

                                bool lResultImportTestCase = false;
                                lResultImportTestCase = ImportExcel.ImportTestCaseExcel(lFullPath, lFeedProcessDetailsId, lExportLogPath, constring, schemaname);
                                if (lResultImportTestCase)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        if (type.ToUpper() == "RESULTSET")
                        {
                            cflag = 1;
                            lFileType = "RESULTSET";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                           
                            if (lFeedProcessDetailsId != 0)
                            {
                                bool lResultImportResultset = false;
                                //Import result set  
                              
                                lResultImportResultset = ImportExcel.ImportExcelResultSet(lFullPath, lFeedProcessDetailsId, lExportLogPath, project, name, desc, resultmode, constring, schemaname);
                                
                                if (lResultImportResultset)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        if (type.ToUpper() == "DATASETTAG")
                        {
                            lFileType = "DATASETTAG";
                            lFeedProcessDetailsId = FeedProcessDetails(lDEFAULT_FEEDPROCESSDETAIL_ID, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                            if (lFeedProcessDetailsId != 0)
                            {

                                lResult = ImportExcel.ImportExcelDatasetTag(lFullPath, lFeedProcessDetailsId, schemaname, constring);
                                if (lResult)
                                {
                                    lOperationobject = "UPDATE";
                                    lStatusobject = "COMPLETED";
                                    int lSuccess1 = FeedProcessDetails(lFeedProcessDetailsId, lFeedProcessId, lOperationobject, lFileName, lCREATEDBY, lStatusobject, lFileType, schemaname, constring);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        int ISOVERWRITE = 0;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\nSpreadsheet Validation starting at {0}.. Please wait!", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        dbtable.errorlog("Spreadsheet Validation Started!", "", "", 0);
                        if (type.ToUpper() == "RESULTSET")
                            lFinalResult = MAPPINGVALIDATIONFORRESULTSET(lFeedProcessDetailsId, schemaname, constring);
                        else if(type.ToUpper() == "DATASETTAG")
                            lFinalResult = MAPPINGVALIDATIONFORDATASETTAG(lFeedProcessDetailsId, schemaname, constring);
                        else
                            lFinalResult = MAPPINGVALIDATION(lFeedProcessId, schemaname, constring);
                        System.Threading.Thread.Sleep(200);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\nSpreadsheet Validation Completed {0}..!", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

                        System.Threading.Thread.Sleep(200);
                        DataTable dt = new DataTable();
                        if (type.ToUpper() == "RESULTSET" || type.ToUpper() == "DATASETTAG")
                            dt = DbexcelResultset(lFeedProcessDetailsId, schemaname, constring);
                        else
                            dt = Dbexcel(lFeedProcessId, schemaname, constring);
                        if (dt.Rows.Count != 0)
                        {
                            Console.WriteLine("\n");
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                if (type.ToUpper() == "STORYBOARD")
                                {
                                    Console.WriteLine("Testcase\\Tab Name:- " + dt.Rows[i][8].ToString() + " =>" + " Test-step NO:-" + dt.Rows[i][11].ToString() + " =>" + " Error Description:-" + dt.Rows[i][17].ToString());
                                }
                                else if (type.ToUpper() == "TESTCASE")
                                {
                                    Console.WriteLine("Testcase\\Tab Name:- " + dt.Rows[i][10].ToString() + " =>" + " Test-step NO:-" + dt.Rows[i][11].ToString() + " =>" + " Error Description:-" + dt.Rows[i][17].ToString());
                                }
                                else if (type.ToUpper() == "DATASETTAG")
                                {
                                    Console.WriteLine("DatasetTag\\Tab Name:- " + dt.Rows[i][8].ToString() + " =>" + " Test-step NO:-" + dt.Rows[i][11].ToString() + " =>" + " Error Description:-" + dt.Rows[i][17].ToString());
                                }
                                else if (type.ToUpper() == "RESULTSET")
                                {
                                    Console.WriteLine("ResultSet\\Tab Name:- " + dt.Rows[i][10].ToString() + " =>" + " Test-step NO:-" + dt.Rows[i][11].ToString() + " =>" + " Error Description:-" + dt.Rows[i][17].ToString());
                                }
                                else
                                {
                                    Console.WriteLine("Testcase\\Tab Name:- " + dt.Rows[i][8].ToString() + " =>" + " Test-step NO:-" + dt.Rows[i][11].ToString() + " =>" + " Error Description:-" + dt.Rows[i][17].ToString());
                                }
                            }
                            dbtable.dt_Log.Merge(dt);
                            try
                            {
                                //if (type.ToUpper() == "STORYBOARD")
                                //{
                                dbtable.dt_Log.Columns.RemoveAt(21);
                                dbtable.dt_Log.Columns.RemoveAt(21);

                                //if (type.ToUpper() == "TESTCASE")
                                //{
                                //    dbtable.dt_Log.Columns.RemoveAt(17);
                                //    dbtable.dt_Log.Columns.RemoveAt(16);

                                //}

                            }
                            catch
                            {

                            }
                            dbtable.errorlog("Spreadsheet Validation Completed!", "", "", 0);
                            System.Threading.Thread.Sleep(200);
                            return false;
                        }
                        else
                        {
                            dbtable.errorlog("Spreadsheet Validation Completed!", "", "", 0);
                        }
                        System.Threading.Thread.Sleep(200);
                        if (lFinalResult && cflag == 1 && dbtable.dt_Log.Rows.Count <= 6)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nMapping is starting at {0}.. Please wait!", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            dbtable.errorlog("mapping started", "", "", 0);
                            if (type.ToUpper() == "RESULTSET")
                                lFinalResult = DataResultSetMapping(lFeedProcessDetailsId, schemaname, constring);
                            else  if (type.ToUpper() == "DATASETTAG")
                                lFinalResult = DataTagSetMapping(lFeedProcessDetailsId, schemaname, constring); 
                            else
                                lFinalResult = DatawareHouseMapping(lFeedProcessId, lISOVERWRITE, schemaname, constring);
                            System.Threading.Thread.Sleep(200);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nMapping is completed, Log export is staring at {0}.. Please wait !", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            System.Threading.Thread.Sleep(200);
                            dbtable.errorlog("mapping completed", "", "", 0);
                            dbtable.errorlog("log report starting", "", "", 0);

                            if (lFinalResult)
                            {
                                lOperation = "UPDATE";
                                lCreatedBy = "SYSTEM";
                                lStatus = (lFinalResult) ? "COMPLETED" : "ERROR";
                                int lSuccess = FeedProcess(lFeedProcessId, lOperation, lCreatedBy, lStatus, schemaname, constring);

                                string lFileNameExport = "LOGREPORT-" + Path.GetFileName(lFolderLocation);
                                string lExportLogFile = lExportLogPath;

                            }
                            dbtable.errorlog("Log export is completed ", "", "", 0);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nLog export is completed at {0}..", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            System.Threading.Thread.Sleep(200);
                        }
                        else
                        {
                            if (cflag == 0)
                            {
                                dbtable.errorlog("-commit not found in the command!", "", "", 0);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\nRolling back the import as -commit not found in the command!!!");
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                if (ex.Message.Contains("Index was out of range"))
                {
                    msg = "Excel sheet Format is not valid. Please check excelsheet sheet issue. Please check merge cell are proper. No extra cell added in sheet.";
                    dbtable.errorlog(msg, "Master Import", SomeGlobalVariables.functionName, line);
                }
                else if(!msg.Contains("PL/SQL: numeric or value error") && !msg.Contains("at line 1"))
                {
                    dbtable.errorlog(msg, "Master Import", SomeGlobalVariables.functionName, line);
                    //throw new Exception("Error from:MasterImport" + ex.Message);
                }
                else
                    dbtable.errorlog(msg, "Master Import", SomeGlobalVariables.functionName, line);
                lFinalResult = false;
            }
            return lFinalResult;
        }

        //Calls Validation SP to check the logical validations
        public bool MAPPINGVALIDATION(int lFeedProcessId, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->MappingValidation";
            bool lResult = false;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "USP_MAPPING_VALIDATION";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSID1", OracleDbType.Int32)).Value = lFeedProcessId;
            lcmd.Parameters.Add(new OracleParameter("RESULT", OracleDbType.Varchar2, 300000)).Direction = ParameterDirection.Output;

            try
            {
                lcmd.ExecuteNonQuery();
                string lCheckResult = string.Empty;
                lCheckResult = lcmd.Parameters[1].Value.ToString();
                if (!string.IsNullOrEmpty(lCheckResult) && lCheckResult.ToLower() == "error")
                {
                    lResult = false;
                }
                else
                {
                    lResult = true;
                }

            }
            catch (Exception lex)
            {

                ltransaction.Rollback();

                int line;
                string msg = lex.Message;
                line = dbtable.lineNo(lex);
                dbtable.errorlog(msg, "Mapping Validations", SomeGlobalVariables.functionName, line);
                throw new Exception(/*lex.Message*/);
            }

            ltransaction.Commit();


            lconnection.Close();
            return lResult;

        }

        //Calls Validation SP to check the logical validations
        public bool MAPPINGVALIDATIONFORRESULTSET(int lFeedProcessDetailId, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->MappingValidationForResultSet";
            bool lResult = false;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "SP_VALIDATE_IMPORT_RESULTS";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("FPROCESSDETAILID", OracleDbType.Int32)).Value = lFeedProcessDetailId;
            lcmd.Parameters.Add(new OracleParameter("RESULT", OracleDbType.Varchar2, 300000)).Direction = ParameterDirection.Output;

            try
            {
                lcmd.ExecuteNonQuery();
                string lCheckResult = string.Empty;
                lCheckResult = lcmd.Parameters[1].Value.ToString();
                if (!string.IsNullOrEmpty(lCheckResult) && lCheckResult.ToLower() == "error")
                {
                    lResult = false;
                }
                else

                {
                    lResult = true;
                }
            }
            catch (Exception lex)
            {

                ltransaction.Rollback();

                int line;
                string msg = lex.Message;
                line = dbtable.lineNo(lex);
                dbtable.errorlog(msg, "Mapping Validations For Resultset", SomeGlobalVariables.functionName, line);
                throw new Exception(/*lex.Message*/);
            }

            ltransaction.Commit();
            lconnection.Close();
            return lResult;

        }

        //Calls SP to insert the validated data into the respective tables
        public bool DatawareHouseMapping(int lFeedProcessId, int lIsOverWrite, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->DataWareHouseMapping";
            bool lResult = false;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "USP_FEEDPROCESSMAPPING_Mode_D";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSID1", OracleDbType.Int32)).Value = lFeedProcessId;
            lcmd.Parameters.Add(new OracleParameter("ISOVERWRITE", OracleDbType.Int32)).Value = lIsOverWrite;
            lcmd.Parameters.Add(new OracleParameter("RESULT", OracleDbType.Varchar2, 300000)).Direction = ParameterDirection.Output;

            try
            {
                lcmd.ExecuteNonQuery();
                string lCheckResult = string.Empty;
                lCheckResult = lcmd.Parameters[1].Value.ToString();
                if (lCheckResult.ToLower().Contains("error"))
                {
                    lResult = false;
                }
                else
                {
                    lResult = true;
                }
                ltransaction.Commit();
            }
            catch (Exception lex)
            {
                int line;
                string msg = lex.Message;
                //if (!msg.Contains("PL/SQL: numeric or value error") && !msg.Contains("at line 1"))
               // {
                    ltransaction.Rollback();
                    line = dbtable.lineNo(lex);
                    dbtable.errorlog(msg, "DataWarehouse mapping", SomeGlobalVariables.functionName, line);
                    //throw new Exception(lex.Message);
                //}
                //else
               // {
                    //ltransaction.Commit();
                //}
            }
            lconnection.Close();
            return lResult;
        }

        public bool DataResultSetMapping(int lFeedProcessDetailId, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->DataResultSetMapping";
            bool lResult = false;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "SP_SAVE_SB_IMPORT_RESULTS";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("feeddetailid", OracleDbType.Int32)).Value = lFeedProcessDetailId;

            try
            {
                lcmd.ExecuteNonQuery();
                lResult = true;
                ltransaction.Commit();
            }
            catch (Exception lex)
            {
                lResult = false;
                int line;
                string msg = lex.Message;
                if (!msg.Contains("PL/SQL: numeric or value error") && !msg.Contains("at line 1"))
                {
                    ltransaction.Rollback();
                    line = dbtable.lineNo(lex);
                    dbtable.errorlog(msg, "SP_SAVE_SB_IMPORT_RESULTS mapping", SomeGlobalVariables.functionName, line);
                    throw new Exception(lex.Message);
                }
                else
                {
                    ltransaction.Commit();
                }
            }
            lconnection.Close();
            return lResult;
        }

        //Inserts feedprocess id
        public int FeedProcess(int lFeedProcessId, string lOperation, string lCreatedBy, string lStatus, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->FeedProcess";

            int lResult = 0;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.

            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSID", OracleDbType.Int32)).Value = lFeedProcessId;
            lcmd.Parameters.Add(new OracleParameter("OPERATION", OracleDbType.Varchar2)).Value = lOperation;
            lcmd.Parameters.Add(new OracleParameter("CREATEDBY", OracleDbType.Varchar2)).Value = lCreatedBy;
            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSSTATUS", OracleDbType.Varchar2)).Value = lStatus;
            lcmd.Parameters.Add(new OracleParameter("ID", OracleDbType.Int32)).Direction = ParameterDirection.Output;

            lcmd.CommandText = schema + "." + "USP_FEEDPROCESS";

            try
            {
                lcmd.ExecuteNonQuery();
                lResult = int.Parse(lcmd.Parameters[4].Value.ToString());
            }
            catch (Exception lex)
            {

                ltransaction.Rollback();

                int line;
                string msg = lex.Message;
                line = dbtable.lineNo(lex);
                dbtable.errorlog(msg, "Export Storyboard Excel", SomeGlobalVariables.functionName, line);
                throw new Exception(lex.Message);
            }

            ltransaction.Commit();
            lconnection.Close();
            return lResult;
        }

        //Inserts Feedprocess Detail id
        public int FeedProcessDetails(int lFEEDPROCESSDETAILID, int lFeedProcessId, string lOperation, string lFileName, string lCREATEDBY, string lStatus, string lFileType, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->FeedProcessDetails";
            int lResult = 0;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "USP_FEEDPROCESSDETAILS";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32)).Value = lFEEDPROCESSDETAILID;
            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSID", OracleDbType.Int32)).Value = lFeedProcessId;
            lcmd.Parameters.Add(new OracleParameter("OPERATION", OracleDbType.Varchar2)).Value = lOperation;
            lcmd.Parameters.Add(new OracleParameter("FILENAME", OracleDbType.Varchar2)).Value = lFileName;
            lcmd.Parameters.Add(new OracleParameter("CREATEDBY", OracleDbType.Varchar2)).Value = lCREATEDBY;

            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSDETAILSTATUS", OracleDbType.Varchar2)).Value = lStatus;
            lcmd.Parameters.Add(new OracleParameter("FILETYPE", OracleDbType.Varchar2)).Value = lFileType;

            lcmd.Parameters.Add(new OracleParameter("ID", OracleDbType.Int32)).Direction = ParameterDirection.Output;

            try
            {
                lcmd.ExecuteNonQuery();
                lResult = int.Parse(lcmd.Parameters[7].Value.ToString());
            }
            catch (Exception lex)
            {

                string msg = lex.Message;
                if (!(msg.Contains("PL/SQL: numeric or value error") && msg.Contains("at line 1")))
                {
                    ltransaction.Rollback();
                    int line;
                    line = dbtable.lineNo(lex);
                    dbtable.errorlog(msg, "Feed Process Details", SomeGlobalVariables.functionName, line);
                    throw new Exception(/*lex.Message*/);
                }
            }

            ltransaction.Commit();


            lconnection.Close();
            return lResult;

        }

        //Creates a log report
        public DataTable Dbexcel(int lFeedProcessId, string schema, string constring)
        {
            DataTable dt = new DataTable();
            OracleConnection lconnection = GetOracleConnection(constring);


            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            OracleParameter[] ladd_refer_image = new OracleParameter[2];
            ladd_refer_image[0] = new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32);
            ladd_refer_image[0].Value = lFeedProcessId;
            //ladd_refer_image[1] = new OracleParameter("FILENAME", OracleDbType.NChar);
            //ladd_refer_image[1].Value = lFileName.Replace("LOGREPORT-", "");
            ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
            ladd_refer_image[1].Direction = ParameterDirection.Output;

            foreach (OracleParameter p in ladd_refer_image)
            {
                lcmd.Parameters.Add(p);
            }


            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "SP_EXPORT_LOGREPORT";
            lcmd.CommandType = CommandType.StoredProcedure;
            OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
            dataAdapter.Fill(dt);



            lconnection.Close();

            //lobjcommon.ConvertDataSetToExcel1(lds, lLocation + "\\" + lFileName);

            return dt;



        }

        public DataTable DbexcelResultset(int lFeedProcessDetailId, string schema, string constring)
        {
            DataTable dt = new DataTable();
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            OracleParameter[] ladd_refer_image = new OracleParameter[2];
            ladd_refer_image[0] = new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32);
            ladd_refer_image[0].Value = lFeedProcessDetailId;
            ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
            ladd_refer_image[1].Direction = ParameterDirection.Output;

            foreach (OracleParameter p in ladd_refer_image)
            {
                lcmd.Parameters.Add(p);
            }

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "SP_EXPORT_LOGREPORT_RESULT";
            lcmd.CommandType = CommandType.StoredProcedure;
            OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
            dataAdapter.Fill(dt);

            lconnection.Close();
            return dt;
        }

        public bool DataTagSetMapping(int lFeedProcessDetailId, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->DataTagSetMapping";
            bool lResult = false;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "SP_SAVE_IMPORT_DATATAG";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("feeddetailid", OracleDbType.Int32)).Value = lFeedProcessDetailId;

            try
            {
                lcmd.ExecuteNonQuery();
                lResult = true;
                ltransaction.Commit();
            }
            catch (Exception lex)
            {
                lResult = false;
                int line;
                string msg = lex.Message;
                if (!msg.Contains("PL/SQL: numeric or value error") && !msg.Contains("at line 1"))
                {
                    ltransaction.Rollback();
                    line = dbtable.lineNo(lex);
                    dbtable.errorlog(msg, "SP_SAVE_IMPORT_DATATAG mapping", SomeGlobalVariables.functionName, line);
                    throw new Exception(lex.Message);
                }
                else
                {
                    ltransaction.Commit();
                }
            }
            lconnection.Close();
            return lResult;
        }

        public bool MAPPINGVALIDATIONFORDATASETTAG(int lFeedProcessDetailId, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->MappingValidationForDatasettag";
            bool lResult = false;
            OracleConnection lconnection = GetOracleConnection(constring);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = schema + "." + "SP_VALIDATE_IMPORT_DATASETTAG";
            lcmd.CommandType = CommandType.StoredProcedure;

            lcmd.Parameters.Add(new OracleParameter("FPROCESSDETAILID", OracleDbType.Int32)).Value = lFeedProcessDetailId;
            lcmd.Parameters.Add(new OracleParameter("RESULT", OracleDbType.Varchar2, 300000)).Direction = ParameterDirection.Output;

            try
            {
                lcmd.ExecuteNonQuery();
                string lCheckResult = string.Empty;
                lCheckResult = lcmd.Parameters[1].Value.ToString();
                if (!string.IsNullOrEmpty(lCheckResult) && lCheckResult.ToLower() == "error")
                {
                    lResult = false;
                }
                else
                {
                    lResult = true;
                }
            }
            catch (Exception lex)
            {

                ltransaction.Rollback();

                int line;
                string msg = lex.Message;
                line = dbtable.lineNo(lex);
                dbtable.errorlog(msg, "Mapping Validations For Datasettag", SomeGlobalVariables.functionName, line);
                throw new Exception(/*lex.Message*/);
            }

            ltransaction.Commit();
            lconnection.Close();
            return lResult;

        }
    }
}

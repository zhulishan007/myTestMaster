using MARSUtility.ViewModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MARSUtility
{
    public class SomeGlobalVariables
    {
        public static string functionName = "";

    }
    public class ExportHelper
    {
        //public string schema = "";

        //Initialize oracle connection
        //public OracleConnection GetOracleConnection()
        //{

        //    string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    string configFile = System.IO.Path.Combine(appPath, "MarsConsole.exe.config");
        //    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        //    configFileMap.ExeConfigFilename = configFile;
        //    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        //    string ldataSource = config.AppSettings.Settings["DataSource"].Value;
        //    schema = config.AppSettings.Settings["Schema"].Value;
        //    return new OracleConnection(ldataSource);
        //}
        public OracleConnection GetOracleConnection(string constring)
        {
            return new OracleConnection(constring);
        }

        //Gets data from DB and sets in Variable Model
        public List<VariableExportModel> ExportVariable(string schemaname,string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportVariable";
            try
            {
                
                string lFileName = "variable" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                //Create the data set and table
                Boolean lresult = true;
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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


                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schemaname + "." + "SP_EXPORT_VARIABLE";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];

                List<VariableExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new VariableExportModel
                    {
                        Name = row.Field<string>("Name"),
                        Value = Convert.ToString(row.Field<string>("Value")),
                        Type = Convert.ToString(row.Field<string>("Type")),
                        BaseComp = Convert.ToString(row.Field<string>("Base/Comp")),

                    }).ToList();

                dbtable.errorlog("Fetched all variables from DB", "Export Variable","", 0);

                lconnection.Close();

                return resultList;

            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Variable", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :Exportvariable " + ex.Message);

            }
        }

        //Gets data from DB and sets in Object Model
        public List<ObjectExportModel> ExportObject(string pApplication,string schemaname,string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportObject";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTOBJECT";
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
                dbtable.errorlog("Fetched all objects of application["+ pApplication+"] from DB", "Export Object","", 0);
                return resultList;

            }
            catch(Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Object", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportObject " + ex.Message);

            }
            
        }

        //Gets data from DB and sets in Storyboard Model
        public List<StoryBoardResultExportModel> ExportStoryboard(string projectname,string schemaname,string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportStoryboard";
            bool lResult = false; // if result is flase then error will no suitename found in db.
            try
            {
                lResult = ValidateTestSuiteAndApplication(projectname, "PROJECTNAME", schemaname, constring);
                if (lResult)
                {


                    DataSet lds = new DataSet();
                    DataTable ldt = new DataTable();

                    OracleConnection lconnection = GetOracleConnection(constring);
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

                    lcmd.CommandText = schemaname + "." + "SP_EXPORT_STORYBOARD";
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
                        STORYBOARD_NAME = row.Field<string>("STORYBOARD_NAME") == null ? "" : Convert.ToString(row.Field<string>("STORYBOARD_NAME")),
                        ACTIONNAME = row.Field<string>("ACTIONNAME") == null ? "" : Convert.ToString(row.Field<string>("ACTIONNAME")),
                        STEPNAME = row.Field<string>("STEPNAME") == null ? "" : Convert.ToString(row.Field<string>("STEPNAME")),
                        SUITENAME = row.Field<string>("SUITENAME") == null ? "" : Convert.ToString(row.Field<string>("SUITENAME")),
                        CASENAME = row.Field<string>("CASENAME") == null ? "" : Convert.ToString(row.Field<string>("CASENAME")),
                        DATASETNAME = row.Field<string>("DATASETNAME") == null ? "" : Convert.ToString(row.Field<string>("DATASETNAME")),
                        DEPENDENCY = row.Field<string>("DEPENDENCY") == null ? "" : Convert.ToString(row.Field<string>("DEPENDENCY")),
                    }).ToList();
                    dbtable.errorlog("Fetched all Storyboards of Project["+projectname+"] from DB", "", "", 0);
                    return resultList;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n Project ["+projectname+"] does not exist in the system");
                    dbtable.errorlog("Project [" + projectname + "] does not exist in the system", "", "", 0);
                    return null;

                }
            }
            catch(Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Storyboard", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportStoryboard" + ex.Message);
            }
           
        }

        public List<StoryBoardResultExportModel> ExportStoryboardList(string pstoryboard, string pproject, string lstrConn, string schema)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportStoryboardList";
            try
            {
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
                ladd_refer_image[0] = new OracleParameter("PROJECT", OracleDbType.Varchar2);
                ladd_refer_image[0].Value = pproject;

                ladd_refer_image[1] = new OracleParameter("Storyboardname", OracleDbType.Varchar2);
                ladd_refer_image[1].Value = pstoryboard;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schema + "." + "SP_EXPORT_STORYBOARDNEW";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<StoryBoardResultExportModel> resultList = dt.AsEnumerable().Select(row =>
                new StoryBoardResultExportModel
                {
                    STORYBOARDDETAILID = Convert.ToString(row.Field<long>("STORYBOARDDETAILID")),
                    STORYBOARDID = Convert.ToString(row.Field<long>("STORYBOARDID")),
                    PROJECTID = Convert.ToString(row.Field<long>("PROJECTID")),
                    APPLICATIONNAME = Convert.ToString(row.Field<string>("APPLICATIONNAME")),
                    RUNORDER = Convert.ToString(row.Field<long>("RUNORDER")),
                    PROJECTNAME = Convert.ToString(row.Field<string>("PROJECTNAME")),
                    PROJECTDESCRIPTION = Convert.ToString(row.Field<string>("PROJECTDESCRIPTION")),
                    STORYBOARD_NAME = Convert.ToString(row.Field<string>("STORYBOARD_NAME")),
                    ACTIONNAME = Convert.ToString(row.Field<string>("ACTIONNAME")),
                    STEPNAME = Convert.ToString(row.Field<string>("STEPNAME")),
                    SUITENAME = Convert.ToString(row.Field<string>("SUITENAME")),
                    CASENAME = Convert.ToString(row.Field<string>("CASENAME")),
                    DATASETNAME = Convert.ToString(row.Field<string>("DATASETNAME")),
                    DEPENDENCY = Convert.ToString(row.Field<string>("DEPENDENCY")),
                    TEST_STEP_DESCRIPTION = Convert.ToString(row.Field<string>("TEST_STEP_DESCRIPTION")),

                }).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Storyboard", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from : ExportStoryboardlist " + ex.Message);
            }

        }

        //Gets data from DB and passes Dataset of TestSUite to create excel file
        public bool ExportTestSuite(string pTestSuite ,string TempLocation,string schemaname,string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportTestSuite";
            bool flag = false;
            try
            {
                bool lResult = false; // if result is flase then error will no suitename found in db.
                lResult = ValidateTestSuiteAndApplication(pTestSuite, "SUITENAME",schemaname,constring);
                if (lResult)
                {


                    //string lFileName = System.DateTime.Now.ToString("MM-dd-yyyy") + "-TestCase.xlsx";
                    string lFileName = pTestSuite + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    //Create the data set and table

                    DataSet lds = new DataSet();
                    DataTable ldt = new DataTable();

                    OracleConnection lconnection = GetOracleConnection(constring);
                    lconnection.Open();

                    OracleTransaction ltransaction;
                    ltransaction = lconnection.BeginTransaction();

                    OracleCommand lcmd;
                    lcmd = lconnection.CreateCommand();
                    lcmd.Transaction = ltransaction;



                    OracleParameter[] ladd_refer_image = new OracleParameter[2];
                    ladd_refer_image[0] = new OracleParameter("TESTSUITENAME", OracleDbType.Varchar2);
                    ladd_refer_image[0].Value = pTestSuite;

                    ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                    ladd_refer_image[1].Direction = ParameterDirection.Output;

                    foreach (OracleParameter p in ladd_refer_image)
                    {
                        lcmd.Parameters.Add(p);

                    }


                    //The name of the Procedure responsible for inserting the data in the table.
                    lcmd.CommandText = schemaname + "." + "SP_EXPORT_TESTSUITE";
                    lcmd.CommandType = CommandType.StoredProcedure;
                    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                    dataAdapter.Fill(lds);
                    dbtable.errorlog("Fetched all TestCases of TestSuite["+pTestSuite+"] from DB", "", "", 0);
                    Regex re = new Regex("[;\\/:*?\"<>|&']");
                    lconnection.Close();
                    ExportExcel excel = new ExportExcel();
                    // var FullPath = ConvertDataSetToExcelTestSuite(lds, lFileName, TempLocation);
                    var FullPath = excel.ExportTestSuiteExcel(lds, TempLocation);
                    flag = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    dbtable.errorlog("Test Suite ["+pTestSuite+"] does not exist in the system", "","",0);
                    Console.WriteLine("\n Test Suite [" + pTestSuite + "] does not exist in the system");
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                flag = false;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export TestSuite", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportTestSuite " + ex.Message);
            }
            return flag;
        }

        public bool ExportTestCase(string pTestCase, string pTestSuite, string lstrConn, string schema, string TempLocation)
        {
            bool lResult = false;
            try
            {

                //Create the data set and table
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
                ladd_refer_image[0] = new OracleParameter("TESTSUITENAME", OracleDbType.Varchar2);
                ladd_refer_image[0].Value = pTestSuite;

                ladd_refer_image[1] = new OracleParameter("TESTCASENAME", OracleDbType.Varchar2);
                ladd_refer_image[1].Value = pTestCase;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }


                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schema + "." + "SP_EXPORT_TESTCASE";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);

                Regex re = new Regex("[;\\/:*?\"<>|&']");
                lconnection.Close();
                //var FullPath = ConvertDataSetToExcelTestSuite(lds, lFileName, TempLocation);
                ExportExcel excel = new ExportExcel();
                // var FullPath = ConvertDataSetToExcelTestSuite(lds, lFileName, TempLocation);
                var FullPath = excel.ExportTestSuiteExcel(lds, TempLocation);
                lResult = true;
               
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                lResult = false;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Test Case", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportTestcase " + ex.Message);
            }
            return lResult;
        }

        public bool ExportResultSet(string Tag, string storyboard, int mode, string TempLocation, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportResultSet";
            ExportExcel excel = new ExportExcel();
            bool flag = false;
            try
            {
                bool result = false;
                bool lResult = false; // if result is flase then error will no suitename found in db.
                lResult = ValidateTestSuiteAndApplication(Tag, "PROJECTNAME", schemaname, constring);
                if (lResult)
                {
                    result = ValidateTestSuiteAndApplication(storyboard, "STORYBOARDNAME", schemaname, constring);
                    if (result)
                    {
                        DataSet lds = new DataSet();
                        DataTable ldt = new DataTable();

                        OracleConnection lconnection = GetOracleConnection(constring);
                        lconnection.Open();

                        OracleTransaction ltransaction;
                        ltransaction = lconnection.BeginTransaction();

                        OracleCommand lcmd;
                        lcmd = lconnection.CreateCommand();
                        lcmd.Transaction = ltransaction;

                        OracleParameter[] ladd_refer_image = new OracleParameter[4];
                        ladd_refer_image[0] = new OracleParameter("ResultMode", OracleDbType.Int16);
                        ladd_refer_image[0].Value = mode;

                        ladd_refer_image[1] = new OracleParameter("ProjectName", OracleDbType.Varchar2);
                        ladd_refer_image[1].Value = Tag;

                        ladd_refer_image[2] = new OracleParameter("Storyboardname", OracleDbType.Varchar2);
                        ladd_refer_image[2].Value = storyboard;

                        ladd_refer_image[3] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                        ladd_refer_image[3].Direction = ParameterDirection.Output;

                        foreach (OracleParameter p in ladd_refer_image)
                        {
                            lcmd.Parameters.Add(p);
                        }
                        //The name of the Procedure responsible for inserting the data in the table.
                        lcmd.CommandText = schemaname + "." + "SP_EXPORT_SB_RESULTSET";
                        lcmd.CommandType = CommandType.StoredProcedure;
                        OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                        dataAdapter.Fill(lds);

                        dbtable.errorlog("Fetched all ResultSet of Storyborad[" + storyboard + "] from DB", "", "", 0);
                        lconnection.Close();
                        var FullPath = excel.ExportResultSetExcel(lds, TempLocation);
                        if (FullPath != "")
                            flag = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n Storyboard [" + storyboard + "] does not exist in the system");
                        dbtable.errorlog("Storyboard [" + storyboard + "] does not exist in the system", "", "", 0);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n Project [" + Tag + "] does not exist in the system");
                    dbtable.errorlog("Project [" + Tag + "] does not exist in the system", "", "", 0);
                }
                return flag;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ResultSet", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ResultSet " + ex.Message);
            }

        }

        public bool ExportProjectResultSet(string Tag, int mode, string TempLocation, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportResultSet";
            ExportExcel excel = new ExportExcel();
            bool flag = false;
            try
            {
                bool lResult = false; // if result is flase then error will no suitename found in db.
                lResult = ValidateTestSuiteAndApplication(Tag, "PROJECTNAME", schemaname, constring);
                if (lResult)
                {
                    DataSet lds = new DataSet();
                    DataTable ldt = new DataTable();

                    OracleConnection lconnection = GetOracleConnection(constring);
                    lconnection.Open();

                    OracleTransaction ltransaction;
                    ltransaction = lconnection.BeginTransaction();

                    OracleCommand lcmd;
                    lcmd = lconnection.CreateCommand();
                    lcmd.Transaction = ltransaction;

                    OracleParameter[] ladd_refer_image = new OracleParameter[3];
                    ladd_refer_image[0] = new OracleParameter("ResultMode", OracleDbType.Int16);
                    ladd_refer_image[0].Value = mode;

                    ladd_refer_image[1] = new OracleParameter("ProjectName", OracleDbType.Varchar2);
                    ladd_refer_image[1].Value = Tag;

                    ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                    ladd_refer_image[2].Direction = ParameterDirection.Output;

                    foreach (OracleParameter p in ladd_refer_image)
                    {
                        lcmd.Parameters.Add(p);
                    }

                    //The name of the Procedure responsible for inserting the data in the table.
                    lcmd.CommandText = schemaname + "." + "SP_EXPORT_PROJ_RESULTSET";
                    lcmd.CommandType = CommandType.StoredProcedure;
                    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                    dataAdapter.Fill(lds);

                    dbtable.errorlog("Fetched all ResultSet of Projet[" + Tag + "] from DB", "", "", 0);
                    lconnection.Close();
                    var FullPath = excel.ExportProjectResultSetExcel(lds, TempLocation);
                    if (FullPath != "")
                        flag = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n Project [" + Tag + "] does not exist in the system");
                    dbtable.errorlog("Project [" + Tag + "] does not exist in the system", "", "", 0);
                }
                return flag;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ResultSet", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ResultSet " + ex.Message);
            }
        }

        //Gets data from DB and sets in CompareConfig Model
        public List<CompareConfigExportModel> ExportCompareConfig(string schemaname,string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportCompareConfig";
            bool flag = false;
            try
            {
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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


                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schemaname + "." + "SP_EXPORT_COMPAREPARAM";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                lconnection.Close();
                var dt = new DataTable();
                dt = lds.Tables[0];

                ExportExcel excel = new ExportExcel();
                List<CompareConfigExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new CompareConfigExportModel
                    {
                        datasourcename = Convert.ToString(row.Field<string>("Data Source Name")),
                        sourcetype = Convert.ToString(row.Field<short>("Data Source Type")),
                        details = row.Field<string>("Details ") == null ? "" : Convert.ToString(row.Field<string>("Details "))
                    }).ToList();
                dbtable.errorlog("Fetched all Compare Config rows from DB", "Export Config", "", 0);
                // excel.ExportConfigExcel(lds, exportpath);
                flag = true;

                return resultList;
            }
          catch(Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Config", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportConfig " + ex.Message);
            }
        }

        //validates mapping between TestSuite, Project and Application
        public bool ValidateTestSuiteAndApplication(string lValue, string lOperation,string schema,string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ValidateTestSuiteAndApplication";
            try
            {
                bool lResult = false;
                OracleConnection lconnection = GetOracleConnection(constring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = schema + "." + "USP_VALIDATION";
                lcmd.CommandType = CommandType.StoredProcedure;


                lcmd.Parameters.Add(new OracleParameter("DATAVALUE", OracleDbType.Varchar2)).Value = lValue;
                lcmd.Parameters.Add(new OracleParameter("VALIDATETYPE", OracleDbType.Varchar2)).Value = lOperation;
                lcmd.Parameters.Add(new OracleParameter("ID", OracleDbType.Int32)).Direction = ParameterDirection.Output;



                try
                {
                    lcmd.ExecuteNonQuery();
                    int lCheck = int.Parse(lcmd.Parameters[2].Value.ToString());
                    lResult = (lCheck != 0 ? true : false);
                }
                catch (Exception lex)
                {
                    ltransaction.Rollback();
                    int line;
                    string msg = lex.Message;
                    line = dbtable.lineNo(lex);
                    dbtable.errorlog(msg, "Export Validation of TestSuite/Project and Application", SomeGlobalVariables.functionName, line);

                    throw new Exception(lex.Message);
                }

                ltransaction.Commit();


                lconnection.Close();
                return lResult;

            }

            catch(Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Validation of TestSuite/Project and Application", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ValidateTestSuiteApp " + ex.Message);
            }
        }

        public List<DatasetTagExportModel> ExportDatasetTag(string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTag";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTDATASETTAG";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DatasetTagExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new DatasetTagExportModel
                    {
                        ALIAS_NAME = row.Field<string>("ALIAS_NAME"),
                        DESCRIPTION_INFO = row.Field<string>("DESCRIPTION_INFO"),
                        GROUPNAME = row.Field<string>("GROUPNAME"),
                        SETNAME = row.Field<string>("SETNAME"),
                        FOLDERNAME = row.Field<string>("FOLDERNAME"),
                        EXPECTEDRESULTS = row.Field<string>("EXPECTEDRESULTS"),
                        STEPDESC = row.Field<string>("STEPDESC"),
                        DIARY = row.Field<string>("DIARY"),
                        SEQUENCE = row.Field<long?>("SEQUENCE"),
                    }).Distinct().ToList();
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export DatasetTag", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export DatasetTag", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTag " + ex.Message);
            }
        }

        public List<DataTagCommonViewModel> ExportDatasetTagGroup(string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagGroup";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTDATATAGGROUP";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DataTagCommonViewModel> resultList = dt.AsEnumerable().Select(row =>
                    new DataTagCommonViewModel
                    {
                        Id = row.Field<long>("GROUPID"),
                        Name = row.Field<string>("GROUPNAME"),
                        Description = row.Field<string>("DESCRIPTION"),
                        Active = row.Field<short>("ACTIVE"),
                        
                    }).Distinct().ToList();

                resultList.ToList().ForEach(item =>
                {
                    item.Name = item.Name == null ? "" : item.Name;
                    item.Description = item.Description == null ? "" : item.Description;
                    item.Status = item.Active == 0 ? "InActive" : "Active";
                });
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export ExportDatasetTagGroup", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportDatasetTagGroup", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagGroup " + ex.Message);
            }
        }

        public List<DataTagCommonViewModel> ExportDatasetTagSet(string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagSet";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTDATASETTAGSET";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DataTagCommonViewModel> resultList = dt.AsEnumerable().Select(row =>
                    new DataTagCommonViewModel
                    {
                        Id = row.Field<long>("SETID"),
                        Name = row.Field<string>("SETNAME"),
                        Description = row.Field<string>("DESCRIPTION"),
                        Active = row.Field<short>("ACTIVE"),

                    }).Distinct().ToList();

                resultList.ToList().ForEach(item =>
                {
                    item.Name = item.Name == null ? "" : item.Name;
                    item.Description = item.Description == null ? "" : item.Description;
                    item.Status = item.Active == 0 ? "InActive" : "Active";
                });
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export ExportDatasetTagSet", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportDatasetTagSet", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagSet " + ex.Message);
            }
        }

        public List<DataTagCommonViewModel> ExportDatasetTagFolder(string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagFolder";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTDATATAGFOLDER";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DataTagCommonViewModel> resultList = dt.AsEnumerable().Select(row =>
                    new DataTagCommonViewModel
                    {
                        Id = row.Field<long>("FOLDERID"),
                        Name = row.Field<string>("FOLDERNAME"),
                        Description = row.Field<string>("DESCRIPTION"),
                        Active = row.Field<short>("ACTIVE"),

                    }).Distinct().ToList();

                resultList.ToList().ForEach(item =>
                {
                    item.Name = item.Name == null ? "" : item.Name;
                    item.Description = item.Description == null ? "" : item.Description;
                    item.Status = item.Active == 0 ? "InActive" : "Active";
                });
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export ExportDatasetTagFolder", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportDatasetTagFolder", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagFolder " + ex.Message);
            }
        }

        public List<DatasetTagReportExportModel> ExportReportDatasetTag(string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportReportDatasetTag";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTDATATAGREPORT";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DatasetTagReportExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new DatasetTagReportExportModel
                    {
                        PROJECT_ID = row.Field<long?>("PROJECT_ID"),
                        PROJECT_NAME = row.Field<string>("PROJECT_NAME"),
                        STORYBOARD_ID = row.Field<long?>("STORYBOARD_ID"),
                        STORYBOARD_NAME = row.Field<string>("STORYBOARD_NAME"),
                        RUN_ORDER = row.Field<long?>("RUN_ORDER"),
                        TEST_CASE_NAME = row.Field<string>("TEST_CASE_NAME"),
                        ALIAS_NAME = row.Field<string>("ALIAS_NAME"),
                        DESCRIPTION_INFO = row.Field<string>("DESCRIPTION_INFO"),
                        GROUPNAME = row.Field<string>("GROUPNAME"),
                        GROUPDESCRIPTION = row.Field<string>("GROUPDESCRIPTION"),
                        SETNAME = row.Field<string>("SETNAME"),
                        SETDESCRIPTION = row.Field<string>("SETDESCRIPTION"),
                        FOLDERNAME = row.Field<string>("FOLDERNAME"),
                        FOLDERDESCRIPTION = row.Field<string>("FOLDERDESCRIPTION"),
                        SEQUENCE = row.Field<long?>("SEQUENCE"),
                        EXPECTEDRESULTS = row.Field<string>("EXPECTEDRESULTS"),
                        DIARY = row.Field<string>("DIARY"),
                        STEPDESC = row.Field<string>("STEPDESC"),
                    }).ToList();
                dbtable.errorlog("Fetched all ReportDatasetTag List from DB", "Export ReportDatasetTag", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ReportDatasetTag", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportReportDatasetTag " + ex.Message);
            }
        }

        public List<StoryBoardExportModel> ExportAllStoryborad(string Project, string TempLocation, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportAllStoryborad";
            try
            {
                List<StoryBoardExportModel> resultList = new List<StoryBoardExportModel>();
                bool lResult = false; // if result is flase then error will no suitename found in db.
                lResult = ValidateTestSuiteAndApplication(Project, "PROJECTNAME", schemaname, constring);
                if (lResult)
                {
                    string lFileName = Project + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";

                    DataSet lds = new DataSet();
                    DataTable ldt = new DataTable();

                    OracleConnection lconnection = GetOracleConnection(constring);
                    lconnection.Open();

                    OracleTransaction ltransaction;
                    ltransaction = lconnection.BeginTransaction();

                    OracleCommand lcmd;
                    lcmd = lconnection.CreateCommand();
                    lcmd.Transaction = ltransaction;

                    OracleParameter[] ladd_refer_image = new OracleParameter[2];
                    ladd_refer_image[0] = new OracleParameter("PROJECT", OracleDbType.Varchar2);
                    ladd_refer_image[0].Value = Project;

                    ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                    ladd_refer_image[1].Direction = ParameterDirection.Output;

                    foreach (OracleParameter p in ladd_refer_image)
                    {
                        lcmd.Parameters.Add(p);

                    }
                    //The name of the Procedure responsible for inserting the data in the table.
                    lcmd.CommandText = schemaname + "." + "SP_EXPORT_PROJ_STORYBOARD";
                    lcmd.CommandType = CommandType.StoredProcedure;
                    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                    dataAdapter.Fill(lds);
                    var dt = new DataTable();
                    dt = lds.Tables[0];
                    resultList = dt.AsEnumerable().Select(row =>
                        new StoryBoardExportModel
                        {
                            FOLDERNAME = row.Field<string>("FOLDERNAME"),
                            FOLDERDESC = row.Field<string>("FOLDERDESC"),
                            STORYBOARD_NAME = row.Field<string>("STORYBOARD_NAME"),
                            ACTIONNAME = row.Field<string>("ACTIONNAME"),
                            SUITENAME = row.Field<string>("SUITENAME"),
                            CASENAME = row.Field<string>("CASENAME"),
                            DATASETNAME = row.Field<string>("DATASETNAME"),
                            BTEST_RESULT = row.Field<string>("BTEST_RESULT"),
                            BTEST_RESULT_IN_TEXT = row.Field<string>("BTEST_RESULT_IN_TEXT"),
                            BTEST_BEGIN_TIME = row.Field<DateTime?>("BTEST_BEGIN_TIME"),
                            BTEST_END_TIME = row.Field<DateTime?>("BTEST_END_TIME"),
                            CTEST_RESULT = row.Field<string>("CTEST_RESULT"),
                            CTEST_RESULT_IN_TEXT = row.Field<string>("CTEST_RESULT_IN_TEXT"),
                            CTEST_BEGIN_TIME = row.Field<DateTime?>("CTEST_BEGIN_TIME"),
                            CTEST_END_TIME = row.Field<DateTime?>("CTEST_END_TIME"),
                            BHistid = row.Field<long?>("BHIST_ID"),
                            CHistid = row.Field<long?>("CHIST_ID")
                        }).OrderBy(x => x.STORYBOARD_NAME).ToList();


                    foreach (var item in resultList)
                    {
                        if (item.BHistid > 0 && item.CHistid > 0)
                        {
                            var pResult = GetCompareResultStatusList(schemaname, constring, (long)item.BHistid, (long)item.CHistid);
                            item.CTEST_RESULT = pResult;
                        }
                    }

                    dbtable.errorlog("Fetched all ReportDatasetTag List from DB", "Export ReportDatasetTag", "", 0);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    dbtable.errorlog("Project [" + Project + "] does not exist in the system", "", "", 0);
                    Console.WriteLine("\n Project [" + Project + "] does not exist in the system");
                }
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export All Storyborad", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportAllStoryborad " + ex.Message);
            }
        }

        public string GetCompareResultStatusList(string schema, string lconstring, long BhistedId, long ChistedId)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->GetCompareResultStatusList";
            try
            {
               
                DataSet lds = new DataSet();
                DataTable ldt = new DataTable();

                OracleConnection lconnection = GetOracleConnection(lconstring);
                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;


                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("Compare_HISTID", OracleDbType.Long);
                ladd_refer_image[0].Value = ChistedId;

                ladd_refer_image[1] = new OracleParameter("Baseline_HISTID", OracleDbType.Long);
                ladd_refer_image[1].Value = BhistedId;

                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }

                lcmd.CommandText = schema + "." + "SP_GET_STORYBOARD_RESULT";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                IList<TestResultExportModel> result = dt.AsEnumerable().Select(row =>
                  new TestResultExportModel
                  {
                      BaselineStepId = row.Field<long?>("BaselineStepID"),
                      CompareStepId = row.Field<long?>("CompareStepID"),
                      BreturnValues = row.Field<string>("Baseline_RETURN_VALUES"),
                      CreturnValues = row.Field<string>("Compare_RETURN_VALUES"),
                      InputValueSetting = row.Field<string>("INPUT_VALUE_SETTING"),
                      Keyword = row.Field<string>("key_word_name"),
                      ActualInputData = row.Field<string>("ACTUAL_INPUT_DATA"),
                      COMMENT = row.Field<string>("COMMENT")
                  }).ToList();

                result.ToList().ForEach(item =>
                {
                    item.BaselineStepId = item.BaselineStepId == null ? 0 : item.BaselineStepId;
                    item.CompareStepId = item.CompareStepId == null ? 0 : item.CompareStepId;
                    item.BreturnValues = item.BreturnValues == null ? "" : item.BreturnValues.Trim();
                    item.CreturnValues = item.CreturnValues == null ? "" : item.CreturnValues.Trim();
                    item.InputValueSetting = item.InputValueSetting == null ? "" : item.InputValueSetting.Trim();
                    item.ActualInputData = item.ActualInputData == null ? "" : item.ActualInputData.Trim();
                    item.COMMENT = item.COMMENT == null ? "" : item.COMMENT.Trim();

                    if (item.Keyword == "CaptureValue")
                        item.Result = "";
                    else if (item.COMMENT.Contains("TOL:"))
                    {
                        var splitTOL = item.COMMENT.Split(' ');
                        var lfun = splitTOL[0].Trim();
                        var lparameter = splitTOL[1].Trim();

                        bool flagDP = decimal.TryParse(lparameter, out decimal i);
                        bool flagDB = decimal.TryParse(item.BreturnValues, out decimal j);
                        bool flagDC = decimal.TryParse(item.CreturnValues, out decimal k);

                        bool flagIP = int.TryParse(lparameter, out int ii);
                        bool flagIB = int.TryParse(item.BreturnValues, out int jj);
                        bool flagIC = int.TryParse(item.CreturnValues, out int kk);

                        if (lfun.Contains("TOL_COMPARE") && (flagDP || flagIP) && (flagDB || flagIB) && (flagDC || flagIC))
                        {
                            item.BreturnValues = item.BreturnValues.Replace(",", "");
                            item.CreturnValues = item.CreturnValues.Replace(",", "");
                            lparameter = lparameter.Replace(",", "");
                            try
                            {
                                if (Math.Abs((Convert.ToDecimal(item.BreturnValues) - Convert.ToDecimal(item.CreturnValues))) < Convert.ToDecimal(lparameter))
                                {
                                    item.Result = "TRUE";
                                }
                                else
                                {
                                    item.Result = "FALSE";
                                }
                            }
                            catch (Exception ex)
                            {
                                item.Result = "FALSE";
                            }
                        }
                        else
                        {
                            item.Result = item.BreturnValues == item.CreturnValues ? "TRUE" : "FALSE";
                        }
                    }
                    else
                        item.Result = item.BreturnValues == item.CreturnValues ? "TRUE" : "FALSE";
                });

                var lList = result.ToList();
                //var lList = result.Where(x => !string.IsNullOrEmpty(x.BreturnValues) || !string.IsNullOrEmpty(x.CreturnValues)).ToList();
                var TrueCount = lList.Count(x => x.Result == "TRUE");
                var FalseCount = lList.Count(x => x.Result == "FALSE");
                var lPass = "PASS";
                if (TrueCount > 0 && FalseCount > 0)
                {
                    lPass = "PARTIAL";
                }
                else if (TrueCount == 0 && FalseCount > 0)
                {
                    lPass = "FAIL";
                }
                else
                {
                    lPass = "PASS";
                }
                return lPass;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export All Storyborad compare Result set", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :GetCompareResultStatusList " + ex.Message);
            }
        }

        public List<DatasetTagReportExportModel> ExportReportDatasetTagById(string schemaname, string constring, string projectIds, string storyboradIds,string FolderName)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportReportDatasetTagById";
            try
            {
                DataTable ldt = new DataTable();
                DataSet lds = new DataSet();

                OracleConnection lconnection = GetOracleConnection(constring);
                lconnection.Open();
                OracleCommand lcmd = lconnection.CreateCommand();
                lcmd.CommandText = $@"select distinct TP.PROJECT_ID, tp.project_name,tss.storyboard_id,tss.storyboard_name, tpj.run_order, tsc.test_case_name, ts.alias_name,TS.DESCRIPTION_INFO, tg.groupname, tg.description as GROUPDESCRIPTION,  
                               tts.setname, tts.description as SETDESCRIPTION, tf.foldername, tf.description as FOLDERDESCRIPTION,dts.EXPECTEDRESULTS, dts.sequence,dts.stepdesc, dts.diary from T_TEST_DATASETTAG dts 
                               join t_test_data_summary ts ON dts.datasetId = ts.data_summary_id
                               left join t_test_group tg on tg.groupid = dts.groupid
                               left join T_test_set tts on tts.setid = dts.setid
                               left join t_test_folder tf on tf.folderid = dts.folderid
                               join REL_TC_DATA_SUMMARY rts on rts.data_summary_id = ts.data_summary_id
                               join t_test_case_summary tsc on tsc.test_case_id = rts.test_case_id
                               join t_proj_tc_mgr tpj on tpj.test_case_id = tsc.test_case_id
                               join t_storyboard_dataset_setting tdss on tdss.data_summary_id = dts.datasetId and tpj.storyboard_detail_id=tdss.storyboard_detail_id
                               join t_storyboard_summary tss on tss.storyboard_id = tpj.storyboard_id
                               join T_TEST_PROJECT tp on tp.project_id = tpj.project_id 
                               where  tf.FOLDERNAME ='{FolderName}'  and  TP.PROJECT_ID in ({projectIds}) and tss.storyboard_id in ({storyboradIds})
                               order by  tg.groupname,tpj.run_order ";
                /*OracleTransaction ltransaction;
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

                lcmd.CommandText = schemaname + "." + "SP_EXPORT_EXPORTDATATAGREPORT";
                lcmd.CommandType = CommandType.StoredProcedure;*/
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);
                var dt = new DataTable();
                dt = lds.Tables[0];
                List<DatasetTagReportExportModel> resultList = dt.AsEnumerable().Select(row =>
                    new DatasetTagReportExportModel
                    {
                        PROJECT_ID = row.Field<long?>("PROJECT_ID"),
                        PROJECT_NAME = row.Field<string>("PROJECT_NAME"),
                        STORYBOARD_ID = row.Field<long?>("STORYBOARD_ID"),
                        STORYBOARD_NAME = row.Field<string>("STORYBOARD_NAME"),
                        RUN_ORDER = row.Field<long?>("RUN_ORDER"),
                        TEST_CASE_NAME = row.Field<string>("TEST_CASE_NAME"),
                        ALIAS_NAME = row.Field<string>("ALIAS_NAME"),
                        DESCRIPTION_INFO = row.Field<string>("DESCRIPTION_INFO"),
                        GROUPNAME = row.Field<string>("GROUPNAME"),
                        GROUPDESCRIPTION = row.Field<string>("GROUPDESCRIPTION"),
                        SETNAME = row.Field<string>("SETNAME"),
                        SETDESCRIPTION = row.Field<string>("SETDESCRIPTION"),
                        FOLDERNAME = row.Field<string>("FOLDERNAME"),
                        FOLDERDESCRIPTION = row.Field<string>("FOLDERDESCRIPTION"),
                        SEQUENCE = row.Field<long?>("SEQUENCE"),
                        EXPECTEDRESULTS = row.Field<string>("EXPECTEDRESULTS"),
                        DIARY = row.Field<string>("DIARY"),
                        STEPDESC = row.Field<string>("STEPDESC"),
                    }).ToList();

                /*List<int> projectId = projectIds.Split(',').Select(int.Parse).ToList();
                List<int> storyboradId = storyboradIds.Split(',').Select(int.Parse).ToList();
                resultList = resultList.Where(a => a.FOLDERNAME == FolderName).ToList();
                resultList = resultList.Where(a => projectId.Any(b => a.PROJECT_ID == b)).ToList();
                resultList = resultList.Where(a => storyboradId.Any(b => a.STORYBOARD_ID == b)).ToList();*/

                dbtable.errorlog("Fetched all ReportDatasetTag List from DB", "Export ExportReportDatasetTagById", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportReportDatasetTagById", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportReportDatasetTagById " + ex.Message);
            }
        }

        public List<DatasetTagExportModel> ExportDatasetTag(DbCommand lcmd, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTag";
            List<DatasetTagExportModel> resultList = new List<DatasetTagExportModel>();
            try
            {
                lcmd.CommandText = @"select ts.alias_name,TS.DESCRIPTION_INFO, tg.groupname,   
                                       tts.setname, tf.foldername,dts.expectedresults, 
                                       dts.stepdesc, dts.diary,dts.sequence from T_TEST_DATASETTAG dts 
                                       join t_test_data_summary ts ON dts.datasetId = ts.data_summary_id
                                       left join t_test_group tg on tg.groupid = dts.groupid
                                       left join T_test_set tts on tts.setid = dts.setid
                                       left join t_test_folder tf on tf.folderid = dts.folderid
                                       order by dts.folderid, dts.sequence";
                lcmd.CommandType = CommandType.Text;
                using (DbDataReader dr = lcmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DatasetTagExportModel dataset = new DatasetTagExportModel();
                        dataset.ALIAS_NAME = GetDBValue<string>(dr["alias_name"], string.Empty);
                        dataset.DESCRIPTION_INFO = GetDBValue<string>(dr["DESCRIPTION_INFO"], string.Empty);
                        dataset.GROUPNAME = GetDBValue<string>(dr["groupname"], string.Empty);
                        dataset.SETNAME = GetDBValue<string>(dr["setname"], string.Empty);
                        dataset.FOLDERNAME = GetDBValue<string>(dr["foldername"], string.Empty);
                        dataset.EXPECTEDRESULTS = GetDBValue<string>(dr["expectedresults"], string.Empty);
                        dataset.STEPDESC = GetDBValue<string>(dr["stepdesc"], string.Empty);
                        dataset.DIARY = GetDBValue<string>(dr["diary"], string.Empty);
                        dataset.SEQUENCE = GetDBValue<long?>(dr["sequence"], null);
                        resultList.Add(dataset);
                    }
                }
                
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export DatasetTag", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export DatasetTag", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTag " + ex.Message);
            }
        }

        public List<DataTagCommonViewModel> ExportDatasetTagGroup(DbCommand lcmd,string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagGroup";
            List<DataTagCommonViewModel> resultList = new List<DataTagCommonViewModel>();
            try
            {
                lcmd.CommandText = @"select groupid, groupname,description,active from t_test_group";
                lcmd.CommandType = CommandType.Text;
                using (DbDataReader dr = lcmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DataTagCommonViewModel datatag = new DataTagCommonViewModel();
                        datatag.Id = GetDBValue<long>(dr["GROUPID"], 0);
                        datatag.Name = GetDBValue<string>(dr["GROUPNAME"], string.Empty);
                        datatag.Description = GetDBValue<string>(dr["DESCRIPTION"], string.Empty);
                        datatag.Active = GetDBValue<short>(dr["ACTIVE"],0);
                        resultList.Add(datatag);
                    }
                }
                resultList.ToList().ForEach(item =>
                {
                    item.Status = item.Active == 0 ? "InActive" : "Active";
                });
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export ExportDatasetTagGroup", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportDatasetTagGroup", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagGroup " + ex.Message);
            }
        }

        public List<DataTagCommonViewModel> ExportDatasetTagSet(DbCommand lcmd, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagSet";
            List<DataTagCommonViewModel> resultList = new List<DataTagCommonViewModel>();
            try
            {
                lcmd.CommandText = @" select setid, setname,description, active from T_test_set ";
                lcmd.CommandType = CommandType.Text;
                using (DbDataReader dr = lcmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DataTagCommonViewModel datatag = new DataTagCommonViewModel();
                        datatag.Id = GetDBValue<long>(dr["SETID"], 0);
                        datatag.Name = GetDBValue<string>(dr["SETNAME"], string.Empty);
                        datatag.Description = GetDBValue<string>(dr["DESCRIPTION"], string.Empty);
                        datatag.Active = GetDBValue<short>(dr["ACTIVE"], 0);
                        resultList.Add(datatag);
                    }
                }
                
                resultList.ToList().ForEach(item =>
                {
                    item.Status = item.Active == 0 ? "InActive" : "Active";
                });
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export ExportDatasetTagSet", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportDatasetTagSet", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagSet " + ex.Message);
            }
        }

        public List<DataTagCommonViewModel> ExportDatasetTagFolder(DbCommand lcmd, string schemaname, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagFolder";
            List<DataTagCommonViewModel> resultList = new List<DataTagCommonViewModel>();
            try
            {
                lcmd.CommandText = @" select folderid, foldername,description ,active from t_test_folder ";
                lcmd.CommandType = CommandType.Text;
                using (DbDataReader dr = lcmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DataTagCommonViewModel datatag = new DataTagCommonViewModel();
                        datatag.Id = GetDBValue<long>(dr["FOLDERID"], 0);
                        datatag.Name = GetDBValue<string>(dr["FOLDERNAME"], string.Empty);
                        datatag.Description = GetDBValue<string>(dr["DESCRIPTION"], string.Empty);
                        datatag.Active = GetDBValue<short>(dr["ACTIVE"], 0);
                        resultList.Add(datatag);
                    }
                }

                resultList.ToList().ForEach(item =>
                { 
                    item.Status = item.Active == 0 ? "InActive" : "Active";
                });
                dbtable.errorlog("Fetched all DatasetTag List from DB", "Export ExportDatasetTagFolder", "", 0);
                return resultList;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ExportDatasetTagFolder", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagFolder " + ex.Message);
            }
        }

        public  T GetDBValue<T>(object value, T defaultValue)
        {
            if (System.DBNull.Value != value)
            {
                return (T)value; 
            }

            return defaultValue;
        }
    }
}

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MARSUtility
{
    public class ImportExcel
    {
        //Initialize Oracle connection
        public static OracleConnection GetOracleConnection(string constring)
        {
            return new OracleConnection(constring);
        }
        static CommonHelper ObjCommon = new CommonHelper();
        #region Variable Import
        //Read excel file of Variable and creates entry in staging table
        public static bool ImportExcelVariable(string pFilePath, int pFEEDPROCESSDETAILID, string schema, string constring)
        {
            bool lResult = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelVariable";
            try
            {
                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
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
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import Variable Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ImportExcelVariable " + ex.Message);

            }

        }
        //Checks variable's excel validations
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
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->VariableValidation";
            try
            {
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

                        ErrorLogExcel(hedarlst[0].CellReference, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[1]).ToUpper() != "VALUE")
                    {
                        flag = 1;
                        title = "VALUE Tag";
                        Description = "VALUE Tag  NOT FOUND";
                        ErrorLogExcel(hedarlst[1].CellReference, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[2]).ToUpper() != "TYPE")
                    {
                        flag = 1;
                        title = "TYPE";
                        Description = "TYPE TAG NOT FOUND";
                        ErrorLogExcel(hedarlst[2].CellReference, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[3]).ToUpper() != "BASE/COMP")
                    {
                        flag = 1;
                        title = "Base/Comp";
                        Description = "Base/Comp TAG NOT FOUND";
                        ErrorLogExcel(hedarlst[3].CellReference, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                }

                for (int i = 1; i < rows.Count; i++)
                {
                    var celllst = rows[i].Descendants<Cell>().ToList();

                    if (celllst.Any())
                    {
                        var a = celllst[0].CellReference;
                        var b = celllst[2].CellReference;
                        var c = celllst[3].CellReference;
                        if (celllst[0].CellValue == null)
                        {
                            title = "Non Empty field";
                            Description = "Field must not be empty";
                            ErrorLogExcel(a, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                        if (GetValue(doc, celllst[2]) != "GLOBAL_VAR" && GetValue(doc, celllst[2]) != "MODAL_VAR" && GetValue(doc, celllst[2]) != "LOOP_VAR")
                        {
                            title = "Type Value";
                            Description = "type value is not proper";
                            ErrorLogExcel(b, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                        if (GetValue(doc, celllst[3]) != "BASELINE" && GetValue(doc, celllst[3]) != "COMPARE" && GetValue(doc, celllst[3]) != null && GetValue(doc, celllst[3]) != "")
                        {
                            title = "Base/comp Value";
                            Description = "Base/comp Value is not proper";
                            ErrorLogExcel(c, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                }
                return td;
            }

            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "VariableValidation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :VariableValidation " + ex.Message);
            }
        }
        #endregion

        #region Object Import

        //Read excel file of Object and creates entry in staging table
        public static bool ImportExcelObject(string pFilePath, int pFEEDPROCESSDETAILID, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelObject";
            try
            {
                bool lResult = false;
                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
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
                        dr["OBJECTNAME"] = GetValue(doc, celllst[0]).Trim();
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
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import Excel Object", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from:importExcelObject" + ex.Message);
            }
        }

        //Checks Object's excel validations
        static DataTable ObjectValidation(List<Row> rows, SpreadsheetDocument doc)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ObjectValidation";
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
            try
            {
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
                        ErrorlogExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }

                    if (GetValue(doc, hedarlst[0]).ToUpper() != "OBJECT NAME")
                    {
                        flag = 1;
                        title = "OBJECT_HAPPY_NAME";
                        Description = "OBJECT_HAPPY_NAME TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[1]).ToUpper() != "TYPE")
                    {
                        flag = 1;
                        title = "TYPE";
                        Description = "TYPE TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[2]).ToUpper() != "QUICK_ACCESS")
                    {
                        flag = 1;
                        title = "QUICK_ACCESS";
                        Description = "QUICK_ACCESS TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[3]).ToUpper() != "PARENT")
                    {
                        flag = 1;
                        title = "PARENT";
                        Description = "PARENT TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[4]).ToUpper() != "COMMENT")
                    {
                        flag = 1;
                        title = "COMMENT";
                        Description = "COMMENT TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 4, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[5]).ToUpper() != "ENUM_TYPE")
                    {
                        flag = 1;
                        title = "ENUM_TYPE";
                        Description = "ENUM_TYPE TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 5, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[6]).ToUpper() != "SQL")
                    {
                        flag = 1;
                        title = "SQL";
                        Description = "SQL TAG NOT FOUND";
                        Application_name = GetValue(doc, fristRow[0]);
                        ErrorlogExcel(2, 6, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }

                    if (fristRow.Count > 1)
                    {
                        for (int j = 1; j < fristRow.Count; j++)
                        {
                            flag = 1;
                            title = "Empty field";
                            Description = "Field must be empty";
                            Application_name = GetValue(doc, fristRow[0]);
                            ErrorlogExcel(1, Convert.ToInt32(GetColumnIndex(fristRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                ErrorlogExcel(k + 1, Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                ErrorlogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                            if (celllst[1].CellValue == null)
                            {
                                flag = 1;
                                title = " NON Empty field";
                                Description = "Field must NOT be empty";
                                Application_name = GetValue(doc, fristRow[0]);
                                ErrorlogExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                            if (celllst[2].CellValue == null)
                            {
                                flag = 1;
                                title = " NON Empty field";
                                Description = "Field must NOT be empty";
                                Application_name = GetValue(doc, fristRow[0]);
                                ErrorlogExcel(i + 1, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                            if (celllst[3].CellValue == null)
                            {
                                flag = 1;
                                title = " NON Empty field";
                                Description = "Field must NOT be empty";
                                Application_name = GetValue(doc, fristRow[0]);
                                ErrorlogExcel(i + 1, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                        }
                    }
                }
                return td;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Object Validation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ObjectValidation " + ex.Message);
            }


        }
        #endregion


        #region Storyboard Import
        //Read excel file of Storyboard and creates entry in staging table
        public static bool ImportExcelStoryboard(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string lstrConn, string schema)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelStoryboard";
            try
            {
                bool lResult = true;
                string lProjectName = string.Empty, lProjectDesc = string.Empty, lStoryBoardName = string.Empty;
                string lApplication = string.Empty, lNewApplicationName = string.Empty;
                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
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
                            lResult = true;
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
                                //return lResult;
                            }
                            if (lResult)
                            {
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
                                        lNewApplicationName = GetValue(doc, fristRow[m]).Trim();
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

                                lProjectName = secondRow[1].CellValue == null ? "" : GetValue(doc, secondRow[1]).Trim();
                                lProjectDesc = thirdRow[1].CellValue == null ? "" : GetValue(doc, thirdRow[1]).Trim();
                                lStoryBoardName = fourthRow[1].CellValue == null ? "" : GetValue(doc, fourthRow[1]).Trim();

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
                                        dr["ACTIONNAME"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]).Trim();
                                        dr["STEPNAME"] = "";
                                        dr["SUITENAME"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]).Trim();
                                        dr["CASENAME"] = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]).Trim();
                                        dr["DATASETNAME"] = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]).Trim();
                                        dr["DEPENDENCY"] = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]).Trim();
                                    }
                                    else
                                    {
                                        dr["ACTIONNAME"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]).Trim();
                                        dr["STEPNAME"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]).Trim();
                                        dr["SUITENAME"] = celllst[3].CellValue == null ? "" : GetValue(doc, celllst[3]).Trim();
                                        dr["CASENAME"] = celllst[4].CellValue == null ? "" : GetValue(doc, celllst[4]).Trim();
                                        dr["DATASETNAME"] = celllst[5].CellValue == null ? "" : GetValue(doc, celllst[5]).Trim();
                                        dr["DEPENDENCY"] = celllst[6].CellValue == null ? "" : GetValue(doc, celllst[6]).Trim();
                                    }
                                    dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                    dr["TABNAME"] = tabName;

                                    dtImport.Rows.Add(dr);
                                    dtImport.AcceptChanges();
                                }
                            }

                        }
                        if (lResult == false)
                            dtImport = null;
                        if (dtImport != null)
                        {
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
                                        lResult = false;
                                        ltransaction.Rollback();
                                        throw new Exception(lex.Message);
                                    }
                                    ltransaction.Commit();
                                }
                            }
                        }

                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import Storyboard Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :importExcelStoryboard " + ex.Message);

            }
        }

        //Checks Storyboard's excel validations
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
            try
            {
                if (rows.Any())
                {
                    var fristRow = rows[0].Descendants<Cell>().ToList();
                    var secondRow = rows[1].Descendants<Cell>().ToList();
                    var thirdRow = rows[2].Descendants<Cell>().ToList();
                    var fourthRow = rows[3].Descendants<Cell>().ToList();
                    var headerlst = rows[4].Descendants<Cell>().ToList();


                    if (GetValue(doc, fristRow[0]).ToUpper() != "APPLICATION")
                    {
                        flag = 1;
                        title = "APPLICATION TAG";
                        Description = "APPLICATION TAG NOT FOUND";
                        ErrorlogExcel(1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }

                    if (fristRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                    if (GetValue(doc, secondRow[0]).ToUpper() != "PROJECT NAME")
                    {
                        flag = 1;
                        title = "Project Name TAG";
                        Description = "Project Name TAG NOT FOUND";
                        ErrorlogExcel(2, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }

                    if (secondRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(2, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                    ErrorlogExcel(2, Convert.ToInt32(GetColumnIndex(secondRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                }
                            }
                        }
                    }

                    if (GetValue(doc, thirdRow[0]).ToUpper() != "PROJECT DESCRIPTION")
                    {
                        flag = 1;
                        title = "Project Description TAG";
                        Description = "Project Description TAG NOT FOUND";
                        ErrorlogExcel(3, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }

                    if (thirdRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(3, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                    ErrorlogExcel(3, Convert.ToInt32(GetColumnIndex(thirdRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                }
                            }
                        }
                    }

                    if (GetValue(doc, fourthRow[0]).ToUpper() != "STORYBOARD NAME")
                    {
                        flag = 1;
                        title = "Storyboard Name TAG";
                        Description = "Storyboard Name TAG NOT FOUND";
                        ErrorlogExcel(4, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                    }

                    if (fourthRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(4, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                    ErrorlogExcel(4, Convert.ToInt32(GetColumnIndex(fourthRow[j].CellReference)), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                }
                            }
                        }
                    }

                    if (headerlst.Any())
                    {

                        if (GetValue(doc, headerlst[0]).ToUpper() != "ROW NUMBER")
                        {
                            flag = 1;
                            title = "Order TAG";
                            Description = "Order TAG NOT FOUND";
                            ErrorlogExcel(5, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }


                        if (GetValue(doc, headerlst[1]).ToUpper() != "ACTION")
                        {
                            flag = 1;
                            title = "Action TAG";
                            Description = "Action TAG NOT FOUND";
                            ErrorlogExcel(5, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }


                        if (GetValue(doc, headerlst[2]).ToUpper() != "STEP NAME")
                        {
                            flag = 1;
                            title = "Step Name TAG";
                            Description = "Step Name TAG NOT FOUND";
                            ErrorlogExcel(5, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }

                        if (GetValue(doc, headerlst[3]).ToUpper() != "TEST SUIT NAME")
                        {
                            flag = 1;
                            title = "Test Suit Name TAG";
                            Description = "Test Suit Name TAG NOT FOUND";
                            ErrorlogExcel(5, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }

                        if (GetValue(doc, headerlst[4]).ToUpper() != "TEST CASE NAME")
                        {
                            flag = 1;
                            title = "Test Case Name TAG";
                            Description = "Test Case Name TAG NOT FOUND";
                            ErrorlogExcel(5, 4, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }


                        if (GetValue(doc, headerlst[5]).ToUpper() != "DATA SET NAME")
                        {
                            flag = 1;
                            title = "Data Set Name TAG";
                            Description = "Data Set Name TAG NOT FOUND";
                            ErrorlogExcel(5, 5, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                        }

                        if (GetValue(doc, headerlst[6]).ToUpper() != "DEPENDENCY")
                        {
                            flag = 1;
                            title = "Dependency TAG";
                            Description = "Dependency TAG NOT FOUND";
                            ErrorlogExcel(5, 6, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                ErrorlogExcel(k + 1, Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                ErrorlogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                                ErrorlogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            var c = GetColumnIndex(celllst[0].CellReference);
                                            flag = 1;
                                            title = "Empty field";
                                            Description = "Field should be Empty";
                                            ErrorlogExcel(Convert.ToInt32(rows[i].RowIndex.ToString()), Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                                            for (int p = 1; p < celllst.Count; p++)
                                            {
                                                c = GetColumnIndex(celllst[p].CellReference);
                                                flag = 1;
                                                title = "Empty field";
                                                Description = "Field should be Empty";
                                                ErrorlogExcel(Convert.ToInt32(rows[i].RowIndex.ToString()), Convert.ToInt32(c), Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                ErrorlogExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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
                                ErrorlogExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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

                                ErrorlogExcel(i + 1, 3, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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

                                ErrorlogExcel(i + 1, 4, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
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

                                ErrorlogExcel(i + 1, 5, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, TestCase_name, TabName);
                            }
                        }
                    EndCondition:;
                    }
                }
                return td;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Storyboard Validation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :StoryboardValidation " + ex.Message);
            }
        }
        #endregion

        #region ResultSet Import
        public static bool ImportExcelResultSet(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string project, string name, string desc, int resultmode, string lstrConn, string schema)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelResultSet";
            try
            {
                bool lResult = true;
                string lProjectName = string.Empty, lDesc = string.Empty, lName = string.Empty;
                string lResultMode = string.Empty, lTestCase = string.Empty, lDataset = string.Empty;
                string lRowNumber = string.Empty, lStoryboradName = string.Empty, columnindex = string.Empty, columnindex2 = string.Empty;
                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();
                    lResult = true;
                    if (worksheetcount > 0)
                    {
                        DataTable dtImport = new DataTable();
                        dtImport.Columns.Add("FEEDPROCESSDETAILID");
                        dtImport.Columns.Add("PROJECTNAME");
                        dtImport.Columns.Add("STORYBOARDNAME");
                        dtImport.Columns.Add("RESULTMODE");
                        dtImport.Columns.Add("TESTCASENAME");
                        dtImport.Columns.Add("DATASETNAME");
                        dtImport.Columns.Add("ROWNUMBER");
                        dtImport.Columns.Add("NAME");
                        dtImport.Columns.Add("DESCRIPTITON");
                        dtImport.Columns.Add("OBJTAG");
                        dtImport.Columns.Add("OBJVALUE");
                        dtImport.AcceptChanges();
                        for (int i = 0; i < worksheetcount; i++)
                        {
                            //lResult = true;
                            Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(i);
                            Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                            List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                            var tabName = mysheet.Name;
                            DataTable dt = new DataTable();

                            dt = ValidateResultSet(rows, doc, tabName, project, resultmode, name);
                            if (dt.Rows.Count != 0)
                            {
                                lResult = false;
                                dbtable.dt_Log.Merge(dt);
                            }

                            if (lResult)
                            {
                                lProjectName = project;
                                lDesc = desc;
                                lName = name;
                                lResultMode = Convert.ToString(resultmode);

                                var row2 = rows[1].Descendants<Cell>().ToList();
                                lStoryboradName = row2[1].CellValue == null ? "" : GetValue(doc, row2[1]);

                                var row5 = rows[4].Descendants<Cell>().ToList();
                                var sixRow = rows[5].Descendants<Cell>().ToList();
                                var sevenRow = rows[6].Descendants<Cell>().ToList();
                                string cell = row5[0].CellReference.ToString();
                                int rowcell, acell = getIndexofNumber(cell);
                                string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                rowcell = Convert.ToInt32(Numberpartcell);
                                string Stringpart = cell.Substring(0, acell);
                                if (rowcell != 5)
                                {
                                    row5 = rows[3].Descendants<Cell>().ToList();
                                    sixRow = rows[4].Descendants<Cell>().ToList();
                                    sevenRow = rows[5].Descendants<Cell>().ToList();
                                }
                                //for (int m = 0; m < row5.Count; m += 2)
                                for (int m = 0; m < row5.Count; m++)
                                {
                                    if (row5[m].CellValue != null)
                                    {
                                        var cellrefrence = row5[m].CellReference.ToString();
                                        //columnindex = Convert.ToString(cellrefrence.Substring(0, cellrefrence.Length - 1));

                                        int a = getIndexofNumber(cellrefrence);
                                        string Numberpart = cellrefrence.Substring(a, cellrefrence.Length - a);
                                        columnindex = cellrefrence.Substring(0, a);

                                        var Incrementcolumn2 = ExportExcel.IncrementColumnCellReference(cellrefrence);

                                        int aa = getIndexofNumber(Incrementcolumn2);
                                        string Numberpart2 = Incrementcolumn2.Substring(aa, Incrementcolumn2.Length - aa);
                                        columnindex2 = Incrementcolumn2.Substring(0, aa);

                                        var rowval = GetValue(doc, row5[m]);
                                        var rowarray = rowval.Split(':');

                                        if (rowarray.Count() > 3)
                                        {
                                            lTestCase = Convert.ToString(rowarray[1]);
                                            lTestCase = Convert.ToString(lTestCase.Substring(0, lTestCase.Length - 3));
                                            lDataset = Convert.ToString(rowarray[2]);
                                            lDataset = Convert.ToString(lDataset.Substring(0, lDataset.Length - 4));
                                            lRowNumber = Convert.ToString(rowarray[3]);
                                        }

                                        if (sixRow.Any() && string.IsNullOrEmpty(name))
                                        {
                                            string cellcolumnindex2 = string.Empty, cellrefrence2 = string.Empty;
                                            if (sixRow[m + 1].CellReference != null)
                                            {
                                                cellrefrence2 = sixRow[m + 1].CellReference.ToString();

                                                int sixaa = getIndexofNumber(cellrefrence2);
                                                string sixNumberpart2 = cellrefrence2.Substring(sixaa, cellrefrence2.Length - sixaa);
                                                cellcolumnindex2 = cellrefrence2.Substring(0, sixaa);
                                            }
                                            if (cellcolumnindex2 == columnindex2)
                                                lName = GetValue(doc, sixRow[m + 1]);
                                        }

                                        if (sevenRow.Any() && string.IsNullOrEmpty(desc))
                                        {
                                            string cellcolumnindex2 = string.Empty, cellrefrence2 = string.Empty;
                                            if (sevenRow[m + 1].CellReference != null)
                                            {
                                                cellrefrence2 = sevenRow[m + 1].CellReference.ToString();

                                                int sevaa = getIndexofNumber(cellrefrence2);
                                                string sevNumberpart2 = cellrefrence2.Substring(sevaa, cellrefrence2.Length - sevaa);
                                                cellcolumnindex2 = cellrefrence2.Substring(0, sevaa);
                                            }
                                            if (cellcolumnindex2 == columnindex2)
                                                lDesc = GetValue(doc, sevenRow[m + 1]);
                                        }

                                        bool Rowflag = true;
                                        //for (int j = 8; j < rows.Count; j++)
                                        //{
                                        //    if (rowcell != 5 && Rowflag)
                                        //    {
                                        //        j = 7;
                                        //        Rowflag = false;
                                        //    }
                                        for (int j = 7; j < rows.Count; j++)
                                        {
                                            if (rowcell == 5 && Rowflag)
                                            {
                                                j = 8;
                                                Rowflag = false;
                                            }

                                            var celllst = rows[j].Descendants<Cell>().ToList();

                                            for (int k = 0; k < celllst.Count; k++)
                                            {
                                                if (celllst[k] != null || celllst[k].CellReference != null)
                                                {
                                                    var cellcolumnindex = string.Empty;
                                                    var cellrefrence1 = celllst[k].CellReference.ToString();
                                                    int a1 = getIndexofNumber(cellrefrence1);
                                                    string Numberpart1 = cellrefrence1.Substring(a1, cellrefrence1.Length - a1);
                                                    cellcolumnindex = cellrefrence1.Substring(0, a1);

                                                    string cellcolumnindex2 = string.Empty, cellrefrence2 = string.Empty;
                                                    if (k + 1 < celllst.Count)
                                                    {
                                                        if (celllst[k + 1] != null || celllst[k + 1].CellReference != null)
                                                        {
                                                            cellrefrence2 = celllst[k + 1].CellReference.ToString();

                                                            int aaa = getIndexofNumber(cellrefrence2);
                                                            string aNumberpart2 = cellrefrence2.Substring(aaa, cellrefrence2.Length - aaa);
                                                            cellcolumnindex2 = cellrefrence2.Substring(0, aaa);
                                                        }
                                                    }

                                                    if (cellcolumnindex == columnindex)
                                                    {
                                                        DataRow dr = dtImport.NewRow();

                                                        dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                                        dr["PROJECTNAME"] = lProjectName;
                                                        dr["STORYBOARDNAME"] = lStoryboradName;
                                                        dr["RESULTMODE"] = lResultMode;
                                                        dr["TESTCASENAME"] = lTestCase;
                                                        dr["DATASETNAME"] = lDataset;
                                                        dr["ROWNUMBER"] = lRowNumber;
                                                        dr["NAME"] = lName;
                                                        dr["DESCRIPTITON"] = lDesc;
                                                        dr["OBJTAG"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);

                                                        if (k + 1 < celllst.Count && cellcolumnindex2 == columnindex2)
                                                            dr["OBJVALUE"] = celllst[k + 1].CellValue == null ? "" : GetValue(doc, celllst[k + 1]);
                                                        else
                                                            dr["OBJVALUE"] = "";

                                                        dtImport.Rows.Add(dr);
                                                        dtImport.AcceptChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (lResult == false)
                        {
                            dtImport = null;
                        }

                        if (dtImport != null)
                        {
                            if (dtImport.Rows.Count > 0)
                            {
                                OracleTransaction ltransaction;
                                OracleConnection lconnection = new OracleConnection(lstrConn);
                                lconnection.Open();
                                ltransaction = lconnection.BeginTransaction();
                                string lcmdquery = "insert into TBLSTGSTORYBOARDRESULT ( FEEDPROCESSDETAILID,PROJECTNAME,STORYBOARDNAME,RESULTMODE,TESTCASENAME,DATASETNAME,ROWNUMBER,NAME,DESCRIPTITON,OBJTAG,OBJVALUE,ROWSID) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12)";
                                int[] ids = new int[dtImport.Rows.Count];
                                using (var lcmd = lconnection.CreateCommand())
                                {
                                    lcmd.CommandText = lcmdquery;
                                    lcmd.ArrayBindCount = ids.Length;
                                    string[] FEEDPROCESSDETAILID_param = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                                    string[] PROJECTNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("PROJECTNAME")).ToArray();
                                    string[] STORYBOARDNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("STORYBOARDNAME")).ToArray();
                                    string[] RESULTMODE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("RESULTMODE")).ToArray();
                                    string[] TESTCASENAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TESTCASENAME")).ToArray();
                                    string[] DATASETNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                                    string[] ROWNUMBER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("ROWNUMBER")).ToArray();
                                    string[] NAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("NAME")).ToArray();
                                    string[] DESCRIPTITON_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DESCRIPTITON")).ToArray();
                                    string[] OBJTAG_param = dtImport.AsEnumerable().Select(r => r.Field<string>("OBJTAG")).ToArray();
                                    string[] OBJVALUE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("OBJVALUE")).ToArray();

                                    int[] ROWSID = new int[ids.Length];
                                    for (int runs = 0; runs < ids.Length; runs++)
                                    {
                                        ROWSID[runs] = runs + 1;
                                    }



                                    OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                                    FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                                    FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                                    OracleParameter PROJECTNAME_oparam = new OracleParameter();
                                    PROJECTNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    PROJECTNAME_oparam.Value = PROJECTNAME_param;

                                    OracleParameter STORYBOARDNAME_oparam = new OracleParameter();
                                    STORYBOARDNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    STORYBOARDNAME_oparam.Value = STORYBOARDNAME_param;

                                    OracleParameter RESULTMODE_oparam = new OracleParameter();
                                    RESULTMODE_oparam.OracleDbType = OracleDbType.Varchar2;
                                    RESULTMODE_oparam.Value = RESULTMODE_param;

                                    OracleParameter TESTCASENAME_oparam = new OracleParameter();
                                    TESTCASENAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    TESTCASENAME_oparam.Value = TESTCASENAME_param;

                                    OracleParameter DATASETNAME_oparam = new OracleParameter();
                                    DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    DATASETNAME_oparam.Value = DATASETNAME_param;

                                    OracleParameter ROWNUMBER_oparam = new OracleParameter();
                                    ROWNUMBER_oparam.OracleDbType = OracleDbType.Varchar2;
                                    ROWNUMBER_oparam.Value = ROWNUMBER_param;

                                    OracleParameter NAME_oparam = new OracleParameter();
                                    NAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    NAME_oparam.Value = NAME_param;

                                    OracleParameter DESCRIPTITON_oparam = new OracleParameter();
                                    DESCRIPTITON_oparam.OracleDbType = OracleDbType.Varchar2;
                                    DESCRIPTITON_oparam.Value = DESCRIPTITON_param;

                                    OracleParameter OBJTAG_oparam = new OracleParameter();
                                    OBJTAG_oparam.OracleDbType = OracleDbType.Varchar2;
                                    OBJTAG_oparam.Value = OBJTAG_param;

                                    OracleParameter OBJVALUE_oparam = new OracleParameter();
                                    OBJVALUE_oparam.OracleDbType = OracleDbType.Varchar2;
                                    OBJVALUE_oparam.Value = OBJVALUE_param;

                                    OracleParameter ROWSID_oparam = new OracleParameter();
                                    ROWSID_oparam.OracleDbType = OracleDbType.Long;
                                    ROWSID_oparam.Value = ROWSID;

                                    lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                                    lcmd.Parameters.Add(PROJECTNAME_oparam);
                                    lcmd.Parameters.Add(STORYBOARDNAME_oparam);
                                    lcmd.Parameters.Add(RESULTMODE_oparam);
                                    lcmd.Parameters.Add(TESTCASENAME_oparam);
                                    lcmd.Parameters.Add(DATASETNAME_oparam);
                                    lcmd.Parameters.Add(ROWNUMBER_oparam);
                                    lcmd.Parameters.Add(NAME_oparam);
                                    lcmd.Parameters.Add(DESCRIPTITON_oparam);
                                    lcmd.Parameters.Add(OBJTAG_oparam);
                                    lcmd.Parameters.Add(OBJVALUE_oparam);
                                    lcmd.Parameters.Add(ROWSID_oparam);
                                    try
                                    {
                                        lcmd.ExecuteNonQuery();
                                        lResult = true;
                                    }
                                    catch (Exception lex)
                                    {
                                        lResult = false;
                                        ltransaction.Rollback();
                                        throw new Exception(lex.Message);
                                    }

                                    ltransaction.Commit();
                                    lconnection.Close();

                                }
                            }
                        }
                    }
                }
                return lResult;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import ResultSet Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :importExcelResultSet " + ex.Message);
            }
        }

        public static int getIndexofNumber(string cell)
        {
            int indexofNum = -1;
            foreach (char c in cell)
            {
                indexofNum++;
                if (Char.IsDigit(c))
                {
                    return indexofNum;
                }
            }
            return indexofNum;
        }

        static DataTable ValidateResultSet(List<Row> rows, SpreadsheetDocument doc, string TabName, string project, int resultmode, string name)
        {
            int flag = 0;

            string project_name = string.Empty;
            string storyboard_name = string.Empty;
            string Datasetname = string.Empty;
            string TestSuite_name = string.Empty;
            string TestCase_name = string.Empty;
            string DataSet_name = string.Empty;
            string title = string.Empty;
            string Description = string.Empty;
            string dtname = string.Empty;
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

            try
            {
                if (rows.Any())
                {
                    var flag1 = 0;
                    List<Cell> fiveRow = null;
                    List<Cell> sixRow = null;
                    List<Cell> sevenRow = null;
                    List<Cell> eightRow = null;
                    var fristRow = rows[0].Descendants<Cell>().ToList();
                    var secondRow = rows[1].Descendants<Cell>().ToList();
                    var thirdRow = rows[2].Descendants<Cell>().ToList();

                    var fourthRow = rows[3].Descendants<Cell>().ToList();

                    if (fourthRow.Count > 0)
                    {
                        string cell = fourthRow[0].CellReference.ToString();

                        int rowcell, acell = getIndexofNumber(cell);

                        string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                        rowcell = Convert.ToInt32(Numberpartcell);
                        string Stringpart = cell.Substring(0, acell);
                        if (rowcell != 4)
                        {
                            flag1 = 1;
                            fiveRow = rows[3].Descendants<Cell>().ToList();
                            sixRow = rows[4].Descendants<Cell>().ToList();
                            sevenRow = rows[5].Descendants<Cell>().ToList();
                            eightRow = rows[6].Descendants<Cell>().ToList();
                        }
                    }
                    else
                    {
                        fiveRow = rows[4].Descendants<Cell>().ToList();
                        sixRow = rows[5].Descendants<Cell>().ToList();
                        sevenRow = rows[6].Descendants<Cell>().ToList();
                        eightRow = rows[7].Descendants<Cell>().ToList();
                    }
                    if (fristRow[0].CellValue != null)
                    {
                        if (GetValue(doc, fristRow[0]).ToUpper() != "PROJECT")
                        {
                            flag = 1;
                            title = "PROJECT TAG";
                            Description = "PROJECT TAG NOT FOUND";
                            ErrorlogExcel(1, 0, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    if (fristRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 1, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                    }

                    if (fristRow[1].CellValue != null)
                    {
                        project_name = GetValue(doc, fristRow[1]);

                        if (project_name.ToLower() != project.ToLower())
                        {
                            flag = 1;
                            title = " Project Name value Field";
                            Description = "Project Name and Excel Project Name not matched";
                            ErrorlogExcel(1, 1, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    if (fristRow.Count > 2)
                    {
                        for (int j = 2; j < fristRow.Count; j++)
                        {
                            flag = 1;
                            title = "Empty Field";
                            Description = "Field should be Empty";
                            ErrorlogExcel(1, Convert.ToInt32(GetColumnIndex(fristRow[j].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    //secound row
                    if (secondRow[0].CellValue != null)
                    {
                        if (GetValue(doc, secondRow[0]).ToUpper() != "STORYBOARD")
                        {
                            flag = 1;
                            title = "STORYBOARD TAG";
                            Description = "STORYBOARD TAG NOT FOUND";
                            ErrorlogExcel(2, 0, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    if (secondRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(2, 1, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                    }

                    if (secondRow[1].CellValue != null)
                        storyboard_name = GetValue(doc, secondRow[1]);

                    if (secondRow.Count > 2)
                    {
                        for (int j = 2; j < secondRow.Count; j++)
                        {
                            flag = 1;
                            title = "Empty Field";
                            Description = "Field should be Empty";
                            ErrorlogExcel(2, Convert.ToInt32(GetColumnIndex(secondRow[j].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    //third Row
                    if (thirdRow[0].CellValue != null)
                    {
                        if (GetValue(doc, thirdRow[0]).ToUpper() != "BASELINE/COMPARE")
                        {
                            flag = 1;
                            title = "BASELINE/COMPARE TAG";
                            Description = "BASELINE/COMPARE TAG NOT FOUND";
                            ErrorlogExcel(3, 0, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    if (thirdRow[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(3, 1, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                    }

                    if (thirdRow[1].CellValue != null)
                    {
                        string lResultSet = resultmode == 1 ? "BASELINE" : "COMPARE";
                        if (GetValue(doc, thirdRow[1]).ToLower() != lResultSet.ToLower())
                        {
                            flag = 1;
                            title = " Result Mode Name value Field";
                            Description = "Result Mode and Excel Result Mode not matched";
                            ErrorlogExcel(3, 1, "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    if (thirdRow.Count > 2)
                    {
                        for (int j = 2; j < thirdRow.Count; j++)
                        {
                            flag = 1;
                            title = "Empty Field";
                            Description = "Field should be Empty";
                            ErrorlogExcel(3, Convert.ToInt32(GetColumnIndex(thirdRow[j].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                        }
                    }

                    if (flag1 == 0)
                    {

                        if (fourthRow.Count > 0)
                        {
                            dbtable.errorlog("fourthrow count condition ", "Import ResultSet", "", 0);
                            for (int j = 0; j < fourthRow.Count; j++)
                            {
                                dbtable.errorlog("fourthrow loop ", "Import ResultSet", "", 0);
                                flag = 1;
                                title = "Empty Field";
                                Description = "Field should be Empty";
                                dbtable.errorlog("fourthrow log ", "Import ResultSet", "", 0);
                                ErrorlogExcel(4, Convert.ToInt32(GetColumnIndex(fourthRow[j].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                            }
                        }
                    }

                    // five row

                    if (fiveRow.Count > 0)
                    {
                        string lTestCase = string.Empty, lDataset = string.Empty, lRowNumber = string.Empty;
                        for (int m = 0; m < fiveRow.Count; m += 2)
                        {
                            var test = fiveRow[m].CellValue;
                            if (fiveRow[m].CellValue == null)
                            {
                                var c = GetColumnIndex(fiveRow[m].CellReference);
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(5, Convert.ToInt32(c), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                            }

                            if (fiveRow[m].CellValue != null)
                            {
                                var rowval = GetValue(doc, fiveRow[m]);
                                var rowarray = rowval.Split(':');

                                if (rowarray.Count() > 3)
                                {
                                    lTestCase = Convert.ToString(rowarray[1]);
                                    lTestCase = Convert.ToString(lTestCase.Substring(0, lTestCase.Length - 3));
                                    if (string.IsNullOrEmpty(lTestCase))
                                    {
                                        var c = GetColumnIndex(fiveRow[m].CellReference);
                                        flag = 1;
                                        title = " non Empty Field";
                                        Description = "TaseCase Name missing on this field.";
                                        ErrorlogExcel(5, Convert.ToInt32(c), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                    }
                                    else
                                        TestCase_name = lTestCase;

                                    lDataset = Convert.ToString(rowarray[2]);
                                    lDataset = Convert.ToString(lDataset.Substring(0, lDataset.Length - 4));
                                    if (string.IsNullOrEmpty(lDataset))
                                    {
                                        var c = GetColumnIndex(fiveRow[m].CellReference);
                                        flag = 1;
                                        title = " non Empty Field";
                                        Description = "DataSet Name missing on this field.";
                                        ErrorlogExcel(5, Convert.ToInt32(c), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                    }
                                    else
                                        Datasetname = lDataset;

                                    lRowNumber = Convert.ToString(rowarray[3]);

                                    if (string.IsNullOrEmpty(lRowNumber))
                                    {
                                        var c = GetColumnIndex(fiveRow[m].CellReference);
                                        flag = 1;
                                        title = " non Empty Field";
                                        Description = "RowNumber missing on this field.";
                                        ErrorlogExcel(5, Convert.ToInt32(c), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                    }
                                    else
                                    {
                                        var isNumeric = int.TryParse(lRowNumber, out int n);
                                        if (!isNumeric)
                                        {
                                            var c = GetColumnIndex(fiveRow[m].CellReference);
                                            flag = 1;
                                            title = " non Empty Field";
                                            Description = "RowNumber Should be Integer.";
                                            ErrorlogExcel(5, Convert.ToInt32(c), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                        }
                                    }
                                }
                                else
                                {
                                    var c = GetColumnIndex(fiveRow[m].CellReference);
                                    flag = 1;
                                    title = " non Empty Field";
                                    Description = "Something missing on this field.";
                                    ErrorlogExcel(5, Convert.ToInt32(c), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                }
                            }
                        }
                    }

                    if (sixRow.Count > 0)
                    {

                        for (int i = 0; i < sixRow.Count; i += 2)
                        {
                            if (sixRow[i].CellValue == null)
                            {
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(6, Convert.ToInt32(GetColumnIndex(sixRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                            }
                            else
                            {
                                if (GetValue(doc, sixRow[i]).ToUpper() != "NAME")
                                {
                                    flag = 1;
                                    title = "Name TAG";
                                    Description = "Name TAG NOT FOUND";
                                    ErrorlogExcel(6, Convert.ToInt32(GetColumnIndex(sixRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(name))
                        {
                            for (int i = 1; i < sixRow.Count; i += 2)
                            {
                                var cell = sixRow[i - 1].CellReference.ToString();
                                int rowcell, acell = getIndexofNumber(cell);
                                string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                rowcell = Convert.ToInt32(Numberpartcell);
                                string Stringpart = cell.Substring(0, acell);

                                if (fiveRow.Any())
                                {
                                    var fifcell = Stringpart + Convert.ToString(rowcell - 1);
                                    var fifrow = fiveRow.Where(x => x.CellReference == fifcell).FirstOrDefault();
                                    if (fifrow != null)
                                    {
                                        var rowval = GetValue(doc, fifrow);
                                        var rowarray = rowval.Split(':');

                                        if (rowarray.Count() > 3)
                                        {
                                            dtname = Convert.ToString(rowarray[2]);
                                            dtname = Convert.ToString(dtname.Substring(0, dtname.Length - 4));
                                        }
                                    }
                                }

                                if (sixRow[i].CellValue == null)
                                {
                                    flag = 1;
                                    title = " non Empty Field";
                                    Description = "Result set's name is required field. Enter the name either in the sheet or on MARS GUI/Command line.";
                                    ErrorlogExcel(6, Convert.ToInt32(GetColumnIndex(sixRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, dtname, 0, "", "", "", TestCase_name, TabName);
                                }
                                else
                                {
                                    if (GetValue(doc, sixRow[i]) == "")
                                    {
                                        flag = 1;
                                        title = " non Empty Field";
                                        Description = "Result set's name is required field. Enter the name either in the sheet or on MARS GUI/Command line.";
                                        ErrorlogExcel(6, Convert.ToInt32(GetColumnIndex(sixRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, dtname, 0, "", "", "", TestCase_name, TabName);
                                    }
                                }
                            }
                        }
                    }

                    if (sevenRow.Count > 0)
                    {
                        for (int i = 0; i < sevenRow.Count; i += 2)
                        {
                            if (sevenRow[i].CellValue == null)
                            {
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(7, Convert.ToInt32(GetColumnIndex(sevenRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                            }
                            else
                            {
                                if (GetValue(doc, sevenRow[i]).ToUpper() != "DESCRIPTION")
                                {
                                    flag = 1;
                                    title = "Description TAG";
                                    Description = "Description TAG NOT FOUND";
                                    ErrorlogExcel(7, Convert.ToInt32(GetColumnIndex(sevenRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                }
                            }
                        }
                    }

                    if (eightRow.Count > 0)
                    {
                        for (int i = 0; i < eightRow.Count; i += 2)
                        {
                            if (eightRow[i].CellValue == null)
                            {
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(8, Convert.ToInt32(GetColumnIndex(eightRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                            }
                            else
                            {
                                if (GetValue(doc, eightRow[i]).ToUpper() != "TAG")
                                {
                                    flag = 1;
                                    title = "TAG Name TAG";
                                    Description = "TAG NOT FOUND";
                                    ErrorlogExcel(8, Convert.ToInt32(GetColumnIndex(eightRow[i].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                }
                            }

                            if (i + 1 < eightRow.Count)
                            {
                                if (eightRow[i + 1].CellValue == null)
                                {
                                    flag = 1;
                                    title = " non Empty Field";
                                    Description = "Field should not be Empty";
                                    ErrorlogExcel(8, Convert.ToInt32(GetColumnIndex(eightRow[i + 1].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                }
                                else
                                {
                                    if (GetValue(doc, eightRow[i + 1]).ToUpper() != "VALUE")
                                    {
                                        flag = 1;
                                        title = "VALUE TAG";
                                        Description = "VALUE TAG NOT FOUND";
                                        ErrorlogExcel(8, Convert.ToInt32(GetColumnIndex(eightRow[i + 1].CellReference)), "", project_name, storyboard_name, TestSuite_name, "", "", td, title, Description, Datasetname, 0, "", "", "", TestCase_name, TabName);
                                    }
                                }
                            }
                        }
                    }
                }
                return td;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "ResultSet Validation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ResultSetValidation " + ex.Message);
            }
        }
        #endregion

        #region TestSuite Import
        //Read excel file of TestSuite and creates entry in staging table
        public static bool ImportTestCaseExcel(string pFilePath, int pFEEDPROCESSDETAILID, string LogPath, string lstrConn, string schema)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportTestCaseExcel";
            string tab = string.Empty;
            try
            {
                bool lResult = false;
                string lTSName = string.Empty, lTCName = string.Empty, lTCDesc = string.Empty;
                string lApplication = string.Empty;
                string lNewApplicationName = string.Empty;
                string Stringpart = string.Empty, Stringpart2 = string.Empty;
                int rowcell = 0;

                DataTable table = new DataTable();

                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
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
                            tab = mysheet.Name;
                            DataTable dt = new DataTable();

                            dt = ValidateExcel(rows, doc, tabName);
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
                                bool fristrow = true;
                                for (int j = 0; j < rows.Count; j++)
                                {
                                    l++;

                                    //if (l > 7 && k % 2 == 0)
                                    if (l > 7)
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
                                               if (j == 8 && fristrow)
                                                {
                                                    fristrow = false;

                                                    var cell = headerlst[k].CellReference.ToString();
                                                    int rowcell1, acell = getIndexofNumber(cell);
                                                    string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                                    rowcell1 = Convert.ToInt32(Numberpartcell);
                                                    Stringpart = cell.Substring(0, acell);
                                                    rowcell = rowcell1;

                                                    var dataval = ExportExcel.IncrementColumnCellReference(cell);
                                                    int rowcell2, acell1 = getIndexofNumber(dataval);
                                                    string Numberpartcell1 = cell.Substring(acell1, dataval.Length - acell1);
                                                    rowcell = Convert.ToInt32(Numberpartcell1);
                                                    Stringpart2 = dataval.Substring(0, acell1);
                                                }


                                                //for (int p = 1; p < fifthRow.Count; p++)
                                                //{
                                                //    var modeIndex = GetColumnIndex(fifthRow[p].CellReference);
                                                //    if (cIndex == modeIndex)
                                                //        dr["DATASETMODE"] = fifthRow[p].CellValue == null ? "" : GetValue(doc, fifthRow[p]).Trim();
                                                //}
                                                //for (int q = 0; q < seventhRow.Count; q++)
                                                //{
                                                //    var decIndex = GetColumnIndex(seventhRow[q].CellReference);
                                                //    if (cIndex == decIndex)
                                                //        dr["DATASETDESCRIPTION"] = seventhRow[q].CellValue == null ? "" : GetValue(doc, seventhRow[q]).Trim();
                                                //}

                                                if (fifthRow.Any())
                                                {
                                                    var fifcell = Stringpart + Convert.ToString(rowcell - 1);
                                                    var fifrow = fifthRow.Where(x => x.CellReference == fifcell).FirstOrDefault();
                                                    if (fifrow != null)
                                                        dr["DATASETMODE"] = fifrow.CellValue == null ? "" : GetValue(doc, fifrow).Trim();
                                                }

                                                if (seventhRow.Any())
                                                {
                                                    var sevcell = Stringpart + Convert.ToString(rowcell + 1);
                                                    var sevrow = seventhRow.Where(x => x.CellReference == sevcell).FirstOrDefault();

                                                    if (sevrow != null)
                                                        dr["DATASETDESCRIPTION"] = sevrow.CellValue == null ? "" : GetValue(doc, sevrow).Trim();
                                                }
                                            }
                                            if (dr["DATASETMODE"].ToString() == "")
                                                dr["DATASETMODE"] = "";

                                            if (dr["DATASETDESCRIPTION"].ToString() == "")
                                                dr["DATASETDESCRIPTION"] = "";

                                            var skipref = Stringpart + Convert.ToString(j + 1);
                                            var dataref = Stringpart2 + Convert.ToString(j + 1);
                                            var skipval = celllst.Where(aa => aa.CellReference == skipref).FirstOrDefault();
                                            var datavalue = celllst.Where(aa => aa.CellReference == dataref).FirstOrDefault();

                                            if (celllst.Any())
                                            {
                                                //if (celllst.FirstOrDefault().CellReference == null && celllst.FirstOrDefault().DataType == "str")
                                                //{
                                                //    dr["DATASETVALUE"] = celllst[k + 1].CellValue == null ? "" : GetValue(doc, celllst[k + 1]).Trim();
                                                //    dr["SKIP"] = celllst[k].CellValue != null && GetValue(doc, celllst[k]).ToUpper().Trim() == "SKIP" ? 4 : 0;
                                                //}
                                                //else
                                                //{
                                                    if (skipval != null)
                                                        dr["SKIP"] = skipval.CellValue != null && GetValue(doc, skipval).ToUpper().Trim() == "SKIP" ? 4 : 0;
                                                    else
                                                        dr["SKIP"] = 0;

                                                    if (datavalue != null)
                                                        dr["DATASETVALUE"] = datavalue.CellValue == null ? "" : GetValue(doc, datavalue).Trim();
                                                    else
                                                        dr["DATASETVALUE"] = "";
                                                //}
                                            }
                                            //dr["DATASETVALUE"] = celllst[k + 1].CellValue == null ? "" : GetValue(doc, celllst[k + 1]).Trim();
                                            //dr["SKIP"] = celllst[k].CellValue != null && GetValue(doc, celllst[k]).ToUpper().Trim() == "SKIP" ? 4 : 0;
                                                                                    
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
                            OracleConnection lconnection = GetOracleConnection(lstrConn);
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

                                    lcmd.ExecuteNonQuery();
                                    lResult = true;

                                }
                                catch (Exception lex)
                                {

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
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                if (ex.Message.Contains("Index was out of range"))
                {
                    msg = "Excel sheet Format is not valid. Please check excelsheet [" + tab + "] sheet issue. Please check merge cell are proper. No extra cell added in sheet.";
                    dbtable.errorlog(msg, "ImportTestcase Excel", SomeGlobalVariables.functionName, line);
                }
                else
                    dbtable.errorlog(msg, "ImportTestcase Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ImportTestCaseExcel " + ex.Message);
            }
        }

        //Checks TestSuite's excel validations
        static DataTable ValidateExcel(List<Row> rows, SpreadsheetDocument doc, string TabName)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ValidateExcel";
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

            try
            {
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
                            ErrorlogExcel(1, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "APPLICATION TAG";
                        Description = "APPLICATION TAG NOT FOUND";
                        ErrorlogExcel(1, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        //  ErrorlogExcel(1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                            ErrorlogExcel(2, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            //ErrorlogExcel(2, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "Test Suite Name TAG";
                        Description = "Test Suite Name TAG NOT FOUND";
                        ErrorlogExcel(2, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        //ErrorlogExcel(2, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, fristRow[1]) == null || GetValue(doc, fristRow[1]) == "")
                    {

                        flag = 1;
                        title = "Application name is required";
                        Description = "Application name not found";
                        ErrorlogExcel(1, 1, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, secondRow[1]) == null || GetValue(doc, secondRow[1]) == "")
                    {

                        flag = 1;
                        title = "Test Suite name is required";
                        Description = "Test Suite name not found";
                        ErrorlogExcel(2, 1, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, thirdRow[1]) == null || GetValue(doc, thirdRow[1]) == "")
                    {

                        flag = 1;
                        title = "Test Case name is required";
                        Description = "Test Case name not found";
                        ErrorlogExcel(3, 1, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, fourthRow[1]) == null || GetValue(doc, fourthRow[1]) == "")
                    {

                        flag = 1;
                        title = "Test Case description is required";
                        Description = "Test Case description not found";
                        ErrorlogExcel(4, 1, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (thirdRow[0].CellValue != null)
                    {
                        if (GetValue(doc, thirdRow[0]) != "Test Case Name")
                        {
                            flag = 1;
                            title = "Test Case Name TAG";
                            Description = "Test Case Name TAG NOT FOUND";
                            ErrorlogExcel(3, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(3, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                        Testcase_name = GetValue(doc, thirdRow[1]);
                    }
                    else
                    {
                        flag = 1;
                        title = "Test Case Name TAG";
                        Description = "Test Case Name TAG NOT FOUND";
                        ErrorlogExcel(3, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        // ErrorlogExcel(3, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (fourthRow[0].CellValue != null)
                    {
                        if (GetValue(doc, fourthRow[0]) != "Test Case Description")
                        {
                            flag = 1;
                            title = "Test Case Description TAG";
                            Description = "Test Case Description TAG NOT FOUND";
                            ErrorlogExcel(4, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(4, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "Test Case Description TAG";
                        Description = "Test Case Description TAG NOT FOUND";
                        ErrorlogExcel(4, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        // ErrorlogExcel(4, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (fifthRow[0].CellValue != null)
                    {
                        if (GetValue(doc, fifthRow[0]).ToUpper().Trim() != "MODE")
                        {
                            flag = 1;
                            title = "MODE TAG";
                            Description = "MODE TAG NOT FOUND";
                            ErrorlogExcel(5, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(5, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "MODE TAG";
                        Description = "MODE TAG NOT FOUND";
                        ErrorlogExcel(5, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        // ErrorlogExcel(5, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (headerlst[0].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[0]).ToUpper().Trim() != "KEYWORD")
                        {
                            flag = 1;
                            title = "KEYWORD TAG";
                            Description = "KEYWORD TAG NOT FOUND";
                            ErrorlogExcel(6, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(6, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "KEYWORD TAG";
                        Description = "KEYWORD TAG NOT FOUND";
                        ErrorlogExcel(6, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        // ErrorlogExcel(6, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (headerlst[1].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[1]).ToUpper().Trim() != "OBJECT")
                        {
                            flag = 1;
                            title = "OBJECT TAG";
                            Description = "OBJECT TAG NOT FOUND";
                            ErrorlogExcel(6, 1, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            //ErrorlogExcel(6, 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "OBJECT TAG";
                        Description = "OBJECT TAG NOT FOUND";
                        ErrorlogExcel(6, 1, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        //  ErrorlogExcel(6, 1, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (headerlst[2].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[2]).ToUpper().Trim() != "PARAMETERS")
                        {
                            flag = 1;
                            title = "PARAMETERS TAG";
                            Description = "PARAMETERS TAG NOT FOUND";
                            ErrorlogExcel(6, 2, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            //   ErrorlogExcel(6, 2, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "PARAMETERS TAG";
                        Description = "PARAMETERS TAG NOT FOUND";
                        ErrorlogExcel(6, 2, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        //  ErrorlogExcel(6, 2, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (headerlst[3].CellValue != null)
                    {
                        if (GetValue(doc, headerlst[3]).ToUpper().Trim() != "COMMENT")
                        {
                            flag = 1;
                            title = "COMMENT TAG";
                            Description = "COMMENT TAG NOT FOUND";
                            ErrorlogExcel(6, 3, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(6, 3, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        flag = 1;
                        title = "COMMENT TAG";
                        Description = "COMMENT TAG NOT FOUND";
                        ErrorlogExcel(6, 3, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        // ErrorlogExcel(6, 3, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                        ErrorlogExcel(7, cIndex, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        //ErrorlogExcel(7, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                        ErrorlogExcel(7, cIndex, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        //ErrorlogExcel(7, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                        ErrorlogExcel(5, cIndex, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        //ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                            ErrorlogExcel(5, cIndex, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                            //ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        }
                                        if (GetValue(doc, fifthRow[j]).Trim() == "d" && GetValue(doc, fifthRow[j]) != "")
                                        {
                                            flag = 1;
                                            title = "Mode";
                                            Description = "Invalid Mode, Please enter D instead of d.";
                                            ErrorlogExcel(5, cIndex, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                            // ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                        ErrorlogExcel(5, cIndex, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        //ErrorlogExcel(5, cIndex, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                ErrorlogExcel(1, j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                // ErrorlogExcel(1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                        }
                    }
                    if (headerlst.Count > 4)
                    {
                        if (headerlst[4].CellValue == null)
                        {
                            title = "Data-set Name";
                            Description = "Dataset name should be there";
                            ErrorlogExcel(6, 4, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(6, 4, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        }
                    }
                    else
                    {
                        title = "Data-set Name";
                        Description = "Dataset name should be there";
                        ErrorlogExcel(6, 4, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        //ErrorlogExcel(6, 4, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                            ErrorlogExcel(6, 3 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(6, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                            ErrorlogExcel(7, 3 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            //  ErrorlogExcel(7, 3 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                            ErrorlogExcel(6, 4 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(6, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                            ErrorlogExcel(7, 4 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            // ErrorlogExcel(7, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                        ErrorlogExcel(8, 4 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        // ErrorlogExcel(8, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                    }
                                }
                                else
                                {
                                    flag = 1;
                                    title = "skip flag tag";
                                    Description = "skip flag tag not found";
                                    ErrorlogExcel(8, 4 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                    //ErrorlogExcel(8, 4 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                            ErrorlogExcel(8, 5 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                            //ErrorlogExcel(8, 5 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        }
                                    }
                                    else
                                    {
                                        flag = 1;
                                        title = "Data tag";
                                        Description = "Data tag not found";
                                        ErrorlogExcel(8, 5 + j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        // ErrorlogExcel(8, 5 + j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                                    ErrorlogExcel(x + 1, j, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                    // ErrorlogExcel(x + 1, j, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                            ErrorlogExcel(r + 1, 0, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            //  ErrorlogExcel(r + 1, 0, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
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
                        ErrorlogExcel(5, cVal, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                        // ErrorlogExcel(5, cVal, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }

                    for (int k = 4; k < dataset_coloum - 1; k += 2)
                    {
                        var cIndex = GetColumnIndex(headerlst[k].CellReference);

                        var cell = headerlst[k].CellReference.ToString();
                        int rowcell, acell = getIndexofNumber(cell);
                        string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                        rowcell = Convert.ToInt32(Numberpartcell);
                        string Stringpart = cell.Substring(0, acell);

                        for (int x = 8; x <= (rcount - row_cc - 1); x++)
                         {
                            var celllist = rows[x].Descendants<Cell>().ToList();
                            var cellref = Stringpart + Convert.ToString(x + 1);
                            var slipval = celllist.Where(aa => aa.CellReference == cellref).FirstOrDefault();
                            if (slipval != null)
                            {
                                if(slipval.CellValue != null)
                                {
                                    if (GetValue(doc, slipval).ToUpper().Trim() != "SKIP")
                                    {
                                        if (GetValue(doc, slipval) != "")
                                        {
                                            flag = 1;
                                            title = "skip column value";
                                            Description = "Invalid input";

                                            Datasetname = GetValue(doc, headerlst[k]);
                                            TestStepNumber = x - 7;
                                            objectname = GetValue(doc, celllist[1]);
                                            comment = GetValue(doc, celllist[3]);
                                            ErrorlogExcel(x + 1, k, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                            // ErrorlogExcel(x + 1, k, Application_name, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                        }
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
                                ErrorlogExcel(x + 1, k, Application_name, "", "", "", "", "", td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                // ErrorlogExcel(x + 1, k, Application_name, td, title, Description, Datasetname, 0, objectname, comment, location, Testcase_name, TabName);
                            }
                        }
                    }
                    Application_name = null;
                }
                return td;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                if (ex.Message.Contains("Index was out of range"))
                {
                    msg = "Excel sheet Format is not valid. Please check excelsheet [" + TabName + "] sheet issue. Please check merge cell are proper. No extra cell added in sheet.";
                    dbtable.errorlog(msg, "TestCase validation", SomeGlobalVariables.functionName, line);
                }
                else
                    dbtable.errorlog(msg, "TestCase validation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :TestCaseValidation " + ex.Message);

            }
        }
        #endregion

        public static bool ImportExcelCompareConfig(string pFilePath, int pFEEDPROCESSDETAILID, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelCompareConfig";
            bool lResult = false;
            try
            {
                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();
                    Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(0);
                    Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                    List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                    DataTable dt = new DataTable();
                    dt = CompareConfigValidation(rows, doc);
                    if (dt.Rows.Count != 0)
                    {
                        lResult = false;
                        dbtable.dt_Log.Merge(dt);
                        return lResult;
                    }

                    DataTable ldt = new DataTable();
                    ldt.Columns.Add("DATASOURCENAME");
                    ldt.Columns.Add("DATASOURCETYPE");
                    ldt.Columns.Add("DETAILS");
                    ldt.Columns.Add("FEEDPROCESSDETAILID");

                    ldt.AcceptChanges();
                    for (int i = 1; i < rows.Count(); i++)
                    {
                        var celllst = rows[i].Descendants<Cell>().ToList();
                        if (celllst.Any())
                        {
                            DataRow dr = ldt.NewRow();
                            dr["DATASOURCENAME"] = celllst[0].CellValue == null ? "" : GetValue(doc, celllst[0]);
                            dr["DATASOURCETYPE"] = celllst[1].CellValue == null ? "" : GetValue(doc, celllst[1]);
                            dr["DETAILS"] = celllst[2].CellValue == null ? "" : GetValue(doc, celllst[2]);
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

                            lcmd.CommandText = schema + "." + "SP_IMPORT_FILE_COMPAREPARAM";
                            lcmd.CommandType = CommandType.StoredProcedure;
                            lcmd.Parameters.Add(new OracleParameter("DATASOURCENAME", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["DATASOURCENAME"]);
                            lcmd.Parameters.Add(new OracleParameter("DATASOURCETYPE", OracleDbType.Int32)).Value = ldt.Rows[i]["DATASOURCETYPE"];
                            lcmd.Parameters.Add(new OracleParameter("DETAILS", OracleDbType.Varchar2)).Value = Convert.ToString(ldt.Rows[i]["DETAILS"]);
                            lcmd.Parameters.Add(new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32)).Value = ldt.Rows[i]["FEEDPROCESSDETAILID"];

                            try
                            {
                                lcmd.ExecuteNonQuery();
                                lResult = true;
                            }
                            catch (Exception lex)
                            {
                                lResult = false;
                                ltransaction.Rollback();
                                throw new Exception(lex.Message);
                            }
                            ltransaction.Commit();
                        }
                    }
                    lconnection.Close();

                }
                return lResult;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import CompareConfig", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ImportCompareConfig " + ex.Message);

            }
        }
        static DataTable CompareConfigValidation(List<Row> rows, SpreadsheetDocument doc)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->CompareConfigValidation";
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
            string xmlpath = null;

            try
            {
                if (rows.Any())
                {
                    var hedar = rows[0].Descendants<Cell>();
                    var hedarlst = hedar.ToList();
                    var test = GetValue(doc, hedarlst[0]).ToUpper();
                    if (test != "DATA SOURCE NAME")

                    {
                        flag = 1;
                        title = "Data Source Name";
                        Description = "Data Source Name tag NOT FOUND";
                        ErrorlogExcel(0, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[1]).ToUpper() != "DATA SOURCE TYPE")

                    {
                        flag = 1;
                        title = "DATA SOURCE TYPE";
                        Description = "Data Source Type TAG NOT FOUND";
                        //Application_name = xlWorksheet.Cells[1, 1].Value;
                        ErrorlogExcel(0, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    if (GetValue(doc, hedarlst[2]).ToUpper() != "DETAILS")

                    {
                        flag = 1;
                        title = "Details ";
                        Description = "Details  TAG NOT FOUND";
                        //Application_name = xlWorksheet.Cells[1, 1].Value;
                        ErrorlogExcel(0, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                    }
                    for (int i = 1; i < rows.Count; i++)
                    {
                        var celllst = rows[i].Descendants<Cell>().ToList();
                        if (celllst.Any())
                        {
                            if (celllst[0].CellValue == null)
                            {
                                title = "Non Empty field";
                                Description = "Field must NOT be empty";
                                ErrorlogExcel(i + 1, 0, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                            if (!(GetValue(doc, celllst[1]) == "1" || GetValue(doc, celllst[1]) == "2" || GetValue(doc, celllst[1]) == "3" || GetValue(doc, celllst[1]) == "4"))
                            {
                                title = "DATA SOURCE TYPE VALUE";
                                Description = "Value is not proper";
                                ErrorlogExcel(i + 1, 1, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                            if (celllst[2].CellValue == null)
                            {
                                title = " NON Empty field";
                                Description = "Value is not proper";
                                ErrorlogExcel(i + 1, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                            }
                            xmlpath = GetValue(doc, celllst[2]);
                            if (xmlpath != "" && xmlpath != null)
                            {
                                string last = xmlpath.Substring(xmlpath.Length - 1, 1);
                                string first = xmlpath.Substring(0, 1);
                                if (!(last == ">" && first == "<"))
                                {

                                    flag = 1;
                                    title = "xml format";
                                    Description = "xml format is not proper";
                                    //Application_name = xlWorksheet.Cells[1, 1].Value;
                                    ErrorlogExcel(i + 1, 2, Application_name, project_name, storyboard_name, TestSuite_name, Dependancy, run_order, td, title, Description, Datasetname, TestStepNumber, objectname, comment, location, Testcase_name, TabName);
                                }
                            }


                        }
                    }
                }
                return td;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "VariableValidation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :VariableValidation " + ex.Message);

            }
        }
        //Gets Cell value
        private static string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            // SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->GetValue";
            try
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
                else if (cell.DataType == null)
                {

                    if (cell.StyleIndex != null)
                    {
                        var cellFormat = doc.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements[
                        int.Parse(cell.StyleIndex.InnerText)] as CellFormat;

                        // only focus on Date
                        if (cellFormat != null)
                        {
                            var dateFormat = GetDateTimeFormat(cellFormat.NumberFormatId);

                            if (dateFormat == "dd/MM/yyyy" || dateFormat == "d-MMM-yy" || dateFormat == "d-MMM")
                            {

                                return DateTime.FromOADate(double.Parse(value)).ToShortDateString();
                            }
                            else if (dateFormat == "h:mm" || dateFormat == "h:mm:ss" || dateFormat == "mm:ss")
                            {
                                return DateTime.FromOADate(double.Parse(value)).ToShortTimeString();
                            }
                        }
                    }
                }
                else if (cell.DataType.Value == CellValues.Date)
                {
                    if (cell.StyleIndex != null)
                    {
                        var cellFormat = doc.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements[
                        int.Parse(cell.StyleIndex.InnerText)] as CellFormat;
                        // only focus on Date
                        if (cellFormat != null)
                        {
                            var dateFormat = GetDateTimeFormat(cellFormat.NumberFormatId);
                            if (dateFormat == "h:mm" || dateFormat == "h:mm:ss" || dateFormat == "mm:ss")
                            {
                                return DateTime.Parse(value).ToString("H:mm:ss");
                            }
                        }
                    }
                }
                else if (cell.DataType.Value == CellValues.Boolean)
                {
                    if (value == "1")
                    {
                        return "TRUE";
                    }
                    else
                    {
                        return "FALSE";
                    }
                }
                return value;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                // dbtable.errorlog(msg, "Get Cell Value", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :GetValue " + ex.Message);
            }
        }
        private static string GetDateTimeFormat(uint numberFormatId)
        {
            return DateFormatDictionary.ContainsKey(numberFormatId) ? DateFormatDictionary[numberFormatId] : string.Empty;
        }

        private static readonly Dictionary<uint, string> DateFormatDictionary = new Dictionary<uint, string>()
        {
            [14] = "dd/MM/yyyy",
            [15] = "d-MMM-yy",
            [16] = "d-MMM",
            [17] = "MMM-yy",
            [18] = "h:mm AM/PM",
            [19] = "h:mm:ss AM/PM",
            [20] = "h:mm",
            [21] = "h:mm:ss",
            [22] = "M/d/yy h:mm",
            [30] = "M/d/yy",
            [34] = "yyyy-MM-dd",
            [45] = "mm:ss",
            [46] = "[h]:mm:ss",
            [47] = "mmss.0",
            [51] = "MM-dd",
            [52] = "yyyy-MM-dd",
            [53] = "yyyy-MM-dd",
            [55] = "yyyy-MM-dd",
            [56] = "yyyy-MM-dd",
            [58] = "MM-dd",
            [165] = "M/d/yy",
            [166] = "dd MMMM yyyy",
            [167] = "dd/MM/yyyy",
            [168] = "dd/MM/yy",
            [169] = "d.M.yy",
            [170] = "yyyy-MM-dd",
            [171] = "dd MMMM yyyy",
            [172] = "d MMMM yyyy",
            [173] = "M/d",
            [174] = "M/d/yy",
            [175] = "MM/dd/yy",
            [176] = "d-MMM",
            [177] = "d-MMM-yy",
            [178] = "dd-MMM-yy",
            [179] = "MMM-yy",
            [180] = "MMMM-yy",
            [181] = "MMMM d, yyyy",
            [182] = "M/d/yy hh:mm t",
            [183] = "M/d/y HH:mm",
            [184] = "MMM",
            [185] = "MMM-dd",
            [186] = "M/d/yyyy",
            [187] = "d-MMM-yyyy"
        };

        //returns Column's index
        private static int? GetColumnIndex(string cellReference)
        {
            //SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->GetColumnIndex";
            try
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
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                // dbtable.errorlog(msg, "Get Column Index", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :GetColumnIndex " + ex.Message);
            }

        }

        public static bool IsFileLocked(string filePath)
        {
            bool lockStatus = false;
            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // File/Stream manipulating code here

                    lockStatus = !fileStream.CanWrite;

                }
            }
            catch
            {
                //check here why it failed and ask user to retry if the file is in use.
                lockStatus = true;
            }
            return lockStatus;
        }
        //Fills Datatable if validation occurs
        static void ErrorlogExcel(int r, int c, string app_name, string project_name, string storyboard_name, string TestSuite_name, string Dependancy, string run_order, DataTable dt, string title, string Description, string Datasetname, int TestStepNumber, string objectname, string comment, string location, string Testcase_name, string TabName)
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

        static void ErrorLogExcel(string r, int c, string app_name, string project_name, string storyboard_name, string TestSuite_name, string Dependancy, string run_order, DataTable dt, string title, string Description, string Datasetname, int TestStepNumber, string objectname, string comment, string location, string Testcase_name, string TabName)
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

            dr["SpreadSheet cell Address"] = r;
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
            Console.WriteLine("Tab Name:" + TabName + " :-" + "check input in cell:- " + r + " ==> " + "Error Description:- " + Description);




        }

        public static bool ImportExcelDatasetTag(string pFilePath, int pFEEDPROCESSDETAILID, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelDatasetTag";
            try
            {
                bool lResult = true;

                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();
                    lResult = true;
                    if (worksheetcount > 0)
                    {
                        DataTable dtImport = new DataTable();
                        dtImport.Columns.Add("FEEDPROCESSDETAILID");
                        dtImport.Columns.Add("DATASETNAME");
                        dtImport.Columns.Add("DESCRIPTION");
                        dtImport.Columns.Add("TAGGROUP");
                        dtImport.Columns.Add("TAGSET");
                        dtImport.Columns.Add("TAGFOLDER");
                        dtImport.Columns.Add("EXPECTEDRESULTS");
                        dtImport.Columns.Add("STEPDESC");
                        dtImport.Columns.Add("DIARY");
                        dtImport.Columns.Add("SEQUENCE");
                        dtImport.AcceptChanges();

                        DataTable dtt = new DataTable();
                        dtt.Columns.Add("FEEDPROCESSDETAILID");
                        dtt.Columns.Add("NAME");
                        dtt.Columns.Add("DESCRIPTION");
                        dtt.Columns.Add("ACTIVE");
                        dtt.Columns.Add("TYPE");
                        dtt.AcceptChanges();
                        for (int i = 0; i < worksheetcount; i++)
                        {
                            Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(i);
                            Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                            List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                            DataTable dt = new DataTable();

                            if (i == 0)
                            {
                                dt = DatasetTagValidation(rows, doc);
                                if (dt.Rows.Count != 0)
                                {
                                    lResult = false;
                                    dbtable.dt_Log.Merge(dt);
                                }

                                if (lResult)
                                {
                                    for (int m = 1; m < rows.Count; m++)
                                    {
                                        var celllst = rows[m].Descendants<Cell>().ToList();

                                        DataRow dr = dtImport.NewRow();
                                        dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                        for (int k = 0; k < celllst.Count; k++)
                                        {
                                            if (celllst[k].CellReference != null)
                                            {
                                                var cell = celllst[k].CellReference.ToString();
                                                int rowcell, acell = getIndexofNumber(cell);
                                                string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                                rowcell = Convert.ToInt32(Numberpartcell);
                                                string Stringpart = cell.Substring(0, acell);

                                                if (Stringpart == "A")
                                                    dr["DATASETNAME"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "B")
                                                    dr["DESCRIPTION"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "C")
                                                    dr["TAGGROUP"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "D")
                                                    dr["TAGSET"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "E")
                                                    dr["TAGFOLDER"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "F")
                                                    dr["EXPECTEDRESULTS"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "G")
                                                    dr["STEPDESC"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "H")
                                                    dr["DIARY"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "I")
                                                    dr["SEQUENCE"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);
                                            }
                                        }

                                        if (dr["DATASETNAME"].ToString() == "")
                                            dr["DATASETNAME"] = "";

                                        if (dr["DESCRIPTION"].ToString() == "")
                                            dr["DESCRIPTION"] = "";

                                        if (dr["TAGGROUP"].ToString() == "")
                                            dr["TAGGROUP"] = "";

                                        if (dr["TAGSET"].ToString() == "")
                                            dr["TAGSET"] = "";

                                        if (dr["TAGFOLDER"].ToString() == "")
                                            dr["TAGFOLDER"] = "";

                                        if (dr["EXPECTEDRESULTS"].ToString() == "")
                                            dr["EXPECTEDRESULTS"] = "";

                                        if (dr["STEPDESC"].ToString() == "")
                                            dr["STEPDESC"] = "";

                                        if (dr["DIARY"].ToString() == "")
                                            dr["DIARY"] = "";

                                        if (dr["SEQUENCE"].ToString() == "")
                                            dr["SEQUENCE"] = "";

                                        dtImport.Rows.Add(dr);
                                        dtImport.AcceptChanges();
                                    }
                                }
                                if (lResult == false)
                                {
                                    dtImport = null;

                                }
                                if (dtImport != null)
                                {
                                    if (dtImport.Rows.Count > 0)
                                    {
                                        OracleTransaction ltransaction;
                                        OracleConnection lconnection = new OracleConnection(constring);
                                        lconnection.Open();
                                        ltransaction = lconnection.BeginTransaction();
                                        string lcmdquery = "insert into TBLSTGDATASETTAG ( FEEDPROCESSDETAILID,DATASETNAME,TAGGROUP,TAGSET,TAGFOLDER,EXPECTEDRESULTS,STEPDESC,DIARY,SEQUENCE,DESCRIPTION) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10)";
                                        int[] ids = new int[dtImport.Rows.Count];
                                        using (var lcmd = lconnection.CreateCommand())
                                        {
                                            lcmd.CommandText = lcmdquery;
                                            lcmd.ArrayBindCount = ids.Length;
                                            string[] FEEDPROCESSDETAILID_param = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                                            string[] DATASETNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                                            string[] DESCRIPTION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DESCRIPTION")).ToArray();
                                            string[] TAGGROUP_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TAGGROUP")).ToArray();
                                            string[] TAGSET_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TAGSET")).ToArray();
                                            string[] TAGFOLDER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TAGFOLDER")).ToArray();
                                            string[] EXPECTEDRESULTS_param = dtImport.AsEnumerable().Select(r => r.Field<string>("EXPECTEDRESULTS")).ToArray();
                                            string[] STEPDESC_param = dtImport.AsEnumerable().Select(r => r.Field<string>("STEPDESC")).ToArray();
                                            string[] DIARY_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DIARY")).ToArray();
                                            string[] SEQUENCE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("SEQUENCE")).ToArray();

                                            OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                                            FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                                            FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                                            OracleParameter DATASETNAME_oparam = new OracleParameter();
                                            DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                            DATASETNAME_oparam.Value = DATASETNAME_param;

                                            OracleParameter TAGGROUP_oparam = new OracleParameter();
                                            TAGGROUP_oparam.OracleDbType = OracleDbType.Varchar2;
                                            TAGGROUP_oparam.Value = TAGGROUP_param;

                                            OracleParameter TAGSET_oparam = new OracleParameter();
                                            TAGSET_oparam.OracleDbType = OracleDbType.Varchar2;
                                            TAGSET_oparam.Value = TAGSET_param;

                                            OracleParameter TAGFOLDER_oparam = new OracleParameter();
                                            TAGFOLDER_oparam.OracleDbType = OracleDbType.Varchar2;
                                            TAGFOLDER_oparam.Value = TAGFOLDER_param;

                                            OracleParameter EXPECTEDRESULTS_oparam = new OracleParameter();
                                            EXPECTEDRESULTS_oparam.OracleDbType = OracleDbType.Varchar2;
                                            EXPECTEDRESULTS_oparam.Value = EXPECTEDRESULTS_param;

                                            OracleParameter STEPDESC_oparam = new OracleParameter();
                                            STEPDESC_oparam.OracleDbType = OracleDbType.Varchar2;
                                            STEPDESC_oparam.Value = STEPDESC_param;

                                            OracleParameter DIARY_oparam = new OracleParameter();
                                            DIARY_oparam.OracleDbType = OracleDbType.Varchar2;
                                            DIARY_oparam.Value = DIARY_param;

                                            OracleParameter SEQUENCE_oparam = new OracleParameter();
                                            SEQUENCE_oparam.OracleDbType = OracleDbType.Varchar2;
                                            SEQUENCE_oparam.Value = SEQUENCE_param;

                                            OracleParameter DESCRIPTION_oparam = new OracleParameter();
                                            DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                                            DESCRIPTION_oparam.Value = DESCRIPTION_param;


                                            lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                                            lcmd.Parameters.Add(DATASETNAME_oparam);
                                            lcmd.Parameters.Add(TAGGROUP_oparam);
                                            lcmd.Parameters.Add(TAGSET_oparam);
                                            lcmd.Parameters.Add(TAGFOLDER_oparam);
                                            lcmd.Parameters.Add(EXPECTEDRESULTS_oparam);
                                            lcmd.Parameters.Add(STEPDESC_oparam);
                                            lcmd.Parameters.Add(DIARY_oparam);
                                            lcmd.Parameters.Add(SEQUENCE_oparam);
                                            lcmd.Parameters.Add(DESCRIPTION_oparam);
                                            try
                                            {
                                                lcmd.ExecuteNonQuery();
                                                lResult = true;
                                            }
                                            catch (Exception lex)
                                            {
                                                lResult = false;
                                                ltransaction.Rollback();
                                                throw new Exception(lex.Message);
                                            }

                                            ltransaction.Commit();
                                            lconnection.Close();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var tabName = mysheet.Name;
                                dt = DatasetTagCommonValidation(rows, doc, tabName);
                                if (dt.Rows.Count != 0)
                                {
                                    lResult = false;
                                    dbtable.dt_Log.Merge(dt);
                                }

                                if (lResult)
                                {
                                    for (int m = 1; m < rows.Count; m++)
                                    {
                                        var celllst = rows[m].Descendants<Cell>().ToList();

                                        DataRow dr = dtt.NewRow();
                                        dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                        for (int k = 0; k < celllst.Count; k++)
                                        {
                                            if (celllst[k].CellReference != null)
                                            {
                                                var cell = celllst[k].CellReference.ToString();
                                                int rowcell, acell = getIndexofNumber(cell);
                                                string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                                rowcell = Convert.ToInt32(Numberpartcell);
                                                string Stringpart = cell.Substring(0, acell);

                                                if (Stringpart == "A")
                                                    dr["NAME"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);

                                                if (Stringpart == "B")
                                                    dr["DESCRIPTION"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);

                                                if (Stringpart == "C")
                                                {
                                                    if (celllst[k].CellValue != null)
                                                        dr["ACTIVE"] = GetValue(doc, celllst[k]) == "Active" ? 1 : 0;
                                                }
                                            }
                                        }

                                        if (dr["NAME"].ToString() == "")
                                            dr["NAME"] = "";

                                        if (dr["DESCRIPTION"].ToString() == "")
                                            dr["DESCRIPTION"] = "";

                                        if (dr["ACTIVE"].ToString() == "")
                                            dr["ACTIVE"] = 0;

                                        dr["TYPE"] = tabName;
                                        dtt.Rows.Add(dr);
                                        dtt.AcceptChanges();
                                    }
                                }
                            }

                        }

                        if (lResult == false)
                        {
                            dtt = null;
                        }

                        //if (dtt != null)
                        if (dtt != null && dtImport != null)
                        {
                            if (dtt.Rows.Count > 0)
                            {
                                OracleTransaction ltransaction;
                                OracleConnection lconnection = new OracleConnection(constring);
                                lconnection.Open();
                                ltransaction = lconnection.BeginTransaction();
                                string lcmdquery = "insert into TBLSTGCOMMONDATASETTAG ( FEEDPROCESSDETAILID,NAME,DESCRIPTION,ACTIVE,TYPE) values(:1,:2,:3,:4,:5)";
                                int[] ids = new int[dtt.Rows.Count];
                                using (var lcmd = lconnection.CreateCommand())
                                {
                                    lcmd.CommandText = lcmdquery;
                                    lcmd.ArrayBindCount = ids.Length;
                                    string[] FEEDPROCESSDETAILID_param = dtt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                                    string[] NAME_param = dtt.AsEnumerable().Select(r => r.Field<string>("NAME")).ToArray();
                                    string[] DESCRIPTION_param = dtt.AsEnumerable().Select(r => r.Field<string>("DESCRIPTION")).ToArray();
                                    string[] ACTIVE_param = dtt.AsEnumerable().Select(r => r.Field<string>("ACTIVE")).ToArray();
                                    string[] TYPE_param = dtt.AsEnumerable().Select(r => r.Field<string>("TYPE")).ToArray();

                                    OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                                    FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                                    FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                                    OracleParameter NAME_oparam = new OracleParameter();
                                    NAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    NAME_oparam.Value = NAME_param;

                                    OracleParameter DESCRIPTION_oparam = new OracleParameter();
                                    DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                                    DESCRIPTION_oparam.Value = DESCRIPTION_param;

                                    OracleParameter ACTIVE_oparam = new OracleParameter();
                                    ACTIVE_oparam.OracleDbType = OracleDbType.Long;
                                    ACTIVE_oparam.Value = ACTIVE_param;

                                    OracleParameter TYPE_oparam = new OracleParameter();
                                    TYPE_oparam.OracleDbType = OracleDbType.Varchar2;
                                    TYPE_oparam.Value = TYPE_param;

                                    lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                                    lcmd.Parameters.Add(NAME_oparam);
                                    lcmd.Parameters.Add(DESCRIPTION_oparam);
                                    lcmd.Parameters.Add(ACTIVE_oparam);
                                    lcmd.Parameters.Add(TYPE_oparam);
                                    try
                                    {
                                        lcmd.ExecuteNonQuery();
                                        lResult = true;
                                    }
                                    catch (Exception lex)
                                    {
                                        lResult = false;
                                        ltransaction.Rollback();
                                        throw new Exception(lex.Message);
                                    }

                                    ltransaction.Commit();
                                    lconnection.Close();
                                }
                            }
                        }
                    }
                }
                return lResult;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import DataSetTag Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :importExcelDataSetTag " + ex.Message);
            }
        }

        public static bool ImportExcelDatasetTagNew(string pFilePath, long pFEEDPROCESSDETAILID, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ImportExcelDatasetTag";
            try
            {
                bool lResult = true;

                bool open = IsFileLocked(pFilePath);
                if (open)
                {
                    dbtable.errorlog("File is open in Background", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n");
                    Console.WriteLine("File is open in Background.... Please close it before importing..");
                    return false;
                }
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pFilePath, false))
                {
                    WorkbookPart wbPart = doc.WorkbookPart;
                    int worksheetcount = doc.WorkbookPart.Workbook.Sheets.Count();
                    lResult = true;
                    if (worksheetcount > 0)
                    {
                        DataTable dtImport = new DataTable();
                        dtImport.Columns.Add("FEEDPROCESSDETAILID");
                        dtImport.Columns.Add("DATASETNAME");
                        dtImport.Columns.Add("DESCRIPTION");
                        dtImport.Columns.Add("TAGGROUP");
                        dtImport.Columns.Add("TAGSET");
                        dtImport.Columns.Add("TAGFOLDER");
                        dtImport.Columns.Add("EXPECTEDRESULTS");
                        dtImport.Columns.Add("STEPDESC");
                        dtImport.Columns.Add("DIARY");
                        dtImport.Columns.Add("SEQUENCE");
                        dtImport.AcceptChanges();

                        DataTable dtt = new DataTable();
                        dtt.Columns.Add("FEEDPROCESSDETAILID");
                        dtt.Columns.Add("NAME");
                        dtt.Columns.Add("DESCRIPTION");
                        dtt.Columns.Add("ACTIVE");
                        dtt.Columns.Add("TYPE");
                        dtt.AcceptChanges();
                        for (int i = 0; i < worksheetcount; i++)
                        {
                            Sheet mysheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(i);
                            Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                            List<Row> rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>().ToList();
                            DataTable dt = new DataTable();

                            if (i == 0)
                            {
                                dt = DatasetTagValidation(rows, doc);
                                if (dt.Rows.Count != 0)
                                {
                                    lResult = false;
                                    dbtable.dt_Log.Merge(dt);
                                }

                                if (lResult)
                                {
                                    for (int m = 1; m < rows.Count; m++)
                                    {
                                        var celllst = rows[m].Descendants<Cell>().ToList();

                                        DataRow dr = dtImport.NewRow();
                                        dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                        for (int k = 0; k < celllst.Count; k++)
                                        {
                                            if (celllst[k].CellReference != null)
                                            {
                                                var cell = celllst[k].CellReference.ToString();
                                                int rowcell, acell = getIndexofNumber(cell);
                                                string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                                rowcell = Convert.ToInt32(Numberpartcell);
                                                string Stringpart = cell.Substring(0, acell);

                                                if (Stringpart == "A")
                                                    dr["DATASETNAME"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "B")
                                                    dr["DESCRIPTION"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "C")
                                                    dr["TAGGROUP"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "D")
                                                    dr["TAGSET"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "E")
                                                    dr["TAGFOLDER"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "F")
                                                    dr["EXPECTEDRESULTS"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "G")
                                                    dr["STEPDESC"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "H")
                                                    dr["DIARY"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]).Trim();

                                                if (Stringpart == "I")
                                                    dr["SEQUENCE"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);
                                            }
                                        }

                                        if (dr["DATASETNAME"].ToString() == "")
                                            dr["DATASETNAME"] = "";

                                        if (dr["DESCRIPTION"].ToString() == "")
                                            dr["DESCRIPTION"] = "";

                                        if (dr["TAGGROUP"].ToString() == "")
                                            dr["TAGGROUP"] = "";

                                        if (dr["TAGSET"].ToString() == "")
                                            dr["TAGSET"] = "";

                                        if (dr["TAGFOLDER"].ToString() == "")
                                            dr["TAGFOLDER"] = "";

                                        if (dr["EXPECTEDRESULTS"].ToString() == "")
                                            dr["EXPECTEDRESULTS"] = "";

                                        if (dr["STEPDESC"].ToString() == "")
                                            dr["STEPDESC"] = "";

                                        if (dr["DIARY"].ToString() == "")
                                            dr["DIARY"] = "";

                                        if (dr["SEQUENCE"].ToString() == "")
                                            dr["SEQUENCE"] = "";

                                        dtImport.Rows.Add(dr);
                                        dtImport.AcceptChanges();
                                    }
                                }
                                if (lResult == false)
                                {
                                    dtImport = null;

                                }
                                if (dtImport != null)
                                {
                                    if (dtImport.Rows.Count > 0)
                                    {
                                        OracleTransaction ltransaction;
                                        OracleConnection lconnection = new OracleConnection(constring);
                                        lconnection.Open();
                                        ltransaction = lconnection.BeginTransaction();
                                        string lcmdquery = "insert into TBLSTGDATASETTAG ( FEEDPROCESSDETAILID,DATASETNAME,TAGGROUP,TAGSET,TAGFOLDER,EXPECTEDRESULTS,STEPDESC,DIARY,SEQUENCE,DESCRIPTION) values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10)";
                                        int[] ids = new int[dtImport.Rows.Count];
                                        using (var lcmd = lconnection.CreateCommand())
                                        {
                                            lcmd.CommandText = lcmdquery;
                                            lcmd.ArrayBindCount = ids.Length;
                                            string[] FEEDPROCESSDETAILID_param = dtImport.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                                            string[] DATASETNAME_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DATASETNAME")).ToArray();
                                            string[] DESCRIPTION_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DESCRIPTION")).ToArray();
                                            string[] TAGGROUP_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TAGGROUP")).ToArray();
                                            string[] TAGSET_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TAGSET")).ToArray();
                                            string[] TAGFOLDER_param = dtImport.AsEnumerable().Select(r => r.Field<string>("TAGFOLDER")).ToArray();
                                            string[] EXPECTEDRESULTS_param = dtImport.AsEnumerable().Select(r => r.Field<string>("EXPECTEDRESULTS")).ToArray();
                                            string[] STEPDESC_param = dtImport.AsEnumerable().Select(r => r.Field<string>("STEPDESC")).ToArray();
                                            string[] DIARY_param = dtImport.AsEnumerable().Select(r => r.Field<string>("DIARY")).ToArray();
                                            string[] SEQUENCE_param = dtImport.AsEnumerable().Select(r => r.Field<string>("SEQUENCE")).ToArray();

                                            OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                                            FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                                            FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                                            OracleParameter DATASETNAME_oparam = new OracleParameter();
                                            DATASETNAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                            DATASETNAME_oparam.Value = DATASETNAME_param;

                                            OracleParameter TAGGROUP_oparam = new OracleParameter();
                                            TAGGROUP_oparam.OracleDbType = OracleDbType.Varchar2;
                                            TAGGROUP_oparam.Value = TAGGROUP_param;

                                            OracleParameter TAGSET_oparam = new OracleParameter();
                                            TAGSET_oparam.OracleDbType = OracleDbType.Varchar2;
                                            TAGSET_oparam.Value = TAGSET_param;

                                            OracleParameter TAGFOLDER_oparam = new OracleParameter();
                                            TAGFOLDER_oparam.OracleDbType = OracleDbType.Varchar2;
                                            TAGFOLDER_oparam.Value = TAGFOLDER_param;

                                            OracleParameter EXPECTEDRESULTS_oparam = new OracleParameter();
                                            EXPECTEDRESULTS_oparam.OracleDbType = OracleDbType.Varchar2;
                                            EXPECTEDRESULTS_oparam.Value = EXPECTEDRESULTS_param;

                                            OracleParameter STEPDESC_oparam = new OracleParameter();
                                            STEPDESC_oparam.OracleDbType = OracleDbType.Varchar2;
                                            STEPDESC_oparam.Value = STEPDESC_param;

                                            OracleParameter DIARY_oparam = new OracleParameter();
                                            DIARY_oparam.OracleDbType = OracleDbType.Varchar2;
                                            DIARY_oparam.Value = DIARY_param;

                                            OracleParameter SEQUENCE_oparam = new OracleParameter();
                                            SEQUENCE_oparam.OracleDbType = OracleDbType.Varchar2;
                                            SEQUENCE_oparam.Value = SEQUENCE_param;

                                            OracleParameter DESCRIPTION_oparam = new OracleParameter();
                                            DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                                            DESCRIPTION_oparam.Value = DESCRIPTION_param;


                                            lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                                            lcmd.Parameters.Add(DATASETNAME_oparam);
                                            lcmd.Parameters.Add(TAGGROUP_oparam);
                                            lcmd.Parameters.Add(TAGSET_oparam);
                                            lcmd.Parameters.Add(TAGFOLDER_oparam);
                                            lcmd.Parameters.Add(EXPECTEDRESULTS_oparam);
                                            lcmd.Parameters.Add(STEPDESC_oparam);
                                            lcmd.Parameters.Add(DIARY_oparam);
                                            lcmd.Parameters.Add(SEQUENCE_oparam);
                                            lcmd.Parameters.Add(DESCRIPTION_oparam);
                                            try
                                            {
                                                lcmd.ExecuteNonQuery();
                                                lResult = true;
                                            }
                                            catch (Exception lex)
                                            {
                                                lResult = false;
                                                ltransaction.Rollback();
                                                throw new Exception(lex.Message);
                                            }

                                            ltransaction.Commit();
                                            lconnection.Close();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var tabName = mysheet.Name;
                                dt = DatasetTagCommonValidation(rows, doc, tabName);
                                if (dt.Rows.Count != 0)
                                {
                                    lResult = false;
                                    dbtable.dt_Log.Merge(dt);
                                }

                                if (lResult)
                                {
                                    for (int m = 1; m < rows.Count; m++)
                                    {
                                        var celllst = rows[m].Descendants<Cell>().ToList();

                                        DataRow dr = dtt.NewRow();
                                        dr["FEEDPROCESSDETAILID"] = pFEEDPROCESSDETAILID;
                                        for (int k = 0; k < celllst.Count; k++)
                                        {
                                            if (celllst[k].CellReference != null)
                                            {
                                                var cell = celllst[k].CellReference.ToString();
                                                int rowcell, acell = getIndexofNumber(cell);
                                                string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                                rowcell = Convert.ToInt32(Numberpartcell);
                                                string Stringpart = cell.Substring(0, acell);

                                                if (Stringpart == "A")
                                                    dr["NAME"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);

                                                if (Stringpart == "B")
                                                    dr["DESCRIPTION"] = celllst[k].CellValue == null ? "" : GetValue(doc, celllst[k]);

                                                if (Stringpart == "C")
                                                {
                                                    if (celllst[k].CellValue != null)
                                                        dr["ACTIVE"] = GetValue(doc, celllst[k]) == "Active" ? 1 : 0;
                                                }
                                            }
                                        }

                                        if (dr["NAME"].ToString() == "")
                                            dr["NAME"] = "";

                                        if (dr["DESCRIPTION"].ToString() == "")
                                            dr["DESCRIPTION"] = "";

                                        if (dr["ACTIVE"].ToString() == "")
                                            dr["ACTIVE"] = 0;

                                        dr["TYPE"] = tabName;
                                        dtt.Rows.Add(dr);
                                        dtt.AcceptChanges();
                                    }
                                }
                            }

                        }

                        if (lResult == false)
                        {
                            dtt = null;
                        }

                        //if (dtt != null)
                        if (dtt != null && dtImport != null)
                        {
                            if (dtt.Rows.Count > 0)
                            {
                                OracleTransaction ltransaction;
                                OracleConnection lconnection = new OracleConnection(constring);
                                lconnection.Open();
                                ltransaction = lconnection.BeginTransaction();
                                string lcmdquery = "insert into TBLSTGCOMMONDATASETTAG ( FEEDPROCESSDETAILID,NAME,DESCRIPTION,ACTIVE,TYPE) values(:1,:2,:3,:4,:5)";
                                int[] ids = new int[dtt.Rows.Count];
                                using (var lcmd = lconnection.CreateCommand())
                                {
                                    lcmd.CommandText = lcmdquery;
                                    lcmd.ArrayBindCount = ids.Length;
                                    string[] FEEDPROCESSDETAILID_param = dtt.AsEnumerable().Select(r => r.Field<string>("FEEDPROCESSDETAILID")).ToArray();
                                    string[] NAME_param = dtt.AsEnumerable().Select(r => r.Field<string>("NAME")).ToArray();
                                    string[] DESCRIPTION_param = dtt.AsEnumerable().Select(r => r.Field<string>("DESCRIPTION")).ToArray();
                                    string[] ACTIVE_param = dtt.AsEnumerable().Select(r => r.Field<string>("ACTIVE")).ToArray();
                                    string[] TYPE_param = dtt.AsEnumerable().Select(r => r.Field<string>("TYPE")).ToArray();

                                    OracleParameter FEEDPROCESSDETAILID_oparam = new OracleParameter();
                                    FEEDPROCESSDETAILID_oparam.OracleDbType = OracleDbType.Varchar2;
                                    FEEDPROCESSDETAILID_oparam.Value = FEEDPROCESSDETAILID_param;

                                    OracleParameter NAME_oparam = new OracleParameter();
                                    NAME_oparam.OracleDbType = OracleDbType.Varchar2;
                                    NAME_oparam.Value = NAME_param;

                                    OracleParameter DESCRIPTION_oparam = new OracleParameter();
                                    DESCRIPTION_oparam.OracleDbType = OracleDbType.Varchar2;
                                    DESCRIPTION_oparam.Value = DESCRIPTION_param;

                                    OracleParameter ACTIVE_oparam = new OracleParameter();
                                    ACTIVE_oparam.OracleDbType = OracleDbType.Long;
                                    ACTIVE_oparam.Value = ACTIVE_param;

                                    OracleParameter TYPE_oparam = new OracleParameter();
                                    TYPE_oparam.OracleDbType = OracleDbType.Varchar2;
                                    TYPE_oparam.Value = TYPE_param;

                                    lcmd.Parameters.Add(FEEDPROCESSDETAILID_oparam);
                                    lcmd.Parameters.Add(NAME_oparam);
                                    lcmd.Parameters.Add(DESCRIPTION_oparam);
                                    lcmd.Parameters.Add(ACTIVE_oparam);
                                    lcmd.Parameters.Add(TYPE_oparam);
                                    try
                                    {
                                        lcmd.ExecuteNonQuery();
                                        lResult = true;
                                    }
                                    catch (Exception lex)
                                    {
                                        lResult = false;
                                        ltransaction.Rollback();
                                        throw new Exception(lex.Message);
                                    }

                                    ltransaction.Commit();
                                    lconnection.Close();
                                }
                            }
                        }
                    }
                }
                return lResult;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import DataSetTag Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :importExcelDataSetTag " + ex.Message);
            }
        }

        static DataTable DatasetTagValidation(List<Row> rows, SpreadsheetDocument doc)
        {
            int flag = 0;

            string title = string.Empty;
            string comment = string.Empty;
            string Description = string.Empty;
            string location = string.Empty;
            string TabName = string.Empty;

            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->DatasetTagValidation";
            try
            {
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

                    if (hedarlst[0].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[0]).ToUpper() != "DATASETNAME")
                        {
                            flag = 1;
                            title = "DatasetName Tag";
                            Description = "DatasetName Tag NOT FOUND";

                            ErrorlogExcel(1, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }

                    if (hedarlst[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 1, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[1]).ToUpper() != "DESCRIPTION")
                        {
                            flag = 1;
                            title = "DatasetName Tag";
                            Description = "DatasetName Tag NOT FOUND";

                            ErrorlogExcel(1, 1, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }

                    if (hedarlst[2].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 2, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[2]).ToUpper() != "GROUP")
                        {
                            flag = 1;
                            title = "Group Tag";
                            Description = "Group Tag  NOT FOUND";
                            ErrorlogExcel(1, 2, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[3].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 3, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[3]).ToUpper() != "SET")
                        {
                            flag = 1;
                            title = "Set Tag";
                            Description = "Set TAG NOT FOUND";
                            ErrorlogExcel(1, 3, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[4].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 4, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[4]).ToUpper() != "FOLDER")
                        {
                            flag = 1;
                            title = "Folder Tag";
                            Description = "Folder TAG NOT FOUND";
                            ErrorlogExcel(1, 4, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[5].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 5, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[5]).ToUpper() != "EXPECTEDRESULTS")
                        {
                            flag = 1;
                            title = "ExpectedResults Tag";
                            Description = "ExpectedResults TAG NOT FOUND";
                            ErrorlogExcel(1, 5, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[6].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 6, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[6]).ToUpper() != "STEPDESC")
                        {
                            flag = 1;
                            title = "StepDesc Tag";
                            Description = "StepDesc TAG NOT FOUND";
                            ErrorlogExcel(1, 6, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[7].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 7, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[7]).ToUpper() != "DIARY")
                        {
                            flag = 1;
                            title = "Diary Tag";
                            Description = "Diary TAG NOT FOUND";
                            ErrorlogExcel(1, 7, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[8].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 8, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[8]).ToUpper() != "SEQUENCE")
                        {
                            flag = 1;
                            title = "Sequence Tag";
                            Description = "Sequence TAG NOT FOUND";
                            ErrorlogExcel(1, 8, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }

                    if (hedarlst.Count > 9)
                    {
                        for (int j = 9; j < hedarlst.Count; j++)
                        {
                            if (hedarlst[j].CellValue != null)
                            {
                                if (GetValue(doc, hedarlst[j]) != "")
                                {
                                    flag = 1;
                                    title = "Empty Field";
                                    Description = "Field should be Empty";
                                    ErrorlogExcel(1, Convert.ToInt32(GetColumnIndex(hedarlst[j].CellReference)), "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                                }
                            }
                        }
                    }

                    for (int i = 1; i < rows.Count; i++)
                    {
                        var celllst = rows[i].Descendants<Cell>().ToList();

                        if (celllst.Any())
                        {
                            if (celllst[0].CellValue == null)
                            {
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(i + 1, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", "", "", "", TabName);
                            }

                            if (celllst.Count > 9)
                            {
                                for (int j = 9; j < celllst.Count; j++)
                                {
                                    if (celllst[j].CellValue != null)
                                    {
                                        if (GetValue(doc, celllst[j]) != "")
                                        {
                                            flag = 1;
                                            title = "Empty Field";
                                            Description = "Field should be Empty";
                                            ErrorlogExcel(i + 1, Convert.ToInt32(GetColumnIndex(celllst[j].CellReference)), "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                                        }
                                    }
                                }
                            }
                            var foldername = string.Empty;
                            for (int k = 0; k < celllst.Count; k++)
                            {
                                if (celllst[k].CellReference != null)
                                {
                                    var cell = celllst[k].CellReference.ToString();
                                    int rowcell, acell = getIndexofNumber(cell);
                                    string Numberpartcell = cell.Substring(acell, cell.Length - acell);
                                    rowcell = Convert.ToInt32(Numberpartcell);
                                    string Stringpart = cell.Substring(0, acell);

                                    if (Stringpart == "E")
                                    {
                                        if (celllst[k].CellValue != null)
                                        {
                                            foldername = GetValue(doc, celllst[k]);
                                        }
                                    }

                                    if (Stringpart == "I")
                                    {
                                        var cellval = GetValue(doc, celllst[k]);
                                        if (foldername == "")
                                        {
                                            if (!string.IsNullOrEmpty(cellval))
                                            {
                                                var c = GetColumnIndex(celllst[k].CellReference);
                                                flag = 1;
                                                title = " non Empty Field";
                                                Description = "Sequence Should be Empty.";
                                                ErrorlogExcel(i + 1, Convert.ToInt32(GetColumnIndex(celllst[k].CellReference)), "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                                            }
                                        }

                                        if (cellval != "")
                                        {
                                            var isNumeric = int.TryParse(cellval, out int n);
                                            if (!isNumeric)
                                            {
                                                var c = GetColumnIndex(celllst[k].CellReference);
                                                flag = 1;
                                                title = " non Empty Field";
                                                Description = "Sequence Should be Integer.";
                                                ErrorlogExcel(i + 1, Convert.ToInt32(GetColumnIndex(celllst[k].CellReference)), "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return td;
            }

            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "DatasetTagValidation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :DatasetTagValidation " + ex.Message);
            }
        }

        static DataTable DatasetTagCommonValidation(List<Row> rows, SpreadsheetDocument doc, string TabName)
        {
            int flag = 0;

            string title = string.Empty;
            string comment = string.Empty;
            string Description = string.Empty;
            string location = string.Empty;

            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->DatasetTagCommonValidation";
            try
            {
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

                List<string> Tablist = new List<string>();
                Tablist.Add("GROUP");
                Tablist.Add("SET");
                Tablist.Add("FOLDER");

                if (rows.Any())
                {
                    var hedar = rows[0].Descendants<Cell>();
                    var hedarlst = hedar.ToList();

                    if (!Tablist.Any(str => str.Contains(TabName.ToUpper())))
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = TabName + " wrong tab name and group,set or folder one of them tab missing on sheet";
                        ErrorlogExcel(0, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }

                    if (hedarlst[0].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[0]).ToUpper() != "NAME")
                        {
                            flag = 1;
                            title = "NAME Tag";
                            Description = "NAME Tag NOT FOUND";

                            ErrorlogExcel(1, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[1].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 1, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[1]).ToUpper() != "DESCRIPTION")
                        {
                            flag = 1;
                            title = "DESCRIPTION Tag";
                            Description = "DESCRIPTION Tag  NOT FOUND";
                            ErrorlogExcel(1, 1, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }
                    if (hedarlst[2].CellValue == null)
                    {
                        flag = 1;
                        title = " non Empty Field";
                        Description = "Field should not be Empty";
                        ErrorlogExcel(1, 2, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                    }
                    else
                    {
                        if (GetValue(doc, hedarlst[2]).ToUpper() != "STATUS")
                        {
                            flag = 1;
                            title = "STATUS Tag";
                            Description = "STATUS TAG NOT FOUND";
                            ErrorlogExcel(1, 2, "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                        }
                    }

                    if (hedarlst.Count > 3)
                    {
                        for (int j = 3; j < hedarlst.Count; j++)
                        {
                            if (hedarlst[j].CellValue != null)
                            {
                                if (GetValue(doc, hedarlst[j]) != "")
                                {
                                    flag = 1;
                                    title = "Empty Field";
                                    Description = "Field should be Empty";
                                    ErrorlogExcel(1, Convert.ToInt32(GetColumnIndex(hedarlst[j].CellReference)), "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                                }
                            }
                        }
                    }

                    for (int i = 1; i < rows.Count; i++)
                    {
                        var celllst = rows[i].Descendants<Cell>().ToList();

                        if (celllst.Any())
                        {
                            if (celllst[0].CellValue == null)
                            {
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(i + 1, 0, "", "", "", "", "", "", td, title, Description, "", 0, "", "", "", "", TabName);
                            }

                            if (celllst[2].CellValue == null)
                            {
                                flag = 1;
                                title = " non Empty Field";
                                Description = "Field should not be Empty";
                                ErrorlogExcel(i + 1, 2, "", "", "", "", "", "", td, title, Description, "", 0, "", "", "", "", TabName);
                            }
                            else
                            {
                                if (GetValue(doc, celllst[2]).ToUpper().Trim() != "ACTIVE" && GetValue(doc, celllst[2]).ToUpper().Trim() != "INACTIVE")
                                {
                                    flag = 1;
                                    title = "non Empty Field";
                                    Description = "Field should have value active or inactive";
                                    ErrorlogExcel(i + 1, 2, "", "", "", "", "", "", td, title, Description, "", 0, "", "", "", "", TabName);
                                }
                            }

                            if (celllst.Count > 3)
                            {
                                for (int j = 3; j < celllst.Count; j++)
                                {
                                    if (celllst[j].CellValue != null)
                                    {
                                        if (GetValue(doc, celllst[j]) != "")
                                        {
                                            flag = 1;
                                            title = "Empty Field";
                                            Description = "Field should be Empty";
                                            ErrorlogExcel(i + 1, Convert.ToInt32(GetColumnIndex(celllst[j].CellReference)), "", "", "", "", "", "", td, title, Description, "", 0, "", comment, location, "", TabName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return td;
            }

            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "DatasetTagCommonValidation", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :DatasetTagCommonValidation " + ex.Message);
            }
        }
    }
}
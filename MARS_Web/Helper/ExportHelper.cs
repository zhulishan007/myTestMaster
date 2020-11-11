
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Font = DocumentFormat.OpenXml.Spreadsheet.Font;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;

namespace MARS_Web.Helper
{
  public class ExportHelper
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);
    public void ReleaseObject(object obj)
    {
      try
      {
        Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Error from : releaseObject" + ex.Message);
      }
      finally
      {
        GC.Collect();
      }
    }
    //public string ExportProject(string projectname, string lstrConn, string schema, string TempLocation)
    //{

    //  try
    //  {

    //    bool lResult = false; // if result is flase then error will no suitename found in db.


    //    //string lFileName = System.DateTime.Now.ToString("MM-dd-yyyy") + "-TestCase.xlsx";
    //    string lFileName = projectname + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
    //    //Create the data set and table

    //    DataSet lds = new DataSet();
    //    DataTable ldt = new DataTable();

    //    OracleConnection lconnection = GetOracleConnection(lstrConn);
    //    lconnection.Open();

    //    OracleTransaction ltransaction;
    //    ltransaction = lconnection.BeginTransaction();

    //    OracleCommand lcmd;
    //    lcmd = lconnection.CreateCommand();
    //    lcmd.Transaction = ltransaction;



    //    OracleParameter[] ladd_refer_image = new OracleParameter[2];
    //    ladd_refer_image[0] = new OracleParameter("PROJECT", OracleDbType.Varchar2);
    //    ladd_refer_image[0].Value = projectname;

    //    ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
    //    ladd_refer_image[1].Direction = ParameterDirection.Output;

    //            foreach (OracleParameter p in ladd_refer_image)
    //            {
    //                lcmd.Parameters.Add(p);
    //            }

    //    //The name of the Procedure responsible for inserting the data in the table.
    //    lcmd.CommandText = schema + "." + "SP_EXPORT_STORYBOARD";
    //    lcmd.CommandType = CommandType.StoredProcedure;
    //    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
    //    dataAdapter.Fill(lds);

    //    Regex re = new Regex("[;\\/:*?\"<>|&']");
    //    lconnection.Close();
    //    var FullPath = ConvertDataSetToExcelStoryBoard(lds, lFileName, TempLocation);


    //    return FullPath;
    //  }
    //  catch (Exception ex)
    //  {
    //    string msg = ex.Message;
    //    throw new Exception("Error from : ExportTestCase " + ex.Message);
    //  }

    //}
    public string ExportTestSuite(string pTestSuite, string lstrConn, string schema, string TempLocation)
    {

      try
      {

        bool lResult = false; // if result is flase then error will no suitename found in db.

        string lFileName = pTestSuite + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
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

        OracleParameter[] ladd_refer_image = new OracleParameter[2];
        ladd_refer_image[0] = new OracleParameter("TESTSUITENAME", OracleDbType.Varchar2);
        ladd_refer_image[0].Value = pTestSuite;

        ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
        ladd_refer_image[1].Direction = ParameterDirection.Output;

        foreach (OracleParameter p in ladd_refer_image)
        {
          lcmd.Parameters.Add(p);

        }


        lcmd.CommandText = schema + "." + "SP_EXPORT_TESTSUITE";
        lcmd.CommandType = CommandType.StoredProcedure;
        OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
        dataAdapter.Fill(lds);

        Regex re = new Regex("[;\\/:*?\"<>|&']");
        lconnection.Close();
        var FullPath = WriteExcel(lds, TempLocation);

        return FullPath;
      }
      catch (Exception ex)
      {
        string msg = ex.Message;
        throw new Exception("Error from : ExportTestCase " + ex.Message);
      }

    }
  
    public static OracleConnection GetOracleConnection(string StrConnection)
    {
      return new OracleConnection(StrConnection);
    }

    public string ExportTestCase(string pTestCase, string pTestSuite, string lstrConn, string schema, string TempLocation)
    {
      try
      {

        bool lResult = false; // if result is flase then error will no suitename found in db.


        //string lFileName = System.DateTime.Now.ToString("MM-dd-yyyy") + "-TestCase.xlsx";
        string lFileName = pTestCase + "_" + pTestSuite + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
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
        var FullPath = WriteExcel(lds, TempLocation);

        return FullPath;
      }
      catch (Exception ex)
      {
        string msg = ex.Message;
        throw new Exception("Error from : ExportTestCase " + ex.Message);
      }

    }
    //public string ExportStoryboard(string pstoryboard, string pproject, string lstrConn, string schema, string TempLocation)
    //{
    //  try
    //  {

    //    bool lResult = false; // if result is flase then error will no suitename found in db.


    //    //string lFileName = System.DateTime.Now.ToString("MM-dd-yyyy") + "-TestCase.xlsx";
    //    string lFileName = pstoryboard + "_" + pproject + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
    //    //Create the data set and table
    //    DataSet lds = new DataSet();
    //    DataTable ldt = new DataTable();

    //    OracleConnection lconnection = GetOracleConnection(lstrConn);
    //    lconnection.Open();

    //    OracleTransaction ltransaction;
    //    ltransaction = lconnection.BeginTransaction();

    //    OracleCommand lcmd;
    //    lcmd = lconnection.CreateCommand();
    //    lcmd.Transaction = ltransaction;



    //    OracleParameter[] ladd_refer_image = new OracleParameter[3];
    //    ladd_refer_image[0] = new OracleParameter("PROJECT", OracleDbType.Varchar2);
    //    ladd_refer_image[0].Value = pproject;

    //    ladd_refer_image[1] = new OracleParameter("Storyboardname", OracleDbType.Varchar2);
    //    ladd_refer_image[1].Value = pstoryboard;

    //    ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
    //    ladd_refer_image[2].Direction = ParameterDirection.Output;

    //    foreach (OracleParameter p in ladd_refer_image)
    //    {
    //      lcmd.Parameters.Add(p);
    //    }


    //    //The name of the Procedure responsible for inserting the data in the table.
    //    lcmd.CommandText = schema + "." + "SP_EXPORT_STORYBOARDNEW";
    //    lcmd.CommandType = CommandType.StoredProcedure;
    //    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
    //    dataAdapter.Fill(lds);

    //    Regex re = new Regex("[;\\/:*?\"<>|&']");
    //    lconnection.Close();
    //    var FullPath = ConvertDataSetToExcelStoryBoard(lds, lFileName, TempLocation);


    //    return FullPath;
    //  }
    //  catch (Exception ex)
    //  {
    //    string msg = ex.Message;
    //    throw new Exception("Error from : ExportTestCase " + ex.Message);
    //  }

    //}

    //public string ExportVariable(string lstrConn, string schema, string lExportLogPath)
    //{
    //  try
    //  {

    //    string lFileName = "variable" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
    //    //Create the data set and table
    //    Boolean lresult = true;
    //    DataSet lds = new DataSet();

    //    OracleConnection lconnection = GetOracleConnection(lstrConn);
    //    lconnection.Open();

    //    OracleTransaction ltransaction;
    //    ltransaction = lconnection.BeginTransaction();

    //    OracleCommand lcmd;
    //    lcmd = lconnection.CreateCommand();
    //    lcmd.Transaction = ltransaction;

    //    OracleParameter[] ladd_refer_image = new OracleParameter[1];

    //    ladd_refer_image[0] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
    //    ladd_refer_image[0].Direction = ParameterDirection.Output;

    //    foreach (OracleParameter p in ladd_refer_image)
    //    {
    //      lcmd.Parameters.Add(p);
    //    }


    //    //The name of the Procedure responsible for inserting the data in the table.
    //    lcmd.CommandText = schema + "." + "SP_EXPORT_VARIABLE";
    //    lcmd.CommandType = CommandType.StoredProcedure;
    //    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
    //    dataAdapter.Fill(lds);




    //    lconnection.Close();
    //    var FullPath = ConvertDataSetToExcelVariable(lds, lFileName, lExportLogPath);

    //    return FullPath;

    //  }
    //  catch (Exception ex)
    //  {
    //    int line;
    //    string msg = ex.Message;
    //    //line = dbtable.lineNo(ex);
    //    //dbtable.errorlog(msg, "export variable", SomeGlobalVariables.functionName, line);
    //    throw new Exception("Error from :Exportvariable " + ex.Message);

    //  }
    //}
    //public string ConvertDataSetToExcelVariable(DataSet ds, string lfilename, string lExportLogPath)
    //{
    //  string fullPath = "";
    //  XL.Application lexcelApp = new XL.Application();
    //  XL.Workbook lexcelWorkBook = null;
    //  object lmisValue = System.Reflection.Missing.Value;
    //  try
    //  {
    //    //Creae an Excel application instance
    //    //XL.Application lexcelApp = new XL.Application();
    //    string Path = lExportLogPath;
    //    int cp = 0;
    //    //Create an Excel workbook instance and open it from the predefined location
    //    //XL.Workbook lexcelWorkBook = null;
    //    lexcelWorkBook = lexcelApp.Workbooks.Add();
    //    //object lmisValue = System.Reflection.Missing.Value;

    //    foreach (System.Data.DataTable ltable in ds.Tables)
    //    {
    //      //Add a new worksheet to workbook with the Datatable name
    //      XL.Worksheet lexcelWorkSheet = lexcelWorkBook.Sheets.Add();
    //      lexcelWorkSheet.Name = ltable.TableName;

    //      for (int i = 1; i < ltable.Columns.Count + 1; i++)
    //      {
    //        lexcelWorkSheet.Cells[1, i] = ltable.Columns[i - 1].ColumnName;
    //        lexcelWorkSheet.Cells[1, i].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(83, 133, 3));
    //        lexcelWorkSheet.Cells[1, i].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(187, 226, 227));
    //      }

    //      for (int j = 0; j < ltable.Rows.Count; j++)
    //      {
    //        for (int k = 0; k < ltable.Columns.Count; k++)
    //        {
    //          lexcelWorkSheet.Cells[j + 2, k + 1] = ltable.Rows[j].ItemArray[k].ToString();
    //        }
    //        cp++;
    //        lexcelWorkSheet.Columns.AutoFit();
    //      }
    //      XL.Range cell1 = lexcelWorkSheet.Cells[1, 1];
    //      XL.Range cell2 = lexcelWorkSheet.Cells[cp + 1, 4];
    //      XL.Range range = lexcelWorkSheet.get_Range(cell1, cell2);
    //      range.BorderAround(lmisValue, XL.XlBorderWeight.xlMedium, XL.XlColorIndex.xlColorIndexAutomatic, ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 192, 0)));
    //      range.Cells.Borders.Weight = XL.XlBorderWeight.xlThin;

    //      ReleaseObject(lexcelWorkSheet);

    //      if (ltable.Rows.Count > 0)
    //      {

    //        foreach (XL.Worksheet sheet in lexcelWorkBook.Worksheets)
    //        {
    //          if (sheet.UsedRange.Count < 2)
    //          {
    //            sheet.Delete();
    //          }
    //        }
    //      }
    //    }
    //    fullPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), lfilename);
    //    lexcelWorkBook.SaveAs(fullPath, XL.XlFileFormat.xlOpenXMLWorkbook, lmisValue, lmisValue, lmisValue, lmisValue, XL.XlSaveAsAccessMode.xlExclusive, lmisValue, lmisValue, lmisValue, lmisValue, lmisValue);

    //    //lexcelWorkBook.Close(true, lmisValue, lmisValue);
    //    //lexcelApp.Quit();


    //    //releaseObject(lexcelWorkBook);
    //    //releaseObject(lexcelApp);
    //    //lexcelWorkBook.Close(0);
    //    //lexcelApp.Quit();
    //  }
    //  catch (Exception ex)
    //  {
    //    int line;
    //    string msg = ex.Message;
    //    // line = dbtable.lineNo(ex);
    //    //dbtable.errorlog(msg, "Convert dataset to excel object", SomeGlobalVariables.functionName, line);
    //    throw new Exception("Error from : ConvertDataSetToExcelObject" + ex.Message);
    //  }
    //  finally
    //  {
    //    lexcelWorkBook.Close(0);
    //    lexcelApp.Quit();
    //    int ProId;
    //    GetWindowThreadProcessId(lexcelApp.Hwnd, out ProId);
    //    Process[] AllProcesses = Process.GetProcessesByName("excel");
    //    foreach (Process ExcelProcess in AllProcesses)
    //    {
    //      if (ExcelProcess.Id == ProId)
    //      {
    //        ExcelProcess.Kill();
    //      }
    //    }
    //    AllProcesses = null;
    //  }

    //  return fullPath;
    //}
    //public string ExportAllStoryboards(string lstrConn, string schema, string TempLocation)
    //{
    //  try
    //  {

    //    bool lResult = false; // if result is flase then error will no suitename found in db.

    //    //string lFileName = System.DateTime.Now.ToString("MM-dd-yyyy") + "-TestCase.xlsx";
    //    string lFileName = "Storyboards" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
    //    //Create the data set and table
    //    DataSet lds = new DataSet();
    //    DataTable ldt = new DataTable();

    //    OracleConnection lconnection = GetOracleConnection(lstrConn);
    //    lconnection.Open();

    //    OracleTransaction ltransaction;
    //    ltransaction = lconnection.BeginTransaction();

    //    OracleCommand lcmd;
    //    lcmd = lconnection.CreateCommand();
    //    lcmd.Transaction = ltransaction;

    //    OracleParameter[] ladd_refer_image = new OracleParameter[1];

    //    ladd_refer_image[0] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
    //    ladd_refer_image[0].Direction = ParameterDirection.Output;

    //    foreach (OracleParameter p in ladd_refer_image)
    //    {
    //      lcmd.Parameters.Add(p);
    //    }

    //    //The name of the Procedure responsible for inserting the data in the table.
    //    lcmd.CommandText = schema + "." + "SP_EXPORT_ALL_STORYBOARDS";
    //    lcmd.CommandType = CommandType.StoredProcedure;
    //    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
    //    dataAdapter.Fill(lds);

    //    Regex re = new Regex("[;\\/:*?\"<>|&']");
    //    lconnection.Close();
    //    var FullPath = ConvertDataSetToExcelStoryBoard(lds, lFileName, TempLocation);

    //    return FullPath;
    //  }
    //  catch (Exception ex)
    //  {
    //    string msg = ex.Message;
    //    throw new Exception("Error from : ExportTestCase " + ex.Message);
    //  }

    //}
    //public string ExportAllTestSuites(string lstrConn, string schema, string TempLocation)
    //{
    //  try
    //  {

    //    bool lResult = false; // if result is flase then error will no suitename found in db.


    //    //string lFileName = System.DateTime.Now.ToString("MM-dd-yyyy") + "-TestCase.xlsx";
    //    string lFileName = "TestSuites" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
    //    //Create the data set and table
    //    DataSet lds = new DataSet();
    //    DataTable ldt = new DataTable();

    //    OracleConnection lconnection = GetOracleConnection(lstrConn);
    //    lconnection.Open();

    //    OracleTransaction ltransaction;
    //    ltransaction = lconnection.BeginTransaction();

    //    OracleCommand lcmd;
    //    lcmd = lconnection.CreateCommand();
    //    lcmd.Transaction = ltransaction;



    //    OracleParameter[] ladd_refer_image = new OracleParameter[1];

    //    ladd_refer_image[0] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
    //    ladd_refer_image[0].Direction = ParameterDirection.Output;

    //    foreach (OracleParameter p in ladd_refer_image)
    //    {
    //      lcmd.Parameters.Add(p);
    //    }


    //    //The name of the Procedure responsible for inserting the data in the table.
    //    lcmd.CommandText = schema + "." + "SP_EXPORT_ALL_TESTSUITES";
    //    lcmd.CommandType = CommandType.StoredProcedure;
    //    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
    //    dataAdapter.Fill(lds);

    //    Regex re = new Regex("[;\\/:*?\"<>|&']");
    //    lconnection.Close();
    //    var FullPath = ConvertDataSetToExcelTestSuite(lds, lFileName, TempLocation);



    //    return FullPath;
    //  }
    //  catch (Exception ex)
    //  {
    //    string msg = ex.Message;
    //    throw new Exception("Error from : ExportTestCase " + ex.Message);
    //  }

    //}
    //public string ExportObject(string pApplication, string lstrconn, string schema, string lExportLogPath)
    //{
    //  try
    //  {

    //    bool lResult = true; // if result is flase then error will no APPLICATION found in db.

    //    string lFileName = pApplication + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + "_" + "Object" + ".xlsx";
    //    //Create the data set and table
    //    DataSet lds = new DataSet();

    //    OracleConnection lconnection = GetOracleConnection(lstrconn);
    //    lconnection.Open();

    //    OracleTransaction ltransaction;
    //    ltransaction = lconnection.BeginTransaction();

    //    OracleCommand lcmd;
    //    lcmd = lconnection.CreateCommand();
    //    lcmd.Transaction = ltransaction;

    //    OracleParameter[] ladd_refer_image = new OracleParameter[2];
    //    ladd_refer_image[0] = new OracleParameter("APPLICATION", OracleDbType.Varchar2);
    //    ladd_refer_image[0].Value = pApplication;
    //    ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
    //    ladd_refer_image[1].Direction = ParameterDirection.Output;

    //    foreach (OracleParameter p in ladd_refer_image)
    //    {
    //      lcmd.Parameters.Add(p);
    //    }


    //    //The name of the Procedure responsible for inserting the data in the table.
    //    lcmd.CommandText = schema + "." + "SP_EXPORT_EXPORTOBJECT";
    //    lcmd.CommandType = CommandType.StoredProcedure;
    //    OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
    //    dataAdapter.Fill(lds);




    //    lconnection.Close();
    //    var fullpath = ConvertDataSetToExcelObject(lds, lFileName, pApplication);

    //    return fullpath;
    //  }
    //  catch (Exception ex)
    //  {

    //    throw new Exception("Error from :ExportObject " + ex.Message);
    //  }
    //}
    //public string ConvertDataSetToExcelObject(DataSet ds, string filename, string application)
    //{
    //  XL.Application lexcelApp = new XL.Application();
    //  XL.Workbook lexcelWorkBook = null;
    //  object lmisValue = System.Reflection.Missing.Value;
    //  try
    //  {

    //    //string Path = lExportPath;
    //    int cp = 0;
    //    //Create an Excel workbook instance and open it from the predefined location
    //    //XL.Workbook lexcelWorkBook = null;
    //    lexcelWorkBook = lexcelApp.Workbooks.Add();
    //    //object lmisValue = System.Reflection.Missing.Value;

    //    foreach (System.Data.DataTable ltable in ds.Tables)
    //    {
    //      //Add a new worksheet to workbook with the Datatable name
    //      XL.Worksheet lexcelWorkSheet = lexcelWorkBook.Sheets.Add();
    //      lexcelWorkSheet.Name = ltable.TableName;

    //      lexcelWorkSheet.Cells[1, 1].Value = application;


    //      for (int i = 1; i < ltable.Columns.Count + 1; i++)
    //      {
    //        lexcelWorkSheet.Cells[2, i] = ltable.Columns[i - 1].ColumnName;
    //        lexcelWorkSheet.Cells[2, i].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(83, 133, 3));
    //        lexcelWorkSheet.Cells[2, i].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(187, 226, 227));
    //      }

    //      for (int j = 0; j < ltable.Rows.Count; j++)
    //      {
    //        for (int k = 0; k < ltable.Columns.Count; k++)
    //        {
    //          lexcelWorkSheet.Cells[j + 3, k + 1] = ltable.Rows[j].ItemArray[k].ToString();
    //        }
    //        cp++;
    //        lexcelWorkSheet.Columns.AutoFit();
    //      }
    //      XL.Range cell1 = lexcelWorkSheet.Cells[1, 1];
    //      XL.Range cell2 = lexcelWorkSheet.Cells[cp + 2, 7];
    //      XL.Range range = lexcelWorkSheet.get_Range(cell1, cell2);
    //      range.BorderAround(lmisValue, XL.XlBorderWeight.xlMedium, XL.XlColorIndex.xlColorIndexAutomatic, ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 192, 0)));
    //      range.Cells.Borders.Weight = XL.XlBorderWeight.xlThin;



    //      ReleaseObject(lexcelWorkSheet);


    //      if (ltable.Rows.Count > 0)
    //      {

    //                    foreach (XL.Worksheet sheet in lexcelWorkBook.Worksheets)
    //                    {
    //                        if (sheet.UsedRange.Count < 2)
    //                        {
    //                            sheet.Delete();
    //                        }
    //                    }
    //                }
    //            }
    //           var fullPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TempExport/"), filename);
    //            lexcelWorkBook.SaveAs(fullPath, XL.XlFileFormat.xlOpenXMLWorkbook, lmisValue, lmisValue, lmisValue, lmisValue, XL.XlSaveAsAccessMode.xlExclusive, lmisValue, lmisValue, lmisValue, lmisValue, lmisValue);
    //            return fullPath;
    //        }
    //        catch (Exception ex)
    //        {
               
    //            throw new Exception("Error from : ConvertDataSetToExcelObject" + ex.Message);
    //        }
    //        finally
    //        {
    //            lexcelWorkBook.Close(0);
    //            lexcelApp.Quit();
    //            int ProId;
    //            GetWindowThreadProcessId(lexcelApp.Hwnd, out ProId);
    //            Process[] AllProcesses = Process.GetProcessesByName("excel");
    //            foreach (Process ExcelProcess in AllProcesses)
    //            {
    //                if (ExcelProcess.Id == ProId)
    //                {
    //                    ExcelProcess.Kill();
    //                }
    //            }
    //            AllProcesses = null;
    //        }
    //    }

    public Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
    {
     
      return new Cell()
      {

        CellValue = new CellValue(value),
        DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(dataType),
        StyleIndex = styleIndex
      };
    }
    public Cell ConstructCellWithRef(string value, CellValues dataType, uint styleIndex = 0,string cellref="")
    {
      
      return new Cell()
      {
        CellReference = cellref,
        CellValue = new CellValue(value),
        DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(dataType),
        StyleIndex = styleIndex
      };
    }
    public Stylesheet GenerateStylesheet()
    {
      Stylesheet styleSheet = null;

      Fonts fonts = new Fonts(
          new Font( // Index 0 - default
              new FontSize() { Val = 11D },
              new FontName() { Val = "Calibri" }
          ),
          new Font( // Index 1 - header
              new FontSize() { Val = 11D },
              new Color() { Rgb = "155e22" },
              new FontName() { Val = "Calibri" }
          ),
           new Font( // Index 2 
              new FontSize() { Val = 11D },
              new Color() { Rgb = "b37f07" },
              new FontName() { Val = "Calibri" }
          )
          );

      Fills fills = new Fills(
              new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
              new Fill(new PatternFill() { PatternType = PatternValues.LightUp }), // Index 1 - default
              new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "c3ebe2" } })
              { PatternType = PatternValues.Solid }), // Index 2 - header
              new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "f5cf6e" } })
              { PatternType = PatternValues.Solid }) // Index 3 
          );

      Borders borders = new Borders(
              new Border(), // index 0 default
              new Border( // index 1 black border
                  new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                  new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                  new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                  new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                  new DiagonalBorder())
          );

      CellFormats cellFormats = new CellFormats(
              new CellFormat(), // default
              new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // body
              new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true }, // header
              new CellFormat { FontId = 2, FillId = 3, BorderId = 1, ApplyFill = true } // extra
          );

      styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

      return styleSheet;
    }


    public Columns ObjectColumnsStyle()
    {
      Columns columns = new Columns(
                new Column
                {
                  Min = 1,
                  Max = 1,
                  Width = 40,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 2,
                  Max = 2,
                  Width = 12,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 3,
                  Max = 3,
                  Width = 40,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 4,
                  Max = 4,
                  Width = 20,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 5,
                  Max = 5,
                  Width = 10,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 6,
                  Max = 6,
                  Width = 12,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 7,
                  Max = 7,
                  Width = 5,
                  CustomWidth = true
                });

      return columns;
    }

    public Columns VariableColumnsStyle()
    {
      Columns columns = new Columns(
                new Column
                {
                  Min = 1,
                  Max = 1,
                  Width = 25,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 2,
                  Max = 2,
                  Width = 20,
                  CustomWidth = true
                },
                new Column
                {
                  Min = 3,
                  Max = 4,
                  Width = 12,
                  CustomWidth = true
                });

      return columns;
    }

        public Columns StoryBordColumnsStyle()
        {
            Columns columns = new Columns(
                      new Column
                      {
                          Min = 1,
                          Max = 1,
                          Width = 20,
                          CustomWidth = true,
                      },
                      new Column
                      {
                          Min = 2,
                          Max = 2,
                          Width = 25,
                          CustomWidth = true
                      },
                       new Column
                       {
                           Min = 3,
                           Max = 3,
                           Width = 20,
                           CustomWidth = true
                       },
                      new Column
                      {
                          Min = 4,
                          Max = 7,
                          Width = 15,
                          CustomWidth = true
                      });

      return columns;
    }
        public Columns TestCaseColumnsStyle()
        {
            Columns columns = new Columns(
                     new Column
                     {
                         Min = 1,
                         Max = 1,
                         Width = 20,
                         CustomWidth = true,
                         BestFit=true
                     },
                      new Column
                      {
                          Min = 2,
                          Max = 2,
                          Width = 30,
                          CustomWidth = true,
                          BestFit = true
                      },
                       new Column
                       {
                           Min = 3,
                           Max = 3,
                           Width = 20,
                           CustomWidth = true,
                           BestFit = true
                       },
                        new Column
                        {
                            Min = 4,
                            Max = 4,
                            Width = 20,
                            CustomWidth = true,
                            BestFit = true
                        }
                     );
            return columns;
        }


            public string WriteExcel(DataSet ds, string excelFile)
    {
      DataTable ldt1 = new DataTable();
      RangeSet R1 = null;
      RangeSet R2 = null;
      ldt1.Columns.Add("KEYWORD");
      ldt1.Columns.Add("OBJECT");
      ldt1.Columns.Add("PARAMETER");
      ldt1.Columns.Add("COMMENTS");

      string lTSName = string.Empty, lTCName = string.Empty, lTCDesc = string.Empty;

      DataTable ldtTESTCASE = new DataTable();
      ldtTESTCASE = ds.Tables[0];

      DataView dtvTESTCASE = new DataView();
      dtvTESTCASE = ldtTESTCASE.DefaultView;

      DataTable dtTESTCASE = new DataTable();
      dtTESTCASE = dtvTESTCASE.Table;

      ldtTESTCASE = dtTESTCASE;

      ldtTESTCASE = ldtTESTCASE.AsEnumerable()
          .GroupBy(r => new { Col1 = r["TEST_CASE_NAME"] })
          .Select(g => g.OrderBy(r => r["TEST_CASE_NAME"]).First())
          .CopyToDataTable();
      string KEYWORD = string.Empty;
      string OBJECT = string.Empty;
      string PARAMETER = string.Empty;
      string COMMENTS = string.Empty;

      string DATASETNAME = string.Empty;
      string DATASETVALUE = string.Empty;
      string[] applications;
      string[] datasetnames;
      string[] datavalues;

      string[] datasetdescription;
      string[] skipvalues;
      using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFile, SpreadsheetDocumentType.Workbook))
      {
        WorkbookPart workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();
        
                
                WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylePart.Stylesheet = GenerateStylesheet();
                stylePart.Stylesheet.Save();
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
               
        if (ldtTESTCASE.Rows.Count > 0)
        {
                    List<string> tabnamelist = new List<string>();
                    int tabid = 1;
          for (int m = 0; m < ldtTESTCASE.Rows.Count; m++)
          {
            DataTable ldt = new DataTable();
            ldt = ds.Tables[0];

            DataView dtv = new DataView();
            dtv = ldt.DefaultView;

            DataTable dtTable = new DataTable();
            dtTable = dtv.Table;

            ldt = (from datatabledt in ldt.AsEnumerable()
                   where datatabledt.Field<string>("TEST_CASE_NAME") == ldtTESTCASE.Rows[m]["TEST_CASE_NAME"].ToString()
                   select datatabledt).CopyToDataTable();
            Regex re = new Regex("[;\\/:*?\"<>|&']");
            string tabname = re.Replace(ldtTESTCASE.Rows[m]["TEST_CASE_NAME"].ToString(), "_").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");
                        tabname = tabname.Length > 30 ? tabname.Substring(0, 29) : tabname;
                        if (tabnamelist.Count(x => x.Equals(tabname)) == 1)
                        {
                            tabname = tabname.Trim() + "_" + tabid;
                            tabid++;
                        }
                           
                        tabnamelist.Add(tabname);
                        KEYWORD = string.Empty;
                        OBJECT = string.Empty;
            PARAMETER = string.Empty;
            COMMENTS = string.Empty;

            DATASETNAME = string.Empty;
            DATASETVALUE = string.Empty;
            applications = null;
            datasetnames = null;
            datavalues = null;

            datasetdescription = null;
            skipvalues = null;
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();

                        worksheetPart.Worksheet.AppendChild(TestCaseColumnsStyle());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = Convert.ToUInt32(m + 1), Name = tabname};

              sheets.Append(sheet);
              workbookPart.Workbook.Save();
              SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

              Row arow = new Row();
              Row keyrow = new Row();

              uint newrowindex = 9;
              for (int k = 0; k < ldt.Rows.Count; k++)
              {
                if (k == 0)
                {
                  arow.Append(
                    ConstructCell("Application", CellValues.String, 1));
                  var acellref = "B1";
                  applications = Convert.ToString(ldt.Rows[k]["APPLICATION"]).Split(',');
                                for (int l = 0; l < applications.Length; l++)
                                {
                                        Cell cells = new Cell();
                                        cells.CellReference = acellref;
                                        cells.CellValue = new CellValue(applications[l]);
                                        cells.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.String);
                                        cells.StyleIndex = 1;
                                    acellref = IncrementColumnCellReference(acellref);
                                    arow.AppendChild(cells);
                                   
                                }
                                sheetData.AppendChild(arow);
                  Row srow = new Row();
                  srow.Append(
                    ConstructCell("Test Suite Name", CellValues.String, 1),
                    ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["TEST_SUITE_NAME"]), CellValues.String, 1, "B2"));
                  sheetData.AppendChild(srow);

                  Row crow = new Row();
                  crow.Append(
                   ConstructCell("Test Case Name", CellValues.String, 1),
                   ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["TEST_CASE_NAME"]), CellValues.String, 1, "B3"));
                  sheetData.AppendChild(crow);

                  Row drow = new Row();
                  drow.Append(
                   ConstructCell("Test Case Description", CellValues.String, 1),
                   ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["test_step_description"]), CellValues.String, 1, "B4"));
                  sheetData.AppendChild(drow);

                  Row mrow = new Row();
                  mrow.Append(
                    ConstructCell("Mode", CellValues.String, 3));
                  sheetData.AppendChild(mrow);

                  Row frow = new Row() { RowIndex = 6 };
                  frow.Append(
                   ConstructCell("Keyword", CellValues.String, 2),
                   ConstructCell("Object", CellValues.String, 2),
                   ConstructCell("Parameters", CellValues.String, 2),
                   ConstructCell("Comment", CellValues.String, 2)
                   );

                  //worksheetPart.Worksheet.Save();
                  var cellref = "";
                  var cellref1 = "";
                  var descref = "";
                  var descref1 = "";
                  var skipref = "E8";
                  var dataref = "F8";
                  int b = 0;
                  int desc = 0;
                  datasetnames = Convert.ToString(ldt.Rows[ldt.Rows.Count - 1]["DATASETNAME"]).Split(',');
                  MergeCells mergeCells = new MergeCells();
                                for (int l = 0; l < datasetnames.Length; l++)
                                {
                                    if (b == 0)
                                    {
                                        Cell cell = ConstructCellWithRef(datasetnames[l], CellValues.String, 2, "E6");
                                        frow.AppendChild(cell);
                                        b++;
                                        cellref = IncrementColumnCellReference("E6");
                                        MergeCell mergeCellt = new MergeCell() { Reference = new StringValue("E6:" + cellref) };

                                        mergeCells.Append(mergeCellt);
                                        //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                                    }
                                    else
                                    {
                                        cellref = IncrementColumnCellReference(cellref);
                                        Cell cell = ConstructCellWithRef(datasetnames[l], CellValues.String, 2, cellref);
                                        frow.AppendChild(cell);
                                        cellref1 = IncrementColumnCellReference(cellref);
                                        MergeCell mergeCellt = new MergeCell() { Reference = new StringValue(cellref1 + ":" + cellref) };
                                        mergeCells.Append(mergeCellt);
                                    }
                                    //worksheetPart.Worksheet.Save();
                                    if (cellref1 != "")
                                        cellref = cellref1;
                                }
                               
                                sheetData.AppendChild(frow);
                                Row descrow = new Row();
                                Row datarow = new Row() { RowIndex = (UInt32)8 };
                                datasetdescription = Convert.ToString(ldt.Rows[k]["DATASETDESCRIPTION"]).Split(',');
                              
                                for (int l = 0; l < datasetdescription.Length; l++)
                                {

                                    if (desc == 0)
                                    {
                                        Cell cell = ConstructCellWithRef(datasetdescription[l], CellValues.String, 2, "E7");
                                        descrow.AppendChild(cell);
                                        desc++;
                                        descref = IncrementColumnCellReference("E7");
                                        MergeCell mergeCellt = new MergeCell() { Reference = new StringValue("E7:" + descref) };
                                        mergeCells.Append(mergeCellt);

                                    }
                                    else
                                    {
                                        descref = IncrementColumnCellReference(descref);
                                        Cell cell = ConstructCellWithRef(datasetdescription[l], CellValues.String, 2, descref);
                                        descrow.AppendChild(cell);
                                        descref1 = IncrementColumnCellReference(descref);
                                        MergeCell mergeCellt = new MergeCell() { Reference = new StringValue(descref1 + ":" + descref) };
                                        mergeCells.Append(mergeCellt);

                                    }
                                    Cell cellskip = ConstructCellWithRef("SKIP Flag", CellValues.String, 2, skipref);
                                    skipref = IncrementColumnCellReference(skipref);
                                    skipref = IncrementColumnCellReference(skipref);
                                    datarow.Append(cellskip);
                                    Cell celldata = ConstructCellWithRef("Data", CellValues.String, 2, dataref);
                                    dataref = IncrementColumnCellReference(dataref);
                                    dataref = IncrementColumnCellReference(dataref);
                                    datarow.Append(celldata);
                                    if (descref1 != "")
                                        descref = descref1;

                                }
                                MergeCell mergeCellr = new MergeCell() { Reference = new StringValue("A7:D8") };
                                mergeCells.Append(mergeCellr);
                                if (mergeCells != null)
                                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                                sheetData.AppendChild(descrow);
                               sheetData.AppendChild(datarow);
                               // worksheetPart.Worksheet.Save();

                            }
                keyrow = new Row() { RowIndex = (UInt32)newrowindex };
                skipvalues = Convert.ToString(ldt.Rows[k]["SKIP"]).Split(',');
                datavalues = Convert.ToString(ldt.Rows[k]["DATASETVALUE"]).Split(',');

                            var lColumnList = new List<OpenXmlElement>();
                            lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["KEY_WORD_NAME"]), CellValues.String, 1));
                            lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["OBJECT_HAPPY_NAME"]), CellValues.String, 1));
                            lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["PARAMETER"]), CellValues.String, 1));
                            lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["COMMENT"]), CellValues.String, 1));
                            var datasetlength = datasetnames.Length;//datatset length
                            for (int i = 0; i < datasetlength; i++)
                            {
                                if (!string.IsNullOrEmpty(skipvalues[i]))
                                {
                                    lColumnList.Add(ConstructCell(Convert.ToString(skipvalues[i] == "0" ? "" : "SKIP"), CellValues.String, 1));
                                }
                                else
                                {
                                    lColumnList.Add(ConstructCell(Convert.ToString(""), CellValues.String, 1));
                                }
                                if (datavalues.Length > i)
                                {
                                    if (!string.IsNullOrEmpty(datavalues[i]))
                                    {
                                        lColumnList.Add(ConstructCell(Convert.ToString(datavalues[i].Replace("&amp;", "&").Replace("&quot;", "\"")), CellValues.String, 1));
                                    }
                                    else
                                    {
                                        lColumnList.Add(ConstructCell(Convert.ToString(""), CellValues.String, 1));
                                    }
                                }
                                else
                                {
                                    lColumnList.Add(ConstructCell(Convert.ToString(""), CellValues.String, 1));
                                }

                            }


                            keyrow.Append(lColumnList);
                            newrowindex = System.Convert.ToUInt32(keyrow.RowIndex.Value + 1);
                            sheetData.AppendChild(keyrow);

                            worksheetPart.Worksheet.Save();
                        }




                    }
        }
      }
      //var template = new FileInfo(excelFile);

      return excelFile;
    }
    
    public static string IncrementColumnCellReference(string cell)
    {
      string reg = @"^([A-Za-z]+)(\d+)$";
      Match m = Regex.Match(cell, reg);
      string colLetters = m.Groups[1].Value.ToUpper();
      int len = colLetters.Length;
      char lastLetter = colLetters[len - 1];

      if (lastLetter < 'Z')
      {
        colLetters = colLetters.Substring(0, len - 1) + (++lastLetter);
      }
      else if (Regex.IsMatch(colLetters, "^Z+$"))
      {
        colLetters = new string('A', len + 1);
      }
      else
      {
        int base26 = 0;
        int multiplier = 1;

        for (int i = len - 1; i >= 0; --i)
        {
          base26 += multiplier * (colLetters[i] - 65);
          multiplier *= 26;
        }

        base26++;
        string temp = "";

        while (base26 > 0)
        {
          temp = (char)(base26 % 26 + 65) + temp;
          base26 /= 26;
        }

        colLetters = temp;
      }

      return colLetters + m.Groups[2].Value;

    }
   
  }
}

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public static class FeedProcessHelper
    {
        public  static int FeedProcess(int lFeedProcessId, string lOperation, string lCreatedBy, string lStatus, string lstrConn, string lSchema)

        {

            int lResult = 0;
            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
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

            lcmd.CommandText = lSchema+ "." + "USP_FEEDPROCESS";

            try
            {
                lcmd.ExecuteNonQuery();
                lResult = int.Parse(lcmd.Parameters[4].Value.ToString());
            }
            catch (Exception lex)
            {

                ltransaction.Rollback();

                throw new Exception(lex.Message);
            }

            ltransaction.Commit();


            lconnection.Close();
            return lResult;

        }

        public static int FeedProcessDetails(int lFEEDPROCESSDETAILID, int lFeedProcessId, string lOperation, string lFileName, string lCREATEDBY, string lStatus, string lFileType, string lstrConn, string lSchema)
        {
            int lResult = 0;
            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = lSchema + "." + "USP_FEEDPROCESSDETAILS";
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
                throw lex;
            }

            ltransaction.Commit();


            lconnection.Close();
            return lResult;

        }

        public static bool MappingValidation(int lFeedProcessId, string lstrConn, string lSchema)
        {
            bool lResult = false;
            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = lSchema + "." + "USP_MAPPING_VALIDATION";
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
                throw lex;
            }

            ltransaction.Commit();
            lconnection.Close();
            return lResult;

        }

        public static DataTable DbExcel(int lFeedProcessId, string lstrConn, string lSchema)
        {
            DataTable dt = new DataTable();
            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
            lconnection.Open();
            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            OracleParameter[] ladd_refer_image = new OracleParameter[2];
            ladd_refer_image[0] = new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32);
            ladd_refer_image[0].Value = lFeedProcessId;
            ladd_refer_image[1] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
            ladd_refer_image[1].Direction = ParameterDirection.Output;

            foreach (OracleParameter p in ladd_refer_image)
            {
                lcmd.Parameters.Add(p);
            }

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = lSchema + "." + "SP_EXPORT_LOGREPORT";
            lcmd.CommandType = CommandType.StoredProcedure;
            OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
            dataAdapter.Fill(dt);

            lconnection.Close();

            return dt;
        }

        public static bool DatawareHouseMapping(int lFeedProcessId, int lIsOverWrite, string lstrConn, string lSchema)
        {
            bool lResult = false;
            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
            lconnection.Open();

            OracleTransaction ltransaction;
            ltransaction = lconnection.BeginTransaction();

            OracleCommand lcmd;
            lcmd = lconnection.CreateCommand();
            lcmd.Transaction = ltransaction;

            //The name of the Procedure responsible for inserting the data in the table.
            lcmd.CommandText = lSchema + "." + "USP_FEEDPROCESSMAPPING_Mode_D";
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
                
                ImportHelper.WriteMessage("Error :" + lex.ToString(), ImportHelper.logFilePath, "TestSuite", ImportHelper.Import_TestSuiteName);
                string msg = lex.Message;
                if (!msg.Contains("PL/SQL: numeric or value error") && !msg.Contains("at line 1"))
                {
                    throw lex;
                }
                else
                {
                    ltransaction.Commit();
                }
            }

            lconnection.Close();
            return lResult;

        }

        public static void ExportLogReport(int lFeedProcessId, string lLocation, string lstrConn, string lSchema)
        {
            DataSet lds = new DataSet();
            OracleConnection lconnection = ExportHelper.GetOracleConnection(lstrConn);
            try
            {

                lconnection.Open();

                OracleTransaction ltransaction;
                ltransaction = lconnection.BeginTransaction();

                OracleCommand lcmd;
                lcmd = lconnection.CreateCommand();
                lcmd.Transaction = ltransaction;

                OracleParameter[] ladd_refer_image = new OracleParameter[3];
                ladd_refer_image[0] = new OracleParameter("FEEDPROCESSDETAILID", OracleDbType.Int32);
                ladd_refer_image[0].Value = lFeedProcessId;
                //ladd_refer_image[1] = new OracleParameter("FILENAME", OracleDbType.NChar);
                //ladd_refer_image[1].Value = lFileName.Replace("LOGREPORT-", "");
                ladd_refer_image[2] = new OracleParameter("sl_cursor", OracleDbType.RefCursor);
                ladd_refer_image[2].Direction = ParameterDirection.Output;

                foreach (OracleParameter p in ladd_refer_image)
                {
                    lcmd.Parameters.Add(p);
                }


                //The name of the Procedure responsible for inserting the data in the table.
                lcmd.CommandText = lSchema + "." + "SP_EXPORT_LOGREPORT";
                lcmd.CommandType = CommandType.StoredProcedure;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(lcmd);
                dataAdapter.Fill(lds);



                lconnection.Close();
                //lobjcommon.ConvertDataSetToExcel1(lds, lLocation + "\\" + lFileName);
            }
            catch (Exception ex)
            {
                if (lds != null && lds.Tables.Count > 0 && lds.Tables[0].Rows.Count > 0)
                {
                    lconnection.Close();
                    //  lobjcommon.ConvertDataSetToExcel1(lds, lLocation + "\\" + lFileName);

                    return;
                }
               
            }
            //Create the data set and table

        }

        //public static string Excel(DataTable dv, string fileName, string LogPath)

        //{
        //    XL.Application lexcelApp1 = new XL.Application();

        //    XL.Workbook lexcelWorkBook1 = null;
        //    XL._Worksheet excelWS = null;
        //    lexcelWorkBook1 = lexcelApp1.Workbooks.Add();
        //    object lmisValue = System.Reflection.Missing.Value;
        //    string strPath = "";
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
        //        string logpath = LogPath;
        //        string fname =fileName ;
        //        strPath = System.IO.Path.Combine(logpath, fname);
        //        lexcelWorkBook1.SaveAs(strPath, XL.XlFileFormat.xlOpenXMLWorkbook, lmisValue, lmisValue, lmisValue, lmisValue, XL.XlSaveAsAccessMode.xlExclusive, lmisValue, lmisValue, lmisValue, lmisValue, lmisValue);

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        lexcelWorkBook1.Close(0);
        //        lexcelApp1.Quit();                               
        //    }
        //    return strPath;
        //}
        public static Columns LogReportColumnStyle()
        {
            Columns columns = new Columns(
                     new Column
                     {
                         Min = 1,
                         Max = 1,
                         Width = 8,
                         CustomWidth = true,
                         BestFit = true
                     },
                      new Column
                      {
                          Min = 2,
                          Max = 4,
                          Width = 30,
                          CustomWidth = true,
                          BestFit = true
                      },
                       new Column
                       {
                           Min = 5,
                           Max = 5,
                           Width = 9,
                           CustomWidth = true,
                           BestFit = true
                       },
                        new Column
                        {
                            Min = 6,
                            Max = 6,
                            Width = 30,
                            CustomWidth = true,
                            BestFit = true
                        },
                        new Column
                        {
                            Min = 7,
                            Max = 7,
                            Width = 60,
                            CustomWidth = true,
                            BestFit = true
                        },
                        new Column
                        {
                            Min = 8,
                            Max = 12,
                            Width = 30,
                            CustomWidth = true,
                            BestFit = true
                        },
                        new Column
                        {
                            Min = 13,
                            Max = 13,
                            Width = 15,
                            CustomWidth = true,
                            BestFit = true
                        },
                        new Column
                        {
                            Min = 14,
                            Max = 19,
                            Width = 30,
                            CustomWidth = true,
                            BestFit = true
                        },
                        new Column
                        {
                            Min = 20,
                            Max = 21,
                            Width = 35,
                            CustomWidth = true,
                            BestFit = true
                        },
                        new Column
                        {
                            Min = 22,
                            Max = 22,
                            Width = 65,
                            CustomWidth = true,
                            BestFit = true
                        }
                     );
            return columns;
        }
        public static string Excel(DataTable dv, string filename,string LogPath)
        {

            filename = filename + ".xlsx";
            string fpath = Path.Combine(LogPath, filename);
           
            
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(fpath, SpreadsheetDocumentType.Workbook))
            {
               // ImportHelper.WriteMessage("excel create" + fpath, ImportHelper.logFilePath, "TestSuite", ImportHelper.Import_TestSuiteName);
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                ExportHelper ex = new ExportHelper();
                stylePart.Stylesheet = ex.GenerateStylesheet();
                stylePart.Stylesheet.Save();

                worksheetPart.Worksheet.AppendChild((LogReportColumnStyle()));

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Log" };

                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                Row row = new Row();

                row.Append(
                    ex.ConstructCell("Sequence Counter", CellValues.String, 2),
                    ex.ConstructCell("TimeStamp", CellValues.String, 2),
                    ex.ConstructCell("Message Type", CellValues.String, 2),
                    ex.ConstructCell("Action", CellValues.String, 2),
                    ex.ConstructCell("SpreadSheet cell Address", CellValues.String, 2),
                    ex.ConstructCell("Validation Name", CellValues.String, 2),
                    ex.ConstructCell("Validation Fail Description", CellValues.String, 2),
                    ex.ConstructCell("Application Name", CellValues.String, 2),
                    ex.ConstructCell("Project Name", CellValues.String, 2),
                    ex.ConstructCell("Storyboard Name", CellValues.String, 2),
                    ex.ConstructCell("Test Suite Name", CellValues.String, 2),
                    ex.ConstructCell("TestCase Name", CellValues.String, 2),
                    ex.ConstructCell("Test Step Name", CellValues.String, 2),
                    ex.ConstructCell("Data Set Name", CellValues.String, 2),
                    ex.ConstructCell("Dependancy", CellValues.String, 2),
                    ex.ConstructCell("Run Order", CellValues.String, 2),
                    ex.ConstructCell("Object Name", CellValues.String, 2),
                    ex.ConstructCell("Comment", CellValues.String, 2),
                    ex.ConstructCell("Error Description", CellValues.String, 2),
                    ex.ConstructCell("Program Location", CellValues.String, 2),
                    ex.ConstructCell("Tab Name", CellValues.String, 2),
                    ex.ConstructCell("General", CellValues.String, 2));
                sheetData.AppendChild(row);
                Row drow = new Row();
                if (dv.Rows.Count > 0)
                {
                    for (int i = 1; i <= dv.Rows.Count; i++)
                    {
                        drow = new Row();
                        for (int j = 1; j <= dv.Columns.Count; j++)
                        {
                            drow.Append(ex.ConstructCell(dv.Rows[i - 1][j - 1] == null ? "" : dv.Rows[i - 1][j - 1].ToString(), CellValues.String, 1));

                        }
                        sheetData.AppendChild(drow);
                    }
                }
                worksheetPart.Worksheet.Save();
                
            }
            return fpath;
        }
    }
}

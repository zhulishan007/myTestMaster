using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MARSUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARSUtility
{
    public class CommonHelper
    {
        public void excel(DataTable dv, string LogPath, string mode, string name, string filetype)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->excel";
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(LogPath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    ExportExcel ex = new ExportExcel();
                    stylePart.Stylesheet = ex.GenerateStylesheet();
                    stylePart.Stylesheet.Save();
                    ExportExcel excel = new ExportExcel();
                    worksheetPart.Worksheet.AppendChild((excel.LogReportColumnStyle()));

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
            }
            catch(Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Log", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportLog " + ex.Message);

            }
        }
        public string ExtractString(string s, string tag)
        {
            // You should check for errors in real-world code, omitted for brevity
            var startTag = "" + tag + "";
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            int endIndex = s.IndexOf("" + tag + "", startIndex);
            return s.Substring(startIndex, endIndex - startIndex);
        }
    }
    public static class dbtable
    {
        public static DataTable dt_Log = new DataTable();



        public static void errorlog(string ex, string action, string location, int line)
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
            dr["Project Name"] = "";
            dr["StoryBoard Name"] = "";
            dr["Test Suite Name"] = "";


            dr["TestCase Name"] = "";
            dr["Test step Number"] = "";

            dr["Dataset Name"] = "";
            dr["Dependancy"] = "";
            dr["Run Order"] = "";



            dr["Object Name"] = "";
            dr["Comment"] = "";
            dr["Error Description"] = line;
            dr["Program Location"] = location;
            dr["Tab Name"] = "";
            dbtable.dt_Log.Rows.Add(dr);


        }
        public static int lineNo(Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            return line;
        }
       
    }
}

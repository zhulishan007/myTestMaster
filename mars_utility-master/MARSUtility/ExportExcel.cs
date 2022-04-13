using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MARSUtility.ViewModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MARSUtility
{
    public class ExportExcel
    {
        #region Excel Styles
        //Styles the Excel sheet
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
                ),
                   new Font( // Index 3 
                    new FontSize() { Val = 11D },
                    new Color() { Rgb = "ffffff" },
                    new FontName() { Val = "Calibri" }
                )
                );


            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.LightUp }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "c3ebe2" } })
                    { PatternType = PatternValues.Solid }), // Index 2 - header
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "f5cf6e" } })
                    { PatternType = PatternValues.Solid }),// Index 3 
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "999999" } })
                    { PatternType = PatternValues.Solid }) // Index 4
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
                    new CellFormat(new Alignment() { WrapText = true }) { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true }, // header
                    new CellFormat { FontId = 2, FillId = 3, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 3, FillId = 4, BorderId = 1, ApplyFill = true }
                // extra
                );
            cellFormats.Append(new CellFormat(new Alignment() { WrapText = true }));


            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }

        //sets the column's width of variable
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
        //sets the column's width of Config
        public Columns ConfigColumnStyle()
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
                          Max = 3,
                          Width = 140,
                          CustomWidth = true
                      });

            return columns;
        }
        //sets the column's width of Object
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
        //sets the column's width of Storyboard
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
        //sets the column's width of TestCase
        public Columns TestCaseColumnsStyle()
        {
            Columns columns = new Columns(
                     new Column
                     {
                         Min = 1,
                         Max = 1,
                         Width = 20,
                         CustomWidth = true,
                         BestFit = true
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

        public Columns ResultsetColumnsStyle(int length)
        {
            Columns columns = new Columns();
            for (int i = 1; i < length; i++)
            {
                Column column = new Column
                {
                    Min = Convert.ToUInt32(i),
                    Max = Convert.ToUInt32(i),
                    Width = 30,
                    CustomWidth = true,
                    BestFit = true
                };
                columns.Append(column);
            }
            return columns;
        }

        public Columns DatasetTagColumnsStyle()
        {
            Columns columns = new Columns(
                      new Column
                      {
                          Min = 1,
                          Max = 3,
                          Width = 27,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 4,
                          Max = 5,
                          Width = 20,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 6,
                          Max = 6,
                          Width = 40,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 7,
                          Max = 8,
                          Width = 12,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 9,
                          Max = 9,
                          Width = 10,
                          CustomWidth = true
                      });

            return columns;
        }

        public Columns DatasetTagReportColumnsStyle()
        {
            Columns columns = new Columns(
                      new Column
                      {
                          Min = 1,
                          Max = 2,
                          Width = 30,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 3,
                          Max = 3,
                          Width = 10,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 4,
                          Max = 6,
                          Width = 32,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 7,
                          Max = 9,
                          Width = 25,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 10,
                          Max = 10,
                          Width = 10,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 11,
                          Max = 11,
                          Width = 20,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 12,
                          Max = 13,
                          Width = 18,
                          CustomWidth = true
                      });

            return columns;
        }

        public Columns DatasetTagCommanColumnsStyle()
        {
            Columns columns = new Columns(
                      new Column
                      {
                          Min = 1,
                          Max = 2,
                          Width = 20,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 3,
                          Max = 3,
                          Width = 15,
                          CustomWidth = true
                      });

            return columns;
        }

        public Columns AllStoryboardColumnsStyle()
        {
            Columns columns = new Columns(
                      new Column
                      {
                          Min = 1,
                          Max = 2,
                          Width = 22,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 3,
                          Max = 3,
                          Width = 12,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 4,
                          Max = 6,
                          Width = 25,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 7,
                          Max = 7,
                          Width = 15,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 8,
                          Max = 10,
                          Width = 35,
                          CustomWidth = true
                      }, new Column
                      {
                          Min = 11,
                          Max = 11,
                          Width = 15,
                          CustomWidth = true
                      },
                      new Column
                      {
                          Min = 12,
                          Max = 14,
                          Width = 35,
                          CustomWidth = true
                      });

            return columns;
        }
        #endregion
        public Columns LogReportColumnStyle()
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
        //Increments Cell Reference and returns the incremented cell reference
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

        //Constructs excel's cell with specific cell Reference
        public Cell ConstructCellWithRef(string value, CellValues dataType, uint styleIndex = 0, string cellref = "")
        {

            return new Cell()
            {
                CellReference = cellref,
                CellValue = new CellValue(value),
                DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        //Constructs excel's cell 
        public Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {

            return new Cell()
            {

                CellValue = new CellValue(value),
                DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex,

            };
        }

        // ConstructCellWithRef
        public Cell ConstructCellWithRef(string value, string reff)
        {
            return new Cell()
            {
                CellReference = reff,
                DataType = CellValues.String,
                CellValue = new CellValue(value),
                StyleIndex = 1
            };
        }

        //Converts Variable Model list into Excel using open xml
        public bool ExportVariableExcel(string lExportPath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportVariableExcel";
            try
            {

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(lExportPath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();

                    worksheetPart.Worksheet.AppendChild(VariableColumnsStyle());

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Table" };

                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();
                    ExportHelper helper = new ExportHelper();
                    var list = helper.ExportVariable(schema, constring);

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row();

                    row.Append(
                        ConstructCell("Name", CellValues.String, 2),
                        ConstructCell("Value", CellValues.String, 2),
                        ConstructCell("Type", CellValues.String, 2),
                        ConstructCell("Base/Comp", CellValues.String, 2));

                    sheetData.AppendChild(row);

                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            row = new Row();

                            row.Append(
                                ConstructCell(item.Name, CellValues.String, 1),
                                ConstructCell(item.Value, CellValues.String, 1),
                                ConstructCell(item.Type, CellValues.String, 1),
                                ConstructCell(item.BaseComp, CellValues.String, 1));

                            sheetData.AppendChild(row);
                        }

                    }
                    worksheetPart.Worksheet.Save();
                    dbtable.errorlog("Excel sheet created.", "Export Variable", "", 0);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Variable Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :Exportvariable " + ex.Message);
            }

            return flag;
        }

        //Converts Object Model list into Excel using open xml
        public bool ExportObjectExcel(string application, string fullpath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportObjectExcel";

            try
            {
                ExportHelper helper = new ExportHelper();
                bool validapp = false;
                validapp = helper.ValidateTestSuiteAndApplication(application.Trim(), "APPLICATION", schema, constring);
                if (validapp)
                {
                    var list = helper.ExportObject(application.Trim(), schema, constring);
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();


                        WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                        stylePart.Stylesheet = GenerateStylesheet();
                        stylePart.Stylesheet.Save();

                        worksheetPart.Worksheet.AppendChild(ObjectColumnsStyle());

                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Table" };

                        sheets.Append(sheet);

                        workbookPart.Workbook.Save();


                        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        Row frow = new Row();
                        frow.Append(
                         ConstructCell(application, CellValues.String, 1));

                        sheetData.AppendChild(frow);

                        Row row = new Row();

                        row.Append(
                            ConstructCell("OBJECT NAME", CellValues.String, 2),
                            ConstructCell("TYPE", CellValues.String, 2),
                            ConstructCell("QUICK_ACCESS", CellValues.String, 2),
                            ConstructCell("PARENT", CellValues.String, 2),
                            ConstructCell("COMMENT", CellValues.String, 2),
                            ConstructCell("ENUM_TYPE", CellValues.String, 2),
                            ConstructCell("SQL", CellValues.String, 2));

                        sheetData.AppendChild(row);

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                row = new Row();

                                row.Append(
                                    ConstructCell(item.OBJECTNAME, CellValues.String, 1),
                                    ConstructCell(item.TYPE, CellValues.String, 1),
                                    ConstructCell(item.QUICK_ACCESS, CellValues.String, 1),
                                    ConstructCell(item.PARENT, CellValues.String, 1),
                                    ConstructCell(item.COMMENT, CellValues.String, 1),
                                    ConstructCell(item.ENUM_TYPE, CellValues.String, 1),
                                    ConstructCell(item.SQL, CellValues.String, 1));

                                sheetData.AppendChild(row);
                            }
                        }
                        worksheetPart.Worksheet.Save();
                        dbtable.errorlog("Excel sheet created", "Export Object Excel", "", 0);
                        flag = true;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n Application [" + application + "] does not exist in the system");
                    dbtable.errorlog("Application [" + application + "] does not exist in the system", "", "", 0);
                    //return null;

                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Object Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportObjectExcel " + ex.Message);
            }

            return flag;
        }

        //Converts Storyboard Model list into Excel using open xml
        public bool ExportStoryboardExcel(string project, string fullpath, string schema, string constring)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportStoryboardExcel";
            bool flag = false;
            string tabname = string.Empty;
            List<string> tabnamelist = new List<string>();
            int tabid = 1;
            ExportHelper helper = new ExportHelper();
            try
            {
                var list = helper.ExportStoryboard(project, schema, constring);
                if (list != null)
                {
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                    {

                        list = list.Where(x => x.STORYBOARD_NAME != null).ToList();
                        if (list.Any())
                        {
                            var storyboradlst = list.Where(x => x.STORYBOARD_NAME != null).OrderBy(x => x.STORYBOARD_NAME).Select(x => x.STORYBOARD_NAME).Distinct().ToList();
                            storyboradlst = storyboradlst.Where(x => x != "").ToList();
                            WorkbookPart workbookPart = document.AddWorkbookPart();
                            workbookPart.Workbook = new Workbook();

                            WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                            stylePart.Stylesheet = GenerateStylesheet();
                            stylePart.Stylesheet.Save();

                            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                            for (int i = 0; i < storyboradlst.Count(); i++)
                            {
                                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                                worksheetPart.Worksheet = new Worksheet();

                                worksheetPart.Worksheet.AppendChild(StoryBordColumnsStyle());

                                Regex re = new Regex("[;\\/:*?\"<>|&']");
                                tabname = re.Replace(storyboradlst[i].ToString(), "-").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");
                                tabname = tabname.Length > 30 ? tabname.Substring(0, 29) : tabname;
                                if (tabnamelist.Count(x => x.ToUpper().Equals(tabname.ToUpper())) == 1)
                                {
                                    tabname = tabname.Trim() + "_" + tabid;
                                    tabid++;
                                }

                                tabnamelist.Add(tabname);
                                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = Convert.ToUInt32(i + 1), Name = tabname };

                                var storyboradTabList = list.Where(x => x.STORYBOARD_NAME.Trim() == storyboradlst[i].Trim()).ToList();

                                sheets.Append(sheet);

                                workbookPart.Workbook.Save();

                                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                                Row row1 = new Row();
                                row1.Append(
                                    ConstructCell("Application", CellValues.String, 1));
                                var apparray = storyboradTabList[0].APPLICATIONNAME.Split(',');
                                foreach (var item in apparray.ToList())
                                {
                                    row1.Append(ConstructCell(item == null ? "" : item, CellValues.String, 1));
                                }
                                //exportExcel.ConstructCell(storyboradTabList[0].APPLICATIONNAME == null ? "" : storyboradTabList[0].APPLICATIONNAME, CellValues.String, 1));
                                sheetData.AppendChild(row1);

                                Row row2 = new Row();
                                row2.Append(
                                    ConstructCell("Project Name", CellValues.String, 1),
                                    ConstructCell(storyboradTabList[0].PROJECTNAME == null ? "" : storyboradTabList[0].PROJECTNAME, CellValues.String, 1));
                                sheetData.AppendChild(row2);

                                Row row3 = new Row();
                                row3.Append(
                                    ConstructCell("Project Description", CellValues.String, 1),
                                    ConstructCell(storyboradTabList[0].PROJECTDESCRIPTION == null ? "" : storyboradTabList[0].PROJECTDESCRIPTION, CellValues.String, 1));
                                sheetData.AppendChild(row3);

                                Row row4 = new Row();
                                row4.Append(
                                    ConstructCell("Storyboard Name", CellValues.String, 1),
                                    ConstructCell(storyboradTabList[0].STORYBOARD_NAME == null ? "" : storyboradTabList[0].STORYBOARD_NAME, CellValues.String, 1));
                                sheetData.AppendChild(row4);

                                Row row5 = new Row();
                                row5.Append(
                                    ConstructCell("Row Number", CellValues.String, 2),
                                    ConstructCell("Action", CellValues.String, 2),
                                    ConstructCell("Step Name", CellValues.String, 2),
                                    ConstructCell("Test Suit Name", CellValues.String, 2),
                                    ConstructCell("Test Case Name", CellValues.String, 2),
                                    ConstructCell("Data Set Name", CellValues.String, 2),
                                    ConstructCell("Dependency", CellValues.String, 2));
                                sheetData.AppendChild(row5);

                                Row row = new Row();
                                //for(i = 0; i < storyboradTabList.Length; i++)
                                //{
                                //    row = new Row();
                                //    row.Append(ConstructCell(storyboradTabList[i].RUNORDER == null ? "" : storyboradTabList[i].RUNORDER, CellValues.String, 1),
                                //         ConstructCell(storyboradTabList[i].ACTIONNAME == null ? "" : storyboradTabList[i].ACTIONNAME, CellValues.String, 1),
                                //         ConstructCell(storyboradTabList[i].STEPNAME == null ? "" : storyboradTabList[i].STEPNAME, CellValues.String, 1),
                                //         ConstructCell(storyboradTabList[i].SUITENAME == null ? "" : storyboradTabList[i].SUITENAME, CellValues.String, 1),
                                //         ConstructCell(storyboradTabList[i].CASENAME == null ? "" : storyboradTabList[i].CASENAME, CellValues.String, 1),
                                //         ConstructCell(storyboradTabList[i].DATASETNAME == null ? "" : storyboradTabList[i].DATASETNAME, CellValues.String, 1),
                                //         ConstructCell(storyboradTabList[i].DEPENDENCY == null ? "" : storyboradTabList[i].DEPENDENCY, CellValues.String, 1));
                                //    sheetData.AppendChild(row);
                                //}
                                foreach (var item in storyboradTabList)
                                {
                                    row = new Row();
                                    row.Append(
                                         ConstructCell(item.RUNORDER == null ? "" : item.RUNORDER, CellValues.String, 1),
                                         ConstructCell(item.ACTIONNAME == null ? "" : item.ACTIONNAME, CellValues.String, 1),
                                         ConstructCell(item.STEPNAME == null ? "" : item.STEPNAME, CellValues.String, 1),
                                         ConstructCell(item.SUITENAME == null ? "" : item.SUITENAME, CellValues.String, 1),
                                         ConstructCell(item.CASENAME == null ? "" : item.CASENAME, CellValues.String, 1),
                                         ConstructCell(item.DATASETNAME == null ? "" : item.DATASETNAME, CellValues.String, 1),
                                         ConstructCell(item.DEPENDENCY == null ? "" : item.DEPENDENCY, CellValues.String, 1));

                                    sheetData.AppendChild(row);
                                }
                                worksheetPart.Worksheet.Save();
                            }
                            flag = true;
                        }
                        dbtable.errorlog("Excel is created", "Export Storyboard Excel", "", 0);
                    }

                }
            }
            catch (Exception ex)
            {
                int line;
                flag = false;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Storyboard Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportStoryboardExcel " + ex.Message);
            }
            return flag;
        }

        public bool ExportStoryboardByProject(string project, string storyboard, string fullpath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportStoryboardByProject";
            ExportHelper helper = new ExportHelper();
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                {
                    var list = helper.ExportStoryboardList(storyboard, project, constring, schema);
                    string tabname = string.Empty;
                    if (list.Any())
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();

                        WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                        stylePart.Stylesheet = GenerateStylesheet();
                        stylePart.Stylesheet.Save();

                        worksheetPart.Worksheet.AppendChild(StoryBordColumnsStyle());

                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                        Regex re = new Regex("[;\\/:*?\"<>|&']");
                        tabname = re.Replace(list[0].STORYBOARD_NAME, "-").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");
                        tabname = tabname.Length > 30 ? tabname.Substring(0, 29) : tabname;
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = tabname };

                        sheets.Append(sheet);

                        workbookPart.Workbook.Save();

                        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        Row row1 = new Row();
                        row1.Append(
                           ConstructCell("Application", CellValues.String, 1));
                        var apparray = list[0].APPLICATIONNAME.Split(',');
                        foreach (var item in apparray.ToList())
                        {
                            row1.Append(ConstructCell(item, CellValues.String, 1));
                        }

                        sheetData.AppendChild(row1);

                        Row row2 = new Row();
                        row2.Append(
                            ConstructCell("Project Name", CellValues.String, 1),
                            ConstructCell(list[0].PROJECTNAME, CellValues.String, 1));
                        sheetData.AppendChild(row2);

                        Row row3 = new Row();
                        row3.Append(
                            ConstructCell("Project Description", CellValues.String, 1),
                            ConstructCell(list[0].PROJECTDESCRIPTION, CellValues.String, 1));
                        sheetData.AppendChild(row3);

                        Row row4 = new Row();
                        row4.Append(
                            ConstructCell("Storyboard Name", CellValues.String, 1),
                            ConstructCell(list[0].STORYBOARD_NAME, CellValues.String, 1));
                        sheetData.AppendChild(row4);

                        Row row5 = new Row();
                        row5.Append(
                            ConstructCell("Row Number", CellValues.String, 2),
                            ConstructCell("Action", CellValues.String, 2),
                            ConstructCell("Step Name", CellValues.String, 2),
                            ConstructCell("Test Suit Name", CellValues.String, 2),
                            ConstructCell("Test Case Name", CellValues.String, 2),
                            ConstructCell("Data Set Name", CellValues.String, 2),
                            ConstructCell("Dependency", CellValues.String, 2));
                        sheetData.AppendChild(row5);

                        Row row = new Row();
                        foreach (var item in list)
                        {
                            row = new Row();
                            row.Append(
                                 ConstructCell(item.RUNORDER == null ? "" : item.RUNORDER, CellValues.String, 1),
                                 ConstructCell(item.ACTIONNAME == null ? "" : item.ACTIONNAME, CellValues.String, 1),
                                 ConstructCell(item.STEPNAME == null ? "" : item.STEPNAME, CellValues.String, 1),
                                 ConstructCell(item.SUITENAME == null ? "" : item.SUITENAME, CellValues.String, 1),
                                 ConstructCell(item.CASENAME == null ? "" : item.CASENAME, CellValues.String, 1),
                                 ConstructCell(item.DATASETNAME == null ? "" : item.DATASETNAME, CellValues.String, 1),
                                 ConstructCell(item.DEPENDENCY == null ? "" : item.DEPENDENCY, CellValues.String, 1));

                            sheetData.AppendChild(row);
                        }
                        flag = true;
                        worksheetPart.Worksheet.Save();
                    }
                }

            }

            catch (Exception ex)
            {
                int line;
                flag = false;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Storyboard Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportStoryboard " + ex.Message);

                // throw new Exception("Error from :ExportStoryboardByProject " + ex.Message);
            }
            return flag;
        }

        //Converts TestSuite's whole Dataset into Excel using open xml
        public string ExportTestSuiteExcel(DataSet ds, string excelFile)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportTestSuiteExcel";
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
            try
            {
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
                            if (tabnamelist.Count(x => x.ToUpper().Equals(tabname.ToUpper())) == 1)
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
                            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = Convert.ToUInt32(m + 1), Name = tabname };

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
                                            datasetnames[l] = datasetnames[l].Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",");
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
                                //keyrow = new Row() { RowIndex = (UInt32)newrowindex };
                                skipvalues = Convert.ToString(ldt.Rows[k]["SKIP"]).Split(',');
                                datavalues = Convert.ToString(ldt.Rows[k]["DATASETVALUE"]).Split(',');

                                //var lColumnList = new List<OpenXmlElement>();
                                //lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["KEY_WORD_NAME"]), CellValues.String, 1));
                                //lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["OBJECT_HAPPY_NAME"]), CellValues.String, 1));
                                //lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["PARAMETER"]), CellValues.String, 1));
                                //lColumnList.Add(ConstructCell(Convert.ToString(ldt.Rows[k]["COMMENT"]), CellValues.String, 1));
                                var datasetlength = datasetnames.Length;//datatset length 
                                //for (int i = 0; i < datasetlength; i++)
                                //{
                                //    if (!string.IsNullOrEmpty(skipvalues[i]))
                                //    {
                                //        lColumnList.Add(ConstructCell(Convert.ToString(skipvalues[i] == "0" ? "" : "SKIP"), CellValues.String, 1));
                                //    }
                                //    else
                                //    {
                                //        lColumnList.Add(ConstructCell(Convert.ToString(""), CellValues.String, 1));
                                //    }
                                //    if (datavalues.Length > i)
                                //    {
                                //        if (!string.IsNullOrEmpty(datavalues[i]))
                                //        {
                                //            lColumnList.Add(ConstructCell(Convert.ToString(datavalues[i].Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",")), CellValues.String, 1));
                                //            //lColumnList.Add(ConstructCell(Convert.ToString(datavalues[i].Replace("&amp;", "&").Replace("&quot;", "\"")), CellValues.String, 1));
                                //        }
                                //        else
                                //        {
                                //            lColumnList.Add(ConstructCell(Convert.ToString(""), CellValues.String, 1));
                                //        }
                                //    }
                                //    else
                                //    {
                                //        lColumnList.Add(ConstructCell(Convert.ToString(""), CellValues.String, 1));
                                //    }

                                //}


                                //keyrow.Append(lColumnList);
                                //newrowindex = System.Convert.ToUInt32(keyrow.RowIndex.Value + 1);
                                //sheetData.AppendChild(keyrow);

                                if (k > 0)
                                    newrowindex++;

                                Row rt = new Row();//A9

                                rt.Append(
                                ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["KEY_WORD_NAME"]), "A" + Convert.ToString(newrowindex)),
                                ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["OBJECT_HAPPY_NAME"]), "B" + Convert.ToString(newrowindex)),
                                ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["PARAMETER"]), "C" + Convert.ToString(newrowindex)),
                                ConstructCellWithRef(Convert.ToString(ldt.Rows[k]["COMMENT"]), "D" + Convert.ToString(newrowindex))
                                );
                                var rtcellref = "E" + Convert.ToString(newrowindex);
                                for (int i = 0; i < datasetlength; i++)
                                {
                                    // skip 
                                    if (i > 0)
                                    {
                                        rtcellref = IncrementColumnCellReference(rtcellref);
                                    }

                                    if (!string.IsNullOrEmpty(skipvalues[i]))
                                    {
                                        rt.Append(ConstructCellWithRef(Convert.ToString(skipvalues[i] == "0" ? "" : "SKIP"), rtcellref));
                                    }
                                    else
                                    {
                                        rt.Append(ConstructCellWithRef(Convert.ToString(""), rtcellref));
                                    }

                                    // datavalue
                                    rtcellref = IncrementColumnCellReference(rtcellref);
                                    if (datavalues.Length > i)
                                    {
                                        if (!string.IsNullOrEmpty(datavalues[i]))
                                        {
                                            rt.Append(ConstructCellWithRef(Convert.ToString(datavalues[i].Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("~", ",")), rtcellref));
                                        }
                                        else
                                        {
                                            rt.Append(ConstructCellWithRef(Convert.ToString(""), rtcellref));
                                        }
                                    }
                                    else
                                    {
                                        rt.Append(ConstructCellWithRef(Convert.ToString(""), rtcellref));
                                    }
                                }

                                sheetData.AppendChild(rt);

                                //Row rowt = new Row();
                                //Cell cellt = new Cell()
                                //{
                                //    CellReference = "A16",
                                //    DataType = CellValues.String,
                                //    CellValue = new CellValue("Microsofttest")
                                //};
                                //rowt.Append(cellt);
                                //sheetData.Append(rowt);

                                worksheetPart.Worksheet.Save();
                            }
                        }
                    }

                    dbtable.errorlog("Excel is created", "", "", 0);
                }

                return excelFile;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export TestSuite Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportTestSuiteExcel " + ex.Message);
            }
        }

        //Converts Compare config Model list into Excel using open xml
        public bool ExportConfigExcel(string excel, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportConfigExcel";
            try
            {
                ExportHelper exp = new ExportHelper();
                var list = exp.ExportCompareConfig(schema, constring);
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(excel, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();

                    stylePart.Stylesheet.Save();

                    worksheetPart.Worksheet.AppendChild(ConfigColumnStyle());

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Compareconfig" };

                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();
                    ExportHelper helper = new ExportHelper();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row();

                    row.Append(
                        ConstructCell("Data Source Name", CellValues.String, 2),
                        ConstructCell("Data Source Type", CellValues.String, 2),
                        ConstructCell("Details", CellValues.String, 2));

                    sheetData.AppendChild(row);
                    Row datarow = new Row();
                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            datarow = new Row();

                            datarow.Append(
                                ConstructCell(item.datasourcename == null ? "" : item.datasourcename, CellValues.String, 1),
                                ConstructCell(item.sourcetype == null ? "" : item.sourcetype, CellValues.String, 1),
                                ConstructCell(item.details == null ? "" : item.details, CellValues.String, 1));

                            sheetData.AppendChild(datarow);
                        }
                    }

                    worksheetPart.Worksheet.Save();
                    dbtable.errorlog("Excel is created", "", "", 0);
                    flag = true;
                }

                return flag;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Config Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportConfigExcel " + ex.Message);
            }
        }

        public string ExportResultSetExcel(DataSet ds, string excelFile)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportResultSetExcel";

            string lTSName = string.Empty, lTCName = string.Empty, lTCDesc = string.Empty;

            DataTable ldtRESULTSET = new DataTable();
            ldtRESULTSET = ds.Tables[0];

            DataView dtvRESULTSET = new DataView();
            dtvRESULTSET = ldtRESULTSET.DefaultView;

            DataTable dtRESULTSET = new DataTable();
            dtRESULTSET = dtvRESULTSET.Table;

            ldtRESULTSET = dtRESULTSET;

            try
            {
                if (ldtRESULTSET.Rows.Count > 0)
                {
                    var rownumberResult = ldtRESULTSET.AsEnumerable()
                        .GroupBy(r => new { Col1 = r["ROWNUMBER"] })
                        .Select(g => g.OrderBy(r => r["TEST_CASE_NAME"]).First())
                        .CopyToDataTable();

                    var rowlengh = (rownumberResult.Rows.Count * 2) + 1;
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFile, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                        stylePart.Stylesheet = GenerateStylesheet();
                        stylePart.Stylesheet.Save();
                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                        Regex re = new Regex("[;\\/:*?\"<>|&']");
                        string tabname = re.Replace(ldtRESULTSET.Rows[0]["STORYBOARD_NAME"].ToString(), "_").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");
                        tabname = tabname.Length > 30 ? tabname.Substring(0, 29) : tabname;

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();

                        worksheetPart.Worksheet.AppendChild(ResultsetColumnsStyle(rowlengh));
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = tabname };

                        sheets.Append(sheet);
                        workbookPart.Workbook.Save();

                        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        Row row1 = new Row();
                        row1.Append(ConstructCell("Project", CellValues.String, 4));
                        var project = ldtRESULTSET.Rows[0]["PROJECT_NAME"] == null ? "" : ldtRESULTSET.Rows[0]["PROJECT_NAME"].ToString();
                        row1.Append(ConstructCell(project, CellValues.String, 4));
                        sheetData.AppendChild(row1);

                        Row row2 = new Row();
                        row2.Append(ConstructCell("Storyboard", CellValues.String, 4));
                        var storyborad = ldtRESULTSET.Rows[0]["STORYBOARD_NAME"] == null ? "" : ldtRESULTSET.Rows[0]["STORYBOARD_NAME"].ToString();
                        row2.Append(ConstructCell(storyborad, CellValues.String, 4));
                        sheetData.AppendChild(row2);

                        Row row3 = new Row();
                        row3.Append(ConstructCell("Baseline/Compare", CellValues.String, 4));
                        var rmode = ldtRESULTSET.Rows[0]["RESULTMODE"] == null ? "" : ldtRESULTSET.Rows[0]["RESULTMODE"].ToString();
                        row3.Append(ConstructCell(rmode, CellValues.String, 4));
                        sheetData.AppendChild(row3);

                        Row row4 = new Row();
                        sheetData.AppendChild(row4);

                        var descref = "";
                        var descref1 = "";
                        int desc = 0;
                        var rowFullName = "";
                        Row row5 = new Row();
                        Row row6 = new Row();
                        Row row7 = new Row();
                        Row row8 = new Row();

                        MergeCells mergeCells = new MergeCells();
                        for (int l = 0; l < rownumberResult.Rows.Count; l++)
                        {
                            var testcase = rownumberResult.Rows[l]["TEST_CASE_NAME"] == null ? "" : rownumberResult.Rows[l]["TEST_CASE_NAME"].ToString();
                            var dataset = rownumberResult.Rows[l]["DATASETNAME"] == null ? "" : rownumberResult.Rows[l]["DATASETNAME"].ToString();
                            var rowname = rownumberResult.Rows[l]["ROWNUMBER"] == null ? "" : rownumberResult.Rows[l]["ROWNUMBER"].ToString();

                            rowFullName = "TC:" + testcase + " DS:" + dataset + " Row:" + rowname;
                            if (desc == 0)
                            {
                                Cell cell = ConstructCellWithRef(rowFullName, CellValues.String, 2, "A5");
                                row5.AppendChild(cell);
                                desc++;
                                descref = IncrementColumnCellReference("A5");
                                MergeCell mergeCellt = new MergeCell() { Reference = new StringValue("A5:" + descref) };
                                mergeCells.Append(mergeCellt);
                            }
                            else
                            {
                                descref = IncrementColumnCellReference(descref);
                                Cell cell = ConstructCellWithRef(rowFullName, CellValues.String, 2, descref);
                                row5.AppendChild(cell);
                                descref1 = IncrementColumnCellReference(descref);
                                MergeCell mergeCellt = new MergeCell() { Reference = new StringValue(descref1 + ":" + descref) };
                                mergeCells.Append(mergeCellt);
                            }
                            if (descref1 != "")
                                descref = descref1;

                            row6.Append(ConstructCell("Name", CellValues.String, 4));
                            var Name = rownumberResult.Rows[l]["NAME"] == null ? "" : rownumberResult.Rows[l]["NAME"].ToString();
                            row6.Append(ConstructCell(Name, CellValues.String, 4));

                            row7.Append(ConstructCell("Description", CellValues.String, 4));
                            var Description = rownumberResult.Rows[l]["DESCRIPTITON"] == null ? "" : rownumberResult.Rows[l]["DESCRIPTITON"].ToString();
                            row7.Append(ConstructCell(Description, CellValues.String, 4));

                            row8.Append(ConstructCell("Tag", CellValues.String, 3));
                            row8.Append(ConstructCell("Value", CellValues.String, 3));
                        }
                        if (mergeCells != null)
                            worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                        sheetData.AppendChild(row5);
                        sheetData.AppendChild(row6);
                        sheetData.AppendChild(row7);
                        sheetData.AppendChild(row8);

                        Row trow = new Row();
                        Row tempRow = new Row();
                        int rowindex = 9;
                        var columnindex1 = "A";
                        var columnindex2 = "B";
                        var Incrementcolumn1 = "A9";
                        var Incrementcolumn2 = "B9";
                        for (int i = 0; i < rownumberResult.Rows.Count; i++)
                        {
                            if (rownumberResult.Rows[i]["ROWNUMBER"] != null)
                            {
                                var list = ldtRESULTSET.Select("ROWNUMBER = " + rownumberResult.Rows[i]["ROWNUMBER"]).ToList();
                                foreach (var item in list)
                                {
                                    var tempRowCount = sheetData.Elements<Row>().ToList();
                                    if (tempRowCount.Count() >= rowindex)
                                    {
                                        tempRow = tempRowCount[rowindex - 1];
                                        var cfrist = columnindex1 + Convert.ToString(rowindex);
                                        var csec = columnindex2 + Convert.ToString(rowindex);

                                        Cell cell1 = ConstructCellWithRef(item["OBJTAG"] == null ? "" : item["OBJTAG"].ToString(), CellValues.String, 1, cfrist);
                                        Cell cell2 = ConstructCellWithRef(item["OBJVALUE"] == null ? "" : item["OBJVALUE"].ToString(), CellValues.String, 1, csec);
                                        tempRow.AppendChild(cell1);
                                        tempRow.AppendChild(cell2);
                                    }
                                    else
                                    {
                                        trow = new Row();
                                        var cfrist = columnindex1 + Convert.ToString(rowindex);
                                        var csec = columnindex2 + Convert.ToString(rowindex);

                                        Cell cell1 = ConstructCellWithRef(item["OBJTAG"] == null ? "" : item["OBJTAG"].ToString(), CellValues.String, 1, cfrist);
                                        Cell cell2 = ConstructCellWithRef(item["OBJVALUE"] == null ? "" : item["OBJVALUE"].ToString(), CellValues.String, 1, csec);
                                        trow.AppendChild(cell1);
                                        trow.AppendChild(cell2);
                                        sheetData.AppendChild(trow);
                                    }
                                    rowindex++;
                                }
                            }
                            rowindex = 9;
                            Incrementcolumn1 = IncrementColumnCellReference(Incrementcolumn1);
                            Incrementcolumn1 = IncrementColumnCellReference(Incrementcolumn1);
                            Incrementcolumn2 = IncrementColumnCellReference(Incrementcolumn2);
                            Incrementcolumn2 = IncrementColumnCellReference(Incrementcolumn2);

                            columnindex1 = Convert.ToString(Incrementcolumn1.Substring(0, Incrementcolumn1.Length - 1));
                            columnindex2 = Convert.ToString(Incrementcolumn2.Substring(0, Incrementcolumn2.Length - 1));
                        }
                        worksheetPart.Worksheet.Save();
                    }
                    dbtable.errorlog("Excel is created", "", "", 0);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nResultSet The source contains no DataRows.");
                    dbtable.errorlog("ResultSet The source contains no DataRows.", "", "", 0);
                    excelFile = "";
                }
                return excelFile;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ResultSet Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportResultSetExcel " + ex.Message);
            }
        }

        public string ExportProjectResultSetExcel(DataSet ds, string excelFile)
        {
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportResultSetExcel";
            List<string> tabnamelist = new List<string>();
            int tabid = 1;
            string lTSName = string.Empty, lTCName = string.Empty, lTCDesc = string.Empty;

            DataTable ldtRESULTSET = new DataTable();
            ldtRESULTSET = ds.Tables[0];

            DataView dtvRESULTSET = new DataView();
            dtvRESULTSET = ldtRESULTSET.DefaultView;

            DataTable dtRESULTSET = new DataTable();
            dtRESULTSET = dtvRESULTSET.Table;

            ldtRESULTSET = dtRESULTSET;

            try
            {
                if (ldtRESULTSET.Rows.Count > 0)
                {
                    var result = ldtRESULTSET.AsEnumerable()
                        .GroupBy(r => new { Col1 = r["STORYBOARD_NAME"] })
                        .Select(g => g.OrderBy(r => r["STORYBOARD_NAME"]).First())
                        .CopyToDataTable();

                    if (result.Rows.Count > 0)
                    {
                        using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFile, SpreadsheetDocumentType.Workbook))
                        {
                            WorkbookPart workbookPart = document.AddWorkbookPart();
                            workbookPart.Workbook = new Workbook();

                            WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                            stylePart.Stylesheet = GenerateStylesheet();
                            stylePart.Stylesheet.Save();
                            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                            for (int m = 0; m < result.Rows.Count; m++)
                            {
                                var storyboard_name = result.Rows[m]["STORYBOARD_NAME"] == null ? "" : result.Rows[m]["STORYBOARD_NAME"].ToString();

                                var storyboardlst = (from dt in ldtRESULTSET.AsEnumerable()
                                                     where dt.Field<string>("STORYBOARD_NAME") == storyboard_name
                                                     select dt).CopyToDataTable();

                                if (storyboardlst.Rows.Count > 0)
                                {
                                    var rownumberResult = storyboardlst.AsEnumerable()
                                    .GroupBy(r => new { Col1 = r["ROWNUMBER"] })
                                    .Select(g => g.OrderBy(r => r["TEST_CASE_NAME"]).First())
                                    .CopyToDataTable();

                                    var rowlengh = (rownumberResult.Rows.Count * 2) + 1;
                                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                                    worksheetPart.Worksheet = new Worksheet();

                                    Regex re = new Regex("[;\\/:*?\"<>|&']");
                                    string tabname = re.Replace(result.Rows[m]["STORYBOARD_NAME"].ToString(), "_").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");
                                    tabname = re.Replace(result.Rows[m]["STORYBOARD_NAME"].ToString(), "-").Replace("\\", "&bs").Replace("/", "&fs").Replace("*", "&ast").Replace("[", "&ob").Replace("]", "&cb").Replace(":", "&col").Replace("?", "&qtn");
                                    tabname = tabname.Length > 30 ? tabname.Substring(0, 29) : tabname;
                                    if (tabnamelist.Count(x => x.ToUpper().Equals(tabname.ToUpper())) == 1)
                                    {
                                        tabname = tabname.Trim() + "_" + tabid;
                                        tabid++;
                                    }

                                    tabnamelist.Add(tabname);

                                    worksheetPart.Worksheet.AppendChild(ResultsetColumnsStyle(rowlengh));
                                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = Convert.ToUInt32(m + 1), Name = tabname };

                                    sheets.Append(sheet);
                                    workbookPart.Workbook.Save();
                                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                                    Row row1 = new Row();
                                    row1.Append(ConstructCell("Project", CellValues.String, 4));
                                    var project = storyboardlst.Rows[0]["PROJECT_NAME"] == null ? "" : storyboardlst.Rows[0]["PROJECT_NAME"].ToString();
                                    row1.Append(ConstructCell(project, CellValues.String, 4));
                                    sheetData.AppendChild(row1);

                                    Row row2 = new Row();
                                    row2.Append(ConstructCell("Storyboard", CellValues.String, 4));
                                    var storyborad = storyboardlst.Rows[0]["STORYBOARD_NAME"] == null ? "" : storyboardlst.Rows[0]["STORYBOARD_NAME"].ToString();
                                    row2.Append(ConstructCell(storyborad, CellValues.String, 4));
                                    sheetData.AppendChild(row2);

                                    Row row3 = new Row();
                                    row3.Append(ConstructCell("Baseline/Compare", CellValues.String, 4));
                                    var rmode = storyboardlst.Rows[0]["RESULTMODE"] == null ? "" : storyboardlst.Rows[0]["RESULTMODE"].ToString();
                                    row3.Append(ConstructCell(rmode, CellValues.String, 4));
                                    sheetData.AppendChild(row3);

                                    Row row4 = new Row();
                                    sheetData.AppendChild(row4);

                                    var descref = "";
                                    var descref1 = "";
                                    int desc = 0;
                                    var rowFullName = "";
                                    Row row5 = new Row();
                                    Row row6 = new Row();
                                    Row row7 = new Row();
                                    Row row8 = new Row();

                                    MergeCells mergeCells = new MergeCells();
                                    if (rownumberResult.Rows.Count > 0)
                                    {
                                        for (int l = 0; l < rownumberResult.Rows.Count; l++)
                                        {
                                            var testcase = rownumberResult.Rows[l]["TEST_CASE_NAME"] == null ? "" : rownumberResult.Rows[l]["TEST_CASE_NAME"].ToString();
                                            var dataset = rownumberResult.Rows[l]["DATASETNAME"] == null ? "" : rownumberResult.Rows[l]["DATASETNAME"].ToString();
                                            var rowname = rownumberResult.Rows[l]["ROWNUMBER"] == null ? "" : rownumberResult.Rows[l]["ROWNUMBER"].ToString();

                                            rowFullName = "TC:" + testcase + " DS:" + dataset + " Row:" + rowname;
                                            if (desc == 0)
                                            {
                                                Cell cell = ConstructCellWithRef(rowFullName, CellValues.String, 2, "A5");
                                                row5.AppendChild(cell);
                                                desc++;
                                                descref = IncrementColumnCellReference("A5");
                                                MergeCell mergeCellt = new MergeCell() { Reference = new StringValue("A5:" + descref) };
                                                mergeCells.Append(mergeCellt);
                                            }
                                            else
                                            {
                                                descref = IncrementColumnCellReference(descref);
                                                Cell cell = ConstructCellWithRef(rowFullName, CellValues.String, 2, descref);
                                                row5.AppendChild(cell);
                                                descref1 = IncrementColumnCellReference(descref);
                                                MergeCell mergeCellt = new MergeCell() { Reference = new StringValue(descref1 + ":" + descref) };
                                                mergeCells.Append(mergeCellt);
                                            }
                                            if (descref1 != "")
                                                descref = descref1;

                                            row6.Append(ConstructCell("Name", CellValues.String, 4));
                                            var Name = rownumberResult.Rows[l]["NAME"] == null ? "" : rownumberResult.Rows[l]["NAME"].ToString();
                                            row6.Append(ConstructCell(Name, CellValues.String, 4));

                                            row7.Append(ConstructCell("Description", CellValues.String, 4));
                                            var Description = rownumberResult.Rows[l]["DESCRIPTITON"] == null ? "" : rownumberResult.Rows[l]["DESCRIPTITON"].ToString();
                                            row7.Append(ConstructCell(Description, CellValues.String, 4));

                                            row8.Append(ConstructCell("Tag", CellValues.String, 3));
                                            row8.Append(ConstructCell("Value", CellValues.String, 3));
                                        }
                                        if (mergeCells != null)
                                            worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                                        sheetData.AppendChild(row5);
                                        sheetData.AppendChild(row6);
                                        sheetData.AppendChild(row7);
                                        sheetData.AppendChild(row8);

                                        Row trow = new Row();
                                        Row tempRow = new Row();
                                        int rowindex = 9;
                                        var columnindex1 = "A";
                                        var columnindex2 = "B";
                                        var Incrementcolumn1 = "A9";
                                        var Incrementcolumn2 = "B9";
                                        for (int i = 0; i < rownumberResult.Rows.Count; i++)
                                        {
                                            if (rownumberResult.Rows[i]["ROWNUMBER"] != null)
                                            {
                                                var list = storyboardlst.Select("ROWNUMBER = " + rownumberResult.Rows[i]["ROWNUMBER"]).ToList();
                                                foreach (var item in list)
                                                {
                                                    var tempRowCount = sheetData.Elements<Row>().ToList();
                                                    if (tempRowCount.Count() >= rowindex)
                                                    {
                                                        tempRow = tempRowCount[rowindex - 1];
                                                        var cfrist = columnindex1 + Convert.ToString(rowindex);
                                                        var csec = columnindex2 + Convert.ToString(rowindex);

                                                        Cell cell1 = ConstructCellWithRef(item["OBJTAG"] == null ? "" : item["OBJTAG"].ToString(), CellValues.String, 1, cfrist);
                                                        Cell cell2 = ConstructCellWithRef(item["OBJVALUE"] == null ? "" : item["OBJVALUE"].ToString(), CellValues.String, 1, csec);
                                                        tempRow.AppendChild(cell1);
                                                        tempRow.AppendChild(cell2);
                                                    }
                                                    else
                                                    {
                                                        trow = new Row();
                                                        var cfrist = columnindex1 + Convert.ToString(rowindex);
                                                        var csec = columnindex2 + Convert.ToString(rowindex);

                                                        Cell cell1 = ConstructCellWithRef(item["OBJTAG"] == null ? "" : item["OBJTAG"].ToString(), CellValues.String, 1, cfrist);
                                                        Cell cell2 = ConstructCellWithRef(item["OBJVALUE"] == null ? "" : item["OBJVALUE"].ToString(), CellValues.String, 1, csec);
                                                        trow.AppendChild(cell1);
                                                        trow.AppendChild(cell2);
                                                        sheetData.AppendChild(trow);
                                                    }
                                                    rowindex++;
                                                }
                                            }
                                            rowindex = 9;
                                            Incrementcolumn1 = IncrementColumnCellReference(Incrementcolumn1);
                                            Incrementcolumn1 = IncrementColumnCellReference(Incrementcolumn1);
                                            Incrementcolumn2 = IncrementColumnCellReference(Incrementcolumn2);
                                            Incrementcolumn2 = IncrementColumnCellReference(Incrementcolumn2);

                                            int a = ImportExcel.getIndexofNumber(Incrementcolumn1);
                                            string Numberpart = Incrementcolumn1.Substring(a, Incrementcolumn1.Length - a);
                                            columnindex1 = Incrementcolumn1.Substring(0, a);

                                            int a1 = ImportExcel.getIndexofNumber(Incrementcolumn2);
                                            string Numberpart1 = Incrementcolumn2.Substring(a1, Incrementcolumn2.Length - a1);
                                            columnindex2 = Incrementcolumn2.Substring(0, a1);
                                        }
                                    }
                                    worksheetPart.Worksheet.Save();
                                }
                            }
                        }
                    }
                    dbtable.errorlog("Excel is created", "", "", 0);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nResultSet The source contains no DataRows.");
                    excelFile = "";
                }
                return excelFile;
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ResultSet Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportResultSetExcel " + ex.Message);
            }
        }

        public bool ExportDatasetTagExcel(string fullpath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagExcel";

            try
            {
                ExportHelper helper = new ExportHelper();

                var list = helper.ExportDatasetTag(schema, constring);
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();
                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    //frist tab
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();
                    worksheetPart.Worksheet.AppendChild(DatasetTagColumnsStyle());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "DataSetTag" };

                    sheets.Append(sheet);
                    workbookPart.Workbook.Save();
                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row();
                    row.Append(
                        ConstructCell("DatasetName", CellValues.String, 2),
                        ConstructCell("Description", CellValues.String, 2),
                        ConstructCell("Group", CellValues.String, 2),
                        ConstructCell("Set", CellValues.String, 2),
                        ConstructCell("Folder", CellValues.String, 2),
                        ConstructCell("ExpectedResults", CellValues.String, 2),
                        ConstructCell("StepDesc", CellValues.String, 2),
                        ConstructCell("Diary", CellValues.String, 2),
                        ConstructCell("Sequence", CellValues.String, 2));

                    sheetData.AppendChild(row);

                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            row = new Row();

                            row.Append(
                                ConstructCell(item.ALIAS_NAME == null ? "" : item.ALIAS_NAME, CellValues.String, 1),
                                ConstructCell(item.DESCRIPTION_INFO == null ? "" : item.DESCRIPTION_INFO, CellValues.String, 1),
                                ConstructCell(item.GROUPNAME == null ? "" : item.GROUPNAME, CellValues.String, 1),
                                ConstructCell(item.SETNAME == null ? "" : item.SETNAME, CellValues.String, 1),
                                ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                ConstructCell(item.EXPECTEDRESULTS == null ? "" : item.EXPECTEDRESULTS, CellValues.String, 1),
                                ConstructCell(item.STEPDESC == null ? "" : item.STEPDESC, CellValues.String, 1),
                                ConstructCell(item.DIARY == null ? "" : item.DIARY, CellValues.String, 1),
                                ConstructCell(item.SEQUENCE == null ? "" : Convert.ToString(item.SEQUENCE), CellValues.String, 1));

                            sheetData.AppendChild(row);
                        }
                    }
                    worksheetPart.Worksheet.Save();

                    //secound sheet
                    WorksheetPart worksheetPart2 = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart2.Worksheet = new Worksheet();
                    worksheetPart2.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                    Sheet sheet2 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart2), SheetId = 2, Name = "Group" };

                    sheets.Append(sheet2);
                    workbookPart.Workbook.Save();
                    SheetData sheetData2 = worksheetPart2.Worksheet.AppendChild(new SheetData());

                    Row row1 = new Row();
                    row1.Append(
                        ConstructCell("Name", CellValues.String, 2),
                        ConstructCell("Description", CellValues.String, 2),
                        ConstructCell("Status", CellValues.String, 2));

                    sheetData2.AppendChild(row1);

                    var grouplist = helper.ExportDatasetTagGroup(schema, constring);

                    foreach (var item in grouplist)
                    {
                        row = new Row();
                        if (item.Name != null && item.Name != "")
                        {
                            row.Append(
                            ConstructCell(item.Name == null ? "" : item.Name, CellValues.String, 1),
                            ConstructCell(item.Description == null ? "" : item.Description, CellValues.String, 1),
                            ConstructCell(item.Status == null ? "" : item.Status, CellValues.String, 1));

                            sheetData2.AppendChild(row);
                        }
                    }
                    worksheetPart2.Worksheet.Save();

                    //third sheet
                    WorksheetPart worksheetPart3 = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart3.Worksheet = new Worksheet();
                    worksheetPart3.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                    Sheet sheet3 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart3), SheetId = 3, Name = "Set" };

                    sheets.Append(sheet3);
                    workbookPart.Workbook.Save();
                    SheetData sheetData3 = worksheetPart3.Worksheet.AppendChild(new SheetData());

                    Row row2 = new Row();
                    row2.Append(
                        ConstructCell("Name", CellValues.String, 2),
                        ConstructCell("Description", CellValues.String, 2),
                        ConstructCell("Status", CellValues.String, 2));

                    sheetData3.AppendChild(row2);

                    var setlist = helper.ExportDatasetTagSet(schema, constring);

                    foreach (var item in setlist)
                    {
                        if (item.Name != null && item.Name != "")
                        {
                            row = new Row();

                            row.Append(
                                ConstructCell(item.Name == null ? "" : item.Name, CellValues.String, 1),
                                ConstructCell(item.Description == null ? "" : item.Description, CellValues.String, 1),
                                ConstructCell(item.Status == null ? "" : item.Status, CellValues.String, 1));

                            sheetData3.AppendChild(row);
                        }
                    }
                    worksheetPart3.Worksheet.Save();

                    //fourth sheet 
                    WorksheetPart worksheetPart4 = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart4.Worksheet = new Worksheet();
                    worksheetPart4.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                    Sheet sheet4 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart4), SheetId = 4, Name = "Folder" };

                    sheets.Append(sheet4);
                    workbookPart.Workbook.Save();
                    SheetData sheetData4 = worksheetPart4.Worksheet.AppendChild(new SheetData());

                    Row row3 = new Row();
                    row3.Append(
                        ConstructCell("Name", CellValues.String, 2),
                        ConstructCell("Description", CellValues.String, 2),
                        ConstructCell("Status", CellValues.String, 2));

                    sheetData4.AppendChild(row3);

                    var folderlist = helper.ExportDatasetTagFolder(schema, constring);

                    foreach (var item in folderlist)
                    {
                        if (item.Name != null && item.Name != "")
                        {
                            row = new Row();

                            row.Append(
                                ConstructCell(item.Name == null ? "" : item.Name, CellValues.String, 1),
                                ConstructCell(item.Description == null ? "" : item.Description, CellValues.String, 1),
                                ConstructCell(item.Status == null ? "" : item.Status, CellValues.String, 1));

                            sheetData4.AppendChild(row);
                        }
                    }
                    worksheetPart4.Worksheet.Save();

                    dbtable.errorlog("Excel sheet created", "Export DatasetTag Excel", "", 0);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export DatasetTag Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagExcel " + ex.Message);
            }

            return flag;
        }

        public bool ExportDatasetTagExcelNew(string fullpath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportDatasetTagExcel";

            try
            {
                ExportHelper helper = new ExportHelper();

                using (OracleConnection lconnection = helper.GetOracleConnection(constring))
                {
                    lconnection.Open();
                    OracleCommand lcmd = lconnection.CreateCommand();
                    var list = helper.ExportDatasetTag(lcmd,schema, constring);
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                        stylePart.Stylesheet = GenerateStylesheet();
                        stylePart.Stylesheet.Save();
                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                        //frist tab
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();
                        worksheetPart.Worksheet.AppendChild(DatasetTagColumnsStyle());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "DataSetTag" };

                        sheets.Append(sheet);
                        workbookPart.Workbook.Save();
                        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        Row row = new Row();
                        row.Append(
                            ConstructCell("DatasetName", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2),
                            ConstructCell("Group", CellValues.String, 2),
                            ConstructCell("Set", CellValues.String, 2),
                            ConstructCell("Folder", CellValues.String, 2),
                            ConstructCell("ExpectedResults", CellValues.String, 2),
                            ConstructCell("StepDesc", CellValues.String, 2),
                            ConstructCell("Diary", CellValues.String, 2),
                            ConstructCell("Sequence", CellValues.String, 2));

                        sheetData.AppendChild(row);

                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                row = new Row();

                                row.Append(
                                    ConstructCell(item.ALIAS_NAME == null ? "" : item.ALIAS_NAME, CellValues.String, 1),
                                    ConstructCell(item.DESCRIPTION_INFO == null ? "" : item.DESCRIPTION_INFO, CellValues.String, 1),
                                    ConstructCell(item.GROUPNAME == null ? "" : item.GROUPNAME, CellValues.String, 1),
                                    ConstructCell(item.SETNAME == null ? "" : item.SETNAME, CellValues.String, 1),
                                    ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                    ConstructCell(item.EXPECTEDRESULTS == null ? "" : item.EXPECTEDRESULTS, CellValues.String, 1),
                                    ConstructCell(item.STEPDESC == null ? "" : item.STEPDESC, CellValues.String, 1),
                                    ConstructCell(item.DIARY == null ? "" : item.DIARY, CellValues.String, 1),
                                    ConstructCell(item.SEQUENCE == null ? "" : Convert.ToString(item.SEQUENCE), CellValues.String, 1));

                                sheetData.AppendChild(row);
                            }
                        }
                        worksheetPart.Worksheet.Save();

                        //secound sheet
                        WorksheetPart worksheetPart2 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart2.Worksheet = new Worksheet();
                        worksheetPart2.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet2 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart2), SheetId = 2, Name = "Group" };

                        sheets.Append(sheet2);
                        workbookPart.Workbook.Save();
                        SheetData sheetData2 = worksheetPart2.Worksheet.AppendChild(new SheetData());

                        Row row1 = new Row();
                        row1.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2),
                            ConstructCell("Status", CellValues.String, 2));

                        sheetData2.AppendChild(row1);

                        var grouplist = helper.ExportDatasetTagGroup(lcmd,schema, constring);

                        foreach (var item in grouplist)
                        {
                            row = new Row();
                            if (item.Name != null && item.Name != "")
                            {
                                row.Append(
                                ConstructCell(item.Name == null ? "" : item.Name, CellValues.String, 1),
                                ConstructCell(item.Description == null ? "" : item.Description, CellValues.String, 1),
                                ConstructCell(item.Status == null ? "" : item.Status, CellValues.String, 1));

                                sheetData2.AppendChild(row);
                            }
                        }
                        worksheetPart2.Worksheet.Save();

                        //third sheet
                        WorksheetPart worksheetPart3 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart3.Worksheet = new Worksheet();
                        worksheetPart3.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet3 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart3), SheetId = 3, Name = "Set" };

                        sheets.Append(sheet3);
                        workbookPart.Workbook.Save();
                        SheetData sheetData3 = worksheetPart3.Worksheet.AppendChild(new SheetData());

                        Row row2 = new Row();
                        row2.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2),
                            ConstructCell("Status", CellValues.String, 2));

                        sheetData3.AppendChild(row2);

                        var setlist = helper.ExportDatasetTagSet(lcmd,schema, constring);

                        foreach (var item in setlist)
                        {
                            if (item.Name != null && item.Name != "")
                            {
                                row = new Row();

                                row.Append(
                                    ConstructCell(item.Name == null ? "" : item.Name, CellValues.String, 1),
                                    ConstructCell(item.Description == null ? "" : item.Description, CellValues.String, 1),
                                    ConstructCell(item.Status == null ? "" : item.Status, CellValues.String, 1));

                                sheetData3.AppendChild(row);
                            }
                        }
                        worksheetPart3.Worksheet.Save();

                        //fourth sheet 
                        WorksheetPart worksheetPart4 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart4.Worksheet = new Worksheet();
                        worksheetPart4.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet4 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart4), SheetId = 4, Name = "Folder" };

                        sheets.Append(sheet4);
                        workbookPart.Workbook.Save();
                        SheetData sheetData4 = worksheetPart4.Worksheet.AppendChild(new SheetData());

                        Row row3 = new Row();
                        row3.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2),
                            ConstructCell("Status", CellValues.String, 2));

                        sheetData4.AppendChild(row3);

                        var folderlist = helper.ExportDatasetTagFolder(lcmd,schema, constring);

                        foreach (var item in folderlist)
                        {
                            if (item.Name != null && item.Name != "")
                            {
                                row = new Row();

                                row.Append(
                                    ConstructCell(item.Name == null ? "" : item.Name, CellValues.String, 1),
                                    ConstructCell(item.Description == null ? "" : item.Description, CellValues.String, 1),
                                    ConstructCell(item.Status == null ? "" : item.Status, CellValues.String, 1));

                                sheetData4.AppendChild(row);
                            }
                        }
                        worksheetPart4.Worksheet.Save();

                        dbtable.errorlog("Excel sheet created", "Export DatasetTag Excel", "", 0);
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export DatasetTag Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportDatasetTagExcel " + ex.Message);
            }

            return flag;
        }

        public bool ExportReportDatasetTagExcel(string fullpath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportReportDatasetTagExcel";

            try
            {
                ExportHelper helper = new ExportHelper();

                var list = helper.ExportReportDatasetTag(schema, constring);
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();
                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    //frist tab
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();
                    worksheetPart.Worksheet.AppendChild(DatasetTagReportColumnsStyle());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "ReportMap" };

                    sheets.Append(sheet);
                    workbookPart.Workbook.Save();
                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row();
                    row.Append(
                        ConstructCell("Project", CellValues.String, 2),
                        ConstructCell("SB", CellValues.String, 2),
                        ConstructCell("SB_Row", CellValues.String, 2),
                        ConstructCell("TC", CellValues.String, 2),
                        ConstructCell("DS", CellValues.String, 2),
                        ConstructCell("DESCRIPTION", CellValues.String, 2),
                        ConstructCell("GROUP", CellValues.String, 2),
                        ConstructCell("SET", CellValues.String, 2),
                        ConstructCell("FOLDER", CellValues.String, 2),
                        ConstructCell("SEQ", CellValues.String, 2),
                        ConstructCell("EXPECTED RESULTS", CellValues.String, 2),
                        ConstructCell("DIARY", CellValues.String, 2),
                        ConstructCell("STEPDESC", CellValues.String, 2));

                    sheetData.AppendChild(row);

                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            row = new Row();

                            row.Append(
                                ConstructCell(item.PROJECT_NAME == null ? "" : item.PROJECT_NAME, CellValues.String, 1),
                                ConstructCell(item.STORYBOARD_NAME == null ? "" : item.STORYBOARD_NAME, CellValues.String, 1),
                                ConstructCell(item.RUN_ORDER == null ? "" : Convert.ToString(item.RUN_ORDER), CellValues.String, 1),
                                ConstructCell(item.TEST_CASE_NAME == null ? "" : item.TEST_CASE_NAME, CellValues.String, 1),
                                ConstructCell(item.ALIAS_NAME == null ? "" : item.ALIAS_NAME, CellValues.String, 1),
                                ConstructCell(item.DESCRIPTION_INFO == null ? "" : item.DESCRIPTION_INFO, CellValues.String, 1),
                                ConstructCell(item.GROUPNAME == null ? "" : item.GROUPNAME, CellValues.String, 1),
                                ConstructCell(item.SETNAME == null ? "" : item.SETNAME, CellValues.String, 1),
                                ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                ConstructCell(item.SEQUENCE == null ? "" : Convert.ToString(item.SEQUENCE), CellValues.String, 1),
                                ConstructCell(item.EXPECTEDRESULTS == null ? "" : item.EXPECTEDRESULTS, CellValues.String, 1),
                                ConstructCell(item.DIARY == null ? "" : item.DIARY, CellValues.String, 1),
                                ConstructCell(item.STEPDESC == null ? "" : item.STEPDESC, CellValues.String, 1));

                            sheetData.AppendChild(row);
                        }
                        worksheetPart.Worksheet.Save();

                        //secound sheet
                        WorksheetPart worksheetPart2 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart2.Worksheet = new Worksheet();
                        worksheetPart2.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet2 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart2), SheetId = 2, Name = "Group" };

                        sheets.Append(sheet2);
                        workbookPart.Workbook.Save();
                        SheetData sheetData2 = worksheetPart2.Worksheet.AppendChild(new SheetData());

                        Row row1 = new Row();
                        row1.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2));

                        sheetData2.AppendChild(row1);

                        var grouplist = list.Select(s => new
                        {
                            GROUPNAME = s.GROUPNAME,
                            GROUPDESCRIPTION = s.GROUPDESCRIPTION
                        }).OrderBy(x => x.GROUPNAME).Distinct().ToList();

                        foreach (var item in grouplist)
                        {
                            if (item.GROUPNAME != null)
                            {
                                row = new Row();
                                row.Append(
                                    ConstructCell(item.GROUPNAME == null ? "" : item.GROUPNAME, CellValues.String, 1),
                                    ConstructCell(item.GROUPDESCRIPTION == null ? "" : item.GROUPDESCRIPTION, CellValues.String, 1));

                                sheetData2.AppendChild(row);
                            }
                        }
                        worksheetPart2.Worksheet.Save();

                        //third sheet
                        WorksheetPart worksheetPart3 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart3.Worksheet = new Worksheet();
                        worksheetPart3.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet3 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart3), SheetId = 3, Name = "Set" };

                        sheets.Append(sheet3);
                        workbookPart.Workbook.Save();
                        SheetData sheetData3 = worksheetPart3.Worksheet.AppendChild(new SheetData());

                        Row row2 = new Row();
                        row2.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2));

                        sheetData3.AppendChild(row2);

                        var setlist = list.Select(s => new
                        {
                            SETNAME = s.SETNAME,
                            SETDESCRIPTION = s.SETDESCRIPTION
                        }).OrderBy(x => x.SETNAME).Distinct().ToList();

                        foreach (var item in setlist)
                        {
                            if (item.SETNAME != null)
                            {
                                row = new Row();
                                row.Append(
                                    ConstructCell(item.SETNAME == null ? "" : item.SETNAME, CellValues.String, 1),
                                    ConstructCell(item.SETDESCRIPTION == null ? "" : item.SETDESCRIPTION, CellValues.String, 1));

                                sheetData3.AppendChild(row);
                            }
                        }
                        worksheetPart3.Worksheet.Save();

                        //fourth sheet 
                        WorksheetPart worksheetPart4 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart4.Worksheet = new Worksheet();
                        worksheetPart4.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet4 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart4), SheetId = 4, Name = "Folder" };

                        sheets.Append(sheet4);
                        workbookPart.Workbook.Save();
                        SheetData sheetData4 = worksheetPart4.Worksheet.AppendChild(new SheetData());

                        Row row3 = new Row();
                        row3.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2));

                        sheetData4.AppendChild(row3);

                        var folderlist = list.Select(s => new
                        {
                            FOLDERNAME = s.FOLDERNAME,
                            FOLDERDESCRIPTION = s.FOLDERDESCRIPTION
                        }).OrderBy(x => x.FOLDERNAME).Distinct().ToList();



                        foreach (var item in folderlist)
                        {
                            if (item.FOLDERNAME != null)
                            {
                                row = new Row();
                                row.Append(
                                    ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                    ConstructCell(item.FOLDERDESCRIPTION == null ? "" : item.FOLDERDESCRIPTION, CellValues.String, 1));

                                sheetData4.AppendChild(row);
                            }
                        }
                        worksheetPart4.Worksheet.Save();

                        dbtable.errorlog("Excel sheet created", "Export ReportDataset Excel", "", 0);
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ReportDataset Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportReportDatasetTagExcel " + ex.Message);
            }

            return flag;
        }

        //Converts Variable Model list into Excel using open xml
        public bool ExportAllStoryboradByProjectExcel(string project, string lExportPath, string schema, string constring)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportAllStoryboradByProjectExcel";
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(lExportPath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();

                    worksheetPart.Worksheet.AppendChild(AllStoryboardColumnsStyle());

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Table" };

                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();
                    ExportHelper helper = new ExportHelper();
                    var list = helper.ExportAllStoryborad(project, lExportPath, schema, constring);

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row();

                    row.Append(
                        ConstructCell("Scenario/Folder", CellValues.String, 2),
                        ConstructCell("Folder Description", CellValues.String, 2),
                        ConstructCell("StoryBoard Name", CellValues.String, 2),
                        ConstructCell("Action", CellValues.String, 2),
                        ConstructCell("Test Suite", CellValues.String, 2),
                        ConstructCell("Test Case Description", CellValues.String, 2),
                        ConstructCell("Dataset", CellValues.String, 2),
                        ConstructCell("Baseline Result", CellValues.String, 2),
                        ConstructCell("Baseline Error Cause", CellValues.String, 2),
                        ConstructCell("Baseline Script Start", CellValues.String, 2),
                        ConstructCell("Baseline Script Duration", CellValues.String, 2),
                        ConstructCell("Compareline Result", CellValues.String, 2),
                        ConstructCell("Compareline Error Cause", CellValues.String, 2),
                        ConstructCell("Compareline Script Start", CellValues.String, 2),
                        ConstructCell("Compareline Script Duration", CellValues.String, 2));

                    sheetData.AppendChild(row);

                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            string bDurationtime = "", cDurationtime = "";
                            row = new Row();
                            CultureInfo provider = CultureInfo.InvariantCulture;

                            if (item.BTEST_BEGIN_TIME != null && item.BTEST_END_TIME != null)
                            {
                                DateTime firstDate = Convert.ToDateTime(item.BTEST_BEGIN_TIME);
                                DateTime secondDate = Convert.ToDateTime(item.BTEST_END_TIME);

                                System.TimeSpan diff = secondDate.Subtract(firstDate);
                                System.TimeSpan diff1 = secondDate - firstDate;
                                var diff2 = (secondDate - firstDate).TotalSeconds;
                                TimeSpan time = TimeSpan.FromSeconds(diff2);
                                bDurationtime = time.ToString(@"hh\:mm\:ss");
                            }

                            if (item.CTEST_BEGIN_TIME != null && item.CTEST_END_TIME != null)
                            {
                                DateTime firstDate = Convert.ToDateTime(item.CTEST_BEGIN_TIME);
                                DateTime secondDate = Convert.ToDateTime(item.CTEST_END_TIME);

                                System.TimeSpan diff = secondDate.Subtract(firstDate);
                                System.TimeSpan diff1 = secondDate - firstDate;
                                var diff2 = (secondDate - firstDate).TotalSeconds;
                                TimeSpan time = TimeSpan.FromSeconds(diff2);
                                cDurationtime = time.ToString(@"hh\:mm\:ss");
                            }

                            row.Append(
                                ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                ConstructCell(item.FOLDERDESC == null ? "" : item.FOLDERDESC, CellValues.String, 1),
                                ConstructCell(item.STORYBOARD_NAME == null ? "" : item.STORYBOARD_NAME, CellValues.String, 1),
                                ConstructCell(item.ACTIONNAME == null ? "" : item.ACTIONNAME, CellValues.String, 1),
                                ConstructCell(item.SUITENAME == null ? "" : item.SUITENAME, CellValues.String, 1),
                                ConstructCell(item.CASENAME == null ? "" : item.CASENAME, CellValues.String, 1),
                                ConstructCell(item.DATASETNAME == null ? "" : item.DATASETNAME, CellValues.String, 1),
                                ConstructCell(item.BTEST_RESULT == null ? "" : item.BTEST_RESULT, CellValues.String, 1),
                                ConstructCell(item.BTEST_RESULT_IN_TEXT == null ? "" : item.BTEST_RESULT_IN_TEXT, CellValues.String, 1),
                                ConstructCell(item.BTEST_BEGIN_TIME == null ? "" : item.BTEST_BEGIN_TIME.ToString(), CellValues.String, 1),
                                ConstructCell(bDurationtime, CellValues.String, 1),
                                ConstructCell(item.CTEST_RESULT == null ? "" : item.CTEST_RESULT, CellValues.String, 1),
                                ConstructCell(item.CTEST_RESULT_IN_TEXT == null ? "" : item.CTEST_RESULT_IN_TEXT, CellValues.String, 1),
                                ConstructCell(item.CTEST_BEGIN_TIME == null ? "" : item.CTEST_BEGIN_TIME.ToString(), CellValues.String, 1),
                                ConstructCell(cDurationtime, CellValues.String, 1));

                            sheetData.AppendChild(row);
                        }
                    }
                    worksheetPart.Worksheet.Save();
                    dbtable.errorlog("Excel sheet created.", "Export Storyboard", "", 0);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export Storyborad Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportAllStoryboradByProjectExcel " + ex.Message);
            }

            return flag;
        }

        public bool ExportReportDatasetTagByFilterExcel(string fullpath, string schema, string constring, string projectIds, string storyboradIds, string FolderName)
        {
            bool flag = false;
            SomeGlobalVariables.functionName = SomeGlobalVariables.functionName + "->ExportReportDatasetTagByFilterExcel";
           
            try
            {
                ExportHelper helper = new ExportHelper();

                var list = helper.ExportReportDatasetTagById(schema, constring, projectIds, storyboradIds, FolderName);
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();
                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    //frist tab
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();
                    worksheetPart.Worksheet.AppendChild(DatasetTagReportColumnsStyle());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "ReportMap" };

                    sheets.Append(sheet);
                    workbookPart.Workbook.Save();
                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row();
                    row.Append(
                        ConstructCell("Project", CellValues.String, 2),
                        ConstructCell("SB", CellValues.String, 2),
                        ConstructCell("SB_Row", CellValues.String, 2),
                        ConstructCell("TC", CellValues.String, 2),
                        ConstructCell("DS", CellValues.String, 2),
                        ConstructCell("DESCRIPTION", CellValues.String, 2),
                        ConstructCell("GROUP", CellValues.String, 2),
                        ConstructCell("SET", CellValues.String, 2),
                        ConstructCell("FOLDER", CellValues.String, 2),
                        ConstructCell("SEQ", CellValues.String, 2),
                        ConstructCell("EXPECTED RESULTS", CellValues.String, 2),
                        ConstructCell("DIARY", CellValues.String, 2),
                        ConstructCell("STEPDESC", CellValues.String, 2));

                    sheetData.AppendChild(row);

                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            row = new Row();

                            row.Append(
                                ConstructCell(item.PROJECT_NAME == null ? "" : item.PROJECT_NAME, CellValues.String, 1),
                                ConstructCell(item.STORYBOARD_NAME == null ? "" : item.STORYBOARD_NAME, CellValues.String, 1),
                                ConstructCell(item.RUN_ORDER == null ? "" : Convert.ToString(item.RUN_ORDER), CellValues.String, 1),
                                ConstructCell(item.TEST_CASE_NAME == null ? "" : item.TEST_CASE_NAME, CellValues.String, 1),
                                ConstructCell(item.ALIAS_NAME == null ? "" : item.ALIAS_NAME, CellValues.String, 1),
                                ConstructCell(item.DESCRIPTION_INFO == null ? "" : item.DESCRIPTION_INFO, CellValues.String, 1),
                                ConstructCell(item.GROUPNAME == null ? "" : item.GROUPNAME, CellValues.String, 1),
                                ConstructCell(item.SETNAME == null ? "" : item.SETNAME, CellValues.String, 1),
                                ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                ConstructCell(item.SEQUENCE == null ? "" : Convert.ToString(item.SEQUENCE), CellValues.String, 1),
                                ConstructCell(item.EXPECTEDRESULTS == null ? "" : item.EXPECTEDRESULTS, CellValues.String, 1),
                                ConstructCell(item.DIARY == null ? "" : item.DIARY, CellValues.String, 1),
                                ConstructCell(item.STEPDESC == null ? "" : item.STEPDESC, CellValues.String, 1));

                            sheetData.AppendChild(row);
                        }
                        worksheetPart.Worksheet.Save();

                        //secound sheet
                        WorksheetPart worksheetPart2 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart2.Worksheet = new Worksheet();
                        worksheetPart2.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet2 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart2), SheetId = 2, Name = "Group" };

                        sheets.Append(sheet2);
                        workbookPart.Workbook.Save();
                        SheetData sheetData2 = worksheetPart2.Worksheet.AppendChild(new SheetData());

                        Row row1 = new Row();
                        row1.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2));

                        sheetData2.AppendChild(row1);

                        var grouplist = list.Select(s => new
                        {
                            GROUPNAME = s.GROUPNAME,
                            GROUPDESCRIPTION = s.GROUPDESCRIPTION
                        }).OrderBy(x => x.GROUPNAME).Distinct().ToList();

                        foreach (var item in grouplist)
                        {
                            if (item.GROUPNAME != null)
                            {
                                row = new Row();
                                row.Append(
                                    ConstructCell(item.GROUPNAME == null ? "" : item.GROUPNAME, CellValues.String, 1),
                                    ConstructCell(item.GROUPDESCRIPTION == null ? "" : item.GROUPDESCRIPTION, CellValues.String, 1));

                                sheetData2.AppendChild(row);
                            }
                        }
                        worksheetPart2.Worksheet.Save();

                        //third sheet
                        WorksheetPart worksheetPart3 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart3.Worksheet = new Worksheet();
                        worksheetPart3.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet3 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart3), SheetId = 3, Name = "Set" };

                        sheets.Append(sheet3);
                        workbookPart.Workbook.Save();
                        SheetData sheetData3 = worksheetPart3.Worksheet.AppendChild(new SheetData());

                        Row row2 = new Row();
                        row2.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2));

                        sheetData3.AppendChild(row2);

                        var setlist = list.Select(s => new
                        {
                            SETNAME = s.SETNAME,
                            SETDESCRIPTION = s.SETDESCRIPTION
                        }).OrderBy(x => x.SETNAME).Distinct().ToList();

                        foreach (var item in setlist)
                        {
                            if (item.SETNAME != null)
                            {
                                row = new Row();
                                row.Append(
                                    ConstructCell(item.SETNAME == null ? "" : item.SETNAME, CellValues.String, 1),
                                    ConstructCell(item.SETDESCRIPTION == null ? "" : item.SETDESCRIPTION, CellValues.String, 1));

                                sheetData3.AppendChild(row);
                            }
                        }
                        worksheetPart3.Worksheet.Save();

                        //fourth sheet 
                        WorksheetPart worksheetPart4 = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart4.Worksheet = new Worksheet();
                        worksheetPart4.Worksheet.AppendChild(DatasetTagCommanColumnsStyle());
                        Sheet sheet4 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart4), SheetId = 4, Name = "Folder" };

                        sheets.Append(sheet4);
                        workbookPart.Workbook.Save();
                        SheetData sheetData4 = worksheetPart4.Worksheet.AppendChild(new SheetData());

                        Row row3 = new Row();
                        row3.Append(
                            ConstructCell("Name", CellValues.String, 2),
                            ConstructCell("Description", CellValues.String, 2));

                        sheetData4.AppendChild(row3);

                        var folderlist = list.Select(s => new
                        {
                            FOLDERNAME = s.FOLDERNAME,
                            FOLDERDESCRIPTION = s.FOLDERDESCRIPTION
                        }).OrderBy(x => x.FOLDERNAME).Distinct().ToList();



                        foreach (var item in folderlist)
                        {
                            if (item.FOLDERNAME != null)
                            {
                                row = new Row();
                                row.Append(
                                    ConstructCell(item.FOLDERNAME == null ? "" : item.FOLDERNAME, CellValues.String, 1),
                                    ConstructCell(item.FOLDERDESCRIPTION == null ? "" : item.FOLDERDESCRIPTION, CellValues.String, 1));

                                sheetData4.AppendChild(row);
                            }
                        }
                        worksheetPart4.Worksheet.Save();

                        dbtable.errorlog("Excel sheet created", "Export ReportDataset Excel", "", 0);
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Export ReportDataset Excel", SomeGlobalVariables.functionName, line);
                throw new Exception("Error from :ExportReportDatasetTagByFilterExcel " + ex.Message);
            }

            return flag;
        }
    }
}

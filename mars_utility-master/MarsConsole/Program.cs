using MARSUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MarsConsole
{
    internal static class AssemblyCreationDate
    {

        public static readonly DateTime Value;
        public static readonly Version version;


        static AssemblyCreationDate()

        {

            version = Assembly.GetExecutingAssembly().GetName().Version;

            Value = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);

        }

    }
    public class Program
    {
        static string name = "";
        static string log_path = "";

        static bool LoadinFlag = false;
        static int ThreadIndex = 0;
        static Stopwatch stopwatch = new Stopwatch();
        static CommonHelper ObjCommon = new CommonHelper();

        private static void printHeader()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("===========================");
            Console.WriteLine("█▀▄▀█ ░ █▀▀█ ░ █▀▀█ ░ █▀▀ ░");
            Console.WriteLine("█░▀░█ ░ █▄▄█ ░ █▄▄▀ ░ ▀▀█ ░");
            Console.WriteLine("▀░░░▀ ▄ ▀░░▀ ▄ ▀░▀▀ ▄ ▀▀▀ ▄");
            Console.WriteLine("===========================");
            Console.WriteLine("MARS Import/Export Utility");
            Console.WriteLine("\n1. Import File\n2. Export File\n3. Help\n4. Clear Screen\n5. Update Connection\n6. Exit");
            Console.WriteLine();
            Console.WriteLine("Please enter you input or type -h or -? for help");
        }
        private static void ClearScreen()
        {
            //Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            //FormatConsole();
            printHeader();

        }
        public static void Main(string[] args1)
        {

            //args1 = null;
            DataTable td = new DataTable();
            DataColumn sequence_counter = new DataColumn();
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

            dbtable.dt_Log = td.Copy();

            string cmd = "";
            int dindex = 0, lindex = 0, logindex = 0, typeindex = 0, commitindex = 0, dbindex = 0, pindex = 0, sindex=0, modeindex = 0, oindex = 0;
            int tagindex = 0, descrindex = 0, iindex = 0;
            int logtake = 1;
            string command = "";

            try
            {
                if (args1 != null && args1.Length == 1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Command format is not proper\n");
                    ShowHelp();
                    return;
                }
                if (args1 != null && args1.Length > 1)
                {
                    for (int i = 0; i < args1.Length; i++)
                    {
                        if (args1[i].ToLower() == "-d")
                        {
                            dindex = i;
                        }
                        if (args1[i].ToLower() == "-l")
                        {
                            lindex = i;
                        }
                        if (args1[i].ToLower() == "-log")
                        {
                            logindex = i;
                        }
                        if (args1[i].ToLower() == "-type")
                        {
                            typeindex = i;
                        }
                        if (args1[i].ToLower() == "-tag")
                        {
                            tagindex = i;
                        }
                        if (args1[i].ToLower() == "-commit")
                        {
                            commitindex = i;
                        }
                        if (args1[i].ToLower() == "-db")
                        {
                            //   Console.WriteLine(i);
                            dbindex = i;
                        }
                        if(args1[i].ToLower() == "-p")
                        {
                            pindex = i;
                        }
                        if (args1[i].ToLower() == "-s")
                        {
                            sindex = i;
                        }
                        if (args1[i].ToLower() == "-mode")
                        {
                            modeindex = i;
                        }
                        if (args1[i].ToLower() == "-o")
                        {
                            oindex = i;
                        }
                        if (args1[i].ToLower() == "-descr")
                        {
                            descrindex = i;
                        }
                        if (args1[i].ToLower() == "-i")
                        {
                            iindex = i;
                        }
                        if (args1[i].ToLower() == "-h")
                        {
                            ShowHelp();
                            return;
                        }
                    }
                    if (args1[dindex + 1] == "I" || args1[dindex].ToUpper() == "-IMPORT")
                    {
                        for (int i = 0; i < args1.Length; i++)
                        {
                            if(lindex == 0)
                            {
                                cmd += i == dbindex + 1 || i == typeindex + 1 || i == pindex + 1 || i == modeindex + 1 || i == tagindex + 1 || i == descrindex + 1 || i == iindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                            }
                            else
                            {
                                cmd += i == lindex + 1 || i == dbindex + 1 || i == logindex + 1 || i == typeindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                            }
                        }
                        dbtable.errorlog(cmd, "Command", "", 0);
                        int f = 0;
                        
                        if(lindex == 0)
                        {
                            if (logindex == 0)
                            {
                                logtake = 0;
                            }
                            if (args1[typeindex].ToLower() != "-type")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-type not found");
                                dbtable.errorlog("-type not found", "import excel", "", 0);
                            }
                           
                            if (args1[pindex].ToLower() != "-p")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-p not found");
                                dbtable.errorlog("-p not found", "import excel", "", 0);
                            }
                            if (args1[modeindex].ToLower() != "-mode")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-mode not found");
                                dbtable.errorlog("-mode not found", "import excel", "", 0);
                            }
                            if (args1[tagindex].ToLower() != "-tag")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-tag found, it should not be present in Import Command");
                                dbtable.errorlog("-tag found, it should not be present in Import Command", "import excel", "", 0);
                            }
                            if (args1[descrindex].ToLower() != "-descr")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-descr found, it should not be present in Import Command");
                                dbtable.errorlog("-descr found, it should not be present in Import Command", "import excel", "", 0);
                            }
                            if (args1[iindex].ToLower() != "-i")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-i found, it should not be present in Import Command");
                                dbtable.errorlog("-i found, it should not be present in Import Command", "import excel", "", 0);
                            }
                        }
                        else
                        {
                            if (logindex == 0)
                            {
                                logtake = 0;
                            }
                            if (args1[dindex].ToLower() != "-d")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-d not found");
                                dbtable.errorlog("-d not found", "Import excel", "", 0);
                            }
                            if (args1[typeindex].ToLower() != "-type")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-type not found");
                                dbtable.errorlog("-type not found", "import excel", "", 0);
                            }
                            if (args1[logindex].ToLower() != "-log")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-log not found");
                                dbtable.errorlog("-log not found", "import excel", "", 0);
                            }
                            if (args1[lindex].ToLower() != "-l")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-l not found");
                                dbtable.errorlog("-l not found", "import excel", "", 0);
                            }
                            if (args1[tagindex].ToLower() == "-tag")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-tag found, it should not be present in Import Command");
                                dbtable.errorlog("-tag found, it should not be present in Import Command", "import excel", "", 0);
                            }
                            if (args1[lindex + 1] == "")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-l value is not proper");
                                dbtable.errorlog("-l not found", "import excel", "", 0);
                            }
                            if (args1[logindex + 1] == "")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-log value is not proper");
                                dbtable.errorlog("-log not found", "import excel", "", 0);
                            }
                            if (!(args1[typeindex + 1].ToUpper() == "TESTCASE" || args1[typeindex + 1].ToUpper() == "OBJECT" || args1[typeindex + 1].ToUpper() == "COMPARECONFIG" || args1[typeindex + 1].ToUpper() == "STORYBOARD" || args1[typeindex + 1].ToUpper() == "VARIABLE" || args1[typeindex + 1].ToUpper() == "OBJECT" || args1[typeindex + 1].ToUpper() == " "))
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-type value is not proper");
                                dbtable.errorlog("-type value is not proper", "import excel", "", 0);
                            }

                        }

                        if (f == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nPath Format is not proper\n");
                            ShowHelp();
                            string log = args1[logindex + 1];
                            if (!Directory.Exists(log))
                            {
                                Directory.CreateDirectory("log");
                            }
                            if (log == "")
                            {
                                log = ConfigurationManager.AppSettings["LogPath"];
                            }
                            // ShowHelp();
                            string name = Path.GetFileNameWithoutExtension(args1[lindex + 1]);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nSaving log file.......");
                            CommonHelper Obj1 = new CommonHelper();
                            Obj1.excel(dbtable.dt_Log, log, "Import", name, args1[typeindex + 1]);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nlog file saved successfully!!");
                            dbtable.dt_Log = null;
                            return;
                        }
                        else
                        {
                            if (logtake == 1 && (!Directory.Exists(args1[logindex + 1]) || args1[logindex + 1] == "" || args1[logindex + 1] == "-Type"))
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine(" Log Path Does not Exist\n");
                                f = 1;
                            }

                            if(lindex == 0)
                            {
                                if (File.Exists(args1[iindex + 1]))
                                {
                                    for (int i = 0; i < args1.Length; i++)
                                    {
                                        cmd += i == typeindex + 1 || i == pindex + 1 || i == modeindex + 1 || i == tagindex + 1 || i == descrindex + 1 || i == iindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                    }
                                    if (f == 0)
                                    {
                                        ImportExcel(cmd);
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine(" Import Path Does not Exist\n");
                                    return;
                                }
                            }
                            else
                            {
                                if (File.Exists(args1[lindex + 1]))
                                {
                                    for (int i = 0; i < args1.Length; i++)
                                    {
                                        cmd += i == lindex + 1 || i == logindex + 1 || i == typeindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                    }
                                    if (f == 0)
                                    {
                                        ImportExcel(cmd);
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine(" Import Path Does not Exist\n");
                                    return;
                                }
                            }
                        }
                    }

                    else if (args1[dindex + 1] == "E" || args1[dindex].ToUpper() == "-EXPORT")
                    {
                        //Console.WriteLine(dindex);
                        for (int i = 0; i < args1.Length; i++)
                        {
                            if(lindex == 0)
                            {
                                if(sindex > 0)
                                {
                                    cmd += i == dbindex + 1 || i == typeindex + 1 || i == pindex + 1 || i == sindex + 1 || i == modeindex + 1 || i == oindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                }else
                                {
                                    cmd += i == dbindex + 1 || i == typeindex + 1 || i == pindex + 1 || i == modeindex + 1 || i == oindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                }
                            }
                            else
                            {
                                cmd += i == lindex + 1 || i == dbindex + 1 || i == typeindex + 1 || i == tagindex + 1 || i == logindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                            }
                        }
                        dbtable.errorlog(cmd, "Command", "", 0);
                        int f = 0;

                        if (lindex == 0)
                        {
                            if (logindex == 0)
                            {
                                logindex = oindex;
                            }
                            if (args1[typeindex].ToLower() != "-type" && typeindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-type not found");
                                dbtable.errorlog("-type not found", "export excel", "", 0);
                            }
                            if (args1[pindex].ToLower() != "-p" && pindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-p not found");
                                dbtable.errorlog("-p not found", "export excel", "", 0);
                            }
                            if (args1[modeindex].ToLower() != "-mode" && modeindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-mode not found");
                                dbtable.errorlog("-mode not found", "export excel", "", 0);
                            }
                            if (args1[oindex].ToLower() != "-o" && oindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-o not found");
                                dbtable.errorlog("-o not found", "export excel", "", 0);
                            }
                        }
                        else
                        {
                            if (logindex == 0)
                            {
                                logtake = 0;
                            }

                            //Console.WriteLine(args1[typeindex].ToLower());
                            //if (args1[dindex-1] == "-D" && args1[lindex-1] == "-L" && args1[typeindex-1] == "-Type" && (args1[typeindex] == "TESTCASE"|| args1[typeindex] == "OBJECT" || args1[typeindex] == "COMPARECONFIG") && args1[tagindex-1] == "-tag" && args1[logindex-1] == "-log")
                            if (args1[dindex].ToLower() != "-d" && dindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-d not found");
                                dbtable.errorlog("-d not found", "export excel", "", 0);
                            }
                            if (args1[typeindex].ToLower() != "-type" && typeindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-type not found");
                                dbtable.errorlog("-type not found", "export excel", "", 0);
                            }
                            if (args1[logindex].ToLower() != "-log" && logindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-log not found");
                                dbtable.errorlog("-log not found", "export excel", "", 0);
                            }
                            if (args1[lindex].ToLower() != "-l" && lindex == 0)
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-l not found");
                                dbtable.errorlog("-l not found", "export excel", "", 0);
                            }
                            if (args1[typeindex + 1] != "COMPARECONFIG" && args1[typeindex + 1].ToLower() != "variable")
                            {


                                if (args1[tagindex].ToLower() != "-tag" && tagindex == 0)
                                {
                                    f = 1;
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine("-tag not found");
                                    dbtable.errorlog("-tag not found", "export excel", "", 0);
                                }

                            }
                            if (args1[typeindex + 1] == "COMPARECONFIG" & tagindex != 0)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-tag should not be there");
                                dbtable.errorlog("-tag should not be there", "export excel", "", 0);
                            }
                            if (args1[typeindex + 1] != "TESTCASE" && args1[typeindex + 1] != "OBJECT" && args1[typeindex + 1] != "COMPARECONFIG" && args1[typeindex + 1] == " ")
                            {
                                f = 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("-type value is not proper");
                                dbtable.errorlog("type value is not proper", "export excel", "", 0);
                            }
                        }

                        if (f == 1)
                        {
                            string log = args1[logindex + 1];
                            if (!Directory.Exists(log))
                            {
                                Directory.CreateDirectory("log");
                            }
                            if (log == "")
                            {
                                log = ConfigurationManager.AppSettings["LogPath"];
                            }
                            ShowHelp();
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nSaving log file.......");
                            CommonHelper Obj1 = new CommonHelper();
                            Obj1.excel(dbtable.dt_Log, log, "Export", args1[tagindex + 1], args1[typeindex + 1]);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nlog file saved successfully!!");
                            // dbtable.dt_Log = null;

                            return;
                        }
                        else
                        {
                            if (logtake == 1 && (!Directory.Exists(args1[logindex + 1])))
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine(" Log Path Does not Exist\n");
                                return;
                            }

                            if(lindex == 0)
                            {
                                if (Directory.Exists(args1[oindex + 1]))
                                {
                                    for (int i = 0; i < args1.Length; i++)
                                    {
                                        if (sindex > 0)
                                        {
                                            cmd += i == typeindex + 1 || i == pindex + 1 || i == sindex + 1 || i == modeindex + 1 || i == oindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                        }
                                        else
                                        {
                                            cmd += i == typeindex + 1 || i == pindex + 1 || i == modeindex + 1 || i == oindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                        }
                                    }
                                    ExportExcel(cmd);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine(" Export Path Does not Exist \n");
                                    return;
                                }
                            }
                            else
                            {
                                if (Directory.Exists(args1[lindex + 1]))
                                {
                                    for (int i = 0; i < args1.Length; i++)
                                    {
                                        if (args1[typeindex + 1] != "COMPARECONFIG" && tagindex != 0 && args1[typeindex + 1].ToLower() != "variable")
                                        {
                                            cmd += i == lindex + 1 || i == typeindex + 1 || i == tagindex + 1 || i == logindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                        }
                                        else
                                        {
                                            cmd += i == lindex + 1 || i == typeindex + 1 || i == logindex + 1 ? "\"" + args1[i].ToString() + "\"" + " " : args1[i].ToString() + " ";
                                        }
                                    }

                                    //dbtable.errorlog(cmd, "Command", "", 0);
                                    ExportExcel(cmd);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine(args1[dindex + 1].ToString());
                                    Console.WriteLine(" Export Path Does not Exist \n");
                                    return;
                                }
                            }
                          
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Command is not proper");
                        ShowHelp();
                        return;
                    }
                }
                else
                {
                    Maximize();
                    FormatConsole();
                    Console.Clear();
                    //printHeader();
                    //ShowHelp();
                    printHeader();

                    string choice = "";
                    while (choice != "5")
                    {

                        Console.Write("Please Select Your Choice: ");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "1":
                                name = "Import";
                                ImportExcel(null);
                                break;
                            case "2":
                                name = "Export";
                                ExportExcel(null);
                                break;
                            case "3":
                                name = "help";
                                ShowHelp();
                                break;
                            case "4":
                                ClearScreen();
                                break;
                            case "5":
                                //UpdateConnection();
                                break;
                            case "6":
                                Environment.Exit(0);
                                break;
                            case "-h":
                                ShowHelp();
                                break;
                            case "-?":
                                ShowHelp();
                                break;
                            default:

                                ClearScreen();
                                break;
                        }
                    }
                }
                args1 = null; //DefineOperation(args);


            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                //dbtable.errorlog(msg, "Error message", SomeGlobalVariables.functionName, line);
                //LoadinFlag = false;
                //ThreadIndex = 0;
                //Console.Clear();
                //Console.WriteLine("Some error occured. Press enter to continue");
                ////Console.WriteLine("Error Message: " + ex.Message);
                //Console.ReadLine();
                //DefineOperation(args);
            }
        }

        private static void ExportExcel(string cmd)
        {
            string Tag = "";
            string Type = "";
            bool presult = true;
            string log = "";
            string schema = "";
            string constring = "";
            try
            {
                SomeGlobalVariables.functionName = "ExportExcel";

                string Command = null;
                string databasename = null;
                //Console.Write("Enter database name:");
                //databasename=Console.ReadLine();
                //databasename = databasename + "";

                //if (databasename == "" || databasename == null)
                //{
                //    databasename = det.Schema;
                //    Console.Write("Do you want to proceed with Default Database[" + databasename + "] ?.Type Y to proceed: ");
                //    defaultdb = Console.ReadLine();
                //    defaultdb = defaultdb + "";

                //}
                //if(det==null)
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.Write("Database["+ databasename + "] is invalid. Please enter Database name again");
                //    Console.ReadLine();
                //    ClearScreen();
                //    dbtable.errorlog("Database [" + databasename + "] is invalid", "", "", 0);
                //    return;
                //}
                Console.ForegroundColor = ConsoleColor.White;

                //updatecon(det.Host, det.Port, det.Login, det.Password, det.Schema, det.ServiceName,det.ConnString);
                int overwr = 0;
                //if (defaultdb.ToUpper() == "Y")
                //{
                if (cmd == null)
                {
                    Console.Write("Enter Command: ");
                    Command = Console.ReadLine();
                    Command = Command + " ";
                    dbtable.errorlog(Command, "", "", 0);
                }
                else
                {
                    Command = Command + cmd;
                }
                string Location = "";

                string Config = "";
                string temp1 = "";
                string tempp2 = "";
                string storyboard = "";
                string mode = "";
                if (Command == " " || Command == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No command found to execute. Please try again");
                    dbtable.errorlog("Invalid command", "", "", 0);
                    Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    ClearScreen();
                    return;
                }

                if (Command.ToLower().IndexOf("-db ") > 0)
                {

                    temp1 = Command.Substring(Command.ToLower().IndexOf("-db "));
                    databasename = ObjCommon.ExtractString(temp1, "\"");
                    string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Config\\Mars.config";
                    //string path = ConfigurationManager.AppSettings["MarsConfig"];
                    MarsConfiguration mars = MarsConfiguration.Configure(path, databasename);
                    DatabaseConnectionDetails det = mars.GetDatabaseConnectionDetails();
                    if (det == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Database[" + databasename + "] is invalid. Please enter Database name again");
                        Console.ReadLine();
                        //ClearScreen();
                        dbtable.errorlog("Database [" + databasename + "] is invalid", "", "", 0);
                        return;
                    }
                    dbtable.errorlog("Database Name: " + databasename, "", "", 0);
                    schema = det.Schema;
                    constring = det.ConnString;
                }
                else
                {
                    string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Config\\Mars.config";
                    MarsConfiguration mars = MarsConfiguration.Configure(path, databasename);
                    DatabaseConnectionDetails det = mars.GetDatabaseConnectionDetails();
                    databasename = det.Schema;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Proceeding with default database: " + databasename);
                    schema = det.Schema;
                    constring = det.ConnString;
                    dbtable.errorlog("Database:" + databasename, "", "", 0);
                }
                if (Command.ToLower().IndexOf("-type ") > 0)
                {

                    temp1 = Command.Substring(Command.ToLower().IndexOf("-type "));
                    Type = ObjCommon.ExtractString(temp1, "\"");

                }
                if (Command.ToLower().IndexOf("-l ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-l "));
                    Location = ObjCommon.ExtractString(temp1, "\"");


                }
                if (Command.ToLower().IndexOf("-tag ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-tag "));
                    Tag = ObjCommon.ExtractString(temp1, "\"");

                }
                if (Command.ToLower().IndexOf("-log ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-log "));
                    log = ObjCommon.ExtractString(temp1, "\"");
                    if (!Directory.Exists(log))
                    {
                        Directory.CreateDirectory("log");
                    }
                    if (log == "")
                    {
                        log = ConfigurationManager.AppSettings["LogPath"];
                    }
                }
                if (Command.ToLower().IndexOf("-config") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-config"));
                    Config = ObjCommon.ExtractString(temp1, "\"");
                }
                if (Command.Contains("-h"))
                {
                    ShowHelp();

                    return;
                }
                // result set export 
                if (Command.ToLower().IndexOf("-p ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-p "));
                    Tag = ObjCommon.ExtractString(temp1, "\"");
                }

                if (Command.ToLower().IndexOf("-s ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-s "));
                    storyboard = ObjCommon.ExtractString(temp1, "\"");
                }

                if (Command.ToLower().IndexOf("-mode ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-mode "));
                    mode = ObjCommon.ExtractString(temp1, "\"");
                }

                if (Command.ToLower().IndexOf("-o ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-o "));
                    Location = ObjCommon.ExtractString(temp1, "\"");
                }
                ExportHelper expdata = new ExportHelper();
                ExportExcel exp = new ExportExcel();
                if (cmd == null || cmd == "")
                {
                    ClearScreen();
                }
                log_path = log;
                ThreadStart ts = new ThreadStart(ShowLoading);
                System.Threading.Thread t = new Thread(ts);
                LoadinFlag = true;
                ThreadIndex = 1;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Export is starting at {0}.. Please wait!", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                dbtable.errorlog("Export has been started", "", "", 0);
                t.Start();
                if (Type.ToUpper() == "VARIABLE")
                {
                    string lFileName = "variable" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, lFileName);
                    presult = exp.ExportVariableExcel(Location, schema, constring);
                }
                else if (Type.ToUpper() == "OBJECT")
                {
                    string filename = "Objects" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, filename);
                    presult = exp.ExportObjectExcel(Tag, Location, schema, constring);
                }
                else if (Type.ToUpper() == "STORYBOARD")
                {
                    string lFileName = Tag + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, lFileName);
                    presult = exp.ExportStoryboardExcel(Tag, Location, schema, constring);
                }
                else if (Type.ToUpper() == "TESTCASE" && Tag != "")
                {
                    string lFileName = Tag + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, lFileName);
                    presult = expdata.ExportTestSuite(Tag, Location, schema, constring);
                }
                else if (Type.ToUpper() == "COMPARECONFIG")
                {
                    string filename = "CompareConfig" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, filename);
                    presult = exp.ExportConfigExcel(Location, schema, constring);
                }
                else if (Type.ToUpper() == "RESULTSET")
                {
                    if (mode.ToUpper() == "BASELINE" || mode.ToUpper() == "COMPARE")
                    {
                        //Console.ForegroundColor = ConsoleColor.White;
                        //Console.WriteLine("\n {0} Export Start on {1}.", mode.ToUpper(), System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                       // dbtable.errorlog(mode.ToUpper() + "Export Start", "", "", 0);
                        int ResultMode = mode.ToUpper() == "BASELINE" ? 1 : 0;
                        if (!string.IsNullOrEmpty(storyboard))
                        {
                            string filename = "RESULT" + "_" + mode.ToUpper() + "_" + Tag + "_" + storyboard + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                            Location = Path.Combine(Location, filename);
                            presult = expdata.ExportResultSet(Tag, storyboard, ResultMode, Location, schema, constring);
                        }
                        else
                        {
                            string filename = "RESULT" + "_" + mode.ToUpper() + "_" + Tag + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                            Location = Path.Combine(Location, filename);
                            presult = expdata.ExportProjectResultSet(Tag, ResultMode, Location, schema, constring);
                        }
                    }
                    else
                    {
                        presult = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Mode is incorrect: " + mode);
                        dbtable.errorlog("Mode is incorrect: " + mode, "", "", 0);
                    }
                }
                else if (Type.ToUpper() == "DATASETTAG")
                {
                    string filename = "DATASETTAG" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, filename);
                    presult = exp.ExportDatasetTagExcel(Location, schema, constring);
                }
                else if (Type.ToUpper() == "REPORTDATASETTAG")
                {
                    string filename = "REPORTDATASETTAG" + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, filename);
                    presult = exp.ExportReportDatasetTagExcel(Location, schema, constring);
                }
                else if (Type.ToUpper() == "ALLSTORYBOARD")
                {
                    string lFileName = Tag + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                    Location = Path.Combine(Location, lFileName);
                    presult = exp.ExportAllStoryboradByProjectExcel(Tag, Location, schema, constring);
                }
                else
                {
                    LoadinFlag = false;
                    ThreadIndex = 0;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Please enter tag with proper value as \"OBJECT\" or \"COMPAREPARAM\" or \"TESTCASE\" or \"StoryBoard\" or \"Variable\"or \"ResultSet\" and try again.");
                    Console.ReadLine();
                    //Console.ForegroundColor = ConsoleColor.Blue;
                    //ClearScreen();
                    return;

                }
                if (presult == true)
                {
                    dbtable.errorlog("Export has been Completed", "", "", 0);

                    //DebugLogging(SomeGlobalVariables.functionName);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nExport has been completed on {0} !!", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                }
                else
                {
                    dbtable.errorlog("Export has been stopped", "", "", 0);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nExport has been stopped");
                }
                //}
                //else
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine("Invalid Input. Please try again");
                //    dbtable.errorlog("Invalid Input.", "", "", 0);
                //    Console.ReadLine();
                //    Console.ForegroundColor = ConsoleColor.White;
                //    ClearScreen();
                //}
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;
                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "export excel", SomeGlobalVariables.functionName, line);
                LoadinFlag = false;
                ThreadIndex = 0;
                Thread.Sleep(1000);
                ErrorLogging(ex, log, name);
            }
            finally
            {

                string name = "Log_" + Tag + "_" + Type + "_" + "Export" + "_" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
                if (log_path == "")
                    log_path = ConfigurationManager.AppSettings["LogPath"];
                string strPath = System.IO.Path.Combine(log_path, name);
                ObjCommon.excel(dbtable.dt_Log, strPath, "Export", Tag, Type);
                dbtable.dt_Log = null;
                DataTable td = new DataTable();
                DataColumn sequence_counter = new DataColumn();
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



                dbtable.dt_Log = td.Copy();

                LoadinFlag = false;
                ThreadIndex = 0;
                //Console.ForegroundColor = ConsoleColor.Blue;
                if (cmd == null || cmd == "")
                {
                    //ClearScreen();
                }
            }
        }

        private static void ImportExcel(string cmd)
        {
            int commit_flag = 0;
            string log = "";
            string Command = null;
            string Tag = "";
            string filename = "";
            string Type = "";
            string defaultdb = "Y";
            string schema = "";
            string constring = "";
            try
            {
                SomeGlobalVariables.functionName = "ImportExcel";
                int overwr = 0;
                string databasename = null;

                //updatecon(det.Host, det.Port, det.Login, det.Password, det.Schema, det.ServiceName, det.ConnString);
                //if (defaultdb.ToUpper() == "Y")
                //{
                if (cmd == null)
                {
                    Console.Write("Enter Command: ");
                    Command = Console.ReadLine();
                    Command = Command + " ";
                    dbtable.errorlog(Command, "", "", 0);
                }
                else
                {
                    Command = cmd;
                }
                if (Command == " " || Command == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No command found to execute. Please try again");
                    dbtable.errorlog("Invalid command", "", "", 0);
                    Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    ClearScreen();
                    return;
                }
                string Location = "";

                string Config = "";
                string temp1 = "";
                string tempp2 = "";
                string project = "";
                string desc = "";
                string mode = "";
                if (Command.ToLower().IndexOf("-db ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-db "));
                    databasename = ObjCommon.ExtractString(temp1, "\"");
                    string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Config\\Mars.config";
                    //string path = ConfigurationManager.AppSettings["MarsConfig"];
                    MarsConfiguration mars = MarsConfiguration.Configure(path, databasename);
                    DatabaseConnectionDetails det = mars.GetDatabaseConnectionDetails();
                    if (det == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Database[" + databasename + "] is invalid. Please enter Database name again");
                        Console.ReadLine();
                        //ClearScreen();
                        dbtable.errorlog("Database [" + databasename + "] is invalid", "", "", 0);
                        return;
                    }
                    dbtable.errorlog("Database Name: " + databasename, "", "", 0);
                    schema = det.Schema;
                    constring = det.ConnString;
                }
                else
                {
                    string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Config\\Mars.config";
                    MarsConfiguration mars = MarsConfiguration.Configure(path, databasename);
                    DatabaseConnectionDetails det = mars.GetDatabaseConnectionDetails();
                    databasename = det.Schema;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Proceeding with default database: " + databasename);
                    schema = det.Schema;
                    constring = det.ConnString;
                    dbtable.errorlog("Database: [" + databasename+"]", "", "", 0);
                }

                if (Command.ToLower().IndexOf("-type ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-type "));
                    Type = ObjCommon.ExtractString(temp1, "\"");
                }
                if (Command.ToLower().IndexOf("-l ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-l "));
                    Location = ObjCommon.ExtractString(temp1, "\"");
                    filename = Path.GetFileNameWithoutExtension(Location);
                }
                if (Command.ToLower().IndexOf("-i ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-i "));
                    Location = ObjCommon.ExtractString(temp1, "\"");
                    filename = Path.GetFileNameWithoutExtension(Location);
                }
                if (Command.ToLower().IndexOf("-tag ") > 0)
                {
                    temp1 = Command.ToLower().Substring(Command.ToLower().IndexOf("-tag "));
                    Tag = ObjCommon.ExtractString(temp1, "\"");
                }
                if (Command.ToLower().IndexOf("-log ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-log "));
                    log = ObjCommon.ExtractString(temp1, "\"");
                    if (!Directory.Exists(log))
                    {
                        Directory.CreateDirectory("log");
                    }
                    if (log == "")
                    {
                        log = ConfigurationManager.AppSettings["LogPath"];
                    }
                }
                else
                    log = ConfigurationManager.AppSettings["LogPath"];

                if (Command.ToLower().IndexOf("-config ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-config "));
                    Config = ObjCommon.ExtractString(temp1, "\"");
                }
                if (Command.ToLower().IndexOf("-p ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-p "));
                    project = ObjCommon.ExtractString(temp1, "\"");
                }

                if (Command.ToLower().IndexOf("-descr ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-descr "));
                    desc = ObjCommon.ExtractString(temp1, "\"");
                }

                if (Command.ToLower().IndexOf("-mode ") > 0)
                {
                    temp1 = Command.Substring(Command.ToLower().IndexOf("-mode "));
                    mode = ObjCommon.ExtractString(temp1, "\"");
                }

                if (Command.ToLower().Contains("-commit"))
                {
                    commit_flag = 1;
                }
                if (Command.Contains("-h"))
                {
                    ShowHelp();

                    return;
                }
                ImportHelper helper = new ImportHelper();
                if (cmd == null || cmd == "")
                {
                    ClearScreen();
                }
                log_path = log;
                ThreadStart ts = new ThreadStart(ShowLoading);
                System.Threading.Thread t = new Thread(ts);
                LoadinFlag = true;
                ThreadIndex = 1;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n\nImport is staring with date and time as {0}.. Please wait!", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                t.Start();
                bool y = true;
                log_path = log;
                int resultmode = mode.ToUpper() == "BASELINE" ? 1 : 0;
                y = helper.MasterImport(overwr, Location, log, Type, commit_flag, project, Tag, desc, resultmode, schema, constring);
                if (y == true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nImport has been completed on {0}.", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    dbtable.errorlog("Import complete", "", "", 0);
                }
                else
                {
                    dbtable.errorlog("Import stopped", "", "", 0);
                    DataTable td = new DataTable();

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nImport has been stopped");
                }
            }
            catch (Exception ex)
            {
                int line;
                string msg = ex.Message;

                line = dbtable.lineNo(ex);
                dbtable.errorlog(msg, "Import excel", SomeGlobalVariables.functionName, line);
                LoadinFlag = false;
                ThreadIndex = 0;
                //Console.ForegroundColor = ConsoleColor.Blue;
                //Console.Clear();
                // Console.WriteLine("Some error occured. Press enter to continue");
                // Console.WriteLine("Error Message: " + ex.Message);
                // Console.ReadLine();
                ErrorLogging(ex, log, name);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nSaving log file.......");
                //CommonHelper Obj1 = new CommonHelper();
                if (log_path == "")
                    log_path = ConfigurationManager.AppSettings["LogPath"];
                string name = "Log_" + Type + "_" + "Import" + "_" + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".xlsx";
                string strPath = System.IO.Path.Combine(log_path, name);
                ObjCommon.excel(dbtable.dt_Log, strPath, "Import", filename, Type);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nlog file saved successfully!!");
                dbtable.dt_Log = null;

                DataTable td = new DataTable();
                DataColumn sequence_counter = new DataColumn();
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

                dbtable.dt_Log = td.Copy();
                LoadinFlag = false;
                ThreadIndex = 0;
                if (cmd == null || cmd == "")
                {
                    ClearScreen();
                }
            }
        }
        private static void Maximize()
        {
            try
            {
                Process p = Process.GetCurrentProcess();
                //ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
            }
            catch (Exception ex)
            {
                throw new Exception("Error from :Maximize() " + ex.Message);
            }
        }

        private static void FormatConsole()
        {
            try
            {
                //Console.BackgroundColor = ConsoleColor.White;
                //Console.ForegroundColor = ConsoleColor.Blue;
                Console.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Error from :FormatConsole() " + ex.Message);
            }
        }
        private static void ShowHelp()
        {
            try
            {
                Console.CursorSize = 10;
                Console.WriteLine("*** Following are the options which you can use for this utility. ***");
                Console.WriteLine();
                Console.WriteLine("    Key        |    Used For                                          |    Description                                                                   ");
                Console.WriteLine("---------------|------------------------------------------------------|--------------------------------------------------------------------------------  ");
                Console.WriteLine("   -D          |    Import or Export                                  |  \"I\" for Import and \"E\" for Export                                           ");
                Console.WriteLine("   -L          |    Location                                          |  Location of the directory                                                     ");
                Console.WriteLine("   -Tag        |    Application or Test Suit Name or Project Name     |  For GUI use Application Name, for Test Case use Test Case Suit Name,          ");
                Console.WriteLine("                                                                         for StoryBoard use project name [Optional]                                    ");
                Console.WriteLine("   -Type       |    Type of Extract                                   |  \"OBJECT\"  for Object, \"COMPARECONFIG\" for Compare Config,\"STORYBOARD\" for STORYBOARD,");
                Console.WriteLine("                                                                         \"VARIABLE\"  for VARIABLE,\"TESTCASE\" for Testcase.   [Optional] for Import ");

                Console.WriteLine("   -log        |    Path of Log File                                  |  Location of Directory for Log Files[Optional]                                                                                                                              ");

                Console.WriteLine();
                Console.WriteLine("\n2019 (©) Marquis Business and Technology Solutions. LLC\n ");
                Console.WriteLine("Note:- All Attribute Values must be in Double Quotes.");
                Console.WriteLine("Example for TESTCASE      Import: -D I -L \"C:\\mars\\GUIObject.xls\" -Type \"TESTCASE\" -log \"C:\\mars\\log\"");
                Console.WriteLine("Exmaple for TESTCASE      Export: -D E -L \"C:\\mars\" -Type \"TESTCASE\" -Tag \"Summit6.0\" -log \"C:\\mars\\log\"");

                Console.WriteLine("Example for STORYBOARD    Import: -D I -L \"C:\\mars\\GUIObject.xls\" -Type \"STORYBOARD\" -log \"C:\\mars\\log\"");
                Console.WriteLine("Exmaple for STORYBOARD    Export: -D E -L \"C:\\mars\" -Type \"STORYBOARD\" -Tag \"OpicsTest\" -log \"C:\\mars\\log\"");

                Console.WriteLine("Example for OBJECT        Import: -D I -L \"C:\\mars\\GUIObject.xls\" -Type \"OBJECT\" -log \"C:\\mars\\log\"");
                Console.WriteLine("Exmaple for OBJECT        Export: -D E -L \"C:\\mars\" -Type \"OBJECT\" -Tag \"Summit6.0\" -log \"C:\\mars\\log\"");

                Console.WriteLine("Example for VARIABLE      Import: -D I -L \"C:\\mars\\GUIObject.xls\" -Type \"VARIABLE\" -log \"C:\\mars\\log\"");
                Console.WriteLine("Exmaple for VARIABLE      Export: -D E -L \"C:\\mars\" -Type \"VARIABLE\" -log \"C:\\mars\\log\"");

                Console.WriteLine("Example for COMPARECONFIG Import: -D I -L \"C:\\mars\\GUIObject.xls\" -Type \"COMPARECONFIG\" -log \"C:\\mars\\log\"");
                Console.WriteLine("Exmaple for COMPARECONFIG Export: -D E -L \"C:\\mars\" -Type \"COMPARECONFIG\" -log \"C:\\mars\\log\"");

                Console.WriteLine("Version:-" + AssemblyCreationDate.version);
                Console.WriteLine("Compilation Date & Time:-" + AssemblyCreationDate.Value);

                //Console.ReadLine();
                //ClearScreen();
            }

            catch (Exception ex)
            {
                throw new Exception("Error from :UpdateConnection() " + ex.Message);
            }

        }
        private static void ShowLoading()
        {
            try
            {
                stopwatch.Start();
                //Console.WriteLine("\n");
                while (LoadinFlag)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    //Console.Clear();
                    //Console.ForegroundColor = ConsoleColor.DarkGreen;
                    if (ThreadIndex == 5)
                    {
                        Console.Write("\rElapsed Time: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + " ................... |");
                        Thread.Sleep(200);
                        ThreadIndex = 1;
                    }
                    else if (ThreadIndex == 4)
                    {
                        Console.Write("\rElapsed Time: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + " ................... \\");
                        Thread.Sleep(200);
                        ThreadIndex++;
                    }
                    else if (ThreadIndex == 3)
                    {
                        Console.Write("\rElapsed Time: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + " ................... -");
                        Thread.Sleep(200);
                        ThreadIndex++;
                    }
                    else if (ThreadIndex == 1)
                    {
                        Console.Write("\rElapsed Time: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + " ................... |");
                        Thread.Sleep(200);
                        ThreadIndex++;
                    }
                    else if (ThreadIndex == 2)
                    {
                        Console.Write("\rElapsed Time: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + " ................... /");
                        Thread.Sleep(200);
                        ThreadIndex++;
                    }
                    //Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\rElapsed Time: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + "                             ");
                Console.ForegroundColor = ConsoleColor.White;
                //Console.ForegroundColor = ConsoleColor.Blue;
            }
            catch (Exception ex)
            {
                throw new Exception("Error from :ShowLoading() " + ex.Message);
            }
            stopwatch.Stop();
        }
        public static void ErrorLogging(Exception ex, string path, string filename)
        {
            string path1 = filename + DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss") + ".txt";
            string strPath = System.IO.Path.Combine(path, path1);
            if (!File.Exists(strPath))
            {
                FileStream fs = File.Create(strPath);
                fs.Close();
            }
            try
            {
                using (StreamWriter sw = File.AppendText(strPath))
                {
                    sw.WriteLine("=============Error Logging ===========");
                    sw.WriteLine("===========Start============= " + DateTime.Now);
                    sw.WriteLine("Error Message: " + ex.Message);
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                    sw.WriteLine("===========End=============\n " + DateTime.Now);

                }
            }
            catch (Exception ex1)
            {
            }
        }
        //private static void ValidateCommands(string Command)
        //{
        //    int dindex = 0, lindex = 0, logindex = 0, typeindex = 0, commitindex = 0;
        //    int tagindex = 0;
        //    int logtake = 1;
        //    string cmd = "";
        //    if (Command != null)
        //    {
        //        if (Command.ToLower().IndexOf("-type ") <=0)
        //        {

        //            Console.WriteLine("Type Tag not found");
        //        }
        //        if (Command.ToLower().IndexOf("-l ") <= 0)
        //        {

        //            Console.WriteLine("-l tag not found");
        //        }
        //        if (Command.ToLower().IndexOf("-tag ") <= 0 && Command.ToLower().IndexOf("-type ")!="COMPARECONFIG")
        //        {
        //            Console.WriteLine("Tag not found");
        //        }
        //        if (Command.ToLower().IndexOf("-log ") <= 0)
        //        {
        //            Console.WriteLine("Log path not found");

        //        }
        //        if (Command.ToLower().IndexOf("-config") > 0)
        //        {

        //        }
        //        if (Command.Contains("-h"))
        //        {
        //            ShowHelp();

        //            return;
        //        }

        //    }
        //}

        //private static void updatecon(string host,string port,string login,string password,string schema,string servicename,string constring)
        //{
        //    string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    string configFile = System.IO.Path.Combine(appPath, "MarsConsole.exe.config");
        //    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        //    configFileMap.ExeConfigFilename = configFile;
        //    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

        //    config.AppSettings.Settings["DataSource"].Value = constring;
        //    config.AppSettings.Settings["Schema"].Value = schema;
        //    config.Save();

        //}

    }
}

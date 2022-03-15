using Mars_Serialization.DBConfig;
using Mars_Serialization.JsonSerialization;
using MARS_Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace MarsCacheTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string configPath = System.Configuration.ConfigurationManager.AppSettings["ConfigFilePath"];
            string jsonPath = System.Configuration.ConfigurationManager.AppSettings["JsonFilePath"];
            MarsConfig mc = MarsConfig.Configure(configPath,string.Empty);

            var connections = mc.GetConnectionDetails();
            //storyboard  (all /id) db  (db)   
            try
            {
                //args = new string[] { "Storyboard","11"/*,"db", "GEN_MARS_20" ,"true"*/};

                bool needReflesh = false;
                if (args.Length == 5)
                {
                    needReflesh = Convert.ToBoolean(args[4]);
                }
                
                if (args.Length >= 4)
                {
                    var connection = connections.FirstOrDefault(r => r.Schema.ToLower().Trim() == args[3].ToLower().Trim());
                    if (connection != null&& args[2].ToLower() == "db")
                    {
                        MarsConfig config = MarsConfig.Configure(configPath, args[3]);
                        MARS_Web.Helper.DatabaseConnectionDetails det = config.GetDatabaseConnectionDetails();

                        long dataid = 0;
                        if (args[1].ToLower() != "all")
                        {
                            dataid = Convert.ToInt64(args[1]);
                        }
                        if (args[0]  == "Storyboard")
                        {
                            JsonFileHelper.InitStoryBoardJson(det.Schema, configPath, dataid,needReflesh);
                        }
                        else 
                        { 
                            var applist = SerializationFile.GetAppList(det.ConnString);
                            SerializationFile.conString = det.ConnString;
                            SerializationFile.CreateJsonFilesNew(det.Schema, jsonPath, args[0],applist,dataid,needReflesh);
                        }
                    }
                    else
                    {
                        Console.WriteLine("args input error.");
                    }
                }
                else if (args.Length == 2)
                {
                    long dataid = 0;
                    if (args[1].ToLower() == "all" || long.TryParse(args[1],out dataid))
                    {
                        foreach (var connect in connections)
                        {
                            MarsConfig config = MarsConfig.Configure(configPath, connect.Schema);
                            MARS_Web.Helper.DatabaseConnectionDetails det = config.GetDatabaseConnectionDetails();

                            if (args[1].ToLower() != "all")
                            {
                                dataid = Convert.ToInt64(args[1]);
                            }
                            if (args[0]  == "Storyboard")
                            {
                                JsonFileHelper.InitStoryBoardJson(det.Schema, configPath,dataid, needReflesh);
                            }
                            else
                            {
                                var applist = SerializationFile.GetAppList(det.ConnString);
                                SerializationFile.conString = det.ConnString;
                                SerializationFile.CreateJsonFilesNew(det.Schema, jsonPath, args[0], applist, dataid, needReflesh);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("args input error.");
                    }
                }
                else
                {
                    Console.WriteLine("args input error.");
                }

                Console.WriteLine("Init finshed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

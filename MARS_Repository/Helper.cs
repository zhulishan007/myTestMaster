using MARS_Repository.Entities;


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.EntityClient;
using System.Data.Mapping;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MARS_Repository
{
    public class Helper
    {
        public static long NextTestSuiteId(string SeqName)
        {
                DBEntities enty = Helper.GetMarsEntitiesInstance();
                ObjectParameter outparam = new ObjectParameter("v_NEXTVAL", typeof(Int32));
                var projectId = enty.GETNEXT_VAL(SeqName, outparam);
                var ID = long.Parse(outparam.Value.ToString());
                return ID;
           
        }

        //public static string getConnectionString()
        //{
        //    return ConfigurationManager.AppSettings["Entities"];
        //}

        private static MetadataWorkspace workspace = null;
        private static bool isThesame = false;
        private static string newschemaname;
        public static string NewSchemaName
        {
            get
            {
                return newschemaname;
            }
            set
            {
                newschemaname = value;
            }
        }
        public static EntityConnectionStringBuilder connectionBuilder = null;
        private static DbProviderFactory DBProviderFactory = null;
        private static string EntityModelName = "Entities.Db_Context";
        private static DBEntities gMarsEntites = null;
        public static string ConnectionStr = string.Empty;
        public static string Schema = string.Empty;
        //
        public static EntityConnection MarsEntityConnection
        {
            get
            {
                //connectionBuilder = new EntityConnectionStringBuilder(ConfigurationManager.AppSettings["Entities"]);
                if (!string.IsNullOrEmpty(DBEntities.ConnectionString))
                {
                    connectionBuilder = new EntityConnectionStringBuilder(DBEntities.ConnectionString);
                }
                //else
                //{
                //    connectionBuilder = new EntityConnectionStringBuilder(ConfigurationManager.AppSettings["Entities"]);
                //}
                //if (NewSchemaName == null)
                //{
                //    return new EntityConnection(connectionBuilder.ToString());
                //}

                //if (workspace != null)
                //{
                    //if (isThesame)
                    //{
                    //    return new EntityConnection(connectionBuilder.ToString());
                    //}

                    //NewSchemaName = ConfigurationManager.AppSettings["Schema"];
                    if (!string.IsNullOrEmpty(DBEntities.Schema))
                    {
                        NewSchemaName = DBEntities.Schema;
                    }
                    //else
                    //{
                    //    NewSchemaName = ConfigurationManager.AppSettings["Schema"];
                    //}

                    Func<string, Stream> generateStream =
                        extension => Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Concat(EntityModelName, extension));
                    Action<IEnumerable<Stream>> disposeCollection = streams =>
                    {
                        if (streams == null)
                            return;

                        foreach (var stream in streams.Where(stream => stream != null))
                            stream.Dispose();
                    };
                    var conceptualReader = generateStream(".csdl");
                    var mappingReader = generateStream(".msl");
                    var storageReader = generateStream(".ssdl");

                    if (conceptualReader == null || mappingReader == null || storageReader == null)
                    {
                        disposeCollection(new[] { conceptualReader, mappingReader, storageReader });
                        return null;
                    }
                    var storageXml = XElement.Load(storageReader);
                    XNamespace store = XNamespace.Get("http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator");
                    try
                    {
                        foreach (var entitySet in storageXml.Descendants())
                        {
                            //var schemaAttribute = entitySet.Attributes("Schema").FirstOrDefault();
                            var schemaAttribute = entitySet.Attributes(store + "Schema").FirstOrDefault();
                            if (schemaAttribute != null)
                                schemaAttribute.SetValue(store + NewSchemaName);
                        }
                    }
                    catch (Exception e)
                    {
                        //  Logger.Error("MarsEntityConnection", string.Format("Exception:[{0}]", e.Message), e);
                    }

                    string strEntityName = "";
                    foreach (var entitySet in storageXml.Descendants())
                    {
                        var schemaAttribute = entitySet.Attributes("Schema").FirstOrDefault();
                        if (schemaAttribute != null)
                        {
                            strEntityName += "\r\n" + entitySet.Name;
                            if (string.Compare(schemaAttribute.Value, NewSchemaName, true) == 0)
                            {
                                isThesame = true;
                                break;
                            }
                            //schemaAttribute.SetValue(schemaName);
                            schemaAttribute.Value = !string.IsNullOrEmpty(NewSchemaName) ? NewSchemaName : NewSchemaName;
                        }
                    }
                   // if (!isThesame)
                   // {
                        storageXml.CreateReader();

                        workspace = new MetadataWorkspace();

                        var storageCollection = new StoreItemCollection(new[] { storageXml.CreateReader() });
                        var conceptualCollection = new EdmItemCollection(new[] { XmlReader.Create(conceptualReader) });
                        var mappingCollection = new StorageMappingItemCollection(conceptualCollection,
                                                                                storageCollection,
                                                                                new[] { XmlReader.Create(mappingReader) });

                try
                {
                    workspace.RegisterItemCollection(conceptualCollection);
                    workspace.RegisterItemCollection(storageCollection);
                    workspace.RegisterItemCollection(mappingCollection);
                }
                catch(Exception ex)
                {

                }
                 //   }
                    DBProviderFactory = DbProviderFactories.GetFactory(connectionBuilder.Provider);
                    if (DBProviderFactory == null) return null;
                    if (isThesame)
                    {
                       // return new EntityConnection(connectionBuilder.ToString());
                    }
                //}
              
                DbConnection dbCnn = DBProviderFactory.CreateConnection();
                dbCnn.ConnectionString = connectionBuilder.ProviderConnectionString;
                dbCnn.StateChange += new StateChangeEventHandler((sender, stateE) => {
                    StateChangeEventArgs z = (StateChangeEventArgs)stateE;
                    if ((z.CurrentState == ConnectionState.Open) && (z.OriginalState != ConnectionState.Open))
                    {
                        DbCommand cmd = dbCnn.CreateCommand();
                        cmd.CommandText = string.Format("ALTER SESSION SET CURRENT_SCHEMA ={0}", NewSchemaName);
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //  Logger.Error("StateChangeEventHandler", string.Format("Exception:[{0}]", ex.Message), ex);
                        }
                    }
                }
                );
                //if (!isThesame)
                return new EntityConnection(workspace, dbCnn);

            }
        }

        public static DBEntities GetMarsEntitiesInstance(bool needReopen = true)
        {


            if (needReopen)
            {
                gMarsEntites = null;
            }
            if (gMarsEntites == null)
            {
                gMarsEntites = new DBEntities(Helper.MarsEntityConnection);

            }

            return gMarsEntites;
        }
    }
}

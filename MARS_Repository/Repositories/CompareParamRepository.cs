using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MARS_Repository.Repositories
{
    public class CompareParamRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;
        public List<CompareParam> ListCompareConfig()
        {
            try
            {
                logger.Info(string.Format("List Compare Config start | Username: {0}", Username));
                var Comparelist = (from c in entity.T_DATA_SOURCE
                                   select new CompareParam { DATA_SOURCE_ID = c.DATA_SOURCE_ID, DATA_SOURCE_NAME = c.DATA_SOURCE_NAME, DATA_SOURCE_TYPE = c.DATA_SOURCE_TYPE, DETAILS = c.DETAILS, DB_TYPE = c.DB_TYPE, DB_CONNECTION = c.DB_CONNECTION, TEST_CONNECTION = c.TEST_CONNECTION }).ToList();

                logger.Info(string.Format("List Compare Config end | Username: {0}", Username));
                return Comparelist;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured CompareParamRepository in ListCompareConfig method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured CompareParamRepository in ListCompareConfig method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured CompareParamRepository in ListCompareConfig method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
        }
        
        public string AddorEditCompareconfig(string name, string data, short datatype)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("AddorEditCompareconfig start | Username: {0}", Username));
                    var datasource = (from o in entity.T_DATA_SOURCE
                                      where o.DATA_SOURCE_NAME == name
                                      && o.DATA_SOURCE_TYPE == datatype
                                      select o).FirstOrDefault();
                    if (datasource == null)
                    {
                        datasource = new T_DATA_SOURCE();
                        datasource.DATA_SOURCE_NAME = name;
                        datasource.DATA_SOURCE_ID = Helper.NextTestSuiteId("T_TEST_STEPS_SEQ");
                        datasource.DATA_SOURCE_TYPE = datatype;
                        entity.T_DATA_SOURCE.Add(datasource);
                    }
                    datasource.DETAILS = data;
                    entity.SaveChanges();

                    logger.Info(string.Format("AddorEditCompareconfig end | Username: {0}", Username));
                    scope.Complete();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured AddorEditCompareconfig in ListCompareConfig method | DataSource Name: {0} | UserName: {1}", name, Username));
                ELogger.ErrorException(string.Format("Error occured AddorEditCompareconfig in ListCompareConfig method | DataSource Name: {0} | UserName: {1}", name, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured AddorEditCompareconfig in ListCompareConfig method | DataSource Name: {0} | UserName: {1}", name, Username), ex.InnerException);
                throw;
            }
        }
       
        public string DeleteCompareConfig(string id,short dataType)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    logger.Info(string.Format("DeleteCompareConfig start | Username: {0}", Username));
                    var dataSource = (from o in entity.T_DATA_SOURCE
                                      where o.DATA_SOURCE_NAME == id
                                      && o.DATA_SOURCE_TYPE == dataType
                                      select o).FirstOrDefault();
                    if (dataSource != null)
                    {
                        entity.T_DATA_SOURCE.Remove(dataSource);
                        entity.SaveChanges();
                        logger.Info(string.Format("DeleteCompareConfig end | Username: {0}", Username));
                        return "success";
                    }
                    logger.Info(string.Format("DeleteCompareConfig end | Username: {0}", Username));
                    scope.Complete();
                    return "error";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured DeleteCompareConfig in ListCompareConfig method | DataSource Name: {0} | UserName: {1}", id, Username));
                ELogger.ErrorException(string.Format("Error occured DeleteCompareConfig in ListCompareConfig method | DataSource Name: {0} | UserName: {1}", id, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured DeleteCompareConfig in ListCompareConfig method | DataSource Name: {0} | UserName: {1}", id, Username), ex.InnerException);
                throw;
            }
        }
    }
}

using MARS_Repository.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.Repositories
{
    public class CompanyRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities entity = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;

        public List<T_MARS_COMPANY> GetCompanyList(){
            try
            {
                logger.Info(string.Format("Get CompanyList start | Username: {0}", Username));
                var result = entity.T_MARS_COMPANY.ToList();
                logger.Info(string.Format("Get CompanyList end | Username: {0}", Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured User in GetCompanyList method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured User in GetCompanyList method | UserName: {0}", Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured User in GetCompanyList method | UserName: {0}", Username), ex.InnerException);
                throw;
            }
           
        }
    }
}

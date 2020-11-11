using MARS_Repository.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.Repositories
{
    public class TestProjectRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;


        public bool ChangeTestProjectName(string lTestProjectName, long lTestProjectId)
        {
            try
            {
                logger.Info(string.Format("Change TestProjectName start | ProjectId: {0} | UserName: {1}", lTestProjectId, Username));
                var lresult = false;
                var lTestProject = enty.T_TEST_PROJECT.Find(lTestProjectId);
                lTestProject.PROJECT_NAME = lTestProjectName;
                enty.SaveChanges();
                lresult = true;

                logger.Info(string.Format("Change TestProjectName end | ProjectId: {0} | UserName: {1}", lTestProjectId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestProjectRepository in ChangeTestProjectName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestProjectRepository in ChangeTestProjectName method | UserName: {0}", Username), ex);
                throw;
            }
        }

        public bool CheckDuplicateTestProjectName(string lTestProjectName, long? lTestProjectId)
        {
            try
            {
                logger.Info(string.Format("Check Duplicate TestProjectName start | ProjectId: {0} | UserName: {1}", lTestProjectId, Username));
                var lresult = false;

                if (lTestProjectId != null)
                {
                    lresult = enty.T_TEST_PROJECT.Any(x => x.PROJECT_ID != lTestProjectId && x.PROJECT_NAME.ToLower().Trim() == lTestProjectName.ToLower().Trim());
                }
                else
                {
                    lresult = enty.T_TEST_PROJECT.Any(x => x.PROJECT_NAME.ToLower().Trim() == lTestProjectName.ToLower().Trim());
                }
                logger.Info(string.Format("Check Duplicate TestProjectName end | ProjectId: {0} | UserName: {1}", lTestProjectId, Username));
                return lresult;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured TestProjectRepository in CheckDuplicateTestProjectName method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured TestProjectRepository in CheckDuplicateTestProjectName method | UserName: {0}", Username), ex);
                throw;
            }
        }
        public string GetProjectNameById(long ProjectId)
        {
            try
            {
                logger.Info(string.Format("Get ProjectName start | ProjectId: {0} | UserName: {1}", ProjectId, Username));
                var lProjectName = enty.T_TEST_PROJECT.FirstOrDefault(x => x.PROJECT_ID == ProjectId).PROJECT_NAME;
                logger.Info(string.Format("Get ProjectName end | ProjectId: {0} | UserName: {1}", ProjectId, Username));
                return lProjectName;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured storyborad in GetProjectNameById method | UserName: {0}", Username));
                ELogger.ErrorException(string.Format("Error occured storyborad in GetProjectNameById method | UserName: {0}", Username), ex);
                throw;
            }
        }
    }
}

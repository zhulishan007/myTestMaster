using MARS_Api.Helper;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace MARS_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ConnectionController : ApiController
    {
        // GET: Connection
        [System.Web.Http.Route("api/GetConnectionList")]
        public BaseModel GetConnectionList()
        {
            BaseModel baseModel = new BaseModel();
            try
            {
                List<ConnectionModel> connectionModels = new List<ConnectionModel>();
                string MarsEnvironment = string.Empty;
                MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
                var defaultDb = mc.GetDefaultDatabase();
                var Connlst = mc.GetConnectionDetails();
                foreach (var item in Connlst)
                {
                    ConnectionModel connectionModel = new ConnectionModel();
                    connectionModel.DatabaseName = item.Databasename;
                    connectionModel.Host = item.Host;
                    connectionModel.IsDefault = item.Databasename == defaultDb ? true : false;
                    connectionModels.Add(connectionModel);
                }
                baseModel.data = connectionModels;
                baseModel.status = 1;
                baseModel.message = "Success";
            }
            catch (Exception ex)
            {
                baseModel.data = null;
                baseModel.status = 0;
                baseModel.message = "Error : " + ex.ToString();
            }
            return baseModel;
        }
    }
}
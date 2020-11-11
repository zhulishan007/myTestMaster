using MARS_Api.Helper;
using MARS_Repository.ViewModel;
using MARS_Web.Helper;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Http;


namespace MARS_Api.Controllers
{
    public class DecodeTokenController : ApiController
    {
        // GET: DecodeToken
        [Route("api/DecodeToken")]
        [AcceptVerbs("GET", "POST")]
        public UserMasterModel DecodeToken(string accesstoken)
        {
            UserMasterModel userMasterModel = new UserMasterModel();
             var secureDataFormat = new TicketDataFormat(new MachineKeyProtector());
            AuthenticationTicket ticket = secureDataFormat.Unprotect(accesstoken);

            if (ticket.Identity.Claims.Any())
            {
                var Claimslst = ticket.Identity.Claims.ToList();
                userMasterModel.UserName = Claimslst[0].Value;
                userMasterModel.UserEmail = Claimslst[1].Value;
                userMasterModel.DBConnection = Claimslst[2].Value;
            }
            return userMasterModel;
        }
    }
}
using MARS_Api.Controllers;
using MARS_Repository.Entities;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;

using MARS_Web.Helper;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace MARS_Api.Helper
{
    public static class CommonHelper
    {
        public static UserMasterModel ValidateUser(string username, string password)
        {
            UserMasterModel userMasterModel = new UserMasterModel();
            AccountRepository Accountrepo = new AccountRepository();
            var lUser = Accountrepo.GetUserByEmailAndLoginName(username);

            if (lUser != null)
            {
                var lUserPassword = PasswordHelper.DecodeString(lUser.TESTER_PWD);
                if (lUserPassword == password)
                {
                    userMasterModel.UserName = lUser.TESTER_LOGIN_NAME;
                    userMasterModel.UserEmail = lUser.TESTER_MAIL;
                    userMasterModel.Password = lUser.TESTER_PWD;
                }
            }
            return userMasterModel;
        }

        public static string GetHeaderToken(HttpRequestMessage httpRequest)
        {
            string token = string.Empty;
            var headers = httpRequest.Headers;
            if (headers.Contains("Authorization"))
            {
                var Headerval = headers.GetValues("Authorization").First();
                var Headerarr = Headerval.Split(' ');
                if (Headerarr.Any())
                    token = Headerarr[1];
            }
            return token;
        }
        public static UserMasterModel DecodeToken(string accesstoken)
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
        public static DatabaseConnectionDetails GetConnectionSting(string MarsEnvironment)
        {
            MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
            return mc.GetDatabaseConnectionDetails();
        }
        public static void SetConnectionString(HttpRequestMessage Request)
        {
            string token = GetHeaderToken(Request);
            if (!string.IsNullOrEmpty(token))
            {
                var decodeVal = DecodeToken(token);
                var dbConnResult = GetConnectionSting(decodeVal.DBConnection);

                if (dbConnResult != null)
                {
                    DBEntities.ConnectionString = dbConnResult.EntityConnString;
                    DBEntities.Schema = dbConnResult.Schema;
                }
                else
                    SetConnectionstring();
            }
            else
                SetConnectionstring();
        }

        public static void SetConnectionstring()
        {
            string MarsEnvironment = string.Empty;
            MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
            var defaultDb = mc.GetDefaultDatabase();
            var Result = GetConnectionSting(defaultDb);
            DBEntities.ConnectionString = Result.EntityConnString;
            DBEntities.Schema = Result.Schema;
        }

        public static DatabaseConnectionDetails SetAppConnectionString(HttpRequestMessage Request)
        {
            string MarsEnvironment = string.Empty;
            string token = GetHeaderToken(Request);
            var decodeVal = DecodeToken(token);
            var Result = GetConnectionSting(decodeVal.DBConnection);
            if (Result == null)
            {
                MarsConfig mc = MarsConfig.Configure(MarsEnvironment);
                var defaultDb = mc.GetDefaultDatabase();
                Result = GetConnectionSting(defaultDb);
            }
            return Result;
        }
    }
}
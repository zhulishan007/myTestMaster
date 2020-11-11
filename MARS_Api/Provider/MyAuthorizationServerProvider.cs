using MARS_Api.Helper;
using MARS_Repository.Entities;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MARS_Api.Provider
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var Scopelst = context.Scope;

            var dbConnResult = CommonHelper.GetConnectionSting(Scopelst[0]);

            if (dbConnResult != null)
            {
                DBEntities.ConnectionString = dbConnResult.EntityConnString;
                DBEntities.Schema = dbConnResult.Schema;
            }
            var user = CommonHelper.ValidateUser(context.UserName, context.Password);
            if (user == null || (user.UserEmail == null && user.UserName == null))
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("Email", user.UserEmail));
            if(Scopelst.Any())
                identity.AddClaim(new Claim("DBConnection", Scopelst[0]));
            context.Validated(identity);

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
        }
    }
}
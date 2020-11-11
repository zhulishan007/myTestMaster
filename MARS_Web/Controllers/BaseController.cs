using MARS_Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public void SetDatabaseConnectionString(string ConnectionString, string Schema)
        {
            HttpCookie ckU = new HttpCookie("ConnectionString");
            ckU.Expires = DateTime.Now.AddDays(1);
            ckU.Value = ConnectionString;
            Response.Cookies.Add(ckU);

            HttpCookie ckP = new HttpCookie("Schema");
            ckP.Expires = DateTime.Now.AddDays(1);
            ckP.Value = Schema;
            Response.Cookies.Add(ckP);
        }

        public void GetConectionString()
        {
            if (Request != null)
            {
                if (Request.Cookies["ConnectionString"] != null)
                    SessionManager.ConnectionString = Request.Cookies["ConnectionString"].Value;

                if (Request.Cookies["Schema"] != null)
                    SessionManager.ConnectionString = Request.Cookies["Schema"].Value;
            }
        }
    }
}
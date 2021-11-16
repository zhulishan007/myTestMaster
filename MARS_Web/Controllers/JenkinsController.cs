using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class JenkinsController : Controller
    {
        // GET: Jenkins
        public ActionResult JenkinsIntegration()
        {
            return PartialView("JenkinsIntegration");
        }
    }
}
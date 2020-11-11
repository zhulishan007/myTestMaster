using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Routing;

namespace MARS_Api.Helper
{
    public class CustomExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext filterContext)
        {

            //ErrorLog log = new ErrorLog()
            //{
            //    ExceptionMessage = filterContext.Exception.Message,
            //    ExceptionStackTrace = filterContext.Exception.StackTrace,
            //    ControllerName = filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
            //    LogTime = DateTime.Now,
            //    CreatedBy = "API",
            //    URL = filterContext.Request.RequestUri.ToString(),
            //    UrlReferrer = "",
            //    UserAgent = filterContext.Request.Headers.UserAgent.ToString(),
            //    UserHostAddress = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress,
            //    UserHostName = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Request.UserHostName
            //};

            //ApplicationDbContext db = new ApplicationDbContext();
            //db.ErrorLog.Add(log);
            //db.SaveChanges();

        }
    }
}
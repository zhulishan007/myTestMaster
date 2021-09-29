using MARS_Web.Helper;
using MARS_Web.MarsUtility;
using MARS_Web.RESTfulApiClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

using MARS_Web.controllerPartner;

namespace MARS_Web.Controllers
{

    public class MarsBasicController : Controller
    {
        public const string cnst_view_key_isViewWithError = "_IS_VIEWWITHERROR";
        public const string cnst_view_key_currentViewError = "_CURRENT_VIEW_ERROR";
        public const string cnst_view_key_MarjorData = "_VIEW_MARJOR_DATA";

        public bool isViewWithError = false;
        public string currentViewError = "";
    }
    
    [SessionTimeout]
    public class ReportManagementController : MarsBasicController
    {


        private static MarsLog Logger = MarsLog.GetLogger(Mars_LogType._normal, typeof(ReportManagementController));
        private static MarsLog ELogger = MarsLog.GetLogger(Mars_LogType._errLog, typeof(ReportManagementController));
        // GET: ReportManagement
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ReportManagementMainView()
        {
            Logger.LogBegin("ReportManagementMainView");
            string strAdv = "";
            try
            {
                /// get all data from api then set to session
                /// 
                /**
                 * 算法：
                 * 1，获得目标RESTful 地址，
                 * 2，获得已经存储的数据库连接和相关的数据source
                 * 3，put into session
                 * 4， 展示页面
                 * */
                //* 1，获得目标RESTful 地址
                string strURL = $"{MarsConfig.restfulInfo.HostName.Trim()}:{MarsConfig.restfulInfo.Port}";
                if (string.IsNullOrEmpty(MarsRESTfulApiclient.WebURLPrefix))
                {
                    MarsRESTfulApiclient.WebURLPrefix = strURL;
                }
                /// 2，获得已经存储的数据库连接和相关的数据source, 
                ///   需要的数据包包括：
                ///   i，数据库链接
                MarsWebRESTfulApiClientExtend clnt = new MarsWebRESTfulApiClientExtend(SessionManager.Schema);
                string strError = "", strStack = "" ;
                bool isOk = false;
                var dataSource = clnt.GetDataSource(ref isOk, ref strError, ref strStack);
                if ((dataSource == null) || (isOk == false))
                {
                    //显示错误信息
                    Logger.Error("ReportManagementMainView",strError, strStack);
                    strAdv = $"Error [{strError}]\r\nPlease contact Marquis";
                    ViewData.Add(new KeyValuePair<string, object>(cnst_view_key_isViewWithError, true));
                    ViewData.Add(new KeyValuePair<string, object>(cnst_view_key_currentViewError, strAdv));
                    return PartialView("ReportManagementMainView");
                }
                ViewData.Add(new KeyValuePair<string, object>(cnst_view_key_isViewWithError, false));
                ViewData.Add(new KeyValuePair<string, object>(cnst_view_key_MarjorData, dataSource));
                ViewBag.dataSource = dataSource;
                var p = new ReportManagerControllerPartner();
                string strData = p.convertDataToJson(dataSource, ref isOk, ref strError, ref strStack, ref strAdv);
                var data = new System.Net.Http.HttpResponseMessage()
                {
                    Content = new System.Net.Http.StringContent(strData, System.Text.Encoding.UTF8, "application/json"),
                };
                ViewData.Add(new KeyValuePair<string, object>("_TABLE_RESOURCE", data));
                
                return PartialView("ReportManagementMainView");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                strAdv = $"Error [{e.Message}]\r\nPlease contact Marquis";
                ViewData.Add(new KeyValuePair<string, object>(cnst_view_key_isViewWithError, true));
                ViewData.Add(new KeyValuePair<string, object>(cnst_view_key_currentViewError, strAdv));
                return PartialView("ReportManagementMainView"); 
            }
            finally
            {
                Logger.LogEnd();
            }
        }
        [HttpGet]
        public ActionResult Test()
        {
            Logger.LogBegin("Test");
            return null;
        }

        [HttpPost]
        public ActionResult TestPost()
        {
            Logger.LogBegin("TestPost");
            return null;
        }

    }
}
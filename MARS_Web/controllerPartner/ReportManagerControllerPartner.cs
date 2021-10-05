using MARS_Web.Controllers;
using MARS_Web.MarsUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.controllerPartner
{

    public class ReportManagerControllerPartner
    {

        private static MarsLog Logger = MarsLog.GetLogger(Mars_LogType._normal, typeof(ReportManagementController));

        public const string CNST_PRIVILEGE_ADD_DATASROUCE = "Add Datasource";

        public bool GetIsviewWithError(ViewDataDictionary vd)
        {
            if (vd == null) return true;
            object o = null;
            if ((o=vd[MarsBasicController.cnst_view_key_isViewWithError]) == null) return true;
            bool isWithError = false;
            if (!bool.TryParse(o.ToString(), out isWithError))
            {
                return true;
            }
            return isWithError;
        }

        public string GetCurrentError(ViewDataDictionary vd)
        {
            if (vd == null) return null;
            object o = vd[MarsBasicController.cnst_view_key_currentViewError];
            return o == null ? null : o.ToString();
        }

        public T GetData<T>(ViewDataDictionary vd,ref bool isOk, ref string strError, ref string strStack, ref string strAdv)
        {
            if (vd == null)
            {
                isOk = false;
                strError = "view data is null";
                strAdv = "Contact Marquis";
                strStack = Environment.StackTrace;
                return default(T);
            }
            if (vd[MarsBasicController.cnst_view_key_MarjorData] == null)
            {
                isOk = false;
                strError = "no view data is set";
                strAdv = "Contact Marquis";
                strStack = Environment.StackTrace;
                return default(T);
            }
            
            if (typeof(List<Mars.Dto.T_DATA_SOURCEDTO>) != typeof(T))
            {
                strStack = Environment.StackTrace;
                strAdv = "Contact Marquis";
                strError = "Data type is wrong, only List<Mars.Dto.T_DATA_SOURCEDTO> is supported";
                return default(T);
            }
            isOk = true;
            return (T)vd[MarsBasicController.cnst_view_key_MarjorData] ;
        }

        public string convertDataToJson(object data, ref bool isOk, ref string strError, ref string strStack, ref string strAdv)
        {
            Logger.LogBegin("convertDataToJson");
            try
            {
                isOk = false;
                if (data == null) return null;
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                string strData = js.Serialize(data);
                isOk = true;
                //return new HttpResponseMessage()
                //{
                //    Content =
                //    new System.Net.Http.StringContent(strData, System.Text.Encoding.UTF8, "application/json")
                //}
                return strData;
            }
            catch (Exception e)
            {
                Logger.Error("convertDataToJson", strError = e.Message, strStack = e.StackTrace);
                strAdv = $"Contact Marquis\r\n{strError} ";
                return "";
            }
            finally
            {
                Logger.LogEnd("convertDataToJson");
            }
        }
    }
}
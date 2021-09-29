using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace MARS_Web.MarsUtility
{

    public enum Mars_LogType
    {
        _normal,
        _errLog
    }
    public sealed class MarsLog
    {
        private Logger logger = null;
        public static MarsLog GetLogger(Mars_LogType strType, Type t)
        {
            return new MarsLog(strType, t);
        }

        private MarsLog(Mars_LogType strType, Type t)
        {
            if (strType == Mars_LogType._normal)
                logger = LogManager.GetLogger("Log", t);
            else
                logger = LogManager.GetLogger("ErrorLog", t);
        }

        public void LogBegin(string strMethodName , string strPara = null,[CallerLineNumber] int iLn = 0 )
        {
            logger.Info($"[BEGIN] at [ln:{iLn}] {strMethodName}, {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}" + strPara ?? "");
        }
        public void LogEnd(string strPara = null, string strMethodName = null,[CallerLineNumber] int iLn = 0)
        {
            logger.Info($"[END] at [ln:{iLn}] {strMethodName}, {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}" + strPara ?? "");
        }
        public void Info(string strInfo, string strPara = null, string strMethodName = null,[CallerLineNumber] int iLn = 0 )
        {
            logger.Info($"[INFO] at [ln:{iLn}] {strMethodName}, {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}" + strPara ?? "");
        }

        public void Error(string strMethodName ,string strInfo, string strStack=null, string strPara = null, [CallerLineNumber] int iLn = 0)
        {
            logger.Info($"[ERROR] at [ln:{iLn}] {strMethodName}, {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}" + strPara ?? "" + $"\r\n {strStack}");
        }

        //private string getInnerExceptions(Exception e)
        public void Error(string strMethodName,string strInfo, Exception e , [CallerLineNumber] int iLn = 0)
        {
            string strInnerExceptions = "";
            Exception eTmp = e.InnerException;
            while (eTmp != null)
            {
                strInnerExceptions += $"\r\n{eTmp.Message}\r\n{eTmp.StackTrace}";
                eTmp = eTmp.InnerException;
            }
            logger.Info($"[ERROR] [{strInfo}], Excepiton at [ln:{iLn}] {strMethodName}{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")} \r\n\t[{e.Message}]\r\n\t[{e.StackTrace}]"
                + (string.IsNullOrEmpty(strInnerExceptions) ? "" : strInnerExceptions));
        }

        public void Error(string strInfo, Exception e, [CallerLineNumber] int iLn = 0, [CallerMemberName] string strMethodName = null)
        {
            string strInnerExceptions = "";
            Exception eTmp = e.InnerException;
            while (eTmp != null)
            {
                strInnerExceptions += $"\r\n{eTmp.Message}\r\n{eTmp.StackTrace}";
                eTmp = eTmp.InnerException;
            }
            logger.Info($"[ERROR] [{strInfo}], Excepiton at [ln:{iLn}] {strMethodName}{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")} \r\n\t[{e.Message}]\r\n\t[{e.StackTrace}]"
                + (string.IsNullOrEmpty(strInnerExceptions) ? "" : strInnerExceptions));
        }

    }

}
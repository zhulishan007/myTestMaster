using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public static class SessionManager
    {
        public static decimal TESTER_ID
        {
            get { return Convert.ToDecimal(HttpContext.Current.Session["TESTER_ID"]); }
            set { HttpContext.Current.Session["TESTER_ID"] = value; }
        }

        public static String TESTER_MAIL
        {
            get { return Convert.ToString(HttpContext.Current.Session["TESTER_MAIL"]); }
            set { HttpContext.Current.Session["TESTER_MAIL"] = value; }
        }

        public static String TESTER_NAME_F
        {
            get { return Convert.ToString(HttpContext.Current.Session["TESTER_NAME_F"]); }
            set { HttpContext.Current.Session["TESTER_NAME_F"] = value; }
        }

        public static String TESTER_NAME_M
        {
            get { return Convert.ToString(HttpContext.Current.Session["TESTER_NAME_M"]); }
            set { HttpContext.Current.Session["TESTER_NAME_M"] = value; }
        }

        public static String TESTER_NAME_LAST
        {
            get { return Convert.ToString(HttpContext.Current.Session["TESTER_NAME_LAST"]); }
            set { HttpContext.Current.Session["TESTER_NAME_LAST"] = value; }
        }

        public static String TESTER_LOGIN_NAME
        {
            get { return Convert.ToString(HttpContext.Current.Session["TESTER_LOGIN_NAME"]); }
            set { HttpContext.Current.Session["TESTER_LOGIN_NAME"] = value; }
        }

        public static String TESTER_NUMBER
        {
            get { return Convert.ToString(HttpContext.Current.Session["TESTER_NUMBER"]); }
            set { HttpContext.Current.Session["TESTER_NUMBER"] = value; }
        }

        public static string ConnectionString
        {
            get { return Convert.ToString(HttpContext.Current.Session["ConnectionString"]); }
            set { HttpContext.Current.Session["ConnectionString"] = value; }
        }

        public static string Schema
        {
            get { return Convert.ToString(HttpContext.Current.Session["Schema"]); }
            set { HttpContext.Current.Session["Schema"] = value; }
        }
        public static string APP
        {
            get { return Convert.ToString(HttpContext.Current.Session["APP"]); }
            set { HttpContext.Current.Session["APP"] = value; }
        }
        public static string Host
        {
            get { return Convert.ToString(HttpContext.Current.Session["Host"]); }
            set { HttpContext.Current.Session["Host"] = value; }
        }
        public static string Accesstoken
        {
            get { return Convert.ToString(HttpContext.Current.Session["Accesstoken"]); }
            set { HttpContext.Current.Session["Accesstoken"] = value; }
        }
    }
}
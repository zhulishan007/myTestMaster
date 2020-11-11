using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Web.Helper
{
    public class PasswordHelper
    {
        private static byte[] cnst_key = {84,105,71,101,114,76,73,69,87,
            84, 105, 71, 101, 114, 76, 73};//87
        public static string EncodeString(string strSrc)
        {
            TripleDESCryptoServiceProvider objDesProvider = new TripleDESCryptoServiceProvider();
            objDesProvider.Key = UTF8Encoding.UTF8.GetBytes(System.Text.Encoding.Default.GetString(cnst_key));
            objDesProvider.Mode = CipherMode.ECB;
            objDesProvider.Padding = PaddingMode.PKCS7;
            ICryptoTransform icTransform = objDesProvider.CreateEncryptor();
            byte[] arrSrc = UTF8Encoding.UTF8.GetBytes(strSrc);
            byte[] byResult = icTransform.TransformFinalBlock(arrSrc, 0, arrSrc.Length);
            objDesProvider.Clear();
            return Convert.ToBase64String(byResult, 0, byResult.Length);
        }

        public static string DecodeString(string strSrc)
        {
            try
            {
                TripleDESCryptoServiceProvider objDesProvider = new TripleDESCryptoServiceProvider();
                objDesProvider.Key = UTF8Encoding.UTF8.GetBytes(System.Text.Encoding.Default.GetString(cnst_key));
                objDesProvider.Mode = CipherMode.ECB;
                objDesProvider.Padding = PaddingMode.PKCS7;
                ICryptoTransform icTransform = objDesProvider.CreateDecryptor();
                byte[] arrSrc = Convert.FromBase64String(strSrc);
                byte[] byResult = icTransform.TransformFinalBlock(arrSrc, 0, arrSrc.Length);
                objDesProvider.Clear();
                return UTF8Encoding.UTF8.GetString(byResult, 0, byResult.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
            return strSrc;
        }
    }
}

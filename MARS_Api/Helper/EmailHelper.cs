﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MarsApi.Helper
{
    public class EmailHelper
    {

        public static string ForgotPsw(string lUserEmail, string lPsw, string lUserName, string body, string ltempKey)
        {

            var lMsg = "";
            //var lFile = AppDomain.CurrentDomain.BaseDirectory + "EmailHTML\\ForgotEmail.html";
            //StreamReader sr = new StreamReader(lFile);
            //text = text.Replace("some text", "new value");
            //File.WriteAllText("test.txt", text);


            body = body.Replace("[UserName]", lUserName);
            body = body.Replace("[UserEmail]", lUserEmail);
            body = body.Replace("[UserPsw]", lPsw);
            body = body.Replace("[TempKey]", ltempKey);

            using (MailMessage mailMessage = new MailMessage())
            {
                //mailMessage.From = new MailAddress("cherishpatel43@gmail.com");
                //mailMessage.Subject = "MARS Forgot Password";
                //mailMessage.Body = body;
                //mailMessage.IsBodyHtml = true;
                //mailMessage.To.Add(new MailAddress(lUserEmail));
                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = "smtp.gmail.com";
                ////smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                //System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                //NetworkCred.UserName = "cherishpatel43@gmail.com";
                //NetworkCred.Password = "cherish7043patel";
                //smtp.UseDefaultCredentials = true;
                //smtp.Credentials = NetworkCred;
                //smtp.Port = 587;
                //smtp.Send(mailMessage);
                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.convergesolution.com", 587);
                mail.From = new MailAddress("info@convergesolution.com");
                mail.To.Add(new MailAddress(lUserEmail));
                mail.Subject = "MARS Forgot Password";
                mail.Body = body;
                mail.IsBodyHtml = true;
                smtpServer.Credentials = new NetworkCredential("info@convergesolution.com", "Csinfo@123");
                smtpServer.EnableSsl = true;
                NEVER_EAT_POISON_Disable_CertificateValidation();
                smtpServer.Send(mail);
                lMsg = "Succefully password sent in your email.";
            }

            return lMsg;
        }


        static void NEVER_EAT_POISON_Disable_CertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }
    }
}

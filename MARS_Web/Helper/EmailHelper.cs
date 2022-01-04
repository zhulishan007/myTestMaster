using MARS_Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace MARS_Web.Helper
{
  public class EmailHelper
  {
        public static string logFilePath = string.Empty;
        public static string Port = WebConfigurationManager.AppSettings["Port"];
        public static string smtpclient = WebConfigurationManager.AppSettings["smtpclient"];
        public static string EmailId = WebConfigurationManager.AppSettings["EmailId"];
        public static string EmailPassword = WebConfigurationManager.AppSettings["EmailPassword"];
        public static string ForgotPasswordSubject = WebConfigurationManager.AppSettings["ForgotPasswordSubject"];
        public static string logPath = WebConfigurationManager.AppSettings["LogPathLocation"];
        
        public static string ForgotPsw(string lUserEmail, string lPsw, string lUserName, string body, string ltempKey, string lConnection,string lLink)
    {
     
      var lMsg = "";
      

      body = body.Replace("[UserName]", lUserName);
      body = body.Replace("[UserEmail]", lUserEmail);
      body = body.Replace("[UserPsw]", lPsw);
      body = body.Replace("[TempKey]", ltempKey);
      body = body.Replace("[ConnectionString]", lConnection);
      body = body.Replace("[Link]", lLink);

            using (MailMessage mailMessage = new MailMessage())
      {
        
        var mail = new MailMessage();
        var smtpServer = new SmtpClient(smtpclient, Convert.ToInt32(Port));
        mail.From = new MailAddress(EmailId);
        mail.To.Add(new MailAddress(lUserEmail));
        mail.Subject = ForgotPasswordSubject;
        mail.Body = body;
        mail.IsBodyHtml = true;
        smtpServer.Credentials = new NetworkCredential(EmailId, EmailPassword);
        smtpServer.EnableSsl = true;
        NEVER_EAT_POISON_Disable_CertificateValidation();
        smtpServer.Send(mail);
        lMsg = "Succefully password sent in your email.";
      }

      return lMsg;
    }
        public static void WriteMessage(string message, string currentPath, string logtype, string filename)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string Filepath = currentPath + "\\Log." + dt.Day + "." + dt.Month + "." + dt.Year + ".txt";
                string Ip = GetLocalIPAddress();
                string Content = DateTime.Now + " | " + Convert.ToString(logtype) + " | " + filename + " | " + message;
                WriteToFile(Filepath, Content + Environment.NewLine, true);
            }
            catch (Exception EX)
            {
                string s = EX.Message;
            }
        }

        public static void WriteLogMessage(string message)
        {
            try
            {
                string currentPath = System.Web.HttpContext.Current.Server.MapPath("~/" + logPath + "/");
                DateTime dt = DateTime.Now;
                string Filepath = currentPath + "\\Log." + dt.Day + "." + dt.Month + "." + dt.Year + ".txt";
                string Ip = GetLocalIPAddress();
                string Content = DateTime.Now + " | " + "HostName: " + Ip + " | " + message;
                WriteToFile(Filepath, Content + Environment.NewLine, true);
            }
            catch (Exception EX)
            {
                string s = EX.Message;
            }
        }
        private static void WriteToFile(string filePath, string content, bool append)
        {
            using (StreamWriter sw = new StreamWriter(filePath, append))
            {
                sw.WriteAsync(content);
                sw.Flush();
                sw.Close();
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
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
          )
          {
            return true;
          };
    }
  }
}

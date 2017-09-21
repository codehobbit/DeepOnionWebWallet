using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace OnionWallet.Framework.Email
{
    public static class GmailHandler
    {
        public static void SendMail(string recipient, string subject, string body)
        {
            // TODO ErrorHandling

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(ConfigurationManager.AppSettings["SiteEmail"].ToString());
            msg.To.Add(recipient);
            msg.Subject = subject;
            msg.Body = body;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SiteEmail"].ToString(), ConfigurationManager.AppSettings["SiteEmailPassword"].ToString());
            client.Timeout = 20000;
            client.Send(msg);
            msg.Dispose();
        }
    }
}

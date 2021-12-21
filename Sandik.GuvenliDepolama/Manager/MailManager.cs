using Sandik.GuvenliDepolama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Sandik.GuvenliDepolama.Manager
{
    public class MailManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subject">Mail Konusu</param>
        /// <param name="Body">Mail İçeriği</param>
        /// <param name="To">Mail Göndilrcek Adres. adresler arasında , veya ; olabilir.</param>
        /// <returns></returns>
        public bool SendMail(string Subject,string Body,string To)
        {
            var ret = false;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Setting.MailFrom);
                mail.To.Add(To.Replace(';', ','));
                mail.Subject = Subject;
                mail.IsBodyHtml = true;
                mail.Body = Body;

                SmtpClient c = new SmtpClient
                {
                    Port = Setting.MailPort,
                    EnableSsl = Setting.MailSecurity.IndexOf("SSL") >= 0,
                    Host = Setting.MailHost,
                    Credentials = new NetworkCredential(Setting.MailUser, Setting.MailPassword)
                };

                c.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Mail gönderilemedi", ex);
            }
            return ret;                 
        }
    }
}
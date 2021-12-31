using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandik.GuvenliDepolama.Models
{
    public static class Setting
    {
        public static string ConnectionString { get; set; }
        public static string NotificationMailAdress { get; set; }
        public static int TwoFactorTimeOut { get; set; }
        public static string MailHost { get; set; }
        public static int MailPort { get; set; } = 0;
        public static string MailFrom { get; set; }
        public static string MailUser { get; set; }
        public static string MailPassword { get; set; }
        public static string MailSecurity { get; set; }
        public static string FrontendKey { get; set; }
        public static string BackendKey { get; set; }
        public static string UploadFilePath { get; set; }
        public static string AesKey { get; set; }
    }
}
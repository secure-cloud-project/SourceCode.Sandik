using Sandik.GuvenliDepolama.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sandik.GuvenliDepolama
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Setting.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
            Setting.TwoFactorTimeOut = int.Parse(ConfigurationManager.AppSettings["TwoFactorTimeOut"].ToString());

            Setting.UploadFilePath = ConfigurationManager.AppSettings["UploadFilePath"].ToString();

            //Mail Settings
            Setting.MailHost = ConfigurationManager.AppSettings["MailHost"];
            Setting.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            Setting.MailUser = ConfigurationManager.AppSettings["MailUser"];
            Setting.MailPassword = ConfigurationManager.AppSettings["MailPassword"];
            Setting.MailPort = int.Parse(ConfigurationManager.AppSettings["MailPort"]);
            Setting.MailSecurity = ConfigurationManager.AppSettings["MailSecurity"];

            Setting.AesKey = ConfigurationManager.AppSettings["AesKey"];
        }
    }
}

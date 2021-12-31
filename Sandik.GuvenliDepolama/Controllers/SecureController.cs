using Sandik.GuvenliDepolama.Helper;
using Sandik.GuvenliDepolama.Manager;
using Sandik.GuvenliDepolama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sandik.GuvenliDepolama.Controllers
{
    public class SecureController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Login(User item)
        {
            var isOK = false;
            try
            {
                SqlManager sql = new SqlManager();
                GenerateRandom gr = new GenerateRandom();
                var secureCode = gr.GetRandomNumeric(6);

                //var secureCodeEnc = AesManager.Encrypt(secureCode, Setting.AesKey);
                //item.MailEnc = AesManager.Encrypt(item.Mail, Setting.AesKey);
                //item.PasswordEnc = AesManager.Encrypt(item.Password, Setting.AesKey);

                var user = sql.CheckUser(item.Mail, item.Password, secureCode);
                if (user != null)
                {
                    isOK = true;
                    MailManager m = new MailManager();
                    m.SendMail("Sandık Doğrulama Kodu", secureCode, user.Mail);
                    SetLogin(user);
                }
            }
            catch (System.Exception)
            {

                throw;
            }

            return Json(new { isOk = isOK }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SignIn(User item)
        {
            var isOK = false;
            var err = "";
            try
            {
                item.NameEnc = AesManager.Encrypt(item.Name, Setting.AesKey);
                item.SurNameEnc = AesManager.Encrypt(item.SurName, Setting.AesKey);
                item.MailEnc = AesManager.Encrypt(item.Mail, Setting.AesKey);
                item.PasswordEnc = AesManager.Encrypt(item.Password, Setting.AesKey);

                SqlManager sql = new SqlManager();
                err = sql.AddUser(item);
                isOK = err.Length == 0;
            }
            catch (System.Exception)
            {
                throw;
            }

            return Json(new { isOk = isOK, Msj = err }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginTwoFactor()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoginTwoFactor(String SecureCode)
        {
            var isOK = false;
            var err = "";
            SqlManager sql = new SqlManager();
            var user = (User)Session["User"];
            if (user != null)
            {
                var retSql = sql.CheckSecureCode(user.Mail, SecureCode);
                isOK = retSql.Length == 0;
                if (isOK)
                {
                    user.IsTwoFactorPass = true;
                }
                err = retSql;
            }
            return Json(new { isOk = isOK, Msj = err }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            SetLogOut();
            return RedirectToAction("/Login");
        }

        private void SetLogin(User user)
        {
            //FormsAuthentication.SetAuthCookie(user.Name + " " + user.SurName, true);
            Session["User"] = user;
        }

        private void SetLogOut()
        {
            //FormsAuthentication.SignOut();
            Session["User"] = "";
        }
    }
}
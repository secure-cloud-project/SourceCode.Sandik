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

        /// <summary>
        /// login olurken session dan kullanici bilgilerini aliyor, random numeric kod uretiyor ve maile gonderiyor
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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

        // Kullanici kayit kismi, disaridan gelen User model tipindeki item listinin icerisindeki verileri
        // aes ile sifreliyor ve sql manager kullanarak sql procedure'une gonderiyor
        [HttpPost]
        public JsonResult SignIn(User item)
        {
            // degiskenler olusturuluyor
            var isOK = false;
            var err = "";

            try
            {
                item.NameEnc = AesManager.Encrypt(item.Name, Setting.AesKey);
                // Burada isim aes manager kullanilarak sifreleniyor
                item.SurNameEnc = AesManager.Encrypt(item.SurName, Setting.AesKey);
                // Burada soy isim aes manager kullanilarak sifreleniyor
                item.MailEnc = AesManager.Encrypt(item.Mail, Setting.AesKey);
                // Burada mail aes manager kullanilarak sifreleniyor
                item.PasswordEnc = AesManager.Encrypt(item.Password, Setting.AesKey);
                // Burada sifre aes manager kullanilarak sifreleniyor

                SqlManager sql = new SqlManager();
                // sql manager ornegi olusturuluyor
                err = sql.AddUser(item);
                // sql'de AddUser isimli precedure calistirilarak donen hata err degiskenine atiliyor
                isOK = err.Length == 0;
                // burada eger err degiskenine bir mesaj gelmez ise isOK bool degiskenine 1 degeri giriliyor
                // yani hata yok demek, eger procedure den bir hata mesaji geri donerse isOK'e 0 degeri atanir
                // ve hata oldugu belli olur
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

        /// <summary>
        /// iki adimli dogrulama disaridan kodu aliyor ve sesion dan maili aliyor
        /// </summary>
        /// <param name="SecureCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoginTwoFactor(String SecureCode)
        {
            // degiskenler olusturuluyor
            var isOK = false;
            var err = "";
            SqlManager sql = new SqlManager();
            var user = (User)Session["User"];

            if (user != null)// user bilgilerini tutan listesi bos degil ise
            {
                var retSql = sql.CheckSecureCode(user.Mail, SecureCode);
                // Burada bilgiler Manager dosyalarinin oldugu yerde sqlManager a yollaniyor 
                // icerisine mail ve girilen secure kod gonderiliyor
                // procedure un hata olması durumunda geri donderecegi metin retSql isimli degiskene atiliyor
                isOK = retSql.Length == 0;
                // eger retSQL degiskeninin ici bos ise bir hata olmadigini kayit etmek icin isOK degiskenine 1 degeri giriliyor
                if (isOK)// hata yoksa (dogru kod girilmisse) iki asamali dogrulamadan geciriliyor
                {
                    user.IsTwoFactorPass = true;
                }
                err = retSql;
            }

            // eger hata varsa (secure kod yanlis girilmisse) procedure den gelen hata mesaji json olarak javascript e gonderiliyor
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

            // login olundugunda bilgileri session a kayit ediyoruz
            Session["User"] = user;
        }

        private void SetLogOut()
        {
            //FormsAuthentication.SignOut();

            //logout olundugunda session u temizliyoruz
            Session["User"] = "";
        }
    }
}
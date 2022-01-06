using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandik.GuvenliDepolama.Models
{
    public class User
    {
        public int ID { get; set; }
        public string LicenseID { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; } = "";
        public DateTime CreationTime { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsTwoFactorPass { get; set; } = false;

        public string NameEnc { get; set; }
        public string SurNameEnc { get; set; }
        public string MailEnc { get; set; }
        public string PasswordEnc { get; set; } = "";

    }
}
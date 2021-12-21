using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandik.GuvenliDepolama.Models
{
    public class TwoFactor
    {
        public int ID { get; set; }
        public int LastTimeMinute { get; set; }
    }
}
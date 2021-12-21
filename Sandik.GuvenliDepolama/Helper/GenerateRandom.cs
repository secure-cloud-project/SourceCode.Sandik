using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandik.GuvenliDepolama.Helper
{
    public class GenerateRandom
    {
        public String GetRandomNumeric(int Size)
        {
            var ret = "";
            Random rnd = new Random();
            for (int i = 0; i < Size; i++)
            {
                ret += (rnd.Next(0, 9)).ToString();
            }
            return ret;
        }
    }
}
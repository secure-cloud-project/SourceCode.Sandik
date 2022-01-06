using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SharpAESCrypt;

namespace Sandik.GuvenliDepolama.Manager
{
    public class FileManager
    {
        public byte[] Encrypt(Stream input, string password)
        {
            //Stream inputStream = new MemoryStream(input);
            var outputStream = new MemoryStream();
            SharpAESCrypt.SharpAESCrypt.Encrypt(password, input,outputStream);
            return outputStream.ToArray();
        }

        public byte[] Decrypt(byte[] input, string password)
        {
            Stream inputStream = new MemoryStream(input);
            var outputStream = new MemoryStream();
            SharpAESCrypt.SharpAESCrypt.Decrypt(password, inputStream, outputStream);
            return outputStream.ToArray();
        }

    }
}
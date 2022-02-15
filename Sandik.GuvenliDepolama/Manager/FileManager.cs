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
        /// <summary>
        /// disaridan algidi stream dosyasini yine disaridan aldigi password key i kullanarak sifreliyor
        /// </summary>
        /// <param name="input"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] Encrypt(Stream input, string password)
        {
            //Stream inputStream = new MemoryStream(input);
            var outputStream = new MemoryStream();
            SharpAESCrypt.SharpAESCrypt.Encrypt(password, input,outputStream);
            return outputStream.ToArray();
        }

        /// <summary>
        /// disaridan algidi byte dizisini yine disaridan aldigi password key i kullanarak desifre ediyor
        /// </summary>
        /// <param name="input"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] input, string password)
        {
            Stream inputStream = new MemoryStream(input);
            var outputStream = new MemoryStream();
            SharpAESCrypt.SharpAESCrypt.Decrypt(password, inputStream, outputStream);
            return outputStream.ToArray();
        }

    }
}
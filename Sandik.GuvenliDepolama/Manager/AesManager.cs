using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using SharpAESCrypt;

namespace Sandik.GuvenliDepolama.Manager
{
    public class AesManager
    {
        private static readonly int iterations = 1000;

        public static string Encrypt(string input, string password)
        {
            try
            {
                byte[] encrypted;
                byte[] IV;
                byte[] Salt = GetSalt();
                byte[] Key = CreateKey(password, Salt);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = 256;
                    aesAlg.BlockSize = 128;
                    aesAlg.Key = Key;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.Mode = CipherMode.CBC;

                    aesAlg.GenerateIV();
                    IV = Encoding.UTF8.GetBytes(ByteArrayToString(aesAlg.IV));

                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(input);
                            }

                            encrypted = Encoding.UTF8.GetBytes(Convert.ToBase64String(msEncrypt.ToArray()));
                        }
                    }
                }

                byte[] combinedIvSaltCt = new byte[Salt.Length + IV.Length + encrypted.Length];
                Array.Copy(Salt, 0, combinedIvSaltCt, 0, Salt.Length);
                Array.Copy(IV, 0, combinedIvSaltCt, Salt.Length, IV.Length);
                Array.Copy(encrypted, 0, combinedIvSaltCt, Salt.Length + IV.Length, encrypted.Length);

                return Encoding.UTF8.GetString(combinedIvSaltCt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Decrypt(string input, string password)
        {
            byte[] inputAsByteArray;
            string plaintext = null;
            try
            {
                inputAsByteArray = Encoding.UTF8.GetBytes(input);

                byte[] Salt = new byte[64];
                byte[] IV = new byte[32];
                byte[] Encoded = new byte[inputAsByteArray.Length - Salt.Length - IV.Length];

                Array.Copy(inputAsByteArray, 0, Salt, 0, Salt.Length);
                Array.Copy(inputAsByteArray, Salt.Length, IV, 0, IV.Length);
                Array.Copy(inputAsByteArray, Salt.Length + IV.Length, Encoded, 0, Encoded.Length);

                byte[] Key = CreateKey(password, Salt);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = HexStringToByteArray(Encoding.UTF8.GetString(IV));
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(Encoding.UTF8.GetString(Encoded))))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] CreateKey(string password, byte[] salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, HexStringToByteArray(Encoding.UTF8.GetString(salt)), iterations))
                return rfc2898DeriveBytes.GetBytes(32);
        }

        private static byte[] GetSalt()
        {
            var salt = new byte[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return Encoding.UTF8.GetBytes(ByteArrayToString(salt));
        }

        public static byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }
}
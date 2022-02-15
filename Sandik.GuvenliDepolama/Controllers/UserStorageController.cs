using Sandik.GuvenliDepolama.Manager;
using Sandik.GuvenliDepolama.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sandik.GuvenliDepolama.Controllers
{
    public class UserStorageController : Controller
    {
        // GET: UserStorage
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        ///  dosya yukleme kismi
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase File)
        {
            try
            {
                // kullanici bilgileri session dan cekiliyor
                #region Kullanıcı Bilgileri
                var user = (User)Session["User"];
                #endregion


                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;

                        #region Dosya Şifreleme
                        FileManager fm = new FileManager();
                        var dataEncrypt = fm.Encrypt(stream, user.LicenseID);
                        //var dataEncrypt = AesManager.Encrypt(ToByteArray(stream), user.LicenseID);                        
                        #endregion

                        // and optionally write the file to disk
                        var fileNameOrginal = Path.GetFileName(fileContent.FileName);                        
                        var fileNameGuid = $"{Guid.NewGuid()}{Path.GetExtension(fileContent.FileName)}";


                        var filePathDirection = Setting.UploadFilePath.IndexOf(":\\") >= 0 ? Setting.UploadFilePath : Server.MapPath($"~/{Setting.UploadFilePath}");
                        var path = Path.Combine(filePathDirection, fileNameGuid);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            Stream dataStream = new MemoryStream(dataEncrypt);
                            dataStream.CopyTo(fileStream);

                            #region Dosya bilgisi dbye yazılıyor
                            UserFile uf = new UserFile {
                                UserID = user.ID,
                                FileNameGuid = fileNameGuid,
                                FileNameOrjinal = fileNameOrginal,
                                FilePath = path,
                                FileSizeByte = fileContent.ContentLength
                            };

                            SqlManager sql = new SqlManager();
                            sql.SetUserFile(uf);
                            #endregion
                        }
                        #region TEST
                        //var dataDecrypt = AesManager.Decrypt(dataEncrypt, user.LicenseID);
                        //var path1 = Path.Combine(Server.MapPath($"~/{Setting.UploadFilePath}"), fileNameOrginal);
                        //using (var fileStream = System.IO.File.Create(path1))
                        //{
                        //    Stream dataStream1 = new MemoryStream(dataDecrypt);
                        //    dataStream1.CopyTo(fileStream);
                        //}
                        #endregion

                    }
                }
                //PythoneEncript();
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        /// <summary>
        /// dosya indirme kismi
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string fileName)
        {
            var user = (User)Session["User"];
            SqlManager sql = new SqlManager();
            var file = sql.GetUserFile(user.ID, fileName).FirstOrDefault();

            var fileByte =  FileToByteArray(file.FilePath);

            //var fileDecrypt = AesManager.Decrypt(fileByte, user.LicenseID);
            FileManager fm = new FileManager();
            var fileDecrypt = fm.Decrypt(fileByte, user.LicenseID);

            return File(fileDecrypt, Path.GetExtension(file.FilePath), file.FileNameOrjinal);
        }

        /// <summary>
        /// dosyayi silme kismi
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteFile(string fileID)
        {
            var isOK = true;
            var user = (User)Session["User"];
            SqlManager sql = new SqlManager();
            var file = sql.GetUserFile(user.ID, fileID).FirstOrDefault();

            //Dosya var mı kontrolü
            if (System.IO.File.Exists(file.FilePath))
            {
                //Fiziksel Dosya silme
                System.IO.File.Delete(file.FilePath);
                //Sql dosya silme                
                UserFile uf = new UserFile
                {
                    ID = file.ID,
                    IsDeleted = 1,
                    UserID = user.ID                   
                };
                sql.SetUserFileDelete(uf);
            }

            return Json(new { isOk = isOK }, JsonRequestBehavior.AllowGet);
        }


        public static byte[] ToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// burada dosyayi indirmek icin dogru formata aliyoruz
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        //public void PythoneEncript()
        //{
        //    try
        //    {
        //        string progToRun = $" python \"{Server.MapPath("~/Python/test.py")}\"";
        //        Process cmd = new Process();
        //        cmd.StartInfo.FileName = "cmd.exe";
        //        cmd.StartInfo.RedirectStandardInput = true;
        //        cmd.StartInfo.RedirectStandardOutput = true;
        //        cmd.StartInfo.CreateNoWindow = true;
        //        cmd.StartInfo.UseShellExecute = false;
        //        cmd.Start();
        //        cmd.StandardInput.WriteLine(progToRun);
        //        cmd.StandardInput.Flush();
        //        cmd.StandardInput.Close();
        //        cmd.WaitForExit();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }
}
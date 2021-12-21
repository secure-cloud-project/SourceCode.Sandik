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

        [HttpPost]
        public async Task<JsonResult> UploadFile(string id)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileName = Path.GetFileName(fileContent.FileName);
                        var path = Path.Combine(Server.MapPath($"~/{Setting.UploadFilePath}"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {                           
                            stream.CopyTo(fileStream);
                        }
                    }
                }
                PythoneEncript();
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        public void PythoneEncript()
        {
            try
            {
                string progToRun = $" python \"{Server.MapPath("~/Python/test.py")}\"";
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine(progToRun);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FivePSocialNetwork.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            List<string> Img = new List<string>();
            if (Request.Files.Count > 0)
            {
                var files = Request.Files;

                //iterating through multiple file collection   
                foreach (string str in files)
                {
                    HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string path = "";
                        //  string strExtexsion = Path.GetFileName(new Guid().ToString().Substring(6)).Trim();
                        var fileName = Guid.NewGuid().ToString().Substring(6) + file.FileName;
                        path = Server.MapPath("~/Content/Img/" + fileName);
                        try
                        {
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            file.SaveAs(path);
                        }
                        catch { }
                        path = "/Content/Img/" + fileName;
                        Img.Add(path);

                    }

                }
                Session["Img"] = Img;
                return Json(new { Img }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
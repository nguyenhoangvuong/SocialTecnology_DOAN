using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        // GET: Admin/HomeAdmin
        public ActionResult IndexAdmin()
        {
            return View();
        }
        public ActionResult LogoutAdmin()
        {
            HttpCookie cookie = new HttpCookie("admin_id");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            return Redirect("/Center/IndexCenter");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class DenounceAdminController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Admin/DenounceAdmin
        public ActionResult Denounce()
        {
            return View();
        }
        public JsonResult DenounceJson()
        {
            List<Denounce_User> denounce_Users = db.Denounce_User.Where(n => n.denounceUser_recycleBin == false).ToList();
            List<DenounceAdmin> denounceAdmins = denounce_Users.Select(n => new DenounceAdmin
            {
                denounceUser_id = n.denounceUser_id,
                accuser_id = n.accuser_id,
                nameAccuser = n.User.user_firstName + n.User.user_lastName,
                discredit_id = n.discredit_id,
                namediscredit = n.User1.user_firstName + n.User1.user_lastName,
                denounceUser_content = n.denounceUser_content,
                denounceUser_dateCreate = n.denounceUser_dateCreate.ToString(),
                denounceUser_recycleBin = n.denounceUser_recycleBin,
                denounce_viewStatus = n.denounce_viewStatus,
                denounce_status = n.denounce_status,

            }).OrderByDescending(n => n.denounceUser_dateCreate).ToList();
            return Json(denounceAdmins, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DenounceViewStatus(int? id)
        {
            Denounce_User denounce_User = db.Denounce_User.Find(id);
            denounce_User.denounce_viewStatus = true;
            db.SaveChanges();
            return View(denounce_User);
        }
        public ActionResult ViewStatus(int? id)
        {
            db.Denounce_User.Find(id).denounce_viewStatus = false;
            db.SaveChanges();
            return RedirectToAction("Denounce");
        }
        public ActionResult OpenAccount(int? id)
        {
            db.Users.Find(id).user_activate = true;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult LockAccount(int? user_id, int? denounce_id)
        {
            db.Users.Find(user_id).user_activate = false;
            db.Denounce_User.Find(denounce_id).denounce_status = true;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult DeleteDenounce(int? id)
        {
            db.Denounce_User.Remove(db.Denounce_User.Find(id));
            db.SaveChanges();
            return RedirectToAction("Denounce");
        }
    }
}
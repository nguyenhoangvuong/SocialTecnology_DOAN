using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class PostsController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Admin/Posts
        public ActionResult Index()
        {
            return View();
        }

        //xóa tạm thời 
        public JsonResult RecycleBinPost(int? id)
        {
            Post provincial = db.Posts.Find(id);
            provincial.post_admin_recycleBin = !provincial.post_admin_recycleBin;
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecycleBinPostJson()
        {
            List<Post> questions = db.Posts.Where(x => x.post_admin_recycleBin == true).ToList();
            List<ListPosts> listQuestions = questions.Select(n => new ListPosts
            {
                post_id = n.post_id,
                user_firstName = db.Users.Find(n.user_id).user_firstName,
                user_lastName = db.Users.Find(n.user_id).user_lastName,
                post_title = n.post_title.ToString(),
                post_totalLike = n.post_totalLike,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }

        // sửa tỉnh
        [HttpPost, ValidateInput(false)]
        public ActionResult EditPost(int post_id, string post_content
            , string post_title)
        {
            Post provincial = db.Posts.Find(post_id);
            provincial.post_content = post_content;
            provincial.post_title = post_title;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }

        public JsonResult PostJson()
        {
            List<Post> questions = db.Posts.Where(x => x.post_admin_recycleBin == false).ToList();
            List<ListPosts> listQuestions = questions.Select(n => new ListPosts
            {
                post_id = n.post_id,
                user_firstName = db.Users.Find(n.user_id).user_firstName,
                user_lastName = db.Users.Find(n.user_id).user_lastName,
                post_title = n.post_title.ToString(),
                post_totalLike = n.post_totalLike,
                post_content = n.post_content,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
    }
}
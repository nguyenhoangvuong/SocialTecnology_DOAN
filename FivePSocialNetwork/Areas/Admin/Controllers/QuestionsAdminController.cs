using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class QuestionsAdminController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Admin/QuestionsAdmin
        public ActionResult Index()
        {
            return View();
        }

        //xóa tạm thời 
        public JsonResult RecycleBinQuestion(int? id)
        {
            Question provincial = db.Questions.Find(id);
            provincial.question_admin_recycleBin = !provincial.question_admin_recycleBin;
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecycleBinQuestionJson()
        {
            List<Question> questions = db.Questions.Where(x=>x.question_admin_recycleBin == true).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = db.Users.Find(n.user_id).user_firstName,
                user_lastName = db.Users.Find(n.user_id).user_lastName,
                question_title = n.question_title.ToString(),
                question_popular = n.question_popular,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }

        // sửa tỉnh
        [HttpPost, ValidateInput(false)]
        public ActionResult EditQuestion(string question_title,
            int question_id, string question_content, int question_popular)
        {
            Question provincial = db.Questions.Find(question_id);
            provincial.question_title = question_title;
            provincial.question_content = question_content;
            provincial.question_popular = question_popular;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }

        public JsonResult QuestionJson()
        {
            List<Question> questions = db.Questions.Where(x=>x.question_admin_recycleBin == false).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = db.Users.Find(n.user_id).user_firstName,
                user_lastName = db.Users.Find(n.user_id).user_lastName,
                question_title = n.question_title.ToString(),
                question_content = n.question_content.ToString(),
                question_popular=n.question_popular,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UserView()
        {
            return View();
        }
        public JsonResult UserViewJson()
        {
            List<View_Question> view_Questions = db.View_Question.ToList();
            List<ListViewQuestion> listView_Posts = view_Questions.Select(n => new ListViewQuestion
            {
                viewQuestion_id = n.viewQuestion_id,
                question_id = n.question_id,
                question_title = n.Question.question_title,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_id = n.user_id,
            }).ToList();
            return Json(listView_Posts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Tick()
        {
            return View();
        }
        public JsonResult TickJson()
        {
            List<Tick_Question> tick_Questions = db.Tick_Question.ToList();
            List<ListTick> listTicks = tick_Questions.Select(n => new ListTick
            {
                question_id = n.question_id,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                question_title = n.Question.question_title,
            }).ToList();
            return Json(listTicks, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowActivate()
        {
            return View();
        }
        public JsonResult ShowActivateJson()
        {
            List<Question> questions = db.Questions.ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                question_title = n.question_title.ToString(),
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Rate()
        {
            return View();
        }
        public JsonResult RateJson()
        {
            List<Question> questions = db.Questions.ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                question_title = n.question_title.ToString(),
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Answer()
        {
            return View();
        }
        public JsonResult AnswerJson()
        {
            List<Question> questions = db.Questions.ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                question_title = n.question_title.ToString(),
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Comment()
        {
            return View();
        }
        public JsonResult CommentJson()
        {
            List<Question> questions = db.Questions.ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                question_title = n.question_title.ToString(),
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RateAnswer()
        {
            return View();
        }
        public JsonResult RateAnswerJson()
        {
            List<Question> questions = db.Questions.ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                question_title = n.question_title.ToString(),
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
    }
}
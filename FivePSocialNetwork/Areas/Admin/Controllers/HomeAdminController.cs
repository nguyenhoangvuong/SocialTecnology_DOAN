using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();

        //hiển thị json
        public JsonResult GetListUserTotal()
        {
            string query = "select day(u.user_dateCreate) as Month, count(*) as Total from [User] u where month(u.user_dateCreate) = month(GETDATE()) and year(u.user_dateCreate) = year(GETDATE()) group by day(u.user_dateCreate)";
            var list = db.Database.SqlQuery<UserTotal>(query).ToList();
            var date = DateTime.Now;
            var totalDay = DateTime.DaysInMonth(date.Year, date.Month);
            int[] arrayResult = new int[totalDay];
            int[] arrayResult2 = new int[totalDay];
            int[] arrayResult3 = new int[totalDay];
            string query2 = "select day(u.user_dateLogin) as Month, count(*) as Total from [User] u where month(u.user_dateLogin) = month(GETDATE()) and year(u.user_dateLogin) = year(GETDATE()) group by day(u.user_dateLogin)";
            var list2 = db.Database.SqlQuery<UserTotal>(query2).ToList();

            string query3 = "select day(u.question_dateCreate) as Month, count(*) as Total from [Question] u where month(u.question_dateCreate) = month(GETDATE()) and year(u.question_dateCreate) = year(GETDATE()) group by day(u.question_dateCreate)";
            var list3 = db.Database.SqlQuery<UserTotal>(query3).ToList();
            foreach (var item in list)
            {
                arrayResult[item.Month-1] = item.Total;
            }
            foreach (var item in list2)
            {
                arrayResult2[item.Month - 1] = item.Total;
            }
            foreach (var item in list3)
            {
                arrayResult3[item.Month - 1] = item.Total;
            }
            return Json(new { data = arrayResult, data2 = arrayResult2, data3 = arrayResult3 }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/HomeAdmin
        public ActionResult IndexAdmin()
        {
            DateTime dateNow = DateTime.Now;
            ViewBag.Circle1 = db.Questions
                .Where(x=> x.question_dateCreate.Value.Year == dateNow.Year
                && x.question_dateCreate.Value.Month == dateNow.Month).Count();
            ViewBag.Circle2 = db.Answers
                .Where(x=> x.answer_dateCreate.Value.Year == dateNow.Year
                && x.answer_dateCreate.Value.Month == dateNow.Month).Count();
            ViewBag.Circle3 = db.Rate_Question
                .Where(x => x.rateQuestion_dateCreate.Value.Year == dateNow.Year
                && x.rateQuestion_dateCreate.Value.Month == dateNow.Month).Count();
            ViewBag.Circle4 = db.Posts
                .Where(x => x.post_dateCreate.Value.Year == dateNow.Year
                && x.post_dateCreate.Value.Month == dateNow.Month).Count();
            ViewBag.Circle5 = db.Rate_Post
                .Where(x => x.ratePost_dateCreate.Value.Year == dateNow.Year
                && x.ratePost_dateCreate.Value.Month == dateNow.Month).Count();
            ViewBag.Circle6 = db.Posts
                .Where(x => x.post_dateCreate.Value.Year == dateNow.Year
                && x.post_dateCreate.Value.Month == dateNow.Month).Sum(x=>x.post_view);
            if (ViewBag.Circle6 == null)
            {
                ViewBag.Circle6 = 1;
            }
            // get list top technology
            var listTechnology = db.Technologies
                .OrderByDescending(x => x.technology_popular)
                .ThenByDescending(x => x.technology_name).ToList();
            ViewBag.ListTechnology = listTechnology;
            // get list top user
            var listTopUser = db.Users
                .OrderByDescending(x=>x.user_popular)
                .ThenByDescending(x=>x.user_lastName).ToList();
            ViewBag.listUserTop = listTopUser;
            // get list top question
            ViewBag.listQuestionTop = db.Questions
                .OrderByDescending(x => x.question_popular).ToList();
            // git list top answer
            var query = "select * from [User] us left join (select a.user_id, count(*) as 'TotalCorrect' from [User] u join [Answer] a on u.user_id = a.user_id where a.answer_correct = 1 group by a.user_id) T on us.user_id = T.user_id order by TotalCorrect desc";
            var listAnswerTop = db.Database.SqlQuery<User>(query).ToList();
            ViewBag.listAnswerTop = listAnswerTop;
            // git list top post
            ViewBag.listPostTop = db.Posts
                .OrderByDescending(x => x.post_totalLike).ToList();
            ViewBag.SoNguoiTruyCap = HttpContext.Application["Online"].ToString();
            return View();
        }

        [HttpGet]
        public ActionResult _viewDetailTechnology(long technologyId)
        {
            var technology = db.Technologies
                .Where(x => x.technology_id == technologyId).FirstOrDefault();
            return View(technology);
        }

        [HttpGet]
        public ActionResult _viewDetailUser(long userId)
        {
            var user = db.Users.Where(x => x.user_id == userId).FirstOrDefault();
            return View(user);
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
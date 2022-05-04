using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers
{
    public class ViewController : Controller
    {
        // GET: View
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        //Tất cả thành viên đang hoạt động trên 5p
        public ActionResult Member()
        {
            return View();
        }
        // danh sách Menber
        public JsonResult ListMenber()
        {
            List<User> users = db.Users.Where(n => n.user_activate == true && n.user_recycleBin == false && n.role_id == 1).ToList();
            List<ListUsers> listUsers = users.Select(n => new ListUsers
            {
                user_id = n.user_id,
                user_popular = n.user_popular,
                user_firstName = n.user_firstName,
                user_lastName = n.user_lastName,
                user_email = n.user_email,
                user_avatar = n.user_avatar,
                user_goldMedal = n.user_goldMedal,
                user_silverMedal = n.user_silverMedal,
                user_brozeMedal = n.user_brozeMedal,
                user_vipMedal = n.user_vipMedal,
                total_answer = db.Answers.Where(m=>m.user_id == n.user_id).ToList().Count(),
                total_Question = db.Questions.Where(m => m.user_id == n.user_id).ToList().Count()
            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }
        //Tất cả công nghệ đang hoạt động trên 5P   
        public ActionResult Technology()
        {
            return View();
        }
        //------------------------------------------------ danh sách công nghệ------------------------------------------------
        public JsonResult ListTechnology()
        {
            List<Technology> technologies = db.Technologies.Where(n => n.technology_activate == true && n.technology_recycleBin == false).ToList();
            List<ListTechnology> listTechnologies = technologies.Select(n => new ListTechnology
            {
                technology_id = n.technology_id,
                technology_name = n.technology_name,
                technology_popular = n.technology_popular,
                technology_dateEdit = n.technology_dateEdit.Value.ToShortDateString(),
                technology_note = n.technology_note,
                technology_totalQuestion = n.technology_totalQuestion,
            }).ToList();
            return Json(listTechnologies, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Post()
        {
            return View();
        }
        // ------------------------------------------------danh sách bài viết------------------------------------------------
        public JsonResult ListPost()
        {
            List<Teachnology_Question> teachnology_Questions = db.Teachnology_Question.Where(n => n.teachnologyQuestion_recycleBin == false).ToList();
            List<ListTechnologyQuestion> listTechnologyQuestions = teachnology_Questions.Select(n => new ListTechnologyQuestion
            {
                teachnologyQuestion_id = n.teachnologyQuestion_id,
                technology_id = n.technology_id,
                question_id = n.question_id,
                teachnologyQuestion_recycleBin = n.teachnologyQuestion_recycleBin,
                technology_name = n.Technology.technology_name,
            }).ToList();
            return Json(listTechnologyQuestions, JsonRequestBehavior.AllowGet);
        }
    }
}
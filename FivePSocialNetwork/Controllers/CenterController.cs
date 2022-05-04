using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers
{
    public class CenterController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Center
        public ActionResult IndexCenter()
        {

            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_dateCreate).Take(7).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();

            return View(listQuestions);
        }
        //ds câu hỏi
        public JsonResult QuestionsJson()
        {
            List<Question> questions = db.Questions.Where(n => (n.question_recycleBin == false && n.question_userStatus == true) || (n.question_activate == true && n.question_admin_recycleBin == false) ).ToList();
            

            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        //---------------------------------------------- danh sách công nghệ------------------------------------
        public JsonResult ListTechnologyPopular()
        {
            List<Technology> technologies = db.Technologies.Where(n => n.technology_activate == true && n.technology_recycleBin == false).OrderByDescending(n=>n.technology_popular).Take(12).ToList();
            List<ListTechnology> listTechnologies = technologies.Select(n => new ListTechnology
            {
                technology_id = n.technology_id,
                technology_name = n.technology_name,
                technology_popular = n.technology_popular,
                technology_note = n.technology_note,
                technology_totalQuestion = n.technology_totalQuestion,
            }).ToList();
            return Json(listTechnologies, JsonRequestBehavior.AllowGet);
        }
        //----------------------------------------- danh sách câu hỏi gần đây---------------------------------
        public JsonResult ListQuestionNew()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus ==true && n.question_recycleBin == false).OrderByDescending(n => n.question_dateCreate).Take(7).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        //----------------------------------------- danh sách câu hỏi của bạn bè---------------------------------
        public JsonResult ListQuestionOfFriend()
        {
            if (Request.Cookies["user_id"] != null)
            {
                // khi tồn tại cookies
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<ListQuestions> listQuestionOfFriend = new List<ListQuestions>();
                List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_dateCreate).ToList();
                List<Friend> friends = db.Friends.Where(n => (n.userRequest_id == user_id || n.userResponse_id == user_id) && n.friend_status == true && n.friend_recycleBin == false).ToList();
                foreach(var item in questions)
                {
                    foreach(var item2 in friends)
                    {
                        if(item2.userRequest_id != user_id)
                        {
                            if(item.user_id == item2.userRequest_id)
                            {
                                listQuestionOfFriend.Add(new ListQuestions()
                                {
                                    question_id = item.question_id,
                                    question_content = item.question_content,
                                    user_id = item.user_id,
                                    question_dateCreate = item.question_dateCreate.Value.ToShortDateString(),
                                    question_dateEdit = item.question_dateEdit.Value.ToShortDateString(),
                                    user_firstName = item.User.user_firstName,
                                    user_lastName = item.User.user_lastName,
                                    user_popular = item.User.user_popular,
                                    user_silverMedal = item.User.user_silverMedal,
                                    user_goldMedal = item.User.user_goldMedal,
                                    user_brozeMedal = item.User.user_brozeMedal,
                                    user_vipMedal = item.User.user_vipMedal,
                                    question_title = item.question_title,
                                    question_Answer = item.question_Answer,
                                    question_totalComment = item.question_totalComment,
                                    question_view = item.question_view,
                                    question_totalRate = item.question_totalRate,
                                    question_medalCalculator = item.question_medalCalculator,
                                    question_recycleBin = item.question_recycleBin,
                                    question_userStatus = item.question_userStatus,
                                    question_popular = item.question_popular,
                                    question_admin_recycleBin = item.question_admin_recycleBin,
                                    question_keywordSearch = item.question_keywordSearch,
                                    user_avatar = item.User.user_avatar,
                                });
                            }
                        }
                        else if(item2.userResponse_id != user_id)
                        {
                            if (item.user_id == item2.userResponse_id)
                            {
                                listQuestionOfFriend.Add(new ListQuestions()
                                {
                                    question_id = item.question_id,
                                    question_content = item.question_content,
                                    user_id = item.user_id,
                                    question_dateCreate = item.question_dateCreate.Value.ToShortDateString(),
                                    question_dateEdit = item.question_dateEdit.Value.ToShortDateString(),
                                    user_firstName = item.User.user_firstName,
                                    user_lastName = item.User.user_lastName,
                                    user_popular = item.User.user_popular,
                                    user_silverMedal = item.User.user_silverMedal,
                                    user_goldMedal = item.User.user_goldMedal,
                                    user_brozeMedal = item.User.user_brozeMedal,
                                    user_vipMedal = item.User.user_vipMedal,
                                    question_title = item.question_title,
                                    question_Answer = item.question_Answer,
                                    question_totalComment = item.question_totalComment,
                                    question_view = item.question_view,
                                    question_totalRate = item.question_totalRate,
                                    question_medalCalculator = item.question_medalCalculator,
                                    question_recycleBin = item.question_recycleBin,
                                    question_userStatus = item.question_userStatus,
                                    question_popular = item.question_popular,
                                    question_admin_recycleBin = item.question_admin_recycleBin,
                                    question_keywordSearch = item.question_keywordSearch,
                                    user_avatar = item.User.user_avatar,
                                });
                            }
                        }
                    }
                }
                return Json(listQuestionOfFriend.OrderByDescending(n=>n.question_dateCreate).Take(7), JsonRequestBehavior.AllowGet);
            }
            return Json("ko có cookie", JsonRequestBehavior.AllowGet);
        }
        //----------------------------------------- danh sách câu hỏi trả lời nhiều---------------------------------
        public JsonResult ListQuestion_ManyAnswer()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_Answer).Take(7).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        //----------------------------------------- danh sách câu hỏi Vote cao---------------------------------
        public JsonResult ListQuestionVote()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_totalRate).Take(7).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        //----------------------------------------- danh sách câu hỏi nhiều điểm thưởng---------------------------------
        public JsonResult ListQuestionReward()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_popular).Take(7).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        //-----------------------------------------Các câu hỏi dành cho User đã đăng nhập và có công nghệ riêng---------------------------------
        public ActionResult ForUser()
        {
            return View();
        }
        public JsonResult ForUserJson()
        {
            //giải pháp lấy danh sách công nghệ của user / sau đó lấy danh sách công nghệ của câu hỏi / sau đó so sánh và id công nghệ giống thì thêm vào ds và thoát khỏi vòng lặp và so sánh vòng tiếp theo
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            List<ListQuestions> listQuestions1 = new List<ListQuestions>();
            List<Teachnology_User> teachnology_Users = db.Teachnology_User.Where(n => n.user_id == user_id && n.technology_recycleBin == false).ToList();
            List<Teachnology_Question> teachnology_Questions = db.Teachnology_Question.Where(n => n.teachnologyQuestion_recycleBin == false).ToList();
            foreach(var item in teachnology_Questions)
            {
                foreach(var item2 in teachnology_Users)
                {
                    if(item.technology_id == item2.technology_id)
                    {
                        listQuestions1.Add(new ListQuestions
                        {
                            question_id = (int)item.question_id,
                            question_content = item.Question.question_content,
                            user_id = item.Question.user_id,
                            question_dateCreate = item.Question.question_dateCreate.Value.ToShortDateString(),
                            question_dateEdit = item.Question.question_dateEdit.Value.ToShortDateString(),
                            user_firstName = item.Question.User.user_firstName,
                            user_lastName = item.Question.User.user_lastName,
                            user_popular = item.Question.User.user_popular,
                            user_silverMedal = item.Question.User.user_silverMedal,
                            user_goldMedal = item.Question.User.user_goldMedal,
                            user_brozeMedal = item.Question.User.user_brozeMedal,
                            user_vipMedal = item.Question.User.user_vipMedal,
                            question_title = item.Question.question_title,
                            question_Answer = item.Question.question_Answer,
                            question_totalComment = item.Question.question_totalComment,
                            question_view = item.Question.question_view,
                            question_totalRate = item.Question.question_totalRate,
                            question_medalCalculator = item.Question.question_medalCalculator,
                            question_recycleBin = item.Question.question_recycleBin,
                            question_userStatus = item.Question.question_userStatus,
                            question_popular = item.Question.question_popular,
                            question_admin_recycleBin = item.Question.question_admin_recycleBin,
                            question_keywordSearch = item.Question.question_keywordSearch,
                            user_avatar = item.Question.User.user_avatar,
                        });
                        break;
                    }
                }
            }
            return Json(listQuestions1.GroupBy(n => n.question_id).Select(n => n.FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }
        //-----------------------------------------Tìm kiếm---------------------------------
        public ActionResult Search(string search)
        {
            TempData["search"] = search;
            return View();
        }
        public JsonResult SearchJson()
        {
            string search = TempData["search"].ToString();
            List<Question> questions = db.Questions.Where(n => n.question_userStatus == true && n.question_activate == true && n.question_admin_recycleBin == false && n.question_recycleBin == false && n.question_keywordSearch.Contains(search)).OrderByDescending(n=>n.question_view).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        //-----------------------------------------Hiển thị theo công nghệ---------------------------------
        public ActionResult ShowFollowTechnology(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        public JsonResult ShowFollowTechnologyJson(int? id)
        {
            List<Teachnology_Question> teachnology_Questions = db.Teachnology_Question.Where(n => n.technology_id == id && n.teachnologyQuestion_recycleBin == false).ToList();
            List<ListQuestions> listQuestions = teachnology_Questions.Select(n => new ListQuestions
            {
                question_id = (int)n.question_id,
                question_content = n.Question.question_content,
                user_id = n.Question.user_id,
                question_dateCreate = n.Question.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.Question.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.Question.User.user_firstName,
                user_lastName = n.Question.User.user_lastName,
                user_popular = n.Question.User.user_popular,
                user_silverMedal = n.Question.User.user_silverMedal,
                user_goldMedal = n.Question.User.user_goldMedal,
                user_brozeMedal = n.Question.User.user_brozeMedal,
                user_vipMedal = n.Question.User.user_vipMedal,
                question_title = n.Question.question_title,
                question_Answer = n.Question.question_Answer,
                question_totalComment = n.Question.question_totalComment,
                question_view = n.Question.question_view,
                question_totalRate = n.Question.question_totalRate,
                question_medalCalculator = n.Question.question_medalCalculator,
                question_recycleBin = n.Question.question_recycleBin,
                question_userStatus = n.Question.question_userStatus,
                question_popular = n.Question.question_popular,
                question_admin_recycleBin = n.Question.question_admin_recycleBin,
                question_keywordSearch = n.Question.question_keywordSearch,
                user_avatar = n.Question.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------Phần sắp xếp theo các ưu điểm---------------------------------
        // Sắp xếp theo điểm thưởng.
        public ActionResult FilterReward()
        {
            return View();
        }
        public JsonResult FilterRewardJson()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_popular).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        // Sắp xếp theo câu hỏi mới nhất
        public ActionResult FilterQuestionNew()
        {
            return View();
        }
        public JsonResult FilterQuestionNewJson()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_dateCreate).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        // Sắp xếp theo câu hỏi vote cao tới thấp.
        public ActionResult FilterQuestionVote()
        {
            return View();
        }
        public JsonResult FilterQuestionVoteJson()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_totalRate).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        // Sắp xếp câu hỏi theo lượt xem cao tới thấp.
        public ActionResult FilterQuestionView()
        {
            return View();
        }
        public JsonResult FilterQuestionViewJson()
        {
            List<Question> questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_userStatus == true && n.question_recycleBin == false).OrderByDescending(n => n.question_view).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                user_id = n.user_id,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_silverMedal = n.User.user_silverMedal,
                user_goldMedal = n.User.user_goldMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_totalComment = n.question_totalComment,
                question_view = n.question_view,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_recycleBin = n.question_recycleBin,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                question_admin_recycleBin = n.question_admin_recycleBin,
                question_keywordSearch = n.question_keywordSearch,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
    }
}
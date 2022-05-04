using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers
{
    public class FunctionAtDetailQuestionController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: FunctionAtDetailQuestion
        public ActionResult DetailQuestion(int? id ,View_Question view_Question)
        {
            if(id == null)
            {
                return Redirect("/Home/Index");
            }
            //Neu61 ton62 tai5 cookei
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                View_Question check = db.View_Question.SingleOrDefault(n => n.question_id == id && n.user_id == user_id);
                if (check != null && (check.viewQuestion_dateCreate.Value.Hour != DateTime.Now.Hour || check.viewQuestion_dateCreate.Value.Day != DateTime.Now.Day || check.viewQuestion_dateCreate.Value.Year != DateTime.Now.Year))
                {
                    // Lưu bảng View_Question
                    db.View_Question.Find(check.viewQuestion_id).viewQuestion_dateCreate = DateTime.Now;
                    // lưu bảng question
                    db.Questions.Find(id).question_view += 1;
                    db.Questions.Find(id).question_popular += 1;
                    //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                    List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == id && n.answer_medalCalculate > 3).ToList();
                    if (checkAnswerMedal != null)
                    {
                        foreach (var item in checkAnswerMedal)
                        {
                            db.Users.Find(item.user_id).user_popular += 1;
                        }
                    }
                    db.SaveChanges();
                }
                else if(check == null)
                {
                    //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                    List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == id && n.answer_medalCalculate > 3).ToList();
                    if (checkAnswerMedal != null)
                    {
                        foreach (var item in checkAnswerMedal)
                        {
                            db.Users.Find(item.user_id).user_popular += 1;
                        }
                    }
                    // lưu xem
                    db.Questions.Find(id).question_view += 1;
                    db.Questions.Find(id).question_popular += 1;
                    // lu7 bảng view
                    view_Question.viewQuestion_dateCreate = DateTime.Now;
                    view_Question.question_id = id;
                    view_Question.user_id = user_id;
                    db.View_Question.Add(view_Question);
                    db.SaveChanges();
                }
            }
            Question question = db.Questions.SingleOrDefault(n => n.question_id == id && n.question_activate == true && n.question_recycleBin == false && n.question_admin_recycleBin == false);
            if (question == null)
            {
                return Redirect("/Home/Index");
            }
            else if(question.question_userStatus == true)
            {
                return View(question);
            }
            else if(question.question_userStatus == false && question.user_id == int.Parse(Request.Cookies["user_id"].Value.ToString()))
            {
                return View(question);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }
        //--------------------------------------------Tùy chọn cho bài viết----------------------------------
        //Đồi trường userStatur trong question == true
        public ActionResult QuestionStaturT(int? id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Question question = db.Questions.FirstOrDefault(n => n.question_id == id && n.user_id == user_id);
            if(question != null)
            {
                question.question_userStatus = true;
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult QuestionStaturF(int? id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Question question = db.Questions.FirstOrDefault(n => n.question_id == id && n.user_id == user_id);
            if (question != null)
            {
                question.question_userStatus = false;
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult recycleBinQueston(int? id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Question question = db.Questions.FirstOrDefault(n => n.question_id == id && n.user_id == user_id);
            if (question != null)
            {
                question.question_recycleBin = true;
                db.SaveChanges();
            }
            return Redirect("/UserManagement/PageUser");
        }
        //--------------------------------------------Danh giá questions true----------------------------------
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RateQuestionT(Rate_Question rate_Question,int? question_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Center/IndexCenter");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Rate_Question checkRate_Question = db.Rate_Question.Where(n => n.question_id == question_id && n.user_id == user_id).SingleOrDefault();
            Question question = db.Questions.Find(question_id);
            if (checkRate_Question == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                question.question_popular++;
                question.question_medalCalculator++;
                question.question_totalRate++;
                db.SaveChanges();
                //tính huy chương đưa vào user
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 4)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 8)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 15)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                }
                else if (postCalulateMedal == 30)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal++;
                }
                //Lưu đánh giá của bài viết
                rate_Question.user_id = user_id;
                rate_Question.rateQuestion_rateStatus = true;
                rate_Question.rateQuestion_dateCreate = DateTime.Now;
                db.Rate_Question.Add(rate_Question);
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else if (checkRate_Question.rateQuestion_rateStatus == true)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                question.question_medalCalculator--;
                question.question_totalRate--;
                question.question_popular--;

                db.SaveChanges();
                //tính huy chương đưa vào user
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 3)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 7)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 14)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                }
                else if (postCalulateMedal == 29)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal--;
                }
                //Đánh giá
                db.Rate_Question.Find(checkRate_Question.rateQuestion_id).rateQuestion_rateStatus = null;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());

            }
            else if (checkRate_Question.rateQuestion_rateStatus == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                question.question_medalCalculator++;
                question.question_totalRate++;
                question.question_popular++;
                db.SaveChanges();
                //tính huy chương đưa vào user
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 4)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 8)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 15)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                }
                else if (postCalulateMedal == 30)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                }
                // lưu đánh giá
                db.Rate_Question.Find(checkRate_Question.rateQuestion_id).rateQuestion_rateStatus = true;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());

            }
            else
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 2;
                    }
                }

                question.question_medalCalculator+=2;
                question.question_totalRate+=2;
                question.question_popular+=2;
                db.SaveChanges();
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 4 || postCalulateMedal == 5)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 8 || postCalulateMedal == 9)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 15 || postCalulateMedal == 16)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                }
                else if (postCalulateMedal == 30 || postCalulateMedal == 31)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal++;
                }
                db.Rate_Question.Find(checkRate_Question.rateQuestion_id).rateQuestion_rateStatus = true;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());

            }
        }
        // đánh giá question false
        [HttpPost]
        public ActionResult RateQuestionF(Rate_Question rate_Question, int? question_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Center/IndexCenter");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Rate_Question checkRateQuestion = db.Rate_Question.Where(n => n.question_id == question_id && n.user_id == user_id).SingleOrDefault();
            Question question = db.Questions.Find(question_id);
            if (checkRateQuestion == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                //tính huy chương user
                question.question_medalCalculator--;
                question.question_totalRate--;
                question.question_popular--;
                db.SaveChanges();
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 3)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 7)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 14)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                }
                else if (postCalulateMedal == 29)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                }
                //Lưu đánh giá
                rate_Question.user_id = user_id;
                rate_Question.rateQuestion_rateStatus = false;
                rate_Question.rateQuestion_dateCreate = DateTime.Now;
                db.Rate_Question.Add(rate_Question);
                db.SaveChanges();
                return View();
            }
            else if (checkRateQuestion.rateQuestion_rateStatus == false)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                //Tính huy chương cho user
                question.question_medalCalculator++;
                question.question_totalRate++;
                question.question_popular++;
                db.SaveChanges();
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 4)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 8)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 15)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                }
                else if (postCalulateMedal == 30)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal++;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                }
                //Lưu đánh giá
                db.Rate_Question.Find(checkRateQuestion.rateQuestion_id).rateQuestion_rateStatus = null;
                db.SaveChanges();
                return View();
            }
            else if (checkRateQuestion.rateQuestion_rateStatus == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                //Lưu Huy chương user
                question.question_medalCalculator--;
                question.question_totalRate--;
                question.question_popular--;
                db.SaveChanges();
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 3)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 7)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 14)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                }
                else if (postCalulateMedal == 29)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                }
                //Lưu đánh giá
                db.Rate_Question.Find(checkRateQuestion.rateQuestion_id).rateQuestion_rateStatus = false;
                db.SaveChanges();
                return View();
            }
            else
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 2;
                    }
                }

                //tính huy chương user
                question.question_medalCalculator-=2;
                question.question_totalRate-=2;
                question.question_popular-=2;
                db.SaveChanges();
                var postCalulateMedal = db.Questions.Find(question_id).question_medalCalculator;
                if (postCalulateMedal == 3 || postCalulateMedal == 2)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal--;
                }
                else if (postCalulateMedal == 7 || postCalulateMedal == 6)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_brozeMedal++;
                }
                else if (postCalulateMedal == 14 || postCalulateMedal == 13)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_silverMedal++;
                }
                else if (postCalulateMedal == 29 || postCalulateMedal == 28)
                {
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_vipMedal--;
                    db.Users.Find(db.Questions.Find(question_id).user_id).user_goldMedal++;
                }
                //Luu đánh giá
                db.Rate_Question.Find(checkRateQuestion.rateQuestion_id).rateQuestion_rateStatus = false;
                db.SaveChanges();
                return View();
            }
        }
        //Đánh giấu bài viết tick post
        [HttpPost]
        public ActionResult TickQuestion(Tick_Question tick_Question, int? question_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Center/IndexCenter");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Tick_Question checkTickQuestion = db.Tick_Question.Where(n => n.question_id == question_id && n.user_id == user_id).FirstOrDefault();
            Question question = db.Questions.Find(question_id);
            if (checkTickQuestion == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                question.question_popular++;
                question.question_totalTick++;
                tick_Question.user_id = user_id;
                tick_Question.tickQuestion_recycleBin = false;
                tick_Question.tickQuestion_dateCreate = DateTime.Now;
                db.Tick_Question.Add(tick_Question);
                db.SaveChanges();
                return View();
            }
            else if(checkTickQuestion.tickQuestion_recycleBin == true)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                question.question_popular++;
                question.question_totalTick++;
                checkTickQuestion.tickQuestion_recycleBin = false;
                checkTickQuestion.tickQuestion_dateCreate = DateTime.Now;
                db.SaveChanges();
                return View();
            }
            else
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                question.question_popular--;
                question.question_totalTick--;
                db.Tick_Question.Remove(db.Tick_Question.Find(checkTickQuestion.tickQuestion_id));
                db.SaveChanges();
                return View();
            }
        }
        // show hoạt động cho bài viết
        [HttpPost]
        public ActionResult ShowActivateQuestion(Show_Activate_Question show_Activate_Question, int? question_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Center/IndexCenter");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Show_Activate_Question check = db.Show_Activate_Question.FirstOrDefault(n => n.question_id == question_id && n.user_id == user_id);
            
            if (check == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                db.Questions.Find(question_id).question_popular++;
                show_Activate_Question.user_id = user_id;
                show_Activate_Question.showActivateQ_dateCreate = DateTime.Now;
                db.Show_Activate_Question.Add(show_Activate_Question);
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                db.Questions.Find(question_id).question_popular--;
                db.Show_Activate_Question.Remove(db.Show_Activate_Question.Find(check.showActivateQ_id));
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }

        }
        public JsonResult TagsQuestionJson(int? id)
        {
            List<Tags_Question> tags_Questions = db.Tags_Question.Where(n => n.question_id == id).ToList();
            List<ListTags> listTags = tags_Questions.Select(n => new ListTags
            {
                tagsQuestion_id = n.tagsQuestion_id,
                question_id = n.question_id,
                tagsQuestion_name = n.tagsQuestion_name,
                tagsQuestion_dateCreate = n.tagsQuestion_dateCreate.Value.ToShortDateString(),
            }).ToList();
            return Json(listTags, JsonRequestBehavior.AllowGet);
        }
        // thẻ tags
        public JsonResult QuestionRelationship(int? id)
        {
            Teachnology_Question teachnology_Questions = db.Teachnology_Question.FirstOrDefault(n => n.question_id == id);

            List<Question> questions = db.Teachnology_Question.Where(n=>n.technology_id == teachnology_Questions.technology_id && n.Question.question_admin_recycleBin == false && n.Question.question_recycleBin == false && n.Question.question_userStatus == true).Select(n=>n.Question).ToList();
            List<ListQuestions> listTags = questions.Select(n => new ListQuestions
            {
                question_title = n.question_title,
                question_id = n.question_id,
                question_popular = n.question_popular
            }).Take(8).ToList();
            return Json(listTags, JsonRequestBehavior.AllowGet);
        }
    }
}
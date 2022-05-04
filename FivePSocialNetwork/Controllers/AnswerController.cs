using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers
{
    public class AnswerController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Answer
        public ActionResult PostAnswer(int? id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Answer answer = db.Answers.SingleOrDefault(n => n.answer_id == id && n.user_id == user_id);
            if(answer != null)
            {
                return View(answer);
            }
            return Redirect("/Center/IndexCenter");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditAnser(int? answer_id , string answer_content)
        {
            db.Answers.Find(answer_id).answer_content = answer_content;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult RecycleBinAnswer(int? id)
        {
            db.Answers.Find(id).answer_recycleBin = true;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult RecycleBinComment(int? id)
        {
            db.Comment_Answer.Find(id).commentAnswer_userStatus = false;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult PostAnswer([Bind(Include = "answer_id,answer_content,answer_dateCreate,answer_dateEdit,user_id,answer_activate,answer_userStatus,question_id,answer_totalRate,answer_medalCalculate,answer_recycleBin,answer_admin_recycleBin,answer_correct")] Answer answer,Notification notification)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Home/Index");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            Answer checkAnswer = db.Answers.FirstOrDefault(n => n.user_id == user_id && n.question_id == answer.question_id);
            if(checkAnswer != null)
            {
                Session["doneReply"] = "Đã trả lời, nếu có ý tưởng mới xin hay cập nhập vào câu trả lời trước!";
                return Redirect(Request.UrlReferrer.ToString());
            }
            //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
            List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 3).ToList();
            if (checkAnswerMedal != null)
            {
                foreach (var item in checkAnswerMedal)
                {
                    db.Users.Find(item.user_id).user_popular += 1;
                }
            }

            //Bài viết
            db.Questions.Find(answer.question_id).question_Answer++;
            db.Questions.Find(answer.question_id).question_popular++;
            //Thông báo 
            List<Show_Activate_Question> show_Activate_Questions = db.Show_Activate_Question.Where(n => n.question_id == answer.question_id).ToList();
            if(show_Activate_Questions != null)
            {
                foreach (var item in show_Activate_Questions)
                {
                    if (item.user_id != user_id)
                    {
                        notification.receiver_id = item.user_id;
                        notification.impactUser_id = user_id;
                        notification.notification_dateCreate = DateTime.Now;
                        notification.question_id = answer.question_id;
                        notification.notification_recycleBin = false;
                        notification.notification_content = user.user_firstName + user.user_lastName + " Đã trả lời bài viết " + db.Questions.Find(answer.question_id).question_title;
                        notification.notification_status = false;
                        db.Notifications.Add(notification);
                    }
                }
            }
            //tự động đánh dấu hoạt động cho người trả lời bài viết để dc thông báo!
            Show_Activate_Question checkUser = db.Show_Activate_Question.SingleOrDefault(n => n.user_id == user_id && n.question_id == answer.question_id);
            if(checkUser == null)
            {
                db.Show_Activate_Question.Add(new Show_Activate_Question
                {
                    showActivateQ_dateCreate = DateTime.Now,
                    user_id = user_id,
                    question_id = answer.question_id
                });
            }
            // thông báo ai trả lời bài viết cho người viết bài
            var idUserPost = db.Questions.Find(answer.question_id).user_id;
            if (idUserPost != user_id)
            {
                notification.receiver_id = idUserPost;
                notification.impactUser_id = user_id;
                notification.question_id = answer.question_id;
                notification.notification_recycleBin = false;
                notification.notification_dateCreate = DateTime.Now;
                notification.notification_content = user.user_firstName + user.user_lastName + " Đã trả lời bài viết " + db.Questions.Find(answer.question_id).question_title;
                notification.notification_status = false;
                db.Notifications.Add(notification);
            }
            // lưu bth
            answer.answer_correct = false;
            answer.answer_dateCreate = DateTime.Now;
            answer.answer_dateEdit = DateTime.Now;
            answer.user_id = user_id;
            answer.answer_activate = true;
            answer.answer_userStatus = true;
            answer.answer_totalRate = 0;
            answer.answer_medalCalculate = 0;
            answer.answer_recycleBin = false;
            answer.answer_admin_recycleBin = false;
            db.Answers.Add(answer);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        // danh sách đáp án
        public JsonResult AnswerJson(int? id)
        {
            List<Answer> answers = db.Answers.Where(n => (n.answer_activate == true && n.answer_admin_recycleBin== false)||(n.answer_activate == true && n.answer_userStatus == true && n.answer_recycleBin == false && n.answer_admin_recycleBin == false)).ToList();
            List<ListAnswer> listAnswers = answers.Select(n => new ListAnswer
            {
                answer_id = n.answer_id,
                answer_content = n.answer_content,
                question_id = n.question_id,
                answer_dateCreate = n.answer_dateCreate.Value.ToShortDateString(),
                answer_dateEdit = n.answer_dateEdit.Value.ToShortDateString(),
                user_id = n.user_id,
                answer_activate = n.answer_activate,
                answer_userStatus = n.answer_userStatus,
                answer_medalCalculate = n.answer_medalCalculate,
                answer_recycleBin = n.answer_recycleBin,
                answer_admin_recycleBin = n.answer_admin_recycleBin,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_goldMedal = n.User.user_goldMedal,
                user_silverMedal = n.User.user_silverMedal,
                user_vipMedal = n.User.user_vipMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_avatar = n.User.user_avatar,
                answer_correct = n.answer_correct

            }).ToList();
            return Json(listAnswers, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult CreateComment([Bind(Include = "commentAnswer_id,commentAnswer_content,commentAnswer_dateCreate,commentAnswer_dateEdit,user_id,answer_id,commentAnswer_recycleBin,commentAnswer_activate,commentAnswer_userStatus")] Comment_Answer comment_Answer, int? question_id, Notification notification)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Home/Index");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            //thông báo cho người hiển thị hoạt động
            List<Show_Activate_Question> show_Activate_Questions = db.Show_Activate_Question.Where(n => n.question_id == question_id).ToList();
            foreach (var item in show_Activate_Questions)
            {
                if (item.user_id != user_id)
                {
                    notification.receiver_id = item.user_id;
                    notification.impactUser_id = user_id;
                    notification.question_id = question_id;
                    notification.notification_recycleBin = false;
                    notification.notification_dateCreate = DateTime.Now;
                    notification.notification_content = user.user_firstName + user.user_lastName + " Đã comment bài viết " + db.Questions.Find(question_id).question_title;
                    notification.notification_status = false;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                }
            }
            //Thông báo cho ai là người đăng câu trả lời này
            var idUserPost = db.Answers.Find(comment_Answer.answer_id);
            if (idUserPost.user_id != user.user_id)
            {
                notification.receiver_id = idUserPost.user_id;
                notification.impactUser_id = user_id;
                notification.question_id = question_id;
                notification.notification_recycleBin = false;
                notification.notification_dateCreate = DateTime.Now;
                notification.notification_content = user.user_firstName + user.user_lastName + " Đã comment bài viết " + db.Questions.Find(question_id).question_title;
                notification.notification_status = false;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
            comment_Answer.commentAnswer_dateCreate = DateTime.Now;
            comment_Answer.commentAnswer_dateEdit = DateTime.Now;
            comment_Answer.user_id = user_id;
            comment_Answer.commentAnswer_recycleBin = false;
            comment_Answer.commentAnswer_activate = true;
            comment_Answer.commentAnswer_userStatus = true;
            // lưu tổng comment câu hỏi
            Question question = db.Questions.Find(question_id);
            question.question_totalComment += 1;
            question.question_popular += 1;
            //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
            List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == question_id && n.answer_medalCalculate > 3).ToList();
            if (checkAnswerMedal != null)
            {
                foreach (var item in checkAnswerMedal)
                {
                    db.Users.Find(item.user_id).user_popular += 1;
                    db.SaveChanges();
                }
            }
            db.Comment_Answer.Add(comment_Answer);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        public ActionResult RateAnswerT(Rate_Answer rate_Answer, int? answer_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Home/Index");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Rate_Answer checkRateAnswer = db.Rate_Answer.Where(n => n.answer_id == answer_id && n.user_id == user_id).SingleOrDefault();
            Answer answer = db.Answers.Find(answer_id);
            if (checkRateAnswer == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                //Lưu đánh giá huy chương
                answer.answer_medalCalculate++;
                answer.answer_totalRate++;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 4)
                {
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                    db.Users.Find(answer.user_id).user_popular += db.Questions.Find(answer.question_id).question_popular;

                }
                else if (replyPostCalculateMedal == 8)
                {
                    db.Users.Find(answer.user_id).user_silverMedal++;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 15)
                {
                    db.Users.Find(answer.user_id).user_goldMedal++;
                    db.Users.Find(answer.user_id).user_silverMedal--;
                }
                else if (replyPostCalculateMedal == 30)
                {
                    db.Users.Find(answer.user_id).user_vipMedal++;
                    db.Users.Find(answer.user_id).user_goldMedal--;
                }
                //lưu đánh giá bài trả lời
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular++;
                rate_Answer.user_id = user_id;
                rate_Answer.rateAnswer_rateStatus = true;
                rate_Answer.rateAnswer_dateCreate = DateTime.Now;
                db.Rate_Answer.Add(rate_Answer);
                db.SaveChanges();
                return View();
            }
            else if (checkRateAnswer.rateAnswer_rateStatus == true)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                //Lưu huy chương người dùng
                answer.answer_medalCalculate--;
                answer.answer_totalRate--;
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular--;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 3)
                {
                    db.Users.Find(answer.user_id).user_popular -= db.Questions.Find(answer.question_id).question_popular;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 7)
                {
                    db.Users.Find(answer.user_id).user_silverMedal--;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 14)
                {
                    db.Users.Find(answer.user_id).user_goldMedal--;
                    db.Users.Find(answer.user_id).user_silverMedal++;
                }
                else if (replyPostCalculateMedal == 29)
                {
                    db.Users.Find(answer.user_id).user_goldMedal++;
                    db.Users.Find(answer.user_id).user_vipMedal--;
                }
                //Lưu đánh giá trả lời bài viết
                
                db.Rate_Answer.Find(checkRateAnswer.rateAnswer_id).rateAnswer_rateStatus = null;
                db.SaveChanges();
                return View();
            }
            else if (checkRateAnswer.rateAnswer_rateStatus == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                //Lưu huy chương người dùng
                answer.answer_medalCalculate++;
                answer.answer_totalRate++;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 4)
                {
                    db.Users.Find(answer.user_id).user_popular += db.Questions.Find(answer.question_id).question_popular;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 8)
                {
                    db.Users.Find(answer.user_id).user_silverMedal++;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 15)
                {
                    db.Users.Find(answer.user_id).user_goldMedal++;
                    db.Users.Find(answer.user_id).user_silverMedal--;
                }
                else if (replyPostCalculateMedal == 30)
                {
                    db.Users.Find(answer.user_id).user_vipMedal++;
                    db.Users.Find(answer.user_id).user_goldMedal--;
                }
                //Lưu đánh giá bài viết
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular++;
                db.Rate_Answer.Find(checkRateAnswer.rateAnswer_id).rateAnswer_rateStatus = true;
                db.SaveChanges();
                return View();
            }
            else
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 2;
                    }
                }

                //Lưu huy chương
                answer.answer_medalCalculate += 2;
                answer.answer_totalRate+=2;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 4 || replyPostCalculateMedal == 5)
                {
                    db.Users.Find(answer.user_id).user_popular += db.Questions.Find(answer.question_id).question_popular + 1;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 8 || replyPostCalculateMedal == 9)
                {
                    db.Users.Find(answer.user_id).user_silverMedal++;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 15 || replyPostCalculateMedal == 16)
                {
                    db.Users.Find(answer.user_id).user_goldMedal++;
                    db.Users.Find(answer.user_id).user_silverMedal--;
                }
                else if (replyPostCalculateMedal == 30 || replyPostCalculateMedal == 31)
                {
                    db.Users.Find(answer.user_id).user_vipMedal++;
                }
                //Lưu đánh giá 
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular += 2;
                db.Rate_Answer.Find(checkRateAnswer.rateAnswer_id).rateAnswer_rateStatus = true;
                db.SaveChanges();
                return View();
            }
        }
        [HttpPost]
        public ActionResult RateAnswerF(Rate_Answer rate_Answer, int? answer_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Home/Index");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Rate_Answer check = db.Rate_Answer.Where(n => n.answer_id == answer_id && n.user_id == user_id).SingleOrDefault();
            Answer answer = db.Answers.Find(answer_id);
            if (check == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                //Lưu huy chương
                answer.answer_medalCalculate--;
                answer.answer_totalRate--;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 3)
                {
                    db.Users.Find(answer.user_id).user_popular -= db.Questions.Find(answer.question_id).question_popular;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 7)
                {
                    db.Users.Find(answer.user_id).user_silverMedal--;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 14)
                {
                    db.Users.Find(answer.user_id).user_goldMedal--;
                    db.Users.Find(answer.user_id).user_silverMedal++;
                }
                else if (replyPostCalculateMedal == 29)
                {
                    db.Users.Find(answer.user_id).user_vipMedal--;
                    db.Users.Find(answer.user_id).user_goldMedal++;
                }
                //Lưu đánh giá
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular--;
                rate_Answer.user_id = user_id;
                rate_Answer.rateAnswer_rateStatus = false;
                rate_Answer.rateAnswer_dateCreate = DateTime.Now;
                db.Rate_Answer.Add(rate_Answer);
                db.SaveChanges();
                return View();
            }
            else if (check.rateAnswer_rateStatus == false)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular += 1;
                    }
                }

                //Lưu huy chương
                answer.answer_medalCalculate++;
                answer.answer_totalRate++;
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular++;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 4)
                {
                    db.Users.Find(answer.user_id).user_popular += db.Questions.Find(answer.question_id).question_popular;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 8)
                {
                    db.Users.Find(answer.user_id).user_silverMedal++;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 15)
                {
                    db.Users.Find(answer.user_id).user_goldMedal++;
                    db.Users.Find(answer.user_id).user_silverMedal--;
                }
                else if (replyPostCalculateMedal == 30)
                {
                    db.Users.Find(answer.user_id).user_vipMedal++;
                    db.Users.Find(answer.user_id).user_goldMedal--;
                }
                //Lưu đánh giá
                
                db.Rate_Answer.Find(check.rateAnswer_id).rateAnswer_rateStatus = null;
                db.SaveChanges();
                return View();
            }
            else if (check.rateAnswer_rateStatus == null)
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 1;
                    }
                }

                //Lưu huy chương
                answer.answer_medalCalculate--;
                answer.answer_totalRate--;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 3)
                {
                    db.Users.Find(answer.user_id).user_popular -= db.Questions.Find(answer.question_id).question_popular;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 7)
                {
                    db.Users.Find(answer.user_id).user_silverMedal--;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 14)
                {
                    db.Users.Find(answer.user_id).user_goldMedal--;
                    db.Users.Find(answer.user_id).user_silverMedal++;
                }
                else if (replyPostCalculateMedal == 29)
                {
                    db.Users.Find(answer.user_id).user_vipMedal--;
                    db.Users.Find(answer.user_id).user_goldMedal++;
                }
                //Lưu đánh giá
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular--;
                db.Rate_Answer.Find(check.rateAnswer_id).rateAnswer_rateStatus = false;
                db.SaveChanges();
                return View();
            }
            else
            {
                //kiểm tra xem user trả lời có dc vote 4 chưa để tự động công điểm.
                List<Answer> checkAnswerMedal = db.Answers.Where(n => n.question_id == answer.question_id && n.answer_medalCalculate > 4).ToList();
                if (checkAnswerMedal != null)
                {
                    foreach (var item in checkAnswerMedal)
                    {
                        db.Users.Find(item.user_id).user_popular -= 2;
                    }
                }

                //Lưu huy chương
                answer.answer_medalCalculate -= 2;
                answer.answer_totalRate-=2;
                var idRepLyPost = answer_id;
                var idPost = db.Answers.Find(idRepLyPost).question_id.Value;
                db.Questions.Find(idPost).question_popular -= 2;
                db.SaveChanges();
                var replyPostCalculateMedal = answer.answer_medalCalculate;
                if (replyPostCalculateMedal == 3 || replyPostCalculateMedal == 2)
                {
                    db.Users.Find(answer.user_id).user_popular -= db.Questions.Find(answer.question_id).question_popular +1;
                    db.Users.Find(answer.user_id).user_brozeMedal--;
                }
                else if (replyPostCalculateMedal == 7 || replyPostCalculateMedal == 6)
                {
                    db.Users.Find(answer.user_id).user_silverMedal--;
                    db.Users.Find(answer.user_id).user_brozeMedal++;
                }
                else if (replyPostCalculateMedal == 14 || replyPostCalculateMedal == 13)
                {
                    db.Users.Find(answer.user_id).user_goldMedal--;
                    db.Users.Find(answer.user_id).user_silverMedal++;
                }
                else if (replyPostCalculateMedal == 29 || replyPostCalculateMedal == 28)
                {
                    db.Users.Find(answer.user_id).user_vipMedal--;
                    db.Users.Find(answer.user_id).user_goldMedal++;
                }
                //Lưu đánh giá
                
                db.Rate_Answer.Find(check.rateAnswer_id).rateAnswer_rateStatus = false;
                db.SaveChanges();
                return View();
            }
        }
        public ActionResult AnswerCorrect(int? answer_id, int? question_id)
        {
            Answer answer = db.Answers.Find(answer_id);
            User user = db.Users.Find(answer.user_id);
            Question question = db.Questions.Find(question_id);

            if (answer.answer_correct == true)
            {
                answer.answer_correct = false;
                user.user_vipMedal -= 1;
                user.user_popular -= 60;
                question.question_popular -= 30;
                db.SaveChanges();
            }
            else
            {
                Answer check = db.Answers.FirstOrDefault(n => n.question_id == question_id && n.answer_correct == true && n.answer_activate == true && n.answer_userStatus == true && n.answer_admin_recycleBin == false && n.answer_recycleBin == false);
                if (check == null)
                {
                    answer.answer_correct = true;
                    user.user_vipMedal += 1;
                    question.question_popular += 30;
                    user.user_popular += 60;
                    db.SaveChanges();
                }
                else
                {
                    //user bỏ dấu tick
                    db.Answers.Find(check.answer_id).answer_correct = false;
                    db.Users.Find(check.user_id).user_vipMedal -= 1;
                    db.Users.Find(check.user_id).user_popular -= 60;
                    //user đánh dấu tick
                    answer.answer_correct = true;
                    user.user_vipMedal += 1;
                    user.user_popular += 60;
                    db.SaveChanges();
                }
            }
            return View();
        }
    }
}
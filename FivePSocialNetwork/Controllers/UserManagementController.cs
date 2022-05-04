using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers
{
    public class UserManagementController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: UserManagement
        public ActionResult PageUser(int? id)
        {
            if(id!= null)
            {
                User user = db.Users.SingleOrDefault(n => n.user_id == id && n.user_activate == true && n.user_recycleBin == false);
                return View(user);
            }
            else
            {
                //nếu ko có cookies cho về trang tất cả câu hỏi.
                if (Request.Cookies["user_id"] == null)
                {
                    return Redirect("/Home/Index");
                }
                // khi tồn tại cookies
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                User user = db.Users.SingleOrDefault(n => n.user_id == user_id && n.user_activate == true && n.user_recycleBin == false);
                return View(user);
            }
            
        }
        public JsonResult ListQuestions_UserAnswer(int? user_id)
        {
            List<Question> questions = db.Answers.Where(n => n.answer_activate == true && n.answer_userStatus == true && n.answer_recycleBin == false && n.answer_admin_recycleBin == false && n.user_id == user_id).GroupBy(x => x.question_id).Select(y => y.FirstOrDefault()).Select(n=>n.Question).ToList();
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_id = n.user_id,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_view = n.question_view,
                question_totalComment = n.question_totalComment,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_goldMedal = n.User.user_goldMedal,
                user_silverMedal = n.User.user_silverMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddFriend([Bind(Include = "friend_id,userRequest_id,userResponse_id,friend_status,friend_dateRequest,friend_dateResponse,friend_dateUnfriend,friend_recycleBin,friend_follow,friend_follow2_Response")] Friend friend, Message message, Notification notification)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Home/Index");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Friend friend1 = db.Friends.FirstOrDefault(n => n.userRequest_id == user_id && n.userResponse_id == friend.userResponse_id);
            Friend friend2 = db.Friends.FirstOrDefault(n => n.userRequest_id == friend.userResponse_id && n.userResponse_id == user_id);
            Friend friend3 = db.Friends.FirstOrDefault(n => n.userRequest_id == user_id && n.userResponse_id == friend.userResponse_id && n.friend_status == null);
            Friend friend4 = db.Friends.FirstOrDefault(n => n.userRequest_id == friend.userResponse_id && n.userResponse_id == user_id && n.friend_status == true);
            Friend friend5 = db.Friends.FirstOrDefault(n => n.userRequest_id == friend.userResponse_id && n.userResponse_id == user_id && n.friend_status == null);

            if (friend5 != null)
            {
                User user = db.Users.Find(user_id);
                //thông báo
                notification.receiver_id = friend.userResponse_id;
                notification.impactUser_id = user_id;
                notification.question_id = 1;
                notification.notification_recycleBin = false;
                notification.notification_dateCreate = DateTime.Now;
                notification.notification_content = user.user_firstName + user.user_lastName + " muốn làm bạn với bạn.";
                notification.notification_status = false;
                db.Notifications.Add(notification);
                db.SaveChanges();

                // friend
                db.Friends.Find(friend5.friend_id).friend_id = friend5.friend_id;
                db.Friends.Find(friend5.friend_id).userRequest_id = user_id;
                db.Friends.Find(friend5.friend_id).userResponse_id = friend.userResponse_id;
                db.Friends.Find(friend5.friend_id).friend_status = false;
                db.Friends.Find(friend5.friend_id).friend_dateRequest = DateTime.Now;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            if (friend4 != null)
            {
                db.Friends.Find(friend4.friend_id).friend_status = null;
                db.Friends.Find(friend4.friend_id).friend_dateUnfriend = DateTime.Now;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else if (friend3 != null)
            {
                User user = db.Users.Find(user_id);
                //thông báo
                notification.receiver_id = friend.userResponse_id;
                notification.impactUser_id = user_id;
                notification.question_id = 1;
                notification.notification_recycleBin = false;
                notification.notification_dateCreate = DateTime.Now;
                notification.notification_content = user.user_firstName + user.user_lastName + " muốn làm bạn với bạn.";
                notification.notification_status = false;
                db.Notifications.Add(notification);
                db.SaveChanges();
                //fiend
                db.Friends.Find(friend3.friend_id).friend_status = false;
                db.Friends.Find(friend3.friend_id).friend_dateRequest = DateTime.Now;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else if (friend1 != null)
            {
                db.Friends.Find(friend1.friend_id).friend_status = null;
                db.Friends.Find(friend1.friend_id).friend_dateUnfriend = DateTime.Now;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else if (friend2 != null)
            {
                Message checkMessage = db.Messages.FirstOrDefault(n => (n.messageRecipients_id == friend2.userRequest_id && n.messageSender_id == friend2.userResponse_id)|| (n.messageRecipients_id == friend2.userResponse_id && n.messageSender_id == friend2.userRequest_id));
                //kiểm tra trong mess 2 user kết bạn này đã chào nhau chưa.
                if(checkMessage == null)
                {
                    //Thêm vào mess để ko bị lỗi nội dung. tostring().
                    message.message_content = " Chào !";
                    message.message_dateSend = DateTime.Now;
                    message.message_status = false;
                    message.message_recycleBin = false;
                    message.messageSender_id = friend2.userResponse_id;
                    message.messageRecipients_id = friend2.userRequest_id;
                    db.Messages.Add(message);
                    db.SaveChanges();
                    message.message_content = " Chào !";
                    message.message_dateSend = DateTime.Now;
                    message.message_recycleBin = false;
                    message.message_status = false;
                    message.messageSender_id = friend2.userRequest_id;
                    message.messageRecipients_id = friend2.userResponse_id;
                    db.Messages.Add(message);
                }
                //thông báo
                notification.receiver_id = friend2.userRequest_id;
                notification.impactUser_id = user_id;
                notification.question_id = 1;
                notification.notification_recycleBin = false;
                notification.notification_dateCreate = DateTime.Now;
                notification.notification_content ="Bạn và "+ friend2.User1.user_firstName + friend2.User1.user_lastName+" đã là bạn bè";
                notification.notification_status = false;
                db.Notifications.Add(notification);
                // lưu bạn bình thường
                db.Friends.Find(friend2.friend_id).friend_status = true;
                db.Friends.Find(friend2.friend_id).friend_follow = true;
                db.Friends.Find(friend2.friend_id).friend_follow2_Response = true;
                db.Friends.Find(friend2.friend_id).friend_dateResponse = DateTime.Now;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                User user = db.Users.Find(user_id);
                //thông báo
                notification.receiver_id = friend.userResponse_id;
                notification.impactUser_id = user_id;
                notification.question_id = 1;
                notification.notification_recycleBin = false;
                notification.notification_dateCreate = DateTime.Now;
                notification.notification_content = user.user_firstName + user.user_lastName + " muốn làm bạn với bạn.";
                notification.notification_status = false;
                db.Notifications.Add(notification);
                // lưu bạn
                friend.userRequest_id = user_id;
                friend.friend_status = false;
                friend.friend_dateRequest = DateTime.Now;
                friend.friend_recycleBin = false;
                db.Friends.Add(friend);
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
        // Chế độ theo dõi
        public ActionResult FriendFollow(int? id)
        {
            db.Friends.Find(id).friend_follow = !db.Friends.Find(id).friend_follow;
            db.SaveChanges();
            return View();
        }
        public ActionResult friend_follow2_Response(int? id)
        {
            db.Friends.Find(id).friend_follow2_Response = !db.Friends.Find(id).friend_follow2_Response;
            db.SaveChanges();
            return View();
        }

        //---------------------------------------------user Quản lý câu hỏi --------------------------------
        public JsonResult ListQuestions(int? user_id)
        {
            List<Question> questions = new List<Question>();
            if (Request.Cookies["user_id"] == null || (user_id != int.Parse(Request.Cookies["user_id"].Value.ToString())))
            {
                questions = db.Questions.Where(n => n.question_activate == true && n.question_userStatus == true && n.question_admin_recycleBin == false && n.question_recycleBin == false && n.user_id == user_id).ToList();
            }
            else if (user_id == int.Parse(Request.Cookies["user_id"].Value.ToString()))
            {
                questions = db.Questions.Where(n => n.question_activate == true && n.question_admin_recycleBin == false && n.question_recycleBin == false && n.user_id == user_id).ToList();
            }
            List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
            {
                question_id = n.question_id,
                question_content = n.question_content,
                question_dateCreate = n.question_dateCreate.Value.ToShortDateString(),
                question_dateEdit = n.question_dateEdit.Value.ToShortDateString(),
                user_id = n.user_id,
                question_title = n.question_title,
                question_Answer = n.question_Answer,
                question_view = n.question_view,
                question_totalComment = n.question_totalComment,
                question_totalRate = n.question_totalRate,
                question_medalCalculator = n.question_medalCalculator,
                question_userStatus = n.question_userStatus,
                question_popular = n.question_popular,
                user_firstName = n.User.user_firstName,
                user_lastName = n.User.user_lastName,
                user_popular = n.User.user_popular,
                user_goldMedal = n.User.user_goldMedal,
                user_silverMedal = n.User.user_silverMedal,
                user_brozeMedal = n.User.user_brozeMedal,
                user_vipMedal = n.User.user_vipMedal,
                user_avatar = n.User.user_avatar,
            }).ToList();
            return Json(listQuestions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListTechnologyQuestion()
        {
            List<Teachnology_Question> teachnology_Questions = db.Teachnology_Question.Where(n => n.teachnologyQuestion_recycleBin == false).ToList();
            List<ListTechnologyQuestion> listTechnologyQuestions = teachnology_Questions.Select(n => new ListTechnologyQuestion
            {
                teachnologyQuestion_id = n.teachnologyQuestion_id,
                technology_id = n.technology_id,
                question_id = n.question_id,
                teachnologyQuestion_recycleBin = n.teachnologyQuestion_recycleBin,
                technology_name = n.Technology.technology_name
            }).ToList();
            return Json(listTechnologyQuestions, JsonRequestBehavior.AllowGet);
        }

        //---------------------------------------------user quản lý bạn bè--------------------------------
        //Tất cả bạn bè
        public ActionResult ManagementFriend()
        {
            return View();
        }
        public JsonResult ListFriend()
        {
            //Kiểm tra cookie
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Friend> friends = db.Friends.Where(n => (n.userResponse_id == user_id || n.userRequest_id == user_id) && n.friend_status == true && n.friend_recycleBin == false).ToList();
                List<ListUsers> listUsers = new List<ListUsers>();
                foreach(var item in friends)
                {
                    if(item.userRequest_id == user_id)
                    {
                        listUsers.Add(new ListUsers
                        {
                            user_id = (int)item.userResponse_id,
                            user_firstName = item.User1.user_firstName,
                            user_lastName = item.User1.user_lastName,
                            user_avatar = item.User1.user_avatar,
                            user_popular = item.User1.user_popular
                        });
                    }
                    else if(item.userResponse_id == user_id)
                    {
                        listUsers.Add(new ListUsers
                        {
                            user_id = (int)item.userRequest_id,
                            user_firstName = item.User.user_firstName,
                            user_lastName = item.User.user_lastName,
                            user_avatar = item.User.user_avatar,
                            user_popular = item.User.user_popular
                        });
                    }
                }
                return Json(listUsers, JsonRequestBehavior.AllowGet);
            }

            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        //Bạn bè mới kết bạn gần đây
        public ActionResult FriendNew()
        {
            return View();
        }
        public JsonResult FriendNewJson()
        {
            //Kiểm tra cookie
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Friend> friends = db.Friends.Where(n => (n.userResponse_id == user_id || n.userRequest_id == user_id) && n.friend_status == true && n.friend_recycleBin == false).OrderByDescending(n=>n.friend_dateResponse).Take(18).ToList();
                List<ListUsers> listUsers = new List<ListUsers>();
                foreach (var item in friends)
                {
                    if (item.userRequest_id == user_id)
                    {
                        listUsers.Add(new ListUsers
                        {
                            user_id = (int)item.userResponse_id,
                            user_firstName = item.User1.user_firstName,
                            user_lastName = item.User1.user_lastName,
                            user_avatar = item.User1.user_avatar,
                            user_popular = item.User1.user_popular
                        });
                    }
                    else if (item.userResponse_id == user_id)
                    {
                        listUsers.Add(new ListUsers
                        {
                            user_id = (int)item.userRequest_id,
                            user_firstName = item.User.user_firstName,
                            user_lastName = item.User.user_lastName,
                            user_avatar = item.User.user_avatar,
                            user_popular = item.User.user_popular
                        });
                    }
                }
                return Json(listUsers, JsonRequestBehavior.AllowGet);
            }

            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        //Lời mời kết bạn
        public ActionResult FriendInvitation()
        {
            return View();
        }
        public JsonResult FriendInvitationJson()
        {
            //Kiểm tra cookie
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Friend> friends = db.Friends.Where(n => n.userResponse_id == user_id && n.friend_status == false && n.friend_recycleBin == false).ToList();
                List<ListUsers> listUsers = new List<ListUsers>();
                foreach (var item in friends)
                {
                    listUsers.Add(new ListUsers
                    {
                        user_id = (int)item.userRequest_id,
                        user_firstName = item.User.user_firstName,
                        user_lastName = item.User.user_lastName,
                        user_avatar = item.User.user_avatar,
                        user_popular = item.User.user_popular
                    });
                }
                return Json(listUsers, JsonRequestBehavior.AllowGet);
            }

            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        //Lời kết bạn gửi đi
        public ActionResult SendFriend()
        {
            return View();
        }
        public JsonResult SendFriendJson()
        {
            //Kiểm tra cookie
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Friend> friends = db.Friends.Where(n => n.userRequest_id == user_id && n.friend_status == false && n.friend_recycleBin == false).ToList();
                List<ListUsers> listUsers = new List<ListUsers>();
                foreach (var item in friends)
                {
                    listUsers.Add(new ListUsers
                    {
                        user_id = (int)item.userResponse_id,
                        user_firstName = item.User1.user_firstName,
                        user_lastName = item.User1.user_lastName,
                        user_avatar = item.User1.user_avatar,
                        user_popular = item.User1.user_popular
                    });
                }
                return Json(listUsers, JsonRequestBehavior.AllowGet);
            }

            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        //----------------------------------------Công nghệ user--------------------------------
        public JsonResult ListTechnologyUser()
        {
            List<Teachnology_User> teachnology_Users = db.Teachnology_User.Where(n => n.technology_recycleBin == false).ToList();
            List<ListtechnologyUsers> listtechnologyUsers = teachnology_Users.Select(n => new ListtechnologyUsers
            {
                technologyUser_id = n.technologyUser_id,
                user_id = n.user_id,
                technology_id = n.technology_id,
                technology_name = n.Technology.technology_name
            }).ToList();
            return Json(listtechnologyUsers, JsonRequestBehavior.AllowGet);
        }
        //---------------------------------------------user quản lý các bài viết đã đánh dấu--------------------------------
        public ActionResult ManagementTick()
        {
            return View();
        }
        public JsonResult ListTick()
        {
            //Kiểm tra cookie
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Tick_Question> tick_Questions = db.Tick_Question.Where(n => n.tickQuestion_recycleBin == false && n.user_id == user_id).ToList();
                List<ListTick> listTicks = tick_Questions.Select(n => new ListTick
                {
                    tickQuestion_id = n.tickQuestion_id,
                    question_id = n.question_id,
                    tickQuestion_dateCreate = n.tickQuestion_dateCreate.Value.ToShortDateString(),
                    question_content = n.Question.question_content,
                    question_title = n.Question.question_title,
                    question_totalRate = n.Question.question_totalRate,
                    question_Answer = n.Question.question_Answer,
                    question_view = n.Question.question_view,
                    question_totalComment = n.Question.question_totalComment
                }).ToList();
                return Json(listTicks, JsonRequestBehavior.AllowGet);
            }
            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UnTick(int? id)
        {
            if(Request.Cookies["user_id"] != null)
            {
                db.Tick_Question.Find(id).tickQuestion_recycleBin = true;
                db.SaveChanges();
                return View();
            }
            return View();
        }
        //---------------------------------------------user quản lý các thông báo--------------------------------
        //hiển thị tất cả thông báo trong phần quản lý
        public ActionResult ManagementNotification()
        {
            return View();
        }
        //hiển thị thông báo trên chuông thông báo
        public JsonResult ListNotification()
        {
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Notification> notifications = db.Notifications.Where(n => n.notification_recycleBin == false && n.receiver_id == user_id).ToList();
                List<ListNotification> listNotifications = notifications.Select(n => new ListNotification
                {
                    notification_id = n.notification_id,
                    notification_content = n.notification_content,
                    receiver_id = n.receiver_id,
                    notification_dateCreate = n.notification_dateCreate.Value.ToShortDateString(),
                    impactUser_id = n.impactUser_id,
                    question_id = n.question_id,
                    notification_status = n.notification_status,
                    notification_recycleBin = n.notification_recycleBin,
                    impactUser_user_firstName = n.User1.user_firstName,
                    impactUser_user_lastName = n.User1.user_lastName,
                    impactUser_avatar = n.User.user_avatar,
                    question_title = n.Question.question_title
                }).ToList();
                return Json(listNotifications, JsonRequestBehavior.AllowGet);
            }
            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecycleBinNotification(int? id)
        {
            if (Request.Cookies["user_id"] != null)
            {
                db.Notifications.Find(id).notification_recycleBin = true;
                db.SaveChanges();
                return View();
            }
            return View();
        }
        //---------------------------------------------user quản lý các câu trả lời đã duyệt--------------------------------
        public ActionResult ManagementAcceptAnswer()
        {
            return View();
        }
        public JsonResult ListAcceptAnswer()
        {
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Answer> answers = db.Answers.Where(n => n.answer_admin_recycleBin == false && n.answer_recycleBin == false && n.answer_userStatus == true && n.answer_activate == true && n.Question.user_id == user_id && n.answer_correct == true).ToList();
                List<ListAnswer> listAnswers = answers.Select(n => new ListAnswer
                {
                    answer_id = n.answer_id,
                    answer_content = n.answer_content,
                    user_id = n.user_id,
                    question_id = n.question_id,
                    answer_dateCreate = n.answer_dateCreate.Value.ToShortDateString(),
                    answer_dateEdit = n.answer_dateEdit.Value.ToShortDateString(),
                    
                }).ToList();
                return Json(listAnswers, JsonRequestBehavior.AllowGet);
            }
            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        //---------------------------------------------user quản lý lịch sử xóa câu hỏi--------------------------------
        public ActionResult HistoryDeleteQuestion()
        {
            return View();
        }
        public JsonResult ListHistoryDeleteQuestion()
        {
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Question> questions = db.Questions.Where(n => n.question_admin_recycleBin == false && n.question_recycleBin == true && n.question_activate == true && n.user_id == user_id).ToList();
                List<ListQuestions> listQuestions = questions.Select(n => new ListQuestions
                {
                    question_id = n.question_id,
                    user_id = n.user_id,
                    question_title = n.question_title

                }).ToList();
                return Json(listQuestions, JsonRequestBehavior.AllowGet);
            }
            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecycleBinQuestion(int? id)
        {
            if (Request.Cookies["user_id"] != null)
            {
                db.Questions.Find(id).question_recycleBin = false;
                db.SaveChanges();
                return View();
            }
            return View();
        }
        //---------------------------------------------user quản lý lịch sử xóa câu trả lời--------------------------------
        public ActionResult HistoryDeleteAnswer()
        {
            return View();
        }
        public JsonResult ListHistoryDeleteAnswer()
        {
            if (Request.Cookies["user_id"] != null)
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                List<Answer> answers = db.Answers.Where(n => n.answer_admin_recycleBin == false && n.answer_recycleBin == true && n.answer_activate == true && n.user_id == user_id).ToList();
                List<ListAnswer> listAnswers = answers.Select(n => new ListAnswer
                {
                    answer_id = n.answer_id,
                    answer_content = n.answer_content,
                    user_id = n.user_id,
                    question_id = n.question_id

                }).ToList();
                return Json(listAnswers, JsonRequestBehavior.AllowGet);
            }
            return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecycleBinAnswer(int? id)
        {
            if (Request.Cookies["user_id"] != null)
            {
                db.Answers.Find(id).answer_recycleBin = false;
                db.SaveChanges();
                return View();
            }
            return View();
        }
        //---------------------------------------------user quản lý các bài viết--------------------------------
        public ActionResult ManagementPost()
        {
            return View();
        }
        public JsonResult ListPost()
        {
            List<User> users = db.Users.Where(n => n.user_activate == true && n.user_recycleBin == false && n.role_id == 1).ToList();
            List<ListUsers> listUsers = users.Select(n => new ListUsers
            {
                user_id = n.user_id,
                user_firstName = n.user_firstName,
                user_lastName = n.user_lastName,
                user_email = n.user_email,
                user_avatar = n.user_avatar,
                user_goldMedal = n.user_goldMedal,
                user_silverMedal = n.user_silverMedal,
                user_brozeMedal = n.user_brozeMedal,
                user_vipMedal = n.user_vipMedal,
                total_answer = db.Answers.Where(m => m.user_id == n.user_id).ToList().Count(),
                total_Question = db.Questions.Where(m => m.user_id == n.user_id).ToList().Count()
            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }
        //---------------------------------------------Tố cáo user--------------------------------
        [HttpPost]
        public ActionResult Denounce(Denounce_User denounce_User, string reasonWrite, string reasonOption, int discredit_id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            if ((reasonOption == "0" || reasonOption == "6") && reasonWrite != "")
            {
                denounce_User.denounceUser_content = reasonWrite;
            }
            else if(reasonOption == "1" && reasonWrite != "")
            {
                denounce_User.denounceUser_content = "Tài khoản giả mạo! ----" + reasonWrite;
            }
            else if (reasonOption == "2" && reasonWrite != "")
            {
                denounce_User.denounceUser_content = "Tài khoản của bạn đã bị hack! ----" + reasonWrite;

            }
            else if (reasonOption == "3" && reasonWrite != "")
            {
                denounce_User.denounceUser_content = "Đăng câu hỏi không liên quan hoặc bậy bạ! ----" + reasonWrite;

            }
            else if (reasonOption == "4" && reasonWrite != "")
            {
                denounce_User.denounceUser_content = "Xúc phạm danh tự của bạn! ----" + reasonWrite;

            }
            else if (reasonOption == "5" && reasonWrite != "")
            {
                denounce_User.denounceUser_content = "Cố ý đăng câu trả lời không đúng với câu hỏi! ----" + reasonWrite;

            }
            else if (reasonOption == "1" && reasonWrite == "")
            {
                denounce_User.denounceUser_content = "Tài khoản giả mạo! ----";
            }
            else if (reasonOption == "2" && reasonWrite == "")
            {
                denounce_User.denounceUser_content = "Tài khoản của bạn đã bị hack! ----";

            }
            else if (reasonOption == "3" && reasonWrite == "")
            {
                denounce_User.denounceUser_content = "Đăng câu hỏi không liên quan hoặc bậy bạ! ----";

            }
            else if (reasonOption == "4" && reasonWrite == "")
            {
                denounce_User.denounceUser_content = "Xúc phạm danh tự của bạn! ----";

            }
            else if (reasonOption == "5" && reasonWrite == "")
            {
                denounce_User.denounceUser_content = "Cố ý đăng câu trả lời không đúng với câu hỏi! ----";

            }
            denounce_User.accuser_id = user_id;
            denounce_User.discredit_id = discredit_id;
            denounce_User.denounceUser_dateCreate = DateTime.Now;
            denounce_User.denounceUser_recycleBin = false;
            denounce_User.denounce_viewStatus = false;
            denounce_User.denounce_status = false;
            db.Denounce_User.Add(denounce_User);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
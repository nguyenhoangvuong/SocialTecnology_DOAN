using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;
using FivePSocialNetwork.Hubs;

namespace FivePSocialNetwork.Controllers
{
    
    public class MessageController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Message
        //--------------------------------------TEST--------------------
        //--------------------------------------MAIN--------------------
        public ActionResult Index(int? id)
        {
            if(id == null)
            {
                return Redirect("/Center/IndexCenter");
            }

            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Center/IndexCenter");
            }
            TempData["id"] = id;
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Friend friend = db.Friends.SingleOrDefault(n => (n.userRequest_id == id && n.userResponse_id == user_id && n.friend_status == true && n.friend_recycleBin == false) || (n.userRequest_id == user_id && n.userResponse_id == id && n.friend_status == true && n.friend_recycleBin == false));
            if(friend == null)
            {
                return Redirect("/Center/IndexCenter");

            }
            ViewBag.idFriend = friend.friend_id;
            ViewBag.id = id;
            return View();
        }
        // TRạng thái hoạt động của friend
        public JsonResult StatusUser()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            user.user_statusOnline = !user.user_statusOnline;
            db.SaveChanges();
            return Json(null);
        }
        // Trạng thái haot5 động Friend
        public JsonResult StatusFriend(int? id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [user_id],[user_statusOnline],[user_SecurityAccount]FROM [dbo].[User]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(status_friend);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    var statusFriend = from item in db.Users
                               where item.user_id == id && item.user_activate == true
                               select new
                               {
                                   user_statusOnline = item.user_statusOnline
                               };
                    return Json(new { statusFriend = statusFriend }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void status_friend(object sender, SqlNotificationEventArgs e)
        {
            StatusFriendInMess.StatusFriend();
        }

        //-------------- message-------------
        public JsonResult GetMessage(int? id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [message_id],[message_content],[messageSender_id],[messageRecipients_id],[message_dateSend],[message_recycleBin],[message_status],[messageRecipients_status]FROM [dbo].[Message]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange_Mess);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //.Where(n => n.notification_recycleBin == false && n.receiver_id == user_id)
                    int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                    List<Message> messages = db.Messages.Where(n => ((n.messageSender_id == user_id && n.messageRecipients_id == id)||(n.messageSender_id == id && n.messageRecipients_id == user_id)) && n.message_recycleBin == false).OrderByDescending(n => n.message_dateSend).Take(11).ToList();
                    var listChat = from item in messages.OrderBy(n => n.message_dateSend)
                                   select new
                                   {
                                       message_content = item.message_content,
                                       messageRecipients_status = item.messageRecipients_status,
                                       messageSender_id = item.messageSender_id,
                                       messageRecipients_id = item.messageRecipients_id,
                                       message_dateSend = item.message_dateSend.ToString(),
                                       messageSender_avatar = item.User.user_avatar,
                                       messageSender_firstName = item.User.user_firstName,
                                       messageSender_lastName = item.User.user_lastName,
                                       message_id = item.message_id
                                   };
                    return Json(new { listChat = listChat }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependency_OnChange_Mess(object sender, SqlNotificationEventArgs e)
        {
            HubMess.Message();
        }
        [HttpPost]
        public ActionResult SaveMessage(Message message,string message_content, int messageRecipients_id)
        {
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect("/Center/IndexCenter");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            if (db.Users.Find(messageRecipients_id).user_statusOnline == true)
            {
                message.messageRecipients_status = "đã nhận.";

            }
            else
            {
                message.messageRecipients_status = "đã gửi.";
            }
            message.messageSender_id = user_id;
            message.messageRecipients_id = messageRecipients_id;
            message.message_content = message_content;
            message.message_dateSend = DateTime.Now;
            message.message_status = false;
            message.message_recycleBin = false;
            db.Messages.Add(message);
            db.SaveChanges();
            return View();
        }
        public JsonResult StatusMessage(int? messageRecipients_id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            List<Message> checkMessages = db.Messages.Where(n => (n.messageSender_id == messageRecipients_id && n.messageRecipients_id == user_id && n.message_status == false) || (n.messageSender_id == messageRecipients_id && n.messageRecipients_id == user_id && n.message_status == true && n.messageRecipients_status == "đã xem.")).OrderByDescending(n=>n.message_dateSend).ToList();
            int dk = 1;
            if(checkMessages.Count() > 1)
            {
                foreach (var item in checkMessages)
                {

                    if (dk == 0 && item.message_status == false)
                    {
                        item.message_status = true;
                        item.messageRecipients_status = " ";
                        db.SaveChanges();
                    }
                    else if (dk == 1 && item.message_status == false)
                    {
                        item.message_status = true;
                        item.messageRecipients_status = "đã xem.";
                        db.SaveChanges();
                        dk = 0;
                    }
                    else
                    {
                        item.messageRecipients_status = " ";
                        db.SaveChanges();
                    }
                }
            }
            return Json(null);
        }
        //-------------- danh sách bạn bè trong mess-------------
        public JsonResult ListFirend(int? id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [message_id],[message_content],[messageSender_id],[messageRecipients_id],[message_dateSend],[message_recycleBin],[message_status],[messageRecipients_status],[user_id],[user_pass],[user_firstName]
      ,[user_lastName],[user_email],[user_token],[role_id],[user_code],[user_avatar]
      ,[user_coverImage],[user_activate],[user_recycleBin],[user_dateCreate],[user_dateEdit],[user_dateLogin]
      ,[user_emailAuthentication],[user_verifyPhoneNumber],[user_loginAuthentication],[provincial_id],[district_id]
      ,[commune_id],[user_addressRemaining],[sex_id],[user_linkFacebook]
      ,[user_linkGithub],[user_anotherWeb],[user_hobbyWork],[user_hobby],[user_birthday],[user_popular],[user_goldMedal]
      ,[user_silverMedal],[user_brozeMedal],[user_vipMedal],[user_phone],[user_statusOnline],[user_SecurityAccount]FROM [dbo].[Message],[dbo].[User]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange_ListFiendInMess);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //Kiểm tra cookie
                    if (Request.Cookies["user_id"] != null)
                    {
                        int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                        //List<Friend> friends = db.Friends.Where(n => (n.userResponse_id == user_id || n.userRequest_id == user_id) && n.friend_recycleBin == false && n.friend_status == true).ToList();
                        var friends = from item in db.Friends
                                      where (item.userResponse_id == user_id || item.userRequest_id == user_id) && item.friend_recycleBin == false && item.friend_status == true
                                      select item;
                        List<ListUsers> fiterUsers = new List<ListUsers>();
                        foreach (var item in friends)
                        {
                            if (item.userRequest_id != user_id)
                            {
                                fiterUsers.Add(new ListUsers
                                {
                                    user_statusOnline = item.User.user_statusOnline,
                                    user_id = (int)item.userRequest_id,
                                    user_firstName = item.User.user_firstName,
                                    user_lastName = item.User.user_lastName,
                                    user_vipMedal = item.User.user_vipMedal,
                                    user_goldMedal = item.User.user_goldMedal,
                                    user_silverMedal = item.User.user_silverMedal,
                                    user_brozeMedal = item.User.user_brozeMedal,
                                    user_avatar = item.User.user_avatar
                                });
                            }
                            else if (item.userResponse_id != user_id)
                            {
                                fiterUsers.Add(new ListUsers
                                {
                                    user_statusOnline = item.User1.user_statusOnline,
                                    user_id = (int)item.userResponse_id,
                                    user_firstName = item.User1.user_firstName,
                                    user_lastName = item.User1.user_lastName,
                                    user_vipMedal = item.User1.user_vipMedal,
                                    user_goldMedal = item.User1.user_goldMedal,
                                    user_silverMedal = item.User1.user_silverMedal,
                                    user_brozeMedal = item.User1.user_brozeMedal,
                                    user_avatar = item.User1.user_avatar
                                });
                            }
                        }
                        List<ListUsers> listUsers = fiterUsers.Select(n => new ListUsers
                        {
                            user_id = n.user_id,
                            user_statusOnline = n.user_statusOnline,
                            user_firstName = n.user_firstName,
                            user_lastName = n.user_lastName,
                            user_avatar = n.user_avatar,
                            user_vipMedal = n.user_vipMedal,
                            user_goldMedal = n.user_goldMedal,
                            user_silverMedal = n.user_silverMedal,
                            user_brozeMedal = n.user_brozeMedal,
                            messageRecipients_status = db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => m.messageSender_id == user_id).messageRecipients_status,
                            message_status = db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => m.messageSender_id == n.user_id).message_status,
                            message = db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => (m.messageSender_id == n.user_id && m.messageRecipients_id == user_id)||(m.messageSender_id == user_id && m.messageRecipients_id == n.user_id)).message_content,
                            temporaryDate = (DateTime)db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => m.messageSender_id == n.user_id).message_dateSend
                        }).ToList();
                        DateTime dateTime = DateTime.Now;
                        foreach(var item in listUsers)
                        {
                            if(item.temporaryDate.ToShortDateString() == dateTime.ToShortDateString() && item.temporaryDate.Hour == dateTime.Hour)
                            {
                                item.hoursSend = dateTime.Minute - item.temporaryDate.Minute;
                                item.message_dateSend = item.temporaryDate.ToString();
                                db.SaveChanges();
                            }
                            else
                            {
                                item.hoursSend = 66;
                                item.message_dateSend = item.temporaryDate.ToString();
                                db.SaveChanges();
                            }
                        }
                        return Json(new { listChat = listUsers.OrderByDescending(n => n.temporaryDate) }, JsonRequestBehavior.AllowGet);
                    }
                    return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependency_OnChange_ListFiendInMess(object sender, SqlNotificationEventArgs e)
        {
            ListFriendInMess.ListFirend();
        }
        public ActionResult MessageRetrieval(int? id)
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Message checkMess = db.Messages.Find(id);
            if (checkMess.messageSender_id == user_id)
            {
                checkMess.message_recycleBin = !checkMess.message_status;
                db.SaveChanges();
            }
            return View();
        }
    }
}
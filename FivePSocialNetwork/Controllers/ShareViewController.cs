using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Hubs;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers
{
    public class ShareViewController : Controller
    {
        // GET: ShareView
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        //Menu của trang chủ index
        public PartialViewResult MenuIndex()
        {
            return PartialView();
        }
        //panel trên cùng của indexcenter
        public PartialViewResult PanelCenter()
        {
            return PartialView();
        }
        //-----------------------thông báo tin nhắn ---------------------
        public JsonResult GetMessage()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [message_id],[message_content],[messageSender_id],[messageRecipients_id],[message_dateSend],[message_recycleBin],[message_status]FROM [dbo].[Message]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                    //.OrderByDescending(n => n.message_dateSend)
                    List<Message> messages = db.Messages.Where(n => n.messageRecipients_id == user_id && n.message_recycleBin == false).OrderByDescending(n => n.message_dateSend).Take(16).ToList();
                    List<ListMessage> listChat = messages.Select(n => new ListMessage
                    {
                        message_content = n.message_content,
                        messageSender_id = n.messageSender_id,
                        messageRecipients_id = n.messageRecipients_id,
                        message_dateSend = n.message_dateSend.ToString(),
                        messageSender_avatar = n.User.user_avatar,
                        messageSender_firstName = n.User.user_firstName,
                        messageSender_lastName = n.User.user_lastName
                    }).ToList();
                    return Json(new { listChat = listChat }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        //-----------------------thông báo ----------------------------
        public JsonResult GetNotification()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [notification_id],[notification_content],[receiver_id],[impactUser_id],[question_id],[notification_status],[notification_dateCreate],[notification_recycleBin]FROM [dbo].[Notification]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange_notification);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //.Where(n => n.notification_recycleBin == false && n.receiver_id == user_id)
                    int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                    List<Notification> notifications = db.Notifications.Where(n=>n.receiver_id == user_id && n.notification_recycleBin == false).OrderByDescending(n => n.notification_dateCreate).Take(4).ToList();
                    List<ListNotification> listChat = notifications.Select(n => new ListNotification
                    {
                        notification_id = n.notification_id,
                        notification_content = n.notification_content,
                        impactUser_id = n.impactUser_id,
                        impactUser_avatar = n.User1.user_avatar,
                        impactUser_user_firstName = n.User1.user_firstName,
                        impactUser_user_lastName = n.User1.user_lastName,
                        notification_dateCreate = n.notification_dateCreate.ToString(),
                        question_id = n.question_id
                    }).ToList();
                    return Json(new { listChat = listChat }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependency_OnChange_notification(object sender, SqlNotificationEventArgs e)
        {
            DemoNotification.Notification();
        }

        //-----------------------đếm thông báo----------------------------
        public JsonResult CountNotification()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [notification_id],[notification_content],[receiver_id],[impactUser_id],[question_id],[notification_status],[notification_dateCreate],[notification_recycleBin]FROM [dbo].[Notification]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange_CountN);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    List<Notification> notificationsCount = db.Notifications.Where(n => n.notification_status == false && n.notification_recycleBin == false && n.receiver_id == user_id).ToList();
                    return Json(notificationsCount.Count(), JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependency_OnChange_CountN(object sender, SqlNotificationEventArgs e)
        {
            DemoCountNotification.CountNotification();
        }

        //-----------------------đếm tin nhắn----------------------------
        public JsonResult CountMessage()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [message_id],[message_content],[messageSender_id],[messageRecipients_id],[message_dateSend],[message_recycleBin],[message_status]FROM [dbo].[Message]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange_CountMess);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    List<Message> messagesCount = db.Messages.Where(n => n.messageRecipients_id == user_id && n.message_status == false && n.message_recycleBin == false).ToList();

                    return Json(messagesCount.Count(), JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependency_OnChange_CountMess(object sender, SqlNotificationEventArgs e)
        {
            DemoCountMess.CountMess();
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            DemoChat.Message();
        }
        //--------------------------đồi trạng thái tin nhắn ---------------------------
        public ActionResult StatusMessage()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            List<Message> checkMessages = db.Messages.Where(n => n.messageRecipients_id == user_id && n.message_status == false).OrderByDescending(n=>n.message_dateSend).ToList();
            int dk = 1;
            if (checkMessages != null)
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
            return View();
        }
        //--------------------------đồi trạng thái thông báo ---------------------------
        public ActionResult StatusNotification()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            List<Notification> checkNotification = db.Notifications.Where(n => n.receiver_id == user_id && n.notification_status == false && n.notification_recycleBin == false).ToList();
            if(checkNotification !=null)
            {
                foreach (var item in checkNotification)
                {
                    db.Notifications.Find(item.notification_id).notification_status = true;
                }
                db.SaveChanges();
            }
            return View();
        }
        //menu tùy chọn bên trái của center
        public PartialViewResult MenuCenter()
        {
            return PartialView();
        }
        //Thống kê của center

        public PartialViewResult SelectMuntiple()
        {
            return PartialView();
        }
        //----------------share thanh đặt câu hỏi/ đăng bài viết/ công nghệ phổ biến/ các lọc câu hỏi-----------------
        public PartialViewResult DefaultIndexCenter()
        {
            return PartialView();
        }
        //----------------------------------------------share Thanh bạn bè ------------------------------
        public PartialViewResult FriendIndexCenter()
        {
            return PartialView();
        }
        public PartialViewResult FriendIndexCenterRealTime()
        {
            return PartialView();
        }
        //-----------------------trạng thái online----------------------------
        public JsonResult StatusOnlineFriend()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [user_id],[user_pass],[user_firstName],[user_lastName],[user_email],[user_token]
      ,[role_id],[user_code],[user_avatar],[user_coverImage],[user_activate],[user_recycleBin],[user_dateCreate],[user_dateEdit]
      ,[user_dateLogin],[user_emailAuthentication],[user_verifyPhoneNumber],[user_loginAuthentication],[provincial_id]
      ,[district_id],[commune_id],[user_addressRemaining],[sex_id],[user_linkFacebook],[user_linkGithub],[user_anotherWeb],[user_hobbyWork],[user_hobby],[user_birthday],[user_popular],[user_goldMedal],[user_silverMedal],[user_brozeMedal]
      ,[user_vipMedal],[user_phone],[user_statusOnline],[user_SecurityAccount]FROM [dbo].[User] ", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange_statusOnlineFriend);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //Kiểm tra cookie
                    if (Request.Cookies["user_id"] != null)
                    {
                        int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                        User user = db.Users.Find(user_id);
                        List<Friend> friends = db.Friends.Where(n => (n.userResponse_id == user_id || n.userRequest_id == user_id) && n.friend_recycleBin == false && n.friend_status == true ).ToList();
                       
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
                            //message_status = db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => m.messageSender_id == n.user_id).message_status,
                            //message = db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => m.messageSender_id == n.user_id).message_content,
                            //message_dateSend = db.Messages.OrderByDescending(m => m.message_dateSend).FirstOrDefault(m => m.messageSender_id == n.user_id).message_dateSend.ToString()
                        }).OrderByDescending(n=>n.user_statusOnline == true).ToList();
                        return Json(new { listChat = listUsers }, JsonRequestBehavior.AllowGet);
                    }

                    return Json("Hello bạn !", JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependency_OnChange_statusOnlineFriend(object sender, SqlNotificationEventArgs e)
        {
            StatusOnline.StatusOnlineFriend();
        }

        //----------------------------------------------các đề xuất cho trang indexcenter ------------------------------
        public PartialViewResult OfferIndexCenter()
        {
            return PartialView();
        }
        // gợi ý kết bạn
        public JsonResult SuggestiotMakeFriends()
        {
            //giải pháp lấy danh sách công nghệ của user / sau đó lấy danh sách công nghệ của của các user khác / sau đó so sánh và id công nghệ giống thì thêm vào ds và thoát khỏi vòng lặp và so sánh vòng tiếp theo
            int user_id = 0;
            if (Request.Cookies["user_id"] != null)
            {
                user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            }
            else if (Request.Cookies["admin_id"] != null)
            {
                user_id = int.Parse(Request.Cookies["admin_id"].Value.ToString());
            }
            List<ListUsers> listUser = new List<ListUsers>();
            var user = db.Users.Find(user_id);
            List<Teachnology_User> teachnology_User = db.Teachnology_User.Where(n => n.user_id == user_id && n.technology_recycleBin == false).ToList();
            List<Teachnology_User> teachnology_Users = db.Teachnology_User.Where(n => n.technology_recycleBin == false && n.user_id != user_id).ToList();
            // lấy ds bạn bè
            List<Friend> friends = db.Friends.Where(n => (n.userResponse_id == user_id || n.userRequest_id == user_id) && n.friend_recycleBin == false && n.friend_status == true).ToList();
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
            List<ListUsers> listFriend = fiterUsers.Select(n => new ListUsers
            {
                user_id = n.user_id,
                user_statusOnline = n.user_statusOnline,
                user_firstName = n.user_firstName,
                user_lastName = n.user_lastName,
                user_vipMedal = n.user_vipMedal,
                user_goldMedal = n.user_goldMedal,
                user_silverMedal = n.user_silverMedal,
                user_brozeMedal = n.user_brozeMedal,
                user_avatar = n.user_avatar,
                provincial_id = n.provincial_id,
              
            }).ToList();
            // ^
            //lọc các user cùng công nghệ
            foreach (var item in teachnology_User)
            {
                foreach (var item2 in teachnology_Users)
                {
                    if (item.technology_id == item2.technology_id)
                    {
                        listUser.Add(new ListUsers
                        {
                            user_id = (int)item2.user_id,
                            user_statusOnline = item2.User.user_statusOnline,
                            user_firstName = item2.User.user_firstName,
                            user_lastName = item2.User.user_lastName,
                            user_vipMedal = item2.User.user_vipMedal,
                            user_goldMedal = item2.User.user_goldMedal,
                            user_silverMedal = item2.User.user_silverMedal,
                            user_brozeMedal = item2.User.user_brozeMedal,
                            user_avatar = item2.User.user_avatar,
                            provincial_id=item2?.User.provincial_id,
                            district_id = item2?.User.district_id,

                        });
                    }
                }
            }
            if (listUser.Count== 0)
            {
                foreach (var item2 in teachnology_Users)
                {
                     listUser.Add(new ListUsers
                        {
                            user_id = (int)item2.user_id,
                            user_statusOnline = item2.User.user_statusOnline,
                            user_firstName = item2.User.user_firstName,
                            user_lastName = item2.User.user_lastName,
                            user_vipMedal = item2.User.user_vipMedal,
                            user_goldMedal = item2.User.user_goldMedal,
                            user_silverMedal = item2.User.user_silverMedal,
                            user_brozeMedal = item2.User.user_brozeMedal,
                            user_avatar = item2.User.user_avatar,
                            provincial_id = item2?.User.provincial_id,
                            district_id = item2?.User.district_id,

                        });
                    
                }
            }
           
            List<ListUsers> listUsers = listUser.GroupBy(n => n.user_id).Select(n => n.FirstOrDefault()).ToList();
            //lọc các user ko phải bạn bè
            List<ListUsers> listSuggestiotMakeFriends = new List<ListUsers>();
            int temp = 1;
            foreach (var item in listUsers)
            {
                foreach (var item2 in listFriend)
                {
                    if(item.user_id == item2.user_id)
                    {
                        temp = 0;
                        break;
                    }
                    else
                    {
                        temp = 1;
                    }
                }
                if(temp == 1)
                {
                    listSuggestiotMakeFriends.Add(new ListUsers
                    {
                        user_id = (int)item.user_id,
                        user_statusOnline = item.user_statusOnline,
                        user_firstName = item.user_firstName,
                        user_lastName = item.user_lastName,
                        user_vipMedal = item.user_vipMedal,
                        user_goldMedal = item.user_goldMedal,
                        user_silverMedal = item.user_silverMedal,
                        user_brozeMedal = item.user_brozeMedal,
                        user_avatar = item.user_avatar,
                        provincial_id = item?.provincial_id,
                        district_id = item?.district_id,

                    });
                    temp = 1;
                }
            }
            if (user != null)
            {
                listSuggestiotMakeFriends = listSuggestiotMakeFriends.Where(x => x.provincial_id == user?.provincial_id && x.district_id == user?.district_id).ToList();
            }
            return Json(listSuggestiotMakeFriends, JsonRequestBehavior.AllowGet);
        }
    }
}
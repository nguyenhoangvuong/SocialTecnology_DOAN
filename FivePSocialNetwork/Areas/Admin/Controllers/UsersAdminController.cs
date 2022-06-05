using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models.Json;
using FivePSocialNetwork.Models;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class UsersAdminController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();

        // GET: Admin/UsersAdmin
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ActivateSex(int? id)
        {
            Sex_User technology = db.Sex_User.Find(id);
            technology.sex_activate = !technology.sex_activate;
            db.SaveChanges();
            return Json(technology, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Activate(int? id)
        {
            Role_User technology = db.Role_User.Find(id);
            technology.role_activate = !technology.role_activate;
            db.SaveChanges();
            return Json(technology, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProfileUser()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int user_id = int.Parse(Request.Cookies["admin_id"].Value.ToString());
            User user = db.Users.SingleOrDefault(n => n.user_id == user_id && n.user_activate == true && n.user_recycleBin == false);
            return View(user);
        }

        public ActionResult Security()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        //-------------------------------------------------Cài đặt thông tin cá nhân----------------------------------
        public ActionResult SettingAccount()
        {
            var cookieUser = Request.Cookies["admin_id"];
            if (cookieUser == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.cookieUser = Int32.Parse(cookieUser.Value);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SettingAccount(User user, string user_firstName, string user_lastName)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["admin_id"].Value.ToString());
            user = db.Users.Find(user_id);
            user.user_firstName = user_firstName;
            user.user_lastName = user_lastName;
            db.SaveChanges();
            return View(user);
        }

        [HttpGet]
        public ActionResult Detail(long user_id)
        {
            var user = db.Users.Find(user_id);
            return View(user);
        }

        public JsonResult UserJson()
        {
            List<User> users = db.Users.Where(x=>x.user_recycleBin == false).ToList();
            List<ListUsers> listUsers = users.Select(n => new ListUsers
            {
                user_id = n.user_id,
                user_pass = n.user_pass,
                user_firstName = n.user_firstName,
                user_lastName = n.user_lastName,
                user_email = n.user_email,
                role_id = n.role_id,
                user_code = n.user_code,
                user_avatar = n.user_avatar,
                user_coverImage = n.user_coverImage,
                user_activate = n.user_activate,
                user_statusOnline = n.user_statusOnline,
                user_recycleBin = n.user_recycleBin,
                user_dateCreate = n.user_dateCreate.ToString(),
                user_dateEdit = n.user_dateEdit.ToString(),
                user_dateLogin = n.user_dateLogin.ToString(),
                user_emailAuthentication = n.user_emailAuthentication,
                user_verifyPhoneNumber = n.user_verifyPhoneNumber,
                provincial_id = n.provincial_id,
                user_loginAuthentication = n.user_loginAuthentication,
                district_id = n.district_id,
                commune_id = n.commune_id,
                user_addressRemaining = n.user_addressRemaining,
                user_token = n.user_token

            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Role()
        {
            return View();
        }


        public JsonResult RoleJson()
        {
            List<Role_User> role_Users = db.Role_User.Where(n => n.role_recycleBin == false).ToList();
            List<RoleAdmin> listUsers = role_Users.Select(n => new RoleAdmin
            {
                role_id = n.role_id,
                role_name = n.role_name,
                role_activate = n.role_activate,
                role_dateCreate = n.role_dateCreate.ToString(),
                role_dateEdit = n.role_dateEdit.ToString(),
                role_recycleBin = n.role_recycleBin

            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Friend()
        {
            return View();
        }
        public JsonResult FriendJson()
        {
            List<Friend> friends = db.Friends.ToList();
            List<ListFriend> listFriends = friends.Select(n => new ListFriend
            {
                friend_id = n.friend_id,
                userRequest_id = n.userRequest_id,
                avatauserRequest = n.User.user_avatar,
                nameuserRequest = n.User.user_firstName +" "+ n.User.user_lastName,
                userResponse_id = n.userResponse_id,
                avatauserResponse = n.User1.user_avatar,
                nameuserResponse = n.User1.user_firstName + " "+ n.User1.user_lastName,
                friend_status = n.friend_status,
            }).ToList();
            return Json(listFriends, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Sex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditSex(string sex_name, int sex_id)
        {
            Sex_User provincial = db.Sex_User.Find(sex_id);
            provincial.sex_name = sex_name;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thành công";
            return RedirectToAction("Sex");
        }

        [HttpPost]
        public ActionResult EditRole(string role_name, int role_id)
        {
            Role_User provincial = db.Role_User.Find(role_id);
            provincial.role_name = role_name;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thành công";
            return RedirectToAction("Role");
        }

        // sửa tỉnh
        [HttpPost]
        public ActionResult EditUser(string user_lastName, 
            int user_id, string user_firstName, string user_email, int role_id)
        {
            User provincial = db.Users.Find(user_id);
            provincial.user_lastName = user_lastName;
            provincial.user_firstName = user_firstName;
            provincial.user_email = user_email;
            provincial.role_id = role_id;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }

        //xóa tạm thời 
        public JsonResult RecycleBinSex(int? id)
        {
            Sex_User provincial = db.Sex_User.Find(id);
            provincial.sex_recycleBin = !provincial.sex_recycleBin;
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        //xóa tạm thời 
        public JsonResult RecycleBinRole(int? id)
        {
            Role_User provincial = db.Role_User.Find(id);
            provincial.role_recycleBin = !provincial.role_recycleBin;
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        //xóa tạm thời 
        public JsonResult RecycleBinUser(int? id)
        {
            User provincial = db.Users.Find(id);
            provincial.user_recycleBin = !provincial.user_recycleBin;
            db.SaveChanges();
            return Json(new {status = true}, JsonRequestBehavior.AllowGet);
        }

        //xóa luôn
        public JsonResult DeleteUser(int? id)
        {
            User provincial = db.Users.Find(id);
            var listRemove = db.Teachnology_User.Where(x => x.user_id == id).ToList();
            db.Teachnology_User.RemoveRange(listRemove);
            db.SaveChanges();
            var listRemoveUserIP = db.User_IP.Where(x => x.user_id == id).ToList();
            db.User_IP.RemoveRange(listRemoveUserIP);
            var listRemoveViewQuestion = db.View_Question.Where(x => x.user_id == id).ToList();
            db.View_Question.RemoveRange(listRemoveViewQuestion);
            var listRemoveFriend = db.Friends.Where(x => x.userRequest_id == id).ToList();
            db.Friends.RemoveRange(listRemoveFriend);
            var listRemoveFriend2 = db.Friends.Where(x => x.userResponse_id == id).ToList();
            db.Friends.RemoveRange(listRemoveFriend2);

            var listRemoveMessage = db.Messages.Where(x => x.messageSender_id == id).ToList();
            db.Messages.RemoveRange(listRemoveMessage);

            var listRemoveMessage2 = db.Messages.Where(x => x.messageRecipients_id == id).ToList();
            db.Messages.RemoveRange(listRemoveMessage2);

            var listRemoveNoti = db.Notifications.Where(x => x.impactUser_id == id).ToList();
            db.Notifications.RemoveRange(listRemoveNoti);

            var listRemoveNoti2 = db.Notifications.Where(x => x.receiver_id == id).ToList();
            db.Notifications.RemoveRange(listRemoveNoti2);

            var listRemoveQuestion = db.Questions.Where(x => x.user_id == id).ToList();
            db.Questions.RemoveRange(listRemoveQuestion);

            db.SaveChanges();
            db.Users.Remove(provincial);
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecycleBinSexJson()
        {
            List<Sex_User> userlist = db.Sex_User.Where(n => n.sex_recycleBin == true).ToList();
            List<SexAdmin> listUsers = userlist.Select(n => new SexAdmin
            {
                sex_id = n.sex_id,
                sex_name = n.sex_name,
                sex_dateCreate = n.sex_dateCreate.ToString(),
            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecycleBinRoleJson()
        {
            List<Role_User> userlist = db.Role_User.Where(n => n.role_recycleBin == true).ToList();
            List<RoleAdmin> listUsers = userlist.Select(n => new RoleAdmin
            {
                role_id = n.role_id,
                role_name = n.role_name,
                role_dateCreate = n.role_dateCreate.ToString(),
            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecycleBinUserJson()
        {
            List<User> userlist = db.Users.Where(n => n.user_recycleBin == true).ToList();
            List<UserAdmin> listUsers = userlist.Select(n => new UserAdmin
            {
                user_id = n.user_id,
                user_lastName = n.user_lastName,
                user_firstName = n.user_firstName,
                email = n.user_email,
            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SexJson()
        {
            List<Sex_User> sex_Users = db.Sex_User.Where(n => n.sex_recycleBin == false).ToList();
            List<SexAdmin> listUsers = sex_Users.Select(n => new SexAdmin
            {
                sex_id = n.sex_id,
                sex_name = n.sex_name,
                sex_activate = n.sex_activate,
                sex_dateCreate = n.sex_dateCreate.ToString(),
                sex_dateEdit = n.sex_dateEdit.ToString(),
                sex_recycleBin = n.sex_recycleBin

            }).ToList();
            return Json(listUsers, JsonRequestBehavior.AllowGet);
        }
    }
}
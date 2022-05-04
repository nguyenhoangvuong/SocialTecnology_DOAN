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
        public JsonResult UserJson()
        {
            List<User> users = db.Users.ToList();
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using System.Web.Helpers;
using System.IO;

namespace FivePSocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        private FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        String HomeCenter = "/Center/IndexCenter";
        //------------------------------------------TEST--------------------------------
        
        //----------------------------------------MAIN--------------------------------
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(FormCollection form)
        {
            String user_email = form["user_email"].ToString();
            String user_pass = form["user_pass"].ToString();
            //Mã hóa mật khẩu
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user_pass));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            user_pass = strBuilder.ToString();
            //kiểm tra tài khoản có đang bị khóa không.
            User userActivate = db.Users.Where(n => n.user_activate == false).FirstOrDefault(n => n.user_email == user_email && n.user_pass == user_pass);
            if(userActivate != null)
            {
                ViewBag.checkLogin = "Tài khoản đã bị khóa!";
                return View(userActivate);
            }
            //kiểm tra admin
            User admin = db.Users.Where(n => n.user_recycleBin == false && n.role_id == 3).FirstOrDefault(n => n.user_email == user_email && n.user_pass == user_pass);
            if (admin != null)
            {
                admin.user_dateLogin = DateTime.Now;
                db.SaveChanges();
                HttpCookie cookie = new HttpCookie("admin_id", admin.user_id.ToString());
                cookie.Expires.AddDays(10);
                Response.Cookies.Set(cookie);
                return Redirect("/Admin/HomeAdmin/IndexAdmin");
            }
            //kiểm tra trong data người dùng bth và nhà văn
            User user = db.Users.Where(n => n.user_recycleBin == false).FirstOrDefault(n => (n.user_email == user_email || n.user_phone == user_email) && n.user_pass == user_pass);
            if(user != null && user.user_loginAuthentication == true && (user.user_emailAuthentication == true || user.user_verifyPhoneNumber == true))
            {
                Session["user"] = user;
                return RedirectToAction("AuthenticationOption");
            }
            else if(user != null)
            {
                var ipuser = Request.UserHostAddress;
                User_IP user_IP = db.User_IP.FirstOrDefault(n => n.user_id == user.user_id && n.userIP_IP == ipuser);
                if(user_IP == null)
                {
                    db.User_IP.Add(new User_IP
                    {
                        userIP_IP = ipuser,
                        user_id = user.user_id,
                        userIP_dateLogin = DateTime.Now
                    });
                    db.SaveChanges();
                }
                if(user_IP == null && user.user_emailAuthentication == true)
                {
                    WebMail.SmtpServer = "smtp.gmail.com";//Máy chủ gmail.
                    WebMail.SmtpPort = 587; // Cổng
                    WebMail.SmtpUseDefaultCredentials = true;
                    //Gửi gmail với giao thức bảo mật.
                    WebMail.EnableSsl = true;
                    //Tài khoản dùng để đăng nhập vào gmail để gửi.
                    WebMail.UserName = "hoangvuong1225@gmail.com";
                    WebMail.Password = "Hoangvuong1";
                    // Nội dung gửi.
                    WebMail.From = "hoangvuong1225@gmail.com";
                    string strTitle = "Có một địa chỉ lạ vừa đăng nhập vào tài khoản của bạn, có địa chỉ IP là : " + ipuser;
                    //Gửi gmail.
                    WebMail.Send(to: user.user_email, subject: "Oversea xin chào :", body: strTitle, isBodyHtml: true);
                }
                if(user_IP == null && user.user_verifyPhoneNumber == true)
                {
                    var userphone = user.user_phone;
                    var remove = userphone.ToString().Remove(0, 1);
                    var to = "+84" + remove;
                    TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                    var from = new PhoneNumber("+19794065417");
                    var message = MessageResource.Create(
                        from: from,
                        to: new Twilio.Types.PhoneNumber("+84347689482"),
                        body: "Five_P xin chào : " + user.user_firstName + " " + user.user_lastName + " Có một địa chỉ lạ vừa đăng nhập vào tài khoản của bạn, có địa chỉ IP là : " + ipuser
                    );
                    Content(message.Sid);
                }
                // đổi trạng thái tin nhắn.
                List<Message> statusStrMess = db.Messages.Where(n => n.messageRecipients_id == user.user_id && n.messageRecipients_status == "đã gửi.").ToList();
                foreach(var item in statusStrMess)
                {
                    db.Messages.Find(item.message_id).messageRecipients_status = "đã nhận.";
                    db.SaveChanges();
                }

                user.user_dateLogin = DateTime.Now;
                user.user_statusOnline = true;
                db.SaveChanges();
                HttpCookie cookie = new HttpCookie("user_id", user.user_id.ToString());
                cookie.Expires.AddDays(10);
                Response.Cookies.Set(cookie);
                return Redirect(HomeCenter);
            }
            ViewBag.checkLogin = "Tài khoản hoặc mật khẩu không đúng!";
            return View(user);
        }
        public ActionResult AuthenticationOption()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Session["user"] == null)
            {
                return Redirect(HomeCenter);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AuthenticationOption(string authenticationOption)
        {
            User user = (User)Session["user"];
            if (authenticationOption == "phone" && user.user_verifyPhoneNumber == true)
            {
                var userphone = user.user_phone;
                var remove = userphone.ToString().Remove(0, 1);
                var to = "+84"+ remove;
                Random random = new Random();
                var verificationCodesPhone = random.Next(100000, 999999).ToString();
                Session["verificationCodesPhone"] = verificationCodesPhone;
                Session.Timeout = 3;
                TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                var message = MessageResource.Create(
                    body: "Five_P xin chào " + user.user_firstName + " " + user.user_lastName + " Mã xác thực của bạn là : " + verificationCodesPhone,
                    from: new Twilio.Types.PhoneNumber("+19794065417"),
                    to: new Twilio.Types.PhoneNumber("+84347689482")
                );
                Content(message.Sid);
                return RedirectToAction("AuthenticationLogin");
            }
            else if(authenticationOption == "email" &&  user.user_emailAuthentication == true)
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";//Máy chủ gmail.
                    WebMail.SmtpPort = 587; // Cổng
                    WebMail.SmtpUseDefaultCredentials = true;
                    //Gửi gmail với giao thức bảo mật.
                    WebMail.EnableSsl = true;
                    //Tài khoản dùng để đăng nhập vào gmail để gửi.
                    WebMail.UserName = "hoangvuong1225@gmail.com";
                    WebMail.Password = "Hoangvuong1";
                    // Nội dung gửi.
                    WebMail.From = "hoangvuong1225@gmail.com";

                    Random random = new Random();
                    var code = random.Next(100000, 999999).ToString();
                    string strTitle = "Mã xác nhận : " + code;
                    Session["confirmemail"] = code;
                    Session.Timeout = 3;
                    //Gửi gmail.
                    WebMail.Send(to: user.user_email, subject: "Mã xác nhận của Five_P", body: strTitle, isBodyHtml: true);
                    return RedirectToAction("AuthenticationLogin");
                }
                catch (Exception)
                {
                    ViewBag.notification = "Không gửi được email";
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                ViewBag.AuthenticationOption = "Lựa chọn của bạn chưa được xác thực! Vui lòng chọn xác thực khác.";
            }
            return View();
        }
        public ActionResult AuthenticationLogin()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Session["user"] == null)
            {
                return Redirect(HomeCenter);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AuthenticationLogin(string codeAuthentication)
        {

            if(Session["confirmemail"] !=null)
            {
                if(codeAuthentication == Session["confirmemail"].ToString())
                {
                    User user = (User)Session["user"];
                    user.user_dateLogin = DateTime.Now;
                    user.user_statusOnline = true;
                    //lưu địa chỉ ip user
                    var ipuser = Request.UserHostAddress;
                    List<User_IP> user_IP = db.User_IP.Where(n => n.user_id == user.user_id && n.userIP_IP == ipuser).ToList();
                    if (user_IP == null)
                    {
                        db.User_IP.Add(new User_IP
                        {
                            userIP_IP = ipuser,
                            user_id = user.user_id,
                            userIP_dateLogin = DateTime.Now
                        });
                    }
                    if (user_IP == null && user.user_emailAuthentication == true)
                    {
                        WebMail.SmtpServer = "smtp.gmail.com";//Máy chủ gmail.
                        WebMail.SmtpPort = 587; // Cổng
                        WebMail.SmtpUseDefaultCredentials = true;
                        //Gửi gmail với giao thức bảo mật.
                        WebMail.EnableSsl = true;
                        //Tài khoản dùng để đăng nhập vào gmail để gửi.
                        WebMail.UserName = "hoangvuong1225@gmail.com";
                        WebMail.Password = "Hoangvuong1";
                        // Nội dung gửi.
                        WebMail.From = "hoangvuong1225@gmail.com";
                        string strTitle = "Có một địa chỉ lạ vừa đăng nhập vào tài khoản của bạn, có địa chỉ IP là : " + ipuser;
                        //Gửi gmail.
                        WebMail.Send(to: user.user_email, subject: "Five_P xin chào :", body: strTitle, isBodyHtml: true);
                    }
                    if (user_IP == null && user.user_verifyPhoneNumber == true)
                    {
                        var userphone = user.user_phone;
                        var remove = userphone.ToString().Remove(0, 1);
                        var to = "+84" + remove;
                        TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                        var from = new PhoneNumber("+19794065417");
                        var message = MessageResource.Create(
                            from: from,
                            to: new Twilio.Types.PhoneNumber("+84347689482"),
                            body: "Five_P xin chào : " + user.user_firstName + " " + user.user_lastName + " Có một địa chỉ lạ vừa đăng nhập vào tài khoản của bạn, có địa chỉ IP là : " + ipuser
                        );
                        Content(message.Sid);
                    }
                    db.SaveChanges();
                    // đổi trạng thái tin nhắn.
                    List<Message> statusStrMess = db.Messages.Where(n => n.messageRecipients_id == user.user_id).ToList();
                    foreach (var item in statusStrMess)
                    {
                        db.Messages.Find(item.message_id).messageRecipients_status = "đã nhận.";
                        db.SaveChanges();
                    }
                    HttpCookie cookie = new HttpCookie("user_id", user.user_id.ToString());
                    cookie.Expires.AddDays(10);
                    Response.Cookies.Set(cookie);
                    Session["user"] = null;
                    Session["confirmemail"] = null;
                    return Redirect(HomeCenter);
                }
                else
                {
                    ViewBag.statusCode = "Sai mã ! Vui lòng nhập lại.";
                    return View();
                }
            }
            else if(Session["verificationCodesPhone"] != null)
            {
                if(codeAuthentication == Session["verificationCodesPhone"].ToString())
                {
                    User user = (User)Session["user"];
                    user.user_dateLogin = DateTime.Now;
                    user.user_statusOnline = true;
                    //lưu địa chỉ ip user
                    var ipuser = Request.UserHostAddress;
                    List<User_IP> user_IP = db.User_IP.Where(n => n.user_id == user.user_id && n.userIP_IP == ipuser).ToList();
                    if (user_IP == null)
                    {
                        db.User_IP.Add(new User_IP
                        {
                            userIP_IP = ipuser,
                            user_id = user.user_id,
                            userIP_dateLogin = DateTime.Now
                        });
                    }
                    if (user_IP == null && user.user_emailAuthentication == true)
                    {
                        WebMail.SmtpServer = "smtp.gmail.com";//Máy chủ gmail.
                        WebMail.SmtpPort = 587; // Cổng
                        WebMail.SmtpUseDefaultCredentials = true;
                        //Gửi gmail với giao thức bảo mật.
                        WebMail.EnableSsl = true;
                        //Tài khoản dùng để đăng nhập vào gmail để gửi.
                        WebMail.UserName = "hoangvuong1225@gmail.com";
                        WebMail.Password = "Hoangvuong1";
                        // Nội dung gửi.
                        WebMail.From = "hoangvuong1225@gmail.com";
                        string strTitle = "Có một địa chỉ lạ vừa đăng nhập vào tài khoản của bạn, có địa chỉ IP là : " + ipuser;
                        //Gửi gmail.
                        WebMail.Send(to: user.user_email, subject: "Five_P xin chào :", body: strTitle, isBodyHtml: true);
                    }
                    if (user_IP == null && user.user_verifyPhoneNumber == true)
                    {
                        var userphone = user.user_phone;
                        var remove = userphone.ToString().Remove(0, 1);
                        var to = "+84" + remove;
                        TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                        var from = new PhoneNumber("+19794065417");
                        var message = MessageResource.Create(
                            from: from,
                            to: new Twilio.Types.PhoneNumber("+84347689482"),
                            body: "Five_P xin chào : " + user.user_firstName + " " + user.user_lastName + " Có một địa chỉ lạ vừa đăng nhập vào tài khoản của bạn, có địa chỉ IP là : " + ipuser
                        );
                        Content(message.Sid);
                    }
                    db.SaveChanges();
                    // đổi trạng thái tin nhắn.
                    List<Message> statusStrMess = db.Messages.Where(n => n.messageRecipients_id == user.user_id).ToList();
                    foreach (var item in statusStrMess)
                    {
                        db.Messages.Find(item.message_id).messageRecipients_status = "đã nhận.";
                        db.SaveChanges();
                    }

                    HttpCookie cookie = new HttpCookie("user_id", user.user_id.ToString());
                    cookie.Expires.AddDays(10);
                    Response.Cookies.Set(cookie);
                    Session["user"] = null;
                    Session["verificationCodesPhone"] = null;
                    return Redirect(HomeCenter);
                }
                else
                {
                    ViewBag.statusCode = "Sai mã ! Vui lòng nhập lại.";
                    return View();
                }
            }
            return View();
        }

        //Đăng ký
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register([Bind(Include = "user_id,user_pass,user_firstName,user_lastName,user_email,user_token,role_id,user_code,user_avatar,user_coverImage,user_activate,user_recycleBin,user_dateCreate,user_dateEdit,user_dateLogin,user_emailAuthentication,user_verifyPhoneNumber,user_loginAuthentication,provincial_id,district_id,commune_id,user_addressRemaining,sex_id,user_linkFacebook,user_linkGithub,user_anotherWeb,user_hobbyWork,user_hobby,user_birthday,user_popular,user_goldMedal,user_silverMedal,user_brozeMedal,user_vipMedal,user_phone,user_SecurityAccount")] User user)
        {
            //kiểm tra email đã được đăng ký chưa
            User checkEmail = db.Users.FirstOrDefault(n => n.user_email == user.user_email);
            if(checkEmail != null)
            {
                ViewBag.checkEmail = "Email đã được sử dụng, vui lòng nhập email mới!";
                return View(user);
            }
            //random ký tự
            Random random = new Random();
            string s1 = RandomString(9, false);
            user.user_code = "#" + s1 +"-"+ random.Next(100, 999).ToString();
            //Mã hóa mật khẩu
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user.user_pass));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            // lưu mật khẩu đã mã hóa
            user.user_pass = strBuilder.ToString();
            
            //lưu các thông tin còn lại
            user.user_token = Guid.NewGuid().ToString();
            user.role_id = 1;
            user.user_coverImage = "coverImage.png";
            user.user_avatar = "user.png";
            user.user_activate = true;
            user.user_recycleBin = false;
            user.user_dateCreate = DateTime.Now;
            user.user_dateEdit = DateTime.Now;
            user.user_dateLogin = DateTime.Now;
            user.user_emailAuthentication = false;
            user.user_loginAuthentication = false;
            user.user_verifyPhoneNumber = false;
            user.user_SecurityAccount = false;
            user.user_popular = 0;
            user.user_statusOnline = true;
            user.user_vipMedal = 0;
            user.user_silverMedal = 0;
            user.user_goldMedal = 0;
            user.user_brozeMedal = 0;
            db.Users.Add(user);
            db.SaveChanges();
            User sUser = db.Users.FirstOrDefault(n => n.user_email == user.user_email);
            //Lưu ip người dùng
            db.User_IP.Add(new User_IP
            {
                userIP_IP = Request.UserHostAddress,
                user_id = sUser.user_id,
                userIP_dateLogin = DateTime.Now
            });
            db.SaveChanges();
            HttpCookie httpCookie = new HttpCookie("user_id", sUser.user_id.ToString());
            httpCookie.Expires.AddDays(10);
            Response.Cookies.Set(httpCookie);
            return Redirect(HomeCenter);
        }

        //random chuỗi ngẫu nhiên
        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                c = Convert.ToChar(Convert.ToInt32(rand.Next(65, 87)));
                sb.Append(c);
            }
            if (lowerCase)
                return sb.ToString().ToLower();
            return sb.ToString();

        }
        public ActionResult Logout()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            db.Users.Find(user_id).user_statusOnline = false;
            db.SaveChanges();
            HttpCookie cookie = new HttpCookie("user_id");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }
        //-------------------------------------------------Cài đặt thông tin cá nhân----------------------------------
        public ActionResult SettingAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SettingAccount(User user, string user_firstName , string user_lastName)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);
            user.user_firstName = user_firstName;
            user.user_lastName = user_lastName;
            db.SaveChanges();
            return View(user);
        }
        // -------------------------------------Nhận thông báo các câu hỏi liên quan đến công nghệ của bạn---
        public ActionResult GetNotification()
        {
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            Functions_User check = db.Functions_User.FirstOrDefault(n => n.user_id == user_id);
            if (check == null)
            {
                Functions_User functions_User = new Functions_User()
                {
                    user_id = user_id,
                    notification_question_technology = true
                };
                db.Functions_User.Add(functions_User);
                db.SaveChanges();
            }
            else
            {
                check.notification_question_technology = !check.notification_question_technology;
                db.SaveChanges();
            }
            return View();
        }
        //-------------------------------------------------lưu giới tính user----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SexUser(User user, int user_sex)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // user_sex == null trả về trang đó.
            if(user_sex == 0)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);
            //lưu giới tính
            user.sex_id = user_sex;
            // lưu hình
            if(user_sex == 1 && user.user_avatar == null)
            {
                user.user_avatar = "Man.png";
            }
            else if(user_sex == 2 && user.user_avatar == null)
            {
                user.user_avatar = "Girl.png";
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //-------------------------------------------------lưu ngày sinh----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult BrithDay(User user , DateTime user_birthday)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);

            user.user_birthday = user_birthday;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //-------------------------------------------------sở thích công việc----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult HobbyWork(User user, string user_hobbyWork)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);

            user.user_hobbyWork = user_hobbyWork;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //-------------------------------------------------sở thích cá nhân----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Hobby(User user, string user_hobby)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);

            user.user_hobby = user_hobby;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //-------------------------------------------------Technology----------------------------------

        // lưu công nghệ user
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult TechnologyUser(int[] technology_id)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());

            List<Teachnology_User> steachnology_User = db.Teachnology_User.Where(n => n.user_id == user_id).ToList();
            if(steachnology_User == null)
            {
                foreach (var item in technology_id)
                {
                    Teachnology_User teachnology_User = new Teachnology_User()
                    {
                        user_id = user_id,
                        technology_id = item,
                        technology_dateCreate = DateTime.Now,
                        technology_recycleBin = false
                    };
                    db.Teachnology_User.Add(teachnology_User);
                }
            }
            else
            {
                int variable = 0;
                foreach (var item in steachnology_User)
                {
                    foreach (var item2 in technology_id)
                    {
                        if (item.technology_id == item2)
                        {
                            variable = 1;
                            break;
                        }
                    }
                    if (variable == 0)
                    {
                        db.Teachnology_User.Remove(db.Teachnology_User.Find(item.technologyUser_id));
                    }
                    variable = 0;
                }
                List<Teachnology_User> Teachnology_User2 = db.Teachnology_User.Where(n => n.user_id == user_id).ToList();
                variable = 0;
                foreach (var item in technology_id)
                {
                    foreach (var item2 in Teachnology_User2)
                    {
                        if (item == item2.technology_id)
                        {
                            variable = 1;
                            break;
                        }
                    }
                    if (variable == 0)
                    {
                        Teachnology_User tag = new Teachnology_User()
                        {
                            technology_dateCreate = DateTime.Now,
                            technology_recycleBin = false,
                            technology_id = item,
                            user_id = user_id,
                        };
                        db.Teachnology_User.Add(tag);
                    }
                    variable = 0;
                }
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult Security()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            return View();
        }
        // đăng nhập 2 lớp
        public ActionResult ChangeLoginAuthentication()
        {
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            if((user.user_phone != null || user.user_phone != "" || user.user_email != null) && (user.user_verifyPhoneNumber == true || user.user_emailAuthentication == true))
            {
                user.user_loginAuthentication = !user.user_loginAuthentication;
                db.SaveChanges();
            }
            return View(user);
        }
        // Bảo vệ tài khoản
        public ActionResult SecurityAccount()
        {
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            
            User user = db.Users.Find(user_id);
            if ((user.user_phone != null || user.user_phone != "") && user.user_verifyPhoneNumber == true)
            {
                if(user.user_SecurityAccount == false)
                {
                    user.user_SecurityAccount = true;
                }
                else if(user.user_SecurityAccount == true)
                {
                    var userphone = user.user_phone;
                    var remove = userphone.ToString().Remove(0, 1);
                    var to = "+84" + remove;
                    Random random = new Random();
                    var verificationCodesPhone = random.Next(100000, 999999).ToString();
                    Session["verificationCodesPhone"] = verificationCodesPhone;
                    Session.Timeout = 3;
                    TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                    var from = new PhoneNumber("+19794065417");
                    var message = MessageResource.Create(
                        from: from,
                        to: new Twilio.Types.PhoneNumber("+84347689482"),
                        body: "Five_P xin chào " + user.user_firstName + " " + user.user_lastName + " Mã xác thực của bạn là : " + verificationCodesPhone
                    );
                    Content(message.Sid);
                    return RedirectToAction("AuthenticationSecurityAccount");
                }
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        // Xác thực tắt bảo vệ tài khoản
        public ActionResult AuthenticationSecurityAccount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AuthenticationSecurityAccount(string codeAuthentication)
        {
            if (codeAuthentication == Session["verificationCodesPhone"].ToString())
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                db.Users.Find(user_id).user_SecurityAccount = false;
                db.SaveChanges();
                Session["verificationCodesPhone"] = null;
                Session["email"] = null;
            }
            else
            {
                ViewBag.statusCode = "Mã code sai! Vui long nhập lại";
                return Redirect(Request.UrlReferrer.ToString());
            }
            return RedirectToAction("Security");
        }
        //-------------------------------------------------Email-Authentication----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Email(User user, string user_email , string password)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());

            //Mã hóa mật khẩu
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            byte[] sresult = md5.Hash;
            StringBuilder sstrBuilder = new StringBuilder();
            for (int i = 0; i < sresult.Length; i++)
            {
                sstrBuilder.Append(sresult[i].ToString("x2"));
            }
            password = sstrBuilder.ToString();
            // kiểm tra password
            if (db.Users.Find(user_id).user_pass != password)
            {
                Session["checkPass"] = "Mật khẩu không đúng! Vui lòng nhập lại mật khẩu.";
                return Redirect(Request.UrlReferrer.ToString());
            }
            if(db.Users.Find(user_id).user_SecurityAccount == true)
            {
                Session["email"] = user_email;
                var userphone = db.Users.Find(user_id).user_phone;
                var remove = userphone.ToString().Remove(0, 1);
                var to = "+84" + remove;
                Random random = new Random();
                var verificationCodesPhone = random.Next(100000, 999999).ToString();
                Session["verificationCodesPhone"] = verificationCodesPhone;
                Session.Timeout = 3;
                var from = new PhoneNumber("+19794065417");
                TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                var message = MessageResource.Create(
                    body: "Five_P xin chào " + user.user_firstName + " " + user.user_lastName + " Mã xác thực của bạn là : " + verificationCodesPhone,
                    from: new Twilio.Types.PhoneNumber("+19794065417"),
                    to: new Twilio.Types.PhoneNumber("+84347689482")
                );
                Content(message.Sid);
                return RedirectToAction("AuthenticationChangeEmail");
            }
            user = db.Users.Find(user_id);
            user.user_email = user_email;
            user.user_emailAuthentication = false;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //Xác thực thay đổi email
        public ActionResult AuthenticationChangeEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AuthenticationChangeEmail(string codeAuthentication)
        {
            if (codeAuthentication == Session["verificationCodesPhone"].ToString())
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                db.Users.Find(user_id).user_email = Session["email"].ToString();
                db.Users.Find(user_id).user_emailAuthentication = false;
                db.SaveChanges();
                Session["verificationCodesPhone"] = null;
                Session["email"] = null;
            }
            else
            {
                ViewBag.statusCode = "Mã code sai! Vui long nhập lại";
                return View();
            }
            return RedirectToAction("Security");
        }
        public ActionResult EmailSend()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";//Máy chủ gmail.
                WebMail.SmtpPort = 587; // Cổng
                WebMail.SmtpUseDefaultCredentials = true;
                //Gửi gmail với giao thức bảo mật.
                //Gửi gmail với giao thức bảo mật.
                WebMail.EnableSsl = true;
                //Tài khoản dùng để đăng nhập vào gmail để gửi.
                WebMail.UserName = "hoangvuong1225@gmail.com";
                WebMail.Password = "Hoangvuong1";
                // Nội dung gửi.
                WebMail.From = "hoangvuong1225@gmail.com";

                Random random = new Random();
                var code = random.Next(100000,999999).ToString();
                string strTitle = "Mã xác nhận : " + code;
                Session["confirmemail"] = code;
                Session.Timeout = 3;
                //Gửi gmail.
                WebMail.Send(to: user.user_email, subject: "Mã xác nhận của Five_P", body: strTitle, isBodyHtml: true);
                ViewBag.notification = "Không gửi được email111";
                return RedirectToAction("EmailAuthentication");
            }
            catch (Exception ex)
            {
                throw ex;
                ViewBag.notification = "Không gửi được email";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
        public ActionResult EmailAuthentication()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            return View();
        }
        [HttpPost]
        public ActionResult EmailAuthentication(string codeAuthentication)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            if (Session["confirmemail"].ToString() == codeAuthentication)
            {
                db.Users.Find(user_id).user_emailAuthentication = true;
                db.SaveChanges();
                Session["confirmemail"] = null;
                return RedirectToAction("SettingAccount");
            }
            else
            {
                ViewBag.statusCode = "Nhập sai mã";
            }
            return View();
        }
        //-------------------------------------------------NumberPhone-Verification----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Numberphone(User user, string user_phone ,string password)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            //kiểm tra sdt này đã tồn tại chưa
            User checkPhone = db.Users.FirstOrDefault(n => n.user_phone == user_phone && n.user_id != user_id);
            if(checkPhone != null)
            {
                Session["checkPhone"] = "Số điện thoại này đã được đăng ký! Vui lòng điền số khác.";
                Session.Timeout = 3;
                return Redirect(Request.UrlReferrer.ToString());
            }
            //Mã hóa mật khẩu
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            byte[] sresult = md5.Hash;
            StringBuilder sstrBuilder = new StringBuilder();
            for (int i = 0; i < sresult.Length; i++)
            {
                sstrBuilder.Append(sresult[i].ToString("x2"));
            }
            password = sstrBuilder.ToString();
            // kiểm tra password
            if (db.Users.Find(user_id).user_pass != password)
            {
                Session["checkPass"] = "Mật khẩu không đúng! Vui lòng nhập lại mật khẩu.";
                return Redirect(Request.UrlReferrer.ToString());
            }
            user = db.Users.Find(user_id);
            if (user.user_SecurityAccount == true)
            {
                Session["phone"] = user_phone;
                var userphone = db.Users.Find(user_id).user_phone;
                var remove = userphone.ToString().Remove(0, 1);
                var to = "+84" + remove;
                Random random = new Random();
                var verificationCodesPhone = random.Next(100000, 999999).ToString();
                Session["verificationCodesPhone"] = verificationCodesPhone;
                Session.Timeout = 3;
                TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                var from = new PhoneNumber("+19794065417");
                var message = MessageResource.Create(
                    from: from,
                    to: new Twilio.Types.PhoneNumber("+84347689482"),
                    body: "Five_P xin chào " + db.Users.Find(user_id).user_firstName + " " + db.Users.Find(user_id).user_lastName + " Mã xác thực của bạn là : " + verificationCodesPhone
                );
                Content(message.Sid);
                return RedirectToAction("AuthenticationChangePhone");
            }
            user.user_phone = user_phone;
            user.user_verifyPhoneNumber = false;
            db.SaveChanges();
            Session["checkPhone"] = null;
            return Redirect(Request.UrlReferrer.ToString());
        }
        //Xác thực thay đổi số điện thoại
        public ActionResult AuthenticationChangePhone()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AuthenticationChangePhone(string codeAuthentication)
        {
            if (codeAuthentication == Session["verificationCodesPhone"].ToString())
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                User user = db.Users.Find(user_id);
                user.user_phone = Session["phone"].ToString();
                user.user_verifyPhoneNumber = false;
                user.user_SecurityAccount = false;
                db.SaveChanges();
                Session["verificationCodesPhone"] = null;
                Session["phone"] = null;
            }
            else
            {
                ViewBag.statusCode = "Mã code sai! Vui long nhập lại";
                return View();
            }
            return RedirectToAction("Security");
        }
        public ActionResult NumberPhoneVerification()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            //kiểm tra đã xác thực chưa
            if (user.user_verifyPhoneNumber == true)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            var userphone = user.user_phone;
            var remove = userphone.ToString().Remove(0, 1);
            var to = "+84" + remove;
            Random random = new Random();
            var verificationCodesPhone = random.Next(100000, 999999).ToString();
            Session["verificationCodesPhone"] = verificationCodesPhone;
            Session.Timeout = 3;
            var from = new PhoneNumber("+19794065417");
            TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
            var message = MessageResource.Create(
                body: "Five_P xin chào " + user.user_firstName + " " + user.user_lastName + " Mã xác thực của bạn là : " + verificationCodesPhone,
                from: new Twilio.Types.PhoneNumber("+19794065417"),
                to: new Twilio.Types.PhoneNumber("+84347689482")
            );
            Content(message.Sid);
            return RedirectToAction("VerificationCodesPhone");
        }
        public ActionResult VerificationCodesPhone()
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            //kiểm tra đã xác thực chưa
            if (user.user_verifyPhoneNumber == true)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return View();
        }
        [HttpPost]
        public ActionResult VerificationCodesPhone(string verificationCode)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            if (Session["verificationCodesPhone"].ToString() == verificationCode)
            {
                db.Users.Find(user_id).user_verifyPhoneNumber = true;
                db.SaveChanges();
                Session["verificationCodesPhone"] = null;
                return RedirectToAction("SettingAccount");
            }
            else
            {
                ViewBag.verificationCodesPhone = "Sai mã xác thực !";
            }
            return View();
        }
        //---------------------------------------------------Lọc địa chỉ-----------------------------------------------
        public PartialViewResult District(int? id)
        {
            List<District> dsHuyen = new List<District>();
            if (id == null)
            {
                dsHuyen = db.Districts.ToList();
            }
            else
            {
                dsHuyen = db.Districts.Where(n => n.provincial_id == id && n.district_activate == true && n.district_recycleBin == false).ToList();
            }
            return PartialView(dsHuyen);
        }
        public PartialViewResult Commune(int? id)
        {
            List<Commune> dsXa = new List<Commune>();
            if (id == null)
            {
                return PartialView();
            }
            else
            {
                dsXa = db.Communes.Where(n => n.district_id == id && n.commune_activate == true && n.commune_recycleBin == false).ToList();
            }
            return PartialView(dsXa);
        }
        //-------------------------------------------------địa chỉ thường trú----------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Address(int provincial_id, int district_id, int commune_id, string user_addressRemaining)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            user.provincial_id = provincial_id;
            user.district_id = district_id;
            user.commune_id = commune_id;
            user.user_addressRemaining = user_addressRemaining;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        //-------------------------------------------------Link các website ----------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LinkWebAnother(User user, string linkWebAnother)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);
            user.user_anotherWeb = linkWebAnother;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        // lưu kink facebook
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LinkFacebook(User user, string linkFacebook)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);
            user.user_linkFacebook = linkFacebook;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //luu link github
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LinkGithub(User user, string linkGithub)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            user = db.Users.Find(user_id);
            user.user_linkGithub = linkGithub;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());

        }
        //---------------------------------------------------Đổi mật khẩu-----------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ChangePassword(string newPass, string user_pass)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            //Mã hóa mật khẩu
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user_pass));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            user_pass = strBuilder.ToString();
            //kiểm tra pass nhập vào
            if (user_pass != user.user_pass)
            {
                Session["checkPass"] = "Mật khẩu không đúng! Vui lòng nhập lại mật khẩu .";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                //Mã hóa mật khẩu
                MD5 smd5 = new MD5CryptoServiceProvider();
                smd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(newPass));
                byte[] sresult = smd5.Hash;
                StringBuilder sstrBuilder = new StringBuilder();
                for (int i = 0; i < sresult.Length; i++)
                {
                    sstrBuilder.Append(sresult[i].ToString("x2"));
                }
                newPass = sstrBuilder.ToString();
                if(user.user_SecurityAccount == true)
                {
                    Session["password"] = newPass;
                    var userphone = user.user_phone;
                    var remove = userphone.ToString().Remove(0, 1);
                    var to = "+84" + remove;
                    Random random = new Random();
                    var verificationCodesPhone = random.Next(100000, 999999).ToString();
                    Session["verificationCodesPhone"] = verificationCodesPhone;
                    Session.Timeout = 3;
                    TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                    var from = new PhoneNumber("+19794065417");
                    var message = MessageResource.Create(
                        from: new Twilio.Types.PhoneNumber("++19794065417"),
                        to: new Twilio.Types.PhoneNumber("+84347689482"),
                        body: "Five_P xin chào " + user.user_firstName + " " + user.user_lastName + " Mã xác thực của bạn là : " + verificationCodesPhone
                    );
                    Content(message.Sid);
                    return RedirectToAction("AuthenticationChangePassword");
                }
                user.user_pass = newPass;
                Session["checkPass"] = null;
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult AuthenticationChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AuthenticationChangePassword(string codeAuthentication)
        {
            if(codeAuthentication == Session["verificationCodesPhone"].ToString())
            {
                int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
                db.Users.Find(user_id).user_pass = Session["password"].ToString();
                db.SaveChanges();
                Session["verificationCodesPhone"] = null;
                Session["password"] = null;
            }
            else
            {
                ViewBag.statusCode = "Mã code sai! Vui long nhập lại";
                return View();
            }
            return RedirectToAction("Security");
        }
        //---------------------------------------------------Đổi Ảnh cho user-----------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ChangeAvatar(HttpPostedFileBase user_avatar)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            if(user_avatar == null)
            {
                user.user_avatar = "user.png";
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            var varFileImg = Path.GetFileName(user_avatar.FileName);
            //Lưu file
            var pa = Path.Combine(Server.MapPath("~/Image/Users"), varFileImg);
            if (System.IO.File.Exists(pa))
            {
                Random random = new Random();
                var ram = random.Next();
                var varFileImg2 = Path.GetFileName(ram+user_avatar.FileName);
                var pa2 = Path.Combine(Server.MapPath("~/Image/Users"), varFileImg2);
                if(System.IO.File.Exists(pa2))
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                user_avatar.SaveAs(pa2);
                user.user_avatar = ram+user_avatar.FileName;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            user_avatar.SaveAs(pa);
            user.user_avatar = user_avatar.FileName;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ChangeCoverImage(HttpPostedFileBase user_coverImage)
        {
            //nếu ko có cookies cho về trang tất cả câu hỏi.
            if (Request.Cookies["user_id"] == null)
            {
                return Redirect(HomeCenter);
            }
            // khi tồn tại cookies
            int user_id = int.Parse(Request.Cookies["user_id"].Value.ToString());
            User user = db.Users.Find(user_id);
            if (user_coverImage == null)
            {
                user.user_coverImage = "coverImage.png";
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());

            }
            var varFileImg = Path.GetFileName(user_coverImage.FileName);
            //Lưu file
            var pa = Path.Combine(Server.MapPath("~/Image/Users"), varFileImg);
            if (System.IO.File.Exists(pa))
            {
                Random random = new Random();
                var ram = random.Next();
                var varFileImg2 = Path.GetFileName(ram + user_coverImage.FileName);
                var pa2 = Path.Combine(Server.MapPath("~/Image/Users"), varFileImg2);
                if (System.IO.File.Exists(pa2))
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                user_coverImage.SaveAs(pa2);
                user.user_coverImage = ram + user_coverImage.FileName;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            user_coverImage.SaveAs(pa);
            user.user_coverImage = user_coverImage.FileName;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        //---------------------------------------------------Quên mật khẩu-----------------------------------------------
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(FormCollection f)
        {
            String strSend = f["send"].ToString();
            User userPhone = db.Users.FirstOrDefault(n => n.user_phone == strSend && n.user_verifyPhoneNumber == true);
            User userEmail = db.Users.FirstOrDefault(n => n.user_email == strSend && n.user_emailAuthentication == true);
            if(userPhone !=null)
            {
                var userphone = strSend;
                var remove = userphone.ToString().Remove(0, 1);
                var to = "+84" + remove;
                Random random = new Random();
                var newPass = random.Next(100000, 999999).ToString();
                Session["verificationCodesPhone"] = newPass;
                Session.Timeout = 3;
                //Mã hóa mật khẩu
                MD5 md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(newPass));
                byte[] sresult = md5.Hash;
                StringBuilder sstrBuilder = new StringBuilder();
                for (int i = 0; i < sresult.Length; i++)
                {
                    sstrBuilder.Append(sresult[i].ToString("x2"));
                }
                var password = sstrBuilder.ToString();
                userPhone.user_pass = password;
                db.SaveChanges();
                TwilioClient.Init("AC575ae7d10fab1921eb5f72bbf35ce76f", "bb3954db9fd7b8ea6d4ac95156e61450");
                var from = new PhoneNumber("+19794065417");
                var message = MessageResource.Create(
                    from: new Twilio.Types.PhoneNumber("++19794065417"),
                to: new Twilio.Types.PhoneNumber("+84347689482"),
                    body: "Five_P xin chào " + " Mật khẩu mới của bạn là : " + newPass
                );
                Content(message.Sid);
                return RedirectToAction("Login");
            }
            else if(userEmail != null)
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";//Máy chủ gmail.
                    WebMail.SmtpPort = 587; // Cổng
                    WebMail.SmtpUseDefaultCredentials = true;
                    //Gửi gmail với giao thức bảo mật.
                    WebMail.EnableSsl = true;
                    //Tài khoản dùng để đăng nhập vào gmail để gửi.
                    WebMail.UserName = "hoangvuong1225@gmail.com";
                    WebMail.Password = "Hoangvuong1";
                    // Nội dung gửi.
                    WebMail.From = "hoangvuong1225@gmail.com";

                    Random random = new Random();
                    var code = random.Next(100000, 999999).ToString();

                    //Mã hóa mật khẩu
                    MD5 md5 = new MD5CryptoServiceProvider();
                    md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(code));
                    byte[] sresult = md5.Hash;
                    StringBuilder sstrBuilder = new StringBuilder();
                    for (int i = 0; i < sresult.Length; i++)
                    {
                        sstrBuilder.Append(sresult[i].ToString("x2"));
                    }
                    var password = sstrBuilder.ToString();
                    userEmail.user_pass = password;
                    db.SaveChanges();

                    string strTitle = "Mật khẩu mới của bạn là : " + code;
                    Session["confirmemail"] = code;
                    Session.Timeout = 3;
                    //Gửi gmail.
                    WebMail.Send(to: userEmail.user_email, subject: "Mật khẩu mới của bạn trên Five_P", body: strTitle, isBodyHtml: true);
                    return RedirectToAction("Login");
                }
                catch (Exception)
                {
                    ViewBag.notification = "Không gửi được email";
                }
            }
            else
            {
                ViewBag.forgotPassword = "Phương thức chưa xác thực!";
            }
            return View();
        }

    }
}
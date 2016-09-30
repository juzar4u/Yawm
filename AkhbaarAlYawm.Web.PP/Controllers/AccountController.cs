using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using Argaam.FM.Users.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult changePassword()
        {
            UserModel _user = AuthHelper.LoginFromCookie();
            if (_user != null)
            {
                Users newUser = new Users();
                newUser.Email = _user.Email;
                newUser.UserID = (int)_user.UserID;
                UserServices.GetInstance.SetEmailToken(newUser,"Akhbaar-change passwork link", (int)EmailTemplateEnum.ForgotPassword);

                ViewBag.message = "Please check Registered Email for Change Password link!";
                return View("~/Views/Shared/PartialCustomPopupMessage.cshtml");
            }
            return Redirect("Account/Login");
        }

        [HttpGet]
        public ActionResult ForgotPassword(int userId, string guid)
        {
            UserModel _user = UserServices.GetInstance.GetUserModelByUserIDandGuid(userId, guid);
            if (_user != null)
            {
                return View(_user);
            }
            ViewBag.message = "Invalid token!";
            return View("~/Views/Shared/PartialCustomPopupMessage.cshtml");
        }


        [HttpPost]
        public ActionResult ForgotPassword(UserModel model)
        {
            if (model.changePassword == model.ConfirmPassword)
            {
                Users _user = UserServices.GetInstance.GetUserByUserID((int)model.UserID);
                _user.UserGUID = Guid.NewGuid().ToString();
                _user.Password = ExtensionMethodsCrypography.Encrypt(model.changePassword);
                UserServices.GetInstance.UpdateUsers(_user);
                ViewBag.message = "Password Changed Successfully";
                return View("~/Views/Shared/PartialCustomPopupMessage.cshtml");

            }
            ViewBag.Error = "Password cannot be changed. Contact Administrator!";
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            UserModel _user = new UserModel();
            _user = AuthHelper.LoginFromCookie();
            if (_user == null)
            {
                return View();
            }
             
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            UserModel _user = new UserModel();
            
                _user = UserServices.GetInstance.LoginPPUserByEmailandPassword(model.Email, ExtensionMethodsCrypography.Encrypt(model.Password));
                if (_user != null && _user.IsVerified && _user.UserStatusID == (int)UserStatusEnum.Active)
                {   
                    AuthHelper.AddSSOCookieIfNotExits(_user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string Error = GetErrorMsg(_user);
                    ViewBag.Error = Error;
                }

            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserModel model)
        {
            Users _user = new Users();
            try
            {
                UserProfile profile = new UserProfile();
                _user.EjamatID = model.EjamatID;
                _user.FirstName = model.FirstName;
                _user.LastName = model.LastName;
                _user.Email = model.Email;
                _user.Password = ExtensionMethodsCrypography.Encrypt(model.Password);
                _user.UserGUID = Guid.NewGuid().ToString();
                _user.CreatedOn = DateTime.Now;
                _user.IsVerified = false;
                _user.UserStatusID = (int)UserStatusEnum.Active;
                _user.RoleID = (int)UserRoleEnum.User;
                HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[0];
                _user.UserID = UserServices.GetInstance.InsertIntoUsers(_user);
                profile.UserID = _user.UserID;
                UserServices.GetInstance.CreateProfile(profile);
                ValidateAndUploadImage(postedFile, _user.UserID);
                UserServices.GetInstance.SetEmailToken(_user, "Akhbaar-Verfication Link", (int)EmailTemplateEnum.AccountVerification);
                try
                {
                    EmailService es = new EmailService();
                    es.SendSimpleEmails();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

            return RedirectToAction("EmailPopup");
        }

        public ActionResult UserInfo()
        {
            UserModel _user = AuthHelper.LoginFromCookie();
            
            return PartialView(_user);
        }

        public ActionResult LogOff()
        {
            AuthHelper.ExpireSSOCookie();
            SessionManager.SessionUserObject = null;
            return Redirect("/");
        }

        public ActionResult EmailPopup()
        {
            return View();
        }

        public ActionResult UserVerification(int userid, string guid)
        {
            Users _user = UserServices.GetInstance.GetUserByUserID(userid);
            if (_user != null && _user.UserGUID == guid)
            {
                UserModel usermodel = new UserModel();

                _user.IsVerified = true;
                _user.UserGUID = Guid.NewGuid().ToString();
                UserServices.GetInstance.UpdateUsers(_user);
                usermodel.UserID = _user.UserID;
                usermodel.Email = _user.Email;
                AuthHelper.AddSSOCookieIfNotExits(usermodel);
                ViewBag.Message = "Your Account has been successfully verified";
                return View();
            }
            if (_user.IsVerified)
            {
                UserModel usermodel = new UserModel();
                usermodel.UserID = _user.UserID;
                usermodel.Email = _user.Email;
                AuthHelper.AddSSOCookieIfNotExits(usermodel);
                ViewBag.Message = "Your account is already verified";
            }
            else if (_user.UserGUID != guid)
            {
                ViewBag.Message = "This Verification Link is no longer valid";
            }
            return View();
        }

        

        public string IsEjamaatExist(int ejamat)
        {
            var rowCount = UserServices.GetInstance.IsEjamatExist(ejamat);
            return (rowCount > 0 ? "1" : "0");
        }

        public string IsEmailExist(string email)
        {
            var rowCount = UserServices.GetInstance.IsEmailExist(email);
            return (rowCount > 0 ? "1" : "0");
        }

        public string ValidateAndUploadImage(HttpPostedFile postedFile, int userId)
        {
            string profileImageUrl = "";
            string thumbnailUrl = "";

            if (postedFile.ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(postedFile.FileName);

                if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                {
                    WebImage img = new WebImage(postedFile.InputStream);
                    if (postedFile != null)
                    {
                        if ((postedFile.ContentType.Contains("image")))
                        {
                            Users _user = UserServices.GetInstance.GetUserByUserID(userId);
                            string fileName = Guid.NewGuid().ToString();
                            if (img.Width > 300 & img.Height > 300)
                            {
                                img.Resize(300, 300);
                                img.Save(HttpContext.Server.MapPath("~/Images/Profile/")  +fileName + "." + extension);
                                profileImageUrl = null;
                                thumbnailUrl = "/Images/Profile/"  +fileName + "." + extension;
                            }
                            else
                            {
                                img.Save(HttpContext.Server.MapPath("~/Images/Profile/")  +fileName + "." + extension);
                                thumbnailUrl = "/Images/Profile/"  +fileName + "." + extension;
                            }
                            _user.ThumbnailProfileImg = thumbnailUrl;
                            _user.ProfileImg = profileImageUrl;
                            UserServices.GetInstance.UpdateUsers(_user);
                        }
                    }
                    else
                    {
                        thumbnailUrl = null;
                    }
                }
                else
                {
                    thumbnailUrl = null;
                }
               
            }
            return thumbnailUrl;
        }


        private string GetErrorMsg(UserModel _user)
        {
            string errorMsg = "";
            if (_user == null)
            {
                errorMsg = "Email or Password Does not match";
            }
            else if (_user.IsVerified == false)
            {
                errorMsg = "Please verify your Email account. Verification link has been sent on registered Email id";
            }
            else if (_user.UserStatusID != (int)UserStatusEnum.Active)
            {
                errorMsg = "Your account has been suspended. Kindly contact administrator!";
            }
            else
            {
                errorMsg = "Login Failed";
            }
            return errorMsg;
        }

        public ActionResult userProfile(int userId)
        {

            UserProfileModel profile = UserServices.GetInstance.getProfileByUserID(userId);
            profile.LoggedInUserID = AuthHelper.LoggedInUserID;
            return View(profile);
        }

        [HttpGet]
        public ActionResult EditUserProfile(int userId)
        {
            UserProfileModel profile = UserServices.GetInstance.getProfileByUserID(userId);
            return View(profile);
        }

        [HttpPost]
        public ActionResult EditUserProfile(UserProfileModel model)
        {
            HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[0];
            if(postedFile.ContentLength > 0)
                ValidateAndUploadImage(postedFile, model.UserID);
            UserProfile profile = new UserProfile();

            profile.UserID = model.UserID;
            profile.UserProfileID = model.UserProfileID;
            if(model.DOB.Year >1000)
            profile.DOB = model.DOB;
            profile.Gender = model.Gender;
            profile.HomeAddress = model.HomeAddress;
            profile.Jamaat = model.Jamaat;
            profile.PhoneNo = model.PhoneNo;
            profile.Specialisation = model.Specialisation;
            UserServices.GetInstance.UpdateUserProfile(profile);

            Users user = UserServices.GetInstance.GetUserByUserID(model.UserID);
            user.EjamatID = model.EjamatID;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            UserServices.GetInstance.UpdateUsers(user);

            return Redirect("/account/UserProfile?userId=1");
        }

        [HttpGet]
        public ActionResult ClearProfile(int userId)
        {
            ClearUserProfileModel model = new ClearUserProfileModel();
            ViewBag.message = "Are you sure you wish to clear your profile info?";
            model.UserID = userId;
            return View(model);
        }

        [HttpPost]
        public ActionResult ClearProfile(ClearUserProfileModel model)
        {
            UserProfile profile = new UserProfile();
            profile = UserServices.GetInstance.GetUserProfileByUserID(model.UserID);
            profile.DOB = (DateTime?) null;
            profile.Gender = "";
            profile.HomeAddress = "";
            profile.Jamaat = "";
            profile.PhoneNo = "";
            profile.Jamaat = "";
            profile.Specialisation = "";
            UserServices.GetInstance.UpdateUserProfile(profile);
            return Redirect("/account/UserProfile?userId=1");
        }

        [HttpGet]
        public ActionResult SendMessage(int fromUserId, int toUserId)
        {
            SendMessageModel model = new SendMessageModel();
            model.FromUserId = fromUserId;
            model.ToUserId = toUserId;
            return View(model);
        }

        [HttpPost]
        public ActionResult SendMessage(SendMessageModel model)
        {
            UserMessage usermessage = new UserMessage();
            usermessage.TO_UserID = model.ToUserId;
            usermessage.From_UserID = model.FromUserId;
            usermessage.UserSubject = model.subject;
            usermessage.UserContent = model.Content;
            usermessage.SentOn = DateTime.Now;
            int userMessageId = UserServices.GetInstance.InsertUserMessages(usermessage);

            Users userFrom = UserServices.GetInstance.GetUserByUserID(model.FromUserId);
            Users userTO = UserServices.GetInstance.GetUserByUserID(model.ToUserId);
            UserServices.GetInstance.SetEmailTokenUserMessage(userTO, string.Format("{0}{1}", "New Message from - ", userFrom.FirstName), model.Content, (int)EmailTemplateEnum.UserToUserMessage, userFrom, userMessageId);
            try
            {
                EmailService es = new EmailService();
                es.SendSimpleEmails();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect(string.Format("{0}{1}", "/account/UserProfile?userId=", model.ToUserId));
        }

    }
}

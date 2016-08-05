using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.Helper;
using Argaam.FM.Users.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        private string GetErrorForLogin(UserModel _user)
        {
            string Error = "";

            if (_user.UserStatusID != (int)UserStatusEnum.Active)
                Error = "Your account has been Blocked!";

            if (_user.IsVerified == false)
                Error = "Your account is not verified. Please contact administrator!";

            if ((_user.RoleID != (int)UserRoleEnum.SuperAdmin) && (_user.RoleID != (int)UserRoleEnum.Admin))
                Error = "You Do not have Control Panel Access!";
            return Error;
        }

        [HttpGet]
        public ActionResult Register()
        {
            UserModel _user = AuthHelper.LoginFromCookie();
            if (_user != null && _user.RoleID == (int)UserRoleEnum.SuperAdmin && _user.UserStatusID == (int)UserStatusEnum.Active)
            {
                _user = new UserModel();
                _user.UserList = UserServices.GetInstance.GetUsersList();
                return View(_user);
            }
            else
            {
                return RedirectToAction("ManagementArticles", "Home");
            }

        }


        [HttpPost]
        public ActionResult Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                Users _user = new Users();
                _user = UserServices.GetInstance.GetUserByEjamatID(model.EjamatID);
                if (_user == null)
                {
                    _user = new Users();
                    _user.EjamatID = model.EjamatID;
                    _user.Password = ExtensionMethodsCrypography.Encrypt(model.Password);
                    _user.FirstName = model.FirstName;
                    _user.LastName = model.LastName;
                    _user.Email = model.Email;
                    _user.UserGUID = Guid.NewGuid().ToString();
                    _user.CreatedOn = DateTime.Now;
                    _user.IsVerified = true;
                    _user.UserStatusID = (int)UserStatusEnum.Active;
                    _user.RoleID = model.RoleID;

                    UserServices.GetInstance.InsertIntoUsers(_user);
                }
                else
                {
                    ViewBag.Error = "The User with EjamatID Exists.";
                    return View();
                }
                return RedirectToAction("ManagementArticles", "Home");
            }
            return View();
        }

        public ActionResult LoginUserInfoSection()
        {
            UserModel usr = new UserModel();
            usr = AuthHelper.LoginFromCookie();
            if (usr != null && usr.UserStatusID == (int)UserStatusEnum.Active)
            {
                SessionManager.SessionUserObject = usr;
            }
            else
            {
                SessionManager.SessionUserObject = null;
                usr = null;
            }


            return PartialView(usr);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel _user = new UserModel();
                _user = UserServices.GetInstance.GetCPUserForLogin(model.Email, ExtensionMethodsCrypography.Encrypt(model.Password));

                if (_user != null)
                {
                    if ((_user.RoleID != (int)UserRoleEnum.SuperAdmin) || (_user.RoleID != (int)UserRoleEnum.Admin) && _user.IsVerified && _user.UserStatusID != (int)UserStatusEnum.InActive)
                    {
                        AuthHelper.AddSSOCookieIfNotExits(_user);
                        return RedirectToAction("ManagementArticles", "Home");
                    }
                    else
                    {
                        ViewBag.Error = GetErrorForLogin(_user);
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Incorrect Email or Password.";
                    return View();
                }

            }
            else
            {
                ViewBag.Error = "Internal Server Error";
                return View();
            }

        }


        public ActionResult LogOff()
        {
            AuthHelper.ExpireSSOCookie();

            return RedirectToAction("ManagementArticles", "Home");
        }

        public ActionResult AccessMenu()
        {
            UserRoleModel _roles = new UserRoleModel();

            UserModel user = AuthHelper.LoginFromCookie();
            if (user != null)
                _roles = UserServices.GetInstance.GetRoleByUserRoleID(user.RoleID);

            return PartialView("~/Views/Shared/sharedRoleBasedAccess.cshtml", _roles);
        }

        public int UserVerification(int UserId)
        {
            int retval = 0;
            Users _user = UserServices.GetInstance.GetUserByUserID(UserId);
            if (_user.IsVerified == true)
            {
                _user.IsVerified = false;
            }

            else
            {
                _user.IsVerified = true;
            }

            UserServices.GetInstance.UpdateUsers(_user);
            return retval;
        }

        public int Status(int UserId)
        {
            int retval = 0;
            Users _user = UserServices.GetInstance.GetUserByUserID(UserId);
            if (_user.UserStatusID == 1)
            {
                _user.UserStatusID = 2;
            }
            else
            {
                _user.UserStatusID = 1;
            }
            UserServices.GetInstance.UpdateUsers(_user);
            return retval;
        }

        public int RoleChange(int UserId, int RoleID)
        {
            int retval = 0;
            Users _user = UserServices.GetInstance.GetUserByUserID(UserId);
            _user.RoleID = RoleID;
            UserServices.GetInstance.UpdateUsers(_user);
            return retval;

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

        public int UpdateUser(int Id, int ejamaat, string name, string email)
        {
            int retval = 0;
            string EmailExists = "";
            string EjamaatExists = "";
            string firstName = "";
            if (!string.IsNullOrEmpty(email))
            {
                EmailExists = IsEmailExist(email);
            }
            if (!string.IsNullOrEmpty(name))
            {
                firstName = "0";
            }
            if (ejamaat > 0)
            {
                EjamaatExists = IsEjamaatExist(ejamaat);
            }
            Users _user = UserServices.GetInstance.GetUserByUserID(Id);
            if (_user != null)
            {
                if (EjamaatExists != "1" && EmailExists != "1")
                {
                    if (EjamaatExists != "")
                        _user.EjamatID = ejamaat;
                    if (firstName == "0")
                        _user.FirstName = name;
                    if (EmailExists != "")
                        _user.Email = email;
                    UserServices.GetInstance.UpdateUsers(_user);
                    retval = 1;
                }
            }
            

            return retval;
        }

        //private string UpdateAndValidateCurrentImage(String fileUploadId)
        //{
        //    string profileImageUrl = "";
        //    string thumbnailUrl = "";
        //    HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[fileUploadId];

        //    if (postedFile.ContentLength > 0)
        //    {
        //        WebImage img = new WebImage(postedFile.InputStream);
        //        if (postedFile != null)
        //        {
        //            if ((postedFile.ContentType.Contains("image")))
        //            {

        //                if (img.Width > 600 & img.Height > 600)
        //                {
        //                    img.Resize(600, 600);
        //                    img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + "Large" + postedFile.FileName);
        //                    img.Resize(300, 300);
        //                    img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + postedFile.FileName);
        //                    profileImageUrl = "/Images/Articles/" + "Large" + postedFile.FileName;
        //                    thumbnailUrl = "/Images/Articles/" + postedFile.FileName;
        //                }
        //                else
        //                {
        //                    img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + postedFile.FileName);
        //                    thumbnailUrl = "/Images/Articles/" + postedFile.FileName;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            thumbnailUrl = null;
        //        }
        //    }
        //    else
        //    {
        //        thumbnailUrl = null;
        //    }
        //    return thumbnailUrl;

        //}
    }

}

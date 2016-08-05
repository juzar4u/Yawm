using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AkhbaarAlYawm.Application.Helper
{
    public static class AuthHelper
    {
        private static readonly string SSOCookieEmailKey = "emailid";
        private static readonly string SSOCookieUserIDKey = "userid";
        private static readonly string SSOCookieCryptKey = "key";
        private static string SSOCookieName = ApplicationConstants.CON_SSOCookieName;
        private static string SSOCookieKeySalt = ApplicationConstants.CON_SSOCookieKeySalt;
        private static string SSOCookieDomain = ApplicationConstants.CON_SSOCookieDomain;

        private static Dictionary<string, string> cookiesvalue = new Dictionary<string, string>();
        public static UserModel LoginFromCookie()
        {
            if (AuthenticateSSOCookie())
            {
                HttpCookie ssoCookie = HttpContext.Current.Request.Cookies[SSOCookieName];
                if (ssoCookie != null)
                {
                    string cookie = HttpContext.Current.Request.Cookies["AkhbaarSSO"].Value.ToString();

                    string[] cookieNew = (cookie).Split('&');

                    string emailid = cookieNew[0].Split('=').Last();
                    int userId = Convert.ToInt32(cookieNew[1].Split('=').Last());
                    return StoreUserToSessionEntity(userId, emailid);
                }
            }
            return null;
        }

        public static string logout()
        {

            return null;
        }
        public static UserModel StoreUserToSessionEntity(int userId, string emailid)
        {
            //bool retVal = false;
            UserModel akhbaarUser = null;

            if (!string.IsNullOrEmpty(emailid))
            {
                akhbaarUser = UserServices.GetInstance.GetUserModelByEmailID(emailid);
            }
            /*else if (userId > 0)
            {
                argaamUser = ArgaamServices.Instance.GetArgaamUserByUserId(userId);
            }*/

            if (akhbaarUser != null)
            {
                SessionManager.SessionUserObject = akhbaarUser;
                //retVal = true;
            }
            return akhbaarUser;
        }

        private static bool CheckIfCookieKeyIsValid(HttpCookie ssoCookie)
        {
            string emailAddress = HttpUtility.UrlDecode(ssoCookie.Values[SSOCookieEmailKey]);
            string key = ssoCookie.Values[SSOCookieCryptKey];

            if (GenerateSHA1Cipher(emailAddress, SSOCookieKeySalt).Equals(key))
            {
                return true;
            }

            return false;
        }

        private static bool AuthenticateSSOCookie()
        {
            bool returnValue = false;

            if (HttpContext.Current.Request.Cookies[SSOCookieName] != null)
            {
                HttpCookie ssoCookie = HttpContext.Current.Request.Cookies[SSOCookieName];

                if (ssoCookie.Values.Count > 0)
                {
                    if (ssoCookie.Values.AllKeys.Contains(SSOCookieUserIDKey) &&
                        ssoCookie.Values.AllKeys.Contains(SSOCookieEmailKey) &&
                        ssoCookie.Values.AllKeys.Contains(SSOCookieCryptKey))
                    {
                        if (!(ssoCookie.Values[SSOCookieUserIDKey] == null || int.Parse(ssoCookie.Values[SSOCookieUserIDKey]) == 0))
                        {
                            returnValue = CheckIfCookieKeyIsValid(ssoCookie);
                        }
                    }
                    else if (ssoCookie.Values.AllKeys.Contains(SSOCookieEmailKey) &&
                        ssoCookie.Values.AllKeys.Contains(SSOCookieCryptKey))
                    {
                        returnValue = CheckIfCookieKeyIsValid(ssoCookie);
                    }
                }
            }

            return returnValue;
        }
        private static void AddSSOCookie(int? userID, string emailAddress, bool createPersistentCookie)
        {
            HttpCookie ssoCookie = new HttpCookie(SSOCookieName);
            ssoCookie.Values.Add(SSOCookieEmailKey, emailAddress);
            if (userID > 0)
            {
                ssoCookie.Values.Add(SSOCookieUserIDKey, userID.ToString());
            }
            ssoCookie.Values.Add(SSOCookieCryptKey, GenerateSHA1Cipher(emailAddress, SSOCookieKeySalt));

            if (createPersistentCookie)
            {
                ssoCookie.Expires = DateTime.Now.AddMonths(1);
            }
            else
            {
                ssoCookie.Expires = DateTime.Now.AddDays(1);
            }

            if (!ApplicationConstants.UseDomainlessCookie)
            {
                ssoCookie.Domain = SSOCookieDomain;
            }

            HttpContext.Current.Response.Cookies.Add(ssoCookie);
        }

        public static void AddSSOCookieIfNotExits(UserModel user)
        {
            if (!AuthenticateSSOCookie())
            {
                AddSSOCookie(user.UserID, user.Email, true);
                StoreUserToSessionEntity((int)user.UserID, user.Email);
            }
        }

        public static string GenerateSHA1Cipher(string stringToBeEncrytped, string salt)
        {
            //string aSalt = ConfigurationManager.AppSettings["hashSalt"];

            System.Security.Cryptography.SHA1 mcsp = System.Security.Cryptography.SHA1.Create();
            byte[] barr = System.Text.ASCIIEncoding.ASCII.GetBytes(string.Concat(stringToBeEncrytped, salt));
            byte[] barr2 = mcsp.ComputeHash(barr);

            StringBuilder strBuild = new StringBuilder();

            for (int i = 0; i < barr2.Length; i++)
            {
                strBuild.Append(barr2[i].ToString(@"x2"));
            }

            return strBuild.ToString();
        }

        public static void StoreUserToSessionEntity(string email)
        {
            UserModel user = UserServices.GetInstance.GetUserModelByEmailID(email);

            if (user != null)
            {
                
                SessionManager.SessionUserObject = user;
            }

        }

        public static void ExpireSSOCookie()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[SSOCookieName] != null)
                {
                    HttpCookie cookie = new HttpCookie(SSOCookieName);

                    cookie.Value = null;


                    cookie.Expires = DateTime.Now.AddYears(-20);

                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
            catch (Exception ex)
            {
                throw (new ApplicationException(string.Format("Exception - SSOCookieHelper -> ExpireSSOCookie : {0}", ex.Message)));
            }
        }

        public static int? LoggedInUserID
        {
            get
            {
                int? retVal = 0;

                if(AuthenticateSSOCookie())
                {
                    if (SessionManager.SessionUserObject != null)
                    {
                        retVal = SessionManager.SessionUserObject.UserID;
                    }

                }
                else
                {
                    if (SessionManager.SessionUserObject != null)
                    {
                        SessionManager.SessionUserObject = null;
                    }
                }
                return retVal;
            }
        }

        public static bool IsUserIsLoggedIn
        {
            get
            {
                return (LoggedInUserID > 0);
            }
        }


    }
}

using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.Web.API.Services.Services;
using Argaam.FM.Users.Application;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AkhbaarAlYamn.Web.API.Controllers
{
    public class UserController : ApiController
    {
        //public HttpResponseMessage GetDeviceRegistrationAndInsert(string type, string version, string osVersion)
        //{
        //    HttpResponseMessage response = null;

        //    try
        //    {

        //        DeviceRegistration _device = new DeviceRegistration();

                
        //        _device.Type = type;
        //        _device.Token = CalculateMD5Hash((Guid.NewGuid()).ToString().Replace("-", "") + DateTime.Now.Ticks.ToString());
        //        _device.Version = version;
        //        _device.OSVersion = osVersion;
        //        //_device.IPAddress = jObj["IPAddress"].Value<string>();
        //        _device.RegisteredOn = DateTime.Now;
        //        //_device.IsNewFollowerRequireNotification = jObj["IsNewFollowerRequireNotification"].Value<bool>();
        //        _device.UserID = null;
        //        string errorMsg = string.Empty;

        //        DeviceRegistration _deviceCheck = UserServices.GetInstance.GetDeviceSpecificDetails(_device.Type, _device.Version, _device.OSVersion);

        //        _device = UserServices.GetInstance.InsertDeviceInDB(_device.Type, _device.Token, _device.Version, _device.OSVersion, _device.IPAddress, _device.RegisteredOn, _device.IsNewFollowerRequireNotification, _device.UserID);

        //        if (_device == null)
        //        {
        //            _device = new DeviceRegistration();

        //            _device.Type = "";
        //            _device.Token = "";
        //            _device.Version = "";
        //            _device.OSVersion = "";
        //            _device.IPAddress = "";
        //            _device.RegisteredOn = null;
        //            _device.IsNewFollowerRequireNotification = false;
        //            _device.UserID = 0;
        //            errorMsg = "Error";
        //            response = this.Request.CreateResponse(HttpStatusCode.Created, new { Type = _device.Type, Token = _device.Token, Version = _device.Version, OSVersion = _device.OSVersion, IPAddress = _device.IPAddress, RegisteredOn = _device.RegisteredOn, IsNewFollowerRequireNotification = _device.IsNewFollowerRequireNotification, UserID = _device.UserID, ErrorMsg = errorMsg });

        //        }
        //        else
        //        {
        //            errorMsg = "Successfull Device Registration";

        //            response = this.Request.CreateResponse(HttpStatusCode.Created, new { Type = _device.Type, Token = _device.Token, Version = _device.Version, OSVersion = _device.OSVersion, IPAddress = _device.IPAddress, RegisteredOn = _device.RegisteredOn, IsNewFollowerRequireNotification = _device.IsNewFollowerRequireNotification, UserID = _device.UserID, ErrorMsg = errorMsg });

        //        }

        //    }



        //    catch (Exception ex)
        //    {
        //        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //    }



        //    return response;
        //}
        //private string CalculateMD5Hash(string input)
        //{
        //    // step 1, calculate MD5 hash from input
        //    MD5 md5 = System.Security.Cryptography.MD5.Create();
        //    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        //    byte[] hash = md5.ComputeHash(inputBytes);

        //    // step 2, convert byte array to hex string
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < hash.Length; i++)
        //    {
        //        sb.Append(hash[i].ToString("X2"));
        //    }
        //    return sb.ToString();
        //}

        //public HttpResponseMessage PostUserDataAndRegister([FromBody]dynamic data)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        string deviceToken = Request.Headers.GetValues("DeviceToken").First();
        //        try
        //        {
        //            DeviceRegistration _device = UserServices.GetInstance.GetDeviceDetailsByToken(deviceToken);

        //            if (_device != null)
        //            {
        //                Users _user = new Users();

        //                var jObj = (JObject)data;
        //                string errorMsg = "";

        //                _user.UserName = jObj["UserName"].Value<string>();
        //                _user.UserPassword = ExtensionMethodsCrypography.Encrypt(jObj["UserPassword"].Value<string>());
        //                _user.FirstName = jObj["FirstName"].Value<string>();
        //                _user.LastName = jObj["LastName"].Value<string>();
        //                _user.UserDescription = jObj["UserDescription"].Value<string>();
        //                _user.PictureUrl = jObj["PictureUrl"].Value<string>();
        //                _user.UserStatusID = 1;
        //                _user.CreatedOn = DateTime.Now;
        //                _user.IsSocialPluginUser = jObj["IsSocialPluginUser"].Value<bool>();
        //                _user.ProviderName = jObj["ProviderName"].Value<string>();
        //                _user.ProviderID = jObj["ProviderID"].Value<string>();

        //                Users _userCheck = UserServices.GetInstance.GetUserByUserName(_user.UserName);


        //                if (_userCheck == null)
        //                {



        //                    _user = UserServices.GetInstance.InsertUserInDB(
        //                        _user.UserName,
        //                        _user.UserPassword,
        //                        _user.FirstName,
        //                        _user.LastName,
        //                        _user.UserDescription,
        //                        _user.PictureUrl,
        //                        _user.UserStatusID,
        //                        _user.CreatedOn,
        //                        _user.IsSocialPluginUser,
        //                        _user.ProviderName,
        //                        _user.ProviderID

        //                        );

                           
        //                    if (_user == null)
        //                    {
        //                        _user = new Users();

        //                        errorMsg = "Error";
        //                        _user.UserName = "";
        //                        _user.UserPassword = "";
        //                        _user.FirstName = "";
        //                        _user.LastName = "";
        //                        _user.UserDescription = "";
        //                        _user.PictureUrl = "";
        //                        _user.UserStatusID = 0;
        //                        _user.CreatedOn = DateTime.Now;
        //                        _user.IsSocialPluginUser = false;
        //                        _user.ProviderName = "";
        //                        _user.ProviderID = "";

        //                        response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                        {
        //                            UserName = _user.UserName,
        //                            UserPassword = _user.UserPassword,
        //                            FirstName = _user.FirstName,
        //                            LastName = _user.LastName,
        //                            UserDescription = _user.UserDescription,
        //                            PictureUrl = _user.PictureUrl,
        //                            UserStatusID = _user.UserStatusID,
        //                            CreatedOn = _user.CreatedOn,
        //                            IsSocialPluginUser = _user.IsSocialPluginUser,
        //                            ProviderName = _user.ProviderName,
        //                            ProviderID = _user.ProviderID,
        //                            Error = errorMsg

        //                        });

        //                    }
        //                    else
        //                    {
        //                        errorMsg = "Success";
        //                        response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                        {
        //                            UserName = _user.UserName,
        //                            UserPassword = _user.UserPassword,
        //                            FirstName = _user.FirstName,
        //                            LastName = _user.LastName,
        //                            UserDescription = _user.UserDescription,
        //                            PictureUrl = _user.PictureUrl,
        //                            UserStatusID = _user.UserStatusID,
        //                            CreatedOn = _user.CreatedOn,
        //                            IsSocialPluginUser = _user.IsSocialPluginUser,
        //                            ProviderName = _user.ProviderName,
        //                            ProviderID = _user.ProviderID,
        //                            Error = errorMsg
        //                        });

        //                    }
        //                }
        //                else
        //                {
        //                    string StatusCode = "400";
        //                    string StatusMessage = "User Already Exists";
        //                    string Data = null;
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                    {
        //                        StatusCode = StatusCode,
        //                        StatusMessage = StatusMessage,
        //                        Data = Data
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                string StatusCode = "500";
        //                string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //                string Data = null;
        //                response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                {
        //                    StatusCode = StatusCode,
        //                    StatusMessage = StatusMessage,
        //                    Data = Data
        //                });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string StatusCode = "500";
        //        string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //        string Data = null;
        //        response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //        {
        //            StatusCode = StatusCode,
        //            StatusMessage = StatusMessage,
        //            Data = Data
        //        });
        //    }

        //    return response;
        //}

        //public HttpResponseMessage PostLogin([FromBody]dynamic data)
        //{

        //    HttpResponseMessage response = null;

        //    try
        //    {
        //        string deviceToken = Request.Headers.GetValues("DeviceToken").First();


        //        try
        //        {
        //            Users _user = new Users();
        //            DeviceRegistration _device = UserServices.GetInstance.GetDeviceDetailsByToken(deviceToken);
        //            if (_device != null)
        //            {
        //                var jObj = (JObject)data;

        //                _user.UserName = jObj["UserName"].Value<string>();
        //                _user.UserPassword = ExtensionMethodsCrypography.Encrypt(jObj["UserPassword"].Value<string>());
        //                string errorMsg = string.Empty;
        //                _user = UserServices.GetInstance.GetUserForLogin(_user.UserName, _user.UserPassword);

        //                if (_user == null)
        //                {
        //                    _user = new Users();

        //                    _user.UserName = "";
        //                    _user.UserPassword = "";
        //                    errorMsg = "Wrong Username or Password";
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new { UserName = _user.UserName, UserPassword = _user.UserPassword, ErrorMsg = errorMsg });

        //                }
        //                else
        //                {
        //                    if (_device != null)
        //                    {
        //                        UserServices.GetInstance.UpdateDeviceRegistration(deviceToken, _user.UserID);
        //                    }
        //                    errorMsg = "Login Successfull";
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new { UserName = _user.UserName, UserPassword = _user.UserPassword, ErrorMsg = errorMsg });

        //                }
        //            }
        //            else
        //            {
        //                string StatusCode = "500";
        //                string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //                string Data = null;
        //                response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                {
        //                    StatusCode = StatusCode,
        //                    StatusMessage = StatusMessage,
        //                    Data = Data
        //                });
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string StatusCode = "500";
        //        string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //        string Data = null;
        //        response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //        {
        //            StatusCode = StatusCode,
        //            StatusMessage = StatusMessage,
        //            Data = Data
        //        });
        //    }
        //    return response;
        //}

        //public HttpResponseMessage PostLogout([FromBody]dynamic data)
        //{

        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        string deviceToken = Request.Headers.GetValues("DeviceToken").First();



        //        try
        //        {
        //            Users _user = new Users();
        //            DeviceRegistration _device = UserServices.GetInstance.GetDeviceDetailsByToken(deviceToken);
        //            if (_device != null)
        //            {
        //                var jObj = (JObject)data;

        //                _user.UserID = jObj["UserID"].Value<int>();
        //                string errorMsg = string.Empty;
        //                _user = UserServices.GetInstance.GetUserByUserId(_user.UserID);

        //                if (_user == null)
        //                {
        //                    _user = new Users();

        //                    _user.UserName = "";
        //                    _user.UserPassword = "";
        //                    errorMsg = "Error";
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new { UserName = _user.UserName, UserPassword = _user.UserPassword, ErrorMsg = errorMsg });

        //                }
        //                else
        //                {
        //                    if (_device != null)
        //                    {
        //                        UserServices.GetInstance.UpdateDeviceRegistration(deviceToken, null);
        //                    }
        //                    errorMsg = "Logout Successfull";
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new { UserName = _user.UserName, UserPassword = _user.UserPassword, ErrorMsg = errorMsg });

        //                }
        //            }
        //            else
        //            {
        //                string StatusCode = "500";
        //                string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //                string Data = null;
        //                response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                {
        //                    StatusCode = StatusCode,
        //                    StatusMessage = StatusMessage,
        //                    Data = Data
        //                });
        //            }
        //            //response = this.Request.CreateResponse(HttpStatusCode.Created, new { CampaignID = data });

        //        }
        //        catch (Exception ex)
        //        {
        //            response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string StatusCode = "500";
        //        string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //        string Data = null;
        //        response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //        {
        //            StatusCode = StatusCode,
        //            StatusMessage = StatusMessage,
        //            Data = Data
        //        });
        //    }
        //    return response;
        //}

        //public HttpResponseMessage PostForgotPassword([FromBody]dynamic data)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        string deviceToken = Request.Headers.GetValues("DeviceToken").First();

        //        try
        //        {
        //            DeviceRegistration _device = UserServices.GetInstance.GetDeviceDetailsByToken(deviceToken);
        //            if (_device != null)
        //            {
        //                Users _user = new Users();

        //                var jObj = (JObject)data;

        //                _user.UserName = jObj["UserName"].Value<string>();

        //                string errorMsg = string.Empty;
        //                _user = UserServices.GetInstance.GetUserByUserName(_user.UserName);

        //                if (_user == null)
        //                {
        //                    _user = new Users();

        //                    _user.UserName = "";
        //                    _user.UserPassword = "";
        //                    errorMsg = "Wrong Username";
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new { UserName = _user.UserName, UserPassword = _user.UserPassword, ErrorMsg = errorMsg });

        //                }
        //                else
        //                {
        //                    errorMsg = "User Details Successfully Emailed";
        //                    response = this.Request.CreateResponse(HttpStatusCode.Created, new { UserName = _user.UserName, UserPassword = ExtensionMethodsCrypography.Decrypt(_user.UserPassword), ErrorMsg = errorMsg });

        //                }
        //            }
        //            else
        //            {
        //                string StatusCode = "500";
        //                string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //                string Data = null;
        //                response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //                {
        //                    StatusCode = StatusCode,
        //                    StatusMessage = StatusMessage,
        //                    Data = Data
        //                });
        //            }

        //            //response = this.Request.CreateResponse(HttpStatusCode.Created, new { CampaignID = data });

        //        }
        //        catch (Exception ex)
        //        {
        //            response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string StatusCode = "500";
        //        string StatusMessage = "Unauthorized: DeviceToken is missing or invalid.";
        //        string Data = null;
        //        response = this.Request.CreateResponse(HttpStatusCode.Created, new
        //        {
        //            StatusCode = StatusCode,
        //            StatusMessage = StatusMessage,
        //            Data = Data
        //        });
        //    }
        //    return response;
        //}

    }
}

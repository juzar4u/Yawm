using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Web.API.Services.Services
{
    public class UserServices
    {
        private static UserServices _instace;

        public static UserServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new UserServices();
                }

                return _instace;
            }
        }

        public int UpdateDeviceRegistration(string token, int? userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                DeviceRegistration _deviceNew = GetDeviceDetailsByToken(token);
                _deviceNew.UserID = userId;
                return (int)context.Update(_deviceNew);
            }
        }

        public DeviceRegistration GetDeviceSpecificDetails(string type, string version, string osversion)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<DeviceRegistration>("select * from DeviceRegistration where Type = @0 and Version = @1 and OSVersion = @2", type, version, osversion).FirstOrDefault();
            }
        }

        public DeviceRegistration InsertDeviceInDB(string type, string token, string version, string osversion, string ipaddress, DateTime? registeredOn, bool isNewNotification, int? userId)
        {
            DeviceRegistration _device = GetDeviceDetails(type, token, version, osversion, ipaddress, registeredOn, isNewNotification, userId);

            if (_device == null)
            {

                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                    DeviceRegistration _newDevice = new DeviceRegistration();

                    _newDevice.Type = type;
                    _newDevice.Token = token;
                    _newDevice.Version = version;
                    _newDevice.OSVersion = osversion;
                    _newDevice.IPAddress = ipaddress;
                    _newDevice.RegisteredOn = registeredOn;
                    _newDevice.IsNewFollowerRequireNotification = isNewNotification;
                    _newDevice.UserID = userId;
                    context.Insert(_newDevice);
                    
                    return _newDevice;
                }

            }

            return _device;
        }
        public DeviceRegistration GetDeviceDetails(string type, string token, string version, string osversion, string ipaddress, DateTime? registeredOn, bool isNewNotification, int? userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<DeviceRegistration>("select * from DeviceRegistration where Type = @0 and Token = @1 and Version = @2 and OSVersion = @3 and IPAddress = @4 and RegisteredOn = @5 and IsNewFollowerRequireNotification = @6 and UserID = @7", type, token, version, osversion, ipaddress, registeredOn, isNewNotification, userId).FirstOrDefault();
            }
        }

        public DeviceRegistration GetDeviceDetailsByToken(string token)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<DeviceRegistration>("select * from DeviceRegistration where Token = @0", token).FirstOrDefault();
            }
        }
        public Users GetUserByUserName(string username)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Users>("select * from Users where UserName = @0", username).FirstOrDefault();
            }
        }

        public Users GetUserForLogin(string username, string password)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Users>("select * from Users where UserName = @0 and UserPassword = @1", username, password).FirstOrDefault();
            }
        }

        public Users GetUserByUserId(int userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Users>("select * from Users where UserId = @0", userId).FirstOrDefault();
            }
        }
        //public Users InsertUserInDB(string username, string password, string firstName, string LastName, string Description, string pictureUrl, int userStatus, DateTime createdOn, bool isSocialUser, string providerName, string providerID)
        //{
        //    Users checkUser = GetUserByUserName(username);

        //    if (checkUser == null)
        //    {
        //        using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
        //        {
        //            Users _user = new Users();

        //            _user.UserName = username;
        //            _user.UserPassword = password;
        //            _user.FirstName = firstName;
        //            _user.LastName = LastName;
        //            _user.UserDescription = Description;
        //            _user.PictureUrl = pictureUrl;
        //            _user.UserStatusID = userStatus;
        //            _user.CreatedOn = createdOn;
        //            _user.IsSocialPluginUser = isSocialUser;
        //            _user.ProviderName = providerName;
        //            _user.ProviderID = providerID;

        //            context.Insert(_user);

        //            return _user;
        //        }
        //    }
        //    else
        //    {

        //        return checkUser;
        //    }
        //}


        public UserModel GetUserModelByUserId(int userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserModel>("select * from Users where UserID =  @0", userId).FirstOrDefault();
            }
        }

    }
}

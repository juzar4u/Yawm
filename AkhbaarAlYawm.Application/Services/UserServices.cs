using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
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

        public int InsertIntoUsers(Users _user)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_user);
            }
        }

        public int InsertIntoEmailTokens(EmailTokens _token)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_token);
            }
        }
        public int CreateProfile(UserProfile profile)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(profile);
            }
        }

        public int UpdateUsers(Users _user)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_user);
            }
        }


        public Users GetUserByEjamatID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Users>("select * from Users where EjamatID = @0", id).FirstOrDefault();
            }
        }

        public UserModel GetCPUserForLogin(string email, string password)
        {
            UserModel _user = new UserModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _user = context.Fetch<UserModel>("select * from Users where email = @0 and Password = @1", email, password).FirstOrDefault();
                if(_user !=null)
                _user.RoleName = context.Fetch<string>("select rolename from roles where roleid = @0", _user.RoleID).FirstOrDefault();
            }
            return _user;
        }

        public UserModel GetUserModelByEmailID(string email)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserModel>("select * from Users where Email = @0", email).FirstOrDefault();
            }
        }

        public UserModel LoginPPUserByEmailandPassword(string email, string password)
        {
            UserModel _user = new UserModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _user = context.Fetch<UserModel>("select * from Users where email = @0 and Password = @1", email, password).FirstOrDefault();

            }
            return _user;
        }
        public SessionAkhbaarUserEntity GetSessionAkhbaarUserEntityByEmailID(string email)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<SessionAkhbaarUserEntity>("select * from Users where Email = @0", email).FirstOrDefault();
            }
        }


        public UserRoleModel GetRoleByUserRoleID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserRoleModel>("select * from Roles where roleid = @0", id).FirstOrDefault();
            }
        }

        public List<UserList> GetUsersList()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserList>("select usr.UserID, usr.EjamatID, usr.Password, usr.FirstName, usr.LastName, usr.Email, usr.CreatedOn, usr.IsVerified, stats.Name as UserStatus , roles.RoleName, usr.RoleID, usr.UserStatusID from users usr left join UserStatus stats on stats.UserStatusID = usr.UserStatusID left join roles on roles.RoleID = usr.RoleID");
            }
        }


        public Users GetUserByUserID(int userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Users>("select * from users where userid = @0", userId).FirstOrDefault();
            }
        }

        public Users GetUserByUserIDandGuid(int userId, string guid)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Users>("select * from users where userid = @0 and userguid = @1", userId, guid).FirstOrDefault();
            }
        }

        public UserModel GetUserModelByUserIDandGuid(int userId, string guid)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserModel>("select * from users where userid = @0 and userguid = @1", userId, guid).FirstOrDefault();
            }
        }

        public int IsEjamatExist(int ejamatId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select count(*) from Users where EjamatID = @0", ejamatId).FirstOrDefault();
            }
        }
        public int IsEmailExist(string email)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.ExecuteScalar<int>("select count(0)  from Users where Email = @0", email);
            }
        }

        public EmailTokens SetEmailToken(Users _user, string subject, int templateId)
        {
            EmailTokens _emailToken = new EmailTokens();

            _emailToken.FromName = "Akhbaar-Al-Mumineen";
            _emailToken.FromAddress = "admin@danatev.com";
            _emailToken.ToAddress = _user.Email;
            _emailToken.EmailSubject = subject;
            _emailToken.EmailBody = "";
            _emailToken.EntityID = _user.UserID;
            _emailToken.EntityType = "U";
            _emailToken.EmailPriority = 5;
            _emailToken.EnqueuedOn = DateTime.Now;
            _emailToken.ProcessedOn = null;
            _emailToken.Status = "P";
            _emailToken.EmailTemplateID = templateId;

            InsertIntoEmailTokens(_emailToken);

            return _emailToken;
        }

        public UserProfileModel getProfileByUserID(int userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserProfileModel>("select usrPro.UserProfileID, usrPro.UserID, usrPro.Specialisation, usrPro.PhoneNo, usrPro.Jamaat, usrPro.HomeAddress, usrPro.Gender, usrPro.DOB, usr.Email, usr.FirstName, usr.LastName, usr.ThumbnailProfileImg, usr.EjamatID from UserProfile usrPro left join Users usr on usr.UserID = usrPro.UserID where usrPro.UserID = @0", userId).FirstOrDefault();
            }
        }
    }
}

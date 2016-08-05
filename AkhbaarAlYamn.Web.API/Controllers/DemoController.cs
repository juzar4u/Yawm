using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.Web.API.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AkhbaarAlYamn.Web.API.Controllers
{
    public class DemoController : ApiController
    {
        //
        // GET: /Demo/
        
        public UserModel GetUser()
        {
            UserModel user = new UserModel();

            user = UserServices.GetInstance.GetUserModelByUserId(1022);

            return user;
        }

    }
}

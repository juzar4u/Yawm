using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class APICallController : Controller
    {
        //
        // GET: /APICall/

        public ActionResult demo()
        {
            UserModel user = ArgaamAPIHelper.GetUserData();
            return View();
        }

    }
}

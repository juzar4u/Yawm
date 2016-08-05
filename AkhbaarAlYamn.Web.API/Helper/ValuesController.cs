using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace AkhbaarAlYamn.Web.API.Controllers
{
    public class ValuesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetArticle()
        {
            return View();
        }

        public ActionResult User()
        {
            return View();
        }
    }
}
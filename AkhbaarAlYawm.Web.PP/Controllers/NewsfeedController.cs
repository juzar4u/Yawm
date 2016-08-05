using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class NewsfeedController : Controller
    {
        //
        // GET: /Newsfeed/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostNow()
        {
            ClassifiedModel _classified = new ClassifiedModel();
            _classified.CountryList = PP_ArticleServices.GetInstance.GetCountryList();
            _classified.CurrencyList = PP_ArticleServices.GetInstance.GetCurrencyList();
            return View(_classified);
        }

        public ActionResult GetFormData(int contentID)
        {
            return View();
        }


        


    }
}

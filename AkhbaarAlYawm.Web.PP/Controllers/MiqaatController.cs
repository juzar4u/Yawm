using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class MiqaatController : Controller
    {
        //
        // GET: /Miqaat/

        public ActionResult Index()
        {
            //int pageSize = 10;
            //PPMiqaatListModel _model = new PPMiqaatListModel();
            //PetaPoco.Page<MasterMiqaatModel> entity = MiqaatServices.GetInstance.GetAllMiqaats(pageNo, pageSize);
            
            //if (pageNo > 1)
            //    return PartialView("~/Views/Shared/sharedMiqaatPartial.Mobile.cshtml", entity.Items);
            //else
            //    return View("~/Views/Shared/sharedMasterMiqaatView.cshtml", entity);
            return View("~/Views/Shared/sharedMasterMiqaatView.cshtml");
        }

        [HttpGet]
        public ActionResult TitleDataAutocomplete(string name)
        {
            var albumList = new List<Calender_Events>();
            albumList = MiqaatServices.GetInstance.GetMiqaatByTitleFilter(name);
            var result = (from c in albumList
                          select new
                          {
                              ArticleID = c.Calender_EventID,
                              ArticleData = c.EName
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateListing(int elementId)
        {
            int pageSize = 10;
            PetaPoco.Page<MasterMiqaatModel> entity = MiqaatServices.GetInstance.GetMiqaatByElementID(1, pageSize, elementId);
            return PartialView("~/Views/Miqaat/Index.Mobile.cshtml", entity);
        }

        public ActionResult Display()
        {
            return View();
        }

    }
}

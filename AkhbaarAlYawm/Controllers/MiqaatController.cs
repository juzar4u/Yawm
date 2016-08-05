using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.CommonCode.Helpers;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Controllers
{
    [Authentication]
    public class MiqaatController : Controller
    {
        //
        // GET: /Miqaat/

        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult AddMiqaats(int pageId)
        {
            int pageSize = 10;
            MiqaatIndexModel _model = new MiqaatIndexModel();
            _model.CountryList = ArticleServices.GetInstance.GetAllCountries();
            _model.MiqaatCategoryList = MiqaatServices.GetInstance.GetAllMiqaatCategories();
            _model._calender = MiqaatServices.GetInstance.GetAllMiqaatCalenderList();
            PetaPoco.Page<MasterMiqaatModel> entities = MiqaatServices.GetInstance.GetAllMiqaats(pageId, pageSize);
            _model.events = entities.Items;
            ViewBag.totalCount = Convert.ToInt32(entities.TotalItems);
            ViewBag.currentPage = Convert.ToInt32(entities.CurrentPage);
            return View(_model);
        }

        [HttpPost]
        public ActionResult AddMiqaats(MiqaatIndexModel _model)
        {
            Calender_Events _calender = new Calender_Events();
            Event_Hijri_Mapping _mapping = new Event_Hijri_Mapping();
            HijriBohraCalenderModel _hijri = new HijriBohraCalenderModel();
            List<HijriBohraCalenderModel> _hijriList = new List<HijriBohraCalenderModel>();
            _calender.EName = _model.EName;
            _calender.CityID = _model.CityID;
            _calender.Rank = _model.Rank;
            _calender.ECategoryID = _model.ECategoryID;
            _calender.Calender_EventID = MiqaatServices.GetInstance.insertInCalenderEvents(_calender);
                
            if (_model.DateSpecific == "true")
            {
                _hijri = MiqaatServices.GetInstance.GetHijriBohraCalenderDetailByDayMonthYear(_model.H_Day, _model.H_Month, _model.H_Year);
                _mapping.Calender_EventID = _calender.Calender_EventID;
                _mapping.ID = _hijri.ID;
                MiqaatServices.GetInstance.insertInEvent_Hijri_Mapping(_mapping);
            }
            else
            {
                _hijriList = MiqaatServices.GetInstance.GetHijriBohraCalenderDetailByDayMonth(_model.H_Day, _model.H_Month);
                foreach (var items in _hijriList)
                {
                    _mapping.Calender_EventID = _calender.Calender_EventID;
                    _mapping.ID = items.ID;
                    MiqaatServices.GetInstance.insertInEvent_Hijri_Mapping(_mapping);
                }
            }
            
            return RedirectToAction("AddMiqaats");
        }


        [HttpGet]
        public ActionResult EditMiqaat(int MiqaatID)
        {
            MiqaatIndexModel _model = new MiqaatIndexModel();
            _model = MiqaatServices.GetInstance.GetMiqaatByID(MiqaatID);
            _model.CountryList = ArticleServices.GetInstance.GetAllCountries();
            _model.MiqaatCategoryList = MiqaatServices.GetInstance.GetAllMiqaatCategories();
            _model.DefaultMiqaatCategory = MiqaatServices.GetInstance.GetDefaultMiqaatCategory(MiqaatID);
            if (_model.DefaultMiqaatCategory == null)
            {
                _model.DefaultMiqaatCategory = "Select An Option";
            }

            _model = MiqaatServices.GetInstance.GetCityStateCountryNameByCityID(_model);
           

            return View(_model);
        }


        [HttpPost]
        public ActionResult EditMiqaat(MiqaatIndexModel _model)
        {
            Calender_Events _calender = MiqaatServices.GetInstance.GetCalender_EventByID(_model.Calender_EventID);
            if (_model.CityID != null || _model.CityID > 0)
            {
                _calender.CityID = _model.CityID;
            }
            if (_model.ECategoryID > 0)
            {
                _calender.ECategoryID = _model.ECategoryID;
            }
            _calender.Rank = _model.Rank;
            _calender.EName = _model.EName;

            MiqaatServices.GetInstance.UpdateCalenderEvents(_calender);
            return RedirectToAction("AddMiqaats");
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
            List<MasterMiqaatModel> _articles = new List<MasterMiqaatModel>();
            PetaPoco.Page<MasterMiqaatModel> entities = MiqaatServices.GetInstance.GetMiqaatByElementID(1, 10, elementId);

            return PartialView("MiqaatList", entities.Items);
        }

        [HttpGet]
        public int UpdateHijriDate(int Id, int day, int month, int year)
        {
            int i = 0;
            Event_Hijri_Mapping _mapping = new Event_Hijri_Mapping();
            HijriBohraCalenderModel _calender = new HijriBohraCalenderModel();
            _mapping = MiqaatServices.GetInstance.GetEventHijriMapping(Id);
            _calender = MiqaatServices.GetInstance.GetHijriBohraCalenderDetailByDayMonthYear(day, month, year);

            _mapping.ID = _calender.ID;
            MiqaatServices.GetInstance.UpdateEvent_Hijri_Mapping(_mapping);

            return 1;
        }
    }
}

using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Helper.CacheManager;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using PetaPoco;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
{
    public class MiqaatServices
    {
        private static MiqaatServices _instace;

        public static MiqaatServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new MiqaatServices();
                }

                return _instace;
            }
        }

        public int insertInCalenderEvents(Calender_Events _calender)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_calender);
            }
        }
        public int insertInEvent_Hijri_Mapping(Event_Hijri_Mapping _mapping)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_mapping);
            }
        }

        public int UpdateEvent_Hijri_Mapping(Event_Hijri_Mapping _mapping)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_mapping);
            }
        }
        public int UpdateCalenderEvents(Calender_Events _calender)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_calender);

            }

        }


        public List<MiqaatCategoryModel> GetAllMiqaatCategories()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<MiqaatCategoryModel>("select * from eventcategories");
            }
        }
        public List<HijriBohraCalenderModel> GetAllMiqaatCalenderList()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<HijriBohraCalenderModel>("select * from HijriBohra_Gregorian_Calendar");
            }
        }

        public List<HijriBohraCalenderModel> GetAllHijriBohraByEventMappingEventID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<HijriBohraCalenderModel>("select * from HijriBohra_Gregorian_Calendar where ID in (select ID from Event_Hijri_Mapping where Calender_EventID = @0)", id);
            }
        }

        //public List<Event_Hijri_Mapping_Model> GetAllMiqaats()
        //{
        //    HijriBohra_Gregorian_Calendar _calenderList = new HijriBohra_Gregorian_Calendar();
        //    List<Event_Hijri_Mapping_Model> _miqaats = new List<Event_Hijri_Mapping_Model>();
        //    List<MiqaatModel> _orderMiqaats = new List<MiqaatModel>();
        //    using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
        //    {
        //        _miqaats = context.Fetch<Event_Hijri_Mapping_Model>("select * from Event_Hijri_Mapping");
        //        foreach (var items in _miqaats)
        //        {
        //            items.miqaat = GetMiqaatByMiqaatID(items.Calender_EventID, items.ID);
        //            _calenderList = context.Fetch<HijriBohra_Gregorian_Calendar>("select * from HijriBohra_Gregorian_Calendar where ID in (select ID from Event_Hijri_Mapping where Event_Hijri_MapID = @0)", items.Event_Hijri_MapID).FirstOrDefault();

        //            items.HijriYear = _calenderList.H_Year;
        //            items.HijriMonth = _calenderList.H_Month;
        //            items.HijriDay = _calenderList.H_Day;

        //        }
        //    }

        //    return _miqaats;
        //}

        public Page<MasterMiqaatModel> GetAllMiqaats(int pageNo, int pageSize)
        {
            string key = string.Format("GetAllMiqaats#{0}#{1}", pageNo, pageSize);
            Page<MasterMiqaatModel> result = CacheManager.Get(key) as Page<MasterMiqaatModel>;
            if (result == null)
            {
                HijriBohra_Gregorian_Calendar _calenderList = new HijriBohra_Gregorian_Calendar();
                List<Event_Hijri_Mapping_Model> _miqaats = new List<Event_Hijri_Mapping_Model>();
                List<MiqaatModel> _orderMiqaats = new List<MiqaatModel>();
                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                    var ppsql = "select events.Event_Hijri_MapID, events.Calender_EventID,events.ID, calender.H_Day,calender.H_Month, calender.H_Year,calender.DayOfWeek, calender.Gregorian,calevents.Ename as MiqaatName, calevents.Rank, calevents.ECategoryID, calevents.CityID, city.Name,country.Name as CountryName,eventCategories.EName from Event_Hijri_Mapping events left join HijriBohra_Gregorian_Calendar calender on events.ID = calender.ID left join Calender_Events calevents on events.Calender_EventID = calevents.Calender_EventID left join Cities city on calevents.CityID = city.CityID left join EventCategories eventCategories on calevents.ECategoryID = eventCategories.ECategoryID left join States on city.StateID = States.StateID left join Countries country on States.CountryID = country.CountryID order by calender.H_Year ASC, calender.H_Month ASC, calender.H_Day ASC";

                    result = context.Page<MasterMiqaatModel>(pageNo, pageSize, ppsql);
                    foreach (var items in result.Items)
                    {
                        items.FormatedLocation = string.Format("{0}{1}{2}", items.Name, ",", items.CountryName);
                        items.FormatedIslamicDate = string.Format("{0}{1}{2}{3}{4}", items.H_Day, "-", getMonth(items.H_Month), "-", items.H_Year);
                    }

                    CacheManager.Set(key, result, DateTime.Now.AddSeconds(ApplicationConstants.ListCacheTimeInSec));
                    //foreach (var items in _miqaats)
                    //{
                    //    items.miqaat = GetMiqaatByMiqaatID(items.Calender_EventID, items.ID);
                    //    _calenderList = context.Fetch<HijriBohra_Gregorian_Calendar>("select * from HijriBohra_Gregorian_Calendar where ID in (select ID from Event_Hijri_Mapping where Event_Hijri_MapID = @0)", items.Event_Hijri_MapID).FirstOrDefault();

                    //    items.HijriYear = _calenderList.H_Year;
                    //    items.HijriMonth = _calenderList.H_Month;
                    //    items.HijriDay = _calenderList.H_Day;

                    //}
                }
            }

            return (result);
        }

        public Page<MasterMiqaatModel> GetMiqaatByElementID(int pageNo, int pageSize, int elementID)
        {
            string key = string.Format("GetMiqaatByElementID#{0}#{1}#{2}", pageNo, pageSize, elementID);
            Page<MasterMiqaatModel> result = CacheManager.Get(key) as Page<MasterMiqaatModel>;
            if (result == null)
            {
                HijriBohra_Gregorian_Calendar _calenderList = new HijriBohra_Gregorian_Calendar();
                List<Event_Hijri_Mapping_Model> _miqaats = new List<Event_Hijri_Mapping_Model>();
                List<MiqaatModel> _orderMiqaats = new List<MiqaatModel>();
                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                    var ppsql = string.Format("select events.Event_Hijri_MapID, events.Calender_EventID,events.ID, calender.H_Day,calender.H_Month, calender.H_Year,calender.DayOfWeek, calender.Gregorian,calevents.Ename as MiqaatName, calevents.Rank, calevents.ECategoryID, calevents.CityID, city.Name,country.Name as CountryName,eventCategories.EName from Event_Hijri_Mapping events left join HijriBohra_Gregorian_Calendar calender on events.ID = calender.ID left join Calender_Events calevents on events.Calender_EventID = calevents.Calender_EventID left join Cities city on calevents.CityID = city.CityID left join EventCategories eventCategories on calevents.ECategoryID = eventCategories.ECategoryID left join States on city.StateID = States.StateID left join Countries country on States.CountryID = country.CountryID where events.Calender_EventID = {0}", elementID);

                    result = context.Page<MasterMiqaatModel>(pageNo, pageSize, ppsql);
                    foreach (var items in result.Items)
                    {
                        items.FormatedLocation = string.Format("{0}{1}{2}", items.Name, ",", items.CountryName);
                        items.FormatedIslamicDate = string.Format("{0}{1}{2}{3}{4}", items.H_Day, "-", getMonth(items.H_Month), "-", items.H_Year);
                    }
                    CacheManager.Set(key, result, DateTime.Now.AddSeconds(ApplicationConstants.ListCacheTimeInSec));
                }
            }

            return (result);
        }

        public List<Event_Hijri_Mapping_Model> GetAllMiqaatByElementID(int id)
        {
            HijriBohra_Gregorian_Calendar _calenderList = new HijriBohra_Gregorian_Calendar();
            List<Event_Hijri_Mapping_Model> _miqaats = new List<Event_Hijri_Mapping_Model>();
            List<MiqaatModel> _orderMiqaats = new List<MiqaatModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _miqaats = context.Fetch<Event_Hijri_Mapping_Model>("select * from Event_Hijri_Mapping where Calender_EventID = @0", id);
                foreach (var items in _miqaats)
                {
                    items.miqaat = GetMiqaatByMiqaatID(items.Calender_EventID, items.ID);
                    _calenderList = context.Fetch<HijriBohra_Gregorian_Calendar>("select * from HijriBohra_Gregorian_Calendar where ID in (select ID from Event_Hijri_Mapping where Event_Hijri_MapID = @0)", items.Event_Hijri_MapID).FirstOrDefault();

                    items.HijriYear = _calenderList.H_Year;
                    items.HijriMonth = _calenderList.H_Month;
                    items.HijriDay = _calenderList.H_Day;

                }
            }

            return _miqaats;
        }


       

        public MiqaatIndexModel GetMiqaatByID(int id)
        {
            MiqaatIndexModel _miqaat = new MiqaatIndexModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _miqaat = context.Fetch<MiqaatIndexModel>("select * from Calender_Events where Calender_EventID = @0", id).FirstOrDefault();
                _miqaat.miqaats = context.Fetch<Event_Hijri_Mapping_Model>("select * from Event_Hijri_Mapping where Calender_EventID = @0", id);
                foreach (var items in _miqaat.miqaats)
                {
                    items.miqaat = GetMiqaatByMiqaatID(items.Calender_EventID, items.ID);
                    items.HijriYear = context.Fetch<int>("select H_Year from HijriBohra_Gregorian_Calendar where ID in (select ID from Event_Hijri_Mapping where Event_Hijri_MapID = @0)", items.Event_Hijri_MapID).FirstOrDefault();
                    //items.Calender = GetAllHijriBohraByEventMappingEventID(items.Calender_EventID);
                }
            }
            return _miqaat;
        }

        public MiqaatModel GetMiqaatByMiqaatID(int id, int hijriID)
        {
            MiqaatModel _miqaat = new MiqaatModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _miqaat = context.Fetch<MiqaatModel>("select * from Calender_Events where Calender_EventID = @0", id).FirstOrDefault();
                if (_miqaat.EName.Length > 58)
                {
                    _miqaat.EName = string.Format("{0}{1}", _miqaat.EName.Substring(0, 58), "...");
                }
                _miqaat.ECategoryName = context.Fetch<string>("select EName from EventCategories where ECategoryID = @0", _miqaat.ECategoryID).FirstOrDefault();
                _miqaat.CityName = context.Fetch<string>("select Name from Cities where CityID = @0", _miqaat.CityID).FirstOrDefault();
                if (string.IsNullOrEmpty(_miqaat.CityName))
                {
                    _miqaat.CityName = "None";
                }
                else
                {
                    _miqaat.CountryName = context.Fetch<string>("select Name from Countries where CountryID = (select CountryID from States where StateID = (Select StateID from Cities where CityID = @0))", _miqaat.CityID).FirstOrDefault();
                    _miqaat.FormatedLocation = string.Format("{0}{1}{2}", _miqaat.CityName, ",", _miqaat.CountryName);
                }
                _miqaat.Calender = context.Fetch<HijriBohraCalenderModel>("select * from HijriBohra_Gregorian_Calendar where ID = @0", hijriID).FirstOrDefault();
                _miqaat.Calender.formattedIslamicDate = string.Format("{0}{1}{2}{3}{4}", _miqaat.Calender.H_Day, "-", getMonth(_miqaat.Calender.H_Month), "-", _miqaat.Calender.H_Year);
            }
            return _miqaat;
        }
        //public int MultipleMiqaatHijriMapping(int day, int month, int year, string dateSpecific, Calender_Events _calender)
        //{
        //    List<HijriBohra_Gregorian_Calendar> _hijriList = new List<HijriBohra_Gregorian_Calendar>();
        //    Event_Hijri_Mapping _mapping = new Event_Hijri_Mapping();
        //    using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
        //    {
        //        if (dateSpecific == "true")
        //        {
        //            _calender.ID = context.Fetch<int>("select ID from HijriBohra_Gregorian_Calendar where H_Day = @0 and H_Month = @1 and H_Year = @2", day, month, year).FirstOrDefault();
        //            _mapping.Calender_EventID = _calender.Calender_EventID;
        //            _mapping.ID = _calender.ID;
        //            insertInEvent_Hijri_Mapping(_mapping);
        //        }
        //        else
        //        {
        //            _hijriList = context.Fetch<HijriBohra_Gregorian_Calendar>("select * from HijriBohra_Gregorian_Calendar where H_Day = @0 and H_Month = @1", day, month);
        //            foreach (var items in _hijriList)
        //            {

        //                _mapping.Calender_EventID = _calender.Calender_EventID;
        //                _mapping.ID = items.ID;
        //                insertInEvent_Hijri_Mapping(_mapping);
        //            }
        //        }
        //    }

        //    return 1;
        //}

        public List<Calender_Events> GetMiqaatByTitleFilter(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Calender_Events>(String.Format("select * from Calender_Events where EName like '%{0}%'", name));
            }
        }

        public string GetDefaultMiqaatCategory(int miqaatId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Ename from EventCategories where ECategoryID = (select ECategoryID from Calender_Events where Calender_EventID = @0)", miqaatId).FirstOrDefault();
            }
        }

        public MiqaatIndexModel GetCityStateCountryNameByCityID(MiqaatIndexModel _model)
        {
            int stateId;
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _model.CityName = context.Fetch<string>("select Name from Cities where CityID = @0", _model.CityID).FirstOrDefault();
                if (string.IsNullOrEmpty(_model.CityName))
                {
                    _model.CityName = "Select An Option";
                }
                _model.CountryName = context.Fetch<string>("select Name from Countries where CountryID = (select CountryID from States where StateID = (select StateID from Cities where CityID = @0))", _model.CityID).FirstOrDefault();
                if (string.IsNullOrEmpty(_model.CountryName))
                {
                    _model.CountryName = "Select An Option";
                }
                stateId = context.Fetch<int>("select StateID from Cities where CityID = @0", _model.CityID).FirstOrDefault();
                _model.StateName = context.Fetch<string>("select Name from States where StateID = @0", stateId).FirstOrDefault();
                _model.StateID = stateId;

                _model.CountryID = context.Fetch<int>("select CountryID from States where StateID = @0", stateId).FirstOrDefault();

                if (string.IsNullOrEmpty(_model.StateName))
                {
                    _model.StateName = "Select An Option";
                }
            }
            return _model;
        }

        public Calender_Events GetCalender_EventByID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Calender_Events>("select * from Calender_Events where Calender_EventID = @0", id).FirstOrDefault();
            }
        }

        public Event_Hijri_Mapping GetEventHijriMapping(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Event_Hijri_Mapping>("select * from Event_Hijri_Mapping where Event_Hijri_MapID = @0", id).FirstOrDefault();
            }
        }

        public HijriBohraCalenderModel GetHijriBohraCalenderDetailByDayMonthYear(int day, int month, int year)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<HijriBohraCalenderModel>("select * from HijriBohra_Gregorian_Calendar where H_Day = @0 and H_Month = @1 and H_Year = @2", day, month, year).FirstOrDefault();
            }
        }

        public HijriBohraCalenderModel GetGregorianCalenderDetailByDayMonthYear(int day, int month, int year)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<HijriBohraCalenderModel>("select * from HijriBohra_Gregorian_Calendar where G_Day = @0 and G_Month = @1 and G_Year = @2", day, month, year).FirstOrDefault();
            }
        }

        public List<HijriBohraCalenderModel> GetHijriBohraCalenderDetailByDayMonth(int day, int month)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<HijriBohraCalenderModel>("select * from HijriBohra_Gregorian_Calendar where H_Day = @0 and H_Month = @1", day, month);
            }
        }

        public string getMonth(int day)
        {
            string month = "";
            switch (day)
            {
                case 01:
                    month = "Moharrum-ul-Haram";
                    break;

                case 02:
                    month = "Safar-ul-Muzaffar";
                    break;

                case 03:
                    month = "Rabi-ul-Awwal";
                    break;

                case 04:
                    month = "Rabi-ul-Aakhar";
                    break;

                case 05:
                    month = "Jumadil-Ula";
                    break;

                case 06:
                    month = "Jumadil-Ukhra";
                    break;

                case 07:
                    month = "Rajab-ul-Asab";
                    break;

                case 08:
                    month = "Shaban-al-Karim";
                    break;

                case 09:
                    month = "Ramadan-al-Moazzam";
                    break;

                case 10:
                    month = "Shawwal-al-Mukarram";
                    break;

                case 11:
                    month = "Zilqadatil-Haram";
                    break;

                case 12:
                    month = "Zilhajjatil-Haram";
                    break;

                default:
                    return "";
            }

            return month;
        }
    }
}

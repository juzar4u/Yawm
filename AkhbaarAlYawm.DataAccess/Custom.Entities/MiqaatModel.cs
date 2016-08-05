using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities
{

    public class MiqaatIndexModel
    {
        
        public List<HijriBohraCalenderModel> _calender { get; set; }
        public List<Country> CountryList { get; set; }
        public List<MiqaatCategoryModel> MiqaatCategoryList { get; set; }
        public List<Event_Hijri_Mapping_Model> miqaats { get; set; }
        public int Calender_EventID { get; set; }
        public int? CityID { get; set; }
        [Required(ErrorMessage = "Please Select Category")]
        public int ECategoryID { get; set; }
        public int H_Day { get; set; }
        public int H_Month { get; set; }
        public int H_Year { get; set; }
        public int? Rank { get; set; }
        [Required(ErrorMessage="Name is Required")]
        public string EName { get; set; }
        public string DateSpecific { get; set; }
        public string DefaultMiqaatCategory { get; set; }
        public int DefaultDay { get; set; }
        public int DefaultHijri { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public List<MasterMiqaatModel> events { get; set; }

    }
    public class MiqaatModel
    {
        public int Calender_EventID { get; set; }
        public string EName { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string FormatedLocation { get; set; }
        public int Rank { get; set; }
        public int ID { get; set; }
        public HijriBohraCalenderModel Calender { get; set; }
        public int ECategoryID { get; set; }
        public string ECategoryName { get; set; }
    }

    public class Event_Hijri_Mapping_Model
    {
        public int Event_Hijri_MapID { get; set; }
        public int Calender_EventID { get; set; }
        public int ID { get; set; }
        public int HijriYear { get; set; }
        public int HijriMonth { get; set; }
        public int HijriDay { get; set; }
        public MiqaatModel miqaat { get; set; }
        public List<HijriBohraCalenderModel> Calender { get; set; }
    }

    public class HijriBohraCalenderModel
    {
        public int ID { get; set; }
        public string Hijri { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Gregorian { get; set; }
        public string DayOfWeek { get; set; }
        public int H_Day { get; set; }
        public int H_Month { get; set; }
        public int H_Year { get; set; }
        public int G_Day { get; set; }
        public int G_Month { get; set; }
        public int G_Year { get; set; }
        public string formattedIslamicDate { get; set; }
    }

    public class MiqaatCategoryModel
    {
        public int ECategoryID { get; set; }
        public string EName { get; set; }
    }

    public class PPMiqaatListModel
    {
        public List<Event_Hijri_Mapping_Model> miqaats { get; set; }
        public int TotalRecordCount { get; set; }

    }


    public class MasterMiqaatModel
    {
        public int Event_Hijri_MapID { get; set; }
        public int Calender_EventID { get; set; }
        public int ID { get; set; }
        public int H_Day { get; set; }
        public int H_Month { get; set; }
        public int H_Year { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime Gregorian { get; set; }
        public string MiqaatName { get; set; }
        public int Rank { get; set; }
        public int ECategoryID { get; set; }
        public int CityID { get; set; }
        public string Name { get; set; }
        public string Ename { get; set; }
        public string CountryName { get; set; }
        public string FormatedLocation { get; set; }
        public string FormatedIslamicDate { get; set; }
    }
}

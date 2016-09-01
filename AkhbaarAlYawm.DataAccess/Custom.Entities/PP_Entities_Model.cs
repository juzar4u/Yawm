using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model
{
    public class ArticleModel
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int SourceID { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int PublishedBy { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public List<City> CityList { get; set; }
        public int CityID { get; set; }
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public int TotalUploadedImages { get; set; }
        public int TotalUploadedVideos { get; set; }
        public List<Source> SourceList { get; set; }
        public string IsFeaturedNameVideos { get; set; }
        public bool? IsFeaturedVideos { get; set; }
        public string IsFeaturedNameImage { get; set; }
        public bool IsFeaturedImage { get; set; }
        public DateTime PublishedOn { get; set; }
        public string CityName { get; set; }
        public string SourceName { get; set; }
        public string CountryName { get; set; }
        public List<EntityMedia> VideoURL { get; set; }
        public List<EntityMedia> ListUploadedPictures { get; set; }
        public int TotalVideo { get; set; }
        public int TotalNewUploadedVideos { get; set; }
        public int TotalNewUploadedImages { get; set; }
        public string ArticleSelectedCity { get; set; }
        public string ArticleSelectedState { get; set; }
        public bool isFeatured { get; set; }
        public string Featuredtext { get; set; }
        public List<featuredArticleModel> _featuredArticleList { get; set; }
        public CategoryWiseList _nonfeaturedArticleList { get; set; }
        public string Mauze { get; set; }
        public string CurrentQiyaam { get; set; }
        public int HuzuralaAkhbaarCount { get; set; }
        public int SaadatAkhbaarCount { get; set; }
        public int QasreAliAkhbaarCount { get; set; }
        public int BilaadImaniaArticles { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string _newMappedDate { get; set; }
        public string DomainUrl { get; set; }
        public bool IsVideo { get; set; }
    }

    public class ArticleDetailModel
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int SourceID { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int PublishedBy { get; set; }
        public int CityID { get; set; }
        public string SourceName { get; set; }
        public string Mauze { get; set; }
        public List<EntityMedia> VideoURL { get; set; }
        public List<EntityMedia> ListUploadedPictures { get; set; }
        public string FeaturedImage { get; set; }
        public string FeaturedVideo { get; set; }
        public List<AkhbaarIndexModel> _RelatedArticles { get; set; }
        public int CategoryID { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string _newMappedDate { get; set; }
        public SocialSharePlugin SocialSharerPlugin { get; set; }
        public string ShortUrl { get; set; }
        public bool IsVideo { get; set; }
        //public List<CommentMaster> Comments { get; set; }
        //public UserModel User { get; set; }
        public CommentMasterModel ArticleComment { get; set; }
    }


    public class SocialSharePlugin
    {
        public string Title { get; set; }
        public string shareurl { get; set; }
    }
    public class mauze
    {
        public string city { get; set; }
        public string country { get; set; }
    }

    public class featuredArticleModel
    {
        public int ArticleID { get; set; }
        public string Title   { get; set; }
        public string sourceName { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
        public int EntityMediaID { get; set; }
        public int CityID { get; set; }
        public string Mauze { get; set; }
        public int SourceID { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string _newMappedDate { get; set; }
        public bool IsVideo { get; set; }

    }

    public class nonfeaturedArticleModel
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string sourceName { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ImageUrl { get; set; }
        public int CityID { get; set; }
        public string Mauze { get; set; }
        public int CategoryID { get; set; }
        public int SourceID { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string _newMappedDate { get; set; }
        public string AkhbaarName { get; set; }
        public int _articleCount { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsVideo { get; set; }
    }

    public class CategoryWiseList
    {
        public List<nonfeaturedArticleModel> HuzuralaRelatedArticles { get; set; }
        public List<nonfeaturedArticleModel> SaadatkiramArticles { get; set; }
        public List<nonfeaturedArticleModel> QasreAliArticles { get; set; }
        public List<nonfeaturedArticleModel> BilaadImaniaArticles { get; set; }
    }

    
    public class VideoIndexModel
    {
        public Categories _category { get; set; }
        public List<AkhbaarIndexModel> _categoryWiseVideos { get; set; }
        public int AllVideosCount { get; set; }
    }

    public class MiqaatModel
    {
        public int Calender_EventID { get; set; }
        public string EName { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
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
    }

    public class AkhbaarIndexModel
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string sourceName { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CityID { get; set; }
        public string Cityname { get; set; }
        public string CountryName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int SourceID { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string AkhbaarName { get; set; }
        public int articleCount { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsVideo { get; set; }
        public string SourceWebsite { get; set; }
        public EntityMedia Entity  { get; set; }
        public string shortIslamicDate { get; set; }
    }

    public class AkhbaarMasterModel
    {
        public List<Categories> _category { get; set; }
        public string CurrentQiyaam { get; set; }
        public List<AkhbaarIndexModel> FeaturedList { get; set; }
        public List<AkhbaarNonFeatured> nonfeatured { get; set;}
    }

    public class AkhbaarNonFeatured
    {
        public List<AkhbaarIndexModel> nonFeaturedList { get; set; }
        public string AkhbaarName { get; set; }
        public int AkhbaarCount { get; set; }
        public int CategoryID { get; set; }
    }



}

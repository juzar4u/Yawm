using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities
{
    public class ArticleModel
    {
        public int ArticleID { get; set; }

        [Required(ErrorMessage="Title is Required")]
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
        public string IslamicDay { get; set; }
        public string IslamicMonth { get; set; }
        public int IslamicYear { get; set; }
        public int CategoryID { get; set; }
        public bool IsVideo { get; set; }
        public string ImageResolutionError { get; set; }
        public DateTime gregorianDate { get; set; }
        public string radioDateType { get; set; }
    }
}

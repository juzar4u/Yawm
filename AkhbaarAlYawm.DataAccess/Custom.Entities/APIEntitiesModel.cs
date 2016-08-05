using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities.APIEntitiesModel
{

   
    public class ArticleDetailModel
    {
        public int ArticleID { get; set; }
        private string _title;
        public string Title { get { return _title ?? string.Empty; } set { _title = value; } }
        private string _body;
        public string Body { get { return _body ?? string.Empty; } set { _body = value; } }
        public int SourceID { get; set; }
        private string _sourceName;
        public string SourceName { get { return _sourceName ?? string.Empty; } set { _sourceName = value; } }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime PublishedOn { get; set; }
        public int CityID { get; set; }
        private string _cityName;
        public string CityName { get { return _cityName ?? string.Empty; } set { _cityName = value; } }
        private string _featuredUrl;
        public string FeaturedUrl { get { return _featuredUrl ?? string.Empty; } set { _featuredUrl = value; } }

        public bool isFeatured { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string newMappedDate { get; set; }
        public string Mauze { get; set; }
        public bool IsVideo { get; set; }
        public bool IsMultiplePhotos { get; set; }
    }

    public class ArticleList
    {
        public int ArticleID { get; set; }
        public List<ArticleDetailModel> RelatedArticleList { get; set; }

    }
   

    public class EntityMediaModel
    {
        public int EntityMediaID { get; set; }
        public string Url { get; set; }
        private string _thumbnail;
        public string Thumbnail { get { return _thumbnail ?? string.Empty; } set { _thumbnail = value; } }
        public bool isFeatured { get; set; }
    }


    public class ArticleListingModel
    {
        public int ArticleID { get; set; }
        private string _title;
        public string Title { get { return _title ?? string.Empty; } set { _title = value; } }
        public int SourceID { get; set; }
        private string _sourceName;
        public string SourceName { get { return _sourceName ?? string.Empty; } set { _sourceName = value; } }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public int CityID { get; set; }
        private string _cityName;
        public string CityName { get { return _cityName ?? string.Empty; } set { _cityName = value; } }
        public string ThmbnailImageUrl { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string IslamicDate { get; set; }
        public DateTime mappedDate { get; set; }
        public string newMappedDate { get; set; }
        public string Mauze { get; set; }

    }

    public class ArticleListingHomeModel
    {
        public int TotalCount { get; set; }
        public string Category { get; set; }
        public List<ArticleDetailModel> Articles { get; set; }
    }

    public class IndexModel
    {
        public string CurrentQiyaamSharif { get; set; }
        public List<ArticleListingHomeModel> Articles { get; set; }
    }

    
        //public List<ArticleListingModel> SaadatArticles { get; set; }
        //public List<ArticleListingModel> QasreAliArticles { get; set; }
        //public List<ArticleListingModel> BilaadImaniyaArticles { get; set; }
    
}

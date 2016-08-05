using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.APIEntitiesModel;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Web.API.Services.Services
{
    public class ArticleServices
    {
        private static ArticleServices _instace;

        public static ArticleServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new ArticleServices();
                }

                return _instace;
            }
        }

        public int UpdateArticle(Article _article)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_article);
            }
        }

        public List<ArticleDetailModel> GetAllArticles()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleDetailModel>("select * from Articles where IsActive = 1");
            }
        }

        public List<ArticleDetailModel> GetAllNonFeaturedAritcles(int categoryId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleDetailModel>("select * from Articles where IsActive = 1 and isFeatured = 0 and categoryId = @0", categoryId);
            }
        }

        public List<ArticleDetailModel> GetAllFeaturedAritcles()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleDetailModel>("select * from Articles where IsActive = 1 and isFeatured = 1");
            }
        }

       

        public List<EntityMediaModel> GetEntityMediaByArticleID(int articleId, int creativeType, string url)
        {
            List<EntityMediaModel> _EntityMedia = new List<EntityMediaModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _EntityMedia = context.Fetch<EntityMediaModel>("select * from EntityMedia where ArticleID = @0 and CreativeTypeID = @1", articleId, creativeType);

                foreach (var items in _EntityMedia)
                {
                    items.Thumbnail = string.Format("{0}{1}", url, items.Thumbnail);

                    items.Url = string.Format("{0}{1}", url, items.Url);
                }
            }
            return _EntityMedia;
        }

        public List<ArticleDetailModel> GetTop5RelatedArticles(int categoryId, int articleId, string url)
        {
            List<ArticleDetailModel> _mostViewedList = new List<ArticleDetailModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _mostViewedList = context.Fetch<ArticleDetailModel>("select top 5 * from Articles where CategoryID = @0 and ArticleID <> @1 order by ViewCount desc", categoryId, articleId);

                _mostViewedList = CommonCodeForArticleListingModel(_mostViewedList, url);
            }

            return _mostViewedList;
        }
        public ArticleDetailModel GetArticleDetailForListingByArticleId(int articleId, string url)
        {
            ArticleDetailModel _article = new ArticleDetailModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _article = context.Fetch<ArticleDetailModel>("select * from Articles where ArticleID = @0 and IsActive = 1", articleId).FirstOrDefault();
                _article.ViewCount = UpdateViewCount(articleId);
                _article.SourceName = GetSourceNameBySourceId(_article.SourceID);
                _article.Mauze = string.Format("{0}{1}{2}", GetCityByCityID(_article.CityID), " ,", GetCountryByCityID(_article.CityID));

                if (_article.IsVideo == true)
                {
                    _article.FeaturedUrl = string.Format("{0}{1}", url, context.Fetch<string>("select thumbnail from entitymedia where articleid = @0 and isfeatured = 1 and creativetypeid = 2", articleId).FirstOrDefault());
                }
                else
                {
                    _article.FeaturedUrl = string.Format("{0}{1}", url, context.Fetch<string>("select thumbnail from entitymedia where articleid = @0 and isfeatured = 1 and creativetypeid = 1", articleId).FirstOrDefault());
                    
                }

                int count = context.Fetch<int>("select count(*) from entitymedia where articleid = @0 and creativetypeid = 1", articleId).FirstOrDefault();
                _article.IsMultiplePhotos = count > 1 ? true : false;
                _article.CityName = GetCityNameByCityID(_article.CityID);

                _article.CategoryName = context.Fetch<string>("select CategoryNameEn from Categories where CategoryID = @0", _article.CategoryID).FirstOrDefault(); 
                
                _article.newMappedDate = _article.mappedDate.ToString("dd/MM/yyyy");
                return _article;
            }
        }
        public string GetCityByCityID(int cityID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Cities where CityID = @0", cityID).FirstOrDefault();
            }
        }
        public string GetCountryByCityID(int cityID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Countries where CountryID = (select CountryID from States where StateID = (select StateID from Cities where CityID = @0))", cityID).FirstOrDefault();
            }
        }
        
        public string GetSourceNameBySourceId(int sourceId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Sources where SourceID = @0", sourceId).FirstOrDefault();
            }
        }

        public string GetCityNameByCityID(int cityId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Cities where CityID = @0", cityId).FirstOrDefault();
            }
        }
        public Article GetArticle(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Article>("select * from Articles where ArticleID = @0", articleId).FirstOrDefault();
            }
        }
        public int UpdateViewCount(int id)
        {
            Article _article = GetArticle(id);

            _article.ViewCount = _article.ViewCount + 1;

            UpdateArticle(_article);

            return _article.ViewCount;
        }


        public List<ArticleDetailModel> GetTop5ArticlesByCategory(int categoryID, string url)
        {
            List<ArticleDetailModel> _articles = new List<ArticleDetailModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _articles = context.Fetch<ArticleDetailModel>("select top 5 * from Articles where isFeatured = 0 and CategoryID = @0 and (GETDATE() - PublishedOn) <= 31 ORDER BY PublishedOn desc", categoryID);

                _articles = CommonCodeForArticleListingModel(_articles, url);
            }

            return _articles;
        }


        public List<ArticleDetailModel> CommonCodeForArticleListingModel(List<ArticleDetailModel> _articles, string url)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                foreach (var items in _articles)
                {

                    items.CategoryName = context.Fetch<string>("select categorynameen from Categories where CategoryID = @0", items.CategoryID).FirstOrDefault();
                    items.FeaturedUrl = string.Format("{0}{1}", url, context.Fetch<string>("select thumbnail from EntityMedia where ArticleID = @0 and IsFeatured = 1 and CreativeTypeID = 1", items.ArticleID).FirstOrDefault());
                    items.Mauze = string.Format("{0}{1}{2}", GetCityByCityID(items.CityID), " ,", GetCountryByCityID(items.CityID));
                    if (items.Mauze == " ,")
                        items.Mauze = "";
                    items.SourceName = GetSourceNameBySourceId(items.SourceID);
                    items.newMappedDate = items.mappedDate.ToString("dd/MM/yyyy");
                    if (items.FeaturedUrl == null)
                    {
                        items.FeaturedUrl = url + "/Content/Images/NoImageAvailable.jpg";
                    }
                    
                }
            }
            return _articles;
        }


        public int GetArticleCountByCategories(int categoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.ExecuteScalar<int>("select count(*) from Articles where isFeatured = 0 and CategoryID = @0 and (GETDATE() - PublishedOn) <= 31", categoryID);
            }
        }
               
        public string GetCategoryNameByCategoryID(int categoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
               return context.Fetch<string>("select CategoryNameEn from Categories where CategoryID = @0", categoryID).FirstOrDefault();
            }
        }

        public string GetCurrentQiyaam()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select CurrentQiyaamMauze from CurrentQiyaam").FirstOrDefault();
            }
        }

    }
}

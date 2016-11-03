using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Helper.CacheManager;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model;
using PetaPoco;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
{
    public class PP_ArticleServices
    {
        private static PP_ArticleServices _instace;

        public static PP_ArticleServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new PP_ArticleServices();
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

        

        public List<AkhbaarIndexModel> GetAkhbaarIndexModel(bool isFeatured, int CategoryID)
        {
            List<AkhbaarIndexModel> _articleList = new List<AkhbaarIndexModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                if (isFeatured == true)
                {
                    _articleList = context.Fetch<AkhbaarIndexModel>("select article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.isFeatured = 1 and category.IsDeleted = 0 and article.IsActive = 1 order by article.PublishedOn desc");
                }
                else
                {
                    _articleList = context.Fetch<AkhbaarIndexModel>(string.Format("select top 5 article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.isFeatured = 0 and category.IsDeleted = 0 and article.IsActive = 1 and category.CategoryID = {0} order by article.PublishedOn desc", CategoryID));
                }

                _articleList = UpdateCommonCodeAkhbaarIndexModel(_articleList);

            }
            return _articleList;
        }
        public List<AkhbaarIndexModel> GetTop5RelatedArticles(int categoryId, int articleId, bool IsVideo)
        {
            List<AkhbaarIndexModel> _UpdatedNonFeaturedList = new List<AkhbaarIndexModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                if (IsVideo == false)
                {
                    _UpdatedNonFeaturedList = context.Fetch<AkhbaarIndexModel>("select top 5 article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.IsVideo = 0 and category.IsDeleted = 0 and article.IsActive = 1 and category.CategoryID = @0 and article.ArticleID <> @1 order by article.ViewCount desc", categoryId, articleId);
                }
                else
                {
                    _UpdatedNonFeaturedList = context.Fetch<AkhbaarIndexModel>("select top 5 article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.IsVideo = 1 and category.IsDeleted = 0 and article.IsActive = 1 and category.CategoryID = @0 and article.ArticleID <> @1 order by article.ViewCount desc", categoryId, articleId);

                }
                _UpdatedNonFeaturedList = UpdateCommonCodeAkhbaarIndexModel(_UpdatedNonFeaturedList);
            }

            return _UpdatedNonFeaturedList;
        }

        public List<AkhbaarIndexModel> GetAllVideosByCategory(int categoryId)
        {

            List<AkhbaarIndexModel> _videos = new List<AkhbaarIndexModel>();

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _videos = context.Fetch<AkhbaarIndexModel>("select top 5 article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.IsVideo = 1 and category.IsDeleted = 0 and article.IsActive = 1 and category.CategoryID = @0 order by article.PublishedOn desc", categoryId);
                
                _videos = UpdateCommonCodeAkhbaarIndexModel(_videos);
                foreach (var item in _videos)
                {
                    item.shortIslamicDate = context.Fetch<string>("select Hijri from HijriBohra_Gregorian_Calendar where G_Day = @0 and G_Month = @1 and G_Year=@2", item.mappedDate.Day, item.mappedDate.Month, item.mappedDate.Year).FirstOrDefault();
                }
            }

            return _videos;
        }


        public List<nonfeaturedArticleModel> CommonCodeUpdatingInformation(List<nonfeaturedArticleModel> _articles)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {

                foreach (var items in _articles)
                {
                    if (items.Title.Length > 52)
                    {
                        items.Title = string.Format("{0}{1}", items.Title.Substring(0, 52), "...");
                    }
                    items.AkhbaarName = context.Fetch<string>("select categorynameen from Categories where CategoryID = @0", items.CategoryID).FirstOrDefault();
                    if (items.IsVideo == true)
                    {
                        items.ImageUrl = GetVideoEntityThumnailFeaturedImages(items.ArticleID);
                    }
                    else
                    {
                        items.ImageUrl = GetEntityFeaturedImages(items.ArticleID);
                    }
                    items.Mauze = string.Format("{0}{1}{2}", GetCityByCityID(items.CityID), " ,", GetCountryByCityID(items.CityID));
                    items.sourceName = GetSourceNameBySourceID(items.SourceID);
                    items._newMappedDate = items.mappedDate.ToString("dd/MM/yyyy");
                    if (items.ImageUrl == null)
                    {
                        items.ImageUrl = "/Content/Images/NoImageAvailable.jpg";
                    }
                }
            }

            return _articles;

        }
        public Article GetArticle(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Article>("select * from Articles where ArticleID = @0", articleId).FirstOrDefault();
            }
        }

        public List<featuredArticleModel> GetLatestFeaturedArticles()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<featuredArticleModel>("select * from Articles where isFeatured = 1 ORDER BY PublishedOn desc");
            }
        }


        public Page<AkhbaarIndexModel> GetLatestnonFeaturedArticlesByCategoryID(int categoryID, int pageNo, int pageSize, bool isVideo)
        {

            string key = string.Format("GetLatestnonFeaturedArticlesByCategoryID#{0}#{1}#{2}#{3}", categoryID, pageNo, pageSize, isVideo);
            Page<AkhbaarIndexModel> result = CacheManager.Get(key) as Page<AkhbaarIndexModel>;
            if (result == null)
            {
                List<AkhbaarIndexModel> _UpdatedNonFeaturedList = new List<AkhbaarIndexModel>();
                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                    var ppsql = "";
                    //_UpdatedNonFeaturedList = context.Fetch<nonfeaturedArticleModel>("select * from Articles where isFeatured = 0 and CategoryID = @0 and (GETDATE() - PublishedOn) <= 31 ORDER BY PublishedOn desc", categoryID);
                   
                    if (isVideo == false)
                    {
                         ppsql = string.Format("select article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.isFeatured = 0 and category.IsDeleted = 0 and article.IsActive = 1 and category.CategoryID = {0} order by article.PublishedOn desc", categoryID);
                    }
                    else
                    {
                         ppsql = string.Format("select article.ArticleID, article.Body, article.CategoryID, article.CityID, article.CommentCount, article.IsActive, article.isFeatured,article.IslamicDate, article.IsVideo, article.mappedDate, article.PublishedOn as PublishedDate, article.ShortUrl, article.SourceID, article.Title, article.ViewCount,source.Name as sourceName,source.Website as SourceWebsite,category.CategoryNameEn as AkhbaarName, city.Name as Cityname, country.Name as CountryName from articles article left join Sources source on article.SourceID = source.SourceID left join Categories category on article.CategoryID = category.CategoryID left join Cities city on article.CityID = city.CityID left join States states on city.StateID = states.StateID left join Countries country on states.CountryID = country.CountryID where article.isFeatured = 0 and category.IsDeleted = 0 and article.IsActive = 1 and category.CategoryID = {0} and article.isVideo = 1 order by article.PublishedOn desc", categoryID);
                    
                    }
                    result = context.Page<AkhbaarIndexModel>(pageNo, pageSize, ppsql);
                    CacheManager.Set(key, result, DateTime.Now.AddSeconds(ApplicationConstants.ListCacheTimeInSec));
                    result.Items = UpdateCommonCodeAkhbaarIndexModel(result.Items);
                }
            }
            return (result);
        }


        public List<AkhbaarIndexModel> UpdateCommonCodeAkhbaarIndexModel(List<AkhbaarIndexModel> articles)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                foreach (var items in articles)
                {
                    if (items.Title.Length > 52)
                    {
                        items.Title = string.Format("{0}{1}", items.Title.Substring(0, 52), "...");
                    }
                    items.Entity = new EntityMedia();
                    if (items.IsVideo == false)
                    {
                        items.Entity = GetFeaturedImageEntityMedia(items.ArticleID);
                    }
                    else
                    {
                        items.Entity = GetFeaturedVideoEntityMedia(items.ArticleID);
                    }
                    if (items.Entity == null)
                    {
                        items.Entity = new EntityMedia();
                        items.Entity.Thumbnail = "/Content/Images/NoImageAvailable.jpg";

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(items.Entity.Thumbnail))
                        {
                            items.Entity.Thumbnail = "/Content/Images/NoImageAvailable.jpg";
                        }
                    }
                }
            }
            return articles;
        }
        public List<nonfeaturedArticleModel> GetTop5ArticlesByCategoryID(int categoryID)
        {
            List<nonfeaturedArticleModel> _articles = new List<nonfeaturedArticleModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _articles = context.Fetch<nonfeaturedArticleModel>("select top 5 * from Articles where categoryId = @0 order by PublishedOn desc", categoryID);
                _articles = CommonCodeUpdatingInformation(_articles);
            }
            return _articles;
        }


        public string GetVideoEntityThumnailFeaturedImages(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select thumbnail from EntityMedia where ArticleID = @0 and IsFeatured = 1 and CreativeTypeID = 2", articleId).FirstOrDefault();
            }
        }

        public EntityMedia GetFeaturedVideoEntityMedia(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where ArticleID = @0 and IsFeatured = 1 and CreativeTypeID = 2", articleId).FirstOrDefault();
            }
        }

        public EntityMedia GetFeaturedImageEntityMedia(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where ArticleID = @0 and IsFeatured = 1 and CreativeTypeID = 1", articleId).FirstOrDefault();
            }
        }

        public string GetEntityFeaturedImages(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select top 1 URL from EntityMedia where ArticleID = @0 and creativetypeid = 1 and IsFeatured = 1", articleId).FirstOrDefault();
            }
        }

        public string GetEntityFeaturedVideo(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select top 1 URL from EntityMedia where ArticleID = @0 and creativetypeid = 2 and IsFeatured = 1", articleId).FirstOrDefault();
            }
        }


        public string GetCityByCityID(int cityID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Cities where CityID = @0", cityID).FirstOrDefault();
            }
        }

        public string GetStateByCityID(int cityID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from states where stateid = (select stateid from cities where cityid = @0)", cityID).FirstOrDefault();
            }
        }

        public string GetSourceNameBySourceID(int sourceId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Sources where SourceID = @0", sourceId).FirstOrDefault();
            }
        }

        public string GetCountryByCityID(int cityID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Countries where CountryID = (select CountryID from States where StateID = (select StateID from Cities where CityID = @0))", cityID).FirstOrDefault();
            }
        }


        public ArticleModel ArticleInformationService()
        {
            ArticleModel _model = new ArticleModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _model._featuredArticleList = new List<featuredArticleModel>();
                _model._nonfeaturedArticleList = new CategoryWiseList();
                _model._featuredArticleList = GetLatestFeaturedArticles();
                foreach (var items in _model._featuredArticleList)
                {
                    EntityMedia _entitymedia = GetFeaturedVideoEntityMedia(items.ArticleID);
                    if (items.Title.Length > 52)
                    {
                        items.Title = string.Format("{0}{1}", items.Title.Substring(0, 52), "...");
                    }

                    if (items.IsVideo == false)
                    {
                        items.ImageUrl = GetEntityFeaturedImages(items.ArticleID);
                    }
                    else
                    {
                        items.EntityMediaID = _entitymedia.EntityMediaID;
                        items.ImageUrl = _entitymedia.Thumbnail;
                        items.Url = _entitymedia.URL;
                    }
                    items.Mauze = string.Format("{0}{1}{2}", GetCityByCityID(items.CityID), " ,", GetCountryByCityID(items.CityID));

                    items.sourceName = GetSourceNameBySourceID(items.SourceID);
                    items._newMappedDate = items.mappedDate.ToString("dd/MM/yyyy");
                    if (items.ImageUrl == null)
                    {
                        items.ImageUrl = "/Content/Images/NoImageAvailable.jpg";
                    }
                }

                _model._nonfeaturedArticleList.HuzuralaRelatedArticles = GetTop5ArticlesByCategoryID(1);
                _model._nonfeaturedArticleList.SaadatkiramArticles = GetTop5ArticlesByCategoryID(2);
                _model._nonfeaturedArticleList.QasreAliArticles = GetTop5ArticlesByCategoryID(3);
                _model._nonfeaturedArticleList.BilaadImaniaArticles = GetTop5ArticlesByCategoryID(6);
                _model.HuzuralaAkhbaarCount = context.Fetch<int>("select count(*) from Articles where isfeatured = 0 and categoryId = @0", 1).FirstOrDefault();
                _model.SaadatAkhbaarCount = context.Fetch<int>("select count(*) from Articles where isfeatured = 0 and categoryId = @0", 2).FirstOrDefault();
                _model.QasreAliAkhbaarCount = context.Fetch<int>("select count(*) from Articles where isfeatured = 0 and categoryId = @0", 3).FirstOrDefault();
                _model.BilaadImaniaArticles = context.Fetch<int>("select count(*) from Articles where isfeatured = 0 and categoryId = @0", 6).FirstOrDefault();


            }
            return _model;
        }

        public ArticleDetailModel GetArticleDetailByArticleID(int id)
        {
            ArticleDetailModel _articleModel = new ArticleDetailModel();

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _articleModel = context.Fetch<ArticleDetailModel>("select * from Articles where ArticleID = @0", id).FirstOrDefault();

                _articleModel.ViewCount = UpdateViewCount(id);
                _articleModel.SourceName = GetSourceNameBySourceID(_articleModel.SourceID);
                _articleModel.Mauze = string.Format("{0}{1}{2}", GetCityByCityID(_articleModel.CityID), " ,", GetCountryByCityID(_articleModel.CityID));
                _articleModel._newMappedDate = _articleModel.mappedDate.ToString("dd/MM/yyyy");
                _articleModel.ListUploadedPictures = context.Fetch<EntityMedia>("select * from EntityMedia where ArticleID = @0 and IsFeatured = 0 and CreativeTypeID = 1", id);
                if (_articleModel.IsVideo == true)
                {
                    _articleModel.FeaturedImage = context.Fetch<string>("select thumbnail from EntityMedia where ArticleID = @0 and IsFeatured = 1 and creativeTypeId = 2", id).FirstOrDefault();
                }
                else
                {
                    _articleModel.FeaturedImage = context.Fetch<string>("select thumbnail from EntityMedia where ArticleID = @0 and IsFeatured = 1 and creativeTypeId = 1", id).FirstOrDefault();
                }
                _articleModel.VideoURL = context.Fetch<EntityMedia>("select * from EntityMedia where ArticleID = @0 and IsFeatured = 0 and CreativeTypeID = 2", id);
                _articleModel.FeaturedVideo = context.Fetch<string>("select URL from EntityMedia where ArticleID = @0 and IsFeatured = 1 and CreativeTypeID = 2", id).FirstOrDefault();
                if (_articleModel.FeaturedImage == null)
                    _articleModel.FeaturedImage = "/Content/Images/NoImageAvailable.jpg";

            }

            return _articleModel;
        }

        public int UpdateViewCount(int id)
        {
            Article _article = GetArticle(id);

            _article.ViewCount = _article.ViewCount + 1;

            UpdateArticle(_article);

            return _article.ViewCount;
        }



        public int GetCountOfArticlesByCategoryID(int categoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select count(*) from articles where categoryId = @0 and isFeatured = 0 and isVideo = 1", categoryID).FirstOrDefault();
            }
        }

        public int GetAllCountOfArticlesByCategoryID(int categoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select count(*) from articles where categoryId = @0 and isFeatured = 0", categoryID).FirstOrDefault();
            }
        }

      


        public List<Currency> GetCurrencyList()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Currency>("select * from currency");
            }

        }


        public List<Country> GetCountryList()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Country>("select * from Countries");
            }
        }
             

    }
}

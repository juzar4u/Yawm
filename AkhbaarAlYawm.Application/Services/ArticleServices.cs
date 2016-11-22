using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
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

        public int UpdateCompaignImage(EntityMedia EM)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(EM);
            }
        }

        public int UpdateArticleImage(EntityMedia EM)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(EM);
            }
        }

        public int UpdateArticle(Article _article)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_article);
            }
        }

        public List<Article> GetAllArticles()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Article>("select * from Articles where IsActive = 1");
            }
        }

        

        public List<ArticleModel> GetAllArticleByTitle(int elementId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleModel>("select * from Articles where ArticleID = @0 and IsActive = 1", elementId);
            }
        }

        public List<ArticleModel> GetAllArticleBySource(int elementId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleModel>("select * from Articles where SourceID = @0 and IsActive = 1", elementId);
            }
        }

        public List<ArticleModel> GetAllArticleIDs(int elementId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleModel>(String.Format("select * from Articles where articleid like '%{0}%'", elementId));
            }
        }

        public List<City> GetArticleListByCityFilter(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {

                return context.Fetch<City>(String.Format("select * from Cities where Name like '%{0}%'", name));
            }
        }

        public List<ArticleModel> GetAllArticleByCity(int elementId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleModel>("select * from Articles where CityID = @0 and IsActive = 1", elementId);
            }
        }

        public List<ArticleModel> GetAllArticlesList()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleModel>("select * from Articles where IsActive = 1");
            }
        }

        public EntityMedia GetEntityMediaVideoRecordByEntityMediaID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where CreativeTypeID = 2 and EntityMediaID = @0", id).FirstOrDefault();
            }
        }

        public EntityMedia GetEntityMediaImageRecordByEntityMediaID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where CreativeTypeID = 1 and EntityMediaID = @0", id).FirstOrDefault();
            }
        }

        public List<EntityMedia> GetEntityMediaAllVideoURLByArticleID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where CreativeTypeID = 2 and ArticleID = @0", id);
            }
        }

        public List<Source> GetAllSources()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Source>("select * from sources");
            }
        }

        public List<Country> GetAllCountries()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Country>("select * from Countries");
            }
        }
        public List<State> GetAllStatesByCountryID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<State>("select * from States where CountryID = @0", id);
            }
        }
        public List<City> GetAllCitiesByStateID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<City>("select * from Cities where StateID = @0", id);
            }
        }

        public ArticleModel GetArticleByArticleID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ArticleModel>("select * from Articles where ArticleID = @0", id).FirstOrDefault();
            }
        }

        public Article GetArticleForUpdatingFeaturedByArticleID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Article>("select * from Articles where ArticleID = @0", id).FirstOrDefault();
            }
        }

        public Article GetArticleRecordByArticleID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Article>("select * from Articles where ArticleID = @0", id).FirstOrDefault();
            }
        }

        public List<EntityMedia> GetEntityMediaImageRecordByArticleID(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where CreativeTypeID = 1 and ArticleID = @0", articleId);
            }
        }

        public List<Article> GetArticleListByTitleFilter(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Article>(String.Format("select * from Articles where Title like '%{0}%'", name));
            }
        }

        public List<Source> GetArticleListBySourceFilter(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {

                return context.Fetch<Source>(String.Format("select * from Sources where Name like '%{0}%'", name));
            }
        }

        public List<Article> GetArticleIDs(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {

                return context.Fetch<Article>(String.Format("select * from articles where articleid like '%{0}%'", name));
            }
        }


        public EntityMedia GetEntityMediaRecordByEntityMediaID(int entityMediaID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EntityMedia>("select * from EntityMedia where EntityMediaID = @0", entityMediaID).FirstOrDefault();
            }
        }


        public string GetCountryNameByStateID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Countries where CountryID = (select CountryID from States where StateID = @0)", id).FirstOrDefault();
            }
        }



        public int GetCountryIdByStateID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select countryId from States where StateID = @0", id).FirstOrDefault();
            }
        }
        public string GetCountryNameByCityID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Countries where CountryID = (select CountryID from States where StateID = (select StateID from Cities where CityID = @0))", id).FirstOrDefault();
            }
        }

        public int GetCountryIdByCityID(int cityId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select countryid from States where StateID = (select StateID from Cities where CityID = @0)", cityId).FirstOrDefault();
            }
        }
        public int GetStateIdByCityID(int cityId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select StateID from Cities where CityID = @0", cityId).FirstOrDefault();
            }
        }

        public int GetCityIdByStateID(int stateId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select CityID from Cities where StateID = @0", stateId).FirstOrDefault();
            }
        }

        public string GetCountryNameByCountryID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Countries where CountryID = @0", id).FirstOrDefault();
            }
        }

        public string GetStateNameByStateID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from States where StateID = @0", id).FirstOrDefault();
            }
        }
        public string GetCityNameByCityID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Cities where CityID = @0", id).FirstOrDefault();
            }
        }

        public string GetNullableCityNameByCityID(int? id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Cities where CityID = @0", id).FirstOrDefault();
            }
        }

        public string GetSourceNameBySourceID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Sources where SourceID = @0", id).FirstOrDefault();
            }
        }

        public string GetNullableSourceNameBySourceID(int? id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select Name from Sources where SourceID = @0", id).FirstOrDefault();
            }
        }

        public int InsertArticles(Article _article)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_article);
            }
        }

        public int InsertSources(Source _source)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_source);
            }
        }

        public void InsertArticleVideo(int articleID, string videoUrl, string videoThumbnail, bool? isfeatured)
        {

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                context.Insert(new EntityMedia()
                {

                    CreativeTypeID = 2,
                    Thumbnail = videoThumbnail,
                    URL = videoUrl,
                    IsFeatured = isfeatured,
                    ArticleID = articleID
                });
            }
        }

        public void InsertArticleImages(int articleID, string ImageUrl, string ImageThumbnail, bool isfeatured)
        {

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                context.Insert(new EntityMedia()
                {

                    CreativeTypeID = 1,
                    Thumbnail = ImageThumbnail,
                    URL = ImageUrl,
                    IsFeatured = isfeatured,
                    ArticleID = articleID
                });
            }
        }


        public List<EntityMedia> GetAllImagesByArticleID(int articleId)
        {
            List<EntityMedia> _entityImages = new List<EntityMedia>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {


                _entityImages = context.Fetch<EntityMedia>("select * from entitymedia where creativetypeid = 1 and articleid = @0", articleId);
            }
            return _entityImages;
        }

        public int DeleteEntityMediaImages(EntityMedia _entityMedia)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                context.Delete(_entityMedia);
            }
            return 1;
        }

        public int DeleteArticleRecord(int articleId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                context.ExecuteScalar<Article>("delete from articles where articleid = @0", articleId);
            }
            return 1;
        }
    }
}

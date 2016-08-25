
using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYamn.Web.API.Helper;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.APIEntitiesModel;
using AkhbaarAlYawm.Web.API.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AkhbaarAlYamn.Web.API.Controllers
{
    public class ArticleController : ApiController
    {
        public ArticleList GetArticleDetailByArticleID(int articleId, int categoryID)
        {
            ArticleList _article = new ArticleList();
            //_article.ArticleID = ArticleServices.GetInstance.GetArticleDetailByArticleId(articleId, Constants.Akhbaar_CP_URL);
            _article.ArticleID = articleId;
            _article.RelatedArticleList = ArticleServices.GetInstance.GetTop5RelatedArticles(categoryID, articleId, Constants.Akhbaar_CP_URL);
            return _article;
        }



        public List<ArticleDetailModel> GetAllArticleListing(bool IsFeatured, int categoryId, int pageNo, int records)
        {
            int pageSize = records;
            List<ArticleDetailModel> _articleList = new List<ArticleDetailModel>();
            if (IsFeatured == true)
            {
                _articleList = ArticleServices.GetInstance.GetAllFeaturedAritcles();
            }
            else
            {
                _articleList = ArticleServices.GetInstance.GetAllNonFeaturedAritcles(categoryId);
            }


            List<ArticleDetailModel> _newArticleList = new List<ArticleDetailModel>();
            ArticleDetailModel _articleDetail = new ArticleDetailModel();

            foreach (var items in _articleList)
            {

                _articleDetail = ArticleServices.GetInstance.GetArticleDetailForListingByArticleId(items.ArticleID, Constants.Akhbaar_CP_URL);
                _newArticleList.Add(_articleDetail);
            }
            _newArticleList = _newArticleList.OrderByDescending(x => x.PublishedOn).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            return _newArticleList;
        }


        public List<EntityMediaModel> GetEntityMediaListByArticleID(int articleId, int creativeTypeId)
        {
            List<EntityMediaModel> _entityMedia = new List<EntityMediaModel>();

            _entityMedia = ArticleServices.GetInstance.GetEntityMediaByArticleID(articleId, creativeTypeId, Constants.Akhbaar_CP_URL);

            return _entityMedia;
        }

        public IndexModel GetHomePage()
        {

            IndexModel _HomeModel = new IndexModel();

            _HomeModel.Articles = new List<ArticleListingHomeModel>();

            _HomeModel.CurrentQiyaamSharif = ArticleServices.GetInstance.GetCurrentQiyaam();
            for (int i = 1; i <= 6; i++)
            {
                if (i != 4 && i != 5)
                {
                    _HomeModel.Articles.Add(new ArticleListingHomeModel()
                    {
                        Category = ArticleServices.GetInstance.GetCategoryNameByCategoryID(i),
                        TotalCount = ArticleServices.GetInstance.GetArticleCountByCategories(i),
                        Articles = ArticleServices.GetInstance.GetTop5ArticlesByCategory(i, Constants.Akhbaar_CP_URL)
                    });
                }
            }
            return _HomeModel;
        }

    }
}

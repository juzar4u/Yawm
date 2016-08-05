using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AkhbaarMasterModel _articleModel = new AkhbaarMasterModel();
            _articleModel.nonfeatured = new List<AkhbaarNonFeatured>();
            _articleModel.FeaturedList = PP_ArticleServices.GetInstance.GetAkhbaarIndexModel(true, 0);
            _articleModel._category = CategoriesServices.GetInstance.GetAllCategories();

            foreach (var items in _articleModel._category)
            {
                _articleModel.nonfeatured.Add(new AkhbaarNonFeatured()
                {
                    nonFeaturedList = PP_ArticleServices.GetInstance.GetAkhbaarIndexModel(false, items.CategoryID),
                    AkhbaarName = items.CategoryNameEn,
                    AkhbaarCount = PP_ArticleServices.GetInstance.GetAllCountOfArticlesByCategoryID(items.CategoryID),
                    CategoryID = items.CategoryID
                });

            }
            _articleModel.CurrentQiyaam = CommonServices.GetInstance.GetCurrentQiyaamMauze(1);
            return View(_articleModel);

        }

        public ActionResult AkhbaarList(int categoryId, bool isVideo, int pageNo)
        {
            PetaPoco.Page<AkhbaarIndexModel> entities = new PetaPoco.Page<AkhbaarIndexModel>();
            List<AkhbaarIndexModel> _nonFeaturedList = new List<AkhbaarIndexModel>();
            Categories Category = new Categories();
            int pageSize = 10;
            ViewBag.categoryID = categoryId;
            entities = PP_ArticleServices.GetInstance.GetLatestnonFeaturedArticlesByCategoryID(categoryId, pageNo, pageSize, isVideo);
            Category = CategoriesServices.GetInstance.GetCategoryByCategoryID(categoryId);
            if (isVideo == true)
            {
                entities.Items = entities.Items.Where(a => a.IsVideo).ToList();
            }
            ViewBag.CategoryName = Category.CategoryNameEn;
            ViewBag.currentPage = entities.CurrentPage;
            ViewBag.TotalPages = entities.TotalPages;
            ViewBag.ArticleCount = entities.TotalItems;
            _nonFeaturedList = entities.Items;
            if (pageNo > 1)
                return PartialView("~/Views/Shared/sharedHomeNonFeaturedView.cshtml", _nonFeaturedList);
            else
                return View(_nonFeaturedList);
        }


        public ActionResult Detail(int id)
        {
            UserModel user = AuthHelper.LoginFromCookie();

            ArticleDetailModel _articleModel = new ArticleDetailModel();
            SocialSharePlugin _social = new SocialSharePlugin();
            _articleModel.ArticleComment = new CommentMasterModel();
            
            _articleModel.SocialSharerPlugin = new SocialSharePlugin();
            _articleModel = PP_ArticleServices.GetInstance.GetArticleDetailByArticleID(id);
            _articleModel._RelatedArticles = PP_ArticleServices.GetInstance.GetTop5RelatedArticles(_articleModel.CategoryID, id, _articleModel.IsVideo);
            _social.shareurl = _articleModel.ShortUrl;
            _social.Title = _articleModel.Title;
            _articleModel.SocialSharerPlugin = _social;
            _articleModel.ArticleComment = CommentServices.GetInstance.GetAllCommentsByArticleID(id,(int)CommentCategoryEnum.ArticleType, user);
            return View(_articleModel);
        }

        public ActionResult PhotoGallery()
        {
            return View();
        }
        public ActionResult About()
        {

            ViewBag.Message = "Your app description page.";

            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult GetStates(int countryID)
        {

            var states = ArticleServices.GetInstance.GetAllStatesByCountryID(countryID);
            var result = (from c in states
                          select new
                          {
                              StateID = c.StateID,
                              StateName = c.Name

                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCities(int StateID)
        {
            var cities = ArticleServices.GetInstance.GetAllCitiesByStateID(StateID);
            ViewBag.StateID = StateID;
            ViewBag.StateName = ArticleServices.GetInstance.GetStateNameByStateID(StateID);
            var result = (from c in cities
                          select new
                          {
                              CityID = c.CityID,
                              CityName = c.Name,
                              StateName = ArticleServices.GetInstance.GetStateNameByStateID(StateID)
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}

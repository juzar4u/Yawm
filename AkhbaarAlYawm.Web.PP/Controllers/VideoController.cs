using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class VideoController : Controller
    {
        //
        // GET: /Video/

        public ActionResult Index()
        {
            //int pageSize = 5;
            List<VideoIndexModel> _model = new List<VideoIndexModel>();
            List<Categories> _categories = new List<Categories>();

            List<AkhbaarIndexModel> _allvideos = new List<AkhbaarIndexModel>(); 
            _categories = CategoriesServices.GetInstance.GetAllCategories();
            foreach (var category in _categories)
            {
                _allvideos = PP_ArticleServices.GetInstance.GetAllVideosByCategory(category.CategoryID);
                _model.Add(new VideoIndexModel()
                {
                    _category = category,
                    _categoryWiseVideos = _allvideos,
                    AllVideosCount = PP_ArticleServices.GetInstance.GetCountOfArticlesByCategoryID(category.CategoryID)
                   
                });
            }
            return View(_model);

        }

    }
}

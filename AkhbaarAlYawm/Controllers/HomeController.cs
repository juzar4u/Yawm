using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.CommonCode.Helpers;
using AkhbaarAlYawm.Controllers;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.CommonModel;
using AkhbaarAlYawm.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace AkhbaarAlYawm.Web.CP.Controllers
{
   
    public class HomeController : Controller
    {

         [Authentication]
        [HttpGet]
        public ActionResult Index()
        {
            CurrentQiyaamModel model = CommonServices.GetInstance.GetCurrentQiyaamModelByCurrentQiyaamID(1);

            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View(model);

        }

        [HttpPost]
        public ActionResult Index(CurrentQiyaamModel model)
        {
            CurrentQiyaam _currentQiyaam = CommonServices.GetInstance.GetCurrentQiyaamDetailsByCurrentQiyaamID(model.CurrentQiyaamID);

            _currentQiyaam.CurrentQiyaamMauze = model.CurrentQiyaamMauze;

            CommonServices.GetInstance.UpdateCurrentQiyaam(_currentQiyaam);

            return RedirectToAction("Index");
        }

        [Authentication]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
         [Authentication]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public int UpdateFeatured(int Id)
        {
            Article _article = ArticleServices.GetInstance.GetArticleForUpdatingFeaturedByArticleID(Id);

            if (_article.isFeatured == true)
            {
                _article.isFeatured = false;
            }
            else
            {
                _article.isFeatured = true;
            }

            ArticleServices.GetInstance.UpdateArticle(_article);

            return 1;
        }

        [Authentication]
        public ActionResult ManagementArticles(int pageId)
        {
            int pageSize = 10;
            List<ArticleModel> _article = ArticleServices.GetInstance.GetAllArticlesList();
            ViewBag.totalCount = _article.Count;
            ViewBag.currentPage = pageId;

            _article = _article.OrderByDescending(x => x.PublishedOn).Skip(pageSize * (pageId - 1)).Take(pageSize).ToList();
            foreach (var items in _article)
            {
                if (items.Title.Length > 52)
                {
                    items.Title = string.Format("{0}{1}", items.Title.Substring(0, 52), "...");
                }
                items.CityName = ArticleServices.GetInstance.GetCityNameByCityID(items.CityID);
                if (items.CityName == null)
                {
                    string stateName = ArticleServices.GetInstance.GetStateNameByStateID(items.StateID);
                    if (stateName != null)
                    {
                        items.CityName = stateName;
                    }
                }
                items.SourceName = ArticleServices.GetInstance.GetSourceNameBySourceID(items.SourceID);
                if (items.CityName == null)
                {
                    items.CityName = "None";
                }

                if (items.SourceName == null)
                {
                    items.SourceName = "None";
                }
            }
            return View(_article);
        }

         [Authentication]
        [HttpGet]
        public ActionResult PostArticles()
        {
            ArticleModel model = new ArticleModel();
            model.SourceList = ArticleServices.GetInstance.GetAllSources();
            model.CountryList = ArticleServices.GetInstance.GetAllCountries();
            model.ImageResolutionError = "";
            return View(model);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostArticles(ArticleModel model)
        {


            Article _article = new Article();
            HijriBohraCalenderModel _hijri = new HijriBohraCalenderModel();

            _article.Title = model.Title;
            if (!model.Body.Contains("<img alt=\"\" class=\"img-responsive\""))
            {
                model.Body = model.Body.Replace("<img alt=\"\"", "<img alt=\"\" class=\"img-responsive\"");
            }
            if (!model.Body.Contains("<iframe id=\"iframeBody\""))
            {
                model.Body = model.Body.Replace("<iframe", "<iframe id=\"iframeBody\"");
            }
            _article.Body = model.Body;
            _article.SourceID = model.SourceID;
            _article.IsActive = true;
            _article.CreatedOn = DateTime.Now;
            _article.UpdatedOn = DateTime.Now;
            _article.PublishedOn = DateTime.Now;
            _article.CreatedBy = AuthHelper.LoggedInUserID;
            _article.UpdatedBy = AuthHelper.LoggedInUserID;
            _article.PublishedBy = AuthHelper.LoggedInUserID;
            _article.isFeatured = false;
            if (model.CityID <= 0)
            {
                _article.CityID = null;
                _article.StateID = model.StateID;
            }
            else
            {
                _article.CityID = model.CityID;
                _article.StateID = null;
            }

            if (model.radioDateType == "gregorian")
            {
                _hijri = MiqaatServices.GetInstance.GetGregorianCalenderDetailByDayMonthYear(model.gregorianDate.Day, model.gregorianDate.Month, model.gregorianDate.Year);
            }
            else
            {
                _hijri = MiqaatServices.GetInstance.GetHijriBohraCalenderDetailByDayMonthYear(Convert.ToInt32(model.IslamicDay), Convert.ToInt32(model.IslamicMonth), model.IslamicYear);
            }
            _article.mappedDate = _hijri.Gregorian;
            _article.IslamicDate = string.Format("{0}{1}{2}{3}{4}", _hijri.H_Day, "-", MiqaatServices.GetInstance.getMonth(_hijri.H_Month), "-", _hijri.H_Year);
            _article.CategoryID = model.CategoryID;
            _article.IsVideo = model.IsVideo;
            _article.ArticleID = ArticleServices.GetInstance.InsertArticles(_article);

            _article.ShortUrl = Constants.GetBitlyURL(_article.ArticleID);

            ArticleServices.GetInstance.UpdateArticle(_article);
            #region Video
            for (int i = 2; i <= model.TotalUploadedVideos; i++)
            {
                if (model.IsFeaturedNameVideos == "radio" + i.ToString())
                {
                    model.IsFeaturedVideos = true;
                }
                else
                {
                    model.IsFeaturedVideos = false;
                }
                ValidateVideoUrlAndUpload("video" + i.ToString(), _article.ArticleID, model.IsFeaturedVideos);
            }
            #endregion
            #region Images
            if (Request.Files.Count > 0)
            {
                for (int i = 2; i <= model.TotalUploadedImages; i++)
                {
                    if (model.IsFeaturedNameImage == "radioImage" + i.ToString())
                    {
                        model.IsFeaturedImage = true;
                    }
                    else
                    {
                        model.IsFeaturedImage = false;
                    }
                    ValidateImageAndUpload("image" + i.ToString(), _article.ArticleID, model.IsFeaturedImage,null);

                }
            }
            #endregion
            #region akhbaarPhoto
            if (model.akhbaarPhotoCount > 0)
            {
                //List<HttpPostedFile> akhbaarPhotos = new List<HttpPostedFile>();
                for (int i = 0; i < model.akhbaarPhotoCount; i++)
                {
                    //akhbaarPhotos.Add(System.Web.HttpContext.Current.Request.Files[i]);
                    ValidateImageAndUpload("", _article.ArticleID, false, System.Web.HttpContext.Current.Request.Files[i]);
                }
            }
            #endregion

            return RedirectToAction("ManagementArticles");

        }

         [Authentication]
        [HttpGet]
        public ActionResult AddSource()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSource(SourceModel model)
        {
            Source _source = new Source();
            _source.Name = model.Name;
            _source.Website = model.Website;

            ArticleServices.GetInstance.InsertSources(_source);

            return RedirectToAction("Index");
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

        private void ValidateVideoUrlAndUpload(String id, int articleID, bool? isfeatured)
        {
            string url = System.Web.HttpContext.Current.Request.Form[id];
            if (url != "")
            {
                string VideoId = YoutubeHelper.getYoutubeVideoID(url);
                string videoThumbnail = string.Format("http://img.youtube.com/vi/{0}/hqdefault.jpg", VideoId);
                ArticleServices.GetInstance.InsertArticleVideo(articleID, url, videoThumbnail, isfeatured);
            }
        }

        private int ValidateImageAndUpload(String fileUploadId, int articleId, bool isfeatured, HttpPostedFile akhbaarFile)
        {
            int creativeId = 0;
            string articleImageUrl = "";
            string thumbnailUrl = "";
            HttpPostedFile postedFile;

            if (akhbaarFile == null && !string.IsNullOrEmpty(fileUploadId))
                postedFile = System.Web.HttpContext.Current.Request.Files[fileUploadId];
            else
                postedFile = akhbaarFile;
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                WebImage img = new WebImage(postedFile.InputStream);
                if ((postedFile.ContentType.Contains("image")))
                {
                    string extension = System.IO.Path.GetExtension(postedFile.FileName);

                    if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                    {
                        string fileName = Guid.NewGuid().ToString();
                        if (img.Width > 1000 & img.Height > 700)
                        {
                            img.Resize(1000, 700);
                            img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + "Large" + fileName + extension);
                            img.Resize(500, 365);
                            img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + fileName + extension);
                            articleImageUrl = "/Images/Articles/" + "Large" + fileName + extension;
                            thumbnailUrl = "/Images/Articles/" + fileName + extension;
                            
                            ArticleServices.GetInstance.InsertArticleImages(articleId, articleImageUrl, thumbnailUrl, isfeatured);
                        }
                        else
                        {
                            img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + fileName + extension);
                            thumbnailUrl = "/Images/Articles/" + fileName + extension;
                            ArticleServices.GetInstance.InsertArticleImages(articleId, thumbnailUrl, thumbnailUrl, isfeatured);
                        }

                    }
                }
            }
            else
            {
                creativeId = -1;
            }

            return creativeId;

        }

        
        private int UpdateAndValidateCurrentImage(String fileUploadId, int articleId, bool isfeatured, int entityMediaID)
        {
            int creativeId = 0;
            string articleImageUrl = "";
            string thumbnailUrl = "";
            HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[fileUploadId];

            if (postedFile.ContentLength > 0)
            {
                WebImage img = new WebImage(postedFile.InputStream);
                string extension = System.IO.Path.GetExtension(postedFile.FileName);
                if (postedFile != null)
                {
                        
                        if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                        {
                            string fileName = Guid.NewGuid().ToString();
                            if (img.Width > 1000 & img.Height > 700)
                            {
                                img.Resize(1000, 700);
                                img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + "Large" + fileName + extension);
                                img.Resize(500, 365);
                                img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + fileName + extension);
                                articleImageUrl = "/Images/Articles/" + "Large" + fileName + extension;
                                thumbnailUrl = "/Images/Articles/" + fileName + extension;
                                EntityMedia _entity = ArticleServices.GetInstance.GetEntityMediaRecordByEntityMediaID(entityMediaID);

                                _entity.URL = articleImageUrl;
                                _entity.Thumbnail = thumbnailUrl;
                                ArticleServices.GetInstance.UpdateCompaignImage(_entity);
                            }
                            else
                            {
                                img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + fileName + extension);
                                thumbnailUrl = "/Images/Articles/" + fileName + extension;
                                EntityMedia _entity = ArticleServices.GetInstance.GetEntityMediaRecordByEntityMediaID(entityMediaID);

                                _entity.URL = thumbnailUrl;
                                _entity.Thumbnail = thumbnailUrl;
                                ArticleServices.GetInstance.UpdateCompaignImage(_entity);
                            }
                        }
                    
                }
                else
                {
                    creativeId = -1;
                }
            }
            else
            {
                creativeId = -1;
            }
            return creativeId;

        }

        [Authentication]
        [HttpGet]
        public ActionResult EditArticle(int Id)
        {
            ArticleModel _article = ArticleServices.GetInstance.GetArticleByArticleID(Id);

            if (_article.StateID > 0)
            {
                _article.CityID = ArticleServices.GetInstance.GetCityIdByStateID(_article.StateID);
                _article.CountryName = ArticleServices.GetInstance.GetCountryNameByStateID(_article.StateID);
                _article.CountryID = ArticleServices.GetInstance.GetCountryIdByStateID(_article.StateID);
            }
            else if (_article.CityID > 0)
            {
                _article.StateID = ArticleServices.GetInstance.GetStateIdByCityID(_article.CityID);
                _article.CountryName = ArticleServices.GetInstance.GetCountryNameByCityID(_article.CityID);
                _article.CountryID = ArticleServices.GetInstance.GetCountryIdByCityID(_article.CityID);
            }
            else
            {
                _article.CountryName = "None";
            }

            if (_article.SourceID > 0)
            {
                _article.SourceName = ArticleServices.GetInstance.GetSourceNameBySourceID(_article.SourceID);
            }
            else
            {
                _article.SourceName = "Select An Option";
            }

            _article.ListUploadedPictures = ArticleServices.GetInstance.GetEntityMediaImageRecordByArticleID(Id);
            _article.VideoURL = ArticleServices.GetInstance.GetEntityMediaAllVideoURLByArticleID(Id);
            _article.SourceList = ArticleServices.GetInstance.GetAllSources();
            _article.CountryList = ArticleServices.GetInstance.GetAllCountries();
            _article.ArticleSelectedState = ArticleServices.GetInstance.GetStateNameByStateID(_article.StateID);
            _article.ArticleSelectedCity = ArticleServices.GetInstance.GetCityNameByCityID(_article.CityID);
            
            return View(_article);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditArticle(ArticleModel model)
        {
            Article _article = ArticleServices.GetInstance.GetArticleRecordByArticleID(model.ArticleID);
            HijriBohraCalenderModel _hijri = new HijriBohraCalenderModel();
            _article.Title = model.Title;
            if(!model.Body.Contains("<img alt=\"\" class=\"img-responsive\""))
            {
            model.Body = model.Body.Replace("<img alt=\"\"", "<img alt=\"\" class=\"img-responsive\"");
            }
            if (!model.Body.Contains("<iframe id=\"iframeBody\""))
            {
                model.Body = model.Body.Replace("<iframe", "<iframe id=\"iframeBody\"");
            }
            _article.Body = model.Body;
            _article.SourceID = model.SourceID;

            if (model.StateID > 1)
            {
                _article.StateID = model.StateID;
            }
            if (model.CityID > 1)
            {
                _article.CityID = model.CityID;
            }
            if (model.gregorianDate.Year >= 1900)
            {
                if (model.radioDateType == "gregorian")
                {
                    _hijri = MiqaatServices.GetInstance.GetGregorianCalenderDetailByDayMonthYear(model.gregorianDate.Day, model.gregorianDate.Month, model.gregorianDate.Year);
                }
                else
                {
                    _hijri = MiqaatServices.GetInstance.GetHijriBohraCalenderDetailByDayMonthYear(Convert.ToInt32(model.IslamicDay), Convert.ToInt32(model.IslamicMonth), model.IslamicYear);
                }
                _article.mappedDate = _hijri.Gregorian;
                _article.IslamicDate = string.Format("{0}{1}{2}{3}{4}", _hijri.H_Day, "-", MiqaatServices.GetInstance.getMonth(_hijri.H_Month), "-", _hijri.H_Year);

            }
            
            _article.CategoryID = model.CategoryID;
            _article.IsVideo = model.IsVideo;
            #region Videos
            model.VideoURL = ArticleServices.GetInstance.GetEntityMediaAllVideoURLByArticleID(model.ArticleID);
            string idVideo;
            int entityMediaID;
            for (int i = 0; i < model.TotalVideo; i++)
            {
                idVideo = string.Format("{0}{1}", i.ToString(), "video");
                entityMediaID = model.VideoURL[i].EntityMediaID;

                EntityMedia _entitymediaRecord = ArticleServices.GetInstance.GetEntityMediaVideoRecordByEntityMediaID(entityMediaID);

                model.IsFeaturedVideos = _entitymediaRecord.IsFeatured;
                ValidateAndUpdateVideoUrl(idVideo, entityMediaID, model.ArticleID, model.IsFeaturedVideos);
            }

            //upload new video url
            for (int i = 1; i <= model.TotalNewUploadedVideos; i++)
            {
                if (model.IsFeaturedNameVideos == "radio" + i.ToString())
                {
                    model.IsFeaturedVideos = true;
                }
                else
                {
                    model.IsFeaturedVideos = false;
                }
                ValidateVideoUrlAndUpload("video" + i.ToString(), model.ArticleID, model.IsFeaturedVideos);
            }
            #endregion
            model.ListUploadedPictures = ArticleServices.GetInstance.GetEntityMediaImageRecordByArticleID(model.ArticleID);
            #region Images
            if (Request.Files.Count > 0)
            {
                string idFinder;
                for (int i = 0; i < model.TotalUploadedImages; i++)
                {
                    idFinder = string.Format("{0}{1}{2}", model.ListUploadedPictures[i].EntityMediaID, "image", i.ToString());
                    UpdateAndValidateCurrentImage(idFinder, model.ArticleID, model.IsFeaturedImage, model.ListUploadedPictures[i].EntityMediaID);
                }
            }

            // upload new image 
            if (Request.Files.Count > 0)
            {
                for (int i = 2; i <= model.TotalNewUploadedImages; i++)
                {
                    if (model.IsFeaturedNameImage == "radioImage" + i.ToString())
                    {
                        model.IsFeaturedImage = true;
                    }
                    else
                    {
                        model.IsFeaturedImage = false;
                    }
                    ValidateImageAndUpload("image" + i.ToString(), _article.ArticleID, model.IsFeaturedImage, null);

                }
            }
            #endregion
           
            ArticleServices.GetInstance.UpdateArticle(_article);
            return RedirectToAction("ManagementArticles");
        }

        private void ValidateAndUpdateVideoUrl(String id, int entityMediaID, int articleId, bool? isfeatured)
        {
            string url = System.Web.HttpContext.Current.Request.Form[id];
            if (url != "" || url == null)
            {
                EntityMedia _entityVideo = ArticleServices.GetInstance.GetEntityMediaVideoRecordByEntityMediaID(entityMediaID);
                string VideoId = YoutubeHelper.getYoutubeVideoID(url);
                string videoThumbnail = string.Format("http://img.youtube.com/vi/{0}/hqdefault.jpg", VideoId);
                _entityVideo.URL = url;
                _entityVideo.Thumbnail = videoThumbnail;
                _entityVideo.IsFeatured = isfeatured;
                ArticleServices.GetInstance.UpdateArticleImage(_entityVideo);
            }
        }

        public int ModifyFeatured(int entityMediaID, int articleId, int creativeTypeID)
        {
            int prevFeatured = 0;
            if (creativeTypeID == 1)
            {
                List<EntityMedia> entities = ArticleServices.GetInstance.GetEntityMediaImageRecordByArticleID(articleId);
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i].IsFeatured == true)
                    {
                        prevFeatured = entities[i].EntityMediaID;
                    }
                }
            }
            else
            {
                List<EntityMedia> entities = ArticleServices.GetInstance.GetEntityMediaAllVideoURLByArticleID(articleId);
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i].IsFeatured == true)
                    {
                        prevFeatured = entities[i].EntityMediaID;
                    }
                }
            }



            if (prevFeatured > 0)
            {
                EntityMedia _entityFeaturedBefore = ArticleServices.GetInstance.GetEntityMediaRecordByEntityMediaID(prevFeatured);
                _entityFeaturedBefore.IsFeatured = false;

                ArticleServices.GetInstance.UpdateCompaignImage(_entityFeaturedBefore);
            }
            EntityMedia _entity = ArticleServices.GetInstance.GetEntityMediaRecordByEntityMediaID(entityMediaID);
            _entity.IsFeatured = true;

            ArticleServices.GetInstance.UpdateCompaignImage(_entity);

            return 1;

        }

        [HttpGet]
        public ActionResult TitleDataAutocomplete(int id, string name)
        {
            var articleList = new List<Article>();
            articleList = ArticleServices.GetInstance.GetArticleListByTitleFilter(name);
            var result = (from c in articleList
                          select new
                          {
                              ArticleID = c.ArticleID,
                              ArticleData = c.Title
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SourceDataAutocomplete(int id, string name)
        {
            var sourceList = new List<Source>();
            sourceList = ArticleServices.GetInstance.GetArticleListBySourceFilter(name);
            var result = (from c in sourceList
                          select new
                          {
                              ArticleID = c.SourceID,
                              ArticleData = c.Name
                          }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CityDataAutocomplete(int id, string name)
        {

            var cityList = new List<City>();
            cityList = ArticleServices.GetInstance.GetArticleListByCityFilter(name);
            var result = (from c in cityList
                          select new
                          {
                              ArticleID = c.CityID,
                              ArticleData = c.Name
                          }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ArticleIDDataAutocomplete(int id, string name)
        {

            var IDList = new List<Article>();
            IDList = ArticleServices.GetInstance.GetArticleIDs(name);
            var result = (from c in IDList
                          select new
                          {
                              ArticleID = c.ArticleID,
                              ArticleData = c.ArticleID
                          }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateListing(int id, int elementId)
        {
            List<ArticleModel> _articles = new List<ArticleModel>();
            if (id == 1)
            {
                _articles = ArticleServices.GetInstance.GetAllArticleByTitle(elementId);
            }
            else if (id == 2)
            {
                _articles = ArticleServices.GetInstance.GetAllArticleBySource(elementId);
            }
            else if (id == 5)
            {
                _articles = ArticleServices.GetInstance.GetAllArticleIDs(elementId);
            }
            else
            {
                _articles = ArticleServices.GetInstance.GetAllArticleByCity(elementId);
            }
            foreach (var items in _articles)
            {
                items.CityName = ArticleServices.GetInstance.GetCityNameByCityID(items.CityID);
                if (items.CityName == null)
                {
                    string stateName = ArticleServices.GetInstance.GetStateNameByStateID(items.StateID);
                    if (stateName != null)
                    {
                        items.CityName = stateName;
                    }
                }
                items.SourceName = ArticleServices.GetInstance.GetSourceNameBySourceID(items.SourceID);
                if (items.CityName == null)
                {
                    items.CityName = "None";
                }

                if (items.SourceName == null)
                {
                    items.SourceName = "None";
                }
            }
            return PartialView("PartialProjectListing", _articles);
        }


        public string ConvertDateCalendar(DateTime DateConv, string Calendar, string DateLangCulture)
        {
            System.Globalization.DateTimeFormatInfo DTFormat;
            DateLangCulture = DateLangCulture.ToLower();
            /// We can't have the hijri date writen in English. We will get a runtime error - LAITH - 11/13/2005 1:01:45 PM -

            if (Calendar == "Hijri" && DateLangCulture.StartsWith("en-"))
            {
                DateLangCulture = "ar-sa";
            }

            /// Set the date time format to the given culture - LAITH - 11/13/2005 1:04:22 PM -
            DTFormat = new System.Globalization.CultureInfo(DateLangCulture, false).DateTimeFormat;

            /// Set the calendar property of the date time format to the given calendar - LAITH - 11/13/2005 1:04:52 PM -
            switch (Calendar)
            {
                case "Hijri":
                    DTFormat.Calendar = new System.Globalization.HijriCalendar();
                    break;

                case "Gregorian":
                    DTFormat.Calendar = new System.Globalization.GregorianCalendar();
                    break;

                default:
                    return "";
            }

            /// We format the date structure to whatever we want - LAITH - 11/13/2005 1:05:39 PM -
            //DTFormat.ShortDatePattern = "dd/MM/yyyy";
            return (DateConv.Date.ToString("f", DTFormat));
        }



    }
}

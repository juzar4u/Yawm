using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class ClassifiedsController : Controller
    {
        //
        // GET: /Classifieds/
        public ActionResult Index(int pageNo)
        {
            int pageSize = 10;
            List<ClassifiedIndexModel> ClassifiedList = new List<ClassifiedIndexModel>();
            UserModel user = AuthHelper.LoginFromCookie();
            if (user != null)
            {
                PetaPoco.Page<ClassifiedIndexModel> entities = ClassifiedServices.GetInstance.GetAllClassified(pageNo, pageSize, user);
                ViewBag.currentPages = entities.CurrentPage;
                ViewBag.TotalPages = entities.TotalPages;
                ClassifiedList = entities.Items;
                ViewBag.currentPage = entities.CurrentPage;
                ViewBag.TotalPages = entities.TotalPages;
                ViewBag.ArticleCount = entities.TotalItems;
                if (pageNo > 1)
                    return PartialView("~/Views/Shared/PartialClassifiedIndexModelView.Mobile.cshtml", ClassifiedList);
                else
                    return View(ClassifiedList);
               
            }
            return Redirect("/Account/Login/?returnUrl=/classifieds/index/1");
        }


        [HttpGet]
        public ActionResult PostNow()
        {
            UserModel user = AuthHelper.LoginFromCookie();
            if (user != null)
            {
                ClassifiedModel _classified = new ClassifiedModel();
                _classified.UserID = (int)user.UserID;
                _classified.CountryList = PP_ArticleServices.GetInstance.GetCountryList();
                _classified.CurrencyList = PP_ArticleServices.GetInstance.GetCurrencyList();
                _classified.ClassifiedAdParentList = ClassifiedServices.GetInstance.GetParentClassifiedCategories();
                return View(_classified);
            }
            return Redirect("/Account/Login/?returnUrl=/classifieds/postnow");
        }

        [HttpPost]
        public ActionResult PostNow(ClassifiedModel model)
        {
           
                try
                {
                    Classified _classified = new Classified();
                    
                    List<CreativeModel> media = new List<CreativeModel>();
                    _classified.Title = model.Title;
                    _classified.Description = model.Description;
                    _classified.CityID = model.CityID;
                    _classified.PhoneCode = model.PhoneCode;
                    _classified.PhoneNo = model.PhoneNo.ToString();
                    _classified.CurrencyID = model.CurrencyID;
                    _classified.Price = model.price;
                    _classified.Address = model.address;
                    _classified.UserID = model.UserID;
                    _classified.PublishedDate = DateTime.Now;
                    _classified.ClassifiedAdCategoryID = model.ChildClassifiedAdCategoryID;
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            HttpPostedFile file = System.Web.HttpContext.Current.Request.Files[i];
                            if(file.ContentLength > 0)
                            media.Add(ValidateAndUploadImage(file));
                        }
                    }
                    foreach (var item in media)
                    {
                        if (!string.IsNullOrEmpty(item.ErrorMessage))
                        {
                            model.CountryList = PP_ArticleServices.GetInstance.GetCountryList();
                            model.CurrencyList = PP_ArticleServices.GetInstance.GetCurrencyList();
                            ViewBag.Error = item.ErrorMessage;
                            return View(model);
                        }
                             
                    }

                    _classified.ClassifiedID = ClassifiedServices.GetInstance.InsertClassified(_classified);
                    foreach (var item in media)
                    {
                        NewsfeedClassifiedMedia _creativeMedia = new NewsfeedClassifiedMedia();
                        _creativeMedia.CreativeTypeID = (int)CreativeTypeEnum.Images;
                        _creativeMedia.EntityID = _classified.ClassifiedID;
                        _creativeMedia.EntityTypeID = (int)EntityTypeEnum.Classifieds;
                        _creativeMedia.Thumbnail = item.Thumbnail;
                        _creativeMedia.URL = item.Url;

                        ClassifiedServices.GetInstance.InsertNewsfeedClassifiedMedia(_creativeMedia);
                    }
                }
    
                catch (Exception ex)
                {
                    ClassifiedModel _classified = new ClassifiedModel();
                    _classified.CountryList = PP_ArticleServices.GetInstance.GetCountryList();
                    _classified.CurrencyList = PP_ArticleServices.GetInstance.GetCurrencyList();
                    ViewBag.Error = ex.Message;
                    return View(_classified);
                }

                return Redirect("/Classifieds/Index/1");
            
        }

        public ActionResult Comment(int classifiedID)
        {
            UserModel user = AuthHelper.LoginFromCookie();
            CommentMasterModel _comment = new CommentMasterModel();
            _comment.User = user;
            _comment.ArticleCategoryTypeID = (int)CommentCategoryEnum.ClassifiedType;
            _comment.ArticleID = classifiedID;
            _comment.MasterComments = ClassifiedServices.GetInstance.GetClassifiedCommentsByClassifiedID(classifiedID, _comment.ArticleCategoryTypeID);
            return View(_comment);
        }

        [HttpGet]
        public ActionResult GetChildClassifiedCategories(int parentCategoryID)
        {

            var childs = ClassifiedServices.GetInstance.GetChildClassifiedCategories(parentCategoryID);
            var result = (from c in childs
                          select new
                          {
                              ClassifiedAdCategoryID = c.ClassifiedAdCategoryID,
                              Name = c.Name

                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public CreativeModel ValidateAndUploadImage(HttpPostedFile postedFile)
        {
            CreativeModel _media = new CreativeModel();

            if (postedFile.ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(postedFile.FileName);

                if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                {
                    WebImage img = new WebImage(postedFile.InputStream);
                    if (postedFile != null)
                    {
                        if ((postedFile.ContentType.Contains("image")))
                        {
                            string fileName = Guid.NewGuid().ToString();
                            if (img.Width > 600 & img.Height > 400)
                            {
                                img.Resize(600, 400);
                                img.Save(HttpContext.Server.MapPath("~/Images/Classifieds/") + "Large" + fileName +extension);
                                img.Resize(400, 150);
                                img.Save(HttpContext.Server.MapPath("~/Images/Classifieds/") + fileName + extension);
                                _media.Url = "/Images/Classifieds/" + "Large" + fileName + extension;
                                _media.Thumbnail = "/Images/Classifieds/" + fileName + extension;
                                _media.creativeType = "Image";
                                _media.ErrorMessage = null;
                            }
                            else
                            {
                                img.Save(HttpContext.Server.MapPath("~/Images/Classifieds/") + fileName + extension);
                                _media.Thumbnail = "/Images/Classifieds/" + fileName + extension;
                                _media.Url = null;
                                _media.creativeType = "Image";
                                _media.ErrorMessage = null;
                            }
                        }
                    }
                    else
                    {
                        _media.ErrorMessage = "File Could not be Uploaded!";
                    }
                }
                else
                {
                    _media.ErrorMessage = "The Uploaded File is not an Image!";
                }
            }
            else
            {
                _media.ErrorMessage = "File Could not be Uploaded!";
            }

            return _media;
        }

        [HttpGet]
        public int TriggerLike(int userId, int entityId, int entityCategoryID, bool isLiked)
        {
            UserLikes like = ClassifiedServices.GetInstance.GetUserLike(userId,entityId,entityCategoryID);
            //JsonUserLikes jsonLikes = new JsonUserLikes();

            int likecount = 0;
            if (like == null)
            {
                UserLikes likes = new UserLikes();
                likes.UserID = userId;
                likes.EntityID = entityId;
                likes.EntityCategoryID = entityCategoryID;
                likes.IsLike = isLiked;

               likecount = ClassifiedServices.GetInstance.InsertUserLikes(likes);

            }
            else
            {
                like.UserID = userId;
                like.EntityID = entityId;
                like.EntityCategoryID = entityCategoryID;
                like.IsLike = isLiked;
                likecount = ClassifiedServices.GetInstance.UpdateUserLikes(like);
            }
            //jsonLikes.CountText = "LikeCount";
            //jsonLikes.CountData = likecount;

            return likecount;
        }

        public ActionResult GetFilterLayout()
        {
            FilterClassifiedModel model = new FilterClassifiedModel();
            model.ParentCategories = ClassifiedServices.GetInstance.GetParentClassifiedCategories();
            model.CityList = ClassifiedServices.GetInstance.GetCityListOfClassifieds();
            return PartialView("~/Views/Classifieds/PartialFilterClassifieds.cshtml", model);
        }
        public ActionResult FilterClassified(FilterClassifiedModel item)
        {
            int elements = 0;
            if (item.ChildClassifiedAdCategoryID > 0)
            {
                elements++;
            }
            if (item.CityID > 0)
            {
                elements++;
            }
            if (item.MinPrice > 0)
            {
                elements++;
            }
            if (item.MaxPrice > 0)
            {
                elements++;
            }
            if (elements != 0)
            {
                List<ClassifiedIndexModel> filterclassifiedLists = ClassifiedServices.GetInstance.GetFilterClassifiedList(item, elements);
                return View("~/Views/Classifieds/Index.Mobile.cshtml", filterclassifiedLists);
            }
            else
            {
                return Redirect("/Classifieds/index/1");
            }
        }


        [HttpGet]
        public ActionResult EditClassified(int classifiedId)
        {
            ClassifiedModel model = new ClassifiedModel();
            UserModel user = AuthHelper.LoginFromCookie();
            
            if (user != null)
            {
                
                model = ClassifiedServices.GetInstance.GetclassifiedById(classifiedId);
                model.UserID = (int)user.UserID;
                model.CountryList = PP_ArticleServices.GetInstance.GetCountryList();
                model.CurrencyList = PP_ArticleServices.GetInstance.GetCurrencyList();
                model.ClassifiedAdParentList = ClassifiedServices.GetInstance.GetParentClassifiedCategories();
                model.ClassifiedAdChildList = ClassifiedServices.GetInstance.GetChildClassifiedCategories(model.ParentClassifiedAdCategoryID);
                if (user.UserID == AuthHelper.LoggedInUserID)
                    return View(model);
                else
                    return Redirect("/Classifieds/index/1");
            }
            return Redirect(string.Format("/Account/Login/?returnUrl=/classifieds/EditClassified/?classifiedId={0}", classifiedId));
        }

        [HttpPost]
        public ActionResult EditClassified(ClassifiedModel model)
        {
            return Redirect("/classifieds/index/1");
        }


        [HttpGet]
        public ActionResult DeleteClassified(int id)
        {
            classifiedDeleteModel model = new classifiedDeleteModel();
            model.ClassifiedID = id;
            ViewBag.message = "Are you sure you wish to delete this classified Ad!";
            return View(model);
        }


        [HttpPost]
        public ActionResult DeleteClassified(classifiedDeleteModel model)
        {
            Classified classifieds = ClassifiedServices.GetInstance.GetclassifiedForDeleteById(model.ClassifiedID);
            ClassifiedServices.GetInstance.DeleteClassified(classifieds);
            return Redirect("/classifieds/index/1");
        }
    }
}

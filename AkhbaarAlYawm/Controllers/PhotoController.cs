using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.CommonCode.Helpers;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PhotoModel;
using AkhbaarAlYawm.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Controllers
{
    [Authentication]
    public class PhotoController : Controller
    {
        //
        // GET: /Photo/

        public ActionResult Index(int pageId)
        {
            int pageSize = 10;
            //List<PhotoAlbumListModel> _photoAlbumList = new List<PhotoAlbumListModel>();
            PetaPoco.Page<PhotoAlbumListModel> entities = PhotoServices.GetInstance.GetPhotoAlbumListModel(pageId, pageSize);
            ViewBag.currentPage = entities.CurrentPage;
            ViewBag.totalCount = entities.TotalItems;
            return View(entities.Items);
        }

        [HttpGet]
        public ActionResult CreateAlbum()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult CreateAlbum(PhotoAlbumModel _photoAlbum)
        {
            PhotoAlbum _photo = new PhotoAlbum();

            _photo.Title = _photoAlbum.Title;
            _photo.PhotoAlbumID = PhotoServices.GetInstance.InsertPhotoAlbum(_photo);

            #region Photo
            if (Request.Files.Count > 0)
            {
                PhotoImageUpload("photoAlbum1", _photo.PhotoAlbumID, _photo.Title);

            }
            #endregion
            PhotoServices.GetInstance.UpdatePhotoAlbumShortUrl(_photo.PhotoAlbumID, Constants.GetBitlyURLForPhotoAlbum(_photo.PhotoAlbumID));
            // PhotoServices.GetInstance.UpdatePhotoAlbum(_photo);
            return RedirectToAction("Index");
        }

        public ActionResult GetPhotoGallery(int Id)
        {
            List<PhotoGalleryModel> _model = new List<PhotoGalleryModel>();

            _model = PhotoServices.GetInstance.GetPhotoGalleryByAlbumID(Id);

            return PartialView("_partialPhotoGalleryView", _model);
        }

        [HttpGet]
        public ActionResult CreateGallery()
        {
            PhotoGalleryModel _photoGallery = new PhotoGalleryModel();
            _photoGallery.AlbumList = PhotoServices.GetInstance.GetPhotoAlbumList();

            return View(_photoGallery);
        }

        [HttpPost]
        public ActionResult CreateGallery(PhotoGalleryModel _photoGallery)
        {
            PhotoGallery _photos = new PhotoGallery();

            _photos.PhotoAlbumID = _photoGallery.PhotoAlbumID;

            #region Photo

            if (Request.Files.Count > 0)
            {
                for (int i = 2; i <= _photoGallery.TotalUploadedImages; i++)
                {
                    PhotoGalleryIndividualImageUpload("image" + i.ToString(),_photos.PhotoAlbumID);
                }
            }
            #endregion
            // PhotoServices.GetInstance.UpdatePhotoAlbum(_photo);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditPhotoAlbum(int PhotoAlbumID)
        {
            PhotoAlbumListModel _model = new PhotoAlbumListModel();
            _model = PhotoServices.GetInstance.GetPhotoAlbumListModelByAlbumID(PhotoAlbumID);
            return View(_model);
        }

        [HttpPost]
        public ActionResult EditPhotoAlbum(PhotoAlbumListModel model)
        {
            List<PhotoGalleryModel> _photoGalleryList = new List<PhotoGalleryModel>();
            _photoGalleryList = PhotoServices.GetInstance.GetPhotoGalleryByAlbumID(model.PhotoAlbumID);
            // upload new CoverImage 
            #region CoverImage
            if (Request.Files.Count > 0)
            {
                for (int i = 1; i <= model.CoverUploads; i++)
                {
                    PhotoImageUpload("cover" + i.ToString(), model.PhotoAlbumID, model.Title);
                }
            }
            #endregion

            #region GalleryImages
            if (Request.Files.Count > 0)
            {
                string idFinder;
                for (int i = 0; i < model.TotalUploadedImages; i++)
                {
                    idFinder = string.Format("{0}{1}{2}", _photoGalleryList[i].PhotoGalleryID, "image", i.ToString());
                    UpdateGalleryImage(idFinder, _photoGalleryList[i].PhotoGalleryID, _photoGalleryList[i].PhotoAlbumID);
                }
            }

            // upload new Gallery image 
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i <= model.TotalNewUploadedImages; i++)
                {
                    PhotoGalleryIndividualImageUpload("image" + i.ToString(), model.PhotoAlbumID);
                }
            }
            #endregion
            return RedirectToAction("Index");
        }

        private int PhotoGalleryIndividualImageUpload(string fileUploadId, int photoAlbumID)
        {
            string ImageUrl = "";
            string thumbnailUrl = "";
            int creativeId = 0;
            HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[fileUploadId];

            if (postedFile != null && postedFile.ContentLength > 0)
            {
                WebImage img = new WebImage(postedFile.InputStream);
                string extension = System.IO.Path.GetExtension(postedFile.FileName);
                if ((postedFile.ContentType.Contains("image")))
                {
                    PhotoGallery _photo = new PhotoGallery();
                    if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                    {
                        string fileName = Guid.NewGuid().ToString();
                        if (img.Width > 1000 & img.Height > 700)
                        {
                            img.Resize(1000, 700);
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + "Large" + fileName + extension);
                            img.Resize(500, 365);
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + fileName + extension);
                            ImageUrl = "/Images/PhotoGallery/" + "Large" + fileName + extension;
                            thumbnailUrl = "/Images/PhotoGallery/" + fileName + extension;
                            _photo.PhotoAlbumID = photoAlbumID;
                            _photo.PhotoImgUrl = ImageUrl;
                            _photo.PhotoThumbnailUrl = thumbnailUrl;

                            PhotoServices.GetInstance.InsertPhotoGallery(_photo);
                        }
                        else
                        {
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + fileName + extension);
                            thumbnailUrl = "/Images/PhotoGallery/" + fileName + extension;
                            _photo.PhotoAlbumID = photoAlbumID;
                            _photo.PhotoThumbnailUrl = thumbnailUrl;
                            PhotoServices.GetInstance.InsertPhotoGallery(_photo);
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


        private int PhotoImageUpload(string fileUploadId, int photoAlbumID, string title)
        {
            string ImageUrl = "";
            string thumbnailUrl = "";
            int creativeId = 0;
            HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[fileUploadId];

            if (postedFile != null && postedFile.ContentLength > 0)
            {
                WebImage img = new WebImage(postedFile.InputStream);
                string extension = System.IO.Path.GetExtension(postedFile.FileName);
                if ((postedFile.ContentType.Contains("image")))
                {
                    if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                    {
                        string fileName = Guid.NewGuid().ToString();
                        PhotoAlbum _photo = new PhotoAlbum();
                        if (img.Width > 1000 & img.Height > 700)
                        {
                            img.Resize(1000, 700);
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + "Large" + fileName + extension);
                            img.Resize(500, 365);
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + fileName + extension);
                            ImageUrl = "/Images/PhotoGallery/" + "Large" + fileName + extension;
                            thumbnailUrl = "/Images/PhotoGallery/" + fileName + extension;
                            _photo.PhotoAlbumID = photoAlbumID;
                            _photo.CoverPhotoImg = ImageUrl;
                            _photo.Title = title;
                            _photo.CoverPhotoThumbnailImg = thumbnailUrl;
                            _photo.publishedDate = DateTime.Now;
                            PhotoServices.GetInstance.UpdatePhotoAlbum(_photo);
                        }
                        else
                        {
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + fileName + extension);
                            thumbnailUrl = "/Images/PhotoGallery/" + fileName + extension;
                            _photo.PhotoAlbumID = photoAlbumID;
                            _photo.Title = title;
                            _photo.CoverPhotoThumbnailImg = thumbnailUrl;
                            _photo.publishedDate = DateTime.Now;
                            PhotoServices.GetInstance.UpdatePhotoAlbum(_photo);
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

        private int UpdateGalleryImage(string fileUploadId, int photoGalleryID, int AlbumID)
        {
            string ImageUrl = "";
            string thumbnailUrl = "";
            int creativeId = 0;
            HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[fileUploadId];

            if (postedFile != null && postedFile.ContentLength > 0)
            {
                WebImage img = new WebImage(postedFile.InputStream);
                string extension = System.IO.Path.GetExtension(postedFile.FileName);
                if ((postedFile.ContentType.Contains("image")))
                {
                    PhotoGallery _photo = new PhotoGallery();
                    if (extension == ".bmp" || extension == ".gif" || extension == ".jpg" || extension == ".png" || extension == ".psd" || extension == ".pspimage" || extension == ".thm" || extension == ".tif" || extension == ".yuv")
                    {
                        string fileName = Guid.NewGuid().ToString();
                        if (img.Width > 1000 & img.Height > 700)
                        {
                            img.Resize(1000, 700);
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + "Large" + fileName + extension);
                            img.Resize(500, 365);
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + fileName + extension);
                            ImageUrl = "/Images/PhotoGallery/" + "Large" + fileName + extension;
                            thumbnailUrl = "/Images/PhotoGallery/" + fileName + extension;
                            _photo.PhotoGalleryID = photoGalleryID;
                            _photo.PhotoAlbumID = AlbumID;
                            _photo.PhotoImgUrl = ImageUrl;
                            _photo.PhotoThumbnailUrl = thumbnailUrl;
                            PhotoServices.GetInstance.UpdatePhotoGallery(_photo);
                        }
                        else
                        {
                            img.Save(HttpContext.Server.MapPath("~/Images/PhotoGallery/") + fileName + extension);
                            thumbnailUrl = "/Images/PhotoGallery/" + fileName + extension;

                            _photo.PhotoGalleryID = photoGalleryID;
                            _photo.PhotoAlbumID = AlbumID;
                            _photo.PhotoThumbnailUrl = thumbnailUrl;
                            PhotoServices.GetInstance.UpdatePhotoGallery(_photo);
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

        [HttpGet]
        public ActionResult TitleDataAutocomplete(string name)
        {
            var albumList = new List<PhotoAlbum>();
            albumList = PhotoServices.GetInstance.GetPhotoByTitleFilter(name);
            var result = (from c in albumList
                          select new
                          {
                              ArticleID = c.PhotoAlbumID,
                              ArticleData = c.Title
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateListing(int elementId)
        {
            List<PhotoAlbumListModel> _articles = new List<PhotoAlbumListModel>();
            _articles = PhotoServices.GetInstance.GetPhotoAlbumByAlbumIDForAutocomplete(elementId);
            
            foreach (var items in _articles)
            {
                items.PhotoGalleryCount = PhotoServices.GetInstance.GetPhotoGalleryCount(items.PhotoAlbumID);
            }
            return PartialView("_PartialPhotoAlbumListingView", _articles);
        }

    }


}


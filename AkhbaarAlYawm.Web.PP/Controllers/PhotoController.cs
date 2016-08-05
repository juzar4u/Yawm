using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PhotoModel;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class PhotoController : Controller
    {
        //
        // GET: /Photo/

        public ActionResult Index(int pageNo)
        {
            int pageSize = 10;
            List<PhotoAlbumModel> _photoAlbumList = new List<PhotoAlbumModel>();

            //_photoAlbumList = PhotoServices.GetInstance.GetPhotoAlbumModelList().OrderByDescending(x=>x.publishedDate).ToList();
            PetaPoco.Page<PhotoAlbumModel> entities = PhotoServices.GetInstance.GetPhotoAlbumModelList(pageNo, pageSize);
            ViewBag.currentPages = entities.CurrentPage;
            ViewBag.TotalPages = entities.TotalPages;
            _photoAlbumList = entities.Items;
            if (pageNo > 1)
                return PartialView("~/Views/Shared/_sharedAkhbaarPhotoListing.Mobile.cshtml", _photoAlbumList);
            else
                return View(_photoAlbumList);
        }

        public ActionResult Detail(int AlbumID)
        {
            DetailModel _Detail = new DetailModel();
            _Detail.SocialShare = new SocialSharePlugin();
            _Detail.Detail = PhotoServices.GetInstance.GetPhotoAlbumListModelByAlbumID(AlbumID);
            _Detail._recentlyAdded = PhotoServices.GetInstance.GetRecentPhotoAlbums(AlbumID);
            _Detail.SocialShare.Title = _Detail.Detail.Title;
            _Detail.SocialShare.shareurl = _Detail.Detail.ShortUrl;
            return View(_Detail);
        }

        public ActionResult demo()
        {
            return View();
        }
             

    }
}

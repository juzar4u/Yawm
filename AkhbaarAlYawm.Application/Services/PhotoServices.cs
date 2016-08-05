using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Helper.CacheManager;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PhotoModel;
using PetaPoco;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
{
    public class PhotoServices
    {
        private static PhotoServices _instace;

        public static PhotoServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new PhotoServices();
                }

                return _instace;
            }
        }

        public int UpdatePhotoAlbumShortUrl(int albumID, string shorturl)
        {
            PhotoAlbum photo = new PhotoAlbum();
            photo = GetPhotoAlbumByAlbumID(albumID);
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                photo.ShortUrl = shorturl;
                return (int)context.Update(photo);
            }
        }

        public int UpdatePhotoAlbum(PhotoAlbum _photoAlbum)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_photoAlbum);
            }
        }

        public int UpdatePhotoGallery(PhotoGallery _photoGallery)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_photoGallery);
            }
        }


        public int InsertPhotoAlbum(PhotoAlbum _photoAlbum)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_photoAlbum);
            }
        }

        public int InsertPhotoGallery(PhotoGallery _photoGallery)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_photoGallery);
            }
        }
        public PhotoAlbum GetPhotoAlbumByAlbumID(int albumID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<PhotoAlbum>("select * from photoalbum where photoalbumid =@0", albumID).FirstOrDefault();
            }
        }

        public List<PhotoAlbumListModel> GetPhotoAlbumByAlbumIDForAutocomplete(int albumID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<PhotoAlbumListModel>("select * from photoalbum where photoalbumid =@0", albumID);
            }
        }

        public List<PhotoAlbum> GetPhotoAlbumList()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<PhotoAlbum>("select * from photoalbum");
            }
        }

        public Page<PhotoAlbumModel> GetPhotoAlbumModelList(int pageNo, int pageSize)
        {
            string key = string.Format("GetPhotoAlbumModelList#{0}#{1}", pageNo, pageSize);
            Page<PhotoAlbumModel> result = CacheManager.Get(key) as Page<PhotoAlbumModel>;
            if (result == null)
            {
                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                   var ppsql = "select * from photoalbum order by publishedDate desc";
                   result = context.Page<PhotoAlbumModel>(pageNo, pageSize, ppsql);
                   foreach (var item in result.Items)
                   {
                       item.photoCount = context.Fetch<int>("select count(*) from PhotoGallery where PhotoAlbumID = @0", item.PhotoAlbumID).FirstOrDefault();
                       item.IslamicDate = context.Fetch<HijriBohra_Gregorian_Calendar>("select * from HijriBohra_Gregorian_Calendar where G_Day = @0 and G_Month = @1 and G_Year=@2", item.publishedDate.Day, item.publishedDate.Month, item.publishedDate.Year).FirstOrDefault();
                   }
                   CacheManager.Set(key, result, DateTime.Now.AddSeconds(ApplicationConstants.ListCacheTimeInSec));
                }
            }

            return (result);
        }

        public List<PhotoGalleryModel> GetPhotoGalleryByAlbumID(int albumID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<PhotoGalleryModel>("select * from PhotoGallery where PhotoAlbumID = @0", albumID);
            }
        }

        public Page<PhotoAlbumListModel> GetPhotoAlbumListModel(int pageNo, int pageSize)
        {
            string key = string.Format("GetPhotoAlbumListModel#{0}#{1}", pageNo, pageSize);
            Page<PhotoAlbumListModel> result = CacheManager.Get(key) as Page<PhotoAlbumListModel>;
            if (result == null)
            {
                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                    var ppsql = "select * from photoalbum";
                    result = context.Page<PhotoAlbumListModel>(pageNo, pageSize, ppsql);
                    foreach (var items in result.Items)
                    {
                        items.PhotoGalleryCount = context.Fetch<int>("select count(*) from PhotoGallery where PhotoAlbumID = @0", items.PhotoAlbumID).FirstOrDefault();
                    }

                }
            }
            return result;
        }

        public PhotoAlbumListModel GetPhotoAlbumListModelByAlbumID(int Id)
        {
            PhotoAlbumListModel _model = new PhotoAlbumListModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _model = context.Fetch<PhotoAlbumListModel>("select * from photoalbum where photoAlbumId = @0", Id).FirstOrDefault();
                _model.PhotoGalleryList = GetPhotoGalleryByAlbumID(_model.PhotoAlbumID);
                _model.PhotoGalleryCount = _model.PhotoGalleryList.Count;
            }
            return _model;
        }

        public List<PhotoAlbumListModel> GetRecentPhotoAlbums(int AlbumID)
        {
            List<PhotoAlbumListModel> model = new List<PhotoAlbumListModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                model = context.Fetch<PhotoAlbumListModel>("select top 5 * from PhotoAlbum where PhotoAlbumID <> @0 order by publishedDate desc", AlbumID);
                foreach (var items in model)
                {
                    items.PhotoGalleryCount = context.Fetch<int>("select count(*) from PhotoGallery where PhotoAlbumID = @0", items.PhotoAlbumID).FirstOrDefault();
                }

            }
            return model;
        }


        public List<PhotoAlbum> GetPhotoByTitleFilter(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<PhotoAlbum>(String.Format("select * from PhotoAlbum where Title like '%{0}%'", name));
            }
        }

        public int GetPhotoGalleryCount(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select count(*) from PhotoGallery where PhotoAlbumID = @0", id).FirstOrDefault();
               
            }
        }

    }
}

using AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities.PhotoModel
{
    public class PhotoAlbumModel
    {
        public int PhotoAlbumID { get; set; }
        public string Title { get; set; }
        public string CoverPhotoImg { get; set; }
        public string CoverPhotoThumbnailImg { get; set; }
        public DateTime publishedDate { get; set; }
        public HijriBohra_Gregorian_Calendar IslamicDate { get; set; }
        public int photoCount { get; set; }
    }

    public class PhotoGalleryModel
    {
        public int PhotoGalleryID { get; set; }
        public string PhotoImgUrl { get; set; }
        public string PhotoThumbnailUrl { get; set; }
        public int PhotoAlbumID { get; set; }
        public List<PhotoAlbum> AlbumList { get; set; }
        public int TotalUploadedImages { get; set; }

    }

    public class PhotoAlbumListModel
    {
        public int PhotoAlbumID { get; set; }
        public string Title { get; set; }
        public string CoverPhotoImg { get; set; }
        public string CoverPhotoThumbnailImg { get; set; }
        public DateTime publishedDate { get; set; }
        public List<PhotoGalleryModel> PhotoGalleryList { get; set; }
        public int CoverUploads { get; set; }
        public int TotalUploadedImages { get; set; }
        public int TotalNewUploadedImages { get; set; }
        public int PhotoGalleryCount { get; set; }
        public string ShortUrl { get; set; }
    }

    public class DetailModel
    {
        public List<PhotoAlbumListModel> _recentlyAdded { get; set; }
        public PhotoAlbumListModel Detail { get; set; }
        public SocialSharePlugin SocialShare { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AkhbaarAlYawm.CDN.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        
        public string Index(WebImage img)
        {
            string profileImageUrl = "";
            string thumbnailUrl = "";

            //if (postedFile.ContentLength > 0)
            //{
            //    WebImage img = new WebImage(postedFile.InputStream);
            //    if (postedFile != null)
            //    {
            //        if ((postedFile.ContentType.Contains("image")))
            //        {

            //            //if (img.Width > 600 & img.Height > 600)
            //            //{
            //            //    img.Resize(600, 600);
            //            //    img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + "Large" + postedFile.FileName);
            //            //    img.Resize(300, 300);
            //            //    img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + postedFile.FileName);
            //            //    profileImageUrl = "/Images/Articles/" + "Large" + postedFile.FileName;
            //            //    thumbnailUrl = "/Images/Articles/" + postedFile.FileName;
            //            //}
            //            //else
            //            //{
            //            //    img.Save(HttpContext.Server.MapPath("~/Images/Articles/") + postedFile.FileName);
            //            //    thumbnailUrl = "/Images/Articles/" + postedFile.FileName;
            //            //}

            //        }
            //    }
            //    else
            //    {
            //        thumbnailUrl = null;
            //    }
            thumbnailUrl = img.FileName;
            return thumbnailUrl;
            }
            //else
            //{
            //    thumbnailUrl = null;
            //}
            
       // }

    }
}

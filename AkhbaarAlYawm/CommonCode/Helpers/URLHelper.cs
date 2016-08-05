using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spotunity.Web.CP.CommonCode.Helpers
{
    public static class URLHelper
    {
        public static string StyleSheet(this UrlHelper helper, string fileName)
        {
            return helper.Content(string.Format("~/content/{0}", fileName)).ToLower();
        }

        public static string Image(this UrlHelper helper, string imageName)
        {
            return helper.Content(string.Format("~/content/images/{0}", imageName)).ToLower();
        }

        public static string Script(this UrlHelper helper, string scriptFileName)
        {
            return helper.Content(string.Format("~/scripts/{0}", scriptFileName)).ToLower();
        }
    }
}
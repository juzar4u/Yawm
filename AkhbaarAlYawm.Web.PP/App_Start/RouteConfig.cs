using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AkhbaarAlYawm.Web.PP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
              "Classifieds",
              "Classifieds/Index/{pageNo}",
              new { controller = "Classifieds", action = "Index" }
          );

            
            routes.MapRoute(
              "AkhbaarList",
              "Home/AkhbaarList/{categoryId}/{isVideo}/{pageNo}",
              new { controller = "Home", action = "AkhbaarList" }
          );

            routes.MapRoute(
              "UserVerification",
              "Account/UserVerification/{userid}/{guid}",
              new { controller = "Account", action = "UserVerification" }
          );

            routes.MapRoute(
             "ForgotPassword",
             "Account/ForgotPassword/{userId}/{guid}",
             new { controller = "Account", action = "ForgotPassword" }
         );

            routes.MapRoute(
              "Detail",
              "Home/Detail/{id}",
              new { controller = "Home", action = "Detail" }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
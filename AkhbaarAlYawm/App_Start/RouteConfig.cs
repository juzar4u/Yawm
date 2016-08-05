using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AkhbaarAlYawm.Web.CP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
              "Miqaat",
              "Miqaat/AddMiqaats/{pageId}",
              new { controller = "Miqaat", action = "AddMiqaats" }
          );

            routes.MapRoute(
              "Photo",
              "Photo/Index/{pageId}",
              new { controller = "Photo", action = "Index" }
          );

            routes.MapRoute(
                "HomeIndex",
                "{controller}/{action}/{pageId}",
                new { controller = "Home", action = "Index", pageId = 1 }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                defaults: new { controller = "Account", action = "Login"}
            );
            
        }
    }
}
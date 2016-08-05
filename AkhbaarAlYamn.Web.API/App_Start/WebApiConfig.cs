using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AkhbaarAlYamn.Web.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                "Demo",
                "api/user",
                new { controller = "Demo", action = "GetUser" }

            );

            config.Routes.MapHttpRoute(
                "PostUserDataAndRegister",
                "api/user-PostUserDataAndRegister",
                new { controller = "User", action = "PostUserDataAndRegister" }

            );
            config.Routes.MapHttpRoute(
               "PostForgotPassword",
               "api/user-PostForgotPassword",
               new { controller = "User", action = "PostForgotPassword" }

           );
            config.Routes.MapHttpRoute(
                "PostLogout",
                "api/user-PostLogout",
                new { controller = "User", action = "PostLogout" }

            );
            config.Routes.MapHttpRoute(
               "PostLogin",
               "api/user-PostLogin",
               new { controller = "User", action = "PostLogin" }

           );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            
        }
    }
}

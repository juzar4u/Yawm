using AkhbaarAlYawm.Web.CP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace AkhbaarAlYawm.Web.CP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private static SimpleMembershipInitializer _initializer;
        //private static object _initializerLock = new object();
        //private static bool _isInitialized;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        //public class SimpleMembershipInitializer
        //{
        //    public SimpleMembershipInitializer()
        //    {
        //        using (var context = new UsersContext())
        //            context.UserProfiles.Find(1);

        //        if (!WebSecurity.Initialized)
        //            WebSecurity.InitializeDatabaseConnection("CPDefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: false);
        //    }
        //}
    }
}
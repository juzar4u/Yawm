using AkhbaarAlYawm.Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace AkhbaarAlYawm.CommonCode.Helpers
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            try
            {
                if (!AuthHelper.IsUserIsLoggedIn)
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/account/login");
                }
            }
            catch (HttpResponseException ex)
            {
                throw new Exception(ex.Response.ReasonPhrase);
            }
        }
    }
}
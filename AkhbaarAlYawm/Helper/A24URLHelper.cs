using System;
using System.Text.RegularExpressions;
using System.Text;
using Spotunity.Web.CP;
using System.Web.Security;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
public static class A24URLHelper
{


    public static string GetURLForPageNumber(int pageNumber, RequestContext reqContext)
    {
        string url = HttpContext.Current.Request.Url.AbsolutePath;
        string currentController = GetControllerAndActionFromURL(url)["Controller"];
        string currentAction = GetControllerAndActionFromURL(url)["Action"];
        RouteData rData = null;
        string retUrl = "";


        foreach (Route route in RouteTable.Routes)
        {
            if (route.RouteHandler is StopRoutingHandler)
                continue;//This rout is Ignored
            HttpContextWrapper req = new HttpContextWrapper(HttpContext.Current);

            rData = route.GetRouteData(req);
            if (rData == null)
                continue;

            string controller = rData.Values["controller"].ToString().ToLower();
            string action = rData.Values["action"].ToString().ToLower();
            string page = rData.Values.ContainsKey("pageId") ? rData.Values["pageId"].ToString() : pageNumber.ToString();
            if (!rData.Values.ContainsKey("pageId"))
            {
                rData.Values.Add("pageId", page);
            }
            else
            {
                rData.Values["pageId"] = pageNumber;
            }

            if (controller == currentController && action == currentAction)
            {
                retUrl = route.Url;
                if (!retUrl.Contains("{pageId}"))
                    retUrl = retUrl + "/{pageId}";
                string[] strs = retUrl.Split("/".ToCharArray());

                foreach (string str in strs)
                {
                    object strWith = rData.Values[str.Replace("{", "").Replace("}", "")];
                    strWith = strWith ?? str;
                    retUrl = retUrl.Replace(str, strWith.ToString());
                }
                break;
                //It is ok to return now we found the route date we need
            }
        }

        return retUrl;
    }

    private static Dictionary<string, string> GetControllerAndActionFromURL(string strRequestedUrl)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        //string strRequestedUrl = HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Request.ApplicationPath + "/", "").ToLower();

        int reqUrlSplitedCount = strRequestedUrl.Split("/".ToCharArray()).Count();
        foreach (Route route in RouteTable.Routes)
        {
            if (route.RouteHandler is StopRoutingHandler)
                continue;//This rout is Ignored
            HttpContextWrapper req = new HttpContextWrapper(HttpContext.Current);

            RouteData rDate = route.GetRouteData(req);
            if (rDate == null)
                continue;
            string controller = rDate.Values["controller"].ToString().ToLower();
            string action = rDate.Values["action"].ToString().ToLower();
            string page = rDate.Values.ContainsKey("pageId") ? rDate.Values["pageId"].ToString() : "1";
            //TODO: We had to verify this
            string routeUrl = route.Url.Replace("{controller}", controller).Replace("{action}", action);

            // if (routeUrl.Contains(strRequestedUrl))
            {
                result.Add("Controller", controller);
                result.Add("Action", action);
                break;
            }
        }
        return result;
    }

    public static string GetCurrentController()
    {
        string url = HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Request.ApplicationPath + "/", "").ToLower();
        string currentController = GetControllerAndActionFromURL(url)["Controller"];
        return currentController;
    }

    public static string GetCurrentAction()
    {
        string url = HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Request.ApplicationPath + "/", "").ToLower();
        string currentAction = GetControllerAndActionFromURL(url)["Action"];
        return currentAction;
    }


    public static string AllVideosURL(this UrlHelper url)
    {
        if (url == null)
        {
            url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        //string routeURL = url.RouteUrl("AllVideosWithTitle", new { title = "فيديو24", pageId = 1 });

        string routeURL = url.RouteUrl("video24", new {pageId = 1 });

        return routeURL.ToLower();
    }

    public static string ArticleListingURL(this UrlHelper url, int displayAreaID)
    {
        if (url == null)
        {
            url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        string titleVal = "اخبار 24"; // fault tolerant ;

        if (displayAreaID == 101)
        {
            titleVal = "حوادث"; 
        }
        else if (displayAreaID == 103)
        {
            titleVal = "عربية وعالمية";
        }
        else if (displayAreaID == 104)
        {
            titleVal = "منوعات";
        }

        string routeURL = url.RouteUrl("ArticleListWithTitle", new { displayAreaId = displayAreaID, title = titleVal, pageId = 1 });

        return routeURL.ToLower();
    }
    
    public static string SportsListURL(this UrlHelper url)
    {
        if (url == null)
        {
            url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        string routeURL = url.RouteUrl("SportsListWithTitle", new { title = "رياضة", pageId = 1 });

        return routeURL.ToLower();
    }

}
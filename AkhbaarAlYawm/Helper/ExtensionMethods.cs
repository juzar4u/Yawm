using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Net;
using System.IO;

/// <summary>
/// Summary description for ExtensionMethods
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Remove HTML tags from string using char array.
    /// </summary>
    public static string StripHtmlTagsCharArray(this string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.Empty;
        }
        char[] array = new char[source.Length];
        int arrayIndex = 0;
        bool inside = false;

        for (int i = 0; i < source.Length; i++)
        {
            char let = source[i];
            if (let == '<')
            {
                inside = true;
                continue;
            }
            if (let == '>')
            {
                inside = false;
                continue;
            }
            if (!inside)
            {
                array[arrayIndex] = let;
                arrayIndex++;
            }
        }

        return new string(array, 0, arrayIndex);
    }

    public static string removeLineBrake(this string content, string replaceWith)
    {
        return content.Replace("\r\n", replaceWith).Replace("\r", replaceWith).Replace("\n", replaceWith);
    }

    public static string TrimToSize(this string news, int size)
    {
        news = news.Trim();
        if (news.Length <= size)
        {
            return news;
        }
        else
        {
            return news.Substring(0, size - 1 );
        }
    }

    public static string TrimToSize(this string news, string more, int size)
    {
        news = news.Trim();
        if (news.Length <= size)
            return news;
        if ((news.Length - 1) > size)
        {
            news = news.Substring(0, (news.IndexOf(" ", size) == -1) ? size : news.IndexOf(" ", size));
        }
        if (news.Length > (size + 5))
        {
            news = news.TrimToSize(more, size - 1);
        }
        news = news.Replace("...", "");
        return string.Concat(news, "...", more);
    }

    public static string PagerPageLink(this HtmlHelper helper, int pageNo, System.Web.Routing.RequestContext reqContext, string linkText, PagerHelper pager)
    {
        if (pager.IsForSearchPage)
        {
            return PagerPageLinkForSearch(helper, pageNo,linkText, reqContext, pager);
        }
        UrlHelper urlHelper = new UrlHelper(reqContext);
        string url = "#";
        if (!pager.AjaxEnabled)
            url = urlHelper.Content(string.Format("~/{0}", A24URLHelper.GetURLForPageNumber(pageNo, reqContext)));
        return string.Format("<a href=\"{0}\" {2} rel='{3}'>{1}</a>", url , linkText, pager.GetAjaxRelatedAttributes(), pageNo);
    }
    public static string PagerPageLink(this HtmlHelper helper, int pageNo, System.Web.Routing.RequestContext reqContext, PagerHelper pager)
    {

        if (pager.IsForSearchPage)
        {
            return PagerPageLinkForSearch(helper,pageNo, pageNo.ToString(), reqContext, pager);
        }
        UrlHelper urlHelper = new UrlHelper(reqContext);
        
        string url = "#";
        if (!pager.AjaxEnabled)
        {
            url = urlHelper.Content(string.Format("~/{0}", A24URLHelper.GetURLForPageNumber(pageNo, reqContext)));
        }
        return  string.Format("<a href=\"{0}\" {2} rel='{3}'>{1}</a>", url, pageNo, pager.GetAjaxRelatedAttributes(), pageNo);
    }


    public static string PagerPageLinkForSearch(this HtmlHelper helper, int pageNo, string linkText, System.Web.Routing.RequestContext reqContext, PagerHelper pager)
    {

        UrlHelper urlHelper = new UrlHelper(reqContext);

        string url = urlHelper.Content(string.Format("~/{0}", A24URLHelper.GetURLForPageNumber(pageNo, reqContext)));

        string refUrl = null;
        if (HttpContext.Current.Request.HttpMethod == @"GET")
        {
            refUrl = HttpContext.Current.Request.RawUrl;
        }
        else
        {
            if (HttpContext.Current.Request.UrlReferrer != null)
                refUrl = HttpContext.Current.Request.UrlReferrer.ToString();
            if (refUrl == null) refUrl = HttpContext.Current.Request.RawUrl;
        }

        string refUrlParams = refUrl.Substring(refUrl.IndexOf('?') + 1);
        var refQryStrings = HttpUtility.ParseQueryString(refUrlParams);
        var refKeyword = HttpContext.Current.Server.UrlEncode(refQryStrings[@"Keyword"]);

        string keywordTxt = string.IsNullOrEmpty(refKeyword) ? "" : string.Format("?keyword={0}", refKeyword);

        url = url + keywordTxt;

        return string.Format("<a href=\"{0}\" rel='{2}'>{1}</a>", url, linkText, pageNo);
    }

    public static bool ContianPartInKey(this Dictionary<string, string> dic, string key)
    {
        var k = dic.Keys.Where(m => m.Contains(key)).FirstOrDefault();
            //(from c in dic.Keys where c.Contains(key) == true select c).FirstOrDefault();
        return(k == null);
    }

    /// <summary>
    /// This is for creating multiple attributes in one line.
    /// </summary>
    /// <param name="value">tThe value should look like "class:my-style|readonly:readonly|onclick:doSomething()"</param>
    /// <returns></returns>
    public static Dictionary<string, object> GetHtmlAttribute(this ViewUserControl control, string value)
    {
        string[] tsrs = value.Split("|".ToCharArray());
        Dictionary<string, object> dics = new Dictionary<string, object>();
        foreach (string s in tsrs)
        {
            string[] ss = s.Split(":".ToCharArray());
            dics.Add(ss[0], ss[1]);
        }
        return dics;
    }

    /// <summary>
    /// This is for creating multiple attributes in one line.
    /// </summary>
    /// <param name="value">tThe value should look like "class:my-style|readonly:readonly|onclick:doSomething()"</param>
    /// <returns></returns>
    public static Dictionary<string, object> GetHtmlAttribute(this ViewPage page, string value)
    {
        string[] tsrs = value.Split("|".ToCharArray());
        Dictionary<string, object> dics = new Dictionary<string, object>();
        foreach (string s in tsrs)
        {
            string[] ss = s.Split(":".ToCharArray());
            dics.Add(ss[0], ss[1]);
        }
        return dics;
    }   


    public static string Concate(this string strValue, params object[] arg0)
    {
        return string.Format(strValue, arg0);
    }

    public static int ToInt(this string value)
    {
        value = value ?? "0"; //If value is Null then convert it to Zero.
        if (value.Trim().Length == 0)
            return 0;
        return int.Parse(value);
    }


    public static string HtmlEncode(this string str)
    {
        return HttpContext.Current.Server.HtmlEncode(str);
    }

    public static string HtmlDecode(this string str)
    {
        return HttpContext.Current.Server.HtmlDecode(str);
    }
    public static string GetWeekName(this DateTime dt)
    {
        string retVal ="";

        if (dt.DayOfWeek == DayOfWeek.Sunday)
        {
            retVal = "أحد";
        } 
        else if (dt.DayOfWeek == DayOfWeek.Monday)
        {
            retVal = "اثنين";
        }
        else if (dt.DayOfWeek == DayOfWeek.Tuesday)
        {
            retVal = "ثلاثاء";
        }
        else if (dt.DayOfWeek == DayOfWeek.Wednesday)
        {
            retVal = "اربعاء";
        }
        else if (dt.DayOfWeek == DayOfWeek.Thursday)
        {
            retVal = "خميس";
        }
        else if (dt.DayOfWeek == DayOfWeek.Friday)
        {
            retVal = "جمعة";
        }
        else if (dt.DayOfWeek == DayOfWeek.Saturday)
        {
            retVal = "سبت";
        }
        return retVal;
    }
}

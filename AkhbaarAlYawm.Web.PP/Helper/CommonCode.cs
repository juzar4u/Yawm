using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Collections.Generic;

public static class CommonCode
{
    public static int maxLengthContent = 128;

    public static string GetImageUrl(string url)
    {
        string retVal = string.IsNullOrWhiteSpace(url) ? "/Content/Images/no-img-bg.gif" : url;
        //TODO : after goes no producion. no use for this line
        retVal = retVal.Replace("content.argaamnews.com.s3.amazonaws.com", "cdn.akhbaar24.com");
        return retVal;
    }

    public static Boolean IsTablet(string strUserAgent)
    {
        var ua = strUserAgent.ToLower();
        return ua.Contains("ipad");
    }
    public static int GetViewCountConvertByRule(int number)
    {
        // View Count business rule
        return number * 3;

    }

    public static string GetDataIconByName(string category)
    {
        string retVal = "a";
        if (category.Trim().Equals("السوق السعودي"))
            retVal = "saudi-flag";
        else if (category.Trim().Equals("الرئيسية"))
            retVal = "m-samsung";
        else if (category.Trim().Equals("منتجات أبل"))
            retVal = "m-apple";
        else if (category.Trim().Equals("فيديوهات"))
            retVal = "m-videos";
        else if (category.Trim().Equals("منتجات سامسونج"))
            retVal = "m-samsung";
        else if (category.Trim().Equals("شركات"))
            retVal = "m-company";
        else if (category.Trim().Equals("جديد التطبيقات"))
            retVal = "m-apps";
        else if (category.Trim().Equals("انفوجراف"))
            retVal = "m-market";
        else if (category.Trim().Equals("فيديو"))
            retVal = "n";
        else if (category.Trim().Equals("خدع وحيل"))
            retVal = "m-tricks";
        else if (category.Trim().Equals("خروج"))
            retVal = "m";


        return retVal;
    }

    public static string GetNumberInKs(int number)
    {

        number = GetViewCountConvertByRule(number);

        string retVal = string.Format("{0:#,###0}", number);


        //decimal retNumber;
        //if (number > 999)
        //{
        //    retNumber = decimal.Divide(number,1000);
        //    retNumber = Math.Round(retNumber, 1);
        //    retVal = retNumber.ToString() + "k";
        //}
        return retVal;
    }


    public static string removeImgFromHTML(string html)
    {
        return Regex.Replace(html, @"<img\s[^>]*>(?:\s*?</img>)?", "", RegexOptions.IgnoreCase);
    }


 

    public static string GetOutsideAppUrl(string url)
    {
        string retVal = url;
        url = url.ToLower().Trim();
        if (!(url.StartsWith("http://") || url.StartsWith("https://")))
        {
            retVal = string.Concat("http://", url);
        }
        return retVal;

    }

    

  


    
    public static string injectClassToImg(string body)
    {
        int imgIndex = body.IndexOf("<img", 0);
        if (imgIndex > 0)
        {
            body = body.Insert(imgIndex + 4, " class=\"article-main-img\" ");
        }
        return body;
    }


    
    public static string FixTedryS3Name(string imgUrl)
    {
        imgUrl = imgUrl.Replace("content.edanat.com/", "content.edanat.com.s3.amazonaws.com/");
        imgUrl = imgUrl.Replace("content.tedry.com/", "content.tedry.com.s3.amazonaws.com/");
        return imgUrl; 
    }
    public static string GetArabicDate(DateTime dt)
    {
        return dt.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("ar-AE"));
    }

    public static string GetArabicDate(DateTime dt, string format)
    {
        return dt.ToString(format, System.Globalization.CultureInfo.CreateSpecificCulture("ar-AE"));
    }

    public static string GetGravatarUrl(string email) 
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return "http://www.gravatar.com/avatar/?d=identicon"; // default img
        }
        string hash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(email.Trim().ToLower(), "MD5"); 
        hash = hash.Trim().ToLower();
        return string.Format("http://0.gravatar.com/avatar/{0}?s=32&d=http%3A%2F%2F0.gravatar.com%2Favatar%2Fad516503a11cd5ca435acc9bb6523536%3Fs%3D32&r=G", hash); 


        //TODO:Include a default image. Querystring parameter example: &default=http%3A%2F%2Fwww.example.com%2Fsomeimage.jpg 
        //http://www.gravatar.com/avatar.php?gravatar_id=ccf3b8c638f15d005e5d070aeb1a3923&rating=G&size=80
        //http://0.gravatar.com/avatar/c8b40fb6d2c53be47b4058cecc4e85d4?s=32&d=http%3A%2F%2F0.gravatar.com%2Favatar%2Fad516503a11cd5ca435acc9bb6523536%3Fs%3D32&r=G
        //return string.Format("http://www.gravatar.com/avatar.php?gravatar_id={0}&rating=G&size=60", hash); 

    }


   
    private static int getRandNumber()
    {
        Random random = new Random((int)(DateTime.Now.Ticks % 1000000));
        int MyRandomNumber = random.Next(1, 1000000);
        return MyRandomNumber;
    }

    public static string FullyQualifiedApplicationPath
    {
        get
        {
            //Return variable declaration 
            string appPath = null;

            //Getting the current context of HTTP request 
            var context = HttpContext.Current;

            //Checking the current context content 
            if (context != null)
            {
                //Formatting the fully qualified website url/name 
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        context.Request.Url.Port == 80
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }
    }


    //this will make sure ,except local host , url will not have port

    public static string FullUrlA24
    {
        get
        {
            //Return variable declaration 
            string appPath = null;

            //Getting the current context of HTTP request 
            var context = HttpContext.Current;

            //Checking the current context content 
            if (context != null)
            {

                //Formatting the fully qualified website url/name 
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        (context.Request.Url.Port == 80 || context.Request.Url.Host != "localhost")
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }
    }

    public static string FullUrlA24AppRoot
    {
        get
        {
            //Return variable declaration 
            string appPath = null;

            //Getting the current context of HTTP request 
            var context = HttpContext.Current;

            //Checking the current context content 
            if (context != null)
            {

                //Formatting the fully qualified website url/name 
                appPath = string.Format("{0}://{1}{2}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        (context.Request.Url.Port == 80 || context.Request.Url.Host != "localhost")
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port);
            }

            return appPath.TrimEnd('/').ToLower(); 
        }
    }



    public static void Url_Redirect_404()
    {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.StatusCode = 404;
        HttpContext.Current.Response.Status = "404 Not Found";
        HttpContext.Current.Response.AddHeader("Location", "");
    }

    public static void Url_Redirect_301(string loc)
    {
        if ((HttpContext.Current.Request.Browser.Browser.ToLower().Equals("ie")) && (HttpContext.Current.Request.Browser.MajorVersion == 6))
        {
            HttpContext.Current.Response.RedirectPermanent(loc, true);
        }
        else
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.StatusCode = 301;
            HttpContext.Current.Response.Status = "301 Moved Permenantly";
            HttpContext.Current.Response.AddHeader("Location", loc);
        }
    }

  
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace AkhbaarAlYawm.Helper
{
    public class Constants
    {
        public static string Akhbaar_CP_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["Akhbaar_CP_URL"];
            }
        }

        public static string BitlyLogin
        {
            get
            {
                return ConfigurationManager.AppSettings["BitlyLogin"];
            }
        }

        public static string BitlyAPIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["BitlyAPIKey"];
            }
        }

        public static string Akhbaar_PP_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["Akhbaar_PP_URL"];
            }
        }

        public static string GetBitlyURL(int articleId)
        {
            string originalURL = string.Format("{0}/Home/Detail/{1}", Akhbaar_PP_URL, articleId);
            return (GenerateBitlyURL(originalURL));
        }

        public static string GetBitlyURLForPhotoAlbum(int albumID)
        {
            string originalURL = string.Format("{0}/photo/Detail/?AlbumID={1}", Akhbaar_PP_URL, albumID);
            return (GenerateBitlyURL(originalURL));
        }



        public static string GenerateBitlyURL(string originalURL)
        {
            string newURL = string.Empty;
            WebResponse response = null;
            StreamReader reader = null;

            try
            {
                string bitlyLogin = Constants.BitlyLogin;
                string bitlyAPIKey = Constants.BitlyAPIKey;

                string url = string.Format("https://api-ssl.bitly.com/v3/shorten?login={0}&apiKey={1}&longUrl={2}&format=txt", bitlyLogin, bitlyAPIKey,
                originalURL);

                WebRequest request = WebRequest.Create(url);
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                newURL = reader.ReadToEnd();

            }
            catch
            {

                //newURL = originalURL;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (reader != null)
                {
                    reader.Close();
                }
            }

            return newURL.Trim();
        }

    }
}
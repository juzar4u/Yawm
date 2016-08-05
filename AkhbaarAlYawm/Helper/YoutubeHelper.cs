using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AkhbaarAlYawm.Helper
{
    public class YoutubeHelper
    {
        public static string getYoutubeVideoID(string youtubeLink)
        {
            Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);

            string videoID = "";

            Match youtubeMatch = YoutubeVideoRegex.Match(youtubeLink);

            string id = string.Empty;

            if (youtubeMatch.Success)
            {
                videoID = youtubeMatch.Groups[1].Value;
            }
            return videoID;
        }
    }
}
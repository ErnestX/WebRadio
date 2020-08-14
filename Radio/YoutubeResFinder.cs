using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Radio
{
    public static class YoutubeResFinder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static Uri FindVideoResByYoutubeLink(Uri url)
        {
            string videoID = HttpUtility.ParseQueryString(url.Query).Get("v");
            string videoInfoUrl = "https://www.youtube.com/get_video_info?video_id=" + videoID;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoInfoUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string videoInfo = HttpUtility.HtmlDecode(reader.ReadToEnd());
            NameValueCollection videoParams = HttpUtility.ParseQueryString(videoInfo);

            Logger.Info("videoParams num of pairs: {0}", videoParams.Count);

            int i = 0; 
            foreach (string key in videoParams.AllKeys)
            {
                string[] values = videoParams.GetValues(key);
                Logger.Info("index: {0}, key: {1}, value: {2}", i, key, values.Length <= 1? values[0] : "multiple values");
                i++;
            }

            if (videoParams["url_encoded_fmt_stream_map"] != null)
            {
                string[] videoURLs = videoParams["url_encoded_fmt_stream_map"].Split(',');

                Logger.Info("printing videoURLs");
                foreach (string s in videoURLs)
                {
                    Logger.Info("{0}", s);
                }
            } 
            else
            {
                Logger.Info("videoURLs is null");
            }

            return null;
        }
    }
}

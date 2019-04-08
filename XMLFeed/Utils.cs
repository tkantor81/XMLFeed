using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace XMLFeed
{
    public class Utils
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string HttpGet(string uri, string credentials)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.PreAuthenticate = true;
            string auth = Base64Encode(credentials);
            request.Headers.Add("Authorization", "Basic " + auth);
            request.Timeout = 300000;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

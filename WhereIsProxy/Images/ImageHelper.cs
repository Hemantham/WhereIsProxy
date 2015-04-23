using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WhereIsProxy.Images
{
    public class ImageHelper
    {
        public static  string ConvertImageUrlToBase64(String url)
        {
            var stringBuilder = new StringBuilder();

            var bytes = GetImage(url);

            stringBuilder.Append(Convert.ToBase64String(bytes, 0, bytes.Length));

            return stringBuilder.ToString();
        }

        private static  byte[] GetImage(string url)
        {
            Stream stream = null;

            byte[] buffer;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader binaryreader = new BinaryReader(stream))
                {
                    int length = (int)(response.ContentLength);
                    buffer = binaryreader.ReadBytes(length);
                    binaryreader.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception exp)
            {
                buffer = null;
            }

            return (buffer);
        }
    }
}
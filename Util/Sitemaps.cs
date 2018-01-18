using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using WebTest.Entities;

namespace WebTest.Util
{
    public class Sitemaps
    {
        /// <summary>
        /// Download sitemap from url
        /// </summary>
        public Sitemap FromUrl(string url)
        {
            var xml = new WebClient().DownloadString(url);

            var sitemap = (Sitemap)new XmlSerializer(typeof(Sitemap)).Deserialize(new StringReader(xml));

            return sitemap;
        }
    }
}

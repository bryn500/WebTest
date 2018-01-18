using System;
using System.Xml.Serialization;

namespace WebTest.Entities
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        [XmlElement("url")]
        public SitemapUrl[] Urls;
    }

    public class SitemapUrl
    {
        [XmlElement("loc")]
        public string Loc;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TerribleDev.Blog.Web.Models
{
    [XmlRoot("urlset", Namespace="http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SiteMapRoot
    {
        [XmlElement("url")]
        public List<SiteMapItem> Urls { get; set; }
    }

    public class SiteMapItem
    {
        [XmlElement("loc")]
        public string Location { get; set; }
        [XmlElement("lastmod")]
        public DateTime LastModified { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TerribleDev.Blog.Web.Models
{
    [XmlRoot("urlset")]
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

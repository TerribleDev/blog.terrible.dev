using System.Collections.Generic;

namespace TerribleDev.Blog.Web.Models
{
    public class HomeViewModel
    {
        public IEnumerable<IPost> Posts { get; set;}
        public int Page { get; set; }
        public string NextUrl { get; set; }
        public string PreviousUrl { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }

        public Schema.NET.Blog BlogLD { get; set; }
        public Schema.NET.WebSite SiteLD { get; set; }
        public string BlogLDString { get; set; }
        public string SiteLDString { get; set; }
    }
}
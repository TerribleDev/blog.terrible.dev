using System.Collections.Generic;
using Microsoft.SyndicationFeed;

namespace TerribleDev.Blog.Web.Models
{
    public class PostCache
    {
        public IList<IPost> PostsAsLists { get; set;}
        public IDictionary<string, IList<Post>> TagsToPosts { get; set; }
        public IDictionary<string, IPost> UrlToPost { get; set; }
        public IDictionary<string, IPost> CaseInsensitiveUrlToPost { get; set; }
        public IDictionary<int, IList<Post>> PostsByPage { get; set; }
        public IList<SyndicationItem> PostsAsSyndication { get; set; }

        public Schema.NET.Blog BlogLD { get; set; }
        public Schema.NET.WebSite SiteLD { get; set; }
        public string BlogLDString { get; set; }
        public string SiteLDString { get; set; }

        public Dictionary<string, LandingPage> LandingPagesUrl { get; set; } 
        
    }
}
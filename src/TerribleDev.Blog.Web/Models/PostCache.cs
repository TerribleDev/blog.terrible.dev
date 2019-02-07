using System.Collections.Immutable;
using Microsoft.SyndicationFeed;

namespace TerribleDev.Blog.Web.Models
{
    public class PostCache
    {
        public ImmutableList<IPost> PostsAsLists { get; set;}
        public ImmutableSortedDictionary<string, ImmutableList<IPost>> TagsToPosts { get; set; }
        public ImmutableDictionary<string, IPost> UrlToPost { get; set; }
        public  ImmutableDictionary<int, ImmutableList<IPost>> PostsByPage { get; set; }
        public ImmutableList<SyndicationItem> PostsAsSyndication { get; set; }
        
    }
}
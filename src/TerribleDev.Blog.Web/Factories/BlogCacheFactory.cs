using System.Collections.Generic;
using TerribleDev.Blog.Web.Models;
using System.Linq;
using System.Collections.Immutable;
using System.Diagnostics;

namespace TerribleDev.Blog.Web.Factories
{
    public static class BlogCacheFactory
    {
        public static PostCache ProjectPostCache(IEnumerable<IPost> rawPosts)
        {
            var posts = rawPosts.OrderByDescending(a => a.PublishDate).ToImmutableList();
            var tagsToPost = posts.Where(a=>a.tags != null)
            .Aggregate(
             ImmutableDictionary.Create<string, ImmutableList<IPost>>(),
            (accum, item) => {
                foreach(var tag in item.tags.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.ToLower()))
                {
                    accum = accum.TryGetValue(tag, out var list) ? accum.SetItem(tag, list.Add(item)) : accum.Add(tag, ImmutableList.Create(item));
                }
                return accum;
            }).ToImmutableSortedDictionary();
            var urlToPosts = posts.ToImmutableDictionary(a => a.Url);
            var postsByPage = 
            posts.Aggregate(ImmutableDictionary.Create<int, ImmutableList<IPost>>(), (accum, item) =>
            {
                if(!accum.Keys.Any()) 
                {
                    accum = accum.SetItem(1, ImmutableList.Create<IPost>());
                }
                var highestPage = accum.Keys.Any() ? accum.Keys.Max() : 1;
                var current = accum[highestPage];
                if (current.Count >= 10)
                {
                    return accum.Add(highestPage + 1, ImmutableList.Create(item));
                }

                return accum.SetItem(highestPage, current.Add(item));
            }).ToImmutableDictionary();
            var syndicationPosts = posts.Select(i => i.ToSyndicationItem()).ToImmutableList();
            var postsToPosition = posts.Select((item, index) => (item, index)).ToImmutableDictionary(i => i.item, i => i.index);
            return new PostCache()
            {
                PostsAsLists = posts,
                TagsToPosts = tagsToPost,
                UrlToPost = urlToPosts,
                PostsByPage = postsByPage,
                PostsAsSyndication = syndicationPosts

            };
        }
    }
}
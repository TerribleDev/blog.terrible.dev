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
                foreach(var tag in item.tags.Select(i => i.ToLower()))
                {
                    if(accum.TryGetValue(tag, out var list))
                    {
                        accum = accum.SetItem(tag, list.Add(item));
                    }
                    else
                    {
                        accum = accum.Add(tag, ImmutableList.Create<IPost>(item));
                    }
                }
                return accum;
            }).ToImmutableSortedDictionary();
            var urlToPosts = posts.ToImmutableDictionary(a => a.UrlWithoutPath);
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

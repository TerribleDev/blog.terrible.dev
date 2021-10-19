using System.Collections.Generic;
using TerribleDev.Blog.Web.Models;
using System.Linq;
using System.Collections.Immutable;
using System.Diagnostics;
using System;
using Microsoft.SyndicationFeed;

namespace TerribleDev.Blog.Web.Factories
{
    public static class BlogCacheFactory
    {
        
        public static PostCache ProjectPostCache(IEnumerable<IPost> rawPosts)
        {
            var orderedPosts = rawPosts.OrderByDescending(a => a.PublishDate);
            var posts = new List<IPost>(orderedPosts);
            var urlToPosts = new Dictionary<string, IPost>();
            var tagsToPost = new Dictionary<string, IList<IPost>>();
            var postsByPage = new Dictionary<int, IList<IPost>>();
            var syndicationPosts = new List<SyndicationItem>();
            foreach(var post in orderedPosts)
            {
                posts.Add(post);
                urlToPosts.Add(post.UrlWithoutPath, post);
                syndicationPosts.Add(post.ToSyndicationItem());
                foreach(var tag in post.ToNormalizedTagList())
                {
                    if(tagsToPost.TryGetValue(tag, out var list))
                    {
                        list.Add(post);
                    }
                    else
                    {
                        tagsToPost.Add(tag, new List<IPost>() { post });
                    }
                }
                if(postsByPage.Keys.Count < 1)
                {
                    postsByPage.Add(1, new List<IPost>() { post });
                }
                else
                {
                    var highestPageKey = postsByPage.Keys.Max();
                    var highestPage = postsByPage[highestPageKey];
                    if(highestPage.Count < 10)
                    {
                        highestPage.Add(post);
                        
                    }
                    else
                    {
                        postsByPage.Add(highestPageKey + 1, new List<IPost>() { post });
                    }
                }
            }
            
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

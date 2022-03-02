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

            var blogPostsLD = new List<Schema.NET.IBlogPosting>();
            foreach(var post in orderedPosts)
            {
                urlToPosts.Add(post.UrlWithoutPath, post);
                syndicationPosts.Add(post.ToSyndicationItem());
                blogPostsLD.Add(post.Content.Value.JsonLD);
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
            var ld = new Schema.NET.Blog() 
            {
                Name = "TerribleDev.Blog",
                Description = "TerribleDev.Blog",
                Author = new Schema.NET.Person() { Name = "TerribleDev" },
                Image = new Schema.NET.ImageObject() { Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/content/tommyAvatar4.jpg")) },
                Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/" )),
                SameAs = new Schema.NET.OneOrMany<Uri>(new Uri("https://twitter.com/terribledev")),
                BlogPost = new Schema.NET.OneOrMany<Schema.NET.IBlogPosting>(blogPostsLD),
            };
            var website = new Schema.NET.WebSite()
            {
                Name = "TerribleDev.Blog",
                Description = "TerribleDev.Blog",
                Author = new Schema.NET.Person() { Name = "TerribleDev" },
                Image = new Schema.NET.ImageObject() { Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/content/tommyAvatar4.jpg")) },
                Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/" )),
                SameAs = new Schema.NET.OneOrMany<Uri>(new Uri("https://twitter.com/terribledev")),
                PotentialAction = new Schema.NET.OneOrMany<Schema.NET.IAction>(
                    // search action
                    new Schema.NET.SearchAction()
                    {
                        Target = new Schema.NET.EntryPoint()
                        {
                            UrlTemplate = new Schema.NET.OneOrMany<string>(@"https://blog.terrible.dev/search?q={search-term}")
                        },
                        QueryInput = new Schema.NET.Values<string, Schema.NET.PropertyValueSpecification>("required name=search-term")
                    }
                )
            };
            return new PostCache()
            {
                PostsAsLists = posts,
                TagsToPosts = tagsToPost,
                UrlToPost = urlToPosts,
                PostsByPage = postsByPage,
                PostsAsSyndication = syndicationPosts,
                BlogLD = ld,
                SiteLD = website

            };
        }
    }
}

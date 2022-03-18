using System.Collections.Generic;
using TerribleDev.Blog.Web.Models;
using System.Linq;
using System.Collections.Immutable;
using System.Diagnostics;
using System;
using Microsoft.SyndicationFeed;
using Schema.NET;

namespace TerribleDev.Blog.Web.Factories
{
    public static class BlogCacheFactory
    {

        public static PostCache ProjectPostCache(IEnumerable<IPost> rawPosts)
        {
            var orderedPosts = rawPosts.OrderByDescending(a => a.PublishDate);
            var posts = new List<IPost>(orderedPosts);
            var urlToPosts = new Dictionary<string, IPost>();
            var tagsToPost = new Dictionary<string, IList<Post>>();
            var postsByPage = new Dictionary<int, IList<Post>>();
            var syndicationPosts = new List<SyndicationItem>();
            var landingPagesUrl = new Dictionary<string, LandingPage>();
            var blogPostsLD = new List<Schema.NET.IBlogPosting>();
            foreach (var post in orderedPosts)
            {
                
                if (post is Post)
                {
                    var castedPost = post as Post;
                    urlToPosts.Add(post.UrlWithoutPath, castedPost);
                    syndicationPosts.Add(castedPost.ToSyndicationItem());
                    blogPostsLD.Add(post.Content.JsonLD);
                    foreach (var tag in castedPost.ToNormalizedTagList())
                    {
                        if (tagsToPost.TryGetValue(tag, out var list))
                        {
                            list.Add(castedPost);
                        }
                        else
                        {
                            tagsToPost.Add(tag, new List<Post>() { castedPost });
                        }
                    }
                    if (postsByPage.Keys.Count < 1)
                    {
                        postsByPage.Add(1, new List<Post>() { castedPost });
                    }
                    else
                    {
                        var highestPageKey = postsByPage.Keys.Max();
                        var highestPage = postsByPage[highestPageKey];
                        if (highestPage.Count < 10)
                        {
                            highestPage.Add(castedPost);

                        }
                        else
                        {
                            postsByPage.Add(highestPageKey + 1, new List<Post>() { castedPost });
                        }
                    }

                }
                if (post is LandingPage)
                {
                    var castedPost = post as LandingPage;
                    landingPagesUrl.Add(castedPost.UrlWithoutPath, castedPost);
                }
            }
                var ld = new Schema.NET.Blog()
                {
                    Name = "TerribleDev Blog",
                    Description = "The blog of Tommy Parnell",
                    Author = new Schema.NET.Person() { Name = "TerribleDev", Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/about")) },
                    Image = new Schema.NET.ImageObject() { Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/content/tommyAvatar4.jpg")) },
                    Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/about")),
                    SameAs = new Schema.NET.OneOrMany<Uri>(new Uri("https://twitter.com/terribledev")),
                    BlogPost = new Schema.NET.OneOrMany<Schema.NET.IBlogPosting>(blogPostsLD),
                };
            var website = new Schema.NET.WebSite()
            {
                Name = "TerribleDev Blog",
                Description = "The blog of Tommy Parnell",
                Author = new Schema.NET.Person() { Name = "TerribleDev", Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/about")) },
                Image = new Schema.NET.ImageObject() { Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/content/tommyAvatar4.jpg")) },
                Url = new Schema.NET.OneOrMany<Uri>(new Uri("https://blog.terrible.dev/")),
                SameAs = new Schema.NET.OneOrMany<Uri>(new Uri("https://twitter.com/terribledev")),
                PotentialAction = new Schema.NET.OneOrMany<Schema.NET.IAction>(
                    // search action
                    new List<Schema.NET.SearchAction>()
                    {
                        new Schema.NET.SearchAction()
                        {
                            Target = new Schema.NET.EntryPoint()
                            {
                                UrlTemplate = new Schema.NET.OneOrMany<string>(@"https://blog.terrible.dev/search?q={search-term}")
                            },
                            QueryInput = new Schema.NET.Values<string, Schema.NET.PropertyValueSpecification>(
                                new OneOrMany<PropertyValueSpecification>(
                                    new PropertyValueSpecification()
                                    {
                                        ValueName = "search-term",
                                        ValueRequired = true,
                                        ValueMinLength = 1,
                                        ValueMaxLength = 500,
                                    }
                                )
                            )
                        }
                    }
                )
            };
            return new PostCache()
            {
                LandingPagesUrl = landingPagesUrl,
                PostsAsLists = posts,
                TagsToPosts = tagsToPost,
                UrlToPost = urlToPosts,
                PostsByPage = postsByPage,
                PostsAsSyndication = syndicationPosts,
                BlogLD = ld,
                SiteLD = website,
                BlogLDString = ld.ToHtmlEscapedString().Replace("https://schema.org", "https://schema.org/true"),
                SiteLDString = website.ToHtmlEscapedString().Replace("https://schema.org", "https://schema.org/true")

            };
        }
    }
}

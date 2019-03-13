using Microsoft.SyndicationFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web
{
    public static class IPostExtensions
    {
        public static SyndicationItem ToSyndicationItem(this IPost x)
        {
            Uri.TryCreate($"https://blog.terrible.dev/{x.Url}/", UriKind.Absolute, out var url);
            var syn = new SyndicationItem()
            {
                Title = x.Title,
                Description = x.Content.ToString(),
                Id = url.ToString(),
                Published = x.PublishDate
            };
            syn.AddLink(new SyndicationLink(url));
            return syn;
        }
    }
}

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
            return new SyndicationItem()
            {
                Title = x.Title,
                Description = x.Content.ToString(),
                Id = $"https://blog.terribledev.io/{x.Url}",
                Published = x.PublishDate
            };
        }
    }
}

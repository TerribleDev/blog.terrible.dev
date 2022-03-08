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
        public static SyndicationItem ToSyndicationItem(this Post x)
        {
            Uri.TryCreate(x.CanonicalUrl, UriKind.Absolute, out var url);
            var syn = new SyndicationItem()
            {
                Title = x.Title,
                Description = x.Content.Value.Content.Value.ToString(),
                Id = url.ToString(),
                Published = x.PublishDate,
            };
            syn.AddLink(new SyndicationLink(url));
            return syn;
        }
        public static ISet<string> ToNormalizedTagList(this Post x)
        {
            if(x.tags == null)
            {
                return new HashSet<string>();
            }
            return new HashSet<string>(x.tags.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.ToLower()));
        }
    }
}

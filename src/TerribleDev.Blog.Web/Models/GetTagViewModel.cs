using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Schema.NET;

namespace TerribleDev.Blog.Web.Models
{
    public class GetTagViewModel
    {
        public IEnumerable<IPost> Posts { get; set; }
        public string Title { get => $"Tag: {Tag}"; }
        public string Tag { get; set; }
        public string CanonicalUrl { get; set; }

        public string ldJson () 
        {
                    var breadcrumb = new Schema.NET.BreadcrumbList()
                    {
                        ItemListElement = new List<IListItem>() // Required
                        {
                            new ListItem() // Required
                            {
                                Position = 1, // Required
                                Url = new Uri("https://blog.terrible.dev/") // Required
                            },
                            new ListItem()
                            {
                                Position = 2,
                                Name = Tag,
                            },
                        },
                    };
                    return breadcrumb.ToHtmlEscapedString().Replace("https://schema.org", "https://schema.org/true");
        }
    }
}

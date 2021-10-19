using Microsoft.AspNetCore.Html;
using YamlDotNet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Models
{
    public interface IPost
    {
        string CanonicalUrl { get; set; }
        string UrlWithoutPath { get; set; }
        string RelativeUrl { get; set; }
        string Title { get; set; }
        DateTime PublishDate { get; set; }
        IList<string> tags { get; set; }
        Lazy<IPostContent> Content { get; set; }
    }

}

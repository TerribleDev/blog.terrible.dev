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
        string AMPUrl { get; set; }
        string CanonicalUrl { get; set; }
        string UrlWithoutPath { get; set; }
        string RelativeUrl { get; set; }
        string Title { get; set; }
        DateTime PublishDate { get; set; }
        DateTime? UpdatedDate { get; set; }
        IPostContent Content { get; set; }
        bool isLanding { get; set; }
    }

}

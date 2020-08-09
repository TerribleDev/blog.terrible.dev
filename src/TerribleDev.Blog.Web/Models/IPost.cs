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
        HtmlString Summary { get; set; }
        DateTime PublishDate { get; set; }
        HtmlString Content { get; set; }
        string ContentPlain { get; set; }
        string SummaryPlain { get; set; }
        string SummaryPlainShort { get; set; }
        IList<string> tags { get; set; }
        IList<string> Images { get; set;}
    }

}

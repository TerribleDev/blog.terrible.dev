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
        string Url { get; set; }
        string Title { get; set; }
        HtmlString Summary { get; set; }
        DateTimeOffset PublishDate { get; set; }
        HtmlString Content { get; set; }
        IEnumerable<string> tags { get; set; }
    }
}

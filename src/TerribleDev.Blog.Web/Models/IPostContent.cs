using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace TerribleDev.Blog.Web.Models 
{
    public interface IPostContent
    {
        Lazy<HtmlString> Content { get; set; }
        Lazy<HtmlString> Summary { get; set; }
        Lazy<string> ContentPlain { get; set; }
        Lazy<string> SummaryPlain { get; set; }
        Lazy<string> SummaryPlainShort { get; set; }
        Lazy<IList<string>> Images { get; set; }
    }
}

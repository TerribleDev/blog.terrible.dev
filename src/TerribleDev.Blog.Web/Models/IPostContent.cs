using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace TerribleDev.Blog.Web.Models 
{
    public interface IPostContent
    {
        HtmlString Content { get; set; }
        HtmlString Summary { get; set; }
        string ContentPlain { get; set; }
        string SummaryPlain { get; set; }
        string SummaryPlainShort { get; set; }
        IList<string> Images { get; set; }
    }
}
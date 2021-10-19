using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace TerribleDev.Blog.Web.Models
{

    public class PostContent : IPostContent
    {
        public HtmlString Content { get; set; }
        public HtmlString Summary { get; set; }
        public string ContentPlain { get; set; }
        public string SummaryPlain { get; set; }
        public string SummaryPlainShort { get; set; }
        public IList<string> Images { get; set; }
    }
}
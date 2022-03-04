using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Schema.NET;

namespace TerribleDev.Blog.Web.Models
{

    public class PostContent : IPostContent
    {
        public HtmlString AmpContent { get; set; }
        public HtmlString Content { get; set; }
        public HtmlString Summary { get; set; }
        public string ContentPlain { get; set; }
        public string SummaryPlain { get; set; }
        public string SummaryPlainShort { get; set; }
        public IList<string> Images { get; set; }
        public BlogPosting JsonLD { get; set; }
        public string JsonLDString { get; set; }
        public BreadcrumbList JsonLDBreadcrumb { get; set; }
        public string JsonLDBreadcrumbString { get; set; }
    }
}
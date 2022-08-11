using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Schema.NET;

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

        BlogPosting JsonLD { get; set; }

        bool HasCode { get; set; }

        public string JsonLDString { get; set; }
        BreadcrumbList JsonLDBreadcrumb { get; set; }
        string JsonLDBreadcrumbString { get; set; }
        string MarkdownMD5 { get; set; }
    }
}

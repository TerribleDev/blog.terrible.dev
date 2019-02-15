using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TerribleDev.Blog.Web.Models
{
    [DebuggerDisplay("{Title}")]
    public class Post : IPost
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public HtmlString Content { get; set; }
        public HtmlString Summary { get; set; }
        public string ContentPlain { get; set; }
        public string SummaryPlain { get; set; }
        public string SummaryPlainShort { get; set; }
        public IList<string> tags { get; set; }
        public IList<string> Images { get; set;}
        public bool HasCode { get; set; }
    }
}

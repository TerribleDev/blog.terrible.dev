using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace TerribleDev.Blog.Web.Models
{
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
    }
}

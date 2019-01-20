using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace TerribleDev.Blog.Web.Models
{
    public class Post : IPost
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public HtmlString Content { get; set; }
        public HtmlString Summary { get; set; }
        public IEnumerable<string> tags { get; set; }
    }
}

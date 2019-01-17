using Microsoft.AspNetCore.Html;
using YamlDotNet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Models
{
    public class Post : IPost
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public HtmlString Content { get; set; }
        public IEnumerable<string> tags { get; set; }
    }
    public interface IPost
    {
        string Url { get; set; }
        string Title { get; set; }
        DateTimeOffset PublishDate { get; set; }
        HtmlString Content { get; set; }
        IEnumerable<string> tags { get; set; }
    }
    public class PostSettings
    {
        public List<string> tags { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public DateTimeOffset date { get; set; }
        public DateTimeOffset updated { get; set; }
        public string id { get; set; }
        public string thumbnail_image { get; set; }
        public string thumbnailImage { get; set; }
        public string thumbnail_image_position { get; set; }
        public string layout { get; set; }
    }
    public interface IPostSettings
    {
        string id { get; set; }
        List<string> tags { get; set; }
        string title { get; set; }
        string permalink { get; set; }
        string thumbnailImage { get; set; }
        DateTimeOffset date { get; set; }
        DateTimeOffset updated { get; set; }
    }
}

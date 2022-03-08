using System;
using System.Collections.Generic;

namespace TerribleDev.Blog.Web.Models
{
    public interface IPostSettings
    {
        string id { get; set; }
        List<string> tags { get; set; }
        string title { get; set; }
        string permalink { get; set; }
        string thumbnailImage { get; set; }
        DateTimeOffset date { get; set; }
        DateTimeOffset updated { get; set; }
        bool isLanding { get; set; }
    }
}

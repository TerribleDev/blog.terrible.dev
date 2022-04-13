using System;
using System.Collections.Generic;

namespace TerribleDev.Blog.Web.Models
{
    public class PostSettings
    {
        public List<string> tags { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public DateTime date { get; set; }
        public DateTime? updated { get; set; }
        public string id { get; set; }
        public string thumbnail_image { get; set; }
        public string thumbnailImage { get; set; }
        public string thumbnail_image_position { get; set; }
        public string layout { get; set; }

        public bool isLanding { get; set; } = false;

        public bool isAmp { get; set; } = true;
    }
}

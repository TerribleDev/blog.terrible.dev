﻿using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TerribleDev.Blog.Web.Models
{
    [DebuggerDisplay("{Title}")]
    public class Post : IPost
    {
        public string CanonicalUrl { get; set; }
        public string UrlWithoutPath { get; set; }
        public string RelativeUrl { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public IList<string> tags { get; set; }
        public IPostContent Content { get; set; }

        public bool isLanding { get; set; } = false;

        public string ThumbnailImage { get; set; }
    }
}

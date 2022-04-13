using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TerribleDev.Blog.Web.Models
{
    [DebuggerDisplay("{Title}")]
    public class LandingPage : IPost
    {
        public string AMPUrl { get; set; }
        public string CanonicalUrl { get; set; }
        public string UrlWithoutPath { get; set; }
        public string RelativeUrl { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public IPostContent Content { get; set; }

        public bool isLanding { get; set; } = false;
        public bool isAmp { get; set; } = true;
        public string ThumbnailImage { get => "https://www.gravatar.com/avatar/333e3cea32cd17ff2007d131df336061?s=640"; }
    }
}

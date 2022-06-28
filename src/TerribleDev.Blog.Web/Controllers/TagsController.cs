using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Filters;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web.Controllers
{
    [Http2PushFilter]
    public class TagsController : Controller
    {
        private readonly PostCache postCache;

        public TagsController(PostCache postCache)
        {
            this.postCache = postCache;
        }
        [Route("/all-tags")]
        [OutputCache(Duration = 31536000)]
        public IActionResult AllTags()
        {
            return View(postCache.TagsToPosts);
        }
        [Route("/tags/{tagName}")]
        [OutputCache(Duration = 31536000, VaryByParam = "tagName")]
        public IActionResult TagPluralRedirect(string tagName)
        {
            if(string.IsNullOrEmpty(tagName))
            {
                return Redirect($"/404/?from=/tags/emptyString/");
            }
            return Redirect($"/tag/{tagName}/");
        }
        [Route("/tag/{tagName}")]
        [OutputCache(Duration = 31536000, VaryByParam = "tagName")]
        public IActionResult GetTag(string tagName)
        {
            if(!postCache.TagsToPosts.TryGetValue(tagName.ToLower(), out var models))
            {
                return Redirect($"/404/?from=/tag/{tagName}/");
            }
            {
                return View(new Models.GetTagViewModel { Tag = tagName, Posts = models, CanonicalUrl = $"https://blog.terrible.dev/tag/{tagName.ToLower()}/" });
            }
        }
    }
}

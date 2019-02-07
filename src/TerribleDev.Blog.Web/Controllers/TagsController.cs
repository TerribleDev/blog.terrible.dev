using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web.Controllers
{
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
        [Route("/tag/{tagName}")]
        [OutputCache(Duration = 31536000, VaryByParam = "tagName")]
        public IActionResult GetTag(string tagName)
        {
            if(!postCache.TagsToPosts.TryGetValue(tagName, out var models))
            {
                return NotFound();
            }
            {
                return View(new Models.GetTagViewModel { Tag = tagName, Posts = models });
            }
        }
    }
}
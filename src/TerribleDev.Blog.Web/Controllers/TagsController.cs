using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TerribleDev.Blog.Web.Controllers
{
    public class TagsController : Controller
    {
        [Route("/all-tags")]
        public IActionResult AllTags()
        {
            return View(HomeController.tagToPost);
        }
        [Route("/tag/{tagName}")]
        public IActionResult GetTag(string tagName)
        {
            if(!HomeController.tagToPost.TryGetValue(tagName, out var models))
            {
                return NotFound();
            }
            {
                return View(new Models.GetTagViewModel { Tag = tagName, Posts = models });
            }
        }
    }
}
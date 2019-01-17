using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Models;
using System.IO;
using Microsoft.AspNetCore.Html;

namespace TerribleDev.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        static IEnumerable<IPost> postsAsList = new BlogFactory().GetAllPosts();
        static IDictionary<string, IPost> posts = postsAsList.ToDictionary(a=>a.Url);
        [Route("/")]
        public IActionResult Index()
        {
            return View(postsAsList);
        }
        [Route("/theme/{*postName}")]
        public IActionResult Theme(string postName)
        {
            return View(model: postName);
        }
        
        [Route("/{*postUrl}")]
        public async Task<IActionResult> Post(string postUrl)
        {
            if(!posts.TryGetValue(postUrl, out var currentPost))
            {
                return NotFound();
            }
            return View(model: currentPost);
        }
        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

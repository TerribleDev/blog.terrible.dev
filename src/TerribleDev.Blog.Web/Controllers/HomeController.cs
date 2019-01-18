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
        static List<IPost> postsAsList = new BlogFactory().GetAllPosts().OrderByDescending(a=>a.PublishDate).ToList();
        static IDictionary<string, IPost> posts = postsAsList.ToDictionary(a=>a.Url);
        static IDictionary<int, List<IPost>> postsByPage = postsAsList.Aggregate(new Dictionary<int, List<IPost>>() { [1] = new List<IPost>() }, (accum, item) =>
        {
            var highestPage = accum.Keys.Max();
            var current = accum[highestPage].Count;
            if (current >= 10)
            {
                accum[highestPage + 1] = new List<IPost>() { item };
                return accum;
            }
            accum[highestPage].Add(item);
            return accum;
        });

        //static IDictionary<int, IPost> postsByPage = postsAsList.
        [Route("/")]
        [Route("/page/{pageNumber}")]
        public IActionResult Index(int pageNumber = 1)
        {
            if(!postsByPage.TryGetValue(pageNumber, out var result))
            {
                return NotFound();
            }
            return View(result);
        }
        [Route("/theme/{postName?}")]
        public IActionResult Theme(string postName)
        {
            return View(model: postName);
        }
        
        [Route("{postUrl}")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Post(string postUrl)
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

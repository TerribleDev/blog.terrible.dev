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
        public static List<IPost> postsAsList = new BlogFactory().GetAllPosts().OrderByDescending(a=>a.PublishDate).ToList();
        public static Dictionary<string, List<IPost>> tagToPost = postsAsList.Where(a=>a.tags != null)
            .Aggregate(
             new Dictionary<string, List<IPost>>(),
            (accum, item) => {
                foreach(var tag in item.tags)
                {
                    if(accum.TryGetValue(tag, out var list))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        accum[tag] = new List<IPost>() { item };
                    }
                }
                return accum;
            });
        public static IDictionary<string, IPost> posts = postsAsList.ToDictionary(a=>a.Url);
        public static IDictionary<int, List<IPost>> postsByPage = postsAsList.Aggregate(new Dictionary<int, List<IPost>>() { [1] = new List<IPost>() }, (accum, item) =>
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

        [Route("/")]
        [Route("/index.html")]
        [Route("/page/{pageNumber?}" )]
        public IActionResult Index(int pageNumber = 1)
        {
            if(!postsByPage.TryGetValue(pageNumber, out var result))
            {
                return NotFound();
            }
            return View(new HomeViewModel() { Posts = result, Page = pageNumber, HasNext = postsByPage.ContainsKey(pageNumber + 1), HasPrevious = postsByPage.ContainsKey(pageNumber - 1) });
        }
        [Route("/theme/{postName?}")]
        public IActionResult Theme(string postName)
        {
            return View(model: postName);
        }
        [Route("/offline")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Offline()
        {
            return View();
        }
        [Route("/debug")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Debug()
        {
            return View();
        }

        [Route("{postUrl}")]
        [OutputCache(Duration = 31536000, VaryByParam = "postUrl")]
        [ResponseCache(Duration = 180)]
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
        [Route("/404")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FourOhFour()
        {
            return View();
        }
    }
}

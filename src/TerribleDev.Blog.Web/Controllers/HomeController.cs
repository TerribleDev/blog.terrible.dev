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
        public static List<TerribleDev.Blog.Core.Models.Post> postsAsList = TerribleDev.Blog.Core.Factories.BlogFactory.getAllPosts("Posts").OrderByDescending(a => a.PublishDate).ToList();
        public static Dictionary<string, List<TerribleDev.Blog.Core.Models.Post>> tagToPost = postsAsList.Where(a=>a.tags != null)
            .Aggregate(
             new Dictionary<string, List<TerribleDev.Blog.Core.Models.Post>>(),
            (accum, item) => {
                foreach(var tag in item.tags)
                {
                    if(accum.TryGetValue(tag, out var list))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        accum[tag] = new List<TerribleDev.Blog.Core.Models.Post>() { item };
                    }
                }
                return accum;
            });
        public static IDictionary<string, TerribleDev.Blog.Core.Models.Post> posts = postsAsList.ToDictionary(a => a.Url);
        public static IDictionary<int, List<TerribleDev.Blog.Core.Models.Post>> postsByPage = postsAsList.Aggregate(new Dictionary<int, List<TerribleDev.Blog.Core.Models.Post>>() { [1] = new List<TerribleDev.Blog.Core.Models.Post>() }, (accum, item) =>
        {
            var highestPage = accum.Keys.Max();
            var current = accum[highestPage].Count;
            if (current >= 10)
            {
                accum[highestPage + 1] = new List<TerribleDev.Blog.Core.Models.Post>() { item };
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
                return Redirect("/404/");
            }
            return View(new HomeViewModel() { Posts = result, Page = pageNumber, HasNext = postsByPage.ContainsKey(pageNumber + 1), HasPrevious = postsByPage.ContainsKey(pageNumber - 1) });
        }
        [Route("/theme/{postName?}")]
        public IActionResult Theme(string postName)
        {
            return View(model: postName);
        }
        [Route("/offline")]
        [Route("/offline.html")]
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
                return Redirect("/404/");
            }
            return View(model: currentPost);
        }
        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            this.Response.StatusCode = 500;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Route("/404")]
        [Route("{*url}", Order = 999)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FourOhFour()
        {
            this.Response.StatusCode = 404;
            return View();
        }
        [Route("/404.html")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FourOhFourCachePage()
        {
            //make a route so the service worker can cache a 404 page, but get a valid status code
            return View(viewName: nameof(FourOhFour));
        }
    }
}

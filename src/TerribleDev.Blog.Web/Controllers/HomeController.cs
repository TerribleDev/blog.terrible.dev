﻿using System;
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
        private readonly PostCache postCache;

        public HomeController(PostCache postCache)
        {
            this.postCache = postCache;
        }
        
        [Route("/")]
        [Route("/index.html")]
        [Route("/page/{pageNumber?}" )]
        [OutputCache(Duration = 31536000, VaryByParam = "pageNumber")]
        [ResponseCache(Duration = 900)]
        public IActionResult Index(int pageNumber = 1)
        {
            if(!postCache.PostsByPage.TryGetValue(pageNumber, out var result))
            {
                return Redirect("/404/");
            }
            return View(new HomeViewModel() { Posts = result, Page = pageNumber, HasNext = postCache.PostsByPage.ContainsKey(pageNumber + 1), HasPrevious = postCache.PostsByPage.ContainsKey(pageNumber - 1) });
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
        [ResponseCache(Duration = 900)]
        public IActionResult Post(string postUrl)
        {
            if(!postCache.UrlToPost.TryGetValue(postUrl, out var currentPost))
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

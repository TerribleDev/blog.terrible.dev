﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Models;
using System.IO;
using Microsoft.AspNetCore.Html;
using TerribleDev.Blog.Web.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.OutputCaching;

namespace TerribleDev.Blog.Web.Controllers
{
    [Http2PushFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly PostCache postCache;

        public HomeController(PostCache postCache, ILogger<HomeController> logger)
        {
            this.logger = logger;
            this.postCache = postCache;
        }



        [Route("/index.html", Order = 2)]
        [Route("/")]
        [Route("/page/{pageNumber:required:int:min(1)}")]
        [OutputCache(Duration = 31536000, VaryByRouteValueNames = new string[] { "pageNumber" })]
        [ResponseCache(Duration = 900)]
        public IActionResult Index(int pageNumber = 1)
        {
            this.logger.LogWarning("Viewing page", pageNumber);
            if (!postCache.PostsByPage.TryGetValue(pageNumber, out var result))
            {
                return Redirect($"/404/?from=/page/{pageNumber}/");
            }
            return View(new HomeViewModel()
            {
                Posts = result,
                Page = pageNumber,
                HasNext = postCache.PostsByPage.ContainsKey(pageNumber + 1),
                HasPrevious = postCache.PostsByPage.ContainsKey(pageNumber - 1),
                BlogLD = postCache.BlogLD,
                SiteLD = postCache.SiteLD,
                BlogLDString = postCache.BlogLDString,
                SiteLDString = postCache.SiteLDString
            });
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

        [Route("{postUrl}/{amp?}")]
        [OutputCache(Duration = 31536000, VaryByRouteValueNames = new string[] { "postUrl", "amp" })]
        [ResponseCache(Duration = 900)]
        public IActionResult Post(string postUrl, string amp = "")
        {
            if (!String.IsNullOrEmpty(amp) && amp != "amp")
            {
                return Redirect($"/404/?from=/{postUrl}/{amp}/");
            }
            var isAmp = amp == "amp";
            if (isAmp)
            {
                return this.RedirectPermanent($"/{postUrl}");
            }
            // case sensitive lookup
            if (postCache.UrlToPost.TryGetValue(postUrl, out var currentPost))
            {
                return View("Post", model: new PostViewModel() { Post = currentPost });
            }
            // case insensitive lookup on post
            if (postCache.CaseInsensitiveUrlToPost.TryGetValue(postUrl, out var caseInsensitivePost))
            {
                return View("Post", model: new PostViewModel() { Post = caseInsensitivePost });
            }
            if (postCache.LandingPagesUrl.TryGetValue(postUrl, out var landingPage))
            {
                return View("Post", model: new PostViewModel() { Post = landingPage });
            }

            this.StatusCode(404);
            return View(nameof(FourOhFour));
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
        public IActionResult FourOhFour(string from = null)
        {
            this.Response.StatusCode = 404;
            return View(viewName: nameof(FourOhFour));
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

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly BlogConfiguration configuration;
        private readonly PostCache postCache;

        public SearchController(BlogConfiguration configuration, PostCache postCache)
        {
            this.configuration = configuration;
            this.postCache = postCache;
        }
        [Route("/search")]
        public IActionResult Index([Required] [MinLength(1)] [MaxLength(500)] string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return Redirect("/404/?from=/search/");
            }
            if(!ModelState.IsValid) 
            {
                return Redirect("/404/?from=/search/");
            }
            var queries = q.Split(" ");
            var posts = postCache
            .PostsAsLists
            .Where(p => 
            queries.Any(query => p.Title.Contains(query, System.StringComparison.InvariantCultureIgnoreCase) )
            || queries.Any(query =>  p.Content.Value.ContentPlain.Contains(query, System.StringComparison.InvariantCultureIgnoreCase))).ToList();
            return View(new SearchViewModel { SearchTerm = q, Posts = posts });
        }
    }
}
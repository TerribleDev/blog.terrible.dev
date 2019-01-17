using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Models;
using System.IO;

namespace TerribleDev.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/theme/{*postName}")]
        public IActionResult Theme(string postName)
        {
            return View(model: postName);
        }
        [Route("/{*postName}")]
        public async Task<IActionResult> Post(string postName)
        {
            var post = await System.IO.File.ReadAllTextAsync(Path.Combine("Posts", "Writing-an-animated-flyout-hamburger-menu.md"));
            var postRendered = Markdig.Markdown.ToHtml(post.Split("---")[1]);
            return View(model: postRendered);
        }
        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web.Controllers
{
    public class SeoController : Controller
    {
        public static DateTimeOffset publishDate = DateTimeOffset.UtcNow; // keep publish date in memory so we just return when the server was kicked 
        public static IEnumerable<SyndicationItem> postsToSyndication = HomeController.postsAsList.Select(a => a.ToSyndicationItem()).ToList();
        [Route("/rss")]
        [Route("/rss.xml")]
        [ResponseCache(Duration = 7200)]
        [OutputCache(Duration = 86400)]
        public async Task Rss()
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/xml";
            using (XmlWriter xmlWriter = XmlWriter.Create(this.Response.Body, new XmlWriterSettings() { Async = true, Indent = false, Encoding = Encoding.UTF8 }))
            {
                var writer = new RssFeedWriter(xmlWriter);
                await writer.WriteTitle("The Ramblings of TerribleDev");
                await writer.WriteValue("link", "https://blog.terribledev.io");
                await writer.WriteDescription("My name is Tommy Parnell. I usually go by TerribleDev on the internets. These are just some of my writings and rants about the software space.");

                foreach (var item in postsToSyndication)
                {
                    await writer.Write(item);
                }


                await xmlWriter.FlushAsync();
            }
        }
        [Route("/sitemap.xml")]
        [ResponseCache(Duration = 7200)]
        [OutputCache(Duration = 86400)]
        public void SiteMap()
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/xml";
            var sitewideLinks = new List<SiteMapItem>(HomeController.tagToPost.Keys.Select(a=> new SiteMapItem() { LastModified = DateTime.UtcNow, Location = $"https://blog.terribledev.io/tag/{a}/"}))
            {
                new SiteMapItem() { LastModified = DateTime.UtcNow, Location="https://blog.terribledev.io/all-tags/" }
            };
            var ser = new XmlSerializer(typeof(SiteMapRoot));
            var sitemap = new SiteMapRoot()
            {
                Urls = HomeController.postsAsList.Select(a => new SiteMapItem() { LastModified = DateTime.UtcNow, Location = $"https://blog.terribledev.io/{a.Url}/" }).ToList()
            };
            sitemap.Urls.AddRange(sitewideLinks);
            ser.Serialize(this.Response.Body, sitemap);
        }
    }
}
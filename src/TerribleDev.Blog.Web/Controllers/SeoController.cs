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
        private readonly BlogConfiguration configuration;
        private readonly PostCache postCache;

        public SeoController(BlogConfiguration configuration, PostCache postCache)
        {
            this.configuration = configuration;
            this.postCache = postCache;
        }
        public static DateTimeOffset publishDate = DateTimeOffset.UtcNow; // keep publish date in memory so we just return when the server was kicked
        [Route("/rss")]
        [Route("/rss.xml")]
        [ResponseCache(Duration = 7200)]
        [OutputCache(Duration = 31536000)]
        public async Task Rss()
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/xml";
            using (XmlWriter xmlWriter = XmlWriter.Create(this.Response.Body, new XmlWriterSettings() { Async = true, Indent = false, Encoding = Encoding.UTF8 }))
            {
                var writer = new RssFeedWriter(xmlWriter);
                await writer.WriteTitle(configuration.Title);
                await writer.WriteValue("link", configuration.Link);
                await writer.WriteDescription("My name is Tommy Parnell. I usually go by TerribleDev on the internets. These are just some of my writings and rants about the software space.");

                foreach (var item in postCache.PostsAsSyndication)
                {
                    await writer.Write(item);
                }


                await xmlWriter.FlushAsync();
            }
        }
        [Route("/sitemap.xml")]
        [ResponseCache(Duration = 7200)]
        [OutputCache(Duration = 31536000)]
        public void SiteMap()
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/xml";
            var sitewideLinks = new List<SiteMapItem>(postCache.TagsToPosts.Keys.Select(a => new SiteMapItem() { LastModified = DateTime.UtcNow, Location = $"https://blog.terrible.dev/tag/{a}/" }))
            {
                new SiteMapItem() { LastModified = DateTime.UtcNow, Location="https://blog.terrible.dev/all-tags/" }
            };
            var ser = new XmlSerializer(typeof(SiteMapRoot));
            var sitemap = new SiteMapRoot()
            {
                Urls = postCache.PostsAsLists.Select(a => new SiteMapItem() { LastModified = DateTime.UtcNow, Location = a.CanonicalUrl }).ToList()
            };
            sitemap.Urls.AddRange(postCache.TagsToPosts.Keys.Select(i => new SiteMapItem() { LastModified = DateTime.UtcNow, Location = $"https://blog.terrible.dev/search?q={i}" }));
            sitemap.Urls.AddRange(sitewideLinks);
            ser.Serialize(this.Response.Body, sitemap);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TerribleDev.Blog.Web.Taghelpers
{
    [HtmlTargetElement("link", Attributes = "rel, href, http-2-push")]
    public class HttpPush : LinkTagHelper
    {
        public bool Http2PushEnabled { get; set; } = true;
        
        public static readonly string Key = "http2push-link";

        public HttpPush(IWebHostEnvironment hostingEnvironment, TagHelperMemoryCacheProvider cacheProvider, IFileVersionProvider fileVersionProvider, HtmlEncoder htmlEncoder, JavaScriptEncoder javaScriptEncoder, IUrlHelperFactory urlHelperFactory) : base(hostingEnvironment, cacheProvider, fileVersionProvider, htmlEncoder, javaScriptEncoder, urlHelperFactory)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(!this.Http2PushEnabled) 
            {
                return;
            }
            var url = base.TryResolveUrl(output.Attributes["href"].Value.ToString(), out string resolvedUrl) ? resolvedUrl : output.Attributes["href"].Value.ToString();
            var linkList = ViewContext.HttpContext.Items.TryGetValue(Key, out var links) ? links as List<string> : null;
            if(linkList == null) 
            {
                linkList = new List<string>() { url };
                ViewContext.HttpContext.Items.Add(HttpPush.Key, linkList);
            }
            else
            {
                linkList.Add(url);
            }
        }
    }
}
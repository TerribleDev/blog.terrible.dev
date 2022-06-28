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
    public record PushUrl(string Url, string asProperty);
    [HtmlTargetElement("link", Attributes = "[rel=stylesheet],href,push")]
    [HtmlTargetElement("img", Attributes = "src,push")]
    public class HttpPush : LinkTagHelper
    {
        [HtmlAttributeNotBound]
        public bool Http2PushEnabled { get; set; } = true;
        
        public static readonly string Key = "http2push-link";

        public HttpPush(IWebHostEnvironment hostingEnvironment, TagHelperMemoryCacheProvider cacheProvider, IFileVersionProvider fileVersionProvider, HtmlEncoder htmlEncoder, JavaScriptEncoder javaScriptEncoder, IUrlHelperFactory urlHelperFactory) : base(hostingEnvironment, cacheProvider, fileVersionProvider, htmlEncoder, javaScriptEncoder, urlHelperFactory)
        {
        }

        private (string Url, string AsProperty) GetTagInfo(string tag) =>
        tag switch {
            "link" => ("href", "link"),
            "img" => ("src", "image"),
            _ => (null, null)
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(!this.Http2PushEnabled) 
            {
                return;
            }
            var (urlAttribute, asProperty) = GetTagInfo(output.TagName);
            // var urlAttribute = context.TagName == "link" ? "href" : "src";
            var url = base.TryResolveUrl(output.Attributes[urlAttribute].Value.ToString(), out string resolvedUrl) ? resolvedUrl : output.Attributes[urlAttribute].Value.ToString();
            var linkList = ViewContext.HttpContext.Items.TryGetValue(Key, out var links) ? links as List<PushUrl> : null;
            
            if(linkList == null) 
            {
                linkList = new List<PushUrl>() { new PushUrl(url, asProperty) };
                ViewContext.HttpContext.Items.Add(HttpPush.Key, linkList);
            }
            else
            {
                linkList.Add(new PushUrl(url, asProperty));
            }
            output.Attributes.Remove(output.Attributes["push"]);
        }
    }
}
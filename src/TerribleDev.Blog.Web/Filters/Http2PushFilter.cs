using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TerribleDev.Blog.Web.Taghelpers;

namespace TerribleDev.Blog.Web.Filters
{
    public class Http2PushFilter : ActionFilterAttribute
    {
        private static bool IsHttp2PushDisabled = String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DISABLE_HTTP2_PUSH"));
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(IsHttp2PushDisabled)
            {
                return;
            }
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<Http2PushFilter>)) as ILogger<Http2PushFilter>;
            logger.LogDebug("Http2PushFilter.OnActionExecuted");
            if(!context.HttpContext.Items.TryGetValue(HttpPush.Key, out var links)) 
            {
                logger.LogDebug("Did not find any links to push");
                return;
            }
            var linkData = links as System.Collections.Generic.List<PushUrl>;
            if(linkData == null || linkData.Count == 0) {
                logger.LogDebug("Http2PushFilter.OnActionExecuted: No links");
                return;
            }
            var headerBuilder = new StringBuilder();
            for(var i = 0; i < linkData.Count; i++) {
                var (url, AsProperty) = linkData[i];
                var resolvedUrl = url.StartsWith("~") ? context.HttpContext.Request.PathBase.ToString() + url.Substring(1) : url;
                headerBuilder.Append($"<{resolvedUrl}>; rel=preload; as={AsProperty}");
                if(i < linkData.Count - 1) {
                    headerBuilder.Append(", ");
                }
            }
            logger.LogDebug("Http2PushFilter.OnActionExecuted: " + headerBuilder.ToString());
            context.HttpContext.Response.Headers.Add("Link", headerBuilder.ToString());
            base.OnActionExecuted(context);
        }
    }
}
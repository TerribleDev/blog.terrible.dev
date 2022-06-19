using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TerribleDev.Blog.Web.Filters
{
    public class StaticETag: ActionFilterAttribute
    {
        public static string staticEtag = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString())).ToHexString().Substring(0,8);
        public static ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var etag = context.HttpContext.Request.Headers["If-None-Match"];
            if(etag == staticEtag)
            {
                Console.WriteLine("ETAG HIT");
                context.Result = new StatusCodeResult(304);
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.HttpContext.Response.StatusCode >= 200 && context.HttpContext.Response.StatusCode < 300 && context.HttpContext.Response.Headers.ETag.Count == 0)
            {
                Console.WriteLine("ETAG MISS");
                context.HttpContext.Response.Headers.Add("ETag", staticEtag);
            }
        }
    }
}
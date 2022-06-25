using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TerribleDev.Blog.Web.Filters
{
    public class StaticETag: ActionFilterAttribute
    {
        public static string staticEtag = "\"" + MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString())).ToHexString().Substring(0,8) + "\"";
        public static ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.OnStarting(() => {
                if(context.HttpContext.Response.StatusCode >= 200 && context.HttpContext.Response.StatusCode < 300 && context.HttpContext.Response.Headers.ETag.Count == 0)
                {
                    context.HttpContext.Response.Headers.Add("ETag", staticEtag);
                }
                return Task.CompletedTask;
            });
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TerribleDev.Blog.Web.Filters
{
    public class StaticETag: ActionFilterAttribute
    {
        static StaticETag() 
        {
            string etagString;
            if(File.Exists("buildtime.txt"))
            {
                Console.WriteLine("buildtime.txt found");
                etagString = File.ReadAllText("buildtime.txt");
            }
            else
            {
                Console.WriteLine("buildtime.txt not found");
                Console.WriteLine("Directory list");
                Console.WriteLine(Directory.GetFiles(".", "*", SearchOption.AllDirectories).Aggregate((a, b) => a + "\n" + b));
                var unixTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                Console.WriteLine("Using Unix Time for Etag: " + unixTime);
                etagString = unixTime;
            }
            StaticETag.staticEtag = "\"" + MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(etagString)).ToHexString().Substring(0,8) + "\"";
        }
        public static string staticEtag;
        public static ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.HttpContext.Response.StatusCode >= 200 && context.HttpContext.Response.StatusCode < 300 && context.HttpContext.Response.Headers.ETag.Count == 0)
            {
                context.HttpContext.Response.Headers.Add("ETag", staticEtag);
            }
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace TerribleDev.Blog.Web.Taghelpers
{
    public abstract class AbstractPlatformTagHelper : TagHelper
    {
        static Regex MobileCheck = new Regex(@"(?:phone|windows\s+phone|ipod|blackberry|(?:android|bb\d+|meego|silk|googlebot) .+? mobile|palm|windows\s+ce|opera\ mini|avantgo|mobilesafari|docomo|ipad)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        static ConcurrentDictionary<string, Platform> CachedChecks = new ConcurrentDictionary<string, Platform>(); // dictionary of user agent -> mobilre
        protected HttpRequest Request => ViewContext.HttpContext.Request;
        protected HttpResponse Response => ViewContext.HttpContext.Response;

        [ViewContext]
        public ViewContext ViewContext { get; set; }
        protected abstract bool ShouldRender();
        public Platform GetPlatform()
        {
            var userAgent = this.Request.Headers.UserAgent;
            if (string.IsNullOrEmpty(userAgent))
            {
                return Platform.Desktop; // mobile is default
            }
            if(CachedChecks.TryGetValue(userAgent, out var cacheResult))
            {
                return cacheResult;
            }
            var isMobile =  AbstractPlatformTagHelper.MobileCheck.IsMatch(this.Request.Headers.UserAgent);
            return isMobile ? Platform.Mobile : Platform.Desktop;
        }
         public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            if(!this.ShouldRender())
            {
                output.SuppressOutput();
                return;
            }
        }
    }
}
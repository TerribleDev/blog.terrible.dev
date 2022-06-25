using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Taghelpers
{
    [HtmlTargetElement("desktopOnly", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class DesktopTagHelper : TagHelper
    {
        static Regex MobileCheck = new Regex(@"(?:phone|windows\s+phone|ipod|blackberry|(?:android|bb\d+|meego|silk|googlebot) .+? mobile|palm|windows\s+ce|opera\ mini|avantgo|mobilesafari|docomo|ipad)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        static ConcurrentDictionary<string, bool> CachedChecks = new ConcurrentDictionary<string, bool>();
        public string UserAgent { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            
            output.TagName = context.TagName;
            if (string.IsNullOrEmpty(UserAgent))
            {
                return;
            }
            var shouldRender = true;
            if(CachedChecks.TryGetValue(UserAgent, out var cacheResult))
            {
                shouldRender = cacheResult;
            }
            else
            {
                var isMobile = MobileCheck.IsMatch(UserAgent);
                shouldRender = !isMobile;
                CachedChecks.TryAdd(UserAgent, !isMobile);
            }
            if(!shouldRender)
            {
                output.SuppressOutput();
            }
        }
    }
}

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
        static Regex MobileCheck = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.ECMAScript | RegexOptions.Compiled);
        static ConcurrentDictionary<string, bool> CachedChecks = new ConcurrentDictionary<string, bool>();
        public string UserAgent { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
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

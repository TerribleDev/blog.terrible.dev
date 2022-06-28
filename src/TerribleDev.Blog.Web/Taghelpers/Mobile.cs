using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Taghelpers
{
    [HtmlTargetElement("mobile", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class MobileTagHelper : AbstractPlatformTagHelper
    {
        protected override bool ShouldRender() => this.GetPlatform() == Platform.Mobile;
    }
}

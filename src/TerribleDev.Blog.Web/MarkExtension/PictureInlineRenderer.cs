using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;
using System.Linq;
using System.Collections.Generic;

namespace TerribleDev.Blog.Web.MarkExtension
{
    // <summary>
    /// A HTML renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.Inlines.LinkInline}" />
    public class PictureInlineRenderer : LinkInlineRenderer
    {
        private void WriteImageTag(HtmlRenderer renderer, LinkInline link, string suffix, string type = null)
        {
            renderer.Write(string.IsNullOrWhiteSpace(type) ? $"<img src=\"" : $"<source type=\"{type}\" srcset=\"");
            var escapeUrl = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;
            renderer.WriteEscapeUrl($"{escapeUrl}{suffix}");
            renderer.Write("\"");
            renderer.WriteAttributes(link);
            // if (renderer.EnableHtmlForInline)
            // {
            //     renderer.Write(" alt=\"");
            // }
            // var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
            // renderer.EnableHtmlForInline = false;
            // renderer.WriteChildren(link);
            // renderer.EnableHtmlForInline = wasEnableHtmlForInline;
            // if (renderer.EnableHtmlForInline)
            // {
            //     renderer.Write("\"");
            // }


            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(" />");
            }
        }
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if (!link.IsImage)
            {
                base.Write(renderer, link);
                return;
            }
            
            
            renderer.Write("<picture>");
            WriteImageTag(renderer, link, ".webp", "image/webp");
            WriteImageTag(renderer, link, string.Empty);
            renderer.Write("</picture>");

        }
    }
}
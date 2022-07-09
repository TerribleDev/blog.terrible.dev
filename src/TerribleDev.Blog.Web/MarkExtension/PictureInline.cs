using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class PictureInline : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
                inlineRenderer.TryWriters.Add(TryLinkInlineRenderer);
            }
        }
        private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
        {
            if (linkInline == null || !linkInline.IsImage)
            {
                return false;
            }
            if(linkInline.Url.EndsWith(".gif"))
            {
                return false;
            }

            renderer.Write("<picture>");
            WriteImageTag(renderer, linkInline, ".webp", "image/webp");
            WriteImageTag(renderer, linkInline, string.Empty);
            renderer.Write("</picture>");
            return true;

        }
        private void WriteImageTag(HtmlRenderer renderer, LinkInline link, string suffix, string type = null)
        {


            renderer.Write(string.IsNullOrWhiteSpace(type) ? $"<img loading=\"lazy\" src=\"" : $"<source type=\"{type}\" srcset=\"");
            var escapeUrl = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

            renderer.WriteEscapeUrl($"{escapeUrl}{suffix}");
            renderer.Write("\"");
            renderer.WriteAttributes(link);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(" alt=\"");
            }
            var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
            renderer.EnableHtmlForInline = false;
            renderer.WriteChildren(link);
            renderer.EnableHtmlForInline = wasEnableHtmlForInline;
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("\"");
            }


            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(" />");
            }
        }
    }
}
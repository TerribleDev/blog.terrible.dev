using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class PictureInline : IMarkdownExtension
    {
        public PictureInline()
        {
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<PictureInlineRenderer>())
            {
                htmlRenderer.ObjectRenderers.ReplaceOrAdd<LinkInlineRenderer>(new PictureInlineRenderer());
            }
        }
    }
}
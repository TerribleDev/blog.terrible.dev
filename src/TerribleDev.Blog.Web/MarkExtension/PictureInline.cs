using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class PictureInline : IMarkdownExtension
    {
        private readonly string postUrl;
        public PictureInline(string postUrl)
        {
            this.postUrl = postUrl;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<PictureInlineRenderer>())
            {
                htmlRenderer.ObjectRenderers.ReplaceOrAdd<LinkInlineRenderer>(new PictureInlineRenderer(postUrl));
            }
        }
    }
}
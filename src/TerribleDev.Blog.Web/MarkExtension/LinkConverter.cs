using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class LinkConverter : IMarkdownExtension
    {
        private readonly Func<string, string> convertLink;

        public LinkConverter(Func<string, string> convertLink) 
        {
            this.convertLink = convertLink;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            
            var htmlRenderer = renderer as HtmlRenderer;
            if(htmlRenderer == null) return;
            var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
            if(inlineRenderer == null) return;
            inlineRenderer.TryWriters.Add((ren, inline) => {
                return false;
                inline.GetDynamicUrl = 
            });
        }
    }
}
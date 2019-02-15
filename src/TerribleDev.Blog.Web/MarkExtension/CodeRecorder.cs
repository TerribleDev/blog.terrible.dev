using Markdig;
using Markdig.Renderers;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class CodeRecorder : IMarkdownExtension
    {
        public CodeRecorder (ref bool hasCode)
        {
            HasCode = hasCode;
        }

        private bool HasCode;

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<Markdig.Syntax.CodeBlock>();
                if (inlineRenderer != null)
                {
                    inlineRenderer.TryWriters.Add(TryLinkInlineRenderer);
                }
            }
        }
    }
}
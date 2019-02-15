using System;
using System.Collections.Generic;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class CodeRecorder : IMarkdownExtension
    {
        public CodeRecorder(ref List<string> codeLanguages)
        {
            CodeLanguages = codeLanguages;
        }

        public List<string> CodeLanguages { get; }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
                if (inlineRenderer != null)
                {
                    inlineRenderer.TryWriters.Add(TryWriter);
                }
            }
        }

        private bool TryWriter(HtmlRenderer renderer, CodeBlock block)
        {
            var fencedBlock = block as FencedCodeBlock;
            if(fencedBlock == null || fencedBlock.Info == null)
            {
                return false;
            }
            CodeLanguages.Add(fencedBlock.Info ?? "");
            return false;
        }
    }
}
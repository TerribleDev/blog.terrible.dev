using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig.Syntax.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Markdig;
    using Markdig.Renderers;
    using Markdig.Renderers.Html;
    using Markdig.Renderers.Html.Inlines;
    using Markdig.Syntax.Inlines;

    namespace TerribleDev.Blog.Web.ExternalLinkParser
    {
        public class ImageRecorder : IMarkdownExtension
        {
            private List<string> images = null;
            public ImageRecorder(List<string> images)
            {
                this.images = images;
            }
            public void Setup(MarkdownPipelineBuilder pipeline)
            {
            }

            public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
            {
                var htmlRenderer = renderer as HtmlRenderer;
                if (htmlRenderer != null)
                {
                    var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
                    if (inlineRenderer != null)
                    {
                        inlineRenderer.TryWriters.Remove(TryLinkInlineRenderer);
                        inlineRenderer.TryWriters.Add(TryLinkInlineRenderer);
                    }
                }
            }

            private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
            {
                if (linkInline.Url == null || !linkInline.IsImage)
                {
                    return false;
                }
                Console.WriteLine(linkInline.Url);
                this.images.Add(linkInline.Url);
                return false;
                // Uri uri;
                // // Only process relative Uri
                // if (!Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out uri) || !uri.IsAbsoluteUri)
                // {
                    
                // }
                
                // return false;
            }
        }
    }
}

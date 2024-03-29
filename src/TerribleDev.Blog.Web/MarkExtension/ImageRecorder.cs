﻿using System;
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
            public ImageRecorder(ref List<string> images)
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
                var url = linkInline.GetDynamicUrl != null ? linkInline.GetDynamicUrl(): linkInline.Url;
                this.images.Add(url);
                return false;
            }
        }
    }
}

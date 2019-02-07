using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class AbsoluteLinkConverter : IMarkdownExtension
    {
        public string BaseUrl { get; }
        public string Domain { get; }

        public AbsoluteLinkConverter(string baseUrl, string domain)
        {
            BaseUrl = baseUrl;
            Domain = domain;
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
                inlineRenderer.TryWriters.Add(TryLinkAbsoluteUrlWriter);
            }
        }
        private bool TryLinkAbsoluteUrlWriter(HtmlRenderer renderer, LinkInline linkInline)
        {
            var prevDynamic = linkInline.GetDynamicUrl;
            linkInline.GetDynamicUrl = () => {
                var escapeUrl = prevDynamic != null ? prevDynamic() ?? linkInline.Url : linkInline.Url;
                if(!System.Uri.TryCreate(escapeUrl, UriKind.RelativeOrAbsolute, out var parsedResult))
                {
                    throw new Exception($"Error making link for {escapeUrl} @ {BaseUrl}");
                }
                if(parsedResult.IsAbsoluteUri)
                {
                    return escapeUrl;
                }
                var uriBuilder = new UriBuilder(Domain);
                if(!escapeUrl.StartsWith("/"))
                {
                    uriBuilder = uriBuilder.WithPathSegment($"/{BaseUrl}/{escapeUrl}");
                }
                else 
                {
                    uriBuilder = uriBuilder.WithPathSegment(parsedResult.ToString());
                }
                return uriBuilder.Uri.ToString();
            };
            
            return false;
        }
    }
}
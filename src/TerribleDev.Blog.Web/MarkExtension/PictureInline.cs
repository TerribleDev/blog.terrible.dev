using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace TerribleDev.Blog.Web.MarkExtension
{
    public class PictureInlineRenderer : HtmlObjectRenderer<LinkInline>
    {
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if(!link.IsImage)
            {
                base.Write(renderer, link);
            }
            
           
        }
    }
}
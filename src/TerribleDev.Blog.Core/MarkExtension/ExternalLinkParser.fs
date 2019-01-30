module TerribleDev.Blog.Core.MarkExtension
open Markdig

let TryLinkInlineRenderer (renderer: Markdig.Renderers.HtmlRenderer, linkInline: Markdig.Syntax.Inlines.LinkInline) = 
        if linkInline.Url <> null
        then false
        else
            let mutable uri: Uri = null;



type ExternalLinkParser () =
    interface IMarkdownExtension with
        member this.Setup(pipeline: MarkdownPipelineBuilder) = ()
        member this.Setup(pipeline, renderer) =
                match renderer with
                | HtmlRenderer ->  
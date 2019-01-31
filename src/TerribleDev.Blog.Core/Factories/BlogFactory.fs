namespace TerribleDev.Blog.Core.Factories
module BlogFactory =

    open System.IO
    open YamlDotNet.Serialization
    open TerribleDev.Blog.Core.Models
    open TerribleDev.Blog.Core
    open Markdig
    open TerribleDev.Blog.MarkdownPlugins
    open Microsoft.AspNetCore.Html
    open System.Linq
    open System.Collections.Generic
    open fs
    open System

    let fixTagName (tag:string) = tag.Replace(' ', '-').WithoutSpecialCharacters().ToLower()
    let mapImgUrlResolver (resolveUrl:string) = 
        fun (imgUrl: string) -> if imgUrl.StartsWith('/') then imgUrl else sprintf "/%s/%s" resolveUrl imgUrl
     

    let getPosts (path) = Directory.EnumerateFiles(path, "*.md", SearchOption.TopDirectoryOnly)
    let parseYml (postText:string) = 
        let split = postText.Split("---")
        match split with
        | [| _ |] -> raise(Exception("No yml found"))
        | [| yml; _|] -> DeserializerBuilder().Build().Deserialize(yml)
        | split when split.Length > 2 -> DeserializerBuilder().Build().Deserialize(split.[0])
        


    let getMarkdownBuilder (imgRef) =
        MarkdownPipelineBuilder()
                                .Use<TargetLinkExtension>()
                                .Use<ImageRecorder>(new ImageRecorder(imgRef))
                                .UseMediaLinks()
                                .UseEmojiAndSmiley()
                                .Build()
    let parsePost (postText:string, fileName:FileInfo, postSettings:PostSettings): Post =
        let mutable images = System.Collections.Generic.List<string>()
        let markdownBuilder = getMarkdownBuilder(images)
        //todo this function is a bit gross
        let markdownText = postText.Split("---") |> Seq.skip 1 |> String.concat ""
        let postContent = Markdown.ToHtml(markdownText, markdownBuilder);
        let postContentPlain =  Markdown.ToPlainText(markdownText, markdownBuilder).Split("<!-- more -->") |> String.concat ""
        //todo pattern match
        let resolvedUrl = if System.String.IsNullOrWhiteSpace(postSettings.permalink) then fileName.Name.Split('.').[0].Replace(' ', '-').WithoutSpecialCharacters() else postSettings.permalink
        let summary = postContent.Split("<!-- more -->").[0];
        let postSummaryPlain = postContentPlain.Split("<!-- more -->").[0];
        let tags = match postSettings.tags with
                    | null -> Seq.empty 
                    | x -> x |> Seq.map fixTagName
        let summaryPlainShort = match postContentPlain with
                                | postContentPlain when postContentPlain.Length <= 147 -> postContentPlain
                                | postContentPlain -> postContentPlain.Substring(0, 146) + "..."
        let mapImgUrlFromResolved = mapImgUrlResolver resolvedUrl
        let images = images |> Seq.distinct |> Seq.map mapImgUrlFromResolved |> Seq.toList
        {
            PublishDate = postSettings.date.ToUniversalTime(); 
            tags = tags |> Seq.toList
            Title = postSettings.title;
            Url = resolvedUrl;
            Content = HtmlString(postContent);
            Summary = HtmlString(summary);
            SummaryPlain = postSummaryPlain;
            SummaryPlainShort = summaryPlainShort;
            ContentPlain = postContentPlain;
            Images = images |> Seq.toList
        }
    let getAllPosts path = 
        getPosts path  
        |> Seq.map getFileInfo
        |> Seq.map(Async.map(fun (text, fileInfo) -> (text, fileInfo, parseYml(text))))
        |> Seq.map(Async.map(parsePost))
        |> Async.Parallel
        // todo stop this
        |> Async.RunSynchronously
        |> System.Collections.Generic.List
        
    
       
    





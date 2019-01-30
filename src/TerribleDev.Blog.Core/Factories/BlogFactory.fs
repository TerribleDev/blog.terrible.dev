module BlogFactory

open System.IO
open YamlDotNet.Serialization
open TerribleDev.Blog.Core.Models
open TerribleDev.Blog.Core
open Markdig
open TerribleDev.Blog.MarkdownPlugins

let fixTagName (tag:string) = tag.Replace(' ', '-').WithoutSpecialCharacters().ToLower()

let getPosts (path) = Directory.EnumerateFiles(path, "*.md", SearchOption.TopDirectoryOnly)
let parseYml (postText:string):PostSettings = 
    postText.Split("---").[0] 
    |>  DeserializerBuilder().Build().Deserialize

let getFileInfo (filePath:string) = 
    let fileInfo = FileInfo(filePath)
    async {
        let! text = File.ReadAllTextAsync(fileInfo.FullName) |> Async.AwaitTask
        return (text, fileInfo)
    }
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
    let markdownText = postText.Split("---") |> Seq.skip 1 |> String.concat ""
    let postContent = Markdown.ToHtml(markdownText, markdownBuilder);
    let postContentPlain =  Markdown.ToPlainText(markdownText, markdownBuilder).Split("<!-- more -->") |> String.concat ""
    //let resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
    let summary = postContent.Split("<!-- more -->")[0];
    let postSummaryPlain = postContentPlain.Split("<!-- more -->")[0];
    {PublishDate = postSettings.date.ToUniversalTime(); postSettings.tags.  }
let getAllPosts path = 
    getPosts path  
    |> Seq.map getFileInfo
    |> Seq.map(Async.map(fun (text, fileInfo) -> (text, fileInfo, parseYml(text))))
    |> Seq.map(Async.map(parsePost))
   
    





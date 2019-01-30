﻿module BlogFactory

open System.IO
open YamlDotNet.Serialization
open TerribleDev.Blog.Core.Models
open TerribleDev.Blog.Core
open Markdig
open TerribleDev.Blog.MarkdownPlugins
open Microsoft.AspNetCore.Html
open System.Linq
open System.Collections.Generic

let fixTagName (tag:string) = tag.Replace(' ', '-').WithoutSpecialCharacters().ToLower()
let mapImgUrl resolveUrl imgUrl = if imgUrl.StartsWith('/') then imgUrl else sprintf "/%s/%s" resolveUrl imgUrl

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
    //todo this function is a bit gross
    let markdownText = postText.Split("---") |> Seq.skip 1 |> String.concat ""
    let postContent = Markdown.ToHtml(markdownText, markdownBuilder);
    let postContentPlain =  Markdown.ToPlainText(markdownText, markdownBuilder).Split("<!-- more -->") |> String.concat ""
    //todo pattern match
    let resolvedUrl = if not(System.String.IsNullOrWhiteSpace(postSettings.permalink)) then postSettings.permalink else ""
    let summary = postContent.Split("<!-- more -->").[0];
    let postSummaryPlain = postContentPlain.Split("<!-- more -->").[0];
    let tags = postSettings.tags |> Seq.map fixTagName |> Seq.toList
    let summaryPlainShort = match postContentPlain with
                            | postContentPlain when postContentPlain.Length <= 147 -> postContentPlain
                            | postContentPlain -> postContentPlain.Substring(0, 146) + "..."
    let mapImgUrlFromResolved = mapImgUrl resolvedUrl
    let images = images |> Seq.distinct |> Seq.map mapImgUrlFromResolved |> Seq.toList
    {
        PublishDate = postSettings.date.ToUniversalTime(); 
        tags = tags; 
        Title = postSettings.title;
        Url = resolvedUrl;
        Content = HtmlString(postContent);
        Summary = HtmlString(summary);
        SummaryPlain = postSummaryPlain;
        SummaryPlainShort = summaryPlainShort;
        ContentPlain = postContentPlain;
        Images = images
    }
let getAllPosts path = 
    getPosts path  
    |> Seq.map getFileInfo
    |> Seq.map(Async.map(fun (text, fileInfo) -> (text, fileInfo, parseYml(text))))
    |> Seq.map(Async.map(parsePost))
   
    





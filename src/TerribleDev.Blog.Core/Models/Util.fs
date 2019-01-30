namespace TerribleDev.Blog.Core.Models
open Microsoft.SyndicationFeed
open System

module Util =
    let ToSyndicationItem (x: Post) = 
        let url = sprintf "https://blog.terribledev.io/%s" x.Url
        let publishDate : DateTimeOffset = (DateTimeOffset) x.PublishDate
        SyndicationItem(Title = x.Title, Description = x.Content.ToString(), Id = url, Published = publishDate) 
namespace TerribleDev.Blog.Core.Models
open System;
open System.Collections.Generic

[<CLIMutable>]
type PostSettings =
    {
        tags: List<string>;
        title: string
        permalink: string 
        date: DateTime
        updated: DateTime
        id:string
        thumbnail_image:string
        thumbnailImage:string
        thumbnail_image_position:string
        layout:string
    }

namespace TerribleDev.Blog.Core.Models
open System;
open Microsoft.AspNetCore.Html;

type Post =
    {
        Url: string;
        Title: string;
        PublishDate: DateTime;
        Content: HtmlString;
        Summary: HtmlString; 
        ContentPlain: string; 
        SummaryPlain: string; 
        SummaryPlainShort: string; 
        tags : List<string>
        Images: List<string>
    }
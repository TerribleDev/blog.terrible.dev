namespace TerribleDev.Blog.Core.Models
open System;
open Microsoft.AspNetCore.Html;
open System.Collections.Generic

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
        tags : IList<string>
        Images: IList<string>
    }
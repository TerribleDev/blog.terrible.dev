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
        tags : System.Collections.Generic.List<string>
        Images: System.Collections.Generic.List<string>
    }
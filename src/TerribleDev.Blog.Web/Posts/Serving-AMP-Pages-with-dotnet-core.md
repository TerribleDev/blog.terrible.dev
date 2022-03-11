title: Serving AMP Pages with Dotnet Core
date: 2022-03-10 06:00
tags:
- dotnet
- dotnetcore
- AMP
---

I remember when (Accelerated Mobile Pages) first came out, and it was very restrictive and weird. I think this ultimately hurt the *AMP Brand* Beyond this, several companies have built AMP experiences which haven't always been the best experience. I do however think AMP pages always load extremely fast. A lot of that is just the constraints of AMP. Last night I put my blog posts on AMP for a laugh, and it was much easier than I thought it would be.

<!-- more -->

## Step 0

Download the [AMP chrome extension](https://chrome.google.com/webstore/detail/amp-validator/nmoffdblmcmgeicmolmhobpoocbbmknc?hl=en) and read what your violations are on an existing page you want to serve as an amp page.



## AMP Requirements

So these days AMP is a webpage with several restrictions.

* No JavaScript, or well very restrictive JS.
  * JS is possible, but not without work. For the sake of this tutorial I decided to skip the JS.
* Inlined only css
* No `picture` tags
* A few other tags you need for AMP.


## Razor

First things first, we need to figure out how we will adjust our layout for AMP. The easiest way for a layout to get a variable either from any controller or any razor page is using the `ViewData` dictionary. I added the following at the top of  my layout page. This lets me read if we are in an amp page.

```csharp
@{
    var amp = ViewData["amp"] as bool? ?? false;
    var htmlTag = amp ? "amp" : "";
}
```

Ok, so lets dive into the required HTML markup. AMP pages require a...
 
* `<html>` tag with an `amp` attribute.
* a `<head>` tag with an `<amp-boilerplate>` tag that contains some boilerplate CSS.
* The amp JS runtime
* `<link>` tags to point the non-amp page at the amp page.
  

HTML tag is an easy start. The code block above has an `htmlTag` attribute that is used for the tag.

```cshtml
<html lang="en"  @htmlTag>
```

The head tag containing the boilerplate CSS is easy. Note that the boilerplate has `@` signs for CSS which need to be `@@` in razor, to escape the `@` sign.

```cshtml
@if(amp) 
{
    <style amp-boilerplate>body{-webkit-animation:-amp-start 8s steps(1,end) 0s 1 normal both;-moz-animation:-amp-start 8s steps(1,end) 0s 1 normal both;-ms-animation:-amp-start 8s steps(1,end) 0s 1 normal both;animation:-amp-start 8s steps(1,end) 0s 1 normal both}@@-webkit-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@-moz-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@-ms-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@-o-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}</style><noscript><style amp-boilerplate>body{-webkit-animation:none;-moz-animation:none;-ms-animation:none;animation:none}</style></noscript>
}
```

Finally, the JS runtime. This needs to also go in the head tag. You can include this with the boilerplate code.

```cshtml
@if(amp) 
{
    <style amp-boilerplate>body{-webkit-animation:-amp-start 8s steps(1,end) 0s 1 normal both;-moz-animation:-amp-start 8s steps(1,end) 0s 1 normal both;-ms-animation:-amp-start 8s steps(1,end) 0s 1 normal both;animation:-amp-start 8s steps(1,end) 0s 1 normal both}@@-webkit-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@-moz-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@-ms-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@-o-keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}@@keyframes -amp-start{from{visibility:hidden}to{visibility:visible}}</style><noscript><style amp-boilerplate>body{-webkit-animation:none;-moz-animation:none;-ms-animation:none;animation:none}</style></noscript>
    
    <script async src="https://cdn.ampproject.org/v0.js"></script>
}
```

### Inline CSS


AMP Pages must have inlined CSS. To accomplish this I wrote this tag helper which loads a comma separated list of files into memory and then inlines them. The `<link>` tag your CSS needs to be in has to have the `amp-custom` attribute.

```csharp
[HtmlTargetElement("inline-style")]
public class InlineStyleTagHelper : TagHelper
{
    [HtmlAttributeName("href")]
    public string Href { get; set; }

    private IWebHostEnvironment HostingEnvironment { get; }
    private IMemoryCache Cache { get; }



    public InlineStyleTagHelper(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
    {
        HostingEnvironment = hostingEnvironment;
        Cache = cache;
    }


    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var paths = Href.Split(',');

        // Get the value from the cache, or compute the value and add it to the cache
        var fileContent = await Cache.GetOrCreateAsync("InlineStyleTagHelper-" + paths, async entry =>
        {
            var fileProvider = HostingEnvironment.WebRootFileProvider;
            var result = paths.Select(async path => {
                if(HostingEnvironment.IsDevelopment())
                {
                    var changeToken = fileProvider.Watch(path);
                    entry.AddExpirationToken(changeToken);
                }

                entry.SetPriority(CacheItemPriority.NeverRemove);

                var file = fileProvider.GetFileInfo(path);
                if (file == null || !file.Exists)
                    return null;

                return await ReadFileContent(file);
            });
            var allFinished = await Task.WhenAll(result);
            return string.Join("\n", allFinished);
        });

        if (fileContent == null)
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = "style";
        output.Attributes.RemoveAll("href");
        output.Content.AppendHtml(fileContent);
    }

    private static async Task<string> ReadFileContent(IFileInfo file)
    {
        using (var stream = file.CreateReadStream())
        using (var textReader = new StreamReader(stream))
        {
            return await textReader.ReadToEndAsync();
        }
    }
}
```

```cshtml
@if(amp) 
{
    <inline-style amp-custom href="css/site.css,css/site.desktop.css,css/site.mobile.css"></inline-style>

}
else 
{
    <link asp-append-version="true" rel="stylesheet" href="~/css/site.css" />
    <link asp-append-version="true" rel="stylesheet" href="~/css/site.mobile.css" />
}
```

### Javascript

AMP, [does allow for using JS](https://amp.dev/documentation/components/amp-script/) in a web worker. This has a lot of caveats, and for my use-case (this blog) it was better to just skip rendering any JS. I guarded the `RenderSection` call for the scripts section behind an `if(amp)` statement.

```cshtml
@if(!amp)
{
    @RenderSection("Scripts", required: false)
    
    <script asp-append-version="true" src="~/your/script.js" async></script>
}
```


### Link tags

On pages that render AMP, you'll need to be able to generate 2 meta tags. The first is a canonical tag that tells google what the canonical URL is of the page. The second is one, to tell google where your amp pages are for a URL. This is data you typically want to pass to the Model of the view you are rendering. Adding these meta to the head of the layout through a section.

```cshtml
@section Head {
    <link rel="canonical" href="@Model.Post.CanonicalUrl" />
    <link rel="amphtml" href="@Model.Post.AMPUrl">
}
```


## Routes

In my implementation I added `/amp` to the end of my URLs for amp. Then in the controller you can set `this.ViewData["amp"] = amp == "amp";` the view data for the page to be an amp page or not. If you would prefer, you can set the boolean with a view model, it would also work very well.


```csharp
        [Route("{postUrl}/{amp?}")]
        public IActionResult Post(string postUrl, string amp = "")
        {
            if(!String.IsNullOrEmpty(amp) && amp != "amp")
            {
                // handle 404s
                return NotFound();
            }
            ViewDictionary["amp"] = amp == "amp";
            return new View(model:  new ViewModel ());
        }
```

## Google Analytics

There is a snippet of code makes GA work in an AMP page. I made the following partial view that I call from the layout page.

```cshtml
@{
    Layout = null;
    var amp = ViewData["amp"] as bool? ?? false;
}

 @if(!amp)
{
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag() { dataLayer.push(arguments); }
        gtag('js', new Date());
        gtag('config', 'GTAG_ID');
        document.addEventListener('DOMContentLoaded', function () {
            var script = document.createElement('script');
            script.src = 'https://www.googletagmanager.com/gtag/js?id=GTAG_ID';
            script.async = true
            document.body.appendChild(script);
        });
    </script>
}
else
{
    <amp-analytics type="gtag" data-credentials="include">
    <script type="application/json">
        {
            "vars" : {
                "gtag_id": "GTAG_ID",
                "config" : {
                "GTAG_ID": { "GTAG_ID": "default"  }
                }
            }
        }
    </script>
    </amp-analytics>
}

```

## So what's next?

Go through your pages and look at the violations in the Chrome Extension. If you push the pages live, and register them in your sitemap. Errors with amp pages will appear in [the Google Search Console.](https://search.google.com/search-console/about) as google indexes your AMP pages.

## I need more help!

You can look at [my implementation](https://github.com/TerribleDev/blog.terrible.dev/commit/83eb1bc565dfb4bdb38d3c5f0cbfbc21b05ad4b2).


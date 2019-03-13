title: 5 web perf tips for 2019
date: 2019-02-23 01:32
tags:
- web
- performance
- javascript
- battle of the bulge
---

As more and more of the world is getting online, a larger part of the internet community is using the internet on lower powered devices. Making websites fast is becoming paramount. Here are 5 tips to improving you web page's performance

<!-- more -->

## Brotli and gzip

So incase you didn't know, when your browser makes a request to the server it sends along a header called `Accept-Encoding` This is a comma separated list of compression types your server can use to compress the data to the user. The common ones in the past have been `gzip, and deflate`. [Broli](https://en.wikipedia.org/wiki/Brotli), is a compression
 algorithm invented by google to be a more efficient for the web. This has about a 35% effectiveness over gzip based on my own testing. This means your content will be almost 1/3rd smaller over the wire. Most browsers [support this already](https://caniuse.com/#feat=brotli). You can use cloudflare to serve Brotli (br) to your users, and most web servers support this today. Make sure your server is serving br, and at minimum gzip.


## Webp, JPEG 2000

Images are among one of the largest types of files on the internet today, and picking the right file type is as important as getting your data structures right. In the past we told everyone to keep photography in `jpeg`, logos and screen shots in `png`. However google has come out with a new file format. One that is massively smaller than either `jpeg` or `png`, and that is `webp`. Webp is only supported on [chrome, edge and firefox](https://caniuse.com/#search=webp), but don't worry for IOS Safari you can use `JPEG 2000`. Sizing images is also a key concern, you can use srcset to size images appropriately, and you can use the picture element to select the right image given browser support.

```html

<picture>
    <source type="image/webp" srcset="3.webp" alt="an image showing the tiny png results">
    <source type="image/jp2" srcset="3.jp2" alt="an image showing the tiny png results">
    <img src="3.png" alt="an image showing the tiny png results">
</picture>

```


## Lighthouse 

Ok so this is less of a trick to implement and more of a tool use use. Man I keep mentioning google, but they keep making amazing web stuff so here we are. Google has made this awesome performance tool called [lighthouse](https://developers.google.com/web/tools/lighthouse/). A version of this tool is built into chrome. Open the developer tools, and click the `audits` tab. That tool is lighthouse. You can install newer versions with `npm install -g lighthouse` or `yarn global add lighthouse`. Then just run `lighthouse --view <url>` so this blog would be `lighthouse --view https://blog.terrible.dev`. You should be hit with a pretty in depth report as to how you can fix and improve your web pages. You can also have your CI system run lighthouse on every build. You can fail PR's if they reduce performance, or just track your accessibility over time.

## HTTP/2

HTTP version 2 is a newer version of the http spec. Supported [by all major browsers](https://caniuse.com/#feat=http2) this protocol offers compression of http headers, a [push feature](https://en.wikipedia.org/wiki/HTTP/2_Server_Push) that lets you push files down to the browser before they are requested, [http pipelining](https://en.wikipedia.org/wiki/HTTP_pipelining), and multiplexing multiple requests over a single TCP connection. You can easily get http2 working if you let [cloudflare](https://www.cloudflare.com/) front your http traffic, but you will still want to implement http2 in your server eventually.


## Service workers

My last and probably favorite feature. [Service Workers](https://developers.google.com/web/fundamentals/primers/service-workers/) are a worker that can stand in between your server and web page in the browser. They are mostly a proxy that let you do things like cache your content, and support offline capabilities. They are easy to implement, you need to have a `manifest.json` file which you can generate from Microsoft's [PWA Builder](https://www.pwabuilder.com/), and just serve traffic over https only. PWA Builder even has [pre-made service workers](https://www.pwabuilder.com/serviceworker) for most scenarios so you don't even need to write your own. I use this for my blog to cache static content, preload blog posts, and provide offline support.
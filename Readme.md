The code for [blog.terrible.dev](https://blog.terrible.dev). My blog powered by a bespoke blog engine, at one time I thought I'd try to make an open source product for dotnet core, but then got lazy. Probably one of the fastest blog site's you will ever see. Prior to this my blog was built with hugo, and thus authoring was made backward compatible with hugo's posts (3+ years ago). I honestly built many versions including an F#, OCAML, and typescript until I just landed on C#. The implementation here was evolved from the F# version but I switched to C# when I realized there was almost no text editor support for razor files in an F# project. At that time as well dotnet was really kicking off on the tech empower benchmarks. This uses an active server, and not a static site build, because HTML can be emitted conditionally on desktop/mobile headers to minimize html size over the wire.

## Technology

Almost everything was made based on page speed, and nothing more. Some of the code, and decisions seem weird until you realize it's all about page speed and nothing else.

- dotnet 6
- Markdown
  - [Markdig](https://github.com/xoofx/markdig) for parsing markdown
  - This also has some extensions to ensure all relative links end in `/` and external links have `rel="noopener"` and `target="_blank"`, also to include a picture tag for webp images
- YML for configuring a blog's posts *front matter* aka configuration
- PWA/Serviceworkers
  - Used to precache pages which could be read offline, and store a 404 page for offline
  - Also used to give an almost SPA like experience (html is swapped from the server, no page reloads occur)
- CSS variables used for theming
- Vanilla JS for handling the hamburger menu.
  - No framework for minimum size



## Authoring

Authoring is done in markdown. Configuration for a post must be at the top of the document and placed before `---` (three dashes). Posts include a `<!-- more -->` tag to indicate where the post should be cut off for the summary.

## Convert images to webp (TODO: make this automatic)

find . -iname '*.png' -exec  cwebp -lossless '{}' -o '{}'.webp \;
find . -iname '*.jpg' -exec  cwebp '{}' -o '{}'.webp \;
find . -iname '*.gif' -exec  gif2webp -mixed '{}' -o '{}'.webp \;


## resize image to max width (TODO: Also make this automatic 🤣)

find . -iname '*' -exec  convert '{}' -resize 750 '{}' \;

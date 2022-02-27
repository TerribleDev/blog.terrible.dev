The code for [blog.terrible.dev](https://blog.terrible.dev). My blog powered by a bespoke blog engine, at one time I thought I'd try to make an open source product for dotnet core. Probably one of the fastest blog site's you will ever see. Prior to this my blog was built with hugo, and thus authoring was made backward compatible with hugo's posts (3+ years ago). 

## Technology

- dotnet 6
- Markdown
  - [Markdig](https://github.com/xoofx/markdig) for parsing markdown
  - This also has some extensions to ensure all relative links end in `/` and external links have `rel="noopener"` and `target="_blank"`
- YML for configuring a blog's posts *front matter* aka configuration

## Convert images to webp (TODO: make this automatic)

find . -iname '*.png' -exec  cwebp -lossless '{}' -o '{}'.webp \;
find . -iname '*.jpg' -exec  cwebp '{}' -o '{}'.webp \;
find . -iname '*.gif' -exec  gif2webp -mixed '{}' -o '{}'.webp \;


## resize image to max width (TODO: Also make this automatic 🤣)

find . -iname '*' -exec  convert '{}' -resize 750 '{}' \;

title: Dynamically changing the site-theme meta tag
date: 2022-04-12 11:05
thumbnailImage: 1.jpg
tags:
- javascript
- js
- react
---

So, incase you are unfamiliar, there is a meta tag called `<meta name="theme-color" content="...">` that is used to change the color of the navbar on desktop safari, mobile safari, and mobile chrome. If you don't set a value these browsers tend to find a color that match the site to the best of their ability. However, sometimes even setting the value can cause the site to look ugly.

<!-- more -->

So, I've been recently working on an NFT project called [Squiggle Squatches](http://squigglesquatches.io/). NFT projects are essentially digital art projects for sale. Our website, really needs to reflect our look and feel as much as we can. When I first loaded our page, I noticed this **huge** white bar on the top of Safari.

![An un-themed page with a big white bar at the top of the page](1.jpg)


> So I set out to change this. I knew there was a  `<meta name="theme-color" content="...">` tag that can add the theme.

I first made the theme be the color of the top section, and this looked great!

![An theme where the top bar color clashes on scroll](2.jpg)

However after scrolling, I noticed this looked super ugly.

![An theme where the top bar color clashes on scroll](3.jpg)

So I decided to write some code to fix this problem.

## Listening to scroll events

So, I started with decorating certain tags with a `data-scroll-theme` attribute that signaled our code to look at this div to manipulate the theme color. This looks like `<section data-scroll-theme class="blue/red/etc">content</section>`

I then ended up crafting this JS code. Basically, make a throttle function so we only fire our event every 100ms. Grab the default color. Then on scroll figure out if any boxes are at the top of the page, and if so set the meta tag to that color.

```js
// a function to only call the wrapped functions every x milliseconds so the scroll event doesn't make our function run all the time
function throttle(func, timeFrame) {
  var lastTime = 0;
  return function(...args) {
    var now = new Date().getTime();
    if (now - lastTime >= timeFrame) {
      func(...args);
      lastTime = now;
    }
  };
}

// get the theme color on load so we can revert to this
const ogColor = document.querySelector('meta[name="theme-color"]')?.getAttribute('content');

// handle scroll event
const handleScroll = throttle(() => {
  // find all tags that have `data-scroll as a property`
  const targets = document.querySelectorAll('[data-scroll-theme]')
  // are any targets at the top of the window?
  const isTop = Array.from(targets).map((target) => {
    const rect = target.getBoundingClientRect();
    if (rect.y > 1) {
      return null;
    }
    return { target, rect }
  }).filter(Boolean).sort((a, b) => b.rect.y - a.rect.y)[0]
  // if we found an element at the top of the document then
  if (isTop) {

    // set theme color meta tag to the background color of div
    const color = window.getComputedStyle(isTop.target).getPropertyValue('background-color')
    if (color) {
      // find the theme color meta tag and set the attribute to it
      document.querySelector('meta[name="theme-color"]')?.setAttribute('content', color);
    }
  } else if (ogColor) {
    // set theme color meta tag to original
    document.querySelector('meta[name="theme-color"]')?.setAttribute('content', ogColor);
  }
  // run every 100ms
}, 100)

document.addEventListener('scroll', handleScroll, { passive: true })

```

## End result

The end result is the top bar of safari changes as you scroll between blocks. This has made [Squiggle Squatches](http://squigglesquatches.io/) look way better on mobile.

<iframe width="662" height="1176" src="https://www.youtube.com/embed/iLksuqZP4L8" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

<!-- ![video](https://youtu.be/iLksuqZP4L8) -->

You can see a simpler example on [replit](https://replit.com/@TerribleDev/ScrollableTheme)



<iframe frameborder="0" width="100%" height="500px" src="https://replit.com/@TerribleDev/ScrollableTheme?embed=true#script.js"></iframe>
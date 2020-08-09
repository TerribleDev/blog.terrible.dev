title: Accessibility Driven Development
date: 2020-08-07 05:27:00
tags:
- a11y
- accessibility
---


I've been working at [CarGurus.com](https://www.cargurus.com) for the last 2 years or so. One of the biggest journeys we've been undertaking is to take accessibility far more seriously. However with an engineering team way into the triple digits it gets harder and harder to scale accessibility knowledge. 
<!-- more -->
Knowledge gaps aside CarGurus has a multitude of technologies UI are build with. The two major are [Freemarker](https://freemarker.apache.org/) and [React](https://reactjs.org/). I manage one of our infrastructure teams, we build the tools and technologies to create the site with. This includes our component library, our build systems, linting tools, authentication systems, and core utilities for product development. When we first started really taking accessibility seriously we went to several teams in the business. Many of them did not have anyone with accessibility expertise. 

> Our first approach was to teach accessibility. At the same time we worked with our brand marketing team to ensure our color pallet would be accessible from the start.


After identifying advocates on every team we set out to streamline identifying accessibility issues. One approach I decided to take was to show borders around failing elements during development. I first heard of this idea years ago when GitHub released something it called [accessibilityjs](https://github.com/github/accessibilityjs). This script Github included in its pages and put a giant ugly red border around failing elements. I thought this was a really slick idea to point out issues during development.

> I was going to use accessibility JS until I found axe-core

So [axe](https://www.deque.com/axe/) is a technology built by deque to identify accessibility issues. This is a highly configurable piece of technology that includes libraries for developers, browser extensions, and bots you can scan sites with. Deque has open sourced the core technology of axe which is a JavaScript called [axe-core](https://github.com/dequelabs/axe-core).     

> I first started out by writing a script to use axe-core and to add a 10px red border around elements, but I quickly ran into trouble

First problem, I need to re-run axe every time the browser changes. If we click to open a nav-bar we'll need to rescan the page. Second problem, every-time we change the DOM the script would crash react apps, and finally axe-core is quite slow on large HTML documents.

## Mutation Observers

So the first problem was easily solvable. The browser has an API called [Mutation Observer](https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver). This is an API that lets you listen to changes to certain elements and fire a function when those elements change. In our case we wanted to listen to any changes to the `<body>` tag and all of its descendants.

```js
function scanForAccesibilityIssues() { /* scan for issues */}
const observer = new MutationObserver(scanForAccesibilityIssues);
observer.observe(document.querySelector('body'), { childList: true, subtree: true });
```

## Shadow DOM

Several UI frameworks such as React keep an in memory representation of the HTML document. The reason for this is when you want to change the UI in React. React will diff its current in-memory DOM with the next DOM and determine the most efficient way to actually apply the changes to the browser. Any application such as a browser extension, or our accessibility detector that edits the DOM outside of React's in-memory DOM will cause React to freak out and either crash of apply a change in an unexpected way. Luckily in recent years browsers have added a [Shadow DOM](https://developer.mozilla.org/en-US/docs/Web/Web_Components/Using_shadow_DOM). This is essentially a DOM that is used to apply visual changes to a user, but sits outside the light DOM (or the regular DOM). However, not all HTML elements support The Shadow DOM. For us to apply the red border we need to use the shadow DOM, and if any elements do not support shadow then we have to apply the border to the parent element. I wrote a [recursive function](https://en.wikipedia.org/wiki/Recursion_(computer_science)#Tail-recursive_functions) called `resolveClosestShadowRoot` which will walk up the DOM document and find the closest parent a target element has that supports shadow. You can tell if a node supports shadow because it will have a `.attachShadow` method. So we can simply access this variable and see if its defined or not.


```js

/**
 *
 * @param {HTMLElement} node
 * @returns
 */
function resolveClosestShadowRoot(node) {
    if (!node) {
        return null;
    }
    if (node.attachShadow) {
        return node;
    }
    return resolveClosestShadowRoot(node.parentElement);
}

```

After we identify which element to style we just have to apply the border. The code below is doing that by calling the attach shadow function and setting its innerHTML. 

```js
const resolvedNode = resolveClosestShadowRoot(node);
const shadowRoot = resolvedNode.attachShadow({ mode: 'open' });
shadowRoot.innerHTML = '<style>:host { outline: red solid 1rem; }</style><slot></slot>';
```

The `<slot></slot>` element is rendering the content of the light DOM. We still have to show the existing content, and the `:host` psudo-class selector is selecting the host of the shadow DOM.

## Debounce 🎉

In web development we often use what's known as a "debounce" to delay doing something. The simple example is sometimes people click on a button multiple times, often on accident, sometimes intentionally. Before taking any action or taking multiple actions you might wait a moment before they stop clicking to do something. You wouldn't want to take the same action multiple times for each click. This is where debounce comes into play. 

```js

function debounce(fn, wait) {
  let timeout = null;
  return function (...args) {
      const next = () => fn.apply(this, args);
      clearTimeout(timeout);
      timeout = setTimeout(next, wait);
  };
}

```

A debounce function accepts a function and a "wait time" or delay before being called to actually executing your function. To debounce a buttons onclick function you would pass its standard onclick function into the debounce function 

```js
const onclick = () => { };
const debouncedClick = debounce(onclick, 500); // 500 milliseconds before the function is actually fired
```

```html
<button onclick="debouncedClick()" ></button>
```
## The result

So the result of all this is a function that listens to changes in the HTML document, waits 1 second for all the changes to finish applying, then scans the page for failing elements and uses The Shadow DOM to apply a red border around those elements. You can see a basic version of the code at [this Github Gist](https://gist.code.cargurus.com/tparnell/4fd6c878dbe64ce2dc2f67d3fc02bd10).

We log the Deque error object to the console which includes links to the failing elements. The result is whenever anyone develops new UI at CarGurus a giant ugly red border surrounds elements they don't write as accessible. This provides **immediate** feedback during the development process and prevents huge categories of accessibility issues from reaching production.

![An example of a failing element](1.jpg)

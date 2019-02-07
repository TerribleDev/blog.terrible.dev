title: Must have vscode plugins for front-end devs
date: 2019-02-06
tags:
- visual studio
- javascript
- css
- front-end
---

I've had a lot of people ask me about my choice of editors, and plugins. A while back I switched to vscode for all my programming work, for both front and back end. In the past I've blogged about [the best plugins for visual studio](/VS-2017-best-extensions-on-launch/) as a backend dev, but I thought I'd give you a more front-end angle

<!-- more -->

## Document this

My first one, and in my opinion the most underrated is [document this](https://marketplace.visualstudio.com/items?itemName=joelday.docthis). So if you have ever had to write [jsdoc](http://usejsdoc.org/) comments you can know how tedious it gets, and if you haven't, trust me you should. VSCode and most other editors can read [jsdoc](http://usejsdoc.org/) comments above functions, and class declarations to improve the intellisense and type completion statements. Simply have your cursor over a function, invoke document this, and quickly you will be given jsdoc comments for your code.

![Animated gif showing off document this](document-this.gif)


## Import Cost

Another extension I find vital to my every day is [import cost](https://marketplace.visualstudio.com/items?itemName=wix.vscode-import-cost). This is a package, that leaves you little notes on the side of any import you have as to how big it will be. This package will even highlight the size text in red for large imports which you can configure. What I love about this package, is it tells me if the package I'm about to use is going to be very expensive size wise. That way I find out long before I commit the code, and my pages get slow.

![a static image showing off import cost](import-cost.png)

## ESlint and Prettier

Hopefully both of these will not be new to you. ESLint is a linting tool that looks for potential errors in your code. Prettier is an opinionated style enforcer for your code. The [eslint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint) and [prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode) extensions for vscode can automatically show you problems in your code as you type, and can even fix your code on save. What I love about both of these tools, is together they make a great force for improving your code base. Prettier eliminates many debates over code style between team members, and eslint prevents you from shipping many bugs to production. These extensions can call out problems as you type, which decreases the feedback loops, and increases your productivity.




## Filesize

As a web developer I spend a lot of my time looking at file size. Right now file sizes are ever inflating, and are causing pain for bandwidth constrained devices. I often download bundles, and inspect their compiled source, or just have to look at how big a file is on the filesystem. A big tool I have in my belt is [filesize](https://marketplace.visualstudio.com/items?itemName=mkxml.vscode-filesize). This is a crazy simple extension, but one that brings me joy everyday. The premise is simple, print the file size of the current file in the status bar at the bottom. Click on it, and you get a nice output of what its like gzipped, and the mime type. Dirt simple, but saved me a ton of time everyday!

![a picture of the filesize plugin in action](filesize2.jpg)


## Runner ups

Here is a list of additional extensions I certainly couldn't live without

* [path intellisense](https://marketplace.visualstudio.com/items?itemName=christian-kohler.path-intellisense) - autocomplete file paths in various files (including html)
* [npm intellisense](https://marketplace.visualstudio.com/items?itemName=christian-kohler.npm-intellisense) - autocomplete npm pages in imports
* [html 5 boilerplate](https://marketplace.visualstudio.com/items?itemName=sidthesloth.html5-boilerplate) - dirt simple html boilerplate snippets
* [icon fonts](https://marketplace.visualstudio.com/items?itemName=idleberg.icon-fonts) - Autocomplete for various icon fonts such as font awesome
* [git lens](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens) - Show git history inline, along with other information from git
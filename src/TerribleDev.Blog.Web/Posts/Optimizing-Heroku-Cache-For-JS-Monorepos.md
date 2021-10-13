title: Optimizing heroku's node_module cache for JS monorepos
tags:
- JS
- javascript
- heroku
- cloud
- devops
date: 2021-10-12 00:00
---

For many of us a JS workspace is the simplest way to structure code for future growth while providing very quick iterations. Incase you are unfamiliar, several technologies exist such as `yarn workspaces`, `lerna`, `npm workspaces`, etc. That can seamlessly stitch npm packages on disk as though they were published to a private NPM registry. This allows for fast iteration inside of a single git repo, while allowing a future where these dependencies could be abstracted.

<!-- more -->

The file system looks something like the following

```
root/
    packages/
        server
        workers
        data
        utils
```

In my quick example we can pretend that an express app in in server, and some background workers are in workers. However both apps need to share code. One strategy would be to version the `data`, and `utils`, packages and ship them to a private NPM registry, or we could use these mono-repo technologies so that `import utils from 'utils'` just works without the need for a remote package store. When installing node modules into a JS workspace the following can occur


```
root/
    node_modules
    packages/
        server/node_modules
        data
        utils
        worker/node_modules
```

In the above scenario node modules are both resolved into the root package but also several layers deep. In heroku you can cache your `node_modules` to improve build speed. However the paths to these directories **must be declared prior to the build**. This becomes an issue when big mono-repos litter `node_modules` everywhere. 

I decided to write the following JS script to walk over the directories where `node_modules` could be placed and rewrite the root `package.json` file so those directories are explicitly declared.


```js
const glob = require('glob');
const fs = require('fs');
const path = require('path');
// do not run this in the heroku build
// we treat this a bit more like a yarn lockfile
if(process.env.NODE_ENV !== 'production') {
    glob("./packages/*/node_modules",  {}, function (er, result) {
        const packageJson = require('./package.json');
        // include the root node_modules
        let cacheDirectories = ['node_modules'];
        cacheDirectories = cacheDirectories.concat(result)
        packageJson.cacheDirectories = cacheDirectories.filter(i => {
            // ensure the directory node_modules are found contain a package.json file
            return fs.existsSync(path.resolve(i, '../package.json'));
        });
        // write out the changes to the root packaage.json
        fs.writeFileSync('./package.json', JSON.stringify(packageJson, null, 2));
    })
}
```

I wired up the script on the post install process of the install lifecycle. Basically adding the following to the root `package.json` file.

```json
{
    "scripts": {
        "postinstall": "node ./computeCacheDirectories.js",
    }
}
```

Now every time a developer runs `yarn install` they will compute the cache directories. When we push changes to prod we get much better cache hits across our yarn workspace
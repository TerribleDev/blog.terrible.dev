title: How to host a javascript monorepo on Heroku
date: 2022-03-01 10:35
tags:
- javascript
- Heroku
---

So I've been using monorepos for some time, and recently I've gotten a lot of questions about how to host them on Heroku. I figured I'd give you the simple guide. There are two basic scenarios. The root of your git repo has your yarn/npm workspace, or you have a folder inside of a gitrepo you wish to use. 

## Scenario 1: yarn/npm workspace

In this case, create a Heroku app with the official nodejs buildpack. Add `heroku-postbuild: "YourBuildCommand"` to your scripts section of the root package.json. This will run after the npm install, and can be used to run any build commands you need (such as compiling typescript). Then use [the multi-procfile buildpack](https://github.com/heroku/heroku-buildpack-multi-procfile) which will grab a procfile from any directory and copy it to the root to boot your app. That way your monorepo can have a `server/package.json` package that contains your web app and in there you can have the procfile `server/Procfile`.

Your buildpacks should have this order:

```
heroku/nodejs
heroku-buildpack-multi-procfile
```

The multi-procfile requires an Environment variable called `PROCFILE` which has the path to the procfile to use. For example it can be `/server/Procfile`. Usually my procfile contains a workspace command to start the server.

```
web: yarn workspace server run start

```

## Scenario 2: Folder inside of Git Repo

So this is a strategy where you make a heroku app in a nested directory. Not using a yarn workspace. In this case you can use the [monorepo buildpack](https://github.com/lstoll/heroku-buildpack-monorepo) to copy a subdirectory to the root directory before the build happens. After that buildpack include the `heroku/nodejs` buildpack which will run the npm/yarn/etc. install commands and then use the `Procfile` in that directory to start your app.



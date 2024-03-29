title: 'Building dotnet core apps, and packages with gulp'
date: 2017-05-04 12:59:43
tags:
- csharp
- dotnet
- dotnet core
- docker
---

Here we are, its 2017 dotnet core is out, and finally dotnet has a proper cli. In a previous post [we explored the new cli](/Exploring-the-dotnet-cli/). In short you can use the dotnet cli to build, test, package, and publish projects. However sometimes just using the cli is not enough. Sometimes, you land in a place where you have many projects to compile, test, and package. 
<!-- more -->

You sometimes need a more complex tool to help you manage your versions, and set the right properties as part of your builds. This is where a tasking system like [gulp](http://gulpjs.com/) can help. Now gulp is not the only task engines. There are Rake, Cake, MSBuild, etc. Plenty to pick from. I personally use gulp a lot, because I'm a web developer. I need a JS based system, to help me run the [babels](https://babeljs.io), and [webpacks](https://webpack.github.io/docs/) of the world. 

## Getting started

So recently I've been working hard to add the dotnet cli's functionality to gulp. Essentially [I authored](https://github.com/Janus-vistaprint/gulp-dotnet-cli) some gulp modules to help you build and package a dotnet application.

So to begin you should install a modern version of node (6+). Run `npm init` which will make you a package.json file. Now run `npm install --save-dev gulp gulp-dotnet-cli`. This will pull down gulp, and my plugin. Now we have to install the gulp cli to our path `npm install -g gulp-cli`

## First gulp task

To begin create a file called `gulpfile.js` in the root of your project. Add the following 2 lines.

```js
let gulp = require('gulp');
let {restore, build, test, pack, publish} = require('gulp-dotnet-cli');


```

The first line just requires in gulp, the second line pulls down my gulp module. My module has 5 actions. Restore which will run `dotnet restore` or essentially download nuget packages. Build which runs `dotnet build`, this compiles out the project. `Test` runs the unit tests. `pack` will convert a csproj to a nuget package, and `publish` will publish the project to the local file system. Each action has optional parameters [documented here](https://github.com/Janus-vistaprint/gulp-dotnet-cli/blob/master/docs/index.md).


Now lets make a restore task

```js
let gulp = require('gulp');
let {restore, build, test, pack, publish} = require('gulp-dotnet-cli');

gulp.task('restore', ()=>{
    return gulp.src('**/*.csproj', {read: false})
            .pipe(restore());
});

```

This is simple, find all csproj files and run restore. You could also use sln files instead of csproj's.

```js
let gulp = require('gulp');
let {restore, build, test, pack, publish} = require('gulp-dotnet-cli');

gulp.task('restore', ()=>{
    return gulp.src('**/*.sln', {read: false})
            .pipe(restore());
});

```

Now lets add a build.

```js
let gulp = require('gulp');
let {restore, build, test, pack, publish} = require('gulp-dotnet-cli');

gulp.task('restore', ()=>{
    return gulp.src('**/*.sln', {read: false})
            .pipe(restore());
});

gulp.task('build', ['restore'], ()=>{
                    //this could be **/*.sln if you wanted to build solutions
    return gulp.src('**/*.csproj', {read: false})
        .pipe(build());
});

```

Very easy, and simple. Glob up the csproj files and compile them out. In the build task, the array containing restore, is stating that build depends on restore.


## Nuget packages

Ok so here we go, this is when we start doing cool things. We want to build all of our projects with a version number, and then make a nuget package with the same version. We want to build in `Release` mode, and we want all packages in an nupkgs directory. At the end we should push our nuget packages to a custom myget feed.

```js
let gulp = require('gulp');
let {restore, build, test, pack, publish} = require('gulp-dotnet-cli');
let version = '1.0.0'; //you could read a git tag here
let configuration = 'Release';
gulp.task('restore', ()=>{
    return gulp.src('**/*.sln', {read: false})
            .pipe(restore());
});

gulp.task('build', ['restore'], ()=>{
                    //this could be **/*.sln if you wanted to build solutions
    return gulp.src('**/*.csproj', {read: false})
        .pipe(build({version: version, configuration: configuration}));
});

gulp.task('pack', ['build'], ()=>{
    return gulp.src('**/*.csproj', {read:false})
            .pipe(pack({ 
                version: version, 
                configuration: configuration, 
                output: path.join(process.cwd(), 'nupkgs') 
                }));
});

//push nuget packages to a server
gulp.task('push', ['pack'], ()=>{
    return gulp.src('nupkgs/**.nupkg', {read: false})
                .pipe(push({
                    apiKey: process.env.NUGET_API_KEY, //my nuget api key from an environment variable
                    source: 'https://myget.org/f/myfeedurl' //a custom nuget feed
                    }));
});

```

## Docker + Dotnet core + gulp


Ok so lets say you have added a docker file to your projects directory, and you have added `<None include="Dockerfile">` to your csproj's item group.

```dockerfile
FROM microsoft/aspnetcore:1.1.1
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "DotnetTerra.dll"]

```


First we need a task to publish our website to a folder, compiled. 

```js
gulp.task('publish', ['restore'], ()=>
gulp.src('**/*.csproj', {read:false})
    .pipe(publish({ configuration: configuration, 
                    version: version, 
                    output: path.join(process.cwd(), 'output') 
                    })))

```

next we need to compile our docker container. run `npm install --save-dev child-process-promise` this is a simple package that converts all child process node calls to promises. Promises are like a callback, but with a nicer syntax. Note that here we are tagging the container with the same version as our package 

```js

const spawn = require('child-process-promise').spawn;

gulp.task('docker:compile', ['publish'], ()=> 
    spawn('docker', ['build', '-t', `myapp-${version}`, 'output'], {stdio:'inherit'})
);


```

In total your file will probably look like this:


```js
const spawn = require('child-process-promise').spawn;
const {restore, test, build, publish} = require('gulp-dotnet-cli');

gulp.task('restore', ['clean'], ()=>
    gulp.src('**/*.csproj')
        .pipe(restore())
          
);

gulp.task('build', ['restore'], ()=>
    gulp.src('**/*.csproj', {read:false})
        .pipe(build(
            {
                configuration: configuration,
                version: version 
            }))
);

gulp.task('publish', ['build'], ()=>
    gulp.src('src/DotnetTerra/DotnetTerra.csproj')
        .pipe(publish( 
            {
                configuration: configuration,
                version: version,
                output:  path.join(process.cwd(), 'output')
            }))
);

gulp.task('docker:compile', ['publish'], ()=> 
    spawn('docker', ['build', '-t', `myapp-${version}`, 'output'], {stdio:'inherit'})
);


```

Gulp can easily lets you add more tasks, and you could easily create a task to tag and push the container.

## Conclusion

In Conclusion, with this module you can easily package any dotnet core based project with gulp. This will work on any project that is compatible with the dotnet cli. By using a task framework like gulp you can ensure your artifacts are all tagged with the same versions, and have the same set of configurations. You can find documentation, and samples [in github](https://github.com/Janus-vistaprint/gulp-dotnet-cli).
title: Building attractive CLIs in TypeScript
date: 2022-07-08 05:18
tags:
- javascript
- typescript
- node
- cli
- tutorials
---

So you've come to a point where you want to build nice CLIs. There's a few different options for building CLI's. My two favorites are [oclif](https://oclif.io/) and [commander.js](https://github.com/tj/commander.js/). I tend toward leaning to commander, unless I know I'm building a super big app. However, I've really enjoyed building smaller CLIs with commander recently.

<!-- more -->

> tl;dr? You can [view this repo](https://github.com/TerribleDev/example-ts-cli)

![a video of the CLI](cli.gif)

## Commander.js Lingo

So commander has a few different nouns.

* `Program` - The root of the CLI. Handles running the core app.
* `Command` - A command that can be run. These must be registered into `Program`
* `Option` - I would also call these `flags` they're the `--something` part of the CLI.
* `Arguments` - These are named positioned arguments. For example `npm install commander` the `commander` string in this case is an argument. `--save` would be an option.



## Initial Setup

First, do an npm init, and install commander, types for node, typescript, esbuild, and optionally ora.

```bash
npm init -y
npm install --save commander typescript @types/node ora
```

Next we have to configure a build command in the package.json. This one runs typescript to check for types and then esbuild to compile the app for node.

```json
  "scripts": {
    "build": "tsc --noEmit ./index.ts && esbuild index.ts --bundle --platform=node --format=cjs --outfile=dist/index.js",
  }
```

We now need to add a bin property in the package.json. This tells the package manager that we have an executable. The key should be the name of your CLI

```json
"bin": {
    "<yourclinamehere>": "./dist/index.js"
  }
```


Make a file called index.ts, and place this string on the first line. This is called a shebang and it tells your shell to use node when the file is ran.

`#!/usr/bin/env node`

## Getting started

Hopefully you have done the above. Now in index.ts you can make a very basic program. Try npm build and then run the CLI with --help. Hopefully you'll get some output.

```ts
#!/usr/bin/env node

import { Command } from 'commander'
import { spinnerError, stopSpinner } from './spinner';
const program = new Command('Our New CLI');
program.version('0.0.1');
program.addHelpCommand()

async function main() {
    await program.parseAsync();

}
console.log() // log a new line so there is a nice space
main();
```

### Setting up the spinner

So, I really like loading spinners. I think it gives the CLI a more polished feel. So I added a spinner using ora. I made a file called `spinner.ts` which is a wrapper to handle states of spinning or stopped.

```ts
import ora from 'ora';

const spinner = ora({ // make a singleton so we don't ever have 2 spinners
    spinner: 'dots',
})

export const updateSpinnerText = (message: string) => {
    if(spinner.isSpinning) {
        spinner.text = message
        return;
    }
        spinner.start(message)
}

export const stopSpinner = () => {
    if(spinner.isSpinning) {
        spinner.stop()
    }
}
export const spinnerError = (message?: string) => {
    if(spinner.isSpinning) {
        spinner.fail(message)
    }
}
export const spinnerSuccess = (message?: string) => {
    if(spinner.isSpinning) {
        spinner.succeed(message)
    }
}
export const spinnerInfo = (message: string) => {
    spinner.info(message)
}
```

### Writing a command

So I like to seperate my commands out into sub-commands. In this case we're making `widgets` a sub-command. Make a new file, I call it widgets.ts. I create a new `Command` called `widgets`. Commands can have commands making them sub-commands. So we can make a sub-command called `list` and `get`. **List** will list all the widgets we have, and **get** will retrive a widget by id. I added some promise to emulate some delay so we can see the spinner in action.

    
```ts
import { Command } from "commander";
import { spinnerError, spinnerInfo, spinnerSuccess, updateSpinnerText } from "./spinner";

export const widgets = new Command("widgets");

widgets.command("list").action(async () => {
    updateSpinnerText("Processing ");
    // do work
    await new Promise(resolve => setTimeout(resolve, 1000)); // emulate work
    spinnerSuccess()
    console.table([{ id: 1, name: "Tommy" }, { id: 2, name: "Bob" }]);
})

widgets.command("get")
.argument("widget id <id>", "the id of the widget")
.option("-f, --format <format>", "the format of the widget") // an optional flag, this will be in options.f
.action(async (id, options) => {
    updateSpinnerText("Getting widget " + id);
    await new Promise(resolve => setTimeout(resolve, 3000));
    spinnerSuccess()
    console.table({ id: 1, name: "Tommy" })
})

```

Now lets register this command into our program. (see the last line)

```ts
#!/usr/bin/env node
import { Command } from 'commander'
import { spinnerError, stopSpinner } from './spinner';
import { widgets } from './widgets';
const program = new Command('Our New CLI');
program.version('0.0.1');
program.addHelpCommand()
program.addCommand(widgets);
```


Do a build! Hopefully you can type `<yourcli> widgets list` and you'll see the spinner. When you call `spinnerSuccess` without any parameters the previous spinner text will stop and become a green check. You can pass a message instead to print that to the console. You can also call `spinnerError` to make the spinner a red `x` and print the message.


### Handle unhandled errors

Back in index.ts we need to add a hook to capture unhandled errors. Add a verbose flag to the program so we can see more details about the error, but by default lets hide the errors.

```ts
const program = new Command('Our New CLI');
program.option('-v, --verbose', 'verbose logging');
```

Now we need to listen for the node unhandled promise rejection event and process it.


```ts
process.on('unhandledRejection', function (err: Error) { // listen for unhandled promise rejections
    const debug = program.opts().verbose; // is the --verbose flag set?
    if(debug) {
        console.error(err.stack); // print the stack trace if we're in verbose mode
    }
    spinnerError() // show an error spinner
    stopSpinner() // stop the spinner
    program.error('', { exitCode: 1 }); // exit with error code 1
})
```


#### Testing our error handling

Lets make a widget action called `unhandled-error`. Do a build, and then run this action. You should see the error is swallowed. Now try again but use `<yourcli> --verbose widgets unhandled-error` and you should see the error stack trace.

```ts
widgets.command("unhandled-error").action(async () => {
    updateSpinnerText("Processing an unhandled failure ");
    await new Promise(resolve => setTimeout(resolve, 3000));
    throw new Error("Unhandled error");
})
```

## Organizing the folders

Ok, so you have the basics all setup. Now, how do you organize the folders. I like to have the top level commands in their own directories. That way the folder structure emulates the CLI. This is an idea I saw in oclif. 

```
- index.ts
- /commands/widgets/index.ts
- /commands/widgets/list.ts
- /commands/widgets/get.ts

```

## So why not OCLIF?

A few simple reasons. OCLIF's getting started template comes with an extremely opinionated typescript configuration. For large projects, I've found it to be incredible. However, for smaller-ish things, I've found conforming to it, a trial of turning down the linter a lot. Overall, they're both great tools. Why not both?
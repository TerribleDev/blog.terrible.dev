title: "Building a remote cache server for Turborepo"
date: 2022-02-12 09:52
tags: 
    - Go
    - Javascript
    - Turborepo
    - Devops
    - Build
    - node.js
---


[Turborepo](https://turborepo.org/) is a tool that came across my virtual desk recently. Monorepo develoment has been around for a long time. This is a strategy where all of your code remains in one repository regardless of services. A lot of people use monorepo's even for microservices. The huge upside is to keep everything in one place, which allows for development efficiency, such as grepping an entire codebase for specific keywords. A quick example would be a top level directory which has child directories that each contain an npm package, unlike publishing these packages, you access them locally as though they were published.
<!-- more -->

There are many tools in the Javascript ecosystem to manage monorepos. [Yarn](https://classic.yarnpkg.com/lang/en/docs/cli/workspaces/), and [npm](https://docs.npmjs.com/cli/v7/using-npm/workspaces) both have their own workspaces. [Lerna](https://lerna.js.org/) is a tool that people use to run commands a cross these packages. I've been a huge fan of monorepos for years. One of the big problems with this setup is build times. At [Quala](https://www.quala.io) we have around 38 packages, and some of my previous employers have had over 100. When you have these large repos sometimes you can make a change in a single package, but when you run `build` you have to wait to build the entire repository which can take a long time.

[Turborepo](https://turborepo.org/), however caches the build output of packages, so when you change a package it will get cache hits on particular packages, and thus you only build the changes you make. This is not a new idea. Years ago, google built [bazel](https://bazel.build/), A lot of people in C++ land have had remote builds. With Turborepo it seems the only official way to have remote caches is to use Vercel, or host your own server. For many reasons at [Quala](https://www.quala.io) I decided to opt for hosting our own server.

So to add turborepo to your monorepo, you need to add some [simple config](https://turborepo.org/docs/features/caching) to the root of your workspace, and your root `package.json` needs to replace its build command with `turborepo build`. In the case of remote caches you need to add

`--api="https://yourCacheServer.dev" --token="token" --team="team"`

Notice, the api flag does not contain a `/` at the end. Now according to the docs you don't need to pass a team, but I was unable to get the caches to register without it 🤷‍♀️

## The API

 According to the [docs](https://turborepo.org/docs/features/remote-caching)

> You can self-host your own Remote Cache or use other remote caching service providers as long as they comply with Turborepo's Remote Caching Server API. I opted to write the server in go, and [I used Go Fiber](https://github.com/gofiber/fiber). At first I figured I could copy their structs to my project but honestly the API is so simple, there is no advantage to this.

To get a list of the API's you need, you are linked to some [code written in Go](https://github.com/vercel/turborepo/blob/main/cli/internal/client/client.go). I reverse engineered this code a bit, and came up with 4 APIs, and an AUTH token

```
Authorization: Bearer ${token}
PUT: /v8/artifacts/:hash
GET: /v8/artifacts/:hash
GET: /v2/user
GET: /v2/teams
```

### Authorization

When turborepo sends requests it appends the `Authorization` header which will contain our token. Ideally you would add to your server a way to auth a user and give them this token. In the below example we have a single token that comes from an environment variable. You really should have per user auth.


```go
	app.Use(func(c *fiber.Ctx) error {
		authHeader := c.Get("Authorization")
		if authHeader != "Bearer "+token {
			c.Status(401).SendString("Unauthorized")
			return nil
		}
		return c.Next()
	})
```

### Handling Requests

The API pretty much breaks down like this.

`PUT: /v8/artifacts/:hash` will send a file that you must write somewhere. Some people opt for sending it to S3, I decided to use a persistent disk, and save on the disk. I wanted the fastest responses for the caches. Heck if I'm going to remote cache something that would still be kinda quick on an M1, it better perform. 

```go
	app.Put("/v8/artifacts/:hash", func(c *fiber.Ctx) error {
		fmt.Println(string(c.Request().URI().QueryString()))
		return os.WriteFile("./cache/"+c.Params("hash"), c.Request().Body(), 0644)
	})
```

The same URL but on a get is simple. Retrieve a file and serve it up, or return a 404

```go
	app.Get("/v8/artifacts/:hash", func(c *fiber.Ctx) error {
		fmt.Println(string(c.Request().URI().QueryString()))
		return c.SendFile("./cache/" + c.Params("hash"))
	})
```

The last two honesty you don't need to make things work. You can just return a 200

```go
	app.Get("/v2/teams", func(c *fiber.Ctx) error {
		return c.SendStatus(fiber.StatusOK)
	})

	app.Get("/v2/user", func(c *fiber.Ctx) error {
		return c.SendStatus(fiber.StatusOK)
	})
```

The `/v2/user` API is supposed to return information about the current user in the following shape. I'm pretty sure (not positive) created at is an [epoch](https://en.wikipedia.org/wiki/Unix_time) of the time the user was created. I'm guessing its largely used for Vercel.

```
{
	ID        string
	Username  string
	Email     string
	Name      string
	CreatedAt int   
}
```

The team api is supposed to look something like the following.

```
{
    Pagination {
        Count: int,
        Next: int,
        Prev: int
    }
    Teams [
        Team {
            ID: string,
            Slug: string,
            Name: string,
            CreatedAt: int,
            Created: string
        }
    ]
}
```

> What about the --team flag?

So when requests are made with `--team` a query string `?slug=team` is added to the request. You can use this to ensure a particular user is in the given team, and you can fragment your caches by team. I ommitted that code from the above example, but the easiest way would be to have `./cache/${team}/${hash}` directory structure for the caches on disk. Note, on the GET requests you should auth the token against the team ID, and return a 404 if the user is not in the team. **I would not opt to return a Unauthorized header**, as that can be used by bad actors to cycle through tokens to know which one will work to cause harm.

## The Result

An extremely minimal server [is in this github repo](https://github.com/TerribleDev/turbogo) (although you shouldn't probably use it without building it out more). 
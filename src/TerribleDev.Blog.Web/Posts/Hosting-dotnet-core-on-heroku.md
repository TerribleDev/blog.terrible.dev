title: Hosting dotnet core on Heroku
date: 2021-07-19 00:01
tags:
- dotnet core
- cloud
- Heroku
- Postgres
---


I've been getting back into building scrappy little web apps for my friends. On top of this, I recently joined [a startup](https://quala.io) and getting away from Enterprise class software has made me make a huge mind-shift. In the recent past when I wanted to build apps I was thinking Kubernetes, Helm Charts, etc. However, in small app, and startup land reducing the barriers to ship is very important.

<!-- more -->

Incase you are not familiar [Heroku](https://www.heroku.com) is a platform to host webapps. They host a free version of Postgres DB, and Redis that is directly connected to your app with environment variables. Heroku has support for many languages, but one I saw missing from the list was dotnet.

To host apps on Heroku, you must know the basic *rules of Heroku*

1. Your app must listen on `$PORT` or `%PORT%` if you come from windows. Basically, any http listeners must listen to the port defined as an environment variable.
2. Postgres is free (to a point), redis is free, most other things cost money.
3. Logs must go to `stdout` which works well for us since that's the default behavior of asp.net core!
4. In dotnet core authentication cookies are encrypted and the key is usually placed in your profile, but in Heroku your app could be moved at any moment
5. Heroku gives you your Postgres connection string as `postgres://<username>:<password>@<host>:<port>/<database>`



## Listening on $PORT

Traditionally dotnet core apps listen for an environment variable called `ASPNETCORE_URLS` but in this case we need to override this behavior. In your `Program.cs` file you can make the following modification, which detects if `$PORT` is defined, and if it is to listen to all requests on that port.

```csharp
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var port = Environment.GetEnvironmentVariable("PORT");
                    if(!string.IsNullOrEmpty(port)) 
                    {
                        webBuilder.UseUrls($"http://*:{port}");
                    }
                    webBuilder.UseStartup<Startup>();
                });
```

## Using Postgres with Entity Framework

On a `dotnet new mvc --auth individual` you are presented with the following block of code in `Startup.cs`

```csharp
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

```

This configures your app to use SqlLite as a DB, we need to switch this. Luckily the Postgres team has an awesome integration with entity framework. Run the following command to add their package to your project

`dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL`

Then simply swap the previous code block for the following, which will parse the database url from Heroku and setup a Postgres connection. You can use the following docker-compose file and `appsettings.Development.json` for local development.

```csharp
            var databaseUrl = Configuration.GetValue<string>("DATABASE_URL");
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.ToString()));
```

*docker-compose.yml*

```yml
version: '3'
services:
  postgres:
    image: 'postgres:13'
    ports:
      - '6666:5432'
    environment:
      POSTGRES_PASSWORD: 'password'
      POSTGRES_USER: 'admin'
```

*appsettings.Development.json*

```json
{
  "DATABASE_URL": "postgres://admin:password@localhost:6666/main"
}
```

## Encryption keys

Ok so you've got the basics running, but you need to store your encryption keys. We can store them in the database using entity framework! Add this to your `startup.cs` `ConfigureServices` Method

```cs

services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

```

## Database Migrations

There are several ways to handle database migrations. For simple webapps you can configure your app to do a migration on startup. More complex apps should shell into the `ef` command line.

```csharp
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using(var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using(var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                ctx.Database.EnsureCreated();
                ctx.Database.Migrate();
            }
        }

```

## Getting your app in Heroku with containers

There are 2 basic methods to getting your app live in Heroku. One is to push a docker container to Heroku, or use a Heroku buildpack to have Heroku build your app for you. I opted for the docker container.

I stole this sample dockerfile from the aspnet core docker docs.

```dockerfile
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["./MyApp.csproj", "."]
RUN dotnet restore "MyApp.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "MyApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MyApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MyApp.dll"]


```

I then found someone had made a *build a docker image and push to Heroku* GitHub action. All I had to do is make this a file in `.github/deployContainerToHeroku.yml`, turn on Github actions, and register my Heroku API key as a secret in GitHub


```yml
name: Deploy to Heroku.

# Run workflow on every push to master branch.
on:
  push:
    branches: [master]

# Your workflows jobs.
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      # Check-out your repository.
      - name: Checkout
        uses: actions/checkout@v2


### ⬇ IMPORTANT PART ⬇ ###

      - name: Build, Push and Release a Docker container to Heroku. # Your custom step name
        uses: gonuit/Heroku-docker-deploy@v1.3.3 # GitHub action name (leave it as it is).
        with:
          # Below you must provide variables for your Heroku app.

          # The email address associated with your Heroku account.
          # If you don't want to use repository secrets (which is recommended) you can do:
          # email: my.email@example.com
          email: ${{ secrets.HEROKU_EMAIL }}
          
          # Heroku API key associated with provided user's email.
          # Api Key is available under your Heroku account settings.
          Heroku_api_key: ${{ secrets.HEROKU_API_KEY }}
          
          # Name of the Heroku application to which the build is to be sent.
          Heroku_app_name: ${{ secrets.HEROKU_APP_NAME }}

          # (Optional, default: "./")
          # Dockerfile directory.
          # For example, if you have a Dockerfile in the root of your project, leave it as follows:
          dockerfile_directory: ./src/MyApp

          # (Optional, default: "Dockerfile")
          # Dockerfile name.
          dockerfile_name: Dockerfile

          # (Optional, default: "")
          # Additional options of docker build command.
          docker_options: "--no-cache"

          # (Optional, default: "web")
          # Select the process type for which you want the docker container to be uploaded.
          # By default, this argument is set to "web".
          # For more information look at https://devcenter.Heroku.com/articles/process-model
          process_type: web
          

```

## Getting your app in Heroku with buildpacks

Heroku has had this system called *buildpacks* which allow you to script the creation of the hosting environment of your app. Someone has done the dirty work and [built a dotnet core buildpack](https://elements.Heroku.com/buildpacks/jincod/dotnetcore-buildpack) which can be used to deploy dotnet core apps to Heroku. To use this, create an app in Heroku, set your [buildpack to the dotnet core buildpack](https://elements.Heroku.com/buildpacks/jincod/dotnetcore-buildpack) in settings. Connect your GitHub repo and Heroku will do the hard work for you!


## Finish

I hope you liked this. Keep on hacking away!